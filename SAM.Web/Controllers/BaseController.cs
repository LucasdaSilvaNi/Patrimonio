using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using Sam.Common.Util;
using SAM.Web.Controllers.IntegracaoContabilizaSP;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using System.Diagnostics;
using System.Security.Claims;

namespace SAM.Web.Controllers
{
    public class BaseController : Controller
    {
        BaseContext db = new BaseContext();
        internal bool ugeIntegradaSiafem = false;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller;
            var actionDescriptor = filterContext.ActionDescriptor;
            var cp = (ClaimsPrincipal)User;
            var identity = cp.Claims.FirstOrDefault();
            string cpf = string.Empty;

            if (identity.Type == "name")
                cpf = cp.Claims.Where(x => x.Type == "cpf").FirstOrDefault().Value;
            else
                cpf = identity.Value;


            //Verifica se esta autenticado
            if (!HttpContext.User.Identity.IsAuthenticated || Session["UsuarioAutenticado"] == null)
            {
                FormsAuthentication.SignOut();
                //redireciona para pagina de autenticação
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Login",
                    action = "Index"
                }));

                //base.OnActionExecuting(filterContext);
            }
            //Verifica se tem perfil relacionado ao usuario
            else if (!TemPerfil(cpf))
                MensagemErro(filterContext, "Não existe perfil relacionado com o seu usuario.");

            //Verifica se tem a necessidade de troca de senha
            else if (db.Users.FirstOrDefault(m => m.CPF == cpf).ChangePassword)
            {
                //Verifica se já não esta na tela de troca de senha
                if (RemoveNumerosUrl(HttpContext.Request.CurrentExecutionFilePath.ToUpper()) != @"/Users/ChangePassword/".ToUpper())
                {
                    //Redireciona para pagina de autenticação
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Users",
                        action = "ChangePassword/" + db.Users.FirstOrDefault(m => m.CPF == cpf).Id
                    }));
                }
            }

            //Caso já esteja preenchido, quer dizer que entrou em umas das condições ifs anteriores
            //Que são: usuário não logado, usuário sem perfil e troca de senha.
            //Nesses casos, não há necessidade de verificar se tem acesso à URL
            if (filterContext.Result != null)
                base.OnActionExecuting(filterContext);
            else
            {
                //Verifica se tem acesso a essa URL
                if (AcessoURL(cpf, HttpContext.Request.CurrentExecutionFilePath))
                {
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    // Utilizado para validar as transações das Views e Acesso as Urls
                    if (WebConfigurationManager.AppSettings["Densenvolvimento"] == "Ativo" || PerfilAdmGeral())
                        base.OnActionExecuting(filterContext);
                    else
                        MensagemErro(filterContext, "Você não tem acesso a esta ação, favor contatar o Administrador responsável.");
                }
            }
        }

        #region Formatos de Data
        //public static string fmtDataFormatoBrasileiro
        //{ get { return "dd/MM/yyyy"; } }
        //public static string fmtDataFormatoBrasileiroNumeral
        //{ get { return "ddMMyyyy"; } }
        public static string fmtDataHoraFormatoBrasileiro
        { get { return "dd/MM/yyyy HH:mm:ss"; } }
        public static string fmtDataFormatoBrasileiro
        { get { return "dd/MM/yyyy"; } }
        public static string fmtDataFormatoBrasileiroNumeral
        { get { return "ddMMyyyy"; } }
        #endregion Formatos de Data

        public static void InitDisconnectContext(ref SAMContext contextoCamadaDados)
        {
            if (contextoCamadaDados.IsNull())
                contextoCamadaDados = new SAMContext();

            if (contextoCamadaDados.IsNotNull())
            {
                contextoCamadaDados = new SAMContext();
                contextoCamadaDados.Configuration.AutoDetectChangesEnabled = false;
                contextoCamadaDados.Configuration.ProxyCreationEnabled = false;
                contextoCamadaDados.Configuration.LazyLoadingEnabled = false;
            }
        }

        /// <summary>
        /// Verifica se UGE possui integração com ContabilizaSP
        /// </summary>
        protected void initDadosSIAFEM()
        {
            int _institutionId;
            int? _managerUnitId;


            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _institutionId = perflLogado.InstitutionId;
            _managerUnitId = perflLogado.ManagerUnitId;


            var contextoBancoDeDados = new HierarquiaContext();
            {
                contextoBancoDeDados.Configuration.AutoDetectChangesEnabled = false;
                contextoBancoDeDados.Configuration.ProxyCreationEnabled = false;
                contextoBancoDeDados.Configuration.LazyLoadingEnabled = false;
            }

            ugeIntegradaSiafem = ((contextoBancoDeDados.Institutions.Count(orgaoSIAFEM => orgaoSIAFEM.Id == _institutionId && orgaoSIAFEM.flagIntegracaoSiafem && orgaoSIAFEM.Status)) +
                                  (contextoBancoDeDados.ManagerUnits.Count(ugeSIAFEM => ugeSIAFEM.Id == _managerUnitId && ugeSIAFEM.FlagIntegracaoSiafem && ugeSIAFEM.Status))) > 0;
        }

        /// <summary>
        /// Cópia do initDadosSIAFEM, só que sem a validação de autenticação do método BuscaHierarquiaPerfilLogadoPorUsuario
        /// </summary>
        protected void initDadosSiafemIntegracao()
        {
            int _institutionId;
            int? _managerUnitId;


            User u = UserCommon.CurrentUser();
            var perflLogado = (from r in db.RelationshipUserProfiles
                               join ri in db.RelationshipUserProfileInstitutions on r.Id equals ri.RelationshipUserProfileId
                               where r.UserId == u.Id && r.DefaultProfile == true
                               select ri).FirstOrDefault();
            _institutionId = perflLogado.InstitutionId;
            _managerUnitId = perflLogado.ManagerUnitId;


            var contextoBancoDeDados = new HierarquiaContext();
            {
                contextoBancoDeDados.Configuration.AutoDetectChangesEnabled = false;
                contextoBancoDeDados.Configuration.ProxyCreationEnabled = false;
                contextoBancoDeDados.Configuration.LazyLoadingEnabled = false;
            }

            ugeIntegradaSiafem = ((contextoBancoDeDados.Institutions.Count(orgaoSIAFEM => orgaoSIAFEM.Id == _institutionId && orgaoSIAFEM.flagIntegracaoSiafem && orgaoSIAFEM.Status)) +
                                  (contextoBancoDeDados.ManagerUnits.Count(ugeSIAFEM => ugeSIAFEM.Id == _managerUnitId && ugeSIAFEM.FlagIntegracaoSiafem && ugeSIAFEM.Status))) > 0;
        }


        /// <summary>
        /// Executa processamento sobre a movimentação para exibir a mensagem pop-up após a persistência da mesma no banco, 
        /// independentemente de chamada ou nao ao webservice do Contabiliza-SP
        /// </summary>
        /// <param name="movementTypeId">Id do Tipo de Movimentação Patrimonial sendo efetuada</param>
        /// <param name="numeroDocumentoSAM">Numero de documento SAM ou processo administrativo a ser enviado ao sistema Contabiliza-SP</param>
        /// <param name="listaMovimentacoesPatrimoniaisEmLote">Relacao de todos os BPs que estao sendo incorporados/movimentados para processamento</param>
        /// <returns></returns>
        protected string processamentoMovimentacaoPatrimonialNoContabilizaSP(int movementTypeId, string loginUsuarioSIAFEM, string senhaUsuarioSIAFEM, string numeroDocumentoSAM, IList<AssetMovements> listaMovimentacoesPatrimoniaisEmLote, bool forcaNaoProcessamentoNoContabilizaSP = false)
        {
            SAMContext contextoBancoDeDados = null;
            string msgRetornoDeInformeAoUsuario = null;
            string fmtMsgInformeAoUsuario = null;
            bool integracaoSIAFEMAtivada = false;
            string cpfUsuarioSessaoLogada = null;
            bool ehEstorno = false;
            User usuarioLogado = null;


            if (movementTypeId > 0)
            {
                this.initDadosSIAFEM();
                integracaoSIAFEMAtivada = this.ugeIntegradaSiafem;
                InitDisconnectContext(ref contextoBancoDeDados);
                fmtMsgInformeAoUsuario = null;


                if (listaMovimentacoesPatrimoniaisEmLote.HasElements())
                {
                    var tipoMovPatrimonial = contextoBancoDeDados.MovementTypes.Where(movPatrimonial => movPatrimonial.Id == movementTypeId).FirstOrDefault();
                    var ugeMovimentacaoId = (listaMovimentacoesPatrimoniaisEmLote.HasElements() ? listaMovimentacoesPatrimoniaisEmLote[0].ManagerUnitId : -1);
                    var ugeMovimentacaoPatrimonial = ((ugeMovimentacaoId > 0) ? contextoBancoDeDados.ManagerUnits.Where(ugeMovPatrimonial => ugeMovPatrimonial.Id == ugeMovimentacaoId).FirstOrDefault() : new ManagerUnit());
                    var codigoUGEMovimentacao = (ugeMovimentacaoPatrimonial.IsNotNull() ? ugeMovimentacaoPatrimonial.Code : "0");
                    var movimentacaoPatrimonial = (listaMovimentacoesPatrimoniaisEmLote.HasElements() ? listaMovimentacoesPatrimoniaisEmLote.First() : null);
                    string nomeTipoMovimentacaoPatrimonial = ((tipoMovPatrimonial.IsNotNull()) ? tipoMovPatrimonial.Description : "DESCONHECIDO");




                    var primeiraMovimentacaoPatrimonial = listaMovimentacoesPatrimoniaisEmLote.First();
                    int mesReferenciaMovimentacao = -1;
                    int mesReferenciaUGE = -1;
                    string strMesReferenciaUGE = ugeMovimentacaoPatrimonial.ManagmentUnit_YearMonthReference;
                    string strMesReferenciaMovimentacao = movimentacaoPatrimonial.MovimentDate.ToString("yyyyMM");
                    Int32.TryParse(strMesReferenciaMovimentacao, out mesReferenciaMovimentacao);
                    Int32.TryParse(strMesReferenciaUGE, out mesReferenciaUGE);
                    ehEstorno = (movimentacaoPatrimonial.Status == false &&
                                 movimentacaoPatrimonial.FlagEstorno == true &&
                                 movimentacaoPatrimonial.DataEstorno.HasValue &&
                                 movimentacaoPatrimonial.LoginEstorno.HasValue);
                    forcaNaoProcessamentoNoContabilizaSP = (((mesReferenciaUGE > 0) || (mesReferenciaMovimentacao > 0))
                                                            && ((mesReferenciaUGE <= 201911) || (mesReferenciaMovimentacao <= 201911)));

                    fmtMsgInformeAoUsuario = (!ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoExecutadaSucesso : IntegracaoContabilizaSPController.fmtMsgEstornoExecutadoSucesso);
                    if (tipoMovPatrimonial.IsNotNull() && tipoMovPatrimonial.PossuiContraPartidaContabil() && ugeIntegradaSiafem && !forcaNaoProcessamentoNoContabilizaSP)
                    {
                        var svcIntegracaoSIAFEM = new IntegracaoContabilizaSPController();
                        usuarioLogado = UserCommon.CurrentUser();
                        cpfUsuarioSessaoLogada = (usuarioLogado.IsNotNull() ? usuarioLogado.CPF : null);
                        msgRetornoDeInformeAoUsuario = svcIntegracaoSIAFEM.ExecutaProcessamentoMovimentacaoNoSIAF(listaMovimentacoesPatrimoniaisEmLote, cpfUsuarioSessaoLogada, loginUsuarioSIAFEM, senhaUsuarioSIAFEM);
                    }
                    else
                    {
                        msgRetornoDeInformeAoUsuario = String.Format(fmtMsgInformeAoUsuario,
                                                                     nomeTipoMovimentacaoPatrimonial,
                                                                     numeroDocumentoSAM,
                                                                     null);
                    }
                }
            }

            return msgRetornoDeInformeAoUsuario;
        }

        /// <summary>
        /// Executa processamento sobre a movimentação para exibir a mensagem pop-up após a persistência da mesma no banco, 
        /// independentemente de chamada ou nao ao webservice do Contabiliza-SP
        /// </summary>
        /// <param name="movementTypeId">Id do Tipo de Movimentação Patrimonial sendo efetuada</param>
        /// <param name="loginUsuarioSIAFEM">CPF/Chave de acesso ao SIAFEM</param>
        /// <param name="senhaUsuarioSIAFEM">Senha de acesso ao SIAFEM</param>
        /// <param name="numeroDocumentoSAM">Numero de documento SAM ou processo administrativo a ser enviado ao sistema Contabiliza-SP</param>
        /// <param name="listaMovimentacoesPatrimoniaisEmLote">Relacao de todos os BPs que estao sendo incorporados/movimentados para processamento</param>
        /// <param name="forcaNaoProcessamentoNoContabilizaSP">Caso alguma situação demande que uma UGE/movimentação integrada não envie o XML ao SIAFEM, setar esse parametro</param>
        /// <returns></returns>
        protected string novoMetodoProcessamentoMovimentacaoPatrimonialNoContabilizaSP(int movementTypeId, string loginUsuarioSIAFEM, string senhaUsuarioSIAFEM, string numeroDocumentoSAM, IList<AssetMovements> listaMovimentacoesPatrimoniaisEmLote, bool forcaNaoProcessamentoNoContabilizaSP = false)
        {
            SAMContext contextoBancoDeDados = null;
            string msgRetornoDeInformeAoUsuario = null;
            string fmtMsgInformeAoUsuario = null;
            bool integracaoSIAFEMAtivada = false;
            string cpfUsuarioSessaoLogada = null;
            string tituloJanelaModal = null;
            string msgConfirmacaoEnvioDadosContabeis = null;
            bool ehEstorno = false;
            User usuarioLogado = null;


            if (movementTypeId > 0)
            {
                this.initDadosSIAFEM();
                integracaoSIAFEMAtivada = this.ugeIntegradaSiafem;
                InitDisconnectContext(ref contextoBancoDeDados);
                fmtMsgInformeAoUsuario = null;


                if (listaMovimentacoesPatrimoniaisEmLote.HasElements())
                {
                    var tipoMovPatrimonial = contextoBancoDeDados.MovementTypes.Where(movPatrimonial => movPatrimonial.Id == movementTypeId).FirstOrDefault();
                    var ugeMovimentacaoId = (listaMovimentacoesPatrimoniaisEmLote.HasElements() ? listaMovimentacoesPatrimoniaisEmLote[0].ManagerUnitId : -1);
                    var ugeMovimentacaoPatrimonial = ((ugeMovimentacaoId > 0) ? contextoBancoDeDados.ManagerUnits.Where(ugeMovPatrimonial => ugeMovPatrimonial.Id == ugeMovimentacaoId).FirstOrDefault() : new ManagerUnit());
                    var codigoUGEMovimentacao = (ugeMovimentacaoPatrimonial.IsNotNull() ? ugeMovimentacaoPatrimonial.Code : "0");
                    var movimentacaoPatrimonial = (listaMovimentacoesPatrimoniaisEmLote.HasElements() ? listaMovimentacoesPatrimoniaisEmLote.First() : null);
                    string nomeTipoMovimentacaoPatrimonial = ((tipoMovPatrimonial.IsNotNull()) ? tipoMovPatrimonial.Description : "DESCONHECIDO");




                    var primeiraMovimentacaoPatrimonial = listaMovimentacoesPatrimoniaisEmLote.First();
                    int mesReferenciaMovimentacao = -1;
                    int mesReferenciaUGE = -1;
                    int numeroNLs = 1;
                    string nlLiquidacao = null;
                    string nlDepreciacao = null;
                    string nlReclassificacao = null;
                    string msgErroSIAFEM = null;
                    string strMesReferenciaUGE = ugeMovimentacaoPatrimonial.ManagmentUnit_YearMonthReference;
                    string strMesReferenciaMovimentacao = movimentacaoPatrimonial.MovimentDate.ToString("yyyyMM");
                    Int32.TryParse(strMesReferenciaMovimentacao, out mesReferenciaMovimentacao);
                    Int32.TryParse(strMesReferenciaUGE, out mesReferenciaUGE);
                    ehEstorno = (movimentacaoPatrimonial.Status == false &&
                                 movimentacaoPatrimonial.FlagEstorno == true &&
                                 movimentacaoPatrimonial.DataEstorno.HasValue &&
                                 movimentacaoPatrimonial.LoginEstorno.HasValue);
                    forcaNaoProcessamentoNoContabilizaSP = (((mesReferenciaUGE > 0) || (mesReferenciaMovimentacao > 0))
                                                            && ((mesReferenciaUGE <= 202005) || (mesReferenciaMovimentacao <= 202005)));

                    fmtMsgInformeAoUsuario = (!ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoExecutadaSucesso : IntegracaoContabilizaSPController.fmtMsgEstornoExecutadoSucesso);
                    if (tipoMovPatrimonial.IsNotNull() && tipoMovPatrimonial.PossuiContraPartidaContabil() && ugeIntegradaSiafem && !forcaNaoProcessamentoNoContabilizaSP)
                    {
                        TipoNotaSIAF tipoNotaSIAFEM = TipoNotaSIAF.Desconhecido;
                        Tuple<string, string, string, string, bool> envioComandoGeracaoNL = null;
                        int auditoriaIntegracaoId = 0;
                        var svcIntegracaoSIAFEM = new IntegracaoContabilizaSPController();
                        usuarioLogado = UserCommon.CurrentUser();
                        cpfUsuarioSessaoLogada = (usuarioLogado.IsNotNull() ? usuarioLogado.CPF : null);

                        //exibir mensagens para o usuario
                        var msgsExtratoPreProcessamentoSIAFEM = svcIntegracaoSIAFEM.GeraMensagensExtratoPreProcessamentoSIAFEM(listaMovimentacoesPatrimoniaisEmLote, true);

                        //se usuario deu ok, fazer o processamento para o SIAFEM
                        //if (usuarioDeuOK para gerar NLs)
                        {
                            numeroNLs = msgsExtratoPreProcessamentoSIAFEM.Count();

                            foreach (var msgExtratoPreProcessamentoSIAFEM in msgsExtratoPreProcessamentoSIAFEM)
                            {
                                tipoNotaSIAFEM = msgExtratoPreProcessamentoSIAFEM.Item2;
                                auditoriaIntegracaoId = msgExtratoPreProcessamentoSIAFEM.Item3;

                                #region NL Liquidacao
                                //NL Liquidacao
                                if (tipoMovPatrimonial.ContraPartidaContabilLiquida() && tipoNotaSIAFEM == TipoNotaSIAF.NL_Liquidacao)
                                {
                                    envioComandoGeracaoNL = svcIntegracaoSIAFEM.NovoTipoDeProcessamentoMovimentacaoNoSIAF(auditoriaIntegracaoId, cpfUsuarioSessaoLogada, loginUsuarioSIAFEM, senhaUsuarioSIAFEM, false, tipoNotaSIAFEM);
                                    if (!envioComandoGeracaoNL.Item5) //se gerou NL SIAFEM
                                    {
                                        nlLiquidacao = envioComandoGeracaoNL.Item3; //captura a NL
                                        continue; //segue o loop
                                    }
                                    else
                                    {
                                        msgErroSIAFEM = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                        break; //interrompe o loop
                                    }
                                }
                                #endregion NL Liquidacao


                                #region NL Depreciacao
                                //NL Depreciacao
                                var tipoMovimentacaoNaoFazLiquidacao = (String.IsNullOrWhiteSpace(nlLiquidacao) && !tipoMovPatrimonial.ContraPartidaContabilLiquida());
                                var temNL_Liquidacao = !String.IsNullOrWhiteSpace(nlLiquidacao);

                                if ((((numeroNLs > 1) && temNL_Liquidacao) || tipoMovimentacaoNaoFazLiquidacao) && //'TipoMovimentacao vai gerar mais de uma NL E tem NL' OU 'TipoMovimentacao' nao faz liquidacao
                                    tipoMovPatrimonial.ContraPartidaContabilDeprecia()) //...e deprecia?
                                {
                                    envioComandoGeracaoNL = svcIntegracaoSIAFEM.NovoTipoDeProcessamentoMovimentacaoNoSIAF(auditoriaIntegracaoId, cpfUsuarioSessaoLogada, loginUsuarioSIAFEM, senhaUsuarioSIAFEM, false, tipoNotaSIAFEM);
                                    if (!envioComandoGeracaoNL.Item5) //se gerou NL SIAFEM
                                    {
                                        nlDepreciacao = envioComandoGeracaoNL.Item3; //captura a NL
                                        continue; //segue o loop
                                    }
                                    else
                                    {
                                        msgErroSIAFEM = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                        break; //interrompe o loop
                                    }
                                }
                                #endregion NL Depreciacao


                                #region NL Reclassificacao
                                //NL Reclassificacao
                                var temNL_Depreciacao = !String.IsNullOrWhiteSpace(nlDepreciacao);
                                if ((((numeroNLs > 1) && temNL_Depreciacao) || tipoMovimentacaoNaoFazLiquidacao) && //'TipoMovimentacao vai gerar mais de uma NL E tem NL' OU 'TipoMovimentacao' nao faz liquidacao
                                    tipoMovPatrimonial.ContraPartidaContabilReclassifica()) //...e reclassifica?
                                {
                                    envioComandoGeracaoNL = svcIntegracaoSIAFEM.NovoTipoDeProcessamentoMovimentacaoNoSIAF(auditoriaIntegracaoId, cpfUsuarioSessaoLogada, loginUsuarioSIAFEM, senhaUsuarioSIAFEM, false, tipoNotaSIAFEM);
                                    if (!envioComandoGeracaoNL.Item5) //se gerou NL SIAFEM
                                    {
                                        nlReclassificacao = envioComandoGeracaoNL.Item3; //captura a NL
                                        continue; //segue o loop
                                    }
                                    else
                                    {
                                        msgErroSIAFEM = envioComandoGeracaoNL.Item2; //captura o erro SIAFEM retornado (ou erro interno ocorrido)
                                        break; //interrompe o loop
                                    }
                                }
                                #endregion NL Reclassificacao
                            }

                            //se ocorreu erro SIAFEM
                            if (!String.IsNullOrWhiteSpace(msgErroSIAFEM))
                            {
                                ////devolver mensagem ao usuario
                                ////se usuario quiser abortar chamar rotina de limpeza
                                //if (retornoUsuarioEhParaAbortar)
                                //{
                                //
                                //}
                                ////se usuario quiser gerar 'Pendencia NL SIAFEM'
                                //else
                                //{
                                ////chamar rotina pra gerar pendencia
                                //}
                            }

                        }
                    }
                    else
                    {
                        fmtMsgInformeAoUsuario = (!ehEstorno ? IntegracaoContabilizaSPController.fmtMsgGravacaoExecutadaSucessoMovimentacaoSemContraPartidaContabilizaSP : IntegracaoContabilizaSPController.fmtMsgEstornoExecutadoSucessoMovimentacaoSemContraPartidaContabilizaSP);
                        msgRetornoDeInformeAoUsuario = String.Format(fmtMsgInformeAoUsuario,
                                                                     nomeTipoMovimentacaoPatrimonial,
                                                                     numeroDocumentoSAM,
                                                                     null);
                    }
                }
            }

            return msgRetornoDeInformeAoUsuario;
        }

        #region Regras de Negócio

        /// <summary>
        /// Verifica se tem Perfil relacionado ao usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        private bool TemPerfil(string usuario)
        {
            var relacaoUsuarioPerfil = (from u in db.Users
                                        join rup in db.RelationshipUserProfiles on u.Id equals rup.UserId
                                        where u.CPF == usuario && u.Status == true && rup.DefaultProfile
                                        select rup).FirstOrDefault();

            if (relacaoUsuarioPerfil.ProfileId > 0)
            {
                HttpContext.Items.Add("RupId", relacaoUsuarioPerfil.Id);
                HttpContext.Items.Add("CPF", usuario);
                HttpContext.Items.Add("perfilId", relacaoUsuarioPerfil.ProfileId);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se perfil é AdmGeral
        /// </summary>
        /// <returns></returns>
        protected bool PerfilAdmGeral()
        {
            return Session["administradorGeral"] != null;
        }

        /// <summary>
        /// Verifica se tem acesso a URL chamada
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="caminho"></param>
        /// <returns></returns>
        private bool AcessoURL(string usuario, string caminho)
        {
            if (usuario != "")
            {
                string url = RemoveNumerosUrl(caminho.ToUpper());

                if ((from m in db.Modules where m.Path.ToUpper() == url && m.ParentId.HasValue && m.Status select m.Id).Any())
                {
                    int? parentId = (from m in db.Modules where m.Path.ToUpper() == url && m.ParentId.HasValue && m.Status select m.ParentId).FirstOrDefault();

                    //Se módulo existente estiver inativo ou o módulo pai estiver inativo, não permite
                    if (parentId == null || !(from m in db.Modules where m.Id == parentId select m.Status).FirstOrDefault())
                    {
                        return false;
                    }
                }

                // Caso a Transacao nao esteja cadastrada no sistema ou caso seja uma do tipo servico deixar executar!
                if (!(from tt in db.Transactions where tt.Path.ToUpper() == url select tt.Id).Any() || (from tt in db.Transactions where tt.Path.ToUpper() == url && tt.TypeTransactionId == (int)EnumTypeTransaction.Servico select tt.Id).Any())
                    return true;

                int perfil = (int)HttpContext.Items["perfilId"];

                return (from p in db.Profiles
                        join rtp in db.RelationshipTransactionProfiles on p.Id equals rtp.ProfileId
                        join t in db.Transactions on rtp.TransactionId equals t.Id
                        join a in db.TypeTransactions on t.TypeTransactionId equals a.Id
                        where t.Path.ToUpper() == url && rtp.Status == true && t.Status == true && p.Id == perfil
                        select t.Id).Count() > 0;
            }
            else
                return false;
        }

        /// <summary>
        /// Controla as transações das Views
        /// </summary>
        /// <param name="user"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public JsonResult ControleTransacoes(string url)
        {
            // Utilizado para validar as transações das Views e Acesso as Urls
            if (!HttpContext.User.Identity.IsAuthenticated || Session["UsuarioAutenticado"] == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

            if (WebConfigurationManager.AppSettings["Densenvolvimento"] == "Ativo" || Session["administradorGeral"] != null)
            {
                int[] resultado = new int[7] { 1, 2, 3, 4, 5, 6, 7 };
                //resultado.Add(1); // Novo
                //resultado.Add(2); // Editar
                //resultado.Add(3); // Detalhe
                //resultado.Add(4); // Excluir
                //resultado.Add(5); // Listar
                //resultado.Add(6); // Servico
                //resultado.Add(7); // Exportação

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string urlFormatada = RemoveNumerosUrl(url.ToUpper());
                int perfilId = (int)HttpContext.Items["perfilId"];
                var resultado = (from p in db.Profiles
                                 join rtp in db.RelationshipTransactionProfiles on p.Id equals rtp.ProfileId
                                 join t in db.Transactions on rtp.TransactionId equals t.Id
                                 join a in db.TypeTransactions on t.TypeTransactionId equals a.Id
                                 where
                                 (t.Path.ToUpper().Contains(urlFormatada + "/") || t.Path.ToUpper() == urlFormatada)
                                 && rtp.Status == true
                                 && t.Status == true
                                 && p.Id == perfilId
                                 select a.Id).ToList();

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Remove os numeros da URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string RemoveNumerosUrl(string url)
        {
            return System.Text.RegularExpressions.Regex.Replace(url, @"[\d-]", string.Empty);
        }

        /// <summary>
        /// Redireciona para a tela principal e exibe a mensagem de erro
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="msg">Mensagem de Erro</param>
        private void MensagemErro(ActionExecutingContext filterContext, string msg)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            {
                controller = "Home",
                action = "Mensagem",
                m = msg
            }));
        }

        /// <summary>
        /// Redireciona para a tela principal e exibe a mensagem de erro
        /// </summary>
        /// <param name="msg">Mensagem de Erro</param>
        public ActionResult MensagemErro(string msg, Exception ex = null)
        {
            if (ex != null)
            {
                var msgErroDetalhada = new StringBuilder();


                if (ex.InnerException == null)
                    msgErroDetalhada.Append("Message: " + ex.Message);
                else if (ex.InnerException.InnerException != null)
                    msgErroDetalhada.Append("Message: " + ex.Message + " || " + "InnerException.InnerException: " + ex.InnerException.InnerException.Message);
                else if (ex.InnerException != null)
                    msgErroDetalhada.Append("Message: " + ex.Message + " || " + "InnerException: " + ex.InnerException.Message);

                if (ex == null)
                    msg = msgErroDetalhada.ToString();
                else
                    msg = msg + GravaLogErro(msgErroDetalhada.ToString(), ex.StackTrace, ex.TargetSite.Name);

                switch (ex.HResult)
                {
                    //(link para ajudar descobrir os erros:https://windows-hexerror.linestarve.com)
                    case -2147168220: //0x8004D024
                    case -2147467259: //0x80004005
                        return new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = "Home",
                            action = "SistemaSobrecarregado"
                        }));
                }
            }

            return new RedirectToRouteResult(new RouteValueDictionary(new
            {
                controller = "Home",
                action = "Mensagem",
                m = msg
            }));
        }

        /// <summary>
        /// Redireciona para a tela principal e exibe a mensagem de erro
        /// </summary>
        /// <param name="msg">Mensagem de Erro</param>
        public JsonResult MensagemErroJson(string msg, Exception ex = null)
        {
            MensagemModel mensagem = new MensagemModel();

            if (ex == null)
            {
                mensagem.Id = 0;
                mensagem.Mensagem = msg;
                return Json(mensagem, JsonRequestBehavior.AllowGet);
            }
            else
            {
                mensagem.Id = GravaLogErro(ex.Message, ex.StackTrace, ex.TargetSite.Name);
                mensagem.Mensagem = msg + mensagem.Id;

                return Json(mensagem, JsonRequestBehavior.AllowGet);
            }
        }

        public int GravaLogErro(string message, string stackTrace, string name)
        {
            ErroLog _erroLog = new ErroLog();
            _erroLog.DataOcorrido = DateTime.Now;
            _erroLog.exMessage = message;
            _erroLog.exStackTrace = stackTrace;
            _erroLog.exTargetSite = name;

            var usuarioLogado = Session["UsuarioLogado"];
            _erroLog.Usuario = usuarioLogado == null ? "" : usuarioLogado.ToString();

            db.ErroLogs.Add(_erroLog);
            db.SaveChanges();

            return _erroLog.Id;
        }

        public void GravaLogErroSemRetorno(Exception ex)
        {
            if (ex != null)
            {
                var msgErroDetalhada = new StringBuilder();

                if (ex.InnerException == null)
                    msgErroDetalhada.Append("Message: " + ex.Message);
                else if (ex.InnerException.InnerException != null)
                    msgErroDetalhada.Append("Message: " + ex.Message + " || " + "InnerException.InnerException: " + ex.InnerException.InnerException.Message);
                else if (ex.InnerException != null)
                    msgErroDetalhada.Append("Message: " + ex.Message + " || " + "InnerException: " + ex.InnerException.Message);

                ErroLog _erroLog = new ErroLog();
                _erroLog.DataOcorrido = DateTime.Now;
                _erroLog.exMessage = msgErroDetalhada.ToString();
                _erroLog.exStackTrace = ex.StackTrace;
                _erroLog.exTargetSite = ex.TargetSite.Name;

                var usuarioLogado = Session["UsuarioLogado"];
                _erroLog.Usuario = usuarioLogado == null ? "" : usuarioLogado.ToString();

                db.ErroLogs.Add(_erroLog);
                db.SaveChanges();
            }
        }

        public int GravaLogErro(Exception erroEmTempoExecucao)
        {
            bool temInnerException = false;
            string descricaoCompletaMensagemErro = null;


            temInnerException = erroEmTempoExecucao.InnerException.IsNotNull();
            descricaoCompletaMensagemErro = (temInnerException ? String.Format("Exception.Message:{0};InnerException.Message:{1}", erroEmTempoExecucao.Message, erroEmTempoExecucao.InnerException.Message) : String.Format("Exception.Message:{0}", erroEmTempoExecucao.Message));


            ErroLog _erroLog = new ErroLog();
            _erroLog.DataOcorrido = DateTime.Now;
            _erroLog.exMessage = descricaoCompletaMensagemErro;
            _erroLog.exStackTrace = erroEmTempoExecucao.StackTrace;
            //_erroLog.exTargetSite = erroEmTempoExecucao.TargetSite.Name;
            _erroLog.exTargetSite = new StackTrace(erroEmTempoExecucao).GetFrame(0).GetMethod().DeclaringType.Name;

            var usuarioLogado = Session["UsuarioLogado"];
            _erroLog.Usuario = usuarioLogado == null ? "" : usuarioLogado.ToString();

            db.ErroLogs.Add(_erroLog);
            db.SaveChanges();

            return _erroLog.Id;
        }

        #region HierarquiaLogin
        protected bool ValidarRequisicao(int? InstitutionId, int? BudgetUnitId, int? ManagerUnitId, int? AdministrativeUnitId, int? SectionId)
        {
            if (PerfilAdmGeral())
            {
                return true;
            }
            else
            {
                var perfilHierarquia = BuscaHierarquiaPerfilLogado((int)HttpContext.Items["RupId"]);

                if (InstitutionId != perfilHierarquia.InstitutionId && perfilHierarquia.InstitutionId != 0 && InstitutionId.IsNotNull())
                    return false;
                else if (BudgetUnitId != perfilHierarquia.BudgetUnitId && perfilHierarquia.BudgetUnitId != 0 && perfilHierarquia.BudgetUnitId.IsNotNull() && BudgetUnitId.IsNotNull())
                    return false;
                else if (ManagerUnitId != perfilHierarquia.ManagerUnitId && perfilHierarquia.ManagerUnitId != 0 && perfilHierarquia.ManagerUnitId.IsNotNull() && ManagerUnitId.IsNotNull())
                    return false;
                else if (AdministrativeUnitId != perfilHierarquia.AdministrativeUnitId && perfilHierarquia.AdministrativeUnitId != 0 && perfilHierarquia.AdministrativeUnitId.IsNotNull() && AdministrativeUnitId.IsNotNull())
                    return false;
                else if (SectionId != perfilHierarquia.SectionId && perfilHierarquia.SectionId != 0 && perfilHierarquia.SectionId.IsNotNull() && SectionId.IsNotNull())
                    return false;
                else
                    return true;
            }

        }

        protected RelationshipUserProfile BuscaPerfilLogado(int userId)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return db.RelationshipUserProfiles.FirstOrDefault(p => p.UserId == userId && p.DefaultProfile == true);
            return null;
        }
        public RelationshipUserProfile BuscaPerfilLogadoMobile(int userId)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return db.RelationshipUserProfiles.FirstOrDefault(p => p.UserId == userId && p.DefaultProfile == true);
            return null;
        }

        protected RelationshipUserProfileInstitution BuscaHierarquiaPerfilLogado(int rupId)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return db.RelationshipUserProfileInstitutions.FirstOrDefault(p => p.RelationshipUserProfileId == rupId && p.Status == true);
            return null;
        }

        public RelationshipUserProfileInstitution BuscaHierarquiaPerfilLogadoPorUsuario(int usuarioId)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return (from r in db.RelationshipUserProfiles
                        join ri in db.RelationshipUserProfileInstitutions on r.Id equals ri.RelationshipUserProfileId
                        where r.UserId == usuarioId && r.DefaultProfile == true
                        select ri).FirstOrDefault();
            return null;
        }

        public RelationshipUserProfileInstitution BuscaHierarquiaPerfilLogadoPorUsuarioTrocaSemVerificarLogado(int usuarioId)
        {
            return (from r in db.RelationshipUserProfiles
                    join ri in db.RelationshipUserProfileInstitutions on r.Id equals ri.RelationshipUserProfileId
                    where r.UserId == usuarioId && r.DefaultProfile == true
                    select ri).FirstOrDefault();
        }

        protected string BuscaDescricaoPerfil(int IdDoPerfil) //usado somente na controller de suporte
        {
            return (from p in db.Profiles where p.Id == IdDoPerfil select p.Description).FirstOrDefault();
        }
        #endregion

        protected List<int> BuscaUsuariosSuporteProdesp()
        {
            return (from r in db.RelationshipUserProfiles
                    where r.ProfileId == (int)EnumProfile.AdministradorGeral
                    select r.UserId).Distinct().ToList();
        }

        #endregion

        public bool UGEEstaComPendenciaSIAFEMNoFechamento(int? IdDaUGE)
        {
            if (IdDaUGE == null || IdDaUGE == 0)
                return false;


            var UGEIntegrada = UGEIntegradaAoSIAFEM(IdDaUGE);

            if (!UGEIntegrada)
            {
                return false;
            }
            else
            {
                int IdDaUGEComoInteiro = (int)IdDaUGE;
                using (var contextoBancoDeDados = new ClosingContext())
                {
                    contextoBancoDeDados.Configuration.AutoDetectChangesEnabled = false;
                    contextoBancoDeDados.Configuration.ProxyCreationEnabled = false;
                    contextoBancoDeDados.Configuration.LazyLoadingEnabled = false;

                    var UGE = (from m in contextoBancoDeDados.ManagerUnits
                               where m.Id == IdDaUGEComoInteiro
                               select m.Code).FirstOrDefault();

                    int CodigoUGE = Convert.ToInt32(UGE);

                    var listaSemNlFechamento = (from a in contextoBancoDeDados.AccountingClosings
                                                where a.ManagerUnitCode == CodigoUGE
                                                && (a.GeneratedNL == null || a.GeneratedNL == string.Empty)
                                                && a.DepreciationMonth > 0
                                                select a.ReferenceMonth).ToList();

                    var listaSemNlFechamentoAposeJunhoDoisMilEVinte = listaSemNlFechamento.Where(t => Convert.ToInt32(t) > 202006).Any();

                    if (listaSemNlFechamentoAposeJunhoDoisMilEVinte)
                        return true;
                    else
                    {
                        var mesRefAtual = (from m in contextoBancoDeDados.ManagerUnits
                                           where m.Id == IdDaUGEComoInteiro
                                           select m.ManagmentUnit_YearMonthReference).FirstOrDefault();


                        return (from a in contextoBancoDeDados.AccountingClosingExcluidos
                                where a.ManagerUnitCode == CodigoUGE
                                && a.DepreciationMonth > 0
                                && a.ReferenceMonth == mesRefAtual
                                && a.NLEstorno == null
                                select a).Count() > 0;
                    }

                }
            }
        }

        public bool UGEIntegradaAoSIAFEM(int? IdDaUGE)
        {
            if (IdDaUGE == null || IdDaUGE == 0)
                return false;

            using (var contextoBancoDeDados = new HierarquiaContext())
            {
                contextoBancoDeDados.Configuration.AutoDetectChangesEnabled = false;
                contextoBancoDeDados.Configuration.ProxyCreationEnabled = false;
                contextoBancoDeDados.Configuration.LazyLoadingEnabled = false;


                int IdDaUGEComoInteiro = (int)IdDaUGE;
                var UGEIntegrada = (from i in contextoBancoDeDados.Institutions
                                    join b in contextoBancoDeDados.BudgetUnits on i.Id equals b.InstitutionId
                                    join m in contextoBancoDeDados.ManagerUnits on b.Id equals m.BudgetUnitId
                                    where m.Id == IdDaUGEComoInteiro
                                    select i.flagIntegracaoSiafem || m.FlagIntegracaoSiafem).FirstOrDefault();

                return UGEIntegrada;
            }
        }
    }
}