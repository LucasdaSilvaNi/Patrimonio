SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'REPORT_RESUMO_INVENTARIO_BP')
BEGIN
    DROP PROCEDURE [dbo].[REPORT_RESUMO_INVENTARIO_BP]
END
GO

CREATE PROCEDURE [dbo].[REPORT_RESUMO_INVENTARIO_BP]
(          
 @idOrgao INT = null,            
 @idUo  INT = null,            
 @idUge  INT = null,            
 @mesRef VARCHAR(6) = null
)           
AS 
BEGIN
	SET DATEFORMAT DMY;


	DECLARE @InstitutionId INT = NULL;
	DECLARE @BudgetUnitId INT = NULL;
	DECLARE @ManagerUnitId INT = NULL;

	DECLARE @ClosingYearMonthReference VARCHAR(6) = NULL;
	DECLARE @ClosingYearMonthReferenceFirstDate VARCHAR(10) = NULL;
	DECLARE @LastClosingYearMonthReference VARCHAR(6) = NULL;
	DECLARE @NextMonthReferenceFirstDate VARCHAR(10) = NULL;





	SET @ClosingYearMonthReference = @mesRef;
	SET @InstitutionId = @idOrgao;
	SET @BudgetUnitId  = @idUo;
	SET @ManagerUnitId = @idUge;


	IF (@ClosingYearMonthReference IS NULL)
		SET @ClosingYearMonthReference = (SELECT ManagmentUnit_YearMonthReference FROM ManagerUnit WHERE Id = @ManagerUnitId);

	SET @ClosingYearMonthReferenceFirstDate = (CONVERT(VARCHAR(10), CONVERT(DATE, DATEADD(month, 0, (SUBSTRING(@ClosingYearMonthReference, 1, 4) + SUBSTRING(@ClosingYearMonthReference, 5, 2) + '01'))),105))
	SET @NextMonthReferenceFirstDate = (CONVERT(VARCHAR(10), CONVERT(DATE, DATEADD(month, 1, (SUBSTRING(@ClosingYearMonthReference, 1, 4) + SUBSTRING(@ClosingYearMonthReference, 5, 2) + '01'))),105))
	(
		SELECT
				  ugeSIAFEM.Code + '-' + ugeSIAFEM.[Description] as UGE
				, contaContabil.BookAccount
				, contaContabil.[Description]
				, SUM(ISNULL(tabelaDepreciacaoBP.CurrentValue, 0.00)) AS Valor
				, SUM(ISNULL(tabelaDepreciacaoBP.ValueAcquisition, 0.00)) AS ValorAquisicao
				, SUM(ISNULL(tabelaDepreciacaoBP.AccumulatedDepreciation, 0.00)) AS DepreciationAccumulated
				, contaContabil.[Status] AS STATUS_CONTA

				, contaContabilDepreciacao.Code AS ContaDepreciacaoCode
				, contaContabilDepreciacao.Description AS ContaDepreciacaoDescricao


				, 0.00 AS SaldoAnterior
				, SUM(ISNULL(tabelaDepreciacaoBP.RateDepreciationMonthly, 0.00)) as DepreciacaoNoMes
				, SUM(ISNULL(tabelaDepreciacaoBP.AccumulatedDepreciation, 0.00)) AS DepreciacaoAcumulada
		FROM
			MonthlyDepreciation tabelaDepreciacaoBP WITH(NOLOCK)
		INNER JOIN Asset BP WITH(NOLOCK) ON tabelaDepreciacaoBP.AssetStartId = BP.Id
		INNER JOIN AssetMovements movimentacaoBP WITH(NOLOCK) ON BP.Id = movimentacaoBP.AssetId
		INNER JOIN ManagerUnit ugeSIAFEM WITH(NOLOCK) ON tabelaDepreciacaoBP.ManagerUnitId = ugeSIAFEM.Id
		INNER JOIN AuxiliaryAccount contaContabil WITH(NOLOCK) ON movimentacaoBP.AuxiliaryAccountId = contaContabil.Id
		INNER JOIN DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) ON contaContabil.DepreciationAccountId = contaContabilDepreciacao.Id
		WHERE 
			(	movimentacaoBP.ManagerUnitId = @ManagerUnitId 
			AND movimentacaoBP.MovimentDate BETWEEN @ClosingYearMonthReferenceFirstDate AND @NextMonthReferenceFirstDate
			AND movimentacaoBP.MovementTypeId NOT IN (23, 24) --BPs Pendentes
			AND NOT (flagVerificado IS NULL AND flagDepreciaAcumulada IS NULL) --VERIFICAR MOTIVO DIFERENCA DEPOIS
			AND BP.[Status] = 1)

		GROUP BY 
			  ugeSIAFEM.Code
			, ugeSIAFEM.[Description]
			, contaContabilDepreciacao.Code
			, contaContabilDepreciacao.[Description]
			, contaContabil.BookAccount
			, contaContabil.[Description]
			, contaContabil.[Status]
		)/*
		UNION ALL
		--BPs TOTALMENTE DEPRECIADOS
		(
		SELECT
				DISTINCT
				  ugeSIAFEM.Code + '-' + ugeSIAFEM.[Description] as UGE
				, contaContabil.BookAccount
				, contaContabil.[Description]
				, SUM(ISNULL(tabelaDepreciacaoBP.CurrentValue, 0.00)) AS Valor
				, SUM(ISNULL(tabelaDepreciacaoBP.ValueAcquisition, 0.00)) AS ValorAquisicao
				, SUM(ISNULL(tabelaDepreciacaoBP.AccumulatedDepreciation, 0.00)) AS DepreciationAccumulated
				, contaContabil.[Status] AS STATUS_CONTA

				, contaContabilDepreciacao.Code AS ContaDepreciacaoCode
				, contaContabilDepreciacao.Description AS ContaDepreciacaoDescricao

				, 0.00 AS SaldoAnterior
				, 0.00 as DepreciacaoNoMes
				, SUM(ISNULL(tabelaDepreciacaoBP.AccumulatedDepreciation, 0.00)) AS DepreciacaoAcumulada
		FROM
			MonthlyDepreciation tabelaDepreciacaoBP WITH(NOLOCK)
		INNER JOIN Asset BP WITH(NOLOCK) ON tabelaDepreciacaoBP.AssetStartId = BP.Id
		INNER JOIN AssetMovements movimentacaoBP WITH(NOLOCK) ON BP.Id = movimentacaoBP.AssetId
		INNER JOIN ManagerUnit ugeSIAFEM WITH(NOLOCK) ON tabelaDepreciacaoBP.ManagerUnitId = ugeSIAFEM.Id
		INNER JOIN AuxiliaryAccount contaContabil WITH(NOLOCK) ON movimentacaoBP.AuxiliaryAccountId = contaContabil.Id
		INNER JOIN DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) ON contaContabil.DepreciationAccountId = contaContabilDepreciacao.Id
		WHERE 
			--tabelaDepreciacaoBP.CurrentDate BETWEEN '01-05-2019' AND '01-06-2019'
			--tabelaDepreciacaoBP.CurrentDate BETWEEN @ClosingYearMonthReferenceFirstDate AND @NextMonthReferenceFirstDate
		     ugeSIAFEM.Id = @ManagerUnitId
		AND (contaContabil.[Status] = 1 AND contaContabilDepreciacao.[Status] = 1)
		--AND tabelaDepreciacaoBP.ManagerUnitTransferId IS NOT NULL

		AND	(BP.flagVerificado IS NULL AND BP.flagDepreciaAcumulada = 1)
		AND	(BP.[Status] = 1)
		--AND (movimentacaoBP.InstitutionId = @InstitutionId OR @InstitutionId IS NULL)
		--AND (movimentacaoBP.BudgetUnitId = @BudgetUnitId OR @BudgetUnitId IS NULL)
		--AND (movimentacaoBP.ManagerUnitId = @ManagerUnitId OR @ManagerUnitId IS NULL)
		--AND (contaContabil.[Status] = 1)
		AND (tabelaDepreciacaoBP.CurrentMonth = tabelaDepreciacaoBP.LifeCycle) --TOTALMENTE DEPRECIADO
		GROUP BY 
			  ugeSIAFEM.Code
			, ugeSIAFEM.[Description]
			, contaContabilDepreciacao.Code
			, contaContabilDepreciacao.[Description]
			, contaContabil.BookAccount
			, contaContabil.[Description]
			, contaContabil.[Status]


			, BP.Id
			, BP.InitialName					
			, BP.NumberIdentification			
			, movimentacaoBP.MovimentDate		
			, BP.[Status]						
			, BP.monthUsed						
			, tabelaDepreciacaoBP.CurrentMonth	
			, tabelaDepreciacaoBP.LifeCycle		

	)*/

	PRINT 'DEPRECIADOS NO MES'
	PRINT @ClosingYearMonthReference
	PRINT @ClosingYearMonthReferenceFirstDate
	PRINT @NextMonthReferenceFirstDate
END
GO