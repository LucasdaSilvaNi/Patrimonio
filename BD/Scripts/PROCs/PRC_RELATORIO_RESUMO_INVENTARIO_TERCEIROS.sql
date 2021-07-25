SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'PRC_RELATORIO_RESUMO_INVENTARIO_TERCEIROS')
BEGIN
    DROP PROCEDURE [dbo].[PRC_RELATORIO_RESUMO_INVENTARIO_TERCEIROS]
END
GO

--SEXTO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO (BEM DE TERCEIRO)
CREATE PROCEDURE [dbo].[PRC_RELATORIO_RESUMO_INVENTARIO_TERCEIROS]
(
	   @MonthReference VARCHAR(6)
	 , @ManagerUnitId INT
) AS
BEGIN

Declare @CodigoUGE varchar(6);

select top 1 @CodigoUGE = Code from ManagerUnit
where id = @ManagerUnitId;

select 
CONVERT(varchar,d.BookAccount) + ' - ' + d.AccountingDescription 'ContaContabil',
d.AccountingValue 'ValorAquisicao'
from DepreciationAccountingClosing d
where d.ManagerUnitCode = @CodigoUGE
and d.ReferenceMonth  = @MonthReference
and d.BookAccount in (select ContaContabilApresentacao from AuxiliaryAccount where RelacionadoBP = 2);

END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO_RESUMO_INVENTARIO_TERCEIROS] TO [ususam]
GO