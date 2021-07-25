SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'SAM_CALCULAR_DEPRECIACAO_FECHAMENTO')
BEGIN
    DROP PROCEDURE [dbo].[SAM_CALCULAR_DEPRECIACAO_FECHAMENTO]
END
GO

CREATE PROCEDURE [dbo].[SAM_CALCULAR_DEPRECIACAO_FECHAMENTO] 
	@AssetStartId INT,
	@ManagerUnitId INT,
	@CodigoMaterial INT
	--@CurrentValueOutPut DECIMAL(18,2) OUTPUT
AS
BEGIN
	DECLARE @CurrentMonth SMALLINT;
	DECLARE @CurrentValue DECIMAL(18,2);
	DECLARE @CurrentDate DATETIME;
	DECLARE @RateDepreciationMonthly DECIMAL(18,2);
	DECLARE @AccumulatedDepreciation DECIMAL(18,2);
	DECLARE @LifeCycle SMALLINT;
	DECLARE @Count INT;
	DECLARE @DataAquisicao AS DATETIME;
	DECLARE @DataIncorporacao AS DATETIME;
	DECLARE @Id INT;
	DECLARE @DataDecreto AS DATETIME;
	DECLARE @ValorAquisicao AS DECIMAL(18,2);
	DECLARE @ValorDecreto AS DECIMAL(18,2);
	DECLARE @AceleratedDepreciation BIT;
	DECLARE @PorcetagemResidual DECIMAL(18,2);
	DECLARE @ValorResidual DECIMAL(18,2);
	DECLARE @DepreciacaoMensal DECIMAL(18,2);
	DECLARE @DataTransferencia AS DATETIME;
	DECLARE @ManagerUnitTransferId INT;
	DECLARE @AssetStartIdTranferencia INT;
	DECLARE @VidaUtilInicio SMALLINT;
	DECLARE @MesReferencia AS VARCHAR(6);
	DECLARE @DataTransfere AS DATETIME

	DECLARE @Table as TABLE(
		Id INT NOT NULL,
		CurrentMonth INT NOT NULL,
		CurrentValue DECIMAL(18,2) NOT NULL,
		CurrentDate DATETIME NOT NULL,
		RateDepreciationMonthly DECIMAL(18,2) NOT NULL,
		AccumulatedDepreciation DECIMAL(18,2) NOT NULL,
		LifeCycle SMALLINT NOT NULL
	) 
	SET DATEFORMAT YMD;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; 
	INSERT INTO @Table(Id,CurrentMonth,CurrentValue,CurrentDate,RateDepreciationMonthly,AccumulatedDepreciation,LifeCycle)
	       SELECT  TOP 1 Id,CurrentMonth,CurrentValue,CurrentDate,RateDepreciationMonthly,AccumulatedDepreciation ,LifeCycle
	         FROM [dbo].[MonthlyDepreciation]
		   WHERE [AssetStartId] = @AssetStartId 
		     AND [ManagerUnitId] = @ManagerUnitId 
			 AND [MaterialItemCode] = @CodigoMaterial
		   ORDER BY Id DESC

	SET @Count = (SELECT COUNT(*) FROM @Table);
	SET @DataDecreto = (SELECT TOP 1 [ast].[MovimentDate] 
							  FROM [dbo].[Asset] [ast]
							 WHERE [ast].[flagDecreto]  = 1
							   AND [ast].[AssetStartId] = @AssetStartId
							 ORDER BY [ast].[Id] ASC);
		
	IF(@Count IS NULL OR @Count < 1)
	BEGIN
		

		SET @DataAquisicao =(SELECT TOP 1 [AcquisitionDate] 
									   FROM [dbo].[Asset] 
										WHERE [AssetStartId] = @AssetStartId 
										 AND [ManagerUnitId] = @ManagerUnitId 
										 AND [MaterialItemCode] = @CodigoMaterial
										ORDER BY [Id] ASC
									);
		SET @DataIncorporacao =(SELECT TOP 1 [MovimentDate] 
								   FROM [dbo].[Asset] 
								  WHERE [AssetStartId] = @AssetStartId 
									AND [ManagerUnitId] = @ManagerUnitId 
									AND [MaterialItemCode] = @CodigoMaterial
								  ORDER BY [Id] ASC
								);
		IF(@DataIncorporacao < @DataAquisicao)
		BEGIN
			SET @DataAquisicao = @DataIncorporacao;
		END;

		IF(@DataDecreto IS NOT NULL)
		BEGIN
			
			SET @ValorDecreto =  (SELECT TOP 1 [ValueUpdate] 
							FROM [dbo].[Asset] [ast] 
							WHERE [AssetStartId] = @AssetStartId);
			UPDATE [dbo].[Asset] SET [DepreciationDateStart] = @DataDecreto, [DepreciationAmountStart] = @ValorDecreto
			WHERE [AssetStartId] = @AssetStartId;

							   
		END
		ELSE
			BEGIN
				
				SET @ValorAquisicao =  (SELECT TOP 1 [ValueAcquisition]
							FROM [dbo].[Asset] [ast] 
							WHERE [AssetStartId] = @AssetStartId);

				UPDATE [dbo].[Asset] SET [DepreciationDateStart] = @DataAquisicao, [DepreciationAmountStart] = @ValorAquisicao
				WHERE [AssetStartId] = @AssetStartId;
			END;

		SET @CurrentMonth =0;
		
		SET @CurrentValue =(SELECT TOP 1 [DepreciationAmountStart] 
		                    FROM [dbo].[Asset]  
						WHERE [AssetStartId] = @AssetStartId)
							
		
		

		SET @CurrentDate =@DataAquisicao;

		SET @AccumulatedDepreciation =0
				--- Retornar a vida util do item de material
		SET @LifeCycle = (SELECT TOP 1 [gr].[LifeCycle] 
						   FROM [dbo].[MaterialGroup] [gr]
						   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
						  WHERE [AssetStartId] = @AssetStartId 
							AND [ManagerUnitId] = @ManagerUnitId 
							AND [MaterialItemCode] = @CodigoMaterial
					);


			SET @AceleratedDepreciation =(SELECT TOP 1 [ast].[AceleratedDepreciation] 
								  FROM [dbo].[Asset] [ast]
								 WHERE [ast].[flagDecreto]  = 1
								   AND [ast].[AssetStartId] = @AssetStartId
								 ORDER BY [ast].[Id] ASC);
			IF(@AceleratedDepreciation IS NULL)
			BEGIN
								
				SET @PorcetagemResidual = (SELECT TOP 1 [gr].[ResidualValue] 
											FROM [dbo].[MaterialGroup] [gr]
											INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
										WHERE [ast].[AssetStartId] = @AssetStartId); 
			END;
			ELSE
				BEGIN
									
					SET @PorcetagemResidual = (SELECT TOP 1 [ast].[ResidualValue] 
											FROM [dbo].[Asset] [ast] 
										WHERE [ast].[AssetStartId] = @AssetStartId); 

					SET @LifeCycle = (SELECT TOP 1 [ast].[LifeCycle] 
											FROM [dbo].[Asset] [ast] 
										WHERE [ast].[AssetStartId] = @AssetStartId); 
			END;			
						
			
			
			SET @ManagerUnitTransferId = (SELECT TOP 1 [mov].[ManagerUnitId] 
												FROM [dbo].[AssetMovements] [mov]
												INNER JOIN [dbo].[Asset] [bem] ON [bem].[Id] = [mov].[AssetId]
												WHERE [bem].[MaterialItemCode] = @CodigoMaterial
												  AND [mov].[AssetTransferenciaId]= @AssetStartId);
				IF(@ManagerUnitTransferId IS NOT NULL)
				BEGIN
					SET @DataTransferencia =  (SELECT TOP 1 [mov].[MovimentDate] 
												FROM [dbo].[AssetMovements] [mov]
												INNER JOIN [dbo].[Asset] [bem] ON [bem].[Id] = [mov].[AssetId]
												WHERE [bem].[MaterialItemCode] = @CodigoMaterial
												  AND [mov].[AssetTransferenciaId]= @AssetStartId);

					IF(@DataTransferencia IS NOT NULL)
					BEGIN
						SET @AssetStartIdTranferencia = (SELECT TOP 1 [mov].[AssetId]
												FROM [dbo].[AssetMovements] [mov]
												INNER JOIN [dbo].[Asset] [bem] ON [bem].[Id] = [mov].[AssetId]
												WHERE [bem].[MaterialItemCode] = @CodigoMaterial
												  AND [mov].[AssetTransferenciaId]= @AssetStartId);

												   
							SET @CurrentMonth =(SELECT MAX(CurrentMonth) 
											   FROM [dbo].[MonthlyDepreciation] 
											  WHERE [AssetStartId] = @AssetStartIdTranferencia
											    AND [MaterialItemCode] = @CodigoMaterial
												AND [ManagerUnitId] = @ManagerUnitTransferId
											   );
							SET @VidaUtilInicio = (SELECT TOP 1 [gr].[LifeCycle] 
											   FROM [dbo].[MaterialGroup] [gr]
											   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
											   WHERE [ast].[MaterialItemCode] = @CodigoMaterial)

							SET @LifeCycle = @VidaUtilInicio - @CurrentMonth;
							SET @CurrentMonth = 0;
							SET @CurrentValue = (SELECT MIN([CurrentValue])
											FROM [dbo].[MonthlyDepreciation] 
										  WHERE [AssetStartId] = @AssetStartIdTranferencia
											    AND [MaterialItemCode] = @CodigoMaterial
												AND [ManagerUnitId] = @ManagerUnitTransferId
										);

								SET @DataIncorporacao =(SELECT TOP 1 [MovimentDate] 
											   FROM [dbo].[Asset] 
											  WHERE [AssetStartId] = @AssetStartId
											  AND [ManagerUnitId] = @ManagerUnitId
											  ORDER BY [Id] ASC
											);
							SET @ManagerUnitTransferId = NULL;

						DECLARE @DataTransferenciaSequence AS DATETIME;

						SET @DataTransferenciaSequence = (SELECT [mov].[MovimentDate] FROM [Asset] [bem] 
												   INNER JOIN [AssetMovements] [mov] ON [mov].[AssetId] = [bem].[Id]
												   WHERE [bem].[MaterialItemCode] = @CodigoMaterial  
													 AND [bem].[AssetStartId] = @AssetStartId
													 AND [mov].[ManagerUnitId] = @ManagerUnitId 
													 AND [mov].[AssetTransferenciaId] IS NOT NULL)

						IF(@DataTransferenciaSequence IS NOT NULL)
						BEGIN
							IF(@CurrentDate > @DataTransferenciaSequence )
							BEGIN
								
								SET @DataTransferencia = NULL;
								RETURN 0;
							END;
						END;
						
						SET @CurrentDate =@DataTransferencia;
							
		

					END;

				END;
				ELSE
					BEGIN
						SET @DataTransferencia = (SELECT [mov].[MovimentDate] FROM [Asset] [bem] 
												   INNER JOIN [AssetMovements] [mov] ON [mov].[AssetId] = [bem].[Id]
												   WHERE [bem].[MaterialItemCode] = @CodigoMaterial  
													 AND [bem].[AssetStartId] = @AssetStartId
													 AND [mov].[ManagerUnitId] = @ManagerUnitId 
													 AND [mov].[AssetTransferenciaId] IS NOT NULL)
						IF(@DataTransferencia IS NOT NULL)
						BEGIN
							SET @DataTransfere = CAST(YEAR(@DataTransferenciaSequence) AS VARCHAR(4)) + '-' + CAST(MONTH(@DataTransferenciaSequence) AS VARCHAR(2)) + '-01';
							IF(@CurrentDate >= @DataTransfere )
							BEGIN
								
								SET @DataTransferencia = NULL;
								RETURN 0;
							END;
						END;

					END 
						
				
			SET @ValorResidual =ROUND((@ValorAquisicao * ROUND(@PorcetagemResidual,2,1)) / 100,2,1);
			SET @DepreciacaoMensal = ROUND((@ValorAquisicao - @ValorResidual) / @LifeCycle,2,1);

			INSERT INTO [dbo].[MonthlyDepreciation]
				   ([AssetStartId]
				   ,[NumberIdentification]
				   ,[ManagerUnitId]
				   ,[MaterialItemCode]
				   ,[InitialId]
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
				   ,[MonthlyDepreciationId]
				   ,[ManagerUnitTransferId])
			 SELECT
				   @AssetStartId
				   ,Null
				   ,@ManagerUnitId
				   ,@CodigoMaterial
				   ,InitialId
				   ,AcquisitionDate
				   ,@CurrentDate
				   ,MovimentDate
				   ,@LifeCycle
				   ,@CurrentMonth
				   ,ValueAcquisition
				   ,@CurrentValue
				   ,@ValorResidual
				   ,@DepreciacaoMensal
				   ,@AccumulatedDepreciation
				   ,Null
				   ,(CASE WHEN flagDecreto IS NULL THEN 0 ELSE flagDecreto END) 'Decree'
				   ,@AssetStartId
				   ,Null
			FROM  [dbo].[Asset]	
			WHERE [AssetStartId] = @AssetStartId 
			  AND [ManagerUnitId] = @ManagerUnitId 
			  AND [MaterialItemCode] = @CodigoMaterial
	END;
	DELETE @Table

	SET @DataTransferenciaSequence = (SELECT [mov].[MovimentDate] FROM [Asset] [bem] 
												   INNER JOIN [AssetMovements] [mov] ON [mov].[AssetId] = [bem].[Id]
												   WHERE [bem].[MaterialItemCode] = @CodigoMaterial  
													 AND [bem].[AssetStartId] = @AssetStartId
													 AND [mov].[ManagerUnitId] = @ManagerUnitId 
													 AND [mov].[AssetTransferenciaId] IS NOT NULL)

	
						

	INSERT INTO @Table(Id,CurrentMonth,CurrentValue,CurrentDate,RateDepreciationMonthly,AccumulatedDepreciation,LifeCycle)
	    SELECT  TOP 1 Id,CurrentMonth,CurrentValue,CurrentDate,RateDepreciationMonthly,AccumulatedDepreciation ,LifeCycle
	        FROM [dbo].[MonthlyDepreciation]
		WHERE [AssetStartId] = @AssetStartId 
		    AND [ManagerUnitId] = @ManagerUnitId 
			AND [MaterialItemCode] = @CodigoMaterial
		ORDER BY Id DESC


	SET @Id=(SELECT TOP 1 Id FROM @Table);
	SET @CurrentMonth =(SELECT CurrentMonth FROM @Table)
	SET @CurrentDate =(SELECT CurrentDate FROM @Table)

	IF(@DataTransferenciaSequence IS NOT NULL)
	BEGIN
		SET @DataTransfere = CAST(YEAR(@DataTransferenciaSequence) AS VARCHAR(4)) + '-' + CAST(MONTH(@DataTransferenciaSequence) AS VARCHAR(2)) + '-01';
		IF(@CurrentDate >= @DataTransfere )
		BEGIN
								
			SET @DataTransferencia = NULL;
			RETURN 0;
		END;
	END;

	SET @MesReferencia =(SELECT TOP 1 ManagmentUnit_YearMonthReference FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)

	IF(@MesReferencia <= CAST(YEAR(@CurrentDate) AS VARCHAR(4)) +  (CAST(MONTH(@CurrentDate) AS VARCHAR(2))))
	BEGIN
		RETURN 0;
	END



	IF(@DataDecreto IS NOT NULL AND YEAR(@DataDecreto) = YEAR(@CurrentDate) AND MONTH(@DataDecreto) = MONTH(@CurrentDate))
	BEGIN
		SET @CurrentValue =(SELECT TOP 1 [DepreciationAmountStart] 
		                     FROM [dbo].[Asset]  
							WHERE [AssetStartId] = @AssetStartId 
							  AND [ManagerUnitId] = @ManagerUnitId 
							  AND [MaterialItemCode] = @CodigoMaterial);
	END
	ELSE
		BEGIN
			SET @CurrentValue =(SELECT CurrentValue FROM @Table)
		END;
	

	
	SET @AccumulatedDepreciation =(SELECT AccumulatedDepreciation FROM @Table)
	SET @RateDepreciationMonthly =(SELECT RateDepreciationMonthly FROM @Table)
	SET @LifeCycle = (SELECT LifeCycle FROM @Table)


		IF(@CurrentMonth < @LifeCycle)
		BEGIN 
			IF(@DataDecreto IS NOT NULL AND YEAR(@DataDecreto) = YEAR(@CurrentDate) AND MONTH(@DataDecreto) = MONTH(@CurrentDate))
			BEGIN
				SET @CurrentValue = @CurrentValue;
				SET @CurrentMonth = @CurrentMonth + 1;
				SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
				SET @AccumulatedDepreciation = @AccumulatedDepreciation;
			END
			ELSE
			BEGIN
				SET @CurrentValue = @CurrentValue - @RateDepreciationMonthly;
				SET @CurrentMonth = @CurrentMonth + 1;
				SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
				SET @AccumulatedDepreciation = @AccumulatedDepreciation + @RateDepreciationMonthly;
			END;
			INSERT INTO [dbo].[MonthlyDepreciation]
				   ([AssetStartId]
				   ,[NumberIdentification]
				   ,[ManagerUnitId]
				   ,[MaterialItemCode]
				   ,[InitialId]
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
				   ,[MonthlyDepreciationId]
				   ,[ManagerUnitTransferId])
			 SELECT
				   @AssetStartId
				   ,NumberIdentification
				   ,@ManagerUnitId
				   ,@CodigoMaterial
				   ,InitialId
				   ,AcquisitionDate
				   ,@CurrentDate
				   ,DateIncorporation
				   ,LifeCycle
				   ,@CurrentMonth
				   ,ValueAcquisition
				   ,@CurrentValue
				   ,ResidualValue
				   ,RateDepreciationMonthly
				   ,@AccumulatedDepreciation
				   ,UnfoldingValue
				   ,(CASE WHEN Decree IS NULL THEN 0 ELSE Decree END) 'Decree'
				   ,MonthlyDepreciationId
				   ,ManagerUnitTransferId
			FROM  [dbo].[MonthlyDepreciation]	
			WHERE [Id] = @Id
		END
		ELSE
			IF(@CurrentMonth = @LifeCycle)
			BEGIN
				SET @CurrentMonth = @CurrentMonth + 1;
				SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));

				INSERT INTO [dbo].[MonthlyDepreciation]
					   ([AssetStartId]
					   ,[NumberIdentification]
					   ,[ManagerUnitId]
					   ,[MaterialItemCode]
					   ,[InitialId]
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
					   ,[MonthlyDepreciationId]
					   ,[ManagerUnitTransferId])
				 SELECT
					   @AssetStartId
					   ,NumberIdentification
					   ,@ManagerUnitId
					   ,@CodigoMaterial
					   ,InitialId
					   ,AcquisitionDate
					   ,@CurrentDate
					   ,DateIncorporation
					   ,LifeCycle
					   ,@CurrentMonth
					   ,ValueAcquisition
					   ,CurrentValue
					   ,ResidualValue
					   ,0.0
					   ,@AccumulatedDepreciation
					   ,UnfoldingValue
					   ,(CASE WHEN Decree IS NULL THEN 0 ELSE Decree END) 'Decree'
					   ,MonthlyDepreciationId
					   ,ManagerUnitTransferId
				FROM  [dbo].[MonthlyDepreciation]	
				WHERE [Id] = @Id
			END;


	--SELECT @CurrentValueOutPut = @CurrentValue;
	--SELECT @CurrentValue
	UPDATE [dbo].[Asset] SET ValueUpdate = @CurrentValue
	WHERE [AssetStartId] = @AssetStartId 
	  AND [ManagerUnitId] = @ManagerUnitId 
	  AND [MaterialItemCode] = @CodigoMaterial 
END;	
GO