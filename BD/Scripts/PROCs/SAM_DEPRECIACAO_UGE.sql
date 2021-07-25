SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SAM_DEPRECIACAO_UGE]') AND type in (N'P', N'PC'))
	DROP PROCEDURE [dbo].[SAM_DEPRECIACAO_UGE]
GO
CREATE PROCEDURE [dbo].[SAM_DEPRECIACAO_UGE]
	@ManagerUnitId INT,
	@MaterialItemCode INT,
	@AssetStartId INT,
	@DataFinal AS DATETIME,
	@Fechamento AS BIT= 0,
	@Retorno AS NVARCHAR(1000) OUTPUT,
	@Erro AS BIT OUTPUT
AS
---PRINT(@DataFinal)
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
DECLARE @DATAORIGEM AS DATETIME;
DECLARE @BEMJADEPRECIADO AS BIGINT = 0
DECLARE @VIDA_UTIL_DECREE INT = NULL;
DECLARE @ULTIMADEPRECIACAO AS BIT = 0;
DECLARE @CURRENTDATE_MANAGERUNIT DATETIME;
DECLARE @BEMDEPRECIADONAOTOTAL AS BIT = 0;
DECLARE @COUNT_PRIMEIRO_FECHAMENTO INT = 0;
DECLARE  @FINAL BIT = 0;
DECLARE @TRANSFERENCIA_BP DATETIME = NULL;

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
DECLARE @AssetIdNaOrigem INT = 0;
DECLARE @ManagerUnitDestino INT = NULL;
DECLARE @ManagerUnitOrigem INT  = NULL;
DECLARE @AssetId INT = NULL;
DECLARE @BemTransferidoJaDepreciadoQuantidade INT = NULL;
DECLARE @MesesFaltanteDepreciarDecreto SMALLINT = 0;
DECLARE @DateReverse DATETIME;
DECLARE @ValueAcquisitionCalcDecree DECIMAL(18,2)= NULL;
--Alteração Aqui
Declare @MaximoMesAtual int = 0;
--para BPs incoporados em um mês, e no mesmo mês terem sidos transferidos (sem ter feito qualquer fechamento)
Declare @AssetStartIdNaOrigem int;

---INFORMAÇÕES
DECLARE @InitialId INT = 0
DECLARE @DepreciationAmountStart DECIMAL(18,2) = 0;

DECLARE @TableTransfencia AS TABLE(
	ManagerUnitId INT NOT NULL,
	AssetId INT NOT NULL,
	MovimentDate DATETIME NULL
)

DECLARE @TABLEBEM AS TABLE(
	[Id] INT NOT NULL IDENTITY(1,1),
	[InitialId] INT NOT NULL,
	[AssetStartId] INT NOT NULL,
	[DataAquisicao] DATETIME NOT NULL,
	[DataIncorporacao] DATETIME NOT NULL,
	[ValorAquisicao] DECIMAL(18,2) NOT NULL
)


SET @COUNT = 1;
SET @DATAFINAL =(SELECT DATEADD(MONTH,1,@DATAFINAL));
SET @Retorno ='Depreciação realizada com sucesso!';
SET @Erro = 0;
---PRINT('DATA FINAL 01')

