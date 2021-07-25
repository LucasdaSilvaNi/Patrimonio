IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'MovementType'
				 AND COLUMN_NAME = 'Description'))
BEGIN
	ALTER TABLE [dbo].[MovementType]
	ALTER COLUMN [Description] VARCHAR (100);
END
GO