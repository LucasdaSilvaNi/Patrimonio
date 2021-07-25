SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AccountingClosingExcluidos]') AND type in (N'U'))
	DROP TABLE [dbo].[AccountingClosingExcluidos]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AccountingClosingExcluidos]') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[AccountingClosingExcluidos](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ManagerUnitCode] [int] NOT NULL,
	[ManagerUnitDescription] [nvarchar](100) NOT NULL,
	[DepreciationAccount] [int] NULL,
	[DepreciationMonth] [decimal](18, 2) NOT NULL,
	[AccumulatedDepreciation] [decimal](18, 2) NOT NULL,
	[Status] [bit] NULL,
	[DepreciationDescription] [nvarchar](500) NULL,
	[ReferenceMonth] [nvarchar](6) NOT NULL,
	[ItemAccounts] [int] NULL,
	[AccountName] [nvarchar](100) NULL,
	[GeneratedNL] [nvarchar](100) NULL,
	[ClosingId] [uniqueidentifier] NULL,
	[ManagerCode] [nchar](5) NULL,
	[AuditoriaIntegracaoId] [int] NULL,
	[NLEstorno] [nvarchar](100) NULL,
	[AuditoriaIntegracaoIdEstorno] [int] NULL,
 CONSTRAINT [PK_AccountingClosingExcluido] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[AccountingClosingExcluidos]  WITH CHECK ADD  CONSTRAINT [FK_AccountingClosingExcluidos_AuditoriaIntegracao] FOREIGN KEY([AuditoriaIntegracaoId])
REFERENCES [dbo].[AuditoriaIntegracao] ([Id])

ALTER TABLE [dbo].[AccountingClosingExcluidos] CHECK CONSTRAINT [FK_AccountingClosingExcluidos_AuditoriaIntegracao]

ALTER TABLE [dbo].[AccountingClosingExcluidos]  WITH CHECK ADD  CONSTRAINT [FK_AccountingClosingExcluidos_AuditoriaIntegracao_Estorno] FOREIGN KEY([AuditoriaIntegracaoIdEstorno])
REFERENCES [dbo].[AuditoriaIntegracao] ([Id])

ALTER TABLE [dbo].[AccountingClosingExcluidos] CHECK CONSTRAINT [FK_AccountingClosingExcluidos_AuditoriaIntegracao_Estorno]
END

GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[AccountingClosingExcluidos] TO [USUSAM]
GO