---PRINT(@DataFinal)
BEGIN TRY
WHILE(@CONTADOR < @COUNT )
BEGIN
	BEGIN TRY
		INSERT INTO @TABLEBEM([AssetStartId],[InitialId],[DataAquisicao],[DataIncorporacao],[ValorAquisicao])
		SELECT TOP 1 @AssetStartId,[bem].[InitialId], [bem].[AcquisitionDate],[bem].[MovimentDate],[bem].[ValueAcquisition] 
		   FROM [dbo].[Asset] [bem] 
		   INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [bem].[Id] 
		   WHERE [bem].[AssetStartId] =@AssetStartId 
		     AND [bem].[InitialId] IS NOT NULL
			 AND [bem].[flagVerificado] IS NULL
			 AND [bem].[flagDepreciaAcumulada] = 1 
			 AND [bem].[AssetStartId] IS NOT NULL
			 AND ([bem].[flagAcervo]  IS NULL OR [bem].[flagAcervo] = 0)
			 AND ([bem].[flagTerceiro] = 0 OR  [bem].[flagTerceiro] IS NULL)
			 AND ([bem].[FlagAnimalNaoServico] IS NULL OR [bem].[FlagAnimalNaoServico] =0)
			 AND ([mov].[AssetTransferenciaId] IS NULL OR [mov].[AssetTransferenciaId] = 0)
			 AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] =0)
		   ORDER BY [bem].[Id] ASC
		     
		SET @COUNTBEM = (SELECT COUNT(*) FROM @TABLEBEM);
		WHILE(@CONTADORBEM < @COUNTBEM)
		BEGIN
			BEGIN TRY
				SET @COUNT_PRIMEIRO_FECHAMENTO = (SELECT COUNT(*) FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)


				IF(@COUNT_PRIMEIRO_FECHAMENTO < 1 AND @Fechamento = 1)
				BEGIN
					SET @Fechamento = 0;
				END;

			SET @LifeCicle = (SELECT TOP 1 [gr].[LifeCycle] 
								FROM [dbo].[MaterialGroup] [gr]
								INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
								WHERE [ast].[MaterialItemCode] = @MaterialItemCode);

			SET @AssetId = (SELECT TOP 1 [AssetId] FROM [AssetMovements] WHERE [ManagerUnitId] = @ManagerUnitId AND [MonthlyDepreciationId] = @AssetStartId ORDER BY [Id] DESC);
			IF(@AssetId IS NOT NULL)
			BEGIN
				SET @AssetIdNaOrigem = (SELECT [AssetId] FROM [dbo].[AssetMovements] WHERE AssetTransferenciaId  = @AssetId)
				---PRINT('PASSOU')
				IF(@AssetIdNaOrigem IS NOT NULL)
				BEGIN
					---PRINT('PASSOU 1')
					---PRINT(@LifeCicle)
					SET @QuantidadeDepreciado = (SELECT COUNT(*) FROM [dbo].[MonthlyDepreciation] WHERE AssetStartId = @AssetStartId);
					---PRINT('TESTE 20')
					---PRINT(@AssetStartId)
					---PRINT(@QuantidadeDepreciado)
					
					IF(ISNULL(@QuantidadeDepreciado,0) >= @LifeCicle)
					BEGIN
						---PRINT('PASSOU 2')
						SET @ManagerUnitDestino = (SELECT TOP 1 [ManagerUnitId] FROM [dbo].[MonthlyDepreciation] WHERE AssetStartId = @AssetStartId ORDER BY [Id] DESC);
						IF(@ManagerUnitDestino <> @ManagerUnitId)
						BEGIN
							---PRINT('PASSOU 3')
							SET @BemTransferidoJaDepreciadoQuantidade = (SELECT COUNT(*) [ManagerUnitId] FROM [dbo].[MonthlyDepreciation] WHERE AssetStartId = @AssetStartId AND [ManagerUnitId] = @ManagerUnitDestino);
							IF(ISNULL(@BemTransferidoJaDepreciadoQuantidade,0) < 1)
							BEGIN
								SET @FINAL = 1;
								GOTO TRANSFERENCIA_FINAL
							END;
							ELSE
								BEGIN
									---PRINT('PAROU');
									RETURN 0;
								END;
						END;
						ELSE
							BEGIN
								---PRINT('AQUI');
								RETURN 0;
							END;
					END;
					
				END;
								
			END;

			---PRINT('PASSOU 10');

			SET @TABLEBEMID =(SELECT TOP 1 [Id] FROM @TABLEBEM);

			SET @AcquisitionDate = (SELECT  [DataAquisicao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			SET @MovimentDate = (SELECT  [DataIncorporacao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			SET @ValueAcquisition  = (SELECT  [ValorAquisicao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			SET @InitialId  = (SELECT  [InitialId] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			

			SET @QuantidadeDepreciado = (SELECT COUNT(*) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)
			
			IF(@QuantidadeDepreciado >= @LifeCicle)
			BEGIN
				SET @Retorno ='Bem Material já éstá totalmente depreciado.'
				RETURN 0;
			END
			
			---PRINT('PASSOU 11');

			SET @VIDA_UTIL = @LifeCicle;

			SET @DateDecree =(SELECT TOP 1 [ast].[MovimentDate] 
									FROM [dbo].[Asset] [ast]
									INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
									WHERE [ast].[flagDecreto]  = 1
									AND [ast].[Id] =@AssetId --@AssetStartId
									AND [ast].[ManagerUnitId] = @ManagerUnitId 
									ORDER BY [ast].[Id] DESC);

			SET @UnfoldingValue =0;
			SET @AccumulatedDepreciation = 0;

				
			SET @ResidualValue = (SELECT TOP 1 [gr].[ResidualValue] 
											FROM [dbo].[MaterialGroup] [gr]
											INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
										WHERE [ast].[MaterialItemCode] = @MaterialItemCode); 
				
			SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] 
			                      WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)

			SET @CurrentDate = (SELECT MAX(CurrentDate) FROM [MonthlyDepreciation] 
			                     WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)

			IF(@CurrentDate IS NULL)
			BEGIN
			SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
										FROM [dbo].[AssetMovements]  
										WHERE [AssetId] = @AssetStartId
											AND [AssetTransferenciaId] IS NOT NULL
										ORDER BY [MovimentDate] ASC, [Id] DESC
											);
			END
			ELSE
				BEGIN
					SET @DATATRANSFERENCIA = (SELECT TOP 1 [mov].[MovimentDate] 
														FROM [dbo].[AssetMovements] [mov]  
														INNER JOIN [dbo].[Asset] [bem] ON [bem].[Id] = [mov].[AssetId] 
														WHERE [bem].[AssetStartId] = @AssetStartId 
															AND [mov].[AssetTransferenciaId] IS NOT NULL
															AND YEAR([mov].[MovimentDate]) = YEAR(@CurrentDate)
															AND MONTH([mov].[MovimentDate]) = MONTH(@CurrentDate)
														ORDER BY [mov].[MovimentDate] ASC, [mov].[Id] DESC
															);
					END

			IF(@DATATRANSFERENCIA IS NOT NULL)
			BEGIN
				select top 1 @AssetStartIdNaOrigem = AssetStartId
				from [dbo].[Asset] where [Id] = @AssetStartId;

				IF(@AssetStartIdNaOrigem IS NULL)
				BEGIN
					SET @ManagerUnitId = (SELECT TOP 1 am.[ManagerUnitId]
											FROM [dbo].[AssetMovements] am
											inner join [dbo].[Asset] a
											ON a.[Id] = am.[AssetId]
											WHERE a.[AssetStartId] = @AssetStartId
											  AND YEAR(am.[MovimentDate]) = YEAR(@DATATRANSFERENCIA)
											  AND MONTH(am.[MovimentDate]) = MONTH(@DATATRANSFERENCIA)
											ORDER BY am.[MovimentDate] ASC, am.[Id] DESC
											);
				END
				ELSE
				BEGIN
					SET @ManagerUnitId = (SELECT TOP 1 [ManagerUnitId] 
											FROM [dbo].[AssetMovements]  
											WHERE [AssetId] = @AssetStartId
											  AND YEAR([MovimentDate]) = YEAR(@DATATRANSFERENCIA)
											  AND MONTH([MovimentDate]) = MONTH(@DATATRANSFERENCIA)
											ORDER BY [MovimentDate] ASC, [Id] DESC
												);
				END
			END;

			SET @DATAORIGEM = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] = @AssetId
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
			

			SET @CURRENTDATE_MANAGERUNIT = (SELECT MAX(CurrentDate) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode AND [ManagerUnitId] = @ManagerUnitId)
			SET @CONTADORVIDAUTIL = 0;
			
			IF(@CurrentDate IS NOT NULL)
			BEGIN
				--SET @VIDA_UTIL = @VIDA_UTIL - DATEDIFF(MONTH,@AcquisitionDate,@CurrentDate);
				SET @CONTADORVIDAUTIL =(SELECT MAX(CurrentMonth) + 1 FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode); --(@LifeCicle - (@VIDA_UTIL - DATEDIFF(MONTH,@AcquisitionDate,@CurrentDate)) + 1 );
				SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));

			
							
				
			END
			
			IF(@CURRENTDATE_MANAGERUNIT IS NOT NULL)
			BEGIN

				IF((@LifeCicle -  DATEDIFF(MONTH,@AcquisitionDate,@CURRENTDATE_MANAGERUNIT)) < 1)
				BEGIN
					SET @FINAL = 1
					GOTO TRANSFERENCIA_FINAL;
				END


			END;
			
			---PRINT('PASSOU 12');
			--Calcula o valor a ser depreciano ao mês
			SET @ResidualPorcetagemValue =ROUND((@ValueAcquisition * ROUND(@ResidualValue,2,1)) / 100,2,1);
			SET @RateDepreciationMonthly = ROUND((@ValueAcquisition - @ResidualPorcetagemValue) / @VIDA_UTIL,2,1);

			IF(@CurrentDate IS NOT NULL)
			BEGIN
				SET @ResidualPorcetagemValue =  (SELECT TOP 1 ResidualValue FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
												
				SET @RateDepreciationMonthly =  (SELECT TOP 1 RateDepreciationMonthly FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
				SET @AccumulatedDepreciation =  (SELECT MAX(AccumulatedDepreciation) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) + @RateDepreciationMonthly;
				SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) - @RateDepreciationMonthly;
				SET @BEMDEPRECIADONAOTOTAL = 1
			END;

			---Executa sempre em penultimo
			IF(@CurrentValue IS NULL)
			BEGIN
				SET @CurrentValue = @ValueAcquisition;
			END;

						
			IF(@CurrentDate IS NULL)
			BEGIN
				---Alterar o dia da data atual para 10, assim padronizando o dia.
				SET @CurrentDate = CAST(YEAR(@AcquisitionDate) AS VARCHAR(4)) + '-' + CAST(MONTH(@AcquisitionDate) AS VARCHAR(2)) + '-' + '10';
			END
			
			---Executa sempre em ultimo
			IF(@DateDecree IS NOT NULL)
			BEGIN
					SET @MesesFaltanteDepreciarDecreto =  DATEDIFF(MONTH,@AcquisitionDate,@DateDecree);
					SET @Decree  = 1;
					SET @MesesFaltanteDepreciarDecreto = IIF(@MesesFaltanteDepreciarDecreto < 0, 0,@MesesFaltanteDepreciarDecreto);
		
					IF(@VIDA_UTIL - @MesesFaltanteDepreciarDecreto > 0)
					BEGIN
						SET @DecreeValue =  (SELECT TOP 1 [ValueAcquisition] 
									FROM [dbo].[Asset] [ast] 
									INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
									WHERE [AssetStartId] = @AssetStartId
										AND [ast].[MovimentDate] = @DateDecree
									ORDER BY [mov].[Id] DESC
									);


					
						SET @ValueAcquisitionCalcDecree = @DecreeValue -- @ResidualValue;
						SET @CurrentValue = @ValueAcquisitionCalcDecree;

						SET @CurrentDate = @DateDecree;
						SET @CONTADORVIDAUTIL =  @MesesFaltanteDepreciarDecreto + 1;;

						SET @ResidualPorcetagemValue =ROUND((@ValueAcquisitionCalcDecree * ROUND(@ResidualValue,2,1)) / 100,2,1);
						SET @RateDepreciationMonthly = ROUND((@ValueAcquisitionCalcDecree - @ResidualPorcetagemValue)  / (@VIDA_UTIL- @MesesFaltanteDepreciarDecreto),2,1);

						SET @AccumulatedDepreciation = @RateDepreciationMonthly + @AccumulatedDepreciation;
						SET @VIDA_UTIL_DECREE = @VIDA_UTIL;

						SET @TRANSFERENCIA_BP = (SELECT [MovimentDate] FROM [AssetMovements] WHERE [MonthlyDepreciationId] = @AssetStartId AND [AssetTransferenciaId] IS NOT NULL)

						IF(@TRANSFERENCIA_BP IS NOT NULL)
						BEGIN
							SET @CurrentDate = @TRANSFERENCIA_BP;
						END;
						
					END;
					ELSE
						BEGIN
							
							---- Decreto realizado com o bem totalmente depreciado, então seta @VIDA_UTIL = -1 para não entrar no loop de calacular
							
							SET @CurrentValue =  (SELECT TOP 1 [ValueAcquisition] 
									FROM [dbo].[Asset] [ast] 
									INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
									WHERE [AssetStartId] = @AssetStartId
										AND [ast].[MovimentDate] = @DateDecree
									ORDER BY [ast].[Id] DESC
									);	
										
							SET @CurrentDate = @DateDecree;

							SET @ResidualPorcetagemValue =ROUND((@CurrentValue * ROUND(@ResidualValue,2,1)) / 100,2,1);
							SET @RateDepreciationMonthly = ROUND((@CurrentValue - @ResidualPorcetagemValue) ,2,1) / @VIDA_UTIL;

							SET @AccumulatedDepreciation =(SELECT TOP 1 [AccumulatedDepreciation] 
											 FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] DESC)
							SET @CONTADORVIDAUTIL = @VIDA_UTIL;
							SET @FINAL = 1;
							
							---PRINT('PASSOU 20');
							INSERT INTO MonthlyDepreciation
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
									,0.0
									,0.0
									,1
									,NULL
									,@AssetStartId)

									SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
										FROM [dbo].[AssetMovements]  
										WHERE [AssetId] = @AssetStartId
											AND [ManagerUnitId] = @ManagerUnitId

											AND [AssetTransferenciaId] IS NOT NULL
										ORDER BY [Id] DESC
											);

									IF(@DATATRANSFERENCIA IS NOT NULL)
									BEGIN
										SET @ManagerUnitDestino = (SELECT TOP 1 SourceDestiny_ManagerUnitId 
																	FROM [dbo].[AssetMovements]  
																	WHERE [AssetId] = @AssetStartId
																		AND [ManagerUnitId] = @ManagerUnitId
																		AND [AssetTransferenciaId] IS NOT NULL
																	ORDER BY [Id] DESC
																		);
										
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
											  SELECT TOP 1 [AssetStartId]
													,@ManagerUnitDestino
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
													,[MonthlyDepreciationId]
											  FROM [dbo].[MonthlyDepreciation]
											  WHERE [AssetStartId] = @AssetStartId
											  ORDER BY [Id] DESC
									END;
									ELSE
										BEGIN
											SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
														AND SourceDestiny_ManagerUnitId = @ManagerUnitId
														AND [AssetTransferenciaId] IS NOT NULL
													ORDER BY [Id] DESC);
											IF(@DATATRANSFERENCIA IS NOT NULL)
											BEGIN
												SET @ManagerUnitDestino = (SELECT TOP 1 [ManagerUnitId] 
																		FROM [dbo].[AssetMovements]  
																		WHERE [AssetId] = @AssetStartId
																			AND SourceDestiny_ManagerUnitId = @ManagerUnitId
																			AND [AssetTransferenciaId] IS NOT NULL
																		ORDER BY [Id] DESC
																			);
										
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
													  SELECT TOP 1 [AssetStartId]
															,@ManagerUnitDestino
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
															,[MonthlyDepreciationId]
													  FROM [dbo].[MonthlyDepreciation]
													  WHERE [AssetStartId] = @AssetStartId
													  ORDER BY [Id] DESC
											END;
										END;
					END;
				
						   
			END
			
			---PRINT('PASSOU 14');
			SET @DepreciationAmountStart = @CurrentValue;
			IF(@Fechamento =1)
			BEGIN
				DECLARE @VidaUtilManagerUnit INT = 0;
				SET @VIDA_UTIL = @CONTADORVIDAUTIL;
				SET @RateDepreciationMonthly =  (SELECT TOP 1 RateDepreciationMonthly FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
				SET @AccumulatedDepreciation =  (SELECT MAX(AccumulatedDepreciation) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) + @RateDepreciationMonthly;
				SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) - @RateDepreciationMonthly;

				IF(@ValueAcquisitionCalcDecree IS NOT NULL)
					BEGIN
						SET @ResidualPorcetagemValue =  (SELECT TOP 1 ResidualValue FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
				
						SET @CurrentDate =  (SELECT TOP 1 CurrentDate FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
						SET @CONTADORVIDAUTIL =  (SELECT TOP 1 CurrentMonth + 1 FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);

						SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
						SET @VIDA_UTIL = @VIDA_UTIL_DECREE;
			
					END;


				SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] IS NOT NULL
													ORDER BY [Id] DESC
													  );

				SET @DATAORIGEM = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] = @AssetId
													ORDER BY [Id] DESC
													  );
				---DESTINO
				IF(@DATAORIGEM IS NOT NULL)
				BEGIN
					---PRINT('DESTINO');

					SET @VidaUtilManagerUnit = DATEDIFF(MONTH,@AcquisitionDate,@DATAORIGEM) + 1;

					SET @QuantidadeDepreciado = (SELECT COUNT(*) FROM [dbo].[MonthlyDepreciation] WHERE AssetStartId = @AssetStartId AND [ManagerUnitId] = @ManagerUnitId);
					IF(@QuantidadeDepreciado >= @VidaUtilManagerUnit)
					BEGIN
						RETURN 0;
					END;
					ELSE
						BEGIN
							SET @VIDA_UTIL = @CONTADORVIDAUTIL;
							SET @RateDepreciationMonthly =  (SELECT TOP 1 RateDepreciationMonthly 
															  FROM [MonthlyDepreciation] 
															  WHERE [AssetStartId] = @AssetStartId 
															    AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
							---PRINT('RateDepreciationMonthly');
							---PRINT(@RateDepreciationMonthly)
							SET @AccumulatedDepreciation =  (SELECT MAX(AccumulatedDepreciation) 
							                                   FROM [MonthlyDepreciation] 
															   WHERE [AssetStartId] = @AssetStartId 
															     AND [MaterialItemCode] = @MaterialItemCode) + @RateDepreciationMonthly;
							SET @CurrentValue = (SELECT MIN([CurrentValue]) 
												   FROM [MonthlyDepreciation] 
												   WHERE [AssetStartId] = @AssetStartId 
												     AND [MaterialItemCode] = @MaterialItemCode) - @RateDepreciationMonthly;

						END;
				END;
				---ORIGEM
				IF(@DATATRANSFERENCIA IS NOT NULL)
				BEGIN
					---PRINT('ORIGEM');

					SET @VidaUtilManagerUnit = DATEDIFF(MONTH,@AcquisitionDate,@DATATRANSFERENCIA) + 1;
					SET @QuantidadeDepreciado = (SELECT COUNT(*) FROM [dbo].[MonthlyDepreciation] WHERE AssetStartId = @AssetStartId AND [ManagerUnitId] = @ManagerUnitId);
					---PRINT(@QuantidadeDepreciado)
					---PRINT(@VidaUtilManagerUnit)
					IF(@QuantidadeDepreciado >= @VidaUtilManagerUnit)
					BEGIN
						RETURN 0;
					END;
					ELSE
						BEGIN
							SET @VIDA_UTIL = @CONTADORVIDAUTIL;
							SET @RateDepreciationMonthly =  (SELECT TOP 1 RateDepreciationMonthly 
															  FROM [MonthlyDepreciation] 
															  WHERE [AssetStartId] = @AssetStartId 
															    AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);

							SET @AccumulatedDepreciation =  (SELECT MAX(AccumulatedDepreciation) 
							                                   FROM [MonthlyDepreciation] 
															   WHERE [AssetStartId] = @AssetStartId 
															     AND [MaterialItemCode] = @MaterialItemCode) + @RateDepreciationMonthly;
							SET @CurrentValue = (SELECT MIN([CurrentValue]) 
												   FROM [MonthlyDepreciation] 
												   WHERE [AssetStartId] = @AssetStartId 
												     AND [MaterialItemCode] = @MaterialItemCode) - @RateDepreciationMonthly;

						END;

				END;
			END;
				
			
			---PRINT('VIDA UTIL');
			---PRINT(@VIDA_UTIL);
			WHILE(@CONTADORVIDAUTIL <= @VIDA_UTIL)
			BEGIN

				--IF(@CONTADORVIDAUTIL = 119)
				--BEGIN 
				--	---PRINT 'OK';
				--END
				
				IF(YEAR(@DataFinal) = YEAR(@CurrentDate) AND MONTH(@DataFinal) = MONTH(@CurrentDate))
				BEGIN
					---PRINT('DATA FINAL');
					BREAK;
				END
			
				---Verifica se na data atual foi realizado uma transferência
				IF(@DATATRANSFERENCIA IS NOT NULL AND @CONTADORVIDAUTIL > 0)
				BEGIN
					--IF(YEAR(DATEADD(MONTH,1,@DATATRANSFERENCIA))  = YEAR(@CurrentDate) AND MONTH(DATEADD(MONTH,1,@DATATRANSFERENCIA)) = MONTH(@CurrentDate))
					---PRINT('PASSOU TRANSFERENCIA')
					---PRINT(@DATATRANSFERENCIA)
					IF(YEAR(@DATATRANSFERENCIA) = YEAR(@CurrentDate) AND MONTH(@DATATRANSFERENCIA) = MONTH(@CurrentDate))
					BEGIN
						---PRINT('TRANSFERÊNCIA PASSOU')
						---PRINT(@DATATRANSFERENCIA)
						---PRINT(@CurrentDate)
						---BREAK;
						---Altera Id da UGE para o destino
						SET @ManagerUnitId = (SELECT TOP 1 [SourceDestiny_ManagerUnitId] 
						                       FROM [dbo].[AssetMovements] [mov]  
												INNER JOIN [dbo].[Asset] [bem] ON [bem].[Id] = [mov].[AssetId] 
												WHERE [bem].[AssetStartId] = @AssetStartId 
													AND [mov].[AssetTransferenciaId] IS NOT NULL
													AND YEAR([mov].[MovimentDate]) = YEAR(@CurrentDate)
													AND MONTH([mov].[MovimentDate]) = MONTH(@CurrentDate)
												ORDER BY [mov].[MovimentDate] ASC, [mov].[Id] DESC)
						---PRINT(@ManagerUnitId);
						SET @DATATRANSFERENCIA = NULL;
					END
					
				END;
				---PRINT('DATA FINAL')
				---PRINT(@DataFinal);
				-----verifica se o bp foi incoprporado por transferência, caso ok depreciar a alterar Id da Uge para a origem
				--IF(@DATAORIGEM IS NOT NULL AND @CONTADORVIDAUTIL > 0)
				--BEGIN
				--	SET @ManagerUnitId = (SELECT TOP 1 [ManagerUnitId] FROM [dbo].[AssetMovements] 
				--	     WHERE [SourceDestiny_ManagerUnitId] = @ManagerUnitId AND [MonthlyDepreciationId] = @AssetStartId AND [AssetTransferenciaId] IS NOT NULL ORDER BY [Id] DESC)
					
				--	SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
				--									FROM [dbo].[AssetMovements]  
				--									WHERE [AssetId] = @AssetStartId
				--									  AND [ManagerUnitId] = @ManagerUnitId
				--									  AND [AssetTransferenciaId] IS NOT NULL
				--									ORDER BY [Id] DESC
				--									  );
				--	SET @DATAORIGEM = NULL;
				--END
			
				---Verifica se na data atual foi realizado um estorno
				IF(@DateReverse IS NOT NULL)
				BEGIN
					IF(YEAR(@DateReverse)  = YEAR(@CurrentDate) AND MONTH(@DateReverse) = MONTH(@CurrentDate))
					BEGIN
						BREAK;
					END
					
				END;

				
				SET @BEMJADEPRECIADO = (SELECT TOP 1 [Id] FROM [dbo].[MonthlyDepreciation] 
													WHERE [AssetStartId] = @AssetStartId
														AND [CurrentDate]= @CurrentDate
														AND [ManagerUnitId] = @ManagerUnitId
														AND [MaterialItemCode] = @MaterialItemCode)

				IF(@BEMJADEPRECIADO IS NULL)
				
				IF(@CONTADORVIDAUTIL = 0)
				BEGIN
					---Verifica se o bem para a data atual já foi depreciado, caso sim não é realizado o insert, assim evitando error de inclusão
				
					IF(ISNULL(@BEMJADEPRECIADO,0) < 1)
					BEGIN
						
							---Alterar o dia do primeiro registro "linha 0".
							SET @CurrentDateInitial =(SELECT DATEADD(DAY,-1,@CurrentDate));
							
							UPDATE [dbo].[Asset] SET [DepreciationDateStart] = @CurrentDate, [DepreciationAmountStart] = @DepreciationAmountStart
												WHERE [AssetStartId] = @AssetStartId AND [DepreciationDateStart] IS NULL;	
							

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
						---PRINT('CURRENT MONTH');
						---PRINT(@CONTADORVIDAUTIL);

						IF(@CONTADORVIDAUTIL = @VIDA_UTIL)
						BEGIN
							IF(@BEMDEPRECIADONAOTOTAL = 1)
							BEGIN
								SET @CurrentMonth =(SELECT COUNT(*) + @CONTADORVIDAUTIL
											FROM MonthlyDepreciation 
											WHERE [AssetStartId] = @AssetStartId
											);
							END;
							ELSE
								BEGIN
									SET @CurrentMonth =(SELECT COUNT(*) 
											FROM MonthlyDepreciation 
											WHERE [AssetStartId] = @AssetStartId
										);
								END;
						
							IF(@CurrentMonth >= @LifeCicle AND @Fechamento = 0)
							BEGIN
								SET @UnfoldingValue = @CurrentValue - @ResidualPorcetagemValue;
								SET @CurrentValue = @CurrentValue - @UnfoldingValue;
								SET @AccumulatedDepreciation = @AccumulatedDepreciation + @UnfoldingValue;
								SET @RateDepreciationMonthly = @RateDepreciationMonthly + @UnfoldingValue;
								SET @ULTIMADEPRECIACAO = 1;
							END;

							IF(@DateDecree IS NOT NULL AND (@MesesFaltanteDepreciarDecreto + @CurrentMonth) >= @LifeCicle AND @Fechamento = 0)
							BEGIN
								SET @UnfoldingValue = @CurrentValue - @ResidualPorcetagemValue;
								SET @CurrentValue = @CurrentValue - @UnfoldingValue;
								SET @AccumulatedDepreciation = @AccumulatedDepreciation + @UnfoldingValue;
								SET @RateDepreciationMonthly = @RateDepreciationMonthly + @UnfoldingValue;
								SET @ULTIMADEPRECIACAO = 1;
							END;
							
						END;
						
						IF(ISNULL(@BEMJADEPRECIADO,0) < 1)
						BEGIN
							---PRINT('RateDepreciationMonthly')
							---PRINT(@RateDepreciationMonthly)
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
											,@AssetStartId);

								
						END;
				

					END;
			
				--Alteração Aqui
				SET @MaximoMesAtual = @CONTADORVIDAUTIL;

				IF(@ULTIMADEPRECIACAO  = 1)
				BEGIN
					SET @MaximoMesAtual = @LifeCicle;
					BREAK;
				END;
			
				IF(@CONTADORVIDAUTIL =0)
				BEGIN
					SET @CurrentDate =(SELECT DATEADD(DAY,1,@CurrentDate));
				END;
				ELSE
					BEGIN
						SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
						
					END;

				SET @DATATRANSFERENCIA = (SELECT TOP 1 [mov].[MovimentDate] 
													FROM [dbo].[AssetMovements] [mov]  
													INNER JOIN [dbo].[Asset] [bem] ON [bem].[Id] = [mov].[AssetId] 
													WHERE [bem].[AssetStartId] = @AssetStartId 
														AND [mov].[AssetTransferenciaId] IS NOT NULL
														AND YEAR([mov].[MovimentDate]) = YEAR(@CurrentDate)
														AND MONTH([mov].[MovimentDate]) = MONTH(@CurrentDate)
													ORDER BY [mov].[MovimentDate] ASC, [mov].[Id] DESC
													  );
				
				IF(@ULTIMADEPRECIACAO =0)
				BEGIN
					SET @AccumulatedDepreciation = @RateDepreciationMonthly + @AccumulatedDepreciation;
					SET @CurrentValue = (@CurrentValue - @RateDepreciationMonthly) - @UnfoldingValue;
				END;
				
				
				SET @CONTADORVIDAUTIL = @CONTADORVIDAUTIL + 1;
				
			END;

			DELETE FROM @TABLEBEM WHERE [Id] = @TABLEBEMID;
			
			---PRINT('FINALIZADO 2')
			---PRINT(@CurrentDate)
			SET @CONTADORBEM = @CONTADORBEM + 1;

		END TRY
		BEGIN CATCH
			SET @Retorno =ERROR_MESSAGE();
			SET @Erro = 1;
			SET @CONTADORBEM = @CONTADORBEM + 1;
			PRINT(ERROR_MESSAGE());
		END CATCH
	END;

	SET @CONTADOR = @CONTADOR + 1;
	END TRY
	
	BEGIN CATCH
		SET @Retorno =ERROR_MESSAGE();
		SET @Erro = 1;
		SET @CONTADOR = @CONTADOR + 1;
		PRINT(ERROR_MESSAGE());
	END CATCH
