using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.Mvc;
using System.Xml;
using Sam.Integracao.SIAF.Core;
using Sam.Integracao.SIAF.Mensagem.Enum;
using SAM.Web.Models;
using SAM.Web.Common;
using SAM.Web.ViewModels;
using System.Threading.Tasks;
using System.Data.Entity;
using AutoMapper;
using AutoMapper.Mappers;
using System.Linq.Expressions;
using SAM.Web.Common.Enum;
using LinqKit;
using Sam.Common.Util;
using static Sam.Common.Util.GeralEnum;
using System.ComponentModel;
using mTratamentoDados = Sam.Common.Util.TratamentoDados;




namespace SAM.Web.Controllers.IntegracaoContabilizaSP
{
    public class AuditoriaIntegracaoController : BaseController
    {
        //private SAMContext db = new SAMContext();internal static DateTime ValorMinimoDataAuditoria = new DateTime(1901, 01, 01);
        private Hierarquia hierarquia;
        /* mini-cache --> estrutura: CODIGO ORGAO; CODIGO UO; CODIGO UGE; ANO-MES-REFERENCIA;ID UGE */
        IList<Tuple<int, int, int, string, int>> dadosEstruturaOrganizacionalOrgao = null;
        private SAMContext contextoCamadaDados = new SAMContext();



        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _perfilId;



        public void getHierarquiaPerfil()
        {
            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
        }

