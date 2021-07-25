CREATE TABLE [dbo].[HouveAlteracaoContabil](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdOrgao] [int] NULL,
	[IdUO] [int] NULL,
	[IdUGE] [int] NULL,
	[IdContaContabil] [int] NULL,
	[HouveAlteracao] [bit] NULL
)
GO

ALTER TABLE [HouveAlteracaoContabil] 
WITH CHECK ADD CONSTRAINT [FK_HouveAlteracaoContabil_Institution] FOREIGN KEY([IdOrgao])
	REFERENCES [dbo].[Institution] ([Id])
GO

ALTER TABLE [HouveAlteracaoContabil] 
WITH CHECK ADD CONSTRAINT [FK_HouveAlteracaoContabil_BudgetUnit] FOREIGN KEY([IdUo])
	REFERENCES [dbo].[BudgetUnit] ([Id])
GO

ALTER TABLE [HouveAlteracaoContabil] 
WITH CHECK ADD CONSTRAINT [FK_HouveAlteracaoContabil_ManagerUnit] FOREIGN KEY([IdUGE])
	REFERENCES [dbo].[ManagerUnit] ([Id])
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[HouveAlteracaoContabil] TO [USUSAM]
GO