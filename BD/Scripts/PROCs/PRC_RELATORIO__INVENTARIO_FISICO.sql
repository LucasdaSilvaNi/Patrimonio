SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PRC_RELATORIO__INVENTARIO_FISICO')
	DROP PROCEDURE [dbo].[PRC_RELATORIO__INVENTARIO_FISICO]  
GO

--RELATORIO DE INVENTARIO FISICO
CREATE PROCEDURE [dbo].[PRC_RELATORIO__INVENTARIO_FISICO]
(
	  @GroupingType			VARCHAR(1) = NULL
	, @MonthReference		VARCHAR(6) = NULL
	, @InstitutionId		INT = NULL
	, @BudgetUnitId			INT = NULL
	, @ManagerUnitId		INT = NULL
	, @AdministrativeUnitId	INT = NULL
	, @SectionId         	INT = NULL
) AS


DECLARE @DataAnoMes AS DATETIME;
DECLARE @Ano AS INT;
DECLARE @Mes AS INT;
DECLARE @Dia AS INT = 1;


SET DATEFORMAT YMD;

BEGIN

	SELECT 
			  CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) AS 'MesReferencia'
			, CASE @GroupingType
									WHEN '1' THEN (SELECT TOP 1 Convert(varchar,[Code]) 
												   FROM [MaterialGroup] [grupoMaterial] WITH(NOLOCK)
												   WHERE [grupoMaterial].[Code] = [bemPatrimonial].[MaterialGroupCode])--m.Code
									WHEN '2' THEN (SELECT TOP 1 [contaContabil].[ContaContabilApresentacao] 
												   FROM [AuxiliaryAccount] [contaContabil] WITH(NOLOCK) 
												   WHERE [movimentacaoBP].[AuxiliaryAccountId] = [contaContabil].[Id])--aux.BookAccount
			  END																																																							AS 'Agrupamento'

			, CASE @GroupingType
									WHEN '1' THEN (SELECT TOP 1 [grupoMaterial].[Description] 
												   FROM [MaterialGroup] [grupoMaterial] WITH(NOLOCK)
												   WHERE [grupoMaterial].[Code] = [bemPatrimonial].[MaterialGroupCode])--m.Code
									WHEN '2' THEN (SELECT TOP 1 [contaContabil].[Description]
												   FROM [AuxiliaryAccount] [contaContabil] WITH(NOLOCK)
												   WHERE [movimentacaoBP].[AuxiliaryAccountId] = [contaContabil].[Id])--aux.BookAccount
			  END																																																							AS 'DescAgrupamento'

			, (SELECT [contaContabil].[Status] FROM [AuxiliaryAccount] [contaContabil] WITH(NOLOCK) WHERE [movimentacaoBP].[AuxiliaryAccountId] = [contaContabil].[Id])																		AS 'STATUS_CONTA'
			, (SELECT (CONVERT(VARCHAR,[orgaoSIAFEM].[Code]) + ' - ' + [orgaoSIAFEM].[Description])	FROM [Institution] [orgaoSIAFEM]  WITH(NOLOCK)		WHERE [movimentacaoBP].[InstitutionId]			= [orgaoSIAFEM].[Id])				AS 'ORGAO'
			, (SELECT (CONVERT(VARCHAR,[uoSIAFEM].[Code])	+ ' - ' + [uoSIAFEM].[Description])		FROM [BudgetUnit] [uoSIAFEM]  WITH(NOLOCK)			WHERE [movimentacaoBP].[BudgetUnitId]			= [uoSIAFEM].[Id])					AS 'UO'
			, (SELECT (CONVERT(VARCHAR,[ugeSIAFEM].[Code])	+ ' - ' + [ugeSIAFEM].[Description])	FROM [ManagerUnit] [ugeSIAFEM]  WITH(NOLOCK)		WHERE [movimentacaoBP].[ManagerUnitId]			= [ugeSIAFEM].[Id])					AS 'UGE'
			, (SELECT (CONVERT(VARCHAR,[uaSIAFEM].[Code])	+ ' - ' + [uaSIAFEM].[Description])		FROM [AdministrativeUnit] [uaSIAFEM]  WITH(NOLOCK)	WHERE [movimentacaoBP].[AdministrativeUnitId]	= [uaSIAFEM].[Id])					AS 'UA'
			, (SELECT (CONVERT(VARCHAR,[divisaoUA].[Code])	+ ' - ' + [divisaoUA].[Description])	FROM [Section] [divisaoUA]  WITH(NOLOCK)			WHERE [movimentacaoBP].[SectionId]				= [divisaoUA].[Id])					AS 'DIVISAO'

			, (SELECT [responsavelBP].[Name] FROM [Responsible] [responsavelBP] WITH(NOLOCK) WHERE [movimentacaoBP].[ResponsibleId] = [responsavelBP].[Id])																					AS 'RESPONSAVEL'

			, (SELECT (ISNULL([siglaBP].[Name],'') +'-'+ [bemPatrimonial].[ChapaCompleta]) 
			   FROM [Initial] [siglaBP] where [bemPatrimonial].[InitialId] = [siglaBP].[Id])																																							AS 'CHAPA'
			
			, (SELECT TOP 1 [descricaoResumidaItemMaterial].[Description]
			   FROM [ShortDescriptionItem] [descricaoResumidaItemMaterial] WITH(NOLOCK)
			   WHERE [descricaoResumidaItemMaterial].[Id] = [bemPatrimonial].[ShortDescriptionItemId])																																					AS 'MATERIAL'
			, 
			[bemPatrimonial].[ValueAcquisition]																																																				AS 'VALOR_AQUISICAO'
			, 
			CASE 
				WHEN [bemPatrimonial].[flagAcervo] = 1 THEN ISNULL([bemPatrimonial].[ValueUpdate],[bemPatrimonial].[ValueAcquisition])
				WHEN [bemPatrimonial].[flagTerceiro] = 1 THEN ISNULL([bemPatrimonial].[ValueUpdate],[bemPatrimonial].[ValueAcquisition])
				WHEN [bemPatrimonial].[flagDecreto] = 1 
				THEN 
					ISNULL((SELECT MIN([dadosDepreciacaoMensal].[CurrentValue])
					  FROM [dbo].[MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
					  WHERE [dadosDepreciacaoMensal].[ManagerUnitId] = [bemPatrimonial].[ManagerUnitId] 
						AND [dadosDepreciacaoMensal].[AssetStartId] = [bemPatrimonial].[AssetStartId] 
						AND [dadosDepreciacaoMensal].[MaterialItemCode] = [bemPatrimonial].[MaterialItemCode] 
						AND Convert(int,Convert(varchar(6),[dadosDepreciacaoMensal].[CurrentDate],112)) <= @MonthReference
					  GROUP BY [dadosDepreciacaoMensal].[MaterialItemCode], [dadosDepreciacaoMensal].[AssetStartId]
					), [bemPatrimonial].[ValueUpdate])
				ELSE
					ISNULL((SELECT MIN([dadosDepreciacaoMensal].[CurrentValue])
					  FROM [dbo].[MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
					  WHERE [dadosDepreciacaoMensal].[ManagerUnitId] = [bemPatrimonial].[ManagerUnitId] 
						AND [dadosDepreciacaoMensal].[AssetStartId] = [bemPatrimonial].[AssetStartId] 
						AND [dadosDepreciacaoMensal].[MaterialItemCode] = [bemPatrimonial].[MaterialItemCode] 
						AND Convert(int,Convert(varchar(6),[dadosDepreciacaoMensal].[CurrentDate],112)) <= @MonthReference
					  GROUP BY [dadosDepreciacaoMensal].[MaterialItemCode], [dadosDepreciacaoMensal].[AssetStartId]
					), [bemPatrimonial].[ValueAcquisition]) END																																																	AS 'VALOR_ATUAL'
	FROM AssetMovements movimentacaoBP WITH(NOLOCK)
	INNER JOIN Asset [bemPatrimonial] WITH(NOLOCK) ON movimentacaoBP.AssetId = [bemPatrimonial].Id
	WHERE
			movimentacaoBP.Id IN (
									SELECT MAX(Id)
									FROM AssetMovements movimentacaoBP WITH(NOLOCK)
									WHERE 
										ManagerUnitId = @ManagerUnitId
										AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
										AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))
									GROUP BY AssetId
								 )
	AND (movimentacaoBP.FlagEstorno = 0 OR movimentacaoBP.FlagEstorno IS NULL)	--RETIRANDO OS ESTORNOS DE MOVIMENTACOES
	AND (movimentacaoBP.MovementTypeId NOT IN (
													11, --Transfêrencia
													12, --Doação
													15, --Extravio
													16, --Obsoleto
													17, --Danificado
													18,	--Sucata
													19, --INSERVÍVEL NA UGE - BENS MÓVEIS
													23, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES) 
													24, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES) 
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
													81, --DEPRECIAÇÃO – BAIXA (EXCLUSIVA CONTABILIZA-SP)
													91, --RECLASSIFICACAO SEM EMPENHO (EXCLUSIVA CONTABILIZA-SP)
													95  --VENDA/LEILÃO - SEMOVENTES
												))
	AND	(	 (movimentacaoBP.InstitutionId		  = @InstitutionId OR @InstitutionId IS NULL)
		 AND (movimentacaoBP.BudgetUnitId		  = @BudgetUnitId OR @BudgetUnitId IS NULL)
		 AND (movimentacaoBP.ManagerUnitId		  = @ManagerUnitId OR @ManagerUnitId IS NULL)
		 AND (movimentacaoBP.AdministrativeUnitId = @AdministrativeUnitId OR @AdministrativeUnitId IS NULL)
		 AND (movimentacaoBP.SectionId			  = @SectionId OR @SectionId IS NULL)
		)
	AND (
		 movimentacaoBP.ManagerUnitId = @ManagerUnitId
		 AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
		 AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAO
		)
	AND [bemPatrimonial].[flagVerificado] IS NULL 
	and [bemPatrimonial].[flagDepreciaAcumulada] = 1
	ORDER BY
			  movimentacaoBP.InstitutionId		 
			, movimentacaoBP.BudgetUnitId ASC		 
			, movimentacaoBP.ManagerUnitId ASC		 
			, movimentacaoBP.AdministrativeUnitId ASC
			, movimentacaoBP.SectionId ASC
			, movimentacaoBP.ResponsibleId ASC
END
GO


GRANT EXECUTE ON [dbo].[PRC_RELATORIO__INVENTARIO_FISICO] TO [ususam]
GO