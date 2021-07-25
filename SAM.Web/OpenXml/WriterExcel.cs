using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SAM.Web.OpenXml
{
    public class WriterExcel
    {
        private String atualColuna(int coluna)
        {
            var intProxima = ((coluna) / 676) + 64;
            var intSecunda = ((coluna % 676) / 26) + 64;
            var intTerceira = (coluna % 26) + 65;

            var proxima = (intProxima > 64)
                ? (char)intProxima : ' ';
            var segunda = (intSecunda > 64)
                ? (char)intSecunda : ' ';
            var terceira = (char)intTerceira;

            return string.Concat(proxima, segunda,
                terceira).Trim();
        }

        private Cell CreateTextoCelula(string header, UInt32 index, string text)
        {
            var cell = new Cell
            {
                DataType = CellValues.InlineString,
                CellReference = header + index
            };

            var istring = new InlineString();
            var t = new Text { Text = text };
            istring.AppendChild(t);
            cell.AppendChild(istring);
            return cell;
        }

        public byte[] GenerateExcel(ExcelDados data)
        {
            var stream = new MemoryStream();
            var document = SpreadsheetDocument
                .Create(stream, SpreadsheetDocumentType.Workbook);

            var workbookpart = document.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();
            var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();

            worksheetPart.Worksheet = new Worksheet(sheetData);

            var sheets = document.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());

            var sheet = new Sheet()
            {
                Id = document.WorkbookPart
                .GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = data.NomePlanilha ?? "Sheet 1"
            };
            sheets.AppendChild(sheet);

            // Add header
            UInt32 rowIdex = 0;
            var row = new Row { RowIndex = ++rowIdex };
            sheetData.AppendChild(row);
            var cellIdex = 0;

            foreach (var header in data.Cabecalho)
            {
                row.AppendChild(CreateTextoCelula(atualColuna(cellIdex++),
                    rowIdex, header ?? string.Empty));
            }
            if (data.Cabecalho.Count > 0)
            {
                // Add the column configuration if available
                if (data.Colunas != null)
                {
                    var columns = (Columns)data.Colunas.Clone();
                    worksheetPart.Worksheet
                        .InsertAfter(columns, worksheetPart
                        .Worksheet.SheetFormatProperties);
                }
            }

            // Add sheet data
            foreach (var rowData in data.Dados)
            {
                cellIdex = 0;
                row = new Row { RowIndex = ++rowIdex };
                sheetData.AppendChild(row);
                foreach (var callData in rowData)
                {
                    var cell = CreateTextoCelula(atualColuna(cellIdex++),
                        rowIdex, callData ?? string.Empty);
                    row.AppendChild(cell);
                }
            }

            workbookpart.Workbook.Save();
            document.Close();

            return stream.ToArray();
        }
    }
}