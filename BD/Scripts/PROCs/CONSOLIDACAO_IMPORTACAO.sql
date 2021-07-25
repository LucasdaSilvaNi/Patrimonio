SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'CONSOLIDACAO_IMPORTACAO')
BEGIN
    DROP PROCEDURE [dbo].[CONSOLIDACAO_IMPORTACAO]
END
GO

CREATE PROCEDURE [dbo].[CONSOLIDACAO_IMPORTACAO]
(    
 @CodOrgao int    
)    
AS    
BEGIN    
    
 DECLARE @OrgaoId int = (select id from Institution where code = @CodOrgao)
  
 --insere Novas descrições resumidas, conforme planilha de carga 
 INSERT INTO ShortDescriptionItem
 select DISTINCT a.MaterialItemDescription
 FROM Asset a
 INNER JOIN AssetMovements as b with(nolock)
 on b.AssetId = a.Id
 INNER JOIN Item_Siafisico as c with(nolock) 
 ON c.Cod_Item_Mat = a.MaterialItemCode
 INNER JOIN MaterialGroup as d with(nolock) 
 ON d.Code = c.Cod_Grupo
 LEFT JOIN ShortDescriptionItem as e with(nolock) 
 ON e.[Description] = a.MaterialItemDescription
 WHERE e.[Description] IS NULL    
 AND b.InstitutionId = @OrgaoId
 and a.flagVerificado is null and flagDepreciaAcumulada is null
    
    
 -- CRIA TABELA TEMPORARIA PARA ARMAZENAR OS ITENS QUE FORAM ALTERADOS    
 CREATE TABLE #assetId    
 (    
   Id  int    
 )    
    
    
 -- INSERE OS ITENS ENCONTRADOS DA TABELA DO SIAFEM NA TABELA TEMPORARIA    
 INSERT INTO #assetId    
 SELECT DISTINCT a.Id
 FROM Asset a
 INNER JOIN AssetMovements b
 on b.AssetId = a.Id
 INNER JOIN Item_Siafisico as c with(nolock)    
 ON c.Cod_Item_Mat = a.MaterialItemCode
 INNER JOIN MaterialGroup as d with(nolock) 
 ON d.Code = c.Cod_Grupo
 INNER JOIN ShortDescriptionItem as e with(nolock) 
 ON e.[Description] = a.MaterialItemDescription
 where b.InstitutionId = @OrgaoId
 and a.flagVerificado is null 
 and flagDepreciaAcumulada is null

    
 -- ATUALIZA OS ITENS ENCONTRADOS DA TABELA DO SIAFEM NA TABELA ASSET DO PATRIMONIO    
 UPDATE a
 SET     
 a.MaterialItemDescription = c.Nome_Item_Mat,    
 a.MaterialGroupCode = d.Code,    
 a.ShortDescriptionItemId = e.Id,    
 a.LifeCycle = d.LifeCycle,     
 a.RateDepreciationMonthly = d.RateDepreciationMonthly,    
 a.ResidualValue = d.ResidualValue    
 FROM Asset as a with(nolock)
 INNER JOIN #assetId as b with(nolock)
	on a.Id = b.Id
 INNER JOIN Item_Siafisico as c with(nolock)
	ON c.Cod_Item_Mat = a.MaterialItemCode    
 INNER JOIN MaterialGroup as d with(nolock)
	ON d.Code = c.Cod_Grupo    
 INNER JOIN ShortDescriptionItem as e with(nolock)   
	ON e.[Description] = a.MaterialItemDescription
 where (a.flagAcervo is null or a.flagAcervo = 0)    
 and (a.flagTerceiro is null or a.flagTerceiro = 0)
    
 -- ATUALIZA O VALOR DA FLAG PARA 1 PARA OS ITENS QUE NAO FORAM ENCONTRADOS E ESTÃO PENDENTES
 UPDATE a        
 SET flagVerificado = 1       
 FROM Asset a with(nolock)     
 inner join AssetMovements m with(nolock)    
 on a.Id = m.AssetId
 WHERE flagVerificado is null 
 AND flagDepreciaAcumulada is null
 AND a.id NOT IN (SELECT Id FROM #assetId)
 AND (flagAcervo is NULL OR flagAcervo = 0)
 AND (flagTerceiro is NULL OR flagTerceiro = 0)
 AND m.InstitutionId = @OrgaoId
    
 DROP TABLE #assetId    
    
    
  -- PENDENTE CASO SIGLA SEJA GENERICA    
 UPDATE a  
 SET a.flagVerificado = 2    
 from Asset a
 INNER JOIN AssetMovements am
 on a.Id = am.AssetId
 INNER JOIN Initial i
 on a.InitialId = i.Id
 WHERE am.InstitutionId = @OrgaoId
 and a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null
 and (i.[Name] = 'SIGLA_GENERICA' OR i.[Description] = 'SIGLA_GENERICA')
    
 -- PENDENTE CASO CHAPA E SIGLA SEJAM REPETIDAS NO MESMO ORGAO        
UPDATE a
SET a.flagVerificado = 3        
FROM Asset as a with(nolock)
INNER JOIN Asset as b  with(nolock)
ON a.NumberIdentification = b.NumberIdentification
AND a.DiferenciacaoChapa = b.DiferenciacaoChapa
AND a.InitialId = b.InitialId
AND a.Id != b.Id
INNER JOIN AssetMovements am
on a.Id = am.AssetId
WHERE a.flagVerificado IS NULL 
AND a.flagDepreciaAcumulada IS NULL 
AND am.InstitutionId = @OrgaoId    
    
 -- PENDENTE CASO CHAPA SEJA GENERICA    
 UPDATE a
 SET a.flagVerificado = 4    
 FROM Asset a
 INNER JOIN AssetMovements am
 on a.Id = am.AssetId
 WHERE am.InstitutionId = @OrgaoId
 and a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null    
 and a.NumberIdentification = 'CHAPA_GENERICA' 
    
-- PENDENTE CASO A UO SEJA GENERICA    
 UPDATE a     
 SET a.flagVerificado = 5    
 FROM Asset as a with(nolock)     
 INNER JOIN AssetMovements as am with(nolock)     
 ON a.Id = am.AssetId    
 INNER JOIN BudgetUnit b
 on b.Id = am.BudgetUnitId
 WHERE am.institutionId = @OrgaoId    
 and a.flagVerificado is null AND a.flagDepreciaAcumulada is null    
 and b.Code = '99999' 
    
-- PENDENTE CASO A UGE SEJA GENERICA    
 UPDATE a     
 SET a.flagVerificado = 6    
 FROM Asset as a with(nolock)     
 INNER JOIN AssetMovements as am with(nolock)     
 ON a.Id = am.AssetId    
 INNER JOIN ManagerUnit as uge
 on uge.Id = am.ManagerUnitId
 WHERE am.institutionId = @OrgaoId    
 and a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null    
 and uge.Code = '999999'    
    
-- PENDENTE CASO A UA SEJA GENERICA    
 UPDATE a     
 SET a.flagVerificado = 7    
 FROM Asset as a with(nolock)     
 INNER JOIN AssetMovements as am with(nolock)     
 ON a.Id = am.AssetId  
 INNER JOIN  MovementType as mov with(nolock)  
 ON am.MovementTypeId = mov.Id
 LEFT JOIN AdministrativeUnit ua
 on ua.Id = am.AdministrativeUnitId
 WHERE am.institutionId = @OrgaoId    
 and a.flagVerificado is null AND a.flagDepreciaAcumulada is null    
 and mov.GroupMovimentId = 1 
 AND (am.AdministrativeUnitId is null OR ua.Code = '99999999')
    
-- PENDENTE CASO A DIVISAO SEJA GENERICA    
 UPDATE a     
 SET a.flagVerificado = 8    
 FROM Asset as a with(nolock)     
 INNER JOIN AssetMovements as am with(nolock)     
 ON a.Id = am.AssetId    
 INNER JOIN Section divisao
 on divisao.Id = am.SectionId
 WHERE am.institutionId = @OrgaoId    
 and a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null    
 and divisao.Code = 999       
        
-- PENDENTE CASO O RESPONSAVEL SEJA GENERICO    
 UPDATE a     
 SET a.flagVerificado = 9
 FROM Asset as a with(nolock)     
 INNER JOIN AssetMovements as am with(nolock)     
 ON a.Id = am.AssetId    
 INNER JOIN MovementType as mov with(nolock)  
 ON am.MovementTypeId = mov.Id
 LEFT JOIN Responsible r
 on am.ResponsibleId = r.Id
 WHERE am.institutionId = @OrgaoId    
 and a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null
 and mov.GroupMovimentId = 1 
 and (am.ResponsibleId IS NULL OR r.[Name] like 'RESPONSAVEL_%')

 -- PENDENTE CASO O VALOR AQUISICAO SEJA GENERICO    
 UPDATE a    
 SET a.flagVerificado = 10    
 FROM Asset a
 INNER JOIN AssetMovements am
 on a.Id = am.AssetId
 WHERE a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null    
 and (a.ValueAcquisition < 1.00 --Nenhum BP pode incorporar com valor de aquisição menor que R$1,00
 OR (
	(a.flagDecreto is null OR a.flagDecreto = 0) 
	AND a.ValueAcquisition < 10.00) --BPs que não são decretos não podem ser incorporados com valor de aquisiçaõ menor que R$10,00
 )
 and am.InstitutionId = @OrgaoId    
    
-- PENDENTE CASO A DATA AQUISICAO SEJA GENERICA    
 UPDATE a    
 SET a.flagVerificado = 11    
 FROM Asset a
 INNER JOIN AssetMovements am
 on a.Id = am.AssetId
 WHERE am.InstitutionId = @OrgaoId 
 and a.flagVerificado is null AND flagDepreciaAcumulada is null
 and (a.AcquisitionDate > a.MovimentDate OR a.AcquisitionDate < '1900-01-01')
    
 -- PENDENTE CASO A DATA AQUISICAO SEJA MAIOR QUE A DATA ATUAL    
 UPDATE a    
 SET a.flagVerificado = 12
 FROM Asset a
 INNER JOIN AssetMovements am
 on a.Id = am.AssetId
 WHERE am.InstitutionId = @OrgaoId 
 and a.flagVerificado is null AND flagDepreciaAcumulada is null    
 and AcquisitionDate > GETDATE()
  
 -- PENDENTE CASO O ESTADO DE CONSERVAÇÃO SEJA INVÁLIDO   
 UPDATE a     
 SET a.flagVerificado = 13  
 FROM Asset as a with(nolock)     
 INNER JOIN AssetMovements as am with(nolock)     
 ON a.Id = am.AssetId
 LEFT JOIN StateConservation estado
 on am.StateConservationId = estado.Id
 WHERE am.institutionId = @OrgaoId    
 and a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null    
 and (am.StateConservationId is null OR am.StateConservationId = 0 OR estado.[Description] = 'ESTADO_GENERICO')
    
--Pendencia por depreciacao acumulada
update Asset 
SET       
	ValueUpdate = ValueAcquisition,             
	LifeCycle = 0,      
	RateDepreciationMonthly = 0,      
	ResidualValue = 0,
	ResidualValueCalc = 0,      
	monthUsed = 0,      
	DepreciationByMonth = 0,      
	DepreciationAccumulated = 0,           
	ValorDesdobramento = 0
FROM Asset a
INNER JOIN AssetMovements am
on a.Id = am.AssetId
where a.flagVerificado IS NULL 
and a.flagDepreciaAcumulada IS NULL 
and am.InstitutionId = @OrgaoId
and (a.flagAcervo = 1 OR a.flagTerceiro = 1)
  
update Asset 
SET flagDepreciaAcumulada = 1
FROM Asset a
INNER JOIN AssetMovements am
on a.Id = am.AssetId
where a.flagVerificado IS NULL 
and a.flagDepreciaAcumulada IS NULL 
and am.InstitutionId = @OrgaoId
    
EXEC SAM_CALCULA_DEPRECIACAO_CARGA_DE_DADOS @IdOrgao = @OrgaoId

END
GO

GRANT EXECUTE ON [dbo].[CONSOLIDACAO_IMPORTACAO] TO [ususam]
GO