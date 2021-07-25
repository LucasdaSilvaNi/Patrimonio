SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PRC_RELATORIO__BP_ACERVO')
DROP PROCEDURE [dbo].[PRC_RELATORIO__BP_ACERVO]
GO


CREATE PROCEDURE [dbo].[PRC_RELATORIO__BP_ACERVO]
(
	  @ManagerUnitId INT
) AS
BEGIN
SELECT
	     [viewBPsDeAcervo].[Responsavel]			AS 'Responsavel'		/*CAMPO RELATORIO*/
	   , SUM([viewBPsDeAcervo].[ValorAquisicao])	AS 'ValorAquisicao'		/*CAMPO RELATORIO*/
	   , SUM([viewBPsDeAcervo].[ValorAtual])		AS 'ValorAtual'			/*CAMPO RELATORIO*/
FROM VW_SAM_PATRIMONIO__BP_ACERVO viewBPsDeAcervo
WHERE 
	[viewBPsDeAcervo].[ManagerUnitId] = @ManagerUnitId
GROUP BY
		  [viewBPsDeAcervo].[Responsavel]
END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO__BP_ACERVO] TO [ususamweb]
GO