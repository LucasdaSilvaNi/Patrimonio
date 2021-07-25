/****** Object:  Index [IX__MonthlyDepreciation_AssetStartId_ManagerUnitId_CurrentDate_AccumulatedDepreciation]    Script Date: 07/08/2019 10:18:45 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Asset]') AND name = N'IDX_ASSET_MANAGERUNITID_DEPRECIACAO')
DROP INDEX [IDX_ASSET_MANAGERUNITID_DEPRECIACAO] ON [dbo].[Asset]
GO


/****** Object:  Index [IDX_ASSET_MANAGERUNITID_DEPRECIACAO]    Script Date: 21/11/2019 14:54:19 ******/
CREATE NONCLUSTERED INDEX [IDX_ASSET_MANAGERUNITID_DEPRECIACAO] ON [dbo].[Asset]
(
	[ManagerUnitId] ASC
)
INCLUDE ( 	[AssetStartId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX__MonthlyDepreciation_AssetStartId_ManagerUnitId_CurrentDate_AccumulatedDepreciation]    Script Date: 07/08/2019 10:18:45 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Asset]') AND name = N'IDX_ASSET_VISAO_GERAL_FLAGS')
DROP INDEX [IDX_ASSET_VISAO_GERAL_FLAGS] ON [dbo].[Asset]
GO

/****** Object:  Index [IDX_ASSET_VISAO_GERAL_FLAGS]    Script Date: 21/11/2019 14:54:12 ******/
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



/****** Object:  Index [IX__MonthlyDepreciation_AssetStartId_ManagerUnitId_CurrentDate_AccumulatedDepreciation]    Script Date: 07/08/2019 10:18:45 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Asset]') AND name = N'IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP')
DROP INDEX [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP] ON [dbo].[Asset]
GO
/****** Object:  Index [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP]    Script Date: 21/11/2019 14:53:52 ******/
CREATE NONCLUSTERED INDEX [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP] ON [dbo].[Asset]
(
	[NumberIdentification] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


/****** Object:  Index [IX__MonthlyDepreciation_AssetStartId_ManagerUnitId_CurrentDate_AccumulatedDepreciation]    Script Date: 07/08/2019 10:18:45 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AssetMovements]') AND name = N'IDX_ASSETMOVENTS_INSTITUTION_MOVIMENTTYPE')
DROP INDEX [IDX_ASSETMOVENTS_INSTITUTION_MOVIMENTTYPE] ON [dbo].[AssetMovements]
GO
/****** Object:  Index [IDX_ASSETMOVENTS_INSTITUTION_MOVIMENTTYPE]    Script Date: 21/11/2019 14:53:37 ******/
CREATE NONCLUSTERED INDEX [IDX_ASSETMOVENTS_INSTITUTION_MOVIMENTTYPE] ON [dbo].[AssetMovements]
(
	[InstitutionId] ASC,
	[MovementTypeId] ASC
)
INCLUDE ( 	[Id],
	[AssetId],
	[BudgetUnitId],
	[ManagerUnitId],
	[AdministrativeUnitId],
	[AuxiliaryAccountId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


/****** Object:  Index [IX__MonthlyDepreciation_AssetStartId_ManagerUnitId_CurrentDate_AccumulatedDepreciation]    Script Date: 07/08/2019 10:18:45 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AssetMovements]') AND name = N'IDX_ASSETMOVENTS_INSTITUTION_DATAESTORNO')
DROP INDEX [IDX_ASSETMOVENTS_INSTITUTION_DATAESTORNO] ON [dbo].[AssetMovements]
GO
/****** Object:  Index [IDX_ASSETMOVENTS_INSTITUTION_DATAESTORNO]    Script Date: 21/11/2019 14:53:29 ******/
CREATE NONCLUSTERED INDEX [IDX_ASSETMOVENTS_INSTITUTION_DATAESTORNO] ON [dbo].[AssetMovements]
(
	[InstitutionId] ASC,
	[DataEstorno] ASC,
	[FlagEstorno] ASC
)
INCLUDE ( 	[Id],
	[AssetId],
	[MovimentDate],
	[BudgetUnitId],
	[ManagerUnitId],
	[AdministrativeUnitId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO