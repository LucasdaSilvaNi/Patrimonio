/****** Object:  StoredProcedure [dbo].[SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE]    Script Date: 09/10/2020 10:43:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_CONTABIL_HOJE')
	DROP PROCEDURE [dbo].[ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_CONTABIL_HOJE]
GO

CREATE PROCEDURE [dbo].[ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_CONTABIL_HOJE]
( 
	@IdOrgao int,
	@IdUO int = NULL,
	@IdUGE int = NULL,
	@IdContaContabil int
)
AS
BEGIN

Declare @ContaContabil int;
Declare @Valor decimal(18,2);

select @ContaContabil = CONVERT(INT,ContaContabilApresentacao) 
from AuxiliaryAccount
where id = @IdContaContabil;

select	   
	@Valor = ISNULL(SUM([visao_geral].ValorContabil),0)
from (
 SELECT  
		CASE 
				WHEN [Extent2].AuxiliaryAccountId = 212508 
				THEN  (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[CurrentValue],0) 'ValorMensal'			
							  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
							  WHERE 
									ManagerUnitId = [Extent2].[ManagerUnitId]
							  AND [dadosDepreciacaoMensal].[MaterialItemCode] = [Extent1].[MaterialItemCode]
							  AND [Extent1].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId] 
							  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <=
							  (select top 1 (CAST(CONVERT(CHAR(6), MovimentDate, 112) AS INT)) from AssetMovements with(nolock) 
							  where AssetId = [Extent1].[Id] AND 
								    FlagEstorno IS NULL AND
									MovementTypeId = 19))
							 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
						)
				ELSE (SELECT TOP 1 ISNULL([Extent1].[ValueAcquisition], 0.00)) 
		END AS 'ValorContabil'
		FROM [dbo].[Asset] AS [Extent1] WITH(NOLOCK)    
		INNER JOIN [dbo].[AssetMovements] AS [Extent2] WITH(NOLOCK) ON[Extent1].[Id] = [Extent2].[AssetId]    
		INNER JOIN [dbo].[MovementType] AS [Extent14] WITH(NOLOCK) ON [Extent14].[Id] = [Extent2].[MovementTypeId]
		LEFT OUTER JOIN [dbo].[AuxiliaryAccount] AS [Extent11] WITH(NOLOCK) ON [Extent11].[Id] = [Extent2].[AuxiliaryAccountId]
		
		WHERE  ([Extent1].[flagVerificado] IS NULL)        
		  AND ([Extent1].[flagDepreciaAcumulada] = 1)
		  AND ([Extent2].[flagEstorno] IS NULL)
		  AND ([Extent2].[DataEstorno] IS NULL) 
		  AND ([Extent2].[Status] = 1)  
		  AND ([Extent2].[MovementTypeId] NOT IN (47,56,57)) --Não contabilizar transferência q pedem aceite no SAM
		  AND ([Extent2].[InstitutionId] = @IdOrgao)
		  AND ([Extent2].[BudgetUnitId] = @IdUO OR @IdUO IS NULL)  
		  AND ([Extent2].[ManagerUnitId] = @IdUGE OR @IdUGE IS NULL)
		  AND ([Extent11].[ContaContabilApresentacao] = @ContaContabil)
	) [visao_geral]

declare @IdRegistro int;

select TOP 1 @IdRegistro = Id from SaldoContabilAtualUGEContaContabil
     where IdOrgao = @IdOrgao
	 and IdUO = @IdUO
	 and IdUGE = @IdUGE
	 and ContaContabil = @ContaContabil


	IF( @IdRegistro is not null)
	BEGIN
		update SaldoContabilAtualUGEContaContabil
		set ValorContabil = @Valor
		where id = @IdRegistro;
	END
	ELSE
	BEGIN 
		insert into SaldoContabilAtualUGEContaContabil
		(
		   [IdOrgao]
	      ,[IdUO]
	      ,[IdUGE]
	      ,[CodigoOrgao]
	      ,[CodigoUO]
	      ,[CodigoUGE]
	      ,[ContaContabil]
	      ,[ValorContabil]
		)
		VALUES(
			@IdOrgao,
			@IdUO,
			@IdUGE,
			(select top 1 Code from Institution where id = @IdOrgao),
			(select top 1 Code from BudgetUnit where id = @IdUO),
			(select top 1 Code from ManagerUnit where id = @IdUGE),
			@ContaContabil,
			@Valor
		);
	END

END
GO

GRANT EXECUTE ON [dbo].[ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_CONTABIL_HOJE] TO [USUSAM]
GO