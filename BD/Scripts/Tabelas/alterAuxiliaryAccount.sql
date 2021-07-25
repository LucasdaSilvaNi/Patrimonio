Alter Table AuxiliaryAccount Alter Column Description Varchar(100)


IF(NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS 
					where TABLE_SCHEMA like 'dbo'
					  AND TABLE_NAME like 'AuxiliaryAccount' 
					  AND COLUMN_NAME like 'RelacionadoBP'))
BEGIN
	--coluna para Relação de Conta Contábil, podendo ser: 
	--por Grupo Material SIAFISICO, por BPs do tipo acervo ou por BPs do tipo terceiros
	alter table [dbo].[AuxiliaryAccount] add RelacionadoBP int 
	CONSTRAINT [DF_AuxiliaryAccount_GrupoRelacionado] default 0 not null;
END

IF(NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS 
					where TABLE_SCHEMA like 'dbo'
					  AND TABLE_NAME like 'AuxiliaryAccount' 
					  AND COLUMN_NAME like 'ContaContabilApresentacao'))
BEGIN
	--coluna para registrar como a Conta Contábil será apresentada no sistema.
	--Na criação desse script, se fez necessário criar uma conta contábil do tipo terceiro 
	--com o número:  791.17.01.01 / 891.17.01.01 (com barra)
	alter table [dbo].[AuxiliaryAccount] add ContaContabilApresentacao varchar(50) NULL
END

IF(NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS 
					where TABLE_SCHEMA like 'dbo'
					  AND TABLE_NAME like 'AuxiliaryAccount' 
					  AND COLUMN_NAME like 'ControleEspecificoResumido'))
BEGIN
	alter table AuxiliaryAccount add ControleEspecificoResumido varchar(15) NULL;
END