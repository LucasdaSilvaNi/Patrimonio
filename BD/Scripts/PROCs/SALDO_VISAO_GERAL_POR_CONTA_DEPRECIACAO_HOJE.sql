/****** Object:  StoredProcedure [dbo].[SALDO_VISAO_GERAL_POR_CONTA_CONTABIL_HOJE]    Script Date: 09/10/2020 10:42:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE')
	DROP PROCEDURE [dbo].[SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE]
GO

CREATE PROCEDURE [dbo].[SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE]
(
	@IdOrgao int,
	@IdUO int = NULL,
	@IdUGE int = NULL
)
AS
BEGIN

Declare @Hierarquias as table (
IdOrgao int, 
CodigoOrgao varchar(6),
IdUo int, 
CodigoUO varchar(6),
IdUGE int, 
CodigoUGE varchar(6),
MesRef varchar(6));

insert into @Hierarquias (IdOrgao, CodigoOrgao, IdUo, CodigoUO, IdUGE, CodigoUGE, MesRef)
select distinct i.Id, i.Code, B.Id, B.Code, 
M.Id, M.Code, M.ManagmentUnit_YearMonthReference
from ManagerUnit M with(nolock)
inner join BudgetUnit b with(nolock)
on b.Id = m.BudgetUnitId
inner join Institution i with(nolock)
on i.Id = b.InstitutionId
where i.Id = @IdOrgao
and (b.Id = @IdUO OR @IdUO IS NULL)
and (m.Id =@IdUGE OR @IdUGE IS NULL)
and m.FlagIntegracaoSiafem = 1
and m.ManagmentUnit_YearMonthReference is not null
and m.Status = 1;

Declare @ContasDepreciacaoAMandarProSIAFEM as table(ContaDepreciacao int, DescricaoConta varchar(255))

insert into @ContasDepreciacaoAMandarProSIAFEM (ContaDepreciacao, DescricaoConta)
select ContaDepreciacao, Descricao from ContasDepreciacoesPraConsultarSIAFEM

Declare @contConta int;
select @contConta = count(ContaDepreciacao) 
from @ContasDepreciacaoAMandarProSIAFEM;

Declare @RetornoComIds as table (
IdOrgao int, 
CodigoOrgao varchar(6),
IdUo int, 
CodigoUO varchar(6),
IdUGE int, 
CodigoUGE varchar(6),
Mes varchar(4),
Ano varchar(6),
ContaDepreciacao int,
DescricaoConta varchar(255),
ValorContabilSAM decimal(18,2));

Declare @contaAtual int, @DescricaoContaAtual varchar(255);

while(@contConta > 0)
BEGIN
	select top 1 
	@contaAtual = ContaDepreciacao,
	@DescricaoContaAtual = DescricaoConta
	from @ContasDepreciacaoAMandarProSIAFEM;

	insert into @RetornoComIds(
	IdOrgao, 
	CodigoOrgao ,
	IdUo, 
	CodigoUO,
	IdUGE, 
	CodigoUGE,
	Mes,
	Ano,
	ContaDepreciacao,
	DescricaoConta,
	ValorContabilSAM)
	select 
		IdOrgao, 
		CodigoOrgao ,
		IdUo, 
		CodigoUO,
		IdUGE, 
		CodigoUGE,
		SUBSTRING(MesRef, 5, 2) 'Mes',
		SUBSTRING(MesRef, 1, 4) 'Ano',
		@contaAtual,
		@DescricaoContaAtual,
		0.0
	from @Hierarquias

	delete from @ContasDepreciacaoAMandarProSIAFEM where ContaDepreciacao = @contaAtual;
	set @contConta = @contConta - 1;
END

update temp
set temp.ValorContabilSAM = s.DepreciacaoAcumulada
from @RetornoComIds temp
inner join SaldoContabilAtualUGEContaDepreciacao s
on s.IdUGE = temp.IdUGE
and s.IdUO = temp.IdUo
and s.IdOrgao = temp.IdOrgao
and s.ContaDepreciacao = temp.ContaDepreciacao;

select CodigoOrgao 'Orgao',
	   CodigoUO 'UO',
	   CodigoUGE 'UGE',
	   ContaDepreciacao 'Conta',
	   DescricaoConta 'DescricaoConta',
	   ValorContabilSAM 'ValorContabilSAM',
	   Mes 'Mes',
	   Ano 'Ano'
from @RetornoComIds
order by Orgao, UO, UGE, Conta

END
GO

GRANT EXECUTE ON [dbo].[SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE] TO [USUSAM]
GO
