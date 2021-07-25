DROP PROCEDURE SAM_CALCULAR_DEPRECIACAO_START_EXECUTAR_FECHAMENTO
GO

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
--CREATE PROCEDURE [dbo].[SAM_CALCULAR_DEPRECIACAO_START_EXECUTAR_FECHAMENTO] ---100112
--	@CodigoUge INT,
--	@DataFinal AS DATETIME,
--	@MaterialItemCode INT
--AS
---- Valores para testes
----SET @CodigoUge = 380273;
----SET @CodigoMaterial = 2409860; 
--BEGIN
--	SET DATEFORMAT dmy;
--	SET LANGUAGE 'Brazilian'

--	DECLARE @DataAtual DATETIME;
--	DECLARE @DataAquisicao DATETIME;
--	DECLARE @AssetStartId INT;
--	DECLARE @Contador SMALLINT = 0;
--	DECLARE @ManagerUnitId INT;
--	DECLARE @InitialId INT;
--	DECLARE @VidaUtil AS SMALLINT;
--	DECLARE @VidaUtilInicio AS SMALLINT;
--	DECLARE @DepreciacaoAcumulada AS DECIMAL(18,2);
--	DECLARE @DepreciacaoMensal AS DECIMAL(18,2);
--	DECLARE @ValorResidual AS DECIMAL(18,2);
--	DECLARE @PorcetagemResidual AS DECIMAL(18,10);
--	DECLARE @ValorAquisicao AS DECIMAL(18,2);
--	DECLARE @ValorAtual AS DECIMAL(18,2);
--	DECLARE @QuantidadeJaDepreciada AS INT;
--	DECLARE @PorcetagemDepreciacaoMensal AS DECIMAL(18,10)
--	DECLARE @Desdobro AS DECIMAL(18,10);
--	DECLARE @DataDecreto AS DATETIME = NULL;
--	DECLARE @ManagerUnitIdTransferencia INT;
--	DECLARE @COUNTITEM SMALLINT;
--	DECLARE @CONTADORITEM SMALLINT = 0;
--	DECLARE @CodigoMaterial INT;
--	DECLARE @ManagerUnitTransferId INT = 0;
--	DECLARE @CurrentMonth SMALLINT = 0;
--	DECLARE @DataIncorporacao DATETIME;
--	DECLARE @DataTransferencia  DATETIME;
--	DECLARE @MonthlyDepreciationId INT;
--	DECLARE @DataTransfere AS DATETIME


--	DECLARE @TableTransferencia AS TABLE(
--		MovimentDate DATETIME NOT NULL,
--		ManagerUnitId INT NOT NULL,
--		MaterialItemCode INT NOT NULL,
--		AssetStartId INT NOT NULL,
--		AssetStartIdTranferencia INT NOT NULL,
--		InitialId	INT NOT NULL
--	);

--	DECLARE @TableItemMaterial AS TABLE(
--		MaterialItemCode INT NOT NULL,
--		ManagerUnitId INT NOT NULL,
--		AssetStartId INT NOT NULL,
--		InitialId	INT NOT NULL,
--		MonthlyDepreciationId INT NOT NULL
--	)

--	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  



--	BEGIN TRY
--		SET @ManagerUnitId = (SELECT TOP 1 [Id] FROM [dbo].[ManagerUnit] WHERE [Code] = @CodigoUge);

--		INSERT INTO @TableItemMaterial(MaterialItemCode, ManagerUnitId,AssetStartId,InitialId,MonthlyDepreciationId)
--		SELECT DISTINCT @MaterialItemCode,@ManagerUnitId,[ast].[AssetStartId],MIN([ast].[InitialId]),[mov].[MonthlyDepreciationId] 
--		 FROM [dbo].[Asset] [ast]
--		  INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id] 
--		 WHERE [ast].[ManagerUnitId] = @ManagerUnitId 
--		   AND [ast].[MaterialItemCode] = @MaterialItemCode
--           AND [ast].[flagVerificado] IS NULL
--           AND [ast].[flagDepreciaAcumulada] = 1 
--		   AND [ast].[AssetStartId] IS NOT NULL
--		   AND ([ast].[flagAcervo]  IS NULL OR [ast].[flagAcervo] = 0)
--		   AND ([ast].[flagTerceiro] = 0 OR  [ast].[flagTerceiro] IS NULL)
--		   AND [mov].[MonthlyDepreciationId] IS NOT NULL
--		 GROUP BY  [ast].[AssetStartId],[mov].[MonthlyDepreciationId]

