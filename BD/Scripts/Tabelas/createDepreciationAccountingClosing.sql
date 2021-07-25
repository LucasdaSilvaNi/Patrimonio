IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DepreciationAccountingClosing_AccountingClosing]') AND parent_object_id = OBJECT_ID(N'[dbo].[DepreciationAccountingClosing]'))
ALTER TABLE [dbo].[DepreciationAccountingClosing] DROP CONSTRAINT [FK_DepreciationAccountingClosing_AccountingClosing]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DepreciationAccountingClosing]') AND type in (N'U'))
DROP TABLE [dbo].[DepreciationAccountingClosing]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DepreciationAccountingClosing]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DepreciationAccountingClosing](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AccountingClosingId] [bigint] NOT NULL,
	[BookAccount] [int] NOT NULL,
	[AccountingValue] [decimal](18, 2) NOT NULL,
	[DepreciationMonth] [decimal](18, 2) NOT NULL,
	[AccumulatedDepreciation] [decimal](18, 2) NOT NULL,
	[Status] [bit] NOT NULL,
	[AccountingDescription] [nvarchar](500) NOT NULL,
	[ReferenceMonth] [nchar](6) NOT NULL,
	[ManagerUnitCode] [int] NOT NULL,
	[ManagerDescription] [nchar](100) NOT NULL,
 CONSTRAINT [PK_DepreciationAccountingClosing] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_DEPRECIATIONACCOUNTING] UNIQUE NONCLUSTERED 
(
	[BookAccount] ASC,
	[ReferenceMonth] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DepreciationAccountingClosing_AccountingClosing]') AND parent_object_id = OBJECT_ID(N'[dbo].[DepreciationAccountingClosing]'))
	ALTER TABLE [dbo].[DepreciationAccountingClosing]  WITH CHECK ADD  CONSTRAINT [FK_DepreciationAccountingClosing_AccountingClosing] FOREIGN KEY([AccountingClosingId])
	REFERENCES [dbo].[AccountingClosing] ([Id])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DepreciationAccountingClosing_AccountingClosing]') AND parent_object_id = OBJECT_ID(N'[dbo].[DepreciationAccountingClosing]'))
	ALTER TABLE [dbo].[DepreciationAccountingClosing] CHECK CONSTRAINT [FK_DepreciationAccountingClosing_AccountingClosing]
GO