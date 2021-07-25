IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AssetMovements'
				 AND COLUMN_NAME = 'ContaContabilAntesDeVirarInservivel'))
BEGIN
	alter table AssetMovements add ContaContabilAntesDeVirarInservivel int NULL;
END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AssetMovements'
				 AND COLUMN_NAME = 'AuxiliaryMovementTypeId'))
BEGIN
	ALTER TABLE [dbo].[AssetMovements]
	ADD AuxiliaryMovementTypeId INT NULL
END
GO
