SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PREENCHE_TABELA_DE_NL_ESTORNADAS')
	DROP PROCEDURE [dbo].[PREENCHE_TABELA_DE_NL_ESTORNADAS]  
GO

CREATE PROCEDURE [dbo].[PREENCHE_TABELA_DE_NL_ESTORNADAS]
	@ManagerUnitCode nvarchar(6),
	@MesRef nvarchar(6)
AS

	insert into AccountingClosingExcluidos
	([ManagerUnitCode],[ManagerUnitDescription],[DepreciationAccount], [DepreciationMonth],
	[AccumulatedDepreciation],[Status],[DepreciationDescription],[ReferenceMonth],[ItemAccounts],
	[AccountName], [GeneratedNL], [ClosingId], [ManagerCode], [AuditoriaIntegracaoId])
	select
	[ManagerUnitCode],[ManagerUnitDescription],[DepreciationAccount], [DepreciationMonth],
	[AccumulatedDepreciation],[Status],[DepreciationDescription],[ReferenceMonth],[ItemAccounts],
	[AccountName], [GeneratedNL], [ClosingId], [ManagerCode], [AuditoriaIntegracaoId]
	from AccountingClosing
	where ManagerUnitCode = CONVERT(int,@ManagerUnitCode) and ReferenceMonth = @MesRef;
GO

GRANT EXECUTE ON [dbo].[PREENCHE_TABELA_DE_NL_ESTORNADAS] TO [USUSAM]
GO