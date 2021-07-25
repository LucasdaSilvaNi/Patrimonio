IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'SiafemCalendar'
				 AND COLUMN_NAME = 'DataParaOperadores'))
BEGIN
	alter table SiafemCalendar
	add DataParaOperadores date
END
GO