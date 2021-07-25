IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AssetNumberIdentification'
				 AND COLUMN_NAME = 'DiferenciacaoChapa'))
BEGIN
	alter table [dbo].[AssetNumberIdentification] add DiferenciacaoChapa varchar(3) not null constraint [DF_AssetNumberIdentification_DiferenciacaoChapa]  DEFAULT ('');
END
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AssetNumberIdentification'
				 AND COLUMN_NAME = 'ChapaCompleta'))
BEGIN
	alter table [dbo].[AssetNumberIdentification] add ChapaCompleta AS (CONCAT(NumberIdentification,DiferenciacaoChapa));
END
GO

IF (NOT EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = 'dbo' 
            AND  TABLE_NAME = 'Asset'
			AND COLUMN_NAME = 'AssetNumberIdentification'
			AND CHARACTER_MAXIMUM_LENGTH = 7))
BEGIN
	alter table [dbo].[AssetNumberIdentification] 
	drop column ChapaCompleta;

	alter table [dbo].[AssetNumberIdentification]
	alter column DiferenciacaoChapa varchar(7) not null;

	alter table [dbo].[AssetNumberIdentification] 
	add ChapaCompleta AS (CONCAT(NumberIdentification,DiferenciacaoChapa));
END
GO