        #region Actions Methods
        #region Index Actions
        // GET: AuditoriaIntegracaos
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            try
            {
                ViewBag.TemPermissao = PerfilAdmGeral();
                return View();
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        //public async Task<JsonResult> IndexJSONResult(AuditoriaIntegracao auditoriaIntegracao)
        [HttpPost]
        public async Task<JsonResult> IndexJSONResult()
        {
            string draw = Request.Form["draw"].ToString();
            string order = Request.Form["order[0][column]"].ToString();
            string orderDir = Request.Form["order[0][dir]"].ToString();
            int startRec = Convert.ToInt32(Request.Form["start"].ToString());
            int length = Convert.ToInt32(Request.Form["length"].ToString());
            string currentFilter = Request.Form["currentFilter"].ToString();

            IEnumerable<AuditoriaIntegracaoViewModel> registrosTabelaIntegracao = null;
            Expression<Func<AuditoriaIntegracao, bool>> expWhere = null;
            Expression<Func<AuditoriaIntegracao, bool>> expWhereAuxiliar = null;
            IQueryable<AuditoriaIntegracao> qryConsulta;
            int totalRegistros = 0;



            try
            {
                getHierarquiaPerfil();
                initAuxiliarStructure();
                //initDadosNotasSIAFEM();
                InitDisconnectContext(ref contextoCamadaDados);



                expWhere = (filtroConsulta => filtroConsulta.Id > 0); //Pendencias NL 'ativas'
                switch (_perfilId.GetValueOrDefault())
                {
                    case (int)EnumProfile.AdministradordeOrgao:
                        {
                            expWhereAuxiliar = (filtroConsulta => filtroConsulta.RelatedManagerUnit
                                                                                .RelatedBudgetUnit
                                                                                .RelatedInstitution.Id == _institutionId);
                        }
                        break;
                    case (int)EnumProfile.AdministradordeUO:
                    case (int)EnumProfile.OperadordeUO:
                        {
                            expWhereAuxiliar = (filtroConsulta => filtroConsulta.RelatedManagerUnit
                                                                                .RelatedBudgetUnit.Id == _budgetUnitId);
                        }
                        break;

                    case (int)EnumProfile.OperadordeUGE: { expWhereAuxiliar = (filtroConsulta => filtroConsulta.ManagerUnitId == _managerUnitId); } break;
                }


                expWhere = (expWhereAuxiliar.IsNotNull() ? expWhere.And(expWhereAuxiliar) : expWhere);
                qryConsulta = contextoCamadaDados.AuditoriaIntegracoes
                                                 .Where(m => m.ManagerUnitId == 2665)                             
                                                 .Where(expWhere)
                                                 .AsQueryable();
                //qryConsulta = qryConsulta.Where(expWhere);
                registrosTabelaIntegracao = qryConsulta.AsNoTracking().OrderByDescending(s => s.DataEnvio).Select(await _instanciadorDTO());

                if (!string.IsNullOrEmpty(currentFilter) && !string.IsNullOrWhiteSpace(currentFilter))
                    registrosTabelaIntegracao = Pesquisa(registrosTabelaIntegracao.AsQueryable(), currentFilter);

                totalRegistros = registrosTabelaIntegracao.Count();
                registrosTabelaIntegracao = registrosTabelaIntegracao.AsQueryable().Skip(startRec).Take(length).ToList();



                //string descricaoTipoMovimentacao = null;
                //string descricaoAgrupamentoTipoMovimentacao = null;
                //string notaExcecao = null;
                //foreach (AuditoriaIntegracaoViewModel registroTabelaIntegracao in registrosTabelaIntegracao)
                //{
                //    //if (String.IsNullOrWhiteSpace(notaContabilizaSPPendente.AssetMovementIds))
                //    //{
                //    descricaoTipoMovimentacao = "\"TIPO INVALIDO\"";
                //    descricaoAgrupamentoTipoMovimentacao = "\"AGRUPAMENTO INVALIDO\"";
                //    notaExcecao = "\"PENDENCIA EM ESTADO INCONSISTENTE\"";

                //    //notaContabilizaSPPendente.ErroSIAFEM = ((String.IsNullOrWhiteSpace(notaContabilizaSPPendente.TipoAgrupamentoMovimentacaoPatrimonial) && String.IsNullOrWhiteSpace(notaContabilizaSPPendente.TipoMovimentacaoPatrimonial)) ? notaExcecao : notaContabilizaSPPendente.ErroSIAFEM);
                //    //notaContabilizaSPPendente.TipoAgrupamentoMovimentacaoPatrimonial = ((!String.IsNullOrWhiteSpace(notaContabilizaSPPendente.TipoAgrupamentoMovimentacaoPatrimonial)) ? notaContabilizaSPPendente.TipoAgrupamentoMovimentacaoPatrimonial : descricaoAgrupamentoTipoMovimentacao);
                //    //notaContabilizaSPPendente.TipoMovimentacaoPatrimonial = ((!String.IsNullOrWhiteSpace(notaContabilizaSPPendente.TipoMovimentacaoPatrimonial)) ? notaContabilizaSPPendente.TipoMovimentacaoPatrimonial : descricaoTipoMovimentacao);
                //    //}

                //    descricaoTipoMovimentacao = null;
                //    descricaoAgrupamentoTipoMovimentacao = null;
                //}

                return Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRegistros, recordsFiltered = totalRegistros, data = registrosTabelaIntegracao }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(MensagemErro(CommonMensagens.PadraoException, ex), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Details Actions

        // GET: AuditoriaIntegracaos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                var viewModel = contextoCamadaDados.AuditoriaIntegracoes
                                                    .Where(registroTabela => registroTabela.Id == id)
                                                    .Select(await _instanciadorDTO())
                                                    .FirstOrDefault();

                if (viewModel == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                if (ValidarRequisicao(_institutionId, _budgetUnitId, _managerUnitId, null, null))
                    return View(viewModel);
                else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);


            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion
        #region Edit Actions
        // GET: AuditoriaIntegracaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                    return MensagemErro(CommonMensagens.IdentificadorNulo);

                var viewModel = contextoCamadaDados.AuditoriaIntegracoes
                                                    .Where(registroTabela => registroTabela.Id == id)
                                                    .Select(await _instanciadorDTO())
                                                    .FirstOrDefault();

                if (viewModel == null)
                    return MensagemErro(CommonMensagens.RegistroNaoExistente);

                if (ValidarRequisicao(_institutionId, _budgetUnitId, _managerUnitId, null, null))
                {
                    viewModel.TemPermissao = (String.IsNullOrEmpty(viewModel.NotaLancamento) && String.IsNullOrWhiteSpace(viewModel.NotaLancamento));
                    if (!viewModel.TemPermissao) {
                        viewModel.TemPermissao = !viewModel.NotaLancamento.Contains("NL");
                    }
                    

                    return View(viewModel);
                }
                else
                    return MensagemErro(CommonMensagens.SemPermissaoDeAcesso);


            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // POST: AuditoriaIntegracaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit(FormCollection fc, [Bind(Include="Id, DataEnvio, MsgEstimuloWS, MsgRetornoWS, NomeSistema, UsuarioSAM, UsuarioSistemaExterno, ManagerUnitId, TokenAuditoriaIntegracao, DataRetorno, DocumentoId, TipoMovimento, Data, UgeOrigem, Gestao, Tipo_Entrada_Saida_Reclassificacao_Depreciacao, CpfCnpjUgeFavorecida, GestaoFavorecida, Item, TipoEstoque, Estoque, EstoqueDestino, EstoqueOrigem, TipoMovimentacao, ValorTotal, ControleEspecifico, ControleEspecificoEntrada, ControleEspecificoSaida, FonteRecurso, NLEstorno, Empenho, Observacao, NotaFiscal, ItemMaterial, NotaLancamento, MsgErro")]AuditoriaIntegracaoViewModel viewModel)
        public async Task<ActionResult> Edit(AuditoriaIntegracaoViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var contextoCamadaDados = new SAMContext())
                    {
                        var registroTabela = contextoCamadaDados.AuditoriaIntegracoes
                                                                .Where(registroTabelaXML => registroTabelaXML.Id == viewModel.Id)
                                                                .FirstOrDefault();

                        if (registroTabela.IsNull())
                        {
                            return MensagemErro(CommonMensagens.RegistroNaoExistente);
                        }
                        else
                        {
                            bool temPermissao = (String.IsNullOrEmpty(registroTabela.NotaLancamento) || (String.IsNullOrEmpty(viewModel.NotaLancamento) && !registroTabela.NotaLancamento.Contains("NL")));
                            if (temPermissao)
                            {
                                viewModel.TemPermissao = temPermissao;
                                //Metodo (_fillingDTO) criado apenas para contornar o caso de a viewModel nao trazer todas as mudancas feitas em tela...
                                registroTabela = PreencheAlteracaoesNaModel(registroTabela, viewModel);

                                contextoCamadaDados.Entry(registroTabela).State = EntityState.Modified;
                                contextoCamadaDados.SaveChanges();
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    return RedirectToAction("Index");
                }

                return MensagemErro(CommonMensagens.PadraoException);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }
        #endregion

        #region Metodos Privados
        private bool ContemValor(int? variavel)
        {
            bool retorno = false;
            if (variavel.HasValue && variavel != null && variavel != 0)
                retorno = true;
            return retorno;
        }

        private void SetCarregaHierarquia(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0)
        {
            getHierarquiaPerfil();
            hierarquia = new Hierarquia();

            if (PerfilAdmGeral())
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description", modelInstitutionId);
                if (modelInstitutionId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(null), "Id", "Description");
                if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(null), "Id", "Description");
            }
            else
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", modelInstitutionId);
                if (_budgetUnitId.HasValue && _budgetUnitId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(_budgetUnitId), "Id", "Description", modelBudgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(_institutionId), "Id", "Description", modelBudgetUnitId);
                if (_managerUnitId.HasValue && _managerUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(_managerUnitId), "Id", "Description", modelManagerUnitId);
                else if (modelBudgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
                else
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(_budgetUnitId), "Id", "Description", modelManagerUnitId);
            }
        }
        #endregion

        #endregion Actions Methods

        #region Metodos Adicionais
        [HttpGet]
        public ActionResult ConsultaTokenIntegracao(string tokenSAMPatrimonio)
        {
            DetalhePendenciaNotaLancamentoSIAFEMViewModel viewModel = null;



            try
            {
                //if (notaLancamentoPendenteSIAFEMCodigo > 0)
                //    viewModel = _instanciadorDTO_DetalhePendenciaNL(notaLancamentoPendenteSIAFEMCodigo);



                return PartialView("_detalhePendenciaNotaLancamentoSIAFEM", viewModel);
            }
            catch (Exception excErroRuntime)
            {
                return MensagemErro("Erro ao executar função 'PreencheDetalhesNotaLancamentoPendente'");
            }

        }


        public IQueryable<AuditoriaIntegracaoViewModel> Pesquisa(IQueryable<AuditoriaIntegracaoViewModel> lista, string textoPesquisa)
        {

            int paraValidacaoInteiro;
            long paraValidacaoLong;
            if (int.TryParse(textoPesquisa, out paraValidacaoInteiro))  //Inteiros pesquisam Documento e Data de Envio
            {
                lista = lista.Where(l => l.NotaFiscal.Contains(textoPesquisa));// || l.DataEnvio.Contains(textoPesquisa));
            }
            else if (long.TryParse(textoPesquisa, out paraValidacaoLong)) //Inteiros longos pesquisam Documento
            {
                lista = lista.Where(l => l.NotaFiscal.Contains(textoPesquisa));
            }
            else //Alfanuméricos pesquisa Tipo Nota, ErroSIAFEM e Data de Envio(formatada) 
            {
                var tipos = new int[Enum.GetValues(typeof(TipoNotaSIAF)).Length];
                int contTipos = 0;

                foreach (int str in Enum.GetValues(typeof(TipoNotaSIAF)))
                {
                    var attr = typeof(TipoNotaSIAF).GetField(((TipoNotaSIAF)str).ToString())
                                               .GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attr.Length > 0)
                    {
                        if ((attr[0] as DescriptionAttribute).Description.Contains(textoPesquisa))
                        {
                            tipos[contTipos] = str;
                        }
                        else
                        {
                            tipos[contTipos] = -1;
                        }
                    }

                    contTipos++;
                }

                if (textoPesquisa.Contains("/"))
                    lista = lista.Where(l => l.MsgErro.Contains(textoPesquisa) || l.DataHoraEnvio.ToString(BaseController.fmtDataFormatoBrasileiro).Contains(textoPesquisa));// || tipos.Contains((int)l.TipoNotaNumero));
                else
                {
                    lista = lista.Where(l => l.MsgErro.Contains(textoPesquisa) || l.DataHoraEnvio.ToString(BaseController.fmtDataFormatoBrasileiro).Contains(textoPesquisa));// || tipos.Contains((int)l.TipoNotaNumero));
                }
            }

            return lista;
        }
        private AuditoriaIntegracao obterEntidadeAuditoriaIntegracao(int auditoriaIntegracaoId)
        {
            AuditoriaIntegracao objEntidade = null;


            if (auditoriaIntegracaoId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    objEntidade = contextoCamadaDados.AuditoriaIntegracoes
                                                     .Where(auditoriaIntegracao => auditoriaIntegracao.Id == auditoriaIntegracaoId)
                                                     .FirstOrDefault();
                }

            return objEntidade;
        }
        private async Task<Func<AuditoriaIntegracao, AuditoriaIntegracaoViewModel>> _instanciadorDTO()
        {
            Func<AuditoriaIntegracao, AuditoriaIntegracaoViewModel> _actionSeletor = null;
            AuditoriaIntegracaoViewModel viewModel = null;
            AuditoriaIntegracao registroAuditoriaIntegracaoVinculado = null;

            if (!dadosEstruturaOrganizacionalOrgao.HasElements())
                initAuxiliarStructure();

            _actionSeletor = (registroAuditoriaIntegracao =>
                                                            {
                                                                registroAuditoriaIntegracaoVinculado = obterEntidadeAuditoriaIntegracao(registroAuditoriaIntegracao.Id);

                                                                viewModel = new AuditoriaIntegracaoViewModel();
                                                                viewModel.Id = registroAuditoriaIntegracao.Id;
                                                                viewModel.Orgao = (dadosEstruturaOrganizacionalOrgao.HasElements() ? (dadosEstruturaOrganizacionalOrgao.Where(dadoParaConsulta => dadoParaConsulta.Item5 == registroAuditoriaIntegracaoVinculado.ManagerUnitId)
                                                                                                                                                                       .Select(dadoParaConsulta => dadoParaConsulta.Item1.ToString())
                                                                                                                                                                       .FirstOrDefault()) : null);
                                                                viewModel.UO = (dadosEstruturaOrganizacionalOrgao.HasElements() ? (dadosEstruturaOrganizacionalOrgao.Where(dadoParaConsulta => dadoParaConsulta.Item5 == registroAuditoriaIntegracaoVinculado.ManagerUnitId)
                                                                                                                                                                    .Select(dadoParaConsulta => dadoParaConsulta.Item2.ToString())
                                                                                                                                                                    .FirstOrDefault()) : null);
                                                                viewModel.UGE = (dadosEstruturaOrganizacionalOrgao.HasElements() ? (dadosEstruturaOrganizacionalOrgao.Where(dadoParaConsulta => dadoParaConsulta.Item5 == registroAuditoriaIntegracaoVinculado.ManagerUnitId)
                                                                                                                                                                     .Select(dadoParaConsulta => dadoParaConsulta.Item3.ToString())
                                                                                                                                                                     .FirstOrDefault()) : null);

                                                                viewModel.DataHoraEnvio = registroAuditoriaIntegracaoVinculado.DataEnvio;
                                                                viewModel.DataHoraEnvioSIAFEM = registroAuditoriaIntegracaoVinculado.DataEnvio.ToString(BaseController.fmtDataHoraFormatoBrasileiro);
                                                                viewModel.DataHoraRetorno = registroAuditoriaIntegracaoVinculado.DataRetorno;
                                                                //viewModel.DataRetornoSIAFEM = registroAuditoriaIntegracaoVinculado.DataRetorno.GetValueOrDefault().ToString(BaseController.fmtDataHoraFormatoBrasileiro);
                                                                viewModel.DataHoraRetornoSIAFEM = (registroAuditoriaIntegracaoVinculado.DataRetorno.HasValue ? registroAuditoriaIntegracaoVinculado.DataRetorno.GetValueOrDefault().ToString(BaseController.fmtDataHoraFormatoBrasileiro) : String.Empty);
                                                                viewModel.NomeSistema = registroAuditoriaIntegracaoVinculado.NomeSistema;
                                                                viewModel.UsuarioSAM = registroAuditoriaIntegracaoVinculado.UsuarioSAM;
                                                                viewModel.UsuarioSistemaExterno = registroAuditoriaIntegracaoVinculado.UsuarioSistemaExterno;
                                                                //viewModel.InstitutionId = registroAuditoriaIntegracaoVinculado.InstitutionId;
                                                                //viewModel.BudgetUnitId = registroAuditoriaIntegracaoVinculado.BudgetUnitId;
                                                                viewModel.ManagerUnitId = registroAuditoriaIntegracaoVinculado.ManagerUnitId;
                                                                //viewModel.ContaContabil = registroAuditoriaIntegracaoVinculado.;
                                                                viewModel.TokenAuditoriaIntegracao = registroAuditoriaIntegracaoVinculado.TokenAuditoriaIntegracao;

                                                                /* Campos do XML Envio */
                                                                viewModel.DocumentoId = registroAuditoriaIntegracaoVinculado.DocumentoId;
                                                                viewModel.TipoMovimento = registroAuditoriaIntegracaoVinculado.TipoMovimento;
                                                                viewModel.Data = registroAuditoriaIntegracaoVinculado.Data;
                                                                viewModel.DataMovimento = (registroAuditoriaIntegracaoVinculado.Data.HasValue ? registroAuditoriaIntegracaoVinculado.Data.GetValueOrDefault().ToString(BaseController.fmtDataFormatoBrasileiro) : null);
                                                                viewModel.UgeOrigem = registroAuditoriaIntegracaoVinculado.UgeOrigem;
                                                                viewModel.Gestao = registroAuditoriaIntegracaoVinculado.Gestao;
                                                                viewModel.Tipo_Entrada_Saida_Reclassificacao_Depreciacao = registroAuditoriaIntegracaoVinculado.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;
                                                                viewModel.CpfCnpjUgeFavorecida = registroAuditoriaIntegracaoVinculado.CpfCnpjUgeFavorecida;
                                                                viewModel.GestaoFavorecida = registroAuditoriaIntegracaoVinculado.GestaoFavorecida;
                                                                viewModel.Item = registroAuditoriaIntegracaoVinculado.Item;
                                                                viewModel.TipoEstoque = registroAuditoriaIntegracaoVinculado.TipoEstoque;
                                                                viewModel.Estoque = registroAuditoriaIntegracaoVinculado.Estoque;
                                                                viewModel.EstoqueDestino = registroAuditoriaIntegracaoVinculado.EstoqueDestino;
                                                                viewModel.EstoqueOrigem = registroAuditoriaIntegracaoVinculado.EstoqueOrigem;
                                                                viewModel.TipoMovimentacao = registroAuditoriaIntegracaoVinculado.TipoMovimentacao;
                                                                viewModel.ValorTotal = registroAuditoriaIntegracaoVinculado.ValorTotal;
                                                                viewModel.ControleEspecifico = registroAuditoriaIntegracaoVinculado.ControleEspecifico;
                                                                viewModel.ControleEspecificoEntrada = registroAuditoriaIntegracaoVinculado.ControleEspecificoEntrada;
                                                                viewModel.ControleEspecificoSaida = registroAuditoriaIntegracaoVinculado.ControleEspecificoSaida;
                                                                viewModel.FonteRecurso = registroAuditoriaIntegracaoVinculado.FonteRecurso;
                                                                viewModel.NLEstorno = registroAuditoriaIntegracaoVinculado.NLEstorno;
                                                                viewModel.Empenho = registroAuditoriaIntegracaoVinculado.Empenho;
                                                                viewModel.Observacao = registroAuditoriaIntegracaoVinculado.Observacao;
                                                                viewModel.NotaFiscal = registroAuditoriaIntegracaoVinculado.NotaFiscal;
                                                                viewModel.ItemMaterial = registroAuditoriaIntegracaoVinculado.ItemMaterial;
                                                                viewModel.NotaLancamento = registroAuditoriaIntegracaoVinculado.NotaLancamento;
                                                                viewModel.MsgErro = registroAuditoriaIntegracaoVinculado.MsgErro;

                                                                viewModel.PossuiPendenciaNL = existePendenciaAtiva(registroAuditoriaIntegracao.Id);
                                                                //mTratamentoDados.ConverteStringNulaEmEspacoEmBranco(ref viewModel);
                                                                return viewModel;
                                                            });

            return _actionSeletor;
        }
        private AuditoriaIntegracao PreencheAlteracaoesNaModel(AuditoriaIntegracao entidadeTabela, AuditoriaIntegracaoViewModel viewModel) {
            entidadeTabela.TipoMovimento = viewModel.TipoMovimento;
            entidadeTabela.CpfCnpjUgeFavorecida = viewModel.CpfCnpjUgeFavorecida;
            entidadeTabela.Item = viewModel.Item;
            entidadeTabela.GestaoFavorecida = viewModel.GestaoFavorecida;
            entidadeTabela.Tipo_Entrada_Saida_Reclassificacao_Depreciacao = viewModel.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;
            entidadeTabela.TipoEstoque = viewModel.TipoEstoque;
            entidadeTabela.Estoque = viewModel.Estoque;
            entidadeTabela.EstoqueOrigem = viewModel.EstoqueOrigem; 
            entidadeTabela.EstoqueDestino = viewModel.EstoqueDestino;
            entidadeTabela.TipoMovimentacao = viewModel.TipoMovimentacao;
            entidadeTabela.ValorTotal = viewModel.ValorTotal;
            entidadeTabela.ControleEspecifico = viewModel.ControleEspecifico;
            entidadeTabela.UgeOrigem = viewModel.UgeOrigem;
            entidadeTabela.Gestao = viewModel.Gestao;
            entidadeTabela.NLEstorno = viewModel.NLEstorno;
            entidadeTabela.NotaFiscal = viewModel.NotaFiscal;
            entidadeTabela.ControleEspecificoEntrada = viewModel.ControleEspecificoEntrada;
            entidadeTabela.ControleEspecificoSaida = viewModel.ControleEspecificoSaida;
            entidadeTabela.FonteRecurso = viewModel.FonteRecurso;
            entidadeTabela.MsgErro = viewModel.MsgErro;

            return entidadeTabela;
        }
        private AuditoriaIntegracao _fillingDTO(ref AuditoriaIntegracao entidadeTabela, AuditoriaIntegracaoViewModel viewModel)
        {
            if (entidadeTabela.IsNotNull() && entidadeTabela.Id > 0)
            {
                int diaDateTime = 0;
                int mesDateTime = 0;
                int anoDateTime = 0;
                int horasDateTime = 0;
                int minutosDateTime = 0;
                int segundosDateTime = 0;
                string _dataEnvioSIAFEM = null;
                string _dataRetornoSIAFEM = null;
                string _dataMovimento = null;



                entidadeTabela.Id = viewModel.Id;
                //entidadeTabela.DataEnvio = viewModel.DataEnvio;

                {
                    if ((viewModel.DataHoraEnvio != DateTime.MinValue))
                    {
                        entidadeTabela.DataEnvio = viewModel.DataHoraEnvio;
                    }
                    else if ((viewModel.DataHoraEnvio == DateTime.MinValue) && String.IsNullOrWhiteSpace(viewModel.DataHoraEnvioSIAFEM))
                    {
                        //if (fc.IsNotNull() && fc["DataEnvioSIAFEM"].IsNotNull())
                        //{
                        //    _dataEnvioSIAFEM = fc["DataEnvioSIAFEM"];
                        //    diaDateTime = Int32.Parse(_dataEnvioSIAFEM.Substring(0, 2));
                        //    mesDateTime = Int32.Parse(_dataEnvioSIAFEM.Substring(3, 2));
                        //    anoDateTime = Int32.Parse(_dataEnvioSIAFEM.Substring(6, 4));
                        //    horasDateTime = Int32.Parse(_dataEnvioSIAFEM.Substring(11, 2));
                        //    minutosDateTime = Int32.Parse(_dataEnvioSIAFEM.Substring(14, 2));
                        //    segundosDateTime = Int32.Parse(_dataEnvioSIAFEM.Substring(17, 2));
                        //    entidadeTabela.DataEnvio = (new DateTime(anoDateTime, mesDateTime, diaDateTime, horasDateTime, minutosDateTime, segundosDateTime));
                        //}
                    }
                    else
                    {
                        _dataEnvioSIAFEM = viewModel.DataHoraEnvioSIAFEM;
                        if (!String.IsNullOrWhiteSpace(_dataEnvioSIAFEM))
                        {
                            diaDateTime = Int32.Parse(_dataEnvioSIAFEM.Substring(0, 2));
                            mesDateTime = Int32.Parse(_dataEnvioSIAFEM.Substring(3, 2));
                            anoDateTime = Int32.Parse(_dataEnvioSIAFEM.Substring(6, 4));
                            horasDateTime = Int32.Parse(_dataEnvioSIAFEM.Substring(11, 2));
                            minutosDateTime = Int32.Parse(_dataEnvioSIAFEM.Substring(14, 2));
                            segundosDateTime = Int32.Parse(_dataEnvioSIAFEM.Substring(17, 2));
                            entidadeTabela.DataEnvio = (new DateTime(anoDateTime, mesDateTime, diaDateTime, horasDateTime, minutosDateTime, segundosDateTime));
                        }
                    }
                }

                entidadeTabela.MsgEstimuloWS = viewModel.MsgEstimuloWS;
                entidadeTabela.MsgRetornoWS = viewModel.MsgRetornoWS;
                entidadeTabela.NomeSistema = viewModel.NomeSistema;
                entidadeTabela.UsuarioSAM = viewModel.UsuarioSAM;
                entidadeTabela.UsuarioSistemaExterno = viewModel.UsuarioSistemaExterno;
                entidadeTabela.ManagerUnitId = viewModel.ManagerUnitId;
                entidadeTabela.TokenAuditoriaIntegracao = viewModel.TokenAuditoriaIntegracao;
                //entidadeTabela.DataRetorno = viewModel.DataRetorno;

                {
                    if ((viewModel.DataHoraRetorno.GetValueOrDefault() != DateTime.MinValue))
                    {
                        entidadeTabela.DataRetorno = viewModel.DataHoraRetorno;
                    }
                    else if ((viewModel.DataHoraRetorno.GetValueOrDefault() == DateTime.MinValue) && String.IsNullOrWhiteSpace(viewModel.DataHoraRetornoSIAFEM))
                    {
                        //if (fc.IsNotNull() && fc["DataRetornoSIAFEM"].IsNotNull())
                        //{
                            //_dataRetornoSIAFEM = fc["DataRetornoSIAFEM"];
                            //if (!String.IsNullOrWhiteSpace(_dataRetornoSIAFEM))
                            //{
                            //    diaDateTime = Int32.Parse(_dataRetornoSIAFEM.Substring(0, 2));
                            //    mesDateTime = Int32.Parse(_dataRetornoSIAFEM.Substring(3, 2));
                            //    anoDateTime = Int32.Parse(_dataRetornoSIAFEM.Substring(6, 4));
                            //    horasDateTime = Int32.Parse(_dataRetornoSIAFEM.Substring(11, 2));
                            //    minutosDateTime = Int32.Parse(_dataRetornoSIAFEM.Substring(14, 2));
                            //    segundosDateTime = Int32.Parse(_dataRetornoSIAFEM.Substring(17, 2));
                            //    entidadeTabela.DataRetorno = (new DateTime(anoDateTime, mesDateTime, diaDateTime, horasDateTime, minutosDateTime, segundosDateTime));

                            //    entidadeTabela.DataRetorno = (entidadeTabela.DataRetorno == DateTime.MinValue ? null : entidadeTabela.DataRetorno);
                            //}
                        //}
                    }
                    else
                    {
                        _dataRetornoSIAFEM = viewModel.DataHoraRetornoSIAFEM;
                        if (!String.IsNullOrWhiteSpace(_dataRetornoSIAFEM))
                        {
                            diaDateTime = Int32.Parse(_dataRetornoSIAFEM.Substring(0, 2));
                            mesDateTime = Int32.Parse(_dataRetornoSIAFEM.Substring(3, 2));
                            anoDateTime = Int32.Parse(_dataRetornoSIAFEM.Substring(6, 4));
                            horasDateTime = Int32.Parse(_dataRetornoSIAFEM.Substring(11, 2));
                            minutosDateTime = Int32.Parse(_dataRetornoSIAFEM.Substring(14, 2));
                            segundosDateTime = Int32.Parse(_dataRetornoSIAFEM.Substring(17, 2));
                            entidadeTabela.DataRetorno = (new DateTime(anoDateTime, mesDateTime, diaDateTime, horasDateTime, minutosDateTime, segundosDateTime));

                            entidadeTabela.DataRetorno = (entidadeTabela.DataRetorno == DateTime.MinValue ? null : entidadeTabela.DataRetorno);
                        }
                    }
                }

                entidadeTabela.DocumentoId = viewModel.DocumentoId;
                entidadeTabela.TipoMovimento = viewModel.TipoMovimento;
                //entidadeTabela.Data = viewModel.Data;

                {
                    if ((viewModel.Data.GetValueOrDefault() != DateTime.MinValue))
                    {
                        entidadeTabela.Data = viewModel.Data;
                    }
                    else if ((viewModel.Data.GetValueOrDefault() == DateTime.MinValue) && String.IsNullOrWhiteSpace(viewModel.DataMovimento))
                    {
                        //if (fc.IsNotNull() && fc["DataMovimento"].IsNotNull())
                        //{
                        //    _dataMovimento = fc["DataMovimento"];
                        //    diaDateTime = Int32.Parse(_dataMovimento.Substring(0, 2));
                        //    mesDateTime = Int32.Parse(_dataMovimento.Substring(3, 2));
                        //    anoDateTime = Int32.Parse(_dataMovimento.Substring(6, 4));
                        //    horasDateTime = Int32.Parse(_dataMovimento.Substring(11, 2));
                        //    minutosDateTime = Int32.Parse(_dataMovimento.Substring(14, 2));
                        //    segundosDateTime = Int32.Parse(_dataMovimento.Substring(17, 2));
                        //    entidadeTabela.Data = (new DateTime(anoDateTime, mesDateTime, diaDateTime));
                        //}
                    }
                    else
                    {
                        _dataMovimento = viewModel.DataMovimento;
                        if (!String.IsNullOrWhiteSpace(_dataMovimento))
                        {
                            diaDateTime = Int32.Parse(_dataMovimento.Substring(0, 2));
                            mesDateTime = Int32.Parse(_dataMovimento.Substring(3, 2));
                            anoDateTime = Int32.Parse(_dataMovimento.Substring(6, 4));
                            entidadeTabela.Data = (new DateTime(anoDateTime, mesDateTime, diaDateTime));
                        }
                    }
                }

                entidadeTabela.UgeOrigem = viewModel.UgeOrigem;
                entidadeTabela.Gestao = viewModel.Gestao;
                entidadeTabela.Tipo_Entrada_Saida_Reclassificacao_Depreciacao = viewModel.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;
                entidadeTabela.CpfCnpjUgeFavorecida = viewModel.CpfCnpjUgeFavorecida;
                entidadeTabela.GestaoFavorecida = viewModel.GestaoFavorecida;
                entidadeTabela.Item = viewModel.Item;
                entidadeTabela.TipoEstoque = viewModel.TipoEstoque;
                entidadeTabela.Estoque = viewModel.Estoque;
                entidadeTabela.EstoqueDestino = viewModel.EstoqueDestino;
                entidadeTabela.EstoqueOrigem = viewModel.EstoqueOrigem;
                entidadeTabela.TipoMovimentacao = viewModel.TipoMovimentacao;
                entidadeTabela.ValorTotal = viewModel.ValorTotal;
                entidadeTabela.ControleEspecifico = viewModel.ControleEspecifico;
                entidadeTabela.ControleEspecificoEntrada = viewModel.ControleEspecificoEntrada;
                entidadeTabela.ControleEspecificoSaida = viewModel.ControleEspecificoSaida;
                entidadeTabela.FonteRecurso = viewModel.FonteRecurso;
                entidadeTabela.NLEstorno = viewModel.NLEstorno;
                entidadeTabela.Empenho = viewModel.Empenho;
                entidadeTabela.Observacao = viewModel.Observacao;
                entidadeTabela.NotaFiscal = viewModel.NotaFiscal;
                entidadeTabela.ItemMaterial = viewModel.ItemMaterial;
                entidadeTabela.MsgErro = viewModel.MsgErro;
            };

            return entidadeTabela;
        }
        private bool existePendenciaAtiva(int auditoriaIntegracaoId)
        {
            bool _existePendenciaAtiva = false;


            if (auditoriaIntegracaoId > 0)
                using (var contextoCamadaDados = new SAMContext())
                {
                    var objEntidade = contextoCamadaDados.NotaLancamentoPendenteSIAFEMs
                                                         .Where(notaLancamentoPendenteSIAFEM => notaLancamentoPendenteSIAFEM.AuditoriaIntegracaoId == auditoriaIntegracaoId
                                                                                             && notaLancamentoPendenteSIAFEM.StatusPendencia == 1 //pendencia NL SIAFEM ativa
                                                               )
                                                         .FirstOrDefault();

                    _existePendenciaAtiva = objEntidade.IsNotNull();
                }

            return _existePendenciaAtiva;
        }

