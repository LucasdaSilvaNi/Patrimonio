SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'SAM_PATRIMONIO_GERA_DADOS_INVENTARIO_MANUAL')
BEGIN
    DROP PROCEDURE [dbo].[SAM_PATRIMONIO_GERA_DADOS_INVENTARIO_MANUAL]
END
GO

CREATE PROCEDURE [dbo].[SAM_PATRIMONIO_GERA_DADOS_INVENTARIO_MANUAL] --6, 22, 205, 2060, 9612
(            
 @idOrgao  INT = NULL,
 @idUo   INT = NULL,
 @idUge   INT = NULL,
 @idUa   INT = NULL,
 @idResponsavel INT = NULL,
 @idDivisao INT = NULL
)
AS             
BEGIN            
 SELECT
 [divisaoUA].[id],
  CONVERT(VARCHAR(MAX), [divisaoUA].[Code]) +' - '+ [divisaoUA].[Description] as [Dados_Divisao],
  ISNULL(i.Name, '') +'-'+ a.NumberIdentification as [Sigla_Chapa],
  CAST(a.MaterialItemCode AS VARCHAR(20)) +' - '+ [sdi].[Description] as [Dados_BP],
  a.Id as [AssetID],
  '3' as [TipoInventario]
 FROM             
  Asset as a WITH (NOLOCK)              
 INNER JOIN AssetMovements as am WITH (NOLOCK) on a.Id = am.AssetId
 LEFT JOIN Section as divisaoUA WITH (NOLOCK) on divisaoUA.Id = am.SectionId
 INNER JOIN Responsible as d WITH (NOLOCK) on d.id = am.ResponsibleId
 INNER JOIN Institution as e WITH (NOLOCK) on e.id = am.InstitutionId
 INNER JOIN BudgetUnit as f WITH (NOLOCK) on f.id = am.BudgetUnitId
 INNER JOIN ManagerUnit as g WITH (NOLOCK) on g.id = am.ManagerUnitId 
 LEFT JOIN AdministrativeUnit as h WITH (NOLOCK) on h.id = am.AdministrativeUnitId
 INNER JOIN Initial as i WITH (NOLOCK) on i.Id = a.InitialId
 LEFT JOIN ShortDescriptionItem as sdi WITH (NOLOCK) on a.ShortDescriptionItemId = sdi.Id
 WHERE
     (am.InstitutionId = @idOrgao OR @idOrgao IS NULL)
 AND (am.BudgetUnitId = @idUo OR @idUo IS NULL)
 AND (am.ManagerUnitId = @idUge OR @idUge IS NULL)
 AND (am.AdministrativeUnitId = @idUa OR @idUa IS NULL)
 AND (am.ResponsibleId = @idResponsavel OR @idResponsavel IS NULL)
 AND (am.SectionId = @idDivisao OR @idDivisao IS NULL)
 AND a.Status = 1 AND am.Status = 1
 AND a.flagVerificado IS NULL
 AND a.flagDepreciaAcumulada = 1
 --AND divisaoUA.Code = 677
 GROUP BY 
	[divisaoUA].[id],[divisaoUA].[Code], [divisaoUA].[Description], i.Name, a.NumberIdentification, a.MaterialItemCode, [sdi].[Description], a.Id
 ORDER BY
 -- a.Id, i.Name, a.NumberIdentification
	divisaoUA.Code ASC,a.NumberIdentification ASC
END
GO