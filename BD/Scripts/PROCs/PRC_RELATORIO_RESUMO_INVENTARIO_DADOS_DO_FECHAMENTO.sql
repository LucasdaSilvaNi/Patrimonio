SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'PRC_RELATORIO_RESUMO_INVENTARIO_DADOS_DO_FECHAMENTO')
BEGIN
    DROP PROCEDURE [dbo].[PRC_RELATORIO_RESUMO_INVENTARIO_DADOS_DO_FECHAMENTO]
END
GO

CREATE PROCEDURE [dbo].[PRC_RELATORIO_RESUMO_INVENTARIO_DADOS_DO_FECHAMENTO]
(
	@ManagerUnitId int,
	@MonthReference varchar(6)
) AS
BEGIN

Declare @CodigoUGE varchar(6);

select top 1 @CodigoUGE = Code from ManagerUnit
where id = @ManagerUnitId;

select 
a.DepreciationAccount 'ContaContabilDepreciacao',
a.DepreciationDescription 'ContaContabilDepreciacaoDescricao',
a.[Status] 'ContaContabilDepreciacaoStatus',
CONVERT(varchar,d.BookAccount) 'ContaContabil',
d.AccountingDescription 'ContaContabilDescricao',
d.[Status] 'ContaContabilStatus',
d.AccountingValue 'ValorContabil',
d.DepreciationMonth 'DepreciacaoNoMes',
d.AccumulatedDepreciation 'DepreciacaoAcumulada'
from AccountingClosing a
inner join DepreciationAccountingClosing d
on a.Id = d.AccountingClosingId
where a.ManagerUnitCode = @CodigoUGE
and a.ReferenceMonth  = @MonthReference
and d.BookAccount not in (select ContaContabilApresentacao from AuxiliaryAccount where RelacionadoBP != 0)
ORDER BY 
a.DepreciationAccount, d.BookAccount

END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO_RESUMO_INVENTARIO_DADOS_DO_FECHAMENTO] TO [ususam]
GO