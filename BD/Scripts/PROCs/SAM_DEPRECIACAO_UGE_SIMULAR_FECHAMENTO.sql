/****** Object:  StoredProcedure [dbo].[SAM_DEPRECIACAO_UGE_SIMULAR]    Script Date: 08/04/2020 16:43:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  StoredProcedure [dbo].[SAM_EXECUTAR_DEPRECIACAO_POR_UGE]    Script Date: 07/04/2020 15:31:15 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SAM_DEPRECIACAO_UGE_SIMULAR]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[SAM_DEPRECIACAO_UGE_SIMULAR]
GO
CREATE PROCEDURE [dbo].[SAM_DEPRECIACAO_UGE_SIMULAR]
	@ManagerUnitId INT,
	@MaterialItemCode INT,
	@AssetStartId INT,
	@DataFinal AS DATETIME,
	@Fechamento AS BIT= 0,
	@Retorno AS NVARCHAR(1000) OUTPUT  
AS

SET DATEFORMAT YMD;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  

DECLARE @COUNT INT =0;
DECLARE @CONTADOR INT = 0;
DECLARE @TABLEID INT = 0;

DECLARE @COUNTBEM INT =0;
DECLARE @CONTADORBEM INT = 0;
DECLARE @TABLEBEMID INT  = 0;

DECLARE @VIDA_UTIL SMALLINT = 0;
DECLARE @CONTADORVIDAUTIL INT = 0;
DECLARE @CONTADORDECREE INT = 0;
DECLARE @DATATRANSFERENCIA AS DATETIME;
DECLARE @BEMJADEPRECIADO AS SMALLINT = 0

DECLARE @ULTIMADEPRECIACAO AS BIT = 0;
DECLARE @CURRENTDATE_MANAGERUNIT DATETIME;
----DEPRECIAÇÃO
DECLARE @AcquisitionDate DATETIME;
DECLARE @MovimentDate DATETIME
DECLARE @ValueAcquisition DECIMAL(18,2);
DECLARE @RateDepreciationMonthlyGroup AS DECIMAL(18,2);
DECLARE @RateDepreciationMonthly AS DECIMAL(18,2);
DECLARE @AccumulatedDepreciation AS DECIMAL(18,2) = 0;
DECLARE @ResidualValue AS DECIMAL(18,2)
DECLARE @ResidualPorcetagemValue AS DECIMAL(18,2)
DECLARE @CurrentValue DECIMAL(18,2);
DECLARE @Decree BIT =0;
DECLARE @UnfoldingValue DECIMAL(18,2)
DECLARE @CurrentDate AS DATETIME;
DECLARE @CurrentDateInitial AS DATETIME;
DECLARE @LifeCicle INT = 0;
DECLARE @DateDecree DATETIME;
DECLARE @DecreeValue AS DECIMAL(18,2) = 0;
DECLARE @CurrentMonth INT = 0;
DECLARE @QuantidadeDepreciado INT=0;
DECLARE @BemNaoDepreciadoNaOrigem INT = 0;
DECLARE @AssetId INT = NULL;
DECLARE @BemTransferidoJaDepreciadoQuantidade INT = NULL;
DECLARE @MesesFaltanteDepreciarDecreto SMALLINT = 0;
DECLARE @DateReverse DATETIME;

---INFORMAÇÕES
DECLARE @InitialId INT = 0
DECLARE @DepreciationAmountStart DECIMAL(18,2) = 0;
DECLARE @ManagerUnitIdInitial AS INT = @ManagerUnitId;

DECLARE @TABLE_ERRO AS TABLE(
 ErrorNumber INT NOT NULL, 
 ErrorMessage VARCHAR(2000) NOT NULL,
 MaterialItemCode INT NOT NULL,
 AssetStartId INT NOT NULL,
 AssetId INT NOT NULL
)

DECLARE @TABLEBEM AS TABLE(
	[Id] INT NOT NULL IDENTITY(1,1),
	[InitialId] INT NOT NULL,
	[AssetStartId] INT NOT NULL,
	[DataAquisicao] DATETIME NOT NULL,
	[DataIncorporacao] DATETIME NOT NULL,
	[ValorAquisicao] DECIMAL(18,2) NOT NULL
)

DECLARE @MonthlyDepreciation AS TABLE (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssetStartId] [int] NULL,
	[NumberIdentification] [varchar](20) NULL,
	[ManagerUnitId] [int] NOT NULL,
	[MaterialItemCode] [int] NOT NULL,
	[InitialId] [int] NOT NULL,
	[AcquisitionDate] [datetime] NOT NULL,
	[CurrentDate] [datetime] NOT NULL,
	[DateIncorporation] [datetime] NOT NULL,
	[LifeCycle] [smallint] NOT NULL,
	[CurrentMonth] [tinyint] NOT NULL,
	[ValueAcquisition] [decimal](18, 2) NOT NULL,
	[CurrentValue] [decimal](18, 2) NOT NULL,
	[ResidualValue] [decimal](18, 2) NOT NULL,
	[RateDepreciationMonthly] [decimal](18, 2) NOT NULL,
	[AccumulatedDepreciation] [decimal](18, 2) NOT NULL,
	[UnfoldingValue] [decimal](18, 2) NULL,
	[Decree] [bit] NOT NULL,
	[ManagerUnitTransferId] [int] NULL,
	[MonthlyDepreciationId] [int] NULL)

SET @COUNT = 1;
SET @DATAFINAL =(SELECT DATEADD(MONTH,1,@DATAFINAL));
SET @Retorno = 'Depreciação realizada com sucesso!';

BEGIN TRY
WHILE(@CONTADOR < @COUNT )
BEGIN
	BEGIN TRY
		INSERT INTO @TABLEBEM([AssetStartId],[InitialId],[DataAquisicao],[DataIncorporacao],[ValorAquisicao])
		SELECT TOP 1 @AssetStartId,[bem].[InitialId], [bem].[AcquisitionDate],[bem].[MovimentDate],[bem].[ValueAcquisition] 
		   FROM [dbo].[Asset] [bem] 
		   WHERE [bem].[AssetStartId] =@AssetStartId 
		     AND [bem].[InitialId] IS NOT NULL
			 AND [bem].[flagVerificado] IS NULL
			 AND [bem].[flagDepreciaAcumulada] = 1 
			 AND [bem].[AssetStartId] IS NOT NULL
			 AND ([bem].[flagAcervo]  IS NULL OR [bem].[flagAcervo] = 0)
			 AND ([bem].[flagTerceiro] = 0 OR  [bem].[flagTerceiro] IS NULL)
		   ORDER BY [bem].[Id] ASC

		SET @COUNTBEM = (SELECT COUNT(*) FROM @TABLEBEM);
		PRINT(@COUNTBEM);
		WHILE(@CONTADORBEM < @COUNTBEM)
		BEGIN
			BEGIN TRY
			SET @LifeCicle = (SELECT TOP 1 [gr].[LifeCycle] 
								FROM [dbo].[MaterialGroup] [gr]
								INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
								WHERE [ast].[MaterialItemCode] = @MaterialItemCode);
			SET @TABLEBEMID =(SELECT TOP 1 [Id] FROM @TABLEBEM);

			SET @AssetId = (SELECT TOP 1 [AssetId] FROM [AssetMovements] WHERE [ManagerUnitId] = @ManagerUnitId AND [MonthlyDepreciationId] = @AssetStartId ORDER BY [Id] DESC);
			IF(@AssetId IS NOT NULL)
			BEGIN
				SET @BemNaoDepreciadoNaOrigem = (SELECT [AssetId] FROM [dbo].[AssetMovements] WHERE AssetTransferenciaId  = @AssetId)
				IF(@BemNaoDepreciadoNaOrigem IS NOT NULL)
				BEGIN
					SET @BemTransferidoJaDepreciadoQuantidade = (SELECT COUNT(*) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @BemNaoDepreciadoNaOrigem AND [MaterialItemCode] = @MaterialItemCode)
					IF(@BemTransferidoJaDepreciadoQuantidade IS NOT NULL)
					BEGIN
						IF(@BemTransferidoJaDepreciadoQuantidade >= @LifeCicle)
						BEGIN
							INSERT INTO @TABLE_ERRO(ErrorNumber,ErrorMessage,MaterialItemCode,AssetStartId,AssetId)
							SELECT 0,'Bem Material já está totalmente depreciado, o mesmo foi realizado na origem da transferência.',@MaterialItemCode,@AssetStartId, (SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = AssetStartId AND  [MaterialItemCode] = @MaterialItemCode)

							GOTO GOTO_ERRO
						END
						ELSE
							IF(ISNULL(@BemTransferidoJaDepreciadoQuantidade,0) = 0)
							BEGIN
								SET @AcquisitionDate = (SELECT  [DataAquisicao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
								SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] IS NOT NULL
													ORDER BY [Id] DESC
													  );
								PRINT('Aquisição');
								PRINT(@AcquisitionDate);
								PRINT('Transferência');
								PRINT(@DATATRANSFERENCIA);

								IF(YEAR(@AcquisitionDate) <= YEAR(@DATATRANSFERENCIA) AND (MONTH(@AcquisitionDate) < MONTH(@DATATRANSFERENCIA)))
								BEGIN
									INSERT INTO @TABLE_ERRO(ErrorNumber,ErrorMessage,MaterialItemCode,AssetStartId,AssetId)
									SELECT 0,'Bem Material ainda não foi depreciado na origem, por favor accese a página de depreciação e realize a depreciação item material',@MaterialItemCode,@AssetStartId,(SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = AssetStartId AND  [MaterialItemCode] = @MaterialItemCode)
									GOTO GOTO_ERRO;
								END;
								--ELSE
								--	BEGIN
								--		---Atribui o Id da ManagerUnit de destino para que seja depreciado o bem no destino
								--		SET @ManagerUnitId=(SELECT TOP 1 [ManagerUnitId]
								--					FROM [dbo].[AssetMovements]  
								--					WHERE [AssetId] = @AssetStartId
								--					  AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId
								--					  AND [AssetTransferenciaId] IS NOT NULL
								--					ORDER BY [Id] DESC
								--					  );
								--	END;
								SET @AcquisitionDate = NULL;
								SET @DATATRANSFERENCIA = NULL;

								
							END; 
						
					END
				END;
			END;


			

			SET @AcquisitionDate = (SELECT  [DataAquisicao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			SET @MovimentDate = (SELECT  [DataIncorporacao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			SET @ValueAcquisition  = (SELECT  [ValorAquisicao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			SET @InitialId  = (SELECT  [InitialId] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			

			SET @QuantidadeDepreciado = (SELECT COUNT(*) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)
			
			IF(@QuantidadeDepreciado >= @LifeCicle)
			BEGIN
				INSERT INTO @TABLE_ERRO(ErrorNumber,ErrorMessage,MaterialItemCode,AssetStartId,AssetId)
				SELECT 0,'Bem Material já éstá totalmente depreciado.',@MaterialItemCode,@AssetStartId,(SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = AssetStartId AND  [MaterialItemCode] = @MaterialItemCode)
				GOTO GOTO_ERRO
			END

			SET @VIDA_UTIL = @LifeCicle;

			SET @DateDecree =(SELECT TOP 1 [ast].[MovimentDate] 
									FROM [dbo].[Asset] [ast]
									INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
									WHERE [ast].[flagDecreto]  = 1
									AND [ast].[AssetStartId] = @AssetStartId
									ORDER BY [ast].[Id] DESC);

			SET @UnfoldingValue =0;
			SET @AccumulatedDepreciation = 0;

				
			SET @ResidualValue = (SELECT TOP 1 [gr].[ResidualValue] 
											FROM [dbo].[MaterialGroup] [gr]
											INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
										WHERE [ast].[MaterialItemCode] = @MaterialItemCode); 
				
						
			SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] IS NOT NULL
													ORDER BY [Id] DESC
													  );
			SET @DateReverse  = (SELECT TOP 1 [DataEstorno]
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] IS NOT NULL
													  AND ([FlagEstorno] IS NULL OR [FlagEstorno] = 0)
													ORDER BY [Id] DESC
													  );	
			

			SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)
			SET @CurrentDate = (SELECT MAX(CurrentDate) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)
			SET @CURRENTDATE_MANAGERUNIT = (SELECT MAX(CurrentDate) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode AND [ManagerUnitId] = @ManagerUnitId)
			SET @CONTADORVIDAUTIL = 0;

			PRINT('ASSETSTARID');
			PRINT(@AssetStartId);
			PRINT('CURRENT DATE');
			PRINT(YEAR(@CurrentDate));
			PRINT(MONTH(@CurrentDate));
			IF(@CurrentDate IS NOT NULL)
			BEGIN
				PRINT('VIDA UTIL');
				--SET @VIDA_UTIL = @VIDA_UTIL - DATEDIFF(MONTH,@AcquisitionDate,@CurrentDate);
				PRINT(@LifeCicle)
				PRINT(@VIDA_UTIL)
				PRINT(@AcquisitionDate);
				PRINT(@CurrentDate);

				SET @CONTADORVIDAUTIL =(SELECT MAX(CurrentMonth) + 1 FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)  ---(@LifeCicle - (@VIDA_UTIL - DATEDIFF(MONTH,@AcquisitionDate,@CurrentDate)) + 1) +1;
				SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
				PRINT('CONTADOR');
				PRINT(@CONTADORVIDAUTIL);
			END
			
			IF(@CURRENTDATE_MANAGERUNIT IS NOT NULL)
			BEGIN
				SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] 
										WHERE [AssetStartId] = @AssetStartId 
										  AND [MaterialItemCode] = @MaterialItemCode 
										  AND [ManagerUnitId] = @ManagerUnitId)
				PRINT('CURRENTDATE_MANAGERUNIT');
				PRINT(@CURRENTDATE_MANAGERUNIT);

				SET @VIDA_UTIL = @LifeCicle -  DATEDIFF(MONTH,@AcquisitionDate,@CURRENTDATE_MANAGERUNIT);
				SET @CONTADORVIDAUTIL =  (SELECT MAX([CurrentMonth]) + 1 FROM [MonthlyDepreciation] 
										WHERE [AssetStartId] = @AssetStartId 
										  AND [MaterialItemCode] = @MaterialItemCode 
										  AND [ManagerUnitId] = @ManagerUnitId)
			END;

			--Calcula o valor a ser depreciano ao mês
			SET @ResidualPorcetagemValue =ROUND((@ValueAcquisition * ROUND(@ResidualValue,2,1)) / 100,2,1);
			SET @RateDepreciationMonthly = ROUND((@ValueAcquisition - @ResidualPorcetagemValue) / @VIDA_UTIL,2,1);

			IF(@CurrentDate IS NOT NULL)
			BEGIN
				PRINT('DATA ATUAL');
				print(@CurrentDate);

				SET @AccumulatedDepreciation = @RateDepreciationMonthly + (SELECT MAX([AccumulatedDepreciation]) FROM [MonthlyDepreciation] 
										WHERE [AssetStartId] = @AssetStartId 
										  AND [MaterialItemCode] = @MaterialItemCode);

				SET @CurrentValue = (@CurrentValue - @RateDepreciationMonthly) - @UnfoldingValue;
			END;

			---Executa sempre em penultimo
			IF(@CurrentValue IS NULL)
			BEGIN
				SET @CurrentValue = @ValueAcquisition;
			END;
			
			IF(@CurrentDate IS NULL)
			BEGIN
				PRINT('Data Atual')
				---Alterar o dia da data atual para 10, assim padronizando o dia.
				SET @CurrentDate = CAST(YEAR(@AcquisitionDate) AS VARCHAR(4)) + '-' + CAST(MONTH(@AcquisitionDate) AS VARCHAR(2)) + '-' + '10';
				print(@CurrentDate);
			END
			---Executa sempre em ultimo
			IF(@DateDecree IS NOT NULL)
			BEGIN
					SET @MesesFaltanteDepreciarDecreto =  DATEDIFF(MONTH,@AcquisitionDate,@DateDecree);
					SET @Decree  = 1;
			
					IF(@VIDA_UTIL - @MesesFaltanteDepreciarDecreto > 0)
					BEGIN
						SET @DecreeValue =  (SELECT TOP 1 [ValueUpdate] 
									FROM [dbo].[Asset] [ast] 
									INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
									WHERE [AssetStartId] = @AssetStartId
										AND [ast].[MovimentDate] = @DateDecree
									ORDER BY [mov].[Id] DESC
									);


						SET @ValueAcquisition = @DecreeValue;
						SET @CurrentValue = @ValueAcquisition;

						SET @CurrentDate = @DateDecree;
						SET @CONTADORVIDAUTIL = @MesesFaltanteDepreciarDecreto;

						SET @ResidualPorcetagemValue =ROUND((@ValueAcquisition * ROUND(@ResidualValue,2,1)) / 100,2,1);
						SET @RateDepreciationMonthly = ROUND((@ValueAcquisition - @ResidualPorcetagemValue) ,2,1) / (@VIDA_UTIL - (DATEDIFF(MONTH,@AcquisitionDate,@DateDecree)));

						SET @AccumulatedDepreciation = @RateDepreciationMonthly + @AccumulatedDepreciation;
						
					END;
					ELSE
						BEGIN
							
							---- Decreto realizado com o bem totalmente depreciado, então seta @VIDA_UTIL = -1 para não entrar no loop de calacular
							
							SET @CurrentValue =  (SELECT TOP 1 [ValueUpdate] 
									FROM [dbo].[Asset] [ast] 
									INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
									WHERE [AssetStartId] = @AssetStartId
										AND [ast].[MovimentDate] = @DateDecree
									ORDER BY [ast].[Id] DESC
									);	
										
							SET @CurrentDate = @DateDecree;

							SET @ResidualPorcetagemValue =ROUND((@CurrentValue * ROUND(@ResidualValue,2,1)) / 100,2,1);
							SET @RateDepreciationMonthly = ROUND((@CurrentValue - @ResidualPorcetagemValue) ,2,1) / @VIDA_UTIL;

							SET @AccumulatedDepreciation = @RateDepreciationMonthly + @AccumulatedDepreciation;
							SET @CONTADORVIDAUTIL = @VIDA_UTIL;
							SET @VIDA_UTIL = -1;
						END;
				
						   
			END

			SET @DepreciationAmountStart = @CurrentValue;

			PRINT('UTIL');
			PRINT(@CONTADORVIDAUTIL)
			PRINT(@VIDA_UTIL);

			IF(@Fechamento =1)
			BEGIN
				SET @VIDA_UTIL = @CONTADORVIDAUTIL;
				SET @RateDepreciationMonthly =  (SELECT TOP 1 RateDepreciationMonthly FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
				SET @AccumulatedDepreciation =  (SELECT MAX(AccumulatedDepreciation) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) + @RateDepreciationMonthly;
				SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) - @RateDepreciationMonthly;
				IF(@CurrentValue IS NULL)
				BEGIN
					SET @VIDA_UTIL = @LifeCicle;
					SET @CurrentValue = @ValueAcquisition;
					--Calcula o valor a ser depreciano ao mês
					SET @ResidualPorcetagemValue =ROUND((@ValueAcquisition * ROUND(@ResidualValue,2,1)) / 100,2,1);
					SET @RateDepreciationMonthly = ROUND((@ValueAcquisition - @ResidualPorcetagemValue) / @VIDA_UTIL,2,1);
					SET @AccumulatedDepreciation = @RateDepreciationMonthly;
					
					SET @CONTADORVIDAUTIL = 0;
					PRINT('PASSOU 1');
				END

				SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] IS NOT NULL
													ORDER BY [Id] DESC
													  );
			END;

			WHILE(@CONTADORVIDAUTIL <= @VIDA_UTIL)
			BEGIN
				PRINT('INICIO');
				IF(YEAR(@DataFinal) = YEAR(@CurrentDate) AND MONTH(@DataFinal) = MONTH(@CurrentDate))
				BEGIN
					
					BREAK;
				END
				---Verifica se na data atual foi realizado uma transferência
				PRINT('Transferencia');
				PRINT(@DATATRANSFERENCIA);

				IF(@DATATRANSFERENCIA IS NOT NULL)
				BEGIN
					PRINT('Transferencia');
					PRINT(@DATATRANSFERENCIA)
					PRINT(@CurrentDate)

					IF(YEAR(@DATATRANSFERENCIA)  = YEAR(@CurrentDate) AND MONTH(@DATATRANSFERENCIA) = MONTH(@CurrentDate))
					BEGIN
						BREAK;
					END
					
				END;
				---Verifica se na data atual foi realizado um estorno
				IF(@DateReverse IS NOT NULL)
				BEGIN
					IF(YEAR(@DateReverse)  = YEAR(@CurrentDate) AND MONTH(@DateReverse) = MONTH(@CurrentDate))
					BEGIN
						BREAK;
					END
					
				END;


				SET @BEMJADEPRECIADO = (SELECT TOP 1 [Id] FROM @MonthlyDepreciation 
													WHERE [AssetStartId] = @AssetStartId
														AND [CurrentDate]= @CurrentDate
														AND [ManagerUnitId] = @ManagerUnitId
														AND [MaterialItemCode] = @MaterialItemCode)

				IF(@CONTADORVIDAUTIL = 0)
				BEGIN
					---Verifica se o bem para a data atual já foi depreciado, caso sim não é realizado o insert, assim evitando error de inclusão

					IF(ISNULL(@BEMJADEPRECIADO,0) < 1)
					BEGIN
						
							---Alterar o dia do primeiro registro "linha 0".
							SET @CurrentDateInitial =(SELECT DATEADD(DAY,-1,@CurrentDate));


							
							UPDATE [dbo].[Asset] SET [DepreciationDateStart] = @CurrentDate, [DepreciationAmountStart] = @DepreciationAmountStart
												WHERE [AssetStartId] = @AssetStartId AND [DepreciationDateStart] IS NULL;	
							
							PRINT('DEPRECIAÇÃO');
							PRINT(@CURRENTVALUE);
							INSERT INTO @MonthlyDepreciation
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
											,@MaterialItemCode
											,@AcquisitionDate
											,@CurrentDateInitial
											,@MovimentDate
											,@LifeCicle
											,@CONTADORVIDAUTIL
											,@ValueAcquisition
											,@CurrentValue
											,@ResidualPorcetagemValue
											,@RateDepreciationMonthly
											,@AccumulatedDepreciation
											,@UnfoldingValue
											,@Decree
											,NULL
											,@AssetStartId)
					END;
			

				END;
				ELSE
					BEGIN
					
						IF(@CONTADORVIDAUTIL = @VIDA_UTIL)
						BEGIN
							SET @CurrentMonth =(SELECT COUNT(*)
											FROM @MonthlyDepreciation 
											WHERE [AssetStartId] = @AssetStartId

											);
						
							IF(@CurrentMonth >= @LifeCicle)
							BEGIN
								print('Final 1');
								SET @UnfoldingValue = @CurrentValue - @ResidualPorcetagemValue;
								SET @CurrentValue = @CurrentValue - @UnfoldingValue;
								SET @AccumulatedDepreciation = @AccumulatedDepreciation + @UnfoldingValue;
								SET @RateDepreciationMonthly = @RateDepreciationMonthly + @UnfoldingValue;
								SET @ULTIMADEPRECIACAO = 1;
							END;

							IF(@DateDecree IS NOT NULL AND (@MesesFaltanteDepreciarDecreto + @CurrentMonth) >= @LifeCicle)
							BEGIN
								print('Final 1');
								SET @UnfoldingValue = @CurrentValue - @ResidualPorcetagemValue;
								SET @CurrentValue = @CurrentValue - @UnfoldingValue;
								SET @AccumulatedDepreciation = @AccumulatedDepreciation + @UnfoldingValue;
								SET @RateDepreciationMonthly = @RateDepreciationMonthly + @UnfoldingValue;
								SET @ULTIMADEPRECIACAO = 1;
							END;
							
						END;

						IF(ISNULL(@BEMJADEPRECIADO,0) < 1)
						BEGIN
							INSERT INTO @MonthlyDepreciation
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
											,@MaterialItemCode
											,@AcquisitionDate
											,@CurrentDate
											,@MovimentDate
											,@LifeCicle
											,@CONTADORVIDAUTIL
											,@ValueAcquisition
											,@CurrentValue
											,@ResidualPorcetagemValue
											,@RateDepreciationMonthly
											,@AccumulatedDepreciation
											,@UnfoldingValue
											,@Decree
											,NULL
											,@AssetStartId)
						END;
				

					END;
			
		
			
				IF(@CONTADORVIDAUTIL =0)
				BEGIN
					SET @CurrentDate =(SELECT DATEADD(DAY,1,@CurrentDate));
				END;
				ELSE
					BEGIN
						SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
					
					END;

				IF(@ULTIMADEPRECIACAO =0)
				BEGIN
					SET @AccumulatedDepreciation = @RateDepreciationMonthly + @AccumulatedDepreciation;
					SET @CurrentValue = (@CurrentValue - @RateDepreciationMonthly) - @UnfoldingValue;
				END;
				

				SET @CONTADORVIDAUTIL = @CONTADORVIDAUTIL + 1;
			END;

			DELETE FROM @TABLEBEM WHERE [Id] = @TABLEBEMID;
			SET @CONTADORBEM = @CONTADORBEM + 1;
		END TRY
		BEGIN CATCH
			INSERT INTO @TABLE_ERRO(ErrorNumber,ErrorMessage,MaterialItemCode,AssetStartId,AssetId)
			SELECT ERROR_NUMBER(),ERROR_MESSAGE(),@MaterialItemCode,@AssetStartId,(SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = @AssetStartId AND  [MaterialItemCode] = @MaterialItemCode)

			SET @CONTADORBEM = @CONTADORBEM + 1;
		END CATCH
		END;

		SET @CONTADOR = @CONTADOR + 1;
	END TRY
	
	BEGIN CATCH
		INSERT INTO @TABLE_ERRO(ErrorNumber,ErrorMessage,MaterialItemCode,AssetStartId,AssetId)
		SELECT ERROR_NUMBER(),ERROR_MESSAGE(),@MaterialItemCode,@AssetStartId,(SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = @AssetStartId AND  [MaterialItemCode] = @MaterialItemCode)

		SET @CONTADOR = @CONTADOR + 1;
	END CATCH
	SET @ManagerUnitId = @ManagerUnitIdInitial;
END;

IF(@Decree = 0 AND @VIDA_UTIL > 0 AND @CONTADORVIDAUTIL >= @LifeCicle)
---Ultimo registro bem sem decreto
BEGIN

	INSERT INTO @MonthlyDepreciation
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
						,@MaterialItemCode
						,@AcquisitionDate
						,@CurrentDate
						,@MovimentDate
						,@LifeCicle
						,@LifeCicle + 1
						,@ValueAcquisition
						,@CurrentValue
						,@ResidualPorcetagemValue
						,0.0
						,@AccumulatedDepreciation
						,0.0
						,@Decree
						,NULL
						,@AssetStartId)

END;
ELSE
	---Ultimo registro bem com decreto
	IF(@Decree = 1 AND @CONTADORVIDAUTIL >= @LifeCicle)
	BEGIN
	PRINT('Ultima com decreto');
	INSERT INTO @MonthlyDepreciation
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
				,@MaterialItemCode
				,@AcquisitionDate
				,@CurrentDate
				,@MovimentDate
				,@LifeCicle
				,@LifeCicle + 1
				,@ValueAcquisition
				,@CurrentValue
				,@ResidualPorcetagemValue
				,@RateDepreciationMonthly
				,@AccumulatedDepreciation
				,0.0
				,1
				,NULL
				,@AssetStartId)

	END;

END TRY
BEGIN CATCH
	INSERT INTO @TABLE_ERRO(ErrorNumber,ErrorMessage,MaterialItemCode,AssetStartId,AssetId )
	SELECT ERROR_NUMBER(),ERROR_MESSAGE(),@MaterialItemCode,@AssetStartId,(SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = @AssetStartId AND  [MaterialItemCode] = @MaterialItemCode)

	GOTO GOTO_ERRO;
END CATCH


GOTO_ERRO:
	SELECT * FROM @TABLE_ERRO;

SELECT * FROM @MonthlyDepreciation

GO
GRANT EXECUTE ON [SAM_DEPRECIACAO_UGE_SIMULAR] TO USUSAM
GO

