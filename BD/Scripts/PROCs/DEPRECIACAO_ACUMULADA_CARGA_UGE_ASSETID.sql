DROP PROCEDURE DEPRECIACAO_ACUMULADA_CARGA_UGE_ASSETID
GO

--CREATE PROCEDURE [dbo].[DEPRECIACAO_ACUMULADA_CARGA_UGE_ASSETID]  --456     
--(
--	@AssetId int,
--	@managerUnitId int
--)            
--AS              
--BEGIN              
                   
--BEGIN TRY              
              
-- Declare @assetIdCurrentLoop int;              
-- Declare @dataAquisicao date;     
-- Declare @contMeses int = 0;    
-- DECLARE @contMesUso int = 0;                    
-- --Declare @managerUnitId int = 0;    
-- Declare @dateFechamentoReferencia datetime;     
    
-- DECLARE @vidaUtil int = 0;    
-- DECLARE @mesesUtilizados int = 0;    
-- DECLARE @valorAquisicao decimal(18,2) = 0;    
-- DECLARE @valorAtual decimal(18,2) = 0;    
-- Declare @valorResidualCalc decimal(18,2);        
-- Declare @depreciacaoMensal decimal(18,10);    
-- DECLARE @depreciacaoAcumulada decimal(18,2) = 0;       
-- Declare @desdobramento decimal(18,2) = 0;  
-- Declare @desdobramentoRestante decimal(18,2) = 0;  
    
-- DECLARE @ResidualValue decimal(18,2) = 0;    
-- DECLARE @AceleratedDepreciation bit = 0;    
-- DECLARE @ManagmentUnit_YearMonthReference varchar(6) = '';    
         
-- DECLARE @EhAcervo bit = 0;       
-- DECLARE @EhTerceiro bit = 0;       
-- DECLARE @FlagDepreciationCompleted bit = 0;
-- DECLARE @EhDecreto bit = 0;        
-- DECLARE @MovementTypeId INT;     


  
--DECLARE @retorno int

--SELECT @retorno = COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS
--WHERE TABLE_NAME = 'Asset' AND  COLUMN_NAME = 'Recalculo'

----SELECT @retorno

--IF (@retorno = 0)
--BEGIN
--      ALTER TABLE Asset ADD Recalculo BIT NULL; 
--END
	  
----Armazena o mês e ano de referência no formato de date            
--     select     
--    @dateFechamentoReferencia = CONVERT(DATE, ManagmentUnit_YearMonthReference  + '01' ,126)             
--     from     
--    ManagerUnit     
--   where     
--    Id = @managerUnitId             
   

-- --Percorre todos os patrimônios ----------------------------------------------------------------------------------------------------------------------------------------              
               
-- DECLARE asset_cursor CURSOR FOR      
-- SELECT Id FROM Asset          
-- where                 
--    flagVerificado IS NULL and               
--    flagDepreciaAcumulada IS NULL and
--	id IN (@AssetId)
--	and  AcquisitionDate < @dateFechamentoReferencia
           
              
-- OPEN asset_cursor;              
-- FETCH NEXT FROM asset_cursor into @assetIdCurrentLoop;              
              
              
-- WHILE @@FETCH_STATUS = 0              
--  BEGIN              
            
-- SET @desdobramento = 0    
-- SET @FlagDepreciationCompleted = 0    
-- SET @EhAcervo = (select flagAcervo from Asset with(nolock) where Id = @assetIdCurrentLoop)    
-- SET @EhTerceiro = (select flagTerceiro from Asset with(nolock) where Id = @assetIdCurrentLoop) 
-- SET @EhDecreto = (select flagDecreto from Asset with(nolock) where Id = @assetIdCurrentLoop)       



	     
--   IF(@EhAcervo = 1 OR @EhTerceiro = 1)    
--    BEGIN    
    
--     --Atualiza os valores dos Patrimônios              
--   update           
--    Asset        
--   SET     
--   -- ValueUpdate = ValueAcquisition,     
    
