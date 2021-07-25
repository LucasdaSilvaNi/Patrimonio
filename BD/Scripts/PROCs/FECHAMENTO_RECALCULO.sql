DROP PROCEDURE FECHAMENTO_RECALCULO
GO

--CREATE PROCEDURE [dbo].[FECHAMENTO_RECALCULO]  --456     
--(
--	 @CodManagerUnit int
--)            
--AS              
--BEGIN              
                   
--BEGIN TRY              
              
--  Declare @assetIdCurrentLoop int;        
--  DECLARE @ClosingYearMonthReference varchar(6);
--  DECLARE @CurrentPrice DECIMAL(18,2);
--  DECLARE @DepreciationPrice DECIMAL(18,10);
--  DECLARE @DepreciationPriceGravar DECIMAL(18,10);
--  DECLARE @MonthUsed INT;
--  DECLARE @ResidualValueCalc DECIMAL(18,2);
--  DECLARE @DepreciationAccumulated DECIMAL(18,2);
--  DECLARE @EhTerceiro bit = 0;       
--  DECLARE @EhAcervo bit = 0; 
      
	  
--DECLARE @retorno int

--SELECT @retorno = COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS
--WHERE TABLE_NAME = 'Closing' AND  COLUMN_NAME = 'Recalculo'

----SELECT @retorno

--IF (@retorno = 0)
--BEGIN
--      ALTER TABLE Closing ADD Recalculo BIT NULL; 
--END

	 
	   
      
--DECLARE @ManagerUnitId int = (select id from ManagerUnit where Code = @CodManagerUnit and Status = 1)   

-- --Percorre todos os patrimônios ----------------------------------------------------------------------------------------------------------------------------------------              
               
-- DECLARE asset_cursorfechamento CURSOR FOR      
-- SELECT distinct AssetId FROM Closing
-- where Status = 1 and ManagerUnitId = @managerUnitId         
  
           
              
-- OPEN asset_cursorfechamento;              
-- FETCH NEXT FROM asset_cursorfechamento into @assetIdCurrentLoop;              
  
  
 
              
-- WHILE @@FETCH_STATUS = 0              
--  BEGIN
  
  
--  SET @EhAcervo = (select flagAcervo from Asset with(nolock) where Id = @assetIdCurrentLoop)    
--  SET @EhTerceiro = (select flagTerceiro from Asset with(nolock) where Id = @assetIdCurrentLoop)   
  
  
--    IF(@EhAcervo = 1 OR @EhTerceiro = 1)    
--    BEGIN 

--	UPDATE Closing
--	SET MonthUsed = 0
--	WHERE AssetId = @assetIdCurrentLoop
	
--	END 
--	ELSE
--	BEGIN      
  
  
--  select * into #temp from (select max(a.Id) id, AssetId   from [Closing] a 
--  inner join Asset b on b.Id = a.AssetId
--  where a.status = 1 and AssetId = @assetIdCurrentLoop
--  group by  AssetId) tb
    

--  select ROW_NUMBER() OVER(ORDER BY id desc) AS Contador, Id, AssetId,ClosingYearMonthReference,CurrentPrice,DepreciationPrice,MonthUsed, ResidualValueCalc,DepreciationAccumulated
--  into #tempClosing from Closing
--  where AssetId =@assetIdCurrentLoop
--  order by id desc