--		SET @COUNTITEM = (SELECT COUNT(*) FROM @TableItemMaterial);

--		WHILE(@CONTADORITEM < @COUNTITEM)
--		BEGIN
--			BEGIN TRY
--				SET @AssetStartId = (SELECT TOP 1 AssetStartId FROM @TableItemMaterial WHERE [MaterialItemCode] = @MaterialItemCode  AND [ManagerUnitId] = @ManagerUnitId);
--				SET @CodigoMaterial = (SELECT TOP 1 MaterialItemCode FROM @TableItemMaterial WHERE AssetStartId = @AssetStartId);
--				SET @QuantidadeJaDepreciada = (SELECT MAX([CurrentMonth]) FROM [dbo].[MonthlyDepreciation] WHERE [MaterialItemCode] = @MaterialItemCode  AND [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = @AssetStartId);

--				SET @VidaUtil = (SELECT TOP 1 [gr].[LifeCycle] 
--										   FROM [dbo].[MaterialGroup] [gr]
--										   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
--										   WHERE [ast].[MaterialItemCode] = @CodigoMaterial)

--				IF(ISNULL(@QuantidadeJaDepreciada,0) <= @VidaUtil)
--				BEGIN



				

--				SET @MonthlyDepreciationId = (SELECT TOP 1 MonthlyDepreciationId FROM @TableItemMaterial WHERE [AssetStartId] = @AssetStartId);

				
--				SET @InitialId = (SELECT TOP 1 InitialId FROM @TableItemMaterial WHERE AssetStartId = @AssetStartId AND MaterialItemCode = @CodigoMaterial);

--				INSERT INTO @TableTransferencia(MovimentDate,AssetStartId,InitialId,ManagerUnitId,MaterialItemCode,AssetStartIdTranferencia)
--				 SELECT DISTINCT [mov].[MovimentDate],@AssetStartId,[ast].[InitialId],[ast].[ManagerUnitId],@CodigoMaterial, [mov].[AssetTransferenciaId]
--					FROM [dbo].[AssetMovements] [mov] 
--					INNER JOIN [dbo].[Asset] [ast] ON [ast].[Id] = [mov].[AssetId] 
--					WHERE [ast].[MaterialItemCode] = @CodigoMaterial
--					AND [ast].[AssetStartId] = @AssetStartId
--					AND [mov].[AssetTransferenciaId] IS NOT NULL
--					AND [mov].[SourceDestiny_ManagerUnitId] = @ManagerUnitId


--				SET @ValorAquisicao = (SELECT TOP 1 [ValueAcquisition] 
--											FROM [dbo].[Asset] 
--										WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] ASC);

--				SET @ValorAtual=  @ValorAquisicao;
--				SET @DataAquisicao =(SELECT TOP 1 [AcquisitionDate] 
--									   FROM [dbo].[Asset] 
--										WHERE [ManagerUnitId] = @ManagerUnitId
--										  AND [AssetStartId]= @AssetStartId
--										ORDER BY [Id] ASC
--									);
--				SET @DataIncorporacao =(SELECT TOP 1 [MovimentDate] 
--								   FROM [dbo].[Asset] 
--								  WHERE [AssetStartId] = @AssetStartId
--								  ORDER BY [Id] ASC
--								);
--				IF(@DataIncorporacao < @DataAquisicao)
--				BEGIN
--					SET @DataAquisicao = @DataIncorporacao;
--				END;
--				SET @CurrentMonth =(SELECT MAX(CurrentMonth) 
--											   FROM [dbo].[MonthlyDepreciation] 
--											  WHERE [AssetStartId] = @AssetStartId
--											   AND [MaterialItemCode] = @CodigoMaterial
--											   AND [ManagerUnitId] = @ManagerUnitId
--											   );

--				IF(@CurrentMonth IS NULL)
--				BEGIN
--					SET @CurrentMonth = 0;
--				END;
--				SET @DataAtual = (SELECT MovimentDate 
--									FROM @TableTransferencia 
--								   WHERE [AssetStartId] = @AssetStartId);

--				IF(@DataAtual IS NOT NULL)
--				BEGIN
				
