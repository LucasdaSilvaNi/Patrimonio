IF Not Exists (Select * From sysobjects Where name ='BPsExcluidos' And xtype='U')
BEGIN

CREATE TABLE [dbo].[BPsExcluidos] (
		Id int PRIMARY KEY IDENTITY(1,1) NOT NULL,
		TipoIncorporacao int not null,
		InitialId int NOT NULL,
		Sigla varchar(10) NULL,
		Chapa varchar(250) NOT NULL,
		ItemMaterial int NOT NULL,
		GrupoMaterial int NOT NULL,
		StateConservationId int NOT NULL,
		ManagerUnitId int not null,
		AdministrativeUnitId int null,
		ResponsibleId int null,
		Processo varchar(25) NULL,
		ValorAquisicao decimal(18, 2) NOT NULL,
		DataAquisicao date NOT NULL,
		DataIncorporacao date NOT NULL,
		flagAcervo bit NOT NULL,
		flagTerceiro bit NOT NULL,
		flagDecretoSEFAZ bit NOT NULL,
		DataAcao date not null,
		LoginAcao int not null,
		NotaLancamento varchar(11) NULL,
		NotaLancamentoEstorno varchar(11) NULL,
		NotaLancamentoDepreciacao varchar(11) NULL,
		NotaLancamentoDepreciacaoEstorno varchar(11) NULL
);

ALTER TABLE [dbo].[BPsExcluidos]  WITH CHECK ADD  CONSTRAINT [FK_BPsExcluidos_MovementType] FOREIGN KEY([TipoIncorporacao])
REFERENCES [dbo].[MovementType] ([Id]);

ALTER TABLE [dbo].[BPsExcluidos]  WITH CHECK ADD  CONSTRAINT [FK_BPsExcluidos_Initial] FOREIGN KEY([InitialId])
REFERENCES [dbo].[Initial] ([Id]);

ALTER TABLE [dbo].[BPsExcluidos]  WITH CHECK ADD  CONSTRAINT [FK_BPsExcluidos_ManagerUnit] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id]);

ALTER TABLE [dbo].[BPsExcluidos]  WITH CHECK ADD  CONSTRAINT [FK_BPsExcluidos_AdministrativeUnit] FOREIGN KEY([AdministrativeUnitId])
REFERENCES [dbo].[AdministrativeUnit] ([Id]);

ALTER TABLE [dbo].[BPsExcluidos]   WITH NOCHECK ADD  CONSTRAINT [FK_BPsExcluidos_Responsible] FOREIGN KEY([ResponsibleId])
REFERENCES [dbo].[Responsible] ([Id]);

Grant Select, Insert, Update, Delete On [dbo].[BPsExcluidos] To [ususamweb]

END