        private void initAuxiliarStructure()
        {
            if (!dadosEstruturaOrganizacionalOrgao.HasElements())
            {
                int codigoOrgao = -1;
                int codigoUO = -1;
                int codigoUGE = -1;
                string anoMesReferencia = null;
                Expression<Func<ManagerUnit, bool>> expWhere = null;



                expWhere = (ugeSIAFEM => ugeSIAFEM.Status == true);
                if (!PerfilAdmGeral())
                    expWhere = expWhere.And(ugeSIAFEM => ugeSIAFEM.RelatedBudgetUnit.RelatedInstitution.Id == _institutionId);

                dadosEstruturaOrganizacionalOrgao = new List<Tuple<int, int, int, string, int>>();
                var _contextoCamadaDados = new SAMContext();
                _contextoCamadaDados.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
                _contextoCamadaDados.Configuration.LazyLoadingEnabled = false;
                _contextoCamadaDados.Configuration.ProxyCreationEnabled = false;
                _contextoCamadaDados.Configuration.AutoDetectChangesEnabled = false;
                var listaUGEs = _contextoCamadaDados.ManagerUnits
                                                    .AsNoTracking()
                                                    .Include("RelatedBudgetUnit.RelatedInstitution")
                                                    .Where(expWhere)
                                                    .ToList();
                foreach (var ugeSIAFEM in listaUGEs)
                {
                    Int32.TryParse(ugeSIAFEM.RelatedBudgetUnit.RelatedInstitution.Code, out codigoOrgao);
                    Int32.TryParse(ugeSIAFEM.RelatedBudgetUnit.Code, out codigoUO);
                    Int32.TryParse(ugeSIAFEM.Code, out codigoUGE);
                    anoMesReferencia = ugeSIAFEM.ManagmentUnit_YearMonthReference;

                    dadosEstruturaOrganizacionalOrgao.Add(Tuple.Create(codigoOrgao, codigoUO, codigoUGE, anoMesReferencia, ugeSIAFEM.Id));
                }
            }
        }
        #endregion Metodos Adicionais

