SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PRC_RELATORIO__INCORPORACOES_BP__MESREF')
DROP PROCEDURE [dbo].[PRC_RELATORIO__INCORPORACOES_BP__MESREF]  
GO


CREATE PROCEDURE [dbo].[PRC_RELATORIO__INCORPORACOES_BP__MESREF]
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
	      [viewAquisicoesBPs].[TipoIncorporacao]					AS 'TipoIncorporacao'						/*CAMPO RELATORIO*/
		, [viewAquisicoesBPs].[ContaContabilDepreciacao]			AS 'ContaContabilDepreciacao'				/*CAMPO RELATORIO*/
		, [viewAquisicoesBPs].[ContaContabilDepreciacaoDescricao]	AS 'ContaContabilDepreciacaoDescricao'		/*CAMPO RELATORIO*/
		, [viewAquisicoesBPs].[ContaContabil]						AS 'ContaContabil'							/*CAMPO RELATORIO*/
		, [viewAquisicoesBPs].[ContaContabilDescricao]				AS 'ContaContabilDescricao'					/*CAMPO RELATORIO*/
		, SUM([viewAquisicoesBPs].[ValorAquisicao])					AS 'ValorAquisicao'							/*CAMPO RELATORIO*/
		, SUM([viewAquisicoesBPs].[DepreciacaoNoMes])				AS 'DepreciacaoNoMes'						/*CAMPO RELATORIO*/
		, SUM([viewAquisicoesBPs].[ValorAtual])						AS 'ValorAtual'								/*CAMPO RELATORIO*/
FROM VW_SAM_PATRIMONIO__INCORPORACOES_BPS viewAquisicoesBPs
WHERE 
	--FILTRO DE DATA (MES-REFERENCIA INFORMADO
	( [viewAquisicoesBPs].[DataUltimaMovimentacao] >= @ClosingYearMonthReferenceFirstDate AND [viewAquisicoesBPs].[DataUltimaMovimentacao] < @NextMonthReferenceFirstDate )

AND ([viewAquisicoesBPs].[ManagerUnitId] = @ManagerUnitId)
GROUP BY
		  [viewAquisicoesBPs].[TipoIncorporacao]
		, [viewAquisicoesBPs].[ContaContabilDepreciacao]
		, [viewAquisicoesBPs].[ContaContabilDepreciacaoDescricao]
		, [viewAquisicoesBPs].[ContaContabil]
		, [viewAquisicoesBPs].[ContaContabilDescricao]
END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO__INCORPORACOES_BP__MESREF] TO [ususamweb]
GO