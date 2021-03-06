SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'PRC_RELATORIO__BP_ACERVO')
BEGIN
    DROP PROCEDURE [dbo].[PRC_RELATORIO__BP_ACERVO]
END
GO

--QUINTO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO (BEM DE ACERVO)
CREATE PROCEDURE [dbo].[PRC_RELATORIO__BP_ACERVO]
(
	   @MonthReference VARCHAR(6)
	 , @ManagerUnitId INT
) AS

BEGIN

DECLARE @tabelaUltimoHistorico table(Id int);

	INSERT INTO @tabelaUltimoHistorico
	 select MAX(am.ID) ID
	 from AssetMovements am WITH(NOLOCK)
	 INNER JOIN Asset a WITH(NOLOCK)
	 on a.Id = am.AssetId
	 WHERE 
	 am.FlagEstorno IS NULL
	 and am.DataEstorno IS NULL
	 and am.ManagerUnitId = @ManagerUnitId
	 and a.flagVerificado IS NULL
	 and a.flagDepreciaAcumulada = 1
	 and (CONVERT(VARCHAR(6),am.MovimentDate,112)) <= @MonthReference
	 and a.flagAcervo = 1
	 and am.ResponsibleId IS NOT NULL --QUANDO EH EFETUADA A MOVIMENTACAO DE UM BP, APOS ACEITE NO DESTINO O SISTEMA LIMPA OS DADOS DE RESPONSAVEL/LOCALIZACAO NO REGISTRO DE MOVIMENTACAO
	 and a.Id NOT IN (
	 		select am.AssetId 
			from AssetMovements am WITH(NOLOCK)
	 		INNER JOIN Asset a WITH(NOLOCK)
	 		on a.Id = am.AssetId
	 		WHERE am.FlagEstorno IS NULL
	 		and am.DataEstorno IS NULL
	 		and am.ManagerUnitId = @ManagerUnitId
	 		and a.flagVerificado IS NULL
	 		and a.flagDepreciaAcumulada = 1
			and a.flagAcervo = 1
	 		and am.Status = 0
	 		and am.MovementTypeId IN (11,12,13,14,15,16,17,18,42,43,45,46,47,48,49,50,51,52,53,55,56,57,58,59,95)
	 		GROUP BY am.AssetId
	 		having (CONVERT(VARCHAR(6),MAX(am.MovimentDate),112)) <= @MonthReference
	 )
	 GROUP BY am.AssetId;

select
			(select top 1 ContaContabilApresentacao + ' - ' + [Description] from AuxiliaryAccount where id = movimetacao.AuxiliaryAccountId) AS 'ContaContabil',
		    SUM(BP.ValueAcquisition) AS 'ValorAquisicao'
from Asset BP
inner join AssetMovements movimetacao
on BP.Id = movimetacao.AssetId
inner join @tabelaUltimoHistorico ultimo
on movimetacao.Id = ultimo.Id
GROUP BY
	 movimetacao.AuxiliaryAccountId
ORDER BY 
	 movimetacao.AuxiliaryAccountId

END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO__BP_ACERVO] TO [ususam]
GO