
IF(EXISTS(select * from sys.indexes
		  where [name] = 'IDX_Asset_VisaoGeral_flagVerificado_flagDepreciaAcumulada')
)
BEGIN
	drop index IDX_Asset_VisaoGeral_flagVerificado_flagDepreciaAcumulada ON [dbo].[Asset]
END

CREATE NONCLUSTERED INDEX [IDX_Asset_VisaoGeral_flagVerificado_flagDepreciaAcumulada] ON [dbo].[Asset]
(
	[flagVerificado] ASC,
	[flagDepreciaAcumulada] ASC
)
INCLUDE ( 	[Id],
	[InitialId],
	[InitialName],
	[NumberIdentification],
	[ChapaCompleta],
	[ValueUpdate],
	[MaterialItemCode],
	[ShortDescriptionItemId],
	[DepreciationByMonth]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO