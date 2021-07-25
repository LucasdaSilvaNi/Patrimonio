IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME ='Relacionamento__Asset_AssetMovements_AuditoriaIntegracao' And xtype='U')
	DROP TABLE [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao];


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME ='Relacionamento__Asset_AssetMovements_AuditoriaIntegracao' And xtype='U')
BEGIN
CREATE TABLE [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao](
	[Id] INT IDENTITY(1,1) NOT NULL,
	[AssetId] INT NULL,
	[AssetMovementsId] INT NULL,
	[AuditoriaintegracaoId] INT NULL,
	CONSTRAINT PK_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] UNIQUE NONCLUSTERED 
(
	[AssetId] ASC,
	[AssetMovementsId] ASC,
	[AuditoriaintegracaoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
)

ALTER TABLE [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] ADD CONSTRAINT [FK_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao_Asset] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])

ALTER TABLE [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] ADD CONSTRAINT [FK_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao_AssetMovements] FOREIGN KEY([AssetMovementsId])
REFERENCES [dbo].[AssetMovements] ([Id])

ALTER TABLE [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao]  WITH CHECK ADD CONSTRAINT [FK_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao_AuditoriaIntegracao] FOREIGN KEY([AuditoriaintegracaoId])
REFERENCES [dbo].[Auditoriaintegracao] ([Id])

GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] TO [USUSAM]
END
GO