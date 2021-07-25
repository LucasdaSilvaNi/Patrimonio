SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'REPORT_RESPONSIBLE')
DROP PROCEDURE [dbo].[REPORT_RESPONSIBLE]  
GO
            
CREATE PROCEDURE [dbo].[REPORT_RESPONSIBLE]        
(              
 @idUa INT = NULL,                
 @idResponsavel INT = NULL,          
 @idDivisao  INT = NULL               
)              
AS               
BEGIN             
 SELECT               
  ISNULL(i.Name, '') +'-'+ a.NumberIdentification AS 'CHAPA',         
  a.OldNumberIdentification AS 'N_ANTERIOR',                     
  aux.[Description] AS 'TIPO_DE_BEM',        
  sdi.[Description] AS 'DESCRIÇÃO_OU_COMENTARIO',        
  a.ValueUpdate AS 'VALOR',        
  ad.Street AS 'RUA',        
  ad.Number AS 'NUMERO',        
  ad.District AS 'BAIRRO',        
  ad.PostalCode AS 'CEP',        
  ad.City AS 'CIDADE',        
  ad.[State] AS 'ESTADO',        
  c.Telephone AS 'TELEFONE',      
  st.Description AS 'ESTADO_CONSERVACAO',    
  c.Description AS 'DIVISAO'    
 FROM              
  Asset AS a WITH (NOLOCK)                
 INNER JOIN               
 (                
  SELECT             
   MAX(a.Id) id,             
   AssetId,            
   SectionId,        
   AuxiliaryAccountId,      
   StateConservationId           
  FROM             
   AssetMovements AS a WITH (NOLOCK)                
  WHERE             
   AdministrativeUnitId = @idUa             
  AND             
   ResponsibleId =  @idResponsavel             
  AND             
   (SectionId =  @idDivisao OR @idDivisao is NULL)          
  AND                 
   Status = 1                
  GROUP BY             
   AssetId,            
   SectionId,        
   AuxiliaryAccountId,      
   StateConservationId            
 ) AS b              
 ON             
 b.AssetId = a.Id                
 LEFT JOIN               
 Section AS c WITH (NOLOCK) ON c.id = b.SectionId                
 INNER JOIN               
 Initial AS i WITH (NOLOCK) ON i.Id = a.InitialId          
 LEFT JOIN        
 AuxiliaryAccount AS aux (NOLOCK) ON b.AuxiliaryAccountId = aux.Id        
 INNER JOIN         
 ShortDescriptionItem AS sdi (NOLOCK) ON a.ShortDescriptionItemId = sdi.Id        
 LEFT JOIN         
 [Address] AS ad (NOLOCK) ON c.AddressId = ad.Id        
  INNER JOIN         
 StateConservation AS st (NOLOCK) ON st.Id = b.StateConservationId      
 WHERE  
 a.flagVerificado is null and a.flagDepreciaAcumulada = 1 and a.Status = 1             
END 

GO

GRANT EXECUTE ON [dbo].[REPORT_RESPONSIBLE] TO [ususamweb]
GO