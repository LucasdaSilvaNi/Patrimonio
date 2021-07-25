/* 001.1-VW__VISAO_GERAL__UGE_MESREF_ATUAL.SQL - POS-INCLUSAO TABELA 'MonthlyDepreciation' */
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MonthlyDepreciation___AssetStartId_CurrentMonth' AND object_id = OBJECT_ID('MonthlyDepreciation'))
	DROP INDEX [IX_MonthlyDepreciation___AssetStartId_CurrentMonth] ON [MonthlyDepreciation]
GO

CREATE NONCLUSTERED INDEX [IX_MonthlyDepreciation___AssetStartId_CurrentMonth]
ON [dbo].[MonthlyDepreciation] ([AssetStartId],[CurrentMonth])
INCLUDE ([AccumulatedDepreciation])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100)
GO


/* 003.1-VW_SAM_PATRIMONIO__BP_TOTALMENTE_DEPRECIADO.SQL - POS-INCLUSAO TABELA 'MonthlyDepreciation' */
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MonthlyDepreciation___AssetStartId' AND object_id = OBJECT_ID('MonthlyDepreciation'))
	DROP INDEX [IX_MonthlyDepreciation___AssetStartId] ON [MonthlyDepreciation]
GO

CREATE NONCLUSTERED INDEX [IX_MonthlyDepreciation___AssetStartId]
ON [dbo].[MonthlyDepreciation] ([AssetStartId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100)
GO


/* 004.1-VW_SAM_PATRIMONIO__BP_TERCEIRO.SQL - POS-INCLUSAO TABELA 'MonthlyDepreciation' */
/* 006.1-VW_SAM_PATRIMONIO__BAIXAS_BPS.sql - POS-INCLUSAO TABELA 'MonthlyDepreciation' */
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MonthlyDepreciation___AssetStartId_ValueAcquisition_CurrentValue' AND object_id = OBJECT_ID('MonthlyDepreciation'))
	DROP INDEX [IX_MonthlyDepreciation___AssetStartId_ValueAcquisition_CurrentValue] ON [MonthlyDepreciation]
GO

CREATE NONCLUSTERED INDEX [IX_MonthlyDepreciation___AssetStartId_ValueAcquisition_CurrentValue]
ON [dbo].[MonthlyDepreciation] ([AssetStartId], [ValueAcquisition], [CurrentValue])
INCLUDE ([DateIncorporation],[RateDepreciationMonthly],[AccumulatedDepreciation], [LifeCycle],[CurrentMonth])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100)
GO
