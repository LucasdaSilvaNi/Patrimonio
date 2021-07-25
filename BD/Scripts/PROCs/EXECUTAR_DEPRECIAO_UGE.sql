SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'EXECUTAR_DEPRECIAO_UGE')
BEGIN
    DROP PROCEDURE [dbo].[EXECUTAR_DEPRECIAO_UGE]
END
GO

CREATE PROCEDURE [dbo].[EXECUTAR_DEPRECIAO_UGE](
 @ManagerUnitId INT,
 @mesAnoReferenciaVerificar DATETIME

)
AS
SET DATEFORMAT YMD;

DECLARE @ManagerUnitCount INT =0;
DECLARE @ManagerUnitContador INT = 0;
DECLARE @_Transferencia BIT = 0;
DECLARE @TransferenciaId INT = NULL;
DECLARE @depreciacaoAssetId INT = NULL;
DECLARE @_assetStartId INT = NULL;
DECLARE @DepreciacaoMaterialItemCode INT = NULL;
DECLARE @transferenciaManagerUnitId INT = NULL;
DECLARE @Retorno NVARCHAR(1000)= NULL;
DECLARE @Erro BIT =  0;

DECLARE @MonthlyDepreciationCount INT = 0;
DECLARE @MonthlyDepreciationContador INT= 0;
DECLARE @MonthlyDepreciationId INT = 0;
DECLARE @MonthlyDepreciationManagerUnitId INT = 0;

DECLARE @MonthlyDepreciation AS TABLE(
	Id INT NOT NULL,
	ManagerUnitId INT NULL

)

DECLARE @TableManagerUnit AS TABLE(
	MaterialItemCode INT NOT NULL,
    AssetStartId INT,
	AssetId INT NOT NULL 
)
INSERT INTO @TableManagerUnit(MaterialItemCode,AssetStartId,AssetId)
SELECT DISTINCT a.MaterialItemCode,
       a.AssetStartId,
	   a.Id 
 FROM [dbo].[Asset] a
INNER JOIN [dbo].[AssetMovements] [m] ON [a].[Id] = [m].[AssetId]
WHERE a.MaterialGroupCode <> 0
   AND a.ManagerUnitId = @ManagerUnitId
   AND a.flagVerificado IS NULL
   AND a.flagDepreciaAcumulada = 1
   AND (a.flagAcervo IS NULL OR a.flagAcervo = 0)
   AND (a.flagTerceiro IS NULL OR a.flagTerceiro = 0)
   AND (m.FlagEstorno IS NULL OR m.FlagEstorno = 0)
   ---AND (m.AssetTransferenciaId IS NULL)

SET @ManagerUnitCount = (SELECT COUNT(*) FROM @TableManagerUnit)


WHILE(@ManagerUnitContador <= @ManagerUnitCount)
BEGIN
	SET @depreciacaoAssetId = (SELECT TOP 1 [AssetId] FROM @TableManagerUnit)
	SET @DepreciacaoMaterialItemCode = (SELECT TOP 1 [MaterialItemCode] FROM @TableManagerUnit WHERE [AssetId] = @depreciacaoAssetId)

    SET @TransferenciaId =(SELECT TOP 1 [t].[MonthlyDepreciationId]  FROM [dbo].[AssetMovements] [t]  
	                        WHERE t.AssetTransferenciaId = @depreciacaoAssetId ORDER BY [t].[Id] DESC)
	
	IF(@TransferenciaId IS NOT NULL)
	BEGIN
		SET @_Transferencia = 1;
		SET @_assetStartId = @TransferenciaId;
		
		UPDATE [dbo].[Asset] SET [AssetStartId] =  @_assetStartId  WHERE [Id] = @TransferenciaId;

        UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] = @_assetStartId WHERE [AssetId] = @TransferenciaId;

        UPDATE [dbo].[Asset] SET [AssetStartId] =  @_assetStartId  WHERE [Id] =  @depreciacaoAssetId;

        UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] = @_assetStartId  WHERE [AssetId] = @depreciacaoAssetId;

	


	END;
	ELSE
		BEGIN
			UPDATE [dbo].[Asset] SET [AssetStartId] = @depreciacaoAssetId WHERE [Id] =  @depreciacaoAssetId;
            UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] = @depreciacaoAssetId WHERE [AssetId] = @depreciacaoAssetId;

            SET @_assetStartId = @depreciacaoAssetId;

		END;

	

          DELETE FROM [dbo].[MonthlyDepreciation] WHERE [MaterialItemCode] = @DepreciacaoMaterialItemCode AND [AssetStartId] = @_assetStartId;

         EXEC [dbo].[SAM_DEPRECIACAO_UGE] @ManagerUnitId= @ManagerUnitId
										 ,@MaterialItemCode = @DepreciacaoMaterialItemCode
										 ,@AssetStartId = @_assetStartId 
										 ,@DataFinal = @mesAnoReferenciaVerificar 
										 ,@Fechamento = 0
										 ,@Retorno = @Retorno
										 ,@Erro =@Erro;
           
		

	DELETE FROM @TableManagerUnit WHERE [AssetId] = @depreciacaoAssetId

	SET @ManagerUnitContador = @ManagerUnitContador + 1;

END
GO