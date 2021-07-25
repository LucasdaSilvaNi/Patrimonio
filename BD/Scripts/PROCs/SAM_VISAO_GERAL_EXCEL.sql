SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

------[SAM_VISAO_GERAL_EXCEL] @Ativo = 1, @Filtro='0.63',@InstitutionId = 6,@Campo ='15',@Size = 10,@PageNumber = 0
ALTER PROCEDURE [dbo].[SAM_VISAO_GERAL_EXCEL]
	@AssetId AS VARCHAR(2000) = NULL,
	@InstitutionId INT =NULL,
	@BudgetUnitId INT = NULL,
	@ManagerUnitId INT = NULL,
	@AdministrativeUnitId INT = NULL,
	@Size SMALLINT = NULL,
	@PageNumber INT = NULL,
	@Estado TINYINT = NULL,
	@Filtro VARCHAR(2000) = NULL,
	@Campo VARCHAR(50) = NULL,
	@CPF varchar(11) = NULL
AS

DECLARE @SectionId INT = NULL;
DECLARE @SectionDescription VARCHAR(200) = NULL;
DECLARE @ResposibleName VARCHAR(100) = NULL;
DECLARE @ValueAcquisition DECIMAL(18,2);
DECLARE @AccumulatedDepreciation DECIMAL(18,2);
DECLARE @RateDepreciationMonthly DECIMAL(18,2);
DECLARE @CurrentValue DECIMAL(18,2) = NULL;
DECLARE @BookAccount INT = NULL;
DECLARE @AuxiliaryAccountDescription VARCHAR(100) = NULL;
DECLARE @Empenho VARCHAR(20) = NULL;
DECLARE @NumberDoc VARCHAR(20) = NULL;
DECLARE	@NumberIdentification VARCHAR(20) = NULL;
DECLARE @MaterialItemCode INT =NULL;
DECLARE @MaterialItemDescription VARCHAR(200) = NULL;
DECLARE @ValueUpdate DECIMAL(18,2) = NULL;
DECLARE @GrupCode AS VARCHAR(120) = NULL;
DECLARE @LifeCycle INT = NULL;
DECLARE @AuxiliaryAccountId INT = NULL;
DECLARE @InitialName VARCHAR(10) = NULL;
DECLARE @FiltroAnd AS VARCHAR(8000) = '';
DECLARE @FiltroOr AS VARCHAR(2000) = '';
DECLARE @NumberIdentificationRetorno VARCHAR(20)= NULL;
DECLARE @ChapaCompleta VARCHAR(20)= NULL;
DECLARE @BuscaGeral VARCHAR(2000) =' ';


