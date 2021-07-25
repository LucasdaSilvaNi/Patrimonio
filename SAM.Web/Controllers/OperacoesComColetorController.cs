using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using Sam.Common.Util;
using SAM.Web.Common;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using SAM.Web.Common.Enum;
using System.IO.Compression;
using System.Threading.Tasks;
using mFile = System.IO.File;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Data.Entity;

namespace SAM.Web.Controllers
{
    public static class StringExtensions
    {
        public static string ComplementaComEspacosAte(this string palavraParaComplementar, int numeroEspacos, bool truncaPalavra = false)
        {
            //if (!String.IsNullOrWhiteSpace(palavraParaComplementar) && numeroEspacos > 0)
            if ((palavraParaComplementar != null) && numeroEspacos > 0)
            {
                //for (int i = 0; i < numeroEspacos; i++)
                for (int i = palavraParaComplementar.Length; i < numeroEspacos; i++)
                    palavraParaComplementar += " ";
            }
            else if ((palavraParaComplementar == null) && numeroEspacos > 0)
            {
                palavraParaComplementar = "";
                palavraParaComplementar = palavraParaComplementar.ComplementaComEspacosAte(numeroEspacos, truncaPalavra);
            }

            return ((truncaPalavra) ? palavraParaComplementar.Substring(0, numeroEspacos) : palavraParaComplementar);
        }
        public static string RemoveUltimoCaracter(this string palavraParaRemovacaoCaracter)
        {
            if (!String.IsNullOrWhiteSpace(palavraParaRemovacaoCaracter))
                palavraParaRemovacaoCaracter = palavraParaRemovacaoCaracter.Remove(palavraParaRemovacaoCaracter.Length - 1);


            return palavraParaRemovacaoCaracter;
        }
        public static IEnumerable<string> QuebraLinhaSiglaChapa(this string siglaChapaBP)
        {
            var parteIdentificador = new List<string> { string.Empty };
            for (var i = 0; i < siglaChapaBP.Length; i++)
            {
                parteIdentificador[parteIdentificador.Count - 1] += siglaChapaBP[i];
                if (i + 1 < siglaChapaBP.Length && char.IsLetter(siglaChapaBP[i]) != char.IsLetter(siglaChapaBP[i + 1]))
                {
                    parteIdentificador.Add(string.Empty);
                }
            }
            return parteIdentificador;
        }
    }

    internal enum StatusBP_ColetorCOMPEX_CPX8000
    {
        OK = 0, //0-BP Ok
        TransferenciaUA = 1, //1-Transf.UA
        TransferenciaResponsavel = 2, //2-Transf.responsável
        NaoEncontrado = 3, //3-BP ñ encon
        OutraUA = 4, //4-BP de outra UA(ñ autoriz.)
        OutroResponsavelUA = 5 //5-BP de outro responsável UA(ñ autoriz.)
    }

    public class OperacoesComColetorController : BaseController
    {
        private SAMContext db = new SAMContext();
        string nomeArquivo = null;
        StreamWriter escritorArquivo = null;
        IList<string> linhasConteudoArquivo = null;
        IDictionary<int, IList<int>> listagemDadosResponsavelBP = null;

        #region Constantes
        const string nomePastaSaida = "Inventario";
        const string nomePastaExportacao = "Exportacao";
        const string nomePastaImportacao = "Importacao";
        const string OperacoesComColetor = "OperacoesComColetor";
        const string COMPEX_CPX8000 = "COMPEX_CPX8000";
        const string NomeArquivoConsultaPorColetor_BemPatrimonial = "TabBP.txt";
        const string NomeArquivoConsultaPorColetor_ItemMaterial = "TabMat.txt";
        const string NomeArquivoConsultaPorColetor_Responsavel = "TabResp.txt";
        const string NomeArquivoConsultaPorColetor_Sigla = "TabSigla.txt";
        const string NomeArquivoConsultaPorColetor_Terceiro = "TabTerc.txt";
        const int ComprimentoNomeArquivoColetor_COMPEX = 16;

        const string fmtMsgEstimuloInventarioWs = "{{ \"ResponsavelId\":\"{0}\", \"UaId\":\"{1}\", \"UgeId\":\"{2}\", \"Usuario\":\"{3}\", \"OrigemDados\":\"{4}\", \"TipoInventario\":\"{5}\" }}";
        const string fmtMsgEstimuloItemInventarioWs = "{{ \"Code\":\"{0}\", \"Estado\":\"{1}\", \"Item\":\"{2}\", \"InitialName\":\"{3}\", \"InventarioId\":\"{4}\", \"AssetId\":\"{5}\" }}";
        #endregion Constantes

        #region Sessao Aberta/Hierarquia
        private Hierarquia hierarquia = new Hierarquia();
        //private FunctionsCommon common = new FunctionsCommon();

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;
        private int? _administrativeUnitId;
        private int? _sectionId;
        private string _login;
        private string diretorioExecucaoSistema;
        private string diretorioExportacaoArquivos;
        private string diretorioImportacaoArquivos;
        private string diretorioOperacoesComColetor;

        public void getHierarquiaPerfil()
        {
            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuarioTrocaSemVerificarLogado(u.Id);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
            _administrativeUnitId = perflLogado.AdministrativeUnitId;
            _sectionId = perflLogado.SectionId;
            _login = u.CPF;
        }
        private void CarregaHierarquiaFiltro(int modelInstitutionId = 0, int modelBudgetUnitId = 0, int modelManagerUnitId = 0, int modelAdministrativeUnitId = 0, int modelSectionId = 0, bool mesmaGestao = false, string managerCode = null)
        {
            if (managerCode == null)
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(modelInstitutionId), "Id", "Description", modelInstitutionId);
            }
            else
            {
                if (mesmaGestao)
                    ViewBag.Institutions = new SelectList(hierarquia.GetOrgaosMesmaGestao(managerCode, modelInstitutionId), "Id", "Description", modelInstitutionId);
                else
                    ViewBag.Institutions = new SelectList(hierarquia.GetOrgaosGestaoDiferente(managerCode, modelInstitutionId), "Id", "Description", modelInstitutionId);
            }


