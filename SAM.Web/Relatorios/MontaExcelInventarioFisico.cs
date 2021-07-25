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
    public class MontaExcelInventarioFisico
    {
        public MemoryStream fs;
        private WorksheetPart wsp;
        private WorkbookPart wbp;
        private Worksheet ws;
        private SheetData sd;
        private Sheets sheets;
        private Row linha;
        private DateTime DataGeracao;
        private UInt32Value IdFolha;
        private MergeCells celulasMescladas;
        private int numeroLinhaCelulaInicialASerMescladaPorUA = 6;
        private int numeroLinhaCelulaInicialASerMescladaPorDivisao = 6;
        private int numeroLinhaCelulaInicialASerMescladaPorResponsavel = 6;
        private double totalValorAquisicao = 0;
        private double totalValorAtual = 0;
        public MontaExcelInventarioFisico(string agrupamento, DataTable dtDadosRelatorio, string UGE, string MesRef) {

            IdFolha = 1;
            DataGeracao = DateTime.Now;

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

            //create a MergeCells class to hold each MergeCell
            celulasMescladas = new MergeCells();

            AdicionaObjetoColuna(1,3,50);
            AdicionaObjetoColuna(4, 4, 20);
            AdicionaObjetoColuna(5, 5, 60);
            AdicionaObjetoColuna(6, 7, 20);

            MontaCabecalho(UGE,MesRef);
            MontaColunas();

            switch (agrupamento)
            {
                case "0":
                    MontaSemAgrupamento(dtDadosRelatorio);
                    break;
                case "1":
                    MontaAgrupandoPorGrupoMaterial(dtDadosRelatorio);
                    break;
                case "2":
                    MontaAgrupandoPorContaContabil(dtDadosRelatorio);
                    break;
            }

            MontaRodape(agrupamento != "0");

            ws.Append(sd);
            wsp.Worksheet = ws;
            wsp.Worksheet.Save();

            wsp.Worksheet.InsertAfter(celulasMescladas, wsp.Worksheet.Elements<SheetData>().First());

            Sheet folha = new Sheet();
            folha.Name = "Inventário Físico";
            folha.SheetId = IdFolha++;
            folha.Id = wbp.GetIdOfPart(wsp);
            sheets.Append(folha);

            Workbook wb = new Workbook();
            wb.Append(sheets);
            document.WorkbookPart.Workbook = wb;
            document.WorkbookPart.Workbook.Save();

            document.Close();
        }

        private void MontaSemAgrupamento(DataTable dados) {
            MontaTabelasAgrupadosPorUA(dados);

            if (dados.Rows.Count > 0)
            {
                totalValorAquisicao = Convert.ToDouble(dados.Compute("Sum(VALOR_AQUISICAO)", null));
                totalValorAtual = Convert.ToDouble(dados.Compute("Sum(VALOR_ATUAL)", null));
            }

            MontaLinhaComTotais();
        }

        private void MontaAgrupandoPorGrupoMaterial(DataTable dados) {

            var enumerable = dados.AsEnumerable();

            var listaDeGruposMateriais = enumerable.Select(e => e.Field<string>("Agrupamento")).Distinct().OrderBy(e => e);
            string descricaoDoGrupo;

            foreach (string codigoGrupoMaterial in listaDeGruposMateriais) {

                descricaoDoGrupo = "";

                var dtDadosDoGrupo = enumerable.Where(e => e.Field<string>("Agrupamento") == codigoGrupoMaterial).CopyToDataTable();

                linha = new Row();

                if (codigoGrupoMaterial == null || string.IsNullOrWhiteSpace(codigoGrupoMaterial))
                    linha.Append(ConstructCell("Sem grupo material", CellValues.String, 8));
                else
                {
                    descricaoDoGrupo = dtDadosDoGrupo.Rows[0].Field<string>("DescAgrupamento");
                    linha.Append(ConstructCell(string.Format("Grupo Material: {0} {1}", codigoGrupoMaterial, descricaoDoGrupo), CellValues.String, 8));
                }

                sd.Append(linha);

                celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:G{1}", numeroLinhaCelulaInicialASerMescladaPorUA, numeroLinhaCelulaInicialASerMescladaPorUA)) });
                numeroLinhaCelulaInicialASerMescladaPorUA++;
                numeroLinhaCelulaInicialASerMescladaPorResponsavel++;
                numeroLinhaCelulaInicialASerMescladaPorDivisao++;

                MontaTabelasAgrupadosPorUA(dtDadosDoGrupo);

                totalValorAquisicao = Convert.ToDouble(dtDadosDoGrupo.Compute("Sum(VALOR_AQUISICAO)", null));
                totalValorAtual = Convert.ToDouble(dtDadosDoGrupo.Compute("Sum(VALOR_ATUAL)", null));

                MontaLinhaComTotais();
                numeroLinhaCelulaInicialASerMescladaPorUA++;
                numeroLinhaCelulaInicialASerMescladaPorResponsavel++;
                numeroLinhaCelulaInicialASerMescladaPorDivisao++;
            }
        }

        private void MontaAgrupandoPorContaContabil(DataTable dados)
        {
            UInt32 indiceDoEstilo = 8;
            bool? statusDaContaContabil = true;
            var enumerable = dados.AsEnumerable();

            var listaDeContasContabeis = enumerable.Select(e => e.Field<string>("Agrupamento")).Distinct().OrderBy(e => e);
            string descricaoDaContaContabil;

            foreach (string codigoContaContabil in listaDeContasContabeis)
            {

                descricaoDaContaContabil = "";

                var dtDadosDaContaContabil = enumerable.Where(e => e.Field<string>("Agrupamento") == codigoContaContabil).CopyToDataTable();

                linha = new Row();

                statusDaContaContabil = dtDadosDaContaContabil.Rows[0].Field<bool?>("STATUS_CONTA");

                if (codigoContaContabil == null || string.IsNullOrWhiteSpace(codigoContaContabil))
                    linha.Append(ConstructCell("Sem Conta Contábil", CellValues.String, 8));
                else
                {
                    descricaoDaContaContabil = dtDadosDaContaContabil.Rows[0].Field<string>("DescAgrupamento");
                    indiceDoEstilo = (UInt32)(statusDaContaContabil == true ? 8 : 10);
                    linha.Append(ConstructCell(string.Format("Conta contábil: {0} {1}", codigoContaContabil, descricaoDaContaContabil), CellValues.String, indiceDoEstilo));
                }

                sd.Append(linha);

                celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:G{1}", numeroLinhaCelulaInicialASerMescladaPorUA, numeroLinhaCelulaInicialASerMescladaPorUA)) });
                numeroLinhaCelulaInicialASerMescladaPorUA++;
                numeroLinhaCelulaInicialASerMescladaPorResponsavel++;
                numeroLinhaCelulaInicialASerMescladaPorDivisao++;

                MontaTabelasAgrupadosPorUA(dtDadosDaContaContabil);

                totalValorAquisicao = Convert.ToDouble(dtDadosDaContaContabil.Compute("Sum(VALOR_AQUISICAO)", null));
                totalValorAtual = Convert.ToDouble(dtDadosDaContaContabil.Compute("Sum(VALOR_ATUAL)", null));

                MontaLinhaComTotaisEContaContabil(codigoContaContabil);
                numeroLinhaCelulaInicialASerMescladaPorUA++;
                numeroLinhaCelulaInicialASerMescladaPorResponsavel++;
                numeroLinhaCelulaInicialASerMescladaPorDivisao++;
            }
        }

        private void MontaTabelasAgrupadosPorUA(DataTable dt) {
            var enumerable = dt.AsEnumerable();

            var listaDeUAs = enumerable.Select(e => e.Field<string>("UA")).Distinct();

            foreach (string UA in listaDeUAs)
            {
                var listaDadosDaUA = enumerable.Where(e => e.Field<string>("UA") == UA);

                var listaDeDivisoes = listaDadosDaUA.Select(e => e.Field<string>("DIVISAO")).Distinct();

                foreach (string divisao in listaDeDivisoes)
                {
                    var listaDadosDaDivisao = listaDadosDaUA.Where(e => e.Field<string>("DIVISAO") == divisao && e.Field<string>("UA") == UA);

                    var listaDeResponsaveis = listaDadosDaDivisao.Select(e => e.Field<string>("RESPONSAVEL")).Distinct();

                    foreach (string responsavel in listaDeResponsaveis) {
                        var listaDadosDoResponsavel = listaDadosDaDivisao.Where(e => e.Field<string>("RESPONSAVEL") == responsavel && e.Field<string>("DIVISAO") == divisao);

                        foreach (DataRow linhaVindaDoBanco in listaDadosDoResponsavel)
                        {
                            linha = new Row();

                            linha.Append(ConstructCell(UA, CellValues.String, 3));
                            linha.Append(ConstructCell(divisao, CellValues.String, 3));
                            linha.Append(ConstructCell(responsavel, CellValues.String, 3));
                            linha.Append(ConstructCell(string.Format("{0}", linhaVindaDoBanco["CHAPA"]), CellValues.String, 3));
                            linha.Append(ConstructCell(string.Format("{0}", linhaVindaDoBanco["MATERIAL"]), CellValues.String, 3));
                            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", linhaVindaDoBanco["VALOR_AQUISICAO"]), CellValues.String, 3));
                            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", linhaVindaDoBanco["VALOR_ATUAL"]), CellValues.String, 3));

                            sd.Append(linha);
                        }

                        celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("C{0}:C{1}", numeroLinhaCelulaInicialASerMescladaPorResponsavel, numeroLinhaCelulaInicialASerMescladaPorResponsavel + listaDadosDoResponsavel.Count() -1)) });
                        numeroLinhaCelulaInicialASerMescladaPorResponsavel += listaDadosDoResponsavel.Count();
                    }

                    celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("B{0}:B{1}", numeroLinhaCelulaInicialASerMescladaPorDivisao, numeroLinhaCelulaInicialASerMescladaPorDivisao + listaDadosDaDivisao.Count() - 1)) });
                    numeroLinhaCelulaInicialASerMescladaPorDivisao += listaDadosDaDivisao.Count();
                }

                celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:A{1}", numeroLinhaCelulaInicialASerMescladaPorUA, numeroLinhaCelulaInicialASerMescladaPorUA + listaDadosDaUA.Count() - 1)) });
                numeroLinhaCelulaInicialASerMescladaPorUA += listaDadosDaUA.Count();
            }

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

        private void MontaCabecalho(string UGE, string mesRef)
        {
            linha = new Row();

            linha.Append(ConstructCell("SAM - Sistema de Administração de Materiais - Módulo Patrimonio", CellValues.String, 1));
            sd.Append(linha);

            linha = new Row();

            linha.Append(ConstructCell("Inventário Físico de Bens Móveis", CellValues.String, 1));
            sd.Append(linha);

            linha = new Row();

            linha.Append(ConstructCell(string.Format("Mês de referência: {0}", mesRef), CellValues.String, 1));
            sd.Append(linha);

            linha = new Row();

            linha.Append(ConstructCell(string.Format("UGE: {0}", UGE), CellValues.String, 8));
            sd.Append(linha);

            //append a MergeCell to the mergeCells for each set of merged cells
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A1:G1") });

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A2:G2") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A3:G3") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A4:G4") });
        }

        private void MontaColunas() {
            linha = new Row();

            linha.Append(ConstructCell("UA", CellValues.String, 2));
            linha.Append(ConstructCell("Divisão", CellValues.String, 2));
            linha.Append(ConstructCell("Responsável", CellValues.String, 2));
            linha.Append(ConstructCell("Chapa", CellValues.String, 2));
            linha.Append(ConstructCell("Material", CellValues.String, 2));
            linha.Append(ConstructCell("Valor Aquisição", CellValues.String, 2));
            linha.Append(ConstructCell("Valor Atual", CellValues.String,2));
            sd.Append(linha);
        }

        private void MontaRodape(bool agrupado)
        {
            linha = new Row();
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "planilha gerada na data: {0:G}", DataGeracao), CellValues.String, 6));
            sd.Append(linha);

            if(agrupado)
                celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:G{1}",numeroLinhaCelulaInicialASerMescladaPorUA, numeroLinhaCelulaInicialASerMescladaPorUA)) });
            else
                celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:G{1}", numeroLinhaCelulaInicialASerMescladaPorUA + 1, numeroLinhaCelulaInicialASerMescladaPorUA + 1)) });
        }

        private void MontaLinhaComTotais() {
            linha = new Row();
            linha.Append(new Cell());
            linha.Append(new Cell());
            linha.Append(new Cell());
            linha.Append(new Cell());
            linha.Append(ConstructCell("Total:", CellValues.String, 4));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAquisicao), CellValues.String, 3));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAtual), CellValues.String, 3));
            sd.Append(linha);
        }

        private void MontaLinhaComTotaisEContaContabil(string contaContabil)
        {
            linha = new Row();
            linha.Append(new Cell());
            linha.Append(new Cell());
            linha.Append(new Cell());
            linha.Append(new Cell());
            linha.Append(ConstructCell(string.Format("Conta Contábil: {0} \n Total:", contaContabil), CellValues.String, 9));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAquisicao), CellValues.String, 3));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAtual), CellValues.String, 3));

            linha.CustomHeight = true;
            linha.Height = 50;
            
            sd.Append(linha);
        }

        private void AdicionaObjetoColuna(UInt32Value min, UInt32Value max, DoubleValue width)
        {
            //inicialmente serve para ajustar a largura da coluna
            Columns columns = new Columns();
            columns.Append(new Column() { Min = min, Max = max, Width = width, CustomWidth = true });
            ws.Append(columns);
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
                new Font( // Index 3 - informar a data de geração da lanilha
                    new FontSize() { Val = 10 },
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
                        new DiagonalBorder())
                );

            CellFormats cellFormats = new CellFormats(
                    new CellFormat(), // default
                    new CellFormat() { FontId = 1, FillId = 0, BorderId = 0, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center } }, //1 - Título
                    new CellFormat() { FontId = 1, FillId = 2, BorderId = 1, Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center }, ApplyFill = true, ApplyBorder = true }, //2 - Nome das colunas
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Center } }, //3 - Coluna com as informações
                    new CellFormat() { FontId = 1, FillId = 2, BorderId = 1, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right } }, //4 - Células com a descrição totais
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right, WrapText = true } }, //5 - Células de aviso em vermelho
                    new CellFormat() { FontId = 3, FillId = 0, BorderId = 0, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right } }, //6 - Célula com a data de geração da planilha
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 1, Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Center } }, //7 - Contas desativadas (usados no primeiro quadrante),
                    new CellFormat() { FontId = 1, FillId = 2, BorderId = 0, Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center }, ApplyFill = true }, //8 - Linha cinza centralizada
                    new CellFormat() { FontId = 1, FillId = 2, BorderId = 1, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right, WrapText = true } }, //9 - Células total e conta contábil
                    new CellFormat() { FontId = 2, FillId = 2, BorderId = 0, Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center }, ApplyFill = true } //10 - Linha cinza centralizada (igual indíce 8) mas com cor vermelha
                );

            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }
        #endregion
    }
}