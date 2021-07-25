IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'DocumentoId')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [DocumentoId] VARCHAR(60) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'TipoMovimento')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [TipoMovimento] VARCHAR(80) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'Data')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [Data] DATE NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'UgeOrigem')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [UgeOrigem] VARCHAR(6) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'Gestao')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [Gestao] VARCHAR(5) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'Tipo_Entrada_Saida_Reclassificacao_Depreciacao')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [Tipo_Entrada_Saida_Reclassificacao_Depreciacao] VARCHAR(80) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'CpfCnpjUgeFavorecida')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [CpfCnpjUgeFavorecida] VARCHAR(14) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'GestaoFavorecida')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [GestaoFavorecida] VARCHAR(5) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'Item')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [Item] VARCHAR(15) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'TipoEstoque')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [TipoEstoque] VARCHAR(10) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'Estoque')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [Estoque] VARCHAR(20) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'EstoqueDestino')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [EstoqueDestino] VARCHAR(80) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'EstoqueOrigem')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [EstoqueOrigem] VARCHAR(80) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'TipoMovimentacao')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [TipoMovimentacao] VARCHAR(80) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'ValorTotal')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [ValorTotal] DECIMAL(18, 2) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'ControleEspecifico')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [ControleEspecifico] VARCHAR(15) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'ControleEspecificoEntrada')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [ControleEspecificoEntrada] VARCHAR(15) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'ControleEspecificoSaida')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [ControleEspecificoSaida] VARCHAR(15) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'FonteRecurso')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [FonteRecurso] VARCHAR(15) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'NLEstorno')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [NLEstorno] VARCHAR(1) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'Empenho')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [Empenho] VARCHAR(11) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'Observacao')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [Observacao] VARCHAR(231) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'NotaFiscal')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [NotaFiscal] VARCHAR(14) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'ItemMaterial')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [ItemMaterial] VARCHAR(15) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'NotaLancamento')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD [NotaLancamento] VARCHAR(11) NULL
END
GO


IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'AuditoriaIntegracao' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'MsgErro')
BEGIN
    ALTER TABLE [dbo].[AuditoriaIntegracao]
    ADD MsgErro VARCHAR(120) NULL
END
GO