END;

--Alteração Aqui
---PRINT(@Decree);
---PRINT(@VIDA_UTIL);
---PRINT(@CONTADORVIDAUTIL);
---PRINT(@LifeCicle);
---PRINT(@MaximoMesAtual);

IF(@Decree = 0 AND @VIDA_UTIL > 0 AND @CONTADORVIDAUTIL >= @LifeCicle and @MaximoMesAtual = @LifeCicle)
---Ultimo registro bem sem decreto
BEGIN
	---PRINT('ULTIMO')
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
						,@MaterialItemCode
						,@AcquisitionDate
						,(SELECT DATEADD(DAY,1,@CurrentDate))
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

				SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [AssetTransferenciaId] IS NOT NULL
													ORDER BY [Id] DESC
													  );
				---PRINT(@AssetId);

							
				---PRINT('DESTINO 4');
				---PRINT(@AssetStartId);
				---PRINT(@ManagerUnitId);
				---PRINT(@AssetId);
				---ORIGEM
				IF(@DATATRANSFERENCIA IS NOT NULL)
				BEGIN
					DECLARE @CountTransfere INT=0;
					DECLARE @ContadorTransfere INT = 0;
					DECLARE @MovimentDateTransfere DATETIME = NULL;


					INSERT INTO @TableTransfencia(ManagerUnitId,AssetId,MovimentDate)
					  SELECT [SourceDestiny_ManagerUnitId],[AssetTransferenciaId],[MovimentDate]
			          FROM [dbo].[AssetMovements]  
					  WHERE [MonthlyDepreciationId] IN(SELECT [MonthlyDepreciationId] FROM [dbo].[AssetMovements]
														 WHERE  [AssetId] = @AssetStartId
														   AND [AssetTransferenciaId] IS NOT NULL)
					  AND [AssetTransferenciaId] IS NOT NULL
					  GROUP BY [SourceDestiny_ManagerUnitId],[AssetTransferenciaId],[MovimentDate]


					SET @CountTransfere = (SELECT COUNT(*) FROM @TableTransfencia)
					---PRINT('QUANTIDADE');
					---PRINT(@CountTransfere);
					WHILE(@ContadorTransfere <= @CountTransfere)
					BEGIN
						---PRINT('PASSOU TRANSFERENCIA')
						SET @ManagerUnitDestino = (SELECT TOP 1 ManagerUnitId 
												  FROM @TableTransfencia);

						SET @AssetId = (SELECT TOP 1 AssetId 
												  FROM @TableTransfencia WHERE ManagerUnitId = @ManagerUnitDestino);
						
						SET @MovimentDateTransfere = (SELECT TOP 1 MovimentDate 
												  FROM @TableTransfencia WHERE ManagerUnitId = @ManagerUnitDestino);
						---PRINT(@ManagerUnitDestino);

						SET @QuantidadeDepreciado = (SELECT COUNT(*) FROM [dbo].[MonthlyDepreciation] WHERE AssetStartId = @AssetStartId AND [ManagerUnitId] = @ManagerUnitDestino);

						IF(ISNULL(@QuantidadeDepreciado,0) < 1)
						BEGIN
							---PRINT('TRANSFERENCIA 20')
							INSERT INTO MonthlyDepreciation
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
								SELECT TOP 1 [AssetStartId]
										,@ManagerUnitDestino
										,[InitialId]
										,[MaterialItemCode]
										,[AcquisitionDate]
										,@MovimentDateTransfere
										,[DateIncorporation]
										,[LifeCycle]
										,[CurrentMonth]
										,[ValueAcquisition]
										,[CurrentValue]
										,[ResidualValue]
										,0.0
										,@AccumulatedDepreciation
										,0.0
										,[Decree]
										,[ManagerUnitTransferId]
										,[MonthlyDepreciationId]
									FROM [dbo].[MonthlyDepreciation]
									WHERE [AssetStartId] = @AssetStartId
									ORDER BY [Id] DESC

									SET @CurrentValue = (SELECT TOP 1 [CurrentValue] FROM [dbo].[MonthlyDepreciation] 
														  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] = @ManagerUnitId ORDER BY [Id] DESC)
										END
									
						DELETE @TableTransfencia WHERE [ManagerUnitId] = @ManagerUnitDestino;
						SET @ContadorTransfere = @ContadorTransfere + 1;
						END;
	
				END;
		END;
