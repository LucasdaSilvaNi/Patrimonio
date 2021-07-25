DROP PROCEDURE CRIAR_ASSETSTARTID_DO_ORGAO
GO

--CREATE PROCEDURE [dbo].[CRIAR_ASSETSTARTID_DO_ORGAO]
--	@InstitutionId INT
--AS


--DECLARE @TableAssetStartId AS TABLE(Id INT IDENTITY(1,1), AssetStartId INT,MaterialItemCode INT, ManagerUnitId INT);


--DECLARE @ManagerUnitId INT;
--DECLARE @MaterialItemCode INT;
--DECLARE @ContadorAssetStartId AS INT = 0;
--DECLARE @CountAssetStartId AS INT = 0;
--DECLARE @AssetStartId INT;
--DECLARE @Id INT;

--SET DATEFORMAT YMD;

--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  

	
--INSERT INTO @TableAssetStartId(AssetStartId,MaterialItemCode,ManagerUnitId)
--SELECT [bem].[AssetStartId],[bem].[MaterialItemCode],[bem].[ManagerUnitId] FROM [Asset] [bem] WITH(NOLOCK)
--INNER JOIN [AssetMovements] [mov] WITH(NOLOCK) ON [mov].[AssetId] = [bem].[Id] 
--WHERE [mov].[MonthlyDepreciationId] IS NULL
--    AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] =0)
--    AND [mov].[DataEstorno] IS NULL
--    AND [mov].[MonthlyDepreciationId] IS NULL
--	AND [mov].[InstitutionId]  =  @InstitutionId
--    AND [bem].[MovementTypeId] <> 11
--    AND [bem].[AssetStartId] IS NOT NULL
--    AND [bem].[MaterialGroupCode] <> 0
--    AND [bem].[flagVerificado] IS NULL
--    AND [bem].[flagDepreciaAcumulada] = 1
--    AND [bem].[AssetStartId] IS NOT NULL
--    AND ([bem].[flagAcervo] IS NULL OR [bem].[flagAcervo] = 0)
--    AND ([bem].[flagTerceiro] IS NULL OR [bem].[flagTerceiro] = 0)
--GROUP BY [bem].[AssetStartId],[bem].[MaterialItemCode],[bem].[ManagerUnitId]

--SET @CountAssetStartId = (SELECT COUNT(*) FROM @TableAssetStartId);
--WHILE(@ContadorAssetStartId <= @CountAssetStartId)
--BEGIN
--	SET @Id = (SELECT TOP 1 Id FROM @TableAssetStartId)
--	SET @AssetStartId = (SELECT TOP 1 AssetStartId FROM @TableAssetStartId WHERE Id= @Id)
--	SET @ManagerUnitId = (SELECT TOP 1 ManagerUnitId FROM @TableAssetStartId WHERE Id= @Id)
--	SET @MaterialItemCode = (SELECT TOP 1 MaterialItemCode FROM @TableAssetStartId WHERE Id= @Id)
	

--	EXEC [dbo].[SAM_ATUALIZAR_ASSETSTART_ID]
--	@InstitutionId =@InstitutionId,
--	@ManangerUnitId = @ManagerUnitId,
--	@MaterialItemCode = @MaterialItemCode

--	EXEC [dbo].[SAM_CRIAR_MONTHLYDEPRECIATION_ID]
--	@InstitutionId =@InstitutionId,
--	@MaterialItemCode = @MaterialItemCode,
--	@AssetStartId =@AssetStartId

--	DELETE FROM @TableAssetStartId WHERE Id = @Id
--	SET @ContadorAssetStartId = @ContadorAssetStartId + 1;
--END;
