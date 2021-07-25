IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AccountingClosing'
				 AND COLUMN_NAME = 'AuditoriaIntegracaoId'))
BEGIN
	ALTER TABLE AccountingClosing ADD AuditoriaIntegracaoId INT NULL
END
GO


IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'AccountingClosing'
				 AND COLUMN_NAME = 'AuditoriaIntegracaoId'))
BEGIN
	ALTER TABLE AccountingClosing WITH CHECK ADD CONSTRAINT [FK_AccountingClosing_AuditoriaIntegracao] FOREIGN KEY([AuditoriaintegracaoId])
	REFERENCES [dbo].[Auditoriaintegracao] ([Id])
END
GO