DROP PROCEDURE SAM_EXECUTAR_DEPRECIACAO_POR_UGE
GO

--CREATE PROCEDURE [dbo].[SAM_EXECUTAR_DEPRECIACAO_POR_UGE]
--	@UGE_Code INT,
--	@DataFinal DATETIME,
--	@DeletarDepreciacaoExistente BIT = 0

--AS
--SET DATEFORMAT YMD;

--DECLARE @TABLE AS TABLE(
--	[Id] INT NOT NULL IDENTITY(1,1),
--	[AssetStartId] INT NOT NULL,
--	[ManagerUnitId] INT NOT NULL,
--	[MaterialItemCode] INT NOT NULL
--)
--DECLARE @COUNT AS INT = 0;
--DECLARE @CONTADOR AS INT = 0;
--DECLARE @ASSETSTARTID INT = 0;
--DECLARE @Id INT;
--DECLARE @MaterialItemCode INT = 0;
--DECLARE @ManagerUnitId INT;
--DECLARE @Retorno AS NVARCHAR(100)
--DECLARE @Erro AS BIT;

------Seta o id da UGE
--SET @ManagerUnitId =(SELECT TOP 1 [Id] FROM [dbo].[ManagerUnit] WHERE [Code] = @UGE_Code)

--	IF(@DeletarDepreciacaoExistente = 1)
--	BEGIN
--		PRINT('Deletar Bens Transferidos');
--		----Bens Transferidos
--		INSERT INTO @TABLE([AssetStartId],[ManagerUnitId],[MaterialItemCode])
--		SELECT [ast].[AssetStartId],[ast].[ManagerUnitId],[ast].[MaterialItemCode]
--				 FROM [dbo].[Asset] [ast]
--				  INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id] 
--				 WHERE  [ast].[flagVerificado] IS NULL
--				   AND [ast].[flagDepreciaAcumulada] = 1 
--				   AND [ast].[AssetStartId] IS NOT NULL
--				   AND ([ast].[flagAcervo]  IS NULL OR [ast].[flagAcervo] = 0)
--				   AND ([ast].[flagTerceiro] = 0 OR  [ast].[flagTerceiro] IS NULL)
--				   AND [mov].[SourceDestiny_ManagerUnitId] = @ManagerUnitId
--				   AND [mov].[AssetTransferenciaId] IS NOT NULL


--				 GROUP BY  [ast].[AssetStartId],[ast].[ManagerUnitId],[ast].[MaterialItemCode]

--		SET @COUNT = (SELECT COUNT(*) FROM @TABLE);

--		WHILE(@CONTADOR <= @COUNT)
--		BEGIN
--			SET @Id = (SELECT TOP 1 [Id] FROM @TABLE)
--			SET @ASSETSTARTID = (SELECT [AssetStartId] FROM @TABLE WHERE [Id] = @Id)
--			SET @MaterialItemCode = (SELECT [MaterialItemCode] FROM @TABLE WHERE [Id] = @Id)
--			SET @ManagerUnitId = (SELECT [ManagerUnitId] FROM @TABLE WHERE [Id] = @Id)

--			---Deleta a depreciação já existente da UGE a que realizou a transferencia

--			DELETE FROM [dbo].[MonthlyDepreciation]  WHERE  [ManagerUnitId] = @ManagerUnitId AND MaterialItemCode =@MaterialItemCode AND AssetStartId = @ASSETSTARTID;

--			UPDATE [bem] SET [bem].[ValueUpdate] = [bem].[DepreciationAmountStart]
--			      FROM [dbo].[Asset] [bem] 
--				  WHERE [bem].[AssetStartId] = @AssetStartId 
--				    AND [bem].[ManagerUnitId] = @ManagerUnitId
--					AND [bem].[MaterialItemCode] = @MaterialItemCode 
--					AND [bem].[DepreciationDateStart] IS NOT NULL
--					AND [bem].[ValueUpdate] < [bem].[DepreciationAmountStart]
				  
