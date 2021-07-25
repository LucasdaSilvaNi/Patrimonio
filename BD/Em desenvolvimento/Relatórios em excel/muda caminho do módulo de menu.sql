BEGIN TRANSACTION

BEGIN TRY

--para o relat�rio de fechamento
update [Module]
set [Path] = '/Patrimonio/Relatorios/Fechamento'
where [Path] = '/Patrimonio/Closings/ReportClosing'

--para o relat�rio saldo cont�bil
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