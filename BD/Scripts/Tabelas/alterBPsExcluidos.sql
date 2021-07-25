alter table [dbo].[BPsExcluidos]
add Observacoes varchar(100) NULL;

IF((select count(*) from sys.columns 
	where name = 'SiglaInicial'
	AND Object_ID = Object_ID(N'dbo.BPsExcluidos')) = 0)
BEGIN
	exec sp_rename 'dbo.BPsExcluidos.Sigla', 'SiglaInicial', 'COLUMN';
END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'BPsExcluidos'
				 AND COLUMN_NAME = 'MotivoExclusao'))
BEGIN
	alter table [dbo].[BPsExcluidos] add MotivoExclusao varchar(100);
END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'BPsExcluidos'
				 AND COLUMN_NAME = 'MotivoExclusao'))
BEGIN
	alter table [dbo].[BPsExcluidos] add MotivoExclusao varchar(100);
END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'BPsExcluidos'
				 AND COLUMN_NAME = 'NumeroDocumento'))
BEGIN
	alter table [dbo].[BPsExcluidos] add NumeroDocumento varchar(20) null;
END
GO