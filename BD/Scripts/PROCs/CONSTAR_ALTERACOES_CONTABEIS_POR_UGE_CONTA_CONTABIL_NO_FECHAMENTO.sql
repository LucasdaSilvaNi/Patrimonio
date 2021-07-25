/****** Object:  StoredProcedure [dbo].[SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE]    Script Date: 09/10/2020 10:43:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'CONSTAR_ALTERACOES_CONTABEIS_POR_UGE_CONTA_CONTABIL_NO_FECHAMENTO')
	DROP PROCEDURE [dbo].[CONSTAR_ALTERACOES_CONTABEIS_POR_UGE_CONTA_CONTABIL_NO_FECHAMENTO]
GO

CREATE PROCEDURE [dbo].[CONSTAR_ALTERACOES_CONTABEIS_POR_UGE_CONTA_CONTABIL_NO_FECHAMENTO]
(
	@IdUGE int
) AS
BEGIN
	Declare @IdOrgao int, @IdUO int;

	Declare @temp as table(
		[Id] int identity(1,1),
		[IdOrgao] [int] NOT NULL,
		[IdUO] [int] NOT NULL,
		[IdUGE] [int] NOT NULL,
		[IdContaContabil] [int] NOT NULL
	);

	Declare @cont int;

	select @IdOrgao = b.InstitutionId,
	@IdUO = b.Id
	from BudgetUnit b with(nolock)
	inner join ManagerUnit m with(nolock) on m.BudgetUnitId = b.Id
	where m.Id = @IdUGE;

	insert into @temp
	(
		IdOrgao,
		IdUO,
		IdUGE,
		IdContaContabil
	)
	select distinct 
		[Extent2].[InstitutionId],
		[Extent2].[BudgetUnitId],
		[Extent2].[ManagerUnitId],
		[Extent2].[AuxiliaryAccountId]
	FROM [dbo].[Asset] AS [Extent1] WITH(NOLOCK)    
	INNER JOIN [dbo].[AssetMovements] AS [Extent2] WITH(NOLOCK) ON[Extent1].[Id] = [Extent2].[AssetId]
	INNER JOIN [dbo].[MovementType] AS [Extent14] WITH(NOLOCK) ON [Extent14].[Id] = [Extent2].[MovementTypeId]
	INNER JOIN [dbo].[AuxiliaryAccount] AS [Extent11] WITH(NOLOCK) ON [Extent11].[Id] = [Extent2].[AuxiliaryAccountId]
	WHERE  ([Extent1].[flagVerificado] IS NULL)    
	   AND ([Extent1].[flagDepreciaAcumulada] = 1)
	   AND ([Extent2].[flagEstorno] IS NULL)
	   AND ([Extent2].[DataEstorno] IS NULL) 
	   AND ([Extent2].[Status] = 1)  
	   AND ([Extent2].[MovementTypeId] NOT IN (47,56,57)) --Não contabilizar transferência q pedem aceite no SAM
	   AND ([Extent2].[InstitutionId] = @IdOrgao)
	   AND ([Extent2].[BudgetUnitId] = @IdUO)
	   AND ([Extent2].[ManagerUnitId] = @IdUGE);
	   
	update h
	set h.HouveAlteracao = 1
	from HouveAlteracaoContabil h
	inner join @temp temp
	on  temp.IdContaContabil = h.IdContaContabil
	and temp.IdOrgao = h.IdOrgao
	and temp.IdUO = h.IdUO
	and temp.IdUGE = h.IdUGE

	delete temp
	from HouveAlteracaoContabil h
	inner join @temp temp
	on  temp.IdContaContabil = h.IdContaContabil
	and temp.IdOrgao = h.IdOrgao
	and temp.IdUO = h.IdUO
	and temp.IdUGE = h.IdUGE

	select @cont = count(Id) from @temp;
	
	IF(@cont > 0)
	BEGIN
		Declare @IdAtual int, @IdContaContabil int;

		while(@cont > 0)
		BEGIN
			select top 1 
			@IdAtual = Id,
		    @IdContaContabil = IdContaContabil,
			@IdOrgao = IdOrgao,
			@IdUO = IdUO,
			@IdUGE = IdUGE
			from @temp;

			insert into HouveAlteracaoContabil
			(IdOrgao, IdUO, IdUGE, IdContaContabil, HouveAlteracao)
			values
			(@IdOrgao, @IdUO, @IdUGE, @IdContaContabil, 1);

			delete from @temp where id = @IdAtual;
			SET @cont = @cont - 1;
		END
	END

END
GO

GRANT EXECUTE ON [dbo].[CONSTAR_ALTERACOES_CONTABEIS_POR_UGE_CONTA_CONTABIL_NO_FECHAMENTO] TO [USUSAM]
GO