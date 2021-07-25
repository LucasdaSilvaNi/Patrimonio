using Microsoft.Reporting.WebForms;
using SAM.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SAM.Web.Relatorios
{
    public class MontaPDFRelatorioFechamento
    {
        public string mimeType;
        public byte[] renderedBytes;
        public MontaPDFRelatorioFechamento(DataTable dados, string UGE, string mesRef, string path, DateTime dataGeracao)
        {
            if (dataGeracao == null)
                dataGeracao = DateTime.Now;

            List <ReportParameter> parametros = new List<ReportParameter>();

            LocalReport lr = SetaPathRelatorio(path);

            int qtdBPs = 0;
            double totalAquisicao = 0;
            double totalAtual = 0;

            if (dados != null && dados.Rows.Count > 0)
            {
                var enumerable = dados.AsEnumerable();

                qtdBPs = enumerable.Select(r => r.Field<int>("ID")).Distinct().Count();

                totalAquisicao = Convert.ToDouble(dados.Compute("Sum(VALOR_AQUISICAO)", null));

                totalAtual = Convert.ToDouble(dados.Compute("Sum(VALOR_ATUAL)", null));
            }

            parametros.Add(new ReportParameter("UGE", UGE));
            parametros.Add(new ReportParameter("qtdBPs", qtdBPs.ToString()));
            parametros.Add(new ReportParameter("totalAquisicao", totalAquisicao.ToString()));
            parametros.Add(new ReportParameter("totalAtual", totalAtual.ToString()));
            parametros.Add(new ReportParameter("mesAnoReferencia", mesRef));

            parametros.Add(new ReportParameter("dataGeracao", dataGeracao.ToString("dd/MM/yyyy HH:mm")));

            lr.SetParameters(parametros);

            ReportDataSource rdBalance = new ReportDataSource("DataSource", dados);
            lr.DataSources.Add(rdBalance);


            string reportType = "PDF";
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