SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PRC_RELATORIO__INVENTARIO_CONTAS_CONTABEIS')
DROP PROCEDURE [dbo].[PRC_RELATORIO__INVENTARIO_CONTAS_CONTABEIS]  
GO

--PRIMEIRO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO
CREATE PROCEDURE [dbo].[PRC_RELATORIO__INVENTARIO_CONTAS_CONTABEIS]
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
			  AND ([mov].[FlagEstorno] IS NOT NULL ) AND ([mov].[DataEstorno] IS NOT NULL)
		GROUP BY [mov].[AssetId],[mov].[AuxiliaryAccountId]
	--END CODE --
	SELECT 
			  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]									AS 'ContaContabilDepreciacao'				/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]						AS 'ContaContabilDepreciacaoDescricao'		/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoStatus]							AS 'ContaContabilDepreciacaoStatus'
			, (select top 1 ContaContabilApresentacao from AuxiliaryAccount where BookAccount = [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]
																			AND Description like [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]
																			AND Status = [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]
			   )																						AS 'ContaContabil'							/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]									AS 'ContaContabilDescricao'					/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]										AS 'ContaContabilStatus'
			, SUM([visaoGeral_UGE__MESREF_ATUAL].[ValorContabil])										AS 'ValorContabil'							/*CAMPO RELATORIO*/
			--, SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoAcumuladaAteMesAnterior])				AS 'DepreciacaoAcumuladaAteMesAnterior'		/*CAMPO RELATORIO*/
			, ISNULL(SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoNoMes]),0)							AS 'DepreciacaoNoMes'						/*CAMPO RELATORIO*/
			, ISNULL(SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoAcumulada]),0)						AS 'DepreciacaoAcumulada'					/*CAMPO RELATORIO*/
	FROM
		(
			SELECT  
					 --(SELECT sigla.Name FROM Initial sigla WITH(NOLOCK) WHERE  bemPatrimonial.InitialId = sigla.Id)																														AS 'Sigla',
					 --(SELECT CONVERT(INT,bemPatrimonial.NumberIdentification) FROM Asset bemPatrimonial WITH(NOLOCK) WHERE bemPatrimonial.Id = movimentacaoBP.AssetId)																	AS 'Chapa',
					 (SELECT [contaContabilDepreciacao].[Code]			FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId 
																																											FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																											WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacao'
					, (SELECT [contaContabilDepreciacao].[Description]	FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId
																																											FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																											WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacaoDescricao'
					, (SELECT [contaContabilDepreciacao].[Status]		FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId
																																											FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																											WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacaoStatus'
																																											
					, (SELECT [contaContabil].[BookAccount] FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																	AS 'ContaContabil'
					, (SELECT [contaContabil].[Description] FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																	AS 'ContaContabilDescricao'
					, (SELECT [contaContabil].[Status]		FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																	AS 'ContaContabilStatus'

					, CASE 
							WHEN movimentacaoBP.AuxiliaryAccountId = 212508 
							THEN  (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[CurrentValue],0) 'ValorMensal'			
										  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
										  WHERE 
												ManagerUnitId = movimentacaoBP.[ManagerUnitId]
										  AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
										  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId] 
										  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <=
										  (select top 1 (CAST(CONVERT(CHAR(6), MovimentDate, 112) AS INT)) from AssetMovements with(nolock) 
										  where AssetId = bemPatrimonial.Id AND 
											    FlagEstorno IS NULL AND
												MovementTypeId = 19))
										 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
									)
							ELSE (SELECT TOP 1 ISNULL([bemPatrimonial].[ValueAcquisition], 0.00)) 
					END AS 'ValorContabil'
					, 
					CASE 
					WHEN movimentacaoBP.AuxiliaryAccountId = 212508 --Conta de inservível
					THEN  0
					ELSE
						(SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[RateDepreciationMonthly],0) 'ValorMensal'			
									  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
									  WHERE 
											ManagerUnitId = movimentacaoBP.[ManagerUnitId]
									  AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
									  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId] 
									  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) = CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
									 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
						) 
					END AS 'DepreciacaoNoMes'
					, 
					CASE 
					WHEN movimentacaoBP.AuxiliaryAccountId = 212508 --Conta de inservível
					THEN  0
					ELSE
					(SELECT TOP 1 [dadosDepreciacaoMensal].[AccumulatedDepreciation] 'ValorAcumulado'			
								  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
								  WHERE 
										ManagerUnitId = movimentacaoBP.[ManagerUnitId]
										AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
								  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId]
								  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
								 ORDER BY [dadosDepreciacaoMensal].[Id] DESC

						) 
					END AS 'DepreciacaoAcumulada'
					--LINHA 'DEBUG' MES-REFERENCIA MOVIMENTACAO NA UGE CONSULTADA
					, CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112)																																						AS 'MesReferenciaMovimentacao'
			FROM 
				AssetMovements movimentacaoBP WITH(NOLOCK)
			INNER JOIN Asset bemPatrimonial WITH(NOLOCK) ON movimentacaoBP.AssetId = bemPatrimonial.Id
			WHERE (bemPatrimonial.flagAcervo IS NULL OR bemPatrimonial.flagAcervo = 0) --RETIRANDO ACERVOS POIS OS MESMOS NAO DEPRECIAM
			  AND (bemPatrimonial.flagTerceiro IS NULL OR bemPatrimonial.flagTerceiro = 0) --RETIRANDO BENS DE TERCEIRO POIS OS MESMOS SERAO CONTROLADOS APENAS FISICAMENTE, NAO CONTABILMENTE
		      AND (bemPatrimonial.[flagVerificado] IS NULL)        
		      AND (bemPatrimonial.[flagDepreciaAcumulada] = 1)
			  AND movimentacaoBP.Id IN (SELECT MAX(Id)
									  FROM 
										AssetMovements movimentacaoBP WITH(NOLOCK)
									  WHERE ManagerUnitId = @ManagerUnitId
										AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
										--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
										--BEGIN CODE --
										AND (movimentacaoBP.AssetId NOT IN(SELECT [AssetId] FROM @AssetIdsEstornados WHERE [AuxiliaryAccountId] = movimentacaoBP.AuxiliaryAccountId)) --
										AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
										--END CODE --
									  GROUP BY AssetId
									 )
			AND (movimentacaoBP.ManagerUnitId = @ManagerUnitId)
			--AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
			
			AND (movimentacaoBP.MovementTypeId NOT IN (
															11,	--Transfêrencia
															12,	--Doação
															15,	--Extravio
															16,	--Obsoleto
															17,	--Danificado
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
															53,	--MORTE VEGETAL - PATRIMONIADO
															54,	--MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO
															55,	--SEMENTES, PLANTAS, INSUMOS E ÁRVORES
															56,	--TRANSFERÊNCIA OUTRO ÓRGÃO - PATRIMONIADO
															57,	--TRANSFERÊNCIA MESMO ÓRGÃO - PATRIMONIADO
															58,	--PERDAS INVOLUNTÁRIAS - BENS MÓVEIS
															59,	--PERDAS INVOLUNTÁRIAS - INSERVÍVEL BENS MÓVEIS
															81,
															91,
															95))
		AND (
				movimentacaoBP.ManagerUnitId = @ManagerUnitId
				AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
				AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
			)
		) AS visaoGeral_UGE__MESREF_ATUAL
	GROUP BY
			  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoStatus]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]
	ORDER BY 
			  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]
END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO__INVENTARIO_CONTAS_CONTABEIS] TO [ususam]
GO