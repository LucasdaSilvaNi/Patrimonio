DROP VIEW VW_SAM_PATRIMONIO__BP_TERCEIRO
GO

--CREATE VIEW VW_SAM_PATRIMONIO__BP_TERCEIRO
--AS
--SELECT
--		   [bemPatrimonial].[InitialName]								AS 'Initial'
--	    ,  [bemPatrimonial].[NumberIdentification]						AS 'NumberIdentification'
--	    , ([responsavelBP].[CPF] + ' - ' + [responsavelBP].[Name])		AS 'Responsavel' 
--	    ,  [bemPatrimonial].[ValueAcquisition]							AS 'ValorAquisicao'
--		,  [movimentacaoBP].[ManagerUnitId]								AS 'ManagerUnitId'
--	    ,  [bemPatrimonial].[flagTerceiro]

--		/* CAMPOS NOVA TABELA DEPRECIACAO MENSAL - 'MonthlyDepreciation' */
--	    ,  [dadosDepreciacaoMensal].[ValueAcquisition]					AS '_ValorAquisicao'
--FROM [dbo].[Asset] [bemPatrimonial] WITH(NOLOCK) 
--INNER JOIN [dbo].[AssetMovements] [movimentacaoBP] WITH(NOLOCK) ON [movimentacaoBP].[AssetId] = [bemPatrimonial].[Id]
--INNER JOIN [dbo].[Responsible] [responsavelBP] WITH(NOLOCK) ON [movimentacaoBP].[ResponsibleId] = [responsavelBP].[Id]
--LEFT JOIN  [dbo].[MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) ON [bemPatrimonial].[Id] = [dadosDepreciacaoMensal].[AssetStartId]
--WHERE 
--	[movimentacaoBP].[Id] IN (SELECT 
--									TOP 1 [mv].[Id] 
--							  FROM [dbo].[AssetMovements] [mv] WITH(NOLOCK) 
--							  WHERE [mv].[AssetId] = [bemPatrimonial].[Id] 
--							  ORDER BY 
--									[mv].[Id] DESC)
----EXCECOES
--AND (     ([movimentacaoBP].MovementTypeId NOT IN (23, 24)) --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)
--	  OR ([bemPatrimonial].[flagAcervo] IS NULL) --NAO CONTABILIZAR ACERVOS
--	)
----FILTRO CONSULTA 'PADRAO' VISAO GERAL
--AND ([bemPatrimonial].flagVerificado IS NULL AND [bemPatrimonial].[flagDepreciaAcumulada] = 1) --ATIVOS
--AND  [bemPatrimonial].[Status] = 1

----CONTABILIZAR 'BEM DE TERCEIROS' E 'COMODATOS DO TIPO 'RECEBIDOS''
--AND (([bemPatrimonial].[flagTerceiro] = 1) OR ([movimentacaoBP].[MovementTypeId] IN (27)))