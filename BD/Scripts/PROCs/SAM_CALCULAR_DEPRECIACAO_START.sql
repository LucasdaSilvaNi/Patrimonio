SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'SAM_CALCULAR_DEPRECIACAO_START')
BEGIN
    DROP PROCEDURE [dbo].[SAM_CALCULAR_DEPRECIACAO_START]
END
GO

CREATE PROCEDURE [dbo].[SAM_CALCULAR_DEPRECIACAO_START] --380273,2409860,12544,3569147,120, null
	@CodigoUge INT,
	@CodigoMaterial INT,
	@InitialId INT,
	@AssetStartId INT, 
	@DataAquisicao DATETIME,
	@DataIncorporacao DATETIME,
	@ValorAquisicao AS DECIMAL(18,2),
	@ValorAtual AS DECIMAL(18,2),
	@VidaUtil SMALLINT,
	@ManagerUnitTransferId INT,
	@DataFinal DATETIME,
	@DataTransferencia DATETIME,
	@MonthlyDepreciationId INT
	AS
	-- Valores para testes
	--SET @CodigoUge = 380273;
	--SET @CodigoMaterial = 2409860; 
	BEGIN
	SET DATEFORMAT dmy;
	SET LANGUAGE 'Brazilian'
	DECLARE @CurrentMonth SMALLINT=0;
	DECLARE @Contador SMALLINT = 0;
	DECLARE @ManagerUnitId INT;
	DECLARE @DataAtual AS DATETIME;
	DECLARE @VidaUtilInicio AS SMALLINT;
	DECLARE @DepreciacaoAcumulada AS DECIMAL(18,2);
	DECLARE @DepreciacaoMensal AS DECIMAL(18,2);
	DECLARE @ValorResidual AS DECIMAL(18,2);
	DECLARE @PorcetagemResidual AS DECIMAL(18,10);
	DECLARE @QuantidadeJaDepreciada AS INT;
	DECLARE @PorcetagemDepreciacaoMensal AS DECIMAL(18,10)
	DECLARE @Desdobro AS DECIMAL(18,10);
	DECLARE @DataDecreto AS DATETIME = NULL;
	DECLARE @Decreto AS BIT =  0;
	DECLARE @ContadorDecreto AS SMALLINT = 0;
	DECLARE @ManagerUnitIdTransfer INT;
	DECLARE @CurrentMonthSequencia SMALLINT=0;
	DECLARE @DataMovimentoTransferencia DATETIME = NULL;

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  


	BEGIN TRY

		SET @ManagerUnitId = (SELECT TOP 1 [Id] FROM [dbo].[ManagerUnit] WHERE [Code] = @CodigoUge);

				--- Retornar a vida util do item de material
		SET @VidaUtilInicio = (SELECT TOP 1 [gr].[LifeCycle] 
							FROM [dbo].[MaterialGroup] [gr]
							INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
							WHERE [ast].[AssetStartId] = @AssetStartId
							); 
		SET @DataDecreto = (SELECT TOP 1 [ast].[MovimentDate] 
								FROM [dbo].[Asset] [ast]
								WHERE [ast].[flagDecreto]  = 1
								AND [ast].[AssetStartId] = @AssetStartId
								ORDER BY [ast].[Id] ASC);
		

		IF(@DataIncorporacao < @DataAquisicao OR @DataTransferencia IS NOT NULL)
		BEGIN
			SET @DataAtual = @DataIncorporacao;
		END;
		ELSE
			BEGIN
				SET @DataAtual = @DataAquisicao;
			END;
		IF(@DataTransferencia IS NOT NULL)
		BEGIN
			SET @CurrentMonthSequencia =(SELECT MAX(CurrentMonth)
							FROM [dbo].[MonthlyDepreciation] 
							WHERE [AssetStartId] = @AssetStartId);

			IF(@CurrentMonthSequencia IS NOT NULL)
			BEGIN
				SET @DataAtual = @DataTransferencia;
			END;
		END;

		IF(@CurrentMonthSequencia IS NULL)
		BEGIN 
			SET @CurrentMonthSequencia = 0;
		END;

		--- Loop para criar as linhas calculadas
		WHILE(@Contador <= @VidaUtil)
		BEGIN
			BEGIN TRY


			SET @DataMovimentoTransferencia = (SELECT TOP 1 MovimentDate 
												FROM [dbo].[AssetMovements]  
												WHERE [AssetId] = @AssetStartId
												  AND [ManagerUnitId] = @ManagerUnitId
												  AND [AssetTransferenciaId] IS NOT NULL);
			IF(@DataMovimentoTransferencia IS NOT NULL)
			BEGIN
				IF(YEAR(@DataMovimentoTransferencia) = YEAR(@DataAtual) AND MONTH(@DataMovimentoTransferencia) = MONTH(@DataAtual))
				BEGIN
					BREAK;
				END
			END;

			SET @QuantidadeJaDepreciada = (SELECT COUNT(Id) FROM [dbo].[MonthlyDepreciation] 
												WHERE [AssetStartId] = @AssetStartId
													AND [CurrentDate]= @DataAtual
													AND [ManagerUnitId] = @ManagerUnitId
													AND [MaterialItemCode] = @CodigoMaterial)
		
				IF(@Contador = 0)
					BEGIN
						SET @Desdobro =0;
						SET @DepreciacaoAcumulada = 0;
						SET @PorcetagemDepreciacaoMensal = (SELECT TOP 1 [gr].[RateDepreciationMonthly] 
													FROM [dbo].[MaterialGroup] [gr]
													INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
													WHERE [ast].[AssetStartId] = @AssetStartId
													); 
				
						SET @PorcetagemResidual = (SELECT TOP 1 [gr].[ResidualValue] 
														FROM [dbo].[MaterialGroup] [gr]
														INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
													WHERE [ast].[AssetStartId] = @AssetStartId); 
				
						

						
						SET @ValorResidual =ROUND((@ValorAquisicao * ROUND(@PorcetagemResidual,2,1)) / 100,2,1);
						SET @DepreciacaoMensal = ROUND((@ValorAquisicao - @ValorResidual) / @VidaUtilInicio,2,1);
						
						
						IF(ISNULL(@QuantidadeJaDepreciada,0) < 1)
						BEGIN
							INSERT INTO [dbo].[MonthlyDepreciation]
									([AssetStartId]
									,[ManagerUnitId]
									,[InitialId]
									,[MaterialItemCode]
									,[AcquisitionDate]
									,[CurrentDate]
									,[DateIncorporation]
									,[LifeCycle]
									,[CurrentMonth]
									,[ValueAcquisition]
									,[CurrentValue]
									,[ResidualValue]
									,[RateDepreciationMonthly]
									,[AccumulatedDepreciation]
									,[UnfoldingValue]
									,[Decree]
									,[ManagerUnitTransferId]
									,[MonthlyDepreciationId])
								VALUES(@AssetStartId
										,@ManagerUnitId 
										,@InitialId
										,@CodigoMaterial
										,@DataAquisicao
										,@DataAtual
										,@DataIncorporacao
										,@VidaUtilInicio
										,@Contador
										,@ValorAquisicao
										,@ValorAtual
										,@ValorResidual
										,@DepreciacaoMensal
										,@DepreciacaoAcumulada
										,@Desdobro
										,CASE WHEN @DataDecreto IS NULL THEN 0 ELSE 1 END
										,NULL
										,@MonthlyDepreciationId)
							
						END;
						
				

						
							
					END;
				ELSE
					BEGIN
					
						---Transferencia
						--IF(@ManagerUnitTransferId IS NOT NULL)
						--BEGIN
						--	IF(YEAR(@DataTransferencia) = YEAR(@DataAtual) AND MONTH(@DataTransferencia) = MONTH(@DataAtual))
						--	BEGIN
						--		SET @ManagerUnitId = @ManagerUnitTransferId;
						--	END;
						--END;
						--Bem com decreto
						IF(@DataDecreto IS NOT NULL AND YEAR(@DataDecreto) = YEAR(@DataAtual) AND MONTH(@DataDecreto) = MONTH(@DataAtual) )
						BEGIN 
							UPDATE [dbo].[MonthlyDepreciation] SET [CurrentValue] = @ValorAquisicao, [AccumulatedDepreciation] = 0.0
								WHERE [AssetStartId] = @AssetStartId
								AND [MaterialItemCode] = @CodigoMaterial
								AND [ManagerUnitId] = @ManagerUnitId
							    
							SET @ValorAtual =  (SELECT TOP 1 [DepreciationAmountStart] 
														FROM [dbo].[Asset] [ast] 
														WHERE [AssetStartId] = @AssetStartId
														AND [ast].[MovimentDate] = @DataDecreto);
			

							SET @ValorResidual =ROUND((@ValorAquisicao * ROUND(@PorcetagemResidual,2,1)) / 100,2,1);
							SET @ContadorDecreto = @VidaUtilInicio - (@Contador -1);
							IF(@ContadorDecreto =0)
							BEGIN
								SET @ContadorDecreto = 1;
							END;
							SET @DepreciacaoMensal = ROUND((@ValorAquisicao - @ValorResidual) / @ContadorDecreto,2,1);
							SET @DataAtual = @DataDecreto;
							SET @Decreto = 1;
							SET @DepreciacaoAcumulada = 0.0;
							

						END;
				
						SET @DepreciacaoAcumulada = @DepreciacaoMensal + @DepreciacaoAcumulada;
						SET @ValorAtual = (@ValorAtual -@DepreciacaoMensal) - @Desdobro;
				
						IF(@Contador = @VidaUtil)
						BEGIN
							SET @CurrentMonth =(SELECT COUNT(*) -1
											FROM [dbo].[MonthlyDepreciation] 
											WHERE [AssetStartId] = @AssetStartId

											);
							IF(@CurrentMonth <> @VidaUtilInicio)
							BEGIN
								SET @CurrentMonth =(SELECT COUNT(*)
											FROM [dbo].[MonthlyDepreciation] 
											WHERE [AssetStartId] = @AssetStartId
											);
							END;
							IF(@CurrentMonth = @VidaUtilInicio)
							BEGIN
								SET @Desdobro = @ValorAtual - @ValorResidual;
								SET @ValorAtual = @ValorAtual - @Desdobro;
								SET @DepreciacaoAcumulada = @DepreciacaoAcumulada + @Desdobro;
								SET @DepreciacaoMensal = @DepreciacaoMensal + @Desdobro;
							END;
							
						END;
				
						IF(ISNULL(@QuantidadeJaDepreciada,0) < 1)
						BEGIN
							INSERT INTO [dbo].[MonthlyDepreciation]
									([AssetStartId]
									,[ManagerUnitId]
									,[InitialId]
									,[MaterialItemCode]
									,[AcquisitionDate]
									,[CurrentDate]
									,[DateIncorporation]
									,[LifeCycle]
									,[CurrentMonth]
									,[ValueAcquisition]
									,[CurrentValue]
									,[ResidualValue]
									,[RateDepreciationMonthly]
									,[AccumulatedDepreciation]
									,[UnfoldingValue]
									,[Decree]
									,[ManagerUnitTransferId]
									,[MonthlyDepreciationId])
								VALUES(@AssetStartId
										,@ManagerUnitId 
										,@InitialId
										,@CodigoMaterial
										,@DataAquisicao
										,@DataAtual
										,@DataIncorporacao
										,@VidaUtilInicio
										,@CurrentMonthSequencia
										,@ValorAquisicao
										,@ValorAtual
										,@ValorResidual
										,@DepreciacaoMensal
										,@DepreciacaoAcumulada
										,@Desdobro
										,CASE WHEN @DataDecreto IS NULL THEN 0 ELSE 1 END
										,CASE WHEN @CONTADOR = @VidaUtil THEN @ManagerUnitTransferId ELSE NULL END
										,@MonthlyDepreciationId);  
						END;
						

					END;
		
				IF(YEAR(@DataAtual) >= YEAR(@DataFinal) AND MONTH(@DataAtual) >= MONTH(@DataFinal) )
				BEGIN
					SET @Contador = @VidaUtil + 1;

					BREAK;
				END;
							
				SET @DataAtual =(SELECT DATEADD(MONTH,1,@DataAtual));
				SET @Contador = @Contador + 1;
				SET @CurrentMonthSequencia = @CurrentMonthSequencia + 1;

				--Bem com decreto
				IF(@DataDecreto IS NOT NULL)
				BEGIN
					PRINT('Decreto');
				END;

			IF(@DataDecreto IS NOT NULL AND @Decreto = 0)
			BEGIN
				UPDATE [dbo].[MonthlyDepreciation] SET [CurrentValue] = @ValorAquisicao,[AccumulatedDepreciation] = 0.0, [Decree] = 0.0
					WHERE [AssetStartId] = @AssetStartId
					AND [MaterialItemCode] = @CodigoMaterial
					AND [ManagerUnitId] = @ManagerUnitId
			

				SET @ValorAtual = (SELECT TOP 1 [DepreciationAmountStart] 
											FROM [dbo].[Asset] [ast] 
											WHERE [AssetStartId] = @AssetStartId
											AND [ast].[MovimentDate] = @DataDecreto);
			

				SET @ValorResidual =ROUND((@ValorAquisicao * ROUND(@PorcetagemResidual,2,1)) / 100,2,1);
				SET @ContadorDecreto = @VidaUtilInicio - (@Contador -1);
				IF(@ContadorDecreto =0)
				BEGIN
					SET @ContadorDecreto = 1;
				END;
				SET @DepreciacaoMensal = ROUND((@ValorAquisicao - @ValorResidual) / @ContadorDecreto,2,1);
				SET @Desdobro = 0.0;
				SET @DepreciacaoAcumulada = 0.0;

				INSERT INTO [dbo].[MonthlyDepreciation]
						([AssetStartId]
						,[ManagerUnitId]
						,[InitialId]
						,[MaterialItemCode]
						,[AcquisitionDate]
						,[CurrentDate]
						,[DateIncorporation]
						,[LifeCycle]
						,[CurrentMonth]
						,[ValueAcquisition]
						,[CurrentValue]
						,[ResidualValue]
						,[RateDepreciationMonthly]
						,[AccumulatedDepreciation]
						,[UnfoldingValue]
						,[Decree]
						,[ManagerUnitTransferId]
						,[MonthlyDepreciationId])
					VALUES(@AssetStartId
							,@ManagerUnitId 
							,@InitialId
							,@CodigoMaterial
							,@DataAquisicao
							,@DataAtual
							,@DataIncorporacao
							,@VidaUtilInicio
							,@CurrentMonthSequencia
							,@ValorAquisicao
							,@ValorAtual
							,@ValorResidual
							,@DepreciacaoMensal
							,@DepreciacaoAcumulada
							,@Desdobro
							,CASE WHEN @DataDecreto IS NULL THEN 0 ELSE 1 END
							,CASE WHEN @CONTADOR = @VidaUtil THEN @ManagerUnitTransferId ELSE NULL END
							,@MonthlyDepreciationId);  
			END;

			END TRY
			BEGIN CATCH
				SET @DataAtual =(SELECT DATEADD(MONTH,1,@DataAtual));
				SET @Contador = @Contador + 1;
				SET @CurrentMonthSequencia = @CurrentMonthSequencia + 1;
			END CATCH
		END;
	END TRY
	BEGIN CATCH
	SELECT  
		ERROR_NUMBER() AS ErrorNumber  
		,ERROR_SEVERITY() AS ErrorSeverity  
		,ERROR_STATE() AS ErrorState  
		,ERROR_PROCEDURE() AS ErrorProcedure  
		,ERROR_LINE() AS ErrorLine  
		,ERROR_MESSAGE() AS ErrorMessage
		,@CodigoMaterial AS CodigoMaterial;

	END CATCH;
	END;

	IF(@Contador >= @VidaUtil AND @DataDecreto IS NULL)

	BEGIN


	INSERT INTO [dbo].[MonthlyDepreciation]
				([AssetStartId]
				,[ManagerUnitId]
				,[InitialId]
				,[MaterialItemCode]
				,[AcquisitionDate]
				,[CurrentDate]
				,[DateIncorporation]
				,[LifeCycle]
				,[CurrentMonth]
				,[ValueAcquisition]
				,[CurrentValue]
				,[ResidualValue]
				,[RateDepreciationMonthly]
				,[AccumulatedDepreciation]
				,[UnfoldingValue]
				,[Decree]
				,[ManagerUnitTransferId]
				,[MonthlyDepreciationId])
			VALUES(@AssetStartId
					,@ManagerUnitId 
					,@InitialId
					,@CodigoMaterial
					,@DataAquisicao
					,@DataAtual
					,@DataIncorporacao
					,@VidaUtilInicio
					,@CurrentMonthSequencia
					,@ValorAquisicao
					,@ValorAtual
					,@ValorResidual
					,0.0
					,@DepreciacaoAcumulada
					,0.0
					,CASE WHEN @DataDecreto IS NULL THEN 0 ELSE 1 END
					,CASE WHEN @CONTADOR = @VidaUtil THEN @ManagerUnitTransferId ELSE NULL END
					,@MonthlyDepreciationId);  


	END;

	UPDATE [dbo].[Asset] SET ValueUpdate = @ValorAtual
						WHERE [AssetStartId] = @AssetStartId 
							AND [ManagerUnitId] = @ManagerUnitId 
							AND [MaterialItemCode] = @CodigoMaterial 
	RETURN @Contador;

GO