        #region Metodos Regras de Negocio
        public bool InserirRegistro(AuditoriaIntegracao entidadeAuditoria)
        {
            int numeroRegistrosManipulados = 0;

            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {

                    contextoCamadaDados.AuditoriaIntegracoes.Add(entidadeAuditoria);
                    numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                var msgErro = "SAM|Erro ao inserir registro na trilha de auditoria de integração.";
                ErroLog rowTabelaLOG = new ErroLog() {
                                                          DataOcorrido = DateTime.Now
                                                        , Usuario = entidadeAuditoria.UsuarioSAM
                                                        , exMessage = msgErro
                                                        , exStackTrace = excErroOperacaoBancoDados.StackTrace
                                                        , exTargetSite = excErroOperacaoBancoDados.TargetSite.ToString()
                                                     };

                contextoCamadaDados.ErroLogs.Add(rowTabelaLOG);
            }

            return (numeroRegistrosManipulados > 0);
        }
        public bool InserirRegistro(AuditoriaIntegracao entidadeAuditoria, IEnumerable<int> listaMovimentacaoPatrimonialIds)
        {
            int numeroRegistrosManipulados = 0;
            int assetId = 0;
            Relacionamento__Asset_AssetMovements_AuditoriaIntegracao registroDeAmarracao = null;

            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (var movimentacaoPatrimonialId in listaMovimentacaoPatrimonialIds)
                    {
                        assetId = contextoCamadaDados.AssetMovements
                                                     .Where(movPatrimonial => movPatrimonial.Id == movimentacaoPatrimonialId)
                                                     .Select(movPatrimonial => movPatrimonial.AssetId)
                                                     .FirstOrDefault();

                        if (assetId > 0)
                        {
                            registroDeAmarracao = new Relacionamento__Asset_AssetMovements_AuditoriaIntegracao();
                            registroDeAmarracao.AuditoriaIntegracaoId = entidadeAuditoria.Id;
                            registroDeAmarracao.AssetMovementsId = movimentacaoPatrimonialId;

                            contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.Add(registroDeAmarracao);
                        }
                    }

                    contextoCamadaDados.AuditoriaIntegracoes.Add(entidadeAuditoria);
                    numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                var msgErro = "SAM|Erro ao inserir registro na trilha de auditoria de integração.";
                ErroLog rowTabelaLOG = new ErroLog()
                {
                    DataOcorrido = DateTime.Now
                                                        ,
                    Usuario = entidadeAuditoria.UsuarioSAM
                                                        ,
                    exMessage = msgErro
                                                        ,
                    exStackTrace = excErroOperacaoBancoDados.StackTrace
                                                        ,
                    exTargetSite = excErroOperacaoBancoDados.TargetSite.ToString()
                };

                contextoCamadaDados.ErroLogs.Add(rowTabelaLOG);
            }

