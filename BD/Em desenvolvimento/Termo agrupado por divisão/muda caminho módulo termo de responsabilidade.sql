BEGIN TRANSACTION

BEGIN TRY

--para o termo de responsabilidade
update Module
set [Path] = '/Patrimonio/Relatorios/TermoDeResponsabilidade'
where [Path] = '/Patrimonio/Responsibles/ReportResponsibles'

COMMIT
END TRY
BEGIN CATCH
-- Execute error retrieval routine.  
    SELECT  
    ERROR_NUMBER() AS ErrorNumber  
    ,ERROR_SEVERITY() AS ErrorSeverity  
    ,ERROR_LINE() AS ErrorLine  
    ,ERROR_MESSAGE() AS ErrorMessage
ROLLBACK
END CATCH