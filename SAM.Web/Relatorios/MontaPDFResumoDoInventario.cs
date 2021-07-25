using Microsoft.Reporting.WebForms;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SAM.Web.Relatorios
{
    public class MontaPDFResumoDoInventario
    {
        public string mimeType;
        public byte[] renderedBytes;
        public MontaPDFResumoDoInventario(ReportResumoInventarioViewModel viewModel, DataTable[] dtDadosRelatorio, string UGE, string MesRef, string path)
        {
            LocalReport lr = SetaPathRelatorio(path);

            List<ReportParameter> parametros = new List<ReportParameter>();
            parametros.Add(new ReportParameter("ClosingYearMonthReference", MesRef));

            parametros.Add(new ReportParameter("ManagerUnitCodeWithDescription", UGE));
            parametros.Add(new ReportParameter("ResumoConsolidado", viewModel.ResumoConsolidado.ToString()));
            parametros.Add(new ReportParameter("AquisicoesCorrentes", viewModel.AquisicoesCorrentes.ToString()));
            parametros.Add(new ReportParameter("BPTotalDepreciados", viewModel.BPTotalDepreciados.ToString()));
            parametros.Add(new ReportParameter("BaixasCorrentes", viewModel.BaixasCorrentes.ToString()));
            parametros.Add(new ReportParameter("Acervos", viewModel.Acervos.ToString()));
            parametros.Add(new ReportParameter("Terceiros", viewModel.Terceiros.ToString()));
            lr.SetParameters(parametros);

            foreach (var dtDadosRelatorioParcial in dtDadosRelatorio)
                lr.DataSources.Add(new ReportDataSource(dtDadosRelatorioParcial.TableName, dtDadosRelatorioParcial));

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

            //Render the report
            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
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
    }
}