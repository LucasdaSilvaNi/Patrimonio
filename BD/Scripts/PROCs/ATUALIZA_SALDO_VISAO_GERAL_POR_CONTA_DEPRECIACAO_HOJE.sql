/****** Object:  StoredProcedure [dbo].[SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE]    Script Date: 09/10/2020 10:43:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE')
	DROP PROCEDURE [dbo].[ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE]
GO

CREATE PROCEDURE [dbo].[ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE]
( 
	@IdOrgao int,
	@IdUO int = NULL,
	@IdUGE int = NULL,
	@IdContaContabil int
)
AS
BEGIN

Declare @IdContaDepreciacao int;
Declare @Valor decimal(18,2);

select @IdContaDepreciacao = CONVERT(INT,DepreciationAccountId)
from AuxiliaryAccount
where id = @IdContaContabil;

IF(@IdContaDepreciacao IS NOT NULL)
BEGIN
	select	   
		@Valor = ISNULL(SUM([visao_geral].ValorContabil),0)
	from (
	 SELECT 
			(SELECT MAX([Extent12].[AccumulatedDepreciation])
			  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
			  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
			    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
			    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
				AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
			) AS 'ValorContabil'		
			FROM [dbo].[Asset] AS [Extent1] WITH(NOLOCK)    
			INNER JOIN [dbo].[AssetMovements] AS [Extent2] WITH(NOLOCK) ON[Extent1].[Id] = [Extent2].[AssetId]    
			INNER JOIN [dbo].[Institution] AS [Extent5] WITH(NOLOCK) ON [Extent5].[Id] = [Extent2].[InstitutionId]    
			INNER JOIN [dbo].[BudgetUnit] AS [Extent6] WITH(NOLOCK) ON [Extent6].[Id] = [Extent2].[BudgetUnitId]    
			INNER JOIN [dbo].[ManagerUnit] AS [Extent7] WITH(NOLOCK) ON [Extent7].[Id] = [Extent2].[ManagerUnitId]
			INNER JOIN [dbo].[AuxiliaryAccount] AS [Extent11] WITH(NOLOCK) ON [Extent11].[Id] = [Extent2].[AuxiliaryAccountId]
			INNER JOIN [dbo].[DepreciationAccount] As [Extent12] WITH(NOLOCK) ON [Extent12].[Id] =  [Extent11].[DepreciationAccountId]
	
			WHERE  ([Extent1].[flagVerificado] IS NULL)        
			  AND ([Extent1].[flagDepreciaAcumulada] = 1)
			  AND ([Extent2].[flagEstorno] IS NULL)
			  AND ([Extent2].[DataEstorno] IS NULL) 
			  AND ([Extent1].[AssetStartId] IS NOT NULL)
			  AND ([Extent2].[Status] = 1)  
			  --Acervo, terceiro, animal nao a Servico não depreciam
			  AND ([Extent1].[flagTerceiro] IS NULL OR [Extent1].[flagTerceiro] = 0)
			  AND ([Extent1].[flagAcervo] IS NULL OR [Extent1].[flagAcervo] = 0)
			  AND ([Extent1].[FlagAnimalNaoServico] IS NULL OR [Extent1].[FlagAnimalNaoServico] = 0)
			  --Não contabilizar saídas que requerem aceite
			  AND ([Extent2].[MovementTypeId] NOT IN (47,56,57))
			  AND ([Extent2].[InstitutionId] = @IdOrgao)  
			  AND ([Extent2].[BudgetUnitId] = @IdUO OR @IdUO IS NULL)  
			  AND ([Extent2].[ManagerUnitId] = @IdUGE OR @IdUGE IS NULL)
			  AND ([Extent2].[AuxiliaryAccountId] != 212508)
			  AND ([Extent12].[Id] = @IdContaDepreciacao)
			  
		) [visao_geral]
	
	declare @IdRegistro int;
	
	select TOP 1 @IdRegistro = Id from SaldoContabilAtualUGEContaDepreciacao
	     where IdOrgao = @IdOrgao
		 and IdUO = @IdUO
		 and IdUGE = @IdUGE
		 and DepreciationAccountId = @IdContaDepreciacao
	
	
		IF( @IdRegistro is not null)
		BEGIN
			update SaldoContabilAtualUGEContaDepreciacao
			set DepreciacaoAcumulada = @Valor
			where id = @IdRegistro;
		END
		ELSE
		BEGIN 
			insert into SaldoContabilAtualUGEContaDepreciacao
			(
			   [IdOrgao]
		      ,[IdUO]
		      ,[IdUGE]
		      ,[CodigoOrgao]
		      ,[CodigoUO]
		      ,[CodigoUGE]
		      ,[DepreciationAccountId]
			  ,[ContaDepreciacao]
		      ,[DepreciacaoAcumulada]
			)
			VALUES(
				@IdOrgao,
				@IdUO,
				@IdUGE,
				(select top 1 Code from Institution where id = @IdOrgao),
				(select top 1 Code from BudgetUnit where id = @IdUO),
				(select top 1 Code from ManagerUnit where id = @IdUGE),
				@IdContaDepreciacao,
				(select top 1 Code from DepreciationAccount where id = @IdContaDepreciacao),
				@Valor
			);
		END
END

END
GO

GRANT EXECUTE ON [dbo].[ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE] TO [USUSAM]
GO