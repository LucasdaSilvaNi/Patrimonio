using SAM.Web.Models;
using SAM.Web.Common.Enum;
using System;
using System.Linq;
using System.Data.Entity;

namespace SAM.Web.Incorporacao
{
    public class DepreciacaoNaIncorporacao
    {
        private SAMContext contexto;

        public DepreciacaoNaIncorporacao() { }

        public DepreciacaoNaIncorporacao(SAMContext contexto) {
            this.contexto = contexto;
        }

        public void Deprecia(Asset bp, AssetMovements incorporacao)
        {
            if(contexto == null)
                contexto = new SAMContext();

            if (BPPrecisaDepreciar(bp,incorporacao)) {
                bp.AssetStartId = bp.Id;
                bp.DepreciationAmountStart = bp.ValueAcquisition;
                bp.DepreciationDateStart = bp.AcquisitionDate;

                incorporacao.MonthlyDepreciationId = bp.AssetStartId;
                contexto.Entry(bp).State = EntityState.Modified;
                contexto.Entry(incorporacao).State = EntityState.Modified;
                contexto.SaveChanges();


                if (bp.flagDecreto == true)
                {
                    CalculoDeDecreto(bp, incorporacao);
                }
                else
                {
                    Calculo(bp, incorporacao);
                }

            }
        }

        public void CopiaDepreciacaoNaIncorporacaoEmLote(Asset bp, AssetMovements incorporacao, int AssetStartIdASerCopiado)
        {
            if (contexto == null)
                contexto = new SAMContext();

            if (BPPrecisaDepreciar(bp, incorporacao))
            {
                bp.AssetStartId = bp.Id;
                bp.DepreciationAmountStart = bp.ValueAcquisition;
                bp.DepreciationDateStart = bp.AcquisitionDate;

                incorporacao.MonthlyDepreciationId = bp.AssetStartId;
                contexto.Entry(bp).State = EntityState.Modified;
                contexto.Entry(incorporacao).State = EntityState.Modified;
                contexto.SaveChanges();

                string instrucao = String.Format("EXEC [SAM_COPIA_DEPRECIACAO_BPS_MESMO_DADOS_INCORPORACAO_EM_LOTE] @AssetStartId = {0}, @AssetStartIdACopiar = {1}"
                                                 ,bp.AssetStartId, AssetStartIdASerCopiado);

                contexto.Database.ExecuteSqlCommand(instrucao);
            }
        }

