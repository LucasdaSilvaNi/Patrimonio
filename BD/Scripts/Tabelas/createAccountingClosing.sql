SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AccountingClosing]') AND type in (N'U'))
	DROP TABLE [dbo].[AccountingClosing]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AccountingClosing]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AccountingClosing](
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
 CONSTRAINT [PK_AccountingClosing] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ACCOUNTING] UNIQUE NONCLUSTERED 
(
	[ManagerUnitCode] ASC,
	[ReferenceMonth] ASC,
	[DepreciationAccount] ASC,
	[ClosingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO