SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'REPORT_ANALITICO_DE_BEM_PATRIMONIAL')
DROP PROCEDURE [dbo].[REPORT_ANALITICO_DE_BEM_PATRIMONIAL]  
GO


CREATE PROCEDURE [dbo].[REPORT_ANALITICO_DE_BEM_PATRIMONIAL] --EXEC REPORT_ANALITICO_DE_BEM_PATRIMONIAL @assertsSelecteds='2529406'          
(            
 @idOrgao int = null,              
 @idUo  int = null,              
 @idUge  int = null,              
 @idUa  int = null,              
 @idDivisao int = null,       
 @assertsSelecteds varchar(max)           
)            
AS              
BEGIN              
 SELECT       
  a.Id,        
  (select SUBSTRING(ManagmentUnit_YearMonthReference,5,2) + '/' + SUBSTRING(ManagmentUnit_YearMonthReference,1,4) from ManagerUnit WITH(NOLOCK) where id = a.ManagerUnitId) AS MES_REF,            
  a.InitialName AS SIGLA,           
  a.ChapaCompleta AS CHAPA,          
  CASE    
  WHEN a.[Status] = 1    
   THEN    
    'Ativo'    
   ELSE    
    'Inativo'    
  END AS SITUACAO,          
  Convert(varchar(max),a.MaterialItemCode) AS CODIGO_MATERIAL,          
  LTRIM(RTRIM(a.MaterialItemDescription)) AS ITEM_MATERIAL,            
  LTRIM(RTRIM(short.Description)) AS DESC_RESUMIDA,       
  i.code AS CODE_ORGAO,          
  i.[Description] AS DESC_ORGAO,          
  b.code AS CODE_UO,          
  b.[Description] AS DESC_UO,          
  m.code AS CODE_UGE,          
  m.[Description] as DESC_UGE,          
  Convert(varchar(max),adm.Code) as CODE_UA,          
  adm.[Description] as DESC_UA,          
   Convert(varchar(max),s.Code) as CODE_DIVISAO,          
  s.[Description] as DESC_DIVISAO,          
    (select [name] from OutSourced WITH(NOLOCK) where id = a.OutSourcedId) AS TERCEIRO,           
  Convert(varchar(max),aux.code) AS CODE_CONTA_AUX,      
  aux.ContaContabilApresentacao AS CONTACONTABIL,        
  aux.[Description] DESC_AUXILIAR,     
  aux.[Status] as STATUS_CONTA,       
  (select [Description] from MovementType WITH(NOLOCK) where id = a.MovementTypeId) AS INCORPORADO,          
  (select [name] from Supplier WITH(NOLOCK) where id = a.SupplierId) AS FORNECEDOR,          
  a.NumberDoc AS NUMERO_DOCUMENTO,       
  (select top 1 NumberPurchaseProcess from AssetMovements WITH(NOLOCK) where AssetId = a.Id order by id desc) AS PROCESSO,          
  a.AcquisitionDate AS DATA_DOCUMENTO,          
  a.MovimentDate AS DATA_INCLUSAO,          
  a.ValueAcquisition AS VALOR_DOCUMENTO,  
  CASE 
		WHEN a.flagAcervo = 1 THEN ISNULL(a.ValueUpdate, a.ValueAcquisition)
		WHEN a.flagTerceiro = 1 THEN ISNULL(a.ValueUpdate, a.ValueAcquisition)
		WHEN a.flagDecreto = 1 
		THEN 
		ISNULL((SELECT MIN(m.[CurrentValue])
		  FROM [dbo].[MonthlyDepreciation] m WITH(NOLOCK) 
		  WHERE m.[ManagerUnitId] = a.ManagerUnitId 
		    AND m.[AssetStartId] = a.AssetStartId 
		    AND m.[MaterialItemCode] = a.MaterialItemCode 
			AND Convert(int,Convert(varchar(6),m.[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = a.[ManagerUnitId])
		), a.ValueUpdate)
		ELSE            
  ISNULL((SELECT MIN(m.[CurrentValue])
		  FROM [dbo].[MonthlyDepreciation] m WITH(NOLOCK) 
		  WHERE m.[ManagerUnitId] = a.ManagerUnitId 
		    AND m.[AssetStartId] = a.AssetStartId 
		    AND m.[MaterialItemCode] = a.MaterialItemCode 
			AND Convert(int,Convert(varchar(6),m.[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = a.[ManagerUnitId])
		), a.ValueAcquisition) END AS VALOR_ATUAL,          
  a.SerialNumber AS SERIE,          
  a.ManufactureDate AS FABRICACAO,          
  a.ChassiNumber AS CHASSI,          
  a.Brand AS MARCA,          
  a.NumberPlate AS PLACA,          
  a.Model AS MODELO,          
  (select [Description] from StateConservation WITH(NOLOCK) where id = am.StateConservationId) AS ESTADO_CONSERVACAO,          
  a.DateGuarantee AS GARANTIA,          
  (select [Description] from Initial WITH(NOLOCK) where id = a.OldInitial) + '-' + a.ChapaAntigaCompleta AS IDENTIFICACAO_ANTIGO,          
  a.AdditionalDescription AS HISTORICO,         
      
  am.MovimentDate AS DATA_INCLUSAO_MOV,      
  CASE WHEN am.FlagEstorno = 1 THEN 'Sim' ELSE '' END AS ESTORNO,       
  (select [Description] from MovementType WITH(NOLOCK) where id = am.MovementTypeId) AS TIPO_MOV,        
  (select [name] from Responsible WITH(NOLOCK) where id = am.ResponsibleId) AS RESPONSAVEL,         
  am.NumberDoc AS NUMERO_DOCUMENTO_MOV,         
  (select Code + ' - ' + [Description] from ManagerUnit WITH(NOLOCK) where id = am.SourceDestiny_ManagerUnitId) AS DESTINATARIO,    
  CASE     
  WHEN am.MovementTypeId = 10       
   THEN    
    (    
     select    
      CONVERT(VARCHAR(15), Code)     
     from     
      AdministrativeUnit     
     where     
      Id = am.AdministrativeUnitId    
    )    
  WHEN am.MovementTypeId = 11 or am.MovementTypeId = 12    
   THEN     
    (    
     (    
      select     
       CONVERT(VARCHAR(15), Code)     
      from    
       ManagerUnit    
      where     
       Id = am.ManagerUnitId    
     )    
     + '/' +    
     (    
      select     
       CONVERT(VARCHAR(15), Code)     
      from    
       ManagerUnit    
      where     
       Id = am.SourceDestiny_ManagerUnitId     
     )    
    )    
       
 END AS ORIG_DEST    
 FROM             
  Asset AS a  WITH(NOLOCK)           
 INNER JOIN               
     AssetMovements AS am WITH(NOLOCK)              
 ON           
  am.AssetId = a.Id            
 INNER JOIN          
    Institution AS i WITH(NOLOCK)           
 ON          
  am.InstitutionId = i.Id          
 INNER JOIN          
    BudgetUnit AS b WITH(NOLOCK)           
 ON          
  am.BudgetUnitId = b.Id          
 INNER JOIN          
    ManagerUnit AS m WITH(NOLOCK)           
 ON          
  am.ManagerUnitId = m.Id          
 LEFT JOIN          
    AdministrativeUnit AS adm WITH(NOLOCK)           
 ON          
  am.AdministrativeUnitId = adm.Id          
 LEFT JOIN          
    Section AS s WITH(NOLOCK)           
 ON          
  am.SectionId = s.Id          
 LEFT JOIN          
  AuxiliaryAccount AS aux WITH(NOLOCK)           
 ON          
  am.AuxiliaryAccountId = aux.Id        
 INNER JOIN  
 ShortDescriptionItem AS short WITH(NOLOCK)  
 ON  
 a.ShortDescriptionItemId = short.Id  
 WHERE          
  (am.InstitutionId = @idOrgao or @idOrgao is null)              
 AND           
  (am.BudgetUnitId = @idUo or @idUo is null)              
 AND           
  (am.ManagerUnitId = @idUge or @idUge is null)              
 AND           
  (am.AdministrativeUnitId = @idUa or @idUa is null)             
 AND           
  (am.SectionId = @idDivisao or @idDivisao is null)             
 AND      
  a.Id IN(select splitdata as Id from fnSplitString(@assertsSelecteds, ','))    
 ORDER BY 
	am.Status DESC   
END
GO

GRANT EXECUTE ON [dbo].[REPORT_ANALITICO_DE_BEM_PATRIMONIAL] TO [ususam]
GO