--			DELETE @TABLE WHERE [Id] = @Id
--			SET @CONTADOR = @CONTADOR + 1;
--		END;
--END;
--SET @CONTADOR = 0
--SET @COUNT = 0
------Seta o id da UGE
--SET @ManagerUnitId =(SELECT TOP 1 [Id] FROM [dbo].[ManagerUnit] WHERE [Code] = @UGE_Code)

--PRINT('Depreciar Bens Transferidos');
--INSERT INTO @TABLE([AssetStartId],[ManagerUnitId],[MaterialItemCode])
--SELECT [ast].[AssetStartId],[ast].[ManagerUnitId],[ast].[MaterialItemCode]
--		 FROM [dbo].[Asset] [ast]
--		  INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id] 
--		 WHERE  [ast].[flagVerificado] IS NULL
--           AND [ast].[flagDepreciaAcumulada] = 1 
--		   AND [ast].[AssetStartId] IS NOT NULL
--		   AND ([ast].[flagAcervo]  IS NULL OR [ast].[flagAcervo] = 0)
--		   AND ([ast].[flagTerceiro] = 0 OR  [ast].[flagTerceiro] IS NULL)
--		   AND [mov].[SourceDestiny_ManagerUnitId] = @ManagerUnitId
--		   AND [mov].[AssetTransferenciaId] IS NOT NULL


--		 GROUP BY  [ast].[AssetStartId],[ast].[ManagerUnitId],[ast].[MaterialItemCode]

--SET @COUNT = (SELECT COUNT(*) FROM @TABLE);

--WHILE(@CONTADOR <= @COUNT)
--BEGIN
	
--    SET @Id = (SELECT TOP 1 [Id] FROM @TABLE)
--	SET @ASSETSTARTID = (SELECT [AssetStartId] FROM @TABLE WHERE [Id] = @Id)
--	SET @MaterialItemCode = (SELECT [MaterialItemCode] FROM @TABLE WHERE [Id] = @Id)
--	SET @ManagerUnitId = (SELECT [ManagerUnitId] FROM @TABLE WHERE [Id] = @Id)

--	EXEC [dbo].[SAM_ATUALIZAR_UGE_ASSETSTART_ID]
--	@MangerUnitId =@ManagerUnitId,
--	@MaterialItemCode =@MaterialItemCode


--	EXEC [SAM_ATUALIZAR_UGE_TRANSFERENCIA_ASSETSTART_ID]
--	@MangerUnitId =@ManagerUnitId,
--	@MaterialItemCode =@MaterialItemCode

--	EXEC [SAM_DEPRECIACAO_UGE]
--	@ManagerUnitId = @ManagerUnitId,
--	@MaterialItemCode =@MaterialItemCode,
--	@AssetStartId = @ASSETSTARTID,
--	@DataFinal = @DataFinal,
--	@Retorno = @Retorno OUT,
--	@Erro = @Erro OUT

--	DELETE @TABLE WHERE [Id] = @Id
--	SET @CONTADOR = @CONTADOR + 1;
--END;


--SET @CONTADOR = 0
--SET @COUNT = 0
--SET @ManagerUnitId =(SELECT TOP 1 [Id] FROM [dbo].[ManagerUnit] WHERE [Code] = @UGE_Code)

--IF(@DeletarDepreciacaoExistente = 1)
--	BEGIN
--		PRINT('Deletar Bens incorporados');
--		----Bens incorporados
--		INSERT INTO @TABLE([AssetStartId],[ManagerUnitId],[MaterialItemCode])
--		SELECT [ast].[AssetStartId],[ast].[ManagerUnitId],[ast].[MaterialItemCode]
--				 FROM [dbo].[Asset] [ast]
--				  INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id] 
--				 WHERE  [ast].[flagVerificado] IS NULL
--				   AND [ast].[flagDepreciaAcumulada] = 1 
--				   AND [ast].[AssetStartId] IS NOT NULL
--				   AND ([ast].[flagAcervo]  IS NULL OR [ast].[flagAcervo] = 0)
--				   AND ([ast].[flagTerceiro] = 0 OR  [ast].[flagTerceiro] IS NULL)
--				   AND [ast].[AssetStartId] IS NOT NULL
--				   AND [ast].[ManagerUnitId] = @ManagerUnitId
--				   AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] = 0)

