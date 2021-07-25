/****** Object:  StoredProcedure [dbo].[REPORT_BALANCE_UA]    Script Date: 25/05/2020 18:58:49 ******/
DROP PROCEDURE [dbo].[REPORT_BALANCE_UA]
GO


--CREATE PROCEDURE [dbo].[REPORT_BALANCE_UA]
--	-- Add the parameters for the stored procedure here
--	@id AS INT
--AS
--BEGIN
--	-- SET NOCOUNT ON added to prevent extra result sets from
--	-- interfering with SELECT statements.
--	SET NOCOUNT ON;

--    -- Insert statements for procedure here
--	SELECT 
--	CONVERT(varchar(10), AdministrativeUnit.Code) + ' - ' + AdministrativeUnit.Description AS UADescription	 
--	, CONVERT(varchar(10), managerUnit.Code) + ' - ' + ManagerUnit.Description AS UGEDescription
--	, Balance.RefMonthBalance 
--	, CONVERT(varchar(10), MaterialItem.Code) + ' - ' + MaterialItem.Description AS ItemMaterialDescription
--	, BalanceDetails.ValueAssetIncorporated
--	, BalanceDetails.ValueAssetOut
--	, BalanceDetails.AmountAssetIncorporated
--	, BalanceDetails.AmountAssetOut
--	, BalanceDetails.ValueMaterialItemBalance
--	, BalanceDetails.AmountMaterialItemBalance
--	, Balance.ValueIncorporationsBalance
--	, Balance.ValueOutsBalance
--	, Balance.AmountIncorporationsBalance
--	, Balance.AmountOutsBalance
--	FROM Balance 
--		INNER JOIN BalanceDetails	ON Balance.Id = BalanceDetails.BalanceId
--		INNER JOIN AdministrativeUnit ON Balance.AdministrativeUnitId = AdministrativeUnit.Id
--		INNER JOIN ManagerUnit ON Balance.ManagerUnitId = ManagerUnit.Id
--		INNER JOIN MaterialItem ON BalanceDetails.MaterialItemId = MaterialItem.Id
--	WHERE Balance.Id = @id
--END
--GO