            if (modelBudgetUnitId != 0)
                ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(modelBudgetUnitId), "Id", "Description", modelBudgetUnitId);
            else
                ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(modelInstitutionId), "Id", "Description", modelBudgetUnitId);

            if (modelManagerUnitId != 0)
                ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(modelManagerUnitId), "Id", "Description", modelManagerUnitId);
            else if (modelBudgetUnitId != 0)
                ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);
            else
                ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(modelBudgetUnitId), "Id", "Description", modelManagerUnitId);

            if (modelAdministrativeUnitId != 0)
                ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUas(modelAdministrativeUnitId), "Id", "Description", modelAdministrativeUnitId);
            else if (modelManagerUnitId != 0)
                ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);
            else
                ViewBag.AdministrativeUnits = new SelectList(hierarquia.GetUasPorUgeId(modelManagerUnitId), "Id", "Description", modelAdministrativeUnitId);

            if (modelSectionId != 0)
                ViewBag.Sections = new SelectList(hierarquia.GetDivisoes(modelSectionId), "Id", "Description", modelSectionId);
            else if (modelAdministrativeUnitId != 0)
                ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
            else
                ViewBag.Sections = new SelectList(hierarquia.GetDivisoesPorUaId(modelAdministrativeUnitId), "Id", "Description", modelSectionId);
        }
        #endregion


        [HttpGet]
        public ActionResult Leitura()
        {
            OperacoesComColetorViewModel operacoesViewModel = new OperacoesComColetorViewModel();

            getHierarquiaPerfil();


            //CarregaComboTipoOperacaoDispositivoInventariante();
            //CarregaComboTipoDispositivoInventariante();
            CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0, _administrativeUnitId ?? 0, _sectionId ?? 0);


            return View(operacoesViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Leitura(int tipoDispositivoInventariante = 0)
        {
            OperacoesComColetorViewModel operacoesViewModel = new OperacoesComColetorViewModel();

            ImportaArquivo_ColetorDados();
            return View(operacoesViewModel);
        }

        internal string geraEspacoEmBranco(int numeroEspacos)
        {
            string strSaida = null;
            for (int i = 0; i < numeroEspacos; i++)
                strSaida += " ";

            return strSaida;
        }
        internal void Init()
        {
            initDiretorioExecucaoSistema();
            initDiretorioRaizOperacoesComColetor();
            initDiretorioExportacaoArquivos();
            initDiretorioImportacaoArquivos();

            {
                //getHierarquiaPerfil();

                //CarregaComboTipoOperacaoDispositivoInventariante();
                //CarregaComboTipoDispositivoInventariante();


                //CarregaHierarquiaFiltro(_institutionId, _budgetUnitId ?? 0, _managerUnitId ?? 0, _administrativeUnitId ?? 0, _sectionId ?? 0);
            }

            LimpezaArquivosIntegracaoColetor();
            db.Database.CommandTimeout = 0;

            if (!verificaExistenciaDiretorios())
                throw new Exception("Erro ao inicializar módulo de integração com coletores");
        }

        internal bool verificaExistenciaDiretorios()
        {
            //DIRETORIO RAIZ DE OPERACOES COM COLETOR
            if (!String.IsNullOrWhiteSpace(diretorioOperacoesComColetor) && !Directory.Exists(diretorioOperacoesComColetor))
                //Directory.CreateDirectory(diretorioOperacoesComColetor, new DirectorySecurity(diretorioExecucaoSistema, AccessControlSections.All));
                Directory.CreateDirectory(diretorioOperacoesComColetor);

            //DIRETORIO EXPORTACAO ARQUIVOS COLETOR
            if (!String.IsNullOrWhiteSpace(diretorioExportacaoArquivos) && !Directory.Exists(diretorioExportacaoArquivos))
                Directory.CreateDirectory(diretorioExportacaoArquivos);

            //DIRETORIO IMPORTACAO ARQUIVOS COLETOR
            if (!String.IsNullOrWhiteSpace(diretorioImportacaoArquivos) && !Directory.Exists(diretorioImportacaoArquivos))
                Directory.CreateDirectory(diretorioImportacaoArquivos);



            return (Directory.Exists(diretorioOperacoesComColetor) &&
                    Directory.Exists(diretorioExportacaoArquivos) &&
                    Directory.Exists(diretorioImportacaoArquivos));
        }

        internal bool initDiretorioExecucaoSistema()
        {
            this.diretorioExecucaoSistema = null;


            if (HttpContext != null)
                diretorioExecucaoSistema = HttpContext.Request.PhysicalApplicationPath;
            else
                diretorioExecucaoSistema = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;


            diretorioExecucaoSistema = diretorioExecucaoSistema.RemoveUltimoCaracter();
            return !String.IsNullOrWhiteSpace(diretorioExecucaoSistema);
        }
        internal bool initDiretorioRaizOperacoesComColetor()
        {
            diretorioOperacoesComColetor = String.Format(@"{0}\{1}", diretorioExecucaoSistema, OperacoesComColetor);
            diretorioOperacoesComColetor.RemoveUltimoCaracter();

            return !String.IsNullOrWhiteSpace(diretorioImportacaoArquivos);
        }
        internal bool initDiretorioExportacaoArquivos()
        {
            diretorioExportacaoArquivos = String.Format(@"{0}\{1}\{2}", diretorioOperacoesComColetor, COMPEX_CPX8000, nomePastaExportacao);
            diretorioExportacaoArquivos.RemoveUltimoCaracter();

            return !String.IsNullOrWhiteSpace(diretorioExportacaoArquivos);
        }
        internal bool initDiretorioImportacaoArquivos()
        {
            diretorioImportacaoArquivos = String.Format(@"{0}\{1}\{2}", diretorioOperacoesComColetor, COMPEX_CPX8000, nomePastaImportacao);
            return !String.IsNullOrWhiteSpace(diretorioImportacaoArquivos);
        }



        public IList<string> LeituraArquivoExportadoColetor(string nomeArquivo)
        {
            IList<string> linhasConteudoArquivo = null;
            //string diretorioExecucaoSistema = null;
            string caminhoCompletoArquivoColetor = null;
            string linhaDadosBP = null;


            //if ((!String.IsNullOrWhiteSpace(nomeArquivo) && !String.IsNullOrWhiteSpace(nomePastaUpload)))
            if (!String.IsNullOrWhiteSpace(nomeArquivo))
            {
                //diretorioExecucaoSistema = obterDirecaoExecucaoSistema();
                //caminhoCompletoArquivoColetor = String.Format(@"{0}\{1}\{2}", diretorioExecucaoSistema, nomePastaUpload, nomeArquivo);
                caminhoCompletoArquivoColetor = String.Format(@"{0}\{1}", diretorioImportacaoArquivos, nomeArquivo);

                if (mFile.Exists(caminhoCompletoArquivoColetor))
                {
                    try
                    {
                        //using (StreamReader sr = new StreamReader(caminhoCompletoArquivoColetor))
                        using (StreamReader sr = new StreamReader(caminhoCompletoArquivoColetor, Encoding.GetEncoding(1252)))
                        {
                            linhasConteudoArquivo = new List<string>();
                            while ((linhaDadosBP = sr.ReadLine()) != null)
                                linhasConteudoArquivo.Add(linhaDadosBP);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }


            return linhasConteudoArquivo;
        }
        public void geraArquivoParaColetor_Siglas(int OrgaoID)
        {
            nomeArquivo = NomeArquivoConsultaPorColetor_Sigla;
            linhasConteudoArquivo = null;
            string linhaParaArquivo = null;
            int codigoOrgao = -1;



            var dadosOrgaoConsulta = db.Institutions.Where(orgaoSAM => orgaoSAM.Id == OrgaoID).FirstOrDefault();
            if (dadosOrgaoConsulta.IsNotNull() && Int32.TryParse(dadosOrgaoConsulta.Code, out codigoOrgao))
            {

                if (codigoOrgao != 20)
                {
                    linhasConteudoArquivo = new List<string>();
                    db.Initials.Where(siglaOrgao => siglaOrgao.RelatedInstitution.Code == codigoOrgao.ToString()
                                                 && siglaOrgao.Status == true
                                     )
                               .ToList()
                               .ForEach(siglaOrgao =>
                               {
                                   linhaParaArquivo = String.Format("{0}{1}", siglaOrgao.Name.ComplementaComEspacosAte(2, true), siglaOrgao.Description.ComplementaComEspacosAte(10, true));
                                   linhasConteudoArquivo.Add(linhaParaArquivo);
                               });
                }
                else if (codigoOrgao == 20)
                {
                    linhasConteudoArquivo = new List<string>() { "01BT        ", "02SF        " };
                }



                GravacaoArquivoIntegracaoColetor(linhasConteudoArquivo, nomeArquivo);
            }
        }
        public void geraArquivoParaColetor_ItemMaterial(int OrgaoID = 0)
        {
            nomeArquivo = NomeArquivoConsultaPorColetor_ItemMaterial;
            string codigoItemMaterial = null;
            string descricaoItemMaterial = null;
            string linhaParaArquivo = null;




            linhasConteudoArquivo = new List<string>();
            var registrosItemMaterialSistema = db.ItemSiafisicos.ToList();
            registrosItemMaterialSistema.ForEach(dadosItemMaterial => 
                                                                      {
                                                                          codigoItemMaterial = dadosItemMaterial.Cod_Item_Mat.ToString("000000000");
                                                                      
                                                                          descricaoItemMaterial = dadosItemMaterial.Nome_Item_Mat;
                                                                          descricaoItemMaterial = ((descricaoItemMaterial.Length > 20) ? (descricaoItemMaterial.Substring(0, 20)) : descricaoItemMaterial);
                                                                          descricaoItemMaterial = descricaoItemMaterial.ComplementaComEspacosAte(20, true);
                                                                      
                                                                          linhaParaArquivo = String.Format("{0}{1}", codigoItemMaterial, descricaoItemMaterial);
                                                                          linhaParaArquivo = linhaParaArquivo.ComplementaComEspacosAte(30, true);
                                                                          linhasConteudoArquivo.Add(linhaParaArquivo);
                                                                      
                                                                      });


            GravacaoArquivoIntegracaoColetor(linhasConteudoArquivo, nomeArquivo);
        }
        public void geraArquivoParaColetor_Responsavel(int? OrgaoID = null, int? UgeID = null, int? UaID = null)
        {
            string nomeResponsavel = null;
            string cargoResponsavel = null;
            string cpfResponsavel = null;
            string pseudoCodigoResponsavel = null;
            string linhaParaArquivo = null;
            int _codigoUA = 0;
            int _codigoUA_UGE = 0;
            int idxResponsavel = 0;
            string CodigoUGE = null;
            IList<AdministrativeUnit> relacaoUAs_UGESelecionada = null;
            IList<DadosGeraColetorResponsaveisViewModel> listaResponsaveis = null;



            nomeArquivo = NomeArquivoConsultaPorColetor_Responsavel;
            linhasConteudoArquivo = new List<string>();
            var _linhasConteudoArquivo = new List<string>();


            string codigoOrgao = db.Institutions
                                .Where(orgaoSAM => orgaoSAM.Id == OrgaoID && orgaoSAM.Status == true)
                                .OrderByDescending(orgao => orgao.Id)
                                .Select(i => i.Code)
                                .FirstOrDefault();

            if (!string.IsNullOrEmpty(codigoOrgao) && !string.IsNullOrWhiteSpace(codigoOrgao))
            {
                var dadosUaConsultada = db.AdministrativeUnits.Where(uaSIAFEM => uaSIAFEM.Id == UaID).FirstOrDefault();
                var relacaoResponsaveisUaConsultada = db.Responsibles.Where(responsavelBP_Ua => responsavelBP_Ua.AdministrativeUnitId == UaID).ToList();

                CodigoUGE = db.ManagerUnits.Where(ugeSIAFEM => ugeSIAFEM.Id == UgeID).Select(m => m.Code).FirstOrDefault();
                var relacaoResponsaveisUgeConsultada = db.Responsibles.Where(responsavelBP_Uge => responsavelBP_Uge.RelatedAdministrativeUnits.RelatedManagerUnit.Id == UgeID).ToList();

                if (UgeID.HasValue && UgeID.Value > 0)
                {
                    if (!string.IsNullOrEmpty(CodigoUGE) && !string.IsNullOrWhiteSpace(CodigoUGE))
                    {
                        relacaoUAs_UGESelecionada = new List<AdministrativeUnit>();
                        relacaoUAs_UGESelecionada = db.AdministrativeUnits.Where(uasUGEConsultada => uasUGEConsultada.RelatedManagerUnit.Code == CodigoUGE).ToList();
                    }
                }

                if (dadosUaConsultada.IsNotNull())
                {
                    listaResponsaveis = (from r in db.Responsibles
                                         join a in db.AdministrativeUnits on r.AdministrativeUnitId equals a.Id
                                         where a.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code == codigoOrgao
                                         && a.Status
                                         && a.Code == dadosUaConsultada.Code
                                         && r.Status
                                         select new DadosGeraColetorResponsaveisViewModel {
                                             IdResponsavel = r.Id,
                                             CodigoUA = a.Code,
                                             NomeResponsavel = r.Name,
                                             CPFResponsavel = r.CPF,
                                             CargoResponsavel = r.Position
                                         }).ToList();
                }
                //REUNIAO SEFAZ @03/09/2019 --> GERAR ARQUIVO COM TODOS OS BPs DE TODAS AS UAs DA UGE SELECIONADA
                else if ((UaID <= 0) && !string.IsNullOrEmpty(CodigoUGE) && !string.IsNullOrWhiteSpace(CodigoUGE)) {
                    listaResponsaveis = (from r in db.Responsibles
                                         join a in db.AdministrativeUnits on r.AdministrativeUnitId equals a.Id
                                         where a.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code == codigoOrgao
                                         && a.RelatedManagerUnit.Code == CodigoUGE
                                         && a.Code == dadosUaConsultada.Code
                                         && r.Status
                                         select new DadosGeraColetorResponsaveisViewModel
                                         {
                                             IdResponsavel = r.Id,
                                             CodigoUA = a.Code,
                                             NomeResponsavel = r.Name,
                                             CPFResponsavel = r.CPF,
                                             CargoResponsavel = r.Position
                                         }).ToList();
                }
                else {
                    listaResponsaveis = (from r in db.Responsibles
                                         join a in db.AdministrativeUnits on r.AdministrativeUnitId equals a.Id
                                         where a.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code == codigoOrgao
                                         && r.Status
                                         select new DadosGeraColetorResponsaveisViewModel
                                         {
                                             IdResponsavel = r.Id,
                                             CodigoUA = a.Code,
                                             NomeResponsavel = r.Name,
                                             CPFResponsavel = r.CPF,
                                             CargoResponsavel = r.Position
                                         }).ToList();
                }

                IList<int> relacaoID_Responsaveis = null;

                if ((UaID > 0) && relacaoResponsaveisUaConsultada.HasElements())
                {
                    _codigoUA = dadosUaConsultada.Code;
                    listagemDadosResponsavelBP = new Dictionary<int, IList<int>>();
                    foreach (var responsavelAssociadoUA in relacaoResponsaveisUaConsultada)
                    {
                        if (listagemDadosResponsavelBP.ContainsKey(_codigoUA))
                        {
                            relacaoID_Responsaveis = listagemDadosResponsavelBP[_codigoUA];
                            if (!relacaoID_Responsaveis.Contains(responsavelAssociadoUA.Id))
                                listagemDadosResponsavelBP[_codigoUA].Add(responsavelAssociadoUA.Id);
                        }
                        else if (!listagemDadosResponsavelBP.ContainsKey(_codigoUA))
                        {
                            listagemDadosResponsavelBP.InserirValor(_codigoUA, new List<int>());
                            listagemDadosResponsavelBP[_codigoUA].Add(responsavelAssociadoUA.Id);
                        }

                    }
                }
                //REUNIAO SEFAZ @03/09/2019 --> GERAR ARQUIVO COM TODOS OS BPs DE TODAS AS UAs DA UGE SELECIONADA
                else if ((UaID <= 0) && relacaoUAs_UGESelecionada.HasElements() && relacaoResponsaveisUgeConsultada.HasElements())
                {
                    _codigoUA_UGE = 0;
                    listagemDadosResponsavelBP = new Dictionary<int, IList<int>>();
                    foreach (var ua_UGESelecionada in relacaoUAs_UGESelecionada)
                    {
                        _codigoUA_UGE = ua_UGESelecionada.Code;
                        foreach (var responsavelAssociadoUA_UGE in relacaoResponsaveisUgeConsultada)
                        {
                            if (responsavelAssociadoUA_UGE.RelatedAdministrativeUnits.Code == _codigoUA_UGE)
                                if (listagemDadosResponsavelBP.ContainsKey(_codigoUA_UGE))
                                {
                                    relacaoID_Responsaveis = listagemDadosResponsavelBP[_codigoUA_UGE];
                                    if (!relacaoID_Responsaveis.Contains(responsavelAssociadoUA_UGE.Id))
                                        listagemDadosResponsavelBP[_codigoUA_UGE].Add(responsavelAssociadoUA_UGE.Id);
                                }
                                else if (!listagemDadosResponsavelBP.ContainsKey(_codigoUA_UGE))
                                {
                                    listagemDadosResponsavelBP.InserirValor(_codigoUA_UGE, new List<int>());
                                    listagemDadosResponsavelBP[_codigoUA_UGE].Add(responsavelAssociadoUA_UGE.Id);
                                }
                        }
                    }
                }

                listagemDadosResponsavelBP = (!listagemDadosResponsavelBP.HasElements() ? (new Dictionary<int, IList<int>>()) : (listagemDadosResponsavelBP));
                listaResponsaveis.OrderByDescending(r => r.IdResponsavel)
                                 .ToList()
                                 .ForEach(responsavelUA =>
                                    {
                                        nomeResponsavel = (!String.IsNullOrWhiteSpace(responsavelUA.NomeResponsavel) ? responsavelUA.NomeResponsavel.Trim() : "");
                                        cargoResponsavel = (!String.IsNullOrWhiteSpace(responsavelUA.CargoResponsavel) ? responsavelUA.CargoResponsavel.Trim() : "");
                                        cargoResponsavel = (!String.IsNullOrWhiteSpace(cargoResponsavel) ? cargoResponsavel : "Indefinido");

                                        cpfResponsavel = (!String.IsNullOrWhiteSpace(responsavelUA.CPFResponsavel) ? responsavelUA.CPFResponsavel.Trim() : "RespSemCPF");

                                        if (String.IsNullOrWhiteSpace(responsavelUA.pseudoCodigoResponsavel) && (listagemDadosResponsavelBP.ContainsKey(responsavelUA.CodigoUA)))
                                        {
                                            if (listagemDadosResponsavelBP[responsavelUA.CodigoUA] != null)
                                                if (!(listagemDadosResponsavelBP[responsavelUA.CodigoUA]).Contains(responsavelUA.IdResponsavel))
                                                    listagemDadosResponsavelBP[responsavelUA.CodigoUA].Add(responsavelUA.IdResponsavel);

                                        }
                                        else if (String.IsNullOrWhiteSpace(pseudoCodigoResponsavel) && (!listagemDadosResponsavelBP.ContainsKey(responsavelUA.CodigoUA)))
                                        {
                                            listagemDadosResponsavelBP.InserirValor(responsavelUA.CodigoUA, new List<int>());
                                            listagemDadosResponsavelBP[responsavelUA.CodigoUA].Add(responsavelUA.IdResponsavel);
                                        }

                                        idxResponsavel = (listagemDadosResponsavelBP[responsavelUA.CodigoUA]).IndexOf(responsavelUA.IdResponsavel);
                                        responsavelUA.pseudoIdResponsavel = idxResponsavel.ToString("D3");

                                        linhaParaArquivo = String.Format("{0}{1}{2}{3}{4}{5}", responsavelUA.CodigoUA.ToString("D6")//.PadLeft(6, '0')
                                                                                                , string.IsNullOrEmpty(responsavelUA.pseudoCodigoResponsavel) || string.IsNullOrWhiteSpace(responsavelUA.pseudoCodigoResponsavel) ? responsavelUA.pseudoIdResponsavel.ComplementaComEspacosAte(3, true) : responsavelUA.pseudoCodigoResponsavel.ComplementaComEspacosAte(3, true)
                                                                                                , nomeResponsavel.ComplementaComEspacosAte(30, true) /* tamanho campo na base do coletor */
                                                                                                , cargoResponsavel.ComplementaComEspacosAte(20, true) /* tamanho campo na base do coletor */
                                                                                                , cpfResponsavel.ComplementaComEspacosAte(10, true) /* tamanho campo na base do coletor; truncado em 10 digitos */
                                                                                                , "1"
                                                                        );

                                        linhasConteudoArquivo.Add(linhaParaArquivo);
                                    });

                GravacaoArquivoIntegracaoColetor(linhasConteudoArquivo, nomeArquivo);
            }

        }
        public void geraArquivoParaColetor_BemPatrimonial(int? OrgaoID = null, int? UgeID = null, int? UaID = null)
        {
            nomeArquivo = NomeArquivoConsultaPorColetor_BemPatrimonial;
            string estadoConservacao = null;
            string siglaChapaBP = null;
            string numeroSerialBP = null;
            string linhaParaArquivo = null;
            int _codigoUA = 0;
            int _codigoUA_UGE = 0;
            IList<AdministrativeUnit> relacaoUAs_UGESelecionada = null;
            IList<DadosGeraColetorBPViewModel> massaDadosBPs;

            string codigoOrgao = db.Institutions
                                .Where(orgaoSAM => orgaoSAM.Id == OrgaoID && orgaoSAM.Status == true)
                                .OrderByDescending(orgao => orgao.Id)
                                .Select(i => i.Code)
                                .FirstOrDefault();

            if (!string.IsNullOrEmpty(codigoOrgao) && !string.IsNullOrWhiteSpace(codigoOrgao))
            {

                var relacaoResponsaveisUaConsultada = db.Responsibles.Where(responsavelBP_Ua => responsavelBP_Ua.AdministrativeUnitId == UaID && responsavelBP_Ua.Status == true).ToList();

                string CodigoUGE = db.ManagerUnits
                                     .Where(uge => uge.Id == UgeID && uge.Status == true)
                                     .OrderByDescending(uge => uge.Id)
                                     .Select(uge => uge.Code)
                                     .FirstOrDefault();

                var dadosUaConsultada = db.AdministrativeUnits.Where(uaSIAFEM => uaSIAFEM.Id == UaID).OrderByDescending(ua => ua.Id).FirstOrDefault();
                if (dadosUaConsultada.IsNotNull() && dadosUaConsultada.Code > 0)
                {
                    massaDadosBPs = (from a in db.Assets
                                     join am in db.AssetMovements on a.Id equals am.AssetId
                                     join i in db.Institutions on am.InstitutionId equals i.Id
                                     where am.Status
                                     && am.ResponsibleId != null
                                     && am.RelatedAdministrativeUnit.Code == dadosUaConsultada.Code
                                     && i.Code == codigoOrgao
                                     select new DadosGeraColetorBPViewModel
                                     {
                                         Sigla = a.InitialName,
                                         Chapa = a.NumberIdentification,
                                         CodigoUA = am.RelatedAdministrativeUnit.Code,
                                         IdEstadoConservacao = am.StateConservationId,
                                         NumeroDeSerie = a.SerialNumber,
                                         CPFResponsavel = am.RelatedResponsible.CPF,
                                         IdResponsavel = am.ResponsibleId ?? 0,
                                         CodigoItemMaterial = a.MaterialItemCode,
                                         IdUA = am.AdministrativeUnitId
                                     }).ToListReadUncommitted();

                }
                //REUNIAO SEFAZ @03/09/2019 --> GERAR ARQUIVO COM TODOS OS BPs DE TODAS AS UAs DA UGE SELECIONADA
                else if ((UaID <= 0) && !string.IsNullOrEmpty(CodigoUGE) && !string.IsNullOrWhiteSpace(CodigoUGE))
                {
                    massaDadosBPs = (from a in db.Assets
                                     join am in db.AssetMovements on a.Id equals am.AssetId
                                     join i in db.Institutions on am.InstitutionId equals i.Id
                                     join m in db.ManagerUnits on am.ManagerUnitId equals m.Id
                                     where am.Status
                                     && am.ResponsibleId != null
                                     && am.RelatedAdministrativeUnit.Code == dadosUaConsultada.Code
                                     && i.Code == codigoOrgao
                                     && m.Code == CodigoUGE
                                     select new DadosGeraColetorBPViewModel
                                     {
                                         Sigla = a.InitialName,
                                         Chapa = a.NumberIdentification,
                                         CodigoUA = am.RelatedAdministrativeUnit.Code,
                                         IdEstadoConservacao = am.StateConservationId,
                                         NumeroDeSerie = a.SerialNumber,
                                         CPFResponsavel = am.RelatedResponsible.CPF,
                                         IdResponsavel = am.ResponsibleId ?? 0,
                                         CodigoItemMaterial = a.MaterialItemCode,
                                         IdUA = am.AdministrativeUnitId
                                     }).ToListReadUncommitted();
                }
                else {
                    massaDadosBPs = (from a in db.Assets
                                     join am in db.AssetMovements on a.Id equals am.AssetId
                                     join i in db.Institutions on am.InstitutionId equals i.Id
                                     where am.Status
                                     && i.Code == codigoOrgao
                                     select new DadosGeraColetorBPViewModel
                                     {
                                         Sigla = a.InitialName,
                                         Chapa = a.NumberIdentification,
                                         CodigoUA = am.RelatedAdministrativeUnit.Code,
                                         IdEstadoConservacao = am.StateConservationId,
                                         NumeroDeSerie = a.SerialNumber,
                                         CPFResponsavel = am.RelatedResponsible.CPF,
                                         IdResponsavel = am.ResponsibleId ?? 0,
                                         CodigoItemMaterial = a.MaterialItemCode,
                                         IdUA = am.AdministrativeUnitId
                                     }).ToListReadUncommitted();
                }

                //IDictionary<int, IList<int>> listagemDadosResponsavelBP = new Dictionary<int, IList<int>>(); //CodigoUA, ResponsibleID, ContadorResposanvelUA /* UTILZAR CAMPO/FIELD DA CLASSE */
                int idxResponsavel = 0;
                IList<int> relacaoID_Responsaveis = null;

                var relacaoResponsaveisUgeConsultada = db.Responsibles.Where(responsavelBP_Uge => responsavelBP_Uge.RelatedAdministrativeUnits.RelatedManagerUnit.Id == UgeID && responsavelBP_Uge.Status == true).ToList();

                if (relacaoResponsaveisUaConsultada.HasElements())
                {
                    listagemDadosResponsavelBP = new Dictionary<int, IList<int>>();
                    if (UaID > 0)
                    {
                        _codigoUA = dadosUaConsultada.Code;
                        foreach (var responsavelAssociadoUA in relacaoResponsaveisUaConsultada)
                        {
                            if (listagemDadosResponsavelBP.ContainsKey(_codigoUA))
                            {
                                relacaoID_Responsaveis = listagemDadosResponsavelBP[_codigoUA];
                                if (!relacaoID_Responsaveis.Contains(responsavelAssociadoUA.Id))
                                    listagemDadosResponsavelBP[_codigoUA].Add(responsavelAssociadoUA.Id);
                            }
                            else
                            {
                                listagemDadosResponsavelBP.InserirValor(_codigoUA, new List<int>());
                                listagemDadosResponsavelBP[_codigoUA].Add(responsavelAssociadoUA.Id);
                            }

                        }
                    }
                    //REUNIAO SEFAZ @03/09/2019 --> GERAR ARQUIVO COM TODOS OS BPs DE TODAS AS UAs DA UGE SELECIONADA
                    else
                    {
                        if (!string.IsNullOrEmpty(CodigoUGE) && !string.IsNullOrWhiteSpace(CodigoUGE))
                        {
                            relacaoUAs_UGESelecionada = db.AdministrativeUnits.Where(uasUGEConsultada => uasUGEConsultada.RelatedManagerUnit.Code == CodigoUGE).ToList();
                        }

                        if (relacaoUAs_UGESelecionada.HasElements()) {
                            _codigoUA_UGE = 0;
                            foreach (var ua_UGESelecionada in relacaoUAs_UGESelecionada)
                            {
                                _codigoUA_UGE = ua_UGESelecionada.Code;
                                foreach (var responsavelAssociadoUA_UGE in relacaoResponsaveisUgeConsultada)
                                {
                                    if (responsavelAssociadoUA_UGE.RelatedAdministrativeUnits.Code == _codigoUA_UGE)
                                        if (listagemDadosResponsavelBP.ContainsKey(_codigoUA_UGE))
                                        {
                                            relacaoID_Responsaveis = listagemDadosResponsavelBP[_codigoUA_UGE];
                                            if (!relacaoID_Responsaveis.Contains(responsavelAssociadoUA_UGE.Id))
                                                listagemDadosResponsavelBP[_codigoUA_UGE].Add(responsavelAssociadoUA_UGE.Id);
                                        }
                                        else
                                        {
                                            listagemDadosResponsavelBP.InserirValor(_codigoUA_UGE, new List<int>());
                                            listagemDadosResponsavelBP[_codigoUA_UGE].Add(responsavelAssociadoUA_UGE.Id);
                                        }
                                }
                            }
                        }

                    }
                }

                linhasConteudoArquivo = new List<string>();
                //listagemDadosResponsavelBP = new Dictionary<int, IList<int>>();

                //massaDadosBPs = db.Assets.Where(expWhereConsulta).ToListReadUncommitted();
                massaDadosBPs.OrderByDescending(bemPatrimonial => bemPatrimonial.IdResponsavel)
                             .ToList()
                             .ForEach(bemPatrimonial =>
                             {

                                siglaChapaBP = String.Format("{0}{1}", bemPatrimonial.Sigla, bemPatrimonial.Chapa);
                                _codigoUA = bemPatrimonial.CodigoUA;
                                estadoConservacao = bemPatrimonial.IdEstadoConservacao.ToString("D1");
                                numeroSerialBP = (!String.IsNullOrWhiteSpace(bemPatrimonial.NumeroDeSerie) ? bemPatrimonial.NumeroDeSerie.ComplementaComEspacosAte(15, true) : geraEspacoEmBranco(15));
                                if (relacaoResponsaveisUgeConsultada.HasElements()) {
                                     if (String.IsNullOrWhiteSpace(bemPatrimonial.pseudoCodigoResponsavel))
                                     {
                                         if ((listagemDadosResponsavelBP.ContainsKey(_codigoUA)))
                                         {
                                             if (listagemDadosResponsavelBP[_codigoUA] != null)
                                                 if (!(listagemDadosResponsavelBP[_codigoUA]).Contains(bemPatrimonial.IdResponsavel))
                                                     listagemDadosResponsavelBP[_codigoUA].Add(bemPatrimonial.IdResponsavel);
                                         }
                                         else
                                         {
                                             listagemDadosResponsavelBP.InserirValor(_codigoUA, new List<int>());
                                             listagemDadosResponsavelBP[_codigoUA].Add(bemPatrimonial.IdResponsavel);
                                         }
                                     }

                                    idxResponsavel = (listagemDadosResponsavelBP[_codigoUA]).IndexOf(bemPatrimonial.IdResponsavel);
                                    bemPatrimonial.pseudoIdResponsavel = idxResponsavel.ToString("D3");

                                    //REUNIAO SEFAZ @03/09/2019 --> GERAR ARQUIVO COM TODOS OS BPs DE TODAS AS UAs DA UGE SELECIONADA
                                    if (UaID <= 0)
                                    {
                                        dadosUaConsultada = db.AdministrativeUnits.Find(bemPatrimonial.IdUA);
                                    }

                                }


                             linhaParaArquivo = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}"
                                                                                       , siglaChapaBP.ComplementaComEspacosAte(16, true) //SIGLA + CHAPA + DESDOBRO -> 16 caracteres
                                                                                                                                         //, codigoUA.ToString().PadLeft(6, '0') //UA -> 6 caracteres
                                                                                       , dadosUaConsultada.Code.ToString().PadLeft(6, '0') //UA -> 6 caracteres
                                                                                       , geraEspacoEmBranco(12) /* space(8) as data_inv, space(4) as hora_inv */
                                                                                       , bemPatrimonial.CodigoItemMaterial.ToString().PadLeft(9, '0') //ItemMaterial(9)
                                                                                       , estadoConservacao /* EstadoConservacao(1) */
                                                                                       , string.IsNullOrEmpty(bemPatrimonial.pseudoCodigoResponsavel) || string.IsNullOrWhiteSpace (bemPatrimonial.pseudoCodigoResponsavel) ? bemPatrimonial.pseudoIdResponsavel: bemPatrimonial.pseudoCodigoResponsavel
                                                                                       /* 3 ultimos digitos de CPF truncado em 1 caracter; ou contador online */
                                                                                       , numeroSerialBP //NumeroSerial(15)
                                                                                       , "3" /* Dado [Status] = '3' -- fixo, via query na base do legado (SCPw) */
                                                                                       , "0" /* Dado [Situacao] = '0' -- fixo, via query na base do legado (SCPw) */
                                                                                       , "0" /*-- fixo, via query na base do legado(SCPw) */
                                                            );
                                linhasConteudoArquivo.Add(linhaParaArquivo);

                             });

                GravacaoArquivoIntegracaoColetor(linhasConteudoArquivo, nomeArquivo, true);
            }

        }
        public void geraArquivoParaColetor_Terceiros(int OrgaoID)
        {
            nomeArquivo = NomeArquivoConsultaPorColetor_Terceiro;
            string linhaParaArquivo = null;


            var dadosOrgaoConsulta = db.Institutions.Where(orgaoSAM => orgaoSAM.Id == OrgaoID).FirstOrDefault();
            if (dadosOrgaoConsulta.IsNotNull())
            {
                linhasConteudoArquivo = new List<string>();
                //var relacaoTerceiros = db.OutSourceds.Where(terceiroResponsavel => terceiroResponsavel.RelatedBudgetUnit.RelatedInstitution.Code == "20"
                var relacaoTerceiros = db.OutSourceds.Where(terceiroResponsavel => terceiroResponsavel.RelatedBudgetUnit.RelatedInstitution.Code == dadosOrgaoConsulta.Code
                                                                                && terceiroResponsavel.Status == true
                                                           //TODO VERIFICAR SE TRAGO TODOS OS RESPONSAVEIS OU SE APENAS OS QUE POSSUEM BPs (E SE FILTRO POR UO)
                                                           //&& terceiroResponsavel.Assets.Count(bemPatrimonial => bemPatrimonial.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code == "20"
                                                           //                                                   && bemPatrimonial.Status == true) == 1
                                                           )
                                                     .ToList();

                relacaoTerceiros.ForEach(terceiroResponsavel =>
                                                                {
                                                                    linhaParaArquivo = String.Format("{0}{1}"
                                                                                                             , terceiroResponsavel.Id.ToString("D2")
                                                                                                             , terceiroResponsavel.Name.PadRight(10, ' ')
                                                                                                    );
                                                                    linhasConteudoArquivo.Add(linhaParaArquivo.ComplementaComEspacosAte(22));
                                                                });


                GravacaoArquivoIntegracaoColetor(linhasConteudoArquivo, nomeArquivo);
            }
        }

        public void LimpezaArquivosIntegracaoColetor()
        {
            string[] relacaoArquivosParaExclusao = null;
            string caminhoCompletoArquivoColetor = null;


            relacaoArquivosParaExclusao = new string[] {  NomeArquivoConsultaPorColetor_BemPatrimonial
                                                        , NomeArquivoConsultaPorColetor_ItemMaterial
                                                        , NomeArquivoConsultaPorColetor_Responsavel
                                                        , NomeArquivoConsultaPorColetor_Sigla
                                                        , NomeArquivoConsultaPorColetor_Terceiro
                                                       };

            foreach (var nomeArquivosParaExclusao in relacaoArquivosParaExclusao)
            {
                caminhoCompletoArquivoColetor = String.Format(@"{0}\{1}", diretorioExportacaoArquivos, nomeArquivosParaExclusao);

                if (mFile.Exists(caminhoCompletoArquivoColetor))
                    mFile.Delete(caminhoCompletoArquivoColetor);
            }
        }
        public void GravacaoArquivoIntegracaoColetor(IList<string> linhasConteudoArquivo, string nomeArquivo, bool geraArquivoVazio = false)
        {
            //string diretorioExecucaoSistema = null;
            //string diretorioExportacaoArquivos = null;
            string caminhoCompletoArquivoColetor = null;


            if ((linhasConteudoArquivo != null && linhasConteudoArquivo.Count() > 0) || geraArquivoVazio)
            {
                //diretorioExecucaoSistema = obterDirecaoExecucaoSistema();
                //caminhoCompletoArquivoColetor = String.Format(@"{0}\{1}\{2}", diretorioExecucaoSistema, nomePastaSaida, nomeArquivo);
                //diretorioExportacaoArquivos = String.Format(@"{0}{1}", diretorioExecucaoSistema, nomePastaExportacao);
                caminhoCompletoArquivoColetor = String.Format(@"{0}\{1}", diretorioExportacaoArquivos, nomeArquivo);

                try
                {
                    //escritorArquivo = new StreamWriter(nomeArquivo);
                    using (escritorArquivo = new StreamWriter(caminhoCompletoArquivoColetor, false, Encoding.GetEncoding(1252)))
                    {
                        if (linhasConteudoArquivo.Count() > 0)
                            linhasConteudoArquivo.ToList().ForEach(_linhaArquivo => escritorArquivo.WriteLine(_linhaArquivo));
                        else
                            escritorArquivo.WriteLine("");


                        escritorArquivo.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public bool ProcessarArquivoImportadoColetor(string nomeArquivo, IList<string> linhasArquivo = null)
        {
            IList<string> listagemMsgEstimuloItemInventarioWs = null;
            bool statusProcessamento = false;
            //var linhasArquivo = LeituraArquivoExportadoColetor(nomePastaUpload, nomeArquivo);
            //linhasArquivo = linhasArquivo ?? LeituraArquivoExportadoColetor(nomePastaUpload, nomeArquivo);
            linhasArquivo = linhasArquivo ?? LeituraArquivoExportadoColetor(nomeArquivo);


            if (linhasArquivo.HasElements())
            {
                var retornoDadosBPsProcessamento = parsingArquivoLeitor(linhasArquivo);

                if (retornoDadosBPsProcessamento.HasElements())
                {
                    var msgEstimuloInventarioWs = geradorEstimuloInventarioWS(retornoDadosBPsProcessamento);
                    var inventarioID = processaChamadaWsPatrimonio(msgEstimuloInventarioWs);

                    Debugger.Log(0, "DEBUG/ANALISE", msgEstimuloInventarioWs);

                    {
                        string msgEstimuloItemInventarioWs = null;
                        listagemMsgEstimuloItemInventarioWs = new List<string>();

                        foreach (var dadosBPProcessado in retornoDadosBPsProcessamento)
                        {
                            msgEstimuloItemInventarioWs = String.Format(fmtMsgEstimuloItemInventarioWs, dadosBPProcessado[4].Value, dadosBPProcessado[5].Value, dadosBPProcessado[6].Value, dadosBPProcessado[7].Value, inventarioID, dadosBPProcessado[8].Value);
                            listagemMsgEstimuloItemInventarioWs.Add(msgEstimuloItemInventarioWs);

                            Debugger.Log(0, "DEBUG/ANALISE", msgEstimuloItemInventarioWs);
                            //processaChamadaWsPatrimonio(msgEstimuloItemInventarioWs);
                        }

                        var lotesMsgEstimuloParaProcessamento = listagemMsgEstimuloItemInventarioWs.ParticionadorLista(10);
                        string BPsConsolidados = null;
                        foreach (var loteMsgEstimuloParaProcessamento in lotesMsgEstimuloParaProcessamento)
                        {
                            BPsConsolidados = String.Join(",", loteMsgEstimuloParaProcessamento);
                            BPsConsolidados = String.Format("[{0}]", BPsConsolidados);
                            processaChamadaWsPatrimonio(BPsConsolidados);
                        }
                    }
                }

                statusProcessamento = (retornoDadosBPsProcessamento != null);
            }

            //statusProcessamento = (retornoDadosBPsProcessamento != null);
            return statusProcessamento;
        }

        [HttpPost]
        public JsonResult ObtemRelacaoUAs_ArquivoRetornoColetor()
        {
            string codigoUA = null;
            string codigoUGE = null;
            int _codigoUA = 0;
            int _codigoUGE = 0;
            int UaId = 0;
            IDictionary<int, IList<int>> relacaoDadosUGE_UA__ArquivoColetor = null;
            IList<int> relacaoUAsArquivoColetor = null;
            IList<string> _linhasConteudoArquivo = null;
            int contadorLinhasProcessadas_UGELogada = 0;
            int codigoUGESessaoLogada = 0;
            int contadorLinhasProcessadas_Arquivo = 0;
            int contadorLinhasNaoProcessadas = 0;
            int contadorLinhasProcessadas_OutrasUGEs = 0;
            IList<int> _relacaoUAsArquivoColetor = null;
            IList<AdministrativeUnit> _relacaoUAs_UGE = null;
            JsonResult retornoCamadaUI = null;
            InventariosController objController = null;
            bool uaComInventarioPendente = false;
            Tuple<bool, string, int> dadosInventariacaoUA = null;

            #region Variaveis Processamento Botao #001
            string caminhoRelativo = null;
            string arquivoUas = null;
            string caminhoNomeArquivo = null;
            #endregion Variaveis Processamento Botao #001


            try
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;
                if (this.Request != null && this.Request.Files.Count > 0 && this.Request.Files[0].ContentLength > 0)
                {
                    if (ModelState.IsValid)
                    {
                        #region Processamento Botao #001
                        {
                            caminhoRelativo = Request.Path.Replace("OperacoesComColetor", "Arquivos").Replace("ObtemRelacaoUAs_ArquivoRetornoColetor", "").Replace("//", "/");
                            arquivoUas = HttpContext.Server.MapPath(caminhoRelativo);
                            if (!Directory.Exists(arquivoUas))
                                Directory.CreateDirectory(arquivoUas);

                            //caminhoNomeArquivo = arquivoUas + "\\" + Request.Files[0].FileName;
                            caminhoNomeArquivo = arquivoUas + Request.Files[0].FileName;
                            this.Request.Files[0].SaveAs(caminhoNomeArquivo);
                        }
                        #endregion Processamento Botao #001


                        var arquivoUploaded = this.Request.Files[0];

                        var processamentoArquivoUploaded = this.fileLoader(arquivoUploaded, caminhoNomeArquivo); //Carga do arquivo como um todo in-memory
                        if (processamentoArquivoUploaded.Item1)
                            _linhasConteudoArquivo = processamentoArquivoUploaded.Item3;


                        getHierarquiaPerfil();
                        if (_managerUnitId.IsNull())
                        {
                            retornoCamadaUI = Json(new { Sucesso = false, Mensagem = "Não foi possível definir a estrutura organizacional SIAFEM, para leitura do arquivo!\nVerificar perfil utilizado nesta sessão!", Uas = string.Empty, nomeArquivoUa = string.Empty }, JsonRequestBehavior.AllowGet);
                            return retornoCamadaUI;
                        }
                        else if (_managerUnitId.IsNotNull() && _managerUnitId > 0)
                        {
                            codigoUGESessaoLogada = Int32.Parse(db.ManagerUnits.AsNoTracking()
                                                                               .Where(ugeLogada => ugeLogada.Id == this._managerUnitId)
                                                                               .FirstOrDefault().Code);
                        }

                        relacaoUAsArquivoColetor = new List<int>();
                        relacaoDadosUGE_UA__ArquivoColetor = new Dictionary<int, IList<int>>();
                        objController = new InventariosController();
                        if (processamentoArquivoUploaded.Item1 && _linhasConteudoArquivo.HasElements())
                        {
                            #region Levantamento/Filtro de UAs presentes no arquivo
                            {
                                foreach (var linhaDadosBP in _linhasConteudoArquivo)
                                {
                                    _codigoUGE = 0;
                                    _codigoUA = 0;
                                    if (linhaDadosBP.Length == 65)
                                    {
                                        codigoUA = linhaDadosBP.Substring(16, 6);
                                        relacaoUAsArquivoColetor = new List<int>();
                                        UaId = obterUaID(codigoUA);
                                        codigoUGE = obterCodigoUGEdaUA(UaId);
                                        if (Int32.TryParse(codigoUA, out _codigoUA) && Int32.TryParse(codigoUGE, out _codigoUGE))
                                        {
                                            dadosInventariacaoUA = objController.ExisteInventarioEmAndamentoParaUA(UaId);
                                            uaComInventarioPendente = dadosInventariacaoUA.Item1;
                                            if (dadosInventariacaoUA.IsNotNull() && uaComInventarioPendente)
                                                objController.DeleteConfirmed(dadosInventariacaoUA.Item3);

                                            if (relacaoDadosUGE_UA__ArquivoColetor.Keys.Contains(_codigoUGE))
                                            {
                                                relacaoUAsArquivoColetor = relacaoDadosUGE_UA__ArquivoColetor[_codigoUGE];

                                                if (!relacaoUAsArquivoColetor.Contains(_codigoUA))
                                                {
                                                    relacaoUAsArquivoColetor.Add(_codigoUA);
                                                    relacaoDadosUGE_UA__ArquivoColetor[_codigoUGE] = relacaoUAsArquivoColetor;

                                                    //++contadorLinhasProcessadas_UGELogada;
                                                }

                                                ++contadorLinhasProcessadas_UGELogada;
                                            }
                                            else if (!relacaoDadosUGE_UA__ArquivoColetor.Keys.Contains(_codigoUGE))
                                            {
                                                relacaoUAsArquivoColetor.Add(_codigoUA);
                                                relacaoDadosUGE_UA__ArquivoColetor.Add(_codigoUGE, relacaoUAsArquivoColetor);


                                                if (_codigoUGE != codigoUGESessaoLogada)
                                                    ++contadorLinhasProcessadas_OutrasUGEs;
                                                else
                                                    ++contadorLinhasProcessadas_UGELogada;
                                            }
                                        }

                                        ++contadorLinhasProcessadas_Arquivo;
                                    }
                                    else if (linhaDadosBP.Length != 65)
                                    {
                                        ++contadorLinhasNaoProcessadas;
                                        continue;
                                    }
                                }
                            }
                            #endregion Levantamento/Filtro de UAs presentes no arquivo
                        }
                    }
                }
                else
                {
                    retornoCamadaUI = Json(new { Sucesso = false, Mensagem = "Não foi selecionado/enviado arquivo para processamento!", Uas = _relacaoUAs_UGE, nomeArquivoUa = caminhoNomeArquivo }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception excErroRuntime)
            {
                throw excErroRuntime;
            }


            if (retornoCamadaUI.IsNotNull())
                return retornoCamadaUI;


            if (contadorLinhasNaoProcessadas > 0 && (contadorLinhasProcessadas_UGELogada == 0))
            {
                retornoCamadaUI = Json(new { Sucesso = false, Mensagem = "Conteúdo do arquivo não pode ser processado\nFavor verificar arquivo!", Uas = _relacaoUAs_UGE, nomeArquivoUa = caminhoNomeArquivo }, JsonRequestBehavior.AllowGet);
            }
            else if (contadorLinhasNaoProcessadas > 0 || (contadorLinhasProcessadas_UGELogada == 0))
            {
                var msgInformeErroProcessamento = String.Format("Verificada a existência de {0:##0} registro(s) inválido(s) para pós-processamento em arquivo com total de {1:##0} registro(s), dentro do arquivo selecionado por usuário!\n\nFavor verificar arquivo, pois o mesmo não pode ser processado com o conteúdo atual!", contadorLinhasNaoProcessadas, _linhasConteudoArquivo.Count());
                retornoCamadaUI = Json(new { Sucesso = false, Mensagem = msgInformeErroProcessamento, Uas = _relacaoUAs_UGE, nomeArquivoUa = caminhoNomeArquivo }, JsonRequestBehavior.AllowGet);
            }
            else if (codigoUGESessaoLogada != _codigoUGE)
            {
                var msgInformeErroProcessamento = String.Format("Verificada a existência de registro(s) inválido(s) para pós-processamento em arquivo com total de {1:##0} registro(s), sendo registros referentes a BPs de outras UGEs/UAs (UGE(s): {3}), dentro do arquivo selecionado por usuário!", contadorLinhasNaoProcessadas, contadorLinhasProcessadas_Arquivo, contadorLinhasProcessadas_OutrasUGEs, String.Join(",", relacaoDadosUGE_UA__ArquivoColetor.Keys.ToList()).RemoveUltimoCaracter());
                retornoCamadaUI = Json(new { Sucesso = false, Mensagem = msgInformeErroProcessamento, Uas = _relacaoUAs_UGE, nomeArquivoUa = caminhoNomeArquivo }, JsonRequestBehavior.AllowGet);
            }
            else if (contadorLinhasProcessadas_UGELogada > 0 && relacaoDadosUGE_UA__ArquivoColetor.Keys.Contains(codigoUGESessaoLogada))
            {
                _relacaoUAsArquivoColetor = relacaoDadosUGE_UA__ArquivoColetor[codigoUGESessaoLogada];
                _relacaoUAs_UGE = db.AdministrativeUnits
                                    .AsNoTracking()
                                    .Where(uaSIAFEM => uaSIAFEM.RelatedManagerUnit.Id == _managerUnitId
                                                    && _relacaoUAsArquivoColetor.Contains(uaSIAFEM.Code))
                                    .ToList();

                _relacaoUAs_UGE.ToList().ForEach(uaSIAFEM => uaSIAFEM.Description = String.Format("{0} - {1}", uaSIAFEM.Code, uaSIAFEM.Description));
                retornoCamadaUI = Json(new { Sucesso = true, Mensagem = "Caso exista algum inventário em andamento para alguma(s) UA(s) selecionada(s), o atual/existente será excluído do sistema, e outro será criado com base neste arquivo a ser processado!", Uas = _relacaoUAs_UGE, nomeArquivoUa = caminhoNomeArquivo }, JsonRequestBehavior.AllowGet);
            }

            return retornoCamadaUI;
        }


        [HttpPost]
        //public Tuple<int, string, string, string> ObterRelacaoUAsArquivoColetor(IList<string> linhasConteudoArquivo)
        public JsonResult ArquivosUASelecionadas(string UaConteudoArquivo, string conteudoArquivo)
        {
            var listaUasSelecionadas = UaConteudoArquivo;

            string linhaDadosBP;
            dynamic uasSelecionadas = JArray.Parse(UaConteudoArquivo);
            List<string> leituraArquivo = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(conteudoArquivo, Encoding.GetEncoding(1252)))
                {

                    while ((linhaDadosBP = sr.ReadLine()) != null)
                    {
                        //linhasConteudoArquivo.Add(linhaDadosBP);
                        var codigoUa = linhaDadosBP.Substring(17, 6);
                        foreach (var item in uasSelecionadas)
                        {
                            if (int.Parse(item.CodigoUa.ToString().Replace("{", "").Replace("}", "")) == int.Parse(codigoUa))
                            {
                                leituraArquivo.Add(linhaDadosBP);
                            }
                        }

                        //linhasConteudoArquivo.Add(codigoUa);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler arquivo em memoria", ex);
            }


            return Json("teste");

        }

        [HttpPost]
        public JsonResult ProcessamentoMultiplasUAs_ArquivoImportadoColetor(string listagemUAsSelecionados, string caminhoCompleto_ArquivoRetornoColetor)
        {
            string codigoUA = null;
            string codigoUGE = null;
            int _codigoUA = 0;
            int _codigoUGE = 0;
            int contadorLinhasNaoProcessadas = 0;
            int contadorLinhasProcessadas_Arquivo = 0;
            int contadorLinhasProcessadas_UGELogada = 0;
            int contadorLinhasProcessadas_UGELogada_UAsSelecionadas = 0;
            int contadorLinhasProcessadas_UGELogada_UAsNaoSelecionadas = 0;
            int contadorLinhasProcessadas_OutrasUGEs = 0;
            IDictionary<int, IList<int>> relacaoDadosUGE_UA__ArquivoColetor = null;
            IDictionary<int, IList<string>> relacaoDadosLinhasBPs_UA__ArquivoColetor = null;
            IList<int> relacaoUAsArquivoColetor = null;
            IList<string> relacaoLinhasBPs_UAArquivoColetor = null;
            Tuple<int, string, string, string> retornoProcessamentoArquivoColetor = null;
            string msgProcessamentoComSucesso = null;
            string msgProcessamentoSemSucesso = null;
            string listagemUAs_UGELogada = null;
            string listagemUAs_OutrasUGEs = null;
            int codigoUGESessaoLogada = 0;
            JsonMensagem msgRetorno = new JsonMensagem();


            dynamic relacaoCodigosUAs = JArray.Parse(listagemUAsSelecionados);
            var relacaoUAs_Selecionadas_UGE = ((JArray)relacaoCodigosUAs).Select(x => Int32.Parse(x["CodigoUa"].ToString())).ToList<int>();


            try
            {
                if (ModelState.IsValid)
                {
                    //HttpPostedFileBase
                    //var arquivoUploaded = this.Request.Files[0];

                    //var processamentoArquivoUploaded = this.fileLoader(arquivoUploaded); //Carga do arquivo como um todo in-memory
                    var processamentoArquivoUploaded = this.fileLoader(caminhoCompleto_ArquivoRetornoColetor); //Carga do arquivo como um todo in-memory
                    if (processamentoArquivoUploaded.Item1)
                        linhasConteudoArquivo = processamentoArquivoUploaded.Item3;


                    getHierarquiaPerfil();
                    codigoUGESessaoLogada = Int32.Parse(db.ManagerUnits.AsNoTracking()
                                                                       .Where(ugeLogada => ugeLogada.Id == this._managerUnitId)
                                                                       .FirstOrDefault().Code);

                    relacaoUAsArquivoColetor = new List<int>();
                    relacaoLinhasBPs_UAArquivoColetor = new List<string>();
                    relacaoDadosUGE_UA__ArquivoColetor = new Dictionary<int, IList<int>>();
                    relacaoDadosLinhasBPs_UA__ArquivoColetor = new Dictionary<int, IList<string>>();
                    if (processamentoArquivoUploaded.Item1 && linhasConteudoArquivo.HasElements())
                    {
                        //tipoArquivoExportado = nomeArquivoExportadoColetor.Substring(2, 2);
                        //if (tipoArquivoExportado.Length == 2)
                        #region Levantamento/Filtro de UAs presentes no arquivo
                        {
                            //if (tipoArquivoExportado == "IN")
                            {
                                foreach (var linhaDadosBP in linhasConteudoArquivo)
                                {
                                    if (linhaDadosBP.Length == 65)
                                    {
                                        codigoUA = linhaDadosBP.Substring(16, 6);
                                        if (Int32.TryParse(codigoUA, out _codigoUA) && relacaoUAs_Selecionadas_UGE.Contains(_codigoUA)) //Se a UA da 'linha lida' estah na lista de UAs para processamento
                                        {
                                            relacaoUAsArquivoColetor = new List<int>();
                                            codigoUGE = obterCodigoUGEdaUA(obterUaID(codigoUA)); //Obter 'UGE da UA'
                                            if (Int32.TryParse(codigoUGE, out _codigoUGE) && _codigoUGE == codigoUGESessaoLogada) //Se 'UGE da UA' igual a da sessao atual
                                            {
                                                if (relacaoDadosUGE_UA__ArquivoColetor.Keys.Contains(_codigoUGE)) //Se a listagem UGEs/UAs contem a 'UGE da UA'
                                                {
                                                    relacaoUAsArquivoColetor = relacaoDadosUGE_UA__ArquivoColetor[_codigoUGE]; //Pega a relacao de UAs da 'UGE da UA'
                                                    if (!relacaoUAsArquivoColetor.Contains(_codigoUA)) //Se relacao de UAs do Coletor nao contem a UA da linha atual
                                                    {
                                                        relacaoLinhasBPs_UAArquivoColetor = new List<string>(); //Cria uma lista para armazenar a linha atual


                                                        relacaoUAsArquivoColetor.Add(_codigoUA); //Adiciona a 'UA da linha atual' na lista de UAs da 'UGE da UA'
                                                        relacaoLinhasBPs_UAArquivoColetor.Add(linhaDadosBP); //Adiciona a linha atual na lista de linhas lidas da 'UA da linha lida'

                                                        relacaoDadosUGE_UA__ArquivoColetor[_codigoUGE] = relacaoUAsArquivoColetor; //Guarda a 'lista de UAs lidas' da 'UGE da UA' atual
                                                        relacaoDadosLinhasBPs_UA__ArquivoColetor[_codigoUA] = relacaoLinhasBPs_UAArquivoColetor; //Guarda a 'lista de lihas lidas da UA atual' na listagem 'Linhas Lidas/UAs'

                                                        ++contadorLinhasProcessadas_UGELogada; //incrementa contador
                                                    }
                                                    else if (relacaoUAsArquivoColetor.Contains(_codigoUA)) //Se a listagem de UAs lidas do arquivo jah contem a 'UA da linha lida'
                                                    {
                                                        relacaoLinhasBPs_UAArquivoColetor = relacaoDadosLinhasBPs_UA__ArquivoColetor[_codigoUA]; //Retira a 'lista de lihas lidas da UA atual' do ''cache'' 
                                                        relacaoLinhasBPs_UAArquivoColetor.Add(linhaDadosBP); //...e adiciona a linha atual


                                                        relacaoDadosLinhasBPs_UA__ArquivoColetor[_codigoUA] = relacaoLinhasBPs_UAArquivoColetor; //Atualiza a 'lista de lihas lidas da UA atual' no ''cache'' 
                                                    }
                                                }
                                                else if (!relacaoDadosUGE_UA__ArquivoColetor.Keys.Contains(_codigoUGE)) //Se a listagem UGEs/UAs NAO contem a 'UGE da UA'
                                                {
                                                    relacaoUAsArquivoColetor.Add(_codigoUA);
                                                    relacaoLinhasBPs_UAArquivoColetor.Add(linhaDadosBP);


                                                    relacaoDadosUGE_UA__ArquivoColetor.Add(_codigoUGE, relacaoUAsArquivoColetor);
                                                    relacaoDadosLinhasBPs_UA__ArquivoColetor.Add(_codigoUA, relacaoLinhasBPs_UAArquivoColetor);
                                                }

                                                ++contadorLinhasProcessadas_UGELogada; //incrementa contador
                                            }

                                            ++contadorLinhasProcessadas_Arquivo; //incrementa contador
                                            ++contadorLinhasProcessadas_UGELogada_UAsSelecionadas; //incrementa contador

                                            _codigoUA = 0;
                                            _codigoUGE = 0;
                                        }
                                        else if (Int32.TryParse(codigoUA, out _codigoUA) && !relacaoUAs_Selecionadas_UGE.Contains(_codigoUA)) //Se UA pertence a 'UGE da UA' mas nao selecionada no combo da tela para processamento
                                        {
                                            ++contadorLinhasProcessadas_UGELogada_UAsNaoSelecionadas; //incrementa contador
                                        }

                                    }
                                    else if (linhaDadosBP.Length != 65)
                                    {
                                        ++contadorLinhasNaoProcessadas;
                                        continue;
                                    }
                                }
                            }
                        }
                        #endregion Levantamento/Filtro de UAs presentes no arquivo

                        if (relacaoDadosUGE_UA__ArquivoColetor.ContainsKey(codigoUGESessaoLogada))
                        {
                            var listagemCodigosUAs = relacaoDadosUGE_UA__ArquivoColetor[codigoUGESessaoLogada];
                            //ViewBag.ListagemUAs_ArquivoRetornoColetor = hierarquia.GetUasPorRelacaoCodigos(listagemCodigosUAs);


                            IList<string> listagemMsgEstimuloItemInventarioWs = new List<string>();
                            foreach (var codigoUA_EmProcessamento in relacaoDadosLinhasBPs_UA__ArquivoColetor.Keys) //FILTRAR DE ACORDO COM O SELECIONADO EM TELA VIA COMBO
                            {
                                var dadosBPs_UA_EmProcessamento = relacaoDadosLinhasBPs_UA__ArquivoColetor[codigoUA_EmProcessamento];
                                var retornoDadosBPsProcessamento = parsingArquivoLeitor(dadosBPs_UA_EmProcessamento);
                                if (retornoDadosBPsProcessamento.HasElements())
                                {
                                    var msgEstimuloInventarioWs = geradorEstimuloInventarioWS(retornoDadosBPsProcessamento);
                                    var inventarioID = processaChamadaWsPatrimonio(msgEstimuloInventarioWs);

                                    Debugger.Log(0, "DEBUG/ANALISE", msgEstimuloInventarioWs);
                                    {
                                        string msgEstimuloItemInventarioWs = null;
                                        listagemMsgEstimuloItemInventarioWs = new List<string>();

                                        foreach (var dadosBPProcessado in retornoDadosBPsProcessamento)
                                        {
                                            msgEstimuloItemInventarioWs = String.Format(fmtMsgEstimuloItemInventarioWs, dadosBPProcessado[4].Value, dadosBPProcessado[5].Value, dadosBPProcessado[6].Value, dadosBPProcessado[7].Value, inventarioID, dadosBPProcessado[8].Value);
                                            listagemMsgEstimuloItemInventarioWs.Add(msgEstimuloItemInventarioWs);

                                            Debugger.Log(0, "DEBUG/ANALISE", msgEstimuloItemInventarioWs);
                                            //processaChamadaWsPatrimonio(msgEstimuloItemInventarioWs);
                                        }

                                        var lotesMsgEstimuloParaProcessamento = listagemMsgEstimuloItemInventarioWs.ParticionadorLista(10);
                                        string BPsConsolidados = null;
                                        foreach (var loteMsgEstimuloParaProcessamento in lotesMsgEstimuloParaProcessamento)
                                        {
                                            BPsConsolidados = String.Join(",", loteMsgEstimuloParaProcessamento);
                                            BPsConsolidados = String.Format("[{0}]", BPsConsolidados);
                                            processaChamadaWsPatrimonio(BPsConsolidados);
                                        }
                                    }
                                }
                            }

                            listagemUAs_UGELogada = String.Join(", ", (relacaoDadosUGE_UA__ArquivoColetor[codigoUGESessaoLogada]));
                            if (listagemUAs_UGELogada[listagemUAs_UGELogada.Length-1] == ',')
                                listagemUAs_UGELogada = String.Join(",", listagemUAs_UGELogada).RemoveUltimoCaracter();

                            msgProcessamentoComSucesso = String.Format("Foram processados {0:##0} registros com sucesso!\nUA(s): {1}.", contadorLinhasProcessadas_UGELogada, listagemUAs_UGELogada);
                        }

                        if ((relacaoDadosUGE_UA__ArquivoColetor.ContainsKey(codigoUGESessaoLogada) && relacaoDadosUGE_UA__ArquivoColetor.Keys.Count() > 1) ||
                             (!relacaoDadosUGE_UA__ArquivoColetor.ContainsKey(codigoUGESessaoLogada) && relacaoDadosUGE_UA__ArquivoColetor.Keys.Count() >= 1))
                        {
                            listagemUAs_OutrasUGEs = String.Join("; ", relacaoDadosUGE_UA__ArquivoColetor.ToList().Select(tupla => ("UGE " + tupla.Key.ToString() + "; (UA(s): " + String.Join(", ", tupla.Value).RemoveUltimoCaracter() + ")")));
                            msgProcessamentoSemSucesso = String.Format("Verificada a existência de {0:##0} registro(s) inválido(s) para pós-processamento em arquivo com total de {1:##0} registro(s), sendo {2:##0} referentes a BPs de outras UGEs/UAs, dentro do arquivo processado!", contadorLinhasNaoProcessadas, contadorLinhasProcessadas_Arquivo, contadorLinhasProcessadas_OutrasUGEs, String.Join(",", listagemUAs_OutrasUGEs).RemoveUltimoCaracter());

                            relacaoDadosUGE_UA__ArquivoColetor.Remove(_codigoUGE);
                            ViewBag.ListagemUAs_ArquivoRetornoColetor = hierarquia.GetUasPorRelacaoCodigos(null);
                        }
                        else if (!relacaoDadosUGE_UA__ArquivoColetor.HasElements() && contadorLinhasNaoProcessadas > 0)
                        {
                            listagemUAs_UGELogada = null;
                            listagemUAs_OutrasUGEs = null;
                            msgProcessamentoComSucesso = null;
                            msgProcessamentoSemSucesso = String.Format("Verificada a existência de {0:##0} registro(s) inválido(s) para processamento, dentro do arquivo processado!", contadorLinhasNaoProcessadas);


                            ViewBag.ListagemUAs_ArquivoRetornoColetor = hierarquia.GetUasPorRelacaoCodigos(null);
                        }
                    }
                    else if (!linhasConteudoArquivo.HasElements())
                    {
                        listagemUAs_UGELogada = null;
                        listagemUAs_OutrasUGEs = null;
                        msgProcessamentoComSucesso = null;
                        msgProcessamentoSemSucesso = "Arquivo Vazio!\nNão há registro(s) para processamento!";


                        ViewBag.ListagemUAs_ArquivoRetornoColetor = hierarquia.GetUasPorRelacaoCodigos(null);
                    }

                    retornoProcessamentoArquivoColetor = Tuple.Create<int, string, string, string>(linhasConteudoArquivo.Count(), listagemUAs_UGELogada, msgProcessamentoComSucesso, msgProcessamentoSemSucesso);
                    //return retornoProcessamentoArquivoColetor;
                    {
                        if ((linhasConteudoArquivo.HasElements()) && (!String.IsNullOrWhiteSpace(msgProcessamentoComSucesso) && String.IsNullOrWhiteSpace(msgProcessamentoSemSucesso)))
                        {
                            msgRetorno.Chave = "Sucesso";
                            msgRetorno.Conteudo = msgProcessamentoComSucesso;
                        }
                        else if ((String.IsNullOrWhiteSpace(msgProcessamentoComSucesso) && !String.IsNullOrWhiteSpace(msgProcessamentoSemSucesso)) || (!linhasConteudoArquivo.HasElements()))
                        {
                            msgRetorno.Chave = "Erro";
                            msgRetorno.Conteudo = msgProcessamentoSemSucesso;
                        }
                    }

                }
            }
            catch (Exception excErroExecucao)
            {
                msgRetorno.Chave = "Erro";
                msgRetorno.Conteudo = "Falha ao executar função 'ProcessamentoMultiplasUAs_ArquivoImportadoColetor(...)': " + excErroExecucao.Message;
            }

            return Json(msgRetorno, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RetornarListaUas()
        {
            IList<AdministrativeUnit> listagemUAs_UGE = null;
            IDictionary<string, IList<int>> uasExistentesArquivoRetornadoPorColetor = null;


            if (uasExistentesArquivoRetornadoPorColetor.IsNotNull() &&
                uasExistentesArquivoRetornadoPorColetor.Values.HasElements())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;

                string codigoUGE = uasExistentesArquivoRetornadoPorColetor.Keys.FirstOrDefault();
                if (!String.IsNullOrWhiteSpace(codigoUGE))
                {
                    listagemUAs_UGE = db.AdministrativeUnits.Where(uASIAFEM => (uASIAFEM.RelatedManagerUnit.Code == codigoUGE && uASIAFEM.Status == true)
                                                                            && (uasExistentesArquivoRetornadoPorColetor[codigoUGE].Contains(uASIAFEM.Code)))
                                                            .ToList();

                    listagemUAs_UGE.ToList().ForEach(uaSIAFEM => uaSIAFEM.Description = String.Format("{0} - {1}", uaSIAFEM.Code.ToString("D6"), uaSIAFEM.Description));
                    ViewBag.ListagemUAs_ArquivoRetornoColetor = listagemUAs_UGE;
                }
            }

            return Json(listagemUAs_UGE, JsonRequestBehavior.AllowGet);
        }


        private IList<AdministrativeUnit> retornaListaUas_UGE(int _managerUnitId)
        {
            IList<AdministrativeUnit> listagemUAs_UGE = null;
            IList<AdministrativeUnit> _listagemUAs_UGE = null;
            IQueryable<AssetMovements> qryMovimentacoesBPs = null;
            IQueryable<Responsible> qryResponsaveisUAs = null;
            int contadorMovimentacoesBP = 0;
            int contadorResponsaveisAssociadosUA = 0;



            if (_managerUnitId > 0)
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;

                string codigoUGE = db.ManagerUnits.Where(ugeSIAFEM => ugeSIAFEM.Id == _managerUnitId).Select(ugeSIAFEM => ugeSIAFEM.Code).FirstOrDefault();
                if (!String.IsNullOrWhiteSpace(codigoUGE))
                {
                    listagemUAs_UGE = _listagemUAs_UGE = db.AdministrativeUnits.AsNoTracking()
                                                                               .Where(uASIAFEM => (uASIAFEM.RelatedManagerUnit.Code == codigoUGE && uASIAFEM.Status == true))
                                                                               .ToList();


                    foreach (var uaVinculada in _listagemUAs_UGE)
                    {
                        qryMovimentacoesBPs = db.AssetMovements.Where(movimentacaoBP => (movimentacaoBP.AdministrativeUnitId.HasValue && movimentacaoBP.AdministrativeUnitId.Value == uaVinculada.Id)
                                                                                     && movimentacaoBP.Status == true);

                        qryResponsaveisUAs = db.Responsibles.Where(responsavelAssociadoUA => responsavelAssociadoUA.AdministrativeUnitId == uaVinculada.Id
                                                                                          && responsavelAssociadoUA.Status == true);

                        contadorMovimentacoesBP = qryMovimentacoesBPs.Count();
                        contadorResponsaveisAssociadosUA = qryMovimentacoesBPs.Count();


                        if (contadorMovimentacoesBP == 0 && contadorResponsaveisAssociadosUA == 0)
                            listagemUAs_UGE.Remove(uaVinculada);
                    }


                    listagemUAs_UGE.ToList().ForEach(uaSIAFEM => uaSIAFEM.Description = String.Format("{0} - {1}", uaSIAFEM.Code.ToString("D6"), uaSIAFEM.Description));
                    ViewBag.ListagemUAs_ArquivoRetornoColetor = listagemUAs_UGE;
                }
            }

            return listagemUAs_UGE;
            //return Json(listagemUAs_UGE, JsonRequestBehavior.AllowGet);
        }

  //      [HttpPost]
		//public JsonResult ArquivosUASelecionadas(string UaConteudoArquivo)
  //      {
		//	var listaUasSelecionadas = UaConteudoArquivo;
		//	return Json("teste");
		//}

        private string processaChamadaWsPatrimonio(string msgEstimuloWS)
        {
            HttpClient httpClient = null;
            Task<string> retornoWsSamPatrimonio = null;
            string retornoWS = null;
            string baseAdress = null;
            string urlConsulta = null;
            string caminhoUrlWS = null;


            caminhoUrlWS = (msgEstimuloWS.Contains("\"InventarioId\"") ? "/Mobiles/FinalizaInventarioItem?estimulo=" : "/Mobiles/FinalizaInventario?estimulo=");
            baseAdress = String.Format(@"{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, HttpContext.Request.ApplicationPath);


            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseAdress);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            urlConsulta = String.Format("{0}{1}{2}", baseAdress, caminhoUrlWS, msgEstimuloWS);

            try
            {
                retornoWsSamPatrimonio = httpClient.GetStringAsync(urlConsulta);
                retornoWS = retornoWsSamPatrimonio.Result;
            }
            catch (AggregateException aggregateException)
            {
                var testeEXC = GetFirstRealException(aggregateException);
                throw new Exception("Erro ao importar inventário de coletor COMPEX");
            }


            return retornoWS;
        }

        private static Exception GetFirstRealException(Exception exception)
        {
            Exception realException = exception;
            var aggregateException = realException as AggregateException;

            if (aggregateException != null)
            {
                realException = aggregateException.Flatten().InnerException; // take first real exception

                while (realException != null && realException.InnerException != null)
                {
                    realException = realException.InnerException;
                }
            }

            return realException ?? exception;
        }

        private string geradorEstimuloInventarioWS(IList<IList<KeyValuePair<string, string>>> retornoDadosBPsProcessamento)
        {
            string msgEstimuloParaInventario = null;
            string descricaoDispositivoInventariante = null;


            int tipoDispositivoInventarianteID = (ViewBag.TipoDispositivoInventarianteID ?? 0);
            //descricaoDispositivoInventariante = (tipoDispositivoInventarianteID == EnumTipoDispositivoInventariante.COMPEX_CPX8000.GetHashCode() ? "Coletor Dados COMPEX CPX-8000" : null);
            descricaoDispositivoInventariante = "COMPEX CPX-8000";

            if (retornoDadosBPsProcessamento.HasElements() && retornoDadosBPsProcessamento[0].HasElements())
                msgEstimuloParaInventario = String.Format(fmtMsgEstimuloInventarioWs, retornoDadosBPsProcessamento[0][0].Value, retornoDadosBPsProcessamento[0][1].Value, retornoDadosBPsProcessamento[0][2].Value, retornoDadosBPsProcessamento[0][3].Value, descricaoDispositivoInventariante, EnumTipoInventario.ColetorDados.GetHashCode().ToString());


            return msgEstimuloParaInventario;
        }

        //Falta Gerar Teste Unitario
        public IList<KeyValuePair<string, string>> parsingNomeArquivoColetor(string nomeArquivoExportadoColetor)
        {
            IList<KeyValuePair<string, string>> dadosOperacao = null;
            string codigoColetorDados = null;
            string tipoOperacao = null;
            string dataOperacao = null;
            string horaOperacao = null;

            if (!String.IsNullOrWhiteSpace(nomeArquivoExportadoColetor) && (nomeArquivoExportadoColetor.Split(new char[] { '.' })[0].Length == ComprimentoNomeArquivoColetor_COMPEX))
            {
                codigoColetorDados = nomeArquivoExportadoColetor.Substring(0, 2);
                tipoOperacao = nomeArquivoExportadoColetor.Substring(2, 2);
                dataOperacao = nomeArquivoExportadoColetor.Substring(4, 8);
                horaOperacao = nomeArquivoExportadoColetor.Substring(12, 4);



                dadosOperacao = new List<KeyValuePair<string, string>>();
                dadosOperacao.Add(new KeyValuePair<string, string>("CodigoColetor", codigoColetorDados));
                dadosOperacao.Add(new KeyValuePair<string, string>("TipoOperacao", tipoOperacao));
                dadosOperacao.Add(new KeyValuePair<string, string>("DataOperacao", dataOperacao));
                dadosOperacao.Add(new KeyValuePair<string, string>("HoraOperacao", horaOperacao));
            }

            return dadosOperacao;
        }

        public int obterResponsavelID(string codigoUA = null, string pseudoCodigoResponsavel = null, string nomeResponsavel = null)
        {
            int responsavelID = -1;
            int _codigoUA = -1;
            int _pseudoCodigoResponsavel = -1;
            Expression<Func<Responsible, bool>> expWhere = null;

            if ((Int32.TryParse(codigoUA, out _codigoUA) && _codigoUA > 0) &&
                (Int32.TryParse(pseudoCodigoResponsavel, out _pseudoCodigoResponsavel) && _pseudoCodigoResponsavel >= 0))
            {
                if (listagemDadosResponsavelBP.HasElements() &&
                    listagemDadosResponsavelBP.ContainsKey(_codigoUA) &&
                    listagemDadosResponsavelBP[_codigoUA].Count() - 1 >= _pseudoCodigoResponsavel
                   )
                {
                    responsavelID = listagemDadosResponsavelBP[_codigoUA][_pseudoCodigoResponsavel];
                }
                else
                {
                    expWhere = (responsavelUA => responsavelUA.RelatedAdministrativeUnits.Code == _codigoUA);
                }
            }
            else if (!String.IsNullOrWhiteSpace(nomeResponsavel))
            {
                expWhere = (registroResponsavel => registroResponsavel.Name.ToUpperInvariant() == nomeResponsavel.ToUpperInvariant());
            }

            //if ((_pseudoCodigoResponsavel < 0 && _codigoUA > 0) && expWhere.IsNotNull())
            if ((_pseudoCodigoResponsavel <= 0 && _codigoUA > 0) && expWhere.IsNotNull())
                responsavelID = db.Responsibles.Where(expWhere).Select(registroResponsavel => registroResponsavel.Id).FirstOrDefault();


            return responsavelID;
        }
        public int obterUgeID_Responsavel(int responsavelID)
        {
            int ugeID = 0;
            ugeID = db.Responsibles.AsNoTracking()
                                   .Where(responsavelConsultado => responsavelConsultado.Id == responsavelID)
                                   .Select(responsavelConsultado => responsavelConsultado.RelatedAdministrativeUnits.RelatedManagerUnit.Id)
                                   .FirstOrDefault();

            return ugeID;
        }
        public string obterCodigoUGEdaUA(int uaID)
        {
            string ugeCodigo = null;
            ugeCodigo = db.AdministrativeUnits.AsNoTracking()
                                              .Where(uaSIAFEM => uaSIAFEM.Id == uaID)
                                              .Select(uaSIAFEM => uaSIAFEM.RelatedManagerUnit.Code)
                                              .FirstOrDefault();

            return ugeCodigo;
        }
        public int obterOrgaoIDdaUA(int uaID)
        {
            string codigoOrgao = db.AdministrativeUnits
                                   .Where(uaSIAFEM => uaSIAFEM.Id == uaID)
                                   .Select(uaSIAFEM => uaSIAFEM.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code)
                                   .FirstOrDefault();

            return string.IsNullOrEmpty(codigoOrgao) ? -1: Int32.Parse(codigoOrgao);
        }
        public int obterUaID(string codigoUA)
        {
            int uaID = -1;
            int _codigoUA = -1;


            if (Int32.TryParse(codigoUA, out _codigoUA))
            {
                uaID = db.AdministrativeUnits.AsNoTracking()
                                             .Where(uaSIAFEM => uaSIAFEM.Code == _codigoUA)
                                             .Select(uaSIAFEM => uaSIAFEM.Id)
                                             .FirstOrDefault();
            }

            return uaID;
        }

        public IList<Tuple<string, string, string>> geraRelacaoBPsPendentesOrgao(int institutionId)
        {
            IList<Tuple<string, string, string>> listaBPsPendentesEncontrados = null;

            var teste = (from a in db.Assets
                         join am in db.AssetMovements
                         on a.Id equals am.AssetId
                         where am.Status == true &&
                             a.flagVerificado.HasValue &&
                             a.flagDepreciaAcumulada != 1 &&
                             am.InstitutionId == institutionId &&
                             //am.BudgetUnitId == (_budgetUnitId != 0 ? _budgetUnitId : am.BudgetUnitId) &&
                             //am.ManagerUnitId == (_managerUnitId != 0 ? _managerUnitId : am.ManagerUnitId) &&
                             a.NumberIdentification != "99999999999"
                         select new { Sigla = (a.RelatedInitial == null ? string.Empty : a.RelatedInitial.Name),
                                      Chapa = a.NumberIdentification,
                                      UA = (am.RelatedAdministrativeUnit == null ? null : am.RelatedAdministrativeUnit.Code.ToString())
                         }).AsNoTracking().AsEnumerable();

            listaBPsPendentesEncontrados = (from t in teste select Tuple.Create(t.Sigla, t.Chapa, t.UA)).ToList();


            return listaBPsPendentesEncontrados;
        }

        private void CarregaResponsaveisDaUAComCodigo(string codigoUA, int IdUA)
        {
            int _codigoUA = -1;

            if (Int32.TryParse(codigoUA, out _codigoUA))
            {
                if (listagemDadosResponsavelBP == null)
                    listagemDadosResponsavelBP = new Dictionary<int, IList<int>>();

                if (!listagemDadosResponsavelBP.ContainsKey(_codigoUA))
                {
                    var listaResponsaveis = (from r in db.Responsibles where r.AdministrativeUnitId == IdUA select r.Id).OrderBy(r => r).ToList();
                    listagemDadosResponsavelBP.InserirValor(_codigoUA, listaResponsaveis);
                }
            }
            
        }

        //Falta Gerar Teste Unitario
        public IList<IList<KeyValuePair<string, string>>> parsingArquivoLeitor(IList<string> linhasConteudoArquivo)
        {
            IList<IList<KeyValuePair<string, string>>> relacaoDadosBPsParaProcessamento = null;
            IList<KeyValuePair<string, string>> chavesBP = null;
            //Inventario objInventario = null;
            IList<Inventario> relacaoBPsInventariados = null;

            string siglaBP = null;
            string chapaBP = null;
            string codigoUA = null;
            string codigoUGE = null;
            string estadoBP = null;
            string numeroSerie = null;
            string codigoItemMaterial = null;
            //string notaFiscal = null;
            string statusBP = null;
            string situacaoBP = null;
            string codigoResponsavelBP = null;
            string siglaChapaBP = null;


            int uaID = 0;
            int ugeID = 0;
            int responsavelID = 0;
            int? assetID = null;
            string cpfUsuarioInventariante = null;
            bool bpComPendenciasIncorporacao = false;

            //string dataRealizacaoInventario = null;
            //string horaRealizacaoInventario = null;
            //string tipoArquivoExportado = null;
            //IList<string> linhasConteudoArquivo = null;

            getHierarquiaPerfil();


            chavesBP = new List<KeyValuePair<string, string>>();
            cpfUsuarioInventariante = _login;
            //string nomeArquivoExportadoColetor = null;
            //if (!String.IsNullOrWhiteSpace(conteudoArquivoExportadoColetor))
            //if (linhasConteudoArquivo != null && linhasConteudoArquivo.Count() > 0)
            //string dadosCodigoUA = " 023637 ";
            if (linhasConteudoArquivo.HasElements())
            {
                //tipoArquivoExportado = nomeArquivoExportadoColetor.Substring(2, 2);
                //if (tipoArquivoExportado.Length == 2)
                {
                    //if (tipoArquivoExportado == "IN")
                    {
                        relacaoBPsInventariados = new List<Inventario>();
                        relacaoDadosBPsParaProcessamento = new List<IList<KeyValuePair<string, string>>>();
                        var relacaoBPsPendentesOrgao = geraRelacaoBPsPendentesOrgao(_institutionId);
                        //linhasConteudoArquivo = linhasConteudoArquivo.ToList().Where(linha => linha.Contains(dadosCodigoUA)).ToList();
                        foreach (var linhaDadosBP in linhasConteudoArquivo)
                        {
                            if (linhaDadosBP.Length == 65)
                            {
                                //SIGLA(2) + CHAPA(6) + DESDOBRO(8) + UA(6)
                                siglaChapaBP = linhaDadosBP.Substring(0, 10).Trim();
                                var parSiglaChapaBP = siglaChapaBP.QuebraLinhaSiglaChapa().ToArray();
                                if (parSiglaChapaBP.Count() == 2)
                                {
                                    siglaBP = linhaDadosBP.Substring(0, 2);
                                    chapaBP = linhaDadosBP.Substring(2, 6);
                                }
                                else
                                {
                                    siglaBP = null;
                                    chapaBP = linhaDadosBP.Substring(0, 6);
                                }
                                //siglaBP = ((parSiglaChapaBP.Count() == 2) ? linhaDadosBP.Substring(0, 2) : null);

                                /* 8 espacos */
                                codigoUA = linhaDadosBP.Substring(16, 6);
                                /* 12 espacos */
                                //ITEM_MATERIAL(9) + CODIGO RESPONSAVEL(3) + NUMERO_SERIE(15)
                                codigoItemMaterial = linhaDadosBP.Substring(34, 9);
                                //estadoConservacao = linhaDadosBP.Substring(43, 1);
                                estadoBP = linhaDadosBP.Substring(43, 1);
                                codigoResponsavelBP = linhaDadosBP.Substring(44, 3);
                                numeroSerie = linhaDadosBP.Substring(47, 15);
                                /* 4 espacos */
                                //statusBP = linhaDadosBP.Substring(63, 1);
                                //situacaoBP = linhaDadosBP.Substring(64, 1);
                                statusBP = linhaDadosBP.Substring(62, 1);
                                situacaoBP = linhaDadosBP.Substring(63, 1);



                                /* PROCESSAMENTO INVENTARIO */
                                uaID = obterUaID(codigoUA);

                                /* CARREGA LISTA DE RESPONSAVEIS*/
                                CarregaResponsaveisDaUAComCodigo(codigoUA, uaID);

                                //TODO VERIFICAR ESTA LINHA, SE CODIGO RESPONSAVEL > 0 E NAO HOUVER ACESSOCIADO NO SISTEMA
                                responsavelID = obterResponsavelID(codigoUA, codigoResponsavelBP);
                                ugeID = obterUgeID_Responsavel(responsavelID);
                                codigoUGE = obterCodigoUGEdaUA(uaID);


                                /* RETORNO DADOS INVENTARIO PARA SISTEMA SAM PATRIMONIO */
                                /* DADOS TABELA INVENTARIO */
                                chavesBP.Add(new KeyValuePair<string, string>("ResponsavelId", responsavelID.ToString()));
                                chavesBP.Add(new KeyValuePair<string, string>("UaId", uaID.ToString()));
                                chavesBP.Add(new KeyValuePair<string, string>("UgeId", ugeID.ToString()));
                                chavesBP.Add(new KeyValuePair<string, string>("Usuario", cpfUsuarioInventariante));

                                /* DADOS TABELA ITEM INVENTARIO */
                                chavesBP.Add(new KeyValuePair<string, string>("Code", chapaBP));
                                //TODO [Douglas Batista] Verificar se aqui qual campo (se 'status' ou 'situacao') que determina se o BP foi encontrado ou não
                                //Campo 'status'
                                //chavesBP.Add(new KeyValuePair<string, string>("Estado", estadoConservacao));
                                //var institutionId = obterOrgaoIDdaUA(uaID);
                                string dePara__SAM_COMPEX_CPX_8000 = null;
                                var bpParaPesquisaNaBase = obterDadosBemPatrimonial(siglaBP, chapaBP, codigoUA);
                                if (relacaoBPsPendentesOrgao.HasElements())
                                    bpComPendenciasIncorporacao = relacaoBPsPendentesOrgao.Contains(Tuple.Create(siglaBP, chapaBP, codigoUA));

                                //TODO 08/11/2018 VOLTAR PRO QUE ESTAVA ANTES, SE NAO ENCONTRADO FISICAMENTE, AIH PESQUISAR A BASE E DEVOLVER ONDE ESTAH NO ORGAO
                                //BP EXISTE NA BASE E ESTA ATIVO
                                //if (bpParaPesquisaNaBase.IsNotNull() && bpParaPesquisaNaBase.AssetMovements.LastOrDefault().IsNotNull())
                                //{
                                //    //LOCALIZACAO FISICA E CONTABIL SAM/COLETOR OK
                                //    if ((bpParaPesquisaNaBase.AssetMovements.LastOrDefault().RelatedAdministrativeUnit.Code == _codigoUA &&
                                //        bpParaPesquisaNaBase.AssetMovements.LastOrDefault().RelatedResponsible.Id == responsavelID) &&
                                //        statusBP == StatusBP_ColetorCOMPEX_CPX8000.OK.GetHashCode().ToString())
                                //        dePara__SAM_COMPEX_CPX_8000 = SituacaoFisicaBP.OK.GetHashCode().ToString(); //"1"

                                //    //MESMA UA (SAM/COLETOR) MAS RESPONSAVEL DIFERENTE
                                //    else if (bpParaPesquisaNaBase.AssetMovements.LastOrDefault().RelatedAdministrativeUnit.Code == _codigoUA &&
                                //             bpParaPesquisaNaBase.AssetMovements.LastOrDefault().RelatedResponsible.Id != responsavelID)
                                //        dePara__SAM_COMPEX_CPX_8000 = SituacaoFisicaBP.OutroResponsavel.GetHashCode().ToString();

                                //    //BP EXISTE NA BASE, UA E RESPONSAVEL DIFERENTES
                                //    else if (bpParaPesquisaNaBase.AssetMovements.LastOrDefault().RelatedAdministrativeUnit.Code != _codigoUA &&
                                //             bpParaPesquisaNaBase.AssetMovements.LastOrDefault().RelatedResponsible.Id != responsavelID)
                                //        dePara__SAM_COMPEX_CPX_8000 = SituacaoFisicaBP.OutraUA.GetHashCode().ToString();
                                //}
                                ////SE EH BP NOVO
                                ////else if (statusBP == "3")
                                //else if (bpParaPesquisaNaBase.IsNull() || statusBP == StatusBP_ColetorCOMPEX_CPX8000.NaoEncontrado.GetHashCode().ToString())
                                //    dePara__SAM_COMPEX_CPX_8000 = SituacaoFisicaBP.NaoEncontrado.GetHashCode().ToString(); //"0"

                                int _codigoUA = -1;
                                if (bpParaPesquisaNaBase.IsNotNull() && Int32.TryParse(codigoUA, out _codigoUA))
                                {
                                    if (statusBP == StatusBP_ColetorCOMPEX_CPX8000.NaoEncontrado.GetHashCode().ToString())
                                        dePara__SAM_COMPEX_CPX_8000 = EnumSituacaoFisicaBP.NaoEncontrado.GetHashCode().ToString(); //"0"
                                    else if (bpComPendenciasIncorporacao && statusBP == StatusBP_ColetorCOMPEX_CPX8000.OK.GetHashCode().ToString())
                                        dePara__SAM_COMPEX_CPX_8000 = EnumSituacaoFisicaBP.OK_ComPendencias.GetHashCode().ToString(); //"1"
                                    else if (statusBP == StatusBP_ColetorCOMPEX_CPX8000.OK.GetHashCode().ToString())
                                        dePara__SAM_COMPEX_CPX_8000 = EnumSituacaoFisicaBP.OK.GetHashCode().ToString(); //"1"
                                    //MESMA UA (SAM/COLETOR) MAS RESPONSAVEL DIFERENTE
                                    else if ((statusBP == StatusBP_ColetorCOMPEX_CPX8000.TransferenciaResponsavel.GetHashCode().ToString()) || //PARA TESTES
                                            (bpParaPesquisaNaBase.AssetMovements.LastOrDefault().RelatedAdministrativeUnit.Code == _codigoUA &&
                                             bpParaPesquisaNaBase.AssetMovements.LastOrDefault().RelatedResponsible.Id != responsavelID))
                                        dePara__SAM_COMPEX_CPX_8000 = EnumSituacaoFisicaBP.OutroResponsavel.GetHashCode().ToString();
                                    //BP EXISTE NA BASE, UA E RESPONSAVEL DIFERENTES
                                    else if (bpParaPesquisaNaBase.AssetMovements.LastOrDefault(ultimaMovimentacaoBP => ultimaMovimentacaoBP.Status == true).RelatedManagerUnit.Code != codigoUGE)
                                        dePara__SAM_COMPEX_CPX_8000 = EnumSituacaoFisicaBP.OutraUGE.GetHashCode().ToString();
                                    else if (statusBP == StatusBP_ColetorCOMPEX_CPX8000.TransferenciaUA.GetHashCode().ToString())
                                        dePara__SAM_COMPEX_CPX_8000 = EnumSituacaoFisicaBP.OutraUA.GetHashCode().ToString();

                                    assetID = bpParaPesquisaNaBase.Id;
                                }
                                //SE EH BP NOVO
                                else if (bpParaPesquisaNaBase.IsNull() && Int32.TryParse(codigoUA, out _codigoUA))
                                {
                                    dePara__SAM_COMPEX_CPX_8000 = EnumSituacaoFisicaBP.OutroResponsavel.GetHashCode().ToString(); //"3";
                                    assetID = null;
                                }

                                chavesBP.Add(new KeyValuePair<string, string>("Estado", dePara__SAM_COMPEX_CPX_8000));

                                chavesBP.Add(new KeyValuePair<string, string>("ItemMaterial", codigoItemMaterial));
                                chavesBP.Add(new KeyValuePair<string, string>("Sigla", siglaBP));
                                chavesBP.Add(new KeyValuePair<string, string>("AssetId", (assetID.HasValue ? assetID.Value.ToString() : null)));

                                //objInventario = new Inventario() { DataInventario = DateTime.Now, Usuario = cpfUsuarioInventariante, UgeId = ugeID, UaId = uaID, ResponsavelId = responsavelID };
                                relacaoDadosBPsParaProcessamento.Add(chavesBP);
                                //relacaoBPsInventariados.Add(objInventario);

                                chavesBP = new List<KeyValuePair<string, string>>();
                                //objInventario = null;
                            }
                            else if (linhaDadosBP.Length != 65)
                            {
                                continue;
                            }
                        }
                    }
                    //else if (tipoArquivoExportado == "II")
                    //{
                    //    foreach (var linhaDadosBP in linhasConteudoArquivo)
                    //    {
                    //        siglaBP = linhaDadosBP.Substring(0, 0);
                    //        siglaBP = linhaDadosBP.Substring(0, 0);
                    //        siglaBP = linhaDadosBP.Substring(0, 0);
                    //        siglaBP = linhaDadosBP.Substring(0, 0);
                    //        siglaBP = linhaDadosBP.Substring(0, 0);
                    //        siglaBP = linhaDadosBP.Substring(0, 0);
                    //        siglaBP = linhaDadosBP.Substring(0, 0);
                    //        siglaBP = linhaDadosBP.Substring(0, 0);
                    //    }
                    //}
                }
            }


            return relacaoDadosBPsParaProcessamento;
        }

        private Asset obterDadosBemPatrimonial(string siglaBP, string chapaBP, string codigoUA)
        {
            Asset dadosBemPatrimonial = null;
            string _chapaBP = null;
            string _siglaBP = null;
            int _codigoUA = -1;
            Expression<Func<Asset, bool>> expWhereConsulta = null;


            _chapaBP = chapaBP;
            _siglaBP = siglaBP;
            if (Int32.TryParse(codigoUA, out _codigoUA))
            {
                var dadosUaConsulta = db.AdministrativeUnits.Where(uaSIAFEM => uaSIAFEM.Code == _codigoUA).FirstOrDefault();
                if (dadosUaConsulta.IsNotNull())
                {
                    expWhereConsulta = (bemPatrimonial => bemPatrimonial.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code == dadosUaConsulta.RelatedManagerUnit.RelatedBudgetUnit.RelatedInstitution.Code
                                                       && bemPatrimonial.InitialName == _siglaBP
                                                       && bemPatrimonial.NumberIdentification == _chapaBP
                                                       && bemPatrimonial.DiferenciacaoChapa == ""
                                                       && bemPatrimonial.Status == true);

                    dadosBemPatrimonial = db.Assets.Where(expWhereConsulta).FirstOrDefault();
                }
            }


            return dadosBemPatrimonial;
        }

        public JsonResult InformaNumRegistrosProcessados(int numeroRegistros)
        {
            MensagemModel mensagem = new MensagemModel() { Id = 0, Mensagem = "" };


            if (numeroRegistros >= 1)
            {
                mensagem.Id = 1;
                mensagem.Mensagem = String.Format("Processado(s) {0:D4} registro(s), do arquivo importado do coletor COMPEX CPX-8000!", numeroRegistros);
                return Json(mensagem, JsonRequestBehavior.AllowGet);
            }
            else
            {
                mensagem.Id = 0;
                mensagem.Mensagem = "Nenhum processamento efetuado!";
                return Json(mensagem, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CarregaPartialViewTipoOperacaoDispositivoInventariante(int tipoDispositivoInventarianteID, int tipoOperacaoDispositivoInventariante)
        {
            string nomeView = null;
            nomeView = (((tipoDispositivoInventarianteID == (int)EnumTipoDispositivoInventariante.COMPEX_CPX8000) &&
                         (tipoOperacaoDispositivoInventariante == (int)EnumTipoOperacaoDispositivoInventariante.GeracaoArquivos)) ? "_compex8000_geracaoArquivos" : "_compex8000_leituraArquivos");

            switch (tipoDispositivoInventarianteID)
            {
                //case (int)EnumTipoDispositivoInventariante.COMPEX_CPX8000: return PartialView("_compex8000");
                case (int)EnumTipoDispositivoInventariante.COMPEX_CPX8000: return PartialView(nomeView);

                default:
                    break;
            }

            return PartialView("");
        }

        public JsonResult ConsistenciaParametros(OperacoesComColetorViewModel viewPagina)
        {
            List<MensagemModel> listaMsgErro = null;
            MensagemModel msgErro = null;



            listaMsgErro = new List<MensagemModel>();
            var orgaoID = viewPagina.InstitutionId;
            var uaID = viewPagina.AdministrativeUnitId;
            if (viewPagina.GeraTabela_ItemMaterial && (orgaoID <= 0))
            {
                msgErro = new MensagemModel() { Id = 0, Mensagem = "Obrigatório selecionar um Órgão, para a geração do arquivo de catálogo de Item Material." };
                listaMsgErro.Add(msgErro);
            }

            if (viewPagina.GeraTabela_BemPatrimonial && (uaID <= 0))
            {
                msgErro = new MensagemModel() { Id = 1, Mensagem = "Obrigatório selecionar uma UA, para a geração do arquivo de Bens Patrimoniais da mesma." };
                listaMsgErro.Add(msgErro);
            }


            if (viewPagina.GeraTabela_Responsavel && (orgaoID <= 0 && uaID <= 0))
            { 
                msgErro = new MensagemModel() { Id = 2, Mensagem = "Obrigatório selecionar uma UA, para a geração do arquivo de Responsáveis para a mesma." };
                listaMsgErro.Add(msgErro);
            }


            if (viewPagina.GeraTabela_Sigla && (orgaoID <= 0))
            {
                msgErro = new MensagemModel() { Id = 3, Mensagem = "Obrigatório selecionar um Órgão, para a geração do arquivo de siglas do mesmo." };
                listaMsgErro.Add(msgErro);
            }

            if (viewPagina.GeraTabela_Terceiros && (orgaoID <= 0))
            {
                msgErro = new MensagemModel() { Id = 4, Mensagem = "Obrigatório selecionar um Órgão, para a geração do arquivo de Terceiros associados ao mesmo." };
                listaMsgErro.Add(msgErro);
            }


            return Json(listaMsgErro, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RetornaArquivo_CargaColetorDownload(OperacoesComColetorViewModel viewPagina)
        {
            int orgaoID = 0;
            int ugeID = 0;
            int uaID = 0;
            string nomeArquivo = null;
            string caminhoCompletoArquivo = null;
            IList<KeyValuePair<string, byte[]>> arquivosDeCargaParaDownload = null;


            Init();

            if (
                viewPagina.IsNotNull() &&
                (   viewPagina.GeraTabela_ItemMaterial
                 || viewPagina.GeraTabela_BemPatrimonial
                 || viewPagina.GeraTabela_Responsavel
                 || viewPagina.GeraTabela_Sigla
                 || viewPagina.GeraTabela_Terceiros
                ) //||
                //(viewPagina.GeraTabela_Responsavel && viewPagina.AdministrativeUnitId > 0)
               )
            {
                //ConsistenciaParametros(viewPagina);
                orgaoID = viewPagina.InstitutionId;
                ugeID = viewPagina.ManagerUnitId;
                uaID = viewPagina.AdministrativeUnitId;
                byte[] conteudoArquivo = null;

                KeyValuePair<string, byte[]> dadosTabelaCarga;
                arquivosDeCargaParaDownload = new List<KeyValuePair<string, byte[]>>();


                //Arquivos de BPs e de Responsaveis devem ser gerados juntos
                if (viewPagina.GeraTabela_Responsavel || viewPagina.GeraTabela_BemPatrimonial)
                    viewPagina.GeraTabela_BemPatrimonial = viewPagina.GeraTabela_Responsavel = (viewPagina.GeraTabela_Responsavel || viewPagina.GeraTabela_BemPatrimonial); 


                if (viewPagina.GeraTabela_ItemMaterial && (orgaoID > 0))
                {
                    geraArquivoParaColetor_ItemMaterial(orgaoID);


                    nomeArquivo = NomeArquivoConsultaPorColetor_ItemMaterial;
                    caminhoCompletoArquivo = String.Format(@"{0}\{1}", diretorioExportacaoArquivos, nomeArquivo);
                    conteudoArquivo = mFile.ReadAllBytes(caminhoCompletoArquivo);
                    dadosTabelaCarga = new KeyValuePair<string, byte[]>(nomeArquivo, conteudoArquivo);
                    arquivosDeCargaParaDownload.Add(dadosTabelaCarga);
                }


                if (viewPagina.GeraTabela_BemPatrimonial && ((orgaoID > 0 && uaID > 0) || (orgaoID > 0 && ugeID > 0)))
                {
                    geraArquivoParaColetor_BemPatrimonial(orgaoID, ugeID, uaID);


                    nomeArquivo = NomeArquivoConsultaPorColetor_BemPatrimonial;
                    caminhoCompletoArquivo = String.Format(@"{0}\{1}", diretorioExportacaoArquivos, nomeArquivo);
                    conteudoArquivo = mFile.ReadAllBytes(caminhoCompletoArquivo);
                    dadosTabelaCarga = new KeyValuePair<string, byte[]>(nomeArquivo, conteudoArquivo);
                    arquivosDeCargaParaDownload.Add(dadosTabelaCarga);
                }
                else if (viewPagina.GeraTabela_BemPatrimonial && (orgaoID <= 0 || ugeID <= 0 || uaID <= 0))
                {
                    //var msgErro = new MensagemModel() { Id = 1, Mensagem = "Obrigatório selecionar uma UA, para a geração do arquivo de Bens Patrimoniais da mesma." };
                    //var msgErro = "Obrigatório selecionar uma UA, para a geração do arquivo de Bens Patrimoniais da mesma.";
                    var msgErro = "Obrigatório selecionar ao menos a UGE (e/ou uma UA), para a geração do arquivo de Bens Patrimoniais da mesma.";
                    ModelState.AddModelError("AdministrativeUnitId", msgErro);
                }


                if (viewPagina.GeraTabela_Responsavel && ((orgaoID > 0 && uaID > 0) || (orgaoID > 0 && ugeID > 0)))
                {
                    geraArquivoParaColetor_Responsavel(orgaoID, ugeID, uaID);


                    nomeArquivo = NomeArquivoConsultaPorColetor_Responsavel;
                    caminhoCompletoArquivo = String.Format(@"{0}\{1}", diretorioExportacaoArquivos, nomeArquivo);
                    conteudoArquivo = mFile.ReadAllBytes(caminhoCompletoArquivo);
                    dadosTabelaCarga = new KeyValuePair<string, byte[]>(nomeArquivo, conteudoArquivo);
                    arquivosDeCargaParaDownload.Add(dadosTabelaCarga);
                }



                if (viewPagina.GeraTabela_Sigla && (orgaoID > 0))
                {
                    geraArquivoParaColetor_Siglas(orgaoID);


                    nomeArquivo = NomeArquivoConsultaPorColetor_Sigla;
                    caminhoCompletoArquivo = String.Format(@"{0}\{1}", diretorioExportacaoArquivos, nomeArquivo);
                    conteudoArquivo = mFile.ReadAllBytes(caminhoCompletoArquivo);
                    dadosTabelaCarga = new KeyValuePair<string, byte[]>(nomeArquivo, conteudoArquivo);
                    arquivosDeCargaParaDownload.Add(dadosTabelaCarga);
                }



                if (viewPagina.GeraTabela_Terceiros && (orgaoID > 0))
                {
                    geraArquivoParaColetor_Terceiros(orgaoID);


                    nomeArquivo = NomeArquivoConsultaPorColetor_Terceiro;
                    caminhoCompletoArquivo = String.Format(@"{0}\{1}", diretorioExportacaoArquivos, nomeArquivo);
                    conteudoArquivo = mFile.ReadAllBytes(caminhoCompletoArquivo);
                    dadosTabelaCarga = new KeyValuePair<string, byte[]>(nomeArquivo, conteudoArquivo);
                    arquivosDeCargaParaDownload.Add(dadosTabelaCarga);
                }


                if (arquivosDeCargaParaDownload.HasElements())
                    if (arquivosDeCargaParaDownload.Count == 1)
                    { return File(arquivosDeCargaParaDownload[0].Value, Application.Octet, arquivosDeCargaParaDownload[0].Key); }
                    else
                    {
                        // create a working memory stream
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            // create a zip
                            using (ZipArchive zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, false))
                            {
                                // interate through the source files
                                foreach (var arquivoDeCarga in arquivosDeCargaParaDownload)
                                {
                                    // add the item name to the zip
                                    ZipArchiveEntry zipItem = zip.CreateEntry(arquivoDeCarga.Key, CompressionLevel.Optimal);
                                    // add the item bytes to the zip entry by opening the original file and copying the bytes 
                                    using (MemoryStream originalFileMemoryStream = new MemoryStream(arquivoDeCarga.Value))
                                    using (Stream entryStream = zipItem.Open())
                                        originalFileMemoryStream.CopyTo(entryStream);
                                }
                            }

                            return File(memoryStream.ToArray(), Application.Octet, "Arquivos_Tabelas_Coletor_COMPEX_CPX8000.zip");
                        }
                    }
            }

            //return RedirectToAction("Gravacao");
            //return View(viewPagina);
            //return View("Gravacao");
            return RedirectToAction("Gravacao", "Inventarios");
        }

        private Tuple<bool, string, IList<string>> fileLoader(HttpPostedFileBase arquivoUploaded, string caminhoCompletoDoArquivo = null)
        {
            Tuple<bool, string, IList<string>> retornoProcessamentoCargaArquivo = null;
            IList<string> linhasConteudoArquivo = null;
            string linhaDadosArquivo = null;

            if (arquivoUploaded.IsNotNull() && arquivoUploaded.ContentLength > 0)
            {
                var arquivoUploadedParaProcessamento = arquivoUploaded;
                var nomeArquivoUploaded = arquivoUploadedParaProcessamento.FileName;
                var conteudoArquivo = arquivoUploadedParaProcessamento.InputStream;
                StreamWriter sw = null;


                try
                {
                    if (!String.IsNullOrWhiteSpace(caminhoCompletoDoArquivo))
                        sw = new StreamWriter(new FileStream(caminhoCompletoDoArquivo, FileMode.OpenOrCreate, FileAccess.Write));

                    using (StreamReader sr = new StreamReader(conteudoArquivo, Encoding.GetEncoding(1252)))
                    {
                        linhasConteudoArquivo = new List<string>();
                        while ((linhaDadosArquivo = sr.ReadLine()) != null)
                        {
                            linhasConteudoArquivo.Add(linhaDadosArquivo);
                            if (sw.IsNotNull())
                                sw.WriteLine(linhaDadosArquivo);
                        }

                        if (sw.IsNotNull())
                            sw.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao processar arquivo de carga", ex);
                }

                retornoProcessamentoCargaArquivo = Tuple.Create(true, nomeArquivoUploaded, linhasConteudoArquivo);
            }


            return retornoProcessamentoCargaArquivo;
        }

        private Tuple<bool, string, IList<string>> fileLoader(string caminhoCompletoDeArquivo)
        {
            Tuple<bool, string, IList<string>> retornoProcessamentoCargaArquivo = null;
            IList<string> linhasConteudoArquivo = null;
            string linhaDadosArquivo = null;

            //if (arquivoUploaded.IsNotNull() && arquivoUploaded.ContentLength > 0)
            if (!String.IsNullOrWhiteSpace(caminhoCompletoDeArquivo) && mFile.Exists(caminhoCompletoDeArquivo))
            {
                //var arquivoUploadedParaProcessamento = arquivoUploaded;
                //var nomeArquivoUploaded = arquivoUploadedParaProcessamento.FileName;
                //var conteudoArquivo = arquivoUploadedParaProcessamento.InputStream;

                try
                {
                    //using (StreamReader sr = new StreamReader(conteudoArquivo, Encoding.GetEncoding(1252)))
                    using (StreamReader sr = new StreamReader(caminhoCompletoDeArquivo, Encoding.GetEncoding(1252)))
                    {
                        linhasConteudoArquivo = new List<string>();
                        while ((linhaDadosArquivo = sr.ReadLine()) != null)
                            linhasConteudoArquivo.Add(linhaDadosArquivo);

                        //mFile.Delete(caminhoCompletoDeArquivo);
                    }

                    mFile.Delete(caminhoCompletoDeArquivo);
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao processar arquivo de carga", ex);
                }

                retornoProcessamentoCargaArquivo = Tuple.Create(true, string.Empty, linhasConteudoArquivo);
            }


            return retornoProcessamentoCargaArquivo;
        }

        [HttpPost]
        public JsonResult ImportaArquivo_ColetorDados()
        {
            MensagemModel mensagem = new MensagemModel() { Id = 0, Mensagem = "" };
            IList<string> linhasConteudoArquivo = null;
            int numeroRegistros = 0;
            string linhaDadosBP = null;
            string _mensagemParaUsuario = null;
            bool processamentoOK = false;

            try
            {

                Init();
                _mensagemParaUsuario = "Nenhum processamento efetuado para este tipo de inventário, com este arquivo!";
                if (this.Request != null && this.Request.Files.Count > 0 && this.Request.Files[0].ContentLength > 0)
                {
                    ////VALIDAR CONTEUDO ARQUIVO
                    //validaConteudoArquivoPorMapaPosicional();


                    if (ModelState.IsValid)
                    {
                        var arquivoDispositivoInventariante = this.Request.Files[0];
                        var nomeArquivoInventario = arquivoDispositivoInventariante.FileName;
                        var conteudoArquivo = arquivoDispositivoInventariante.InputStream;


                        //VALIDAR NOME ARQUIVO
                        validaNomeArquivoImportadoColetor(nomeArquivoInventario);

                        try
                        {
                            using (StreamReader sr = new StreamReader(conteudoArquivo, Encoding.GetEncoding(1252)))
                            {
                                linhasConteudoArquivo = new List<string>();
                                while ((linhaDadosBP = sr.ReadLine()) != null)
                                    linhasConteudoArquivo.Add(linhaDadosBP);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Erro ao processar arquivo de carga", ex);
                        }

                        //SE TUDO OK, ARQUIVO PARA PROCESSAMENTO
                        //ProcessarArquivoExportadoColetor(null, null, linhasConteudoArquivo);
                        processamentoOK = ProcessarArquivoImportadoColetor(null, linhasConteudoArquivo);
                        numeroRegistros = linhasConteudoArquivo.Count();
                    }

                    //return View();
                    //return RedirectToAction("Leitura");
                    //return View("Leitura");

                    if (processamentoOK)
                        _mensagemParaUsuario = String.Format("Processado(s) {0:#,##0} registro(s), do arquivo importado do coletor COMPEX CPX-8000!", numeroRegistros);
                }
                else
                {
                    _mensagemParaUsuario = "Por favor, verifique se o arquivo foi selcionado ou se o mesmo é válido";
                }
            }
            catch (Exception ex)
            {
                _mensagemParaUsuario = ex.Message;
            }

            mensagem.Mensagem = _mensagemParaUsuario;

            return Json(mensagem, JsonRequestBehavior.AllowGet);
        }

        private void validaConteudoArquivoPorMapaPosicional()
        {
            throw new NotImplementedException();
        }

        private void validaNomeArquivoImportadoColetor(string nomeArquivoImportadoColetor)
        {
            if (!String.IsNullOrWhiteSpace(nomeArquivoImportadoColetor) && nomeArquivoImportadoColetor.Split(new char[] { '.' })[0].Length != ComprimentoNomeArquivoColetor_COMPEX)
            {
                throw new Exception("Arquivo selecionado invalido e/ou fora de formato");
            }
            else
            {
                var dadosArquivoColetor = parsingNomeArquivoColetor(nomeArquivoImportadoColetor);
                
                if (dadosArquivoColetor.Count(infoArquivoInventarioColetor => infoArquivoInventarioColetor.Key == "TipoOperacao"
                                                                       && infoArquivoInventarioColetor.Value == "IN") != 1)
                    throw new Exception("Arquivo importado do coletor não é do tipo 'Inventário Normal'!");
            }
        }
    }
}