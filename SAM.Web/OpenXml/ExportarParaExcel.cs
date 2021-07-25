using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SAM.Web.Legado;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SAM.Web.OpenXml
{
    public class ExportarParaExcel
    {
        private IList<ChaveBemPatrimonialJson> chaves = null;

        private ExportarParaExcel()
        {

        }
        internal static ExportarParaExcel createInstance()
        {
            return new ExportarParaExcel();
        }
        internal void ExportDataSet(DataSet dataSet, string destination)
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
                    foreach (DataTable table in dataSet.Tables)
                    {
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

                            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = table.TableName };
                            sheets.Append(sheet);

                            Row headerRow = new Row();

                            List<String> columns = new List<string>();
                            foreach (DataColumn column in table.Columns)
                            {
                               
                           
                                columns.Add(column.ColumnName);

                                Cell cell = new Cell();
                                if (column.ColumnName.Equals("Data de Inclusão") || column.ColumnName.Equals("Data de Aquisição") || column.ColumnName.Equals("Data de Fabricação") || column.ColumnName.Equals("Data de Garantia"))
                                    cell.DataType = CellValues.Date;
                                else
                                    cell.DataType = CellValues.String;

                               
                                cell.CellValue = new CellValue(column.ColumnName);
                                if (column.ColumnName.Equals("Sigla") || column.ColumnName.Equals("Descrição Sigla") || column.ColumnName.Equals("Chapa") || column.ColumnName.Equals("Código Item de Material") || column.ColumnName.Equals("Descrição do Item de Material") || column.ColumnName.Equals("CPF do Responsável") || column.ColumnName.Equals("Nome do Responsável"))
                                    headerRow.AppendChild(ConstructCell(column.ColumnName, CellValues.String, 3));
                                else if(column.ColumnName.Equals("Cargo do Responsável"))
                                    headerRow.AppendChild(ConstructCell(column.ColumnName, CellValues.String, 4));
                                else if (column.ColumnName.Equals("Data de Aquisição") || column.ColumnName.Equals("Valor de Aquisição") || column.ColumnName.Equals("Data de Inclusão") || column.ColumnName.Equals("Estado de Conservação"))
                                    headerRow.AppendChild(ConstructCell(column.ColumnName, CellValues.String, 7));
                                else if (column.ColumnName.Equals("Código da UO") || column.ColumnName.Equals("Nome da UO") || column.ColumnName.Equals("Código da UGE") || column.ColumnName.Equals("Nome da UGE") || column.ColumnName.Equals("Código da Divisão") || column.ColumnName.Equals("Nome da Divisão"))
                                    headerRow.AppendChild(ConstructCell(column.ColumnName, CellValues.String, 6));
                                else if (column.ColumnName.Equals("Código do Orgão")  || column.ColumnName.Equals("Código da UA") || column.ColumnName.Equals("Nome da UA"))
                                    headerRow.AppendChild(ConstructCell(column.ColumnName, CellValues.String, 5));
                                else if (column.ColumnName.Equals("Número do Empenho") || column.ColumnName.Equals("Observações") || column.ColumnName.Equals("Descrição Adicional") || column.ColumnName.Equals("Conta Auxiliar") || column.ColumnName.Equals("Contabil Auxiliar") || column.ColumnName.Equals("Nome da Conta Auxiliar") || column.ColumnName.Equals("Número de Serie") || column.ColumnName.Equals("Data de Fabricação") || column.ColumnName.Equals("Data de Garantia") || column.ColumnName.Equals("Número do Chassi") || column.ColumnName.Equals("Marca") || column.ColumnName.Equals("Modelo") || column.ColumnName.Equals("PLACA") || column.ColumnName.Equals("Sigla Antiga") || column.ColumnName.Equals("Chapa Antiga") || column.ColumnName.Equals("Número do Empenho") || column.ColumnName.Equals("Nota Fiscal"))
                                    headerRow.AppendChild(ConstructCell(column.ColumnName, CellValues.String, 8));
                                else if (column.ColumnName.Equals("Ua Validada") || column.ColumnName.Equals("Responsavel Importado") || column.ColumnName.Equals("Responsavel Mensagem") || column.ColumnName.Equals("Divisao Importado") || column.ColumnName.Equals("Divisao Mensagem") || column.ColumnName.Equals("Sigla Importado") || column.ColumnName.Equals("Sigla Mensagem") || column.ColumnName.Equals("Terceiro Importado") || column.ColumnName.Equals("Terceiro Mensagem") || column.ColumnName.Equals("Fornecedor Importado") || column.ColumnName.Equals("Fornecedor Mensagem") || column.ColumnName.Equals("ContaAuxliar Importado") || column.ColumnName.Equals("ContaAuxiliar Mensagem") || column.ColumnName.Equals("Descricao Importado") || column.ColumnName.Equals("Chapa Antiga") || column.ColumnName.Equals("Descricao Mensagem") || column.ColumnName.Equals("BemPatrimonial Importado") || column.ColumnName.Equals("BemPatrimonial Mensagem"))
                                    headerRow.AppendChild(ConstructCell(column.ColumnName, CellValues.String, 10));
                                else
                                    headerRow.AppendChild(ConstructCell(column.ColumnName, CellValues.String, 9));
                            }


                            sheetData.AppendChild(headerRow);

                            foreach (DataRow dsrow in table.Rows)
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


                                        if (col.Equals("Data de Inclusão") || col.Equals("Data de Aquisição"))
                                        {

                                            var data = conveterDataLegadoParaData(rgx.Replace(dsrow[col].ToString(), replacement));
                                            //result = data.ToShortDateString();
                                            result = data.Year > 1900 ? data.ToShortDateString() : "Data Invalida";

                                            newRow.AppendChild(ConstructCell(result, CellValues.String, 1));
                                        }
                                        else if (col.Equals("Data de Fabricação") || col.Equals("Data de Garantia"))
                                        {
                                            result = rgx.Replace(dsrow[col].ToString(), replacement);
                                            newRow.AppendChild(ConstructCell(result, CellValues.String, 1));
                                        }
                                        else
                                        {
                                            decimal valor;
                                            bool isValor = decimal.TryParse(dsrow[col].ToString(), out valor);
                                            if (isValor)
                                                if (col.Equals("Valor de Aquisição") || col.Equals("Valor de Atualização"))
                                                {
                                                    result = decimal.Round(valor, 2, MidpointRounding.AwayFromZero).ToString(); ;
                                                }
                                                    
                                                else
                                                    result = valor.ToString();
                                            else
                                                result = rgx.Replace(dsrow[col].ToString(), replacement);

                                            if (col.Equals("Valor de Aquisição") || col.Equals("Valor de Atualização"))
                                                newRow.AppendChild(ConstructCell(result.Replace(",", "."), CellValues.Number, 1));
                                            else
                                                newRow.AppendChild(ConstructCell(result, CellValues.String, 1));
                                        }

                                       // cell.CellValue = new CellValue(result); //dsrow[col].ToString().Replace("u001", "")); //
                                      
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
                    new Color() { Rgb = "FFFFFF" }

                ),
                 new Font( // Index 2 - header
                    new FontSize() { Val = 14 },
                    new Bold(),
                    new Color() { Rgb = "FF0000" }
                ),
                 new Font( // Index 3 - header(mensagem)
                    new FontSize() { Val = 14 },
                    new Bold(),
                    new Color() { Rgb = "A9A9A9" }
                ));

            Fills fills = new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }), // Index 1 - default
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "66666666" } })
                    { PatternType = PatternValues.Solid }), // Index 2 - header
                    new Fill(new PatternFill(new ForegroundColor { Rgb = "9FE185" })
                    { PatternType = PatternValues.Solid }), // Index 3 - header (obrigatorio)
                     new Fill(new PatternFill(new ForegroundColor { Rgb = "006400" })
                     { PatternType = PatternValues.Solid }), // Index 4 - header hierarquia (obrigatorio)
                     new Fill(new PatternFill(new ForegroundColor { Rgb = "808080" })
                     { PatternType = PatternValues.Solid }), // Index 5 - header bem patrimonial
                       new Fill(new PatternFill(new ForegroundColor { Rgb = "FFD700" })
                       { PatternType = PatternValues.Solid }), // Index 6 - header complemento
                        new Fill(new PatternFill(new ForegroundColor { Rgb = "ACDAF0" })
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
                    new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true }, //1 - body
                    new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true }, //2 - header
                    new CellFormat { FontId = 2, FillId = 3, BorderId = 1, ApplyFill = true }, //3 - header(obrigatorio)
                    new CellFormat { FontId = 1, FillId = 3, BorderId = 1, ApplyFill = true }, //4 - header(Responsavel não obrigatorio)
                    new CellFormat { FontId = 2, FillId = 4, BorderId = 1, ApplyFill = true }, //5 - header(hierarquia obrigatorio)
                    new CellFormat { FontId = 1, FillId = 4, BorderId = 1, ApplyFill = true }, //6 - header(hierarquia não obrigatorio)
                    new CellFormat { FontId = 2, FillId = 5, BorderId = 1, ApplyFill = true }, //7 - header(bem patrimonial obrigatorio)
                    new CellFormat { FontId = 1, FillId = 5, BorderId = 1, ApplyFill = true }, //8 - header(bem patrimonial não obrigatorio)
                    new CellFormat { FontId = 1, FillId = 6, BorderId = 1, ApplyFill = true }, //9 - header(complemnto)
                    new CellFormat { FontId = 2, FillId = 7, BorderId = 1, ApplyFill = true } //10 - header(mensagem)
                );

            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }

        private DateTime conveterDataLegadoParaData(string dataLegado)
        {
            if (string.IsNullOrWhiteSpace(dataLegado))
                return new DateTime(1900, 1, 1);

            int ano;
            int mes;
            int dia = 1;


            if (dataLegado.Length > 8)
            {
                dataLegado = dataLegado.Split(' ')[0];
            }


            if (dataLegado.Length == 8)
            {
                //Data no padrão americano, alteramos a ordem de leitura dos campos
                ano = getNumerosNoTextoDoLegado(dataLegado.Substring(4, 4));
                if (ano < 1900)
                {
                    dia = getNumerosNoTextoDoLegado(dataLegado.Substring(6, 2));
                    mes = getNumerosNoTextoDoLegado(dataLegado.Substring(4, 2));
                    ano = getNumerosNoTextoDoLegado(dataLegado.Substring(0, 4));
                }
                else
                {
                    dia = getNumerosNoTextoDoLegado(dataLegado.Substring(0, 2));
                    mes = getNumerosNoTextoDoLegado(dataLegado.Substring(2, 2));
                }
			}
            else if (dataLegado.Length == 6)
            {
				//Data no padrão americano, alteramos a ordem de leitura dos campos			
				ano = getNumerosNoTextoDoLegado(dataLegado.Substring(0, 4));
                if (ano < 1900)
                {
                    ano = getNumerosNoTextoDoLegado(dataLegado.Substring(4, 2));
                    mes = getNumerosNoTextoDoLegado(dataLegado.Substring(0, 2));
                }
                else {
                    mes = getNumerosNoTextoDoLegado(dataLegado.Substring(4, 2));
                }

			}
            else if (dataLegado.Length == 5)
            {
                mes = getNumerosNoTextoDoLegado(dataLegado.Substring(0, 1));
                ano = getNumerosNoTextoDoLegado(dataLegado.Substring(1, 4));
            }
            else
            {
                mes = 1;
                ano = 1900;
            }
            try
            {
                DateTime date = new DateTime(ano, mes, dia);
                return date;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }
        private int getNumerosNoTextoDoLegado(string texto)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in texto)
            {

                if (char.IsNumber(c) && builder.Length < 9)
                    builder.Append(c);

            }
            if (builder.Length < 1)
                builder.Append("0");

            return int.Parse(builder.ToString().Trim().TrimEnd().TrimStart().Replace(" ", ""));
        }
    }
}