ELSE
	---Ultimo registro bem com decreto
	--Alteração Aquo
	IF(@Decree = 1 AND @CONTADORVIDAUTIL >= @LifeCicle and @MaximoMesAtual = @LifeCicle)
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
				,1
				,NULL
				,@AssetStartId)

	END;

	DECREE_FINAL:
		---Ultimo registro bem com decreto
		IF(@Decree = 1 AND @FINAL = 1)
		BEGIN
			
			INSERT INTO MonthlyDepreciation
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
						,1
						,NULL
						,@AssetStartId)

			END;
TRANSFERENCIA_FINAL:
	BEGIN
		SET @DateDecree =(SELECT TOP 1 [ast].[MovimentDate] 
							FROM [dbo].[Asset] [ast]
							INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
							WHERE [ast].[flagDecreto]  = 1
								AND [ast].[Id] = @AssetStartId
							ORDER BY [ast].[Id] DESC);
		IF(@DateDecree IS NULL)
		BEGIN
			SET @AccumulatedDepreciation =(SELECT TOP 1 [AccumulatedDepreciation] 
											 FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] DESC)
			SET @CurrentDate =(SELECT TOP 1 [CurrentDate]
											 FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] DESC)

		END;
		ELSE
			BEGIN
				SET @RateDepreciationMonthly = 0.0;
				SET @AccumulatedDepreciation =0.0;
				SET @CurrentDate = (SELECT TOP 1 [MovimentDate] FROM [dbo].[AssetMovements] 
					   WHERE [MonthlyDepreciationId] = @AssetStartId AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId ORDER BY [Id] DESC)

				IF(@CurrentDate IS NULL)
				BEGIN
					SET @CurrentDate =(SELECT TOP 1 [CurrentDate]
											 FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] DESC)

				END;
			END;
		IF(@FINAL = 1)
		BEGIN
