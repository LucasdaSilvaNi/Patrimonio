DROP PROCEDURE DEPRECIACAO_ORGAO
GO

--CREATE PROCEDURE [dbo].[DEPRECIACAO_ORGAO](
--	@InstitutionId INT,
--	@DataReferencia DATETIME,
--	@DeletarDepreciacao BIT = 0
--)
--AS

--DECLARE @TableManagerUnit AS TABLE(ManagerCode INT,ManagerUnitId INT)
--DECLARE @TableAssetStartId AS TABLE(AssetStartId INT);

--DECLARE @ContadorManagerUnit AS INT = 0;
--DECLARE @CountManagerUnit AS INT = 0;
--DECLARE @ManagerUnitId INT;
--DECLARE @ManagerUnitCode INT;


--SET DATEFORMAT YMD;

--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  

--INSERT INTO @TableManagerUnit(ManagerCode,ManagerUnitId)
--SELECT DISTINCT [man].[Code], [man].[Id] FROM BudgetUnit [bud] WITH(NOLOCK)
--INNER JOIN [ManagerUnit] [man] WITH(NOLOCK) ON [bud].[Id] = [man].[BudgetUnitId]
--WHERE [bud].[InstitutionId] = @InstitutionId


--SET @CountManagerUnit = (SELECT  COUNT(*) FROM @TableManagerUnit);

--WHILE(@ContadorManagerUnit <= @CountManagerUnit)
--BEGIN

	

--	SET @ManagerUnitId = (SELECT TOP 1 ManagerUnitId FROM @TableManagerUnit);
--	SET @ManagerUnitCode = (SELECT TOP 1 ManagerCode FROM @TableManagerUnit WHERE ManagerUnitId = @ManagerUnitId);
	
--	IF(@DeletarDepreciacao = 1)
--	BEGIN
--		DELETE MonthlyDepreciation WHERE [ManagerUnitId] =@ManagerUnitId
--	END;


--	DECLARE @Table AS TABLE(
--		MaterialItemCode INT
--	)

--	INSERT INTO @Table(MaterialItemCode)
--	SELECT DISTINCT [MaterialItemCode] FROM [Asset] WHERE [ManagerUnitId] =  @ManagerUnitId
	
--	DECLARE @CONTADOR INT = 0;
--	DECLARE @COUNT INT;
--	DECLARE @MaterialItemCode INT;

--	SET @COUNT =(SELECT COUNT(*) FROM @Table);

--	WHILE(@CONTADOR <= @COUNT)
--	BEGIN
--		SET @MaterialItemCode =(SELECT TOP 1 MaterialItemCode FROM @Table);

--		EXEC  [dbo].[SAM_CALCULAR_DEPRECIACAO_START_EXECUTAR_FECHAMENTO] ---100112
--		@CodigoUge =@ManagerUnitCode,
--		@DataFinal = @DataReferencia,
--		@MaterialItemCode =@MaterialItemCode
--		DELETE @Table WHERE MaterialItemCode = @MaterialItemCode; 
--		SET @CONTADOR = @CONTADOR + 1;
--	END;


--	DELETE FROM @TableManagerUnit WHERE [ManagerUnitId] = @ManagerUnitId;
--	SET @ContadorManagerUnit = @ContadorManagerUnit + 1;
--END;
