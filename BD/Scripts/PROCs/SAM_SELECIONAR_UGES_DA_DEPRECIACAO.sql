DROP PROCEDURE SAM_SELECIONAR_UGES_DA_DEPRECIACAO
GO

--CREATE PROCEDURE [dbo].[SAM_SELECIONAR_UGES_DA_DEPRECIACAO]
--	@InstitutionId INT,
--	@ManagerUnitId INT = NULL,
--	@DataFinal DATETIME
--AS
--BEGIN

--SET DATEFORMAT DMY;
--DECLARE @ManagerUnitCode INT;
--DECLARE @Contador SMALLINT = 0;
--DECLARE @CountManagerUnit SMALLINT;
--DECLARE	@return_value int
----DECLARE @DataFinal AS DATETIME ='01-05-2019';
--DECLARE @ManageUnits AS TABLE(
--	ManagerUnitCode INT
--)
--IF @ManagerUnitId IS NULL
--BEGIN
--	INSERT INTO @ManageUnits(ManagerUnitCode) 
--	SELECT DISTINCT [man].[Code] FROM [dbo].[ManagerUnit] [man] WITH(NOLOCK)
--	INNER JOIN [dbo].[Asset] [ast] WITH(NOLOCK) ON [ast].[ManagerUnitId] = [man].[Id]
--	INNER JOIN [dbo].[BudgetUnit] [bug] WITH(NOLOCK) ON [bug].[Id] = [man].[BudgetUnitId] 
--	WHERE [bug].[InstitutionId] = @InstitutionId
--END
--ELSE
--	BEGIN
--	INSERT INTO @ManageUnits(ManagerUnitCode) 
--		SELECT DISTINCT [man].[Code] FROM [dbo].[ManagerUnit] [man] WITH(NOLOCK)
--		INNER JOIN [dbo].[Asset] [ast] WITH(NOLOCK) ON [ast].[ManagerUnitId] = [man].[Id]
--		INNER JOIN [dbo].[BudgetUnit] [bug] WITH(NOLOCK) ON [bug].[Id] = [man].[BudgetUnitId] 
--		WHERE [bug].[InstitutionId] = @InstitutionId
--		  AND [man].[Id] = @ManagerUnitId
--	END;


--	SET @CountManagerUnit =  (SELECT COUNT(*) FROM @ManageUnits)

--	WHILE(@Contador < @CountManagerUnit)
--	BEGIN
--		SET @ManagerUnitCode = (SELECT TOP 1 ManagerUnitCode FROM @ManageUnits);
--		SELECT @ManagerUnitCode
--		EXEC	@return_value = [dbo].[SAM_CALCULAR_DEPRECIACAO_START_EXECUTAR]
--				@CodigoUge = @ManagerUnitCode,
--				@DataFinal = @DataFinal
--		DELETE @ManageUnits WHERE ManagerUnitCode = @ManagerUnitCode;

--		SELECT	'Return Value' = @return_value
--		SET @Contador = @Contador + 1;
--	END;

--END;