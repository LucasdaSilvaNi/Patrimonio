IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'ItemInventario'
				 AND COLUMN_NAME = 'AssetMovementsIdOriginal'))
BEGIN
	alter table [dbo].[ItemInventario] add [AssetMovementsIdOriginal] [int] NULL;
END