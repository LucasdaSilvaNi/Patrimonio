using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using PatrimonioBusiness.excel.abstrcts;
using PatrimonioBusiness.visaogeral.entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PatrimonioBusiness.excel.exportacao
{
    internal class Exportar: ExportarAbstract
    {
        public override void ExportExcel(IList<VisaoGeral> visaoGerals , string destination)
        {
            //Row linha;
            //try
            //{
            //    var columns = visaoGerals.GetType().GetProperties().Select(k => k.Name).ToList();

            //    using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            //    {
            //        var workbookPart = workbook.AddWorkbookPart();

            //        workbook.WorkbookPart.Workbook = new Workbook();

            //        workbook.WorkbookPart.Workbook.Sheets = new Sheets();

            //        // Adding style
            //        WorkbookStylesPart stylePart = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            //        stylePart.Stylesheet = GenerateStylesheet();
            //        stylePart.Stylesheet.Save();
                   
            //            try
            //            {

            //                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();

            //                var sheetData = new SheetData();
            //                sheetPart.Worksheet = new Worksheet(sheetData);

            //                Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
            //                string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

            //                uint sheetId = 1;
            //                if (sheets.Elements<Sheet>().Count() > 0)
            //                {
            //                    sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            //                }

            //                Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = "VisaoGeral" };
            //                sheets.Append(sheet);

            //                Row headerRow = new Row();

            //                //List<String> _columns = new List<string>();
            //                foreach (string column in columns)
            //                {


            //                   // columns.Add(_column);

            //                    Cell cell = new Cell();
            //                    if (column.Equals("AssetId") || column.Equals("Item") || column.Equals("MovementTypeId") || column.Equals("LifeCycle"))
            //                        cell.DataType = CellValues.Number;
            //                    else
            //                        cell.DataType = CellValues.String;


            //                    cell.CellValue = new CellValue(column);

            //                    if (column.Equals("AssetId") || column.Equals("Chapa") || column.Equals("Item"))
            //                        headerRow.AppendChild(ConstructCell(column, CellValues.String, 3));
            //                    else if (column.Equals("ValorDeAquisicao") || column.Equals("ValorAtual") || column.Equals("DepreciacaoAcumulada") || column.Equals("DepreciacaoMensal"))
            //                        headerRow.AppendChild(ConstructCell(column, CellValues.String, 7));
            //                    else if (column.Equals("DescriçãoDoItem") || column.Equals("Orgao") || column.Equals("UO") || column.Equals("UGE") || column.Equals("UA") || column.Equals("DescricaoDaDivisao"))
            //                        headerRow.AppendChild(ConstructCell(column, CellValues.String, 6));

            //                    else
            //                        headerRow.AppendChild(ConstructCell(column, CellValues.String, 9));
            //                }


            //                sheetData.AppendChild(headerRow);

            //                foreach (var visaoGeral in visaoGerals)
            //                {
                               
            //                    Row newRow = new Row();
            //                    foreach (String col in columns)
            //                    {

            //                        Cell cell = new Cell();

            //                        try
            //                        {
            //                            string result = "";

            //                            cell.DataType = CellValues.String;
            //                            string pattern = @"(?i)[^0-9a-záéíóúàèìòùâêîôûãõç\s]";
            //                            string replacement = "";
            //                            Regex rgx = new Regex(pattern);
            //                            if (col.Equals("ValorDeAquisicao") || col.Equals("ValorAtual") || col.Equals("DepreciacaoAcumulada") || col.Equals("DepreciacaoMensal"))
            //                            {
            //                                result = decimal.Round(valor, 2, MidpointRounding.AwayFromZero).ToString();
            //                            }


            //                        decimal valor;
            //                            bool isValor = decimal.TryParse(dsrow[col].ToString(), out valor);
            //                            if (isValor)
                                            

            //                                else
            //                                    result = valor.ToString();
            //                            else
            //                                result = rgx.Replace(dsrow[col].ToString(), replacement);

            //                            if (col.Equals("ValorDeAquisicao") || col.Equals("ValorAtual") || col.Equals("DepreciacaoAcumulada") || col.Equals("DepreciacaoMensal"))
            //                                newRow.AppendChild(ConstructCell(result.Replace(",", "."), CellValues.Number, 1));
            //                            else
            //                                newRow.AppendChild(ConstructCell(result, CellValues.String, 1));

            //                        }
            //                        catch (Exception ex)
            //                        {


            //                        }

            //                    }
            //                    try
            //                    {
            //                        linha = newRow;

            //                        sheetData.AppendChild(newRow);
            //                    }
            //                    catch (Exception ex)
            //                    {

            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {

            //            }

                    
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ex.Message.ToString();
            //}
        }
        public override void ExportExcel(DataTable dataTable, string destination)
        {
            Row linha;
            try
            {

                using (var workbook = SpreadsheetDocument.Create(destination, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = workbook.AddWorkbookPart();

                    workbook.WorkbookPart.Workbook = new Workbook();

                    workbook.WorkbookPart.Workbook.Sheets = new Sheets();

                    // Adding style
                    WorkbookStylesPart stylePart = workbook.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                    stylePart.Stylesheet = GenerateStylesheet();
                    stylePart.Stylesheet.Save();

                    try
                    {

                        var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();

                        var sheetData = new SheetData();
                        sheetPart.Worksheet = new Worksheet(sheetData);

                        Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>();
                        string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                        uint sheetId = 1;
                        if (sheets.Elements<Sheet>().Count() > 0)
                        {
                            sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                        }

                        Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = dataTable.TableName };
                        sheets.Append(sheet);

                        Row headerRow = new Row();

                        List<String> columns = new List<string>();
                        foreach (DataColumn column in dataTable.Columns)
                        {


                            columns.Add(column.ColumnName);

                            Cell cell = new Cell();
                            if (column.Equals("AssetId") || column.Equals("Item") || column.Equals("MovementTypeId") || column.Equals("LifeCycle"))
                                cell.DataType = CellValues.Number;
                            else
                                cell.DataType = CellValues.String;


                            cell.CellValue = new CellValue(column.ColumnName);

                            if (column.ColumnName.Equals("AssetId") || column.ColumnName.Equals("Chapa") || column.ColumnName.Equals("Item"))
                                headerRow.AppendChild(ConstructCell(column.ColumnName, CellValues.String, 2));
                            else if (column.ColumnName.Equals("ValorDeAquisicao") || column.ColumnName.Equals("ValorAtual") || column.ColumnName.Equals("DepreciacaoAcumulada") || column.ColumnName.Equals("DepreciacaoMensal"))
                                headerRow.AppendChild(ConstructCell(column.ColumnName.Replace("ValorDeAquisicao", "Valor de Aquisição").Replace("ValorAtual", "Valor Atual").Replace("DepreciacaoAcumulada", "Depreciação Acumulada").Replace("DepreciacaoMensal", "Depreciação Mensal"), CellValues.String, 2));
                            else if (column.ColumnName.Equals("DescricaoDoItem") || column.ColumnName.Equals("Orgao") || column.ColumnName.Equals("UO") || column.ColumnName.Equals("UGE") || column.ColumnName.Equals("UA") || column.ColumnName.Equals("DescricaoDaDivisao"))
                                headerRow.AppendChild(ConstructCell(column.ColumnName.Replace("DescricaoDoItem", "Descrição do Item").Replace("DescricaoDaDivisao", "Descrição da Divisão").Replace("Orgao", "Orgão"), CellValues.String, 2));

                            else
                                headerRow.AppendChild(ConstructCell(column.ColumnName.Replace("TipoBp", "Tipo").Replace("GrupoItem", "Grupo do Material").Replace("DivisaoCode", "Código da Divisão").Replace("Responsavel", "Responsável").Replace("ContaContabil", "Conta Contábil")
                                                                                     .Replace("VidaUtil", "Vida Útil(Meses)").Replace("NumeroDocumento", "Número de Documento").Replace("DataUltimoHistorico", "Data do Último Histórico").Replace("DataAquisicao", "Data de Aquisição")
                                                                                     .Replace("UltimoHistorico", "Último Histórico").Replace("DataIncorporacao", "Data de Incorporação")
                                                                                     , CellValues.String, 2));
                        }


                        sheetData.AppendChild(headerRow);

                        foreach (DataRow dsrow in dataTable.Rows)
                        {
                            Row newRow = new Row();
                            foreach (String col in columns)
                            {
                                Cell cell = new Cell();

                                try
                                {
                                    string result = "";

                                    cell.DataType = CellValues.String;
                                    string pattern = @"(?i)[^0-9a-záéíóúàèìòùâêîôûãõç\s]";
                                    string replacement = "";
                                    Regex rgx = new Regex(pattern);

                                    decimal valor;
                                    bool isValor = decimal.TryParse(dsrow[col].ToString(), out valor);
                                    if (isValor)
                                        if (col.Equals("ValorDeAquisicao") || col.Equals("ValorAtual") || col.Equals("DepreciacaoAcumulada") || col.Equals("DepreciacaoMensal"))
                                        {
                                            result = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", valor).Replace(" ", "");
                                        }
                                        else
                                            result = valor.ToString();

                                    else if (col.Equals("ValorDeAquisicao") || col.Equals("ValorAtual") || col.Equals("DepreciacaoAcumulada") || col.Equals("DepreciacaoMensal"))
                                    {
                                        result = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}",new Decimal(0.0)).Replace(" ", "");
                                    }
                                    else
                                    {
                                        result = dsrow[col].ToString();
                                    }

                                    if (!col.Equals("VidaUtil") && isValor == false)
                                    {
                                        result = rgx.Replace(dsrow[col].ToString(), replacement);
                                    }

                                    switch (col) {
                                        case "ValorDeAquisicao":
                                        case "DepreciacaoAcumulada":
                                        case "DepreciacaoMensal":
                                            newRow.AppendChild(ConstructCell(result, CellValues.String, 1));
                                            break;
                                        case "ValorAtual":
                                            if (dsrow["VidaUtil"].ToString().Equals("0/0"))
                                            {
                                                result = dsrow["ValorDeAquisicao"].ToString();
                                            }
                                            newRow.AppendChild(ConstructCell(result, CellValues.String, 1));
                                            break;
                                        case "Status":
                                            newRow.AppendChild(ConstructCell(result.Replace("True", "Ativo").Replace("False", "Baixados"), CellValues.String, 1));
                                            break;
                                        case "DataUltimoHistorico":
                                            //para a data aparecer no formato. (testando com o excel 2007)
                                            result = " " + dsrow["DataUltimoHistorico"].ToString();
                                            newRow.AppendChild(ConstructCell(result, CellValues.String, 1));
                                            break;
                                        case "DataAquisicao":
                                            //para a data aparecer no formato. (testando com o excel 2007)
                                            result = " " + dsrow["DataAquisicao"].ToString();
                                            newRow.AppendChild(ConstructCell(result, CellValues.String, 1));
                                            break;
                                        case "DataIncorporacao":
                                            //para a data aparecer no formato. (testando com o excel 2007)
                                            result = " " + dsrow["DataIncorporacao"].ToString();
                                            newRow.AppendChild(ConstructCell(result, CellValues.String, 1));
                                            break;
                                        default:
                                            newRow.AppendChild(ConstructCell(result, CellValues.String, 1));
                                            break;
                                    }

                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            try
                            {
                                linha = newRow;

                                sheetData.AppendChild(newRow);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }


                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
        private Cell ConstructCell(string value, CellValues dataType, uint styleIndex = 0)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType),
                StyleIndex = styleIndex
            };
        }

        private Stylesheet GenerateStylesheet()
        {
            Stylesheet styleSheet = null;

            Fonts fonts = new Fonts(
                new Font( // Index 0 - default
                    new FontSize() { Val = 12 }

                ),
                new Font( // Index 1 - header
                    new FontSize() { Val = 14 },
                    new Bold(),
                    new Color() { Rgb = "F8F8FF" }

                ),
                 new Font( // Index 2 - header
                    new FontSize() { Val = 14 },
                    new Bold(),
                    new Color() { Rgb = "F8F8FF" }
                ),
                 new Font( // Index 3 - header(mensagem)
                    new FontSize() { Val = 14 },
                    new Bold(),
                    new Color() { Rgb = "F8F8FF" }
                ));

            Fills fills = new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }), // Index 1 - default
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "66666666" } })
                    { PatternType = PatternValues.Solid }), // Index 2 - header
                    new Fill(new PatternFill(new ForegroundColor { Rgb = "FFFFFF" })
                    { PatternType = PatternValues.Solid }), // Index 3 - header (obrigatorio)
                     new Fill(new PatternFill(new ForegroundColor { Rgb = "4F4F4F" })
                     { PatternType = PatternValues.Solid }), // Index 4 - header hierarquia (obrigatorio)
                     new Fill(new PatternFill(new ForegroundColor { Rgb = "4F4F4F" })
                     { PatternType = PatternValues.Solid }), // Index 5 - header bem patrimonial
                       new Fill(new PatternFill(new ForegroundColor { Rgb = "4F4F4F" })
                       { PatternType = PatternValues.Solid }), // Index 6 - header complemento
                        new Fill(new PatternFill(new ForegroundColor { Rgb = "4F4F4F" })
                        { PatternType = PatternValues.Solid }) // Index 7 - header mensagem
                );

            Borders borders = new Borders(
                    new Border(), // index 0 default
                    new Border( // index 1 black border
                        new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                );

            CellFormats cellFormats = new CellFormats(
                    new CellFormat(), // default
                    new CellFormat { FontId = 0, FillId = 3, BorderId = 1, ApplyBorder = true }, //1 - body
                    new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true }, //2 - header
                    new CellFormat { FontId = 1, FillId = 3, BorderId = 1, ApplyFill = true }, //3 - header(obrigatorio)
                    new CellFormat { FontId = 1, FillId = 3, BorderId = 1, ApplyFill = true }, //4 - header(Responsavel não obrigatorio)
                    new CellFormat { FontId = 0, FillId = 3, BorderId = 1, ApplyFill = true }, //5 - header(hierarquia obrigatorio)
                    new CellFormat { FontId = 0, FillId = 3, BorderId = 1, ApplyFill = true }, //6 - header(hierarquia não obrigatorio)
                    new CellFormat { FontId = 0, FillId = 3, BorderId = 1, ApplyFill = true }, //7 - header(bem patrimonial obrigatorio)
                    new CellFormat { FontId = 0, FillId = 3, BorderId = 1, ApplyFill = true }, //8 - header(bem patrimonial não obrigatorio)
                    new CellFormat { FontId = 0, FillId = 3, BorderId = 1, ApplyFill = true }, //9 - header(complemnto)
                    new CellFormat { FontId = 0, FillId = 3, BorderId = 1, ApplyFill = true }, //10 - header(mensagem)
                    new CellFormat { FontId = 1, FillId = 3, BorderId = 1, ApplyFill = true,NumberFormatId = 4, ApplyNumberFormat= true } //11 - Moeda

                );

            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }

      
        
    }

}
