using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SAM.Web.ViewModels;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SAM.Web.Relatorios
{
    public class MontaExcelResumoDoInventario
    {
        public MemoryStream fs;
        private WorksheetPart wsp;
        private WorkbookPart wbp;
        private Worksheet ws;
        private SheetData sd;
        private Sheets sheets;
        private Row linha;
        private string UGE;
        private string MesRef;
        private DateTime DataGeracao;
        private UInt32Value IdFolha;
        public MontaExcelResumoDoInventario(ReportResumoInventarioViewModel viewModel, DataTable[] dtDadosRelatorio, string UGE, string MesRef)
        {
            this.UGE = UGE;
            this.MesRef = MesRef;
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

            if (viewModel.ResumoConsolidado)
                MontaPlanilhaResumoConsolidado(dtDadosRelatorio.Where(dt => dt.TableName == "dsRelatorio_ResumoContasContabeis").First());

            if (viewModel.AquisicoesCorrentes)
                MontaPlanilhaAquisicoesNoMes(dtDadosRelatorio.Where(dt => dt.TableName == "dsBPs_IncorporadosMesRef").First());

            if (viewModel.Terceiros)
                MontaPlanilhaBensDeTerceiros(dtDadosRelatorio.Where(dt => dt.TableName == "dsBPs_Terceiros").First());

            if (viewModel.BaixasCorrentes)
                MontaPlanilhaBaixasNoMes(dtDadosRelatorio.Where(dt => dt.TableName == "dsBPs_BaixadosMesRef").First());

            if (viewModel.Acervos)
                MontaPlanilhaBensDeAcervos(dtDadosRelatorio.Where(dt => dt.TableName == "dsBPs_Acervo").First());

            if (viewModel.BPTotalDepreciados)
                MontaPlanilhaBensTotalmenteDepreciados(dtDadosRelatorio.Where(dt => dt.TableName == "dsBPS_TotalmenteDepreciados").First());

            Workbook wb = new Workbook();
            wb.Append(sheets);
            document.WorkbookPart.Workbook = wb;
            document.WorkbookPart.Workbook.Save();

            document.Close();
        }

        private void MontaPlanilhaResumoConsolidado(DataTable dt) {
            wsp = wbp.AddNewPart<WorksheetPart>();
            ws = new Worksheet();

            sd = new SheetData();

            MontaEInsereLinhaDeCabecalhoComNomeDoRelatorio();

            //create a MergeCells class to hold each MergeCell
            MergeCells celulasMescladas = new MergeCells();

            //append a MergeCell to the mergeCells for each set of merged cells
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A1:E1") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A2:E2") });

            linha = new Row();

            linha.Append(ConstructCell("Resumo Contábil de Inventário de Bens Móveis", CellValues.String, 1));
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A3:E3") });

            MontaEInsereLinhaDeCabecalhoComInformacoesDoRelatorio();
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A4:E4") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A5:E5") });

            linha = MontaLinhaComNomeDasColunas(new string[] { "Conta depreciação", "Conta contábil", "Valor contábil", "Depreciação no mês", "Depreciação acumulada" });
            sd.Append(linha);

            int numeroLinhaCelulaInicialASerMesclada = 7;

            double totalValorContabilContaDepreciacao = 0,
                   totalDepreciacaoMensalContaDepreciacao = 0,
                   totalDepreciacaoAcumuladaContaDepreciacao = 0,
                   totalValorContabilUGE = 0,
                   totalDepreciacaoMensalUGE = 0,
                   totalDepreciacaoAcumuladaUGE = 0;

            var enumerable = dt.AsEnumerable();

            var contasDeDepreciacoes = enumerable.Select(e => e.Field<int?>("ContaContabilDepreciacao")).Distinct();

            foreach (int contaDeDepreciacao in contasDeDepreciacoes) {
                totalValorContabilContaDepreciacao = 0;
                totalDepreciacaoMensalContaDepreciacao = 0;
                totalDepreciacaoAcumuladaContaDepreciacao = 0;
                var listaDaContaDeDepreciacao = enumerable.Where(e => e.Field<int?>("ContaContabilDepreciacao") == contaDeDepreciacao);

                uint indiceDeEstiloDaContaDepreciacao = (uint)(Convert.ToBoolean(listaDaContaDeDepreciacao.First()["ContaContabilDepreciacaoStatus"]) ? 3 : 7);

                var listaDeContasContabeis =  listaDaContaDeDepreciacao.Select(e => e.Field<string>("ContaContabil")).Distinct();
                foreach (string contaContabil in listaDeContasContabeis) {
                    var listaDaContaContabil = enumerable.Where(e => e.Field<string>("ContaContabil") == contaContabil);
                    uint indiceDeEstiloDaContaContabil = (uint)(Convert.ToBoolean(listaDaContaContabil.First()["ContaContabilStatus"]) ? 3 : 7);

                    foreach (DataRow linhaVindaDoBanco in listaDaContaContabil) {
                        linha = new Row();

                        linha.Append(ConstructCell(string.Format("{0} - {1}", linhaVindaDoBanco["ContaContabilDepreciacao"], linhaVindaDoBanco["ContaContabilDepreciacaoDescricao"]), CellValues.String, indiceDeEstiloDaContaDepreciacao));
                        linha.Append(ConstructCell(string.Format("{0} - {1}", linhaVindaDoBanco["ContaContabil"], linhaVindaDoBanco["ContaContabilDescricao"]), CellValues.String, indiceDeEstiloDaContaContabil));
                        linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", linhaVindaDoBanco["ValorContabil"]), CellValues.String, 3));
                        linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", linhaVindaDoBanco["DepreciacaoNoMes"]), CellValues.String, 3));
                        linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", linhaVindaDoBanco["DepreciacaoAcumulada"]), CellValues.String, 3));

                        sd.Append(linha);
                    }
                }

                totalValorContabilContaDepreciacao = Convert.ToDouble(listaDaContaDeDepreciacao.First().Table.Compute("Sum(ValorContabil)", string.Format("ContaContabilDepreciacao = '{0}'", contaDeDepreciacao)));
                totalDepreciacaoMensalContaDepreciacao = Convert.ToDouble(listaDaContaDeDepreciacao.First().Table.Compute("Sum(DepreciacaoNoMes)", string.Format("ContaContabilDepreciacao = '{0}'", contaDeDepreciacao)));
                totalDepreciacaoAcumuladaContaDepreciacao = Convert.ToDouble(listaDaContaDeDepreciacao.First().Table.Compute("Sum(DepreciacaoAcumulada)", string.Format("ContaContabilDepreciacao = '{0}'", contaDeDepreciacao)));
                totalValorContabilUGE += totalValorContabilContaDepreciacao;
                totalDepreciacaoMensalUGE += totalDepreciacaoMensalContaDepreciacao;
                totalDepreciacaoAcumuladaUGE += totalDepreciacaoAcumuladaContaDepreciacao;

                linha = new Row();

                linha.Append(new Cell());
                linha.Append(ConstructCell("Total", CellValues.String, 4));
                linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorContabilContaDepreciacao), CellValues.String, 3));
                linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalDepreciacaoMensalContaDepreciacao), CellValues.String, 3));
                linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalDepreciacaoAcumuladaContaDepreciacao), CellValues.String, 3));
                sd.Append(linha);

                celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:A{1}",numeroLinhaCelulaInicialASerMesclada, numeroLinhaCelulaInicialASerMesclada + listaDeContasContabeis.Count())) });
                numeroLinhaCelulaInicialASerMesclada += listaDeContasContabeis.Count() + 1;
            }

            linha = new Row();

            linha.Append(ConstructCell("Total da UGE", CellValues.String, 4));
            linha.Append(new Cell());
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorContabilUGE), CellValues.String, 3));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalDepreciacaoMensalUGE), CellValues.String, 3));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalDepreciacaoAcumuladaUGE), CellValues.String, 3));
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:B{0}", numeroLinhaCelulaInicialASerMesclada) ) });

            linha = new Row();
            linha.Append(ConstructCell("Neste quadrante não serão contabilizados Bens Patrimoniais do tipo Acervo, nem do tipo Terceiro", CellValues.String, 5));
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:E{0}", numeroLinhaCelulaInicialASerMesclada + 1 )) });

            MontaEInsereLinhaDeRodapeComDataDeGeracao();

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:E{0}", numeroLinhaCelulaInicialASerMesclada + 2)) });

            AdicionaObjetoColuna(1, 5, 30);

            ws.Append(sd);
            wsp.Worksheet = ws;
            wsp.Worksheet.Save();

            wsp.Worksheet.InsertAfter(celulasMescladas, wsp.Worksheet.Elements<SheetData>().First());
            InsereFolha("Resumo Contábil");
        }

        private void MontaPlanilhaAquisicoesNoMes(DataTable dt)
        {
            wsp = wbp.AddNewPart<WorksheetPart>();
            ws = new Worksheet();

            sd = new SheetData();

            MontaEInsereLinhaDeCabecalhoComNomeDoRelatorio();

            //create a MergeCells class to hold each MergeCell
            MergeCells celulasMescladas = new MergeCells();

            //append a MergeCell to the mergeCells for each set of merged cells
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A1:F1") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A2:F2") });

            linha = new Row();

            linha.Append(ConstructCell("Aquisições no mês", CellValues.String, 1));
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A3:F3") });

            MontaEInsereLinhaDeCabecalhoComInformacoesDoRelatorio();
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A4:F4") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A5:F5") });

            linha = MontaLinhaComNomeDasColunas(new string[] { "Tipo Movimentação", "Conta depreciação", "Conta contábil", "Valor de aquisição", "Depreciação acumulada", "ValorAtual" });
            sd.Append(linha);

            int numeroLinhaCelulaInicialASerMesclada = 7, numeroLinhaCelulaInicialASerMescladaParaTipoIncorporacao = 7;

            double totalValorDeAquisicaoContaDepreciacao = 0,
                   totalDepreciacaoMensalContaDepreciacao = 0,
                   totalValorAtualContaDepreciacao = 0,
                   totalValorDeAquisicaoUGE = 0,
                   totalDepreciacaoMensalUGE = 0,
                   totalValorAtualUGE = 0;

            var enumerable = dt.AsEnumerable();

            var tiposDeIncorporacoes = enumerable.Select(e => e.Field<string>("TipoIncorporacao")).Distinct();

            foreach (string tipoDeIncorporacao in tiposDeIncorporacoes)
            {

                var listaDoTipoDeIncorporacao = enumerable.Where(e => e.Field<string>("TipoIncorporacao") == tipoDeIncorporacao);

                var listaDeContasDepreciacoes = listaDoTipoDeIncorporacao.Select(e => e.Field<int?>("ContaContabilDepreciacao")).Distinct();

                foreach (int contaDeDepreciacao in listaDeContasDepreciacoes)
                {
                    totalValorDeAquisicaoContaDepreciacao = 0;
                    totalDepreciacaoMensalContaDepreciacao = 0;
                    totalValorAtualContaDepreciacao = 0;
                    var listaDaContaDeDepreciacao = listaDoTipoDeIncorporacao.Where(e => e.Field<int?>("ContaContabilDepreciacao") == contaDeDepreciacao);

                    foreach (DataRow linhaVindaDoBanco in listaDaContaDeDepreciacao)
                    {
                        linha = new Row();

                        linha.Append(ConstructCell(string.Format("{0}", linhaVindaDoBanco["TipoIncorporacao"]), CellValues.String, 3));
                        linha.Append(ConstructCell(string.Format("{0} - {1}", linhaVindaDoBanco["ContaContabilDepreciacao"], linhaVindaDoBanco["ContaContabilDepreciacaoDescricao"]), CellValues.String, 3));
                        linha.Append(ConstructCell(string.Format("{0} - {1}", linhaVindaDoBanco["ContaContabil"], linhaVindaDoBanco["ContaContabilDescricao"]), CellValues.String, 3));
                        linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", linhaVindaDoBanco["ValorAquisicao"]), CellValues.String, 3));
                        linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", linhaVindaDoBanco["DepreciacaoNoMes"]), CellValues.String, 3));
                        linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", linhaVindaDoBanco["ValorAtual"]), CellValues.String, 3));

                        sd.Append(linha);
                    }

                    totalValorDeAquisicaoContaDepreciacao = Convert.ToDouble(listaDaContaDeDepreciacao.First().Table.Compute("Sum(ValorAquisicao)", string.Format("ContaContabilDepreciacao = '{0}' AND TipoIncorporacao = '{1}'", contaDeDepreciacao, tipoDeIncorporacao)));
                    totalDepreciacaoMensalContaDepreciacao = Convert.ToDouble(listaDaContaDeDepreciacao.First().Table.Compute("Sum(DepreciacaoNoMes)", string.Format("ContaContabilDepreciacao = '{0}' AND TipoIncorporacao = '{1}'", contaDeDepreciacao, tipoDeIncorporacao)));
                    totalValorAtualContaDepreciacao = Convert.ToDouble(listaDaContaDeDepreciacao.First().Table.Compute("Sum(ValorAtual)", string.Format("ContaContabilDepreciacao = '{0}' AND TipoIncorporacao = '{1}'", contaDeDepreciacao, tipoDeIncorporacao)));
                    totalValorDeAquisicaoUGE += totalValorDeAquisicaoContaDepreciacao;
                    totalDepreciacaoMensalUGE += totalDepreciacaoMensalContaDepreciacao;
                    totalValorAtualUGE += totalValorAtualContaDepreciacao;

                    linha = new Row();

                    linha.Append(new Cell());
                    linha.Append(new Cell());
                    linha.Append(ConstructCell("Total", CellValues.String, 4));
                    linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorDeAquisicaoContaDepreciacao), CellValues.String, 3));
                    linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalDepreciacaoMensalContaDepreciacao), CellValues.String, 3));
                    linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAtualContaDepreciacao), CellValues.String, 3));
                    sd.Append(linha);

                    celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("B{0}:B{1}", numeroLinhaCelulaInicialASerMesclada, numeroLinhaCelulaInicialASerMesclada + listaDaContaDeDepreciacao.Count())) });
                    numeroLinhaCelulaInicialASerMesclada += listaDaContaDeDepreciacao.Count() + 1;
                }

                celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:A{1}", numeroLinhaCelulaInicialASerMescladaParaTipoIncorporacao, numeroLinhaCelulaInicialASerMesclada - 1)) });
                numeroLinhaCelulaInicialASerMescladaParaTipoIncorporacao = numeroLinhaCelulaInicialASerMesclada;
            }

            linha = new Row();

            linha.Append(ConstructCell("Total da UGE", CellValues.String, 4));
            linha.Append(new Cell());
            linha.Append(new Cell());
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorDeAquisicaoUGE), CellValues.String, 3));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalDepreciacaoMensalUGE), CellValues.String, 3));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAtualUGE), CellValues.String, 3));
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:C{0}", numeroLinhaCelulaInicialASerMesclada)) });

            MontaEInsereLinhaDeRodapeComDataDeGeracao();

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:F{0}", numeroLinhaCelulaInicialASerMesclada + 1)) });

            AdicionaObjetoColuna(1, 6, 30);

            ws.Append(sd);

            wsp.Worksheet = ws;
            wsp.Worksheet.Save();

            wsp.Worksheet.InsertAfter(celulasMescladas, wsp.Worksheet.Elements<SheetData>().First());

            InsereFolha("Aquisições");
        }

        private void MontaPlanilhaBensDeTerceiros(DataTable dt)
        {
            wsp = wbp.AddNewPart<WorksheetPart>();
            ws = new Worksheet();

            sd = new SheetData();

            MontaEInsereLinhaDeCabecalhoComNomeDoRelatorio();

            //create a MergeCells class to hold each MergeCell
            MergeCells celulasMescladas = new MergeCells();

            //append a MergeCell to the mergeCells for each set of merged cells
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A1:B1") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A2:B2") });

            linha = new Row();

            linha.Append(ConstructCell("Bens de Terceiros", CellValues.String, 1));
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A3:B3") });

            MontaEInsereLinhaDeCabecalhoComInformacoesDoRelatorio();
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A4:B4") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A5:B5") });

            linha = MontaLinhaComNomeDasColunas(new string[] { "Conta contábil", "Valor de Aquisição" });
            sd.Append(linha);

            string total = "R$ 0,00";

            if (dt.Rows.Count == 0)
            {
                sd.Append(new Row());
            }
            else {
                foreach (DataRow linhaVindaDoBanco in dt.Rows) {
                    total = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", linhaVindaDoBanco["ValorAquisicao"]);
                    linha = new Row();
                    linha.Append(ConstructCell(string.Format("{0}",linhaVindaDoBanco["ContaContabil"]), CellValues.String,3));
                    linha.Append(ConstructCell(total, CellValues.String, 3));
                    sd.Append(linha);
                }
            }

            linha = new Row();

            linha.Append(ConstructCell("Total da UGE", CellValues.String, 4));
            linha.Append(ConstructCell(total, CellValues.String, 3));
            sd.Append(linha);

            linha = new Row();
            linha.Append(ConstructCell("Bens Patrimoniais de tipo \"Terceiro\" serão registrados no sistema apenas para controle físico, sendo que os mesmos não depreciam.", CellValues.String, 5));
            linha.Height = 50;
            linha.CustomHeight = true;
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A9:B9") });

            MontaEInsereLinhaDeRodapeComDataDeGeracao();

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A10:B10") });

            AdicionaObjetoColuna(1, 2, 30);

            ws.Append(sd);

            wsp.Worksheet = ws;
            wsp.Worksheet.Save();

            wsp.Worksheet.InsertAfter(celulasMescladas, wsp.Worksheet.Elements<SheetData>().First());

            InsereFolha("Bens de Terceiros");
        }

        private void MontaPlanilhaBaixasNoMes(DataTable dt)
        {
            wsp = wbp.AddNewPart<WorksheetPart>();
            ws = new Worksheet();

            sd = new SheetData();

            MontaEInsereLinhaDeCabecalhoComNomeDoRelatorio();

            //create a MergeCells class to hold each MergeCell
            MergeCells celulasMescladas = new MergeCells();

            //append a MergeCell to the mergeCells for each set of merged cells
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A1:F1") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A2:F2") });

            linha = new Row();

            linha.Append(ConstructCell("Baixas no mês", CellValues.String, 1));
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A3:F3") });

            MontaEInsereLinhaDeCabecalhoComInformacoesDoRelatorio();
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A4:F4") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A5:F5") });

            linha = MontaLinhaComNomeDasColunas(new string[] { "Tipo Movimentação", "Conta depreciação", "Conta contábil", "Valor de aquisição", "Depreciação acumulada", "ValorAtual" });
            sd.Append(linha);

            int numeroLinhaCelulaInicialASerMesclada = 7, numeroLinhaCelulaInicialASerMescladaParaTipoIncorporacao = 7;

            double totalValorDeAquisicaoContaDepreciacao = 0,
                   totalDepreciacaoMensalContaDepreciacao = 0,
                   totalValorAtualContaDepreciacao = 0,
                   totalValorDeAquisicaoUGE = 0,
                   totalDepreciacaoMensalUGE = 0,
                   totalValorAtualUGE = 0;

            var enumerable = dt.AsEnumerable();

            var tiposDeIncorporacoes = enumerable.Select(e => e.Field<string>("TipoIncorporacao")).Distinct();

            foreach (string tipoDeIncorporacao in tiposDeIncorporacoes)
            {

                var listaDoTipoDeIncorporacao = enumerable.Where(e => e.Field<string>("TipoIncorporacao") == tipoDeIncorporacao);

                var listaDeContasDepreciacoes = listaDoTipoDeIncorporacao.Select(e => e.Field<int?>("ContaContabilDepreciacao")).Distinct();

                foreach (int contaDeDepreciacao in listaDeContasDepreciacoes)
                {
                    totalValorDeAquisicaoContaDepreciacao = 0;
                    totalDepreciacaoMensalContaDepreciacao = 0;
                    totalValorAtualContaDepreciacao = 0;
                    var listaDaContaDeDepreciacao = listaDoTipoDeIncorporacao.Where(e => e.Field<int?>("ContaContabilDepreciacao") == contaDeDepreciacao);

                    foreach (DataRow linhaVindaDoBanco in listaDaContaDeDepreciacao)
                    {
                        linha = new Row();

                        linha.Append(ConstructCell(string.Format("{0}", linhaVindaDoBanco["TipoIncorporacao"]), CellValues.String, 3));
                        linha.Append(ConstructCell(string.Format("{0} - {1}", linhaVindaDoBanco["ContaContabilDepreciacao"], linhaVindaDoBanco["ContaContabilDepreciacaoDescricao"]), CellValues.String, 3));
                        linha.Append(ConstructCell(string.Format("{0} - {1}", linhaVindaDoBanco["ContaContabil"], linhaVindaDoBanco["ContaContabilDescricao"]), CellValues.String, 3));
                        linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", linhaVindaDoBanco["ValorAquisicao"]), CellValues.String, 3));
                        linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", linhaVindaDoBanco["DepreciacaoNoMes"]), CellValues.String, 3));
                        linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", linhaVindaDoBanco["ValorAtual"]), CellValues.String, 3));

                        sd.Append(linha);
                    }

                    totalValorDeAquisicaoContaDepreciacao = Convert.ToDouble(listaDaContaDeDepreciacao.First().Table.Compute("Sum(ValorAquisicao)", string.Format("ContaContabilDepreciacao = '{0}' AND TipoIncorporacao = '{1}'", contaDeDepreciacao, tipoDeIncorporacao)));
                    totalDepreciacaoMensalContaDepreciacao = Convert.ToDouble(listaDaContaDeDepreciacao.First().Table.Compute("Sum(DepreciacaoNoMes)", string.Format("ContaContabilDepreciacao = '{0}' AND TipoIncorporacao = '{1}'", contaDeDepreciacao, tipoDeIncorporacao)));
                    totalValorAtualContaDepreciacao = Convert.ToDouble(listaDaContaDeDepreciacao.First().Table.Compute("Sum(ValorAtual)", string.Format("ContaContabilDepreciacao = '{0}' AND TipoIncorporacao = '{1}'", contaDeDepreciacao, tipoDeIncorporacao)));
                    totalValorDeAquisicaoUGE += totalValorDeAquisicaoContaDepreciacao;
                    totalDepreciacaoMensalUGE += totalDepreciacaoMensalContaDepreciacao;
                    totalValorAtualUGE += totalValorAtualContaDepreciacao;

                    linha = new Row();

                    linha.Append(new Cell());
                    linha.Append(new Cell());
                    linha.Append(ConstructCell("Total", CellValues.String, 4));
                    linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorDeAquisicaoContaDepreciacao), CellValues.String, 3));
                    linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalDepreciacaoMensalContaDepreciacao), CellValues.String, 3));
                    linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAtualContaDepreciacao), CellValues.String, 3));
                    sd.Append(linha);

                    celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("B{0}:B{1}", numeroLinhaCelulaInicialASerMesclada, numeroLinhaCelulaInicialASerMesclada + listaDaContaDeDepreciacao.Count())) });
                    numeroLinhaCelulaInicialASerMesclada += listaDaContaDeDepreciacao.Count() + 1;
                }

                celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:A{1}", numeroLinhaCelulaInicialASerMescladaParaTipoIncorporacao, numeroLinhaCelulaInicialASerMesclada - 1)) });
                numeroLinhaCelulaInicialASerMescladaParaTipoIncorporacao = numeroLinhaCelulaInicialASerMesclada;
            }

            linha = new Row();

            linha.Append(ConstructCell("Total da UGE", CellValues.String, 4));
            linha.Append(new Cell());
            linha.Append(new Cell());
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorDeAquisicaoUGE), CellValues.String, 3));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalDepreciacaoMensalUGE), CellValues.String, 3));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAtualUGE), CellValues.String, 3));
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:C{0}", numeroLinhaCelulaInicialASerMesclada)) });

            MontaEInsereLinhaDeRodapeComDataDeGeracao();

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:F{0}", numeroLinhaCelulaInicialASerMesclada + 1)) });

            AdicionaObjetoColuna(1, 6, 30);

            ws.Append(sd);

            wsp.Worksheet = ws;
            wsp.Worksheet.Save();

            wsp.Worksheet.InsertAfter(celulasMescladas, wsp.Worksheet.Elements<SheetData>().First());

            InsereFolha("Baixas");
        }

        private void MontaPlanilhaBensDeAcervos(DataTable dt)
        {
            wsp = wbp.AddNewPart<WorksheetPart>();
            ws = new Worksheet();

            sd = new SheetData();

            MontaEInsereLinhaDeCabecalhoComNomeDoRelatorio();

            //create a MergeCells class to hold each MergeCell
            MergeCells celulasMescladas = new MergeCells();

            //append a MergeCell to the mergeCells for each set of merged cells
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A1:B1") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A2:B2") });

            linha = new Row();

            linha.Append(ConstructCell("Acervos", CellValues.String, 1));
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A3:B3") });

            MontaEInsereLinhaDeCabecalhoComInformacoesDoRelatorio();
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A4:B4") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A5:B5") });

            linha = MontaLinhaComNomeDasColunas(new string[] { "Conta contábil", "Valor de Aquisição" });
            sd.Append(linha);

            string total = "R$ 0,00";

            if (dt.Rows.Count == 0)
            {
                sd.Append(new Row());
            }
            else
            {
                foreach (DataRow linhaVindaDoBanco in dt.Rows)
                {
                    total = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", linhaVindaDoBanco["ValorAquisicao"]);
                    linha = new Row();
                    linha.Append(ConstructCell(string.Format("{0}", linhaVindaDoBanco["ContaContabil"]), CellValues.String, 3));
                    linha.Append(ConstructCell(total, CellValues.String, 3));
                    sd.Append(linha);
                }
            }

            linha = new Row();

            linha.Append(ConstructCell("Total da UGE", CellValues.String, 4));
            linha.Append(ConstructCell(total, CellValues.String, 3));
            sd.Append(linha);

            linha = new Row();
            linha.Append(ConstructCell("Bens Patrimoniais de tipo \"Acervo\" serão registrados no sistema apenas para controle físico, sendo que os mesmos não depreciam.", CellValues.String, 5));
            linha.Height = 50;
            linha.CustomHeight = true;
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A9:B9") });

            MontaEInsereLinhaDeRodapeComDataDeGeracao();

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A10:B10") });

            AdicionaObjetoColuna(1, 2, 30);
            ws.Append(sd);

            wsp.Worksheet = ws;
            wsp.Worksheet.Save();

            wsp.Worksheet.InsertAfter(celulasMescladas, wsp.Worksheet.Elements<SheetData>().First());

            InsereFolha("Bens de Acervos");
        }

        private void MontaPlanilhaBensTotalmenteDepreciados(DataTable dt)
        {
            wsp = wbp.AddNewPart<WorksheetPart>();
            ws = new Worksheet();

            sd = new SheetData();

            MontaEInsereLinhaDeCabecalhoComNomeDoRelatorio();

            //create a MergeCells class to hold each MergeCell
            MergeCells celulasMescladas = new MergeCells();

            //append a MergeCell to the mergeCells for each set of merged cells
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A1:E1") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A2:E2") });

            linha = new Row();

            linha.Append(ConstructCell("Bens totalmente depreciados", CellValues.String, 1));
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A3:E3") });

            MontaEInsereLinhaDeCabecalhoComInformacoesDoRelatorio();
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A4:E4") });
            celulasMescladas.Append(new MergeCell() { Reference = new StringValue("A5:E5") });

            linha = MontaLinhaComNomeDasColunas(new string[] { "Conta depreciação", "Conta contábil", "Valor de aquisição", "Depreciação acumulada", "ValorAtual" });
            sd.Append(linha);

            int numeroLinhaCelulaInicialASerMesclada = 7;

            double totalValorDeAquisicaoContaDepreciacao = 0,
                   totalDepreciacaoAcumuladaContaDepreciacao = 0,
                   totalValorAtualContaDepreciacao = 0,
                   totalValorDeAquisicaoUGE = 0,
                   totalDepreciacaoAcumuladaUGE = 0,
                   totalValorAtualUGE = 0;

            var enumerable = dt.AsEnumerable();

            var contasDeDepreciacoes = enumerable.Select(e => e.Field<int?>("ContaContabilDepreciacao")).Distinct();

            foreach (int contaDeDepreciacao in contasDeDepreciacoes)
            {
                totalValorDeAquisicaoContaDepreciacao = 0;
                totalDepreciacaoAcumuladaContaDepreciacao = 0;
                totalValorAtualContaDepreciacao = 0;
                var listaDaContaDeDepreciacao = enumerable.Where(e => e.Field<int?>("ContaContabilDepreciacao") == contaDeDepreciacao);

                foreach (DataRow linhaVindaDoBanco in listaDaContaDeDepreciacao) {
                    linha = new Row();

                    linha.Append(ConstructCell(string.Format("{0} - {1}", linhaVindaDoBanco["ContaContabilDepreciacao"], linhaVindaDoBanco["ContaContabilDepreciacaoDescricao"]), CellValues.String, 3));
                    linha.Append(ConstructCell(string.Format("{0} - {1}", linhaVindaDoBanco["ContaContabil"], linhaVindaDoBanco["ContaContabilDescricao"]), CellValues.String, 3));
                    linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0 :C}", linhaVindaDoBanco["ValorAquisicao"]), CellValues.String, 3));
                    linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", linhaVindaDoBanco["DepreciacaoAcumulada"]), CellValues.String, 3));
                    linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", linhaVindaDoBanco["ValorAtual"]), CellValues.String, 3));

                    sd.Append(linha);
                }

                totalValorDeAquisicaoContaDepreciacao = Convert.ToDouble(listaDaContaDeDepreciacao.First().Table.Compute("Sum(ValorAquisicao)", string.Format("ContaContabilDepreciacao = '{0}'", contaDeDepreciacao)));
                totalDepreciacaoAcumuladaContaDepreciacao = Convert.ToDouble(listaDaContaDeDepreciacao.First().Table.Compute("Sum(DepreciacaoAcumulada)", string.Format("ContaContabilDepreciacao = '{0}'", contaDeDepreciacao)));
                totalValorAtualContaDepreciacao = Convert.ToDouble(listaDaContaDeDepreciacao.First().Table.Compute("Sum(ValorAtual)", string.Format("ContaContabilDepreciacao = '{0}'", contaDeDepreciacao)));
                totalValorDeAquisicaoUGE += totalValorDeAquisicaoContaDepreciacao;
                totalDepreciacaoAcumuladaUGE += totalDepreciacaoAcumuladaContaDepreciacao;
                totalValorAtualUGE += totalValorAtualContaDepreciacao;

                linha = new Row();

                linha.Append(new Cell());
                linha.Append(ConstructCell("Total", CellValues.String, 4));
                linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorDeAquisicaoContaDepreciacao), CellValues.String, 3));
                linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalDepreciacaoAcumuladaContaDepreciacao), CellValues.String, 3));
                linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAtualContaDepreciacao), CellValues.String, 3));
                sd.Append(linha);

                celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:A{1}", numeroLinhaCelulaInicialASerMesclada, numeroLinhaCelulaInicialASerMesclada + listaDaContaDeDepreciacao.Count())) });
                numeroLinhaCelulaInicialASerMesclada += listaDaContaDeDepreciacao.Count() + 1;
            }

            linha = new Row();

            linha.Append(ConstructCell("Total da UGE", CellValues.String, 4));
            linha.Append(new Cell());
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorDeAquisicaoUGE), CellValues.String, 3));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalDepreciacaoAcumuladaUGE), CellValues.String, 3));
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", totalValorAtualUGE), CellValues.String, 3));
            sd.Append(linha);

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:B{0}", numeroLinhaCelulaInicialASerMesclada)) });

            MontaEInsereLinhaDeRodapeComDataDeGeracao();

            celulasMescladas.Append(new MergeCell() { Reference = new StringValue(string.Format("A{0}:E{0}", numeroLinhaCelulaInicialASerMesclada + 1)) });

            AdicionaObjetoColuna(1, 5, 30);

            ws.Append(sd);

            wsp.Worksheet = ws;
            wsp.Worksheet.Save();

            wsp.Worksheet.InsertAfter(celulasMescladas, wsp.Worksheet.Elements<SheetData>().First());

            InsereFolha("Totalmente depreciados");
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

        private void MontaEInsereLinhaDeCabecalhoComNomeDoRelatorio()
        {
            linha = new Row();

            linha.Append(ConstructCell("SAM - Sistema de Administração de Materiais - Módulo Patrimônio", CellValues.String, 1));
            sd.Append(linha);

            linha = new Row();

            linha.Append(ConstructCell("RESUMO CONSOLIDADO DO INVENTÁRIO DE BENS MÓVEIS", CellValues.String, 1));
            sd.Append(linha);
        }

        private void MontaEInsereLinhaDeCabecalhoComInformacoesDoRelatorio()
        {
            linha = new Row();

            linha.Append(ConstructCell(UGE, CellValues.String, 1));
            sd.Append(linha);

            linha = new Row();

            linha.Append(ConstructCell(string.Format("Mês de referência: {0}", MesRef), CellValues.String, 1));
            sd.Append(linha);
        }

        private void MontaEInsereLinhaDeRodapeComDataDeGeracao()
        {
            linha = new Row();
            linha.Append(ConstructCell(string.Format(CultureInfo.GetCultureInfo("pt-BR"), "planilha gerada na data: {0:G}", DataGeracao), CellValues.String, 6));
            sd.Append(linha);
        }

        private void InsereFolha(string nome) {
            Sheet folha = new Sheet();
            folha.Name = nome;
            folha.SheetId = IdFolha++;
            folha.Id = wbp.GetIdOfPart(wsp);
            sheets.Append(folha);
        }

        private void AdicionaObjetoColuna(UInt32Value min, UInt32Value max, DoubleValue width) {
            //inicialmente serve para ajustar a largura da coluna
            Columns columns = new Columns();
            columns.Append(new Column() { Min = min, Max = max, Width = width, CustomWidth = true });
            ws.Append(columns);
        }

        private Row MontaLinhaComNomeDasColunas(string[] nomes) {
            linha = new Row();

            foreach (string nome in nomes) {
                linha.Append(ConstructCell(nome, CellValues.String,2));
            }

            return linha;
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
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "b3b3b3" } } ) { PatternType = PatternValues.Solid })
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
                    new CellFormat() { FontId = 1, FillId = 2, BorderId = 1, Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Center }, ApplyFill = true}, //2 - Nome das colunas
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Center } }, //3 - Coluna com as informações
                    new CellFormat() { FontId = 1, FillId = 2, BorderId = 1, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right } }, //4 - Células com a descrição totais
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right, WrapText = true } }, //5 - Células de aviso em vermelho
                    new CellFormat() { FontId = 3, FillId = 0, BorderId = 0, Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Right } }, //6 - Célula com a data de geração da planilha
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 1, Alignment = new Alignment() { Vertical = VerticalAlignmentValues.Center } } //7 - Contas desativadas (usados no primeiro quadrante)
                );

            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }
        #endregion 
    }
}