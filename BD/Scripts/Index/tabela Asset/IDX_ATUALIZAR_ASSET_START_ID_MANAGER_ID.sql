IF(EXISTS(select * from sys.indexes
		  where [name] = 'IDX_ATUALIZAR_ASSET_START_ID_MANAGER_ID')
)
BEGIN
	drop index IDX_ATUALIZAR_ASSET_START_ID_MANAGER_ID ON [dbo].[Asset]
END

CREATE NONCLUSTERED INDEX [IDX_ATUALIZAR_ASSET_START_ID_MANAGER_ID] ON [dbo].[Asset]
(
	[Status] ASC,
	[ManagerUnitId] ASC
)
INCLUDE ( 	[Id],
	[MovementTypeId],
	[InitialId],
	[NumberIdentification],
	[ChapaCompleta],
	[MaterialItemCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO