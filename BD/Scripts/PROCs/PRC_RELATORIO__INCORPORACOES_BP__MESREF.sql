SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PRC_RELATORIO__INCORPORACOES_BP__MESREF')
DROP PROCEDURE [dbo].[PRC_RELATORIO__INCORPORACOES_BP__MESREF]  
GO

--SEGUNDO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO (AQUISICOES MES-REF)  
CREATE PROCEDURE [dbo].[PRC_RELATORIO__INCORPORACOES_BP__MESREF]
(  
    @MonthReference VARCHAR(6)  
  , @ManagerUnitId INT  
) AS  
BEGIN

	Declare @mes int = CONVERT(int,SUBSTRING(@MonthReference,5,2));
	Declare @ano int = CONVERT(int,SUBSTRING(@MonthReference,1,4));
	
	select 
		busca.[TipoIncorporacao],
		busca.ContaContabilDepreciacao,
		(select top 1 [Description] from DepreciationAccount
		where Code = busca.ContaContabilDepreciacao) 'ContaContabilDepreciacaoDescricao',
		busca.ContaContabil,
		(select top 1 [Description] from AuxiliaryAccount
		where Id = busca.IdContaContabil) 'ContaContabilDescricao',
		SUM(busca.ValorAquisicao) 'ValorAquisicao',
		SUM(busca.DepreciacaoNoMes) 'DepreciacaoNoMes',
		SUM(busca.ValorAtual) 'ValorAtual'
	
	from (

	select 
	tipoIncorporacao.[Description] 'TipoIncorporacao',
	contaDepreciacao.[Code] 'ContaContabilDepreciacao',
	contaDepreciacao.[Description] 'ContaContabilDepreciacaoDescricao',
	contacontabil.ContaContabilApresentacao 'ContaContabil',
	contacontabil.[Description] 'ContaContabilDescricao',
	bemPatrimonial.ValueAcquisition 'ValorAquisicao',
	CASE 
	WHEN [bemPatrimonial].[flagAcervo] = 1 THEN  0  
    WHEN [bemPatrimonial].[flagTerceiro] = 1 THEN 0
	ELSE (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[RateDepreciationMonthly],0) 'ValorMensal'
				  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
				  WHERE 
						ManagerUnitId = movimentacaoBP.[ManagerUnitId]
				  AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
				  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId] 
				  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT))
				  --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
				 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
	      )
	END AS 'DepreciacaoNoMes',
	CASE 
		WHEN [bemPatrimonial].[flagAcervo] = 1 THEN ISNULL([bemPatrimonial].[ValueUpdate],[bemPatrimonial].[ValueAcquisition])
		WHEN [bemPatrimonial].[flagTerceiro] = 1 THEN ISNULL([bemPatrimonial].[ValueUpdate],[bemPatrimonial].[ValueAcquisition])
		ELSE (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[CurrentValue],0) 'ValorAcumulado'
					  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
					  WHERE 
							ManagerUnitId = movimentacaoBP.[ManagerUnitId]
							AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
					  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId]
					  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT)) 
					  --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
					 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
		)
	END AS 'ValorAtual',
	--valores para agrupamento e ordenação
	tipoIncorporacao.Id 'IdIncorporacao',
	contacontabil.Id 'IdContaContabil',
	movimentacaoBP.MovimentDate 'DataIncorporacao',
	bemPatrimonial.MaterialItemCode 'CodigoItemMaterial',
	bemPatrimonial.AssetStartId 'IdInicialDaDepreciacao'
	from Asset bemPatrimonial
	inner join AssetMovements movimentacaoBP WITH(NOLOCK)
	on bemPatrimonial.Id = movimentacaoBP.AssetId
	inner join MovementType tipoIncorporacao
	on movimentacaoBP.MovementTypeId = tipoIncorporacao.Id
	left join AuxiliaryAccount contacontabil 
	on movimentacaoBP.AuxiliaryAccountId = contacontabil.Id
	inner join DepreciationAccount contaDepreciacao
	on contacontabil.DepreciationAccountId = contaDepreciacao.Id
	where  bemPatrimonial.flagDepreciaAcumulada = 1
	   and bemPatrimonial.flagVerificado is null
	   and tipoIncorporacao.GroupMovimentId = 1 --o grupo das incorprações
	   and movimentacaoBP.ManagerUnitId = @ManagerUnitId 
	   and (movimentacaoBP.FlagEstorno is null OR movimentacaoBP.FlagEstorno = 0)
	   and MONTH(movimentacaoBP.MovimentDate) = @mes
	   and YEAR(movimentacaoBP.MovimentDate) = @ano
	) [busca]
	GROUP BY  
	   [busca].TipoIncorporacao
     , [busca].ContaContabilDepreciacao
	 , [busca].IdContaContabil
	 , [busca].ContaContabil
     , CONVERT(CHAR(6), [busca].[DataIncorporacao], 112)
     , [busca].CodigoItemMaterial
     , [busca].IdInicialDaDepreciacao
 ORDER BY   
       [busca].TipoIncorporacao
     , [busca].ContaContabilDepreciacao
	 , [busca].IdContaContabil
     , [busca].CodigoItemMaterial
     , [busca].IdInicialDaDepreciacao
END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO__INCORPORACOES_BP__MESREF] TO [ususam]
GO