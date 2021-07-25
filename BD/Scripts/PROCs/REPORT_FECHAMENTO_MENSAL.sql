SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'REPORT_FECHAMENTO_MENSAL')
	DROP PROCEDURE [dbo].[REPORT_FECHAMENTO_MENSAL]  
GO

CREATE PROCEDURE [dbo].[REPORT_FECHAMENTO_MENSAL] --'201707',210      
(
	   @mesRef VARCHAR(6) = null,      
	   @UgeId int
) AS
BEGIN

	IF(@mesRef IS NULL)
	BEGIN
		select TOP 1 @mesRef = ManagmentUnit_YearMonthReference from ManagerUnit WITH(NOLOCK) where Id = @UgeId
	END

	Declare @BPsBaixados table(Id int);

	insert into @BPsBaixados
	select am.AssetId 
	from AssetMovements am WITH(NOLOCK)
	INNER JOIN Asset a WITH(NOLOCK)
	on a.Id = am.AssetId
	WHERE am.FlagEstorno IS NULL
	and am.DataEstorno IS NULL
	and am.ManagerUnitId = @UgeId
	and a.flagVerificado IS NULL
	and a.flagDepreciaAcumulada = 1
	and am.Status = 0
	and am.MovementTypeId IN (11,12,13,14,15,16,17,18,42,43,45,46,47,48,49,50,51,52,53,55,56,57,58,59,95)
	GROUP BY am.AssetId
	having (CONVERT(VARCHAR(6),MAX(am.MovimentDate),112)) <= @mesRef

	DECLARE @tabelaUltimoHistorico table(Id int);

	INSERT INTO @tabelaUltimoHistorico
	 select MAX(am.ID) ID
	 from AssetMovements am WITH(NOLOCK)
	 INNER JOIN Asset a WITH(NOLOCK)
	 on a.Id = am.AssetId
	 WHERE 
	 am.FlagEstorno IS NULL
	 and am.DataEstorno IS NULL
	 and am.ManagerUnitId = @UgeId
	 and a.flagVerificado IS NULL
	 and a.flagDepreciaAcumulada = 1
	 and (CONVERT(VARCHAR(6),am.MovimentDate,112)) <= @mesRef
	 GROUP BY am.AssetId;

	 DECLARE @tabelaComoChegouNoMes table(Id int);
	 INSERT INTO @tabelaComoChegouNoMes
	 select MAX(am.ID) 
	 from AssetMovements am WITH(NOLOCK)
	 INNER JOIN Asset a WITH(NOLOCK)
	 on a.Id = am.AssetId
	 WHERE 
	 am.FlagEstorno IS NULL
	 and am.DataEstorno IS NULL
	 and am.ManagerUnitId = @UgeId
	 and a.flagVerificado IS NULL
	 and a.flagDepreciaAcumulada = 1
	 and (CONVERT(VARCHAR(6),am.MovimentDate,112)) < @mesRef
	 GROUP BY am.AssetId;

select 
	BP.Id AS ID,
	ISNULL(historico.AuxiliaryAccountId,0) AS CONTABIL,
	ISNULL(contaContabil.ContaContabilApresentacao, '') AS CONTABIL_MOSTRADO,
	ISNULL(contaContabil.Status, 0) AS STATUS_CONTA,
	tipoMovimento.Description AS TIPO_MOVIMENTO,
	CASE 
	WHEN tipoMovimento.GroupMovimentId = 1 THEN BP.NumberDoc
	ELSE historico.NumberDoc
	END AS DOCUMENTO,
	ISNULL(historico.NumberPurchaseProcess,'') PROCESSO,
	ISNULL(sigla.Name,'') +' - '+ [BP].[ChapaCompleta] AS SIGLACHAPA,
	BP.MaterialItemDescription as MATERIAL,
	CASE WHEN ultimo.Id is not null 
		 THEN bp.ValueAcquisition
		 ELSE 0 
	END AS VALOR_AQUISICAO,
	CASE 
	     WHEN ultimo.Id is not null
		 THEN
		    CASE
			WHEN BP.flagAcervo = 1 THEN ISNULL(BP.ValueUpdate, BP.ValueAcquisition)
		    WHEN BP.flagTerceiro = 1 THEN ISNULL(BP.ValueUpdate, BP.ValueAcquisition)
			ELSE
			ISNULL((select MIN(CurrentValue)
			from MonthlyDepreciation WITH(NOLOCK)
			where MaterialItemCode = BP.MaterialItemCode
			  AND AssetStartId = BP.AssetStartId
			  and CONVERT(VARCHAR(6),CurrentDate,112) <= @mesRef
			  and CurrentMonth <= LifeCycle
			),BP.ValueAcquisition) 
			END
		 ELSE 0
	 END AS VALOR_ATUAL
from AssetMovements historico WITH(NOLOCK)
inner join Asset BP WITH(NOLOCK)
on BP.Id = historico.AssetId
inner join MovementType tipoMovimento WITH(NOLOCK)
on tipoMovimento.Id = historico.MovementTypeId
inner join Initial sigla WITH(NOLOCK)
on sigla.Id = BP.InitialId
left join AuxiliaryAccount contaContabil WITH(NOLOCK)
on contaContabil.Id = historico.AuxiliaryAccountId
left join @tabelaUltimoHistorico ultimo
on ultimo.Id = historico.Id
left join @tabelaComoChegouNoMes chegada
on chegada.Id = historico.Id
where 
BP.Id NOT IN (select Id from @BPsBaixados)
AND (CONVERT(VARCHAR(6),historico.MovimentDate,112) = @mesRef
	 OR chegada.Id is not null)
and historico.FlagEstorno IS NULL
and historico.DataEstorno IS NULL
and historico.ManagerUnitId = @UgeId
and BP.flagVerificado IS NULL
and BP.flagDepreciaAcumulada = 1
ORDER BY               
  sigla.Name, case when ISNUMERIC(BP.NumberIdentification) = 1 THEN convert(float, BP.NumberIdentification) ELSE '99999999999' END;
END
GO

GRANT EXECUTE ON [dbo].[REPORT_FECHAMENTO_MENSAL] TO [ususam]
GO