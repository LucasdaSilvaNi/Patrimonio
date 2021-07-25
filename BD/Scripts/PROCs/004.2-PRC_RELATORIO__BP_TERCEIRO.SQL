SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PRC_RELATORIO__BP_TERCEIRO')
DROP PROCEDURE [dbo].[PRC_RELATORIO__BP_TERCEIRO]  
GO


CREATE PROCEDURE [dbo].[PRC_RELATORIO__BP_TERCEIRO]
(
	  @ManagerUnitId INT
) AS
BEGIN
SELECT
	     [viewBPsDeTerceiro].[Responsavel]					AS 'Responsavel'		/*CAMPO RELATORIO*/
	   , SUM([viewBPsDeTerceiro].[ValorAquisicao])			AS 'ValorAquisicao'		/*CAMPO RELATORIO*/
FROM VW_SAM_PATRIMONIO__BP_TERCEIRO viewBPsDeTerceiro
WHERE 
	[viewBPsDeTerceiro].[ManagerUnitId] = @ManagerUnitId
GROUP BY
		  [viewBPsDeTerceiro].[Responsavel]
END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO__BP_TERCEIRO] TO [ususamweb]
GO