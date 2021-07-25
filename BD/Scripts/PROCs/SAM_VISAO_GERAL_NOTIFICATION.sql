SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SAM_VISAO_GERAL_NOTIFICATION')
DROP PROCEDURE [dbo].[SAM_VISAO_GERAL_NOTIFICATION]  
GO

------EXEC [SAM_VISAO_GERAL_NOTIFICATION] @InstitutionId =43,@BudgetUnitId =217,@ManagerUnitId =1578
CREATE PROCEDURE [dbo].[SAM_VISAO_GERAL_NOTIFICATION]
	@InstitutionId INT =NULL,
	@BudgetUnitId INT = NULL,
	@ManagerUnitId INT = NULL
AS

DECLARE @FiltroAnd AS VARCHAR(150) = '';
DECLARE @SQL AS VARCHAR(2000) = '';	
DECLARE @SQLAuxiliar AS VARCHAR(500) = '';

DECLARE @QtdBolsaSecretaria int;

	SET @SQL = 'DECLARE @Retorno table(qts int);'
	
	SET @SQLAuxiliar = ' count(distinct [Extent1].[Id])
						FROM [dbo].[Asset] AS [Extent1] WITH(NOLOCK)       
						INNER JOIN [dbo].[AssetMovements] AS [Extent2] WITH(NOLOCK) ON [Extent1].[Id] = [Extent2].[AssetId] 	
						INNER JOIN [dbo].[Exchange] AS [Extent3] WITH (NOLOCK) ON [Extent2].[AssetId] = [Extent3].[AssetId]
						WHERE  ([Extent1].[flagVerificado] IS NULL)        
						AND ([Extent1].[flagDepreciaAcumulada] = 1) 
						AND [Extent2].[Status] = 1
						AND [Extent3].[Status] = 1 '

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
	
	SET @SQL = @SQL + 'INSERT INTO @Retorno SELECT ' + @SQLAuxiliar + ' AND [Extent2].[MovementTypeId] = 20 '+ @FiltroAnd + ';';
	
	SET @SQL = @SQL + 'INSERT INTO @Retorno SELECT ' + @SQLAuxiliar + ' AND [Extent2].[MovementTypeId] = 21 '+ @FiltroAnd + ';';
	
	SET @SQL = @SQL + 'INSERT INTO @Retorno SELECT count(distinct AssetId) from [dbo].[AssetMovements] WITH(NOLOCK) where (([Status] = 1 and [MovementTypeId] IN (11,12,47,56,57)) OR ([Status] = 0 and ([FlagEstorno] IS NULL OR [FlagEstorno] = 0) and AssetTransferenciaId IS NULL and [MovementTypeId] IN (42,43))) and [SourceDestiny_ManagerUnitId] = ' + CAST(@ManagerUnitId  AS VARCHAR(20)) + ';select * from @Retorno;';
	
	EXEC(@SQL);

	--PRINT(@SQL)
	--PRINT(@FiltroAnd)
GO

GRANT EXECUTE ON [dbo].[SAM_VISAO_GERAL_NOTIFICATION] TO [ususam]
GO