using SAM.Web.Context;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PatrimonioBusiness.contabiliza.entidades;
using Sam.Common.Util;
using System.Globalization;
using SAM.Web.Controllers.IntegracaoContabilizaSP;
using System.Data.Entity;
using SAM.Web.Common;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;





namespace SAM.Web.Controllers
{
    public class NLComplementarController : BaseController
    {
        private const string CST_CAMPO_OBSERVACAO = "'({0}NL COMPLEMENTAR) FECHAMENTO MENSAL INTEGRADO':'UGE {1}';'DocumentoSAM':'{2}';'Mes-Referencia':'{3}';'Conta Contabil Depreciação':'{4}';'Token SAM-Patrimonio':'{5}'";


        SAMContext contexto;
        SortedList<int, int> relacaoContaDepreciacao_ItemMaterial = new SortedList<int, int>();
        SortedList<int, string> relacaoContaDepreciacao_DescricaoContabilizaSP = new SortedList<int, string>();

        [HttpGet]
        public ActionResult Fechamento()
        {
            CarregaComboBoxes();
            return View();
        }

        [HttpPost]
        public ActionResult Fechamento(NLComplementarViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                GerarNLDeFechamento(viewmodel);
            }

            CarregaComboBoxes(viewmodel.InstitutionId, viewmodel.BudgetUnitId, viewmodel.ManagerUnitId, viewmodel.DepreciationAccountId);
            return View(viewmodel);
        }

        #region Métodos de Geração

        private void init()
        {
            using (SAMContext contextoCamadaDados = new SAMContext())
            {
                if (relacaoContaDepreciacao_ItemMaterial.Keys.Count() == 0)
                {   //contextoCamadaDados.DepreciationMaterialItems
                    //                   .ToList()
                    //                   .ForEach(itemMaterialContaContabilDepreciacao => relacaoContaDepreciacao_ItemMaterial.Add(itemMaterialContaContabilDepreciacao.DepreciationAccount, itemMaterialContaContabilDepreciacao.MaterialItemCode));

                    relacaoContaDepreciacao_ItemMaterial.Add(123810101, 5026660);
                    relacaoContaDepreciacao_ItemMaterial.Add(123810102, 5084784);
                    relacaoContaDepreciacao_ItemMaterial.Add(123810103, 4748875);
                    relacaoContaDepreciacao_ItemMaterial.Add(123810104, 5655080);
                    relacaoContaDepreciacao_ItemMaterial.Add(123810105, 3407489);
                    relacaoContaDepreciacao_ItemMaterial.Add(123810109, 3310566);
                    relacaoContaDepreciacao_ItemMaterial.Add(123810110, 5034280);
                    relacaoContaDepreciacao_ItemMaterial.Add(123810111, 4624777);
                    relacaoContaDepreciacao_ItemMaterial.Add(123810199, 3552675);
                }


                if (relacaoContaDepreciacao_DescricaoContabilizaSP.Keys.Count() == 0)
                    contextoCamadaDados.DepreciationAccounts
                                       .ToList()
                                       .ForEach(contaContabilDepreciacao => relacaoContaDepreciacao_DescricaoContabilizaSP.Add(contaContabilDepreciacao.Code, contaContabilDepreciacao.DescricaoContaDepreciacaoContabilizaSP));
            }
        }

        private void GerarNLDeFechamento(NLComplementarViewModel dados)
        {
            string codigoGestao = null;
            ManagerUnit uge = null;
            DepreciationAccount contaDepreciacao = null;




            init();
            using (contexto = new SAMContext())
            {
                contaDepreciacao = contexto.DepreciationAccounts.Find(dados.DepreciationAccountId);
                uge = contexto.ManagerUnits.Find(dados.ManagerUnitId);
                codigoGestao = BuscaGestaoPorUGE(dados.ManagerUnitId);
            }

            var codigoUGE = uge.Code;
            var contaContabilDepreciacao = contaDepreciacao.Code;
            var objSIAFDOC = geraObjetoSIAFDOC(codigoUGE, dados.MesRef, contaContabilDepreciacao, dados.DepreciacaoMensal, dados.NLEstorno);
            var saidaAuditoriaIntegracao = geraLinhaAuditoriaIntegracao(objSIAFDOC);
            geraNotaLancamentoPendencia(saidaAuditoriaIntegracao);
            geraRegistroFechamentoMensalIntegrado(saidaAuditoriaIntegracao, dados.DepreciacaoAcumulada);
          
        }

