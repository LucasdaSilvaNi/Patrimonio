SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'REPORT_PRINT_BAR_CODE_OR_QRCODE')
	DROP PROCEDURE [dbo].[REPORT_PRINT_BAR_CODE_OR_QRCODE]
GO

CREATE PROCEDURE [REPORT_PRINT_BAR_CODE_OR_QRCODE]
	@idUge int,
	@assertsSelecteds varchar(max) 
AS
BEGIN

	select i.[Name] 'Sigla', 
	a.ChapaCompleta, 
	s.[Description] as ShortDescription, 
	m.[Description] as Description_UGE, 
	CONVERT(varbinary(max), '') as BarCode 
	from Asset as a
	inner join ManagerUnit as m on a.ManagerUnitId = m.id 
	inner join ShortDescriptionItem as s on a.ShortDescriptionItemId = s.Id
	inner join Initial as i on a.InitialId = i.Id
	where a.ManagerUnitId = @idUge and
	      a.Id IN(
				select splitdata as Id from fnSplitString(@assertsSelecteds, ',')
			   )

END
GO

GRANT EXECUTE ON [dbo].[REPORT_PRINT_BAR_CODE_OR_QRCODE] TO [ususam]
GO