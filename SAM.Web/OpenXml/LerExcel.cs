using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SAM.Web.OpenXml
{
    public class LerExcel
    {
        private String getNomeDaColuna(String celulaDeReferencia)
        {
            var regx = new Regex("[A-Za-z]+");
            var match = regx.Match(celulaDeReferencia);

            return match.Value;
        }

        private int converterColunaParaNumero(string nomeDaColuna)
        {
            var alpha = new Regex("^[A-Z]+$");
            if (!alpha.IsMatch(nomeDaColuna)) throw new ArgumentException();

            char[] colunasCaracters = nomeDaColuna.ToCharArray();
            Array.Reverse(colunasCaracters);

            int converterValor = 0;
            for (int i = 0; i < colunasCaracters.Length; i++)
            {
                char caracter = colunasCaracters[i];
                int atual = i == 0 ? caracter - 65 : caracter - 64;

                converterValor += atual * (int)Math.Pow(26, i);
            }

            return converterValor;
        }

        private IEnumerator<Cell> getCelulaExcel(Row linha)
        {
            int atualCount = 0;
            foreach (Cell cell in linha.Descendants<Cell>())
            {
                string nomeDaColuna = getNomeDaColuna(cell.CellReference);
                int atualColunaIndex = converterColunaParaNumero(nomeDaColuna);

                for (; atualCount < atualColunaIndex; atualCount++)
                {
                    var celulaEmpty = new Cell()
                    {
                        DataType = null,
                        CellValue = new CellValue(String.Empty)
                    };

                    yield return celulaEmpty;
                    atualCount++;
                }
            }
        }

        private string lerCelulaExcel(Cell celula, WorkbookPart workBookPart)
        {
            var valorDaCelula = celula.CellValue;
            var text = (valorDaCelula == null) ? celula.InnerText : valorDaCelula.Text;

            if ((celula.DataType != null) && (celula.DataType == CellValues.SharedString))
            {
                text = workBookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(Convert.ToInt32(celula.CellValue.Text)).InnerText;
            }

            return (text ?? string.Empty).Trim();
        }

        public ExcelDados LerExcelDados(HttpPostedFile arquivo)
        {
            var data = new ExcelDados();

            if (arquivo.ContentLength <= 0)
            {
                data.Status.Mensagem = "Arquivo está em branco";
                return data;
            }

            if (arquivo.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                data.Status.Mensagem = "Por favor carregar um arquivo válido do excel na versão 2007 ou anterior";
                return data;
            }

            WorkbookPart workbookPart;
            List<Row> Linhas;

            try
            {
                var document = SpreadsheetDocument.Open(arquivo.InputStream, false);
                workbookPart = document.WorkbookPart;

                var sheets = workbookPart.Workbook.Descendants<Sheet>();
                var sheet = sheets.First();
                data.NomePlanilha = sheet.Name;

                var workSheet = ((WorksheetPart)workbookPart
                    .GetPartById(sheet.Id)).Worksheet;
                var columns = workSheet.Descendants<Columns>().FirstOrDefault();
                data.Colunas = columns;

                var sheetData = workSheet.Elements<SheetData>().First();
                Linhas = sheetData.Elements<Row>().ToList();
            }
            catch (Exception e)
            {
                data.Status.Mensagem = "Habilitar abertura de arquivo";
                return data;
            }

            // Read the header
            if (Linhas.Count > 0)
            {
                var linha = Linhas[0];
                var celulaRetorno = getCelulaExcel(linha);
                while (celulaRetorno.MoveNext())
                {
                    var cell = celulaRetorno.Current;
                    var text = lerCelulaExcel(cell, workbookPart).Trim();
                    data.Cabecalho.Add(text);
                }
            }

            // Read the sheet data
            if (Linhas.Count > 1)
            {
                for (var i = 1; i < Linhas.Count; i++)
                {
                    var dataLinha = new List<string>();
                    data.Dados.Add(dataLinha);
                    var linha = Linhas[i];
                    var celulaRetorno = getCelulaExcel(linha);
                    while (celulaRetorno.MoveNext())
                    {
                        var cell = celulaRetorno.Current;
                        var text = lerCelulaExcel(cell, workbookPart).Trim();
                        dataLinha.Add(text);
                    }
                }
            }

            return data;
        }
    }
}