            return (numeroRegistrosManipulados > 0);
        }
        public bool InserirRegistro(AuditoriaIntegracao entidadeAuditoria, IEnumerable<AssetMovements> listaMovimentacaoPatrimonial)
        {
            int numeroRegistrosManipulados = 0;
            int assetId = 0;
            Relacionamento__Asset_AssetMovements_AuditoriaIntegracao registroDeAmarracao = null;

            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (var movimentacaoPatrimonial in listaMovimentacaoPatrimonial)
                    {
                        registroDeAmarracao = new Relacionamento__Asset_AssetMovements_AuditoriaIntegracao();
                        registroDeAmarracao.AuditoriaIntegracaoId = entidadeAuditoria.Id;
                        registroDeAmarracao.AssetMovementsId = movimentacaoPatrimonial.Id;
                        registroDeAmarracao.AssetId = movimentacaoPatrimonial.AssetId;

                        contextoCamadaDados.Relacionamento__Asset_AssetMovements_AuditoriaIntegracaos.Add(registroDeAmarracao);
                    }


                    contextoCamadaDados.AuditoriaIntegracoes.Add(entidadeAuditoria);
                    numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                var msgErro = "SAM|Erro ao inserir registro na trilha de auditoria de integração.";
                ErroLog rowTabelaLOG = new ErroLog()
                {
                    DataOcorrido = DateTime.Now
                                                        ,
                    Usuario = entidadeAuditoria.UsuarioSAM
                                                        ,
                    exMessage = msgErro
                                                        ,
                    exStackTrace = excErroOperacaoBancoDados.StackTrace
                                                        ,
                    exTargetSite = excErroOperacaoBancoDados.TargetSite.ToString()
                };

                contextoCamadaDados.ErroLogs.Add(rowTabelaLOG);
            }

