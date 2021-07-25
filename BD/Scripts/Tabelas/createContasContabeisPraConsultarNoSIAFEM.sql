create table ContasContabeisPraConsultarNoSIAFEM
(
	[Id] int primary key identity (1,1),
	ContaContabil int,
	[Descricao] [varchar](100) NULL
)
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[ContasContabeisPraConsultarNoSIAFEM] TO [USUSAM]
GO