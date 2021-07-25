
--TORNAR COLUNA 'StockDescription' NULLABLE (DEPRECIACAO E RECLASSIFICACAO, NO CONTABILIZA-SP EH ENVIADA EM BRANCO)
--INCLUSAO COLUNA TEMPORARIA
BEGIN TRANSACTION
IF EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'EventServiceContabilizaSP' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'StockDescription')
BEGIN
    ALTER TABLE [dbo].[EventServiceContabilizaSP]
    ADD [StockDescription2] VARCHAR(19)

END
GO

--ATUALIZACAO DADOS COLUNA TEMPORARIA E RENOMEACAO PARA COLUNA ANTIGA COM DATATYPE NOVO
IF EXISTS(SELECT 1 
          FROM INFORMATION_SCHEMA.COLUMNS
          WHERE TABLE_NAME = 'EventServiceContabilizaSP' AND TABLE_SCHEMA='dbo'
          AND COLUMN_NAME = 'StockDescription2')
BEGIN
    UPDATE [EventServiceContabilizaSP] SET [dbo].[EventServiceContabilizaSP].[StockDescription2] = [dbo].[EventServiceContabilizaSP].[StockDescription]

	ALTER TABLE [dbo].[EventServiceContabilizaSP] DROP CONSTRAINT [DF__EventServiceContabilizaSP_StockDescription]
	ALTER TABLE [dbo].[EventServiceContabilizaSP] DROP COLUMN [StockDescription]

	EXEC SP_RENAME 'EventServiceContabilizaSP.StockDescription2', 'StockDescription', 'COLUMN';
	ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD CONSTRAINT [DF__EventServiceContabilizaSP_StockDescription] DEFAULT ('ESTOQUE LONGO PRAZO') FOR [StockDescription]
END
COMMIT
GO

--INCLUSAO DE COLUNA DE ORDEM DE GERACAO/ENVIO DE NL'S QUANDO O TIPO DE MOVIMENTACAO SAM-PATRIMONIO POSSUIR MAIS DE UM TIPO CONTABILIZA-SP ASSOCIADO
IF NOT EXISTS(SELECT 1 
			  FROM INFORMATION_SCHEMA.COLUMNS
			  WHERE TABLE_NAME = 'EventServiceContabilizaSP' AND TABLE_SCHEMA='dbo'
			  AND COLUMN_NAME = 'ExecutionOrder')
BEGIN
    ALTER TABLE [dbo].[EventServiceContabilizaSP]
    ADD [ExecutionOrder] SMALLINT NULL
END
GO

--PREENCHIMENTO DADOS NOVA COLUNA
IF EXISTS(SELECT 1 
		  FROM INFORMATION_SCHEMA.COLUMNS
		  WHERE TABLE_NAME = 'EventServiceContabilizaSP' AND TABLE_SCHEMA='dbo'
		  AND COLUMN_NAME = 'ExecutionOrder')
BEGIN
	--INCORPORACOES/MOVIMENTACOES
	UPDATE [EventServiceContabilizaSP]
	SET [ExecutionOrder] = '1'
	WHERE [EventServiceContabilizaSP].[AccountEntryTypeId] IN (1, 2) --AGRUPAMENTO DO TIPO INCORPORACAO/MOVIMENTACAO

	--DEPRECIACOES
	UPDATE [EventServiceContabilizaSP]
	SET [ExecutionOrder] = '2'
	WHERE [EventServiceContabilizaSP].[AccountEntryTypeId] IN (8) --AGRUPAMENTO DO TIPO DEPRECIACAO

	--RECLASSIFICACOES
	UPDATE [EventServiceContabilizaSP]
	SET [ExecutionOrder] = '3'
	WHERE [EventServiceContabilizaSP].[AccountEntryTypeId] IN (9) --AGRUPAMENTO DO TIPO RECLASSIFICACAO

	ALTER TABLE [dbo].[EventServiceContabilizaSP]
    ALTER COLUMN [ExecutionOrder] SMALLINT NOT NULL
END
GO