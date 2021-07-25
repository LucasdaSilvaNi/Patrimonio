drop table TicketHistory;
GO

--CREATE TABLE [dbo].[TicketHistory](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[TicketId] [int] NOT NULL,
--	[UserId] [int] NOT NULL,
--	[HistoryDate] [datetime] NOT NULL,
--	[Description] [varchar](max) NOT NULL,
--	[TicketStatusId] [int] NOT NULL,
-- CONSTRAINT [PK_TicketHistory] PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

--GO

--ALTER TABLE [dbo].[TicketHistory]  WITH CHECK ADD  CONSTRAINT [FK_TicketHistory_Ticket] FOREIGN KEY([TicketId])
--REFERENCES [dbo].[Ticket] ([Id])
--GO

--ALTER TABLE [dbo].[TicketHistory] CHECK CONSTRAINT [FK_TicketHistory_Ticket]
--GO

--ALTER TABLE [dbo].[TicketHistory]  WITH CHECK ADD  CONSTRAINT [FK_TicketHistory_TicketStatus] FOREIGN KEY([TicketStatusId])
--REFERENCES [dbo].[TicketStatus] ([Id])
--GO

--ALTER TABLE [dbo].[TicketHistory] CHECK CONSTRAINT [FK_TicketHistory_TicketStatus]
--GO

--ALTER TABLE [dbo].[TicketHistory]  WITH CHECK ADD  CONSTRAINT [FK_TicketHistory_User] FOREIGN KEY([UserId])
--REFERENCES [dbo].[User] ([Id])
--GO

--ALTER TABLE [dbo].[TicketHistory] CHECK CONSTRAINT [FK_TicketHistory_User]
--GO