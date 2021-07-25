SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SAM_COPIA_DEPRECIACAO_BPS_MESMO_DADOS_INCORPORACAO_EM_LOTE')
	DROP PROCEDURE [dbo].[SAM_COPIA_DEPRECIACAO_BPS_MESMO_DADOS_INCORPORACAO_EM_LOTE]
GO

CREATE PROCEDURE [dbo].[SAM_COPIA_DEPRECIACAO_BPS_MESMO_DADOS_INCORPORACAO_EM_LOTE]
(
	@AssetStartId int,
	@AssetStartIdACopiar int
) AS
BEGIN

	insert into MonthlyDepreciation
	(
       [AssetStartId]
      ,[ManagerUnitId]
      ,[MaterialItemCode]
      ,[InitialId]
      ,[AcquisitionDate]
      ,[CurrentDate]
      ,[DateIncorporation]
      ,[LifeCycle]
      ,[CurrentMonth]
      ,[ValueAcquisition]
      ,[CurrentValue]
      ,[ResidualValue]
      ,[RateDepreciationMonthly]
      ,[AccumulatedDepreciation]
      ,[UnfoldingValue]
      ,[Decree]
      ,[ManagerUnitTransferId]
      ,[MonthlyDepreciationId]
      ,[MesAbertoDoAceite]
      ,[QtdLinhaRepetida]
	)
	select 
	   @AssetStartId
      ,[ManagerUnitId]
      ,[MaterialItemCode]
      ,[InitialId]
      ,[AcquisitionDate]
      ,[CurrentDate]
      ,[DateIncorporation]
      ,[LifeCycle]
      ,[CurrentMonth]
      ,[ValueAcquisition]
      ,[CurrentValue]
      ,[ResidualValue]
      ,[RateDepreciationMonthly]
      ,[AccumulatedDepreciation]
      ,[UnfoldingValue]
      ,[Decree]
      ,[ManagerUnitTransferId]
      ,[MonthlyDepreciationId]
      ,[MesAbertoDoAceite]
      ,[QtdLinhaRepetida]
	  from MonthlyDepreciation
	  where AssetStartId = @AssetStartIdACopiar;
END
GO

GRANT EXECUTE ON [dbo].[SAM_COPIA_DEPRECIACAO_BPS_MESMO_DADOS_INCORPORACAO_EM_LOTE] TO [USUSAM]
GO