drop index IX_CLOSING_ASSENT_CLOSINGYEARMONTHREFERENCE ON [dbo].[Closing]
GO

--CREATE NONCLUSTERED INDEX [IX_CLOSING_ASSENT_CLOSINGYEARMONTHREFERENCE] ON [dbo].[Closing]
--(
--	[AssetId] ASC,
--	[ClosingYearMonthReference] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
--GO