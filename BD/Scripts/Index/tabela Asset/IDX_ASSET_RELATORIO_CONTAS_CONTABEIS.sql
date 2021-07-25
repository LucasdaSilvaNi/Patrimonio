IF(EXISTS(select * from sys.indexes
		  where [name] = 'IDX_ASSET_RELATORIO_CONTAS_CONTABEIS')
)
BEGIN
	drop index IDX_ASSET_RELATORIO_CONTAS_CONTABEIS ON [dbo].[Asset]
END

CREATE NONCLUSTERED INDEX [IDX_ASSET_RELATORIO_CONTAS_CONTABEIS] ON [dbo].[Asset]
(
	[Id] ASC,
	[NumberIdentification] ASC,
	[flagVerificado] ASC,
	[flagDepreciaAcumulada] ASC,
	[flagAcervo] ASC,
	[flagTerceiro] ASC
)
INCLUDE ( 	[ValueAcquisition]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO