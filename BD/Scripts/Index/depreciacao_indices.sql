/****** Object:  Index [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP]    Script Date: 09/04/2020 08:55:33 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Asset]') AND name = N'IDX_DEPRECIACAO_UGE_ASSET_MATERIALITEMCODE')
	DROP INDEX [IDX_DEPRECIACAO_UGE_ASSET_MATERIALITEMCODE] ON [dbo].[Asset]
GO
CREATE NONCLUSTERED INDEX [IDX_DEPRECIACAO_UGE_ASSET_MATERIALITEMCODE]
ON [dbo].[Asset] ([ManagerUnitId],[flagVerificado],[flagDepreciaAcumulada],[flagAcervo],[flagTerceiro],[AssetStartId])
INCLUDE ([MaterialItemCode])
GO


/****** Object:  Index [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP]    Script Date: 09/04/2020 08:55:33 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AssetMovements]') AND name = N'IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_ASSETID_TRANSFERENCIA')
	DROP INDEX [IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_ASSETID_TRANSFERENCIA] ON [dbo].[AssetMovements]
GO

CREATE NONCLUSTERED INDEX [IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_ASSETID_TRANSFERENCIA]
ON [dbo].[AssetMovements] ([SourceDestiny_ManagerUnitId],[AssetTransferenciaId])
INCLUDE ([AssetId])
GO

/****** Object:  Index [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP]    Script Date: 09/04/2020 08:55:33 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AssetMovements]') AND name = N'IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_ASSETID')
	DROP INDEX [IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_ASSETID] ON [dbo].[AssetMovements]
GO
CREATE NONCLUSTERED INDEX [IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_ASSETID]
ON [dbo].[AssetMovements] ([ManagerUnitId],[SourceDestiny_ManagerUnitId],[AssetTransferenciaId])
INCLUDE ([AssetId])
GO


/****** Object:  Index [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP]    Script Date: 09/04/2020 08:55:33 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AssetMovements]') AND name = N'IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_ASSETID_ESTORNO')
	DROP INDEX [IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_ASSETID_ESTORNO] ON [dbo].[AssetMovements]
GO
CREATE NONCLUSTERED INDEX [IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_ASSETID_ESTORNO]
ON [dbo].[AssetMovements] ([FlagEstorno])
INCLUDE ([AssetId])
GO

/****** Object:  Index [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP]    Script Date: 09/04/2020 08:55:33 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Asset]') AND name = N'IDX_DEPRECIACAO_UGE_ASSET_FLAGS_MATERIALITEMCODE')
	DROP INDEX [IDX_DEPRECIACAO_UGE_ASSET_FLAGS_MATERIALITEMCODE] ON [dbo].[Asset]
GO
CREATE NONCLUSTERED INDEX [IDX_DEPRECIACAO_UGE_ASSET_FLAGS_MATERIALITEMCODE]
ON [dbo].[Asset] ([ManagerUnitId],[flagVerificado],[flagDepreciaAcumulada],[MaterialGroupCode],[flagAcervo],[flagTerceiro],[AssetStartId])
INCLUDE ([MaterialItemCode])
GO

/****** Object:  Index [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP]    Script Date: 09/04/2020 08:55:33 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AssetMovements]') AND name = N'IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_MANAGERUNITID')
	DROP INDEX [IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_MANAGERUNITID] ON [dbo].[AssetMovements]
GO
CREATE NONCLUSTERED INDEX [IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_MANAGERUNITID]
ON [dbo].[AssetMovements] ([ManagerUnitId],[MonthlyDepreciationId])

GO


/****** Object:  Index [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP]    Script Date: 09/04/2020 08:55:33 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AssetMovements]') AND name = N'IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_ASSETTRANSFERENCIAID')
	DROP INDEX [IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_ASSETTRANSFERENCIAID] ON [dbo].[AssetMovements]
GO
CREATE NONCLUSTERED INDEX [IDX_DEPRECIACAO_UGE_ASSETMOVEMENTS_ASSETTRANSFERENCIAID]
ON [dbo].[AssetMovements] ([AssetTransferenciaId])
INCLUDE ([AssetId])
GO

/****** Object:  Index [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP]    Script Date: 09/04/2020 08:55:33 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MonthlyDepreciation]') AND name = N'IDX_MONTHLYDEPRECIATION_RETORNAR_DEPRECIACAO')
	DROP INDEX [IDX_MONTHLYDEPRECIATION_RETORNAR_DEPRECIACAO] ON [dbo].[MonthlyDepreciation]
GO

/****** Object:  Index [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP]    Script Date: 09/04/2020 08:55:33 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MonthlyDepreciation]') AND name = N'IDX_DEPRECIACAO_MONTHLYDEPRECIATION_ASSETSTART_ID')
	DROP INDEX [IDX_DEPRECIACAO_MONTHLYDEPRECIATION_ASSETSTART_ID] ON [dbo].[MonthlyDepreciation]
GO

/****** Object:  Index [IX_UPDATE_CAMPO_CHAPA__INATIVACAO_BP]    Script Date: 09/04/2020 08:55:33 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MonthlyDepreciation]') AND name = N'IDX_MONTHLYDEPRECIATION_RETORNAR_MANAGERUNIT')
	DROP INDEX [IDX_MONTHLYDEPRECIATION_RETORNAR_MANAGERUNIT] ON [dbo].[MonthlyDepreciation]
GO

/*** ALTERA O TIPO DO CAMPO     ***/
ALTER TABLE [dbo].[MonthlyDepreciation] ALTER COLUMN [CurrentMonth] SMALLINT NOT NULL;

