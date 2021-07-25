using Microsoft.Reporting.WebForms;
using SAM.Web.ViewModels;
using System.Collections.Generic;
using System.Data;

namespace SAM.Web.Relatorios
{
    public class MontaPDFTermoDeResponsabilidade
    {
        public string mimeType;
        public byte[] renderedBytes;

        public MontaPDFTermoDeResponsabilidade(MontaParametroTermoDeResponsabilidade param) {
            List<ReportParameter> parametros = new List<ReportParameter>();

            LocalReport lr = SetaPathRelatorio(param.caminhoRelatorio);

            parametros.Add(new ReportParameter("UA", param.UACompleto));
            parametros.Add(new ReportParameter("RESPONSAVEL", param.nomeResponsavel));
            parametros.Add(new ReportParameter("Desc_Orgao", param.descricaoOrgao));
            parametros.Add(new ReportParameter("Desc_UO", param.descricaoUO));
            parametros.Add(new ReportParameter("Desc_UGE", param.descricaoUGE));
            lr.SetParameters(parametros);

            ReportDataSource rdBalance = new ReportDataSource("DataSource", param.dados);
            lr.DataSources.Add(rdBalance);

            string reportType = "PDF";
            string encoding;
            string fileNameExtension;

            string deviceInfo = "<DeviceInfo><OutputFormat>PDF</OutputFormat></DeviceInfo>";

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