        private AccountingClosing geraRegistroFechamentoMensalIntegrado(AuditoriaIntegracao objAuditoriaIntegracao, decimal valorDepreciacaoAcumuladaMensal)
        {
            AccountingClosing registroFechamentoContabil = null;
            ManagerUnit dadosUGE = null;
            DepreciationAccount dadosContaContabilDepreciacao = null;
            int codigoContaContabilDepreciacao = 0;



            var strCodigoContaContabilDepreciacao = obtemNumeroContaContabilDepreciacao(objAuditoriaIntegracao.NotaFiscal);
            codigoContaContabilDepreciacao = Int32.Parse(strCodigoContaContabilDepreciacao);
            using (var contextoCamadaDados = new SAMContext())
            {
                dadosUGE = contextoCamadaDados.ManagerUnits.Where(ugeSIAFEM => ugeSIAFEM.Code == objAuditoriaIntegracao.UgeOrigem).FirstOrDefault();
                dadosContaContabilDepreciacao = contextoCamadaDados.DepreciationAccounts.Where(contaContabilDepreciacao => contaContabilDepreciacao.Code == codigoContaContabilDepreciacao).FirstOrDefault();
            }

            var anoMesReferencia = objAuditoriaIntegracao.NotaFiscal.Substring(10, 4).Insert(2, "/20");
            registroFechamentoContabil = new AccountingClosing()
                                                                {
                                                                    ManagerUnitCode = Int32.Parse(dadosUGE.Code),
                                                                    ManagerUnitDescription = dadosUGE.Description,
                                                                    DepreciationAccount = codigoContaContabilDepreciacao,
                                                                    DepreciationMonth = objAuditoriaIntegracao.ValorTotal.GetValueOrDefault(),
                                                                    AccumulatedDepreciation = valorDepreciacaoAcumuladaMensal,
                                                                    Status = false,
                                                                    DepreciationDescription = dadosContaContabilDepreciacao.Description,
                                                                    ReferenceMonth = "",
                                                                    ItemAccounts = relacaoContaDepreciacao_ItemMaterial[codigoContaContabilDepreciacao],
                                                                    AccountName = dadosContaContabilDepreciacao.DescricaoContaDepreciacaoContabilizaSP,

                                                                    ClosingId = objAuditoriaIntegracao.TokenAuditoriaIntegracao,
                                                                    ManagerCode = obterGestaoUGE(objAuditoriaIntegracao.ManagerUnitId),
                                                                    AuditoriaIntegracaoId = objAuditoriaIntegracao.Id,
                                                                };

            using (var contextoCamadaDados = new SAMContext())
            {
                contextoCamadaDados.AccountingClosings.Add(registroFechamentoContabil);
                contextoCamadaDados.SaveChanges();
            }

            return registroFechamentoContabil;
        }
        private SIAFDOC geraObjetoSIAFDOC(string codigoUGE, string anoMesReferencia, int codigoContaDepreciacao, decimal valorTotal, bool nlDeEstorno = false)
        {
            SIAFDOC objRetorno = null;
            string tokenAuditoriaIntegracao = null;
            string chaveSIAFMonitora = null;
            string acaoPagamentoNotaLancamento = null;
            string tipoPagamentoNotaLancamento = null;

            int mesReferencia = Int32.Parse(anoMesReferencia.Substring(4, 2));
            int anoReferencia = Int32.Parse(anoMesReferencia.Substring(0, 4));

            acaoPagamentoNotaLancamento = (nlDeEstorno ? "S" : "N");
            tipoPagamentoNotaLancamento = (nlDeEstorno ? "E" : "N");
            var dataPagamentoFechamentoMensal = new DateTime(anoReferencia, mesReferencia, 30);

            tokenAuditoriaIntegracao = Guid.NewGuid().ToString();
            chaveSIAFMonitora = String.Format("F{0}_tokenSAMP:{1}", tipoPagamentoNotaLancamento, tokenAuditoriaIntegracao);
            objRetorno = new SIAFDOC();
            objRetorno.CdMsg = "SIAFNlPatrimonial";

            SiafemDocNlPatrimonial siafemDocNlPatrimonial = new SiafemDocNlPatrimonial();
            NotaFiscal notaFiscal = new NotaFiscal();

            Repeticao repeticao = new Repeticao();
            string finalDaConta = (codigoContaDepreciacao == 0 ? "0000" : codigoContaDepreciacao.ToString().Substring(5, 4));
            //"UGE + conta + mesref exemplo: 380185123110303062020"
            NF nF = new NF() { NotaFiscal = String.Format("{0}{1}{2}{3}", codigoUGE, finalDaConta, mesReferencia.ToString("D2"), anoReferencia.ToString("D2")) };
            IM iM = new IM() { ItemMaterial = relacaoContaDepreciacao_ItemMaterial[codigoContaDepreciacao].ToString() };
            repeticao.IM = iM;
            repeticao.NF = nF;

            ItemMaterial itemMaterial = new ItemMaterial() { Repeticao = repeticao };
            notaFiscal.Repeticao = repeticao;
            siafemDocNlPatrimonial.NotaFiscal = notaFiscal;
            siafemDocNlPatrimonial.ItemMaterial = itemMaterial;

            XmlDoc xmlDoc = new XmlDoc()
                                        {
                                            Id = chaveSIAFMonitora,
                                            TipoMovimento = "DEPRECIAÇÃO",
                                            Data = dataPagamentoFechamentoMensal.ToString("dd/MM/yyyy"),
                                            Gestao = obterGestaoUGE(obterUgeId(Int32.Parse(codigoUGE))),
                                            Tipo_Entrada_Saida_Reclassificacao_Depreciacao = "DEPRECIAÇÃO",
                                            Item = relacaoContaDepreciacao_ItemMaterial[codigoContaDepreciacao].ToString(),
                                            TipoEstoque = "PERMANENTE",
                                            EstoqueDestino = "DESPESA DE DEPRECIAÇÃO DE BENS MÓVEIS (VARIAÇÃO)",
                                            EstoqueOrigem = relacaoContaDepreciacao_DescricaoContabilizaSP[codigoContaDepreciacao],
                                            ValorTotal = valorTotal.ToString().Replace(",", "."),
                                            NLEstorno = acaoPagamentoNotaLancamento,
                                            Observacao = tokenAuditoriaIntegracao,
                                            UgeOrigem = codigoUGE
                                        };

            siafemDocNlPatrimonial.Documento = xmlDoc;
            objRetorno.SiafemDocNlPatrimonial = siafemDocNlPatrimonial;




            return objRetorno;
        }
        public bool geraNotaLancamentoPendencia(AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            SAMContext _contextoCamadaDados = new SAMContext();
            NotaLancamentoPendenteSIAFEM pendenciaNLContabilizaSP = null;
            TipoNotaSIAF tipoAgrupamentoNotaLancamento = TipoNotaSIAF.NL_Depreciacao;
            int numeroRegistrosManipulados = 0;
            bool gravacaoNotaLancamentoPendente = false;


            if (registroAuditoriaIntegracao.IsNotNull())
            {
                tipoAgrupamentoNotaLancamento = obterTipoNotaSIAFEM__AuditoriaIntegracao(registroAuditoriaIntegracao);
                pendenciaNLContabilizaSP = new NotaLancamentoPendenteSIAFEM()
                                                                              {
                                                                                  AuditoriaIntegracaoId = registroAuditoriaIntegracao.Id,
                                                                                  DataHoraEnvioMsgWS = registroAuditoriaIntegracao.DataEnvio,
                                                                                  ManagerUnitId = registroAuditoriaIntegracao.ManagerUnitId,
                                                                                  TipoNotaPendencia = (short)tipoAgrupamentoNotaLancamento,
                                                                                  NumeroDocumentoSAM = registroAuditoriaIntegracao.NotaFiscal,
                                                                                  StatusPendencia = 1, //Status: Ativo
                                                                                  ErroProcessamentoMsgWS = "PENDENCIA DE NL COMPLEMENTAR DE 'FECHAMENTO MENSAL INTEGRADO' GERADA VIA SISTEMA",
                                                                              };

                using (_contextoCamadaDados = new SAMContext())
                {
                    _contextoCamadaDados.NotaLancamentoPendenteSIAFEMs.Add(pendenciaNLContabilizaSP);
                    numeroRegistrosManipulados = _contextoCamadaDados.SaveChanges();
                }
            }


            gravacaoNotaLancamentoPendente = (numeroRegistrosManipulados > 0);
            return gravacaoNotaLancamentoPendente;
        }
        private TipoNotaSIAF obterTipoNotaSIAFEM__AuditoriaIntegracao(AuditoriaIntegracao registroAuditoriaIntegracao)
        {
            TipoNotaSIAF tipoNotaSIAF = TipoNotaSIAF.Desconhecido;


            if (registroAuditoriaIntegracao.IsNotNull() && !String.IsNullOrWhiteSpace(registroAuditoriaIntegracao.TipoMovimento))
            {
                switch (registroAuditoriaIntegracao.TipoMovimento.ToUpperInvariant())
                {
                    case "ENTRADA":
                    case "SAIDA":
                    case "SAÍDA": { tipoNotaSIAF = TipoNotaSIAF.NL_Liquidacao; } break;
                    case "DEPRECIACAO":
                    case "DEPRECIAÇÃO": { tipoNotaSIAF = TipoNotaSIAF.NL_Depreciacao; } break;
                    case "RECLASSIFICACAO":
                    case "RECLASSIFICAÇÃO": { tipoNotaSIAF = TipoNotaSIAF.NL_Reclassificacao; } break;
                }
            }



            return tipoNotaSIAF;
        }
        private string BuscaGestaoPorUGE(int IdUGE) {
            return (from i in contexto.Institutions
                    join b in contexto.BudgetUnits on i.Id equals b.InstitutionId
                    join m in contexto.ManagerUnits on b.Id equals m.BudgetUnitId
                    where m.Id == IdUGE
                    select i.ManagerCode).FirstOrDefault();
        }
        #endregion

