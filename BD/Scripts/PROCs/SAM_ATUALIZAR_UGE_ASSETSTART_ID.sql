DROP PROCEDURE SAM_ATUALIZAR_UGE_ASSETSTART_ID
GO

--CREATE PROCEDURE [dbo].[SAM_ATUALIZAR_UGE_ASSETSTART_ID]

--	@MangerUnitId INT,
--	@MaterialItemCode INT
--AS
--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  


--DECLARE @Table AS TABLE(
--    [Id] INT NOT NULL IDENTITY(1,1),
--    [AssettId] INT NULL,
--	[NumberIdentification] VARCHAR(20) NOT NULL,
--	[MaterialItemCode] INT NOT NULL
--)

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

--INSERT INTO @Table([AssettId]
--				  ,[NumberIdentification]
--				  ,[MaterialItemCode])
--SELECT MIN([ast].[Id])
--      ,[ast].[NumberIdentification]
--      ,[ast].[MaterialItemCode]
--  FROM [dbo].[Asset] [ast]
--  INNER JOIN [dbo].[AssetMovements] [mov] ON [ast].[Id] = [mov].[AssetId]
--  WHERE ([ast].[flagAcervo]  IS NULL OR [ast].[flagAcervo] = 0)
--	AND [ast].[MaterialItemCode] = @MaterialItemCode
--	AND [ast].[ManagerUnitId] = @MangerUnitId
--	AND [mov].[DataEstorno] IS NULL
--	AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] =0)

--  GROUP BY [ast].[NumberIdentification]
--      ,[ast].[MaterialItemCode]




		   



--SET @COUNT =(SELECT COUNT(*) FROM @Table);

--WHILE(@CONTADOR <= @COUNT)
--BEGIN
--	SET @ID = (SELECT TOP 1 [Id] FROM @Table);
--	SET @ASSETSTARTID = (SELECT TOP 1 [AssettId] FROM @Table WHERE [Id] = @ID);
--	SET @CountTransferencia =(SELECT COUNT(*) FROM [AssetMovements] WHERE [AssetTransferenciaId] = @ASSETSTARTID)
--	---Se o bem foi incorporado por transferência, não vai ser alterado o AssetStartId, o mesmo vai ser alterado pela origem do bem.
--	IF(ISNULL(@CountTransferencia,0) < 1)
--	BEGIN
--		UPDATE [bem] SET  [bem].[AssetStartId] = @ASSETSTARTID
--		FROM [dbo].[Asset] [bem]
--		WHERE ([bem].[Id] = @ASSETSTARTID)
--		    AND ([bem].[flagAcervo]  IS NULL OR [bem].[flagAcervo] = 0)
--			AND ([bem].[flagTerceiro] = 0 OR  [bem].[flagTerceiro] IS NULL)
--			AND [bem].[MaterialItemCode] = @MaterialItemCode
--			AND [bem].[ManagerUnitId] = @MangerUnitId
				
--		UPDATE [mov] SET  [mov].[MonthlyDepreciationId] = @ASSETSTARTID
--		FROM [dbo].[Asset] [bem]
--		INNER JOIN [dbo].[AssetMovements] [mov] ON [bem].[Id] = [mov].[AssetId]
--		WHERE  ([bem].[Id] = @ASSETSTARTID)
--		    AND ([bem].[flagAcervo]  IS NULL OR [bem].[flagAcervo] = 0)
--			AND ([bem].[flagTerceiro] = 0 OR  [bem].[flagTerceiro] IS NULL)
--			AND [bem].[MaterialItemCode] = @MaterialItemCode
--			AND [bem].[ManagerUnitId] = @MangerUnitId
--			AND [mov].[DataEstorno] IS NULL
--			AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] =0)
		
--	END;

--	DELETE FROM @Table WHERE [Id] = @ID
--	SET @CONTADOR = @CONTADOR + 1;

--END;

--SET @CONTADOR = 0;
--SET @COUNT = 0;
