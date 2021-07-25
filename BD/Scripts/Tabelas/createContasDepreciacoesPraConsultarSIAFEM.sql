create table ContasDepreciacoesPraConsultarSIAFEM
(
	[Id] int primary key identity (1,1),
	[ContaDepreciacao] int,
	[Descricao] [varchar](255) NULL
)
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[ContasDepreciacoesPraConsultarSIAFEM] TO [USUSAM]
GO