GO
/****** Object:  Index [IDX_MONTHLYDEPRECIATION_RETORNAR_MANAGERUNIT]    Script Date: 17/04/2020 10:31:48 ******/
CREATE NONCLUSTERED INDEX [IDX_MONTHLYDEPRECIATION_RETORNAR_MANAGERUNIT] ON [dbo].[MonthlyDepreciation]
(
	[ManagerUnitId] ASC,
	[MaterialItemCode] ASC
)
INCLUDE ( 	[Id],
	[AssetStartId],
	[NumberIdentification],
	[InitialId],
	[AcquisitionDate],
	[CurrentDate],
	[DateIncorporation],
	[LifeCycle],
	[CurrentMonth],
	[ValueAcquisition],
	[CurrentValue],
	[ResidualValue],
	[RateDepreciationMonthly],
	[AccumulatedDepreciation],
	[UnfoldingValue],
	[Decree],
	[ManagerUnitTransferId],
	[MonthlyDepreciationId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
GO



/****** Object:  Index [IDX_DEPRECIACAO_MONTHLYDEPRECIATION_ASSETSTART_ID]    Script Date: 17/04/2020 10:32:07 ******/
CREATE NONCLUSTERED INDEX [IDX_DEPRECIACAO_MONTHLYDEPRECIATION_ASSETSTART_ID] ON [dbo].[MonthlyDepreciation]
(
	[AssetStartId] ASC,
	[MaterialItemCode] ASC
)
INCLUDE ( 	[CurrentMonth],
	[CurrentDate],
	[LifeCycle],
	[CurrentValue]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

/****** Object:  Index [IDX_MONTHLYDEPRECIATION_RETORNAR_DEPRECIACAO]    Script Date: 17/04/2020 10:31:36 ******/
CREATE NONCLUSTERED INDEX [IDX_MONTHLYDEPRECIATION_RETORNAR_DEPRECIACAO] ON [dbo].[MonthlyDepreciation]
(
	[AssetStartId] ASC,
	[ManagerUnitId] ASC,
	[MaterialItemCode] ASC
)
INCLUDE ( 	[Id],
	[NumberIdentification],
	[InitialId],
	[AcquisitionDate],
	[CurrentDate],
	[DateIncorporation],
	[LifeCycle],
	[CurrentMonth],
	[ValueAcquisition],
	[CurrentValue],
	[ResidualValue],
	[RateDepreciationMonthly],
	[AccumulatedDepreciation],
	[UnfoldingValue],
	[Decree],
	[ManagerUnitTransferId],
	[MonthlyDepreciationId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'MonthlyDepreciation'
				 AND COLUMN_NAME = 'QtdLinhaRepetida'))
BEGIN
	alter table MonthlyDepreciation
	drop constraint [AK_MANAGER_UNIT_MONTHLY_DEPRECIATION];
	
	alter table MonthlyDepreciation
	add constraint [AK_MANAGER_UNIT_MONTHLY_DEPRECIATION]
	UNIQUE (
		[AssetStartId] ASC,
		[ManagerUnitId] ASC,
		[MaterialItemCode] ASC,
		[CurrentDate] ASC,
		[QtdLinhaRepetida] ASC
	)
END
GO