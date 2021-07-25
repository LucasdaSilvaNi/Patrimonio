SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'INSERT_RELATORIO_CONTABIL')
DROP PROCEDURE [dbo].[INSERT_RELATORIO_CONTABIL]  
GO

CREATE PROCEDURE [dbo].[INSERT_RELATORIO_CONTABIL]
	@MonthReference NVARCHAR(6)
  , @ManagerUnitId  INT
AS

SET DATEFORMAT YMD;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; 

DECLARE @AssetIdsEstornados TABLE(
	AssetId INT,
	AuxiliaryAccountId INT
)
--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
	--BEGIN CODE --
	INSERT INTO @AssetIdsEstornados(AssetId,AuxiliaryAccountId)  
	SELECT [mov].[AssetId],[mov].[AuxiliaryAccountId] FROM [AssetMovements] [mov] WITH(NOLOCK)
			WHERE [mov].[ManagerUnitId] = @ManagerUnitId 
			  AND [mov].[MovementTypeId] = 5 --- Inventário Inicial
			  AND ([mov].[FlagEstorno] IS NOT NULL ) AND ([mov].[DataEstorno] IS NOT NULL)
		GROUP BY [mov].[AssetId],[mov].[AuxiliaryAccountId]

DECLARE @Table AS TABLE(
	Id INT NOT NULL IDENTITY(1,1),
	ClosingId NVARCHAR(100) NULL,
	DepreciationAccount INT NULL,
	DepreciationDescription NVARCHAR(500) NULL,
	DepreciationAccountStatus BIT NULL,
	BookAccount INT NOT NULL,
	AccountingDescription NVARCHAR(500),
	BookAccountStatus BIT NOT NULL,
	DepreciationMonth DECIMAL(18,2),
	AccumulatedDepreciation DECIMAL(18,2),
	AccountingValue DECIMAL(18,2)

)


INSERT INTO @Table(ClosingId
                ,DepreciationAccount
				,DepreciationDescription
				,DepreciationAccountStatus
				,BookAccount
				,AccountingDescription
				,BookAccountStatus
				,AccountingValue
				,DepreciationMonth
				,AccumulatedDepreciation)

