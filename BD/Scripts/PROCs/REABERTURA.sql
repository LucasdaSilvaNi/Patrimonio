DROP PROCEDURE REABERTURA
GO

--CREATE PROCEDURE [dbo].[REABERTURA]  
--(  
-- @managerUnitId int,  
-- @loginID int   
--)  
--AS  
--BEGIN  
  
--BEGIN TRANSACTION     
--BEGIN TRY    
    
--  Declare @ClosingYearMonthReference varchar(6) = (select ManagmentUnit_YearMonthReference from ManagerUnit where Id = @managerUnitId);  
--  Declare @DataReferencia datetime;  
--  Declare @AnoMesReferencia varchar(8);  
       
  
--  --Define os nomes a ser reaberto ---------------------------------------------------------------------------------------------------------------------------------------  
--  SET @DataReferencia = DATEADD(month, -1, SUBSTRING(@ClosingYearMonthReference, 1, 4) + SUBSTRING(@ClosingYearMonthReference, 5, 2) + '01')  
--  SET @AnoMesReferencia = RIGHT('0000' + CONVERT(varchar(4), DATEPART(year, @DataReferencia)), 4) + RIGHT('00' + CONVERT(varchar(2), DATEPART(month, @DataReferencia)), 2)   
  
  
--  UPDATE a   
--  SET   
--      --Atualiza a LifeCycle  
--   a.LifeCycle = c.LifeCycle,  
--   --Verifica a necessidade do mês de uso ser regredido  
--   a.monthUsed = c.MonthUsed,  
--   --Atualiza o valor do patrimônio acrescentando o valor, que havia sido depreciado  
--   a.ValueUpdate = c.CurrentPrice,  
--   --Atualiza o valor residual pelo antigo que havia sido gravado na tabela de fechamento  
--   a.ResidualValueCalc = c.ResidualValueCalc,  
--   --Atualiza a depreciação mensal antigo que havia sido gravado na tabela de fechamento  
--   a.DepreciationByMonth = c.DepreciationPrice,  
--   --Retona o valor de depreciação anterior  
--   a.DepreciationAccumulated = c.DepreciationAccumulated,  
--   -- Zera a flag calculo pendente para inativar a notificação na tela
--   a.flagCalculoPendente = NULL,
--   --Caso necessário, zera o valor de desdobramento  
--   a.ValorDesdobramento = CASE   
--          WHEN    
--            a.LifeCycle = a.monthUsed and c.DepreciationPrice = 0  
--          THEN  
--            a.ValorDesdobramento  
--          ELSE   
--            0  
--           END,  
--   a.flagDepreciationCompleted = c.flagDepreciationCompleted  
  
--  FROM Asset as a INNER JOIN Closing as c   
--  ON c.AssetId = a.Id  
--  WHERE   
--  a.ManagerUnitId = @managerUnitId and  
--  a.[Status] = 1 and   
--  a.flagVerificado is NULL and  
--  a.flagDepreciaAcumulada = 1 and  
--  c.ClosingYearMonthReference = @AnoMesReferencia --and  
--  --a.Id = 2484047  
   
--  --Deleta os registros de fechamento antigos, que foram gerados no fechamento que esta sendo reaberto  
--  DELETE c   
--  FROM Closing as c INNER JOIN Asset as a   
--   ON c.AssetId = a.Id  
--  WHERE   
--  a.ManagerUnitId = @managerUnitId and  
--  a.[Status] = 1 and   
--  a.flagVerificado is NULL and  
--  a.flagDepreciaAcumulada = 1 and  
--  c.ClosingYearMonthReference = @AnoMesReferencia    
  
--  --Atualiza o mês de referência do fechamento da UGE     
--  UPDATE ManagerUnit SET ManagmentUnit_YearMonthReference = @AnoMesReferencia where Id = @managerUnitId  
--  SET DATEFORMAT YMD;
--  DELETE FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = @managerUnitId AND [CurrentDate] >= @DataReferencia
--COMMIT TRANSACTION    
--END TRY    
  
--BEGIN CATCH    
-- SELECT      
--         ERROR_PROCEDURE() AS ErrorProcedure      
--        ,ERROR_LINE() AS ErrorLine      
--        ,ERROR_MESSAGE() AS ErrorMessage;     
-- ROLLBACK TRANSACTION    
--END CATCH    
    
--END
