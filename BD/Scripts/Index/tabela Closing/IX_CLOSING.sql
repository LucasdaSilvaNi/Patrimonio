drop index IX_CLOSING ON [dbo].[Closing]
GO

--CREATE NONCLUSTERED INDEX [IX_CLOSING] ON [dbo].[Closing]
--(
--	[ClosingYearMonthReference] ASC
--)
--INCLUDE ( 	[AssetId],
--	[CurrentPrice],
--	[DepreciationPrice],
--	[LifeCycle],
--	[MonthUsed],
--	[ResidualValueCalc],
--	[DepreciationAccumulated],
--	[flagDepreciationCompleted]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--GO