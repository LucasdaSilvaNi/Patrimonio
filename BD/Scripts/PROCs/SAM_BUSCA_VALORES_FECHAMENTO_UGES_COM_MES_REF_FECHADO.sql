/****** Object:  StoredProcedure [dbo].[SAM_BUSCA_VALORES_FECHAMENTO_UGES_COM_MES_REF_FECHADO]    Script Date: 22/10/2020 12:11:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SAM_BUSCA_VALORES_FECHAMENTO_UGES_COM_MES_REF_FECHADO')
	DROP PROCEDURE [dbo].[SAM_BUSCA_VALORES_FECHAMENTO_UGES_COM_MES_REF_FECHADO]
GO

CREATE PROCEDURE [dbo].[SAM_BUSCA_VALORES_FECHAMENTO_UGES_COM_MES_REF_FECHADO]
(
	@IdOrgao int,
	@IdUO int = 0,
	@IdUGE int = 0,
	@MesRef varchar(6) = 0
)
AS
BEGIN
	DECLARE @Mes int;
	DECLARE @Ano int;

	IF(@MesRef IS NULL OR NOT ISNUMERIC(@MesRef) = 1)
	BEGIN
		DECLARE @DataAtual as datetime = GETDATE();
		SET @DataAtual = DATEADD(MONTH,-1,@DataAtual);

		SET @Mes = MONTH(@DataAtual);
		SET @Ano = YEAR(@DataAtual);
		 
		SET @MesRef = CONCAT(CONVERT(varchar,@Ano),REPLACE(@Mes, SPACE(1), '0'))
	END
	ELSE
	BEGIN
		SET @Mes = CONVERT(int,SUBSTRING(@MesRef,5,2));
		SET @Ano = CONVERT(int,SUBSTRING(@MesRef,1,4));
	END
	
	Declare @MesRefComoInteiro int = CONVERT(int,@MesRef);

	Declare @Codigos as table
	(
		Orgao varchar(6),
		UO varchar(6),
		UGE varchar(6)
	);

	insert into @Codigos(Orgao, UO, UGE)
	select i.Code 'Orgao', b.Code 'UO', m.Code 'UGE' 
	from ManagerUnit M with(nolock)
	inner join BudgetUnit b with(nolock)
	on b.Id = m.BudgetUnitId
	inner join Institution i with(nolock)
	on i.Id = b.InstitutionId
	where i.Id = @IdOrgao 
	and (@IdUO = 0 OR b.Id = @IdUO)
	and (@IdUGE = 0 OR m.Id = @IdUGE)
	and m.ManagmentUnit_YearMonthReference IS NOT NULL
	and convert(int,ManagmentUnit_YearMonthReference) >= @MesRefComoInteiro;

	IF((SELECT COUNT(*) FROM @Codigos) > 0)
	BEGIN
		Declare @Retorno as table 
		(Orgao varchar(6),
		 UO varchar(6),
		 UGE varchar(6),
		 Mes varchar(4),
		 Ano varchar(6),
		 Conta int,
		 DescricaoConta varchar(255),
		 ValorContabilSAM decimal(18,2));

		 Declare @ContasAMandarProSIAFEM as table(Conta int, DescricaoConta varchar(255))

		 insert into @ContasAMandarProSIAFEM (Conta, DescricaoConta)
		 select ContaContabil, Descricao from ContasContabeisPraConsultarNoSIAFEM with(nolock)
		 
		 Declare @contConta int;
		 select @contConta = count(Conta)
		 from @ContasAMandarProSIAFEM;

		 Declare @contaAtual int, @DescricaoContaAtual varchar(255);

		 while(@contConta > 0)
		 BEGIN
		 	select top 1 @contaAtual = Conta,
			@DescricaoContaAtual = DescricaoConta
		 	from @ContasAMandarProSIAFEM;
		 
		 		insert into @Retorno(
		 		Orgao,
		 		UO,
		 		UGE,
		 		Mes,
		 		Ano,
		 		Conta,
				DescricaoConta,
		 		ValorContabilSAM)
		 		select 
		 			Orgao ,
		 			UO,
		 			UGE,
		 			@Mes 'Mes',
		 			@Ano 'Ano',
		 			@contaAtual,
					@DescricaoContaAtual,
		 			0.0
		 		from @Codigos
		 	
		 		delete from @ContasAMandarProSIAFEM where Conta = @contaAtual;
		 		set @contConta = @contConta - 1;
		 END

		 insert into @ContasAMandarProSIAFEM (Conta, DescricaoConta)
		 select ContaDepreciacao, Descricao from ContasDepreciacoesPraConsultarSIAFEM with(nolock)

		 select @contConta = count(Conta)
		 from @ContasAMandarProSIAFEM;

		 while(@contConta > 0)
		 BEGIN
		 	select top 1 @contaAtual = Conta, 
			@DescricaoContaAtual = DescricaoConta
		 	from @ContasAMandarProSIAFEM;
		 
		 		insert into @Retorno(
		 		Orgao,
		 		UO,
		 		UGE,
		 		Mes,
		 		Ano,
		 		Conta,
				DescricaoConta,
		 		ValorContabilSAM)
		 		select 
		 			Orgao ,
		 			UO,
		 			UGE,
		 			@Mes 'Mes',
		 			@Ano 'Ano',
		 			@contaAtual,
					@DescricaoContaAtual,
		 			0.0
		 		from @Codigos
		 	
		 		delete from @ContasAMandarProSIAFEM where Conta = @contaAtual;
		 		set @contConta = @contConta - 1;
		 END

		 update temp 
		 set temp.ValorContabilSAM = a.AccumulatedDepreciation
		 from @Retorno temp
		 inner join AccountingClosing a
		 on temp.UGE = a.ManagerUnitCode
		 and temp.Conta = a.DepreciationAccount
		 where a.ReferenceMonth = @MesRef
		 and DepreciationAccount IS NOT NULL
		 AND ClosingId IS NULL

		 update temp 
		 set temp.ValorContabilSAM = a.AccountingValue
		 from @Retorno temp
		 inner join DepreciationAccountingClosing a
		 on temp.UGE = a.ManagerUnitCode
		 and temp.Conta = a.BookAccount
		 where a.ReferenceMonth = @MesRef

		select * from @retorno
		order by Orgao asc, UO asc, UGE asc, Conta asc;
	END

END
GO

GRANT EXECUTE ON [dbo].[SAM_BUSCA_VALORES_FECHAMENTO_UGES_COM_MES_REF_FECHADO] TO [USUSAM]
GO