        private void Calculo(Asset bp, AssetMovements incorporacao) {

            bp.DepreciationAmountStart = bp.ValueAcquisition;
            bp.DepreciationDateStart = bp.AcquisitionDate;
            contexto.Entry(bp).State = EntityState.Modified;
            contexto.SaveChanges();

            DateTime DataLinhaZero;
            if (bp.AcquisitionDate.Month != bp.AcquisitionDate.AddDays(1).Month)
            {
                DataLinhaZero = bp.AcquisitionDate.AddDays(-1);
            }
            else {
                DataLinhaZero = bp.AcquisitionDate;
            }

            MaterialGroup grupo = contexto.MaterialGroups.Where(g => g.Code == bp.MaterialGroupCode).FirstOrDefault();

            decimal ValorResidual = Math.Round((bp.ValueAcquisition * grupo.ResidualValue) / 100, 2);
            decimal DepreciacaoMensal = (decimal)(Math.Truncate((double)((bp.ValueAcquisition - ValorResidual) / grupo.LifeCycle) * 100.0) / 100.0);
            decimal ValorDeDesdobro = 0;

            MonthlyDepreciation depreciacao = new MonthlyDepreciation();
            depreciacao.AssetStartId = bp.Id;
            depreciacao.ManagerUnitId = incorporacao.ManagerUnitId;
            depreciacao.MaterialItemCode = bp.MaterialItemCode;
            depreciacao.InitialId = bp.InitialId;
            depreciacao.AcquisitionDate = bp.AcquisitionDate;
            depreciacao.CurrentDate = DataLinhaZero;
            depreciacao.DateIncorporation = incorporacao.MovimentDate;
            depreciacao.LifeCycle = (short)grupo.LifeCycle;
            depreciacao.CurrentMonth = 0;
            depreciacao.ValueAcquisition = bp.ValueAcquisition;
            depreciacao.CurrentValue = bp.ValueAcquisition;
            depreciacao.ResidualValue = ValorResidual;
            depreciacao.RateDepreciationMonthly = DepreciacaoMensal;
            depreciacao.AccumulatedDepreciation = 0;
            depreciacao.UnfoldingValue = ValorDeDesdobro;
            depreciacao.Decree = bp.flagDecreto == true;
            depreciacao.MonthlyDepreciationId = bp.AssetStartId;
            depreciacao.QtdLinhaRepetida = 0;

            contexto.MonthlyDepreciations.Add(depreciacao);
            contexto.SalvaSemNecessiadeDeGravarLog();

            DateTime MesRefDataIncorporacao = new DateTime(incorporacao.MovimentDate.Year, incorporacao.MovimentDate.Month, 1);

            while (depreciacao.CurrentMonth < grupo.LifeCycle &&
                 depreciacao.CurrentDate < MesRefDataIncorporacao)
            {
                depreciacao.CurrentMonth += 1;
                depreciacao.CurrentDate = depreciacao.CurrentDate.AddMonths(1);

                
                if (depreciacao.CurrentMonth < 3)
                {
                    if (depreciacao.CurrentMonth == 1)
                    {
                        if(DataLinhaZero.Day != bp.AcquisitionDate.Day)
                            depreciacao.CurrentDate = bp.AcquisitionDate;
                        else
                            depreciacao.CurrentDate = bp.AcquisitionDate.AddDays(1);
                    }
                    else
                    {
                        if (depreciacao.CurrentDate.Day != 11)
                        {
                            depreciacao.CurrentDate = new DateTime(depreciacao.CurrentDate.Year, depreciacao.CurrentDate.Month, 11);
                        }
                    }
                }

                if (depreciacao.CurrentDate < MesRefDataIncorporacao)
                {
                    depreciacao.CurrentValue -= depreciacao.RateDepreciationMonthly;
                    depreciacao.AccumulatedDepreciation += depreciacao.RateDepreciationMonthly;

                    if (depreciacao.CurrentMonth == depreciacao.LifeCycle
                       && depreciacao.CurrentValue != depreciacao.ResidualValue)
                    {
                        if (depreciacao.CurrentValue > depreciacao.ResidualValue)
                        {
                            ValorDeDesdobro = depreciacao.CurrentValue - depreciacao.ResidualValue;
                            depreciacao.AccumulatedDepreciation += ValorDeDesdobro;
                            depreciacao.RateDepreciationMonthly += ValorDeDesdobro;
                            depreciacao.CurrentValue -= ValorDeDesdobro;
                            depreciacao.UnfoldingValue = ValorDeDesdobro;
                        }
                        else
                        {
                            ValorDeDesdobro = depreciacao.ResidualValue - depreciacao.CurrentValue;
                            depreciacao.AccumulatedDepreciation -= ValorDeDesdobro;
                            depreciacao.RateDepreciationMonthly -= ValorDeDesdobro;
                            depreciacao.CurrentValue += ValorDeDesdobro;
                            depreciacao.UnfoldingValue = ValorDeDesdobro;
                        }

                    }

                    contexto.MonthlyDepreciations.Add(depreciacao);
                    contexto.SalvaSemNecessiadeDeGravarLog();
                }
            }
        }


