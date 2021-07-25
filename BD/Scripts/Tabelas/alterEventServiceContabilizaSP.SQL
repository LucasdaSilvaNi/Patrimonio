ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD [MetaDataType_MovementTypeContabilizaSP] INT --TipoMovimentoContabilizaSP
	  
ALTER TABLE [dbo].[EventServiceContabilizaSP]  WITH CHECK ADD CONSTRAINT [FK_MetaDataType_MovementTypeContabilizaSP] FOREIGN KEY([MetaDataType_MovementTypeContabilizaSP])
REFERENCES [dbo].[MetaDataTypeServiceContabilizaSP] ([Id])
GO
--CONTROLE ESPECIFICO
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EventServiceContabilizaSP' AND COLUMN_NAME = 'MetaDataType_SpecificControl' AND TABLE_SCHEMA='DBO')
BEGIN
    ALTER TABLE EventServiceContabilizaSP ADD MetaDataType_SpecificControl INT
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EventServiceContabilizaSP' AND COLUMN_NAME = 'SpecificControl' AND TABLE_SCHEMA='DBO')
BEGIN
    ALTER TABLE EventServiceContabilizaSP ADD SpecificControl VARCHAR(15)
END
GO


--CONTROLE ESPECIFICO ENTRADA
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EventServiceContabilizaSP' AND COLUMN_NAME = 'MetaDataType_SpecificInputControl' AND TABLE_SCHEMA='DBO')
BEGIN
    ALTER TABLE EventServiceContabilizaSP ADD MetaDataType_SpecificInputControl INT
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EventServiceContabilizaSP' AND COLUMN_NAME = 'SpecificInputControl' AND TABLE_SCHEMA='DBO')
BEGIN
    ALTER TABLE EventServiceContabilizaSP ADD SpecificInputControl VARCHAR(15)
END
GO



--CONTROLE ESPECIFICO SAIDA
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EventServiceContabilizaSP' AND COLUMN_NAME = 'MetaDataType_SpecificOutputControl' AND TABLE_SCHEMA='DBO')
BEGIN
    ALTER TABLE EventServiceContabilizaSP ADD MetaDataType_SpecificOutputControl INT
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EventServiceContabilizaSP' AND COLUMN_NAME = 'SpecificOutputControl' AND TABLE_SCHEMA='DBO')
BEGIN
    ALTER TABLE EventServiceContabilizaSP ADD SpecificOutputControl VARCHAR(15)
END
GO


--Reaplicação dos tipos de descrições dos Controles Específicos
alter table EventServiceContabilizaSP
alter column SpecificControl varchar(15) not null

alter table EventServiceContabilizaSP
alter column SpecificInputControl varchar(15) not null

alter table EventServiceContabilizaSP
alter column SpecificOutputControl varchar(15) not null

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'EventServiceContabilizaSP'
				 AND COLUMN_NAME = 'DataAtivacaoTipoMovimentacao'))
BEGIN
	alter table [dbo].[EventServiceContabilizaSP] add DataAtivacaoTipoMovimentacao datetime;
	
	ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD CONSTRAINT [DF_EventServiceContabilizaSP_DataAtivacaoTipoMovimentacao]  DEFAULT (GETDATE()) FOR [DataAtivacaoTipoMovimentacao]
END
