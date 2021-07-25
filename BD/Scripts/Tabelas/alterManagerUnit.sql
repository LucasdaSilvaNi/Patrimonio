BEGIN TRANSACTION

IF(NOT EXISTS(select * from INFORMATION_SCHEMA.COLUMNS 
					where TABLE_SCHEMA like 'dbo'
					  AND TABLE_NAME like 'ManagerUnit' 
					  AND COLUMN_NAME like 'flagTratarComoOrgao'))
BEGIN
	alter table [dbo].[ManagerUnit] add flagTratarComoOrgao BIT not null default 0;
END
 
IF @@ERROR = 0
COMMIT
ELSE
ROLLBACK