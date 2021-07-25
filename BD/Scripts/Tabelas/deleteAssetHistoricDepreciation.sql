IF EXISTS (SELECT * FROM sys.objects WHERE type = 'F' AND name = 'FK_AssetHistoricDepreciation')
BEGIN

	ALTER TABLE [dbo].[AssetHistoricDepreciation]
	DROP CONSTRAINT FK_AssetHistoricDepreciation; 
	
	DROP TABLE AssetHistoricDepreciation;

END