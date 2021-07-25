SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PRC_RELATORIO__BAIXAS_BP__MESREF')
DROP PROCEDURE [dbo].[PRC_RELATORIO__BAIXAS_BP__MESREF]  
GO


--QUARTO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO (BAIXAS MES-REF)
CREATE PROCEDURE [dbo].[PRC_RELATORIO__BAIXAS_BP__MESREF]
(
	   @MonthReference VARCHAR(6)
	 , @ManagerUnitId INT
) AS
DECLARE @AssetIdsEstornados TABLE(
	AssetId INT,
	AuxiliaryAccountId INT
)
BEGIN

	--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
	--BEGIN CODE --
	INSERT INTO @AssetIdsEstornados(AssetId,AuxiliaryAccountId)  
	SELECT [mov].[AssetId],[mov].[AuxiliaryAccountId] FROM [AssetMovements] [mov] WITH(NOLOCK)
			WHERE [mov].[ManagerUnitId] = @ManagerUnitId 
			  AND [mov].[MovementTypeId] = 5 --- Inventário Inicial
			  AND ([mov].[FlagEstorno] IS NOT NULL) AND ([mov].[DataEstorno] IS NOT NULL)
		GROUP BY [mov].[AssetId],[mov].[AuxiliaryAccountId]
	--END CODE --
