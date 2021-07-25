IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('dbo.VW_SAM_PATRIMONIO__BP_TOTALMENTE_DEPRECIADO') AND type = 'V') 
	DROP VIEW VW_SAM_PATRIMONIO__BP_TOTALMENTE_DEPRECIADO
GO

CREATE VIEW VW_SAM_PATRIMONIO__BP_TOTALMENTE_DEPRECIADO
AS
SELECT
		  [bemPatrimonial].[InitialName]								AS 'Sigla'
		, [bemPatrimonial].[NumberIdentification]						AS 'Chapa'
		, [bemPatrimonial].[Status]										AS 'Status'
		, [contaContabilDepreciacao].[Code]								AS 'ContaContabilDepreciacao'				/*CAMPO RELATORIO*/
		, [contaContabilDepreciacao].[Description]						AS 'ContaContabilDepreciacaoDescricao'		/*CAMPO RELATORIO*/
		, [contaContabil].[BookAccount]									AS 'ContaContabil'							/*CAMPO RELATORIO*/
		, [contaContabil].[Description]									AS 'ContaContabilDescricao'					/*CAMPO RELATORIO*/
		, [bemPatrimonial].[ValueAcquisition]							AS 'ValorAquisicao'							/*CAMPO RELATORIO*/
		, [bemPatrimonial].[DepreciationAccumulated]					AS 'DepreciacaoAcumulada'					/*CAMPO RELATORIO*/
		, [bemPatrimonial].[ValueUpdate]								AS 'ValorAtual'								/*CAMPO RELATORIO*/
		, [movimentacaoBP].[MovimentDate]								AS 'DataUltimaMovimentacao'
		, [movimentacaoBP].[ManagerUnitId]								AS 'ManagerUnitId'
		, CASE WHEN ([bemPatrimonial].[monthUsed] >= 0) THEN
			CASE
				WHEN (([bemPatrimonial].[flagDecreto] IS NULL) AND ([bemPatrimonial].[monthUsed] >= 0)) THEN [bemPatrimonial].[ValueAcquisition]
				WHEN (([bemPatrimonial].[flagDecreto] IS NOT NULL OR [bemPatrimonial].[flagDecreto] = 0)) THEN [bemPatrimonial].[ValueUpdate]
			 END
		END															
																		AS 'ValorContabil'
	   , [bemPatrimonial].[monthUsed]
	   , [bemPatrimonial].[LifeCycle]

	   /* CAMPOS NOVA TABELA DEPRECIACAO MENSAL - 'MonthlyDepreciation' */
	   , [dadosDepreciacaoMensal].[ValueAcquisition]					AS '_ValorAquisicao'						/*CAMPO RELATORIO*/
	   , [dadosDepreciacaoMensal].[AccumulatedDepreciation]				AS '_DepreciacaoAcumulada'					/*CAMPO RELATORIO*/
	   , [dadosDepreciacaoMensal].[CurrentValue]						AS '_ValorAtual'							/*CAMPO RELATORIO*/
	   , [dadosDepreciacaoMensal].[CurrentMonth]						AS '_MesesEmUso'
	   , [dadosDepreciacaoMensal].[LifeCycle]							AS '_MesesVidaUtil'
	   , [dadosDepreciacaoMensal].[CurrentValue]						AS '_ValorContabil'
FROM [dbo].[Asset] [bemPatrimonial] WITH(NOLOCK) 
INNER JOIN [dbo].[AssetMovements] [movimentacaoBP] WITH(NOLOCK) ON [movimentacaoBP].[AssetId] = [bemPatrimonial].[Id]
INNER JOIN [dbo].[AuxiliaryAccount] [contaContabil] WITH(NOLOCK) ON [movimentacaoBP].[AuxiliaryAccountId] = [contaContabil].[Id]
INNER JOIN [dbo].[DepreciationAccount] [contaContabilDepreciacao] WITH(NOLOCK) ON [contaContabil].[DepreciationAccountId] = [contaContabilDepreciacao].[Id]
LEFT JOIN  [dbo].[MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) ON [bemPatrimonial].[Id] = [dadosDepreciacaoMensal].[AssetStartId]
WHERE 
	[movimentacaoBP].[Id] IN (SELECT 
									TOP 1 [mv].[Id] 
							  FROM [dbo].[AssetMovements] [mv] WITH(NOLOCK) 
							  WHERE [mv].[AssetId] = [bemPatrimonial].[Id] 
							  ORDER BY 
									[mv].[Id] DESC)
--EXCECOES
AND (     ([movimentacaoBP].MovementTypeId NOT IN (23, 24)) --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)
	  OR ([bemPatrimonial].[flagAcervo] IS NULL) --NAO CONTABILIZAR ACERVOS
	  OR ([bemPatrimonial].[flagTerceiro] IS NULL OR [movimentacaoBP].[MovementTypeId] NOT IN (27)) --NAO CONTABILIZAR 'BEM DE TERCEIROS' E 'COMODATOS DO TIPO 'RECEBIDOS''
	)

--FILTRO CONSULTA 'PADRAO' VISAO GERAL
AND ([bemPatrimonial].flagVerificado IS NULL AND [bemPatrimonial].[flagDepreciaAcumulada] = 1) --ATIVOS
AND  [bemPatrimonial].[Status] = 1

--BP TOTALMENTE DEPRECIADO
AND ([bemPatrimonial].[monthUsed] = [bemPatrimonial].[LifeCycle])
GO