--    LifeCycle = 0,    
--    RateDepreciationMonthly = 0,    
--    ResidualValue = 0,    
           
--    flagDepreciaAcumulada = 1, --Informa que o Bem não possui pendencia de depreciação acumulada, pois é um Acervo (Não depende de depreciação)    
--    flagVerificado = NULL,    
--    ResidualValueCalc = 0,    
--    monthUsed = 0,    
--    DepreciationByMonth = 0,    
--    DepreciationAccumulated = 0,         
--    ValorDesdobramento = 0,
--	Recalculo =1              
--   where           
--    Id = @assetIdCurrentLoop          
    
--    END    
--   ELSE  IF(@EhDecreto = 1 AND  ((select count(b.MovementTypeId) from Asset a
--									inner join AssetMovements b on b.AssetId = a.Id
--									where AssetTransferenciaId = @assetIdCurrentLoop) = 0))  
--   BEGIN 

--   DECLARE @MESFECHA INT = 0;
--   SET @MESFECHA = (SELECT COUNT(1) FROM Closing WHERE ASSETID = @assetIdCurrentLoop AND ManagerUnitId = @managerUnitId);


--   IF(@MESFECHA > 0)
--   BEGIN
   
--		  SET @valorAtual = (SELECT CurrentPrice FROM Closing WHERE ID =(SELECT MIN(ID) FROM Closing WHERE ASSETID = @assetIdCurrentLoop AND ManagerUnitId = @managerUnitId));
   
--		   --Atualiza os valores dos Patrimônios              
--		   update           
--			Asset        
--		   SET     
--		   DepreciationByMonth = ROUND(DepreciationByMonth,2,1),
--		   DepreciationAccumulated = ROUND(DepreciationByMonth,2,1) * @MESFECHA,
--		   ValueUpdate = @valorAtual - (ROUND(DepreciationByMonth,2,1) * @MESFECHA),
--		   Recalculo =1,
--		   flagDepreciaAcumulada = 1, --Informa que o Bem não possui pendencia de depreciação acumulada, pois é um Acervo (Não depende de depreciação)    
--		   flagVerificado = NULL
--		   where           
--			Id = @assetIdCurrentLoop 

--	END
--	ELSE
--		BEGIN 

--		 --Atualiza os valores dos Patrimônios              
--			   update           
--				Asset        
--			   SET     
--			   DepreciationByMonth = ROUND(DepreciationByMonth,2,1),
--			   Recalculo =1,
--			   flagDepreciaAcumulada = 1, --Informa que o Bem não possui pendencia de depreciação acumulada, pois é um Acervo (Não depende de depreciação)    
--			   flagVerificado = NULL  		
--			   where           
--				Id = @assetIdCurrentLoop 


--		END 



--   END
--   ELSE       
--    BEGIN    
             
--     --Armazena os valores utilizados no cálculo -------------------------------              
--   select           
--     @vidaUtil = a.LifeCycle,      
--     @valorAtual = a.ValueAcquisition,    
--     @valorAquisicao = a.ValueAcquisition,    
--     @valorResidualCalc = a.ValueAcquisition * (a.ResidualValue/100),    
--	 @depreciacaoMensal = (CASE a.LifeCycle WHEN 0 THEN 0 
--                        ELSE ROUND((a.ValueAcquisition - @valorResidualCalc) / a.LifeCycle, 2, 1) END),
--     @depreciacaoAcumulada = a.ValueAcquisition - a.ValueUpdate,    
--     @managerUnitId = ManagerUnitId,        
--     @dataAquisicao = a.AcquisitionDate,    
       
--     @AceleratedDepreciation = a.AceleratedDepreciation,    
--     @ResidualValue = a.ResidualValue,    
    
