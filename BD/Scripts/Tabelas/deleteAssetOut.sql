DROP TABLE [dbo].[AssetsOut]
GO

--CREATE TABLE [dbo].[AssetsOut](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[AssetId] [int] NOT NULL,
--	[AssetsOutListingId] [int] NULL,
--	[AssetsOutComplementId] [int] NULL,
--	[InitialId] [int] NOT NULL,
--	[Numberidentification] [varchar](6) NOT NULL,
--	[PartNumberIdentification] [varchar](10) NULL,
--	[DateOut] [datetime] NULL,
--	[TypeOutId] [int] NOT NULL,
--	[ProcessNumber] [varchar](25) NULL,
--	[TypeDocumentOutId] [int] NULL,
--	[DocumentNumberOut] [varchar](30) NULL,
--	[NGPB] [varchar](30) NULL,
--	[OldInitial] [varchar](10) NULL,
--	[OldNumberIdentification] [varchar](6) NULL,
--	[OldPartNumberIdentification] [varchar](2) NULL,
--	[ValueOut] [decimal](18, 0) NULL,
--	[Status] [bit] NOT NULL,
-- CONSTRAINT [PK_Baixa] PRIMARY KEY NONCLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO

--ALTER TABLE [dbo].[AssetsOut] ADD  CONSTRAINT [DF_AssetOut_Status]  DEFAULT ((1)) FOR [Status]
--GO

--ALTER TABLE [dbo].[AssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_AssetsOut_AdditionalAssetsOut] FOREIGN KEY([AssetsOutComplementId])
--REFERENCES [dbo].[AdditionalAssetsOut] ([Id])
--GO

--ALTER TABLE [dbo].[AssetsOut] CHECK CONSTRAINT [FK_AssetsOut_AdditionalAssetsOut]
--GO

--ALTER TABLE [dbo].[AssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_Baixa_ArrolamentoBaixa] FOREIGN KEY([AssetsOutListingId])
--REFERENCES [dbo].[AssetsOutListing] ([Id])
--GO

--ALTER TABLE [dbo].[AssetsOut] CHECK CONSTRAINT [FK_Baixa_ArrolamentoBaixa]
--GO

--ALTER TABLE [dbo].[AssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_Baixa_BemPatrimonial] FOREIGN KEY([AssetId])
--REFERENCES [dbo].[Asset] ([Id])
--GO

--ALTER TABLE [dbo].[AssetsOut] CHECK CONSTRAINT [FK_Baixa_BemPatrimonial]
--GO

--ALTER TABLE [dbo].[AssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_Baixa_MotivoBaixa] FOREIGN KEY([TypeOutId])
--REFERENCES [dbo].[AssetOutReason] ([Id])
--GO

--ALTER TABLE [dbo].[AssetsOut] CHECK CONSTRAINT [FK_Baixa_MotivoBaixa]
--GO

--ALTER TABLE [dbo].[AssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_Baixa_Sigla] FOREIGN KEY([InitialId])
--REFERENCES [dbo].[Initial] ([Id])
--GO

--ALTER TABLE [dbo].[AssetsOut] CHECK CONSTRAINT [FK_Baixa_Sigla]
--GO

--ALTER TABLE [dbo].[AssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_Baixa_TipoDocumentoBaixa] FOREIGN KEY([TypeDocumentOutId])
--REFERENCES [dbo].[TypeDocumentOut] ([Id])
--GO

--ALTER TABLE [dbo].[AssetsOut] CHECK CONSTRAINT [FK_Baixa_TipoDocumentoBaixa]
--GO