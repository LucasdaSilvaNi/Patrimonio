SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SAM_PATRIMONIO_CONSULTA_INVENTARIO_MANUAL')
	DROP PROCEDURE [dbo].[SAM_PATRIMONIO_CONSULTA_INVENTARIO_MANUAL]
GO

CREATE PROCEDURE [dbo].[SAM_PATRIMONIO_CONSULTA_INVENTARIO_MANUAL] --6, 22, 205, 2060, 9612, 217673
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
  CONVERT(VARCHAR(MAX), [divisaoUA].[Code]) +' - '+ [divisaoUA].[Description] as [Dados_Divisao],
  ISNULL(itemInventario.InitialName, '') +'-'+ itemInventario.Code as [Sigla_Chapa],
  CAST(itemInventario.item AS VARCHAR(20)) +' - '+ [sdi].[Description] as [Dados_BP],
  itemInventario.AssetId as [AssetID],
  itemInventario.Estado as [SituacaoFisicaBP]
 FROM             
  ItemInventario itemInventario WITH (NOLOCK)
  INNER JOIN Inventario as inventario with(nolock) on inventario.Id = itemInventario.InventarioId 
 INNER JOIN Asset as a WITH (NOLOCK) on a.Id = itemInventario.AssetId
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
 AND inventario.Status = '0'
  GROUP BY 
	[divisaoUA].[id],[divisaoUA].[Code], [divisaoUA].[Description], itemInventario.InitialName, itemInventario.Code, itemInventario.Item, [sdi].[Description], itemInventario.Estado, itemInventario.AssetId
 ORDER BY
 divisaoUA.Code ASC, itemInventario.Code ASC
END
GO

GRANT EXECUTE ON [dbo].[SAM_PATRIMONIO_CONSULTA_INVENTARIO_MANUAL] TO [USUSAM]
GO