--     @managerUnitId = a.ManagerUnitId,    
--     @ManagmentUnit_YearMonthReference = m.ManagmentUnit_YearMonthReference     
--     FROM           
--     Asset as a    
--     INNER JOIN     
--     ManagerUnit as m ON a.ManagerUnitId = m.Id              
--     where           
--    a.Id = @assetIdCurrentLoop       
      
       
--     --Armazena o mês e ano de referência no formato de date            
--     select     
--    @dateFechamentoReferencia = CONVERT(DATE, ManagmentUnit_YearMonthReference  + '01' ,126)             
--     from     
--    ManagerUnit     
--   where     
--    Id = @managerUnitId   

--	 --Calcula a quantidade de meses que o bem foi adquirido              
--     SET @mesesUtilizados = DATEDIFF(month, @dataAquisicao, @dateFechamentoReferencia)                                    
       
	

--	Declare @MovimentDate datetime;
--	--DECLARE @dateFechamentoReferencia datetime = '2018-03-01'
--	-- DECLARE @mesesUtilizados int = 0; 
--	--	DECLARE @dataAquisicao date = '2017-12-31' 

--	SELECT @MovimentDate =  A.MovimentDate FROM AssetMovements A
--	INNER JOIN Asset B ON A.AssetId = B.ID
--	WHERE AssetId = @assetIdCurrentLoop-- 3569147
--	AND (B.MovementTypeId = 2 OR B.MovementTypeId = 3 OR B.MovementTypeId = 31 OR B.MovementTypeId = 40 OR B.MovementTypeId = 41)   
	
--	IF ( @MovimentDate IS NOT NULL AND @dateFechamentoReferencia < @MovimentDate)
--	BEGIN
--	--SELECT 'TESTE'
--	 SET @mesesUtilizados = DATEDIFF(month, @dataAquisicao, @MovimentDate)  
----	  SELECT @mesesUtilizados     
--	END   
    

--	SELECT @MovimentDate =  max(A.MovimentDate) FROM AssetMovements A
--	INNER JOIN Asset B ON A.AssetId = B.ID
--	WHERE AssetId = @assetIdCurrentLoop--  2931924 3569147
--    AND (a.MovementTypeId = 11 OR a.MovementTypeId = 12 OR a.MovementTypeId = 47 OR a.MovementTypeId = 56 OR a.MovementTypeId = 57)   
    
--	IF ( @MovimentDate IS NOT NULL AND @dateFechamentoReferencia > @MovimentDate)
--	BEGIN
--	--SELECT 'TESTE'
--	 SET @mesesUtilizados = DATEDIFF(month, @dataAquisicao, @MovimentDate)  
----	  SELECT @mesesUtilizados     
--	END   
    
              
--     --Caso a quantidade de meses do bem patrimonial seja maior que 0, efetua o cálculo              
--     IF(@mesesUtilizados > 0)              
--     BEGIN                
            
--     --Reseta o contador de meses       
--     SET @contMeses = 1;             
                      
--     --Percorre a cada mês da vida útil do bem    
--     WHILE (@contMeses <= @vidaUtil and @contMeses <= @mesesUtilizados)              
--     BEGIN      
       
--    --Seta a quantidade de meses que o bem foi utilizado    
--    SET @contMesUso = @contMeses                            
            
--    --Verifica se a quantidade de depreciações é igual ao ciclo de vida do Bem, para que seja calculado o desdobramento de valor     
--    IF @vidaUtil = @contMesUso    
--    BEGIN    
--     --Calculo do desdobramento    
--     SET @desdobramento =  @depreciacaoMensal - (@valorAtual - @valorResidualCalc)   
--	 SET @desdobramentoRestante =  (@valorAtual - @valorResidualCalc) - @depreciacaoMensal      
    
--     --Flag que indica se a depreciação já atingio seu valor mínimo    
--     SET @FlagDepreciationCompleted = 1    
--    END    
    
--    --Verifica se o valor do bem se encontra com o mesmo valor residual, para parar a depreciação            
--    IF(@valorAtual <= @valorResidualCalc)              
--       BEGIN                     
--     BREAK              
--       END             
            
