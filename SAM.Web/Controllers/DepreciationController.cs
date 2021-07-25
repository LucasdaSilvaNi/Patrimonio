using SAM.Web.Common;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAM.Web.Controllers
{
    public class DepreciationController : BaseController
    {
        private SAMContext db = new SAMContext();
        // GET: Depreciation
        [HttpGet]
        public ActionResult Index()
        {
            User u = UserCommon.CurrentUser();
            var perflLogado = base.BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);

            var model = new ManagerUnitDepreciationModel();
            model.ManagerUnitId = perflLogado.ManagerUnitId.Value;

            using (db = new SAMContext()) {
                string mesref = db.ManagerUnits
                                .FirstOrDefault(m => m.Id == model.ManagerUnitId)
                                .ManagmentUnit_YearMonthReference;
                model.UGEemAbrilDeDoisMilEVinte = mesref.Equals("202004");
            }


            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> Depreciar(ManagerUnitDepreciationModel model)
        {
            string _mensagemParaUsuario = string.Empty;
            IList<DepreciacaoMensagemViewModel> retornoDepreciacoes = new List<DepreciacaoMensagemViewModel>();
            DateTime dataInicio;
            try
            {

                var managerUnit = (from m in db.ManagerUnits
                                   where m.Id == model.ManagerUnitId
                                   select m).AsNoTracking().FirstOrDefault();

                dataInicio = DateTime.Now;

                if (!managerUnit.ManagmentUnit_YearMonthReference.Equals("202004"))
                {
                    return Json(new { data = "", erro = "UGE precisa estar com mês de referência em abril/2020" }, JsonRequestBehavior.AllowGet);
                }

                int ano = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4));
                int mes = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2));

                DateTime mesAnoReferencia = new DateTime(ano, mes, 1);
                DateTime mesAnoReferenciaVerificar = mesAnoReferencia.AddMonths(-1);

                string Retorno = string.Empty;




                _mensagemParaUsuario = "Fechamento realizado com sucesso";
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                IQueryable<DepreciacaoParametroModel> _query;
                bool depreciouUgeinteira = false;

                if (!string.IsNullOrEmpty(model.NumberIdentification) && model.MaterialItemCode.HasValue)
                {
                    _query = (from a in db.Assets
                              join m in db.AssetMovements on a.Id equals m.AssetId
                              where a.MaterialGroupCode != 0
                                 && a.ManagerUnitId == model.ManagerUnitId
                                 && a.flagVerificado == null
                                 && a.flagDepreciaAcumulada == 1
                                 && a.NumberIdentification.Equals(model.NumberIdentification)
                                 && a.MaterialItemCode == model.MaterialItemCode
                                 && (a.flagAcervo == null || a.flagAcervo == false)
                                 && (a.flagTerceiro == null || a.flagTerceiro == false)
                                 && (m.FlagEstorno == null || m.FlagEstorno == false)

                              select new DepreciacaoParametroModel()
                              {
                                  MaterialItemCode = a.MaterialItemCode,
                                  AssetStartId = a.AssetStartId,
                                  AssetId = a.Id
                              }).Distinct();
                }
                else if (!string.IsNullOrEmpty(model.NumberIdentification) && !model.MaterialItemCode.HasValue)
                {
                    _query = (from a in db.Assets
                              join m in db.AssetMovements on a.Id equals m.AssetId
                              where a.MaterialGroupCode != 0
                                 && a.ManagerUnitId == model.ManagerUnitId
                                 && a.flagVerificado == null
                                 && a.flagDepreciaAcumulada == 1
                                 && a.NumberIdentification.Equals(model.NumberIdentification)
                                 && (a.flagAcervo == null || a.flagAcervo == false)
                                 && (a.flagTerceiro == null || a.flagTerceiro == false)
                                 && (m.FlagEstorno == null || m.FlagEstorno == false)
                              select new DepreciacaoParametroModel()
                              {
                                  MaterialItemCode = a.MaterialItemCode,
                                  AssetStartId = a.AssetStartId,
                                  AssetId = a.Id
                              }).Distinct();
                }
                else if (model.MaterialItemCode.HasValue)
                {
                    _query = (from a in db.Assets
                              join m in db.AssetMovements on a.Id equals m.AssetId
                              where a.MaterialGroupCode != 0
                                 && a.ManagerUnitId == model.ManagerUnitId
                                 && a.flagVerificado == null
                                 && a.flagDepreciaAcumulada == 1
                                 && a.MaterialItemCode == model.MaterialItemCode.Value
                                 && (a.flagAcervo == null || a.flagAcervo == false)
                                 && (a.flagTerceiro == null || a.flagTerceiro == false)
                                 && (m.FlagEstorno == null || m.FlagEstorno == false)

                              select new DepreciacaoParametroModel()
                              {
                                  MaterialItemCode = a.MaterialItemCode,
                                  AssetStartId = a.AssetStartId,
                                  AssetId = a.Id
                              }).Distinct();
                }
                else if (model.BookAccount.HasValue)
                {
                    _query = (from a in db.Assets
                              join m in db.AssetMovements on a.Id equals m.AssetId
                              join b in db.AuxiliaryAccounts on m.AuxiliaryAccountId equals b.Id
                              where a.MaterialGroupCode != 0
                                 && a.ManagerUnitId == model.ManagerUnitId
                                 && a.flagVerificado == null
                                 && a.flagDepreciaAcumulada == 1
                                 && (a.flagAcervo == null || a.flagAcervo == false)
                                 && (a.flagTerceiro == null || a.flagTerceiro == false)
                                 && (m.FlagEstorno == null || m.FlagEstorno == false)
                                 && b.BookAccount == model.BookAccount.Value
                              select new DepreciacaoParametroModel()
                              {
                                  MaterialItemCode = a.MaterialItemCode,
                                  AssetStartId = a.AssetStartId,
                                  AssetId = a.Id
                              }).Distinct();
                }
                else
                {
                    _query = (from a in db.Assets
                              join m in db.AssetMovements on a.Id equals m.AssetId
                              where a.MaterialGroupCode != 0
                                 && a.ManagerUnitId == model.ManagerUnitId
                                 && a.flagVerificado == null
                                 && a.flagDepreciaAcumulada == 1
                                 && (a.flagAcervo == null || a.flagAcervo == false)
                                 && (a.flagTerceiro == null || a.flagTerceiro == false)
                                 && (m.FlagEstorno == null || m.FlagEstorno == false)
                              select new DepreciacaoParametroModel()
                              {
                                  MaterialItemCode = a.MaterialItemCode,
                                  AssetStartId = a.AssetStartId,
                                  AssetId = a.Id
                              }).Distinct();

                    depreciouUgeinteira = true;
                }



                var depreciacoes = _query.ToList();
                int quantidadeDepreciada = 0;
     
                foreach (var depreciacao in depreciacoes)
                {
                    int _assetStartId = 0;
                    var transferencia = await (from t in db.AssetMovements where t.AssetTransferenciaId == depreciacao.AssetId orderby t.Id descending select t).FirstOrDefaultAsync();

                    if (transferencia != null)
                    {
                        _assetStartId = transferencia.AssetId;
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[Asset] SET [AssetStartId] =" + _assetStartId.ToString() + " WHERE [Id] =" + transferencia.AssetId.ToString());
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] =" + _assetStartId.ToString() + " WHERE [AssetId] =" + transferencia.AssetId.ToString());

                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[Asset] SET [AssetStartId] =" + _assetStartId.ToString() + " WHERE [Id] =" + depreciacao.AssetId.ToString());
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] =" + _assetStartId.ToString() + " WHERE [AssetId] =" + depreciacao.AssetId.ToString());
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[Asset] SET [AssetStartId] =" + depreciacao.AssetId.ToString() + " WHERE [Id] =" + depreciacao.AssetId.ToString());
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] =" + depreciacao.AssetId.ToString() + " WHERE [AssetId] =" + depreciacao.AssetId.ToString());

                        _assetStartId = depreciacao.AssetId;
                    }




                    SqlParameter[] parametros =
                    {
                        new SqlParameter("@ManagerUnitId", model.ManagerUnitId),
                        new SqlParameter("@MaterialItemCode", depreciacao.MaterialItemCode),
                        new SqlParameter("@AssetStartId", _assetStartId),
                        new SqlParameter("@DataFinal", mesAnoReferenciaVerificar),
                        new SqlParameter("@Fechamento", false),
                        new SqlParameter("@Retorno",System.Data.SqlDbType.VarChar,1000) { Direction= System.Data.ParameterDirection.Output },
                        new SqlParameter("@Erro",System.Data.SqlDbType.Bit) { Direction= System.Data.ParameterDirection.Output }

                    };

                    var _registro = this.db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[MonthlyDepreciation] WHERE [MaterialItemCode] = " + depreciacao.MaterialItemCode.ToString() + "  AND [AssetStartId] = " + _assetStartId.ToString());
                    var resultado = this.db.Database.ExecuteSqlCommand("EXEC [dbo].[SAM_DEPRECIACAO_UGE] @ManagerUnitId,@MaterialItemCode,@AssetStartId,@DataFinal,@Fechamento,@Retorno OUT,@Erro OUT", parametros);

                    
                    var depreciacaoMensagem = new DepreciacaoMensagemViewModel();

                    var erro = bool.Parse(parametros[6].Value.ToString());
                    if (erro)
                    {
                        depreciacaoMensagem.AssetId = depreciacao.AssetId;
                        depreciacaoMensagem.AssetStartId = _assetStartId;
                        depreciacaoMensagem.MaterialItemCode = depreciacao.MaterialItemCode;

                        var Bp = (from ben in db.Assets where ben.Id == depreciacao.AssetId select ben).AsNoTracking().FirstOrDefault();

                        depreciacaoMensagem.NumberIdentification = Bp.NumberIdentification;

                        if (Bp.AcquisitionDate.Year < 1900)
                        {
                            depreciacaoMensagem.Tipo = "aquisicao";
                            depreciacaoMensagem.AcquisitionDate = Bp.AcquisitionDate.ToString("dd/MM/yyyy");
                            depreciacaoMensagem.Mensagem = "Data de aquisição ínvalida, ano da data é menor que 1900.";
                        }

                        else if (Bp.MovimentDate.Year < 1900)
                        {
                            depreciacaoMensagem.Tipo = "incorporacao";
                            depreciacaoMensagem.MovimentDate = Bp.MovimentDate.ToString("dd/MM/yyyy");
                            depreciacaoMensagem.Mensagem = "Data de incorporação ínvalida, ano da data é menor que 1900.";
                        }

                        if (Bp.AcquisitionDate.Year < 1900 && Bp.MovimentDate.Year < 1900)
                        {
                            depreciacaoMensagem.Tipo = "ambas";
                            depreciacaoMensagem.AcquisitionDate = Bp.AcquisitionDate.ToString("dd/MM/yyyy");
                            depreciacaoMensagem.MovimentDate = Bp.MovimentDate.ToString("dd/MM/yyyy");
                            depreciacaoMensagem.Mensagem = "Datas de aquisição e incorporação ínvalidas, ano das datas é menor que 1900.";

                        }
                        else
                        {
                            depreciacaoMensagem.Mensagem = GetMensagemDataInvalida(parametros[5].Value.ToString());
                        }


                        depreciacaoMensagem.Erro = erro;

                        retornoDepreciacoes.Add(depreciacaoMensagem);
                    }
                    else
                    {
                        quantidadeDepreciada += 1;


                    }



                }

                var _depreciacaoMensagem = new DepreciacaoMensagemViewModel();


                if (depreciacoes.Count < 1)
                {
                    _depreciacaoMensagem.Mensagem = "Bem patrimonial não foi depreciado.";
                    _depreciacaoMensagem.quantidadeDepreciada = quantidadeDepreciada;
                    _depreciacaoMensagem.Erro = false;
                    retornoDepreciacoes.Add(_depreciacaoMensagem);
                    var _retorno = retornoDepreciacoes.OrderBy(x => x.Erro);
                    return Json(new { data = _retorno, mensagem = "Bem patrimonial não foi depreciado." }, JsonRequestBehavior.AllowGet);
                }
                _depreciacaoMensagem = new DepreciacaoMensagemViewModel();
                var retorno = retornoDepreciacoes.OrderBy(x => x.Erro);
                _depreciacaoMensagem.Mensagem = "realizado com sucesso!";
                _depreciacaoMensagem.quantidadeDepreciada = quantidadeDepreciada;
                _depreciacaoMensagem.Erro = false;
                retornoDepreciacoes.Add(_depreciacaoMensagem);

                if (depreciouUgeinteira)
                {
                    if (!db.UGEDepreciaramAbrilDoisMilVintes.Where(u => u.ManagerUnitId == managerUnit.Id).Any())
                    {
                        UGEDepreciaramAbrilDoisMilVinte uge = new UGEDepreciaramAbrilDoisMilVinte();
                        uge.ManagerUnitId = managerUnit.Id;
                        db.UGEDepreciaramAbrilDoisMilVintes.Add(uge);
                        db.SaveChanges();
                    }
                }

                return Json(new { data = retorno, mensagem = "Verificar depreciação." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = "", mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            finally
            {
                dataInicio = DateTime.Now;
            }
        }

        [HttpPost]
        public async Task<JsonResult> Depreciar_Backup(ManagerUnitDepreciationModel model)
        {
          
            string _mensagemParaUsuario = string.Empty;
            IList<DepreciacaoMensagemViewModel> retornoDepreciacoes = new List<DepreciacaoMensagemViewModel>();
            DateTime dataInicio;
            try
            {
                System.Console.Out.Write("Inicio");
                System.Console.Out.Write(DateTime.Now.ToString());
                var managerUnit = (from m in db.ManagerUnits
                                   where m.Id == model.ManagerUnitId
                                   select m).AsNoTracking().FirstOrDefault();

                dataInicio = DateTime.Now;

                if (!managerUnit.ManagmentUnit_YearMonthReference.Equals("202004")) {
                    return Json(new { data = "", erro = "UGE precisa estar com mês de referência em abril/2020" }, JsonRequestBehavior.AllowGet);
                }

                int ano = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(0, 4));
                int mes = Convert.ToInt32(managerUnit.ManagmentUnit_YearMonthReference.Substring(4, 2));
               
                DateTime mesAnoReferencia = new DateTime(ano, mes, 1);
                DateTime mesAnoReferenciaVerificar = mesAnoReferencia.AddMonths(-1);

                string Retorno = string.Empty;

               


                _mensagemParaUsuario = "Fechamento realizado com sucesso";
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.LazyLoadingEnabled = false;
                db.Configuration.ProxyCreationEnabled = false;

                IQueryable<DepreciacaoParametroModel> _query;
                bool depreciouUgeinteira = false;

                if (!string.IsNullOrEmpty(model.NumberIdentification) && model.MaterialItemCode.HasValue) {
                    _query = (from a in db.Assets
                              join m in db.AssetMovements on a.Id equals m.AssetId
                              where a.MaterialGroupCode != 0
                                 && a.ManagerUnitId == model.ManagerUnitId
                                 && a.flagVerificado == null
                                 && a.flagDepreciaAcumulada == 1
                                 && a.NumberIdentification.Equals(model.NumberIdentification)
                                 && a.MaterialItemCode == model.MaterialItemCode
                                 && (a.flagAcervo == null || a.flagAcervo == false)
                                 && (a.flagTerceiro == null || a.flagTerceiro == false)
                                 && (m.FlagEstorno == null || m.FlagEstorno == false)

                              select new DepreciacaoParametroModel()
                              {
                                  MaterialItemCode = a.MaterialItemCode,
                                  AssetStartId = a.AssetStartId,
                                  AssetId = a.Id
                              }).Distinct();
                }
                else if (!string.IsNullOrEmpty(model.NumberIdentification) && !model.MaterialItemCode.HasValue)
                {
                    _query = (from a in db.Assets
                              join m in db.AssetMovements on a.Id equals m.AssetId
                              where a.MaterialGroupCode != 0
                                 && a.ManagerUnitId == model.ManagerUnitId
                                 && a.flagVerificado == null
                                 && a.flagDepreciaAcumulada == 1
                                 && a.NumberIdentification.Equals(model.NumberIdentification)
                                 && (a.flagAcervo == null || a.flagAcervo == false)
                                 && (a.flagTerceiro == null || a.flagTerceiro == false)
                                 && (m.FlagEstorno == null || m.FlagEstorno == false)
                              select new DepreciacaoParametroModel()
                              {
                                  MaterialItemCode = a.MaterialItemCode,
                                  AssetStartId = a.AssetStartId,
                                  AssetId = a.Id
                              }).Distinct();
                }
                else if(model.MaterialItemCode.HasValue)
                {
                    _query = (from a in db.Assets
                              join m in db.AssetMovements on a.Id equals m.AssetId
                              where a.MaterialGroupCode != 0
                                 && a.ManagerUnitId == model.ManagerUnitId
                                 && a.flagVerificado == null
                                 && a.flagDepreciaAcumulada == 1
                                 && a.MaterialItemCode == model.MaterialItemCode.Value
                                 && (a.flagAcervo == null || a.flagAcervo == false)
                                 && (a.flagTerceiro == null || a.flagTerceiro == false)
                                 && (m.FlagEstorno == null || m.FlagEstorno == false)

                              select new DepreciacaoParametroModel()
                              {
                                  MaterialItemCode = a.MaterialItemCode,
                                  AssetStartId = a.AssetStartId,
                                  AssetId = a.Id
                              }).Distinct();
                }
                else if(model.BookAccount.HasValue)
                {
                    _query = (from a in db.Assets
                              join m in db.AssetMovements on a.Id equals m.AssetId
                              join b in db.AuxiliaryAccounts on m.AuxiliaryAccountId equals b.Id
                              where a.MaterialGroupCode != 0
                                 && a.ManagerUnitId == model.ManagerUnitId
                                 && a.flagVerificado == null
                                 && a.flagDepreciaAcumulada == 1
                                 && (a.flagAcervo == null || a.flagAcervo == false)
                                 && (a.flagTerceiro == null || a.flagTerceiro == false)
                                 && (m.FlagEstorno == null || m.FlagEstorno == false)
                                 && b.BookAccount == model.BookAccount.Value
                              select new DepreciacaoParametroModel()
                              {
                                  MaterialItemCode = a.MaterialItemCode,
                                  AssetStartId = a.AssetStartId,
                                  AssetId = a.Id
                              }).Distinct();
                }
                else
                {
                    _query = (from a in db.Assets
                              join m in db.AssetMovements on a.Id equals m.AssetId
                              where a.MaterialGroupCode != 0
                                 && a.ManagerUnitId == model.ManagerUnitId
                                 && a.flagVerificado == null
                                 && a.flagDepreciaAcumulada == 1
                                 && (a.flagAcervo == null || a.flagAcervo == false)
                                 && (a.flagTerceiro == null || a.flagTerceiro == false)
                                 && (m.FlagEstorno == null || m.FlagEstorno == false)
                              select new DepreciacaoParametroModel()
                              {
                                  MaterialItemCode = a.MaterialItemCode,
                                  AssetStartId = a.AssetStartId,
                                  AssetId = a.Id
                              }).Distinct();

                    depreciouUgeinteira = true;
                }



                var depreciacoes = _query.ToList();
                int quantidadeDepreciada = 0;
                bool _transferencia = false;

                foreach (var depreciacao in depreciacoes)
                {
                    _transferencia = false;

                    int _assetStartId = 0;
                   
                   
                   
                    var transferencia = await (from t in db.AssetMovements where t.AssetTransferenciaId == depreciacao.AssetId orderby t.Id descending select t).FirstOrDefaultAsync();


                    if (transferencia != null)
                    {
                        _transferencia = true;

                        _assetStartId = transferencia.AssetId;
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[Asset] SET [AssetStartId] =" + _assetStartId.ToString() + " WHERE [Id] =" + transferencia.AssetId.ToString());
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] =" + _assetStartId.ToString() + " WHERE [AssetId] =" + transferencia.AssetId.ToString());

                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[Asset] SET [AssetStartId] =" + _assetStartId.ToString() + " WHERE [Id] =" + depreciacao.AssetId.ToString());
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] =" + _assetStartId.ToString() + " WHERE [AssetId] =" + depreciacao.AssetId.ToString());

                        SqlParameter[] parametrosTransferencia =
                       {
                            new SqlParameter("@ManagerUnitId", transferencia.ManagerUnitId),
                            new SqlParameter("@MaterialItemCode", depreciacao.MaterialItemCode),
                            new SqlParameter("@AssetStartId", _assetStartId),
                            new SqlParameter("@DataFinal", mesAnoReferenciaVerificar),
                            new SqlParameter("@Fechamento", false),
                            new SqlParameter("@Retorno",System.Data.SqlDbType.VarChar,1000) { Direction= System.Data.ParameterDirection.Output },
                            new SqlParameter("@Erro",System.Data.SqlDbType.Bit) { Direction= System.Data.ParameterDirection.Output }

                        };
                        var registro = this.db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[MonthlyDepreciation] WHERE  [MaterialItemCode] = " + depreciacao.MaterialItemCode.ToString() + "  AND [AssetStartId] = " + _assetStartId.ToString());
                        var resultadoTransferencia = this.db.Database.ExecuteSqlCommand("EXEC [dbo].[SAM_DEPRECIACAO_UGE] @ManagerUnitId,@MaterialItemCode,@AssetStartId,@DataFinal,@Fechamento,@Retorno OUT, @Erro OUT", parametrosTransferencia);
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[Asset] SET [AssetStartId] =" + depreciacao.AssetId.ToString() + " WHERE [Id] =" + depreciacao.AssetId.ToString());
                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] =" + depreciacao.AssetId.ToString() + " WHERE [AssetId] =" + depreciacao.AssetId.ToString());

                        _assetStartId = depreciacao.AssetId;
                    }

                 


                    SqlParameter[] parametros =
                    {
                         new SqlParameter("@ManagerUnitId", model.ManagerUnitId),
                                         new SqlParameter("@MaterialItemCode", depreciacao.MaterialItemCode),
                                         new SqlParameter("@AssetStartId", _assetStartId),
                                         new SqlParameter("@DataFinal", mesAnoReferenciaVerificar),
                                         new SqlParameter("@Fechamento", false),
                                         new SqlParameter("@Retorno",System.Data.SqlDbType.VarChar,1000) { Direction= System.Data.ParameterDirection.Output },
                                         new SqlParameter("@Erro",System.Data.SqlDbType.Bit) { Direction= System.Data.ParameterDirection.Output }

                    };

                    var _registro = this.db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = " + model.ManagerUnitId.ToString() + " AND [MaterialItemCode] = " + depreciacao.MaterialItemCode.ToString() + "  AND [AssetStartId] = " + _assetStartId.ToString());

                  
                    


                    var resultado = this.db.Database.ExecuteSqlCommand("EXEC [dbo].[SAM_DEPRECIACAO_UGE] @ManagerUnitId,@MaterialItemCode,@AssetStartId,@DataFinal,@Fechamento,@Retorno OUT,@Erro OUT", parametros);
                    
                    //verifica se já existe uma depreciação realizada no destino
                    if (_transferencia == false)
                    {
                        IQueryable<MonthlyDepreciation> query = (from m in db.MonthlyDepreciations where m.AssetStartId == _assetStartId && m.MaterialItemCode == depreciacao.MaterialItemCode && m.ManagerUnitId != model.ManagerUnitId select m);

                        var managerUnitExiste = await query.ToListAsync();

                        foreach (var _managerUnit in managerUnitExiste)
                        {
                            SqlParameter[] _parametrosOrigem =
                               {
                                     new SqlParameter("@ManagerUnitId", model.ManagerUnitId),
                                                     new SqlParameter("@MaterialItemCode", depreciacao.MaterialItemCode),
                                                     new SqlParameter("@AssetStartId", _assetStartId),
                                                     new SqlParameter("@DataFinal", mesAnoReferenciaVerificar),
                                                     new SqlParameter("@Fechamento", false),
                                                     new SqlParameter("@Retorno",System.Data.SqlDbType.VarChar,1000) { Direction= System.Data.ParameterDirection.Output },
                                                     new SqlParameter("@Erro",System.Data.SqlDbType.Bit) { Direction= System.Data.ParameterDirection.Output }

                                };
                            SqlParameter[] _parametrosDestino =
                              {
                                    new SqlParameter("@ManagerUnitId", _managerUnit.ManagerUnitId),
                                    new SqlParameter("@MaterialItemCode", depreciacao.MaterialItemCode),
                                    new SqlParameter("@AssetStartId", _assetStartId),
                                    new SqlParameter("@DataFinal", mesAnoReferenciaVerificar),
                                    new SqlParameter("@Fechamento", false),
                                    new SqlParameter("@Retorno",System.Data.SqlDbType.VarChar,1000) { Direction= System.Data.ParameterDirection.Output },
                                    new SqlParameter("@Erro",System.Data.SqlDbType.Bit) { Direction= System.Data.ParameterDirection.Output }

                                };
                            var _registroManager = this.db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = " + _managerUnit.ManagerUnitId.ToString() + " AND [MaterialItemCode] = " + depreciacao.MaterialItemCode.ToString() + "  AND [AssetStartId] = " + _assetStartId.ToString());
                            
                            var resultadoOrigem = this.db.Database.ExecuteSqlCommand("EXEC [dbo].[SAM_DEPRECIACAO_UGE] @ManagerUnitId,@MaterialItemCode,@AssetStartId,@DataFinal,@Fechamento,@Retorno OUT,@Erro OUT", _parametrosOrigem);
                            var _resultadoDestino = this.db.Database.ExecuteSqlCommand("EXEC [dbo].[SAM_DEPRECIACAO_UGE] @ManagerUnitId,@MaterialItemCode,@AssetStartId,@DataFinal,@Fechamento,@Retorno OUT, @Erro OUT", _parametrosDestino);

                        }
                    }

                    var depreciacaoMensagem = new DepreciacaoMensagemViewModel();

                    var erro = bool.Parse(parametros[6].Value.ToString());
                    if (erro)
                    {
                        depreciacaoMensagem.AssetId = depreciacao.AssetId;
                        depreciacaoMensagem.AssetStartId = _assetStartId;
                        depreciacaoMensagem.MaterialItemCode = depreciacao.MaterialItemCode;
                        
                        var Bp = (from ben in db.Assets where ben.Id == depreciacao.AssetId select ben).AsNoTracking().FirstOrDefault();

                        depreciacaoMensagem.NumberIdentification = Bp.NumberIdentification;
             
                        if (Bp.AcquisitionDate.Year < 1900)
                        {
                            depreciacaoMensagem.Tipo = "aquisicao";
                            depreciacaoMensagem.AcquisitionDate = Bp.AcquisitionDate.ToString("dd/MM/yyyy");
                            depreciacaoMensagem.Mensagem = "Data de aquisição ínvalida, ano da data é menor que 1900.";
                        }
                           
                        else if (Bp.MovimentDate.Year < 1900)
                        {
                            depreciacaoMensagem.Tipo = "incorporacao";
                            depreciacaoMensagem.MovimentDate = Bp.MovimentDate.ToString("dd/MM/yyyy");
                            depreciacaoMensagem.Mensagem = "Data de incorporação ínvalida, ano da data é menor que 1900.";
                        }
                            
                        if (Bp.AcquisitionDate.Year < 1900 && Bp.MovimentDate.Year < 1900)
                        {
                            depreciacaoMensagem.Tipo = "ambas";
                            depreciacaoMensagem.AcquisitionDate = Bp.AcquisitionDate.ToString("dd/MM/yyyy");
                            depreciacaoMensagem.MovimentDate = Bp.MovimentDate.ToString("dd/MM/yyyy");
                            depreciacaoMensagem.Mensagem = "Datas de aquisição e incorporação ínvalidas, ano das datas é menor que 1900.";

                        }
                        else
                        {
                            depreciacaoMensagem.Mensagem = GetMensagemDataInvalida(parametros[5].Value.ToString());
                        }
                           

                        depreciacaoMensagem.Erro = erro;

                        retornoDepreciacoes.Add(depreciacaoMensagem);
                    }else
                    {
                        quantidadeDepreciada += 1;


                    }

                    

                }

                var _depreciacaoMensagem = new DepreciacaoMensagemViewModel();


                if (depreciacoes.Count < 1)
                {
                    _depreciacaoMensagem.Mensagem = "Bem patrimonial não foi depreciado.";
                    _depreciacaoMensagem.quantidadeDepreciada = quantidadeDepreciada;
                    _depreciacaoMensagem.Erro = false;
                    retornoDepreciacoes.Add(_depreciacaoMensagem);
                    var _retorno = retornoDepreciacoes.OrderBy(x => x.Erro);
                    return Json(new { data = _retorno, mensagem = "Bem patrimonial não foi depreciado." }, JsonRequestBehavior.AllowGet);
                }
                _depreciacaoMensagem = new DepreciacaoMensagemViewModel();
                var retorno = retornoDepreciacoes.OrderBy(x => x.Erro);
                _depreciacaoMensagem.Mensagem = "realizado com sucesso!";
                _depreciacaoMensagem.quantidadeDepreciada = quantidadeDepreciada;
                _depreciacaoMensagem.Erro = false;
                retornoDepreciacoes.Add(_depreciacaoMensagem);

                if (depreciouUgeinteira)
                {
                    if (!db.UGEDepreciaramAbrilDoisMilVintes.Where(u => u.ManagerUnitId == managerUnit.Id).Any())
                    {
                        UGEDepreciaramAbrilDoisMilVinte uge = new UGEDepreciaramAbrilDoisMilVinte();
                        uge.ManagerUnitId = managerUnit.Id;
                        db.UGEDepreciaramAbrilDoisMilVintes.Add(uge);
                        db.SaveChanges();
                    }
                }

                //var retorno = retornoDepreciacoes.OrderBy(x => x.Erro);

                return Json(new { data = retorno, mensagem="Verificar depreciação." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = "", mensagem = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            finally
            {
                System.Console.Out.Write("Final");
                System.Console.Out.Write(DateTime.Now.ToString());
                dataInicio = DateTime.Now;
            }
        }
        private string GetMensagemDataInvalida(string mensagem)
        {
			if (mensagem.Contains("dados date em um tipo de dados datetime") || mensagem.Contains("date data type to a datetime data type"))
                return "Data de aquisição inválida, ano da data é menor que 1900. ";

            return mensagem;
        }
    }
}