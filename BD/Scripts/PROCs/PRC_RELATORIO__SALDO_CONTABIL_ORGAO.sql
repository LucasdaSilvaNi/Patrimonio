SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PRC_RELATORIO__SALDO_CONTABIL_ORGAO')
	DROP PROCEDURE [dbo].[PRC_RELATORIO__SALDO_CONTABIL_ORGAO] 
GO

--Cópia do primeiro quadrante do RELATORIO RESUMO CONSOLIDADO, a 'nível' de Orgao  
--Conforme pedido em 18/02/2020, deverá trazer os acervos e os terceiros  
CREATE PROCEDURE [dbo].[PRC_RELATORIO__SALDO_CONTABIL_ORGAO]  
(  
   @InstitutionId INT,  
   @MonthReference VARCHAR(6) = NULL  
) AS  
  
DECLARE @MesRefFormatado VARCHAR(7);
DECLARE @AssetIdsEstornados TABLE(  
 AssetId INT,  
 AuxiliaryAccountId INT  
)  
Declare @UGEsComMesFechado TABLE(  
 Id int,  
 MesRef [varchar](6)  
);
  
BEGIN  
 SET DATEFORMAT YMD  

Declare @IntegradoAoSIAFEM bit;

select @IntegradoAoSIAFEM = flagIntegracaoSiafem 
from Institution where id = @InstitutionId;

IF(@IntegradoAoSIAFEM = 1)
BEGIN
	Declare @UGEDadosParaMostarNoExcel as table(
		Descricao [varchar](120),
		Codigo [varchar](6),
		IdDaUO int,
		MesRef [varchar](6)
	);

	IF(@MonthReference is null)  
	 BEGIN    
	  --PEGANDO O MAIOR MES-REFERENCIA DAS UGEs DO ORGAO  
	  SET @MonthReference =  (SELECT MAX(ManagerUnit.ManagmentUnit_YearMonthReference)   
	        FROM ManagerUnit   
	        WHERE BudgetUnitId IN (SELECT Id   
	              FROM BudgetUnit  
	              WHERE InstitutionId = @InstitutionId))   
	  
	  insert into @UGEDadosParaMostarNoExcel(Descricao, Codigo, IdDaUO, MesRef)  
	  SELECT [Description], Code, BudgetUnitId, ManagmentUnit_YearMonthReference
	  FROM ManagerUnit   
	  WHERE BudgetUnitId IN (SELECT Id FROM BudgetUnit WHERE InstitutionId = @InstitutionId)  
	  AND ManagmentUnit_YearMonthReference = @MonthReference;  
	 END  
	 ELSE  
	 BEGIN  
	  insert into @UGEDadosParaMostarNoExcel(Descricao, Codigo, IdDaUO, MesRef)  
	  SELECT [Description],Code, BudgetUnitId, ManagmentUnit_YearMonthReference
	  FROM ManagerUnit   
	  WHERE BudgetUnitId IN (SELECT Id FROM BudgetUnit WHERE InstitutionId = @InstitutionId)
	  AND convert(int,ManagmentUnit_YearMonthReference) > convert(int,@MonthReference)
	 END  

	 SET @MesRefFormatado = (substring(@MonthReference,5,2) + '/' + substring(@MonthReference,1,4))

	 select * from (
	 select
	 (SELECT [orgaoSIAFEM].[Code] FROM Institution orgaoSIAFEM WITH(NOLOCK)  WHERE [orgaoSIAFEM].[Id] = @InstitutionId) AS 'Orgao',
	 (SELECT [UO].[Code] FROM BudgetUnit UO WITH(NOLOCK)  WHERE [UO].[Id] = temp.IdDaUO) AS 'UO',
		temp.Codigo AS 'UGE', 
		temp.Descricao as 'NomeUGE'  ,
		@MesRefFormatado 'MesRef', 
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
		inner join @UGEDadosParaMostarNoExcel temp
		on d.ManagerUnitCode = temp.Codigo
		where d.ReferenceMonth = @MonthReference
	) [dados] 
	order by
	 	 [dados].[Orgao]   
	   , [dados].[UO]    
	   , [dados].[UGE]    
	   , [dados].[NomeUGE]  
	   , [dados].[MesRef]  
	   , [dados].[ContaContabilDepreciacao]  
	   , [dados].[ContaContabil]

