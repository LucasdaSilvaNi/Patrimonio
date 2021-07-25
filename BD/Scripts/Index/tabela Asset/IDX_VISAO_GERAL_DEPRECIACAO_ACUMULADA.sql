IF(EXISTS(select * from sys.indexes
		  where [name] = 'IDX_VISAO_GERAL_DEPRECIACAO_ACUMULADA')
)
BEGIN
	drop index IDX_VISAO_GERAL_DEPRECIACAO_ACUMULADA ON [dbo].[Asset]
END

CREATE NONCLUSTERED INDEX [IDX_VISAO_GERAL_DEPRECIACAO_ACUMULADA] ON [dbo].[Asset]
(
	[flagVerificado] ASC,
	[flagDepreciaAcumulada] ASC
)
INCLUDE ( 	[Id],
	[InitialId],
	[InitialName],
	[NumberIdentification],
	[ChapaCompleta],
	[ValueAcquisition],
	[MaterialItemCode],
	[MaterialGroupCode],
	[Empenho],
	[ShortDescriptionItemId],
	[NumberDoc],
	[ManagerUnitId],
	[flagAcervo],
	[flagTerceiro],
	[AssetStartId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
GO