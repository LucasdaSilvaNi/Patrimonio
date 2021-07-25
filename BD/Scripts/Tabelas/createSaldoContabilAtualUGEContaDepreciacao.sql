CREATE TABLE [dbo].[SaldoContabilAtualUGEContaDepreciacao](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdOrgao] [int] NOT NULL,
	[IdUO] [int] NOT NULL,
	[IdUGE] [int] NOT NULL,
	[CodigoOrgao] [varchar](6) NULL,
	[CodigoUO] [varchar](6) NULL,
	[CodigoUGE] [varchar](6) NULL,
	[DepreciationAccountId] [int] NULL,
	[ContaDepreciacao] [int] NULL,
	[DepreciacaoAcumulada] [decimal](18, 2) NULL
)

ALTER TABLE [SaldoContabilAtualUGEContaDepreciacao] 
WITH CHECK ADD CONSTRAINT [FK_SaldoContabilAtualUGEContaDepreciacao_Institution] FOREIGN KEY([IdOrgao])
	REFERENCES [dbo].[Institution] ([Id])
GO

ALTER TABLE [SaldoContabilAtualUGEContaDepreciacao] 
WITH CHECK ADD CONSTRAINT [FK_SaldoContabilAtualUGEContaDepreciacao_BudgetUnit] FOREIGN KEY([IdUo])
	REFERENCES [dbo].[BudgetUnit] ([Id])
GO

ALTER TABLE [SaldoContabilAtualUGEContaDepreciacao] 
WITH CHECK ADD CONSTRAINT [FK_SaldoContabilAtualUGEContaDepreciacao_ManagerUnit] FOREIGN KEY([IdUGE])
	REFERENCES [dbo].[ManagerUnit] ([Id])
GO

ALTER TABLE [SaldoContabilAtualUGEContaDepreciacao]
WITH CHECK ADD CONSTRAINT [FK_SaldoContabilAtualUGEContaDepreciacao_DepreciationAccount] FOREIGN KEY([DepreciationAccountId])
	REFERENCES [dbo].[DepreciationAccount] ([Id])
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[SaldoContabilAtualUGEContaDepreciacao] TO [USUSAM]
GO