--					IF(@CurrentMonth > 0)
--					BEGIN
--						SET @VidaUtilInicio = (SELECT TOP 1 [gr].[LifeCycle] 
--											   FROM [dbo].[MaterialGroup] [gr]
--											   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
--											   WHERE [ast].[MaterialItemCode] = @CodigoMaterial)
			
--						SET @VidaUtil = @VidaUtilInicio - @CurrentMonth;
--						SET @ValorAtual = (SELECT MIN([CurrentValue])
--											FROM [dbo].[MonthlyDepreciation] 
--										WHERE [AssetStartId] = @AssetStartId
--										  AND [MaterialItemCode] = @CodigoMaterial
--										  AND [ManagerUnitId] = @ManagerUnitId
--										);

--					END
--					ELSE
--					   BEGIN
--							SET @VidaUtil =(SELECT TOP 1 [gr].[LifeCycle] 
--											   FROM [dbo].[MaterialGroup] [gr]
--											   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
--											   WHERE [ast].[MaterialItemCode] = @CodigoMaterial);
--					   END;
				
				
--				END;
--				ELSE
--					BEGIN
--						SET @VidaUtil = (SELECT TOP 1 [gr].[LifeCycle] 
--										   FROM [dbo].[MaterialGroup] [gr]
--										   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
--										   WHERE [ast].[MaterialItemCode] = @CodigoMaterial)
--					END;
			
--				SET @ManagerUnitTransferId = (SELECT ManagerUnitId 
--												FROM @TableTransferencia 
--												WHERE [AssetStartId] = @AssetStartId
--												  AND [MaterialItemCode] = @CodigoMaterial);
--				IF(@ManagerUnitTransferId IS NOT NULL)
--				BEGIN
--					SET @DataTransferencia = (SELECT MovimentDate 
--												FROM @TableTransferencia 
--												WHERE [AssetStartId] = @AssetStartId
--												  AND [MaterialItemCode] = @CodigoMaterial);

--					IF(@DataTransferencia IS NOT NULL)
--					BEGIN

--							SET @ManagerUnitIdTransferencia = (SELECT ManagerUnitId 
--												FROM @TableTransferencia 
--												WHERE [AssetStartId] = @AssetStartId
--												  AND [MaterialItemCode] = @CodigoMaterial);
												   
--							SET @CurrentMonth =(SELECT MAX(CurrentMonth) 
--											   FROM [dbo].[MonthlyDepreciation] 
--											  WHERE [AssetStartId] = @AssetStartId
--											    AND [MaterialItemCode] = @CodigoMaterial
--											   );
--							SET @VidaUtilInicio = (SELECT TOP 1 [gr].[LifeCycle] 
--											   FROM [dbo].[MaterialGroup] [gr]
--											   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
--											   WHERE [ast].[MaterialItemCode] = @CodigoMaterial)

--							SET @VidaUtil = @VidaUtilInicio - @CurrentMonth;
--							SET @CurrentMonth = 0;
--							SET @ValorAtual = (SELECT MIN([CurrentValue])
--											FROM [dbo].[MonthlyDepreciation] 
--										  WHERE [AssetStartId] = @AssetStartId
--											    AND [MaterialItemCode] = @CodigoMaterial
--												AND [ManagerUnitId] = @ManagerUnitIdTransferencia
--										);

--							SET @DataIncorporacao =(SELECT TOP 1 [MovimentDate] 
--											   FROM [dbo].[Asset] 
--											  WHERE [AssetStartId] = @AssetStartId
--											  AND [ManagerUnitId] = @ManagerUnitId
--											  ORDER BY [Id] ASC
--											);
--							SET @ManagerUnitTransferId = NULL;

--							DECLARE @DataTransferenciaSequence AS DATETIME;

--							SET @DataTransferenciaSequence = (SELECT [mov].[MovimentDate] FROM [Asset] [bem] 
--													INNER JOIN [AssetMovements] [mov] ON [mov].[AssetId] = [bem].[Id]
--													WHERE [bem].[MaterialItemCode] = @MaterialItemCode  
--														AND [bem].[AssetStartId] = @AssetStartId
--														AND [mov].[ManagerUnitId] = @ManagerUnitId 
--														AND [mov].[AssetTransferenciaId] IS NOT NULL)

