SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'EXECUTA_FECHAMENTO')
BEGIN
    DROP PROCEDURE [dbo].[EXECUTA_FECHAMENTO]
END
GO

CREATE PROCEDURE [dbo].[EXECUTA_FECHAMENTO]
 @ManagerUnitId INT
AS

 DECLARE @Count INT;
 DECLARE @Contador INT = 0
 DECLARE @Id INT;
 DECLARE @LifeCycle SMALLINT;
 DECLARE @CurrentMonth SMALLINT;
 DECLARE @MaterialItemCode INT;
 DECLARE @AssetStartId INT;
 DECLARE @AssetId INT;
 DECLARE @CountInservivel INT;
 DECLARE @AssetIdInservivel INT = Null;


 DECLARE @RateDepreciationMonthly DECIMAL(18,2); 
 DECLARE @AccumulatedDepreciation DECIMAL(18,2)
 DECLARE @CurrentValue DECIMAL(18,2)
 DECLARE @CurrentDate DATETIME;

 DECLARE @RateDepreciationMonthlyInserir DECIMAL(18,2); 
 DECLARE @AccumulatedDepreciationInserir DECIMAL(18,2)
 DECLARE @CurrentValueInserir DECIMAL(18,2)
 DECLARE @CurrentDateInserir DATETIME;
 DECLARE @CurrentMonthInserir SMALLINT;

 DECLARE @_assetStartId INT;
 DECLARE @TransferenciaCount INT;

 DECLARE @Retorno VARCHAR(1000)
 DECLARE @Erro BIT = 0

 SET DATEFORMAT YMD;
 DECLARE @DataFinal DATETIME = '2020-07-10'

 DECLARE @Table AS Table(
	Id INT IDENTITY(1,1) NOT NULL,
	MaterialItemCode INT NOT NULL,
	AssetStartId INT,
	AssetId INT

 )


 DECLARE @TableInservivel AS TABLE(
	Id INT NOT NULL IDENTITY(1,1),
	AssetId INT NOT NULL,
	MaterialItemCode INT NOT NULL
 )

 INSERT INTO @TableInservivel(AssetId,MaterialItemCode)
 SELECT [bem].[Id], [bem].[MaterialItemCode] FROM [dbo].[Asset] [bem] 
 INNER JOIN [dbo].[AssetMovements] [mov] ON [bem].[Id] = [mov].[AssetId]
  WHERE [mov].[ManagerUnitId] = @ManagerUnitId AND [mov].[AuxiliaryAccountId] = 212508


 INSERT INTO @Table(MaterialItemCode,AssetStartId,AssetId)
 SELECT [MaterialItemCode],[AssetStartId],[AssetId] FROM [dbo].[Asset] [bem]
 INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [bem].[Id] 
 WHERE [bem].[MaterialGroupCode] <> 0
   AND [bem].[ManagerUnitId] = @ManagerUnitId
   AND [bem].[flagVerificado] IS NULL
   AND [bem].[flagDepreciaAcumulada] = 1
   AND [bem].[Status] = 1
   AND ([bem].[flagAcervo] IS NULL OR [bem].[flagAcervo] = 0)
   AND ([bem].[flagTerceiro] IS NULL OR [bem].[flagTerceiro] = 0)
   AND ([bem].[flagAnimalNaoServico] IS NULL OR [bem].[FlagAnimalNaoServico] = 0)
   AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] = 1)
   AND ([mov].[Status] = 1)


SET @Count =(SELECT COUNT(*) FROM @Table)

