IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'MonthlyDepreciation'
				 AND COLUMN_NAME = 'MesAbertoDoAceite'))
BEGIN
	alter table [dbo].[MonthlyDepreciation]
	add MesAbertoDoAceite bit;
END
GO

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'MonthlyDepreciation'
				 AND COLUMN_NAME = 'QtdLinhaRepetida'))
BEGIN
	alter table MonthlyDepreciation
	add QtdLinhaRepetida int not null
	CONSTRAINT [DF_MOnthlyDepreciation_QtdLinhaRepetida] DEFAULT 0;
END
GO