DROP TABLE [dbo].[AdditionalAssetsOut]
GO


--CREATE TABLE [dbo].[AdditionalAssetsOut](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[MeetingDate] [datetime] NULL,
--	[PresidentId] [int] NULL,
--	[FirstMemberId] [int] NULL,
--	[SecondMemberId] [int] NULL,
--	[ThirdMemberId] [int] NULL,
--	[FourthMemberID] [int] NULL,
--	[DirectorID] [int] NULL,
--	[SectionChiefId] [int] NULL,
--	[HistoricId] [int] NULL,
--	[RecordDate] [varchar](8) NULL,
--	[DocumentRecordNumber] [varchar](15) NULL,
--	[RecordDirectorId] [int] NULL,
--	[FirstWitnessId] [int] NULL,
--	[SecondWitnessId] [int] NULL,
--	[ThirdWitnessId] [int] NULL,
--	[FourthWitnessId] [int] NULL,
--	[AddresseeId] [int] NULL,
-- CONSTRAINT [PK_AdditionalAssetsOut] PRIMARY KEY NONCLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Destination] FOREIGN KEY([AddresseeId])
--REFERENCES [dbo].[Addressee] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] CHECK CONSTRAINT [FK_AdditionalAssetsOut_Destination]
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Historic] FOREIGN KEY([HistoricId])
--REFERENCES [dbo].[Historic] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Historic]
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed] FOREIGN KEY([FourthWitnessId])
--REFERENCES [dbo].[Signature] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed]
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed1] FOREIGN KEY([ThirdWitnessId])
--REFERENCES [dbo].[Signature] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed1]
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed10] FOREIGN KEY([PresidentId])
--REFERENCES [dbo].[Signature] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed10]
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed2] FOREIGN KEY([SecondWitnessId])
--REFERENCES [dbo].[Signature] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed2]
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed3] FOREIGN KEY([FirstWitnessId])
--REFERENCES [dbo].[Signature] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed3]
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed4] FOREIGN KEY([RecordDirectorId])
--REFERENCES [dbo].[Signature] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed4]
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed5] FOREIGN KEY([SectionChiefId])
--REFERENCES [dbo].[Signature] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed5]
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed6] FOREIGN KEY([FourthMemberID])
--REFERENCES [dbo].[Signature] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed6]
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed7] FOREIGN KEY([ThirdMemberId])
--REFERENCES [dbo].[Signature] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed7]
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed8] FOREIGN KEY([SecondMemberId])
--REFERENCES [dbo].[Signature] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed8]
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed9] FOREIGN KEY([FirstMemberId])
--REFERENCES [dbo].[Signature] ([Id])
--GO

--ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed9]
--GO