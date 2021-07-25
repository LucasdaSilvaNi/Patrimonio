create table [dbo].[HistoricoValoresDecreto]
(
	Id int primary key identity(1,1),
	AssetId int,
	ValorAquisicao decimal(18,2),
	ValorRevalorizacao decimal(18,2),
	DataAlteracao datetime,
	LoginId int
)
GO

ALTER TABLE [dbo].[HistoricoValoresDecreto]  WITH CHECK ADD  CONSTRAINT [FK_HistoricoValoresDecreto_Asset] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])
GO

ALTER TABLE [dbo].[HistoricoValoresDecreto] WITH CHECK ADD  CONSTRAINT [FK_HistoricoValoresDecreto_User] FOREIGN KEY([LoginId])
REFERENCES [dbo].[User] ([Id])
GO

Grant Select, Insert, Update, Delete On [dbo].[HistoricoValoresDecreto] To [ususam]
GO