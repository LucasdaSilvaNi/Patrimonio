IF EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'DataEnvio')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ALTER COLUMN [DataEnvio] DATETIME NOT NULL
END
GO
BEGIN TRANSACTION
IF EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'DataRetorno')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [DataRetorno2] DATETIME NULL

END
COMMIT
GO

BEGIN TRANSACTION
IF EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'DataRetorno2')
BEGIN
    UPDATE [AuditoriaIntegracao] SET [dbo].[AuditoriaIntegracao].[DataRetorno2] = CONVERT(DATETIME, [dbo].[AuditoriaIntegracao].[DataRetorno])

	ALTER TABLE [dbo].[AuditoriaIntegracao]
    DROP COLUMN [DataRetorno]

	EXEC SP_RENAME 'AuditoriaIntegracao.DataRetorno2', 'DataRetorno', 'COLUMN';
END
COMMIT
GO