        #region CarregaCombo
        private void CarregaComboBoxes(int IdOrgao = 0, int IdUO = 0, int IdUGE = 0, int IdContaDepreciacao = 0) {

            if (IdOrgao == 0)
            {
                ViewBag.Institutions = new SelectList(GetOrgaosIntegrados(), "Id", "Description");
                ViewBag.BudgetUnits = new SelectList(GetUos(), "Id", "Description");
                ViewBag.ManagerUnits = new SelectList(GetUGEsIntegradas(), "Id", "Description");
            }
            else {
                ViewBag.Institutions = new SelectList(GetOrgaosIntegrados(), "Id", "Description", IdOrgao);
                ViewBag.BudgetUnits = new SelectList(GetUos(IdOrgao), "Id", "Description", IdUO);
                ViewBag.ManagerUnits = new SelectList(GetUGEsIntegradas(IdUO), "Id", "Description", IdUGE);
            }

            ViewBag.DepreciationAccounts = new SelectList(GetContasDepreciacao(), "Id", "Description", IdContaDepreciacao);

        }
        
        #endregion

        #region RetornaListaDosCombos

        private List<Institution> GetOrgaosIntegrados()
        {
            List<Institution> query;
            using (var dbHierarquia = new HierarquiaContext())
            {
                query = (from i in dbHierarquia.Institutions
                         where i.flagIntegracaoSiafem
                         select i).ToList();

                query.ForEach(o => o.Description = o.Code + " - " + o.Description);

                Institution institution = new Institution();
                institution.Id = 0;
                institution.Code = "0";
                institution.Description = "Selecione o Orgão";
                query.Add(institution);
            }

            return query.ToList().OrderBy(o => int.Parse(o.Code)).ToList();
        }

