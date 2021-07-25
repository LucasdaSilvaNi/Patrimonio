DROP PROCEDURE SAM_CRIAR_MONTHLYDEPRECIATION_ID
GO

--CREATE PROCEDURE [dbo].[SAM_CRIAR_MONTHLYDEPRECIATION_ID]
--	@InstitutionId INT,
--	@MaterialItemCode INT,
--	@AssetStartId INT
--AS

--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  

--BEGIN

--	UPDATE [mov] SET [mov].[MonthlyDepreciationId] = @AssetStartId
--	  FROM [dbo].[Asset] [bem] WITH(NOLOCK) 
--	  INNER JOIN [dbo].[AssetMovements] [mov] WITH(NOLOCK) ON [mov].[AssetId] = [bem].[Id] 
--	  INNER JOIN [dbo].[ManagerUnit] [man] WITH(NOLOCK) ON [man].[Id] = [bem].[ManagerUnitId]
--	  INNER JOIN [dbo].[BudgetUnit] [bug] WITH(NOLOCK) ON [bug].[Id]= [man].[BudgetUnitId] 
--	  WHERE [bem].[MaterialItemCode] = @MaterialItemCode
--	    AND [bem].[AssetStartId] = @AssetStartId


--END;