DROP TABLE [dbo].[AssetOutReason]
GO

--CREATE TABLE [dbo].[AssetOutReason](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[Description] [varchar](60) NOT NULL,
--	[Status] [bit] NOT NULL,
--	[InstitutionId] [int] NOT NULL,
--	[BudgetUnitId] [int] NOT NULL,
-- CONSTRAINT [PK_MotivoBaixa] PRIMARY KEY NONCLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO

--ALTER TABLE [dbo].[AssetOutReason] ADD  CONSTRAINT [DF_AssetsOutReason_Status]  DEFAULT ((1)) FOR [Status]
--GO

--ALTER TABLE [dbo].[AssetOutReason]  WITH CHECK ADD  CONSTRAINT [FK_AssetOutReason_BudgetUnit] FOREIGN KEY([BudgetUnitId])
--REFERENCES [dbo].[BudgetUnit] ([Id])
--GO

--ALTER TABLE [dbo].[AssetOutReason] CHECK CONSTRAINT [FK_AssetOutReason_BudgetUnit]
--GO

--ALTER TABLE [dbo].[AssetOutReason]  WITH CHECK ADD  CONSTRAINT [FK_AssetOutReason_Institution] FOREIGN KEY([InstitutionId])
--REFERENCES [dbo].[Institution] ([Id])
--GO

--ALTER TABLE [dbo].[AssetOutReason] CHECK CONSTRAINT [FK_AssetOutReason_Institution]
--GO