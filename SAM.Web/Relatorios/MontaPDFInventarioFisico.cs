using Microsoft.Reporting.WebForms;
using SAM.Web.ViewModels;
using System.Collections.Generic;
using System.Data;

namespace SAM.Web.Relatorios
{
    public class MontaPDFInventarioFisico
    {
        public string mimeType;
        public byte[] renderedBytes;
        public MontaPDFInventarioFisico(RelatorioInventarioFisicoViewModel param, DataTable dt, string mesRef, string path)
        {
            List<ReportParameter> parametros = new List<ReportParameter>();

            LocalReport lr = SetaPathRelatorio(path);

            parametros.Add(new ReportParameter("MonthReference", mesRef));
            parametros.Add(new ReportParameter("GroupingType", param.Agrupamento));
            lr.SetParameters(parametros);

            ReportDataSource rdBalance = new ReportDataSource("DataSource", dt);
            lr.DataSources.Add(rdBalance);

            string reportType = "PDF";
            string encoding;
            string fileNameExtension;

            string deviceInfo =
                "<DeviceInfo>" +
                    "  <OutputFormat>" + "PDF" + "</OutputFormat>" +
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