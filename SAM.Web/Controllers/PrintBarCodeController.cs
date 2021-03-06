using BarcodeLib;
using Microsoft.Reporting.WebForms;
using PagedList;
using SAM.Web.Common;
using SAM.Web.Common.Enum;
using SAM.Web.Models;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAM.Web.Controllers
{
    public class PrintBarCodeController : BaseController
    {
        //private SAMContext context;

        private int _institutionId;
        private int? _budgetUnitId;
        private int? _managerUnitId;

        public void getHierarquiaPerfil()
        {
            User u = UserCommon.CurrentUser();
            var perflLogado = BuscaHierarquiaPerfilLogadoPorUsuario(u.Id);
            _institutionId = perflLogado.InstitutionId;
            _budgetUnitId = perflLogado.BudgetUnitId;
            _managerUnitId = perflLogado.ManagerUnitId;
        }

        [HttpGet]
        public ActionResult ReportPrintBarCode()
        {
            try
            {
                var retorno = new PrintBarCodeViewModel();
                LoadFiltersReportPrintCode();
                return View(retorno);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        // TODO: RETIRADO APOS ALTERACAO DA TABELA ASSET E ASSETMOVEMENTS
        [HttpGet]
        public ActionResult LoadDataReportPrintBarCode(int? institutionId, int? budgetUnitId, int? managerUnitId, string contenha, int? chapaInicioRef, int? chapaFimRef)
        {
            try
            {
                List<PrintBarAssetsViewModel> listAssetsDB;

                if (!ContemValor(institutionId))
                    institutionId = 0;
                if (!ContemValor(budgetUnitId))
                    budgetUnitId = 0;
                if (!ContemValor(managerUnitId))
                    managerUnitId = 0;

                using (SAMContext context = new SAMContext())
                {
                    context.Configuration.LazyLoadingEnabled = false;

                    if (string.IsNullOrEmpty(contenha) || string.IsNullOrWhiteSpace(contenha))
                    {
                        listAssetsDB = (from a in context.Assets
                                        join am in context.AssetMovements
                                        on a.Id equals am.AssetId
                                        join i in context.Initials
                                        on a.InitialId equals i.Id
                                        where !a.flagVerificado.HasValue &&
                                              a.flagDepreciaAcumulada == 1 &&
                                              am.Status &&
                                              am.InstitutionId == (institutionId == 0 ? am.InstitutionId : institutionId) &&
                                              am.BudgetUnitId == (budgetUnitId == 0 ? am.BudgetUnitId : budgetUnitId) &&
                                              am.ManagerUnitId == (managerUnitId == 0 ? am.ManagerUnitId : managerUnitId)
                                        select new PrintBarAssetsViewModel
                                        {
                                            Id = a.Id,
                                            Sigla = i.Name,
                                            Chapa = a.NumberIdentification,
                                            ChapaCompleta = a.ChapaCompleta,
                                            MaterialItemDescription = a.MaterialItemDescription
                                        }).Distinct().AsNoTracking().ToList();
                    }
                    else
                    {
                        listAssetsDB = (from a in context.Assets
                                        join am in context.AssetMovements
                                        on a.Id equals am.AssetId
                                        join i in context.Initials
                                        on a.InitialId equals i.Id
                                        where !a.flagVerificado.HasValue &&
                                              a.flagDepreciaAcumulada == 1 &&
                                              am.Status &&
                                              am.InstitutionId == (institutionId == 0 ? am.InstitutionId : institutionId) &&
                                              am.BudgetUnitId == (budgetUnitId == 0 ? am.BudgetUnitId : budgetUnitId) &&
                                              am.ManagerUnitId == (managerUnitId == 0 ? am.ManagerUnitId : managerUnitId) &&
                                              (a.InitialName.Contains(contenha) ||
                                              a.ChapaCompleta.Contains(contenha) ||
                                              a.MaterialItemDescription.Contains(contenha))
                                        select new PrintBarAssetsViewModel
                                        {
                                            Id = a.Id,
                                            Sigla = i.Name,
                                            Chapa = a.NumberIdentification,
                                            ChapaCompleta = a.ChapaCompleta,
                                            MaterialItemDescription = a.MaterialItemDescription
                                        }).Distinct().AsNoTracking().ToList();
                    }
                }

                long paraValidacao = 0;

                if (ContemValor(chapaInicioRef))
                {
                    listAssetsDB = listAssetsDB.Where(a => long.TryParse(a.Chapa, out paraValidacao) ? long.Parse(a.Chapa) >= (int)chapaInicioRef : a.Chapa.Contains(chapaInicioRef.ToString()) ).ToList();
                }

                if (ContemValor(chapaFimRef))
                {
                    listAssetsDB = listAssetsDB.Where(a => long.TryParse(a.Chapa, out paraValidacao) ? long.Parse(a.Chapa) <= (int)chapaFimRef : a.Chapa.Contains(chapaInicioRef.ToString())).ToList();
                }

                listAssetsDB = listAssetsDB.OrderBy(a => long.TryParse(a.Chapa, out paraValidacao) ? long.Parse(a.Chapa) : 99999999999).ToList();

                var oPrintBarCode = new PrintBarCodeViewModel()
                {
                    ManagerUnitId = managerUnitId,
                    ListAssets = listAssetsDB
                };

                return PartialView("_dataReportPrintBarCode", oPrintBarCode);
            }
            catch (Exception ex)
            {
                return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        [HttpPost]
        public void GenerateReportPrintBarCode([Bind(Include = "ManagerUnitId,MesAnoEntradaRef,ListAssetsSelected,TypeReading,ColorPlate")] PrintBarCodeViewModel objPrintBarCode)
        {
            try
            {
                string path = PathDoTipoDeRelatorio(objPrintBarCode.TypeReading);
                LocalReport lr = SetaPathRelatorio(path);
                SetaParametrosRelatorio(lr, objPrintBarCode.ColorPlate);

                DataTable dtBarCodes = RetornaDadosRelatorio(objPrintBarCode);

                if (objPrintBarCode.TypeReading == (short)EnumReportBarCodeTypes.CodigoDeBarras)
                    MontaCodigoDeBarras(dtBarCodes);
                else
                    MontaQrCodes(dtBarCodes);

                ReportDataSource rdBalance = new ReportDataSource("DataSource", dtBarCodes);
                lr.DataSources.Add(rdBalance);

                string data = DateTime.Now.ToString("yyyyMMddHHmmss");

                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;

                string deviceInfo =
                    "<DeviceInfo>" +
                        "  <OutputFormat>PDF</OutputFormat>" +
                    "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;


                //Render the report
                renderedBytes = lr.Render(
                    reportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment; filename=RelatorioCodigo" + data + ".pdf");
                Response.BinaryWrite(renderedBytes); // create the file
                Response.Flush(); // send it to the client to download
            }
            catch (Exception ex)
            {
                // return MensagemErro(CommonMensagens.PadraoException, ex);
            }
        }

        #region Métodos Privados

        private void LoadFiltersReportPrintCode()
        {
            CarregaHierarquia();
        }
        private bool ContemValor(int? variavel)
        {
            bool retorno = false;
            if (variavel.HasValue && variavel != null && variavel != 0)
                retorno = true;
            return retorno;
        }

        private string PathDoTipoDeRelatorio(short typeReading)
        {
            string path = string.Empty;

            if (typeReading == (int)EnumReportBarCodeTypes.CodigoDeBarras)
                path = Path.Combine(Server.MapPath("~/Report"), "PrintBarCode.rdlc");
            else
                path = Path.Combine(Server.MapPath("~/Report"), "PrintQrCode.rdlc");

            return path;
        }
        private LocalReport SetaPathRelatorio(string path)
        {
            LocalReport lr = new LocalReport();
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }

            return lr;
        }
        private DataTable RetornaDadosRelatorio(PrintBarCodeViewModel objPrintBarCode)
        {
            List<int> listAssertsSelected = objPrintBarCode.ListAssetsSelected != null ?
                                            Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(objPrintBarCode.ListAssetsSelected) :
                                            new List<int>();

            string spName = "REPORT_PRINT_BAR_CODE_OR_QRCODE";

            List<ListaParamentros> listaParam = new List<ListaParamentros>();
            listaParam.Add(new ListaParamentros { nomeParametro = "idUge", valor = objPrintBarCode.ManagerUnitId });
            listaParam.Add(new ListaParamentros { nomeParametro = "assertsSelecteds", valor = string.Join(",", listAssertsSelected) });

            FunctionsCommon common = new FunctionsCommon();
            DataTable dtBarCodes = common.ReturnDataFromStoredProcedureReport(listaParam, spName);

            return dtBarCodes;
        }
        private void MontaCodigoDeBarras(DataTable dtBarCodes)
        {
            var objBarCode = new Barcode();

            foreach (DataRow barCode in dtBarCodes.Rows)
            {
                objBarCode.Encode(TYPE.CODE128, barCode["ChapaCompleta"].ToString(), 300, 150);
                barCode["BarCode"] = objBarCode.GetImageData(BarcodeLib.SaveTypes.JPG);
            }
        }
        private void MontaQrCodes(DataTable dtBarCodes)
        {
            var objQRCode = new QrCode();

            foreach (DataRow barCode in dtBarCodes.Rows)
            {
                objQRCode.GerarQRCode(300, 150, barCode["ChapaCompleta"].ToString() + ";" + barCode["ShortDescription"].ToString() + ";" + barCode["Description_UGE"].ToString());
                barCode["BarCode"] = objQRCode.GetImageData(SAM.Web.Common.Enum.SaveTypes.JPG);
            }
        }
        private void CarregaHierarquia()
        {
            Hierarquia hierarquia = new Hierarquia();

            if (PerfilAdmGeral() == true)
            {
                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(null), "Id", "Description");
                ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(null), "Id", "Description");
                ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(null), "Id", "Description");
            }
            else
            {
                getHierarquiaPerfil();

                ViewBag.Institutions = new SelectList(hierarquia.GetOrgaos(_institutionId), "Id", "Description", _institutionId);

                if (_budgetUnitId.HasValue && _budgetUnitId != 0)
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUos(_budgetUnitId), "Id", "Description", _budgetUnitId);
                else
                    ViewBag.BudgetUnits = new SelectList(hierarquia.GetUosPorOrgaoId(_institutionId), "Id", "Description", 0);

                if (_managerUnitId.HasValue && _managerUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUges(_managerUnitId), "Id", "Description", _managerUnitId);
                else if (_budgetUnitId.HasValue && _budgetUnitId != 0)
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(_budgetUnitId), "Id", "Description", 0);
                else
                {
                    ViewBag.ManagerUnits = new SelectList(hierarquia.GetUgesPorUoId(null), "Id", "Description");
                }
            }
        }
        //public string RenderViewToString(string viewName, object model, ControllerContext controllerContext, ViewDataDictionary viewData = null, TempDataDictionary tempData = null)
        //{
        //    if (viewData == null)
        //    {
        //        viewData = new ViewDataDictionary();
        //    }
        //    if (tempData == null)
        //    {
        //        tempData = new TempDataDictionary();
        //    }

        //    // assing model to the viewdata
        //    viewData.Model = model;

        //    using (var sw = new StringWriter())
        //    {
        //        // try to find the specified view
        //        ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
        //        // create the associated context
        //        ViewContext viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, sw);
        //        // write the render view with the given context to the stringwriter
        //        viewResult.View.Render(viewContext, sw);

        //        viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
        //        return sw.GetStringBuilder().ToString();
        //    }
        //}
        private void SetaParametrosRelatorio(LocalReport lr, string colorPlateFilter)
        {
            var listParameters = new List<ReportParameter>();
            string colorPlate = string.IsNullOrEmpty(colorPlateFilter) ? "#337ab7" : colorPlateFilter;
            listParameters.Add(new ReportParameter("parColorPlate", colorPlate));
            lr.SetParameters(listParameters);
        }

        #endregion
    }
}