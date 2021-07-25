DROP TABLE [dbo].[TypeDocumentOut]
GO

--CREATE TABLE [dbo].[TypeDocumentOut](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[Description] [varchar](60) NOT NULL,
--	[Status] [bit] NOT NULL,
-- CONSTRAINT [PK_TipoDocumentoBaixa] PRIMARY KEY NONCLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

--GO

--ALTER TABLE [dbo].[TypeDocumentOut] ADD  CONSTRAINT [DF_TypeDocumentOut_Status]  DEFAULT ((1)) FOR [Status]
--GO