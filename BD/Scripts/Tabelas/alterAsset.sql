IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Asset'
				 AND COLUMN_NAME = 'NumberDoc'))
BEGIN
	alter table [dbo].[Asset] alter column NumberDoc varchar(20) not null
END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Asset'
				 AND COLUMN_NAME = 'FlagAnimalAServico'))
BEGIN
	alter table [dbo].[Asset] add FlagAnimalAServico bit NULL;
	
	ALTER TABLE [dbo].[Asset] ADD CONSTRAINT [DF_Asset_FlagAnimalAServico]  DEFAULT (0) FOR [FlagAnimalAServico]
END

IF((select count(*) from sys.columns where name = 'FlagAnimalNaoServico') = 0)
BEGIN
	exec sp_rename 'Asset.FlagAnimalAServico', 'FlagAnimalNaoServico', 'COLUMN';
END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Asset'
				 AND COLUMN_NAME = 'flagVindoDoEstoque'))
BEGIN
	alter table [dbo].[Asset]
	add flagVindoDoEstoque bit not null
	CONSTRAINT [DF_Asset_flagVindoDoEstoque] DEFAULT 0;
END
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Asset'
				 AND COLUMN_NAME = 'IndicadorOrigemInventarioInicial'))
BEGIN
	alter table [dbo].[Asset]
	add IndicadorOrigemInventarioInicial tinyint null
	CONSTRAINT [DF_Asset_IndicadorOrigemInventarioInicial] DEFAULT 0;
END
GO

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Asset'
				 AND COLUMN_NAME = 'NumberIdentification'))
BEGIN
	alter table [dbo].[Asset]
	alter column NumberIdentification varchar(30);
END
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Asset'
				 AND COLUMN_NAME = 'DiferenciacaoChapa'))
BEGIN
	alter table [dbo].[Asset] add DiferenciacaoChapa varchar(3) not null constraint [DF_Asset_DiferenciacaoChapa]  DEFAULT ('');
END
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Asset'
				 AND COLUMN_NAME = 'ChapaCompleta'))
BEGIN
	alter table [dbo].[Asset] add ChapaCompleta AS (CONCAT(NumberIdentification,DiferenciacaoChapa));
END
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Asset'
				 AND COLUMN_NAME = 'DiferenciacaoChapaAntiga'))
BEGIN
	alter table [dbo].[Asset] add DiferenciacaoChapaAntiga varchar(3) not null constraint [DF_Asset_DiferenciacaoChapaAntiga]  DEFAULT ('');
END
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Asset'
				 AND COLUMN_NAME = 'ChapaAntigaCompleta'))
BEGIN
	alter table [dbo].[Asset] add ChapaAntigaCompleta AS (CONCAT(OldNumberIdentification,DiferenciacaoChapaAntiga));
END
GO

IF (NOT EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = 'dbo' 
            AND  TABLE_NAME = 'Asset'
			AND COLUMN_NAME = 'DiferenciacaoChapa'
			AND CHARACTER_MAXIMUM_LENGTH = 7))
BEGIN
	alter table [dbo].[Asset] 
	drop column ChapaCompleta;

	alter table [dbo].[Asset] 
	alter column DiferenciacaoChapa varchar(7) not null;

	alter table [dbo].[Asset] 
	add ChapaCompleta AS (CONCAT(NumberIdentification,DiferenciacaoChapa));
END
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'Asset'
				 AND COLUMN_NAME = 'DiferenciacaoChapaAntiga'
				 AND CHARACTER_MAXIMUM_LENGTH = 7))
BEGIN
	alter table [dbo].[Asset] 
	drop column ChapaAntigaCompleta;

	alter table [dbo].[Asset] 
	alter column DiferenciacaoChapaAntiga varchar(7) not null;

	alter table [dbo].[Asset] 
	add ChapaAntigaCompleta AS (CONCAT(OldNumberIdentification,DiferenciacaoChapaAntiga));
END
GO