DECLARE @SQL AS VARCHAR(8000) = '';

	IF(@AssetId IS NOT NULL)
	BEGIN
		SET @SQL = 'DECLARE @AssetId AS VARCHAR(2000); SET @AssetId =''' + @AssetId + '''';
	END;
	IF(@Campo IS NOT NULL AND @Filtro IS NULL)
	BEGIN
		SET @Campo = NULL;
	END;
	IF(@Campo IS NULL)
	BEGIN
		IF(@Filtro IS NOT NULL AND ISNUMERIC(@Filtro) = 1)
		BEGIN
			SET @BuscaGeral = 1

			SET @NumberIdentificationRetorno = (SELECT TOP 1 [NumberIdentification] FROM [dbo].[Asset] WHERE [NumberIdentification] LIKE '%' + @Filtro + '%')
		
			SET @MaterialItemCode = (SELECT TOP 1 [MaterialItemCode] FROM [dbo].[Asset] WHERE [MaterialItemCode] LIKE '%' + @Filtro + '%')
		
			SET @RateDepreciationMonthly = (SELECT TOP 1 [RateDepreciationMonthly] FROM [dbo].[MonthlyDepreciation] WHERE [RateDepreciationMonthly] = CAST(@Filtro AS DECIMAL(18,2)))
			
			SET @ValueUpdate = (SELECT TOP 1 [ValueUpdate] FROM [dbo].[Asset] WHERE [ValueUpdate] = CAST(@Filtro AS DECIMAL(28,2)))
		
			SET @NumberDoc  = (SELECT TOP 1 [aset].[NumberDoc] 
							   FROM [dbo].[Asset] [aset] 
						       inner join [dbo].[AssetMovements] [asetMovement]
							   ON [aset].[Id] = [asetMovement].[AssetId]
							   WHERE [asetMovement].[NumberDoc] = @Filtro OR ([aset].[NumberDoc] = @Filtro AND [asetMovement].[NumberDoc] IS NULL));
			
			SET @LifeCycle = (SELECT TOP 1 [LifeCycle] FROM [dbo].[MaterialGroup] WHERE [LifeCycle] = CAST(@Filtro AS BIGINT))

		END;
		ELSE
		BEGIN
		
			IF(@Filtro IS NOT NULL)
			BEGIN
				SET @BuscaGeral = 1

				SET @ChapaCompleta = (SELECT TOP 1 [ChapaCompleta] FROM [dbo].[Asset] WHERE [ChapaCompleta] LIKE '%' + @Filtro + '%')
				IF(@ChapaCompleta IS NOT NULL)
	            BEGIN
	            	SET @FiltroOr = @FiltroOr + ' OR [Extent1].[ChapaCompleta] LIKE ''%' + @Filtro + '%'''
	            END

				SET @InitialName = (SELECT TOP 1 [InitialName] FROM [dbo].[Asset] WHERE [InitialName] LIKE '%' +  @Filtro + '%')
				IF(@InitialName IS NOT NULL)
				BEGIN
					SET @FiltroOr = @FiltroOr + '  [Extent1].[InitialName] LIKE ''%' + @Filtro + '%'''

				END
				
				SET @GrupCode =(SELECT TOP 1 [ma].[Code] 
								  FROM [dbo].[MaterialGroup] [ma]  WHERE [ma].[Description] LIKE '%' + @Filtro + '%');
				IF(@GrupCode IS NOT NULL)
				BEGIN
					SET @FiltroOr = @FiltroOr + ' OR [Extent1].[MaterialGroupCode] IN(SELECT [Code] FROM [dbo].[MaterialGroup] [ma] WHERE [ma].[Description] =''%' + @Filtro + '%'')';
				END;
						
				IF(UPPER(@Filtro) <> 'DECRETO' AND UPPER(@Filtro) <> 'ACERVO' AND UPPER(@Filtro) <> 'TERCEIRO')
				BEGIN
					DECLARE @Id AS INT = (SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [MaterialItemDescription] LIKE '%' + @Filtro + '%')

					IF(@Id IS NOT NULL)
					BEGIN
						SET @FiltroOr = @FiltroOr + ' OR [Extent1].[Id] =' + CAST(@Id AS VARCHAR(20));
					END;
				END
							
				IF(UPPER(@Filtro) <> 'DECRETO' AND UPPER(@Filtro) <> 'ACERVO' AND UPPER(@Filtro) <> 'TERCEIRO')
				BEGIN
					SET @SectionDescription = (SELECT TOP 1 [sec].[Id] 
							  FROM [dbo].[Section] [sec] WHERE [sec].[Description] LIKE '%' + @Filtro + '%');
					IF(@SectionDescription IS NOT NULL)
					BEGIN
						SET @FiltroOr = @FiltroOr + ' OR [Extent9].[Description] LIKE ''%' +  @Filtro + '%''';
					END;
				END;
								
				SET @ResposibleName = (SELECT TOP 1 [res].[Id] FROM [dbo].[Responsible] [res] WHERE [res].[Name] LIKE '%' + @Filtro + '%');
				
				IF(@ResposibleName IS NOT NULL)
				BEGIN
					SET @FiltroOr = @FiltroOr + ' OR [Extent10].[Name] LIKE ''%' +  @Filtro + '%''';
				END;
									
				SET @AuxiliaryAccountDescription  = (SELECT TOP 1 [aux].[Id] FROM [dbo].[AuxiliaryAccount] [aux] WHERE [aux].[Description] LIKE '%' + @Filtro + '%');
				IF(@AuxiliaryAccountDescription IS NOT NULL)
				BEGIN
					SET @FiltroAnd = ' AND [Extent11].[Description] LIKE ''%' +  @Filtro + '%''';
				END
			END;
		END;
	END;
	ELSE
	BEGIN
		IF(@Campo ='1')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent1].[InitialName] LIKE ''%' +  @Filtro + '%'')';
		END
		IF(@Campo ='2')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent1].[ChapaCompleta] LIKE ''%' +  @Filtro + '%'')';
		END
		IF(@Campo ='3')
		BEGIN
			IF(ISNUMERIC(@Filtro) = 1)
			BEGIN
				SET @FiltroAnd = ' AND ([Extent1].[MaterialGroupCode] =' +  @Filtro + ')';
			END
			ELSE
			BEGIN
				SET @FiltroAnd = ' AND 1 = 0';
			END
		END
		IF(@Campo ='4')
		BEGIN
			IF(ISNUMERIC(@Filtro) = 1)
			BEGIN
				SET @FiltroAnd = ' AND ([Extent1].[MaterialItemCode] =' +  @Filtro + ')';
			END
			ELSE
			BEGIN
				SET @FiltroAnd = ' AND 1 = 0';
			END
		END
		IF(@Campo ='5')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent1].[MaterialItemDescription] LIKE ''%' +  @Filtro + '%'')';
		END
		IF(@Campo ='6')
		BEGIN
			SET @InstitutionId = (SELECT TOP 1 [orgao].[Id] FROM [dbo].[Institution] [orgao] WHERE [orgao].[Code] = '' + @Filtro + '' OR [orgao].[Description] LIKE '%' + @Filtro + '%')
			IF(@InstitutionId IS NOT NULL)
				SET @FiltroAnd = ' AND ([Extent2].[InstitutionId] =' + CAST(@InstitutionId AS VARCHAR(20)) + ')';

		END
		IF(@Campo ='7')
		BEGIN
			SET @BudgetUnitId = (SELECT TOP 1 [uo].[Id] FROM [dbo].[BudgetUnit] [uo] WHERE [uo].[Code] = '' + @Filtro + '' OR [uo].[Description] LIKE '%' + @Filtro + '%');

			IF(@BudgetUnitId IS NOT NULL)
				SET @FiltroAnd = ' AND ([Extent2].[BudgetUnitId] =' + CAST(@BudgetUnitId AS VARCHAR(20)) + ')';
		END
		IF(@Campo ='8')
		BEGIN
			SET @ManagerUnitId = (SELECT TOP 1 [uge].[Id] FROM [dbo].[ManagerUnit] [uge] WHERE [uge].[Code] = '' + @Filtro + '' OR [uge].[Description] LIKE '%' + @Filtro + '%');
			IF(@ManagerUnitId IS NOT NULL)
				SET @FiltroAnd = ' AND ([Extent2].[ManagerUnitId] =' + CAST(@ManagerUnitId AS VARCHAR(20)) + ')';
		END
		IF(@Campo ='9')
		BEGIN
			IF(ISNUMERIC(@Filtro) = 1)
			BEGIN
				SET @AdministrativeUnitId = (SELECT TOP 1 [ua].[Id] FROM [dbo].[AdministrativeUnit] [ua] WHERE [ua].[Code] =  CAST(@Filtro AS BIGINT));
				IF(@AdministrativeUnitId IS NOT NULL)
					SET @FiltroAnd = ' AND ([Extent2].[AdministrativeUnitId] =' + CAST(@AdministrativeUnitId AS VARCHAR(20)) + ')';
			END
			ELSE
			BEGIN
				SET @FiltroAnd = ' AND 1 = 0';
			END
		END
		IF(@Campo ='10')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent2].[SectionId] IN(SELECT [id] FROM [dbo].[Section] WHERE [Description] LIKE ''%' + @Filtro + '%''))'
		END
		IF(@Campo ='11')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent2].[ResponsibleId] IN(SELECT [id] FROM [dbo].[Responsible] WHERE [Name] LIKE ''%' + @Filtro + '%''))'
		END
		IF(@Campo ='12')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[ValueAcquisition] =CAST(' + REPLACE(@Filtro,',','.') + ' AS DECIMAL(18,2)))' 
		END
		IF(@Campo ='13')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[AssetStartId] IN(SELECT [AssetStartId] FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode]' + 
							 +' AND Convert(int,Convert(varchar(6),[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])' + 
							 +' AND [AccumulatedDepreciation] LIKE ''%' + REPLACE(@Filtro,',','.') + '%''))'
		END
		IF(@Campo ='14')
		BEGIN
			SET @FiltroAnd = 'AND CAST(''' + REPLACE(@Filtro,',','.') + ''' AS DECIMAL(18,2)) = CASE '+
						     +' WHEN [Extent1].[flagAcervo] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition]) '+
							 +' WHEN [Extent1].[flagTerceiro] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition]) '+
							 +' ELSE ISNULL((SELECT top 1 CurrentValue FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode] AND Convert(int,Convert(varchar(6),[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId]) AND [AssetStartId] = [Extent1].[AssetStartId]), ' +
						     + 'CASE WHEN [Extent1].[flagDecreto] = 1 THEN [Extent1].[ValueUpdate] ELSE [Extent1].[ValueAcquisition] END) END '
		END
		IF(@Campo ='15')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[AssetStartId] IN(SELECT [AssetStartId] FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode]' +
							 +' AND Convert(int,Convert(varchar(6),[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])' + 
							 +' AND [RateDepreciationMonthly] =' + REPLACE(@Filtro,',','.')  +'))'
		END
		IF(@Campo ='16')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[MaterialGroupCode] IN(SELECT [Code] FROM [dbo].[MaterialGroup] WHERE [Description] =''%' + @Filtro  +'%''))';
		END

		IF(@Campo ='17')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent11].[ContaContabilApresentacao] LIKE ''%' + @Filtro + '%'')';
			
		END
		IF(@Campo ='18')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent11].[Description] LIKE ''%' + @Filtro  +'%'')';

		END
		IF(@Campo ='19')
		BEGIN
			SET @FiltroAnd = 'AND (CONVERT(varchar,[Extent1].[AcquisitionDate],103) LIKE ''%' + @Filtro + '%'')' ;
		END
		IF(@Campo ='20')
		BEGIN
			SET @FiltroAnd = 'AND (CONVERT(varchar,[Extent1].[MovimentDate],103) LIKE ''%' + @Filtro + '%'')' ;
		END
		IF(@Campo ='21')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[Empenho] LIKE ''%' + @Filtro  +'%'')';
		END
		IF(@Campo ='22')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent2].[NumberDoc] =''' + @Filtro  +''' OR ([Extent1].[NumberDoc] =''' + @Filtro  +''' AND [Extent2].[NumberDoc] IS NULL))';
		END
		IF(@Campo ='23')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent14].[Description] LIKE ''%' + @Filtro  +'%'')';
		END
		IF(@Campo ='24')
		BEGIN
			SET @FiltroAnd = 'AND (CONVERT(varchar,[Extent2].[MovimentDate],103) LIKE ''%' + @Filtro + '%'')' ;
		END
		IF(@CAMPO ='25')
		BEGIN
			IF(@Filtro ='Acervo')
				SET @FiltroAnd = ' AND [Extent1].[flagAcervo] = 1';
			ELSE
				IF(@Filtro ='Terceiro')
					SET @FiltroAnd = ' AND [Extent1].[flagTerceiro] = 1';
					ELSE
						IF(@Filtro ='Decreto')
							SET @FiltroAnd = ' AND [Extent1].[flagDecreto] = 1';
		END;
	END;

	

		SET @SQL = @SQL + ' SELECT '


		SET @SQL = @SQL + ' [Extent3].[Name] ''Sigla''
		,[Extent1].[ChapaCompleta] AS ''Chapa'',  
		CAST([Extent1].[MaterialGroupCode] AS VARCHAR(20)) ''GrupoItem'',
		[Extent1].[MaterialItemCode] AS ''Item'',  
		[Extent4].[Description] AS ''DescricaoDoItem'', 
		[Extent5].[NameManagerReduced] AS ''Orgao'', 
		[Extent6].[Code] AS ''UO'', 
		[Extent7].[Code] AS ''UGE'', 
		[Extent8].[Code] AS ''UA'', 
		[Extent9].[Description] AS ''DescricaoDaDivisao'', 
		[Extent10].[Name] AS ''Responsavel'', 
		[Extent11].[ContaContabilApresentacao] AS ''ContaContabil'',
		[Extent1].[ValueAcquisition] ''ValorDeAquisicao'',
		CASE
			WHEN [Extent1].[flagAcervo] = 1 THEN 0
			WHEN [Extent1].[flagTerceiro] = 1 THEN 0
			ELSE
		(SELECT MAX([Extent12].[AccumulatedDepreciation])
		  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
		  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
		    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
		    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
			AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
		) END AS ''DepreciacaoAcumulada'',
		CASE
			WHEN [Extent1].[flagAcervo] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition])
			WHEN [Extent1].[flagTerceiro] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition])
			WHEN [Extent1].[flagDecreto] = 1 
		    THEN 
				ISNULL((SELECT MIN([Extent12].[CurrentValue])
			  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
			  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
			    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
			    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
				AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
			), [Extent1].[ValueUpdate]) 
			ELSE
			ISNULL((SELECT MIN([Extent12].[CurrentValue])
			  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
			  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
			    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
			    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
				AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
		), [Extent1].[ValueAcquisition]) 
		END AS ''ValorAtual'',
		CASE
			WHEN [Extent1].[flagAcervo] = 1 THEN 0
			WHEN [Extent1].[flagTerceiro] = 1 THEN 0
			ELSE
		(SELECT TOP 1 [Extent12].[RateDepreciationMonthly]
		  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
		  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
		    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
		    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
			AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
		  ORDER BY [Extent12].[Id] DESC
		) END AS ''DepreciacaoMensal'',
		(
			SELECT (SELECT CAST(ISNULL(MAX([Extent12].[CurrentMonth]),0) AS VARCHAR(10))
																		  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
																		  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
																			AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
																			AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
																			AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId]))
																			 + ''/'' + 
			(SELECT CAST(ISNULL(MAX([Extent13].[LifeCycle]),0) AS VARCHAR(10))  
				FROM [dbo].[MaterialGroup] [Extent13] WITH(NOLOCK) 
            WHERE [Extent13].[Code] = [Extent1].[MaterialGroupCode]) 
																		 
		) AS ''VidaUtil'',
		CONVERT(varchar,[Extent1].[AcquisitionDate],103) AS ''DataAquisicao'',
		CONVERT(varchar,[Extent1].[MovimentDate],103) AS ''DataIncorporacao'',
		[Extent1].[Empenho],
		ISNULL([Extent2].[NumberDoc], [Extent1].[NumberDoc]) AS ''NumeroDocumento'',
		[Extent14].[Description] AS ''UltimoHistorico'',
		CONVERT(varchar,[Extent2].[MovimentDate],103) AS ''DataUltimoHistorico'',
		(CASE WHEN [Extent1].[flagAcervo] = 1 THEN ''Acervo'' 
		      ELSE 
			     CASE WHEN [Extent1].[flagTerceiro] = 1 THEN ''Terceiro'' 
			          ELSE  CASE WHEN [Extent1].[flagDecreto] = 1 THEN ''Decreto'' ELSE '''' END
			      END
		  END) AS ''TipoBp'''
	
		SET @SQL = @SQL + ' FROM [dbo].[Asset] AS [Extent1] WITH(NOLOCK)    
		INNER JOIN [dbo].[Initial] AS [Extent3] WITH(NOLOCK) ON [Extent3].[Id] = [Extent1].[InitialId]    
		INNER JOIN [dbo].[ShortDescriptionItem] AS [Extent4] WITH(NOLOCK) ON [Extent4].[Id] = [Extent1].[ShortDescriptionItemId]    
		INNER JOIN [dbo].[AssetMovements] AS [Extent2] WITH(NOLOCK) ON[Extent1].[Id] = [Extent2].[AssetId]    
		INNER JOIN [dbo].[Institution] AS [Extent5] WITH(NOLOCK) ON [Extent5].[Id] = [Extent2].[InstitutionId]    
		INNER JOIN [dbo].[BudgetUnit] AS [Extent6] WITH(NOLOCK) ON [Extent6].[Id] = [Extent2].[BudgetUnitId]    
		INNER JOIN [dbo].[ManagerUnit] AS [Extent7] WITH(NOLOCK) ON [Extent7].[Id] = [Extent2].[ManagerUnitId]    
		INNER JOIN [dbo].[MovementType] AS [Extent14] WITH(NOLOCK) ON [Extent14].[Id] = [Extent2].[MovementTypeId]
		LEFT OUTER JOIN [dbo].[AdministrativeUnit] AS[Extent8] WITH(NOLOCK) ON [Extent8].[Id] = [Extent2].[AdministrativeUnitId]    
		LEFT OUTER JOIN [dbo].[Section] AS [Extent9] WITH(NOLOCK) ON [Extent9].[Id] = [Extent2].[SectionId]    
		LEFT OUTER JOIN [dbo].[Responsible] AS [Extent10]  WITH(NOLOCK) ON [Extent10].[Id] = [Extent2].[ResponsibleId]  
		LEFT OUTER JOIN [dbo].[AuxiliaryAccount] AS [Extent11] WITH(NOLOCK) ON [Extent11].[Id] = [Extent2].[AuxiliaryAccountId] '
		
		IF(@Estado = 3)
		BEGIN
			SET @SQL = @SQL + ' INNER JOIN [dbo].[BPsARealizaremReclassificacaoContabil] AS [Extent15] WITH(NOLOCK) ON [Extent15].[AssetId] = [Extent1].[Id]';
		END

		SET @SQL = @SQL + 'WHERE  ([Extent1].[flagVerificado] IS NULL)        
		  AND ([Extent1].[flagDepreciaAcumulada] = 1)
		  AND ([Extent2].[flagEstorno] IS NULL)
		  AND ([Extent2].[DataEstorno] IS NULL) '
		IF(@Estado IS NOT NULL)
		BEGIN
			IF(@Estado = 1 OR @Estado = 3)
			BEGIN
				SET @SQL = @SQL + ' AND ([Extent2].[Status] = 1)';
			END
			ELSE IF(@Estado = 2)
			BEGIN
				SET @SQL = @SQL + ' AND ([Extent2].[Status] = 1) AND ([Extent2].[MovementTypeId] IN(11,12,47,56,57))'
			END
			ELSE
			BEGIN
				SET @SQL = @SQL + ' AND ([Extent1].[Status] = 0) AND ([Extent2].[Status] = 0) AND ([Extent2].[MovementTypeId] IN(11,12,13,14,15,16,17,18,42,43,45,46,47,48,49,50,51,52,53,55,56,57,58,59,95))';
			END
		END
		ELSE
		BEGIN
		SET @SQL = @SQL + ' AND (0 = 1)';
		END
	
	IF(@BuscaGeral = 1)
	BEGIN
		SET @SQL = @SQL + ' AND ( 0=1 '
	END	
	IF(@NumberIdentificationRetorno IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'OR [Extent1].[NumberIdentification]  LIKE''%' + @Filtro + '%''' 
	END
	IF(@MaterialItemCode IS NOT  NULL)
	BEGIN
		SET @SQL = @SQL + ' OR [Extent1].[MaterialItemCode] =' + CAST(@MaterialItemCode AS VARCHAR(20)) + '' 
	END
	IF(@RateDepreciationMonthly IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'OR [Extent1].[AssetStartId] IN(SELECT [AssetStartId] FROM [dbo].[MonthlyDepreciation] WHERE  [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode] AND [RateDepreciationMonthly] =' + CAST(@RateDepreciationMonthly AS VARCHAR(20)) + ')' 
	END
	IF(@ValueUpdate IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'OR [Extent1].[ValueUpdate] =' + CAST(@ValueUpdate AS VARCHAR(20)) + '' 
	END
	IF(@LifeCycle IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'OR [Extent1].[MaterialGroupCode] IN(SELECT [Code] FROM [dbo].[MaterialGroup] WHERE LifeCycle =' + CAST(@LifeCycle AS VARCHAR(20))+ ')' 
	END	  
	
	IF(@AssetId IS NOT NULL)
	BEGIN
		SET @SQL = @SQL +  ' AND ([Extent1].[Id] IN(SELECT * FROM [dbo].[fnSplitString](@AssetId,'','')))'
	END;

	IF(@NumberDoc IS NOT NULL)
	BEGIN
		SET @SQL = @SQL +  ' OR ([Extent2].[NumberDoc] =''' + @Filtro  +''' OR ([Extent1].[NumberDoc] =''' + @Filtro  +''' AND [Extent2].[NumberDoc] IS NULL))OR [Extent8].Code LIKE ''%' + @Filtro +'%'''
	END;
		IF(@FiltroOr IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + @FiltroOr
	END;
	IF(@BuscaGeral = 1)
	BEGIN
		SET @SQL = @SQL + ')'
	END	
	IF(@InstitutionId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + ' AND ([Extent2].[InstitutionId] =' + CAST(@InstitutionId AS VARCHAR(20)) + ')';
	END
	IF(@BudgetUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[BudgetUnitId] =' + CAST(@BudgetUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@ManagerUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[ManagerUnitId] =' + CAST(@ManagerUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@AdministrativeUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[AdministrativeUnitId] =' + CAST(@AdministrativeUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@CPF IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[ResponsibleId] IN (select Id from Responsible where cpf = ''' + @CPF+'''))';
	END;
	SET @SQL = @SQL + @FiltroAnd;
	
	EXEC(@SQL);
	PRINT(@SQL)
	--PRINT(@FiltroAnd)
	--PRINT(@FiltroOr)
GO

GRANT EXECUTE ON [dbo].[SAM_VISAO_GERAL_EXCEL] TO [ususam]
GO

