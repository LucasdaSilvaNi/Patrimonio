using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SAM.Web.Relatorios
{
    public class MontaExcelRelatorioFechamento
    {
        public MemoryStream fs;
        private WorksheetPart wsp;
        private WorkbookPart wbp;
        private Worksheet ws;
        private SheetData sd;
        private Sheets sheets;
        private Row linha;
        private DateTime DataGeracao;
        private MergeCells celulasMescladas;
        private int numeroLinhaCelulaInicialASerMesclada = 6;
        private double totalValorAquisicaoContaContabil = 0;
        private double totalValorAtualContaContabil = 0;
        private double totalValorAquisicao = 0;
        private double totalValorAtual = 0;
        public MontaExcelRelatorioFechamento(DataTable dados, string UGE, string MesRef, DateTime paramDataGeracao)
        {
            if (paramDataGeracao == null)
                DataGeracao = DateTime.Now;
            else
                DataGeracao = paramDataGeracao;

            fs = new MemoryStream();
            SpreadsheetDocument document = SpreadsheetDocument.Create(fs, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook);

            wbp = document.AddWorkbookPart();

            // Adding style
            WorkbookStylesPart stylePart = document.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            stylePart.Stylesheet = GenerateStylesheet();
            stylePart.Stylesheet.Save();

            wsp = wbp.AddNewPart<WorksheetPart>();

            sheets = new Sheets();

            wsp = wbp.AddNewPart<WorksheetPart>();
            ws = new Worksheet();

            sd = new SheetData();

            AdicionaObjetoColuna(1, 1, 20);
            AdicionaObjetoColuna(2, 2, 30);
            AdicionaObjetoColuna(3, 4, 20);
            AdicionaObjetoColuna(5, 5, 15);
            AdicionaObjetoColuna(6, 6, 80);
            AdicionaObjetoColuna(7, 8, 20);

            //create a MergeCells class to hold each MergeCell
            celulasMescladas = new MergeCells();

            MontaCabecalho(UGE, MesRef);
            MontaColunas();

            //Montagem da tabela
            var enumerable = dados.AsEnumerable();
            int qtdBPs = enumerable.Select(r => r.Field<int>("ID")).Distinct().Count();

            var listaDeContaContabeis = enumerable.Select(e => e.Field<string>("CONTABIL_MOSTRADO")).Distinct().OrderBy(c => c);

            EnumerableRowCollection<DataRow> listaDaContaContabil;
            uint indiceDeEstiloDaContaContabil = 3;
            int ultimaLinhaMescladaParaConta = 6;

            foreach (string contacontabil in listaDeContaContabeis) {
                listaDaContaContabil = enumerable.Where(e => e.Field<string>("CONTABIL_MOSTRADO") == contacontabil);
                indiceDeEstiloDaContaContabil = (uint)(Convert.ToBoolean(listaDaContaContabil.First()["STATUS_CONTA"])? 3 : 7);

                foreach (var linhaVindaDoBanco in listaDaContaContabil)
                {
                    linha = new Row();

                    linha.Append(ConstructCell(linhaVindaDoBanco["CONTABIL_MOSTRADO"].ToString(), CellValues.String, indiceDeEstiloDaContaContabil));
                    linha.Append(ConstructCell(linhaVindaDoBanco["TIPO_MOVIMENTO"].ToString(), CellValues.String, 3));
                    linha.Append(ConstructCell(linhaVindaDoBanco["DOCUMENTO"].ToString(), CellValues.String, 3));
                    linha.Append(ConstructCell(linhaVindaDoBanco["PROCESSO"].ToString(), CellValues.String, 3));
                    linha.Append(ConstructCell(linhaVindaDoBanco["SIGLACHAPA"].ToString(), CellValues.String, 3));
                    linha.Append(ConstructCell(linhaVindaDoBanco["MATERIAL"].ToString(), CellValues.String, 3));
                    linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", linhaVindaDoBanco["VALOR_AQUISICAO"]), CellValues.String, 3));
                    linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", linhaVindaDoBanco["VALOR_ATUAL"]), CellValues.String, 3));

                    sd.Append(linha);
                }

                totalValorAquisicaoContaContabil = Convert.ToDouble(dados.Compute("Sum(VALOR_AQUISICAO)", string.Format("CONTABIL_MOSTRADO = '{0}'", contacontabil)));
                totalValorAtualContaContabil = Convert.ToDouble(dados.Compute("Sum(VALOR_ATUAL)", string.Format("CONTABIL_MOSTRADO = '{0}'", contacontabil)));
                totalValorAquisicao += totalValorAquisicaoContaContabil;
                totalValorAtual += totalValorAtualContaContabil;

                linha = new Row();

                linha.Append(new Cell());
                linha.Append(ConstructCell("Subtotal:", CellValues.String, 9));
                linha.Append(new Cell());
                linha.Append(new Cell());
                linha.Append(new Cell());
                linha.Append(new Cell());
                linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAquisicaoContaContabil), CellValues.String, 3));
                linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAtualContaContabil), CellValues.String, 3));
                sd.Append(linha);

                ultimaLinhaMescladaParaConta = numeroLinhaCelulaInicialASerMesclada + listaDaContaContabil.Count();

                //para a coluna da conta contábil
                celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:A{1}", numeroLinhaCelulaInicialASerMesclada, ultimaLinhaMescladaParaConta)) });

                //para linha do subtotal
                celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("B{0}:F{1}", ultimaLinhaMescladaParaConta, ultimaLinhaMescladaParaConta)) });

                numeroLinhaCelulaInicialASerMesclada = ultimaLinhaMescladaParaConta + 1;
            }
            //Fim Montagem da tabela

            linha = new Row();

            linha.Append(ConstructCell("Total:", CellValues.String, 4));
            linha.Append(new Cell());
            linha.Append(new Cell());
            linha.Append(new Cell());
            linha.Append(ConstructCell(qtdBPs.ToString(), CellValues.String, 10));
            linha.Append(new Cell());
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAquisicao), CellValues.String, 10));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAtual), CellValues.String, 10));
            sd.Append(linha);

            ultimaLinhaMescladaParaConta += 1;
            //para célula total
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:D{1}", ultimaLinhaMescladaParaConta, ultimaLinhaMescladaParaConta)) });
            //para célula com a quantidade de BPs
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("E{0}:F{1}", ultimaLinhaMescladaParaConta, ultimaLinhaMescladaParaConta)) });

            ultimaLinhaMescladaParaConta += 1;

            linha = new Row();
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "planilha gerada na data: {0:G}", DataGeracao), CellValues.String, 6));
            sd.Append(linha);

            //para célula com a data de geração do excel
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:H{1}", ultimaLinhaMescladaParaConta, ultimaLinhaMescladaParaConta)) });

            ws.Append(sd);
            wsp.Worksheet = ws;
            wsp.Worksheet.Save();

            wsp.Worksheet.InsertAfter(celulasMescladas, wsp.Worksheet.Elements<SheetData>().First());

            Sheet folha = new Sheet();
            folha.Name = "Fechamento";
            folha.SheetId = 1;
            folha.Id = wbp.GetIdOfPart(wsp);
            sheets.Append(folha);

            Workbook wb = new Workbook();
            wb.Append(sheets);
            document.WorkbookPart.Workbook = wb;
            document.WorkbookPart.Workbook.Save();

            document.Close();
        }

        #region Métodos genéricos
        private Cell ConstructCell(string value, CellValues dataType, uint styleIndex = 0)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType),
                StyleIndex = styleIndex
            };
        }

        private void AdicionaObjetoColuna(UInt32Value min, UInt32Value max, DoubleValue width)
        {
            //inicialmente serve para ajustar a largura da coluna
            Columns columns = new Columns();
            columns.Append(new Column() { Min = min, Max = max, Width = width, CustomWidth = true });
            ws.Append(columns);
        }

        private void MontaCabecalho(string UGE, string mesRef)
        {
            linha = new Row();

            linha.Append(ConstructCell("SAM - Sistema de Administração de Materiais - Módulo Patrimonio", CellValues.String, 1));
            sd.Append(linha);

            linha = new Row();

            linha.Append(ConstructCell("Relatório de Fechamento Mensal", CellValues.String, 1));
            sd.Append(linha);

            linha = new Row();

            linha.Append(ConstructCell(string.Format("Mês de referência: {0}", mesRef), CellValues.String, 1));
            sd.Append(linha);

            linha = new Row();

            linha.Append(ConstructCell(string.Format("UGE: {0}", UGE), CellValues.String, 8));
            sd.Append(linha);

            //append a MergeCell to the mergeCells for each set of merged cells
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A1:H1") });

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A2:H2") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A3:H3") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A4:H4") });
        }

        private void MontaColunas()
        {
            linha = new Row();

            linha.Append(ConstructCell("Conta Contábil", CellValues.String, 2));
            linha.Append(ConstructCell("Tipo Movimento", CellValues.String, 2));
            linha.Append(ConstructCell("Documento", CellValues.String, 2));
            linha.Append(ConstructCell("Processo", CellValues.String, 2));
            linha.Append(ConstructCell("Chapa", CellValues.String, 2));
            linha.Append(ConstructCell("Descrição", CellValues.String, 2));
            linha.Append(ConstructCell("Valor de Aquisição", CellValues.String, 2));
            linha.Append(ConstructCell("Valor Atual", CellValues.String, 2));
            sd.Append(linha);

        }
        #endregion

        #region Define estilos 
        private Stylesheet GenerateStylesheet()
        {
            Stylesheet styleSheet = null;

            Fonts fonts = new Fonts(
                new Font( // Index 0 - default
                    new FontSize() { Val = 12 }

                ),
                new Font( // Index 1 - header
                    new FontSize() { Val = 12 },
                    new Bold()
                ),
                new Font( // Index 2 - cor vermelho
                    new FontSize() { Val = 12 },
                    new Color() { Rgb = new HexBinaryValue() { Value = "FF0000" } }
                ),
                new Font( // Index 3 - informar a data de geração da planilha
                    new FontSize() { Val = 10 },
                    new Italic()
                ),
                new Font( // Index 4 - célula total em itálico e em negrito
                    new FontSize() { Val = 12 },
                    new Italic(),
                    new Bold()
                ),
                new Font( // Index 5 - célula com os valores totais em itálico
                    new FontSize() { Val = 12 },
                    new Italic()
                )
             );

            Fills fills = new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }), // Index 1 - default
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "b3b3b3" } }) { PatternType = PatternValues.Solid })
            );

            Borders borders = new Borders(
                    new Border(), // index 0 default
                    new Border( // index 1 black border
                        new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Medium },
                        new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Medium },
                        new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Medium },
                        new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Medium },
                        new DiagonalBorder()),
                    new Border(// index 2 borda a esquerda
                        new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Medium })
                );

            CellFormats cellFormats = new CellFormats(
                    new CellFormat(), // default
                    new CellFormat() { FontId = 1, FillId = 0, BorderId = 0, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center } }, //1 - Título
                    new CellFormat() { FontId = 1, FillId = 2, BorderId = 1, Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center }, ApplyFill = true, ApplyBorder = true }, //2 - Nome das colunas
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Center } }, //3 - Coluna com as informações
                    new CellFormat() { FontId = 4, FillId = 0, BorderId = 0, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right } }, //4 - Células com a descrição totais
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right, WrapText = true } }, //5 - Células de aviso em vermelho
                    new CellFormat() { FontId = 3, FillId = 0, BorderId = 0, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right } }, //6 - Célula com a data de geração da planilha
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 1, Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Center } }, //7 - Contas desativadas (usados no primeiro quadrante),
                    new CellFormat() { FontId = 1, FillId = 2, BorderId = 0, Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center }, ApplyFill = true }, //8 - Linha cinza centralizada
                    new CellFormat() { FontId = 1, FillId = 0, BorderId = 2, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right } }, //9 - Linha subtotal
                    new CellFormat() { FontId = 5, FillId = 0, BorderId = 0, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Center } } //10 - Valores totais em itálico
                );

            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }
        #endregion
    }
}