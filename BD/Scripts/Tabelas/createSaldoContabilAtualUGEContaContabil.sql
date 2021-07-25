CREATE TABLE [dbo].[SaldoContabilAtualUGEContaContabil](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdOrgao] [int] NOT NULL,
	[IdUO] [int] NOT NULL,
	[IdUGE] [int] NOT NULL,
	[CodigoOrgao] [varchar](6) NULL,
	[CodigoUO] [varchar](6) NULL,
	[CodigoUGE] [varchar](6) NULL,
	[ContaContabil] [int] NULL,
	[ValorContabil] [decimal](18, 2) NULL
)
GO

ALTER TABLE [SaldoContabilAtualUGEContaContabil] 
WITH CHECK ADD CONSTRAINT [FK_SaldoContabilAtualUGEContaContabil_Institution] FOREIGN KEY([IdOrgao])
	REFERENCES [dbo].[Institution] ([Id])
GO

ALTER TABLE [SaldoContabilAtualUGEContaContabil] 
WITH CHECK ADD CONSTRAINT [FK_SaldoContabilAtualUGEContaContabil_BudgetUnit] FOREIGN KEY([IdUo])
	REFERENCES [dbo].[BudgetUnit] ([Id])
GO

ALTER TABLE [SaldoContabilAtualUGEContaContabil]
WITH CHECK ADD CONSTRAINT [FK_SaldoContabilAtualUGEContaContabil_ManagerUnit] FOREIGN KEY([IdUGE])
	REFERENCES [dbo].[ManagerUnit] ([Id])
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[SaldoContabilAtualUGEContaContabil] TO [USUSAM]
GO