IF(EXISTS(select * from sys.indexes
		  where [name] = 'IDX_ATAULIZAR_ASSET_START_ID')
)
BEGIN
	drop index IDX_ATAULIZAR_ASSET_START_ID ON [dbo].[Asset]
END

CREATE NONCLUSTERED INDEX [IDX_ATAULIZAR_ASSET_START_ID] ON [dbo].[Asset]
(
	[Status] ASC,
	[MovementTypeId] ASC
)
INCLUDE ( 	[Id],
	[InitialId],
	[NumberIdentification],
	[ChapaCompleta],
	[MaterialItemCode],
	[ManagerUnitId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO