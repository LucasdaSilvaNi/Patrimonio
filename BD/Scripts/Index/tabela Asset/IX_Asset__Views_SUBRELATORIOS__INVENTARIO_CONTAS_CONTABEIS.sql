IF(EXISTS(select * from sys.indexes
		  where [name] = 'IX_Asset__Views_SUBRELATORIOS__INVENTARIO_CONTAS_CONTABEIS')
)
BEGIN
	drop index IX_Asset__Views_SUBRELATORIOS__INVENTARIO_CONTAS_CONTABEIS ON [dbo].[Asset]
END

CREATE NONCLUSTERED INDEX [IX_Asset__Views_SUBRELATORIOS__INVENTARIO_CONTAS_CONTABEIS] ON [dbo].[Asset]
(
	[Status] ASC,
	[flagVerificado] ASC,
	[flagDepreciaAcumulada] ASC
)
INCLUDE ( 	[Id],
	[InitialName],
	[NumberIdentification],
	[ChapaCompleta],
	[ValueAcquisition],
	[ValueUpdate],
	[MovimentDate],
	[DepreciationByMonth],
	[DepreciationAccumulated],
	[LifeCycle],
	[monthUsed],
	[flagAcervo],
	[flagTerceiro],
	[flagDecreto]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
GO