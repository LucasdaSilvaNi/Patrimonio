CREATE NONCLUSTERED INDEX [IDX_Asset_VisaoGeral_flagVerificado_flagDepreciaAcumulada] ON [dbo].[Asset]
(
	[flagVerificado] ASC,
	[flagDepreciaAcumulada] ASC
)
INCLUDE ( 	[Id],
	[InitialId],
	[InitialName],
	[NumberIdentification],
	[ValueUpdate],
	[MaterialItemCode],
	[ShortDescriptionItemId],
	[DepreciationByMonth]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IDX_Asset_VisaoGeral_ShortDescriptionItemId] ON [dbo].[Asset]
(
	[ShortDescriptionItemId] ASC,
	[flagVerificado] ASC,
	[flagDepreciaAcumulada] ASC
)
INCLUDE ( 	[Id],
	[InitialId],
	[InitialName],
	[NumberIdentification],
	[ValueUpdate],
	[MaterialItemCode],
	[DepreciationByMonth]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IDX_ATAULIZAR_ASSET_START_ID] ON [dbo].[Asset]
(
	[Status] ASC,
	[MovementTypeId] ASC
)
INCLUDE ( 	[Id],
	[InitialId],
	[NumberIdentification],
	[MaterialItemCode],
	[ManagerUnitId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IDX_ATUALIZAR_ASSET_START_ID_MANAGER_ID] ON [dbo].[Asset]
(
	[Status] ASC,
	[ManagerUnitId] ASC
)
INCLUDE ( 	[Id],
	[MovementTypeId],
	[InitialId],
	[NumberIdentification],
	[MaterialItemCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IDX_VISAO_GERAL_DEPRECIACAO] ON [dbo].[Asset]
(
	[Status] ASC,
	[flagVerificado] ASC,
	[flagDepreciaAcumulada] ASC
)
INCLUDE ( 	[Id],
	[InitialId],
	[InitialName],
	[NumberIdentification],
	[ValueAcquisition],
	[MaterialItemCode],
	[MaterialGroupCode],
	[Empenho],
	[ShortDescriptionItemId],
	[NumberDoc],
	[ManagerUnitId],
	[monthUsed],
	[DepreciationByMonth],
	[flagAcervo],
	[flagTerceiro],
	[AssetStartId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IDX_VISAO_GERAL_DEPRECIACAO_ACUMULADA] ON [dbo].[Asset]
(
	[flagVerificado] ASC,
	[flagDepreciaAcumulada] ASC
)
INCLUDE ( 	[Id],
	[InitialId],
	[InitialName],
	[NumberIdentification],
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

CREATE NONCLUSTERED INDEX [IX_ASSENT_NumberIdentification_flagVerificado_flagDepreciaAcumulada] ON [dbo].[Asset]
(
	[NumberIdentification] ASC,
	[flagVerificado] ASC,
	[flagDepreciaAcumulada] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_Asset__Views_SUBRELATORIOS__INVENTARIO_CONTAS_CONTABEIS] ON [dbo].[Asset]
(
	[Status] ASC,
	[flagVerificado] ASC,
	[flagDepreciaAcumulada] ASC
)
INCLUDE ( 	[Id],
	[InitialName],
	[NumberIdentification],
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

CREATE NONCLUSTERED INDEX [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP] ON [dbo].[Asset]
(
	[NumberIdentification] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IDX_ASSET_VISAO_GERAL_FLAGS] ON [dbo].[Asset]
(
	[flagVerificado] ASC,
	[flagDepreciaAcumulada] ASC
)
INCLUDE ( 	[Id],
	[InitialId],
	[InitialName],
	[NumberIdentification],
	[ValueAcquisition],
	[MaterialItemCode],
	[MaterialGroupCode],
	[Empenho],
	[ShortDescriptionItemId],
	[NumberDoc],
	[ManagerUnitId],
	[flagAcervo],
	[flagTerceiro],
	[flagDecreto],
	[AssetStartId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
GO

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