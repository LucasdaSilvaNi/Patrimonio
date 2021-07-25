IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'BPsExcluidos' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'NotaLancamentoReclassificacao')
BEGIN
    ALTER TABLE [dbo].[BPsExcluidos]
    ADD [NotaLancamentoReclassificacao] VARCHAR(11) NULL
END
GO

IF NOT EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'BPsExcluidos' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'NotaLancamentoReclassificacaoEstorno')
BEGIN
    ALTER TABLE [dbo].[BPsExcluidos]
    ADD [NotaLancamentoReclassificacaoEstorno] VARCHAR(11) NULL
END
GO