        private List<BudgetUnit> GetUos(int? IdOrgao = null)
        {
            List<BudgetUnit> query;

            if (IdOrgao!= null && IdOrgao != 0)
            {
                using (var dbHierarquia = new HierarquiaContext())
                {
                    query = (from b in dbHierarquia.BudgetUnits
                             where b.InstitutionId == IdOrgao && b.Status == true
                             select b).ToList();

                    query.ForEach(o => o.Description = o.Code + " - " + o.Description);
                }
            }
            else
            {
                query = new List<BudgetUnit>();
            }

            BudgetUnit budgetUnit = new BudgetUnit();
            budgetUnit.Id = 0;
            budgetUnit.Code = "0";
            budgetUnit.Description = "Selecione a UO";
            query.Add(budgetUnit);

            return query.ToList().OrderBy(u => int.Parse(u.Code)).ToList();
        }

        private List<ManagerUnit> GetUGEsIntegradas(int? IdUO = null) {
            List<ManagerUnit> query;

            if (IdUO.HasValue && IdUO != 0)
            {
                using (var dbHierarquia = new HierarquiaContext())
                {
                    query = (from m in dbHierarquia.ManagerUnits
                             where m.BudgetUnitId == IdUO 
                             && m.Status
                             && m.FlagIntegracaoSiafem
                             select m).ToList();
                }
                query.ForEach(o => o.Description = o.Code + " - " + o.Description);
            }
            else
            {
                query = new List<ManagerUnit>();
            }

            ManagerUnit managerUnit = new ManagerUnit();

            managerUnit.Id = 0;
            managerUnit.Code = "0";
            managerUnit.Description = "Selecione a UGE";
            query.Add(managerUnit);

            return query.ToList().OrderBy(uge => int.Parse(uge.Code)).ToList();
        }

