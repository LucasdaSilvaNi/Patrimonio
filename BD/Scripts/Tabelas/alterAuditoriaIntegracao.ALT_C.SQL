IF EXISTS(SELECT *
          FROM   INFORMATION_SCHEMA.COLUMNS
          WHERE  TABLE_NAME = 'AuditoriaIntegracao'
                 AND COLUMN_NAME = 'MsgErro')
BEGIN
	ALTER TABLE AuditoriaIntegracao 
	ALTER COLUMN MsgErro VARCHAR (150)
END
