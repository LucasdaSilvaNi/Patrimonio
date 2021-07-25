SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PRC_RELATORIO__BP_TOTALMENTE_DEPRECIADO')
DROP PROCEDURE [dbo].[PRC_RELATORIO__BP_TOTALMENTE_DEPRECIADO]  
GO


CREATE PROCEDURE [dbo].[PRC_RELATORIO__BP_TOTALMENTE_DEPRECIADO]
(
	  @ManagerUnitId INT
) AS
BEGIN
SELECT
		  [viewBPsTotalmenteDepreciados].[ContaContabilDepreciacao]				AS 'ContaContabilDepreciacao'				/*CAMPO RELATORIO*/
		, [viewBPsTotalmenteDepreciados].[ContaContabilDepreciacaoDescricao]	AS 'ContaContabilDepreciacaoDescricao'		/*CAMPO RELATORIO*/
		, [viewBPsTotalmenteDepreciados].[ContaContabil]						AS 'ContaContabil'							/*CAMPO RELATORIO*/
		, [viewBPsTotalmenteDepreciados].[ContaContabilDescricao]				AS 'ContaContabilDescricao'					/*CAMPO RELATORIO*/
		, SUM([viewBPsTotalmenteDepreciados].[ValorAquisicao])					AS 'ValorAquisicao'							/*CAMPO RELATORIO*/
		, SUM([viewBPsTotalmenteDepreciados].[DepreciacaoAcumulada])			AS 'DepreciacaoAcumulada'					/*CAMPO RELATORIO*/
		, SUM([viewBPsTotalmenteDepreciados].[ValorAtual])						AS 'ValorAtual'								/*CAMPO RELATORIO*/
FROM VW_SAM_PATRIMONIO__BP_TOTALMENTE_DEPRECIADO viewBPsTotalmenteDepreciados
WHERE 
	[viewBPsTotalmenteDepreciados].[ManagerUnitId] = @ManagerUnitId
GROUP BY
		  [viewBPsTotalmenteDepreciados].[ContaContabilDepreciacao]
		, [viewBPsTotalmenteDepreciados].[ContaContabilDepreciacaoDescricao]
		, [viewBPsTotalmenteDepreciados].[ContaContabil]
		, [viewBPsTotalmenteDepreciados].[ContaContabilDescricao]
END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO__BP_TOTALMENTE_DEPRECIADO] TO [ususamweb]
GO