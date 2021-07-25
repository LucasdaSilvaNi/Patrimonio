SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'DEPRECIACAO_ACUMULADA_CARGA')
	DROP PROCEDURE [dbo].[DEPRECIACAO_ACUMULADA_CARGA]; 
GO

     
CREATE PROCEDURE [dbo].[DEPRECIACAO_ACUMULADA_CARGA]       
(
	@OrgaoId int
)            
AS              
BEGIN              
                   
BEGIN TRY              
              
 Declare @assetIdCurrentLoop int;              
 Declare @dataAquisicao date;     
 Declare @contMeses int = 0;    
 DECLARE @contMesUso int = 0;                    
 Declare @managerUnitId int = 0;    
 Declare @dateFechamentoReferencia datetime;     
    
 DECLARE @vidaUtil int = 0;    
 DECLARE @mesesUtilizados int = 0;    
 DECLARE @valorAquisicao decimal(18,2) = 0;    
 DECLARE @valorAtual decimal(18,2) = 0;    
 Declare @valorResidualCalc decimal(18,2);        
 Declare @depreciacaoMensal decimal(18,10);    
 DECLARE @depreciacaoAcumulada decimal(18,2) = 0;       
 Declare @desdobramento decimal(18,2) = 0; 
 Declare @desdobramentoRestante decimal(18,2) = 0;    
    
 DECLARE @ResidualValue decimal(18,2) = 0;    
 DECLARE @AceleratedDepreciation bit = 0;    
 DECLARE @ManagmentUnit_YearMonthReference varchar(6) = '';    
         
 DECLARE @EhAcervo bit = 0;       
 DECLARE @EhTerceiro bit = 0;       
 DECLARE @FlagDepreciationCompleted bit = 0;       
             
 --Percorre todos os patrimônios ----------------------------------------------------------------------------------------------------------------------------------------              
               
 DECLARE asset_cursor CURSOR FOR      
 SELECT TOP 200 Id FROM Asset          
 where                 
    flagVerificado IS NULL and               
    flagDepreciaAcumulada IS NULL and
	id IN (select AssetId from AssetMovements where InstitutionId = @OrgaoId)
           
              
 OPEN asset_cursor;              
 FETCH NEXT FROM asset_cursor into @assetIdCurrentLoop;              
              
              
 WHILE @@FETCH_STATUS = 0              
  BEGIN              
            
 SET @desdobramento = 0    
 SET @FlagDepreciationCompleted = 0    
 SET @EhAcervo = (select flagAcervo from Asset with(nolock) where Id = @assetIdCurrentLoop)    
 SET @EhTerceiro = (select flagTerceiro from Asset with(nolock) where Id = @assetIdCurrentLoop)    
     
     
   IF(@EhAcervo = 1 OR @EhTerceiro = 1)    
    BEGIN    
    
     --Atualiza os valores dos Patrimônios              
   update           
    Asset        
   SET     
    ValueUpdate = ValueAcquisition,     
    
    LifeCycle = 0,    
    RateDepreciationMonthly = 0,    
    ResidualValue = 0,    
           
    flagDepreciaAcumulada = 1, --Informa que o Bem não possui pendencia de depreciação acumulada, pois é um Acervo (Não depende de depreciação)    
    flagVerificado = NULL,    
    ResidualValueCalc = 0,    
    monthUsed = 0,    
    DepreciationByMonth = 0,    
    DepreciationAccumulated = 0,         
    ValorDesdobramento = 0            
   where           
    Id = @assetIdCurrentLoop          
    
    END    
   ELSE    
    BEGIN    
             
     --Armazena os valores utilizados no cálculo -------------------------------              
   select           
     @vidaUtil = a.LifeCycle,      
     @valorAtual = a.ValueAcquisition,    
     @valorAquisicao = a.ValueAcquisition,    
     @valorResidualCalc = a.ValueAcquisition * (a.ResidualValue/100),    
     @depreciacaoMensal =  (CASE a.LifeCycle WHEN 0 THEN 0 
                        ELSE ROUND((a.ValueAcquisition - @valorResidualCalc) / a.LifeCycle, 2, 1) END),
   
     @depreciacaoAcumulada = a.ValueAcquisition - a.ValueUpdate,    
     @managerUnitId = ManagerUnitId,        
     @dataAquisicao = a.AcquisitionDate,    
       
     @AceleratedDepreciation = a.AceleratedDepreciation,    
     @ResidualValue = a.ResidualValue,    
    
     @managerUnitId = a.ManagerUnitId,    
     @ManagmentUnit_YearMonthReference = m.ManagmentUnit_YearMonthReference     
     FROM           
     Asset as a    
     INNER JOIN     
     ManagerUnit as m ON a.ManagerUnitId = m.Id              
     where           
    a.Id = @assetIdCurrentLoop       
      
       
     --Armazena o mês e ano de referência no formato de date            
     select     
    @dateFechamentoReferencia = CONVERT(DATE, ManagmentUnit_YearMonthReference  + '01' ,126)             
     from     
    ManagerUnit     
   where     
    Id = @managerUnitId             
    
    
     --Calcula a quantidade de meses que o bem foi adquirido              
     SET @mesesUtilizados = DATEDIFF(month, @dataAquisicao, @dateFechamentoReferencia)                                    
       
              
     --Caso a quantidade de meses do bem patrimonial seja maior que 0, efetua o cálculo              
     IF(@mesesUtilizados > 0)              
     BEGIN                
            
     --Reseta o contador de meses       
     SET @contMeses = 1;             
                      
     --Percorre a cada mês da vida útil do bem    
     WHILE (@contMeses <= @vidaUtil and @contMeses <= @mesesUtilizados)              
     BEGIN      
       
    --Seta a quantidade de meses que o bem foi utilizado    
    SET @contMesUso = @contMeses                            
            
    --Verifica se a quantidade de depreciações é igual ao ciclo de vida do Bem, para que seja calculado o desdobramento de valor     
    IF @vidaUtil = @contMesUso    
    BEGIN    
     --Calculo do desdobramento    
     SET @desdobramento =  @depreciacaoMensal - (@valorAtual - @valorResidualCalc)    
	  SET @desdobramentoRestante =  (@valorAtual - @valorResidualCalc) - @depreciacaoMensal
    
     --Flag que indica se a depreciação já atingio seu valor mínimo    
     SET @FlagDepreciationCompleted = 1    
    END    
    
    --Verifica se o valor do bem se encontra com o mesmo valor residual, para parar a depreciação            
    IF(@valorAtual <= @valorResidualCalc)              
       BEGIN                     
     BREAK              
       END             
            
    --Subtrai o valor de depreciação do valor do patrimônio              
    SET @valorAtual = @valorAtual - ROUND(@depreciacaoMensal, 10)        
             
    --Incrementa 1 mês para comparação              
    SET @contMeses = @contMeses + 1     
              
     END              
              
   END      
    
   --Seta o valor da depreciação acumulada do bem    
   SET @depreciacaoAcumulada = @valorAquisicao - (@valorAtual + @desdobramento)                       
     
    
    --Atualiza os valores dos Patrimônios              
    update           
    Asset           
   SET           
    ValueUpdate = ROUND(@valorAtual + @desdobramento, 2),     
    ResidualValueCalc = @valorResidualCalc,    
    monthUsed = @contMesUso,    
    DepreciationByMonth = @depreciacaoMensal,    
    DepreciationAccumulated = @depreciacaoAcumulada,    
    flagDepreciaAcumulada = 1,          
    flagVerificado = NULL,    
    ValorDesdobramento = ROUND(@desdobramentoRestante,2),    
    flagDepreciationCompleted = @FlagDepreciationCompleted    
   where           
    Id = @assetIdCurrentLoop            
         
       END    
                        
   FETCH NEXT FROM asset_cursor into @assetIdCurrentLoop              
  END;             
              
 CLOSE asset_cursor              
 DEALLOCATE asset_cursor              
              
 -----------------------------------------------------------------------------------------------------------------------------------------------------------------------              
                      
END TRY                    
                    
BEGIN CATCH                         
 SELECT             
   AssetId = @assetIdCurrentLoop        
        ,ERROR_PROCEDURE() AS ErrorProcedure                      
        ,ERROR_LINE() AS ErrorLine                      
        ,ERROR_MESSAGE() AS ErrorMessage;                               
END CATCH                      
END

GO

GRANT EXECUTE ON [dbo].[DEPRECIACAO_ACUMULADA_CARGA] TO [ususamweb]
GO