--							IF(@DataTransferenciaSequence IS NOT NULL)
--							BEGIN
--								SET @DataTransfere ='01-' + CAST(MONTH(@DataTransferenciaSequence) AS VARCHAR(2)) + '-' +  CAST(YEAR(@DataTransferenciaSequence) AS VARCHAR(4));

--								IF(@DataTransfere < @DataFinal)
--								BEGIN
--									SET @DataFinal = @DataTransferenciaSequence;

--								END;
--							END;
							
--					END;

--				END;
--				ELSE
--					BEGIN
--						SET @DataTransferencia = (SELECT [mov].[MovimentDate] FROM [Asset] [bem] 
--													INNER JOIN [AssetMovements] [mov] ON [mov].[AssetId] = [bem].[Id]
--													WHERE [bem].[MaterialItemCode] = @MaterialItemCode  
--														AND [bem].[AssetStartId] = @AssetStartId
--														AND [mov].[ManagerUnitId] = @ManagerUnitId 
--														AND [mov].[AssetTransferenciaId] IS NOT NULL)
--						IF(@DataTransferencia IS NOT NULL)
--						BEGIN
--							SET @DataTransfere ='01-' + CAST(MONTH(@DataTransferencia) AS VARCHAR(2)) + '-' +  CAST(YEAR(@DataTransferencia) AS VARCHAR(4));

--							IF(@DataTransfere <= @DataFinal)
--							BEGIN
--								SET @DataFinal = @DataTransferencia;
--								SET @DataTransferencia = NULL;
--							END;
--						END;
--					END;

				


				


--				IF(@CurrentMonth < @VidaUtil + 1)
--				BEGIN
--					BEGIN TRY
--						SET @DataDecreto = (SELECT TOP 1 [ast].[MovimentDate] 
--							  FROM [dbo].[Asset] [ast]
--							 WHERE [ast].[flagDecreto]  = 1
--							   AND [ast].[AssetStartId] = @AssetStartId
--							 ORDER BY [ast].[Id] ASC);

--						IF(@DataDecreto IS NOT NULL)
--							BEGIN
--								DECLARE @ValorDecreto AS DECIMAL(18,2);
--									SET @ValorDecreto =  (SELECT TOP 1 [ValueUpdate] 
--													FROM [dbo].[Asset] [ast] 
--													WHERE [AssetStartId] = @AssetStartId);
--									UPDATE [dbo].[Asset] SET [DepreciationDateStart] = @DataDecreto, [DepreciationAmountStart] = @ValorDecreto
--									WHERE [AssetStartId] = @AssetStartId;

							   
--							END
--							ELSE
--								BEGIN
--									UPDATE [dbo].[Asset] SET [DepreciationDateStart] = @DataAquisicao, [DepreciationAmountStart] = @ValorAquisicao
--									WHERE [AssetStartId] = @AssetStartId;
--								END;

--						EXEC [dbo].[SAM_CALCULAR_DEPRECIACAO_START]  @CodigoUge,@CodigoMaterial,@InitialId,@AssetStartId,@DataAquisicao,@DataIncorporacao,@ValorAquisicao,@ValorAtual,@VidaUtil,@ManagerUnitTransferId,@DataFinal,@DataTransferencia,@MonthlyDepreciationId;
--					END TRY
--					BEGIN CATCH
					
--					END CATCH;
--				END;
--			END;
--			PRINT @AssetStartId;
--			DELETE FROM @TableItemMaterial WHERE [AssetStartId] =@AssetStartId AND [ManagerUnitId] = @ManagerUnitId;
--			SET @CONTADORITEM = @CONTADORITEM + 1;
--		END TRY
--		BEGIN CATCH
--			DELETE FROM @TableItemMaterial WHERE [AssetStartId] =@AssetStartId AND [ManagerUnitId] = @ManagerUnitId;
--			SET @CONTADORITEM = @CONTADORITEM + 1;
--		END CATCH
--	END;
--	RETURN @CONTADORITEM;
--	END TRY
--	BEGIN CATCH
--	SELECT  
--		ERROR_NUMBER() AS ErrorNumber  
--		,ERROR_SEVERITY() AS ErrorSeverity  
--		,ERROR_STATE() AS ErrorState  
--		,ERROR_PROCEDURE() AS ErrorProcedure  
--		,ERROR_LINE() AS ErrorLine  
--		,ERROR_MESSAGE() AS ErrorMessage
--		,@CodigoMaterial AS CodigoMaterial;

--	END CATCH;
--END;
