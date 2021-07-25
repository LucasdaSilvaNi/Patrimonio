DROP TABLE [dbo].[Ticket]
GO

--CREATE TABLE [dbo].[Ticket](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[OpenDate] [datetime] NOT NULL,
--	[ClosedDate] [datetime] NOT NULL,
--	[InstitutionId] [int] NOT NULL,
--	[ManagedSystemId] [int] NOT NULL,
--	[Description] [varchar](max) NOT NULL,
--	[OperatorRating] [int] NULL,
--	[TicketRating] [int] NULL,
--	[DescriptionRating] [varchar](max) NULL,
--	[TicketTypeId] [int] NOT NULL,
--	[TicketStatusId] [int] NOT NULL,
--	[UserId] [int] NOT NULL,
-- CONSTRAINT [PK_Ticket] PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

--GO

--ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_ManagedSystem] FOREIGN KEY([ManagedSystemId])
--REFERENCES [dbo].[ManagedSystem] ([Id])
--GO

--ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_ManagedSystem]
--GO

--ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_User] FOREIGN KEY([UserId])
--REFERENCES [dbo].[User] ([Id])
--GO

--ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_User]
--GO