IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('dbo.VW__VISAO_GERAL__UGE_MESREF_ATUAL') AND type = 'V') 
	DROP VIEW VW__VISAO_GERAL__UGE_MESREF_ATUAL
GO

CREATE VIEW VW__VISAO_GERAL__UGE_MESREF_ATUAL
AS
SELECT
		  [bemPatrimonial].[InitialName]																AS 'Sigla'
		, [bemPatrimonial].[NumberIdentification]														AS 'Chapa'
		, [bemPatrimonial].[Status]																		AS 'Status'
		, [contaContabilDepreciacao].[Code]																AS 'ContaContabilDepreciacao'				/*CAMPO RELATORIO*/
		, [contaContabilDepreciacao].[Description]														AS 'ContaContabilDepreciacaoDescricao'		/*CAMPO RELATORIO*/
		, [contaContabilDepreciacao].[Status]															AS 'ContaContabilDepreciacaoStatus'
		, [contaContabil].[BookAccount]																	AS 'ContaContabil'							/*CAMPO RELATORIO*/
		, [contaContabil].[Description]																	AS 'ContaContabilDescricao'					/*CAMPO RELATORIO*/
		, [contaContabil].[Status]																		AS 'ContaContabilStatus'
		, [movimentacaoBP].[MovimentDate]																AS 'DataUltimaMovimentacao'
		, [bemPatrimonial].[ValueAcquisition]															AS 'ValorAquisicao'
		, [bemPatrimonial].[ValueUpdate]																AS 'ValorAtualizado'

		, CASE WHEN ([bemPatrimonial].[monthUsed] >= 0) THEN
			CASE
				WHEN ([bemPatrimonial].[flagDecreto] IS NULL)												THEN ISNULL([bemPatrimonial].[ValueAcquisition], 0.00)
				WHEN (([bemPatrimonial].[flagDecreto] IS NOT NULL) OR ([bemPatrimonial].[flagDecreto] = 0))	THEN ISNULL([bemPatrimonial].[ValueUpdate], 0.00)
			END
		  END																							AS 'ValorContabil'							/*CAMPO RELATORIO*/
		
		--,  (SELECT TOP 1 ISNULL(fechamentoMesAnterior.DepreciationAccumulated, 0.00) 
		--	FROM Closing fechamentoMesAnterior 
		--	INNER JOIN ManagerUnit ugeVisaoGeral WITH(NOLOCK) ON fechamentoMesAnterior.ManagerUnitId = ugeVisaoGeral.Id
		--	WHERE 
		--		--SET @LastClosingYearMonthReference = LEFT(CONVERT(varchar, CONVERT(DATE, DATEADD(month, -1, (SUBSTRING(@ClosingYearMonthReference, 1, 4) + SUBSTRING(@ClosingYearMonthReference, 5, 2) + '01'))),112),6)
		--		--fechamentoMesAnterior.ClosingYearMonthReference = @LastClosingYearMonthReference
		--		fechamentoMesAnterior.ClosingYearMonthReference = LEFT(CONVERT(varchar, CONVERT(DATE, DATEADD(month, -1, (SUBSTRING(ugeVisaoGeral.ManagmentUnit_YearMonthReference, 1, 4) + SUBSTRING(ugeVisaoGeral.ManagmentUnit_YearMonthReference, 5, 2) + '01'))),112),6)
		--		AND fechamentoMesAnterior.ManagerUnitId = [movimentacaoBP].[ManagerUnitId]
		--	ORDER BY fechamentoMesAnterior.ClosingYearMonthReference DESC
		--  )					
		--																								AS 'DepreciacaoAcumuladaAteMesAnterior'		/*CAMPO RELATORIO*/
		, (
		
				SELECT TOP 1 ISNULL(dadosFechamentoPenultimoMes.DepreciationAccumulated, 0.00)
				FROM Closing dadosFechamentoMesAnterior WITH(NOLOCK)
				LEFT JOIN Closing dadosFechamentoPenultimoMes WITH(NOLOCK) ON (dadosFechamentoMesAnterior.AssetId = dadosFechamentoPenultimoMes.AssetId)
				INNER JOIN ManagerUnit ugeVisaoGeral WITH(NOLOCK) ON dadosFechamentoMesAnterior.ManagerUnitId = ugeVisaoGeral.Id
				WHERE 
					dadosFechamentoMesAnterior.ManagerUnitId = [movimentacaoBP].[ManagerUnitId]
				AND dadosFechamentoMesAnterior.MonthUsed = [bemPatrimonial].[monthUsed]
				AND dadosFechamentoMesAnterior.AssetId = [bemPatrimonial].[Id]
				AND dadosFechamentoPenultimoMes.MonthUsed = dadosFechamentoMesAnterior.MonthUsed-1
				AND dadosFechamentoMesAnterior.ClosingYearMonthReference = (LEFT(CONVERT(varchar, CONVERT(DATE, DATEADD(month, -1, (SUBSTRING(ugeVisaoGeral.ManagmentUnit_YearMonthReference, 1, 4) + SUBSTRING(ugeVisaoGeral.ManagmentUnit_YearMonthReference, 5, 2) + '01'))),112),6))
				ORDER BY dadosFechamentoMesAnterior.ClosingYearMonthReference DESC
		)																								AS 'DepreciacaoAcumuladaAteMesAnterior'		/*CAMPO RELATORIO*/

		, ISNULL([bemPatrimonial].[RateDepreciationMonthly], 0.00)										AS 'DepreciacaoNoMes'						/*CAMPO RELATORIO*/
		, ISNULL([bemPatrimonial].[DepreciationAccumulated], 0.00)										AS 'DepreciacaoAcumulada'					/*CAMPO RELATORIO*/
		, [movimentacaoBP].[ManagerUnitId]																AS 'ManagerUnitId'
		, ISNULL([bemPatrimonial].[flagDecreto], 0)														AS 'EhDecreto'
		, [bemPatrimonial].[monthUsed]																	AS 'MesesEmUso'
		, [bemPatrimonial].[LifeCycle]																	AS 'MesesVidaUtil'


		/* CAMPOS NOVA TABELA DEPRECIACAO MENSAL - 'MonthlyDepreciation' */
		, [dadosDepreciacaoMensal].[ValueAcquisition]													AS '_ValorAquisicao'
		, [dadosDepreciacaoMensal].[CurrentValue]														AS '_ValorAtualizado'
		, [dadosDepreciacaoMensal].[CurrentValue]														AS '_ValorContabil'							/*CAMPO RELATORIO*/
		, CASE
					WHEN [dadosDepreciacaoMensal].[CurrentMonth] > 0 THEN 
						(SELECT 
								TOP 1 [_dadosDepreciacaoMesAnterior].[AccumulatedDepreciation] 
								FROM [MonthlyDepreciation] [_dadosDepreciacaoMesAnterior] 
								WHERE  [_dadosDepreciacaoMesAnterior].[CurrentMonth] = [dadosDepreciacaoMensal].[CurrentMonth]-1
								AND [_dadosDepreciacaoMesAnterior].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId]
							ORDER BY [_dadosDepreciacaoMesAnterior].[CurrentMonth] DESC)

					WHEN [dadosDepreciacaoMensal].[CurrentMonth] = 0 THEN 
						[dadosDepreciacaoMensal].[AccumulatedDepreciation]			
		  END																							AS '_DepreciacaoAcumuladaAteMesAnterior'	/*CAMPO RELATORIO*/
		, ISNULL([dadosDepreciacaoMensal].[RateDepreciationMonthly], 0.00)								AS '_DepreciacaoNoMes'						/*CAMPO RELATORIO*/
		, ISNULL([dadosDepreciacaoMensal].[AccumulatedDepreciation], 0.00)								AS '_DepreciacaoAcumulada'					/*CAMPO RELATORIO*/
		, [dadosDepreciacaoMensal].[Decree]																AS '_EhDecreto'
		, [dadosDepreciacaoMensal].[CurrentMonth]														AS '_MesesEmUso'
		, [dadosDepreciacaoMensal].[LifeCycle]															AS '_MesesVidaUtil'
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

AND [bemPatrimonial].[monthUsed] = [dadosDepreciacaoMensal].[CurrentMonth]
GO

