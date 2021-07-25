DROP TABLE [dbo].[BalanceDetails]
GO

--CREATE TABLE [dbo].[BalanceDetails](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[BalanceId] [int] NOT NULL,
--	[MaterialItemId] [int] NOT NULL,
--	[ValueAssetIncorporated] [decimal](10, 2) NULL,
--	[ValueAssetOut] [decimal](10, 2) NULL,
--	[ValueMaterialItemBalance] [decimal](10, 2) NULL,
--	[AmountMaterialItemBalance] [int] NULL,
--	[AmountAssetOut] [int] NULL,
--	[AmountAssetIncorporated] [int] NULL,
-- CONSTRAINT [PK_BalanceDetails] PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO