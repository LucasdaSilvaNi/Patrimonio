drop index IX_Closing_ClosingYearMonthReference ON [dbo].[Closing]
GO

--CREATE NONCLUSTERED INDEX [IX_Closing_ClosingYearMonthReference] ON [dbo].[Closing]
--(
--	[ClosingYearMonthReference] ASC
--)
--INCLUDE ( 	[AssetId],
--	[CurrentPrice],
--	[DepreciationPrice],
--	[AssetMovementsId],
--	[DepreciationAccumulated]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
--GO