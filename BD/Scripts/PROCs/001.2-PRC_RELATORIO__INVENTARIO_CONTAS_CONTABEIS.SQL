SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PRC_RELATORIO__INVENTARIO_CONTAS_CONTABEIS')
DROP PROCEDURE [dbo].[PRC_RELATORIO__INVENTARIO_CONTAS_CONTABEIS]  
GO

CREATE PROCEDURE [dbo].[PRC_RELATORIO__INVENTARIO_CONTAS_CONTABEIS]
(
	  @ClosingYearMonthReference VARCHAR(6),
	  @ManagerUnitId INT
) AS
BEGIN
--VARIAVEIS INTERNAS (DATAS)
DECLARE @LastClosingYearMonthReference VARCHAR(6) = NULL;
DECLARE @NextMonthReferenceFirstDate VARCHAR(10) = NULL;

SELECT
		  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]									AS 'ContaContabilDepreciacao'				/*CAMPO RELATORIO*/
		, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]						AS 'ContaContabilDepreciacaoDescricao'		/*CAMPO RELATORIO*/
		, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoStatus]							AS 'ContaContabilDepreciacaoStatus'
		, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]											AS 'ContaContabil'							/*CAMPO RELATORIO*/
		, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]									AS 'ContaContabilDescricao'					/*CAMPO RELATORIO*/
		, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]										AS 'ContaContabilStatus'
		, SUM(ISNULL([visaoGeral_UGE__MESREF_ATUAL].[ValorContabil], 0.00))							AS 'ValorContabil'							/*CAMPO RELATORIO*/
		, SUM(ISNULL([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoAcumuladaAteMesAnterior], 0.00))	AS 'DepreciacaoAcumuladaAteMesAnterior'		/*CAMPO RELATORIO*/
		, SUM(ISNULL([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoNoMes], 0.00))						AS 'DepreciacaoNoMes'						/*CAMPO RELATORIO*/
		, SUM(ISNULL([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoAcumulada], 0.00))					AS 'DepreciacaoAcumulada'					/*CAMPO RELATORIO*/
FROM [dbo].[VW__VISAO_GERAL__UGE_MESREF_ATUAL] visaoGeral_UGE__MESREF_ATUAL
WHERE 
		[visaoGeral_UGE__MESREF_ATUAL].[ManagerUnitId] = @ManagerUnitId
GROUP BY
		  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]
		, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]
		, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoStatus]
		, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]
		, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]
		, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]
END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO__INVENTARIO_CONTAS_CONTABEIS] TO [ususamweb]
GO