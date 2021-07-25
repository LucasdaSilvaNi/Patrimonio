SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'REPORT_ASSET_INVENTARIO')
BEGIN
    DROP PROCEDURE [dbo].[REPORT_ASSET_INVENTARIO]
END
GO

CREATE PROCEDURE [dbo].[REPORT_ASSET_INVENTARIO] --EXEC [dbo].[REPORT_ASSET_INVENTARIO] 43, 217, 1582, 54006, 0             
(            
 @idOrgao  int = null,              
 @idUo   int = null,              
 @idUge   int = null,              
 @idUa   int = null,        
 @Agrupamento varchar(1) = null,
 @mesRef VARCHAR(6) = null       
)             
AS             
BEGIN 

	select             
		CASE WHEN @Agrupamento = 1 THEN        
			m.Code        
		ELSE        
		CASE WHEN @Agrupamento = 2 THEN        
			aux.BookAccount        
		END        
		END as Agrupamento,        
		CASE WHEN @Agrupamento = 1 THEN        
		m.Description        
		ELSE        
		CASE WHEN @Agrupamento = 2 THEN        
			aux.[Description]        
		END        
		END as DescAgrupamento,        
		aux.Status AS STATUS_CONTA,       
		e.Code+' - '+e.[Description] as ORGAO,             
		f.Code+' - '+f.[Description] as UO,            
		g.Code+' - '+g.[Description] as UGE,               
		Convert(varchar(max),h.Code)+' - '+h.[Description] as UA,             
		Convert(varchar(max),c.Code)+' - '+c.[Description] as DIVISAO,               
		d.Name as RESPONSAVEL,             
		ISNULL(i.Name, '') +'-'+ a.NumberIdentification as CHAPA,               
		a.materialItemDescription as MATERIAL,               
		a.VALUEACQUISITION as VALOR_AQUISICAO,             
		cl.CurrentPrice as VALOR_ATUAL          
		from Asset as a WITH (NOLOCK)              
		inner join AssetMovements as am WITH (NOLOCK) on 
														(
															a.Id = am.AssetId and 
															am.Id = (
																		select top 1 Id from AssetMovements
																		where 
																			AssetId = a.Id and
																			MovimentDate < (
																								CONVERT(date, DATEADD(month, 1, (SUBSTRING(@mesRef, 1, 4) + SUBSTRING(@mesRef, 5, 2) + '01')))
																							) and
																			(FlagEstorno is null or FlagEstorno = 0)
																		order by Id desc	
																	)
														)
		inner join AuxiliaryAccount as aux WITH (NOLOCK) on am.AuxiliaryAccountId = aux.Id            
		LEFT JOIN Section as c WITH (NOLOCK) on c.Id = am.SectionId              
		inner join Responsible as d WITH (NOLOCK) on d.id = am.ResponsibleId              
		inner join Institution as e WITH (NOLOCK) on e.id = am.InstitutionId              
		inner join BudgetUnit as f WITH (NOLOCK) on f.id = am.BudgetUnitId              
		inner join ManagerUnit as g WITH (NOLOCK) on g.id = am.ManagerUnitId              
		LEFT JOIN AdministrativeUnit as h WITH (NOLOCK) on h.id = am.AdministrativeUnitId             
		inner join Initial as i WITH (NOLOCK) on i.Id = a.InitialId        
		LEFT JOIN MaterialGroup as m WITH (NOLOCK) on a.MaterialGroupCode = m.Code 
		--ALTERACAO @05/06/2019 DOUGLAS BATISTA
		--RETORNA NESTE RELATORIO BP'S QUE NAO ESTEAO NO FECHAMENTO MAS PERTENCEM A UGE NO MES_REFERENCIA EM QUESTAO
		--INNER JOIN
		LEFT JOIN Closing as cl WITH(NOLOCK) ON
												(
													a.Id = cl.AssetId and
													cl.ClosingYearMonthReference = @mesRef and
													cl.AssetMovementsId = am.Id
												)
		WHERE              
			(am.InstitutionId = @idOrgao or @idOrgao is null)              
		and (am.BudgetUnitId = @idUo or @idUo is null)              
		and (am.ManagerUnitId = @idUge or @idUge is null)              
		and (am.AdministrativeUnitId = @idUa or @idUa is null)          
		and a.flagVerificado is null         
		and a.flagDepreciaAcumulada = 1       
		ORDER BY                 
		i.Name,CAST(a.NumberIdentification as bigint)  
END
GO