END
ELSE
BEGIN
	 
	 IF(@MonthReference is null)  
	 BEGIN    
	  --PEGANDO O MAIOR MES-REFERENCIA DAS UGEs DO ORGAO  
	  SET @MonthReference =  (SELECT MAX(ManagerUnit.ManagmentUnit_YearMonthReference)   
	        FROM ManagerUnit   
	        WHERE BudgetUnitId IN (SELECT Id   
	              FROM BudgetUnit  
	              WHERE InstitutionId = @InstitutionId))   
	  
	  insert into @UGEsComMesFechado(Id,MesRef)  
	  SELECT Id, ManagmentUnit_YearMonthReference   
	  FROM ManagerUnit   
	  WHERE BudgetUnitId IN (SELECT Id FROM BudgetUnit WHERE InstitutionId = @InstitutionId)  
	  AND ManagmentUnit_YearMonthReference = @MonthReference;  
	 END  
	 ELSE  
	 BEGIN  
	  insert into @UGEsComMesFechado(Id,MesRef)  
	  SELECT Id, ManagmentUnit_YearMonthReference   
	  FROM ManagerUnit   
	  WHERE BudgetUnitId IN (SELECT Id FROM BudgetUnit WHERE InstitutionId = @InstitutionId)  
	  AND convert(int,ManagmentUnit_YearMonthReference) > convert(int,@MonthReference)  
	 END  
	  
	 ----TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'  
	 ----BEGIN CODE --  
	 INSERT INTO @AssetIdsEstornados(AssetId,AuxiliaryAccountId)    
	 SELECT [mov].[AssetId],[mov].[AuxiliaryAccountId] FROM [AssetMovements] [mov] WITH(NOLOCK)  
	   WHERE [mov].[ManagerUnitId] IN(select Id from @UGEsComMesFechado)  
	     AND [mov].[MovementTypeId] = 5 --- Inventário Inicial  
	     AND ([mov].[FlagEstorno] IS NOT NULL ) AND ([mov].[DataEstorno] IS NOT NULL)  
	  GROUP BY [mov].[AssetId],[mov].[AuxiliaryAccountId]  
	 ----END CODE --  
	
	SET @MesRefFormatado = (substring(@MonthReference,5,2) + '/' + substring(@MonthReference,1,4))
	  
	 SELECT   
	     CONVERT(VARCHAR(2), [visaoGeral_UGE__MESREF_ATUAL].[Orgao])             AS 'Orgao'         /*CAMPO RELATORIO*/  
	   , CONVERT(VARCHAR(5), [visaoGeral_UGE__MESREF_ATUAL].[UO])              AS 'UO'          /*CAMPO RELATORIO*/  
	   , CONVERT(VARCHAR(6), [visaoGeral_UGE__MESREF_ATUAL].[UGE])              AS 'UGE'         /*CAMPO RELATORIO*/  
	   , [visaoGeral_UGE__MESREF_ATUAL].[NomeUGE]                  AS 'NomeUGE'        /*CAMPO RELATORIO*/  
	   , [visaoGeral_UGE__MESREF_ATUAL].[MesRef]                  AS 'MesRef'       /*CAMPO RELATORIO*/  
	   , CONVERT(VARCHAR(10), [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao])        AS 'ContaContabilDepreciacao'    /*CAMPO RELATORIO*/  
	   , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]           AS 'ContaContabilDepreciacaoDescricao'  /*CAMPO RELATORIO*/  
	   , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]                                AS 'ContaContabil'       /*CAMPO RELATORIO*/  
	   , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]              AS 'ContaContabilDescricao'     /*CAMPO RELATORIO*/  
	   , SUM([visaoGeral_UGE__MESREF_ATUAL].[ValorContabil])               AS 'ValorContabil'       /*CAMPO RELATORIO*/  
	   , ISNULL(SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoNoMes]),0.0)           AS 'DepreciacaoNoMes'      /*CAMPO RELATORIO*/  
	   , ISNULL(SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoAcumulada]),0.0)          AS 'DepreciacaoAcumulada'     /*CAMPO RELATORIO*/  
	 FROM  
	  (  
	   SELECT    
	       (SELECT [orgaoSIAFEM].[Code] FROM Institution orgaoSIAFEM WITH(NOLOCK)  WHERE [orgaoSIAFEM].[Id] = movimentacaoBP.InstitutionId)                    AS 'Orgao'  
	     , (SELECT [uoSIAFEM].[Code]  FROM BudgetUnit uoSIAFEM WITH(NOLOCK)   WHERE [uoSIAFEM].[Id] = movimentacaoBP.BudgetUnitId)                     AS 'UO'  
	     , (SELECT [ugeSIAFEM].[Code] FROM ManagerUnit ugeSIAFEM WITH(NOLOCK)   WHERE [ugeSIAFEM].[Id] = movimentacaoBP.ManagerUnitId)                     AS 'UGE'  
	     , (SELECT [ugeSIAFEM].[Description] FROM ManagerUnit ugeSIAFEM WITH(NOLOCK)   WHERE [ugeSIAFEM].[Id] = movimentacaoBP.ManagerUnitId)                    AS 'NomeUGE'  
	     , @MesRefFormatado 'MesRef'  
	     , (SELECT [contaContabilDepreciacao].[Id] FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK)   
	     INNER JOIN AuxiliaryAccount contaContabil WITH(NOLOCK) ON contaContabil.DepreciationAccountId = contaContabilDepreciacao.Id  
	     WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId  
	     ) AS 'ContaContabilDepreciacaoId'  
	  
	     , (SELECT [contaContabilDepreciacao].[Code]     
	          FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK)   
	       INNER JOIN AuxiliaryAccount contaContabil WITH(NOLOCK)  ON contaContabil.DepreciationAccountId = contaContabilDepreciacao.Id  
	       WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId  
	       ) AS 'ContaContabilDepreciacao'  
	  
	     , (SELECT [contaContabilDepreciacao].[Description]   
	          FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK)   
	       INNER JOIN AuxiliaryAccount contaContabil WITH(NOLOCK)  ON contaContabil.DepreciationAccountId = contaContabilDepreciacao.Id  
	       WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId  
	     ) AS 'ContaContabilDepreciacaoDescricao'  
	  
	     , (SELECT [contaContabil].[Id] FROM AuxiliaryAccount contaContabil WITH(NOLOCK)   
	         WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)                    AS 'ContaContabilId'                                             
	     , (SELECT [contaContabil].[ContaContabilApresentacao] FROM AuxiliaryAccount contaContabil WITH(NOLOCK)   
	         WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)                 AS 'ContaContabil'  
	     , (SELECT [contaContabil].[Description] FROM AuxiliaryAccount contaContabil WITH(NOLOCK)   
	         WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)                 AS 'ContaContabilDescricao'  
	     , CASE 
			WHEN bemPatrimonial.flagAcervo = 1 THEN (SELECT TOP 1 ISNULL([bemPatrimonial].[ValueAcquisition], 0.00))
			WHEN bemPatrimonial.flagTerceiro = 1 THEN (SELECT TOP 1 ISNULL([bemPatrimonial].[ValueAcquisition], 0.00))
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
	     , CASE   
	      WHEN [bemPatrimonial].[flagAcervo] = 1 THEN 0  
	      WHEN [bemPatrimonial].[flagTerceiro] = 1 THEN 0  
		  WHEN [movimentacaoBP].AuxiliaryAccountId = 212508 THEN 0
	      ELSE  
	      (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[RateDepreciationMonthly],0) 'ValorMensal'     
	           FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK)   
	           WHERE   
	           ManagerUnitId = movimentacaoBP.[ManagerUnitId]  
	           AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]  
	           AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId]   
	           AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT))   
	           --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO  
	          ORDER BY [dadosDepreciacaoMensal].[Id] DESC  
	      )  
	      END                        AS 'DepreciacaoNoMes'  
	     ,  
	     CASE   
	      WHEN [bemPatrimonial].[flagAcervo] = 1 THEN 0  
	      WHEN [bemPatrimonial].[flagTerceiro] = 1 THEN 0  
	      WHEN [movimentacaoBP].AuxiliaryAccountId = 212508 THEN 0
		  ELSE   
	      (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[AccumulatedDepreciation],0) 'ValorAcumulado'     
	           FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK)   
	           WHERE   
	           ManagerUnitId = movimentacaoBP.[ManagerUnitId]  
	           AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]  
	           AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId]  
	           AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO  
	          ORDER BY [dadosDepreciacaoMensal].[Id] DESC  
	  
	      )  
	     END                          AS 'DepreciacaoAcumulada'  
	     , CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112)                                        AS 'MesReferenciaMovimentacao'  
	   FROM   
	    AssetMovements movimentacaoBP WITH(NOLOCK)  
	   INNER JOIN Asset bemPatrimonial WITH(NOLOCK,INDEX(IDX_ASSET_RELATORIO_CONTAS_CONTABEIS)) ON movimentacaoBP.AssetId = bemPatrimonial.Id  
	   WHERE  
	        (bemPatrimonial.[flagVerificado] IS NULL)          
	        AND (bemPatrimonial.[flagDepreciaAcumulada] = 1)  
	     AND  movimentacaoBP.Id IN (SELECT MAX(Id)  
	           FROM   
	          AssetMovements movimentacaoBP WITH(NOLOCK)  
	           WHERE  movimentacaoBP.InstitutionId = @InstitutionId  
	             and movimentacaoBP.ManagerUnitId IN (select Id from @UGEsComMesFechado)  
	          AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)  
	          --TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'  
	          --BEGIN CODE --  
	          AND (movimentacaoBP.AssetId NOT IN(SELECT [AssetId] FROM @AssetIdsEstornados WHERE [AuxiliaryAccountId] = movimentacaoBP.AuxiliaryAccountId)) --  
	          AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL)) --RETIRANDO ESTORNOS DE MOVIMENTACAOES  
	          --END CODE --  
	           GROUP BY AssetId  
	          )  
	  
	   AND (movimentacaoBP.InstitutionId = @InstitutionId)  
	   AND (movimentacaoBP.ManagerUnitId IN (select Id from @UGEsComMesFechado))  
	   AND (movimentacaoBP.MovementTypeId NOT IN (  
	               11, --Transfêrencia  
	               12, --Doação  
	               15, --Extravio  
	               16, --Obsoleto  
	               17, --Danificado  
	               18, --Sucata  
	               23, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)    
	               24, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)    
	               42, --SAÍDA INSERVÍVEL DA UGE - DOAÇÃO                         
	               43, --SAÍDA INSERVÍVEL DA UGE - TRANSFERÊNCIA                         
	               44, --COMODATO - CONCEDIDOS - PATRIMONIADO                         
	               45, --COMODATO/DE TERCEIROS - RECEBIDOS                         
	               46, --DOAÇÃO CONSOLIDAÇÃO - PATRIMÔNIO                         
	               47, --DOAÇÃO INTRA - NO - ESTADO - PATRIMÔNIO                        
	               48, --DOAÇÃO MUNÍCIPIO - PATRIMÔNIO  
	               49, --DOAÇÃO OUTROS ESTADOS - PATRIMÔNIO  
	               50, --DOAÇÃO UNIÃO - PATRIMÔNIO  
	               51, --ESTRAVIO, FURTO, ROUBO - PATRIMONIADO  
	               52, --MORTE ANIMAL - PATRIMONIADO  
	               53, --MORTE VEGETAL - PATRIMONIADO  
	               54, --MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO  
	               55, --SEMENTES, PLANTAS, INSUMOS E ÁRVORES  
	               56, --TRANSFERÊNCIA OUTRO ÓRGÃO - PATRIMONIADO  
	               57, --TRANSFERÊNCIA MESMO ÓRGÃO - PATRIMONIADO  
	               58, --PERDAS INVOLUNTÁRIAS - BENS MÓVEIS  
	               59, --PERDAS INVOLUNTÁRIAS - INSERVÍVEL BENS MÓVEIS  
	               81,  
	               91,
				   95))  
	  ) AS visaoGeral_UGE__MESREF_ATUAL  
	 GROUP BY  
	     [visaoGeral_UGE__MESREF_ATUAL].[Orgao]   
	   , [visaoGeral_UGE__MESREF_ATUAL].[UO]    
	   , [visaoGeral_UGE__MESREF_ATUAL].[UGE]    
	   , [visaoGeral_UGE__MESREF_ATUAL].[NomeUGE]  
	   , [visaoGeral_UGE__MESREF_ATUAL].[MesRef]  
	   , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoId]  
	   , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]  
	   , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]  
	   , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilId]  
	   , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]  
	   , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]  
	 ORDER BY   
	     [visaoGeral_UGE__MESREF_ATUAL].[Orgao]   
	   , [visaoGeral_UGE__MESREF_ATUAL].[UO]    
	   , [visaoGeral_UGE__MESREF_ATUAL].[UGE]    
	   , [visaoGeral_UGE__MESREF_ATUAL].[NomeUGE]  
	   , [visaoGeral_UGE__MESREF_ATUAL].[MesRef]  
	   , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]  
	   , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]  
END

END
GO

GRANT EXECUTE ON [dbo].[PRC_RELATORIO__SALDO_CONTABIL_ORGAO] TO [ususam]
GO