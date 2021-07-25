DROP TABLE [dbo].[Balance]
GO

--CREATE TABLE [dbo].[Balance](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[InstitutionId] [int] NOT NULL,
--	[BudgetUnitId] [int] NOT NULL,
--	[ManagerUnitId] [int] NOT NULL,
--	[AdministrativeUnitId] [int] NOT NULL,
--	[RefMonthBalance] [varchar](7) NULL,
--	[ValueIncorporationsBalance] [decimal](10, 2) NULL,
--	[ValueOutsBalance] [decimal](10, 2) NULL,
--	[ValueAtualBalance] [decimal](10, 2) NULL,
--	[AmountIncorporationsBalance] [int] NULL,
--	[AmountOutsBalance] [int] NULL,
--	[AmountAtualBalance] [int] NULL,
--	[BalanceDate] [datetime] NULL,
-- CONSTRAINT [PK_Balance] PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO