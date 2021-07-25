SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'DEPRECIACAO_ACUMULADA_UNITARIO')
	DROP PROCEDURE [dbo].[DEPRECIACAO_ACUMULADA_UNITARIO]; 
GO

CREATE PROCEDURE [dbo].[DEPRECIACAO_ACUMULADA_UNITARIO] -- 3768633,'13928683861',0,'74556', 40
(        
 @assetId int,  
 @login varchar(12),  
 @valueUpdate decimal(18,2) = 0,
 @flagDepreciaPelaDataIncorporacao bit = 1,
 @movementTypeId int = null
)           
AS            
BEGIN            
          
BEGIN TRANSACTION             
BEGIN TRY            
            
 Declare @assetIdCurrentLoop int;            
 Declare @dataAquisicao date;                    
 Declare @contMeses int = 0;    
 DECLARE @contMesUso int = 0;          
 Declare @managerUnitId int = null;          
 Declare @dateFechamentoReferencia datetime;          
            
   
 DECLARE @vidaUtil int = 0;  
 DECLARE @mesesUtilizados int = 0;  
 DECLARE @valorAquisicao decimal(18,2) = 0;  
 DECLARE @valorAtual decimal(18,2) = 0;  
 DECLARE @valorResidualCalc decimal(18,2) = 0;  
 DECLARE @depreciacaoMensal decimal(18,10) = 0;   
 DECLARE @depreciacaoAcumulada decimal(18,2) = 0;    
 Declare @desdobramento decimal(18,10) = 0;
 Declare @desdobramentoRestante decimal(18,10) = 0;    
  
 DECLARE @ResidualValue decimal(18,2) = 0;  
 DECLARE @AceleratedDepreciation bit = 0;  
 DECLARE @ManagmentUnit_YearMonthReference varchar(6) = '';  
  
 DECLARE @FlagDepreciationCompleted bit = 0;     
  
 DECLARE @EhAcervo bit = ((select flagAcervo from Asset with(nolock) where Id = @assetId));  
 DECLARE @EhTerceiro bit = ((select flagTerceiro from Asset  with(nolock) where Id = @assetId));  
 
      
   IF(@EhAcervo = 1 OR @EhTerceiro = 1)  
    BEGIN  
  
     --Atualiza os valores dos Patrim�nios            
   update         
    Asset      
   SET   
    ValueUpdate = @valueUpdate,   
  
    LifeCycle = 0,  
    RateDepreciationMonthly = 0,  
    ResidualValue = 0,  
         
    flagDepreciaAcumulada = 1, --Informa que o Bem n�o possui pendencia de deprecia��o acumulada, pois � um Acervo (N�o depende de deprecia��o)  
    flagVerificado = NULL,  
    ResidualValueCalc = 0,  
    monthUsed = 0,  
    DepreciationByMonth = 0,  
    DepreciationAccumulated = 0,       
    ValorDesdobramento = 0          
   where         
    Id = @assetId and        
    flagDepreciaAcumulada IS NULL           
  
    END  
   ELSE  
    BEGIN  
                 
     --Armazena os valores utilizados no c�lculo -------------------------------            
     select         
     @vidaUtil = LifeCycle,    
     @valorAtual = ValueAcquisition,  
     @valorAquisicao = a.ValueAcquisition,  
     @valorResidualCalc = ValueAcquisition * (ResidualValue/100),  
	 @depreciacaoMensal = (CASE a.LifeCycle WHEN 0 THEN 0 
                        ELSE ROUND((a.ValueAcquisition - @valorResidualCalc) / a.LifeCycle, 2, 1) END),
     @depreciacaoAcumulada = ValueAcquisition - ValueUpdate,  
     @managerUnitId = ManagerUnitId,    
     @dataAquisicao = (case when @flagDepreciaPelaDataIncorporacao = 1 then AcquisitionDate else MovimentDate end),  
     
     @AceleratedDepreciation = AceleratedDepreciation,  
     @ResidualValue = ResidualValue,  
  
     @managerUnitId = a.ManagerUnitId,  
     @ManagmentUnit_YearMonthReference = m.ManagmentUnit_YearMonthReference   
     FROM         
     Asset as a  
     INNER JOIN   
     ManagerUnit as m ON a.ManagerUnitId = m.Id                    
     where         
    a.Id = @assetId    AND         
    a.flagDepreciaAcumulada IS NULL           
  
  
     --Armazena o m�s e ano de refer�ncia no formato de date          
   select         
    @dateFechamentoReferencia = CONVERT(DATE,  ManagmentUnit_YearMonthReference  + '01' ,126)           
   from         
    ManagerUnit         
   where         
    Id = @managerUnitId          
          
     --Calcula a quantidade de meses que o bem foi adquirido            
     SET @mesesUtilizados = DATEDIFF(month, @dataAquisicao, @dateFechamentoReferencia)            
                 
      
     --Caso a quantidade de meses do bem patrimonial seja maior que 0, efetua o c�lculo            
     IF(@mesesUtilizados > 0)            
   BEGIN              
           
     --Reseta o contador de meses             
     SET @contMeses = 1;     
             
     --Percorre a cada m�s da vida �til do bem        
     WHILE (@contMeses <= @vidaUtil and @contMeses <= @mesesUtilizados)        
     BEGIN  
  
    --Seta a quantidade de meses que o bem foi utilizado  
    SET @contMesUso = @contMeses  
  
    --Verifica se a quantidade de deprecia��es � igual ao ciclo de vida do Bem, para que seja calculado o desdobramento de valor   
    IF @vidaUtil = @contMesUso  
    BEGIN  
     --Calculo do desdobramento  
     SET @desdobramento =  @depreciacaoMensal - (@valorAtual - @valorResidualCalc)  
	 SET @desdobramentoRestante =  (@valorAtual - @valorResidualCalc) - @depreciacaoMensal  
  
     --Flag que indica se a deprecia��o j� atingio seu valor m�nimo  
     SET @FlagDepreciationCompleted = 1  
    END  
  
    --Verifica se o valor do bem se encontra com o mesmo valor residual, para parar a deprecia��o          
    IF(@valorAtual <= @valorResidualCalc)            
    BEGIN                   
     BREAK            
    END           
          
    --Subtrai o valor de deprecia��o do valor do patrim�nio            
    SET @valorAtual = @valorAtual - ROUND(@depreciacaoMensal, 10)      
       
    --Incrementa 1 m�s para compara��o            
    SET @contMeses = @contMeses + 1      
                              
     END            
            
   END                          
  
   --Seta o valor da deprecia��o acumulada do bem  
   SET @depreciacaoAcumulada = @valorAquisicao - (@valorAtual + @desdobramento)        


    IF(@movementTypeId = 40 OR @movementTypeId = 31 OR @movementTypeId = 41)  
    BEGIN  
   
	 SET @depreciacaoAcumulada = (select top 1 DepreciationAccumulated from Asset a
								inner join AssetMovements b on b.AssetId = a.Id
								where AssetTransferenciaId = @assetId)

   END
                
     
   --Atualiza os valores dos Patrim�nios            
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
    Id = @assetId    
    
    
	IF(@movementTypeId = 40 OR @movementTypeId = 31 OR @movementTypeId = 41)  
    BEGIN  
   
	declare @monthUsed int;
	declare @flagDecreto bit;

	 select 
	 @depreciacaoAcumulada = DepreciationAccumulated,
	 @ValueUpdate =  ValueUpdate,
	 @monthUsed = monthUsed,
	 @EhAcervo = flagAcervo,
	 @EhTerceiro = flagTerceiro,
	 @flagDecreto = flagDecreto,
	 @vidaUtil =LifeCycle,
	 @valorResidualCalc = ResidualValueCalc,
	 @depreciacaoMensal = DepreciationByMonth,
	 @desdobramento = ValorDesdobramento
		from Asset a
		inner join AssetMovements b on b.AssetId = a.Id
		where AssetTransferenciaId = @assetId


	update         
    Asset         
    SET	
    DepreciationAccumulated = @depreciacaoAcumulada,
	monthUsed = @monthUsed,
	ValueUpdate = @ValueUpdate,
	flagAcervo = @EhAcervo,
	flagDecreto = @flagDecreto,
	flagTerceiro = @EhTerceiro,
	LifeCycle = @vidaUtil,
	ResidualValueCalc= @valorResidualCalc,
	DepreciationByMonth = @depreciacaoMensal,
	ValorDesdobramento = @desdobramento
    where         
    Id = @assetId 

   END
           

   ----Inclui um registro na tabela de hist�rico de deprecia��o  
   --insert into AssetHistoricDepreciation (  
   --          AssetId,  
   --          LifeCycle,  
   --          ResidualValue,  
   --          AceleratedDepreciation,  
   --          ResidualValueCalc,  
   --          MonthUsed,  
   --          DepreciationByMonth,  
   --          DepreciationAccumulated,  
   --          Observation,  
   --          ValueDevalue,  
   --          Login,  
   --          InclusionDate,  
   --          ManagerUnitId,  
   --          ManagmentUnit_YearMonthReference  
   --           )  
   --values           
   --          (  
   --          @assetId,  
   --          @vidaUtil,  
   --          @ResidualValue,  
   --          @AceleratedDepreciation,  
   --          @valorResidualCalc,  
   --          @contMesUso,  
   --          @depreciacaoMensal,  
   --          @depreciacaoAcumulada,  
   --          null,  
   --          null,  
   --          @login,  
   --          getdate(),  
   --          @managerUnitId,  
   --          @ManagmentUnit_YearMonthReference  
   --           )  
  
    END  

	

            
 -----------------------------------------------------------------------------------------------------------------------------------------------------------------------            
           
COMMIT TRANSACTION            
END TRY            
            
BEGIN CATCH                 
 SELECT          
         ERROR_PROCEDURE() AS ErrorProcedure              
        ,ERROR_LINE() AS ErrorLine              
        ,ERROR_MESSAGE() AS ErrorMessage;             
 ROLLBACK TRANSACTION      
   
UPDATE         
 Asset         
SET           
 flagVerificado = 20 -- ERRO PROCEDURE        
WHERE         
 Id = @assetId         
  
END CATCH              
END

GO

GRANT EXECUTE ON [dbo].[DEPRECIACAO_ACUMULADA_UNITARIO] TO [ususamweb]
GO