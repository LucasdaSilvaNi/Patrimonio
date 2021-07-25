DROP PROCEDURE SAM_ATUALIZAR_UGE_TRANSFERENCIA_ASSETSTART_ID
GO

--CREATE PROCEDURE [dbo].[SAM_ATUALIZAR_UGE_TRANSFERENCIA_ASSETSTART_ID]

--	@MangerUnitId INT,
--	@MaterialItemCode INT
--AS
--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  


--DECLARE @Table2 AS TABLE(
--    [Id] INT NOT NULL IDENTITY(1,1),
--  	[AssetTransferenciaId] INT NOT NULL,
--	[AssettId] INT NULL
--)


--DECLARE @COUNT INT = 0;
--DECLARE @CONTADOR INT = 0;
--DECLARE @ID INT= 0;
--DECLARE @CountTransferencia INT =0;
--DECLARE @ASSETSTARTID INT= 0;
--DECLARE @TRANSFERENCIAID INT = 0;




------TRANSFERENCIA

--INSERT INTO @Table2( [AssetTransferenciaId],[AssettId])
--SELECT [mov].[AssetTransferenciaId],[mov].[AssetId]
--  FROM [dbo].[AssetMovements] [mov]
--  WHERE [mov].[ManagerUnitId] = @MangerUnitId
--	AND ([mov].[SourceDestiny_ManagerUnitId] IS NOT NULL)
--	AND ([mov].[AssetTransferenciaId]  IS NOT NULL)

--  GROUP BY [mov].[AssetTransferenciaId],[mov].[AssetId]

--SET @COUNT =(SELECT COUNT(*) FROM @Table2);

--WHILE(@CONTADOR <= @COUNT)
--BEGIN
--	SET @ID = (SELECT TOP 1 [Id] FROM @Table2);
--	SET @ASSETSTARTID = (SELECT TOP 1 [AssettId] FROM @Table2 WHERE [Id] = @ID);
--	SET @TRANSFERENCIAID =(SELECT [AssetTransferenciaId] FROM @Table2 WHERE [Id] = @ID)
--		PRINt(@ASSETSTARTID);
--		print(@TRANSFERENCIAID)
--		UPDATE [bem] SET [bem].[AssetStartId] = @ASSETSTARTID
--		FROM [dbo].[Asset] [bem]
--		 WHERE [bem].[Id] = @ASSETSTARTID

--		UPDATE [bem] SET [bem].[AssetStartId] = @ASSETSTARTID
--		FROM [dbo].[Asset] [bem]
--		 WHERE [bem].[Id] = @TRANSFERENCIAID
	
--		UPDATE [mov] SET  [mov].[MonthlyDepreciationId] = @ASSETSTARTID
--		FROM [dbo].[Asset] [bem]
--		INNER JOIN [dbo].[AssetMovements] [mov] ON [bem].[Id] = [mov].[AssetId]
--		 WHERE [bem].[Id] = @ASSETSTARTID

--		UPDATE [mov] SET  [mov].[MonthlyDepreciationId] = @ASSETSTARTID
--		FROM [dbo].[Asset] [bem]
--		INNER JOIN [dbo].[AssetMovements] [mov] ON [bem].[Id] = [mov].[AssetId]
--		 WHERE [bem].[Id] = @TRANSFERENCIAID

--	DELETE FROM @Table2 WHERE [Id] = @ID
--	SET @CONTADOR = @CONTADOR + 1;

--END;
