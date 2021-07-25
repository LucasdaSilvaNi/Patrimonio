drop index IX_Closing_ManagerUnit_ClosingYear ON [dbo].[Closing]
GO

--CREATE NONCLUSTERED INDEX [IX_Closing_ManagerUnit_ClosingYear] ON [dbo].[Closing]
--(
--	[ManagerUnitId] ASC,
--	[ClosingYearMonthReference] ASC
--)
--INCLUDE ( 	[AssetId],
--	[CurrentPrice],
--	[MaterialItemCode],
--	[AssetMovementsId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
--GO