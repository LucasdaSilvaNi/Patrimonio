create table [dbo].[BPsARealizaremReclassificacaoContabil]
(
	Id int identity(1,1),
	AssetId int,
	GrupoMaterial int,
	IdUGE int
)
GO

ALTER TABLE [dbo].[BPsARealizaremReclassificacaoContabil]
WITH CHECK ADD CONSTRAINT [FK_BPsARealizaremReclassificacaoContabil_Asset] FOREIGN KEY([AssetId])
	REFERENCES [dbo].[Asset] ([Id])
GO

ALTER TABLE [dbo].[BPsARealizaremReclassificacaoContabil]
WITH CHECK ADD CONSTRAINT [FK_BPsARealizaremReclassificacaoContabil_ManagerUnit] FOREIGN KEY([IdUGE])
	REFERENCES [dbo].[ManagerUnit] ([Id])
GO

ALTER TABLE [dbo].[BPsARealizaremReclassificacaoContabil]
add constraint [UK_BPsARealizaremReclassificacaoContabil_Asset]
UNIQUE ([AssetId])
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[BPsARealizaremReclassificacaoContabil] TO [USUSAM]
GO