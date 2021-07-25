DROP TABLE IF EXISTS [dbo].[DepreciationMaterialItem]
GO

CREATE TABLE [dbo].[DepreciationMaterialItem](
	DepreciationAccount INT NOT NULL,
	MaterialItemCode INT NOT NULL
)