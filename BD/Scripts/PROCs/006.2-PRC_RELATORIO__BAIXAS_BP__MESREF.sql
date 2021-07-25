SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PRC_RELATORIO__BAIXAS_BP__MESREF')
DROP PROCEDURE [dbo].[PRC_RELATORIO__BAIXAS_BP__MESREF]  
GO


CREATE PROCEDURE [dbo].[PRC_RELATORIO__BAIXAS_BP__MESREF]
(
	  @ClosingYearMonthReference VARCHAR(6)
	, @ManagerUnitId INT
) AS
BEGIN

--VARIAVEIS INTERNAS (INTERVALO DATAS)
DECLARE @ClosingYearMonthReferenceFirstDate VARCHAR(10) = NULL;
DECLARE @NextMonthReferenceFirstDate VARCHAR(10) = NULL;


IF (@ClosingYearMonthReference IS NULL)
	SET @ClosingYearMonthReference = (SELECT ManagmentUnit_YearMonthReference FROM ManagerUnit WHERE Id = @ManagerUnitId);

SET @ClosingYearMonthReferenceFirstDate = CONVERT(DATE, DATEADD(month, 0, (SUBSTRING(@ClosingYearMonthReference, 1, 4) + SUBSTRING(@ClosingYearMonthReference, 5, 2) + '01')))
SET @NextMonthReferenceFirstDate = CONVERT(DATE, DATEADD(month, 1, (SUBSTRING(@ClosingYearMonthReference, 1, 4) + SUBSTRING(@ClosingYearMonthReference, 5, 2) + '01')))


SELECT
	      [viewBaixasDeBPs].[TipoMovimentacao]						AS 'TipoMovimentacao'						/*CAMPO RELATORIO*/
		, [viewBaixasDeBPs].[ContaContabilDepreciacao]				AS 'ContaContabilDepreciacao'				/*CAMPO RELATORIO*/
		, [viewBaixasDeBPs].[ContaContabilDepreciacaoDescricao]		AS 'ContaContabilDepreciacaoDescricao'		/*CAMPO RELATORIO*/
		, [viewBaixasDeBPs].[ContaContabil]							AS 'ContaContabil'							/*CAMPO RELATORIO*/
		, [viewBaixasDeBPs].[ContaContabilDescricao]				AS 'ContaContabilDescricao'					/*CAMPO RELATORIO*/
		, SUM([viewBaixasDeBPs].[ValorAquisicao])					AS 'ValorAquisicao'							/*CAMPO RELATORIO*/
		, SUM([viewBaixasDeBPs].[DepreciacaoNoMes])					AS 'DepreciacaoNoMes'						/*CAMPO RELATORIO*/
		, SUM([viewBaixasDeBPs].[ValorAtual])						AS 'ValorAtual'								/*CAMPO RELATORIO*/
FROM VW_SAM_PATRIMONIO__BAIXAS_BPS viewBaixasDeBPs
WHERE 
	--FILTRO DE DATA (MES-REFERENCIA INFORMADO
	( [viewBaixasDeBPs].[DataUltimaMovimentacao] >= @ClosingYearMonthReferenceFirstDate AND [viewBaixasDeBPs].[DataUltimaMovimentacao] < @NextMonthReferenceFirstDate )

AND ([viewBaixasDeBPs].[ManagerUnitId] = @ManagerUnitId)
GROUP BY
		  [viewBaixasDeBPs].[TipoMovimentacao]
		, [viewBaixasDeBPs].[ContaContabilDepreciacao]
		, [viewBaixasDeBPs].[ContaContabilDepreciacaoDescricao]
		, [viewBaixasDeBPs].[ContaContabil]
		, [viewBaixasDeBPs].[ContaContabilDescricao]
END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO__BAIXAS_BP__MESREF] TO [ususamweb]
GO