        private List<DepreciationAccount> GetContasDepreciacao() {
            List<DepreciationAccount> lista = null;
            using (var contexto = new SAMContext()) {
                lista = (from d in contexto.DepreciationAccounts
                         where d.Status
                         select d).ToList();

                lista.ForEach(o => o.Description = o.Code + " - " + o.Description);
            }
            return lista;
        }
        #endregion


        #region Metodos Importados de Outras Controllers
        private AuditoriaIntegracao criaRegistroAuditoria(SIAFDOC objSIAFDOC, bool ehEstorno = false)
        {
            AuditoriaIntegracao registroAuditoriaIntegracao = null;
            string chaveSIAFMONITORA = null;
            string campoObservacao = null;
            string campoNotaFiscal = null;


            if (objSIAFDOC.IsNotNull())
            {
                chaveSIAFMONITORA = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Id.Substring(13, 36);
                CultureInfo cultureInfo = new CultureInfo("pt-BR");
                registroAuditoriaIntegracao = new AuditoriaIntegracao();

                registroAuditoriaIntegracao.DocumentoId                                    = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Id;
                registroAuditoriaIntegracao.TipoMovimento                                  = objSIAFDOC.SiafemDocNlPatrimonial.Documento.TipoMovimento;
                registroAuditoriaIntegracao.Data                                           = DateTime.ParseExact(objSIAFDOC.SiafemDocNlPatrimonial.Documento.Data, "dd/MM/yyyy", cultureInfo);
                registroAuditoriaIntegracao.UgeOrigem                                      = objSIAFDOC.SiafemDocNlPatrimonial.Documento.UgeOrigem;
                registroAuditoriaIntegracao.Gestao                                         = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Gestao;
                registroAuditoriaIntegracao.Tipo_Entrada_Saida_Reclassificacao_Depreciacao = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Tipo_Entrada_Saida_Reclassificacao_Depreciacao;
                registroAuditoriaIntegracao.CpfCnpjUgeFavorecida                           = objSIAFDOC.SiafemDocNlPatrimonial.Documento.CpfCnpjUgeFavorecida;
                registroAuditoriaIntegracao.GestaoFavorecida                               = objSIAFDOC.SiafemDocNlPatrimonial.Documento.GestaoFavorecida;
                registroAuditoriaIntegracao.Item                                           = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Item;
                registroAuditoriaIntegracao.TipoEstoque                                    = objSIAFDOC.SiafemDocNlPatrimonial.Documento.TipoEstoque;
                registroAuditoriaIntegracao.Estoque                                        = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Estoque;
                registroAuditoriaIntegracao.EstoqueDestino                                 = objSIAFDOC.SiafemDocNlPatrimonial.Documento.EstoqueDestino;
                registroAuditoriaIntegracao.EstoqueOrigem                                  = objSIAFDOC.SiafemDocNlPatrimonial.Documento.EstoqueOrigem;
                registroAuditoriaIntegracao.TipoMovimentacao                               = objSIAFDOC.SiafemDocNlPatrimonial.Documento.TipoMovimentacao;
                registroAuditoriaIntegracao.ValorTotal                                     = Decimal.Parse(objSIAFDOC.SiafemDocNlPatrimonial.Documento.ValorTotal.Replace(".", ","), cultureInfo);
                registroAuditoriaIntegracao.ControleEspecifico                             = objSIAFDOC.SiafemDocNlPatrimonial.Documento.ControleEspecifico;
                registroAuditoriaIntegracao.ControleEspecificoEntrada                      = objSIAFDOC.SiafemDocNlPatrimonial.Documento.ControleEspecificoEntrada;
                registroAuditoriaIntegracao.ControleEspecificoSaida                        = objSIAFDOC.SiafemDocNlPatrimonial.Documento.ControleEspecificoSaida;
                registroAuditoriaIntegracao.FonteRecurso                                   = objSIAFDOC.SiafemDocNlPatrimonial.Documento.FonteRecurso;
                registroAuditoriaIntegracao.NLEstorno                                      = objSIAFDOC.SiafemDocNlPatrimonial.Documento.NLEstorno;
                registroAuditoriaIntegracao.Empenho                                        = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Empenho;
                //registroAuditoriaIntegracao.Observacao                                     = objSIAFDOC.SiafemDocNlPatrimonial.Documento.Observacao;
                //registroAuditoriaIntegracao.NotaFiscal                                     = objSIAFDOC.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal.Substring(0, 14);

                campoObservacao = montaCampoObservacao(objSIAFDOC);
                campoNotaFiscal = ((objSIAFDOC.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal.Length > 14)? objSIAFDOC.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal.Substring(0, 14) : objSIAFDOC.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal);
                registroAuditoriaIntegracao.Observacao                                     = campoObservacao;
                registroAuditoriaIntegracao.NotaFiscal                                     = campoNotaFiscal;
                registroAuditoriaIntegracao.ItemMaterial                                   = objSIAFDOC.SiafemDocNlPatrimonial.ItemMaterial.Repeticao.IM.ItemMaterial;
                registroAuditoriaIntegracao.TokenAuditoriaIntegracao                       = Guid.Parse(chaveSIAFMONITORA);


                registroAuditoriaIntegracao.ManagerUnitId                                  = obterUgeId(Int32.Parse(objSIAFDOC.SiafemDocNlPatrimonial.Documento.UgeOrigem));
                registroAuditoriaIntegracao.DataEnvio                                      = DateTime.Now;
            }



            return registroAuditoriaIntegracao;
        }
        private AuditoriaIntegracao geraLinhaAuditoriaIntegracao(SIAFDOC objSIAFDoc)
        {
            SAMContext contextoCamadaDados = new SAMContext();
            AuditoriaIntegracao linhaAuditoriaIntegracaoCriadaNoBancoDeDados = null;
            AuditoriaIntegracao registroAuditoriaIntegracaoParaBD = null;
            string xmlParaWebserviceModoNovo = null;
            bool ehEstorno = false;



            if (objSIAFDoc.IsNotNull())
            {
                var usuarioLogado = UserCommon.CurrentUser();
                var cpfUsuarioSessaoLogada = (usuarioLogado.IsNotNull() ? usuarioLogado.CPF : null);
                linhaAuditoriaIntegracaoCriadaNoBancoDeDados = new AuditoriaIntegracao();
                ehEstorno = objSIAFDoc.SiafemDocNlPatrimonial.Documento.NLEstorno.ToUpperInvariant() == "S";
                /*
                string finalDaConta = (item.DepreciationAccount == null ? "0000" : item.DepreciationAccount.ToString().Substring(5, 4));
                string mes = item.ReferenceMonth.Substring(4, 2);
                string ano = item.ReferenceMonth.Substring(2, 2);
                NotaFiscal = string.Format("{0}{1}{2}{3}", managerUnitCode, finalDaConta, mes, ano) //"UGE + conta + mesref exemplo: 380185123110303062020"
                 */
                registroAuditoriaIntegracaoParaBD = this.criaRegistroAuditoria(objSIAFDoc, ehEstorno);
                xmlParaWebserviceModoNovo = AuditoriaIntegracaoGeradorXml.SiafemDocNLPatrimonial(registroAuditoriaIntegracaoParaBD, ehEstorno);
                registroAuditoriaIntegracaoParaBD.MsgEstimuloWS = xmlParaWebserviceModoNovo;
                registroAuditoriaIntegracaoParaBD.UsuarioSAM = cpfUsuarioSessaoLogada;
                insereRegistroAuditoriaParcialNaBaseDeDados(registroAuditoriaIntegracaoParaBD);


                linhaAuditoriaIntegracaoCriadaNoBancoDeDados = registroAuditoriaIntegracaoParaBD;
            }


            return linhaAuditoriaIntegracaoCriadaNoBancoDeDados;
        }
        private bool insereRegistroAuditoriaParcialNaBaseDeDados(AuditoriaIntegracao entidadeAuditoria)
        {
            int numeroRegistrosManipulados = 0;
            //SAMContext contextoCamadaDados = null;
            try
            {
                using (var contextoCamadaDados = new SAMContext())
                {
                    contextoCamadaDados.AuditoriaIntegracoes.Add(entidadeAuditoria);
                    numeroRegistrosManipulados = contextoCamadaDados.SaveChanges();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                var msgErro = "SAM|Erro ao inserir registro na trilha de auditoria de integração.";

                using (var contextoCamadaDados = new SAMContext())
                {
                    ErroLog rowTabelaLOG = new ErroLog()
                    {
                        DataOcorrido = DateTime.Now,
                        Usuario = entidadeAuditoria.UsuarioSAM,
                        exMessage = msgErro,
                        exStackTrace = excErroOperacaoBancoDados.StackTrace,
                        exTargetSite = excErroOperacaoBancoDados.TargetSite.ToString()
                    };

                    contextoCamadaDados.ErroLogs.Add(rowTabelaLOG);
                }

            }

            return (numeroRegistrosManipulados > 0);
        }
        private string montaCampoObservacao(SIAFDOC objXML)
        {
            string campoObservacao = null;
            string campoNotaFiscal = null;
            string contaContabilDepreciacao = null;
            string codigoUGE = null;
            string numeroDocumentoSAM = null;
            string mesReferencia = null;
            string mesReferenciaFechamento = null;
            string anoMesReferencia = null;
            string tokenSAMPatrimonio = null;
            string indicativoEstorno = null;
            bool ehEstorno = false;

            /*
               NotaFiscal = string.Format("{0}{1}{2}{3}", managerUnitCode, finalDaConta, mes, ano) "380128 0109 07 20"
               'FECHAMENTO MENSAL INTEGRADO':'UGE 380xxx'; 
               'DocumentoSAM':'380xxx01010720'; 
               'Mes-Referencia':'07/2020'; 
               'Conta Contabil Depreciação':'12380101'; 
               'Token SAM-Patrimonio': '00000000-0000-0000-0000-000000000000'.
            */

            if (objXML.IsNotNull())
            {
                numeroDocumentoSAM = objXML.SiafemDocNlPatrimonial.NotaFiscal.Repeticao.NF.NotaFiscal.Substring(0, 14);
                campoNotaFiscal = numeroDocumentoSAM;

                ehEstorno = (objXML.SiafemDocNlPatrimonial.Documento.NLEstorno.ToUpperInvariant() == "S");
                indicativoEstorno = (ehEstorno ? "ESTORNO " : null);
                codigoUGE = campoNotaFiscal.Substring(0, 6);
                mesReferencia = campoNotaFiscal.Substring(10, 2);
                anoMesReferencia = campoNotaFiscal.Substring(12, 2);
                mesReferenciaFechamento = String.Format("{0}/20{1}", mesReferencia, anoMesReferencia);
                contaContabilDepreciacao = obtemNumeroContaContabilDepreciacao(campoNotaFiscal);
                tokenSAMPatrimonio = objXML.SiafemDocNlPatrimonial.Documento.Id.Substring(13, 36);
                campoObservacao = String.Format(CST_CAMPO_OBSERVACAO, indicativoEstorno, codigoUGE, numeroDocumentoSAM, mesReferenciaFechamento, contaContabilDepreciacao, tokenSAMPatrimonio);
            }

            return campoObservacao;
        }

        internal string obterGestaoUGE(int? ugeId)
        {
            string codigoGestao = null;
            SAMContext contextoCamadaDados = null;
            ManagerUnit dadosUGE = null;


            if (ugeId.HasValue && ugeId > 0)
            {
                BaseController.InitDisconnectContext(ref contextoCamadaDados);
                dadosUGE = contextoCamadaDados.ManagerUnits.Where(ugeSIAFEM => ugeSIAFEM.Id == ugeId).FirstOrDefault();
                if (dadosUGE.IsNotNull())
                {
                    dadosUGE.RelatedBudgetUnit = contextoCamadaDados.BudgetUnits.Where(uoSIAFEM => uoSIAFEM.Id == dadosUGE.BudgetUnitId).FirstOrDefault();
                    if (dadosUGE.RelatedBudgetUnit.IsNotNull())
                    {
                        dadosUGE.RelatedBudgetUnit.RelatedInstitution = contextoCamadaDados.Institutions.Where(orgaoSIAFEM => orgaoSIAFEM.Id == dadosUGE.RelatedBudgetUnit.InstitutionId).FirstOrDefault();
                        if (dadosUGE.RelatedBudgetUnit.RelatedInstitution.IsNotNull())
                        {
                            codigoGestao = dadosUGE.RelatedBudgetUnit.RelatedInstitution.ManagerCode;
                            codigoGestao = codigoGestao.PadLeft(5, '0');
                        }
                    }
                }
            }

            return codigoGestao;
        }
        private int obterUgeId(int codigoUGE)
        {
            int UGEId = 0;
            SAMContext contextoCamadaDados = null;
            ManagerUnit dadosUGE = null;


            if (codigoUGE > 0)
            {
                BaseController.InitDisconnectContext(ref contextoCamadaDados);
                dadosUGE = contextoCamadaDados.ManagerUnits.Where(ugeSIAFEM => ugeSIAFEM.Code == codigoUGE.ToString()
                                                                            && ugeSIAFEM.Status == true)
                                                           .FirstOrDefault();
                if (dadosUGE.IsNotNull())
                    UGEId = dadosUGE.Id;
            }

            return UGEId;
        }
        private string obtemNumeroContaContabilDepreciacao(string notaFiscal)
        {
            string contaContabilDepreciacao = null;

            if (!String.IsNullOrWhiteSpace(notaFiscal) && (notaFiscal.Length >= 10))
            {
                contaContabilDepreciacao = notaFiscal.Substring(6, 4);
                if (contaContabilDepreciacao == "0000")
                {
                    contaContabilDepreciacao = null;
                }
                else
                {
                    if (contaContabilDepreciacao == "9999")
                        contaContabilDepreciacao = "99999" + contaContabilDepreciacao;
                    else
                        contaContabilDepreciacao = "12381" + contaContabilDepreciacao;
                }
            }


            return contaContabilDepreciacao;
        }
        #endregion Metodos Importados de Outras Controllers
    }
}