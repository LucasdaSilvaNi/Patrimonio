IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'REPORT_ASSET_PENDING')
BEGIN 
	DROP PROCEDURE REPORT_ASSET_PENDING
END
GO

--CREATE PROCEDURE [dbo].[REPORT_ASSET_PENDING]
--	@assertsSelecteds varchar(max) 
--AS
--	select 
--			i.Description, 
--		    a.NumberIdentification, 
--			a.MaterialItemDescription,
--			case 
--				when a.flagVerificado = 1 then 'Item Material não encontrado' 
				
--				else
--					case 
--						when a.flagVerificado = 2 then 'Grupo Material não encontrado' 
--					end
--			end as Pendencia
--	from Asset as a
--	inner join Initial as i on a.InitialId = i.Id
--	where a.Id IN(
--				 select splitdata as Id from fnSplitString(@assertsSelecteds, ',')
--			   )
--GO