---Transferência com depreciação total na origem
		INSERT INTO MonthlyDepreciation
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
			SELECT TOP 1 [AssetStartId]
					,@ManagerUnitId
					,[InitialId]
					,[MaterialItemCode]
					,[AcquisitionDate]
					,(SELECT TOP 1 [MovimentDate] FROM [dbo].[AssetMovements] 
					   WHERE [MonthlyDepreciationId] = @AssetStartId AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId ORDER BY [Id] DESC)
					,[DateIncorporation]
					,[LifeCycle]
					,[CurrentMonth]
					,[ValueAcquisition]
					,[CurrentValue]
					,[ResidualValue]
					,0.0
					,@AccumulatedDepreciation
					,0.0
					,[Decree]
					,[ManagerUnitTransferId]
					,[MonthlyDepreciationId]
				FROM [dbo].[MonthlyDepreciation]
				WHERE [AssetStartId] = @AssetStartId
				ORDER BY [Id] DESC

				SET @CurrentValue = (SELECT TOP 1 [CurrentValue] FROM [dbo].[MonthlyDepreciation] 
				                      WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] = @ManagerUnitId ORDER BY [Id] DESC)

		END;
	END;

	UPDATE [dbo].[Asset] SET ValueUpdate = @CurrentValue 
						WHERE [AssetStartId] = @AssetStartId 
						  AND [ManagerUnitId] = @ManagerUnitId 
						  AND [MaterialItemCode] = @MaterialItemCode 
	RETURN @Contador;
END TRY
BEGIN CATCH
	SET @Retorno =ERROR_MESSAGE();
END CATCH
GO
GRANT EXECUTE ON [dbo].[SAM_DEPRECIACAO_UGE] TO [USUSAM]
GO