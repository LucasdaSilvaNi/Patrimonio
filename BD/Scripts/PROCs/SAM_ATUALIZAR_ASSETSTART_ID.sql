DROP PROCEDURE SAM_ATUALIZAR_ASSETSTART_ID
GO

--CREATE PROCEDURE [dbo].[SAM_ATUALIZAR_ASSETSTART_ID]
--	@InstitutionId INT,
--	@ManangerUnitId INT,
--	@MaterialItemCode INT
--AS
--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  


--DECLARE @Table AS TABLE(
--    [AssettId] INT NULL,
--	[InitialId] INT NOT NULL,
--	[NumberIdentification] VARCHAR(20) NOT NULL,
--	[MaterialItemCode] INT NOT NULL,
--	[ManagerUnitId] INT NOT NULL
--)


--INSERT INTO @Table([AssettId]
--                  ,[InitialId]
--				  ,[NumberIdentification]
--				  ,[MaterialItemCode]
--				  ,[ManagerUnitId])
--SELECT MIN([ast].[Id])
--      ,[ast].[InitialId]
--      ,[ast].[NumberIdentification]
--      ,[ast].[MaterialItemCode]
--      ,[ast].[ManagerUnitId]
--  FROM [dbo].[Asset] [ast]
--  INNER JOIN [dbo].[ManagerUnit] [man] WITH(NOLOCK) ON [man].[Id] = [ast].[ManagerUnitId]
--  INNER JOIN [dbo].[AssetMovements] [mov] ON [ast].[Id] = [mov].[AssetId]
--  WHERE ([ast].[flagAcervo]  IS NULL OR [ast].[flagAcervo] = 0)
--	AND ([ast].[flagTerceiro] = 0 OR  [ast].[flagTerceiro] IS NULL)
--    AND [ast].[MovementTypeId] <> 11
--	AND [ast].[MaterialItemCode] = @MaterialItemCode
--	AND [ast].[ManagerUnitId] = @ManangerUnitId
--	AND [mov].[InstitutionId] = @InstitutionId
--	AND [mov].[DataEstorno] IS NULL
--	AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] =0)
	
	

--  GROUP BY [ast].[InitialId]
--      ,[ast].[NumberIdentification]
--      ,[ast].[MaterialItemCode]
--      ,[ast].[ManagerUnitId]


--UPDATE [bem] SET [bem].[AssetStartId] = [table].[AssettId]
--FROM [dbo].[Asset] [bem]
--INNER JOIN [dbo].[ManagerUnit] [man] WITH(NOLOCK) ON [man].[Id] = [bem].[ManagerUnitId]
--INNER JOIN [dbo].[BudgetUnit] [bug] WITH(NOLOCK) ON [bug].[Id]= [man].[BudgetUnitId] 
--INNER JOIN [dbo].[AssetMovements] [mov] ON [bem].[Id] = [mov].[AssetId]
--INNER JOIN @Table [table] ON [bem].[InitialId] = [table].[InitialId]
--  AND [bem].[NumberIdentification] = [table].[NumberIdentification]
--  AND [bem].[MaterialItemCode] = [table].[MaterialItemCode]
--  AND [bem].[ManagerUnitId] = [table].[ManagerUnitId]
--  AND [bug].[InstitutionId] = @InstitutionId
-- WHERE  ([bem].[flagAcervo]  IS NULL OR [bem].[flagAcervo] = 0)
--	AND ([bem].[flagTerceiro] = 0 OR  [bem].[flagTerceiro] IS NULL)
--	AND [bem].[MaterialItemCode] = @MaterialItemCode
--	AND [bem].[ManagerUnitId] = @ManangerUnitId
--	AND [mov].[DataEstorno] IS NULL
--	AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] =0)



--  DECLARE @TABLE1 AS TABLE(
--	[AssetTransferenciaId] INT NOT NULL  )

--  INSERT INTO @TABLE1([AssetTransferenciaId])   
--  SELECT [mov].[AssetId]
--   FROM  [dbo].[AssetMovements] [mov] 
--   INNER JOIN [Asset] [bem] ON [mov].[AssetId] = [bem].[Id]
--   WHERE [mov].[AssetTransferenciaId] IS NOT NULL
--     AND [mov].[ManagerUnitId] = @ManangerUnitId
--	 AND [bem].[MaterialItemCode] = @MaterialItemCode
--	 AND [mov].[DataEstorno] IS NULL
--	 AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] =0)

--  GROUP BY [mov].[AssetId]
    

--  UPDATE [bem] SET [bem].[AssetStartId] = [table].[AssetTransferenciaId]
--FROM [dbo].[Asset] [bem]
--INNER JOIN [dbo].[AssetMovements] [mov] ON [bem].[Id] = [mov].[AssetId]
--INNER JOIN @TABLE1 [table] ON [table].[AssetTransferenciaId] = [mov].[AssetTransferenciaId]
-- WHERE [mov].[AssetTransferenciaId] IS NOT NULL
-- AND [mov].[InstitutionId] = @InstitutionId
-- AND [mov].[ManagerUnitId]  = @ManangerUnitId
-- AND [mov].[DataEstorno] IS NULL
-- AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] =0)
-- AND [bem].[MaterialItemCode] = @MaterialItemCode
-- AND ([bem].[flagAcervo]  IS NULL OR [bem].[flagAcervo] = 0)
-- AND ([bem].[flagTerceiro] = 0 OR  [bem].[flagTerceiro] IS NULL)

--  DECLARE @TABLE2 AS TABLE(
--	[AssetTransferenciaId] INT NOT NULL,
--	[AssetId] INT NOT NULL
--	  )
--  INSERT INTO @TABLE2([AssetTransferenciaId],[AssetId])   
--  SELECT [mov].[AssetTransferenciaId],[mov].[AssetId]
--   FROM  [dbo].[AssetMovements] [mov] 
--   INNER JOIN [Asset] [bem] ON [mov].[AssetId] = [bem].[Id]
--   WHERE [mov].[AssetTransferenciaId] IS NOT NULL
--     AND [mov].[ManagerUnitId] = @ManangerUnitId
--	 AND [bem].[MaterialItemCode] = @MaterialItemCode
--	 AND [mov].[DataEstorno] IS NULL
--	 AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] =0)

--  GROUP BY [mov].[AssetTransferenciaId],[mov].[AssetId]

--UPDATE [bem] SET [bem].[AssetStartId] = [table].[AssetId]
--FROM [dbo].[Asset] [bem]
--INNER JOIN @TABLE2 [table] ON [table].[AssetTransferenciaId] = [bem].[Id]
-- WHERE [bem].[MaterialItemCode] = @MaterialItemCode
-- AND ([bem].[flagAcervo]  IS NULL OR [bem].[flagAcervo] = 0)
-- AND ([bem].[flagTerceiro] = 0 OR  [bem].[flagTerceiro] IS NULL) 
