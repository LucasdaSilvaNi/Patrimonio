DROP TABLE [dbo].[Transfer]
GO

--CREATE TABLE [dbo].[Transfer](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[AssetMovementsId] [int] NOT NULL,
--	[DestinationAssetMovementsId] [int] NULL,
--	[DestinationInstituitionId] [int] NOT NULL,
--	[DestinationBudgetUnitId] [int] NULL,
--	[DestinationManagmentUnitId] [int] NULL,
--	[DestinationAdministrativeUnitId] [int] NULL,
--	[DestinationSectionId] [int] NULL,
--	[DestinationAuxiliaryAccountId] [int] NULL,
--	[DestinationOutSourcedId] [int] NULL,
--	[DestinationAssetId] [int] NULL,
--	[TransferDate] [datetime] NULL,
--	[CompletionTransferDate] [datetime] NULL,
--	[Status] [bit] NOT NULL,
--	[DestinationUserId] [int] NULL,
--	[DestinationResponsibleId] [int] NOT NULL
--) ON [PRIMARY]

--GO

--ALTER TABLE [dbo].[Transfer] ADD  CONSTRAINT [DF_Transfer_Status]  DEFAULT ((1)) FOR [Status]
--GO

--ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transfer_User] FOREIGN KEY([DestinationUserId])
--REFERENCES [dbo].[User] ([Id])
--GO

--ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transfer_User]
--GO