SELECT  (CASE WHEN  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao] IS NULL THEN NEWID() ELSE NULL END) AS 'Id'
	        , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]									AS 'ContaContabilDepreciacao'				/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]						AS 'ContaContabilDepreciacaoDescricao'		/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoStatus]							AS 'ContaContabilDepreciacaoStatus'
			, (select top 1 ContaContabilApresentacao from AuxiliaryAccount where BookAccount = [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]
																			AND Description like [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]
																			AND Status = [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]
			   )																						AS 'ContaContabil'							/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]									AS 'ContaContabilDescricao'					/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]										AS 'ContaContabilStatus'
			, SUM([visaoGeral_UGE__MESREF_ATUAL].[ValorContabil])										AS 'ValorContabil'							/*CAMPO RELATORIO*/
			--, SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoAcumuladaAteMesAnterior])				AS 'DepreciacaoAcumuladaAteMesAnterior'		/*CAMPO RELATORIO*/
			, ISNULL(SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoNoMes]),0)							AS 'DepreciacaoNoMes'						/*CAMPO RELATORIO*/
			, ISNULL(SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoAcumulada]),0)						AS 'DepreciacaoAcumulada'					/*CAMPO RELATORIO*/
	FROM
		(
			SELECT  
					 --(SELECT sigla.Name FROM Initial sigla WITH(NOLOCK) WHERE  bemPatrimonial.InitialId = sigla.Id)																														AS 'Sigla',
					 --(SELECT CONVERT(INT,bemPatrimonial.NumberIdentification) FROM Asset bemPatrimonial WITH(NOLOCK) WHERE bemPatrimonial.Id = movimentacaoBP.AssetId)																	AS 'Chapa',
					 (SELECT [contaContabilDepreciacao].[Code]			FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId 
																																											FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																											WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacao'
					, (SELECT [contaContabilDepreciacao].[Description]	FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId
																																											FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																											WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacaoDescricao'
					, (SELECT [contaContabilDepreciacao].[Status]		FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId
																																											FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																											WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacaoStatus'
																																											
					, (SELECT [contaContabil].[BookAccount] FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																	AS 'ContaContabil'
					, (SELECT [contaContabil].[Description] FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																	AS 'ContaContabilDescricao'
					, (SELECT [contaContabil].[Status]		FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																	AS 'ContaContabilStatus'

					, CASE 
							WHEN movimentacaoBP.AuxiliaryAccountId = 212508 
							THEN  (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[CurrentValue],0) 'ValorMensal'			
										  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
										  WHERE 
												ManagerUnitId = movimentacaoBP.[ManagerUnitId]
										  AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
										  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId] 
										  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <=
										  (select top 1 (CAST(CONVERT(CHAR(6), MovimentDate, 112) AS INT)) from AssetMovements with(nolock)
										  where AssetId = bemPatrimonial.Id AND 
											    FlagEstorno IS NULL AND
												MovementTypeId = 19))
										 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
									)
							ELSE (SELECT TOP 1 ISNULL([bemPatrimonial].[ValueAcquisition], 0.00))
					END AS 'ValorContabil'
					, 
					CASE 
						WHEN movimentacaoBP.AuxiliaryAccountId = 212508 THEN 0
						ELSE (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[RateDepreciationMonthly],0) 'ValorMensal'			
									  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
									  WHERE 
											ManagerUnitId = movimentacaoBP.[ManagerUnitId]
									  AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
									  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId] 
									  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) = CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
									 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
						      ) 
						END AS 'DepreciacaoNoMes'
					, 
					CASE 
						WHEN movimentacaoBP.AuxiliaryAccountId = 212508 THEN 0
						ELSE (SELECT TOP 1 [dadosDepreciacaoMensal].[AccumulatedDepreciation] 'ValorAcumulado'			
									  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
									  WHERE 
											ManagerUnitId = movimentacaoBP.[ManagerUnitId]
											AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
									  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId]
									  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
									 ORDER BY [dadosDepreciacaoMensal].[Id] DESC

							 )
					END AS 'DepreciacaoAcumulada'
					--LINHA 'DEBUG' MES-REFERENCIA MOVIMENTACAO NA UGE CONSULTADA
					, CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112)																																						AS 'MesReferenciaMovimentacao'
			FROM 
				AssetMovements movimentacaoBP WITH(NOLOCK)
			INNER JOIN Asset bemPatrimonial WITH(NOLOCK) ON movimentacaoBP.AssetId = bemPatrimonial.Id
			WHERE (bemPatrimonial.[flagVerificado] IS NULL)        
		      AND (bemPatrimonial.[flagDepreciaAcumulada] = 1)
			  AND movimentacaoBP.Id IN (SELECT MAX(Id)
									  FROM 
										AssetMovements movimentacaoBP WITH(NOLOCK)
									  WHERE ManagerUnitId = @ManagerUnitId
										AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
										--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
										--BEGIN CODE --
										AND (movimentacaoBP.AssetId NOT IN(SELECT [AssetId] FROM @AssetIdsEstornados WHERE [AuxiliaryAccountId] = movimentacaoBP.AuxiliaryAccountId)) --
										AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
										--END CODE --
									  GROUP BY AssetId
									 )
			AND (movimentacaoBP.ManagerUnitId = @ManagerUnitId)
			--AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
			
			AND (movimentacaoBP.MovementTypeId NOT IN (
															11,	--Transfêrencia
															12,	--Doação
															15,	--Extravio
															16,	--Obsoleto
															17,	--Danificado
															18,	--Sucata
															23, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)  
															24, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)  
															27, --COMODATOS/DE TERCEIROS RECEBIDOS
															42,	--SAÍDA INSERVÍVEL DA UGE - DOAÇÃO																		     
															43,	--SAÍDA INSERVÍVEL DA UGE - TRANSFERÊNCIA																		     
															44,	--COMODATO - CONCEDIDOS - PATRIMONIADO																		     
															45,	--COMODATO/DE TERCEIROS - RECEBIDOS																		     
															46,	--DOAÇÃO CONSOLIDAÇÃO - PATRIMÔNIO																		     
															47,	--DOAÇÃO INTRA - NO - ESTADO - PATRIMÔNIO																	     
															48,	--DOAÇÃO MUNÍCIPIO - PATRIMÔNIO
															49,	--DOAÇÃO OUTROS ESTADOS - PATRIMÔNIO
															50,	--DOAÇÃO UNIÃO - PATRIMÔNIO
															51,	--ESTRAVIO, FURTO, ROUBO - PATRIMONIADO
															52,	--MORTE ANIMAL - PATRIMONIADO
															53,	--MORTE VEGETAL - PATRIMONIADO
															54,	--MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO
															55,	--SEMENTES, PLANTAS, INSUMOS E ÁRVORES
															56,	--TRANSFERÊNCIA OUTRO ÓRGÃO - PATRIMONIADO
															57,	--TRANSFERÊNCIA MESMO ÓRGÃO - PATRIMONIADO
															58,	--PERDAS INVOLUNTÁRIAS - BENS MÓVEIS
															59,	--PERDAS INVOLUNTÁRIAS - INSERVÍVEL BENS MÓVEIS
															81,
															91,
															95))
		AND (
				movimentacaoBP.ManagerUnitId = @ManagerUnitId
				AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
				AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
			)
		) AS visaoGeral_UGE__MESREF_ATUAL
	GROUP BY
			  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoStatus]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]
	ORDER BY 
			  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]


