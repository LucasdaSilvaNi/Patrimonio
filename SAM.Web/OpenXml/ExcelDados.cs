using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAM.Web.OpenXml
{
    public class ExcelDados
    {
        public ExcelStatus Status { get; set; }
        public Columns Colunas { get; set; }
        public List<String> Cabecalho { get; set; }
        public List<List<String>> Dados { get; set; }
        public String NomePlanilha { get; set; }

        public ExcelDados()
        {
            Status = new ExcelStatus();
            Cabecalho = new List<string>();
            Dados = new List<List<string>>();
        }
    }
}