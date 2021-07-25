DROP VIEW VW_SAM_PATRIMONIO__INCORPORACOES_BPS
GO

--CREATE VIEW VW_SAM_PATRIMONIO__INCORPORACOES_BPS
--AS
--SELECT
--		  [bemPatrimonial].[InitialName]								AS 'Sigla'
--		, [bemPatrimonial].[NumberIdentification]						AS 'Chapa'
--		, [tipoMovimentacaoBP].[Description]							AS 'TipoIncorporacao'						/*CAMPO RELATORIO*/
--		, [bemPatrimonial].[ValueAcquisition]							AS 'ValorAquisicao'							/*CAMPO RELATORIO*/
--		, [bemPatrimonial].[DepreciationAccumulated]					AS 'DepreciacaoAcumulada'
--		, [bemPatrimonial].[DepreciationByMonth]						AS 'DepreciacaoNoMes'						/*CAMPO RELATORIO*/
--		, [bemPatrimonial].[ValueUpdate]								AS 'ValorAtual'								/*CAMPO RELATORIO*/
--		, [movimentacaoBP].[MovimentDate]								AS 'DataUltimaMovimentacao'					/*CAMPO RELATORIO*/
--		, [bemPatrimonial].[MovimentDate]								AS 'DataIncorporacao'
--		, [contaContabilDepreciacao].[Code]								AS 'ContaContabilDepreciacao'				/*CAMPO RELATORIO*/
--		, [contaContabilDepreciacao].[Description]						AS 'ContaContabilDepreciacaoDescricao'		/*CAMPO RELATORIO*/
--		, [contaContabil].[BookAccount]									AS 'ContaContabil'							/*CAMPO RELATORIO*/
--		, [contaContabil].[Description]									AS 'ContaContabilDescricao'					/*CAMPO RELATORIO*/
--		, [movimentacaoBP].[ManagerUnitId]								AS 'ManagerUnitId'
--		, [movimentacaoBP].[AdministrativeUnitId]						AS 'AdministrativeUnitId'
--		, [movimentacaoBP].[BudgetUnitId]								AS 'BudgetUnitId'
--		, [movimentacaoBP].[InstitutionId]								AS 'InstitutionId'

--		/* CAMPOS NOVA TABELA DEPRECIACAO MENSAL - 'MonthlyDepreciation' */
--		, [depreciacaoMensal].[ValueAcquisition]						AS '_ValorAquisicao'						/*CAMPO RELATORIO*/
--		, [depreciacaoMensal].[AccumulatedDepreciation]					AS '_DepreciacaoAcumulada'
--		, [depreciacaoMensal].[RateDepreciationMonthly]					AS '_DepreciacaoNoMes'						/*CAMPO RELATORIO*/
--		, [depreciacaoMensal].[CurrentValue]							AS '_ValorAtual'							/*CAMPO RELATORIO*/
--		, [depreciacaoMensal].[DateIncorporation]						AS '_DataIncorporacao'
--FROM [dbo].[Asset] [bemPatrimonial] WITH(NOLOCK) 
--INNER JOIN [dbo].[AssetMovements] [movimentacaoBP] WITH(NOLOCK) ON [movimentacaoBP].[AssetId] = [bemPatrimonial].[Id]
--INNER JOIN [dbo].[MovementType] [tipoMovimentacaoBP] WITH(NOLOCK) ON [movimentacaoBP].[MovementTypeId] = [tipoMovimentacaoBP].[Id]
--INNER JOIN [dbo].[AuxiliaryAccount] [contaContabil] WITH(NOLOCK) ON [movimentacaoBP].[AuxiliaryAccountId] = [contaContabil].[Id]
--INNER JOIN [dbo].[DepreciationAccount] [contaContabilDepreciacao] WITH(NOLOCK) ON [contaContabil].[DepreciationAccountId] = [contaContabilDepreciacao].[Id]
--LEFT  JOIN [dbo].[MonthlyDepreciation] [depreciacaoMensal] WITH(NOLOCK) ON [bemPatrimonial].[Id] = [depreciacaoMensal].[AssetStartId]
--WHERE 
--	[movimentacaoBP].[Id] IN (SELECT 
--									TOP 1 [mv].[Id] 
--							  FROM [dbo].[AssetMovements] [mv] WITH(NOLOCK) 
--							  WHERE [mv].[AssetId] = [bemPatrimonial].[Id] 
--							  ORDER BY 
--									[mv].[Id] DESC)

----TRAZER APENAS INCORPORACOES
--AND ([tipoMovimentacaoBP].[GroupMovimentId] = 1)

----EXCECOES
--AND (     ([movimentacaoBP].MovementTypeId NOT IN (23, 24)) --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)
--	  OR ([bemPatrimonial].[flagAcervo] IS NULL) --NAO CONTABILIZAR ACERVOS
--	  OR ([bemPatrimonial].[flagTerceiro] IS NULL OR [movimentacaoBP].[MovementTypeId] NOT IN (27)) --NAO CONTABILIZAR 'BEM DE TERCEIROS' E 'COMODATOS DO TIPO 'RECEBIDOS''
--	)

----FILTRO CONSULTA 'PADRAO' VISAO GERAL
--AND ([bemPatrimonial].flagVerificado IS NULL AND [bemPatrimonial].[flagDepreciaAcumulada] = 1) --ATIVOS
--AND  [bemPatrimonial].[Status] = 1