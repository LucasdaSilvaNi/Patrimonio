IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('dbo.VW_SAM_PATRIMONIO__BP_ACERVO') AND type = 'V') 
	DROP VIEW VW_SAM_PATRIMONIO__BP_ACERVO
GO

CREATE VIEW VW_SAM_PATRIMONIO__BP_ACERVO
AS
SELECT
		  [bemPatrimonial].[InitialName]								AS 'Sigla'
	   ,  [bemPatrimonial].[NumberIdentification]						AS 'Chapa'
	   , ([responsavelBP].[CPF] + ' - ' + [responsavelBP].[Name])		AS 'Responsavel' 
	   ,  [bemPatrimonial].[ValueAcquisition]							AS 'ValorAquisicao'
	   ,  [bemPatrimonial].[ValueUpdate]								AS 'ValorAtual'
	   ,  [movimentacaoBP].[ManagerUnitId]								AS 'ManagerUnitId'
	   --,  [bemPatrimonial].[flagAcervo]

	   /* CAMPOS NOVA TABELA DEPRECIACAO MENSAL - 'MonthlyDepreciation' */
	   ,  [dadosDepreciacaoMensal].[ValueAcquisition]					AS '_ValorAquisicao'
	   ,  [dadosDepreciacaoMensal].[CurrentValue]						AS '_ValorAtual'
FROM [dbo].[Asset] [bemPatrimonial] WITH(NOLOCK) 
INNER JOIN [dbo].[AssetMovements] [movimentacaoBP] WITH(NOLOCK) ON [movimentacaoBP].[AssetId] = [bemPatrimonial].[Id]
INNER JOIN [dbo].[Responsible] [responsavelBP] WITH(NOLOCK) ON [movimentacaoBP].[ResponsibleId] = [responsavelBP].[Id]
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
	  --OR ([bemPatrimonial].[flagAcervo] IS NULL) --NAO CONTABILIZAR ACERVOS
	  OR ([bemPatrimonial].[flagTerceiro] IS NULL OR [movimentacaoBP].[MovementTypeId] NOT IN (27)) --NAO CONTABILIZAR 'BEM DE TERCEIROS' E 'COMODATOS DO TIPO 'RECEBIDOS''
	)
--FILTRO CONSULTA 'PADRAO' VISAO GERAL
AND ([bemPatrimonial].flagVerificado IS NULL AND [bemPatrimonial].[flagDepreciaAcumulada] = 1) --ATIVOS
AND  [bemPatrimonial].[Status] = 1


--RETORNAR ACERVOS
AND ([bemPatrimonial].[flagAcervo] = 1)
GO