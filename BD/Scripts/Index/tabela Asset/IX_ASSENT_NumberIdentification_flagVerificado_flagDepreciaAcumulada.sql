CREATE NONCLUSTERED INDEX [IX_ASSENT_NumberIdentification_flagVerificado_flagDepreciaAcumulada] ON [dbo].[Asset]
(
	[NumberIdentification] ASC,
	[flagVerificado] ASC,
	[flagDepreciaAcumulada] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
GO