--    --Subtrai o valor de depreciação do valor do patrimônio              
--    SET @valorAtual = @valorAtual - ROUND(@depreciacaoMensal, 10)        
             
--    --Incrementa 1 mês para comparação              
--    SET @contMeses = @contMeses + 1     
              
--     END              
              
--   END      
    
--   --Seta o valor da depreciação acumulada do bem    
--   SET @depreciacaoAcumulada = @valorAquisicao - (@valorAtual + @desdobramento)                       
     
    
--    --Atualiza os valores dos Patrimônios              
--    update           
--    Asset           
--   SET           
--    ValueUpdate = ROUND(@valorAtual + @desdobramento, 2),     
--    ResidualValueCalc = @valorResidualCalc,    
--    monthUsed = @contMesUso,    
--    DepreciationByMonth = @depreciacaoMensal,    
--    DepreciationAccumulated = @depreciacaoAcumulada,    
--    flagDepreciaAcumulada = 1,          
--    flagVerificado = NULL,    
--    ValorDesdobramento = ROUND(@desdobramentoRestante,2),    
--    flagDepreciationCompleted = @FlagDepreciationCompleted,
--	Recalculo =1      
--   where           
--    Id = @assetIdCurrentLoop 
	

--	SET @MovementTypeId= (select b.MovementTypeId from Asset a
--		inner join AssetMovements b on b.AssetId = a.Id
--		where AssetTransferenciaId = @assetIdCurrentLoop)



--		IF(@movementTypeId = 11 OR @movementTypeId = 12 OR @movementTypeId = 47 OR @movementTypeId = 56 OR @movementTypeId = 57)
--		BEGIN
		
--			declare @monthUsed int;
--			declare @flagDecreto bit;

--			 select 
--			 @depreciacaoAcumulada = DepreciationAccumulated,
--			 @valorAtual =  ValueUpdate,
--			 @monthUsed = monthUsed,
--			 @EhAcervo = flagAcervo,
--			 @EhTerceiro = flagTerceiro,
--			 @flagDecreto = flagDecreto,
--			 @vidaUtil =LifeCycle,
--			 @valorResidualCalc = ResidualValueCalc,
--			 @depreciacaoMensal = DepreciationByMonth,
--			 @desdobramento = ValorDesdobramento
--		from Asset a
--		inner join AssetMovements b on b.AssetId = a.Id
--		where AssetTransferenciaId = @assetIdCurrentLoop

--		DECLARE @MESUTIL INT;

--		SET @MESUTIL =  (SELECT monthUsed FROM ASSET WHERE ID =@assetIdCurrentLoop) - (SELECT A.monthUsed from Asset a inner join AssetMovements b on b.AssetId = a.Id where AssetTransferenciaId = @assetIdCurrentLoop)
		

--			update        
--			Asset         
--			SET	
--			DepreciationAccumulated = @depreciacaoAcumulada + (@depreciacaoMensal * @MESUTIL),
--			monthUsed = @monthUsed + @MESUTIL,
--			ValueUpdate = @valorAtual - (@depreciacaoMensal * @MESUTIL),
--			flagAcervo = @EhAcervo,
--			flagDecreto = @flagDecreto,
--			flagTerceiro = @EhTerceiro,
--			LifeCycle = @vidaUtil,
--			ResidualValueCalc= @valorResidualCalc,
--			DepreciationByMonth = @depreciacaoMensal,
--			ValorDesdobramento = @desdobramento,
--			Recalculo =1,
--		   flagDepreciaAcumulada = 1, --Informa que o Bem não possui pendencia de depreciação acumulada, pois é um Acervo (Não depende de depreciação)    
--		   flagVerificado = NULL  
--			where         
--			Id = @assetIdCurrentLoop 


		
--		END  

--       END    
                        
--   FETCH NEXT FROM asset_cursor into @assetIdCurrentLoop              
--  END;             
              
-- CLOSE asset_cursor              
-- DEALLOCATE asset_cursor              
              
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