--				 GROUP BY  [ast].[AssetStartId],[ast].[ManagerUnitId],[ast].[MaterialItemCode]

--		SET @COUNT = (SELECT COUNT(*) FROM @TABLE);

--		WHILE(@CONTADOR <= @COUNT)
--		BEGIN
--			SET @Id = (SELECT TOP 1 [Id] FROM @TABLE)
--			SET @ASSETSTARTID = (SELECT [AssetStartId] FROM @TABLE WHERE [Id] = @Id)
--			SET @MaterialItemCode = (SELECT [MaterialItemCode] FROM @TABLE WHERE [Id] = @Id)


--			---Deleta a depreciação já existente da UGE a ser depreciada
--			DELETE FROM [dbo].[MonthlyDepreciation]  WHERE  ManagerUnitId = @ManagerUnitId AND MaterialItemCode = @MaterialItemCode AND	AssetStartId = @ASSETSTARTID

--			UPDATE [bem] SET [bem].[ValueUpdate] = [bem].[DepreciationAmountStart]
--			      FROM [dbo].[Asset] [bem] 
--				  WHERE [bem].[AssetStartId] = @AssetStartId 
--				    AND [bem].[ManagerUnitId] = @ManagerUnitId
--					AND [bem].[MaterialItemCode] = @MaterialItemCode 
--					AND [bem].[DepreciationDateStart] IS NOT NULL
--					AND [bem].[ValueUpdate] < [bem].[DepreciationAmountStart]

--			DELETE @TABLE WHERE [Id] = @Id
--			SET @CONTADOR = @CONTADOR + 1;
--		END;
--END

--SET @CONTADOR = 0
--SET @COUNT = 0
--PRINT('Depreciar Bens incorporados');
------Bens incorporados
--INSERT INTO @TABLE([AssetStartId],[ManagerUnitId],[MaterialItemCode])
--SELECT [ast].[AssetStartId],[ast].[ManagerUnitId],[ast].[MaterialItemCode]
--		 FROM [dbo].[Asset] [ast]
--		  INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id] 
--		 WHERE  [ast].[flagVerificado] IS NULL
--           AND [ast].[flagDepreciaAcumulada] = 1 
--		   AND [ast].[MaterialGroupCode] <> 0
--		   AND [ast].[AssetStartId] IS NOT NULL
--		   AND ([ast].[flagAcervo]  IS NULL OR [ast].[flagAcervo] = 0)
--		   AND ([ast].[flagTerceiro] = 0 OR  [ast].[flagTerceiro] IS NULL)
--		   AND [ast].[ManagerUnitId] = @ManagerUnitId
--		   AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] = 0)

--		 GROUP BY  [ast].[AssetStartId],[ast].[ManagerUnitId],[ast].[MaterialItemCode]

--SET @COUNT = (SELECT COUNT(*) FROM @TABLE);

--WHILE(@CONTADOR <= @COUNT)
--BEGIN

--    SET @Id = (SELECT TOP 1 [Id] FROM @TABLE)
--	SET @ASSETSTARTID = (SELECT [AssetStartId] FROM @TABLE WHERE [Id] = @Id)
--	SET @MaterialItemCode = (SELECT [MaterialItemCode] FROM @TABLE WHERE [Id] = @Id)

--	EXEC [dbo].[SAM_ATUALIZAR_UGE_ASSETSTART_ID]
--	@MangerUnitId =@ManagerUnitId,
--	@MaterialItemCode =@MaterialItemCode

--	EXEC [SAM_DEPRECIACAO_UGE]
--	@ManagerUnitId = @ManagerUnitId,
--	@MaterialItemCode =@MaterialItemCode,
--	@AssetStartId = @ASSETSTARTID,
--	@DataFinal = @DataFinal,
--	@Retorno = @Retorno OUT,
--	@Erro = @Erro OUT

--	DELETE @TABLE WHERE [Id] = @Id
--	SET @CONTADOR = @CONTADOR + 1;
--END;