            return (numeroRegistrosManipulados > 0);
        }
        #endregion Metodos Regras de Negocio

    }

    public static class AuditoriaIntegracaoGeradorXml
    {
        static StringBuilder sbXml = new StringBuilder();
        static XmlWriter xmlMontadorEstimulo = null;
        static XmlWriterSettings xmlSettings = null;
        const int CST_TAMANHO_MAXIMO_CAMPO_OBSERVACAO_CONTABILIZASP = 231;
        const int CST_TAMANHO_MAXIMO_CAMPO_OBSERVACAO_SEM_TOKEN = 169;




        private static void init()
        {
            sbXml = null;
            xmlMontadorEstimulo = null;
            xmlSettings = null;

            sbXml = new StringBuilder();
            xmlSettings = obterFormatacaoPadraoParaXml();
            xmlMontadorEstimulo = XmlWriter.Create(sbXml, xmlSettings);
        }
        private static void dispose()
        {
            sbXml = null;
            xmlMontadorEstimulo = null;
            xmlSettings = null;
        }
        private static XmlWriterSettings obterFormatacaoPadraoParaXml()
        {
            return new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };
        }
        public static string SiafemDocNLPatrimonial(AuditoriaIntegracao registroAuditoriaIntegracao, bool isEstorno = false)
        {
            string strRetorno = null;
            string acaoPagamentoNotaLancamento = null;
            string tipoLancamento = null;
            string dataMovimentacao = null;



            tipoLancamento = ((isEstorno) ? "S" : "N");
            acaoPagamentoNotaLancamento = ((isEstorno) ? "E" : "N");


            init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteFullElementString("cdMsg", "SIAFNlPatrimonial");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocNlPatrimonial");
            xmlMontadorEstimulo.WriteStartElement("documento");

            #region Chave para MONITORASIAF
            //Monitoramento, via WS SIAF SIAFMONITORA
            xmlMontadorEstimulo.WriteAttributeString("id", registroAuditoriaIntegracao.DocumentoId);
            #endregion Chave para MONITORASIAF

            dataMovimentacao = registroAuditoriaIntegracao.Data.Value.ToString("dd/MM/yyyy");
            xmlMontadorEstimulo.WriteFullElementString("TipoMovimento", registroAuditoriaIntegracao.TipoMovimento);
            xmlMontadorEstimulo.WriteFullElementString("Data", dataMovimentacao);
            xmlMontadorEstimulo.WriteFullElementString("UgeOrigem", registroAuditoriaIntegracao.UgeOrigem);
            xmlMontadorEstimulo.WriteFullElementString("Gestao", registroAuditoriaIntegracao.Gestao);
            xmlMontadorEstimulo.WriteFullElementString("Tipo_Entrada_Saida_Reclassificacao_Depreciacao", registroAuditoriaIntegracao.Tipo_Entrada_Saida_Reclassificacao_Depreciacao);
            xmlMontadorEstimulo.WriteFullElementString("CpfCnpjUgeFavorecida", registroAuditoriaIntegracao.CpfCnpjUgeFavorecida);
            xmlMontadorEstimulo.WriteFullElementString("GestaoFavorecida", registroAuditoriaIntegracao.GestaoFavorecida);
            xmlMontadorEstimulo.WriteFullElementString("Item", registroAuditoriaIntegracao.Item);
            xmlMontadorEstimulo.WriteFullElementString("TipoEstoque", registroAuditoriaIntegracao.TipoEstoque);
            xmlMontadorEstimulo.WriteFullElementString("Estoque", registroAuditoriaIntegracao.Estoque);
            xmlMontadorEstimulo.WriteFullElementString("EstoqueDestino", registroAuditoriaIntegracao.EstoqueDestino);
            xmlMontadorEstimulo.WriteFullElementString("EstoqueOrigem", registroAuditoriaIntegracao.EstoqueOrigem);
            xmlMontadorEstimulo.WriteFullElementString("TipoMovimentacao", registroAuditoriaIntegracao.TipoMovimentacao);

            //VALOR MOVIMENTACAO
            string valorDoc = registroAuditoriaIntegracao.ValorTotal.Value.ToString("0,0.00");
            xmlMontadorEstimulo.WriteFullElementString("ValorTotal", valorDoc);

            xmlMontadorEstimulo.WriteFullElementString("ControleEspecifico", registroAuditoriaIntegracao.ControleEspecifico);
            xmlMontadorEstimulo.WriteFullElementString("ControleEspecificoEntrada", registroAuditoriaIntegracao.ControleEspecificoEntrada);
            xmlMontadorEstimulo.WriteFullElementString("ControleEspecificoSaida", registroAuditoriaIntegracao.ControleEspecificoSaida);
            xmlMontadorEstimulo.WriteFullElementString("FonteRecurso", "");
            xmlMontadorEstimulo.WriteFullElementString("NLEstorno", tipoLancamento);
            xmlMontadorEstimulo.WriteFullElementString("Empenho", registroAuditoriaIntegracao.Empenho);

            #region Campo Observacao
            //quebraDeLinha = Environment.NewLine;
            //string tokenSamPatrimonio = String.Format("'Token SAM-Patrimonio': '{0}'", registroAuditoriaIntegracao.TokenAuditoriaIntegracao.ToString());
            //string descricaoTipoMovimentacao = String.Format("'{0}':'{1}'", registroAuditoriaIntegracao.TipoMovimento, registroAuditoriaIntegracao.Tipo_Entrada_Saida_Reclassificacao_Depreciacao);
            //string documentoSAM = String.Format("'DocumentoSAM': '{0}'", registroAuditoriaIntegracao.NotaFiscal);
            ////observacaoMovimentacao = String.Format("{0}{1}{2}", observacaoMovimentacao, quebraDeLinha, tokenSamPatrimonio);
            //observacaoMovimentacao = String.Format("{1};{0}{2};", quebraDeLinha, descricaoTipoMovimentacao, documentoSAM);
            //int tamanhoString = ((observacaoMovimentacao.Length > CST_TAMANHO_MAXIMO_CAMPO_OBSERVACAO_SEM_TOKEN) ? CST_TAMANHO_MAXIMO_CAMPO_OBSERVACAO_SEM_TOKEN : observacaoMovimentacao.Length);
            //observacaoMovimentacao = observacaoMovimentacao.Substring(0, tamanhoString);
            //observacaoMovimentacao = String.Format("{0}{1}{2}.", observacaoMovimentacao, quebraDeLinha, tokenSamPatrimonio);
            //xmlMontadorEstimulo.WriteFullElementString("Observacao", observacaoMovimentacao);
            xmlMontadorEstimulo.WriteFullElementString("Observacao", registroAuditoriaIntegracao.Observacao);
            #endregion Campo Observacao


            xmlMontadorEstimulo.WriteEndElement();


            //CAMPO NOTA FISCAL
            xmlMontadorEstimulo.WriteStartElement("NotaFiscal");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");
            xmlMontadorEstimulo.WriteStartElement("NF");
            xmlMontadorEstimulo.WriteFullElementString("NotaFiscal", registroAuditoriaIntegracao.NotaFiscal);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();


            //CAMPO ITEM
            xmlMontadorEstimulo.WriteStartElement("ItemMaterial");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");
            xmlMontadorEstimulo.WriteStartElement("IM");
            xmlMontadorEstimulo.WriteFullElementString("ItemMaterial", registroAuditoriaIntegracao.ItemMaterial);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();


            xmlMontadorEstimulo.WriteFullEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            dispose();


            return strRetorno;
        }

        public static string SiafemDocNLPatrimonial(AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            string strRetorno = null;
            string acaoPagamentoNotaLancamento = null;
            string tipoLancamento = null;
            string dataMovimentacao = null;



            init();
            xmlMontadorEstimulo.WriteStartDocument(false);
            xmlMontadorEstimulo.WriteStartElement("SIAFDOC");
            xmlMontadorEstimulo.WriteFullElementString("cdMsg", "SIAFNlPatrimonial");
            xmlMontadorEstimulo.WriteStartElement("SiafemDocNlPatrimonial");
            xmlMontadorEstimulo.WriteStartElement("documento");

            #region Chave para MONITORASIAF
            //Monitoramento, via WS SIAF SIAFMONITORA
            xmlMontadorEstimulo.WriteAttributeString("id", registroAuditoriaIntegracao.DocumentoId);
            #endregion Chave para MONITORASIAF

            dataMovimentacao = registroAuditoriaIntegracao.Data.Value.ToString("dd/MM/yyyy");
            xmlMontadorEstimulo.WriteFullElementString("TipoMovimento", registroAuditoriaIntegracao.TipoMovimento);
            xmlMontadorEstimulo.WriteFullElementString("Data", dataMovimentacao);
            xmlMontadorEstimulo.WriteFullElementString("UgeOrigem", registroAuditoriaIntegracao.UgeOrigem);
            xmlMontadorEstimulo.WriteFullElementString("Gestao", registroAuditoriaIntegracao.Gestao);
            xmlMontadorEstimulo.WriteFullElementString("Tipo_Entrada_Saida_Reclassificacao_Depreciacao", registroAuditoriaIntegracao.Tipo_Entrada_Saida_Reclassificacao_Depreciacao);
            xmlMontadorEstimulo.WriteFullElementString("CpfCnpjUgeFavorecida", registroAuditoriaIntegracao.CpfCnpjUgeFavorecida);
            xmlMontadorEstimulo.WriteFullElementString("GestaoFavorecida", registroAuditoriaIntegracao.GestaoFavorecida);
            xmlMontadorEstimulo.WriteFullElementString("Item", registroAuditoriaIntegracao.Item);
            xmlMontadorEstimulo.WriteFullElementString("TipoEstoque", registroAuditoriaIntegracao.TipoEstoque);
            xmlMontadorEstimulo.WriteFullElementString("Estoque", registroAuditoriaIntegracao.Estoque);
            xmlMontadorEstimulo.WriteFullElementString("EstoqueDestino", registroAuditoriaIntegracao.EstoqueDestino);
            xmlMontadorEstimulo.WriteFullElementString("EstoqueOrigem", registroAuditoriaIntegracao.EstoqueOrigem);
            xmlMontadorEstimulo.WriteFullElementString("TipoMovimentacao", registroAuditoriaIntegracao.TipoMovimentacao);

            //VALOR MOVIMENTACAO
            string valorDoc = registroAuditoriaIntegracao.ValorTotal.Value.ToString("0,0.00");
            xmlMontadorEstimulo.WriteFullElementString("ValorTotal", valorDoc);

            xmlMontadorEstimulo.WriteFullElementString("ControleEspecifico", registroAuditoriaIntegracao.ControleEspecifico);
            xmlMontadorEstimulo.WriteFullElementString("ControleEspecificoEntrada", registroAuditoriaIntegracao.ControleEspecificoEntrada);
            xmlMontadorEstimulo.WriteFullElementString("ControleEspecificoSaida", registroAuditoriaIntegracao.ControleEspecificoSaida);
            xmlMontadorEstimulo.WriteFullElementString("FonteRecurso", registroAuditoriaIntegracao.FonteRecurso);
            xmlMontadorEstimulo.WriteFullElementString("NLEstorno", registroAuditoriaIntegracao.NLEstorno);
            xmlMontadorEstimulo.WriteFullElementString("Empenho", registroAuditoriaIntegracao.Empenho);
            xmlMontadorEstimulo.WriteFullElementString("Observacao", registroAuditoriaIntegracao.Observacao);


            xmlMontadorEstimulo.WriteEndElement();


            //CAMPO NOTA FISCAL
            xmlMontadorEstimulo.WriteStartElement("NotaFiscal");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");
            xmlMontadorEstimulo.WriteStartElement("NF");
            xmlMontadorEstimulo.WriteFullElementString("NotaFiscal", registroAuditoriaIntegracao.NotaFiscal);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();


            //CAMPO ITEM
            xmlMontadorEstimulo.WriteStartElement("ItemMaterial");
            xmlMontadorEstimulo.WriteStartElement("Repeticao");
            xmlMontadorEstimulo.WriteStartElement("IM");
            xmlMontadorEstimulo.WriteFullElementString("ItemMaterial", registroAuditoriaIntegracao.ItemMaterial);
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();
            xmlMontadorEstimulo.WriteEndElement();


            xmlMontadorEstimulo.WriteFullEndElement();
            xmlMontadorEstimulo.WriteEndDocument();

            xmlMontadorEstimulo.Flush();
            xmlMontadorEstimulo.Close();

            strRetorno = sbXml.ToString();
            dispose();


            return strRetorno;
        }
    }
    public class TrimAllStringProperty : IMappingAction<object, object>
    {
        public void Process(object source, object destination)
        {
            var stringProperties = destination.GetType().GetProperties().Where(p => p.PropertyType == typeof(string));
            foreach (var stringProperty in stringProperties)
            {
                string currentValue = (string)stringProperty.GetValue(destination, null);
                if (currentValue != null)
                    stringProperty.SetValue(destination, currentValue.Trim(), null);
            }
        }
    }
}