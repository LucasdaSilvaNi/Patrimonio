IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('dbo.VW_SAM_PATRIMONIO__BAIXAS_BPS') AND type = 'V') 
	DROP VIEW VW_SAM_PATRIMONIO__BAIXAS_BPS
GO

CREATE VIEW VW_SAM_PATRIMONIO__BAIXAS_BPS
AS
SELECT
		  [bemPatrimonial].[InitialName]								AS 'Sigla'
		, [bemPatrimonial].[NumberIdentification]						AS 'Chapa'
		, [tipoMovimentacaoBP].[Description]							AS 'TipoMovimentacao'						/*CAMPO RELATORIO*/
		, [bemPatrimonial].[ValueAcquisition]							AS 'ValorAquisicao'							/*CAMPO RELATORIO*/
		, [bemPatrimonial].[DepreciationAccumulated]					AS 'DepreciacaoAcumulada'
		, [bemPatrimonial].[DepreciationByMonth]						AS 'DepreciacaoNoMes'						/*CAMPO RELATORIO*/
		, [bemPatrimonial].[ValueUpdate]								AS 'ValorAtual'								/*CAMPO RELATORIO*/
		, [movimentacaoBP].[MovimentDate]								AS 'DataUltimaMovimentacao'					
		, [bemPatrimonial].[MovimentDate]								AS 'DataIncorporacao'
		, [contaContabilDepreciacao].[Code]								AS 'ContaContabilDepreciacao'				/*CAMPO RELATORIO*/
		, [contaContabilDepreciacao].[Description]						AS 'ContaContabilDepreciacaoDescricao'		/*CAMPO RELATORIO*/
		, [contaContabil].[BookAccount]									AS 'ContaContabil'							/*CAMPO RELATORIO*/
		, [contaContabil].[Description]									AS 'ContaContabilDescricao'					/*CAMPO RELATORIO*/
		, [movimentacaoBP].[ManagerUnitId]								AS 'ManagerUnitId'
		, [movimentacaoBP].[AdministrativeUnitId]						AS 'AdministrativeUnitId'
		, [movimentacaoBP].[BudgetUnitId]								AS 'BudgetUnitId'
		, [movimentacaoBP].[InstitutionId]								AS 'InstitutionId'

		/* CAMPOS NOVA TABELA DEPRECIACAO MENSAL - 'MonthlyDepreciation' */
		, [depreciacaoMensal].[ValueAcquisition]						AS '_ValorAquisicao'						/*CAMPO RELATORIO*/
		, [depreciacaoMensal].[AccumulatedDepreciation]					AS '_DepreciacaoAcumulada'
		, [depreciacaoMensal].[RateDepreciationMonthly]					AS '_DepreciacaoNoMes'						/*CAMPO RELATORIO*/
		, [depreciacaoMensal].[CurrentValue]							AS '_ValorAtual'							/*CAMPO RELATORIO*/
		, [depreciacaoMensal].[DateIncorporation]						AS '_DataIncorporacao'
FROM [dbo].[Asset] [bemPatrimonial] WITH(NOLOCK) 
INNER JOIN [dbo].[AssetMovements] [movimentacaoBP] WITH(NOLOCK) ON [movimentacaoBP].[AssetId] = [bemPatrimonial].[Id]
INNER JOIN [dbo].[MovementType] [tipoMovimentacaoBP] WITH(NOLOCK) ON [movimentacaoBP].[MovementTypeId] = [tipoMovimentacaoBP].[Id]
INNER JOIN [dbo].[AuxiliaryAccount] [contaContabil] WITH(NOLOCK) ON [movimentacaoBP].[AuxiliaryAccountId] = [contaContabil].[Id]
INNER JOIN [dbo].[DepreciationAccount] [contaContabilDepreciacao] WITH(NOLOCK) ON [contaContabil].[DepreciationAccountId] = [contaContabilDepreciacao].[Id]
LEFT  JOIN [dbo].[MonthlyDepreciation] [depreciacaoMensal] WITH(NOLOCK) ON [bemPatrimonial].[Id] = [depreciacaoMensal].[AssetStartId]
WHERE 
	[movimentacaoBP].[Id] IN (SELECT 
									TOP 1 [mv].[Id] 
							  FROM [dbo].[AssetMovements] [mv] WITH(NOLOCK) 
							  WHERE [mv].[AssetId] = [bemPatrimonial].[Id] 
							  ORDER BY 
									[mv].[Id] DESC)

--PERGUNTAR CONCEITO
----TRAZER APENAS MOVIMENTACOES
--AND ([tipoMovimentacaoBP].[GroupMovimentId] = 2)

AND (     [movimentacaoBP].MovementTypeId NOT IN (23, 24) --Baixa por Movimentacao de Consumo (direto da tela de BPs Pendentes)
	  --FILTRO UTILIZADO PELA TELA (ORIGEM: FONTE SISTEMA)
	  AND [movimentacaoBP].MovementTypeId IN (13, --VOLTA CONSERTO
											  15, --Extravio
											  16, --Obsoleto
											  17, --Danificado
											  18, --Sucata
											  12, --Doa��o
											  11, --Transf�rencia 
											  42, --SA�DA INSERV�VEL DA UGE - DOA��O
											  43, --SA�DA INSERV�VEL DA UGE - TRANSFER�NCIA
											  46, --DOA��O CONSOLIDA��O - PATRIM�NIO
											  48, --DOA��O MUN�CIPIO - PATRIM�NIO
											  49, --DOA��O OUTROS ESTADOS - PATRIM�NIO
											  50, --DOA��O UNI�O - PATRIM�NIO
											  47, --DOA��O INTRA - NO - ESTADO - PATRIM�NIO
											  51, --ESTRAVIO, FURTO, ROUBO - PATRIMONIADO
											  52, --MORTE ANIMAL - PATRIMONIADO
											  53, --MORTE VEGETAL - PATRIMONIADO
											  55, --SEMENTES, PLANTAS, INSUMOS E �RVORES
											  58, --PERDAS INVOLUNT�RIAS - BENS M�VEIS
											  59, --PERDAS INVOLUNT�RIAS - INSERV�VEL BENS M�VEIS
											  57, --TRANSFER�NCIA MESMO �RG�O - PATRIMONIADO
											  56  --TRANSFER�NCIA OUTRO �RG�O - PATRIMONIADO
											 )
	)
AND ([bemPatrimonial].flagVerificado IS NULL AND [bemPatrimonial].[flagDepreciaAcumulada] = 1) --ATIVOS
AND ([bemPatrimonial].[Status] = 0) --INATIVOS
AND ([movimentacaoBP].[Status] = 0) --INATIVOS
GO