DECLARE @Count INT = 0;
DECLARE @Contador INT  = 0;
DECLARE @Id INT;



	 INSERT INTO [dbo].[AccountingClosing]
           ([ManagerUnitCode]
           ,[ManagerUnitDescription]
           ,[DepreciationAccount]
           ,[DepreciationMonth]
           ,[AccumulatedDepreciation]
           ,[Status]
           ,[DepreciationDescription]
           ,[ReferenceMonth]
		   ,[ItemAccounts]
		   ,[AccountName]
		   ,[GeneratedNL]
		   ,[ClosingId]
		   ,[ManagerCode])
	SELECT 
	     (SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId )
        ,(SELECT [Description] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
        ,[tab].DepreciationAccount
        ,SUM([tab].DepreciationMonth)
        ,SUM([tab].AccumulatedDepreciation)
        ,[tab].DepreciationAccountStatus
        ,[tab].DepreciationDescription
        ,@MonthReference
		,(SELECT TOP 1 [dep].[MaterialItemCode] 
		    FROM [dbo].[DepreciationMaterialItem] [dep] 
			WHERE [dep].[DepreciationAccount] = [tab].DepreciationAccount)
		,(SELECT TOP 1 [DescricaoContaDepreciacaoContabilizaSP] FROM [dbo].[DepreciationAccount] WHERE  [Code]= [tab].DepreciationAccount)
		,NULL
		,NULL
		,(SELECT TOP 1 [ManagerCode] FROM [dbo].[Institution] [it]
		   INNER JOIN [dbo].[BudgetUnit] [bu] ON [bu].[InstitutionId] = [it].[Id] 
		   INNER JOIN [dbo].[ManagerUnit] [ma] ON [ma].[BudgetUnitId] = [bu].[Id] 
		   WHERE [ma].[Id] = @ManagerUnitId)
	 FROM @Table [tab] WHERE [tab].[ClosingId] IS NULL
	 GROUP BY
		[tab].DepreciationAccount
       ,[tab].DepreciationAccountStatus
       ,[tab].DepreciationDescription

	   INSERT INTO [dbo].[DepreciationAccountingClosing]
        ([AccountingClosingId]
        ,[BookAccount]
        ,[AccountingValue]
        ,[DepreciationMonth]
        ,[AccumulatedDepreciation]
        ,[Status]
        ,[AccountingDescription]
		,[ReferenceMonth]
		,[ManagerUnitCode]
		,[ManagerDescription])

	SELECT [clg].[Id]
		,[tbl].[BookAccount]
		,ISNULL([tbl].[AccountingValue],0)
		,[tbl].[DepreciationMonth]
		,[tbl].[AccumulatedDepreciation] 
		,[tbl].[BookAccountStatus]
		,[tbl].[AccountingDescription]
		,@MonthReference
		,(SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId )
        ,(SELECT [Description] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
	FROM [dbo].[AccountingClosing] [clg] 
    INNER JOIN @Table [tbl] ON [clg].[DepreciationAccount] = [tbl].[DepreciationAccount]
				AND [clg].[ReferenceMonth] = @MonthReference
				and [clg].[ManagerUnitCode] = (SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId )
				and [clg].ClosingId IS NULL

SET @Count = (SELECT COUNT(*) FROM @Table WHERE DepreciationAccount IS NULL)

WHILE(@Contador < @Count)
BEGIN
	SET @Id = (SELECT TOP 1 [Id] FROM @Table);

	
	 INSERT INTO [dbo].[AccountingClosing]
           ([ManagerUnitCode]
           ,[ManagerUnitDescription]
           ,[DepreciationAccount]
           ,[DepreciationMonth]
           ,[AccumulatedDepreciation]
           ,[Status]
           ,[DepreciationDescription]
           ,[ReferenceMonth]
		   ,[ItemAccounts]
		   ,[AccountName]
		   ,[GeneratedNL]
		   ,[ClosingId]
		   ,[ManagerCode])
	SELECT 
	     (SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId )
        ,(SELECT [Description] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
        ,[tab].DepreciationAccount
        ,SUM([tab].DepreciationMonth)
        ,SUM([tab].AccumulatedDepreciation)
        ,[tab].DepreciationAccountStatus
        ,[tab].DepreciationDescription
        ,@MonthReference
		,(SELECT TOP 1 [dep].[MaterialItemCode] FROM [dbo].[DepreciationMaterialItem] [dep] WHERE [dep].[DepreciationAccount] = [tab].DepreciationAccount)
		,(SELECT TOP 1 [DescricaoContaDepreciacaoContabilizaSP] FROM [dbo].[DepreciationAccount] WHERE  [Code]= [tab].DepreciationAccount)
		,NULL
		,ClosingId
		,(SELECT TOP 1 [ManagerCode] FROM [dbo].[Institution] [it]
		   INNER JOIN [dbo].[BudgetUnit] [bu] ON [bu].[InstitutionId] = [it].[Id] 
		   INNER JOIN [dbo].[ManagerUnit] [ma] ON [ma].[BudgetUnitId] = [bu].[Id] 
		   WHERE [ma].[Id] = @ManagerUnitId)
	 FROM @Table [tab] WHERE [tab].[Id] = @Id
	 GROUP BY
		[tab].DepreciationAccount
       ,[tab].DepreciationAccountStatus
       ,[tab].DepreciationDescription
	   ,[tab].ClosingId

	INSERT INTO [dbo].[DepreciationAccountingClosing]
        ([AccountingClosingId]
        ,[BookAccount]
        ,[AccountingValue]
        ,[DepreciationMonth]
        ,[AccumulatedDepreciation]
        ,[Status]
        ,[AccountingDescription]
		,[ReferenceMonth]
		,[ManagerUnitCode]
		,[ManagerDescription])

	SELECT [clg].[Id]
		,[tbl].[BookAccount]
		,ISNULL([tbl].[AccountingValue],0)
		,[tbl].[DepreciationMonth]
		,[tbl].[AccumulatedDepreciation] 
		,[tbl].[BookAccountStatus]
		,[tbl].[AccountingDescription]
		,@MonthReference
		,(SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId )
        ,(SELECT [Description] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
	FROM [dbo].[AccountingClosing] [clg] 
    INNER JOIN @Table [tbl] ON [clg].ClosingId = [tbl].ClosingId
	
	DELETE FROM @Table WHERE [Id] = @Id 
	SET @Contador = @Contador + 1;
END;
DELETE FROM @Table;

declare @tabelaValorBPSTerceiros as table
( descricao varchar(40),
  valorDaConta decimal(18,2)
);

insert into @tabelaValorBPSTerceiros(descricao, valorDaConta)
exec [PRC_RELATORIO__BP_TERCEIRO]
@ManagerUnitId = @ManagerUnitId,
@MonthReference = @MonthReference

IF((select count(*) from @tabelaValorBPSTerceiros) > 0)
BEGIN 
	Declare @ContaContabilTerceiro int, @StatusDaConta bit, 
			@ValorContaTerceiro decimal(18,2), @DescriptionDaConta [varchar](100);

	select top 1 
	@ContaContabilTerceiro = BookAccount, 
	@StatusDaConta = [Status],
	@DescriptionDaConta = [Description]
	from AuxiliaryAccount where RelacionadoBP = 2;

	select top 1 @ValorContaTerceiro = valorDaConta from @tabelaValorBPSTerceiros;

	IF( (select count(*) from DepreciationAccountingClosing
		WHERE BookAccount = @ContaContabilTerceiro
		and ManagerUnitCode = (select top 1 Code from ManagerUnit where id = @ManagerUnitId)
		and ReferenceMonth = @MonthReference) > 0)
	BEGIN
		update DepreciationAccountingClosing
		set AccountingValue = @ValorContaTerceiro
		where BookAccount = @ContaContabilTerceiro
		and ManagerUnitCode = (select top 1 Code from ManagerUnit where id = @ManagerUnitId)
		and ReferenceMonth = @MonthReference
	END
	ELSE
	BEGIN
	
	Declare @novoGUId [uniqueidentifier];
	SET @novoGUId = NEWID();

	INSERT INTO [dbo].[AccountingClosing]
           ([ManagerUnitCode]
           ,[ManagerUnitDescription]
           ,[DepreciationAccount]
           ,[DepreciationMonth]
           ,[AccumulatedDepreciation]
           ,[Status]
           ,[DepreciationDescription]
           ,[ReferenceMonth]
		   ,[ItemAccounts]
		   ,[AccountName]
		   ,[GeneratedNL]
		   ,[ClosingId]
		   ,[ManagerCode])
	VALUES(
	     (SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId )
        ,(SELECT [Description] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
        ,NULL --Conta Contábil de Terceiro não tem Conta de Depreciação vinculada
        ,0 --BP Terceiro não deprecia
        ,0 --BP Terceiro não deprecia
        ,NULL --Conta Contábil de Terceiro não tem Conta de Depreciação vinculada
        ,NULL --Conta Contábil de Terceiro não tem Conta de Depreciação vinculada
        ,@MonthReference
		,NULL --Conta terceiro não manda pro Contabiliza
		,NULL --Conta terceiro não manda pro Contabiliza
		,NULL
		,@novoGUId
		,(SELECT TOP 1 [ManagerCode] FROM [dbo].[Institution] [it]
		   INNER JOIN [dbo].[BudgetUnit] [bu] ON [bu].[InstitutionId] = [it].[Id] 
		   INNER JOIN [dbo].[ManagerUnit] [ma] ON [ma].[BudgetUnitId] = [bu].[Id] 
		   WHERE [ma].[Id] = @ManagerUnitId)
	);

	INSERT INTO [dbo].[DepreciationAccountingClosing]
        ([AccountingClosingId]
        ,[BookAccount]
        ,[AccountingValue]
        ,[DepreciationMonth]
        ,[AccumulatedDepreciation]
        ,[Status]
        ,[AccountingDescription]
		,[ReferenceMonth]
		,[ManagerUnitCode]
		,[ManagerDescription])

	SELECT [clg].[Id]
		,@ContaContabilTerceiro
		,@ValorContaTerceiro
		,[clg].[DepreciationMonth]
		,[clg].[AccumulatedDepreciation] 
		,@StatusDaConta
		,@DescriptionDaConta
		,@MonthReference
		,(SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
        ,(SELECT [Description] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
	FROM [dbo].[AccountingClosing] [clg] 
	where [clg].ClosingId = @novoGUId;

	END

END

GO

GRANT EXECUTE ON [dbo].[INSERT_RELATORIO_CONTABIL] TO [USUSAM]
GO