--  DECLARE @Counter INT = 1;
--  DECLARE @TotalLoops INT = (select count(1) from #tempClosing)
                  
            
-- WHILE (@Counter <= @TotalLoops)
--	BEGIN

--		IF(@Counter = 1)
--		BEGIN

--		IF((SELECT COUNT(1) from Asset a 
--			  inner join Closing b on b.AssetId = a.id
--			  inner join #temp c on c.id = b.Id
--			  inner join ManagerUnit d on d.Id = a.ManagerUnitId
--			  where a.monthUsed = b.MonthUsed) > 0)
--			  BEGIN
--					  select @CurrentPrice = a.ValueUpdate, @DepreciationPriceGravar = 0,
--					  @DepreciationPrice = a.DepreciationByMonth , @MonthUsed = a.monthUsed, @ResidualValueCalc = a.ResidualValueCalc , 
--					  @DepreciationAccumulated = a.DepreciationAccumulated from Asset a 
--					  inner join Closing b on b.AssetId = a.id
--					  inner join #temp c on c.id = b.Id
--					  inner join ManagerUnit d on d.Id = a.ManagerUnitId

--			  END
--		ELSE
--			  BEGIN 		  


--						select @ClosingYearMonthReference= d.ManagmentUnit_YearMonthReference - 1 , @CurrentPrice = a.ValueUpdate + a.DepreciationByMonth,
--						@DepreciationPriceGravar = a.DepreciationByMonth, @DepreciationPrice = a.DepreciationByMonth , @MonthUsed = a.monthUsed - 1 , @ResidualValueCalc = a.ResidualValueCalc , 
--					  @DepreciationAccumulated = a.DepreciationAccumulated - a.DepreciationByMonth   from Asset a 
--					  inner join Closing b on b.AssetId = a.id
--					  inner join #temp c on c.id = b.Id
--					  inner join ManagerUnit d on d.Id = a.ManagerUnitId
--			END

--		update #tempClosing
--		SET --ClosingYearMonthReference = @ClosingYearMonthReference,
--		CurrentPrice = @CurrentPrice,
--		DepreciationPrice = @DepreciationPriceGravar,
--		ResidualValueCalc =@ResidualValueCalc,
--		DepreciationAccumulated = @DepreciationAccumulated,
--		MonthUsed = @MonthUsed
--		where Contador = 1
		


--		END
--		ELSE
--		BEGIN

--		IF((SELECT COUNT(1) from Closing a
--			inner join #tempClosing b on b.Id = a.Id and b.MonthUsed = @MonthUsed and Contador = @Counter) > 0)

--		BEGIN
--				set @DepreciationPriceGravar =  0
--				set @CurrentPrice = @CurrentPrice;
--				set @MonthUsed = @MonthUsed;
--				set @DepreciationAccumulated = @DepreciationAccumulated;

--		END
--		ELSE

--		BEGIN
--				set @DepreciationPriceGravar = @DepreciationPrice;
--				set @CurrentPrice = @CurrentPrice + @DepreciationPrice;
--				set @MonthUsed = @MonthUsed - 1;
--				set @DepreciationAccumulated = @DepreciationAccumulated - @DepreciationPrice

--		END

--		update #tempClosing
--		SET --ClosingYearMonthReference = @ClosingYearMonthReference,
--		CurrentPrice = @CurrentPrice,
--		DepreciationPrice = @DepreciationPriceGravar,
--		ResidualValueCalc =@ResidualValueCalc,
--		DepreciationAccumulated = @DepreciationAccumulated,
--		MonthUsed = @MonthUsed
--		where Contador = @Counter

--		END

		
--		SET @Counter = @Counter+1
--	END

--	update Closing
--	SET CurrentPrice = b.CurrentPrice,
--		DepreciationPrice = b.DepreciationPrice,
--		ResidualValueCalc =b.ResidualValueCalc,
--		DepreciationAccumulated = b.DepreciationAccumulated,
--		MonthUsed = b.MonthUsed,
--		Recalculo = 1
--	from Closing a
--	inner join #tempClosing b on b.Id = a.Id


--	drop table #temp
--	drop table #tempClosing

--	END
     
--   FETCH NEXT FROM asset_cursorfechamento into @assetIdCurrentLoop              
--  END;             
              
-- CLOSE asset_cursorfechamento              
-- DEALLOCATE asset_cursorfechamento              
              
-- -----------------------------------------------------------------------------------------------------------------------------------------------------------------------              
                      
--END TRY                    
                    
--BEGIN CATCH                         
-- SELECT             
--   AssetId = @assetIdCurrentLoop        
--        ,ERROR_PROCEDURE() AS ErrorProcedure                      
--        ,ERROR_LINE() AS ErrorLine                      
--        ,ERROR_MESSAGE() AS ErrorMessage;                               
--END CATCH                      
--END
