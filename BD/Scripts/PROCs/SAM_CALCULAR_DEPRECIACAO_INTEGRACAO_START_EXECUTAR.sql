SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'SAM_CALCULAR_DEPRECIACAO_INTEGRACAO_START_EXECUTAR')
BEGIN
    DROP PROCEDURE [dbo].[SAM_CALCULAR_DEPRECIACAO_INTEGRACAO_START_EXECUTAR]
END
GO

CREATE PROCEDURE [dbo].[SAM_CALCULAR_DEPRECIACAO_INTEGRACAO_START_EXECUTAR] ---100112
	@CodigoUge INT,
	@DataFinal AS DATETIME,
	@AssetStartId INT
AS
-- Valores para testes
--SET @CodigoUge = 380273;
--SET @CodigoMaterial = 2409860; 
BEGIN
	SET DATEFORMAT dmy;
	SET LANGUAGE 'Brazilian'

	DECLARE @DataAtual DATETIME;
	DECLARE @DataAquisicao DATETIME;
	DECLARE @Contador SMALLINT = 0;
	DECLARE @ManagerUnitId INT;
	DECLARE @InitialId INT;
	DECLARE @VidaUtil AS SMALLINT;
	DECLARE @VidaUtilInicio AS SMALLINT;
	DECLARE @DepreciacaoAcumulada AS DECIMAL(18,2);
	DECLARE @DepreciacaoMensal AS DECIMAL(18,2);
	DECLARE @ValorResidual AS DECIMAL(18,2);
	DECLARE @PorcetagemResidual AS DECIMAL(18,10);
	DECLARE @ValorAquisicao AS DECIMAL(18,2);
	DECLARE @ValorAtual AS DECIMAL(18,2);
	DECLARE @QuantidadeJaDepreciada AS INT;
	DECLARE @PorcetagemDepreciacaoMensal AS DECIMAL(18,10)
	DECLARE @Desdobro AS DECIMAL(18,10);
	DECLARE @DataDecreto AS DATETIME = NULL;
	DECLARE @ManagerUnitIdTransferencia INT;
	DECLARE @COUNTITEM SMALLINT;
	DECLARE @CONTADORITEM SMALLINT = 0;
	DECLARE @CodigoMaterial INT;
	DECLARE @ManagerUnitTransferId INT = 0;
	DECLARE @CurrentMonth SMALLINT = 0;
	DECLARE @DataIncorporacao DATETIME;
	

	DECLARE @TableTransferencia AS TABLE(
		MovimentDate DATETIME NOT NULL,
		ManagerUnitId INT NOT NULL,
		MaterialItemCode INT NOT NULL,
		AssetStartId INT NOT NULL,
		InitialId	INT NOT NULL
	);

	DECLARE @TableItemMaterial AS TABLE(
		MaterialItemCode INT NOT NULL,
		ManagerUnitId INT NOT NULL,
		AssetStartId INT NOT NULL,
		InitialId	INT NOT NULL
	)

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  

	BEGIN TRY
		SET @ManagerUnitId = (SELECT TOP 1 [Id] FROM [dbo].[ManagerUnit] WHERE [Code] = @CodigoUge);

		INSERT INTO @TableItemMaterial(MaterialItemCode, ManagerUnitId,AssetStartId,InitialId)
		SELECT MIN([ast].[MaterialItemCode]),@ManagerUnitId,[ast].[AssetStartId],MIN([ast].[InitialId]) 
		 FROM [dbo].[Asset] [ast]
		  INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id] 
		 WHERE [ast].[ManagerUnitId] = @ManagerUnitId 
		   AND [ast].[Status] = 1
           AND [flagVerificado] =0
           AND [flagDepreciaAcumulada] IS NULL
		   AND [ast].[AssetStartId] = @AssetStartId
		   AND [ast].[NumberIdentification] ='99999999999'
		 GROUP BY  [ast].[AssetStartId]

		SET @COUNTITEM = (SELECT COUNT(*) FROM @TableItemMaterial);

		WHILE(@CONTADORITEM < @COUNTITEM)
		BEGIN

			SET @CodigoMaterial = (SELECT TOP 1 MaterialItemCode FROM @TableItemMaterial WHERE AssetStartId = @AssetStartId);
			SET @InitialId = (SELECT TOP 1 InitialId FROM @TableItemMaterial WHERE AssetStartId = @AssetStartId AND MaterialItemCode = @CodigoMaterial);

			INSERT INTO @TableTransferencia(MovimentDate,AssetStartId,InitialId,ManagerUnitId,MaterialItemCode)
			 SELECT DISTINCT [ast].[MovimentDate],@AssetStartId,[ast].[InitialId],[mov].[SourceDestiny_ManagerUnitId],@CodigoMaterial
				FROM [dbo].[AssetMovements] [mov] 
				INNER JOIN [dbo].[Asset] [ast] ON [ast].[Id] = [mov].[AssetId] 
				WHERE [mov].[AssetTransferenciaId] =@AssetStartId
				  AND [ast].[MaterialItemCode] = @CodigoMaterial;

			SET @ValorAquisicao = (SELECT TOP 1 [ValueAcquisition] 
										FROM [dbo].[Asset] 
									WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] ASC);

			SET @ValorAtual=  @ValorAquisicao;
			SET @DataAquisicao =(SELECT TOP 1 [AcquisitionDate] 
								   FROM [dbo].[Asset] 
								    WHERE [ManagerUnitId] = @ManagerUnitId
							          AND [AssetStartId]= @AssetStartId
								    ORDER BY [Id] ASC
								);
			SET @DataIncorporacao =(SELECT TOP 1 [MovimentDate] 
							   FROM [dbo].[Asset] 
							  WHERE [AssetStartId] = @AssetStartId
							  ORDER BY [Id] ASC
							);
			IF(@DataIncorporacao < @DataAquisicao)
			BEGIN
				SET @DataAquisicao = @DataIncorporacao;
			END;
			SET @CurrentMonth =(SELECT MAX(CurrentMonth) 
			           FROM [dbo].[MonthlyDepreciation] 
										  WHERE [AssetStartId] = @AssetStartId
										   );

			IF(@CurrentMonth IS NULL)
			BEGIN
				SET @CurrentMonth = 0;
			END;
			SET @DataAtual = (SELECT MovimentDate 
			                    FROM @TableTransferencia 
			                   WHERE [AssetStartId] = @AssetStartId);

			IF(@DataAtual IS NOT NULL)
			BEGIN
				
				IF(@CurrentMonth > 0)
				BEGIN
					SET @VidaUtilInicio = (SELECT TOP 1 [gr].[LifeCycle] 
										   FROM [dbo].[MaterialGroup] [gr]
										   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
										   WHERE [ast].[MaterialItemCode] = @CodigoMaterial)
			
					SET @VidaUtil = @VidaUtilInicio - @CurrentMonth;
					SET @ValorAtual = (SELECT MIN([CurrentValue])
										FROM [dbo].[MonthlyDepreciation] 
									WHERE [AssetStartId] = @AssetStartId);

				END
				ELSE
				   BEGIN
						SET @VidaUtil =(SELECT DATEDIFF(MONTH,@DataAquisicao,@DataAtual));
				   END;
				
				SET @ManagerUnitTransferId = (SELECT ManagerUnitId 
												FROM @TableTransferencia 
											   WHERE [AssetStartId] = @AssetStartId
											     AND [ManagerUnitId] = @ManagerUnitId);
				
			END;
			ELSE
				BEGIN
					SET @VidaUtil = (SELECT TOP 1 [gr].[LifeCycle] 
									   FROM [dbo].[MaterialGroup] [gr]
									   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
									   WHERE [ast].[MaterialItemCode] = @CodigoMaterial)
				END;

		    IF(@CurrentMonth < @VidaUtil)
			BEGIN
				EXEC [dbo].[SAM_CALCULAR_DEPRECIACAO_START]  @CodigoUge,@CodigoMaterial,@InitialId,@AssetStartId,@DataAquisicao,@DataIncorporacao,@ValorAquisicao,@ValorAtual,@VidaUtil,@ManagerUnitTransferId,@DataFinal;
			END;
			PRINT @AssetStartId;
			DELETE FROM @TableItemMaterial WHERE [AssetStartId] =@AssetStartId AND [ManagerUnitId] = @ManagerUnitId;
			SET @CONTADORITEM = @CONTADORITEM + 1;
		END;
		RETURN @CONTADORITEM;
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
GO