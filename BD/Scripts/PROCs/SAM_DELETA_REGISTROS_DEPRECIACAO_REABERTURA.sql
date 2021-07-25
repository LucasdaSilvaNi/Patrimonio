SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SAM_DELETA_REGISTROS_DEPRECIACAO_REABERTURA]
(
	 @ManagerUnitId INT
) AS
BEGIN
	
	Declare @ClosingYearMonthReference varchar(6) = (select ManagmentUnit_YearMonthReference from ManagerUnit where Id = @managerUnitId);  
  Declare @DataReferencia datetime;  
  Declare @AnoMesReferencia varchar(8);  
       
  
  --Define os nomes a ser reaberto ---------------------------------------------------------------------------------------------------------------------------------------  
  SET @DataReferencia = DATEADD(month, -1, SUBSTRING(@ClosingYearMonthReference, 1, 4) + SUBSTRING(@ClosingYearMonthReference, 5, 2) + '01')  
  SET @AnoMesReferencia = RIGHT('0000' + CONVERT(varchar(4), DATEPART(year, @DataReferencia)), 4) + RIGHT('00' + CONVERT(varchar(2), DATEPART(month, @DataReferencia)), 2)  
  
  SET DATEFORMAT YMD;
  DELETE FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = @managerUnitId AND [CurrentDate] >= @DataReferencia
END


GO

GRANT EXECUTE ON [dbo].[SAM_DELETA_REGISTROS_DEPRECIACAO_REABERTURA] TO [ususamweb]
GO