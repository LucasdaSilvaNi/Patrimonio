DROP PROCEDURE CONSOLIDACAO_IMPORTACAO_UGE_IDASSET
GO

--CREATE PROCEDURE [dbo].[CONSOLIDACAO_IMPORTACAO_UGE_IDASSET]  --510101 -- 380251
--(    
-- --@CodOrgao int
-- @AssetId int,
-- @ManagerUnitId int    
--)    
--AS    
--BEGIN    

--IF((select COUNT(AssetId) from Closing WHERE AssetId = @AssetId) = 0 ) 
--BEGIN   
--			 --DECLARE @OrgaoId int = (select id from Institution where code = @CodOrgao)    
   
--			 INSERT INTO ShortDescriptionItem     
--			 SELECT DISTINCT     
--			  a.MaterialItemDescription      
--			 FROM     
--			  Asset a    
--			 INNER JOIN     
--			  Item_Siafisico as b     
--			 ON     
--			  b.Cod_Item_Mat = a.MaterialItemCode    
--			 INNER JOIN     
--			  MaterialGroup as c  with(nolock)    
--			 ON     
--			  c.Code = b.Cod_Grupo    
--			 LEFT JOIN     
--			  ShortDescriptionItem as d  with(nolock)    
--			 ON     
--			  d.[Description] = a.MaterialItemDescription    
--			 WHERE     
--			  d.[Description] IS NULL    
--			 AND    
--			  a.Id in (@AssetId)    
--			 and    
--			  a.flagVerificado is null and flagDepreciaAcumulada is null     
    
    
--			 -- CRIA TABELA TEMPORARIA PARA ARMAZENAR OS ITENS QUE FORAM ALTERADOS    
--			 CREATE TABLE #assetId    
--			 (    
--			   Id  int    
--			 )    
    
    
--			 -- INSERE OS ITENS ENCONTRADOS DA TABELA DO SIAFEM NA TABELA TEMPORARIA    
--			 INSERT INTO #assetId    
--			 SELECT DISTINCT     
--			  a.id     
--			 FROM     
--			  Asset a    
--			 INNER JOIN     
--			  Item_Siafisico as b with(nolock)    
--			 ON     
--			  b.Cod_Item_Mat = a.MaterialItemCode    
--			 INNER JOIN     
--			  MaterialGroup as c with(nolock)    
--			 ON     
--			  c.Code = b.Cod_Grupo    
--			 INNER JOIN     
--			  ShortDescriptionItem as d with(nolock)    
--			 ON     
--			  d.[Description] = a.MaterialItemDescription    
--			 where    
--			  a.Id in (@AssetId)    
--			 and    
--			  a.flagVerificado is null and flagDepreciaAcumulada is null    
    
    
--			 -- ATUALIZA OS ITENS ENCONTRADOS DA TABELA DO SIAFEM NA TABELA ASSET DO PATRIMONIO    
--			 UPDATE     
--			  Asset     
--			 SET     
--			  MaterialItemDescription = SUBSTRING(b.Nome_Item_Mat,1,120),    
--			  MaterialGroupCode = c.Code,    
--			  ShortDescriptionItemId = d.Id,    
--			  LifeCycle = c.LifeCycle,     
--			  RateDepreciationMonthly = c.RateDepreciationMonthly,    
--			  ResidualValue = c.ResidualValue    
--			 FROM     
--			  Asset a    
--			 INNER JOIN     
--			  Item_Siafisico as b with(nolock)    
--			 ON    
--			  b.Cod_Item_Mat = a.MaterialItemCode    
--			 INNER JOIN     
--			  MaterialGroup as c with(nolock)    
--			 ON     
--			  c.Code = b.Cod_Grupo    
--			 INNER JOIN     
--			  ShortDescriptionItem as d with(nolock)    
--			 ON     
--			  d.[Description] = a.MaterialItemDescription    
--			 where    
--			  a.Id in (@AssetId)    
--			 and    
--			  a.flagVerificado is null     
--			 and     
--			  a.flagDepreciaAcumulada is null    
--			 and    
--			  (a.flagAcervo is null or a.flagAcervo = 0)    
--			 and     
--			  (a.flagTerceiro is null or a.flagTerceiro = 0)    
    
    
--			 -- ATUALIZA O VALOR DA FLAG PARA 1 PARA OS ITENS QUE NAO FORAM ENCONTRADOS E ESTÃO PENDENTES        
--			 UPDATE         
--			  a        
--			 SET         
--			  flagVerificado = 1       
--			  FROM      
--			 asset a with(nolock)     
--			  inner join      
--			 AssetMovements m with(nolock)    
--			  on      
--			 a.Id = m.AssetId      
--			  WHERE         
--			   flagVerificado is null AND flagDepreciaAcumulada is null AND        
--			   a.id NOT IN ( SELECT Id FROM #assetId) AND (flagAcervo is NULL OR flagAcervo = 0) AND (flagTerceiro is NULL OR flagTerceiro = 0)  AND      
--			   m.AssetId = @AssetId  
    
--			 DROP TABLE #assetId    
    
    
--			 -- PENDENTE CASO SIGLA SEJA GENERICA    
--			 UPDATE     
--			  Asset     
--			 SET     
--			  flagVerificado = 2    
--			 WHERE     
--			  Id in (@AssetId)    
--			 and    
--			  flagVerificado is null AND flagDepreciaAcumulada is null    
--			 and    
--			  InitialId IN (SELECT ID FROM Initial with(nolock) WHERE Name = 'SIGLA_GENERICA' OR Description = 'SIGLA_GENERICA')    
    
    
--			 -- PENDENTE CASO CHAPA E SIGLA SEJAM REPETIDAS NO MESMO ORGAO        
--			 UPDATE         
--			  Asset        
--			 SET         
--			  flagVerificado = 3        
--			 WHERE        
--			  id in (        
--				SELECT DISTINCT         
--			  a.id         
--				FROM         
--			  asset as a with(nolock)         
--				INNER JOIN         
--			  asset as b  with(nolock)         
--				ON             
--			  a.NumberIdentification = b.NumberIdentification      
--				AND         
--			  a.InitialId = b.InitialId      
--			 AND     
--			  a.Id > b.Id       
--				WHERE         
--			  a.flagVerificado IS NULL AND a.flagDepreciaAcumulada IS NULL AND      
--			  a.Id in (@AssetId)  
--			  and a.Status = 1 and b.Status = 1)
        
    
    
--			 -- PENDENTE CASO CHAPA SEJA GENERICA    
--			 UPDATE     
--			  Asset     
--			 SET     
--			  flagVerificado = 4    
--			 WHERE     
--			  Id in (@AssetId)    
--			 and    
--			  flagVerificado is null AND flagDepreciaAcumulada is null    
--			 and    
--			  NumberIdentification = 'CHAPA_GENERICA'    
    
    
    
    
--			 -- PENDENTE CASO A UO SEJA GENERICA    
--			 UPDATE     
--			  a     
--			 SET     
--			  a.flagVerificado = 5    
--			 FROM    
--			  Asset as a with(nolock)     
--			 INNER JOIN    
--			  AssetMovements as am with(nolock)     
--			 ON     
--			  a.Id = am.AssetId    
--			 WHERE     
--			  am.AssetId =@AssetId    
--			 and    
--			  a.flagVerificado is null AND a.flagDepreciaAcumulada is null    
--			 and    
--			  am.BudgetUnitId IN (SELECT ID FROM BudgetUnit with(nolock) WHERE Code = '99999')    
    
    
    
    
--			 -- PENDENTE CASO A UGE SEJA GENERICA    
--			 UPDATE     
--			  a     
--			 SET     
--			  a.flagVerificado = 6    
--			 FROM    
--			  Asset as a with(nolock)     
--			 INNER JOIN    
--			  AssetMovements as am with(nolock)     
--			 ON     
--			  a.Id = am.AssetId    
--			 WHERE     
--			  am.AssetId = @AssetId    
--			 and    
--			  a.flagVerificado is null AND a.flagDepreciaAcumulada is null    
--			 and    
--			  am.ManagerUnitId IN (SELECT ID FROM ManagerUnit with(nolock) WHERE Code = '999999')    
    
    
    
--			 -- PENDENTE CASO A UA SEJA GENERICA    
--			 UPDATE     
--			  a     
--			 SET     
--			  a.flagVerificado = 7    
--			 FROM    
--			  Asset as a with(nolock)     
--			 INNER JOIN    
--			  AssetMovements as am with(nolock)     
--			 ON     
--			  a.Id = am.AssetId  
--			 INNER JOIN 
--				MovementType as mov with(nolock)  
--			 ON
--				am.MovementTypeId = mov.Id
--			 WHERE     
--			  am.AssetId =@AssetId 
--			 and    
--			  a.flagVerificado is null AND a.flagDepreciaAcumulada is null    
--			 and    
--				mov.GroupMovimentId = 1 AND
--			  (am.AdministrativeUnitId IN (SELECT ID FROM AdministrativeUnit with(nolock) WHERE Code = 99999999) OR am.AdministrativeUnitId is NULL)
    
--			 -- PENDENTE CASO A DIVISAO SEJA GENERICA    
--			 UPDATE     
--			  a     
--			 SET     
--			  a.flagVerificado = 8    
--			 FROM    
--			  Asset as a with(nolock)     
--			 INNER JOIN    
--			  AssetMovements as am with(nolock)     
--			 ON     
--			  a.Id = am.AssetId    
--			 WHERE     
--			  am.AssetId =@AssetId   
--			 and    
--			  a.flagVerificado is null AND a.flagDepreciaAcumulada is null    
--			 and    
--			  am.SectionId IN (SELECT ID FROM Section with(nolock) WHERE Code = 999)    
    
    
    
--			 -- PENDENTE CASO A CONTA AUXILIAR SEJA GENERICA    
--			 --UPDATE     
--			 -- a     
--			 --SET     
--			 -- a.flagVerificado = 1    
--			 --FROM    
--			 -- Asset as a    
--			 --INNER JOIN    
--			 -- AssetMovements as am    
--			 --ON     
--			 -- a.Id = am.AssetId    
--			 --WHERE     
--			 -- am.AuxiliaryAccountId IN (SELECT ID FROM AuxiliaryAccount WHERE Code = 0)    
    
    
    
--			 -- PENDENTE CASO O RESPONSAVEL SEJA GENERICO    
--			 UPDATE     
--			  a     
--			 SET     
--			  a.flagVerificado = 9    
--			 FROM    
--			  Asset as a with(nolock)     
--			 INNER JOIN    
--			  AssetMovements as am with(nolock)     
--			 ON     
--			  a.Id = am.AssetId    
--			   INNER JOIN 
--				MovementType as mov with(nolock)  
--			 ON
--				am.MovementTypeId = mov.Id
--			 WHERE     
--			  am.AssetId =@AssetId
--			 and    
--			  a.flagVerificado is null AND a.flagDepreciaAcumulada is null   
--			   and    
--				mov.GroupMovimentId = 1 
--			 and    
--			  (am.ResponsibleId IN (SELECT ID FROM Responsible with(nolock) WHERE Name like 'RESPONSAVEL_%') OR am.ResponsibleId IS NULL)
    
    
    
--			 -- PENDENTE CASO O VALOR AQUISICAO SEJA GENERICO    
--			 UPDATE     
--			  Asset    
--			 SET     
--			  flagVerificado = 10    
--			 WHERE     
--			  Id in (@AssetId)    
--			 and    
--			  flagVerificado is null AND flagDepreciaAcumulada is null    
--			 and    
--			  ValueAcquisition < '10.00'  and (flagDecreto is null OR flagDecreto <> 1) 
    
    

--			 -- PENDENTE CASO O VALOR AQUISICAO SEJA GENERICO DECRETO    
--			 UPDATE     
--			  Asset    
--			 SET     
--			  flagVerificado = 10    
--			 WHERE     
--			  Id in (@AssetId)    
--			 and    
--			  flagVerificado is null AND flagDepreciaAcumulada is null    
--			 and    
--			  ValueAcquisition < '1.00'  and flagDecreto = 1  


    
--			 -- PENDENTE CASO A DATA AQUISICAO SEJA GENERICA    
--			 UPDATE     
--			  Asset    
--			 SET     
--			  flagVerificado = 11    
--			 WHERE     
--			  Id in (@AssetId)    
--			 and    
--			  flagVerificado is null AND flagDepreciaAcumulada is null    
--			 and    
--			  AcquisitionDate = '9999/12/31'    
    
--			 -- PENDENTE CASO A DATA AQUISICAO SEJA MAIOR QUE A DATA ATUAL    
--			 UPDATE     
--			  Asset    
--			 SET     
--			  flagVerificado = 12    
--			 WHERE     
--			  Id in (@AssetId)    
--			 and    
--			  flagVerificado is null AND flagDepreciaAcumulada is null    
--			 and    
--			  AcquisitionDate > GETDATE()  
  
--			 -- PENDENTE CASO O ESTADO DE CONSERVAÇÃO SEJA INVÁLIDO   
--			 UPDATE     
--			  a     
--			 SET     
--			  a.flagVerificado = 13  
--			 FROM    
--			  Asset as a with(nolock)     
--			 INNER JOIN    
--			  AssetMovements as am with(nolock)     
--			 ON     
--			  a.Id = am.AssetId    
--			 WHERE     
--			  am.AssetId =@AssetId    
--			 and    
--			  a.flagVerificado is null AND a.flagDepreciaAcumulada is null    
--			 and    
--			  am.StateConservationId IN (0, (SELECT Id FROM StateConservation with(nolock) WHERE Description = 'ESTADO_GENERICO'))     
--   END     
--			  --Armazena o mês e ano de referência no formato de date 
--			   Declare @dateFechamentoReferencia datetime;              
--				 select     
--				@dateFechamentoReferencia = CONVERT(DATE, ManagmentUnit_YearMonthReference  + '01' ,126)             
--				 from     
--				ManagerUnit     
--			   where     
--				Id = @managerUnitId    


--	 --Tratando incorporação com data aquisição maior que mesRef inicial 
--	UPDATE Asset
--	SET DepreciationByMonth = (CASE LifeCycle WHEN 0 THEN 0 
--                        ELSE ROUND((ValueAcquisition - (ValueAcquisition * (ResidualValue/100))) / LifeCycle, 2, 1) END),
--	monthUsed = 0,
--	flagDepreciaAcumulada = 1,
--	ValueUpdate = ValueAcquisition,
--	DepreciationAccumulated = 0
--	FROM Asset          
--	 where                 
--    flagVerificado IS NULL and               
--    flagDepreciaAcumulada IS NULL and
--	id IN (@AssetId)
--	and  AcquisitionDate >= @dateFechamentoReferencia


-- -- Loop para realizar a depreciação dos itens consolidados        
-- DECLARE @contador INT = 1;        
-- --WHILE(@contador = 1)        
-- --BEGIN        
--  SET @contador = (        
--   SELECT         
--    TOP 1 CASE WHEN Id IS NULL THEN 0 ELSE 1 END FROM Asset with(nolock)           
--   where                     
--   flagVerificado IS NULL AND                   
--   flagDepreciaAcumulada IS NULL AND    
--   Id in (select AssetId from AssetMovements with(nolock) where AssetId  = @AssetId)
--    and  AcquisitionDate < @dateFechamentoReferencia)        
--  IF(@contador = 1)    
--  BEGIN        

--  --select 'exe'
--   EXEC [DEPRECIACAO_ACUMULADA_CARGA_UGE_ASSETID]  @AssetId,@ManagerUnitId      
--  END        
--  ELSE        
--  BEGIN         
--   SET @contador = 0        
--  END        
---- END   
    
--END