WHILE(@Contador < @Count)
BEGIN
	SET @Id =(SELECT TOP 1 [Id] FROM @Table)
	SET @MaterialItemCode = (SELECT [MaterialItemCode] FROM @Table WHERE [Id]= @Id);
	SET @AssetId = (SELECT [AssetId] FROM @Table WHERE [Id]= @Id); 
	SET @AssetStartId = (SELECT [AssetStartId] FROM @Table WHERE [Id]= @Id); 

   --- var _query = (db.MonthlyDepreciations.Where(x => x.AssetStartId == item.AssetStartId && x.ManagerUnitId == managerUnitId).OrderByDescending(x => x.Id)).Take(1);


	SET @LifeCycle =(SELECT TOP 1 [LifeCycle] FROM [dbo].[MonthlyDepreciation] 
					  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] =@ManagerUnitId
					  ORDER BY [Id] DESC);

	SET @CurrentMonth =(SELECT TOP 1 [CurrentMonth] FROM [dbo].[MonthlyDepreciation] 
					  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] =@ManagerUnitId
					  ORDER BY [Id] DESC);

	SET @RateDepreciationMonthly = (SELECT TOP 1 [RateDepreciationMonthly] FROM [dbo].[MonthlyDepreciation] 
					  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] =@ManagerUnitId
					  ORDER BY [Id] DESC);

	SET @AccumulatedDepreciation = (SELECT TOP 1 [AccumulatedDepreciation] FROM [dbo].[MonthlyDepreciation] 
					  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] =@ManagerUnitId
					  ORDER BY [Id] DESC);

	SET @CurrentValue = (SELECT TOP 1 [CurrentValue] FROM [dbo].[MonthlyDepreciation] 
					  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] =@ManagerUnitId
					  ORDER BY [Id] DESC);
	SET @CurrentDate = (SELECT TOP 1 [CurrentDate] FROM [dbo].[MonthlyDepreciation] 
					  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] =@ManagerUnitId
					  ORDER BY [Id] DESC);

	SET @CountInservivel =(SELECT COUNT(*) FROM @TableInservivel);
	
	IF(@CountInservivel IS NOT NULL)
	BEGIN
		SET @AssetIdInservivel =(SELECT AssetId FROM @TableInservivel WHERE [MaterialItemCode] = @MaterialItemCode)
	END;

	
	IF(ISNULL(@LifeCycle,0) > 0 AND @LifeCycle >= @CurrentMonth)
	BEGIN
		IF (@AssetIdInservivel IS NULL)
		BEGIN
			SET @RateDepreciationMonthlyInserir = @RateDepreciationMonthly;
            SET @AccumulatedDepreciationInserir = @AccumulatedDepreciation + @RateDepreciationMonthly;
            SET @CurrentValueInserir = @CurrentValue - @RateDepreciationMonthly;
			SET @CurrentMonthInserir =@CurrentMonth + 1;
			SET @CurrentDateInserir = (SELECT DATEADD(MONTH,1,@CurrentDate));

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
						,[ManagerUnitTransferId]
						,[MonthlyDepreciationId])
			SELECT TOP 1 [AssetStartId]
				   ,[NumberIdentification]
				   ,[ManagerUnitId]
				   ,[MaterialItemCode]
				   ,[InitialId]
				   ,[AcquisitionDate]
				   ,@CurrentDateInserir
				   ,[DateIncorporation]
				   ,[LifeCycle]
				   ,@CurrentMonthInserir
				   ,[ValueAcquisition]
				   ,@CurrentValueInserir
				   ,[ResidualValue]
				   ,@RateDepreciationMonthlyInserir
				   ,@AccumulatedDepreciationInserir
				   ,[UnfoldingValue]
				   ,[Decree]
				   ,[ManagerUnitTransferId]
				   ,[MonthlyDepreciationId]
			FROM [dbo].[MonthlyDepreciation] 
			WHERE [AssetStartId] =@AssetStartId AND [ManagerUnitId] = @ManagerUnitId 
			ORDER BY [Id] DESC

		
		END
	END
	ELSE
		BEGIN
			IF (@AssetIdInservivel IS NULL)
			BEGIN
				SET @TransferenciaCount = (SELECT COUNT(*) FROM [dbo].[AssetMovements] WHERE [AssetTransferenciaId] = @AssetId);
				
				IF(@AssetStartId IS NULL)
				BEGIN
					IF(@TransferenciaCount IS NOT NULL)
					BEGIN 
						SET @_assetStartId = (SELECT [AssetId] FROM [dbo].[AssetMovements] WHERE [AssetTransferenciaId] = @AssetId);;
					END
					ELSE
						BEGIN
							SET @_assetStartId = @AssetId
						END;
                          
					UPDATE [dbo].[Asset] SET [AssetStartId] = @_assetStartId  WHERE [Id] = @AssetId;
                    UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] = @_assetStartId WHERE [AssetId] = @AssetId;
				END
				ELSE
					BEGIN
						SET @_assetStartId = @AssetStartId;
					END;

				EXEC [dbo].[SAM_DEPRECIACAO_UGE] @ManagerUnitId,@MaterialItemCode,@AssetStartId,@DataFinal,0,@Retorno OUT,@Erro OUT
			END

		END

	DELETE FROM @Table WHERE [Id] = @Id
	SET @Contador = @Contador + 1;
END;
GO