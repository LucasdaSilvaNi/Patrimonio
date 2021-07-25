BEGIN TRANSACTION

BEGIN TRY

--para o relatório de fechamento
update [Module]
set [Path] = '/Patrimonio/Relatorios/Fechamento'
where [Path] = '/Patrimonio/Closings/ReportClosing'

--para o relatório saldo contábil
update [Module]
set [Path] = '/Patrimonio/Relatorios/SaldoContabilOrgao'
where [Path] = '/Patrimonio/Assets/RelatorioSaldoContaOrgao'

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