        private void CalculoDeDecreto(Asset bp, AssetMovements incorporacao)
        {
            MaterialGroup grupo = contexto.MaterialGroups.Where(g => g.Code == bp.MaterialGroupCode).FirstOrDefault();

            decimal ValorResidual = Math.Round((bp.ValueAcquisition * grupo.ResidualValue) / 100, 2);
            decimal DepreciacaoMensal = Math.Round((bp.ValueAcquisition - ValorResidual) / grupo.LifeCycle, 2);
            decimal ValorAtual = bp.ValueAcquisition;
            decimal ValorDeDesdobro = 0;

            DateTime DataAquisicao = bp.AcquisitionDate;
            DateTime DataMesRefMaisUm = incorporacao.MovimentDate.AddMonths(1);
            int CicloDeVida = 0;


            bool GeraLinhaDepreciacaoAMais = false;
            while (DataAquisicao < DataMesRefMaisUm) {
                CicloDeVida++;
                DataAquisicao = DataAquisicao.AddMonths(1);
            }

            if (CicloDeVida > grupo.LifeCycle)
            {
                CicloDeVida = grupo.LifeCycle + 1;
                DepreciacaoMensal = 0;
            }
            else if(CicloDeVida == grupo.LifeCycle)
            {
                GeraLinhaDepreciacaoAMais = true;
            }

            MonthlyDepreciation depreciacao = new MonthlyDepreciation();
            depreciacao.AssetStartId = bp.Id;
            depreciacao.NumberIdentification = bp.NumberIdentification;
            depreciacao.ManagerUnitId = incorporacao.ManagerUnitId;
            depreciacao.MaterialItemCode = bp.MaterialItemCode;
            depreciacao.InitialId = bp.InitialId;
            depreciacao.AcquisitionDate = bp.AcquisitionDate;
            depreciacao.CurrentDate = incorporacao.MovimentDate;
            depreciacao.DateIncorporation = incorporacao.MovimentDate;
            depreciacao.LifeCycle = (short)grupo.LifeCycle;
            depreciacao.CurrentMonth = (short)CicloDeVida;
            depreciacao.ValueAcquisition = bp.ValueAcquisition;
            depreciacao.CurrentValue = ValorAtual;
            depreciacao.ResidualValue = ValorResidual;
            depreciacao.RateDepreciationMonthly = DepreciacaoMensal;
            depreciacao.AccumulatedDepreciation = DepreciacaoMensal;
            depreciacao.UnfoldingValue = ValorDeDesdobro;
            depreciacao.Decree = true;
            depreciacao.MonthlyDepreciationId = bp.AssetStartId;
            depreciacao.QtdLinhaRepetida = 0;

            contexto.MonthlyDepreciations.Add(depreciacao);
            contexto.SalvaSemNecessiadeDeGravarLog();

            if (GeraLinhaDepreciacaoAMais) {
                ++depreciacao.CurrentMonth;

                depreciacao.CurrentValue = depreciacao.ResidualValue;
                depreciacao.AccumulatedDepreciation += depreciacao.RateDepreciationMonthly;
                depreciacao.RateDepreciationMonthly = 0;
                depreciacao.CurrentDate = depreciacao.CurrentDate.AddMonths(1);

                contexto.MonthlyDepreciations.Add(depreciacao);
                contexto.SalvaSemNecessiadeDeGravarLog();
            }

        }

        private bool BPPrecisaDepreciar(Asset bp, AssetMovements incorporacao) {
            DateTime MesRefDataIncorporacao = new DateTime(incorporacao.MovimentDate.Year, incorporacao.MovimentDate.Month, 1);

            if (bp.flagAcervo != true
              && bp.flagTerceiro != true
              && bp.flagAnimalNaoServico != true
              && bp.flagDepreciaAcumulada == 1
              && (bp.flagVerificado == null || bp.flagVerificado == 0))
            {

                if ((incorporacao.MovementTypeId == (int)EnumMovimentType.IncorpDoacaoIntraNoEstado ||
                   incorporacao.MovementTypeId == (int)EnumMovimentType.IncorpTransferenciaOutroOrgaoPatrimoniado ||
                   incorporacao.MovementTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGEDoacao ||
                   incorporacao.MovementTypeId == (int)EnumMovimentType.IncorpRecebimentoDeInservivelUGETranferencia)
                   && incorporacao.SourceDestiny_ManagerUnitId == null)
                {
                    return false;
                }
                if (incorporacao.MovementTypeId == (int)EnumMovimentType.IncorpTransferenciaMesmoOrgaoPatrimoniado) {
                    return false;
                }
                else
                {
                    return bp.AcquisitionDate < MesRefDataIncorporacao;
                }
            }

            return false;
        }
    }
}