SELECT 
		  (SELECT tipoMovimentacao.[Description] FROM MovementType tipoMovimentacao WITH(NOLOCK) WHERE movimentacaoBP.MovementTypeId = tipoMovimentacao.Id)																AS 'TipoMovimentacao'
		, (SELECT [contaContabilDepreciacao].[Code] FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId 
																																						FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																						WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacao'
		, (SELECT [contaContabilDepreciacao].[Description] FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId
																																						FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																						WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacaoDescricao'

		, (SELECT ContaContabilApresentacao FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																AS 'ContaContabil'
		, (SELECT [Description] FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																			AS 'ContaContabilDescricao'

		, SUM(dadosDepreciacaoMensal.ValueAcquisition)																																									AS 'ValorAquisicao'
		, SUM(CASE 
					WHEN [BP].[flagAcervo] = 1 THEN  0
					WHEN [BP].[flagTerceiro] = 1 THEN  0 
					ELSE dadosDepreciacaoMensal.RateDepreciationMonthly END)																																			    AS 'DepreciacaoNoMes'
		, SUM(CASE 
					WHEN [BP].[flagAcervo] = 1 THEN  ISNULL([BP].[ValueUpdate],[BP].[ValueAcquisition])
					WHEN [BP].[flagTerceiro] = 1 THEN  ISNULL([BP].[ValueUpdate],[BP].[ValueAcquisition])
					ELSE dadosDepreciacaoMensal.CurrentValue END)																																							AS 'ValorAtual'

		--LINHA 'DEBUG' MES-REFERENCIA DEPRECIACAO NA UGE CONSULTADA
		--, CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112)																																				AS 'MesReferenciaMovimentacaoDepreciacao'
		--, CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112)																																						AS 'MesReferenciaMovimentacao'
FROM 
	AssetMovements movimentacaoBP WITH(NOLOCK)
	INNER JOIN Asset BP WITH(NOLOCK) ON movimentacaoBP.AssetId = BP.Id
	--POR CONTA DAS TRANSFERENCIAS, SERAH UTILIZADA A COLUNA '.MonthlyDepreciationId' PARA SE PODER 'PUXAR' A DEPRECIACAO PARA UM DADO ANO-MES-REFERENCIA A PARTIR DA TABELA DE MOVIMENTACOES (AssetMovements)
	INNER JOIN MonthlyDepreciation dadosDepreciacaoMensal WITH(NOLOCK) ON movimentacaoBP.MonthlyDepreciationId = dadosDepreciacaoMensal.MonthlyDepreciationId
WHERE
	movimentacaoBP.Id IN (SELECT MAX(Id)
						  FROM 
							AssetMovements movimentacaoBP WITH(NOLOCK)
						  WHERE ManagerUnitId = @ManagerUnitId
							AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) = @MonthReference)
							--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
							--BEGIN CODE --
							AND (movimentacaoBP.AssetId NOT IN(SELECT [AssetId] FROM @AssetIdsEstornados WHERE [AuxiliaryAccountId] = movimentacaoBP.AuxiliaryAccountId)) 
							AND((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
							--END CODE --
						  GROUP BY AssetId
						 )
/*************************************************/
AND dadosDepreciacaoMensal.Id IN (
									SELECT MAX(Id)
									FROM 
									  MonthlyDepreciation dadosDepreciacaoMensal WITH(NOLOCK)
									WHERE ManagerUnitId = @ManagerUnitId
									  --AND (CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) <= @MonthReference) --PARA TRAZER O ULTIMO MES-REFERENCIADO DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
									  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
									GROUP BY [dadosDepreciacaoMensal].[MaterialItemCode], [dadosDepreciacaoMensal].[AssetStartId]
								 )
/*************************************************/
AND movimentacaoBP.ManagerUnitId = @ManagerUnitId
AND (FlagEstorno = 0 OR FlagEstorno IS NULL)
AND ([movimentacaoBP].MovementTypeId IN (
											11, --Transfêrencia
											12, --Doação
											15, --Extravio
											16, --Obsoleto
											17, --Danificado
											18,	--Sucata
											23, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)  
											24, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)  
											27, --COMODATOS/DE TERCEIROS RECEBIDOS
											42,	--SAÍDA INSERVÍVEL DA UGE - DOAÇÃO			
											43,	--SAÍDA INSERVÍVEL DA UGE - TRANSFERÊNCIA	
											44,	--COMODATO - CONCEDIDOS - PATRIMONIADO		
											45,	--COMODATO/DE TERCEIROS - RECEBIDOS			
											46,	--DOAÇÃO CONSOLIDAÇÃO - PATRIMÔNIO			
											47,	--DOAÇÃO INTRA - NO - ESTADO - PATRIMÔNIO	
											48,	--DOAÇÃO MUNÍCIPIO - PATRIMÔNIO
											49,	--DOAÇÃO OUTROS ESTADOS - PATRIMÔNIO
											50,	--DOAÇÃO UNIÃO - PATRIMÔNIO
											51,	--ESTRAVIO, FURTO, ROUBO - PATRIMONIADO
											52,	--MORTE ANIMAL - PATRIMONIADO
											53, --MORTE VEGETAL - PATRIMONIADO
											54, --MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO
											55, --SEMENTES, PLANTAS, INSUMOS E ÁRVORES
											56,	--TRANSFERÊNCIA OUTRO ÓRGÃO - PATRIMONIADO
											57,	--TRANSFERÊNCIA MESMO ÓRGÃO - PATRIMONIADO
											58,	--PERDAS INVOLUNTÁRIAS - BENS MÓVEIS
											59, --PERDAS INVOLUNTÁRIAS - INSERVÍVEL BENS MÓVEIS
											95 --VENDA/LEILÃO - SEMOVENTES
											--81, --DEPRECIAÇÃO – BAIXA (EXCLUSIVA CONTABILIZA-SP)
											--91  --RECLASSIFICACAO SEM EMPENHO (EXCLUSIVA CONTABILIZA-SP)
										)) 
AND (
		--SUBSELECT 01
		movimentacaoBP.ManagerUnitId = @ManagerUnitId
		AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) = @MonthReference)
		AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
		--SUBSELECT 02
		--AND (CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) <= @MonthReference) --PARA TRAZER O ULTIMO MES-REFERENCIADO DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
		AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT))
    )
GROUP BY
			  movimentacaoBP.MovementTypeId
		    , movimentacaoBP.AuxiliaryAccountId
			, [movimentacaoBP].[MovimentDate]
		    , CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112)

			--LINHA 'DEBUG' MES-REFERENCIA DEPRECIACAO NA UGE CONSULTADA
			--, [dadosDepreciacaoMensal].[CurrentDate]
			--, CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112)
			, dadosDepreciacaoMensal.MaterialItemCode
			, dadosDepreciacaoMensal.AssetStartId
ORDER BY
			  movimentacaoBP.AuxiliaryAccountId
			, dadosDepreciacaoMensal.MaterialItemCode
			, dadosDepreciacaoMensal.AssetStartId

END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO__BAIXAS_BP__MESREF] TO [ususam]
GO