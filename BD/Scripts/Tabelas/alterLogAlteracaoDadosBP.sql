IF (EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = 'dbo' 
            AND  TABLE_NAME = 'LogAlteracaoDadosBP'
			AND COLUMN_NAME = 'ValorAntigo'))
BEGIN
	alter table [dbo].[LogAlteracaoDadosBP] 
	alter column ValorAntigo varchar(250);
END

IF (EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = 'dbo' 
            AND  TABLE_NAME = 'LogAlteracaoDadosBP'
			AND COLUMN_NAME = 'ValorNovo'))
BEGIN
	alter table [dbo].[LogAlteracaoDadosBP] 
	alter column ValorNovo varchar(250);
END