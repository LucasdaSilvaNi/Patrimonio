/****** Object:  UserDefinedFunction [dbo].[fnSplitString]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnSplitString] 
( 
    @string NVARCHAR(MAX), 
    @delimiter CHAR(1) 
) 
RETURNS @output TABLE(splitdata NVARCHAR(MAX) 
) 
BEGIN 
    DECLARE @start INT, @end INT 
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @string) 
    WHILE @start < LEN(@string) + 1 BEGIN 
        IF @end = 0  
            SET @end = LEN(@string) + 1
       
        INSERT INTO @output (splitdata)  
        VALUES(SUBSTRING(@string, @start, @end - @start)) 
        SET @start = @end + 1 
        SET @end = CHARINDEX(@delimiter, @string, @start)
        
    END 
    RETURN 
END


GO
/****** Object:  UserDefinedFunction [dbo].[RETORNA_NUMERO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[RETORNA_NUMERO] 
(
	@STRING AS VARCHAR(MAX)
)
RETURNS BIGINT
WITH EXECUTE AS CALLER  
AS
BEGIN
	DECLARE
	 @RESULTADO VARCHAR (1000), 
	 @NUMERO VARCHAR(1),
	 @QTD_PALAVRA BIGINT,
	 @CONT BIGINT

	SET @CONT = 0 
	SET @QTD_PALAVRA = LEN(@STRING)
	SET @RESULTADO = 0

	WHILE @CONT < @QTD_PALAVRA 
	 BEGIN 
		SET @CONT = @CONT + 1 

		SET @NUMERO = SUBSTRING(@STRING,@CONT,1)
		IF @NUMERO  IN ('0','1','2','3','4','5','6','7','8','9') 
		BEGIN
			SET @RESULTADO =  @RESULTADO +  @NUMERO 
		END
			
	 END
	 RETURN CAST(@RESULTADO AS BIGINT)
END







GO
/****** Object:  UserDefinedFunction [dbo].[RETORNA_NUMERO_DO_ENDERECO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[RETORNA_NUMERO_DO_ENDERECO] 
(
	@ENDERECO AS VARCHAR(MAX)
)
RETURNS INT
WITH EXECUTE AS CALLER  
AS
BEGIN
	DECLARE
	 @RESULTADO VARCHAR (1000), 
	 @LETRA VARCHAR(1),
	 @QTD_PALAVRA INTEGER,
	 @CONT INTEGER

	SET @CONT = 0 
	SET @QTD_PALAVRA = LEN(@ENDERECO)
	SET @RESULTADO = 0

	WHILE @CONT < @QTD_PALAVRA 
	 BEGIN 
		SET @CONT = @CONT + 1 

		SET @LETRA = SUBSTRING(@ENDERECO,@CONT,1)
		IF @LETRA  IN ('0','1','2','3','4','5','6','7','8','9' ) 
			BEGIN
				SET @RESULTADO =  @RESULTADO +  @LETRA 
			END
			ELSE
			BEGIN
				SET @RESULTADO = ''
			END
	 END
	 RETURN CAST(@RESULTADO AS INT)
END






GO
/****** Object:  Table [dbo].[AccountingClosing]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
	[AuditoriaIntegracaoId] [int] NULL,
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

GO
/****** Object:  Table [dbo].[AccountingClosingExcluidos]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
/****** Object:  Table [dbo].[Address]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Street] [varchar](200) NULL,
	[Number] [varchar](10) NULL,
	[ComplementAddress] [varchar](30) NULL,
	[District] [varchar](50) NULL,
	[City] [varchar](50) NULL,
	[State] [varchar](2) NULL,
	[PostalCode] [char](8) NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Addressee]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Addressee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](60) NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Addressee] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdministrativeUnit]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdministrativeUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManagerUnitId] [int] NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[RelationshipAdministrativeUnitId] [int] NULL,
	[Status] [bit] NOT NULL,
	[RowId] [int] NULL,
 CONSTRAINT [PK_AdministrativeUnit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Asset]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Asset](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MovementTypeId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[InitialId] [int] NOT NULL,
	[InitialName] [varchar](10) NULL,
	[NumberIdentification] [varchar](30) NULL,
	[AcquisitionDate] [date] NOT NULL,
	[ValueAcquisition] [decimal](18, 2) NOT NULL,
	[ValueUpdate] [decimal](18, 2) NULL,
	[MaterialItemCode] [int] NOT NULL,
	[MaterialItemDescription] [varchar](255) NOT NULL,
	[MaterialGroupCode] [int] NOT NULL,
	[LifeCycle] [int] NOT NULL,
	[RateDepreciationMonthly] [decimal](18, 2) NOT NULL,
	[ResidualValue] [decimal](18, 2) NOT NULL,
	[AceleratedDepreciation] [bit] NULL,
	[Empenho] [varchar](15) NULL,
	[ShortDescriptionItemId] [int] NOT NULL,
	[OldInitial] [int] NULL,
	[OldNumberIdentification] [varchar](250) NULL,
	[NumberDoc] [varchar](20) NOT NULL,
	[MovimentDate] [date] NOT NULL,
	[ManagerUnitId] [int] NULL,
	[OutSourcedId] [int] NULL,
	[SupplierId] [int] NULL,
	[SerialNumber] [varchar](50) NULL,
	[ManufactureDate] [date] NULL,
	[DateGuarantee] [date] NULL,
	[ChassiNumber] [varchar](20) NULL,
	[Brand] [varchar](20) NULL,
	[Model] [varchar](20) NULL,
	[NumberPlate] [varchar](8) NULL,
	[AdditionalDescription] [varchar](20) NULL,
	[flagVerificado] [int] NULL,
	[flagDepreciaAcumulada] [int] NULL,
	[RowId] [int] NULL,
	[ResidualValueCalc] [decimal](18, 2) NULL,
	[monthUsed] [int] NULL,
	[DepreciationByMonth] [decimal](18, 10) NULL,
	[DepreciationAccumulated] [decimal](18, 2) NULL,
	[flagCalculoPendente] [bit] NULL,
	[Login] [varchar](20) NULL,
	[DataLogin] [datetime] NULL,
	[ValorDesdobramento] [decimal](18, 2) NULL,
	[flagAcervo] [bit] NULL,
	[flagDepreciationCompleted] [bit] NULL,
	[flagTerceiro] [bit] NULL,
	[flagDecreto] [bit] NULL,
	[CPFCNPJ] [varchar](14) NULL,
	[Recalculo] [bit] NULL,
	[AssetStartId] [int] NULL,
	[DepreciationDateStart] [datetime] NULL,
	[DepreciationAmountStart] [decimal](18, 2) NULL,
	[FlagAnimalNaoServico] [bit] NULL,
	[flagVindoDoEstoque] [bit] NOT NULL,
	[IndicadorOrigemInventarioInicial] [tinyint] NULL,
	[DiferenciacaoChapa] [varchar](7) NOT NULL,
	[DiferenciacaoChapaAntiga] [varchar](7) NOT NULL,
	[ChapaCompleta]  AS (concat([NumberIdentification],[DiferenciacaoChapa])),
	[ChapaAntigaCompleta]  AS (concat([OldNumberIdentification],[DiferenciacaoChapaAntiga])),
 CONSTRAINT [PK_Asset_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AssetAlterado]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssetAlterado](
	[Id] [int] NULL,
	[Validacao] [varchar](35) NOT NULL,
	[DataRealizado] [datetime] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AssetMovements]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssetMovements](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssetId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[MovimentDate] [date] NOT NULL,
	[MovementTypeId] [int] NOT NULL,
	[StateConservationId] [int] NOT NULL,
	[NumberPurchaseProcess] [varchar](25) NULL,
	[InstitutionId] [int] NOT NULL,
	[BudgetUnitId] [int] NOT NULL,
	[ManagerUnitId] [int] NOT NULL,
	[AdministrativeUnitId] [int] NULL,
	[SectionId] [int] NULL,
	[AuxiliaryAccountId] [int] NULL,
	[ResponsibleId] [int] NULL,
	[SourceDestiny_ManagerUnitId] [int] NULL,
	[AssetTransferenciaId] [int] NULL,
	[ExchangeId] [int] NULL,
	[ExchangeDate] [date] NULL,
	[ExchangeUserId] [int] NULL,
	[TypeDocumentOutId] [int] NULL,
	[Observation] [varchar](500) NULL,
	[FlagEstorno] [bit] NULL,
	[DataEstorno] [date] NULL,
	[LoginEstorno] [int] NULL,
	[RepairValue] [money] NULL,
	[Login] [varchar](20) NULL,
	[DataLogin] [datetime] NULL,
	[NumberDoc] [varchar](20) NULL,
	[flagUGENaoUtilizada] [bit] NULL,
	[CPFCNPJ] [varchar](14) NULL,
	[NumeroNL] [varchar](11) NULL,
	[MonthlyDepreciationId] [int] NULL,
	[NotaLancamento] [varchar](11) NULL,
	[NotaLancamentoEstorno] [varchar](11) NULL,
	[NotaLancamentoDepreciacao] [varchar](11) NULL,
	[NotaLancamentoDepreciacaoEstorno] [varchar](11) NULL,
	[AuditoriaIntegracaoId] [int] NULL,
	[NotaLancamentoPendenteSIAFEMId] [int] NULL,
	[NotaLancamentoReclassificacao] [varchar](11) NULL,
	[NotaLancamentoReclassificacaoEstorno] [varchar](11) NULL,
	[ContaContabilAntesDeVirarInservivel] [int] NULL,
	[AuxiliaryMovementTypeId] [int] NULL,
 CONSTRAINT [PK_AssetMovements_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AssetNumberIdentification]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssetNumberIdentification](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NumberIdentification] [varchar](250) NOT NULL,
	[Status] [bit] NOT NULL,
	[AssetId] [int] NOT NULL,
	[Login] [varchar](50) NULL,
	[InclusionDate] [datetime] NOT NULL,
	[InitialId] [int] NULL,
	[DiferenciacaoChapa] [varchar](7) NOT NULL,
	[ChapaCompleta]  AS (concat([NumberIdentification],[DiferenciacaoChapa])),
 CONSTRAINT [PK_AssetNumberIdentification] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AssetStartIds]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssetStartIds](
	[AssetStartId] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AttachmentSupport]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttachmentSupport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupportId] [int] NOT NULL,
	[File] [image] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Extension] [varchar](5) NOT NULL,
	[ContentType] [varchar](200) NOT NULL,
	[InclusionDate] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AuditoriaIntegracao]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditoriaIntegracao](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DataEnvio] [datetime] NOT NULL,
	[MsgEstimuloWS] [varchar](max) NULL,
	[MsgRetornoWS] [varchar](max) NULL,
	[NomeSistema] [varchar](15) NULL,
	[UsuarioSAM] [varchar](11) NULL,
	[UsuarioSistemaExterno] [varchar](11) NULL,
	[ManagerUnitId] [int] NULL,
	[AssetMovementIds] [varchar](max) NULL,
	[TokenAuditoriaIntegracao] [uniqueidentifier] NULL,
	[DataRetorno] [datetime] NULL,
	[DocumentoId] [varchar](60) NULL,
	[TipoMovimento] [varchar](80) NULL,
	[Data] [date] NULL,
	[UgeOrigem] [varchar](6) NULL,
	[Gestao] [varchar](5) NULL,
	[Tipo_Entrada_Saida_Reclassificacao_Depreciacao] [varchar](80) NULL,
	[CpfCnpjUgeFavorecida] [varchar](14) NULL,
	[GestaoFavorecida] [varchar](5) NULL,
	[Item] [varchar](15) NULL,
	[TipoEstoque] [varchar](10) NULL,
	[Estoque] [varchar](20) NULL,
	[EstoqueDestino] [varchar](80) NULL,
	[EstoqueOrigem] [varchar](80) NULL,
	[TipoMovimentacao] [varchar](80) NULL,
	[ValorTotal] [decimal](18, 2) NULL,
	[ControleEspecifico] [varchar](15) NULL,
	[ControleEspecificoEntrada] [varchar](15) NULL,
	[ControleEspecificoSaida] [varchar](15) NULL,
	[FonteRecurso] [varchar](15) NULL,
	[NLEstorno] [varchar](1) NULL,
	[Empenho] [varchar](11) NULL,
	[Observacao] [varchar](231) NULL,
	[NotaFiscal] [varchar](14) NULL,
	[ItemMaterial] [varchar](15) NULL,
	[NotaLancamento] [varchar](11) NULL,
	[MsgErro] [varchar](150) NULL,
 CONSTRAINT [PK_AuditoriaIntegracao] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AuxiliaryAccount]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuxiliaryAccount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](100) NULL,
	[BookAccount] [int] NULL,
	[Status] [bit] NOT NULL,
	[DepreciationAccountId] [int] NULL,
	[RelacionadoBP] [int] NOT NULL,
	[ContaContabilApresentacao] [varchar](50) NULL,
	[ControleEspecificoResumido] [varchar](15) NULL,
	[TipoMovimentacaoContabilizaSP] [varchar](80) NULL,
 CONSTRAINT [PK_ContaAuxiliar] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BPsARealizaremReclassificacaoContabil]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BPsARealizaremReclassificacaoContabil](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssetId] [int] NULL,
	[GrupoMaterial] [int] NULL,
	[IdUGE] [int] NULL,
 CONSTRAINT [UK_BPsARealizaremReclassificacaoContabil_Asset] UNIQUE NONCLUSTERED 
(
	[AssetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BPsExcluidos]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BPsExcluidos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TipoIncorporacao] [int] NOT NULL,
	[InitialId] [int] NOT NULL,
	[SiglaInicial] [varchar](10) NULL,
	[Chapa] [varchar](250) NOT NULL,
	[ItemMaterial] [int] NOT NULL,
	[GrupoMaterial] [int] NOT NULL,
	[StateConservationId] [int] NOT NULL,
	[ManagerUnitId] [int] NOT NULL,
	[AdministrativeUnitId] [int] NULL,
	[ResponsibleId] [int] NULL,
	[Processo] [varchar](25) NULL,
	[ValorAquisicao] [decimal](18, 2) NOT NULL,
	[DataAquisicao] [date] NOT NULL,
	[DataIncorporacao] [date] NOT NULL,
	[flagAcervo] [bit] NOT NULL,
	[flagTerceiro] [bit] NOT NULL,
	[flagDecretoSEFAZ] [bit] NOT NULL,
	[DataAcao] [date] NOT NULL,
	[LoginAcao] [int] NOT NULL,
	[NotaLancamento] [varchar](11) NULL,
	[NotaLancamentoEstorno] [varchar](11) NULL,
	[NotaLancamentoDepreciacao] [varchar](11) NULL,
	[NotaLancamentoDepreciacaoEstorno] [varchar](11) NULL,
	[Observacoes] [varchar](100) NULL,
	[NotaLancamentoReclassificacao] [varchar](11) NULL,
	[NotaLancamentoReclassificacaoEstorno] [varchar](11) NULL,
	[FlagModoExclusao] [tinyint] NULL,
	[MotivoExclusao] [varchar](100) NULL,
	[NumeroDocumento] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BudgetUnit]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BudgetUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[Code] [varchar](10) NULL,
	[Description] [varchar](120) NOT NULL,
	[Direct] [bit] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_UO] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Closing]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Closing](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssetId] [int] NOT NULL,
	[ManagerUnitId] [int] NOT NULL,
	[ClosingYearMonthReference] [varchar](6) NOT NULL,
	[CurrentPrice] [decimal](18, 2) NOT NULL,
	[DepreciationPrice] [decimal](18, 10) NOT NULL,
	[MaterialItemCode] [int] NOT NULL,
	[AceleratedDepreciation] [int] NOT NULL,
	[MaterialGroupCode] [int] NOT NULL,
	[LifeCycle] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[LoginID] [int] NOT NULL,
	[ClosingDate] [datetime] NOT NULL,
	[AssetMovementsId] [int] NULL,
	[MonthUsed] [int] NULL,
	[ResidualValueCalc] [decimal](18, 2) NULL,
	[DepreciationAccumulated] [decimal](18, 2) NULL,
	[flagDepreciationCompleted] [bit] NULL,
	[Recalculo] [bit] NULL,
 CONSTRAINT [PK_Fechamento] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Configuration]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuration](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InitialYearMonthAsset] [date] NULL,
	[ReferenceYearMonthAsset] [date] NULL,
 CONSTRAINT [PK_Configuracoes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ContasContabeisPraConsultarNoSIAFEM]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContasContabeisPraConsultarNoSIAFEM](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ContaContabil] [int] NULL,
	[Descricao] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ContasDepreciacoesPraConsultarSIAFEM]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContasDepreciacoesPraConsultarSIAFEM](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ContaDepreciacao] [int] NULL,
	[Descricao] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CostCenter]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CostCenter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](22) NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_CentroCusto] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DadosImportacaoBens]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DadosImportacaoBens](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImportacaoPlanilhaId] [int] NOT NULL,
	[Data_Importacao] [date] NOT NULL,
	[Sigla] [varchar](100) NOT NULL,
	[Sigla_Descricao] [varchar](100) NULL,
	[Chapa] [varchar](100) NOT NULL,
	[Codigo_do_Item] [varchar](100) NOT NULL,
	[Descricao_do_Item] [varchar](100) NULL,
	[CPF_Responsavel] [varchar](100) NULL,
	[Nome_Responsavel] [varchar](100) NULL,
	[Cargo_Responsavel] [varchar](100) NULL,
	[Data_Aquisicao] [varchar](100) NULL,
	[Valor_Aquisicao] [varchar](100) NULL,
	[Data_Inclusao] [varchar](100) NULL,
	[Estado_de_Conservacao] [varchar](100) NOT NULL,
	[Codigo_Orgao] [varchar](100) NULL,
	[Codigo_UO] [varchar](100) NOT NULL,
	[Nome_UO] [varchar](100) NULL,
	[Codigo_UGE] [varchar](100) NULL,
	[Nome_UGE] [varchar](100) NULL,
	[Codigo_UA] [varchar](100) NOT NULL,
	[Nome_UA] [varchar](100) NULL,
	[Codigo_Divisao] [varchar](100) NOT NULL,
	[Nome_Divisao] [varchar](100) NULL,
	[Conta_Auxiliar] [varchar](100) NOT NULL,
	[Contabil_Auxiliar] [varchar](100) NULL,
	[Nome_Conta_Auxiliar] [varchar](100) NULL,
	[Numero_Serie] [varchar](100) NULL,
	[Data_Fabricacao] [varchar](100) NULL,
	[Data_Garantia] [varchar](100) NULL,
	[Numero_Chassi] [varchar](100) NULL,
	[Marca] [varchar](100) NULL,
	[Modelo] [varchar](100) NULL,
	[Placa] [varchar](100) NULL,
	[Sigla_Antiga] [varchar](100) NULL,
	[Chapa_Antiga] [varchar](100) NULL,
	[NEmpenho] [varchar](100) NULL,
	[Observacoes] [varchar](100) NULL,
	[Descricao_Adicional] [varchar](100) NULL,
	[Nota_Fiscal] [varchar](100) NULL,
	[CNPJ_Fornecedor] [varchar](100) NULL,
	[Nome_Fornecedor] [varchar](100) NULL,
	[Telefone_Fornecedor] [varchar](100) NULL,
	[CEP_Fornecedor] [varchar](100) NULL,
	[Endereco_Fornecedor] [varchar](100) NULL,
	[Cidade_Fornecedor] [varchar](100) NULL,
	[Estado_Fornecedor] [varchar](100) NULL,
	[Inf_Complementares_Fornecedores] [varchar](100) NULL,
	[CPF_CNPJ_Terceiro] [varchar](100) NULL,
	[Nome_Terceiro] [varchar](100) NULL,
	[Telefone_terceiro] [varchar](100) NULL,
	[CEP_Terceiro] [varchar](100) NULL,
	[Endereco_Terceiro] [varchar](100) NULL,
	[Cidade_Terceiro] [varchar](100) NULL,
	[Estado_Terceiro] [varchar](100) NULL,
	[Acervo] [varchar](100) NULL,
	[Terceiro] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DadosImportacaoUsuario]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DadosImportacaoUsuario](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImportacaoPlanilhaId] [int] NOT NULL,
	[Data_Importacao] [date] NOT NULL,
	[CARGA_SEQ] [int] NULL,
	[PERFIL] [varchar](100) NULL,
	[USUARIO_CPF] [varchar](100) NULL,
	[USUARIO_NOME_USUARIO] [varchar](100) NULL,
	[ORGAO_CODIGO] [varchar](100) NULL,
	[UO_CODIGO] [varchar](100) NULL,
	[UGE_CODIGO] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DepreciationAccount]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DepreciationAccount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[Status] [bit] NOT NULL,
	[DescricaoContaDepreciacaoContabilizaSP] [varchar](80) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DepreciationAccountingClosing]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
	[ReferenceMonth] ASC,
	[ManagerUnitCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DepreciationMaterialItem]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DepreciationMaterialItem](
	[DepreciationAccount] [int] NOT NULL,
	[MaterialItemCode] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ELMAH_Error]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ELMAH_Error](
	[ErrorId] [uniqueidentifier] NOT NULL,
	[Application] [nvarchar](60) NOT NULL,
	[Host] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[Source] [nvarchar](60) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[StatusCode] [int] NOT NULL,
	[TimeUtc] [datetime] NOT NULL,
	[Sequence] [int] IDENTITY(1,1) NOT NULL,
	[AllXml] [ntext] NOT NULL,
 CONSTRAINT [PK_ELMAH_Error] PRIMARY KEY NONCLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ErroLog]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErroLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[exMessage] [varchar](max) NULL,
	[exStackTrace] [varchar](max) NULL,
	[exTargetSite] [varchar](max) NULL,
	[DataOcorrido] [datetime] NULL,
	[Usuario] [varchar](30) NULL,
 CONSTRAINT [PK__ErroLog__3214EC071FF8A574] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EstadoItemInventario]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EstadoItemInventario](
	[Id] [int] NOT NULL,
	[Descricao] [varchar](120) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EventServiceContabilizaSP]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventServiceContabilizaSP](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountEntryTypeId] [int] NOT NULL,
	[StockType] [char](10) NOT NULL,
	[StockSource] [varchar](100) NULL,
	[StockDestination] [varchar](100) NULL,
	[MaterialType] [char](18) NOT NULL,
	[InputOutputReclassificationDepreciationTypeCode] [int] NOT NULL,
	[SpecificControl] [varchar](15) NOT NULL,
	[SpecificInputControl] [varchar](15) NOT NULL,
	[SpecificOutputControl] [varchar](15) NOT NULL,
	[TipoMovimento_SamPatrimonio] [varchar](100) NULL,
	[TipoMovimentacao_ContabilizaSP] [varchar](100) NOT NULL,
	[MetaDataType_DateField] [int] NOT NULL,
	[MetaDataType_AccountingValueField] [int] NULL,
	[MetaDataType_StockSource] [int] NULL,
	[MetaDataType_StockDestination] [int] NULL,
	[Status] [bit] NOT NULL,
	[MetaDataType_MovementTypeContabilizaSP] [int] NULL,
	[MetaDataType_SpecificControl] [int] NULL,
	[MetaDataType_SpecificInputControl] [int] NULL,
	[MetaDataType_SpecificOutputControl] [int] NULL,
	[StockDescription] [varchar](19) NULL,
	[ExecutionOrder] [smallint] NOT NULL,
	[FavoreceDestino] [bit] NOT NULL,
	[UtilizaTipoMovimentacaoContabilizaSPAlternativa] [bit] NOT NULL,
	[VerificaSeOrigemUtilizaSAM] [bit] NOT NULL,
	[TipoMovimentacaoContabilizaSPAlternativa] [varchar](100) NULL,
	[DataAtivacaoTipoMovimentacao] [datetime] NULL,
 CONSTRAINT [PK_EventServiceContabilizaSP] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Exchange]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Exchange](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssetId] [int] NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[BudgetUnitId] [int] NOT NULL,
	[ManagerUnitId] [int] NOT NULL,
	[DateRequisition] [datetime] NOT NULL,
	[DateService] [datetime] NULL,
	[Login] [varchar](20) NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Exchange] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GroupMoviment]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupMoviment](
	[Id] [int] NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Historic]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Historic](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[Description] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Historico_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Historico]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Historico](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Acao] [varchar](50) NULL,
	[Tabela] [varchar](75) NULL,
	[TabelaId] [int] NULL,
	[Data] [datetime] NULL,
	[Login] [varchar](50) NULL,
 CONSTRAINT [PK_Historico] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HistoricoCampo]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HistoricoCampo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[HistoricoId] [bigint] NULL,
	[Campo] [varchar](50) NULL,
	[ValorAntigo] [varchar](max) NULL,
	[ValorNovo] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HistoricoValoresDecreto]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HistoricoValoresDecreto](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssetId] [int] NULL,
	[ValorAquisicao] [decimal](18, 2) NULL,
	[ValorRevalorizacao] [decimal](18, 2) NULL,
	[DataAlteracao] [datetime] NULL,
	[LoginId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HistoricSupport]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HistoricSupport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupportId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[InclusionDate] [datetime] NOT NULL,
	[Observation] [varchar](max) NULL,
	[Status] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HouveAlteracaoContabil]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HouveAlteracaoContabil](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdOrgao] [int] NULL,
	[IdUO] [int] NULL,
	[IdUGE] [int] NULL,
	[IdContaContabil] [int] NULL,
	[HouveAlteracao] [bit] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ImportacaoPlanilha]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImportacaoPlanilha](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NomeArquivo] [varchar](150) NULL,
	[Processado] [bit] NOT NULL,
	[Login_Importacao] [varchar](100) NOT NULL,
	[Data_Importacao] [date] NOT NULL,
	[Login_Processamento] [varchar](20) NULL,
	[Data_Processamento] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Initial]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Initial](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](10) NOT NULL,
	[Description] [varchar](60) NULL,
	[BarCode] [varchar](2) NULL,
	[InstitutionId] [int] NOT NULL,
	[BudgetUnitId] [int] NULL,
	[ManagerUnitId] [int] NULL,
	[Status] [bit] NOT NULL,
	[RowId] [int] NULL,
 CONSTRAINT [PK_Initial] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Institution]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Institution](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](10) NULL,
	[Description] [varchar](120) NOT NULL,
	[Status] [bit] NOT NULL,
	[ManagerCode] [varchar](5) NULL,
	[NameManagerReduced] [varchar](500) NULL,
	[flagImplantado] [bit] NULL,
	[flagIntegracaoSiafem] [bit] NOT NULL,
 CONSTRAINT [PK_Orgao] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Inventario]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Inventario](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descricao] [varchar](120) NULL,
	[DataInventario] [datetime] NULL,
	[UaId] [int] NULL,
	[DivisaoId] [int] NULL,
	[Usuario] [varchar](11) NULL,
	[Status] [char](1) NULL,
	[UserId] [int] NULL,
	[ResponsavelId] [int] NOT NULL,
	[UgeId] [int] NULL,
	[TipoInventario] [int] NULL,
 CONSTRAINT [PK_Inventario] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Item_Siafisico]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Item_Siafisico](
	[Cod_Grupo] [float] NULL,
	[Nome_Grupo] [nvarchar](255) NULL,
	[Cod_Material] [float] NULL,
	[Nome_Material] [nvarchar](255) NULL,
	[Cod_Item_Mat] [float] NULL,
	[Nome_Item_Mat] [nvarchar](255) NULL,
	[STATUS] [char](2) NULL,
	[BEC] [char](2) NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemInventario]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemInventario](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InventarioId] [int] NULL,
	[Code] [varchar](250) NULL,
	[Item] [varchar](120) NULL,
	[Estado] [int] NULL,
	[AssetId] [int] NULL,
	[InitialName] [varchar](10) NULL,
	[AssetMovementsIdOriginal] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Level]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Level](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NULL,
	[Description] [varchar](30) NOT NULL,
	[Status] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LogAlteracaoDadosBP]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogAlteracaoDadosBP](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssetId] [int] NOT NULL,
	[AssetMovementId] [int] NULL,
	[Campo] [varchar](30) NULL,
	[ValorAntigo] [varchar](250) NULL,
	[ValorNovo] [varchar](250) NULL,
	[UserId] [int] NOT NULL,
	[DataHora] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ManagedSystem]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ManagedSystem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Status] [bit] NOT NULL,
	[Name] [varchar](20) NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[Sequence] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Manager]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Manager](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](120) NOT NULL,
	[ShortName] [varchar](25) NOT NULL,
	[AddressId] [int] NOT NULL,
	[Telephone] [varchar](20) NULL,
	[Code] [varchar](10) NULL,
	[Image] [image] NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Gestor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ManagerUnit]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ManagerUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BudgetUnitId] [int] NOT NULL,
	[Code] [varchar](10) NULL,
	[Description] [varchar](120) NOT NULL,
	[Status] [bit] NOT NULL,
	[ManagmentUnit_YearMonthStart] [varchar](6) NULL,
	[ManagmentUnit_YearMonthReference] [varchar](6) NULL,
	[RowId] [int] NULL,
	[FlagIntegracaoSiafem] [bit] NULL,
	[flagTratarComoOrgao] [bit] NOT NULL,
 CONSTRAINT [PK_UGE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Material]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Material](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[MaterialClassId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Material] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MaterialClass]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaterialClass](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[MaterialGroupId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_ClasseMaterial] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MaterialGroup]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaterialGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[Status] [bit] NOT NULL,
	[LifeCycle] [int] NULL,
	[RateDepreciationMonthly] [decimal](18, 2) NULL,
	[ResidualValue] [decimal](18, 2) NULL,
 CONSTRAINT [Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MaterialItem]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaterialItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[MaterialId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_ItemMaterial] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MaterialSubItem]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaterialSubItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MaterialItemId] [int] NOT NULL,
	[Code] [bigint] NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[BarCode] [varchar](25) NULL,
	[Lot] [bit] NOT NULL,
	[ActivityIndicator] [bit] NOT NULL,
	[ManagerId] [int] NOT NULL,
	[SpendingOriginId] [int] NOT NULL,
	[AuxiliaryAccountId] [int] NOT NULL,
	[SupplyUnitId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_SubItemMaterial] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MetaDataTypeServiceContabilizaSP]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MetaDataTypeServiceContabilizaSP](
	[Id] [int] NOT NULL,
	[Name] [varchar](75) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Mobile]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Mobile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Brand] [varchar](50) NULL,
	[Model] [varchar](50) NULL,
	[MacAddress] [varchar](50) NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[BudgetUnitId] [int] NOT NULL,
	[ManagerUnitId] [int] NOT NULL,
	[AdministrativeUnitId] [int] NOT NULL,
	[Status] [bit] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Module]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Module](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManagedSystemId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[Name] [varchar](100) NULL,
	[MenuName] [varchar](100) NULL,
	[Description] [varchar](300) NULL,
	[Path] [varchar](255) NULL,
	[ParentId] [int] NULL,
	[Sequence] [int] NULL,
 CONSTRAINT [PK__Modulo__3214EC0703317E3D] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MonthlyDepreciation]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthlyDepreciation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssetStartId] [int] NULL,
	[NumberIdentification] [varchar](20) NULL,
	[ManagerUnitId] [int] NOT NULL,
	[MaterialItemCode] [int] NOT NULL,
	[InitialId] [int] NOT NULL,
	[AcquisitionDate] [datetime] NOT NULL,
	[CurrentDate] [datetime] NOT NULL,
	[DateIncorporation] [datetime] NOT NULL,
	[LifeCycle] [smallint] NOT NULL,
	[CurrentMonth] [smallint] NOT NULL,
	[ValueAcquisition] [decimal](18, 2) NOT NULL,
	[CurrentValue] [decimal](18, 2) NOT NULL,
	[ResidualValue] [decimal](18, 2) NOT NULL,
	[RateDepreciationMonthly] [decimal](18, 2) NOT NULL,
	[AccumulatedDepreciation] [decimal](18, 2) NOT NULL,
	[UnfoldingValue] [decimal](18, 2) NULL,
	[Decree] [bit] NOT NULL,
	[ManagerUnitTransferId] [int] NULL,
	[MonthlyDepreciationId] [int] NULL,
	[MesAbertoDoAceite] [bit] NULL,
	[QtdLinhaRepetida] [int] NOT NULL,
 CONSTRAINT [PK_MonthlyDepreciation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY],
 CONSTRAINT [AK_MANAGER_UNIT_MONTHLY_DEPRECIATION] UNIQUE NONCLUSTERED 
(
	[AssetStartId] ASC,
	[ManagerUnitId] ASC,
	[MaterialItemCode] ASC,
	[CurrentDate] ASC,
	[QtdLinhaRepetida] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MonthyDepreciationIntial]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthyDepreciationIntial](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManagerUnitId] [int] NOT NULL,
	[DateStart] [datetime] NOT NULL,
	[DateEnd] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MovementType]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MovementType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](100) NULL,
	[GroupMovimentId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_TipoMovimento] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[NotaLancamentoPendenteSIAFEM]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotaLancamentoPendenteSIAFEM](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManagerUnitId] [int] NOT NULL,
	[AuditoriaIntegracaoId] [int] NOT NULL,
	[NumeroDocumentoSAM] [varchar](20) NULL,
	[DataHoraEnvioMsgWS] [datetime] NOT NULL,
	[ErroProcessamentoMsgWS] [varchar](max) NULL,
	[DataHoraReenvioMsgWS] [datetime] NULL,
	[StatusPendencia] [smallint] NOT NULL,
	[TipoNotaPendencia] [smallint] NOT NULL,
	[AssetMovementIds] [varchar](max) NULL,
 CONSTRAINT [PK_NotaLancamentoPendenteSIAFEM_ID] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Notification]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Titulo] [varchar](255) NOT NULL,
	[CorpoMensagem] [image] NULL,
	[DataCriacao] [datetime] NOT NULL,
	[Status] [bit] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OutSourced]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OutSourced](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](60) NULL,
	[CPFCNPJ] [varchar](14) NULL,
	[AddressId] [int] NULL,
	[Telephone] [varchar](11) NULL,
	[Status] [bit] NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[BudgetUnitId] [int] NOT NULL,
 CONSTRAINT [PK__Terceiro__3214EC072CF2ADDF] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PlanilhaData]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanilhaData](
	[Id] [bigint] NULL,
	[Sigla] [nvarchar](255) NULL,
	[Chapa] [float] NULL,
	[Data Aquisição] [nvarchar](255) NULL,
	[Valor de Aqusição] [money] NULL,
	[Item de Material] [float] NULL,
	[Data Aquisição Correta] [datetime] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Profile]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Profile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Status] [bit] NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[flagPerfilMaster] [bit] NULL,
 CONSTRAINT [PK__Perfil__3214EC0729572725] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssetId] [int] NULL,
	[AssetMovementsId] [int] NULL,
	[AuditoriaintegracaoId] [int] NULL,
 CONSTRAINT [PK_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] UNIQUE NONCLUSTERED 
(
	[AssetId] ASC,
	[AssetMovementsId] ASC,
	[AuditoriaintegracaoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RelationshipAuxiliaryAccountItemGroup]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipAuxiliaryAccountItemGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MaterialGroupId] [int] NULL,
	[AuxiliaryAccountId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_RelationshipAuxiliaryAccountItemGroup] UNIQUE NONCLUSTERED 
(
	[MaterialGroupId] ASC,
	[AuxiliaryAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RelationshipAuxiliaryAccountMovementType]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipAuxiliaryAccountMovementType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AuxiliaryAccountId] [int] NULL,
	[MovementTypeId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_RelationshipAuxiliaryAccountMovementType] UNIQUE NONCLUSTERED 
(
	[MovementTypeId] ASC,
	[AuxiliaryAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RelationshipModuleProfile]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipModuleProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModuleId] [int] NOT NULL,
	[ProfileId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK__RelationshipModuleProfile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RelationshipProfileLevel]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipProfileLevel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProfileId] [int] NOT NULL,
	[LevelId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RelationshipProfileManagedSystem]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipProfileManagedSystem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProfileId] [int] NOT NULL,
	[ManagedSystemId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RelationshipTransactionProfile]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipTransactionProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransactionId] [int] NOT NULL,
	[ProfileId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK__RelTrans__3214EC07300424B4] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RelationshipUserProfile]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipUserProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ProfileId] [int] NOT NULL,
	[DefaultProfile] [bit] NOT NULL,
	[FlagResponsavel] [bit] NULL,
 CONSTRAINT [PK_RelUsuarioPerfil_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RelationshipUserProfileInstitution]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipUserProfileInstitution](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RelationshipUserProfileId] [int] NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[ManagerId] [int] NULL,
	[BudgetUnitId] [int] NULL,
	[ManagerUnitId] [int] NULL,
	[AdministrativeUnitId] [int] NULL,
	[SectionId] [int] NULL,
 CONSTRAINT [PK_RelUsuarioPerfilOrgao] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Repair]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Repair](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InitialId] [int] NOT NULL,
	[AssetId] [int] NOT NULL,
	[NumberIdentification] [int] NULL,
	[PartNumberIdentification] [decimal](18, 4) NOT NULL,
	[Reaseon] [varchar](30) NULL,
	[Destination] [varchar](20) NULL,
	[DateExpectedReturn] [datetime] NULL,
	[DateOut] [datetime] NULL,
	[EstimatedCost] [decimal](18, 4) NULL,
	[FinalCost] [decimal](18, 4) NULL,
	[ReturnDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Responsible]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Responsible](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[Position] [varchar](60) NULL,
	[Status] [bit] NOT NULL,
	[AdministrativeUnitId] [int] NULL,
	[CPF] [varchar](11) NULL,
	[Email] [varchar](100) NULL,
 CONSTRAINT [PK_Responsavel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SaldoContabilAtualUGEContaContabil]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SaldoContabilAtualUGEContaContabil](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdOrgao] [int] NOT NULL,
	[IdUO] [int] NOT NULL,
	[IdUGE] [int] NOT NULL,
	[CodigoOrgao] [varchar](6) NULL,
	[CodigoUO] [varchar](6) NULL,
	[CodigoUGE] [varchar](6) NULL,
	[ContaContabil] [int] NULL,
	[ValorContabil] [decimal](18, 2) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SaldoContabilAtualUGEContaDepreciacao]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SaldoContabilAtualUGEContaDepreciacao](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdOrgao] [int] NOT NULL,
	[IdUO] [int] NOT NULL,
	[IdUGE] [int] NOT NULL,
	[CodigoOrgao] [varchar](6) NULL,
	[CodigoUO] [varchar](6) NULL,
	[CodigoUGE] [varchar](6) NULL,
	[DepreciationAccountId] [int] NULL,
	[ContaDepreciacao] [int] NULL,
	[DepreciacaoAcumulada] [decimal](18, 2) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Section]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Section](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdministrativeUnitId] [int] NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](120) NULL,
	[AddressId] [int] NULL,
	[Telephone] [varchar](20) NULL,
	[ResponsibleId] [int] NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Divisao] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ShortDescriptionItem]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShortDescriptionItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](255) NULL,
 CONSTRAINT [PK_DescriptionItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SiafemCalendar]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SiafemCalendar](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FiscalYear] [int] NOT NULL,
	[ReferenceMonth] [int] NOT NULL,
	[DateClosing] [date] NOT NULL,
	[Status] [bit] NOT NULL,
	[DataParaOperadores] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ReferenceMonth] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Signature]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Signature](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[PositionCamex] [varchar](100) NULL,
	[Post] [varchar](50) NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Assinatura] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SpendingOrigin]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SpendingOrigin](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[ActivityIndicator] [bit] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_NaturezaDespesa] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StateConservation]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StateConservation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](60) NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_EstadoConservacao] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Supplier]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Supplier](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CPFCNPJ] [varchar](14) NOT NULL,
	[Name] [varchar](60) NOT NULL,
	[AddressId] [int] NULL,
	[Telephone] [varchar](20) NULL,
	[Email] [varchar](50) NULL,
	[AdditionalData] [varchar](255) NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Fornecedor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SupplyUnit]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplyUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](12) NOT NULL,
	[Description] [varchar](30) NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_UnidadeFornecimento] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Support]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Support](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[BudgetUnitId] [int] NOT NULL,
	[ManagerUnitId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[SupportStatusProdespId] [int] NOT NULL,
	[InclusionDate] [datetime] NOT NULL,
	[CloseDate] [datetime] NULL,
	[Email] [varchar](200) NOT NULL,
	[SupportStatusUserId] [int] NOT NULL,
	[Responsavel] [varchar](200) NOT NULL,
	[ModuleId] [int] NOT NULL,
	[Login] [varchar](12) NOT NULL,
	[DataLogin] [datetime] NOT NULL,
	[SupportTypeId] [int] NOT NULL,
	[Observation] [varchar](max) NULL,
	[Status] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SupportStatusProdesp]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupportStatusProdesp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](200) NOT NULL,
	[Status] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SupportStatusUser]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupportStatusUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](200) NOT NULL,
	[Status] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SupportType]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupportType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](200) NULL,
	[Status] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TableDecreto]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TableDecreto](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ValueAcquisition] [decimal](18, 2) NOT NULL,
	[ValueUpdate] [decimal](18, 2) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Transaction]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModuleId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[Initial] [varchar](300) NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[Path] [varchar](255) NOT NULL,
	[TypeTransactionId] [int] NULL,
 CONSTRAINT [PK_Transacao] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TypeTransaction]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypeTransaction](
	[Id] [int] NOT NULL,
	[Description] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UGEDepreciaramAbrilDoisMilVinte]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UGEDepreciaramAbrilDoisMilVinte](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManagerUnitId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CPF] [varchar](11) NOT NULL,
	[Status] [bit] NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Email] [varchar](100) NULL,
	[Password] [varchar](255) NULL,
	[Phrase] [varchar](50) NOT NULL,
	[AddedDate] [datetime] NULL,
	[InvalidAttempts] [int] NULL,
	[Blocked] [bit] NOT NULL,
	[ChangePassword] [bit] NOT NULL,
	[AddressId] [int] NULL,
	[DataUltimoTreinamento] [datetime] NULL,
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Addressee] ADD  CONSTRAINT [DF_Addressee_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Asset] ADD  DEFAULT ((0)) FOR [monthUsed]
GO
ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_FlagAnimalAServico]  DEFAULT ((0)) FOR [FlagAnimalNaoServico]
GO
ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_flagVindoDoEstoque]  DEFAULT ((0)) FOR [flagVindoDoEstoque]
GO
ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_IndicadorOrigemInventarioInicial]  DEFAULT ((0)) FOR [IndicadorOrigemInventarioInicial]
GO
ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_DiferenciacaoChapa]  DEFAULT ('') FOR [DiferenciacaoChapa]
GO
ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_DiferenciacaoChapaAntiga]  DEFAULT ('') FOR [DiferenciacaoChapaAntiga]
GO
ALTER TABLE [dbo].[AssetMovements] ADD  CONSTRAINT [DF_AssetMovements_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[AssetNumberIdentification] ADD  CONSTRAINT [DF_AssetNumberIdentification_DiferenciacaoChapa]  DEFAULT ('') FOR [DiferenciacaoChapa]
GO
ALTER TABLE [dbo].[AuxiliaryAccount] ADD  CONSTRAINT [DF_AuxiliaryAccount_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[AuxiliaryAccount] ADD  CONSTRAINT [DF_AuxiliaryAccount_GrupoRelacionado]  DEFAULT ((0)) FOR [RelacionadoBP]
GO
ALTER TABLE [dbo].[BPsExcluidos] ADD  CONSTRAINT [DF_BPsExcluidos_FlagModoExclusao]  DEFAULT ((0)) FOR [FlagModoExclusao]
GO
ALTER TABLE [dbo].[BudgetUnit] ADD  CONSTRAINT [DF_BudgetUnit_Direct]  DEFAULT ((1)) FOR [Direct]
GO
ALTER TABLE [dbo].[BudgetUnit] ADD  CONSTRAINT [DF_BudgetUnit_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[CostCenter] ADD  CONSTRAINT [DF_CostCenter_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[ELMAH_Error] ADD  CONSTRAINT [DF_ELMAH_Error_ErrorId]  DEFAULT (newid()) FOR [ErrorId]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD  CONSTRAINT [DF__EventServiceContabilizaSP_StockType]  DEFAULT ('PERMANENTE') FOR [StockType]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD  CONSTRAINT [DF__EventServiceContabilizaSP_MaterialType]  DEFAULT ('MaterialPermanente') FOR [MaterialType]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD  CONSTRAINT [DF__EventServiceContabilizaSP_SpecificControl]  DEFAULT ('') FOR [SpecificControl]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD  CONSTRAINT [DF__EventServiceContabilizaSP_SpecificInputControl]  DEFAULT ('') FOR [SpecificInputControl]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD  CONSTRAINT [DF__EventServiceContabilizaSP_SpecificOutputControl]  DEFAULT ('') FOR [SpecificOutputControl]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD  CONSTRAINT [DF_EventServiceContabilizaSP_Status]  DEFAULT ('true') FOR [Status]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD  CONSTRAINT [DF__EventServiceContabilizaSP_StockDescription]  DEFAULT ('ESTOQUE LONGO PRAZO') FOR [StockDescription]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD  CONSTRAINT [DF__EventServiceContabilizaSP_FavoreceDestino]  DEFAULT ('0') FOR [FavoreceDestino]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD  CONSTRAINT [DF__EventServiceContabilizaSP_UtilizaTipoMovimentacaoContabilizaSPAlternativa]  DEFAULT ('0') FOR [UtilizaTipoMovimentacaoContabilizaSPAlternativa]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD  CONSTRAINT [DF__EventServiceContabilizaSP_VerificaSeOrigemUtilizaSAM]  DEFAULT ('0') FOR [VerificaSeOrigemUtilizaSAM]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] ADD  CONSTRAINT [DF_EventServiceContabilizaSP_DataAtivacaoTipoMovimentacao]  DEFAULT (getdate()) FOR [DataAtivacaoTipoMovimentacao]
GO
ALTER TABLE [dbo].[Historic] ADD  DEFAULT (getdate()) FOR [InsertDate]
GO
ALTER TABLE [dbo].[Initial] ADD  CONSTRAINT [DF_Initial_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Institution] ADD  CONSTRAINT [DF_Institution_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Institution] ADD  CONSTRAINT [DF_Institution_flagImplantado]  DEFAULT ('0') FOR [flagImplantado]
GO
ALTER TABLE [dbo].[Institution] ADD  CONSTRAINT [DF__Institution_flagIntegracaoSiafem]  DEFAULT ((0)) FOR [flagIntegracaoSiafem]
GO
ALTER TABLE [dbo].[Level] ADD  DEFAULT (NULL) FOR [ParentId]
GO
ALTER TABLE [dbo].[Level] ADD  CONSTRAINT [DF_Level_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Manager] ADD  CONSTRAINT [DF_Manager_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[ManagerUnit] ADD  CONSTRAINT [DF_ManagerUnit_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[ManagerUnit] ADD  DEFAULT ((0)) FOR [flagTratarComoOrgao]
GO
ALTER TABLE [dbo].[Material] ADD  CONSTRAINT [DF_Material_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[MaterialClass] ADD  CONSTRAINT [DF_MaterialClass_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[MaterialGroup] ADD  CONSTRAINT [DF_MaterialGroup_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[MaterialItem] ADD  CONSTRAINT [DF_MaterialItem_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[MaterialSubItem] ADD  CONSTRAINT [DF_MaterialSubItem_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Module] ADD  CONSTRAINT [DF_Module_Sequence]  DEFAULT ((0)) FOR [Sequence]
GO
ALTER TABLE [dbo].[MonthlyDepreciation] ADD  CONSTRAINT [DF_MonthlyDepreciation__Decree]  DEFAULT ((0)) FOR [Decree]
GO
ALTER TABLE [dbo].[MonthlyDepreciation] ADD  CONSTRAINT [DF_MonthlyDepreciation_QtdLinhaRepetida]  DEFAULT ((0)) FOR [QtdLinhaRepetida]
GO
ALTER TABLE [dbo].[MovementType] ADD  CONSTRAINT [DF_MovementType_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[NotaLancamentoPendenteSIAFEM] ADD  DEFAULT ((2)) FOR [TipoNotaPendencia]
GO
ALTER TABLE [dbo].[OutSourced] ADD  CONSTRAINT [DF_OutSourced_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Signature] ADD  CONSTRAINT [DF_Signature_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[SpendingOrigin] ADD  CONSTRAINT [DF_SpendingOrigin_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[StateConservation] ADD  CONSTRAINT [DF_StateConservation_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Supplier] ADD  CONSTRAINT [DF_Supplier_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[SupplyUnit] ADD  CONSTRAINT [DF_SupplyUnit_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_Usuario_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__Usuario__DataCad__4D94879B]  DEFAULT (getdate()) FOR [AddedDate]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_Usuario_TentivasInvalidas]  DEFAULT ((0)) FOR [InvalidAttempts]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF__Usuario__Bloquea__4F7CD00D]  DEFAULT ((0)) FOR [Blocked]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_Usuario_Bloqueado]  DEFAULT ((0)) FOR [ChangePassword]
GO
ALTER TABLE [dbo].[AccountingClosing]  WITH CHECK ADD  CONSTRAINT [FK_AccountingClosing_AuditoriaIntegracao] FOREIGN KEY([AuditoriaIntegracaoId])
REFERENCES [dbo].[AuditoriaIntegracao] ([Id])
GO
ALTER TABLE [dbo].[AccountingClosing] CHECK CONSTRAINT [FK_AccountingClosing_AuditoriaIntegracao]
GO
ALTER TABLE [dbo].[AccountingClosingExcluidos]  WITH CHECK ADD  CONSTRAINT [FK_AccountingClosingExcluidos_AuditoriaIntegracao] FOREIGN KEY([AuditoriaIntegracaoId])
REFERENCES [dbo].[AuditoriaIntegracao] ([Id])
GO
ALTER TABLE [dbo].[AccountingClosingExcluidos] CHECK CONSTRAINT [FK_AccountingClosingExcluidos_AuditoriaIntegracao]
GO
ALTER TABLE [dbo].[AccountingClosingExcluidos]  WITH CHECK ADD  CONSTRAINT [FK_AccountingClosingExcluidos_AuditoriaIntegracao_Estorno] FOREIGN KEY([AuditoriaIntegracaoIdEstorno])
REFERENCES [dbo].[AuditoriaIntegracao] ([Id])
GO
ALTER TABLE [dbo].[AccountingClosingExcluidos] CHECK CONSTRAINT [FK_AccountingClosingExcluidos_AuditoriaIntegracao_Estorno]
GO
ALTER TABLE [dbo].[AdministrativeUnit]  WITH CHECK ADD  CONSTRAINT [FK_AdministrativeUnit_ManagerUnit] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[AdministrativeUnit] CHECK CONSTRAINT [FK_AdministrativeUnit_ManagerUnit]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Initial] FOREIGN KEY([InitialId])
REFERENCES [dbo].[Initial] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_Initial]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_ManagerUnit] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_ManagerUnit]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_MovementType] FOREIGN KEY([MovementTypeId])
REFERENCES [dbo].[MovementType] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_MovementType]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_OutSourced] FOREIGN KEY([OutSourcedId])
REFERENCES [dbo].[OutSourced] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_OutSourced]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_ShortDescriptionItem] FOREIGN KEY([ShortDescriptionItemId])
REFERENCES [dbo].[ShortDescriptionItem] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_ShortDescriptionItem]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_Supplier]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Id] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_Asset_Id]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH CHECK ADD  CONSTRAINT [FK_AssetMovements_AdministrativeUnit] FOREIGN KEY([AdministrativeUnitId])
REFERENCES [dbo].[AdministrativeUnit] ([Id])
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_AssetMovements_AdministrativeUnit]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH CHECK ADD  CONSTRAINT [FK_AssetMovements_Asset] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_AssetMovements_Asset]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH CHECK ADD  CONSTRAINT [FK_AssetMovements_AuxiliaryAccount] FOREIGN KEY([AuxiliaryAccountId])
REFERENCES [dbo].[AuxiliaryAccount] ([Id])
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_AssetMovements_AuxiliaryAccount]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH CHECK ADD  CONSTRAINT [FK_AssetMovements_BudgetUnit] FOREIGN KEY([BudgetUnitId])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_AssetMovements_BudgetUnit]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH CHECK ADD  CONSTRAINT [FK_AssetMovements_Institution] FOREIGN KEY([InstitutionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_AssetMovements_Institution]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH CHECK ADD  CONSTRAINT [FK_AssetMovements_ManagerUnit] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_AssetMovements_ManagerUnit]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH CHECK ADD  CONSTRAINT [FK_AssetMovements_MovementType] FOREIGN KEY([MovementTypeId])
REFERENCES [dbo].[MovementType] ([Id])
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_AssetMovements_MovementType]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH NOCHECK ADD  CONSTRAINT [FK_AssetMovements_Responsible] FOREIGN KEY([ResponsibleId])
REFERENCES [dbo].[Responsible] ([Id])
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_AssetMovements_Responsible]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH CHECK ADD  CONSTRAINT [FK_AssetMovements_Section] FOREIGN KEY([SectionId])
REFERENCES [dbo].[Section] ([Id])
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_AssetMovements_Section]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH CHECK ADD  CONSTRAINT [FK_AssetMovements_SourceDestinyManagerUnit] FOREIGN KEY([SourceDestiny_ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_AssetMovements_SourceDestinyManagerUnit]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH CHECK ADD  CONSTRAINT [FK_AssetMovements_User] FOREIGN KEY([LoginEstorno])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_AssetMovements_User]
GO
ALTER TABLE [dbo].[AssetMovements]  WITH CHECK ADD  CONSTRAINT [FK_MovementType_id] FOREIGN KEY([MovementTypeId])
REFERENCES [dbo].[MovementType] ([Id])
GO
ALTER TABLE [dbo].[AssetMovements] CHECK CONSTRAINT [FK_MovementType_id]
GO
ALTER TABLE [dbo].[AssetNumberIdentification]  WITH CHECK ADD  CONSTRAINT [FK_AssetNumberIdentification] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])
GO
ALTER TABLE [dbo].[AssetNumberIdentification] CHECK CONSTRAINT [FK_AssetNumberIdentification]
GO
ALTER TABLE [dbo].[AssetNumberIdentification]  WITH CHECK ADD  CONSTRAINT [FK_AssetNumberIdentification_Initial] FOREIGN KEY([InitialId])
REFERENCES [dbo].[Initial] ([Id])
GO
ALTER TABLE [dbo].[AssetNumberIdentification] CHECK CONSTRAINT [FK_AssetNumberIdentification_Initial]
GO
ALTER TABLE [dbo].[AttachmentSupport]  WITH CHECK ADD  CONSTRAINT [FK_AttachmentSupport_Support] FOREIGN KEY([SupportId])
REFERENCES [dbo].[Support] ([Id])
GO
ALTER TABLE [dbo].[AttachmentSupport] CHECK CONSTRAINT [FK_AttachmentSupport_Support]
GO
ALTER TABLE [dbo].[AuditoriaIntegracao]  WITH CHECK ADD  CONSTRAINT [FK_AuditoriaIntegracao_ManagerUnit] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[AuditoriaIntegracao] CHECK CONSTRAINT [FK_AuditoriaIntegracao_ManagerUnit]
GO
ALTER TABLE [dbo].[AuxiliaryAccount]  WITH CHECK ADD  CONSTRAINT [FK_AuxiliaryAccount_DepreciationAccount] FOREIGN KEY([DepreciationAccountId])
REFERENCES [dbo].[DepreciationAccount] ([Id])
GO
ALTER TABLE [dbo].[AuxiliaryAccount] CHECK CONSTRAINT [FK_AuxiliaryAccount_DepreciationAccount]
GO
ALTER TABLE [dbo].[BPsARealizaremReclassificacaoContabil]  WITH CHECK ADD  CONSTRAINT [FK_BPsARealizaremReclassificacaoContabil_Asset] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])
GO
ALTER TABLE [dbo].[BPsARealizaremReclassificacaoContabil] CHECK CONSTRAINT [FK_BPsARealizaremReclassificacaoContabil_Asset]
GO
ALTER TABLE [dbo].[BPsARealizaremReclassificacaoContabil]  WITH CHECK ADD  CONSTRAINT [FK_BPsARealizaremReclassificacaoContabil_ManagerUnit] FOREIGN KEY([IdUGE])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[BPsARealizaremReclassificacaoContabil] CHECK CONSTRAINT [FK_BPsARealizaremReclassificacaoContabil_ManagerUnit]
GO
ALTER TABLE [dbo].[BPsExcluidos]  WITH CHECK ADD  CONSTRAINT [FK_BPsExcluidos_AdministrativeUnit] FOREIGN KEY([AdministrativeUnitId])
REFERENCES [dbo].[AdministrativeUnit] ([Id])
GO
ALTER TABLE [dbo].[BPsExcluidos] CHECK CONSTRAINT [FK_BPsExcluidos_AdministrativeUnit]
GO
ALTER TABLE [dbo].[BPsExcluidos]  WITH CHECK ADD  CONSTRAINT [FK_BPsExcluidos_Initial] FOREIGN KEY([InitialId])
REFERENCES [dbo].[Initial] ([Id])
GO
ALTER TABLE [dbo].[BPsExcluidos] CHECK CONSTRAINT [FK_BPsExcluidos_Initial]
GO
ALTER TABLE [dbo].[BPsExcluidos]  WITH CHECK ADD  CONSTRAINT [FK_BPsExcluidos_ManagerUnit] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[BPsExcluidos] CHECK CONSTRAINT [FK_BPsExcluidos_ManagerUnit]
GO
ALTER TABLE [dbo].[BPsExcluidos]  WITH CHECK ADD  CONSTRAINT [FK_BPsExcluidos_MovementType] FOREIGN KEY([TipoIncorporacao])
REFERENCES [dbo].[MovementType] ([Id])
GO
ALTER TABLE [dbo].[BPsExcluidos] CHECK CONSTRAINT [FK_BPsExcluidos_MovementType]
GO
ALTER TABLE [dbo].[BPsExcluidos]  WITH NOCHECK ADD  CONSTRAINT [FK_BPsExcluidos_Responsible] FOREIGN KEY([ResponsibleId])
REFERENCES [dbo].[Responsible] ([Id])
GO
ALTER TABLE [dbo].[BPsExcluidos] CHECK CONSTRAINT [FK_BPsExcluidos_Responsible]
GO
ALTER TABLE [dbo].[BudgetUnit]  WITH CHECK ADD  CONSTRAINT [FK_BudgetUnit_Institution] FOREIGN KEY([InstitutionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[BudgetUnit] CHECK CONSTRAINT [FK_BudgetUnit_Institution]
GO
ALTER TABLE [dbo].[Closing]  WITH CHECK ADD  CONSTRAINT [FK_Closing_Asset] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])
GO
ALTER TABLE [dbo].[Closing] CHECK CONSTRAINT [FK_Closing_Asset]
GO
ALTER TABLE [dbo].[Closing]  WITH CHECK ADD  CONSTRAINT [FK_Closing_AssetMovements] FOREIGN KEY([AssetMovementsId])
REFERENCES [dbo].[AssetMovements] ([Id])
GO
ALTER TABLE [dbo].[Closing] CHECK CONSTRAINT [FK_Closing_AssetMovements]
GO
ALTER TABLE [dbo].[Closing]  WITH CHECK ADD  CONSTRAINT [FK_Closing_ManagmentUnit] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[Closing] CHECK CONSTRAINT [FK_Closing_ManagmentUnit]
GO
ALTER TABLE [dbo].[Closing]  WITH CHECK ADD  CONSTRAINT [FK_Closing_User] FOREIGN KEY([LoginID])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Closing] CHECK CONSTRAINT [FK_Closing_User]
GO
ALTER TABLE [dbo].[DadosImportacaoBens]  WITH CHECK ADD  CONSTRAINT [FK_DadosImportacaoBens_ImportacaoPlanilha] FOREIGN KEY([ImportacaoPlanilhaId])
REFERENCES [dbo].[ImportacaoPlanilha] ([Id])
GO
ALTER TABLE [dbo].[DadosImportacaoBens] CHECK CONSTRAINT [FK_DadosImportacaoBens_ImportacaoPlanilha]
GO
ALTER TABLE [dbo].[DadosImportacaoUsuario]  WITH CHECK ADD  CONSTRAINT [FK_DadosImportacaoUsuario_ImportacaoPlanilha] FOREIGN KEY([ImportacaoPlanilhaId])
REFERENCES [dbo].[ImportacaoPlanilha] ([Id])
GO
ALTER TABLE [dbo].[DadosImportacaoUsuario] CHECK CONSTRAINT [FK_DadosImportacaoUsuario_ImportacaoPlanilha]
GO
ALTER TABLE [dbo].[DepreciationAccountingClosing]  WITH CHECK ADD  CONSTRAINT [FK_DepreciationAccountingClosing_AccountingClosing] FOREIGN KEY([AccountingClosingId])
REFERENCES [dbo].[AccountingClosing] ([Id])
GO
ALTER TABLE [dbo].[DepreciationAccountingClosing] CHECK CONSTRAINT [FK_DepreciationAccountingClosing_AccountingClosing]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP]  WITH CHECK ADD  CONSTRAINT [FK_MetaDataType_AccountingValueField] FOREIGN KEY([MetaDataType_AccountingValueField])
REFERENCES [dbo].[MetaDataTypeServiceContabilizaSP] ([Id])
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] CHECK CONSTRAINT [FK_MetaDataType_AccountingValueField]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP]  WITH CHECK ADD  CONSTRAINT [FK_MetaDataType_DateField] FOREIGN KEY([MetaDataType_DateField])
REFERENCES [dbo].[MetaDataTypeServiceContabilizaSP] ([Id])
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] CHECK CONSTRAINT [FK_MetaDataType_DateField]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP]  WITH CHECK ADD  CONSTRAINT [FK_MetaDataType_MovementTypeContabilizaSP] FOREIGN KEY([MetaDataType_MovementTypeContabilizaSP])
REFERENCES [dbo].[MetaDataTypeServiceContabilizaSP] ([Id])
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] CHECK CONSTRAINT [FK_MetaDataType_MovementTypeContabilizaSP]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP]  WITH CHECK ADD  CONSTRAINT [FK_MetaDataType_StockDestination] FOREIGN KEY([MetaDataType_StockDestination])
REFERENCES [dbo].[MetaDataTypeServiceContabilizaSP] ([Id])
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] CHECK CONSTRAINT [FK_MetaDataType_StockDestination]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP]  WITH CHECK ADD  CONSTRAINT [FK_MetaDataType_StockSource] FOREIGN KEY([MetaDataType_StockSource])
REFERENCES [dbo].[MetaDataTypeServiceContabilizaSP] ([Id])
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] CHECK CONSTRAINT [FK_MetaDataType_StockSource]
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP]  WITH CHECK ADD  CONSTRAINT [FK_MovementTypeId] FOREIGN KEY([InputOutputReclassificationDepreciationTypeCode])
REFERENCES [dbo].[MovementType] ([Id])
GO
ALTER TABLE [dbo].[EventServiceContabilizaSP] CHECK CONSTRAINT [FK_MovementTypeId]
GO
ALTER TABLE [dbo].[Exchange]  WITH CHECK ADD  CONSTRAINT [FK_AssetExchange] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])
GO
ALTER TABLE [dbo].[Exchange] CHECK CONSTRAINT [FK_AssetExchange]
GO
ALTER TABLE [dbo].[Historic]  WITH CHECK ADD  CONSTRAINT [FK_Historico_Usuario] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Historic] CHECK CONSTRAINT [FK_Historico_Usuario]
GO
ALTER TABLE [dbo].[HistoricoCampo]  WITH CHECK ADD  CONSTRAINT [FK_HistoricoCampo_Historico] FOREIGN KEY([HistoricoId])
REFERENCES [dbo].[Historico] ([Id])
GO
ALTER TABLE [dbo].[HistoricoCampo] CHECK CONSTRAINT [FK_HistoricoCampo_Historico]
GO
ALTER TABLE [dbo].[HistoricoValoresDecreto]  WITH CHECK ADD  CONSTRAINT [FK_HistoricoValoresDecreto_Asset] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])
GO
ALTER TABLE [dbo].[HistoricoValoresDecreto] CHECK CONSTRAINT [FK_HistoricoValoresDecreto_Asset]
GO
ALTER TABLE [dbo].[HistoricoValoresDecreto]  WITH CHECK ADD  CONSTRAINT [FK_HistoricoValoresDecreto_User] FOREIGN KEY([LoginId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[HistoricoValoresDecreto] CHECK CONSTRAINT [FK_HistoricoValoresDecreto_User]
GO
ALTER TABLE [dbo].[HistoricSupport]  WITH CHECK ADD  CONSTRAINT [FK_HistoricSupport_Support] FOREIGN KEY([SupportId])
REFERENCES [dbo].[Support] ([Id])
GO
ALTER TABLE [dbo].[HistoricSupport] CHECK CONSTRAINT [FK_HistoricSupport_Support]
GO
ALTER TABLE [dbo].[HistoricSupport]  WITH CHECK ADD  CONSTRAINT [FK_HistoricSupport_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[HistoricSupport] CHECK CONSTRAINT [FK_HistoricSupport_User]
GO
ALTER TABLE [dbo].[HouveAlteracaoContabil]  WITH CHECK ADD  CONSTRAINT [FK_HouveAlteracaoContabil_BudgetUnit] FOREIGN KEY([IdUO])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[HouveAlteracaoContabil] CHECK CONSTRAINT [FK_HouveAlteracaoContabil_BudgetUnit]
GO
ALTER TABLE [dbo].[HouveAlteracaoContabil]  WITH CHECK ADD  CONSTRAINT [FK_HouveAlteracaoContabil_Institution] FOREIGN KEY([IdOrgao])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[HouveAlteracaoContabil] CHECK CONSTRAINT [FK_HouveAlteracaoContabil_Institution]
GO
ALTER TABLE [dbo].[HouveAlteracaoContabil]  WITH CHECK ADD  CONSTRAINT [FK_HouveAlteracaoContabil_ManagerUnit] FOREIGN KEY([IdUGE])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[HouveAlteracaoContabil] CHECK CONSTRAINT [FK_HouveAlteracaoContabil_ManagerUnit]
GO
ALTER TABLE [dbo].[Initial]  WITH CHECK ADD  CONSTRAINT [FK_Initial_BudgetUnit] FOREIGN KEY([BudgetUnitId])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[Initial] CHECK CONSTRAINT [FK_Initial_BudgetUnit]
GO
ALTER TABLE [dbo].[Initial]  WITH CHECK ADD  CONSTRAINT [FK_Initial_Institution] FOREIGN KEY([InstitutionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[Initial] CHECK CONSTRAINT [FK_Initial_Institution]
GO
ALTER TABLE [dbo].[Inventario]  WITH CHECK ADD  CONSTRAINT [FK_Inventario_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Inventario] CHECK CONSTRAINT [FK_Inventario_User]
GO
ALTER TABLE [dbo].[Inventario]  WITH CHECK ADD  CONSTRAINT [FK_SectionInventario] FOREIGN KEY([DivisaoId])
REFERENCES [dbo].[Section] ([Id])
GO
ALTER TABLE [dbo].[Inventario] CHECK CONSTRAINT [FK_SectionInventario]
GO
ALTER TABLE [dbo].[Level]  WITH CHECK ADD FOREIGN KEY([ParentId])
REFERENCES [dbo].[Level] ([Id])
GO
ALTER TABLE [dbo].[LogAlteracaoDadosBP]  WITH CHECK ADD  CONSTRAINT [FK_LogAlteracaoDadosBP_Asset] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])
GO
ALTER TABLE [dbo].[LogAlteracaoDadosBP] CHECK CONSTRAINT [FK_LogAlteracaoDadosBP_Asset]
GO
ALTER TABLE [dbo].[LogAlteracaoDadosBP]  WITH CHECK ADD  CONSTRAINT [FK_LogAlteracaoDadosBP_AssetMovements] FOREIGN KEY([AssetMovementId])
REFERENCES [dbo].[AssetMovements] ([Id])
GO
ALTER TABLE [dbo].[LogAlteracaoDadosBP] CHECK CONSTRAINT [FK_LogAlteracaoDadosBP_AssetMovements]
GO
ALTER TABLE [dbo].[LogAlteracaoDadosBP]  WITH CHECK ADD  CONSTRAINT [FK_LogAlteracaoDadosBP_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[LogAlteracaoDadosBP] CHECK CONSTRAINT [FK_LogAlteracaoDadosBP_User]
GO
ALTER TABLE [dbo].[Manager]  WITH CHECK ADD  CONSTRAINT [FK_Gestor_Endereco] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[Manager] CHECK CONSTRAINT [FK_Gestor_Endereco]
GO
ALTER TABLE [dbo].[ManagerUnit]  WITH CHECK ADD  CONSTRAINT [FK_ManagerUnit_BudgetUnit] FOREIGN KEY([BudgetUnitId])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[ManagerUnit] CHECK CONSTRAINT [FK_ManagerUnit_BudgetUnit]
GO
ALTER TABLE [dbo].[Material]  WITH CHECK ADD  CONSTRAINT [FK_Material_ClasseMaterial] FOREIGN KEY([MaterialClassId])
REFERENCES [dbo].[MaterialClass] ([Id])
GO
ALTER TABLE [dbo].[Material] CHECK CONSTRAINT [FK_Material_ClasseMaterial]
GO
ALTER TABLE [dbo].[MaterialClass]  WITH CHECK ADD  CONSTRAINT [FK_ClasseMaterial_GrupoMaterial] FOREIGN KEY([MaterialGroupId])
REFERENCES [dbo].[MaterialGroup] ([Id])
GO
ALTER TABLE [dbo].[MaterialClass] CHECK CONSTRAINT [FK_ClasseMaterial_GrupoMaterial]
GO
ALTER TABLE [dbo].[MaterialItem]  WITH CHECK ADD  CONSTRAINT [FK_ItemMaterial_Material] FOREIGN KEY([MaterialId])
REFERENCES [dbo].[Material] ([Id])
GO
ALTER TABLE [dbo].[MaterialItem] CHECK CONSTRAINT [FK_ItemMaterial_Material]
GO
ALTER TABLE [dbo].[MaterialSubItem]  WITH CHECK ADD  CONSTRAINT [FK_SubItemMaterial_ContaAuxiliar] FOREIGN KEY([AuxiliaryAccountId])
REFERENCES [dbo].[AuxiliaryAccount] ([Id])
GO
ALTER TABLE [dbo].[MaterialSubItem] CHECK CONSTRAINT [FK_SubItemMaterial_ContaAuxiliar]
GO
ALTER TABLE [dbo].[MaterialSubItem]  WITH CHECK ADD  CONSTRAINT [FK_SubItemMaterial_Gestor] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Manager] ([Id])
GO
ALTER TABLE [dbo].[MaterialSubItem] CHECK CONSTRAINT [FK_SubItemMaterial_Gestor]
GO
ALTER TABLE [dbo].[MaterialSubItem]  WITH CHECK ADD  CONSTRAINT [FK_SubItemMaterial_ItemMaterial] FOREIGN KEY([MaterialItemId])
REFERENCES [dbo].[MaterialItem] ([Id])
GO
ALTER TABLE [dbo].[MaterialSubItem] CHECK CONSTRAINT [FK_SubItemMaterial_ItemMaterial]
GO
ALTER TABLE [dbo].[MaterialSubItem]  WITH CHECK ADD  CONSTRAINT [FK_SubItemMaterial_NaturezaDespesa] FOREIGN KEY([SpendingOriginId])
REFERENCES [dbo].[SpendingOrigin] ([Id])
GO
ALTER TABLE [dbo].[MaterialSubItem] CHECK CONSTRAINT [FK_SubItemMaterial_NaturezaDespesa]
GO
ALTER TABLE [dbo].[MaterialSubItem]  WITH CHECK ADD  CONSTRAINT [FK_SubItemMaterial_UnidadeFornecimento] FOREIGN KEY([SupplyUnitId])
REFERENCES [dbo].[SupplyUnit] ([Id])
GO
ALTER TABLE [dbo].[MaterialSubItem] CHECK CONSTRAINT [FK_SubItemMaterial_UnidadeFornecimento]
GO
ALTER TABLE [dbo].[Module]  WITH NOCHECK ADD  CONSTRAINT [FK__Modulo__IdPai__060DEAE8] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Module] ([Id])
GO
ALTER TABLE [dbo].[Module] NOCHECK CONSTRAINT [FK__Modulo__IdPai__060DEAE8]
GO
ALTER TABLE [dbo].[Module]  WITH CHECK ADD  CONSTRAINT [FK__Modulo__IdSistem__0CBAE877] FOREIGN KEY([ManagedSystemId])
REFERENCES [dbo].[ManagedSystem] ([Id])
GO
ALTER TABLE [dbo].[Module] CHECK CONSTRAINT [FK__Modulo__IdSistem__0CBAE877]
GO
ALTER TABLE [dbo].[MonthlyDepreciation]  WITH CHECK ADD  CONSTRAINT [PK_MANAGER_UNIT_MONTHLY_DEPRECIATION] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[MonthlyDepreciation] CHECK CONSTRAINT [PK_MANAGER_UNIT_MONTHLY_DEPRECIATION]
GO
ALTER TABLE [dbo].[MovementType]  WITH CHECK ADD  CONSTRAINT [FK_MovementType_GroupMoviment] FOREIGN KEY([GroupMovimentId])
REFERENCES [dbo].[GroupMoviment] ([Id])
GO
ALTER TABLE [dbo].[MovementType] CHECK CONSTRAINT [FK_MovementType_GroupMoviment]
GO
ALTER TABLE [dbo].[NotaLancamentoPendenteSIAFEM]  WITH CHECK ADD  CONSTRAINT [FK_AuditoriaIntegracaoId] FOREIGN KEY([AuditoriaIntegracaoId])
REFERENCES [dbo].[AuditoriaIntegracao] ([Id])
GO
ALTER TABLE [dbo].[NotaLancamentoPendenteSIAFEM] CHECK CONSTRAINT [FK_AuditoriaIntegracaoId]
GO
ALTER TABLE [dbo].[NotaLancamentoPendenteSIAFEM]  WITH CHECK ADD  CONSTRAINT [FK_ManagerUnitId] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[NotaLancamentoPendenteSIAFEM] CHECK CONSTRAINT [FK_ManagerUnitId]
GO
ALTER TABLE [dbo].[OutSourced]  WITH CHECK ADD  CONSTRAINT [FK_OutSourced_BudgetUnit] FOREIGN KEY([BudgetUnitId])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[OutSourced] CHECK CONSTRAINT [FK_OutSourced_BudgetUnit]
GO
ALTER TABLE [dbo].[OutSourced]  WITH CHECK ADD  CONSTRAINT [FK_OutSourced_Institution] FOREIGN KEY([InstitutionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[OutSourced] CHECK CONSTRAINT [FK_OutSourced_Institution]
GO
ALTER TABLE [dbo].[OutSourced]  WITH CHECK ADD  CONSTRAINT [FK_Terceiro_Endereco] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[OutSourced] CHECK CONSTRAINT [FK_Terceiro_Endereco]
GO
ALTER TABLE [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao]  WITH CHECK ADD  CONSTRAINT [FK_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao_Asset] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])
GO
ALTER TABLE [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] CHECK CONSTRAINT [FK_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao_Asset]
GO
ALTER TABLE [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao]  WITH CHECK ADD  CONSTRAINT [FK_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao_AssetMovements] FOREIGN KEY([AssetMovementsId])
REFERENCES [dbo].[AssetMovements] ([Id])
GO
ALTER TABLE [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] CHECK CONSTRAINT [FK_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao_AssetMovements]
GO
ALTER TABLE [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao]  WITH CHECK ADD  CONSTRAINT [FK_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao_AuditoriaIntegracao] FOREIGN KEY([AuditoriaintegracaoId])
REFERENCES [dbo].[AuditoriaIntegracao] ([Id])
GO
ALTER TABLE [dbo].[Relacionamento__Asset_AssetMovements_AuditoriaIntegracao] CHECK CONSTRAINT [FK_Relacionamento__Asset_AssetMovements_AuditoriaIntegracao_AuditoriaIntegracao]
GO
ALTER TABLE [dbo].[RelationshipAuxiliaryAccountItemGroup]  WITH CHECK ADD FOREIGN KEY([AuxiliaryAccountId])
REFERENCES [dbo].[AuxiliaryAccount] ([Id])
GO
ALTER TABLE [dbo].[RelationshipAuxiliaryAccountItemGroup]  WITH CHECK ADD FOREIGN KEY([MaterialGroupId])
REFERENCES [dbo].[MaterialGroup] ([Id])
GO
ALTER TABLE [dbo].[RelationshipAuxiliaryAccountMovementType]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipAuxiliaryAccountMovementType_AuxiliaryAccount] FOREIGN KEY([AuxiliaryAccountId])
REFERENCES [dbo].[AuxiliaryAccount] ([Id])
GO
ALTER TABLE [dbo].[RelationshipAuxiliaryAccountMovementType] CHECK CONSTRAINT [FK_RelationshipAuxiliaryAccountMovementType_AuxiliaryAccount]
GO
ALTER TABLE [dbo].[RelationshipAuxiliaryAccountMovementType]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipAuxiliaryAccountMovementType_MovementType] FOREIGN KEY([MovementTypeId])
REFERENCES [dbo].[MovementType] ([Id])
GO
ALTER TABLE [dbo].[RelationshipAuxiliaryAccountMovementType] CHECK CONSTRAINT [FK_RelationshipAuxiliaryAccountMovementType_MovementType]
GO
ALTER TABLE [dbo].[RelationshipModuleProfile]  WITH CHECK ADD  CONSTRAINT [FK_RelModuleProfile_Module] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Module] ([Id])
GO
ALTER TABLE [dbo].[RelationshipModuleProfile] CHECK CONSTRAINT [FK_RelModuleProfile_Module]
GO
ALTER TABLE [dbo].[RelationshipModuleProfile]  WITH CHECK ADD  CONSTRAINT [FK_RelModuleProfile_Profile] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profile] ([Id])
GO
ALTER TABLE [dbo].[RelationshipModuleProfile] CHECK CONSTRAINT [FK_RelModuleProfile_Profile]
GO
ALTER TABLE [dbo].[RelationshipProfileLevel]  WITH CHECK ADD FOREIGN KEY([LevelId])
REFERENCES [dbo].[Level] ([Id])
GO
ALTER TABLE [dbo].[RelationshipProfileLevel]  WITH CHECK ADD  CONSTRAINT [FK_RelPerfilNivel_Perfil] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profile] ([Id])
GO
ALTER TABLE [dbo].[RelationshipProfileLevel] CHECK CONSTRAINT [FK_RelPerfilNivel_Perfil]
GO
ALTER TABLE [dbo].[RelationshipProfileManagedSystem]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipManagedSystem] FOREIGN KEY([ManagedSystemId])
REFERENCES [dbo].[ManagedSystem] ([Id])
GO
ALTER TABLE [dbo].[RelationshipProfileManagedSystem] CHECK CONSTRAINT [FK_RelationshipManagedSystem]
GO
ALTER TABLE [dbo].[RelationshipProfileManagedSystem]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipProfile] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profile] ([Id])
GO
ALTER TABLE [dbo].[RelationshipProfileManagedSystem] CHECK CONSTRAINT [FK_RelationshipProfile]
GO
ALTER TABLE [dbo].[RelationshipTransactionProfile]  WITH CHECK ADD  CONSTRAINT [FK_RelTransacaoPerfil_Perfil] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profile] ([Id])
GO
ALTER TABLE [dbo].[RelationshipTransactionProfile] CHECK CONSTRAINT [FK_RelTransacaoPerfil_Perfil]
GO
ALTER TABLE [dbo].[RelationshipTransactionProfile]  WITH CHECK ADD  CONSTRAINT [FK_RelTransacaoPerfil_Transacao] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[Transaction] ([Id])
GO
ALTER TABLE [dbo].[RelationshipTransactionProfile] CHECK CONSTRAINT [FK_RelTransacaoPerfil_Transacao]
GO
ALTER TABLE [dbo].[RelationshipUserProfile]  WITH CHECK ADD  CONSTRAINT [FK_RelUsuarioPerfil_Perfil] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profile] ([Id])
GO
ALTER TABLE [dbo].[RelationshipUserProfile] CHECK CONSTRAINT [FK_RelUsuarioPerfil_Perfil]
GO
ALTER TABLE [dbo].[RelationshipUserProfile]  WITH CHECK ADD  CONSTRAINT [FK_RelUsuarioPerfil_Usuario] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[RelationshipUserProfile] CHECK CONSTRAINT [FK_RelUsuarioPerfil_Usuario]
GO
ALTER TABLE [dbo].[RelationshipUserProfileInstitution]  WITH CHECK ADD  CONSTRAINT [FK_RelUsuarioPerfilOrgao_RelUsuarioPerfil] FOREIGN KEY([RelationshipUserProfileId])
REFERENCES [dbo].[RelationshipUserProfile] ([Id])
GO
ALTER TABLE [dbo].[RelationshipUserProfileInstitution] CHECK CONSTRAINT [FK_RelUsuarioPerfilOrgao_RelUsuarioPerfil]
GO
ALTER TABLE [dbo].[Repair]  WITH CHECK ADD  CONSTRAINT [PK_RepairAsset] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])
GO
ALTER TABLE [dbo].[Repair] CHECK CONSTRAINT [PK_RepairAsset]
GO
ALTER TABLE [dbo].[Repair]  WITH CHECK ADD  CONSTRAINT [PK_RepairInitial] FOREIGN KEY([InitialId])
REFERENCES [dbo].[Initial] ([Id])
GO
ALTER TABLE [dbo].[Repair] CHECK CONSTRAINT [PK_RepairInitial]
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaContabil]  WITH CHECK ADD  CONSTRAINT [FK_SaldoContabilAtualUGEContaContabil_BudgetUnit] FOREIGN KEY([IdUO])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaContabil] CHECK CONSTRAINT [FK_SaldoContabilAtualUGEContaContabil_BudgetUnit]
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaContabil]  WITH CHECK ADD  CONSTRAINT [FK_SaldoContabilAtualUGEContaContabil_Institution] FOREIGN KEY([IdOrgao])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaContabil] CHECK CONSTRAINT [FK_SaldoContabilAtualUGEContaContabil_Institution]
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaContabil]  WITH CHECK ADD  CONSTRAINT [FK_SaldoContabilAtualUGEContaContabil_ManagerUnit] FOREIGN KEY([IdUGE])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaContabil] CHECK CONSTRAINT [FK_SaldoContabilAtualUGEContaContabil_ManagerUnit]
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaDepreciacao]  WITH CHECK ADD  CONSTRAINT [FK_SaldoContabilAtualUGEContaDepreciacao_BudgetUnit] FOREIGN KEY([IdUO])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaDepreciacao] CHECK CONSTRAINT [FK_SaldoContabilAtualUGEContaDepreciacao_BudgetUnit]
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaDepreciacao]  WITH CHECK ADD  CONSTRAINT [FK_SaldoContabilAtualUGEContaDepreciacao_DepreciationAccount] FOREIGN KEY([DepreciationAccountId])
REFERENCES [dbo].[DepreciationAccount] ([Id])
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaDepreciacao] CHECK CONSTRAINT [FK_SaldoContabilAtualUGEContaDepreciacao_DepreciationAccount]
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaDepreciacao]  WITH CHECK ADD  CONSTRAINT [FK_SaldoContabilAtualUGEContaDepreciacao_Institution] FOREIGN KEY([IdOrgao])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaDepreciacao] CHECK CONSTRAINT [FK_SaldoContabilAtualUGEContaDepreciacao_Institution]
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaDepreciacao]  WITH CHECK ADD  CONSTRAINT [FK_SaldoContabilAtualUGEContaDepreciacao_ManagerUnit] FOREIGN KEY([IdUGE])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[SaldoContabilAtualUGEContaDepreciacao] CHECK CONSTRAINT [FK_SaldoContabilAtualUGEContaDepreciacao_ManagerUnit]
GO
ALTER TABLE [dbo].[Supplier]  WITH CHECK ADD  CONSTRAINT [FK_Fornecedor_Endereco] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[Supplier] CHECK CONSTRAINT [FK_Fornecedor_Endereco]
GO
ALTER TABLE [dbo].[Support]  WITH CHECK ADD  CONSTRAINT [FK_Support_BudgetUnit] FOREIGN KEY([BudgetUnitId])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[Support] CHECK CONSTRAINT [FK_Support_BudgetUnit]
GO
ALTER TABLE [dbo].[Support]  WITH CHECK ADD  CONSTRAINT [FK_Support_Institution] FOREIGN KEY([InstitutionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[Support] CHECK CONSTRAINT [FK_Support_Institution]
GO
ALTER TABLE [dbo].[Support]  WITH CHECK ADD  CONSTRAINT [FK_Support_ManagerUnit] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[Support] CHECK CONSTRAINT [FK_Support_ManagerUnit]
GO
ALTER TABLE [dbo].[Support]  WITH CHECK ADD  CONSTRAINT [FK_Support_Module] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Module] ([Id])
GO
ALTER TABLE [dbo].[Support] CHECK CONSTRAINT [FK_Support_Module]
GO
ALTER TABLE [dbo].[Support]  WITH CHECK ADD  CONSTRAINT [FK_Support_SupportStatusProdesp] FOREIGN KEY([SupportStatusProdespId])
REFERENCES [dbo].[SupportStatusProdesp] ([Id])
GO
ALTER TABLE [dbo].[Support] CHECK CONSTRAINT [FK_Support_SupportStatusProdesp]
GO
ALTER TABLE [dbo].[Support]  WITH CHECK ADD  CONSTRAINT [FK_Support_SupportStatusUser] FOREIGN KEY([SupportStatusUserId])
REFERENCES [dbo].[SupportStatusUser] ([Id])
GO
ALTER TABLE [dbo].[Support] CHECK CONSTRAINT [FK_Support_SupportStatusUser]
GO
ALTER TABLE [dbo].[Support]  WITH CHECK ADD  CONSTRAINT [FK_Support_SupportType] FOREIGN KEY([SupportTypeId])
REFERENCES [dbo].[SupportType] ([Id])
GO
ALTER TABLE [dbo].[Support] CHECK CONSTRAINT [FK_Support_SupportType]
GO
ALTER TABLE [dbo].[Support]  WITH CHECK ADD  CONSTRAINT [FK_Support_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Support] CHECK CONSTRAINT [FK_Support_User]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD FOREIGN KEY([TypeTransactionId])
REFERENCES [dbo].[TypeTransaction] ([Id])
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transacao__Modulo] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Module] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transacao__Modulo]
GO
ALTER TABLE [dbo].[UGEDepreciaramAbrilDoisMilVinte]  WITH CHECK ADD  CONSTRAINT [FK_UGEDepreciaramAbrilDoisMilVinte_ManagerUnit] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[UGEDepreciaramAbrilDoisMilVinte] CHECK CONSTRAINT [FK_UGEDepreciaramAbrilDoisMilVinte_ManagerUnit]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_Usuario_Endereco] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_Usuario_Endereco]
GO
/****** Object:  StoredProcedure [dbo].[ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_CONTABIL_HOJE]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_CONTABIL_HOJE]
( 
	@IdOrgao int,
	@IdUO int = NULL,
	@IdUGE int = NULL,
	@IdContaContabil int
)
AS
BEGIN

Declare @ContaContabil int;
Declare @Valor decimal(18,2);

select @ContaContabil = CONVERT(INT,ContaContabilApresentacao) 
from AuxiliaryAccount
where id = @IdContaContabil;

select	   
	@Valor = ISNULL(SUM([visao_geral].ValorContabil),0)
from (
 SELECT  
		CASE 
				WHEN [Extent2].AuxiliaryAccountId = 212508 
				THEN  (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[CurrentValue],0) 'ValorMensal'			
							  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
							  WHERE 
									ManagerUnitId = [Extent2].[ManagerUnitId]
							  AND [dadosDepreciacaoMensal].[MaterialItemCode] = [Extent1].[MaterialItemCode]
							  AND [Extent1].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId] 
							  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <=
							  (select top 1 (CAST(CONVERT(CHAR(6), MovimentDate, 112) AS INT)) from AssetMovements with(nolock) 
							  where AssetId = [Extent1].[Id] AND 
								    FlagEstorno IS NULL AND
									MovementTypeId = 19))
							 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
						)
				ELSE (SELECT TOP 1 ISNULL([Extent1].[ValueAcquisition], 0.00)) 
		END AS 'ValorContabil'
		FROM [dbo].[Asset] AS [Extent1] WITH(NOLOCK)    
		INNER JOIN [dbo].[AssetMovements] AS [Extent2] WITH(NOLOCK) ON[Extent1].[Id] = [Extent2].[AssetId]    
		INNER JOIN [dbo].[MovementType] AS [Extent14] WITH(NOLOCK) ON [Extent14].[Id] = [Extent2].[MovementTypeId]
		LEFT OUTER JOIN [dbo].[AuxiliaryAccount] AS [Extent11] WITH(NOLOCK) ON [Extent11].[Id] = [Extent2].[AuxiliaryAccountId]
		
		WHERE  ([Extent1].[flagVerificado] IS NULL)        
		  AND ([Extent1].[flagDepreciaAcumulada] = 1)
		  AND ([Extent2].[flagEstorno] IS NULL)
		  AND ([Extent2].[DataEstorno] IS NULL) 
		  AND ([Extent2].[Status] = 1)  
		  AND ([Extent2].[MovementTypeId] NOT IN (47,56,57)) --Não contabilizar transferência q pedem aceite no SAM
		  AND ([Extent2].[InstitutionId] = @IdOrgao)
		  AND ([Extent2].[BudgetUnitId] = @IdUO OR @IdUO IS NULL)  
		  AND ([Extent2].[ManagerUnitId] = @IdUGE OR @IdUGE IS NULL)
		  AND ([Extent11].[ContaContabilApresentacao] = @ContaContabil)
	) [visao_geral]

declare @IdRegistro int;

select TOP 1 @IdRegistro = Id from SaldoContabilAtualUGEContaContabil
     where IdOrgao = @IdOrgao
	 and IdUO = @IdUO
	 and IdUGE = @IdUGE
	 and ContaContabil = @ContaContabil


	IF( @IdRegistro is not null)
	BEGIN
		update SaldoContabilAtualUGEContaContabil
		set ValorContabil = @Valor
		where id = @IdRegistro;
	END
	ELSE
	BEGIN 
		insert into SaldoContabilAtualUGEContaContabil
		(
		   [IdOrgao]
	      ,[IdUO]
	      ,[IdUGE]
	      ,[CodigoOrgao]
	      ,[CodigoUO]
	      ,[CodigoUGE]
	      ,[ContaContabil]
	      ,[ValorContabil]
		)
		VALUES(
			@IdOrgao,
			@IdUO,
			@IdUGE,
			(select top 1 Code from Institution where id = @IdOrgao),
			(select top 1 Code from BudgetUnit where id = @IdUO),
			(select top 1 Code from ManagerUnit where id = @IdUGE),
			@ContaContabil,
			@Valor
		);
	END

END

GO
/****** Object:  StoredProcedure [dbo].[ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ATUALIZA_SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE]
( 
	@IdOrgao int,
	@IdUO int = NULL,
	@IdUGE int = NULL,
	@IdContaContabil int
)
AS
BEGIN

Declare @IdContaDepreciacao int;
Declare @Valor decimal(18,2);

select @IdContaDepreciacao = CONVERT(INT,DepreciationAccountId)
from AuxiliaryAccount
where id = @IdContaContabil;

IF(@IdContaDepreciacao IS NOT NULL)
BEGIN
	select	   
		@Valor = ISNULL(SUM([visao_geral].ValorContabil),0)
	from (
	 SELECT 
			(SELECT MAX([Extent12].[AccumulatedDepreciation])
			  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
			  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
			    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
			    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
				AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
			) AS 'ValorContabil'		
			FROM [dbo].[Asset] AS [Extent1] WITH(NOLOCK)    
			INNER JOIN [dbo].[AssetMovements] AS [Extent2] WITH(NOLOCK) ON[Extent1].[Id] = [Extent2].[AssetId]    
			INNER JOIN [dbo].[Institution] AS [Extent5] WITH(NOLOCK) ON [Extent5].[Id] = [Extent2].[InstitutionId]    
			INNER JOIN [dbo].[BudgetUnit] AS [Extent6] WITH(NOLOCK) ON [Extent6].[Id] = [Extent2].[BudgetUnitId]    
			INNER JOIN [dbo].[ManagerUnit] AS [Extent7] WITH(NOLOCK) ON [Extent7].[Id] = [Extent2].[ManagerUnitId]
			INNER JOIN [dbo].[AuxiliaryAccount] AS [Extent11] WITH(NOLOCK) ON [Extent11].[Id] = [Extent2].[AuxiliaryAccountId]
			INNER JOIN [dbo].[DepreciationAccount] As [Extent12] WITH(NOLOCK) ON [Extent12].[Id] =  [Extent11].[DepreciationAccountId]
	
			WHERE  ([Extent1].[flagVerificado] IS NULL)        
			  AND ([Extent1].[flagDepreciaAcumulada] = 1)
			  AND ([Extent2].[flagEstorno] IS NULL)
			  AND ([Extent2].[DataEstorno] IS NULL) 
			  AND ([Extent1].[AssetStartId] IS NOT NULL)
			  AND ([Extent2].[Status] = 1)  
			  --Acervo, terceiro, animal nao a Servico não depreciam
			  AND ([Extent1].[flagTerceiro] IS NULL OR [Extent1].[flagTerceiro] = 0)
			  AND ([Extent1].[flagAcervo] IS NULL OR [Extent1].[flagAcervo] = 0)
			  AND ([Extent1].[FlagAnimalNaoServico] IS NULL OR [Extent1].[FlagAnimalNaoServico] = 0)
			  --Não contabilizar saídas que requerem aceite
			  AND ([Extent2].[MovementTypeId] NOT IN (47,56,57))
			  AND ([Extent2].[InstitutionId] = @IdOrgao)  
			  AND ([Extent2].[BudgetUnitId] = @IdUO OR @IdUO IS NULL)  
			  AND ([Extent2].[ManagerUnitId] = @IdUGE OR @IdUGE IS NULL)
			  AND ([Extent2].[AuxiliaryAccountId] != 212508)
			  AND ([Extent12].[Id] = @IdContaDepreciacao)
			  
		) [visao_geral]
	
	declare @IdRegistro int;
	
	select TOP 1 @IdRegistro = Id from SaldoContabilAtualUGEContaDepreciacao
	     where IdOrgao = @IdOrgao
		 and IdUO = @IdUO
		 and IdUGE = @IdUGE
		 and DepreciationAccountId = @IdContaDepreciacao
	
	
		IF( @IdRegistro is not null)
		BEGIN
			update SaldoContabilAtualUGEContaDepreciacao
			set DepreciacaoAcumulada = @Valor
			where id = @IdRegistro;
		END
		ELSE
		BEGIN 
			insert into SaldoContabilAtualUGEContaDepreciacao
			(
			   [IdOrgao]
		      ,[IdUO]
		      ,[IdUGE]
		      ,[CodigoOrgao]
		      ,[CodigoUO]
		      ,[CodigoUGE]
		      ,[DepreciationAccountId]
			  ,[ContaDepreciacao]
		      ,[DepreciacaoAcumulada]
			)
			VALUES(
				@IdOrgao,
				@IdUO,
				@IdUGE,
				(select top 1 Code from Institution where id = @IdOrgao),
				(select top 1 Code from BudgetUnit where id = @IdUO),
				(select top 1 Code from ManagerUnit where id = @IdUGE),
				@IdContaDepreciacao,
				(select top 1 Code from DepreciationAccount where id = @IdContaDepreciacao),
				@Valor
			);
		END
END

END

GO
/****** Object:  StoredProcedure [dbo].[CONSOLIDACAO_IMPORTACAO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CONSOLIDACAO_IMPORTACAO]
(    
 @CodOrgao int    
)    
AS    
BEGIN    
    
 DECLARE @OrgaoId int = (select id from Institution where code = @CodOrgao)
  
 --insere Novas descrições resumidas, conforme planilha de carga 
 INSERT INTO ShortDescriptionItem
 select DISTINCT a.MaterialItemDescription
 FROM Asset a
 INNER JOIN AssetMovements as b with(nolock)
 on b.AssetId = a.Id
 INNER JOIN Item_Siafisico as c with(nolock) 
 ON c.Cod_Item_Mat = a.MaterialItemCode
 INNER JOIN MaterialGroup as d with(nolock) 
 ON d.Code = c.Cod_Grupo
 LEFT JOIN ShortDescriptionItem as e with(nolock) 
 ON e.[Description] = a.MaterialItemDescription
 WHERE e.[Description] IS NULL    
 AND b.InstitutionId = @OrgaoId
 and a.flagVerificado is null and flagDepreciaAcumulada is null
    
    
 -- CRIA TABELA TEMPORARIA PARA ARMAZENAR OS ITENS QUE FORAM ALTERADOS    
 CREATE TABLE #assetId    
 (    
   Id  int    
 )    
    
    
 -- INSERE OS ITENS ENCONTRADOS DA TABELA DO SIAFEM NA TABELA TEMPORARIA    
 INSERT INTO #assetId    
 SELECT DISTINCT a.Id
 FROM Asset a
 INNER JOIN AssetMovements b
 on b.AssetId = a.Id
 INNER JOIN Item_Siafisico as c with(nolock)    
 ON c.Cod_Item_Mat = a.MaterialItemCode
 INNER JOIN MaterialGroup as d with(nolock) 
 ON d.Code = c.Cod_Grupo
 INNER JOIN ShortDescriptionItem as e with(nolock) 
 ON e.[Description] = a.MaterialItemDescription
 where b.InstitutionId = @OrgaoId
 and a.flagVerificado is null 
 and flagDepreciaAcumulada is null

    
 -- ATUALIZA OS ITENS ENCONTRADOS DA TABELA DO SIAFEM NA TABELA ASSET DO PATRIMONIO    
 UPDATE a
 SET     
 a.MaterialItemDescription = c.Nome_Item_Mat,    
 a.MaterialGroupCode = d.Code,    
 a.ShortDescriptionItemId = e.Id,    
 a.LifeCycle = d.LifeCycle,     
 a.RateDepreciationMonthly = d.RateDepreciationMonthly,    
 a.ResidualValue = d.ResidualValue    
 FROM Asset as a with(nolock)
 INNER JOIN #assetId as b with(nolock)
	on a.Id = b.Id
 INNER JOIN Item_Siafisico as c with(nolock)
	ON c.Cod_Item_Mat = a.MaterialItemCode    
 INNER JOIN MaterialGroup as d with(nolock)
	ON d.Code = c.Cod_Grupo    
 INNER JOIN ShortDescriptionItem as e with(nolock)   
	ON e.[Description] = a.MaterialItemDescription
 where (a.flagAcervo is null or a.flagAcervo = 0)    
 and (a.flagTerceiro is null or a.flagTerceiro = 0)
    
 -- ATUALIZA O VALOR DA FLAG PARA 1 PARA OS ITENS QUE NAO FORAM ENCONTRADOS E ESTÃO PENDENTES
 UPDATE a        
 SET flagVerificado = 1       
 FROM Asset a with(nolock)     
 inner join AssetMovements m with(nolock)    
 on a.Id = m.AssetId
 WHERE flagVerificado is null 
 AND flagDepreciaAcumulada is null
 AND a.id NOT IN (SELECT Id FROM #assetId)
 AND (flagAcervo is NULL OR flagAcervo = 0)
 AND (flagTerceiro is NULL OR flagTerceiro = 0)
 AND m.InstitutionId = @OrgaoId
    
 DROP TABLE #assetId    
    
    
  -- PENDENTE CASO SIGLA SEJA GENERICA    
 UPDATE a  
 SET a.flagVerificado = 2    
 from Asset a
 INNER JOIN AssetMovements am
 on a.Id = am.AssetId
 INNER JOIN Initial i
 on a.InitialId = i.Id
 WHERE am.InstitutionId = @OrgaoId
 and a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null    
 and (i.[Name] = 'SIGLA_GENERICA' OR i.[Description] = 'SIGLA_GENERICA')
    
 -- PENDENTE CASO CHAPA E SIGLA SEJAM REPETIDAS NO MESMO ORGAO        
UPDATE a
SET a.flagVerificado = 3        
FROM Asset as a with(nolock)
INNER JOIN Asset as b  with(nolock)
ON a.NumberIdentification = b.NumberIdentification
AND a.DiferenciacaoChapa = b.DiferenciacaoChapa
AND a.InitialId = b.InitialId
AND a.Id != b.Id
INNER JOIN AssetMovements am
on a.Id = am.AssetId
WHERE a.flagVerificado IS NULL 
AND a.flagDepreciaAcumulada IS NULL 
AND am.InstitutionId = @OrgaoId    
    
 -- PENDENTE CASO CHAPA SEJA GENERICA    
 UPDATE a
 SET a.flagVerificado = 4    
 FROM Asset a
 INNER JOIN AssetMovements am
 on a.Id = am.AssetId
 WHERE am.InstitutionId = @OrgaoId
 and a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null    
 and a.NumberIdentification = 'CHAPA_GENERICA' 
    
-- PENDENTE CASO A UO SEJA GENERICA    
 UPDATE a     
 SET a.flagVerificado = 5    
 FROM Asset as a with(nolock)     
 INNER JOIN AssetMovements as am with(nolock)     
 ON a.Id = am.AssetId    
 INNER JOIN BudgetUnit b
 on b.Id = am.BudgetUnitId
 WHERE am.institutionId = @OrgaoId    
 and a.flagVerificado is null AND a.flagDepreciaAcumulada is null    
 and b.Code = '99999' 
    
-- PENDENTE CASO A UGE SEJA GENERICA    
 UPDATE a     
 SET a.flagVerificado = 6    
 FROM Asset as a with(nolock)     
 INNER JOIN AssetMovements as am with(nolock)     
 ON a.Id = am.AssetId    
 INNER JOIN ManagerUnit as uge
 on uge.Id = am.ManagerUnitId
 WHERE am.institutionId = @OrgaoId    
 and a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null    
 and uge.Code = '999999'    
    
-- PENDENTE CASO A UA SEJA GENERICA    
 UPDATE a     
 SET a.flagVerificado = 7    
 FROM Asset as a with(nolock)     
 INNER JOIN AssetMovements as am with(nolock)     
 ON a.Id = am.AssetId  
 INNER JOIN  MovementType as mov with(nolock)  
 ON am.MovementTypeId = mov.Id
 LEFT JOIN AdministrativeUnit ua
 on ua.Id = am.AdministrativeUnitId
 WHERE am.institutionId = @OrgaoId    
 and a.flagVerificado is null AND a.flagDepreciaAcumulada is null    
 and mov.GroupMovimentId = 1 
 AND (am.AdministrativeUnitId is null OR ua.Code = '99999999')
    
-- PENDENTE CASO A DIVISAO SEJA GENERICA    
 UPDATE a     
 SET a.flagVerificado = 8    
 FROM Asset as a with(nolock)     
 INNER JOIN AssetMovements as am with(nolock)     
 ON a.Id = am.AssetId    
 INNER JOIN Section divisao
 on divisao.Id = am.SectionId
 WHERE am.institutionId = @OrgaoId    
 and a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null    
 and divisao.Code = 999       
        
-- PENDENTE CASO O RESPONSAVEL SEJA GENERICO    
 UPDATE a     
 SET a.flagVerificado = 9
 FROM Asset as a with(nolock)     
 INNER JOIN AssetMovements as am with(nolock)     
 ON a.Id = am.AssetId    
 INNER JOIN MovementType as mov with(nolock)  
 ON am.MovementTypeId = mov.Id
 LEFT JOIN Responsible r
 on am.ResponsibleId = r.Id
 WHERE am.institutionId = @OrgaoId    
 and a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null
 and mov.GroupMovimentId = 1 
 and (am.ResponsibleId IS NULL OR r.[Name] like 'RESPONSAVEL_%')

 -- PENDENTE CASO O VALOR AQUISICAO SEJA GENERICO    
 UPDATE a    
 SET a.flagVerificado = 10    
 FROM Asset a
 INNER JOIN AssetMovements am
 on a.Id = am.AssetId
 WHERE a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null    
 and (a.ValueAcquisition < 1.00 --Nenhum BP pode incorporar com valor de aquisição menor que R$1,00
 OR (
	(a.flagDecreto is null OR a.flagDecreto = 0) 
	AND a.ValueAcquisition < 10.00) --BPs que não são decretos não podem ser incorporados com valor de aquisiçaõ menor que R$10,00
 )
 and am.InstitutionId = @OrgaoId    
    
-- PENDENTE CASO A DATA AQUISICAO SEJA GENERICA    
 UPDATE a    
 SET a.flagVerificado = 11    
 FROM Asset a
 INNER JOIN AssetMovements am
 on a.Id = am.AssetId
 WHERE am.InstitutionId = @OrgaoId 
 and a.flagVerificado is null AND flagDepreciaAcumulada is null
 and (a.AcquisitionDate > a.MovimentDate OR a.AcquisitionDate < '1900-01-01')
    
 -- PENDENTE CASO A DATA AQUISICAO SEJA MAIOR QUE A DATA ATUAL    
 UPDATE a    
 SET a.flagVerificado = 12
 FROM Asset a
 INNER JOIN AssetMovements am
 on a.Id = am.AssetId
 WHERE am.InstitutionId = @OrgaoId 
 and a.flagVerificado is null AND flagDepreciaAcumulada is null    
 and AcquisitionDate > GETDATE()
  
 -- PENDENTE CASO O ESTADO DE CONSERVAÇÃO SEJA INVÁLIDO   
 UPDATE a     
 SET a.flagVerificado = 13  
 FROM Asset as a with(nolock)     
 INNER JOIN AssetMovements as am with(nolock)     
 ON a.Id = am.AssetId
 LEFT JOIN StateConservation estado
 on am.StateConservationId = estado.Id
 WHERE am.institutionId = @OrgaoId    
 and a.flagVerificado is null 
 AND a.flagDepreciaAcumulada is null    
 and (am.StateConservationId is null OR am.StateConservationId = 0 OR estado.[Description] = 'ESTADO_GENERICO')
    
--Pendencia por depreciacao acumulada
update Asset 
SET       
	ValueUpdate = ValueAcquisition,             
	LifeCycle = 0,      
	RateDepreciationMonthly = 0,      
	ResidualValue = 0,
	ResidualValueCalc = 0,      
	monthUsed = 0,      
	DepreciationByMonth = 0,      
	DepreciationAccumulated = 0,           
	ValorDesdobramento = 0
FROM Asset a
INNER JOIN AssetMovements am
on a.Id = am.AssetId
where a.flagVerificado IS NULL 
and a.flagDepreciaAcumulada IS NULL 
and am.InstitutionId = @OrgaoId
and (a.flagAcervo = 1 OR a.flagTerceiro = 1)
  
update Asset 
SET flagDepreciaAcumulada = 1
FROM Asset a
INNER JOIN AssetMovements am
on a.Id = am.AssetId
where a.flagVerificado IS NULL 
and a.flagDepreciaAcumulada IS NULL 
and am.InstitutionId = @OrgaoId
    
EXEC SAM_CALCULA_DEPRECIACAO_CARGA_DE_DADOS @IdOrgao = @OrgaoId

END

GO
/****** Object:  StoredProcedure [dbo].[CONSTAR_ALTERACOES_CONTABEIS_POR_UGE_CONTA_CONTABIL_NO_FECHAMENTO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CONSTAR_ALTERACOES_CONTABEIS_POR_UGE_CONTA_CONTABIL_NO_FECHAMENTO]
(
	@IdUGE int
) AS
BEGIN
	Declare @IdOrgao int, @IdUO int;

	Declare @temp as table(
		[Id] int identity(1,1),
		[IdOrgao] [int] NOT NULL,
		[IdUO] [int] NOT NULL,
		[IdUGE] [int] NOT NULL,
		[IdContaContabil] [int] NOT NULL
	);

	Declare @cont int;

	select @IdOrgao = b.InstitutionId,
	@IdUO = b.Id
	from BudgetUnit b with(nolock)
	inner join ManagerUnit m with(nolock) on m.BudgetUnitId = b.Id
	where m.Id = @IdUGE;

	insert into @temp
	(
		IdOrgao,
		IdUO,
		IdUGE,
		IdContaContabil
	)
	select distinct 
		[Extent2].[InstitutionId],
		[Extent2].[BudgetUnitId],
		[Extent2].[ManagerUnitId],
		[Extent2].[AuxiliaryAccountId]
	FROM [dbo].[Asset] AS [Extent1] WITH(NOLOCK)    
	INNER JOIN [dbo].[AssetMovements] AS [Extent2] WITH(NOLOCK) ON[Extent1].[Id] = [Extent2].[AssetId]
	INNER JOIN [dbo].[MovementType] AS [Extent14] WITH(NOLOCK) ON [Extent14].[Id] = [Extent2].[MovementTypeId]
	INNER JOIN [dbo].[AuxiliaryAccount] AS [Extent11] WITH(NOLOCK) ON [Extent11].[Id] = [Extent2].[AuxiliaryAccountId]
	WHERE  ([Extent1].[flagVerificado] IS NULL)    
	   AND ([Extent1].[flagDepreciaAcumulada] = 1)
	   AND ([Extent2].[flagEstorno] IS NULL)
	   AND ([Extent2].[DataEstorno] IS NULL) 
	   AND ([Extent2].[Status] = 1)  
	   AND ([Extent2].[MovementTypeId] NOT IN (47,56,57)) --Não contabilizar transferência q pedem aceite no SAM
	   AND ([Extent2].[InstitutionId] = @IdOrgao)
	   AND ([Extent2].[BudgetUnitId] = @IdUO)
	   AND ([Extent2].[ManagerUnitId] = @IdUGE);
	   
	update h
	set h.HouveAlteracao = 1
	from HouveAlteracaoContabil h
	inner join @temp temp
	on  temp.IdContaContabil = h.IdContaContabil
	and temp.IdOrgao = h.IdOrgao
	and temp.IdUO = h.IdUO
	and temp.IdUGE = h.IdUGE

	delete temp
	from HouveAlteracaoContabil h
	inner join @temp temp
	on  temp.IdContaContabil = h.IdContaContabil
	and temp.IdOrgao = h.IdOrgao
	and temp.IdUO = h.IdUO
	and temp.IdUGE = h.IdUGE

	select @cont = count(Id) from @temp;
	
	IF(@cont > 0)
	BEGIN
		Declare @IdAtual int, @IdContaContabil int;

		while(@cont > 0)
		BEGIN
			select top 1 
			@IdAtual = Id,
		    @IdContaContabil = IdContaContabil,
			@IdOrgao = IdOrgao,
			@IdUO = IdUO,
			@IdUGE = IdUGE
			from @temp;

			insert into HouveAlteracaoContabil
			(IdOrgao, IdUO, IdUGE, IdContaContabil, HouveAlteracao)
			values
			(@IdOrgao, @IdUO, @IdUGE, @IdContaContabil, 1);

			delete from @temp where id = @IdAtual;
			SET @cont = @cont - 1;
		END
	END

END

GO
/****** Object:  StoredProcedure [dbo].[DEPRECIACAO_ACUMULADA_CARGA]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

     
CREATE PROCEDURE [dbo].[DEPRECIACAO_ACUMULADA_CARGA]       
(
	@OrgaoId int
)            
AS              
BEGIN              
                   
BEGIN TRY              
              
 Declare @assetIdCurrentLoop int;              
 Declare @dataAquisicao date;     
 Declare @contMeses int = 0;    
 DECLARE @contMesUso int = 0;                    
 Declare @managerUnitId int = 0;    
 Declare @dateFechamentoReferencia datetime;     
    
 DECLARE @vidaUtil int = 0;    
 DECLARE @mesesUtilizados int = 0;    
 DECLARE @valorAquisicao decimal(18,2) = 0;    
 DECLARE @valorAtual decimal(18,2) = 0;    
 Declare @valorResidualCalc decimal(18,2);        
 Declare @depreciacaoMensal decimal(18,10);    
 DECLARE @depreciacaoAcumulada decimal(18,2) = 0;       
 Declare @desdobramento decimal(18,2) = 0; 
 Declare @desdobramentoRestante decimal(18,2) = 0;    
    
 DECLARE @ResidualValue decimal(18,2) = 0;    
 DECLARE @AceleratedDepreciation bit = 0;    
 DECLARE @ManagmentUnit_YearMonthReference varchar(6) = '';    
         
 DECLARE @EhAcervo bit = 0;       
 DECLARE @EhTerceiro bit = 0;       
 DECLARE @FlagDepreciationCompleted bit = 0;       
             
 --Percorre todos os patrimônios ----------------------------------------------------------------------------------------------------------------------------------------              
               
 DECLARE asset_cursor CURSOR FOR      
 SELECT TOP 200 Id FROM Asset          
 where                 
    flagVerificado IS NULL and               
    flagDepreciaAcumulada IS NULL and
	id IN (select AssetId from AssetMovements where InstitutionId = @OrgaoId)
           
              
 OPEN asset_cursor;              
 FETCH NEXT FROM asset_cursor into @assetIdCurrentLoop;              
              
              
 WHILE @@FETCH_STATUS = 0              
  BEGIN              
            
 SET @desdobramento = 0    
 SET @FlagDepreciationCompleted = 0    
 SET @EhAcervo = (select flagAcervo from Asset with(nolock) where Id = @assetIdCurrentLoop)    
 SET @EhTerceiro = (select flagTerceiro from Asset with(nolock) where Id = @assetIdCurrentLoop)    
     
     
   IF(@EhAcervo = 1 OR @EhTerceiro = 1)    
    BEGIN    
    
     --Atualiza os valores dos Patrimônios              
   update           
    Asset        
   SET     
    ValueUpdate = ValueAcquisition,     
    
    LifeCycle = 0,    
    RateDepreciationMonthly = 0,    
    ResidualValue = 0,    
           
    flagDepreciaAcumulada = 1, --Informa que o Bem não possui pendencia de depreciação acumulada, pois é um Acervo (Não depende de depreciação)    
    flagVerificado = NULL,    
    ResidualValueCalc = 0,    
    monthUsed = 0,    
    DepreciationByMonth = 0,    
    DepreciationAccumulated = 0,         
    ValorDesdobramento = 0            
   where           
    Id = @assetIdCurrentLoop          
    
    END    
   ELSE    
    BEGIN    
             
     --Armazena os valores utilizados no cálculo -------------------------------              
   select           
     @vidaUtil = a.LifeCycle,      
     @valorAtual = a.ValueAcquisition,    
     @valorAquisicao = a.ValueAcquisition,    
     @valorResidualCalc = a.ValueAcquisition * (a.ResidualValue/100),    
     @depreciacaoMensal =  (CASE a.LifeCycle WHEN 0 THEN 0 
                        ELSE ROUND((a.ValueAcquisition - @valorResidualCalc) / a.LifeCycle, 2, 1) END),
   
     @depreciacaoAcumulada = a.ValueAcquisition - a.ValueUpdate,    
     @managerUnitId = ManagerUnitId,        
     @dataAquisicao = a.AcquisitionDate,    
       
     @AceleratedDepreciation = a.AceleratedDepreciation,    
     @ResidualValue = a.ResidualValue,    
    
     @managerUnitId = a.ManagerUnitId,    
     @ManagmentUnit_YearMonthReference = m.ManagmentUnit_YearMonthReference     
     FROM           
     Asset as a    
     INNER JOIN     
     ManagerUnit as m ON a.ManagerUnitId = m.Id              
     where           
    a.Id = @assetIdCurrentLoop       
      
       
     --Armazena o mês e ano de referência no formato de date            
     select     
    @dateFechamentoReferencia = CONVERT(DATE, ManagmentUnit_YearMonthReference  + '01' ,126)             
     from     
    ManagerUnit     
   where     
    Id = @managerUnitId             
    
    
     --Calcula a quantidade de meses que o bem foi adquirido              
     SET @mesesUtilizados = DATEDIFF(month, @dataAquisicao, @dateFechamentoReferencia)                                    
       
              
     --Caso a quantidade de meses do bem patrimonial seja maior que 0, efetua o cálculo              
     IF(@mesesUtilizados > 0)              
     BEGIN                
            
     --Reseta o contador de meses       
     SET @contMeses = 1;             
                      
     --Percorre a cada mês da vida útil do bem    
     WHILE (@contMeses <= @vidaUtil and @contMeses <= @mesesUtilizados)              
     BEGIN      
       
    --Seta a quantidade de meses que o bem foi utilizado    
    SET @contMesUso = @contMeses                            
            
    --Verifica se a quantidade de depreciações é igual ao ciclo de vida do Bem, para que seja calculado o desdobramento de valor     
    IF @vidaUtil = @contMesUso    
    BEGIN    
     --Calculo do desdobramento    
     SET @desdobramento =  @depreciacaoMensal - (@valorAtual - @valorResidualCalc)    
	  SET @desdobramentoRestante =  (@valorAtual - @valorResidualCalc) - @depreciacaoMensal
    
     --Flag que indica se a depreciação já atingio seu valor mínimo    
     SET @FlagDepreciationCompleted = 1    
    END    
    
    --Verifica se o valor do bem se encontra com o mesmo valor residual, para parar a depreciação            
    IF(@valorAtual <= @valorResidualCalc)              
       BEGIN                     
     BREAK              
       END             
            
    --Subtrai o valor de depreciação do valor do patrimônio              
    SET @valorAtual = @valorAtual - ROUND(@depreciacaoMensal, 10)        
             
    --Incrementa 1 mês para comparação              
    SET @contMeses = @contMeses + 1     
              
     END              
              
   END      
    
   --Seta o valor da depreciação acumulada do bem    
   SET @depreciacaoAcumulada = @valorAquisicao - (@valorAtual + @desdobramento)                       
     
    
    --Atualiza os valores dos Patrimônios              
    update           
    Asset           
   SET           
    ValueUpdate = ROUND(@valorAtual + @desdobramento, 2),     
    ResidualValueCalc = @valorResidualCalc,    
    monthUsed = @contMesUso,    
    DepreciationByMonth = @depreciacaoMensal,    
    DepreciationAccumulated = @depreciacaoAcumulada,    
    flagDepreciaAcumulada = 1,          
    flagVerificado = NULL,    
    ValorDesdobramento = ROUND(@desdobramentoRestante,2),    
    flagDepreciationCompleted = @FlagDepreciationCompleted    
   where           
    Id = @assetIdCurrentLoop            
         
       END    
                        
   FETCH NEXT FROM asset_cursor into @assetIdCurrentLoop              
  END;             
              
 CLOSE asset_cursor              
 DEALLOCATE asset_cursor              
              
 -----------------------------------------------------------------------------------------------------------------------------------------------------------------------              
                      
END TRY                    
                    
BEGIN CATCH                         
 SELECT             
   AssetId = @assetIdCurrentLoop        
        ,ERROR_PROCEDURE() AS ErrorProcedure                      
        ,ERROR_LINE() AS ErrorLine                      
        ,ERROR_MESSAGE() AS ErrorMessage;                               
END CATCH                      
END


GO
/****** Object:  StoredProcedure [dbo].[DEPRECIACAO_ACUMULADA_UNITARIO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DEPRECIACAO_ACUMULADA_UNITARIO] -- 3768633,'13928683861',0,'74556', 40
(        
 @assetId int,  
 @login varchar(12),  
 @valueUpdate decimal(18,2) = 0,
 @flagDepreciaPelaDataIncorporacao bit = 1,
 @movementTypeId int = null
)           
AS            
BEGIN            
          
BEGIN TRANSACTION             
BEGIN TRY            
            
 Declare @assetIdCurrentLoop int;            
 Declare @dataAquisicao date;                    
 Declare @contMeses int = 0;    
 DECLARE @contMesUso int = 0;          
 Declare @managerUnitId int = null;          
 Declare @dateFechamentoReferencia datetime;          
            
   
 DECLARE @vidaUtil int = 0;  
 DECLARE @mesesUtilizados int = 0;  
 DECLARE @valorAquisicao decimal(18,2) = 0;  
 DECLARE @valorAtual decimal(18,2) = 0;  
 DECLARE @valorResidualCalc decimal(18,2) = 0;  
 DECLARE @depreciacaoMensal decimal(18,10) = 0;   
 DECLARE @depreciacaoAcumulada decimal(18,2) = 0;    
 Declare @desdobramento decimal(18,10) = 0;
 Declare @desdobramentoRestante decimal(18,10) = 0;    
  
 DECLARE @ResidualValue decimal(18,2) = 0;  
 DECLARE @AceleratedDepreciation bit = 0;  
 DECLARE @ManagmentUnit_YearMonthReference varchar(6) = '';  
  
 DECLARE @FlagDepreciationCompleted bit = 0;     
  
 DECLARE @EhAcervo bit = ((select flagAcervo from Asset with(nolock) where Id = @assetId));  
 DECLARE @EhTerceiro bit = ((select flagTerceiro from Asset  with(nolock) where Id = @assetId));  
 
      
   IF(@EhAcervo = 1 OR @EhTerceiro = 1)  
    BEGIN  
  
     --Atualiza os valores dos Patrimônios            
   update         
    Asset      
   SET   
    ValueUpdate = @valueUpdate,   
  
    LifeCycle = 0,  
    RateDepreciationMonthly = 0,  
    ResidualValue = 0,  
         
    flagDepreciaAcumulada = 1, --Informa que o Bem não possui pendencia de depreciação acumulada, pois é um Acervo (Não depende de depreciação)  
    flagVerificado = NULL,  
    ResidualValueCalc = 0,  
    monthUsed = 0,  
    DepreciationByMonth = 0,  
    DepreciationAccumulated = 0,       
    ValorDesdobramento = 0          
   where         
    Id = @assetId and        
    flagDepreciaAcumulada IS NULL           
  
    END  
   ELSE  
    BEGIN  
                 
     --Armazena os valores utilizados no cálculo -------------------------------            
     select         
     @vidaUtil = LifeCycle,    
     @valorAtual = ValueAcquisition,  
     @valorAquisicao = a.ValueAcquisition,  
     @valorResidualCalc = ValueAcquisition * (ResidualValue/100),  
	 @depreciacaoMensal = (CASE a.LifeCycle WHEN 0 THEN 0 
                        ELSE ROUND((a.ValueAcquisition - @valorResidualCalc) / a.LifeCycle, 2, 1) END),
     @depreciacaoAcumulada = ValueAcquisition - ValueUpdate,  
     @managerUnitId = ManagerUnitId,    
     @dataAquisicao = (case when @flagDepreciaPelaDataIncorporacao = 1 then AcquisitionDate else MovimentDate end),  
     
     @AceleratedDepreciation = AceleratedDepreciation,  
     @ResidualValue = ResidualValue,  
  
     @managerUnitId = a.ManagerUnitId,  
     @ManagmentUnit_YearMonthReference = m.ManagmentUnit_YearMonthReference   
     FROM         
     Asset as a  
     INNER JOIN   
     ManagerUnit as m ON a.ManagerUnitId = m.Id                    
     where         
    a.Id = @assetId    AND         
    a.flagDepreciaAcumulada IS NULL           
  
  
     --Armazena o mês e ano de referência no formato de date          
   select         
    @dateFechamentoReferencia = CONVERT(DATE,  ManagmentUnit_YearMonthReference  + '01' ,126)           
   from         
    ManagerUnit         
   where         
    Id = @managerUnitId          
          
     --Calcula a quantidade de meses que o bem foi adquirido            
     SET @mesesUtilizados = DATEDIFF(month, @dataAquisicao, @dateFechamentoReferencia)            
                 
      
     --Caso a quantidade de meses do bem patrimonial seja maior que 0, efetua o cálculo            
     IF(@mesesUtilizados > 0)            
   BEGIN              
           
     --Reseta o contador de meses             
     SET @contMeses = 1;     
             
     --Percorre a cada mês da vida útil do bem        
     WHILE (@contMeses <= @vidaUtil and @contMeses <= @mesesUtilizados)        
     BEGIN  
  
    --Seta a quantidade de meses que o bem foi utilizado  
    SET @contMesUso = @contMeses  
  
    --Verifica se a quantidade de depreciações é igual ao ciclo de vida do Bem, para que seja calculado o desdobramento de valor   
    IF @vidaUtil = @contMesUso  
    BEGIN  
     --Calculo do desdobramento  
     SET @desdobramento =  @depreciacaoMensal - (@valorAtual - @valorResidualCalc)  
	 SET @desdobramentoRestante =  (@valorAtual - @valorResidualCalc) - @depreciacaoMensal  
  
     --Flag que indica se a depreciação já atingio seu valor mínimo  
     SET @FlagDepreciationCompleted = 1  
    END  
  
    --Verifica se o valor do bem se encontra com o mesmo valor residual, para parar a depreciação          
    IF(@valorAtual <= @valorResidualCalc)            
    BEGIN                   
     BREAK            
    END           
          
    --Subtrai o valor de depreciação do valor do patrimônio            
    SET @valorAtual = @valorAtual - ROUND(@depreciacaoMensal, 10)      
       
    --Incrementa 1 mês para comparação            
    SET @contMeses = @contMeses + 1      
                              
     END            
            
   END                          
  
   --Seta o valor da depreciação acumulada do bem  
   SET @depreciacaoAcumulada = @valorAquisicao - (@valorAtual + @desdobramento)        


    IF(@movementTypeId = 40 OR @movementTypeId = 31 OR @movementTypeId = 41)  
    BEGIN  
   
	 SET @depreciacaoAcumulada = (select top 1 DepreciationAccumulated from Asset a
								inner join AssetMovements b on b.AssetId = a.Id
								where AssetTransferenciaId = @assetId)

   END
                
     
   --Atualiza os valores dos Patrimônios            
   update         
    Asset         
   SET         
    ValueUpdate = ROUND(@valorAtual + @desdobramento, 2),   
    ResidualValueCalc = @valorResidualCalc,  
    monthUsed = @contMesUso,  
    DepreciationByMonth = @depreciacaoMensal,  
    DepreciationAccumulated = @depreciacaoAcumulada,  
    flagDepreciaAcumulada = 1,        
    flagVerificado = NULL,  
    ValorDesdobramento = ROUND(@desdobramentoRestante,2),  
    flagDepreciationCompleted = @FlagDepreciationCompleted        
   where         
    Id = @assetId    
    
    
	IF(@movementTypeId = 40 OR @movementTypeId = 31 OR @movementTypeId = 41)  
    BEGIN  
   
	declare @monthUsed int;
	declare @flagDecreto bit;

	 select 
	 @depreciacaoAcumulada = DepreciationAccumulated,
	 @ValueUpdate =  ValueUpdate,
	 @monthUsed = monthUsed,
	 @EhAcervo = flagAcervo,
	 @EhTerceiro = flagTerceiro,
	 @flagDecreto = flagDecreto,
	 @vidaUtil =LifeCycle,
	 @valorResidualCalc = ResidualValueCalc,
	 @depreciacaoMensal = DepreciationByMonth,
	 @desdobramento = ValorDesdobramento
		from Asset a
		inner join AssetMovements b on b.AssetId = a.Id
		where AssetTransferenciaId = @assetId


	update         
    Asset         
    SET	
    DepreciationAccumulated = @depreciacaoAcumulada,
	monthUsed = @monthUsed,
	ValueUpdate = @ValueUpdate,
	flagAcervo = @EhAcervo,
	flagDecreto = @flagDecreto,
	flagTerceiro = @EhTerceiro,
	LifeCycle = @vidaUtil,
	ResidualValueCalc= @valorResidualCalc,
	DepreciationByMonth = @depreciacaoMensal,
	ValorDesdobramento = @desdobramento
    where         
    Id = @assetId 

   END
           

   ----Inclui um registro na tabela de histórico de depreciação  
   --insert into AssetHistoricDepreciation (  
   --          AssetId,  
   --          LifeCycle,  
   --          ResidualValue,  
   --          AceleratedDepreciation,  
   --          ResidualValueCalc,  
   --          MonthUsed,  
   --          DepreciationByMonth,  
   --          DepreciationAccumulated,  
   --          Observation,  
   --          ValueDevalue,  
   --          Login,  
   --          InclusionDate,  
   --          ManagerUnitId,  
   --          ManagmentUnit_YearMonthReference  
   --           )  
   --values           
   --          (  
   --          @assetId,  
   --          @vidaUtil,  
   --          @ResidualValue,  
   --          @AceleratedDepreciation,  
   --          @valorResidualCalc,  
   --          @contMesUso,  
   --          @depreciacaoMensal,  
   --          @depreciacaoAcumulada,  
   --          null,  
   --          null,  
   --          @login,  
   --          getdate(),  
   --          @managerUnitId,  
   --          @ManagmentUnit_YearMonthReference  
   --           )  
  
    END  

	

            
 -----------------------------------------------------------------------------------------------------------------------------------------------------------------------            
           
COMMIT TRANSACTION            
END TRY            
            
BEGIN CATCH                 
 SELECT          
         ERROR_PROCEDURE() AS ErrorProcedure              
        ,ERROR_LINE() AS ErrorLine              
        ,ERROR_MESSAGE() AS ErrorMessage;             
 ROLLBACK TRANSACTION      
   
UPDATE         
 Asset         
SET           
 flagVerificado = 20 -- ERRO PROCEDURE        
WHERE         
 Id = @assetId         
  
END CATCH              
END


GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorsXml]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ELMAH_GetErrorsXml]
(
    @Application NVARCHAR(60),
    @PageIndex INT = 0,
    @PageSize INT = 15,
    @TotalCount INT OUTPUT
)
AS 

    SET NOCOUNT ON

    DECLARE @FirstTimeUTC DATETIME
    DECLARE @FirstSequence INT
    DECLARE @StartRow INT
    DECLARE @StartRowIndex INT

    SELECT 
        @TotalCount = COUNT(1) 
    FROM 
        [ELMAH_Error]
    WHERE 
        [Application] = @Application

    -- Get the ID of the first error for the requested page

    SET @StartRowIndex = @PageIndex * @PageSize + 1

    IF @StartRowIndex <= @TotalCount
    BEGIN

        SET ROWCOUNT @StartRowIndex

        SELECT  
            @FirstTimeUTC = [TimeUtc],
            @FirstSequence = [Sequence]
        FROM 
            [ELMAH_Error]
        WHERE   
            [Application] = @Application
        ORDER BY 
            [TimeUtc] DESC, 
            [Sequence] DESC

    END
    ELSE
    BEGIN

        SET @PageSize = 0

    END

    -- Now set the row count to the requested page size and get
    -- all records below it for the pertaining application.

    SET ROWCOUNT @PageSize

    SELECT 
        errorId     = [ErrorId], 
        application = [Application],
        host        = [Host], 
        type        = [Type],
        source      = [Source],
        message     = [Message],
        [user]      = [User],
        statusCode  = [StatusCode], 
        time        = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'
    FROM 
        [ELMAH_Error] error
    WHERE
        [Application] = @Application
    AND
        [TimeUtc] <= @FirstTimeUTC
    AND 
        [Sequence] <= @FirstSequence
    ORDER BY
        [TimeUtc] DESC, 
        [Sequence] DESC
    FOR
        XML AUTO






GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorXml]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ELMAH_GetErrorXml]
(
    @Application NVARCHAR(60),
    @ErrorId UNIQUEIDENTIFIER
)
AS

    SET NOCOUNT ON

    SELECT 
        [AllXml]
    FROM 
        [ELMAH_Error]
    WHERE
        [ErrorId] = @ErrorId
    AND
        [Application] = @Application






GO
/****** Object:  StoredProcedure [dbo].[ELMAH_LogError]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ELMAH_LogError]
(
    @ErrorId UNIQUEIDENTIFIER,
    @Application NVARCHAR(60),
    @Host NVARCHAR(30),
    @Type NVARCHAR(100),
    @Source NVARCHAR(60),
    @Message NVARCHAR(500),
    @User NVARCHAR(50),
    @AllXml NTEXT,
    @StatusCode INT,
    @TimeUtc DATETIME
)
AS

    SET NOCOUNT ON

    INSERT
    INTO
        [ELMAH_Error]
        (
            [ErrorId],
            [Application],
            [Host],
            [Type],
            [Source],
            [Message],
            [User],
            [AllXml],
            [StatusCode],
            [TimeUtc]
        )
    VALUES
        (
            @ErrorId,
            @Application,
            @Host,
            @Type,
            @Source,
            @Message,
            @User,
            @AllXml,
            @StatusCode,
            @TimeUtc
        )






GO
/****** Object:  StoredProcedure [dbo].[EXECUTA_FECHAMENTO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EXECUTA_FECHAMENTO]
 @ManagerUnitId INT
AS

 DECLARE @Count INT;
 DECLARE @Contador INT = 0
 DECLARE @Id INT;
 DECLARE @LifeCycle SMALLINT;
 DECLARE @CurrentMonth SMALLINT;
 DECLARE @MaterialItemCode INT;
 DECLARE @AssetStartId INT;
 DECLARE @AssetId INT;
 DECLARE @CountInservivel INT;
 DECLARE @AssetIdInservivel INT = Null;


 DECLARE @RateDepreciationMonthly DECIMAL(18,2); 
 DECLARE @AccumulatedDepreciation DECIMAL(18,2)
 DECLARE @CurrentValue DECIMAL(18,2)
 DECLARE @CurrentDate DATETIME;

 DECLARE @RateDepreciationMonthlyInserir DECIMAL(18,2); 
 DECLARE @AccumulatedDepreciationInserir DECIMAL(18,2)
 DECLARE @CurrentValueInserir DECIMAL(18,2)
 DECLARE @CurrentDateInserir DATETIME;
 DECLARE @CurrentMonthInserir SMALLINT;

 DECLARE @_assetStartId INT;
 DECLARE @TransferenciaCount INT;

 DECLARE @Retorno VARCHAR(1000)
 DECLARE @Erro BIT = 0

 SET DATEFORMAT YMD;
 DECLARE @DataFinal DATETIME = '2020-07-10'

 DECLARE @Table AS Table(
	Id INT IDENTITY(1,1) NOT NULL,
	MaterialItemCode INT NOT NULL,
	AssetStartId INT,
	AssetId INT

 )


 DECLARE @TableInservivel AS TABLE(
	Id INT NOT NULL IDENTITY(1,1),
	AssetId INT NOT NULL,
	MaterialItemCode INT NOT NULL
 )

 INSERT INTO @TableInservivel(AssetId,MaterialItemCode)
 SELECT [bem].[Id], [bem].[MaterialItemCode] FROM [dbo].[Asset] [bem] 
 INNER JOIN [dbo].[AssetMovements] [mov] ON [bem].[Id] = [mov].[AssetId]
  WHERE [mov].[ManagerUnitId] = @ManagerUnitId AND [mov].[AuxiliaryAccountId] = 212508


 INSERT INTO @Table(MaterialItemCode,AssetStartId,AssetId)
 SELECT [MaterialItemCode],[AssetStartId],[AssetId] FROM [dbo].[Asset] [bem]
 INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [bem].[Id] 
 WHERE [bem].[MaterialGroupCode] <> 0
   AND [bem].[ManagerUnitId] = @ManagerUnitId
   AND [bem].[flagVerificado] IS NULL
   AND [bem].[flagDepreciaAcumulada] = 1
   AND [bem].[Status] = 1
   AND ([bem].[flagAcervo] IS NULL OR [bem].[flagAcervo] = 0)
   AND ([bem].[flagTerceiro] IS NULL OR [bem].[flagTerceiro] = 0)
   AND ([bem].[flagAnimalNaoServico] IS NULL OR [bem].[FlagAnimalNaoServico] = 0)
   AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] = 1)
   AND ([mov].[Status] = 1)


SET @Count =(SELECT COUNT(*) FROM @Table)

WHILE(@Contador < @Count)
BEGIN
	SET @Id =(SELECT TOP 1 [Id] FROM @Table)
	SET @MaterialItemCode = (SELECT [MaterialItemCode] FROM @Table WHERE [Id]= @Id);
	SET @AssetId = (SELECT [AssetId] FROM @Table WHERE [Id]= @Id); 
	SET @AssetStartId = (SELECT [AssetStartId] FROM @Table WHERE [Id]= @Id); 

   --- var _query = (db.MonthlyDepreciations.Where(x => x.AssetStartId == item.AssetStartId && x.ManagerUnitId == managerUnitId).OrderByDescending(x => x.Id)).Take(1);


	SET @LifeCycle =(SELECT TOP 1 [LifeCycle] FROM [dbo].[MonthlyDepreciation] 
					  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] =@ManagerUnitId
					  ORDER BY [Id] DESC);

	SET @CurrentMonth =(SELECT TOP 1 [CurrentMonth] FROM [dbo].[MonthlyDepreciation] 
					  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] =@ManagerUnitId
					  ORDER BY [Id] DESC);

	SET @RateDepreciationMonthly = (SELECT TOP 1 [RateDepreciationMonthly] FROM [dbo].[MonthlyDepreciation] 
					  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] =@ManagerUnitId
					  ORDER BY [Id] DESC);

	SET @AccumulatedDepreciation = (SELECT TOP 1 [AccumulatedDepreciation] FROM [dbo].[MonthlyDepreciation] 
					  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] =@ManagerUnitId
					  ORDER BY [Id] DESC);

	SET @CurrentValue = (SELECT TOP 1 [CurrentValue] FROM [dbo].[MonthlyDepreciation] 
					  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] =@ManagerUnitId
					  ORDER BY [Id] DESC);
	SET @CurrentDate = (SELECT TOP 1 [CurrentDate] FROM [dbo].[MonthlyDepreciation] 
					  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] =@ManagerUnitId
					  ORDER BY [Id] DESC);

	SET @CountInservivel =(SELECT COUNT(*) FROM @TableInservivel);
	
	IF(@CountInservivel IS NOT NULL)
	BEGIN
		SET @AssetIdInservivel =(SELECT AssetId FROM @TableInservivel WHERE [MaterialItemCode] = @MaterialItemCode)
	END;

	
	IF(ISNULL(@LifeCycle,0) > 0 AND @LifeCycle >= @CurrentMonth)
	BEGIN
		IF (@AssetIdInservivel IS NULL)
		BEGIN
			SET @RateDepreciationMonthlyInserir = @RateDepreciationMonthly;
            SET @AccumulatedDepreciationInserir = @AccumulatedDepreciation + @RateDepreciationMonthly;
            SET @CurrentValueInserir = @CurrentValue - @RateDepreciationMonthly;
			SET @CurrentMonthInserir =@CurrentMonth + 1;
			SET @CurrentDateInserir = (SELECT DATEADD(MONTH,1,@CurrentDate));

			INSERT INTO [dbo].[MonthlyDepreciation]
						([AssetStartId]
						,[NumberIdentification]
						,[ManagerUnitId]
						,[MaterialItemCode]
						,[InitialId]
						,[AcquisitionDate]
						,[CurrentDate]
						,[DateIncorporation]
						,[LifeCycle]
						,[CurrentMonth]
						,[ValueAcquisition]
						,[CurrentValue]
						,[ResidualValue]
						,[RateDepreciationMonthly]
						,[AccumulatedDepreciation]
						,[UnfoldingValue]
						,[Decree]
						,[ManagerUnitTransferId]
						,[MonthlyDepreciationId])
			SELECT TOP 1 [AssetStartId]
				   ,[NumberIdentification]
				   ,[ManagerUnitId]
				   ,[MaterialItemCode]
				   ,[InitialId]
				   ,[AcquisitionDate]
				   ,@CurrentDateInserir
				   ,[DateIncorporation]
				   ,[LifeCycle]
				   ,@CurrentMonthInserir
				   ,[ValueAcquisition]
				   ,@CurrentValueInserir
				   ,[ResidualValue]
				   ,@RateDepreciationMonthlyInserir
				   ,@AccumulatedDepreciationInserir
				   ,[UnfoldingValue]
				   ,[Decree]
				   ,[ManagerUnitTransferId]
				   ,[MonthlyDepreciationId]
			FROM [dbo].[MonthlyDepreciation] 
			WHERE [AssetStartId] =@AssetStartId AND [ManagerUnitId] = @ManagerUnitId 
			ORDER BY [Id] DESC

		
		END
	END
	ELSE
		BEGIN
			IF (@AssetIdInservivel IS NULL)
			BEGIN
				SET @TransferenciaCount = (SELECT COUNT(*) FROM [dbo].[AssetMovements] WHERE [AssetTransferenciaId] = @AssetId);
				
				IF(@AssetStartId IS NULL)
				BEGIN
					IF(@TransferenciaCount IS NOT NULL)
					BEGIN 
						SET @_assetStartId = (SELECT [AssetId] FROM [dbo].[AssetMovements] WHERE [AssetTransferenciaId] = @AssetId);;
					END
					ELSE
						BEGIN
							SET @_assetStartId = @AssetId
						END;
                          
					UPDATE [dbo].[Asset] SET [AssetStartId] = @_assetStartId  WHERE [Id] = @AssetId;
                    UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] = @_assetStartId WHERE [AssetId] = @AssetId;
				END
				ELSE
					BEGIN
						SET @_assetStartId = @AssetStartId;
					END;

				EXEC [dbo].[SAM_DEPRECIACAO_UGE] @ManagerUnitId,@MaterialItemCode,@AssetStartId,@DataFinal,0,@Retorno OUT,@Erro OUT
			END

		END

	DELETE FROM @Table WHERE [Id] = @Id
	SET @Contador = @Contador + 1;
END;


GO
/****** Object:  StoredProcedure [dbo].[EXECUTAR_DEPRECIAO_UGE]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EXECUTAR_DEPRECIAO_UGE](
 @ManagerUnitId INT,
 @mesAnoReferenciaVerificar DATETIME

)
AS
SET DATEFORMAT YMD;

DECLARE @ManagerUnitCount INT =0;
DECLARE @ManagerUnitContador INT = 0;
DECLARE @_Transferencia BIT = 0;
DECLARE @TransferenciaId INT = NULL;
DECLARE @depreciacaoAssetId INT = NULL;
DECLARE @_assetStartId INT = NULL;
DECLARE @DepreciacaoMaterialItemCode INT = NULL;
DECLARE @transferenciaManagerUnitId INT = NULL;
DECLARE @Retorno NVARCHAR(1000)= NULL;
DECLARE @Erro BIT =  0;

DECLARE @MonthlyDepreciationCount INT = 0;
DECLARE @MonthlyDepreciationContador INT= 0;
DECLARE @MonthlyDepreciationId INT = 0;
DECLARE @MonthlyDepreciationManagerUnitId INT = 0;

DECLARE @MonthlyDepreciation AS TABLE(
	Id INT NOT NULL,
	ManagerUnitId INT NULL

)

DECLARE @TableManagerUnit AS TABLE(
	MaterialItemCode INT NOT NULL,
    AssetStartId INT,
	AssetId INT NOT NULL 
)
INSERT INTO @TableManagerUnit(MaterialItemCode,AssetStartId,AssetId)
SELECT DISTINCT a.MaterialItemCode,
       a.AssetStartId,
	   a.Id 
 FROM [dbo].[Asset] a
INNER JOIN [dbo].[AssetMovements] [m] ON [a].[Id] = [m].[AssetId]
WHERE a.MaterialGroupCode <> 0
   AND a.ManagerUnitId = @ManagerUnitId
   AND a.flagVerificado IS NULL
   AND a.flagDepreciaAcumulada = 1
   AND (a.flagAcervo IS NULL OR a.flagAcervo = 0)
   AND (a.flagTerceiro IS NULL OR a.flagTerceiro = 0)
   AND (m.FlagEstorno IS NULL OR m.FlagEstorno = 0)
   ---AND (m.AssetTransferenciaId IS NULL)

SET @ManagerUnitCount = (SELECT COUNT(*) FROM @TableManagerUnit)


WHILE(@ManagerUnitContador <= @ManagerUnitCount)
BEGIN
	SET @depreciacaoAssetId = (SELECT TOP 1 [AssetId] FROM @TableManagerUnit)
	SET @DepreciacaoMaterialItemCode = (SELECT TOP 1 [MaterialItemCode] FROM @TableManagerUnit WHERE [AssetId] = @depreciacaoAssetId)

    SET @TransferenciaId =(SELECT TOP 1 [t].[MonthlyDepreciationId]  FROM [dbo].[AssetMovements] [t]  
	                        WHERE t.AssetTransferenciaId = @depreciacaoAssetId ORDER BY [t].[Id] DESC)
	
	IF(@TransferenciaId IS NOT NULL)
	BEGIN
		SET @_Transferencia = 1;
		SET @_assetStartId = @TransferenciaId;
		
		UPDATE [dbo].[Asset] SET [AssetStartId] =  @_assetStartId  WHERE [Id] = @TransferenciaId;

        UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] = @_assetStartId WHERE [AssetId] = @TransferenciaId;

        UPDATE [dbo].[Asset] SET [AssetStartId] =  @_assetStartId  WHERE [Id] =  @depreciacaoAssetId;

        UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] = @_assetStartId  WHERE [AssetId] = @depreciacaoAssetId;

	


	END;
	ELSE
		BEGIN
			UPDATE [dbo].[Asset] SET [AssetStartId] = @depreciacaoAssetId WHERE [Id] =  @depreciacaoAssetId;
            UPDATE [dbo].[AssetMovements] SET [MonthlyDepreciationId] = @depreciacaoAssetId WHERE [AssetId] = @depreciacaoAssetId;

            SET @_assetStartId = @depreciacaoAssetId;

		END;

	

          DELETE FROM [dbo].[MonthlyDepreciation] WHERE [MaterialItemCode] = @DepreciacaoMaterialItemCode AND [AssetStartId] = @_assetStartId;

         EXEC [dbo].[SAM_DEPRECIACAO_UGE] @ManagerUnitId= @ManagerUnitId
										 ,@MaterialItemCode = @DepreciacaoMaterialItemCode
										 ,@AssetStartId = @_assetStartId 
										 ,@DataFinal = @mesAnoReferenciaVerificar 
										 ,@Fechamento = 0
										 ,@Retorno = @Retorno
										 ,@Erro =@Erro;
           
		

	DELETE FROM @TableManagerUnit WHERE [AssetId] = @depreciacaoAssetId

	SET @ManagerUnitContador = @ManagerUnitContador + 1;

END 


GO
/****** Object:  StoredProcedure [dbo].[INSERT_RELATORIO_CONTABIL]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[INSERT_RELATORIO_CONTABIL]
	@MonthReference NVARCHAR(6)
  , @ManagerUnitId  INT
AS

SET DATEFORMAT YMD;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; 

DECLARE @AssetIdsEstornados TABLE(
	AssetId INT,
	AuxiliaryAccountId INT
)
--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
	--BEGIN CODE --
	INSERT INTO @AssetIdsEstornados(AssetId,AuxiliaryAccountId)  
	SELECT [mov].[AssetId],[mov].[AuxiliaryAccountId] FROM [AssetMovements] [mov] WITH(NOLOCK)
			WHERE [mov].[ManagerUnitId] = @ManagerUnitId 
			  AND [mov].[MovementTypeId] = 5 --- Inventário Inicial
			  AND ([mov].[FlagEstorno] IS NOT NULL ) AND ([mov].[DataEstorno] IS NOT NULL)
		GROUP BY [mov].[AssetId],[mov].[AuxiliaryAccountId]

DECLARE @Table AS TABLE(
	Id INT NOT NULL IDENTITY(1,1),
	ClosingId NVARCHAR(100) NULL,
	DepreciationAccount INT NULL,
	DepreciationDescription NVARCHAR(500) NULL,
	DepreciationAccountStatus BIT NULL,
	BookAccount INT NOT NULL,
	AccountingDescription NVARCHAR(500),
	BookAccountStatus BIT NOT NULL,
	DepreciationMonth DECIMAL(18,2),
	AccumulatedDepreciation DECIMAL(18,2),
	AccountingValue DECIMAL(18,2)

)


INSERT INTO @Table(ClosingId
                ,DepreciationAccount
				,DepreciationDescription
				,DepreciationAccountStatus
				,BookAccount
				,AccountingDescription
				,BookAccountStatus
				,AccountingValue
				,DepreciationMonth
				,AccumulatedDepreciation)

SELECT  (CASE WHEN  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao] IS NULL THEN NEWID() ELSE NULL END) AS 'Id'
	        , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]									AS 'ContaContabilDepreciacao'				/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]						AS 'ContaContabilDepreciacaoDescricao'		/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoStatus]							AS 'ContaContabilDepreciacaoStatus'
			, (select top 1 ContaContabilApresentacao from AuxiliaryAccount where BookAccount = [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]
																			AND Description like [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]
																			AND Status = [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]
			   )																						AS 'ContaContabil'							/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]									AS 'ContaContabilDescricao'					/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]										AS 'ContaContabilStatus'
			, SUM([visaoGeral_UGE__MESREF_ATUAL].[ValorContabil])										AS 'ValorContabil'							/*CAMPO RELATORIO*/
			--, SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoAcumuladaAteMesAnterior])				AS 'DepreciacaoAcumuladaAteMesAnterior'		/*CAMPO RELATORIO*/
			, ISNULL(SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoNoMes]),0)							AS 'DepreciacaoNoMes'						/*CAMPO RELATORIO*/
			, ISNULL(SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoAcumulada]),0)						AS 'DepreciacaoAcumulada'					/*CAMPO RELATORIO*/
	FROM
		(
			SELECT  
					 --(SELECT sigla.Name FROM Initial sigla WITH(NOLOCK) WHERE  bemPatrimonial.InitialId = sigla.Id)																														AS 'Sigla',
					 --(SELECT CONVERT(INT,bemPatrimonial.NumberIdentification) FROM Asset bemPatrimonial WITH(NOLOCK) WHERE bemPatrimonial.Id = movimentacaoBP.AssetId)																	AS 'Chapa',
					 (SELECT [contaContabilDepreciacao].[Code]			FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId 
																																											FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																											WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacao'
					, (SELECT [contaContabilDepreciacao].[Description]	FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId
																																											FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																											WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacaoDescricao'
					, (SELECT [contaContabilDepreciacao].[Status]		FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId
																																											FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																											WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacaoStatus'
																																											
					, (SELECT [contaContabil].[BookAccount] FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																	AS 'ContaContabil'
					, (SELECT [contaContabil].[Description] FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																	AS 'ContaContabilDescricao'
					, (SELECT [contaContabil].[Status]		FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																	AS 'ContaContabilStatus'

					, CASE 
							WHEN movimentacaoBP.AuxiliaryAccountId = 212508 
							THEN  (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[CurrentValue],0) 'ValorMensal'			
										  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
										  WHERE 
												ManagerUnitId = movimentacaoBP.[ManagerUnitId]
										  AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
										  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId] 
										  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <=
										  (select top 1 (CAST(CONVERT(CHAR(6), MovimentDate, 112) AS INT)) from AssetMovements with(nolock)
										  where AssetId = bemPatrimonial.Id AND 
											    FlagEstorno IS NULL AND
												MovementTypeId = 19))
										 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
									)
							ELSE (SELECT TOP 1 ISNULL([bemPatrimonial].[ValueAcquisition], 0.00))
					END AS 'ValorContabil'
					, 
					CASE 
						WHEN movimentacaoBP.AuxiliaryAccountId = 212508 THEN 0
						ELSE (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[RateDepreciationMonthly],0) 'ValorMensal'			
									  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
									  WHERE 
											ManagerUnitId = movimentacaoBP.[ManagerUnitId]
									  AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
									  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId] 
									  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) = CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
									 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
						      ) 
						END AS 'DepreciacaoNoMes'
					, 
					CASE 
						WHEN movimentacaoBP.AuxiliaryAccountId = 212508 THEN 0
						ELSE (SELECT TOP 1 [dadosDepreciacaoMensal].[AccumulatedDepreciation] 'ValorAcumulado'			
									  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
									  WHERE 
											ManagerUnitId = movimentacaoBP.[ManagerUnitId]
											AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
									  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId]
									  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
									 ORDER BY [dadosDepreciacaoMensal].[Id] DESC

							 )
					END AS 'DepreciacaoAcumulada'
					--LINHA 'DEBUG' MES-REFERENCIA MOVIMENTACAO NA UGE CONSULTADA
					, CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112)																																						AS 'MesReferenciaMovimentacao'
			FROM 
				AssetMovements movimentacaoBP WITH(NOLOCK)
			INNER JOIN Asset bemPatrimonial WITH(NOLOCK) ON movimentacaoBP.AssetId = bemPatrimonial.Id
			WHERE (bemPatrimonial.[flagVerificado] IS NULL)        
		      AND (bemPatrimonial.[flagDepreciaAcumulada] = 1)
			  AND movimentacaoBP.Id IN (SELECT MAX(Id)
									  FROM 
										AssetMovements movimentacaoBP WITH(NOLOCK)
									  WHERE ManagerUnitId = @ManagerUnitId
										AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
										--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
										--BEGIN CODE --
										AND (movimentacaoBP.AssetId NOT IN(SELECT [AssetId] FROM @AssetIdsEstornados WHERE [AuxiliaryAccountId] = movimentacaoBP.AuxiliaryAccountId)) --
										AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
										--END CODE --
									  GROUP BY AssetId
									 )
			AND (movimentacaoBP.ManagerUnitId = @ManagerUnitId)
			--AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
			
			AND (movimentacaoBP.MovementTypeId NOT IN (
															11,	--Transfêrencia
															12,	--Doação
															15,	--Extravio
															16,	--Obsoleto
															17,	--Danificado
															18,	--Sucata
															23, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)  
															24, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)  
															27, --COMODATOS/DE TERCEIROS RECEBIDOS
															42,	--SAÍDA INSERVÍVEL DA UGE - DOAÇÃO																		     
															43,	--SAÍDA INSERVÍVEL DA UGE - TRANSFERÊNCIA																		     
															44,	--COMODATO - CONCEDIDOS - PATRIMONIADO																		     
															45,	--COMODATO/DE TERCEIROS - RECEBIDOS																		     
															46,	--DOAÇÃO CONSOLIDAÇÃO - PATRIMÔNIO																		     
															47,	--DOAÇÃO INTRA - NO - ESTADO - PATRIMÔNIO																	     
															48,	--DOAÇÃO MUNÍCIPIO - PATRIMÔNIO
															49,	--DOAÇÃO OUTROS ESTADOS - PATRIMÔNIO
															50,	--DOAÇÃO UNIÃO - PATRIMÔNIO
															51,	--ESTRAVIO, FURTO, ROUBO - PATRIMONIADO
															52,	--MORTE ANIMAL - PATRIMONIADO
															53,	--MORTE VEGETAL - PATRIMONIADO
															54,	--MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO
															55,	--SEMENTES, PLANTAS, INSUMOS E ÁRVORES
															56,	--TRANSFERÊNCIA OUTRO ÓRGÃO - PATRIMONIADO
															57,	--TRANSFERÊNCIA MESMO ÓRGÃO - PATRIMONIADO
															58,	--PERDAS INVOLUNTÁRIAS - BENS MÓVEIS
															59,	--PERDAS INVOLUNTÁRIAS - INSERVÍVEL BENS MÓVEIS
															81,
															91,
															95))
		AND (
				movimentacaoBP.ManagerUnitId = @ManagerUnitId
				AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
				AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
			)
		) AS visaoGeral_UGE__MESREF_ATUAL
	GROUP BY
			  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoStatus]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]
	ORDER BY 
			  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]


DECLARE @Count INT = 0;
DECLARE @Contador INT  = 0;
DECLARE @Id INT;



	 INSERT INTO [dbo].[AccountingClosing]
           ([ManagerUnitCode]
           ,[ManagerUnitDescription]
           ,[DepreciationAccount]
           ,[DepreciationMonth]
           ,[AccumulatedDepreciation]
           ,[Status]
           ,[DepreciationDescription]
           ,[ReferenceMonth]
		   ,[ItemAccounts]
		   ,[AccountName]
		   ,[GeneratedNL]
		   ,[ClosingId]
		   ,[ManagerCode])
	SELECT 
	     (SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId )
        ,(SELECT [Description] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
        ,[tab].DepreciationAccount
        ,SUM([tab].DepreciationMonth)
        ,SUM([tab].AccumulatedDepreciation)
        ,[tab].DepreciationAccountStatus
        ,[tab].DepreciationDescription
        ,@MonthReference
		,(SELECT TOP 1 [dep].[MaterialItemCode] 
		    FROM [dbo].[DepreciationMaterialItem] [dep] 
			WHERE [dep].[DepreciationAccount] = [tab].DepreciationAccount)
		,(SELECT TOP 1 [DescricaoContaDepreciacaoContabilizaSP] FROM [dbo].[DepreciationAccount] WHERE  [Code]= [tab].DepreciationAccount)
		,NULL
		,NULL
		,(SELECT TOP 1 [ManagerCode] FROM [dbo].[Institution] [it]
		   INNER JOIN [dbo].[BudgetUnit] [bu] ON [bu].[InstitutionId] = [it].[Id] 
		   INNER JOIN [dbo].[ManagerUnit] [ma] ON [ma].[BudgetUnitId] = [bu].[Id] 
		   WHERE [ma].[Id] = @ManagerUnitId)
	 FROM @Table [tab] WHERE [tab].[ClosingId] IS NULL
	 GROUP BY
		[tab].DepreciationAccount
       ,[tab].DepreciationAccountStatus
       ,[tab].DepreciationDescription

	   INSERT INTO [dbo].[DepreciationAccountingClosing]
        ([AccountingClosingId]
        ,[BookAccount]
        ,[AccountingValue]
        ,[DepreciationMonth]
        ,[AccumulatedDepreciation]
        ,[Status]
        ,[AccountingDescription]
		,[ReferenceMonth]
		,[ManagerUnitCode]
		,[ManagerDescription])

	SELECT [clg].[Id]
		,[tbl].[BookAccount]
		,ISNULL([tbl].[AccountingValue],0)
		,[tbl].[DepreciationMonth]
		,[tbl].[AccumulatedDepreciation] 
		,[tbl].[BookAccountStatus]
		,[tbl].[AccountingDescription]
		,@MonthReference
		,(SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId )
        ,(SELECT [Description] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
	FROM [dbo].[AccountingClosing] [clg] 
    INNER JOIN @Table [tbl] ON [clg].[DepreciationAccount] = [tbl].[DepreciationAccount]
				AND [clg].[ReferenceMonth] = @MonthReference
				and [clg].[ManagerUnitCode] = (SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId )
				and [clg].ClosingId IS NULL

SET @Count = (SELECT COUNT(*) FROM @Table WHERE DepreciationAccount IS NULL)

WHILE(@Contador < @Count)
BEGIN
	SET @Id = (SELECT TOP 1 [Id] FROM @Table);

	
	 INSERT INTO [dbo].[AccountingClosing]
           ([ManagerUnitCode]
           ,[ManagerUnitDescription]
           ,[DepreciationAccount]
           ,[DepreciationMonth]
           ,[AccumulatedDepreciation]
           ,[Status]
           ,[DepreciationDescription]
           ,[ReferenceMonth]
		   ,[ItemAccounts]
		   ,[AccountName]
		   ,[GeneratedNL]
		   ,[ClosingId]
		   ,[ManagerCode])
	SELECT 
	     (SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId )
        ,(SELECT [Description] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
        ,[tab].DepreciationAccount
        ,SUM([tab].DepreciationMonth)
        ,SUM([tab].AccumulatedDepreciation)
        ,[tab].DepreciationAccountStatus
        ,[tab].DepreciationDescription
        ,@MonthReference
		,(SELECT TOP 1 [dep].[MaterialItemCode] FROM [dbo].[DepreciationMaterialItem] [dep] WHERE [dep].[DepreciationAccount] = [tab].DepreciationAccount)
		,(SELECT TOP 1 [DescricaoContaDepreciacaoContabilizaSP] FROM [dbo].[DepreciationAccount] WHERE  [Code]= [tab].DepreciationAccount)
		,NULL
		,ClosingId
		,(SELECT TOP 1 [ManagerCode] FROM [dbo].[Institution] [it]
		   INNER JOIN [dbo].[BudgetUnit] [bu] ON [bu].[InstitutionId] = [it].[Id] 
		   INNER JOIN [dbo].[ManagerUnit] [ma] ON [ma].[BudgetUnitId] = [bu].[Id] 
		   WHERE [ma].[Id] = @ManagerUnitId)
	 FROM @Table [tab] WHERE [tab].[Id] = @Id
	 GROUP BY
		[tab].DepreciationAccount
       ,[tab].DepreciationAccountStatus
       ,[tab].DepreciationDescription
	   ,[tab].ClosingId

	INSERT INTO [dbo].[DepreciationAccountingClosing]
        ([AccountingClosingId]
        ,[BookAccount]
        ,[AccountingValue]
        ,[DepreciationMonth]
        ,[AccumulatedDepreciation]
        ,[Status]
        ,[AccountingDescription]
		,[ReferenceMonth]
		,[ManagerUnitCode]
		,[ManagerDescription])

	SELECT [clg].[Id]
		,[tbl].[BookAccount]
		,ISNULL([tbl].[AccountingValue],0)
		,[tbl].[DepreciationMonth]
		,[tbl].[AccumulatedDepreciation] 
		,[tbl].[BookAccountStatus]
		,[tbl].[AccountingDescription]
		,@MonthReference
		,(SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId )
        ,(SELECT [Description] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
	FROM [dbo].[AccountingClosing] [clg] 
    INNER JOIN @Table [tbl] ON [clg].ClosingId = [tbl].ClosingId
	
	DELETE FROM @Table WHERE [Id] = @Id 
	SET @Contador = @Contador + 1;
END;
DELETE FROM @Table;

declare @tabelaValorBPSTerceiros as table
( descricao varchar(40),
  valorDaConta decimal(18,2)
);

insert into @tabelaValorBPSTerceiros(descricao, valorDaConta)
exec [PRC_RELATORIO__BP_TERCEIRO]
@ManagerUnitId = @ManagerUnitId,
@MonthReference = @MonthReference

IF((select count(*) from @tabelaValorBPSTerceiros) > 0)
BEGIN 
	Declare @ContaContabilTerceiro int, @StatusDaConta bit, 
			@ValorContaTerceiro decimal(18,2), @DescriptionDaConta [varchar](100);

	select top 1 
	@ContaContabilTerceiro = BookAccount, 
	@StatusDaConta = [Status],
	@DescriptionDaConta = [Description]
	from AuxiliaryAccount where RelacionadoBP = 2;

	select top 1 @ValorContaTerceiro = valorDaConta from @tabelaValorBPSTerceiros;

	IF( (select count(*) from DepreciationAccountingClosing
		WHERE BookAccount = @ContaContabilTerceiro
		and ManagerUnitCode = (select top 1 Code from ManagerUnit where id = @ManagerUnitId)
		and ReferenceMonth = @MonthReference) > 0)
	BEGIN
		update DepreciationAccountingClosing
		set AccountingValue = @ValorContaTerceiro
		where BookAccount = @ContaContabilTerceiro
		and ManagerUnitCode = (select top 1 Code from ManagerUnit where id = @ManagerUnitId)
		and ReferenceMonth = @MonthReference
	END
	ELSE
	BEGIN
	
	Declare @novoGUId [uniqueidentifier];
	SET @novoGUId = NEWID();

	INSERT INTO [dbo].[AccountingClosing]
           ([ManagerUnitCode]
           ,[ManagerUnitDescription]
           ,[DepreciationAccount]
           ,[DepreciationMonth]
           ,[AccumulatedDepreciation]
           ,[Status]
           ,[DepreciationDescription]
           ,[ReferenceMonth]
		   ,[ItemAccounts]
		   ,[AccountName]
		   ,[GeneratedNL]
		   ,[ClosingId]
		   ,[ManagerCode])
	VALUES(
	     (SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId )
        ,(SELECT [Description] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
        ,NULL --Conta Contábil de Terceiro não tem Conta de Depreciação vinculada
        ,0 --BP Terceiro não deprecia
        ,0 --BP Terceiro não deprecia
        ,NULL --Conta Contábil de Terceiro não tem Conta de Depreciação vinculada
        ,NULL --Conta Contábil de Terceiro não tem Conta de Depreciação vinculada
        ,@MonthReference
		,NULL --Conta terceiro não manda pro Contabiliza
		,NULL --Conta terceiro não manda pro Contabiliza
		,NULL
		,@novoGUId
		,(SELECT TOP 1 [ManagerCode] FROM [dbo].[Institution] [it]
		   INNER JOIN [dbo].[BudgetUnit] [bu] ON [bu].[InstitutionId] = [it].[Id] 
		   INNER JOIN [dbo].[ManagerUnit] [ma] ON [ma].[BudgetUnitId] = [bu].[Id] 
		   WHERE [ma].[Id] = @ManagerUnitId)
	);

	INSERT INTO [dbo].[DepreciationAccountingClosing]
        ([AccountingClosingId]
        ,[BookAccount]
        ,[AccountingValue]
        ,[DepreciationMonth]
        ,[AccumulatedDepreciation]
        ,[Status]
        ,[AccountingDescription]
		,[ReferenceMonth]
		,[ManagerUnitCode]
		,[ManagerDescription])

	SELECT [clg].[Id]
		,@ContaContabilTerceiro
		,@ValorContaTerceiro
		,[clg].[DepreciationMonth]
		,[clg].[AccumulatedDepreciation] 
		,@StatusDaConta
		,@DescriptionDaConta
		,@MonthReference
		,(SELECT [Code] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
        ,(SELECT [Description] FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)
	FROM [dbo].[AccountingClosing] [clg] 
	where [clg].ClosingId = @novoGUId;

	END

END


GO
/****** Object:  StoredProcedure [dbo].[PRC_RELATORIO__BAIXAS_BP__MESREF]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--QUARTO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO (BAIXAS MES-REF)
CREATE PROCEDURE [dbo].[PRC_RELATORIO__BAIXAS_BP__MESREF]
(
	   @MonthReference VARCHAR(6)
	 , @ManagerUnitId INT
) AS
DECLARE @AssetIdsEstornados TABLE(
	AssetId INT,
	AuxiliaryAccountId INT
)
BEGIN

	--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
	--BEGIN CODE --
	INSERT INTO @AssetIdsEstornados(AssetId,AuxiliaryAccountId)  
	SELECT [mov].[AssetId],[mov].[AuxiliaryAccountId] FROM [AssetMovements] [mov] WITH(NOLOCK)
			WHERE [mov].[ManagerUnitId] = @ManagerUnitId 
			  AND [mov].[MovementTypeId] = 5 --- Inventário Inicial
			  AND ([mov].[FlagEstorno] IS NOT NULL) AND ([mov].[DataEstorno] IS NOT NULL)
		GROUP BY [mov].[AssetId],[mov].[AuxiliaryAccountId]
	--END CODE --
SELECT 
		  (SELECT tipoMovimentacao.[Description] FROM MovementType tipoMovimentacao WITH(NOLOCK) WHERE movimentacaoBP.MovementTypeId = tipoMovimentacao.Id)																AS 'TipoMovimentacao'
		, (SELECT [contaContabilDepreciacao].[Code] FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId 
																																						FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																						WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacao'
		, (SELECT [contaContabilDepreciacao].[Description] FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId
																																						FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																						WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacaoDescricao'

		, (SELECT ContaContabilApresentacao FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																AS 'ContaContabil'
		, (SELECT [Description] FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																			AS 'ContaContabilDescricao'

		, SUM(dadosDepreciacaoMensal.ValueAcquisition)																																									AS 'ValorAquisicao'
		, SUM(CASE 
					WHEN [BP].[flagAcervo] = 1 THEN  0
					WHEN [BP].[flagTerceiro] = 1 THEN  0 
					ELSE dadosDepreciacaoMensal.RateDepreciationMonthly END)																																			    AS 'DepreciacaoNoMes'
		, SUM(CASE 
					WHEN [BP].[flagAcervo] = 1 THEN  ISNULL([BP].[ValueUpdate],[BP].[ValueAcquisition])
					WHEN [BP].[flagTerceiro] = 1 THEN  ISNULL([BP].[ValueUpdate],[BP].[ValueAcquisition])
					ELSE dadosDepreciacaoMensal.CurrentValue END)																																							AS 'ValorAtual'

		--LINHA 'DEBUG' MES-REFERENCIA DEPRECIACAO NA UGE CONSULTADA
		--, CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112)																																				AS 'MesReferenciaMovimentacaoDepreciacao'
		--, CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112)																																						AS 'MesReferenciaMovimentacao'
FROM 
	AssetMovements movimentacaoBP WITH(NOLOCK)
	INNER JOIN Asset BP WITH(NOLOCK) ON movimentacaoBP.AssetId = BP.Id
	--POR CONTA DAS TRANSFERENCIAS, SERAH UTILIZADA A COLUNA '.MonthlyDepreciationId' PARA SE PODER 'PUXAR' A DEPRECIACAO PARA UM DADO ANO-MES-REFERENCIA A PARTIR DA TABELA DE MOVIMENTACOES (AssetMovements)
	INNER JOIN MonthlyDepreciation dadosDepreciacaoMensal WITH(NOLOCK) ON movimentacaoBP.MonthlyDepreciationId = dadosDepreciacaoMensal.MonthlyDepreciationId
WHERE
	movimentacaoBP.Id IN (SELECT MAX(Id)
						  FROM 
							AssetMovements movimentacaoBP WITH(NOLOCK)
						  WHERE ManagerUnitId = @ManagerUnitId
							AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) = @MonthReference)
							--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
							--BEGIN CODE --
							AND (movimentacaoBP.AssetId NOT IN(SELECT [AssetId] FROM @AssetIdsEstornados WHERE [AuxiliaryAccountId] = movimentacaoBP.AuxiliaryAccountId)) 
							AND((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
							--END CODE --
						  GROUP BY AssetId
						 )
/*************************************************/
AND dadosDepreciacaoMensal.Id IN (
									SELECT MAX(Id)
									FROM 
									  MonthlyDepreciation dadosDepreciacaoMensal WITH(NOLOCK)
									WHERE ManagerUnitId = @ManagerUnitId
									  --AND (CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) <= @MonthReference) --PARA TRAZER O ULTIMO MES-REFERENCIADO DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
									  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
									GROUP BY [dadosDepreciacaoMensal].[MaterialItemCode], [dadosDepreciacaoMensal].[AssetStartId]
								 )
/*************************************************/
AND movimentacaoBP.ManagerUnitId = @ManagerUnitId
AND (FlagEstorno = 0 OR FlagEstorno IS NULL)
AND ([movimentacaoBP].MovementTypeId IN (
											11, --Transfêrencia
											12, --Doação
											15, --Extravio
											16, --Obsoleto
											17, --Danificado
											18,	--Sucata
											23, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)  
											24, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)  
											27, --COMODATOS/DE TERCEIROS RECEBIDOS
											42,	--SAÍDA INSERVÍVEL DA UGE - DOAÇÃO			
											43,	--SAÍDA INSERVÍVEL DA UGE - TRANSFERÊNCIA	
											44,	--COMODATO - CONCEDIDOS - PATRIMONIADO		
											45,	--COMODATO/DE TERCEIROS - RECEBIDOS			
											46,	--DOAÇÃO CONSOLIDAÇÃO - PATRIMÔNIO			
											47,	--DOAÇÃO INTRA - NO - ESTADO - PATRIMÔNIO	
											48,	--DOAÇÃO MUNÍCIPIO - PATRIMÔNIO
											49,	--DOAÇÃO OUTROS ESTADOS - PATRIMÔNIO
											50,	--DOAÇÃO UNIÃO - PATRIMÔNIO
											51,	--ESTRAVIO, FURTO, ROUBO - PATRIMONIADO
											52,	--MORTE ANIMAL - PATRIMONIADO
											53, --MORTE VEGETAL - PATRIMONIADO
											54, --MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO
											55, --SEMENTES, PLANTAS, INSUMOS E ÁRVORES
											56,	--TRANSFERÊNCIA OUTRO ÓRGÃO - PATRIMONIADO
											57,	--TRANSFERÊNCIA MESMO ÓRGÃO - PATRIMONIADO
											58,	--PERDAS INVOLUNTÁRIAS - BENS MÓVEIS
											59, --PERDAS INVOLUNTÁRIAS - INSERVÍVEL BENS MÓVEIS
											95 --VENDA/LEILÃO - SEMOVENTES
											--81, --DEPRECIAÇÃO – BAIXA (EXCLUSIVA CONTABILIZA-SP)
											--91  --RECLASSIFICACAO SEM EMPENHO (EXCLUSIVA CONTABILIZA-SP)
										)) 
AND (
		--SUBSELECT 01
		movimentacaoBP.ManagerUnitId = @ManagerUnitId
		AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) = @MonthReference)
		AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
		--SUBSELECT 02
		--AND (CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) <= @MonthReference) --PARA TRAZER O ULTIMO MES-REFERENCIADO DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
		AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT))
    )
GROUP BY
			  movimentacaoBP.MovementTypeId
		    , movimentacaoBP.AuxiliaryAccountId
			, [movimentacaoBP].[MovimentDate]
		    , CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112)

			--LINHA 'DEBUG' MES-REFERENCIA DEPRECIACAO NA UGE CONSULTADA
			--, [dadosDepreciacaoMensal].[CurrentDate]
			--, CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112)
			, dadosDepreciacaoMensal.MaterialItemCode
			, dadosDepreciacaoMensal.AssetStartId
ORDER BY
			  movimentacaoBP.AuxiliaryAccountId
			, dadosDepreciacaoMensal.MaterialItemCode
			, dadosDepreciacaoMensal.AssetStartId

END

GO
/****** Object:  StoredProcedure [dbo].[PRC_RELATORIO__BP_ACERVO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--QUINTO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO (BEM DE ACERVO)
CREATE PROCEDURE [dbo].[PRC_RELATORIO__BP_ACERVO]
(
	   @MonthReference VARCHAR(6)
	 , @ManagerUnitId INT
) AS

BEGIN

DECLARE @tabelaUltimoHistorico table(Id int);

	INSERT INTO @tabelaUltimoHistorico
	 select MAX(am.ID) ID
	 from AssetMovements am WITH(NOLOCK)
	 INNER JOIN Asset a WITH(NOLOCK)
	 on a.Id = am.AssetId
	 WHERE 
	 am.FlagEstorno IS NULL
	 and am.DataEstorno IS NULL
	 and am.ManagerUnitId = @ManagerUnitId
	 and a.flagVerificado IS NULL
	 and a.flagDepreciaAcumulada = 1
	 and (CONVERT(VARCHAR(6),am.MovimentDate,112)) <= @MonthReference
	 and a.flagAcervo = 1
	 and am.ResponsibleId IS NOT NULL --QUANDO EH EFETUADA A MOVIMENTACAO DE UM BP, APOS ACEITE NO DESTINO O SISTEMA LIMPA OS DADOS DE RESPONSAVEL/LOCALIZACAO NO REGISTRO DE MOVIMENTACAO
	 and a.Id NOT IN (
	 		select am.AssetId 
			from AssetMovements am WITH(NOLOCK)
	 		INNER JOIN Asset a WITH(NOLOCK)
	 		on a.Id = am.AssetId
	 		WHERE am.FlagEstorno IS NULL
	 		and am.DataEstorno IS NULL
	 		and am.ManagerUnitId = @ManagerUnitId
	 		and a.flagVerificado IS NULL
	 		and a.flagDepreciaAcumulada = 1
			and a.flagAcervo = 1
	 		and am.Status = 0
	 		and am.MovementTypeId IN (11,12,13,14,15,16,17,18,42,43,45,46,47,48,49,50,51,52,53,55,56,57,58,59,95)
	 		GROUP BY am.AssetId
	 		having (CONVERT(VARCHAR(6),MAX(am.MovimentDate),112)) <= @MonthReference
	 )
	 GROUP BY am.AssetId;

select
			(select top 1 ContaContabilApresentacao + ' - ' + [Description] from AuxiliaryAccount where id = movimetacao.AuxiliaryAccountId) AS 'ContaContabil',
		    SUM(BP.ValueAcquisition) AS 'ValorAquisicao'
from Asset BP
inner join AssetMovements movimetacao
on BP.Id = movimetacao.AssetId
inner join @tabelaUltimoHistorico ultimo
on movimetacao.Id = ultimo.Id
GROUP BY
	 movimetacao.AuxiliaryAccountId
ORDER BY 
	 movimetacao.AuxiliaryAccountId

END

GO
/****** Object:  StoredProcedure [dbo].[PRC_RELATORIO__BP_TERCEIRO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--SEXTO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO (BEM DE TERCEIRO)
CREATE PROCEDURE [dbo].[PRC_RELATORIO__BP_TERCEIRO]
(
	   @MonthReference VARCHAR(6)
	 , @ManagerUnitId INT
) AS
BEGIN

DECLARE @tabelaUltimoHistorico table(Id int);

	INSERT INTO @tabelaUltimoHistorico
	 select MAX(am.ID) ID
	 from AssetMovements am WITH(NOLOCK)
	 INNER JOIN Asset a WITH(NOLOCK)
	 on a.Id = am.AssetId
	 WHERE 
	 am.FlagEstorno IS NULL
	 and am.DataEstorno IS NULL
	 and am.ManagerUnitId = @ManagerUnitId
	 and a.flagVerificado IS NULL
	 and a.flagDepreciaAcumulada = 1
	 and (CONVERT(VARCHAR(6),am.MovimentDate,112)) <= @MonthReference
	 and a.flagTerceiro = 1
	 and am.ResponsibleId IS NOT NULL --QUANDO EH EFETUADA A MOVIMENTACAO DE UM BP, APOS ACEITE NO DESTINO O SISTEMA LIMPA OS DADOS DE RESPONSAVEL/LOCALIZACAO NO REGISTRO DE MOVIMENTACAO
	 and a.Id NOT IN (
	 		select am.AssetId 
			from AssetMovements am WITH(NOLOCK)
	 		INNER JOIN Asset a WITH(NOLOCK)
	 		on a.Id = am.AssetId
	 		WHERE am.FlagEstorno IS NULL
	 		and am.DataEstorno IS NULL
	 		and am.ManagerUnitId = @ManagerUnitId
	 		and a.flagVerificado IS NULL
	 		and a.flagDepreciaAcumulada = 1
			and a.flagTerceiro = 1
	 		and am.Status = 0
	 		and am.MovementTypeId IN (11,12,13,14,15,16,17,18,42,43,45,46,47,48,49,50,51,52,53,55,56,57,58,59,95)
	 		GROUP BY am.AssetId
	 		having (CONVERT(VARCHAR(6),MAX(am.MovimentDate),112)) <= @MonthReference
	 )
	 GROUP BY am.AssetId;

select
			(select top 1 ContaContabilApresentacao + ' - ' + [Description] from AuxiliaryAccount where id = movimetacao.AuxiliaryAccountId ) AS 'ContaContabil',
		    SUM(BP.ValueAcquisition) AS 'ValorAquisicao'
from Asset BP
inner join AssetMovements movimetacao
on BP.Id = movimetacao.AssetId
inner join @tabelaUltimoHistorico ultimo
on movimetacao.Id = ultimo.Id
GROUP BY
	 movimetacao.AuxiliaryAccountId
ORDER BY 
	 movimetacao.AuxiliaryAccountId
END

GO
/****** Object:  StoredProcedure [dbo].[PRC_RELATORIO__BP_TOTALMENTE_DEPRECIADO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--TERCEIRO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO (TOTALMENTE DEPRECIADOS)
CREATE PROCEDURE [dbo].[PRC_RELATORIO__BP_TOTALMENTE_DEPRECIADO]
(
	   @MonthReference VARCHAR(6)
	 , @ManagerUnitId INT
) AS
DECLARE @AssetIdsEstornados TABLE(
	AssetId INT,
	AuxiliaryAccountId INT
)

BEGIN
	--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
	--BEGIN CODE --
	INSERT INTO @AssetIdsEstornados(AssetId,AuxiliaryAccountId)  
	SELECT [mov].[AssetId],[mov].[AuxiliaryAccountId] FROM [AssetMovements] [mov] WITH(NOLOCK)
			WHERE [mov].[ManagerUnitId] = @ManagerUnitId 
			  AND [mov].[MovementTypeId] = 5 --- Inventário Inicial
			  AND ([mov].[FlagEstorno] IS NOT NULL) AND ([mov].[DataEstorno] IS NOT NULL)
		GROUP BY [mov].[AssetId],[mov].[AuxiliaryAccountId]
	--END CODE --
	SELECT  

			  (SELECT [contaContabilDepreciacao].[Code] FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId 
																																							FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																							WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacao'
			, (SELECT [contaContabilDepreciacao].[Description] FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId
																																							FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																							WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacaoDescricao'

			, (SELECT ContaContabilApresentacao FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																				AS 'ContaContabil'
			, (SELECT [Description] FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																			AS 'ContaContabilDescricao'

			, SUM(dadosDepreciacaoMensal.ValueAcquisition)																																									AS 'ValorAquisicao'
			, SUM(CASE 
					WHEN [bemPatrimonial].[flagAcervo] = 1 THEN  0
					WHEN [bemPatrimonial].[flagTerceiro] = 1 THEN  0
					ELSE dadosDepreciacaoMensal.AccumulatedDepreciation END)																																							AS 'DepreciacaoAcumulada'
			, SUM(CASE 
					WHEN [bemPatrimonial].[flagAcervo] = 1 THEN  ISNULL([bemPatrimonial].[ValueUpdate],[bemPatrimonial].[ValueAcquisition])
					WHEN [bemPatrimonial].[flagTerceiro] = 1 THEN  ISNULL([bemPatrimonial].[ValueUpdate],[bemPatrimonial].[ValueAcquisition])
					ELSE dadosDepreciacaoMensal.CurrentValue END)																																							AS 'ValorAtual'
		
			--LINHA 'DEBUG' MES-REFERENCIA DEPRECIACAO NA UGE CONSULTADA																																					
			--, CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112)																																						AS 'MesReferenciaMovimentacaoConsulta'
			--, CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112)																																				AS 'MesReferenciaDepreciacaoConsulta'

	FROM 
		AssetMovements movimentacaoBP WITH(NOLOCK)
		--POR CONTA DAS TRANSFERENCIAS, SERAH UTILIZADA A COLUNA '.MonthlyDepreciationId' PARA SE PODER 'PUXAR' A DEPRECIACAO PARA UM DADO ANO-MES-REFERENCIA A PARTIR DA TABELA DE MOVIMENTACOES (AssetMovements)
		INNER JOIN MonthlyDepreciation dadosDepreciacaoMensal WITH(NOLOCK) ON movimentacaoBP.MonthlyDepreciationId = dadosDepreciacaoMensal.MonthlyDepreciationId

		INNER JOIN Asset bemPatrimonial WITH(NOLOCK) ON movimentacaoBP.AssetId = bemPatrimonial.AssetStartId
	WHERE
		movimentacaoBP.Id IN  (SELECT MAX(Id)
							   FROM 
								AssetMovements movimentacaoBP
								where ManagerUnitId = @ManagerUnitId
								AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
								--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
								--BEGIN CODE --
								AND (movimentacaoBP.AssetId NOT IN(SELECT [AssetId] FROM @AssetIdsEstornados WHERE [AuxiliaryAccountId] = movimentacaoBP.AuxiliaryAccountId))
								AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
								--END CODE --
								GROUP BY AssetId
							  )
	/*************************************************/
	AND dadosDepreciacaoMensal.Id IN (
										SELECT MAX(Id)
										FROM 
										  MonthlyDepreciation dadosDepreciacaoMensal WITH(NOLOCK)
										WHERE 
											ManagerUnitId = @ManagerUnitId
										--  AND (CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) <= @MonthReference) --PARA TRAZER O ULTIMO MES-REFERENCIADO DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
										--GROUP BY AssetStartId
										AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
										GROUP BY [dadosDepreciacaoMensal].[MaterialItemCode], [dadosDepreciacaoMensal].[AssetStartId]
									 )
	/*************************************************/
	AND movimentacaoBP.ManagerUnitId = @ManagerUnitId
	AND (movimentacaoBP.FlagEstorno = 0 OR FlagEstorno IS NULL)
	AND (movimentacaoBP.MovementTypeId NOT IN (
												11, --Transfêrencia
												12, --Doação
												15, --Extravio
												16, --Obsoleto
												17, --Danificado
												18,	--Sucata
												23, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)  
												24, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)  
												27, --COMODATOS/DE TERCEIROS RECEBIDOS
												42,	--SAÍDA INSERVÍVEL DA UGE - DOAÇÃO			
												43,	--SAÍDA INSERVÍVEL DA UGE - TRANSFERÊNCIA	
												44,	--COMODATO - CONCEDIDOS - PATRIMONIADO		
												45,	--COMODATO/DE TERCEIROS - RECEBIDOS			
												46,	--DOAÇÃO CONSOLIDAÇÃO - PATRIMÔNIO			
												47,	--DOAÇÃO INTRA - NO - ESTADO - PATRIMÔNIO	
												48,	--DOAÇÃO MUNÍCIPIO - PATRIMÔNIO
												49,	--DOAÇÃO OUTROS ESTADOS - PATRIMÔNIO
												50,	--DOAÇÃO UNIÃO - PATRIMÔNIO
												51,	--ESTRAVIO, FURTO, ROUBO - PATRIMONIADO
												52,	--MORTE ANIMAL - PATRIMONIADO
												53, --MORTE VEGETAL - PATRIMONIADO
												54, --MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO
												55, --SEMENTES, PLANTAS, INSUMOS E ÁRVORES
												56,	--TRANSFERÊNCIA OUTRO ÓRGÃO - PATRIMONIADO
												57,	--TRANSFERÊNCIA MESMO ÓRGÃO - PATRIMONIADO
												58,	--PERDAS INVOLUNTÁRIAS - BENS MÓVEIS
												59, --PERDAS INVOLUNTÁRIAS - INSERVÍVEL BENS MÓVEIS
												81, --DEPRECIAÇÃO – BAIXA (EXCLUSIVA CONTABILIZA-SP)
												91, --RECLASSIFICACAO SEM EMPENHO (EXCLUSIVA CONTABILIZA-SP)
												95  --VENDA/LEILÃO - SEMOVENTE
											  ))
	--SE O NUMERO DE MESES DEPRECIADOS FOR IGUAL AO NUMERO DE MESES DE VIDA UTIL, NO MES DE REFERENCIA CONSULTADO O BP ESTARA TOTALMENTE DEPRECIADO.
	/***********************************************************************************************************************************
	--CONCEITUALMENTE, DEVERIA TER FUNCIONADO, MAS TROUXE O QUANTITATIVO DA EXTRACAO EXCEL COM O FILTRO ABAIXO (Tabela Asset)
	AND (	 (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= (CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112)))
		 AND [dadosDepreciacaoMensal].[CurrentMonth] = [dadosDepreciacaoMensal].[LifeCycle]
		)
	************************************************************************************************************************************/
	AND (	(CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
		 AND [bemPatrimonial].[monthUsed] = [bemPatrimonial].[LifeCycle]
		)
	AND (
			--SUBSELECT --01
			movimentacaoBP.ManagerUnitId = @ManagerUnitId
			AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
			AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
			--SUBSELECT --02
			--AND (CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) <= @MonthReference) --PARA TRAZER O ULTIMO MES-REFERENCIADO DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
			AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
		)
	AND (bemPatrimonial.[flagVerificado] IS NULL)        
    AND (bemPatrimonial.[flagDepreciaAcumulada] = 1)
	GROUP BY
				  movimentacaoBP.AuxiliaryAccountId

				--LINHA 'DEBUG' MES-REFERENCIA DEPRECIACAO NA UGE CONSULTADA
				--, [dadosDepreciacaoMensal].[CurrentDate]
				--, CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112)
				, dadosDepreciacaoMensal.MaterialItemCode
				, dadosDepreciacaoMensal.AssetStartId
	ORDER BY 
				  movimentacaoBP.AuxiliaryAccountId
				, dadosDepreciacaoMensal.MaterialItemCode
				, dadosDepreciacaoMensal.AssetStartId
END

GO
/****** Object:  StoredProcedure [dbo].[PRC_RELATORIO__INCORPORACOES_BP__MESREF]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--SEGUNDO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO (AQUISICOES MES-REF)  
CREATE PROCEDURE [dbo].[PRC_RELATORIO__INCORPORACOES_BP__MESREF]
(  
    @MonthReference VARCHAR(6)  
  , @ManagerUnitId INT  
) AS  
BEGIN

	Declare @mes int = CONVERT(int,SUBSTRING(@MonthReference,5,2));
	Declare @ano int = CONVERT(int,SUBSTRING(@MonthReference,1,4));
	
	select 
		busca.[TipoIncorporacao],
		busca.ContaContabilDepreciacao,
		(select top 1 [Description] from DepreciationAccount
		where Code = busca.ContaContabilDepreciacao) 'ContaContabilDepreciacaoDescricao',
		busca.ContaContabil,
		(select top 1 [Description] from AuxiliaryAccount
		where Id = busca.IdContaContabil) 'ContaContabilDescricao',
		SUM(busca.ValorAquisicao) 'ValorAquisicao',
		SUM(busca.DepreciacaoNoMes) 'DepreciacaoNoMes',
		SUM(busca.ValorAtual) 'ValorAtual'
	
	from (

	select 
	tipoIncorporacao.[Description] 'TipoIncorporacao',
	contaDepreciacao.[Code] 'ContaContabilDepreciacao',
	contaDepreciacao.[Description] 'ContaContabilDepreciacaoDescricao',
	contacontabil.ContaContabilApresentacao 'ContaContabil',
	contacontabil.[Description] 'ContaContabilDescricao',
	bemPatrimonial.ValueAcquisition 'ValorAquisicao',
	CASE 
	WHEN [bemPatrimonial].[flagAcervo] = 1 THEN  0  
    WHEN [bemPatrimonial].[flagTerceiro] = 1 THEN 0
	ELSE (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[RateDepreciationMonthly],0) 'ValorMensal'
				  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
				  WHERE 
						ManagerUnitId = movimentacaoBP.[ManagerUnitId]
				  AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
				  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId] 
				  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT))
				  --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
				 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
	      )
	END AS 'DepreciacaoNoMes',
	CASE 
		WHEN [bemPatrimonial].[flagAcervo] = 1 THEN ISNULL([bemPatrimonial].[ValueUpdate],[bemPatrimonial].[ValueAcquisition])
		WHEN [bemPatrimonial].[flagTerceiro] = 1 THEN ISNULL([bemPatrimonial].[ValueUpdate],[bemPatrimonial].[ValueAcquisition])
		ELSE (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[CurrentValue],0) 'ValorAcumulado'
					  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
					  WHERE 
							ManagerUnitId = movimentacaoBP.[ManagerUnitId]
							AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
					  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId]
					  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT)) 
					  --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
					 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
		)
	END AS 'ValorAtual',
	--valores para agrupamento e ordenação
	tipoIncorporacao.Id 'IdIncorporacao',
	contacontabil.Id 'IdContaContabil',
	movimentacaoBP.MovimentDate 'DataIncorporacao',
	bemPatrimonial.MaterialItemCode 'CodigoItemMaterial',
	bemPatrimonial.AssetStartId 'IdInicialDaDepreciacao'
	from Asset bemPatrimonial
	inner join AssetMovements movimentacaoBP WITH(NOLOCK)
	on bemPatrimonial.Id = movimentacaoBP.AssetId
	inner join MovementType tipoIncorporacao
	on movimentacaoBP.MovementTypeId = tipoIncorporacao.Id
	left join AuxiliaryAccount contacontabil 
	on movimentacaoBP.AuxiliaryAccountId = contacontabil.Id
	inner join DepreciationAccount contaDepreciacao
	on contacontabil.DepreciationAccountId = contaDepreciacao.Id
	where  bemPatrimonial.flagDepreciaAcumulada = 1
	   and bemPatrimonial.flagVerificado is null
	   and tipoIncorporacao.GroupMovimentId = 1 --o grupo das incorprações
	   and movimentacaoBP.ManagerUnitId = @ManagerUnitId 
	   and (movimentacaoBP.FlagEstorno is null OR movimentacaoBP.FlagEstorno = 0)
	   and MONTH(movimentacaoBP.MovimentDate) = @mes
	   and YEAR(movimentacaoBP.MovimentDate) = @ano
	) [busca]
	GROUP BY  
	   [busca].TipoIncorporacao
     , [busca].ContaContabilDepreciacao
	 , [busca].IdContaContabil
	 , [busca].ContaContabil
     , CONVERT(CHAR(6), [busca].[DataIncorporacao], 112)
     , [busca].CodigoItemMaterial
     , [busca].IdInicialDaDepreciacao
 ORDER BY   
       [busca].TipoIncorporacao
     , [busca].ContaContabilDepreciacao
	 , [busca].IdContaContabil
     , [busca].CodigoItemMaterial
     , [busca].IdInicialDaDepreciacao
END

GO
/****** Object:  StoredProcedure [dbo].[PRC_RELATORIO__INVENTARIO_CONTAS_CONTABEIS]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--PRIMEIRO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO
CREATE PROCEDURE [dbo].[PRC_RELATORIO__INVENTARIO_CONTAS_CONTABEIS]
(
	   @MonthReference VARCHAR(6)
	 , @ManagerUnitId INT
) AS
DECLARE @AssetIdsEstornados TABLE(
	AssetId INT,
	AuxiliaryAccountId INT
)

BEGIN
	--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
	--BEGIN CODE --
	INSERT INTO @AssetIdsEstornados(AssetId,AuxiliaryAccountId)  
	SELECT [mov].[AssetId],[mov].[AuxiliaryAccountId] FROM [AssetMovements] [mov] WITH(NOLOCK)
			WHERE [mov].[ManagerUnitId] = @ManagerUnitId 
			  AND [mov].[MovementTypeId] = 5 --- Inventário Inicial
			  AND ([mov].[FlagEstorno] IS NOT NULL ) AND ([mov].[DataEstorno] IS NOT NULL)
		GROUP BY [mov].[AssetId],[mov].[AuxiliaryAccountId]
	--END CODE --
	SELECT 
			  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]									AS 'ContaContabilDepreciacao'				/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]						AS 'ContaContabilDepreciacaoDescricao'		/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoStatus]							AS 'ContaContabilDepreciacaoStatus'
			, (select top 1 ContaContabilApresentacao from AuxiliaryAccount where BookAccount = [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]
																			AND Description like [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]
																			AND Status = [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]
			   )																						AS 'ContaContabil'							/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]									AS 'ContaContabilDescricao'					/*CAMPO RELATORIO*/
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]										AS 'ContaContabilStatus'
			, SUM([visaoGeral_UGE__MESREF_ATUAL].[ValorContabil])										AS 'ValorContabil'							/*CAMPO RELATORIO*/
			--, SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoAcumuladaAteMesAnterior])				AS 'DepreciacaoAcumuladaAteMesAnterior'		/*CAMPO RELATORIO*/
			, ISNULL(SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoNoMes]),0)							AS 'DepreciacaoNoMes'						/*CAMPO RELATORIO*/
			, ISNULL(SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoAcumulada]),0)						AS 'DepreciacaoAcumulada'					/*CAMPO RELATORIO*/
	FROM
		(
			SELECT  
					 --(SELECT sigla.Name FROM Initial sigla WITH(NOLOCK) WHERE  bemPatrimonial.InitialId = sigla.Id)																														AS 'Sigla',
					 --(SELECT CONVERT(INT,bemPatrimonial.NumberIdentification) FROM Asset bemPatrimonial WITH(NOLOCK) WHERE bemPatrimonial.Id = movimentacaoBP.AssetId)																	AS 'Chapa',
					 (SELECT [contaContabilDepreciacao].[Code]			FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId 
																																											FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																											WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacao'
					, (SELECT [contaContabilDepreciacao].[Description]	FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId
																																											FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																											WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacaoDescricao'
					, (SELECT [contaContabilDepreciacao].[Status]		FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) WHERE contaContabilDepreciacao.Id = (SELECT DepreciationAccountId
																																											FROM AuxiliaryAccount contaContabil WITH(NOLOCK) 
																																											WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId))	AS 'ContaContabilDepreciacaoStatus'
																																											
					, (SELECT [contaContabil].[BookAccount] FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																	AS 'ContaContabil'
					, (SELECT [contaContabil].[Description] FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																	AS 'ContaContabilDescricao'
					, (SELECT [contaContabil].[Status]		FROM AuxiliaryAccount contaContabil WITH(NOLOCK) WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)																	AS 'ContaContabilStatus'

					, CASE 
							WHEN movimentacaoBP.AuxiliaryAccountId = 212508 
							THEN  (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[CurrentValue],0) 'ValorMensal'			
										  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
										  WHERE 
												ManagerUnitId = movimentacaoBP.[ManagerUnitId]
										  AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
										  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId] 
										  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <=
										  (select top 1 (CAST(CONVERT(CHAR(6), MovimentDate, 112) AS INT)) from AssetMovements with(nolock) 
										  where AssetId = bemPatrimonial.Id AND 
											    FlagEstorno IS NULL AND
												MovementTypeId = 19))
										 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
									)
							ELSE (SELECT TOP 1 ISNULL([bemPatrimonial].[ValueAcquisition], 0.00)) 
					END AS 'ValorContabil'
					, 
					CASE 
					WHEN movimentacaoBP.AuxiliaryAccountId = 212508 --Conta de inservível
					THEN  0
					ELSE
						(SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[RateDepreciationMonthly],0) 'ValorMensal'			
									  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
									  WHERE 
											ManagerUnitId = movimentacaoBP.[ManagerUnitId]
									  AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
									  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId] 
									  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) = CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
									 ORDER BY [dadosDepreciacaoMensal].[Id] DESC
						) 
					END AS 'DepreciacaoNoMes'
					, 
					CASE 
					WHEN movimentacaoBP.AuxiliaryAccountId = 212508 --Conta de inservível
					THEN  0
					ELSE
					(SELECT TOP 1 [dadosDepreciacaoMensal].[AccumulatedDepreciation] 'ValorAcumulado'			
								  FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
								  WHERE 
										ManagerUnitId = movimentacaoBP.[ManagerUnitId]
										AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]
								  AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId]
								  AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO
								 ORDER BY [dadosDepreciacaoMensal].[Id] DESC

						) 
					END AS 'DepreciacaoAcumulada'
					--LINHA 'DEBUG' MES-REFERENCIA MOVIMENTACAO NA UGE CONSULTADA
					, CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112)																																						AS 'MesReferenciaMovimentacao'
			FROM 
				AssetMovements movimentacaoBP WITH(NOLOCK)
			INNER JOIN Asset bemPatrimonial WITH(NOLOCK) ON movimentacaoBP.AssetId = bemPatrimonial.Id
			WHERE (bemPatrimonial.flagAcervo IS NULL OR bemPatrimonial.flagAcervo = 0) --RETIRANDO ACERVOS POIS OS MESMOS NAO DEPRECIAM
			  AND (bemPatrimonial.flagTerceiro IS NULL OR bemPatrimonial.flagTerceiro = 0) --RETIRANDO BENS DE TERCEIRO POIS OS MESMOS SERAO CONTROLADOS APENAS FISICAMENTE, NAO CONTABILMENTE
		      AND (bemPatrimonial.[flagVerificado] IS NULL)        
		      AND (bemPatrimonial.[flagDepreciaAcumulada] = 1)
			  AND movimentacaoBP.Id IN (SELECT MAX(Id)
									  FROM 
										AssetMovements movimentacaoBP WITH(NOLOCK)
									  WHERE ManagerUnitId = @ManagerUnitId
										AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
										--TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'
										--BEGIN CODE --
										AND (movimentacaoBP.AssetId NOT IN(SELECT [AssetId] FROM @AssetIdsEstornados WHERE [AuxiliaryAccountId] = movimentacaoBP.AuxiliaryAccountId)) --
										AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
										--END CODE --
									  GROUP BY AssetId
									 )
			AND (movimentacaoBP.ManagerUnitId = @ManagerUnitId)
			--AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
			
			AND (movimentacaoBP.MovementTypeId NOT IN (
															11,	--Transfêrencia
															12,	--Doação
															15,	--Extravio
															16,	--Obsoleto
															17,	--Danificado
															18,	--Sucata
															23, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)  
															24, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)  
															27, --COMODATOS/DE TERCEIROS RECEBIDOS
															42,	--SAÍDA INSERVÍVEL DA UGE - DOAÇÃO																		     
															43,	--SAÍDA INSERVÍVEL DA UGE - TRANSFERÊNCIA																		     
															44,	--COMODATO - CONCEDIDOS - PATRIMONIADO																		     
															45,	--COMODATO/DE TERCEIROS - RECEBIDOS																		     
															46,	--DOAÇÃO CONSOLIDAÇÃO - PATRIMÔNIO																		     
															47,	--DOAÇÃO INTRA - NO - ESTADO - PATRIMÔNIO																	     
															48,	--DOAÇÃO MUNÍCIPIO - PATRIMÔNIO
															49,	--DOAÇÃO OUTROS ESTADOS - PATRIMÔNIO
															50,	--DOAÇÃO UNIÃO - PATRIMÔNIO
															51,	--ESTRAVIO, FURTO, ROUBO - PATRIMONIADO
															52,	--MORTE ANIMAL - PATRIMONIADO
															53,	--MORTE VEGETAL - PATRIMONIADO
															54,	--MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO
															55,	--SEMENTES, PLANTAS, INSUMOS E ÁRVORES
															56,	--TRANSFERÊNCIA OUTRO ÓRGÃO - PATRIMONIADO
															57,	--TRANSFERÊNCIA MESMO ÓRGÃO - PATRIMONIADO
															58,	--PERDAS INVOLUNTÁRIAS - BENS MÓVEIS
															59,	--PERDAS INVOLUNTÁRIAS - INSERVÍVEL BENS MÓVEIS
															81,
															91,
															95))
		AND (
				movimentacaoBP.ManagerUnitId = @ManagerUnitId
				AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
				AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAOES
			)
		) AS visaoGeral_UGE__MESREF_ATUAL
	GROUP BY
			  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoStatus]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilStatus]
	ORDER BY 
			  [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]
			, [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]
END

GO
/****** Object:  StoredProcedure [dbo].[PRC_RELATORIO__INVENTARIO_FISICO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--RELATORIO DE INVENTARIO FISICO
CREATE PROCEDURE [dbo].[PRC_RELATORIO__INVENTARIO_FISICO]
(
	  @GroupingType			VARCHAR(1) = NULL
	, @MonthReference		VARCHAR(6) = NULL
	, @InstitutionId		INT = NULL
	, @BudgetUnitId			INT = NULL
	, @ManagerUnitId		INT = NULL
	, @AdministrativeUnitId	INT = NULL
	, @SectionId         	INT = NULL
) AS


DECLARE @DataAnoMes AS DATETIME;
DECLARE @Ano AS INT;
DECLARE @Mes AS INT;
DECLARE @Dia AS INT = 1;


SET DATEFORMAT YMD;

BEGIN

	SELECT 
			  CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) AS 'MesReferencia'
			, CASE @GroupingType
									WHEN '1' THEN (SELECT TOP 1 Convert(varchar,[Code]) 
												   FROM [MaterialGroup] [grupoMaterial] WITH(NOLOCK)
												   WHERE [grupoMaterial].[Code] = [bemPatrimonial].[MaterialGroupCode])--m.Code
									WHEN '2' THEN (SELECT TOP 1 [contaContabil].[ContaContabilApresentacao] 
												   FROM [AuxiliaryAccount] [contaContabil] WITH(NOLOCK) 
												   WHERE [movimentacaoBP].[AuxiliaryAccountId] = [contaContabil].[Id])--aux.BookAccount
			  END																																																							AS 'Agrupamento'

			, CASE @GroupingType
									WHEN '1' THEN (SELECT TOP 1 [grupoMaterial].[Description] 
												   FROM [MaterialGroup] [grupoMaterial] WITH(NOLOCK)
												   WHERE [grupoMaterial].[Code] = [bemPatrimonial].[MaterialGroupCode])--m.Code
									WHEN '2' THEN (SELECT TOP 1 [contaContabil].[Description]
												   FROM [AuxiliaryAccount] [contaContabil] WITH(NOLOCK)
												   WHERE [movimentacaoBP].[AuxiliaryAccountId] = [contaContabil].[Id])--aux.BookAccount
			  END																																																							AS 'DescAgrupamento'

			, (SELECT [contaContabil].[Status] FROM [AuxiliaryAccount] [contaContabil] WITH(NOLOCK) WHERE [movimentacaoBP].[AuxiliaryAccountId] = [contaContabil].[Id])																		AS 'STATUS_CONTA'
			, (SELECT (CONVERT(VARCHAR,[orgaoSIAFEM].[Code]) + ' - ' + [orgaoSIAFEM].[Description])	FROM [Institution] [orgaoSIAFEM]  WITH(NOLOCK)		WHERE [movimentacaoBP].[InstitutionId]			= [orgaoSIAFEM].[Id])				AS 'ORGAO'
			, (SELECT (CONVERT(VARCHAR,[uoSIAFEM].[Code])	+ ' - ' + [uoSIAFEM].[Description])		FROM [BudgetUnit] [uoSIAFEM]  WITH(NOLOCK)			WHERE [movimentacaoBP].[BudgetUnitId]			= [uoSIAFEM].[Id])					AS 'UO'
			, (SELECT (CONVERT(VARCHAR,[ugeSIAFEM].[Code])	+ ' - ' + [ugeSIAFEM].[Description])	FROM [ManagerUnit] [ugeSIAFEM]  WITH(NOLOCK)		WHERE [movimentacaoBP].[ManagerUnitId]			= [ugeSIAFEM].[Id])					AS 'UGE'
			, (SELECT (CONVERT(VARCHAR,[uaSIAFEM].[Code])	+ ' - ' + [uaSIAFEM].[Description])		FROM [AdministrativeUnit] [uaSIAFEM]  WITH(NOLOCK)	WHERE [movimentacaoBP].[AdministrativeUnitId]	= [uaSIAFEM].[Id])					AS 'UA'
			, (SELECT (CONVERT(VARCHAR,[divisaoUA].[Code])	+ ' - ' + [divisaoUA].[Description])	FROM [Section] [divisaoUA]  WITH(NOLOCK)			WHERE [movimentacaoBP].[SectionId]				= [divisaoUA].[Id])					AS 'DIVISAO'

			, (SELECT [responsavelBP].[Name] FROM [Responsible] [responsavelBP] WITH(NOLOCK) WHERE [movimentacaoBP].[ResponsibleId] = [responsavelBP].[Id])																					AS 'RESPONSAVEL'

			, (SELECT (ISNULL([siglaBP].[Name],'') +'-'+ [bemPatrimonial].[ChapaCompleta]) 
			   FROM [Initial] [siglaBP] where [bemPatrimonial].[InitialId] = [siglaBP].[Id])																																							AS 'CHAPA'
			
			, (SELECT TOP 1 [descricaoResumidaItemMaterial].[Description]
			   FROM [ShortDescriptionItem] [descricaoResumidaItemMaterial] WITH(NOLOCK)
			   WHERE [descricaoResumidaItemMaterial].[Id] = [bemPatrimonial].[ShortDescriptionItemId])																																					AS 'MATERIAL'
			, 
			[bemPatrimonial].[ValueAcquisition]																																																				AS 'VALOR_AQUISICAO'
			, 
			CASE 
				WHEN [bemPatrimonial].[flagAcervo] = 1 THEN ISNULL([bemPatrimonial].[ValueUpdate],[bemPatrimonial].[ValueAcquisition])
				WHEN [bemPatrimonial].[flagTerceiro] = 1 THEN ISNULL([bemPatrimonial].[ValueUpdate],[bemPatrimonial].[ValueAcquisition])
				WHEN [bemPatrimonial].[flagDecreto] = 1 
				THEN 
					ISNULL((SELECT MIN([dadosDepreciacaoMensal].[CurrentValue])
					  FROM [dbo].[MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
					  WHERE [dadosDepreciacaoMensal].[ManagerUnitId] = [bemPatrimonial].[ManagerUnitId] 
						AND [dadosDepreciacaoMensal].[AssetStartId] = [bemPatrimonial].[AssetStartId] 
						AND [dadosDepreciacaoMensal].[MaterialItemCode] = [bemPatrimonial].[MaterialItemCode] 
						AND Convert(int,Convert(varchar(6),[dadosDepreciacaoMensal].[CurrentDate],112)) <= @MonthReference
					  GROUP BY [dadosDepreciacaoMensal].[MaterialItemCode], [dadosDepreciacaoMensal].[AssetStartId]
					), [bemPatrimonial].[ValueUpdate])
				ELSE
					ISNULL((SELECT MIN([dadosDepreciacaoMensal].[CurrentValue])
					  FROM [dbo].[MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK) 
					  WHERE [dadosDepreciacaoMensal].[ManagerUnitId] = [bemPatrimonial].[ManagerUnitId] 
						AND [dadosDepreciacaoMensal].[AssetStartId] = [bemPatrimonial].[AssetStartId] 
						AND [dadosDepreciacaoMensal].[MaterialItemCode] = [bemPatrimonial].[MaterialItemCode] 
						AND Convert(int,Convert(varchar(6),[dadosDepreciacaoMensal].[CurrentDate],112)) <= @MonthReference
					  GROUP BY [dadosDepreciacaoMensal].[MaterialItemCode], [dadosDepreciacaoMensal].[AssetStartId]
					), [bemPatrimonial].[ValueAcquisition]) END																																																	AS 'VALOR_ATUAL'
	FROM AssetMovements movimentacaoBP WITH(NOLOCK)
	INNER JOIN Asset [bemPatrimonial] WITH(NOLOCK) ON movimentacaoBP.AssetId = [bemPatrimonial].Id
	WHERE
			movimentacaoBP.Id IN (
									SELECT MAX(Id)
									FROM AssetMovements movimentacaoBP WITH(NOLOCK)
									WHERE 
										ManagerUnitId = @ManagerUnitId
										AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
										AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))
									GROUP BY AssetId
								 )
	AND (movimentacaoBP.FlagEstorno = 0 OR movimentacaoBP.FlagEstorno IS NULL)	--RETIRANDO OS ESTORNOS DE MOVIMENTACOES
	AND (movimentacaoBP.MovementTypeId NOT IN (
													11, --Transfêrencia
													12, --Doação
													15, --Extravio
													16, --Obsoleto
													17, --Danificado
													18,	--Sucata
													19, --INSERVÍVEL NA UGE - BENS MÓVEIS
													23, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES) 
													24, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES) 
													42,	--SAÍDA INSERVÍVEL DA UGE - DOAÇÃO			
													43,	--SAÍDA INSERVÍVEL DA UGE - TRANSFERÊNCIA	
													44,	--COMODATO - CONCEDIDOS - PATRIMONIADO		
													45,	--COMODATO/DE TERCEIROS - RECEBIDOS			
													46,	--DOAÇÃO CONSOLIDAÇÃO - PATRIMÔNIO			
													47,	--DOAÇÃO INTRA - NO - ESTADO - PATRIMÔNIO	
													48,	--DOAÇÃO MUNÍCIPIO - PATRIMÔNIO
													49,	--DOAÇÃO OUTROS ESTADOS - PATRIMÔNIO
													50,	--DOAÇÃO UNIÃO - PATRIMÔNIO
													51,	--ESTRAVIO, FURTO, ROUBO - PATRIMONIADO
													52,	--MORTE ANIMAL - PATRIMONIADO
													53, --MORTE VEGETAL - PATRIMONIADO
													54, --MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO
													55, --SEMENTES, PLANTAS, INSUMOS E ÁRVORES
													56,	--TRANSFERÊNCIA OUTRO ÓRGÃO - PATRIMONIADO
													57,	--TRANSFERÊNCIA MESMO ÓRGÃO - PATRIMONIADO
													58,	--PERDAS INVOLUNTÁRIAS - BENS MÓVEIS
													59, --PERDAS INVOLUNTÁRIAS - INSERVÍVEL BENS MÓVEIS
													81, --DEPRECIAÇÃO – BAIXA (EXCLUSIVA CONTABILIZA-SP)
													91, --RECLASSIFICACAO SEM EMPENHO (EXCLUSIVA CONTABILIZA-SP)
													95  --VENDA/LEILÃO - SEMOVENTES
												))
	AND	(	 (movimentacaoBP.InstitutionId		  = @InstitutionId OR @InstitutionId IS NULL)
		 AND (movimentacaoBP.BudgetUnitId		  = @BudgetUnitId OR @BudgetUnitId IS NULL)
		 AND (movimentacaoBP.ManagerUnitId		  = @ManagerUnitId OR @ManagerUnitId IS NULL)
		 AND (movimentacaoBP.AdministrativeUnitId = @AdministrativeUnitId OR @AdministrativeUnitId IS NULL)
		 AND (movimentacaoBP.SectionId			  = @SectionId OR @SectionId IS NULL)
		)
	AND (
		 movimentacaoBP.ManagerUnitId = @ManagerUnitId
		 AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)
		 AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL))	--RETIRANDO ESTORNOS DE MOVIMENTACAO
		)
	AND [bemPatrimonial].[flagVerificado] IS NULL 
	and [bemPatrimonial].[flagDepreciaAcumulada] = 1
	ORDER BY
			  movimentacaoBP.InstitutionId		 
			, movimentacaoBP.BudgetUnitId ASC		 
			, movimentacaoBP.ManagerUnitId ASC		 
			, movimentacaoBP.AdministrativeUnitId ASC
			, movimentacaoBP.SectionId ASC
			, movimentacaoBP.ResponsibleId ASC
END

GO
/****** Object:  StoredProcedure [dbo].[PRC_RELATORIO__SALDO_CONTABIL_ORGAO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Cópia do primeiro quadrante do RELATORIO RESUMO CONSOLIDADO, a 'nível' de Orgao    
--Conforme pedido em 18/02/2020, deverá trazer os acervos e os terceiros    
CREATE PROCEDURE [dbo].[PRC_RELATORIO__SALDO_CONTABIL_ORGAO]
(    
   @InstitutionId INT,    
   @MonthReference VARCHAR(6) = NULL    
) AS    
    
DECLARE @MesRefFormatado VARCHAR(7);  
 
Declare @UGEsComMesFechado TABLE(    
 Id int,    
 MesRef [varchar](6)    
);  
    
BEGIN    
 SET DATEFORMAT YMD    
  
Declare @IntegradoAoSIAFEM bit;  
  
select @IntegradoAoSIAFEM = flagIntegracaoSiafem   
from Institution where id = @InstitutionId;  
  
IF(@IntegradoAoSIAFEM = 1)  
BEGIN  
 Declare @UGEDadosParaMostarNoExcel as table(  
  Descricao [varchar](120),  
  Codigo [varchar](6),  
  IdDaUO int,  
  MesRef [varchar](6)  
 );  
  
 IF(@MonthReference is null)    
  BEGIN      
   --PEGANDO O MAIOR MES-REFERENCIA DAS UGEs DO ORGAO    
   SET @MonthReference =  (SELECT MAX(ManagerUnit.ManagmentUnit_YearMonthReference)     
         FROM ManagerUnit     
         WHERE BudgetUnitId IN (SELECT Id     
               FROM BudgetUnit    
               WHERE InstitutionId = @InstitutionId))     
     
   insert into @UGEDadosParaMostarNoExcel(Descricao, Codigo, IdDaUO, MesRef)    
   SELECT [Description], Code, BudgetUnitId, ManagmentUnit_YearMonthReference  
   FROM ManagerUnit     
   WHERE BudgetUnitId IN (SELECT Id FROM BudgetUnit WHERE InstitutionId = @InstitutionId)    
   AND ManagmentUnit_YearMonthReference = @MonthReference;    
  END    
  ELSE    
  BEGIN    
   insert into @UGEDadosParaMostarNoExcel(Descricao, Codigo, IdDaUO, MesRef)    
   SELECT [Description],Code, BudgetUnitId, ManagmentUnit_YearMonthReference  
   FROM ManagerUnit     
   WHERE BudgetUnitId IN (SELECT Id FROM BudgetUnit WHERE InstitutionId = @InstitutionId)  
   AND convert(int,ManagmentUnit_YearMonthReference) > convert(int,@MonthReference)  
  END    
  
  SET @MesRefFormatado = (substring(@MonthReference,5,2) + '/' + substring(@MonthReference,1,4))  
  
  select * from (  
  select  
  (SELECT [orgaoSIAFEM].[Code] FROM Institution orgaoSIAFEM WITH(NOLOCK)  WHERE [orgaoSIAFEM].[Id] = @InstitutionId) AS 'Orgao',  
  (SELECT [UO].[Code] FROM BudgetUnit UO WITH(NOLOCK)  WHERE [UO].[Id] = temp.IdDaUO) AS 'UO',  
  temp.Codigo AS 'UGE',   
  temp.Descricao as 'NomeUGE'  ,  
  @MesRefFormatado 'MesRef',   
  a.DepreciationAccount 'ContaContabilDepreciacao',  
  a.DepreciationDescription 'ContaContabilDepreciacaoDescricao',  
  a.[Status] 'ContaContabilDepreciacaoStatus',  
  CONVERT(varchar,d.BookAccount) 'ContaContabil',  
  d.AccountingDescription 'ContaContabilDescricao',  
  d.[Status] 'ContaContabilStatus',  
  d.AccountingValue 'ValorContabil',  
  d.DepreciationMonth 'DepreciacaoNoMes',  
  d.AccumulatedDepreciation 'DepreciacaoAcumulada'  
  from AccountingClosing a  
  inner join DepreciationAccountingClosing d  
  on a.Id = d.AccountingClosingId  
  inner join @UGEDadosParaMostarNoExcel temp  
  on d.ManagerUnitCode = temp.Codigo  
  where d.ReferenceMonth = @MonthReference  
 ) [dados]   
 order by  
    [dados].[Orgao]     
    , [dados].[UO]      
    , [dados].[UGE]      
    , [dados].[NomeUGE]    
    , [dados].[MesRef]    
    , [dados].[ContaContabilDepreciacao]    
    , [dados].[ContaContabil]  
  
END  
ELSE  
BEGIN  
    
  IF(@MonthReference is null)    
  BEGIN      
   --PEGANDO O MAIOR MES-REFERENCIA DAS UGEs DO ORGAO    
   SET @MonthReference =  (SELECT MAX(ManagerUnit.ManagmentUnit_YearMonthReference)
         FROM ManagerUnit     
         WHERE BudgetUnitId IN (SELECT Id     
               FROM BudgetUnit    
               WHERE InstitutionId = @InstitutionId))     
     
   insert into @UGEsComMesFechado(Id,MesRef)    
   SELECT Id, ManagmentUnit_YearMonthReference     
   FROM ManagerUnit     
   WHERE BudgetUnitId IN (SELECT Id FROM BudgetUnit WHERE InstitutionId = @InstitutionId)    
   AND ManagmentUnit_YearMonthReference = @MonthReference;    
  END    
  ELSE    
  BEGIN    
   insert into @UGEsComMesFechado(Id,MesRef)    
   SELECT Id, ManagmentUnit_YearMonthReference     
   FROM ManagerUnit     
   WHERE BudgetUnitId IN (SELECT Id FROM BudgetUnit WHERE InstitutionId = @InstitutionId)    
   AND convert(int,ManagmentUnit_YearMonthReference) > convert(int,@MonthReference)    
  END    
       
 SET @MesRefFormatado = (substring(@MonthReference,5,2) + '/' + substring(@MonthReference,1,4))  
     
  SELECT     
      CONVERT(VARCHAR(2), [visaoGeral_UGE__MESREF_ATUAL].[Orgao])             AS 'Orgao'         /*CAMPO RELATORIO*/    
    , CONVERT(VARCHAR(5), [visaoGeral_UGE__MESREF_ATUAL].[UO])              AS 'UO'          /*CAMPO RELATORIO*/    
    , CONVERT(VARCHAR(6), [visaoGeral_UGE__MESREF_ATUAL].[UGE])              AS 'UGE'         /*CAMPO RELATORIO*/    
    , [visaoGeral_UGE__MESREF_ATUAL].[NomeUGE]                  AS 'NomeUGE'        /*CAMPO RELATORIO*/    
    , [visaoGeral_UGE__MESREF_ATUAL].[MesRef]                  AS 'MesRef'       /*CAMPO RELATORIO*/    
    , CONVERT(VARCHAR(10), [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao])        AS 'ContaContabilDepreciacao'    /*CAMPO RELATORIO*/    
    , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]           AS 'ContaContabilDepreciacaoDescricao'  /*CAMPO RELATORIO*/    
    , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]                                AS 'ContaContabil'       /*CAMPO RELATORIO*/    
    , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]              AS 'ContaContabilDescricao'     /*CAMPO RELATORIO*/    
    , SUM([visaoGeral_UGE__MESREF_ATUAL].[ValorContabil])               AS 'ValorContabil'       /*CAMPO RELATORIO*/    
    , ISNULL(SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoNoMes]),0.0)           AS 'DepreciacaoNoMes'      /*CAMPO RELATORIO*/    
    , ISNULL(SUM([visaoGeral_UGE__MESREF_ATUAL].[DepreciacaoAcumulada]),0.0)          AS 'DepreciacaoAcumulada'     /*CAMPO RELATORIO*/    
  FROM    
   (    
    SELECT      
        (SELECT [orgaoSIAFEM].[Code] FROM Institution orgaoSIAFEM WITH(NOLOCK)  WHERE [orgaoSIAFEM].[Id] = movimentacaoBP.InstitutionId)                    AS 'Orgao'    
      , (SELECT [uoSIAFEM].[Code]  FROM BudgetUnit uoSIAFEM WITH(NOLOCK)   WHERE [uoSIAFEM].[Id] = movimentacaoBP.BudgetUnitId)                     AS 'UO'    
      , (SELECT [ugeSIAFEM].[Code] FROM ManagerUnit ugeSIAFEM WITH(NOLOCK)   WHERE [ugeSIAFEM].[Id] = movimentacaoBP.ManagerUnitId)                     AS 'UGE'    
      , (SELECT [ugeSIAFEM].[Description] FROM ManagerUnit ugeSIAFEM WITH(NOLOCK)   WHERE [ugeSIAFEM].[Id] = movimentacaoBP.ManagerUnitId)                    AS 'NomeUGE'    
      , @MesRefFormatado 'MesRef'    
      , (SELECT [contaContabilDepreciacao].[Id] FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK)     
      INNER JOIN AuxiliaryAccount contaContabil WITH(NOLOCK) ON contaContabil.DepreciationAccountId = contaContabilDepreciacao.Id    
      WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId    
      ) AS 'ContaContabilDepreciacaoId'    
     
      , (SELECT [contaContabilDepreciacao].[Code]       
           FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK)     
        INNER JOIN AuxiliaryAccount contaContabil WITH(NOLOCK)  ON contaContabil.DepreciationAccountId = contaContabilDepreciacao.Id    
        WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId    
        ) AS 'ContaContabilDepreciacao'    
     
      , (SELECT [contaContabilDepreciacao].[Description]     
           FROM DepreciationAccount contaContabilDepreciacao WITH(NOLOCK)     
        INNER JOIN AuxiliaryAccount contaContabil WITH(NOLOCK)  ON contaContabil.DepreciationAccountId = contaContabilDepreciacao.Id    
        WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId    
      ) AS 'ContaContabilDepreciacaoDescricao'    
     
      , (SELECT [contaContabil].[Id] FROM AuxiliaryAccount contaContabil WITH(NOLOCK)     
          WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)                    AS 'ContaContabilId'                                               
      , (SELECT [contaContabil].[ContaContabilApresentacao] FROM AuxiliaryAccount contaContabil WITH(NOLOCK)     
          WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)                 AS 'ContaContabil'    
      , (SELECT [contaContabil].[Description] FROM AuxiliaryAccount contaContabil WITH(NOLOCK)     
          WHERE contaContabil.Id = movimentacaoBP.AuxiliaryAccountId)                 AS 'ContaContabilDescricao'    
      , CASE   
   WHEN bemPatrimonial.flagAcervo = 1 THEN (SELECT TOP 1 ISNULL([bemPatrimonial].[ValueAcquisition], 0.00))  
   WHEN bemPatrimonial.flagTerceiro = 1 THEN (SELECT TOP 1 ISNULL([bemPatrimonial].[ValueAcquisition], 0.00))  
   WHEN movimentacaoBP.AuxiliaryAccountId = 212508   
   THEN  (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[CurrentValue],0) 'ValorMensal'     
        FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK)   
        WHERE   
        ManagerUnitId = movimentacaoBP.[ManagerUnitId]  
        AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]  
        AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId]   
        AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <=  
        (select top 1 (CAST(CONVERT(CHAR(6), MovimentDate, 112) AS INT)) from AssetMovements with(nolock)   
        where AssetId = bemPatrimonial.Id AND   
           FlagEstorno IS NULL AND  
        MovementTypeId = 19))  
       ORDER BY [dadosDepreciacaoMensal].[Id] DESC  
     )  
   ELSE (SELECT TOP 1 ISNULL([bemPatrimonial].[ValueAcquisition], 0.00))   
   END AS 'ValorContabil'  
      , CASE     
       WHEN [bemPatrimonial].[flagAcervo] = 1 THEN 0    
       WHEN [bemPatrimonial].[flagTerceiro] = 1 THEN 0    
    WHEN [movimentacaoBP].AuxiliaryAccountId = 212508 THEN 0  
       ELSE    
       (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[RateDepreciationMonthly],0) 'ValorMensal'       
            FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK)     
            WHERE     
            ManagerUnitId = movimentacaoBP.[ManagerUnitId]    
            AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]    
            AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId]     
            AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT))     
            --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO    
           ORDER BY [dadosDepreciacaoMensal].[Id] DESC    
       )    
       END                        AS 'DepreciacaoNoMes'    
      ,    
      CASE     
       WHEN [bemPatrimonial].[flagAcervo] = 1 THEN 0    
       WHEN [bemPatrimonial].[flagTerceiro] = 1 THEN 0    
       WHEN [movimentacaoBP].AuxiliaryAccountId = 212508 THEN 0  
    ELSE     
       (SELECT TOP 1 ISNULL([dadosDepreciacaoMensal].[AccumulatedDepreciation],0) 'ValorAcumulado'       
            FROM [MonthlyDepreciation] [dadosDepreciacaoMensal] WITH(NOLOCK)     
            WHERE     
            ManagerUnitId = movimentacaoBP.[ManagerUnitId]    
            AND [dadosDepreciacaoMensal].[MaterialItemCode] = bemPatrimonial.[MaterialItemCode]    
            AND [bemPatrimonial].[AssetStartId] = [dadosDepreciacaoMensal].[AssetStartId]    
            AND (CAST(CONVERT(CHAR(6), [dadosDepreciacaoMensal].[CurrentDate], 112) AS INT) <= CAST(@MonthReference AS INT)) --PARA TRAZER O ULTIMO MES-REFERENCIA DEPRECIADO, QUE PODE SER MENOR QUE O MES-REFERENCIA CONSULTADO    
           ORDER BY [dadosDepreciacaoMensal].[Id] DESC    
     
       )    
      END                          AS 'DepreciacaoAcumulada'    
      , CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112)                                        AS 'MesReferenciaMovimentacao'    
    FROM     
     AssetMovements movimentacaoBP WITH(NOLOCK)    
    INNER JOIN Asset bemPatrimonial WITH(NOLOCK,INDEX(IDX_ASSET_RELATORIO_CONTAS_CONTABEIS)) ON movimentacaoBP.AssetId = bemPatrimonial.Id    
    WHERE    
         (bemPatrimonial.[flagVerificado] IS NULL)            
         AND (bemPatrimonial.[flagDepreciaAcumulada] = 1)    
      AND  movimentacaoBP.Id IN (SELECT MAX(Id)    
            FROM     
           AssetMovements movimentacaoBP WITH(NOLOCK)    
            WHERE  movimentacaoBP.InstitutionId = @InstitutionId    
              and movimentacaoBP.ManagerUnitId IN (select Id from @UGEsComMesFechado)    
           AND (CONVERT(CHAR(6), [movimentacaoBP].[MovimentDate], 112) <= @MonthReference)    
           --TRATAMENTO ESPECIFICO PARA INCORPORACAO DO TIPO 'INVENTARIO INICIAL'    
           --BEGIN CODE --    
           AND ((movimentacaoBP.FlagEstorno IS NULL OR movimentacaoBP.FlagEstorno = 0) AND (movimentacaoBP.DataEstorno IS NULL)) --RETIRANDO ESTORNOS DE MOVIMENTACAOES    
           --END CODE --    
            GROUP BY AssetId    
           )    
     
    AND (movimentacaoBP.InstitutionId = @InstitutionId)    
    AND (movimentacaoBP.ManagerUnitId IN (select Id from @UGEsComMesFechado))    
    AND (movimentacaoBP.MovementTypeId NOT IN (    
                11, --Transfêrencia    
                12, --Doação    
                15, --Extravio    
                16, --Obsoleto    
                17, --Danificado    
                18, --Sucata    
                23, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)      
                24, --BAIXA POR MOVIMENTACAO DE CONSUMO (DIRETO DA TELA DE BPS PENDENTES)      
                42, --SAÍDA INSERVÍVEL DA UGE - DOAÇÃO                           
                43, --SAÍDA INSERVÍVEL DA UGE - TRANSFERÊNCIA                           
                44, --COMODATO - CONCEDIDOS - PATRIMONIADO                           
                45, --COMODATO/DE TERCEIROS - RECEBIDOS                           
                46, --DOAÇÃO CONSOLIDAÇÃO - PATRIMÔNIO                           
                47, --DOAÇÃO INTRA - NO - ESTADO - PATRIMÔNIO                          
                48, --DOAÇÃO MUNÍCIPIO - PATRIMÔNIO    
                49, --DOAÇÃO OUTROS ESTADOS - PATRIMÔNIO    
                50, --DOAÇÃO UNIÃO - PATRIMÔNIO    
                51, --ESTRAVIO, FURTO, ROUBO - PATRIMONIADO    
                52, --MORTE ANIMAL - PATRIMONIADO    
                53, --MORTE VEGETAL - PATRIMONIADO    
                54, --MUDANÇA DE CATEGORIA / DESVALORIZAÇÃO    
                55, --SEMENTES, PLANTAS, INSUMOS E ÁRVORES    
                56, --TRANSFERÊNCIA OUTRO ÓRGÃO - PATRIMONIADO    
                57, --TRANSFERÊNCIA MESMO ÓRGÃO - PATRIMONIADO    
                58, --PERDAS INVOLUNTÁRIAS - BENS MÓVEIS    
                59, --PERDAS INVOLUNTÁRIAS - INSERVÍVEL BENS MÓVEIS    
                81,    
                91,  
       95))    
   ) AS visaoGeral_UGE__MESREF_ATUAL    
  GROUP BY    
      [visaoGeral_UGE__MESREF_ATUAL].[Orgao]     
    , [visaoGeral_UGE__MESREF_ATUAL].[UO]      
    , [visaoGeral_UGE__MESREF_ATUAL].[UGE]      
    , [visaoGeral_UGE__MESREF_ATUAL].[NomeUGE]    
    , [visaoGeral_UGE__MESREF_ATUAL].[MesRef]    
    , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoId]    
    , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]    
    , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacaoDescricao]    
    , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilId]    
    , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]    
    , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDescricao]    
  ORDER BY     
      [visaoGeral_UGE__MESREF_ATUAL].[Orgao]     
    , [visaoGeral_UGE__MESREF_ATUAL].[UO]      
    , [visaoGeral_UGE__MESREF_ATUAL].[UGE]      
    , [visaoGeral_UGE__MESREF_ATUAL].[NomeUGE]    
    , [visaoGeral_UGE__MESREF_ATUAL].[MesRef]    
    , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabilDepreciacao]    
    , [visaoGeral_UGE__MESREF_ATUAL].[ContaContabil]    
END  
  
END

GO
/****** Object:  StoredProcedure [dbo].[PRC_RELATORIO_RESUMO_INVENTARIO_ACERVOS]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--QUINTO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO (BEM DE ACERVO)
CREATE PROCEDURE [dbo].[PRC_RELATORIO_RESUMO_INVENTARIO_ACERVOS]
(
	   @MonthReference VARCHAR(6)
	 , @ManagerUnitId INT
) AS
BEGIN

Declare @CodigoUGE varchar(6);

select top 1 @CodigoUGE = Code from ManagerUnit
where id = @ManagerUnitId;

select 
CONVERT(varchar,d.BookAccount) + ' - ' + d.AccountingDescription 'ContaContabil',
d.AccountingValue 'ValorAquisicao'
from DepreciationAccountingClosing d
where d.ManagerUnitCode = @CodigoUGE
and d.ReferenceMonth  = @MonthReference
and d.BookAccount in (select ContaContabilApresentacao from AuxiliaryAccount where RelacionadoBP = 1);

END

GO
/****** Object:  StoredProcedure [dbo].[PRC_RELATORIO_RESUMO_INVENTARIO_DADOS_DO_FECHAMENTO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PRC_RELATORIO_RESUMO_INVENTARIO_DADOS_DO_FECHAMENTO]
(
	@ManagerUnitId int,
	@MonthReference varchar(6)
) AS
BEGIN

Declare @CodigoUGE varchar(6);

select top 1 @CodigoUGE = Code from ManagerUnit
where id = @ManagerUnitId;

select 
a.DepreciationAccount 'ContaContabilDepreciacao',
a.DepreciationDescription 'ContaContabilDepreciacaoDescricao',
a.[Status] 'ContaContabilDepreciacaoStatus',
CONVERT(varchar,d.BookAccount) 'ContaContabil',
d.AccountingDescription 'ContaContabilDescricao',
d.[Status] 'ContaContabilStatus',
d.AccountingValue 'ValorContabil',
d.DepreciationMonth 'DepreciacaoNoMes',
d.AccumulatedDepreciation 'DepreciacaoAcumulada'
from AccountingClosing a
inner join DepreciationAccountingClosing d
on a.Id = d.AccountingClosingId
where a.ManagerUnitCode = @CodigoUGE
and a.ReferenceMonth  = @MonthReference
and d.BookAccount not in (select ContaContabilApresentacao from AuxiliaryAccount where RelacionadoBP != 0)
ORDER BY 
a.DepreciationAccount, d.BookAccount

END

GO
/****** Object:  StoredProcedure [dbo].[PRC_RELATORIO_RESUMO_INVENTARIO_TERCEIROS]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--SEXTO QUADRANTE DO RELATORIO RESUMO CONSOLIDADO (BEM DE TERCEIRO)
CREATE PROCEDURE [dbo].[PRC_RELATORIO_RESUMO_INVENTARIO_TERCEIROS]
(
	   @MonthReference VARCHAR(6)
	 , @ManagerUnitId INT
) AS
BEGIN

Declare @CodigoUGE varchar(6);

select top 1 @CodigoUGE = Code from ManagerUnit
where id = @ManagerUnitId;

select 
CONVERT(varchar,d.BookAccount) + ' - ' + d.AccountingDescription 'ContaContabil',
d.AccountingValue 'ValorAquisicao'
from DepreciationAccountingClosing d
where d.ManagerUnitCode = @CodigoUGE
and d.ReferenceMonth  = @MonthReference
and d.BookAccount in (select ContaContabilApresentacao from AuxiliaryAccount where RelacionadoBP = 2);

END

GO
/****** Object:  StoredProcedure [dbo].[PREENCHE_TABELA_DE_NL_ESTORNADAS]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  StoredProcedure [dbo].[REPORT_ANALITICO_DE_BEM_PATRIMONIAL]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[REPORT_ANALITICO_DE_BEM_PATRIMONIAL] --EXEC REPORT_ANALITICO_DE_BEM_PATRIMONIAL @assertsSelecteds='2529406'          
(            
 @idOrgao int = null,              
 @idUo  int = null,              
 @idUge  int = null,              
 @idUa  int = null,              
 @idDivisao int = null,       
 @assertsSelecteds varchar(max)           
)            
AS              
BEGIN              
 SELECT       
  a.Id,        
  (select SUBSTRING(ManagmentUnit_YearMonthReference,5,2) + '/' + SUBSTRING(ManagmentUnit_YearMonthReference,1,4) from ManagerUnit WITH(NOLOCK) where id = a.ManagerUnitId) AS MES_REF,            
  a.InitialName AS SIGLA,           
  a.ChapaCompleta AS CHAPA,          
  CASE    
  WHEN a.[Status] = 1    
   THEN    
    'Ativo'    
   ELSE    
    'Inativo'    
  END AS SITUACAO,          
  Convert(varchar(max),a.MaterialItemCode) AS CODIGO_MATERIAL,          
  LTRIM(RTRIM(a.MaterialItemDescription)) AS ITEM_MATERIAL,            
  LTRIM(RTRIM(short.Description)) AS DESC_RESUMIDA,       
  i.code AS CODE_ORGAO,          
  i.[Description] AS DESC_ORGAO,          
  b.code AS CODE_UO,          
  b.[Description] AS DESC_UO,          
  m.code AS CODE_UGE,          
  m.[Description] as DESC_UGE,          
  Convert(varchar(max),adm.Code) as CODE_UA,          
  adm.[Description] as DESC_UA,          
   Convert(varchar(max),s.Code) as CODE_DIVISAO,          
  s.[Description] as DESC_DIVISAO,          
    (select [name] from OutSourced WITH(NOLOCK) where id = a.OutSourcedId) AS TERCEIRO,           
  Convert(varchar(max),aux.code) AS CODE_CONTA_AUX,      
  aux.ContaContabilApresentacao AS CONTACONTABIL,        
  aux.[Description] DESC_AUXILIAR,     
  aux.[Status] as STATUS_CONTA,       
  (select [Description] from MovementType WITH(NOLOCK) where id = a.MovementTypeId) AS INCORPORADO,          
  (select [name] from Supplier WITH(NOLOCK) where id = a.SupplierId) AS FORNECEDOR,          
  a.NumberDoc AS NUMERO_DOCUMENTO,       
  (select top 1 NumberPurchaseProcess from AssetMovements WITH(NOLOCK) where AssetId = a.Id order by id desc) AS PROCESSO,          
  a.AcquisitionDate AS DATA_DOCUMENTO,          
  a.MovimentDate AS DATA_INCLUSAO,          
  a.ValueAcquisition AS VALOR_DOCUMENTO,  
  CASE 
		WHEN a.flagAcervo = 1 THEN ISNULL(a.ValueUpdate, a.ValueAcquisition)
		WHEN a.flagTerceiro = 1 THEN ISNULL(a.ValueUpdate, a.ValueAcquisition)
		WHEN a.flagDecreto = 1 
		THEN 
		ISNULL((SELECT MIN(m.[CurrentValue])
		  FROM [dbo].[MonthlyDepreciation] m WITH(NOLOCK) 
		  WHERE m.[ManagerUnitId] = a.ManagerUnitId 
		    AND m.[AssetStartId] = a.AssetStartId 
		    AND m.[MaterialItemCode] = a.MaterialItemCode 
			AND Convert(int,Convert(varchar(6),m.[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = a.[ManagerUnitId])
		), a.ValueUpdate)
		ELSE            
  ISNULL((SELECT MIN(m.[CurrentValue])
		  FROM [dbo].[MonthlyDepreciation] m WITH(NOLOCK) 
		  WHERE m.[ManagerUnitId] = a.ManagerUnitId 
		    AND m.[AssetStartId] = a.AssetStartId 
		    AND m.[MaterialItemCode] = a.MaterialItemCode 
			AND Convert(int,Convert(varchar(6),m.[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = a.[ManagerUnitId])
		), a.ValueAcquisition) END AS VALOR_ATUAL,          
  a.SerialNumber AS SERIE,          
  a.ManufactureDate AS FABRICACAO,          
  a.ChassiNumber AS CHASSI,          
  a.Brand AS MARCA,          
  a.NumberPlate AS PLACA,          
  a.Model AS MODELO,          
  (select [Description] from StateConservation WITH(NOLOCK) where id = am.StateConservationId) AS ESTADO_CONSERVACAO,          
  a.DateGuarantee AS GARANTIA,          
  (select [Description] from Initial WITH(NOLOCK) where id = a.OldInitial) + '-' + a.ChapaAntigaCompleta AS IDENTIFICACAO_ANTIGO,          
  a.AdditionalDescription AS HISTORICO,         
      
  am.MovimentDate AS DATA_INCLUSAO_MOV,      
  CASE WHEN am.FlagEstorno = 1 THEN 'Sim' ELSE '' END AS ESTORNO,       
  (select [Description] from MovementType WITH(NOLOCK) where id = am.MovementTypeId) AS TIPO_MOV,        
  (select [name] from Responsible WITH(NOLOCK) where id = am.ResponsibleId) AS RESPONSAVEL,         
  am.NumberDoc AS NUMERO_DOCUMENTO_MOV,         
  (select Code + ' - ' + [Description] from ManagerUnit WITH(NOLOCK) where id = am.SourceDestiny_ManagerUnitId) AS DESTINATARIO,    
  CASE     
  WHEN am.MovementTypeId = 10       
   THEN    
    (    
     select    
      CONVERT(VARCHAR(15), Code)     
     from     
      AdministrativeUnit     
     where     
      Id = am.AdministrativeUnitId    
    )    
  WHEN am.MovementTypeId = 11 or am.MovementTypeId = 12    
   THEN     
    (    
     (    
      select     
       CONVERT(VARCHAR(15), Code)     
      from    
       ManagerUnit    
      where     
       Id = am.ManagerUnitId    
     )    
     + '/' +    
     (    
      select     
       CONVERT(VARCHAR(15), Code)     
      from    
       ManagerUnit    
      where     
       Id = am.SourceDestiny_ManagerUnitId     
     )    
    )    
       
 END AS ORIG_DEST    
 FROM             
  Asset AS a  WITH(NOLOCK)           
 INNER JOIN               
     AssetMovements AS am WITH(NOLOCK)              
 ON           
  am.AssetId = a.Id            
 INNER JOIN          
    Institution AS i WITH(NOLOCK)           
 ON          
  am.InstitutionId = i.Id          
 INNER JOIN          
    BudgetUnit AS b WITH(NOLOCK)           
 ON          
  am.BudgetUnitId = b.Id          
 INNER JOIN          
    ManagerUnit AS m WITH(NOLOCK)           
 ON          
  am.ManagerUnitId = m.Id          
 LEFT JOIN          
    AdministrativeUnit AS adm WITH(NOLOCK)           
 ON          
  am.AdministrativeUnitId = adm.Id          
 LEFT JOIN          
    Section AS s WITH(NOLOCK)           
 ON          
  am.SectionId = s.Id          
 LEFT JOIN          
  AuxiliaryAccount AS aux WITH(NOLOCK)           
 ON          
  am.AuxiliaryAccountId = aux.Id        
 INNER JOIN  
 ShortDescriptionItem AS short WITH(NOLOCK)  
 ON  
 a.ShortDescriptionItemId = short.Id  
 WHERE          
  (am.InstitutionId = @idOrgao or @idOrgao is null)              
 AND           
  (am.BudgetUnitId = @idUo or @idUo is null)              
 AND           
  (am.ManagerUnitId = @idUge or @idUge is null)              
 AND           
  (am.AdministrativeUnitId = @idUa or @idUa is null)             
 AND           
  (am.SectionId = @idDivisao or @idDivisao is null)             
 AND      
  a.Id IN(select splitdata as Id from fnSplitString(@assertsSelecteds, ','))    
 ORDER BY 
	am.Status DESC   
END

GO
/****** Object:  StoredProcedure [dbo].[REPORT_ASSET_INVENTARIO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[REPORT_ASSET_INVENTARIO] --EXEC [dbo].[REPORT_ASSET_INVENTARIO] 43, 217, 1582, 54006, 0             
(            
 @idOrgao  int = null,              
 @idUo   int = null,              
 @idUge   int = null,              
 @idUa   int = null,        
 @Agrupamento varchar(1) = null,
 @mesRef VARCHAR(6) = null       
)             
AS             
BEGIN 

	select             
		CASE WHEN @Agrupamento = 1 THEN        
			m.Code        
		ELSE        
		CASE WHEN @Agrupamento = 2 THEN        
			aux.BookAccount        
		END        
		END as Agrupamento,        
		CASE WHEN @Agrupamento = 1 THEN        
		m.Description        
		ELSE        
		CASE WHEN @Agrupamento = 2 THEN        
			aux.[Description]        
		END        
		END as DescAgrupamento,        
		aux.Status AS STATUS_CONTA,       
		e.Code+' - '+e.[Description] as ORGAO,             
		f.Code+' - '+f.[Description] as UO,            
		g.Code+' - '+g.[Description] as UGE,               
		Convert(varchar(max),h.Code)+' - '+h.[Description] as UA,             
		Convert(varchar(max),c.Code)+' - '+c.[Description] as DIVISAO,               
		d.Name as RESPONSAVEL,             
		ISNULL(i.Name, '') +'-'+ a.NumberIdentification as CHAPA,               
		a.materialItemDescription as MATERIAL,               
		a.VALUEACQUISITION as VALOR_AQUISICAO,             
		cl.CurrentPrice as VALOR_ATUAL          
		from Asset as a WITH (NOLOCK)              
		inner join AssetMovements as am WITH (NOLOCK) on 
														(
															a.Id = am.AssetId and 
															am.Id = (
																		select top 1 Id from AssetMovements
																		where 
																			AssetId = a.Id and
																			MovimentDate < (
																								CONVERT(date, DATEADD(month, 1, (SUBSTRING(@mesRef, 1, 4) + SUBSTRING(@mesRef, 5, 2) + '01')))
																							) and
																			(FlagEstorno is null or FlagEstorno = 0)
																		order by Id desc	
																	)
														)
		inner join AuxiliaryAccount as aux WITH (NOLOCK) on am.AuxiliaryAccountId = aux.Id            
		LEFT JOIN Section as c WITH (NOLOCK) on c.Id = am.SectionId              
		inner join Responsible as d WITH (NOLOCK) on d.id = am.ResponsibleId              
		inner join Institution as e WITH (NOLOCK) on e.id = am.InstitutionId              
		inner join BudgetUnit as f WITH (NOLOCK) on f.id = am.BudgetUnitId              
		inner join ManagerUnit as g WITH (NOLOCK) on g.id = am.ManagerUnitId              
		LEFT JOIN AdministrativeUnit as h WITH (NOLOCK) on h.id = am.AdministrativeUnitId             
		inner join Initial as i WITH (NOLOCK) on i.Id = a.InitialId        
		LEFT JOIN MaterialGroup as m WITH (NOLOCK) on a.MaterialGroupCode = m.Code 
		--ALTERACAO @05/06/2019 DOUGLAS BATISTA
		--RETORNA NESTE RELATORIO BP'S QUE NAO ESTEAO NO FECHAMENTO MAS PERTENCEM A UGE NO MES_REFERENCIA EM QUESTAO
		--INNER JOIN
		LEFT JOIN Closing as cl WITH(NOLOCK) ON
												(
													a.Id = cl.AssetId and
													cl.ClosingYearMonthReference = @mesRef and
													cl.AssetMovementsId = am.Id
												)
		WHERE              
			(am.InstitutionId = @idOrgao or @idOrgao is null)              
		and (am.BudgetUnitId = @idUo or @idUo is null)              
		and (am.ManagerUnitId = @idUge or @idUge is null)              
		and (am.AdministrativeUnitId = @idUa or @idUa is null)          
		and a.flagVerificado is null         
		and a.flagDepreciaAcumulada = 1       
		ORDER BY                 
		i.Name,CAST(a.NumberIdentification as bigint)  
END

GO
/****** Object:  StoredProcedure [dbo].[REPORT_FECHAMENTO_MENSAL]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[REPORT_FECHAMENTO_MENSAL] --'201707',210      
(
	   @mesRef VARCHAR(6) = null,      
	   @UgeId int
) AS
BEGIN

	IF(@mesRef IS NULL)
	BEGIN
		select TOP 1 @mesRef = ManagmentUnit_YearMonthReference from ManagerUnit WITH(NOLOCK) where Id = @UgeId
	END

	Declare @BPsBaixados table(Id int);

	insert into @BPsBaixados
	select am.AssetId 
	from AssetMovements am WITH(NOLOCK)
	INNER JOIN Asset a WITH(NOLOCK)
	on a.Id = am.AssetId
	WHERE am.FlagEstorno IS NULL
	and am.DataEstorno IS NULL
	and am.ManagerUnitId = @UgeId
	and a.flagVerificado IS NULL
	and a.flagDepreciaAcumulada = 1
	and am.Status = 0
	and am.MovementTypeId IN (11,12,13,14,15,16,17,18,42,43,45,46,47,48,49,50,51,52,53,55,56,57,58,59,95)
	GROUP BY am.AssetId
	having (CONVERT(VARCHAR(6),MAX(am.MovimentDate),112)) <= @mesRef

	DECLARE @tabelaUltimoHistorico table(Id int);

	INSERT INTO @tabelaUltimoHistorico
	 select MAX(am.ID) ID
	 from AssetMovements am WITH(NOLOCK)
	 INNER JOIN Asset a WITH(NOLOCK)
	 on a.Id = am.AssetId
	 WHERE 
	 am.FlagEstorno IS NULL
	 and am.DataEstorno IS NULL
	 and am.ManagerUnitId = @UgeId
	 and a.flagVerificado IS NULL
	 and a.flagDepreciaAcumulada = 1
	 and (CONVERT(VARCHAR(6),am.MovimentDate,112)) <= @mesRef
	 GROUP BY am.AssetId;

	 DECLARE @tabelaComoChegouNoMes table(Id int);
	 INSERT INTO @tabelaComoChegouNoMes
	 select MAX(am.ID) 
	 from AssetMovements am WITH(NOLOCK)
	 INNER JOIN Asset a WITH(NOLOCK)
	 on a.Id = am.AssetId
	 WHERE 
	 am.FlagEstorno IS NULL
	 and am.DataEstorno IS NULL
	 and am.ManagerUnitId = @UgeId
	 and a.flagVerificado IS NULL
	 and a.flagDepreciaAcumulada = 1
	 and (CONVERT(VARCHAR(6),am.MovimentDate,112)) < @mesRef
	 GROUP BY am.AssetId;

select 
	BP.Id AS ID,
	ISNULL(historico.AuxiliaryAccountId,0) AS CONTABIL,
	ISNULL(contaContabil.ContaContabilApresentacao, '') AS CONTABIL_MOSTRADO,
	ISNULL(contaContabil.Status, 0) AS STATUS_CONTA,
	tipoMovimento.Description AS TIPO_MOVIMENTO,
	CASE 
	WHEN tipoMovimento.GroupMovimentId = 1 THEN BP.NumberDoc
	ELSE historico.NumberDoc
	END AS DOCUMENTO,
	ISNULL(historico.NumberPurchaseProcess,'') PROCESSO,
	ISNULL(sigla.Name,'') +' - '+ [BP].[ChapaCompleta] AS SIGLACHAPA,
	BP.MaterialItemDescription as MATERIAL,
	CASE WHEN ultimo.Id is not null 
		 THEN bp.ValueAcquisition
		 ELSE 0 
	END AS VALOR_AQUISICAO,
	CASE 
	     WHEN ultimo.Id is not null
		 THEN
		    CASE
			WHEN BP.flagAcervo = 1 THEN ISNULL(BP.ValueUpdate, BP.ValueAcquisition)
		    WHEN BP.flagTerceiro = 1 THEN ISNULL(BP.ValueUpdate, BP.ValueAcquisition)
			ELSE
			ISNULL((select MIN(CurrentValue)
			from MonthlyDepreciation WITH(NOLOCK)
			where MaterialItemCode = BP.MaterialItemCode
			  AND AssetStartId = BP.AssetStartId
			  and CONVERT(VARCHAR(6),CurrentDate,112) <= @mesRef
			  and CurrentMonth <= LifeCycle
			),BP.ValueAcquisition) 
			END
		 ELSE 0
	 END AS VALOR_ATUAL
from AssetMovements historico WITH(NOLOCK)
inner join Asset BP WITH(NOLOCK)
on BP.Id = historico.AssetId
inner join MovementType tipoMovimento WITH(NOLOCK)
on tipoMovimento.Id = historico.MovementTypeId
inner join Initial sigla WITH(NOLOCK)
on sigla.Id = BP.InitialId
left join AuxiliaryAccount contaContabil WITH(NOLOCK)
on contaContabil.Id = historico.AuxiliaryAccountId
left join @tabelaUltimoHistorico ultimo
on ultimo.Id = historico.Id
left join @tabelaComoChegouNoMes chegada
on chegada.Id = historico.Id
where 
BP.Id NOT IN (select Id from @BPsBaixados)
AND (CONVERT(VARCHAR(6),historico.MovimentDate,112) = @mesRef
	 OR chegada.Id is not null)
and historico.FlagEstorno IS NULL
and historico.DataEstorno IS NULL
and historico.ManagerUnitId = @UgeId
and BP.flagVerificado IS NULL
and BP.flagDepreciaAcumulada = 1
ORDER BY               
  sigla.Name, case when ISNUMERIC(BP.NumberIdentification) = 1 THEN convert(float, BP.NumberIdentification) ELSE '99999999999' END;
END

GO
/****** Object:  StoredProcedure [dbo].[REPORT_PRINT_BAR_CODE_OR_QRCODE]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[REPORT_PRINT_BAR_CODE_OR_QRCODE]
	@idUge int,
	@assertsSelecteds varchar(max) 
AS
BEGIN

	select i.[Name] 'Sigla', 
	a.ChapaCompleta, 
	s.[Description] as ShortDescription, 
	m.[Description] as Description_UGE, 
	CONVERT(varbinary(max), '') as BarCode 
	from Asset as a
	inner join ManagerUnit as m on a.ManagerUnitId = m.id 
	inner join ShortDescriptionItem as s on a.ShortDescriptionItemId = s.Id
	inner join Initial as i on a.InitialId = i.Id
	where a.ManagerUnitId = @idUge and
	      a.Id IN(
				select splitdata as Id from fnSplitString(@assertsSelecteds, ',')
			   )

END


GO
/****** Object:  StoredProcedure [dbo].[REPORT_RESPONSIBLE]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[REPORT_RESPONSIBLE]        
(              
 @idUa INT = NULL,                
 @idResponsavel INT = NULL,          
 @idDivisao  INT = NULL               
)              
AS               
BEGIN             
	SELECT               
	 ISNULL(i.Name, '') +'-'+ a.ChapaCompleta AS 'CHAPA',         
	 a.ChapaAntigaCompleta AS 'N_ANTERIOR',
	 aux.[Description] AS 'TIPO_DE_BEM',
	 sdi.[Description] AS 'DESCRIÇÃO_OU_COMENTARIO',
	 a.ValueUpdate AS 'VALOR',        
	 ad.Street AS 'RUA',        
	 ad.Number AS 'NUMERO',        
	 ad.District AS 'BAIRRO',        
	 ad.PostalCode AS 'CEP',        
	 ad.City AS 'CIDADE',        
	 ad.[State] AS 'ESTADO',        
	 c.Telephone AS 'TELEFONE',      
	 st.Description AS 'ESTADO_CONSERVACAO',    
	 c.Description AS 'DIVISAO'    
	FROM              
	 Asset AS a WITH (NOLOCK)                
	INNER JOIN               
	(                
	 SELECT             
	  MAX(a.Id) id,             
	  AssetId,            
	  SectionId,        
	  AuxiliaryAccountId,      
	  StateConservationId           
	 FROM             
	  AssetMovements AS a WITH (NOLOCK)                
	 WHERE             
	  AdministrativeUnitId = @idUa             
	 AND             
	  ResponsibleId =  @idResponsavel             
	 AND             
	  (SectionId =  @idDivisao OR @idDivisao is NULL)          
	 AND                 
	  Status = 1                
	 GROUP BY             
	  AssetId,            
	  SectionId,        
	  AuxiliaryAccountId,      
	  StateConservationId            
	) AS b              
	ON             
	b.AssetId = a.Id                
	LEFT JOIN               
	Section AS c WITH (NOLOCK) ON c.id = b.SectionId                
	INNER JOIN               
	Initial AS i WITH (NOLOCK) ON i.Id = a.InitialId          
	LEFT JOIN        
	AuxiliaryAccount AS aux (NOLOCK) ON b.AuxiliaryAccountId = aux.Id        
	INNER JOIN         
	ShortDescriptionItem AS sdi (NOLOCK) ON a.ShortDescriptionItemId = sdi.Id        
	LEFT JOIN         
	[Address] AS ad (NOLOCK) ON c.AddressId = ad.Id        
	 INNER JOIN         
	StateConservation AS st (NOLOCK) ON st.Id = b.StateConservationId      
	WHERE  
	a.flagVerificado is null and a.flagDepreciaAcumulada = 1 and a.Status = 1
	ORDER BY 
		case when ISNUMERIC(a.NumberIdentification) = 1 THEN convert(float, a.NumberIdentification) ELSE '99999999999' END;
END 

GO
/****** Object:  StoredProcedure [dbo].[REPORT_RESUMO_INVENTARIO_BP]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[REPORT_RESUMO_INVENTARIO_BP]
(          
 @idOrgao INT = null,            
 @idUo  INT = null,            
 @idUge  INT = null,            
 @mesRef VARCHAR(6) = null
)           
AS 
BEGIN
	SET DATEFORMAT DMY;


	DECLARE @InstitutionId INT = NULL;
	DECLARE @BudgetUnitId INT = NULL;
	DECLARE @ManagerUnitId INT = NULL;

	DECLARE @ClosingYearMonthReference VARCHAR(6) = NULL;
	DECLARE @ClosingYearMonthReferenceFirstDate VARCHAR(10) = NULL;
	DECLARE @LastClosingYearMonthReference VARCHAR(6) = NULL;
	DECLARE @NextMonthReferenceFirstDate VARCHAR(10) = NULL;





	SET @ClosingYearMonthReference = @mesRef;
	SET @InstitutionId = @idOrgao;
	SET @BudgetUnitId  = @idUo;
	SET @ManagerUnitId = @idUge;


	IF (@ClosingYearMonthReference IS NULL)
		SET @ClosingYearMonthReference = (SELECT ManagmentUnit_YearMonthReference FROM ManagerUnit WHERE Id = @ManagerUnitId);

	SET @ClosingYearMonthReferenceFirstDate = (CONVERT(VARCHAR(10), CONVERT(DATE, DATEADD(month, 0, (SUBSTRING(@ClosingYearMonthReference, 1, 4) + SUBSTRING(@ClosingYearMonthReference, 5, 2) + '01'))),105))
	SET @NextMonthReferenceFirstDate = (CONVERT(VARCHAR(10), CONVERT(DATE, DATEADD(month, 1, (SUBSTRING(@ClosingYearMonthReference, 1, 4) + SUBSTRING(@ClosingYearMonthReference, 5, 2) + '01'))),105))
	(
		SELECT
				  ugeSIAFEM.Code + '-' + ugeSIAFEM.[Description] as UGE
				, contaContabil.BookAccount
				, contaContabil.[Description]
				, SUM(ISNULL(tabelaDepreciacaoBP.CurrentValue, 0.00)) AS Valor
				, SUM(ISNULL(tabelaDepreciacaoBP.ValueAcquisition, 0.00)) AS ValorAquisicao
				, SUM(ISNULL(tabelaDepreciacaoBP.AccumulatedDepreciation, 0.00)) AS DepreciationAccumulated
				, contaContabil.[Status] AS STATUS_CONTA

				, contaContabilDepreciacao.Code AS ContaDepreciacaoCode
				, contaContabilDepreciacao.Description AS ContaDepreciacaoDescricao


				, 0.00 AS SaldoAnterior
				, SUM(ISNULL(tabelaDepreciacaoBP.RateDepreciationMonthly, 0.00)) as DepreciacaoNoMes
				, SUM(ISNULL(tabelaDepreciacaoBP.AccumulatedDepreciation, 0.00)) AS DepreciacaoAcumulada
		FROM
			MonthlyDepreciation tabelaDepreciacaoBP WITH(NOLOCK)
		INNER JOIN Asset BP WITH(NOLOCK) ON tabelaDepreciacaoBP.AssetStartId = BP.Id
		INNER JOIN AssetMovements movimentacaoBP WITH(NOLOCK) ON BP.Id = movimentacaoBP.AssetId
		INNER JOIN ManagerUnit ugeSIAFEM WITH(NOLOCK) ON tabelaDepreciacaoBP.ManagerUnitId = ugeSIAFEM.Id
		INNER JOIN AuxiliaryAccount contaContabil WITH(NOLOCK) ON movimentacaoBP.AuxiliaryAccountId = contaContabil.Id
		INNER JOIN DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) ON contaContabil.DepreciationAccountId = contaContabilDepreciacao.Id
		WHERE 
			(	movimentacaoBP.ManagerUnitId = @ManagerUnitId 
			AND movimentacaoBP.MovimentDate BETWEEN @ClosingYearMonthReferenceFirstDate AND @NextMonthReferenceFirstDate
			AND movimentacaoBP.MovementTypeId NOT IN (23, 24) --BPs Pendentes
			AND NOT (flagVerificado IS NULL AND flagDepreciaAcumulada IS NULL) --VERIFICAR MOTIVO DIFERENCA DEPOIS
			AND BP.[Status] = 1)

		GROUP BY 
			  ugeSIAFEM.Code
			, ugeSIAFEM.[Description]
			, contaContabilDepreciacao.Code
			, contaContabilDepreciacao.[Description]
			, contaContabil.BookAccount
			, contaContabil.[Description]
			, contaContabil.[Status]
		)/*
		UNION ALL
		--BPs TOTALMENTE DEPRECIADOS
		(
		SELECT
				DISTINCT
				  ugeSIAFEM.Code + '-' + ugeSIAFEM.[Description] as UGE
				, contaContabil.BookAccount
				, contaContabil.[Description]
				, SUM(ISNULL(tabelaDepreciacaoBP.CurrentValue, 0.00)) AS Valor
				, SUM(ISNULL(tabelaDepreciacaoBP.ValueAcquisition, 0.00)) AS ValorAquisicao
				, SUM(ISNULL(tabelaDepreciacaoBP.AccumulatedDepreciation, 0.00)) AS DepreciationAccumulated
				, contaContabil.[Status] AS STATUS_CONTA

				, contaContabilDepreciacao.Code AS ContaDepreciacaoCode
				, contaContabilDepreciacao.Description AS ContaDepreciacaoDescricao

				, 0.00 AS SaldoAnterior
				, 0.00 as DepreciacaoNoMes
				, SUM(ISNULL(tabelaDepreciacaoBP.AccumulatedDepreciation, 0.00)) AS DepreciacaoAcumulada
		FROM
			MonthlyDepreciation tabelaDepreciacaoBP WITH(NOLOCK)
		INNER JOIN Asset BP WITH(NOLOCK) ON tabelaDepreciacaoBP.AssetStartId = BP.Id
		INNER JOIN AssetMovements movimentacaoBP WITH(NOLOCK) ON BP.Id = movimentacaoBP.AssetId
		INNER JOIN ManagerUnit ugeSIAFEM WITH(NOLOCK) ON tabelaDepreciacaoBP.ManagerUnitId = ugeSIAFEM.Id
		INNER JOIN AuxiliaryAccount contaContabil WITH(NOLOCK) ON movimentacaoBP.AuxiliaryAccountId = contaContabil.Id
		INNER JOIN DepreciationAccount contaContabilDepreciacao WITH(NOLOCK) ON contaContabil.DepreciationAccountId = contaContabilDepreciacao.Id
		WHERE 
			--tabelaDepreciacaoBP.CurrentDate BETWEEN '01-05-2019' AND '01-06-2019'
			--tabelaDepreciacaoBP.CurrentDate BETWEEN @ClosingYearMonthReferenceFirstDate AND @NextMonthReferenceFirstDate
		     ugeSIAFEM.Id = @ManagerUnitId
		AND (contaContabil.[Status] = 1 AND contaContabilDepreciacao.[Status] = 1)
		--AND tabelaDepreciacaoBP.ManagerUnitTransferId IS NOT NULL

		AND	(BP.flagVerificado IS NULL AND BP.flagDepreciaAcumulada = 1)
		AND	(BP.[Status] = 1)
		--AND (movimentacaoBP.InstitutionId = @InstitutionId OR @InstitutionId IS NULL)
		--AND (movimentacaoBP.BudgetUnitId = @BudgetUnitId OR @BudgetUnitId IS NULL)
		--AND (movimentacaoBP.ManagerUnitId = @ManagerUnitId OR @ManagerUnitId IS NULL)
		--AND (contaContabil.[Status] = 1)
		AND (tabelaDepreciacaoBP.CurrentMonth = tabelaDepreciacaoBP.LifeCycle) --TOTALMENTE DEPRECIADO
		GROUP BY 
			  ugeSIAFEM.Code
			, ugeSIAFEM.[Description]
			, contaContabilDepreciacao.Code
			, contaContabilDepreciacao.[Description]
			, contaContabil.BookAccount
			, contaContabil.[Description]
			, contaContabil.[Status]


			, BP.Id
			, BP.InitialName					
			, BP.NumberIdentification			
			, movimentacaoBP.MovimentDate		
			, BP.[Status]						
			, BP.monthUsed						
			, tabelaDepreciacaoBP.CurrentMonth	
			, tabelaDepreciacaoBP.LifeCycle		

	)*/

	PRINT 'DEPRECIADOS NO MES'
	PRINT @ClosingYearMonthReference
	PRINT @ClosingYearMonthReferenceFirstDate
	PRINT @NextMonthReferenceFirstDate
END

GO
/****** Object:  StoredProcedure [dbo].[SALDO_VISAO_GERAL_POR_CONTA_CONTABIL_HOJE]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SALDO_VISAO_GERAL_POR_CONTA_CONTABIL_HOJE]
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

Declare @ContasContabeisAMandarProSIAFEM as table(ContaContabeis int, DescricaoConta varchar(255))

insert into @ContasContabeisAMandarProSIAFEM (ContaContabeis, DescricaoConta)
select ContaContabil, Descricao from ContasContabeisPraConsultarNoSIAFEM with(nolock)

Declare @contConta int;
select @contConta = count(ContaContabeis) 
from @ContasContabeisAMandarProSIAFEM;

Declare @RetornoComIds as table (
IdOrgao int, 
CodigoOrgao varchar(6),
IdUo int, 
CodigoUO varchar(6),
IdUGE int, 
CodigoUGE varchar(6),
Mes varchar(4),
Ano varchar(6),
ContaContabil int,
DescricaoConta varchar(255),
ValorContabilSAM decimal(18,2));

Declare @contaAtual int, @DescricaoConta varchar(255);

while(@contConta > 0)
BEGIN
	select top 1 
	@contaAtual = ContaContabeis,
	@DescricaoConta = DescricaoConta
	from @ContasContabeisAMandarProSIAFEM;

	insert into @RetornoComIds(
	IdOrgao, 
	CodigoOrgao ,
	IdUo, 
	CodigoUO,
	IdUGE, 
	CodigoUGE,
	Mes,
	Ano,
	ContaContabil,
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
		@DescricaoConta,
		0.0
	from @Hierarquias

	delete from @ContasContabeisAMandarProSIAFEM where ContaContabeis = @contaAtual;
	set @contConta = @contConta - 1;
END

update temp
set temp.ValorContabilSAM = s.ValorContabil 
from @RetornoComIds temp
inner join SaldoContabilAtualUGEContaContabil s
on s.IdUGE = temp.IdUGE
and s.IdUO = temp.IdUo
and s.IdOrgao = temp.IdOrgao
and s.ContaContabil = temp.ContaContabil;

select CodigoOrgao 'Orgao',
	   CodigoUO 'UO',
	   CodigoUGE 'UGE',
	   ContaContabil 'Conta',
	   DescricaoConta 'DescricaoConta',
	   ValorContabilSAM 'ValorContabilSAM',
	   Mes 'Mes',
	   Ano 'Ano'
from @RetornoComIds
order by Orgao, UO, UGE, Conta

END

GO
/****** Object:  StoredProcedure [dbo].[SALDO_VISAO_GERAL_POR_CONTA_DEPRECIACAO_HOJE]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  StoredProcedure [dbo].[SAM_BUSCA_ULTIMO_NUMERO_DOCUMENTO_SAIDA]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SAM_BUSCA_ULTIMO_NUMERO_DOCUMENTO_SAIDA]
	@ManagerUnitId INT = 0,
	@Ano INT = 0
AS

Declare @lista as table(
		Numero bigint,
		palavra varchar(20)
);
Declare @palavraComValorMaximo varchar(20), @ValorMaximo bigint;

insert into @lista
select case 
	   WHEN ISNUMERIC(NumberDoc) = 1 THEN 
			CASE 
			WHEN CHARINDEX('.', NumberDoc) != 0 THEN -1
			ELSE convert(bigint, NumberDoc)
			END
	   ELSE -1 
	   END,
		NumberDoc
 from AssetMovements WITH(NOLOCK)
where ManagerUnitId = @ManagerUnitId
and MovementTypeId in (select Id from MovementType WITH(NOLOCK) where GroupMovimentId != 1)

IF( (select count(*) from @lista) > 0)
BEGIN

	if((select count(palavra) from @lista where LEN(palavra) = 14) > 0)
	BEGIN
		select top 1 @palavraComValorMaximo = ISNULL(palavra,'0'), @ValorMaximo = Numero from @lista where numero = (select max(numero) from @lista where LEN(palavra) = 14);
	END
	ELSE
	BEGIN
		select top 1 @palavraComValorMaximo = ISNULL(palavra,'0'), @ValorMaximo = Numero from @lista where numero = (select max(numero) from @lista);
	END
	
	IF (LEN(@palavraComValorMaximo) < 4)
	BEGIN
		SELECT '0';
	END
	ELSE
	BEGIN
		--Se o não for um número de documento do ano (ou seja, os 4 primeiros digitos for diferente do ano anual), 
		--retorna 0 para o sistema começar a contagem para o ano.
		IF (SUBSTRING(@palavraComValorMaximo,0,5) != @Ano)
		BEGIN
			SELECT '0';
		END
		ELSE
		BEGIN
			SELECT @palavraComValorMaximo;
		END
	END
END
ELSE
BEGIN
	SELECT '0'
END

GO
/****** Object:  StoredProcedure [dbo].[SAM_BUSCA_VALORES_FECHAMENTO_UGES_COM_MES_REF_FECHADO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
/****** Object:  StoredProcedure [dbo].[SAM_CALCULA_DEPRECIACAO_CARGA_DE_DADOS]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SAM_CALCULA_DEPRECIACAO_CARGA_DE_DADOS]
(
	@IdOrgao int
)
AS BEGIN

BEGIN TRY

set dateformat YMD;

Declare @MesRefInicialDaUGE as varchar(6);
Declare @PrimeiroDiaMesReferencia as date;
Declare @MesAnterior as date;

declare @tabela as table
(	
	Id int identity(1,1),
	AssetId int,
	AcquisitionDate date,
	AcquisitionValue decimal(18,2),
	MaterialGroupCode int,
	IdUGE int,
	IdContaContabil int,
	DataIncorporacao datetime,
	IdIncorporacao int,
	Chapa varchar(250),
	ItemMaterial int,
	InitialId int,
	MesRefInicialUGE varchar(7)
);

insert into @tabela
			(AssetId,AcquisitionDate,AcquisitionValue,MaterialGroupCode,
			IdUGE,IdContaContabil,DataIncorporacao,IdIncorporacao,
			Chapa,ItemMaterial, InitialId, MesRefInicialUGE)
select  a.Id, 
		a.AcquisitionDate, 
		a.ValueAcquisition,
		a.MaterialGroupCode,
		am.ManagerUnitId,
		am.AuxiliaryAccountId,
		am.MovimentDate,
		am.Id,
		a.NumberIdentification,
		a.MaterialItemCode,
		a.InitialId,
		uge.ManagmentUnit_YearMonthStart
from Asset a
inner join AssetMovements am
on a.Id = am.AssetId
inner join ManagerUnit uge
on am.ManagerUnitId = uge.Id
inner join MovementType m
on am.MovementTypeId = m.Id
where a.flagDepreciaAcumulada = 1
and a.flagVerificado is null
and m.GroupMovimentId = 1
and am.InstitutionId = @IdOrgao
and DATEDIFF(MONTH, --verifica se 
	am.MovimentDate, --o mês da data de incorporação esteja 
	CONCAT(SUBSTRING(uge.ManagmentUnit_YearMonthStart,1,4),'-',
	SUBSTRING(uge.ManagmentUnit_YearMonthStart,5,2),'-01') --no mesmo mês que o mês inicial (de uso) da UGE,
	) = 0 -- (a diferença de meses entre eles será igual q zero)
and (a.flagAcervo is null or a.flagAcervo = 0)
and (a.flagTerceiro is null or a.flagTerceiro = 0)
and (a.FlagAnimalNaoServico is null or a.FlagAnimalNaoServico = 0)
and DATEDIFF(MONTH, --verifica se 
	a.AcquisitionDate, --o mês da data de aquisição do BP seja retroativa, 
	CONCAT(SUBSTRING(uge.ManagmentUnit_YearMonthStart,1,4),'-',
	SUBSTRING(uge.ManagmentUnit_YearMonthStart,5,2),'-01') --ou seja, é antes do mês inicial (de uso) da UGE,
	) > 0 -- (a diferença de meses entre eles, nesse caso, será maior q zero)
and a.AssetStartId is null
and a.IndicadorOrigemInventarioInicial = 2; --e o BP tenha vindo da carga

Declare @cont int;

select @cont = count(Id) from @tabela;

Declare @IdAtual int;

--variáveis para a depreciação

Declare @AssetStartId int,@MaterialItemGroup int, @ManagerUnitId int,
		@CicloDeVida int, @DepreciacaoMensal decimal(18,2), @ValorResidualDoGrupo decimal(18,2),
		@DataAquisicao datetime, @ValorDeAquisicao decimal(18,2), @ValorAtual decimal(18,2),
		@DataIncorporacao datetime, @ValorResidual DECIMAL(18,2), @DepreciacaoAcumulada decimal(18,2),
		@MesContado int, @IdContaContabil int, @DataAtual datetime,
		@IdIncorporacao int, @Chapa varchar(250), @ItemMaterial int, @InitialId int,
		@MesDataAtual int, @AnoDataAtual int, @ValorDesdobro decimal(18,2), @DataLinhaUM datetime;

Declare @UltimaDepreciacao as table(
		[AssetStartId] [int] NULL,
		[NumberIdentification] [varchar](20) NULL,
		[ManagerUnitId] [int] NOT NULL,
		[MaterialItemCode] [int] NOT NULL,
		[InitialId] [int] NOT NULL,
		[AcquisitionDate] [datetime] NOT NULL,
		[CurrentDate] [datetime] NOT NULL,
		[DateIncorporation] [datetime] NOT NULL,
		[LifeCycle] [smallint] NOT NULL,
		[CurrentMonth] [smallint] NOT NULL,
		[ValueAcquisition] [decimal](18, 2) NOT NULL,
		[CurrentValue] [decimal](18, 2) NOT NULL,
		[ResidualValue] [decimal](18, 2) NOT NULL,
		[RateDepreciationMonthly] [decimal](18, 2) NOT NULL,
		[AccumulatedDepreciation] [decimal](18, 2) NOT NULL,
		[UnfoldingValue] [decimal](18, 2) NULL,
		[Decree] [bit] NOT NULL,
		[ManagerUnitTransferId] [int] NULL,
		[MonthlyDepreciationId] [int] NULL,
		[MesAbertoDoAceite] [bit] NULL,
		[QtdLinhaRepetida] [int] NOT NULL
	);

while(@cont > 0)
begin
	SET @MesContado = 0;
	SET @ValorResidual = 0;
	SET @DepreciacaoAcumulada = 0;
	SET @ValorAtual = 0;
	SET @DepreciacaoMensal = 0;
	SET @ValorDesdobro = 0;
	SET @DataLinhaUM = null;
	delete from @UltimaDepreciacao;
	
	select top 1 
	@IdAtual = Id,
	@AssetStartId = AssetId,
	@MaterialItemGroup = MaterialGroupCode,
	@ValorDeAquisicao = AcquisitionValue,
	@DataAquisicao = AcquisitionDate,
	@ManagerUnitId = IdUGE,
	@IdContaContabil = IdContaContabil,
	@IdIncorporacao = IdIncorporacao,
	@ItemMaterial = ItemMaterial,
	@InitialId = InitialId,
	@DataIncorporacao = DataIncorporacao,
	@MesRefInicialDaUGE = MesRefInicialUGE
	from @tabela;

	SET @PrimeiroDiaMesReferencia  = CONCAT(SUBSTRING(@MesRefInicialDaUGE,1,4),'-',SUBSTRING(@MesRefInicialDaUGE,5,2),'-01');
	SET @MesAnterior  = DATEADD(MONTH,-1,@PrimeiroDiaMesReferencia);

	update Asset
	set AssetStartId = @AssetStartId, 
		DepreciationAmountStart = @ValorDeAquisicao, 
		DepreciationDateStart = @DataAquisicao
	where id = @AssetStartId

	update AssetMovements
	set MonthlyDepreciationId = @AssetStartId
	where id = @IdIncorporacao

	select top 1 
	@CicloDeVida = LifeCycle,
	@ValorResidualDoGrupo = ResidualValue,
	@DepreciacaoMensal = RateDepreciationMonthly
	from MaterialGroup 
	where Code = @MaterialItemGroup

	set @DataAtual = @DataAquisicao;
	set @ValorAtual = @ValorDeAquisicao;

	IF(DAY(@DataAtual) = 11)
	BEGIN
		set @DataLinhaUM = @DataAtual;
		SET @DataAtual = DATEADD(DAY,-1,@DataAtual)
	END

	IF(DAY(@DataAtual) = 31 AND MONTH(@DataAtual) IN (1,3,5,7,8,10,12))
	BEGIN
		set @DataLinhaUM = @DataAtual;
		SET @DataAtual = DATEADD(DAY,-1,@DataAtual)
	END
	ELSE
	BEGIN
		IF(DAY(@DataAtual) = 30 AND MONTH(@DataAtual) IN (4,6,9,11))
		BEGIN
			set @DataLinhaUM = @DataAtual;
			SET @DataAtual = DATEADD(DAY,-1,@DataAtual)
		END
		ELSE
		BEGIN
			IF(DAY(@DataAtual) in (28,29) AND MONTH(@DataAtual) = 2)
			BEGIN
				set @DataLinhaUM = @DataAtual;
				SET @DataAtual = concat(YEAR(@DataAtual),'-02-27')
			END
		END
	END

	SET @ValorResidual = ROUND((@ValorDeAquisicao * ROUND(@ValorResidualDoGrupo,2,1)) / 100,2,1);
	SET @DepreciacaoMensal = ROUND((@ValorDeAquisicao - @ValorResidual) / @CicloDeVida,2,1);

	insert into MonthlyDepreciation(
			   [AssetStartId]
		      ,[NumberIdentification]
		      ,[ManagerUnitId]
		      ,[MaterialItemCode]
		      ,[InitialId]
		      ,[AcquisitionDate]
		      ,[CurrentDate]
		      ,[DateIncorporation]
		      ,[LifeCycle]
		      ,[CurrentMonth]
		      ,[ValueAcquisition]
		      ,[CurrentValue]
		      ,[ResidualValue]
		      ,[RateDepreciationMonthly]
		      ,[AccumulatedDepreciation]
		      ,[UnfoldingValue]
		      ,[Decree]
		      ,[ManagerUnitTransferId]
		      ,[MonthlyDepreciationId]
		      ,[MesAbertoDoAceite]
			  ,[QtdLinhaRepetida]
		)
		values(
			@AssetStartId,
			@Chapa,
			@ManagerUnitId,
			@ItemMaterial,
			@InitialId,
			@DataAquisicao,
			@DataAtual,
			@DataIncorporacao,
			@CicloDeVida,
			@MesContado,
			@ValorDeAquisicao,
			@ValorDeAquisicao,
			@ValorResidual,
			@DepreciacaoMensal,
			@DepreciacaoAcumulada,
			0,
			0,
			NULL,
			@AssetStartId,
			NULL,
			0
		);

		insert into @UltimaDepreciacao(
			   [AssetStartId]
		      ,[NumberIdentification]
		      ,[ManagerUnitId]
		      ,[MaterialItemCode]
		      ,[InitialId]
		      ,[AcquisitionDate]
		      ,[CurrentDate]
		      ,[DateIncorporation]
		      ,[LifeCycle]
		      ,[CurrentMonth]
		      ,[ValueAcquisition]
		      ,[CurrentValue]
		      ,[ResidualValue]
		      ,[RateDepreciationMonthly]
		      ,[AccumulatedDepreciation]
		      ,[UnfoldingValue]
		      ,[Decree]
		      ,[ManagerUnitTransferId]
		      ,[MonthlyDepreciationId]
		      ,[MesAbertoDoAceite]
			  ,[QtdLinhaRepetida]
		)
		SELECT top 1 [AssetStartId]
		      ,[NumberIdentification]
		      ,[ManagerUnitId]
		      ,[MaterialItemCode]
		      ,[InitialId]
		      ,[AcquisitionDate]
		      ,[CurrentDate]
		      ,[DateIncorporation]
		      ,[LifeCycle]
		      ,[CurrentMonth]
		      ,[ValueAcquisition]
		      ,[CurrentValue]
		      ,[ResidualValue]
		      ,[RateDepreciationMonthly]
		      ,[AccumulatedDepreciation]
		      ,[UnfoldingValue]
		      ,[Decree]
		      ,[ManagerUnitTransferId]
		      ,[MonthlyDepreciationId]
		      ,[MesAbertoDoAceite]
			  ,[QtdLinhaRepetida]
		  FROM [dbo].[MonthlyDepreciation]
		  where AssetStartId = @AssetStartId
		  order by id desc

while(@DataAtual < @PrimeiroDiaMesReferencia and @MesContado <= @CicloDeVida)
BEGIN
	SET @MesContado = @MesContado + 1;
	
	IF(@MesContado = 1)
	BEGIN
		IF(@DataLinhaUM IS NOT NULL)
		BEGIN
			SET @DataAtual = @DataLinhaUM;
		END
		ELSE
		BEGIN
			IF(DAY(@DataAtual) > 11)
			BEGIN
				SET @DataAtual = DATEADD(DAY,1,@DataAtual);
			END
			ELSE
			BEGIN
				SET @MesDataAtual = MONTH(@DataAtual);
				SET @AnoDataAtual = YEAR(@DataAtual);
				SET @DataAtual = concat(@AnoDataAtual,'-',@MesDataAtual,'-11');
			END
		END
		
	END
	IF(@MesContado = 2)
	BEGIN
		SET @MesDataAtual = MONTH(@DataAtual);
		SET @AnoDataAtual = YEAR(@DataAtual);
		SET @DataAtual = concat(@AnoDataAtual,'-',@MesDataAtual,'-11');
	END

	IF(@MesContado <= @CicloDeVida)
	BEGIN
		SET @ValorAtual = @ValorAtual - @DepreciacaoMensal;
		SET @DepreciacaoAcumulada = @DepreciacaoAcumulada + @DepreciacaoMensal;
	
		IF(@MesContado = @CicloDeVida)
		BEGIN
			IF(@ValorAtual != @ValorResidual)
			BEGIN
				IF(@ValorAtual > @ValorResidual)
				BEGIN
					SET @ValorDesdobro = @ValorAtual - @ValorResidual;
					SET @DepreciacaoAcumulada = @DepreciacaoAcumulada + @ValorDesdobro;
					SET @DepreciacaoMensal = @DepreciacaoMensal + @ValorDesdobro;
					SET @ValorAtual = @ValorAtual - @ValorDesdobro; 
				END
				ELSE
				BEGIN
					SET @ValorDesdobro = @ValorResidual - @ValorAtual;
					SET @DepreciacaoAcumulada = @DepreciacaoAcumulada - @ValorDesdobro;
					SET @DepreciacaoMensal = @DepreciacaoMensal - @ValorDesdobro;
					SET @ValorAtual = @ValorAtual + @ValorDesdobro; 
				END
			END
		END
	END
	ELSE
	BEGIN
		SET @DepreciacaoMensal = 0;
		SET @ValorDesdobro = 0;
	END

	insert into MonthlyDepreciation(
					 	 [AssetStartId]
					    ,[NumberIdentification]
					    ,[ManagerUnitId]
					    ,[MaterialItemCode]
					    ,[InitialId]
					    ,[AcquisitionDate]
					    ,[CurrentDate]
					    ,[DateIncorporation]
					    ,[LifeCycle]
					    ,[CurrentMonth]
					    ,[ValueAcquisition]
					    ,[CurrentValue]
					    ,[ResidualValue]
					    ,[RateDepreciationMonthly]
					    ,[AccumulatedDepreciation]
					    ,[UnfoldingValue]
					    ,[Decree]
					    ,[ManagerUnitTransferId]
					    ,[MonthlyDepreciationId]
					    ,[MesAbertoDoAceite]
						,[QtdLinhaRepetida])
					SELECT top 1 [AssetStartId]
					    ,[NumberIdentification]
					    ,ManagerUnitId
					    ,[MaterialItemCode]
					    ,[InitialId]
					    ,[AcquisitionDate]
					    ,@DataAtual
					    ,[DateIncorporation]
					    ,[LifeCycle]
					    ,@MesContado
					    ,[ValueAcquisition]
					    ,@ValorAtual 'Valor Atual'
					    ,@ValorResidual
					    ,@DepreciacaoMensal 'Depreciação Mes'
					    ,@DepreciacaoAcumulada 'Depreciação Acumulada'
					    ,@ValorDesdobro
					    ,[Decree]
					    ,[ManagerUnitTransferId]
					    ,[MonthlyDepreciationId]
					    ,[MesAbertoDoAceite]
						,[QtdLinhaRepetida]
					FROM @UltimaDepreciacao

	SET @DataAtual = DATEADD(MONTH,1,@DataAtual);
END

	delete from @tabela where Id = @IdAtual;
	set @cont = @cont - 1; 
end


END TRY
BEGIN CATCH
-- Execute error retrieval routine.  
    SELECT  
    ERROR_NUMBER() AS ErrorNumber  
    ,ERROR_SEVERITY() AS ErrorSeverity  
    ,ERROR_LINE() AS ErrorLine  
    ,ERROR_MESSAGE() AS ErrorMessage
END CATCH

END

GO
/****** Object:  StoredProcedure [dbo].[SAM_CALCULAR_DEPRECIACAO_FECHAMENTO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SAM_CALCULAR_DEPRECIACAO_FECHAMENTO] 
	@AssetStartId INT,
	@ManagerUnitId INT,
	@CodigoMaterial INT
	--@CurrentValueOutPut DECIMAL(18,2) OUTPUT
AS
BEGIN
	DECLARE @CurrentMonth SMALLINT;
	DECLARE @CurrentValue DECIMAL(18,2);
	DECLARE @CurrentDate DATETIME;
	DECLARE @RateDepreciationMonthly DECIMAL(18,2);
	DECLARE @AccumulatedDepreciation DECIMAL(18,2);
	DECLARE @LifeCycle SMALLINT;
	DECLARE @Count INT;
	DECLARE @DataAquisicao AS DATETIME;
	DECLARE @DataIncorporacao AS DATETIME;
	DECLARE @Id INT;
	DECLARE @DataDecreto AS DATETIME;
	DECLARE @ValorAquisicao AS DECIMAL(18,2);
	DECLARE @ValorDecreto AS DECIMAL(18,2);
	DECLARE @AceleratedDepreciation BIT;
	DECLARE @PorcetagemResidual DECIMAL(18,2);
	DECLARE @ValorResidual DECIMAL(18,2);
	DECLARE @DepreciacaoMensal DECIMAL(18,2);
	DECLARE @DataTransferencia AS DATETIME;
	DECLARE @ManagerUnitTransferId INT;
	DECLARE @AssetStartIdTranferencia INT;
	DECLARE @VidaUtilInicio SMALLINT;
	DECLARE @MesReferencia AS VARCHAR(6);
	DECLARE @DataTransfere AS DATETIME

	DECLARE @Table as TABLE(
		Id INT NOT NULL,
		CurrentMonth INT NOT NULL,
		CurrentValue DECIMAL(18,2) NOT NULL,
		CurrentDate DATETIME NOT NULL,
		RateDepreciationMonthly DECIMAL(18,2) NOT NULL,
		AccumulatedDepreciation DECIMAL(18,2) NOT NULL,
		LifeCycle SMALLINT NOT NULL
	) 
	SET DATEFORMAT YMD;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; 
	INSERT INTO @Table(Id,CurrentMonth,CurrentValue,CurrentDate,RateDepreciationMonthly,AccumulatedDepreciation,LifeCycle)
	       SELECT  TOP 1 Id,CurrentMonth,CurrentValue,CurrentDate,RateDepreciationMonthly,AccumulatedDepreciation ,LifeCycle
	         FROM [dbo].[MonthlyDepreciation]
		   WHERE [AssetStartId] = @AssetStartId 
		     AND [ManagerUnitId] = @ManagerUnitId 
			 AND [MaterialItemCode] = @CodigoMaterial
		   ORDER BY Id DESC

	SET @Count = (SELECT COUNT(*) FROM @Table);
	SET @DataDecreto = (SELECT TOP 1 [ast].[MovimentDate] 
							  FROM [dbo].[Asset] [ast]
							 WHERE [ast].[flagDecreto]  = 1
							   AND [ast].[AssetStartId] = @AssetStartId
							 ORDER BY [ast].[Id] ASC);
		
	IF(@Count IS NULL OR @Count < 1)
	BEGIN
		

		SET @DataAquisicao =(SELECT TOP 1 [AcquisitionDate] 
									   FROM [dbo].[Asset] 
										WHERE [AssetStartId] = @AssetStartId 
										 AND [ManagerUnitId] = @ManagerUnitId 
										 AND [MaterialItemCode] = @CodigoMaterial
										ORDER BY [Id] ASC
									);
		SET @DataIncorporacao =(SELECT TOP 1 [MovimentDate] 
								   FROM [dbo].[Asset] 
								  WHERE [AssetStartId] = @AssetStartId 
									AND [ManagerUnitId] = @ManagerUnitId 
									AND [MaterialItemCode] = @CodigoMaterial
								  ORDER BY [Id] ASC
								);
		IF(@DataIncorporacao < @DataAquisicao)
		BEGIN
			SET @DataAquisicao = @DataIncorporacao;
		END;

		IF(@DataDecreto IS NOT NULL)
		BEGIN
			
			SET @ValorDecreto =  (SELECT TOP 1 [ValueUpdate] 
							FROM [dbo].[Asset] [ast] 
							WHERE [AssetStartId] = @AssetStartId);
			UPDATE [dbo].[Asset] SET [DepreciationDateStart] = @DataDecreto, [DepreciationAmountStart] = @ValorDecreto
			WHERE [AssetStartId] = @AssetStartId;

							   
		END
		ELSE
			BEGIN
				
				SET @ValorAquisicao =  (SELECT TOP 1 [ValueAcquisition]
							FROM [dbo].[Asset] [ast] 
							WHERE [AssetStartId] = @AssetStartId);

				UPDATE [dbo].[Asset] SET [DepreciationDateStart] = @DataAquisicao, [DepreciationAmountStart] = @ValorAquisicao
				WHERE [AssetStartId] = @AssetStartId;
			END;

		SET @CurrentMonth =0;
		
		SET @CurrentValue =(SELECT TOP 1 [DepreciationAmountStart] 
		                    FROM [dbo].[Asset]  
						WHERE [AssetStartId] = @AssetStartId)
							
		
		

		SET @CurrentDate =@DataAquisicao;

		SET @AccumulatedDepreciation =0
				--- Retornar a vida util do item de material
		SET @LifeCycle = (SELECT TOP 1 [gr].[LifeCycle] 
						   FROM [dbo].[MaterialGroup] [gr]
						   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
						  WHERE [AssetStartId] = @AssetStartId 
							AND [ManagerUnitId] = @ManagerUnitId 
							AND [MaterialItemCode] = @CodigoMaterial
					);


			SET @AceleratedDepreciation =(SELECT TOP 1 [ast].[AceleratedDepreciation] 
								  FROM [dbo].[Asset] [ast]
								 WHERE [ast].[flagDecreto]  = 1
								   AND [ast].[AssetStartId] = @AssetStartId
								 ORDER BY [ast].[Id] ASC);
			IF(@AceleratedDepreciation IS NULL)
			BEGIN
								
				SET @PorcetagemResidual = (SELECT TOP 1 [gr].[ResidualValue] 
											FROM [dbo].[MaterialGroup] [gr]
											INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
										WHERE [ast].[AssetStartId] = @AssetStartId); 
			END;
			ELSE
				BEGIN
									
					SET @PorcetagemResidual = (SELECT TOP 1 [ast].[ResidualValue] 
											FROM [dbo].[Asset] [ast] 
										WHERE [ast].[AssetStartId] = @AssetStartId); 

					SET @LifeCycle = (SELECT TOP 1 [ast].[LifeCycle] 
											FROM [dbo].[Asset] [ast] 
										WHERE [ast].[AssetStartId] = @AssetStartId); 
			END;			
						
			
			
			SET @ManagerUnitTransferId = (SELECT TOP 1 [mov].[ManagerUnitId] 
												FROM [dbo].[AssetMovements] [mov]
												INNER JOIN [dbo].[Asset] [bem] ON [bem].[Id] = [mov].[AssetId]
												WHERE [bem].[MaterialItemCode] = @CodigoMaterial
												  AND [mov].[AssetTransferenciaId]= @AssetStartId);
				IF(@ManagerUnitTransferId IS NOT NULL)
				BEGIN
					SET @DataTransferencia =  (SELECT TOP 1 [mov].[MovimentDate] 
												FROM [dbo].[AssetMovements] [mov]
												INNER JOIN [dbo].[Asset] [bem] ON [bem].[Id] = [mov].[AssetId]
												WHERE [bem].[MaterialItemCode] = @CodigoMaterial
												  AND [mov].[AssetTransferenciaId]= @AssetStartId);

					IF(@DataTransferencia IS NOT NULL)
					BEGIN
						SET @AssetStartIdTranferencia = (SELECT TOP 1 [mov].[AssetId]
												FROM [dbo].[AssetMovements] [mov]
												INNER JOIN [dbo].[Asset] [bem] ON [bem].[Id] = [mov].[AssetId]
												WHERE [bem].[MaterialItemCode] = @CodigoMaterial
												  AND [mov].[AssetTransferenciaId]= @AssetStartId);

												   
							SET @CurrentMonth =(SELECT MAX(CurrentMonth) 
											   FROM [dbo].[MonthlyDepreciation] 
											  WHERE [AssetStartId] = @AssetStartIdTranferencia
											    AND [MaterialItemCode] = @CodigoMaterial
												AND [ManagerUnitId] = @ManagerUnitTransferId
											   );
							SET @VidaUtilInicio = (SELECT TOP 1 [gr].[LifeCycle] 
											   FROM [dbo].[MaterialGroup] [gr]
											   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
											   WHERE [ast].[MaterialItemCode] = @CodigoMaterial)

							SET @LifeCycle = @VidaUtilInicio - @CurrentMonth;
							SET @CurrentMonth = 0;
							SET @CurrentValue = (SELECT MIN([CurrentValue])
											FROM [dbo].[MonthlyDepreciation] 
										  WHERE [AssetStartId] = @AssetStartIdTranferencia
											    AND [MaterialItemCode] = @CodigoMaterial
												AND [ManagerUnitId] = @ManagerUnitTransferId
										);

								SET @DataIncorporacao =(SELECT TOP 1 [MovimentDate] 
											   FROM [dbo].[Asset] 
											  WHERE [AssetStartId] = @AssetStartId
											  AND [ManagerUnitId] = @ManagerUnitId
											  ORDER BY [Id] ASC
											);
							SET @ManagerUnitTransferId = NULL;

						DECLARE @DataTransferenciaSequence AS DATETIME;

						SET @DataTransferenciaSequence = (SELECT [mov].[MovimentDate] FROM [Asset] [bem] 
												   INNER JOIN [AssetMovements] [mov] ON [mov].[AssetId] = [bem].[Id]
												   WHERE [bem].[MaterialItemCode] = @CodigoMaterial  
													 AND [bem].[AssetStartId] = @AssetStartId
													 AND [mov].[ManagerUnitId] = @ManagerUnitId 
													 AND [mov].[AssetTransferenciaId] IS NOT NULL)

						IF(@DataTransferenciaSequence IS NOT NULL)
						BEGIN
							IF(@CurrentDate > @DataTransferenciaSequence )
							BEGIN
								
								SET @DataTransferencia = NULL;
								RETURN 0;
							END;
						END;
						
						SET @CurrentDate =@DataTransferencia;
							
		

					END;

				END;
				ELSE
					BEGIN
						SET @DataTransferencia = (SELECT [mov].[MovimentDate] FROM [Asset] [bem] 
												   INNER JOIN [AssetMovements] [mov] ON [mov].[AssetId] = [bem].[Id]
												   WHERE [bem].[MaterialItemCode] = @CodigoMaterial  
													 AND [bem].[AssetStartId] = @AssetStartId
													 AND [mov].[ManagerUnitId] = @ManagerUnitId 
													 AND [mov].[AssetTransferenciaId] IS NOT NULL)
						IF(@DataTransferencia IS NOT NULL)
						BEGIN
							SET @DataTransfere = CAST(YEAR(@DataTransferenciaSequence) AS VARCHAR(4)) + '-' + CAST(MONTH(@DataTransferenciaSequence) AS VARCHAR(2)) + '-01';
							IF(@CurrentDate >= @DataTransfere )
							BEGIN
								
								SET @DataTransferencia = NULL;
								RETURN 0;
							END;
						END;

					END 
						
				
			SET @ValorResidual =ROUND((@ValorAquisicao * ROUND(@PorcetagemResidual,2,1)) / 100,2,1);
			SET @DepreciacaoMensal = ROUND((@ValorAquisicao - @ValorResidual) / @LifeCycle,2,1);

			INSERT INTO [dbo].[MonthlyDepreciation]
				   ([AssetStartId]
				   ,[NumberIdentification]
				   ,[ManagerUnitId]
				   ,[MaterialItemCode]
				   ,[InitialId]
				   ,[AcquisitionDate]
				   ,[CurrentDate]
				   ,[DateIncorporation]
				   ,[LifeCycle]
				   ,[CurrentMonth]
				   ,[ValueAcquisition]
				   ,[CurrentValue]
				   ,[ResidualValue]
				   ,[RateDepreciationMonthly]
				   ,[AccumulatedDepreciation]
				   ,[UnfoldingValue]
				   ,[Decree]
				   ,[MonthlyDepreciationId]
				   ,[ManagerUnitTransferId])
			 SELECT
				   @AssetStartId
				   ,Null
				   ,@ManagerUnitId
				   ,@CodigoMaterial
				   ,InitialId
				   ,AcquisitionDate
				   ,@CurrentDate
				   ,MovimentDate
				   ,@LifeCycle
				   ,@CurrentMonth
				   ,ValueAcquisition
				   ,@CurrentValue
				   ,@ValorResidual
				   ,@DepreciacaoMensal
				   ,@AccumulatedDepreciation
				   ,Null
				   ,(CASE WHEN flagDecreto IS NULL THEN 0 ELSE flagDecreto END) 'Decree'
				   ,@AssetStartId
				   ,Null
			FROM  [dbo].[Asset]	
			WHERE [AssetStartId] = @AssetStartId 
			  AND [ManagerUnitId] = @ManagerUnitId 
			  AND [MaterialItemCode] = @CodigoMaterial
	END;
	DELETE @Table

	SET @DataTransferenciaSequence = (SELECT [mov].[MovimentDate] FROM [Asset] [bem] 
												   INNER JOIN [AssetMovements] [mov] ON [mov].[AssetId] = [bem].[Id]
												   WHERE [bem].[MaterialItemCode] = @CodigoMaterial  
													 AND [bem].[AssetStartId] = @AssetStartId
													 AND [mov].[ManagerUnitId] = @ManagerUnitId 
													 AND [mov].[AssetTransferenciaId] IS NOT NULL)

	
						

	INSERT INTO @Table(Id,CurrentMonth,CurrentValue,CurrentDate,RateDepreciationMonthly,AccumulatedDepreciation,LifeCycle)
	    SELECT  TOP 1 Id,CurrentMonth,CurrentValue,CurrentDate,RateDepreciationMonthly,AccumulatedDepreciation ,LifeCycle
	        FROM [dbo].[MonthlyDepreciation]
		WHERE [AssetStartId] = @AssetStartId 
		    AND [ManagerUnitId] = @ManagerUnitId 
			AND [MaterialItemCode] = @CodigoMaterial
		ORDER BY Id DESC


	SET @Id=(SELECT TOP 1 Id FROM @Table);
	SET @CurrentMonth =(SELECT CurrentMonth FROM @Table)
	SET @CurrentDate =(SELECT CurrentDate FROM @Table)

	IF(@DataTransferenciaSequence IS NOT NULL)
	BEGIN
		SET @DataTransfere = CAST(YEAR(@DataTransferenciaSequence) AS VARCHAR(4)) + '-' + CAST(MONTH(@DataTransferenciaSequence) AS VARCHAR(2)) + '-01';
		IF(@CurrentDate >= @DataTransfere )
		BEGIN
								
			SET @DataTransferencia = NULL;
			RETURN 0;
		END;
	END;

	SET @MesReferencia =(SELECT TOP 1 ManagmentUnit_YearMonthReference FROM [dbo].[ManagerUnit] WHERE [Id] = @ManagerUnitId)

	IF(@MesReferencia <= CAST(YEAR(@CurrentDate) AS VARCHAR(4)) +  (CAST(MONTH(@CurrentDate) AS VARCHAR(2))))
	BEGIN
		RETURN 0;
	END



	IF(@DataDecreto IS NOT NULL AND YEAR(@DataDecreto) = YEAR(@CurrentDate) AND MONTH(@DataDecreto) = MONTH(@CurrentDate))
	BEGIN
		SET @CurrentValue =(SELECT TOP 1 [DepreciationAmountStart] 
		                     FROM [dbo].[Asset]  
							WHERE [AssetStartId] = @AssetStartId 
							  AND [ManagerUnitId] = @ManagerUnitId 
							  AND [MaterialItemCode] = @CodigoMaterial);
	END
	ELSE
		BEGIN
			SET @CurrentValue =(SELECT CurrentValue FROM @Table)
		END;
	

	
	SET @AccumulatedDepreciation =(SELECT AccumulatedDepreciation FROM @Table)
	SET @RateDepreciationMonthly =(SELECT RateDepreciationMonthly FROM @Table)
	SET @LifeCycle = (SELECT LifeCycle FROM @Table)


		IF(@CurrentMonth < @LifeCycle)
		BEGIN 
			IF(@DataDecreto IS NOT NULL AND YEAR(@DataDecreto) = YEAR(@CurrentDate) AND MONTH(@DataDecreto) = MONTH(@CurrentDate))
			BEGIN
				SET @CurrentValue = @CurrentValue;
				SET @CurrentMonth = @CurrentMonth + 1;
				SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
				SET @AccumulatedDepreciation = @AccumulatedDepreciation;
			END
			ELSE
			BEGIN
				SET @CurrentValue = @CurrentValue - @RateDepreciationMonthly;
				SET @CurrentMonth = @CurrentMonth + 1;
				SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
				SET @AccumulatedDepreciation = @AccumulatedDepreciation + @RateDepreciationMonthly;
			END;
			INSERT INTO [dbo].[MonthlyDepreciation]
				   ([AssetStartId]
				   ,[NumberIdentification]
				   ,[ManagerUnitId]
				   ,[MaterialItemCode]
				   ,[InitialId]
				   ,[AcquisitionDate]
				   ,[CurrentDate]
				   ,[DateIncorporation]
				   ,[LifeCycle]
				   ,[CurrentMonth]
				   ,[ValueAcquisition]
				   ,[CurrentValue]
				   ,[ResidualValue]
				   ,[RateDepreciationMonthly]
				   ,[AccumulatedDepreciation]
				   ,[UnfoldingValue]
				   ,[Decree]
				   ,[MonthlyDepreciationId]
				   ,[ManagerUnitTransferId])
			 SELECT
				   @AssetStartId
				   ,NumberIdentification
				   ,@ManagerUnitId
				   ,@CodigoMaterial
				   ,InitialId
				   ,AcquisitionDate
				   ,@CurrentDate
				   ,DateIncorporation
				   ,LifeCycle
				   ,@CurrentMonth
				   ,ValueAcquisition
				   ,@CurrentValue
				   ,ResidualValue
				   ,RateDepreciationMonthly
				   ,@AccumulatedDepreciation
				   ,UnfoldingValue
				   ,(CASE WHEN Decree IS NULL THEN 0 ELSE Decree END) 'Decree'
				   ,MonthlyDepreciationId
				   ,ManagerUnitTransferId
			FROM  [dbo].[MonthlyDepreciation]	
			WHERE [Id] = @Id
		END
		ELSE
			IF(@CurrentMonth = @LifeCycle)
			BEGIN
				SET @CurrentMonth = @CurrentMonth + 1;
				SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));

				INSERT INTO [dbo].[MonthlyDepreciation]
					   ([AssetStartId]
					   ,[NumberIdentification]
					   ,[ManagerUnitId]
					   ,[MaterialItemCode]
					   ,[InitialId]
					   ,[AcquisitionDate]
					   ,[CurrentDate]
					   ,[DateIncorporation]
					   ,[LifeCycle]
					   ,[CurrentMonth]
					   ,[ValueAcquisition]
					   ,[CurrentValue]
					   ,[ResidualValue]
					   ,[RateDepreciationMonthly]
					   ,[AccumulatedDepreciation]
					   ,[UnfoldingValue]
					   ,[Decree]
					   ,[MonthlyDepreciationId]
					   ,[ManagerUnitTransferId])
				 SELECT
					   @AssetStartId
					   ,NumberIdentification
					   ,@ManagerUnitId
					   ,@CodigoMaterial
					   ,InitialId
					   ,AcquisitionDate
					   ,@CurrentDate
					   ,DateIncorporation
					   ,LifeCycle
					   ,@CurrentMonth
					   ,ValueAcquisition
					   ,CurrentValue
					   ,ResidualValue
					   ,0.0
					   ,@AccumulatedDepreciation
					   ,UnfoldingValue
					   ,(CASE WHEN Decree IS NULL THEN 0 ELSE Decree END) 'Decree'
					   ,MonthlyDepreciationId
					   ,ManagerUnitTransferId
				FROM  [dbo].[MonthlyDepreciation]	
				WHERE [Id] = @Id
			END;


	--SELECT @CurrentValueOutPut = @CurrentValue;
	--SELECT @CurrentValue
	UPDATE [dbo].[Asset] SET ValueUpdate = @CurrentValue
	WHERE [AssetStartId] = @AssetStartId 
	  AND [ManagerUnitId] = @ManagerUnitId 
	  AND [MaterialItemCode] = @CodigoMaterial 
END;	

GO
/****** Object:  StoredProcedure [dbo].[SAM_CALCULAR_DEPRECIACAO_INCORPORAR_START_EXECUTAR]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SAM_CALCULAR_DEPRECIACAO_INCORPORAR_START_EXECUTAR] ---100112
	@CodigoUge INT,
	@DataFinal AS DATETIME,
	@AssetStartId INT
AS
-- Valores para testes
--SET @CodigoUge = 380273;
--SET @CodigoMaterial = 2409860; 
BEGIN
	SET DATEFORMAT dmy;
	SET LANGUAGE 'Brazilian'

	DECLARE @DataAtual DATETIME;
	DECLARE @DataAquisicao DATETIME;
	DECLARE @Contador SMALLINT = 0;
	DECLARE @ManagerUnitId INT;
	DECLARE @InitialId INT;
	DECLARE @VidaUtil AS SMALLINT;
	DECLARE @VidaUtilInicio AS SMALLINT;
	DECLARE @DepreciacaoAcumulada AS DECIMAL(18,2);
	DECLARE @DepreciacaoMensal AS DECIMAL(18,2);
	DECLARE @ValorResidual AS DECIMAL(18,2);
	DECLARE @PorcetagemResidual AS DECIMAL(18,10);
	DECLARE @ValorAquisicao AS DECIMAL(18,2);
	DECLARE @ValorAtual AS DECIMAL(18,2);
	DECLARE @QuantidadeJaDepreciada AS INT;
	DECLARE @PorcetagemDepreciacaoMensal AS DECIMAL(18,10)
	DECLARE @Desdobro AS DECIMAL(18,10);
	DECLARE @DataDecreto AS DATETIME = NULL;
	DECLARE @ManagerUnitIdTransferencia INT;
	DECLARE @COUNTITEM SMALLINT;
	DECLARE @CONTADORITEM SMALLINT = 0;
	DECLARE @CodigoMaterial INT;
	DECLARE @ManagerUnitTransferId INT = 0;
	DECLARE @CurrentMonth SMALLINT = 0;
	DECLARE @DataIncorporacao DATETIME;
	

	DECLARE @TableTransferencia AS TABLE(
		MovimentDate DATETIME NOT NULL,
		ManagerUnitId INT NOT NULL,
		MaterialItemCode INT NOT NULL,
		AssetStartId INT NOT NULL,
		InitialId	INT NOT NULL
	);

	DECLARE @TableItemMaterial AS TABLE(
		MaterialItemCode INT NOT NULL,
		ManagerUnitId INT NOT NULL,
		AssetStartId INT NOT NULL,
		InitialId	INT NOT NULL
	)

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  

	BEGIN TRY
		SET @ManagerUnitId = (SELECT TOP 1 [Id] FROM [dbo].[ManagerUnit] WHERE [Code] = @CodigoUge);

		INSERT INTO @TableItemMaterial(MaterialItemCode, ManagerUnitId,AssetStartId,InitialId)
		SELECT MIN([ast].[MaterialItemCode]),@ManagerUnitId,[ast].[AssetStartId],MIN([ast].[InitialId]) 
		 FROM [dbo].[Asset] [ast]
		  INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id] 
		 WHERE [ast].[ManagerUnitId] = @ManagerUnitId 
		   AND [ast].[Status] = 1
           AND [flagVerificado] IS NULL
           AND [flagDepreciaAcumulada] = 1 
		   AND [ast].[AssetStartId] = @AssetStartId
		 GROUP BY  [ast].[AssetStartId]

		SET @COUNTITEM = (SELECT COUNT(*) FROM @TableItemMaterial);

		WHILE(@CONTADORITEM < @COUNTITEM)
		BEGIN

			SET @CodigoMaterial = (SELECT TOP 1 MaterialItemCode FROM @TableItemMaterial WHERE AssetStartId = @AssetStartId);
			SET @InitialId = (SELECT TOP 1 InitialId FROM @TableItemMaterial WHERE AssetStartId = @AssetStartId AND MaterialItemCode = @CodigoMaterial);

			INSERT INTO @TableTransferencia(MovimentDate,AssetStartId,InitialId,ManagerUnitId,MaterialItemCode)
			 SELECT DISTINCT [ast].[MovimentDate],@AssetStartId,[ast].[InitialId],[mov].[SourceDestiny_ManagerUnitId],@CodigoMaterial
				FROM [dbo].[AssetMovements] [mov] 
				INNER JOIN [dbo].[Asset] [ast] ON [ast].[Id] = [mov].[AssetId] 
				WHERE [mov].[AssetTransferenciaId] =@AssetStartId
				  AND [ast].[MaterialItemCode] = @CodigoMaterial;

			SET @ValorAquisicao = (SELECT TOP 1 [ValueAcquisition] 
										FROM [dbo].[Asset] 
									WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] ASC);

			SET @ValorAtual=  @ValorAquisicao;
			SET @DataAquisicao =(SELECT TOP 1 [AcquisitionDate] 
								   FROM [dbo].[Asset] 
								    WHERE [ManagerUnitId] = @ManagerUnitId
							          AND [AssetStartId]= @AssetStartId
								    ORDER BY [Id] ASC
								);
			SET @DataIncorporacao =(SELECT TOP 1 [MovimentDate] 
							   FROM [dbo].[Asset] 
							  WHERE [AssetStartId] = @AssetStartId
							  ORDER BY [Id] ASC
							);
			IF(@DataIncorporacao < @DataAquisicao)
			BEGIN
				SET @DataAquisicao = @DataIncorporacao;
			END;
			SET @CurrentMonth =(SELECT MAX(CurrentMonth) 
			                               FROM [dbo].[MonthlyDepreciation] 
										  WHERE [AssetStartId] = @AssetStartId
										   );

			IF(@CurrentMonth IS NULL)
			BEGIN
				SET @CurrentMonth = 0;
			END;
			SET @DataAtual = (SELECT MovimentDate 
			                    FROM @TableTransferencia 
			                   WHERE [AssetStartId] = @AssetStartId);

			IF(@DataAtual IS NOT NULL)
			BEGIN
				
				IF(@CurrentMonth > 0)
				BEGIN
					SET @VidaUtilInicio = (SELECT TOP 1 [gr].[LifeCycle] 
										   FROM [dbo].[MaterialGroup] [gr]
										   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
										   WHERE [ast].[MaterialItemCode] = @CodigoMaterial)
			
					SET @VidaUtil = @VidaUtilInicio - @CurrentMonth;
					SET @ValorAtual = (SELECT MIN([CurrentValue])
										FROM [dbo].[MonthlyDepreciation] 
									WHERE [AssetStartId] = @AssetStartId);

				END
				ELSE
				   BEGIN
						SET @VidaUtil =(SELECT DATEDIFF(MONTH,@DataAquisicao,@DataAtual));
				   END;
				
				SET @ManagerUnitTransferId = (SELECT ManagerUnitId 
												FROM @TableTransferencia 
											   WHERE [AssetStartId] = @AssetStartId
											     AND [ManagerUnitId] = @ManagerUnitId);
				
			END;
			ELSE
				BEGIN
					SET @VidaUtil = (SELECT TOP 1 [gr].[LifeCycle] 
									   FROM [dbo].[MaterialGroup] [gr]
									   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
									   WHERE [ast].[MaterialItemCode] = @CodigoMaterial)
				END;

		    IF(@CurrentMonth < @VidaUtil)
			BEGIN
				EXEC [dbo].[SAM_CALCULAR_DEPRECIACAO_START]  @CodigoUge,@CodigoMaterial,@InitialId,@AssetStartId,@DataAquisicao,@DataIncorporacao,@ValorAquisicao,@ValorAtual,@VidaUtil,@ManagerUnitTransferId,@DataFinal;
			END;
			PRINT @AssetStartId;
			DELETE FROM @TableItemMaterial WHERE [AssetStartId] =@AssetStartId AND [ManagerUnitId] = @ManagerUnitId;
			SET @CONTADORITEM = @CONTADORITEM + 1;
		END;
		RETURN @CONTADORITEM;
	END TRY
	BEGIN CATCH
	SELECT  
		ERROR_NUMBER() AS ErrorNumber  
		,ERROR_SEVERITY() AS ErrorSeverity  
		,ERROR_STATE() AS ErrorState  
		,ERROR_PROCEDURE() AS ErrorProcedure  
		,ERROR_LINE() AS ErrorLine  
		,ERROR_MESSAGE() AS ErrorMessage
		,@CodigoMaterial AS CodigoMaterial;

	END CATCH;
END;

GO
/****** Object:  StoredProcedure [dbo].[SAM_CALCULAR_DEPRECIACAO_INTEGRACAO_START_EXECUTAR]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SAM_CALCULAR_DEPRECIACAO_INTEGRACAO_START_EXECUTAR] ---100112
	@CodigoUge INT,
	@DataFinal AS DATETIME,
	@AssetStartId INT
AS
-- Valores para testes
--SET @CodigoUge = 380273;
--SET @CodigoMaterial = 2409860; 
BEGIN
	SET DATEFORMAT dmy;
	SET LANGUAGE 'Brazilian'

	DECLARE @DataAtual DATETIME;
	DECLARE @DataAquisicao DATETIME;
	DECLARE @Contador SMALLINT = 0;
	DECLARE @ManagerUnitId INT;
	DECLARE @InitialId INT;
	DECLARE @VidaUtil AS SMALLINT;
	DECLARE @VidaUtilInicio AS SMALLINT;
	DECLARE @DepreciacaoAcumulada AS DECIMAL(18,2);
	DECLARE @DepreciacaoMensal AS DECIMAL(18,2);
	DECLARE @ValorResidual AS DECIMAL(18,2);
	DECLARE @PorcetagemResidual AS DECIMAL(18,10);
	DECLARE @ValorAquisicao AS DECIMAL(18,2);
	DECLARE @ValorAtual AS DECIMAL(18,2);
	DECLARE @QuantidadeJaDepreciada AS INT;
	DECLARE @PorcetagemDepreciacaoMensal AS DECIMAL(18,10)
	DECLARE @Desdobro AS DECIMAL(18,10);
	DECLARE @DataDecreto AS DATETIME = NULL;
	DECLARE @ManagerUnitIdTransferencia INT;
	DECLARE @COUNTITEM SMALLINT;
	DECLARE @CONTADORITEM SMALLINT = 0;
	DECLARE @CodigoMaterial INT;
	DECLARE @ManagerUnitTransferId INT = 0;
	DECLARE @CurrentMonth SMALLINT = 0;
	DECLARE @DataIncorporacao DATETIME;
	

	DECLARE @TableTransferencia AS TABLE(
		MovimentDate DATETIME NOT NULL,
		ManagerUnitId INT NOT NULL,
		MaterialItemCode INT NOT NULL,
		AssetStartId INT NOT NULL,
		InitialId	INT NOT NULL
	);

	DECLARE @TableItemMaterial AS TABLE(
		MaterialItemCode INT NOT NULL,
		ManagerUnitId INT NOT NULL,
		AssetStartId INT NOT NULL,
		InitialId	INT NOT NULL
	)

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  

	BEGIN TRY
		SET @ManagerUnitId = (SELECT TOP 1 [Id] FROM [dbo].[ManagerUnit] WHERE [Code] = @CodigoUge);

		INSERT INTO @TableItemMaterial(MaterialItemCode, ManagerUnitId,AssetStartId,InitialId)
		SELECT MIN([ast].[MaterialItemCode]),@ManagerUnitId,[ast].[AssetStartId],MIN([ast].[InitialId]) 
		 FROM [dbo].[Asset] [ast]
		  INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id] 
		 WHERE [ast].[ManagerUnitId] = @ManagerUnitId 
		   AND [ast].[Status] = 1
           AND [flagVerificado] =0
           AND [flagDepreciaAcumulada] IS NULL
		   AND [ast].[AssetStartId] = @AssetStartId
		   AND [ast].[NumberIdentification] ='99999999999'
		 GROUP BY  [ast].[AssetStartId]

		SET @COUNTITEM = (SELECT COUNT(*) FROM @TableItemMaterial);

		WHILE(@CONTADORITEM < @COUNTITEM)
		BEGIN

			SET @CodigoMaterial = (SELECT TOP 1 MaterialItemCode FROM @TableItemMaterial WHERE AssetStartId = @AssetStartId);
			SET @InitialId = (SELECT TOP 1 InitialId FROM @TableItemMaterial WHERE AssetStartId = @AssetStartId AND MaterialItemCode = @CodigoMaterial);

			INSERT INTO @TableTransferencia(MovimentDate,AssetStartId,InitialId,ManagerUnitId,MaterialItemCode)
			 SELECT DISTINCT [ast].[MovimentDate],@AssetStartId,[ast].[InitialId],[mov].[SourceDestiny_ManagerUnitId],@CodigoMaterial
				FROM [dbo].[AssetMovements] [mov] 
				INNER JOIN [dbo].[Asset] [ast] ON [ast].[Id] = [mov].[AssetId] 
				WHERE [mov].[AssetTransferenciaId] =@AssetStartId
				  AND [ast].[MaterialItemCode] = @CodigoMaterial;

			SET @ValorAquisicao = (SELECT TOP 1 [ValueAcquisition] 
										FROM [dbo].[Asset] 
									WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] ASC);

			SET @ValorAtual=  @ValorAquisicao;
			SET @DataAquisicao =(SELECT TOP 1 [AcquisitionDate] 
								   FROM [dbo].[Asset] 
								    WHERE [ManagerUnitId] = @ManagerUnitId
							          AND [AssetStartId]= @AssetStartId
								    ORDER BY [Id] ASC
								);
			SET @DataIncorporacao =(SELECT TOP 1 [MovimentDate] 
							   FROM [dbo].[Asset] 
							  WHERE [AssetStartId] = @AssetStartId
							  ORDER BY [Id] ASC
							);
			IF(@DataIncorporacao < @DataAquisicao)
			BEGIN
				SET @DataAquisicao = @DataIncorporacao;
			END;
			SET @CurrentMonth =(SELECT MAX(CurrentMonth) 
			                               FROM [dbo].[MonthlyDepreciation] 
										  WHERE [AssetStartId] = @AssetStartId
										   );

			IF(@CurrentMonth IS NULL)
			BEGIN
				SET @CurrentMonth = 0;
			END;
			SET @DataAtual = (SELECT MovimentDate 
			                    FROM @TableTransferencia 
			                   WHERE [AssetStartId] = @AssetStartId);

			IF(@DataAtual IS NOT NULL)
			BEGIN
				
				IF(@CurrentMonth > 0)
				BEGIN
					SET @VidaUtilInicio = (SELECT TOP 1 [gr].[LifeCycle] 
										   FROM [dbo].[MaterialGroup] [gr]
										   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
										   WHERE [ast].[MaterialItemCode] = @CodigoMaterial)
			
					SET @VidaUtil = @VidaUtilInicio - @CurrentMonth;
					SET @ValorAtual = (SELECT MIN([CurrentValue])
										FROM [dbo].[MonthlyDepreciation] 
									WHERE [AssetStartId] = @AssetStartId);

				END
				ELSE
				   BEGIN
						SET @VidaUtil =(SELECT DATEDIFF(MONTH,@DataAquisicao,@DataAtual));
				   END;
				
				SET @ManagerUnitTransferId = (SELECT ManagerUnitId 
												FROM @TableTransferencia 
											   WHERE [AssetStartId] = @AssetStartId
											     AND [ManagerUnitId] = @ManagerUnitId);
				
			END;
			ELSE
				BEGIN
					SET @VidaUtil = (SELECT TOP 1 [gr].[LifeCycle] 
									   FROM [dbo].[MaterialGroup] [gr]
									   INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
									   WHERE [ast].[MaterialItemCode] = @CodigoMaterial)
				END;

		    IF(@CurrentMonth < @VidaUtil)
			BEGIN
				EXEC [dbo].[SAM_CALCULAR_DEPRECIACAO_START]  @CodigoUge,@CodigoMaterial,@InitialId,@AssetStartId,@DataAquisicao,@DataIncorporacao,@ValorAquisicao,@ValorAtual,@VidaUtil,@ManagerUnitTransferId,@DataFinal;
			END;
			PRINT @AssetStartId;
			DELETE FROM @TableItemMaterial WHERE [AssetStartId] =@AssetStartId AND [ManagerUnitId] = @ManagerUnitId;
			SET @CONTADORITEM = @CONTADORITEM + 1;
		END;
		RETURN @CONTADORITEM;
	END TRY
	BEGIN CATCH
	SELECT  
		ERROR_NUMBER() AS ErrorNumber  
		,ERROR_SEVERITY() AS ErrorSeverity  
		,ERROR_STATE() AS ErrorState  
		,ERROR_PROCEDURE() AS ErrorProcedure  
		,ERROR_LINE() AS ErrorLine  
		,ERROR_MESSAGE() AS ErrorMessage
		,@CodigoMaterial AS CodigoMaterial;

	END CATCH;
END;

GO
/****** Object:  StoredProcedure [dbo].[SAM_CALCULAR_DEPRECIACAO_START]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
	CREATE PROCEDURE [dbo].[SAM_CALCULAR_DEPRECIACAO_START] --380273,2409860,12544,3569147,120, null
	@CodigoUge INT,
	@CodigoMaterial INT,
	@InitialId INT,
	@AssetStartId INT, 
	@DataAquisicao DATETIME,
	@DataIncorporacao DATETIME,
	@ValorAquisicao AS DECIMAL(18,2),
	@ValorAtual AS DECIMAL(18,2),
	@VidaUtil SMALLINT,
	@ManagerUnitTransferId INT,
	@DataFinal DATETIME,
	@DataTransferencia DATETIME,
	@MonthlyDepreciationId INT
	AS
	-- Valores para testes
	--SET @CodigoUge = 380273;
	--SET @CodigoMaterial = 2409860; 
	BEGIN
	SET DATEFORMAT dmy;
	SET LANGUAGE 'Brazilian'
	DECLARE @CurrentMonth SMALLINT=0;
	DECLARE @Contador SMALLINT = 0;
	DECLARE @ManagerUnitId INT;
	DECLARE @DataAtual AS DATETIME;
	DECLARE @VidaUtilInicio AS SMALLINT;
	DECLARE @DepreciacaoAcumulada AS DECIMAL(18,2);
	DECLARE @DepreciacaoMensal AS DECIMAL(18,2);
	DECLARE @ValorResidual AS DECIMAL(18,2);
	DECLARE @PorcetagemResidual AS DECIMAL(18,10);
	DECLARE @QuantidadeJaDepreciada AS INT;
	DECLARE @PorcetagemDepreciacaoMensal AS DECIMAL(18,10)
	DECLARE @Desdobro AS DECIMAL(18,10);
	DECLARE @DataDecreto AS DATETIME = NULL;
	DECLARE @Decreto AS BIT =  0;
	DECLARE @ContadorDecreto AS SMALLINT = 0;
	DECLARE @ManagerUnitIdTransfer INT;
	DECLARE @CurrentMonthSequencia SMALLINT=0;
	DECLARE @DataMovimentoTransferencia DATETIME = NULL;

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  


	BEGIN TRY

		SET @ManagerUnitId = (SELECT TOP 1 [Id] FROM [dbo].[ManagerUnit] WHERE [Code] = @CodigoUge);

				--- Retornar a vida util do item de material
		SET @VidaUtilInicio = (SELECT TOP 1 [gr].[LifeCycle] 
							FROM [dbo].[MaterialGroup] [gr]
							INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
							WHERE [ast].[AssetStartId] = @AssetStartId
							); 
		SET @DataDecreto = (SELECT TOP 1 [ast].[MovimentDate] 
								FROM [dbo].[Asset] [ast]
								WHERE [ast].[flagDecreto]  = 1
								AND [ast].[AssetStartId] = @AssetStartId
								ORDER BY [ast].[Id] ASC);
		

		IF(@DataIncorporacao < @DataAquisicao OR @DataTransferencia IS NOT NULL)
		BEGIN
			SET @DataAtual = @DataIncorporacao;
		END;
		ELSE
			BEGIN
				SET @DataAtual = @DataAquisicao;
			END;
		IF(@DataTransferencia IS NOT NULL)
		BEGIN
			SET @CurrentMonthSequencia =(SELECT MAX(CurrentMonth)
							FROM [dbo].[MonthlyDepreciation] 
							WHERE [AssetStartId] = @AssetStartId);

			IF(@CurrentMonthSequencia IS NOT NULL)
			BEGIN
				SET @DataAtual = @DataTransferencia;
			END;
		END;

		IF(@CurrentMonthSequencia IS NULL)
		BEGIN 
			SET @CurrentMonthSequencia = 0;
		END;

		--- Loop para criar as linhas calculadas
		WHILE(@Contador <= @VidaUtil)
		BEGIN
			BEGIN TRY


			SET @DataMovimentoTransferencia = (SELECT TOP 1 MovimentDate 
												FROM [dbo].[AssetMovements]  
												WHERE [AssetId] = @AssetStartId
												  AND [ManagerUnitId] = @ManagerUnitId
												  AND [AssetTransferenciaId] IS NOT NULL);
			IF(@DataMovimentoTransferencia IS NOT NULL)
			BEGIN
				IF(YEAR(@DataMovimentoTransferencia) = YEAR(@DataAtual) AND MONTH(@DataMovimentoTransferencia) = MONTH(@DataAtual))
				BEGIN
					BREAK;
				END
			END;

			SET @QuantidadeJaDepreciada = (SELECT COUNT(Id) FROM [dbo].[MonthlyDepreciation] 
												WHERE [AssetStartId] = @AssetStartId
													AND [CurrentDate]= @DataAtual
													AND [ManagerUnitId] = @ManagerUnitId
													AND [MaterialItemCode] = @CodigoMaterial)
		
				IF(@Contador = 0)
					BEGIN
						SET @Desdobro =0;
						SET @DepreciacaoAcumulada = 0;
						SET @PorcetagemDepreciacaoMensal = (SELECT TOP 1 [gr].[RateDepreciationMonthly] 
													FROM [dbo].[MaterialGroup] [gr]
													INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
													WHERE [ast].[AssetStartId] = @AssetStartId
													); 
				
						SET @PorcetagemResidual = (SELECT TOP 1 [gr].[ResidualValue] 
														FROM [dbo].[MaterialGroup] [gr]
														INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
													WHERE [ast].[AssetStartId] = @AssetStartId); 
				
						

						
						SET @ValorResidual =ROUND((@ValorAquisicao * ROUND(@PorcetagemResidual,2,1)) / 100,2,1);
						SET @DepreciacaoMensal = ROUND((@ValorAquisicao - @ValorResidual) / @VidaUtilInicio,2,1);
						
						
						IF(ISNULL(@QuantidadeJaDepreciada,0) < 1)
						BEGIN
							INSERT INTO [dbo].[MonthlyDepreciation]
									([AssetStartId]
									,[ManagerUnitId]
									,[InitialId]
									,[MaterialItemCode]
									,[AcquisitionDate]
									,[CurrentDate]
									,[DateIncorporation]
									,[LifeCycle]
									,[CurrentMonth]
									,[ValueAcquisition]
									,[CurrentValue]
									,[ResidualValue]
									,[RateDepreciationMonthly]
									,[AccumulatedDepreciation]
									,[UnfoldingValue]
									,[Decree]
									,[ManagerUnitTransferId]
									,[MonthlyDepreciationId])
								VALUES(@AssetStartId
										,@ManagerUnitId 
										,@InitialId
										,@CodigoMaterial
										,@DataAquisicao
										,@DataAtual
										,@DataIncorporacao
										,@VidaUtilInicio
										,@Contador
										,@ValorAquisicao
										,@ValorAtual
										,@ValorResidual
										,@DepreciacaoMensal
										,@DepreciacaoAcumulada
										,@Desdobro
										,CASE WHEN @DataDecreto IS NULL THEN 0 ELSE 1 END
										,NULL
										,@MonthlyDepreciationId)
							
						END;
						
				

						
							
					END;
				ELSE
					BEGIN
					
						---Transferencia
						--IF(@ManagerUnitTransferId IS NOT NULL)
						--BEGIN
						--	IF(YEAR(@DataTransferencia) = YEAR(@DataAtual) AND MONTH(@DataTransferencia) = MONTH(@DataAtual))
						--	BEGIN
						--		SET @ManagerUnitId = @ManagerUnitTransferId;
						--	END;
						--END;
						--Bem com decreto
						IF(@DataDecreto IS NOT NULL AND YEAR(@DataDecreto) = YEAR(@DataAtual) AND MONTH(@DataDecreto) = MONTH(@DataAtual) )
						BEGIN 
							UPDATE [dbo].[MonthlyDepreciation] SET [CurrentValue] = @ValorAquisicao, [AccumulatedDepreciation] = 0.0
								WHERE [AssetStartId] = @AssetStartId
								AND [MaterialItemCode] = @CodigoMaterial
								AND [ManagerUnitId] = @ManagerUnitId
							    
							SET @ValorAtual =  (SELECT TOP 1 [DepreciationAmountStart] 
														FROM [dbo].[Asset] [ast] 
														WHERE [AssetStartId] = @AssetStartId
														AND [ast].[MovimentDate] = @DataDecreto);
			

							SET @ValorResidual =ROUND((@ValorAquisicao * ROUND(@PorcetagemResidual,2,1)) / 100,2,1);
							SET @ContadorDecreto = @VidaUtilInicio - (@Contador -1);
							IF(@ContadorDecreto =0)
							BEGIN
								SET @ContadorDecreto = 1;
							END;
							SET @DepreciacaoMensal = ROUND((@ValorAquisicao - @ValorResidual) / @ContadorDecreto,2,1);
							SET @DataAtual = @DataDecreto;
							SET @Decreto = 1;
							SET @DepreciacaoAcumulada = 0.0;
							

						END;
				
						SET @DepreciacaoAcumulada = @DepreciacaoMensal + @DepreciacaoAcumulada;
						SET @ValorAtual = (@ValorAtual -@DepreciacaoMensal) - @Desdobro;
				
						IF(@Contador = @VidaUtil)
						BEGIN
							SET @CurrentMonth =(SELECT COUNT(*) -1
											FROM [dbo].[MonthlyDepreciation] 
											WHERE [AssetStartId] = @AssetStartId

											);
							IF(@CurrentMonth <> @VidaUtilInicio)
							BEGIN
								SET @CurrentMonth =(SELECT COUNT(*)
											FROM [dbo].[MonthlyDepreciation] 
											WHERE [AssetStartId] = @AssetStartId
											);
							END;
							IF(@CurrentMonth = @VidaUtilInicio)
							BEGIN
								SET @Desdobro = @ValorAtual - @ValorResidual;
								SET @ValorAtual = @ValorAtual - @Desdobro;
								SET @DepreciacaoAcumulada = @DepreciacaoAcumulada + @Desdobro;
								SET @DepreciacaoMensal = @DepreciacaoMensal + @Desdobro;
							END;
							
						END;
				
						IF(ISNULL(@QuantidadeJaDepreciada,0) < 1)
						BEGIN
							INSERT INTO [dbo].[MonthlyDepreciation]
									([AssetStartId]
									,[ManagerUnitId]
									,[InitialId]
									,[MaterialItemCode]
									,[AcquisitionDate]
									,[CurrentDate]
									,[DateIncorporation]
									,[LifeCycle]
									,[CurrentMonth]
									,[ValueAcquisition]
									,[CurrentValue]
									,[ResidualValue]
									,[RateDepreciationMonthly]
									,[AccumulatedDepreciation]
									,[UnfoldingValue]
									,[Decree]
									,[ManagerUnitTransferId]
									,[MonthlyDepreciationId])
								VALUES(@AssetStartId
										,@ManagerUnitId 
										,@InitialId
										,@CodigoMaterial
										,@DataAquisicao
										,@DataAtual
										,@DataIncorporacao
										,@VidaUtilInicio
										,@CurrentMonthSequencia
										,@ValorAquisicao
										,@ValorAtual
										,@ValorResidual
										,@DepreciacaoMensal
										,@DepreciacaoAcumulada
										,@Desdobro
										,CASE WHEN @DataDecreto IS NULL THEN 0 ELSE 1 END
										,CASE WHEN @CONTADOR = @VidaUtil THEN @ManagerUnitTransferId ELSE NULL END
										,@MonthlyDepreciationId);  
						END;
						

					END;
		
				IF(YEAR(@DataAtual) >= YEAR(@DataFinal) AND MONTH(@DataAtual) >= MONTH(@DataFinal) )
				BEGIN
					SET @Contador = @VidaUtil + 1;

					BREAK;
				END;
							
				SET @DataAtual =(SELECT DATEADD(MONTH,1,@DataAtual));
				SET @Contador = @Contador + 1;
				SET @CurrentMonthSequencia = @CurrentMonthSequencia + 1;

				--Bem com decreto
				IF(@DataDecreto IS NOT NULL)
				BEGIN
					PRINT('Decreto');
				END;

			IF(@DataDecreto IS NOT NULL AND @Decreto = 0)
			BEGIN
				UPDATE [dbo].[MonthlyDepreciation] SET [CurrentValue] = @ValorAquisicao,[AccumulatedDepreciation] = 0.0, [Decree] = 0.0
					WHERE [AssetStartId] = @AssetStartId
					AND [MaterialItemCode] = @CodigoMaterial
					AND [ManagerUnitId] = @ManagerUnitId
			

				SET @ValorAtual = (SELECT TOP 1 [DepreciationAmountStart] 
											FROM [dbo].[Asset] [ast] 
											WHERE [AssetStartId] = @AssetStartId
											AND [ast].[MovimentDate] = @DataDecreto);
			

				SET @ValorResidual =ROUND((@ValorAquisicao * ROUND(@PorcetagemResidual,2,1)) / 100,2,1);
				SET @ContadorDecreto = @VidaUtilInicio - (@Contador -1);
				IF(@ContadorDecreto =0)
				BEGIN
					SET @ContadorDecreto = 1;
				END;
				SET @DepreciacaoMensal = ROUND((@ValorAquisicao - @ValorResidual) / @ContadorDecreto,2,1);
				SET @Desdobro = 0.0;
				SET @DepreciacaoAcumulada = 0.0;

				INSERT INTO [dbo].[MonthlyDepreciation]
						([AssetStartId]
						,[ManagerUnitId]
						,[InitialId]
						,[MaterialItemCode]
						,[AcquisitionDate]
						,[CurrentDate]
						,[DateIncorporation]
						,[LifeCycle]
						,[CurrentMonth]
						,[ValueAcquisition]
						,[CurrentValue]
						,[ResidualValue]
						,[RateDepreciationMonthly]
						,[AccumulatedDepreciation]
						,[UnfoldingValue]
						,[Decree]
						,[ManagerUnitTransferId]
						,[MonthlyDepreciationId])
					VALUES(@AssetStartId
							,@ManagerUnitId 
							,@InitialId
							,@CodigoMaterial
							,@DataAquisicao
							,@DataAtual
							,@DataIncorporacao
							,@VidaUtilInicio
							,@CurrentMonthSequencia
							,@ValorAquisicao
							,@ValorAtual
							,@ValorResidual
							,@DepreciacaoMensal
							,@DepreciacaoAcumulada
							,@Desdobro
							,CASE WHEN @DataDecreto IS NULL THEN 0 ELSE 1 END
							,CASE WHEN @CONTADOR = @VidaUtil THEN @ManagerUnitTransferId ELSE NULL END
							,@MonthlyDepreciationId);  
			END;

			END TRY
			BEGIN CATCH
				SET @DataAtual =(SELECT DATEADD(MONTH,1,@DataAtual));
				SET @Contador = @Contador + 1;
				SET @CurrentMonthSequencia = @CurrentMonthSequencia + 1;
			END CATCH
		END;
	END TRY
	BEGIN CATCH
	SELECT  
		ERROR_NUMBER() AS ErrorNumber  
		,ERROR_SEVERITY() AS ErrorSeverity  
		,ERROR_STATE() AS ErrorState  
		,ERROR_PROCEDURE() AS ErrorProcedure  
		,ERROR_LINE() AS ErrorLine  
		,ERROR_MESSAGE() AS ErrorMessage
		,@CodigoMaterial AS CodigoMaterial;

	END CATCH;
	END;

	IF(@Contador >= @VidaUtil AND @DataDecreto IS NULL)

	BEGIN


	INSERT INTO [dbo].[MonthlyDepreciation]
				([AssetStartId]
				,[ManagerUnitId]
				,[InitialId]
				,[MaterialItemCode]
				,[AcquisitionDate]
				,[CurrentDate]
				,[DateIncorporation]
				,[LifeCycle]
				,[CurrentMonth]
				,[ValueAcquisition]
				,[CurrentValue]
				,[ResidualValue]
				,[RateDepreciationMonthly]
				,[AccumulatedDepreciation]
				,[UnfoldingValue]
				,[Decree]
				,[ManagerUnitTransferId]
				,[MonthlyDepreciationId])
			VALUES(@AssetStartId
					,@ManagerUnitId 
					,@InitialId
					,@CodigoMaterial
					,@DataAquisicao
					,@DataAtual
					,@DataIncorporacao
					,@VidaUtilInicio
					,@CurrentMonthSequencia
					,@ValorAquisicao
					,@ValorAtual
					,@ValorResidual
					,0.0
					,@DepreciacaoAcumulada
					,0.0
					,CASE WHEN @DataDecreto IS NULL THEN 0 ELSE 1 END
					,CASE WHEN @CONTADOR = @VidaUtil THEN @ManagerUnitTransferId ELSE NULL END
					,@MonthlyDepreciationId);  


	END;

	UPDATE [dbo].[Asset] SET ValueUpdate = @ValorAtual
						WHERE [AssetStartId] = @AssetStartId 
							AND [ManagerUnitId] = @ManagerUnitId 
							AND [MaterialItemCode] = @CodigoMaterial 
	RETURN @Contador;


GO
/****** Object:  StoredProcedure [dbo].[SAM_COPIA_DEPRECIACAO_BPS_MESMO_DADOS_INCORPORACAO_EM_LOTE]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SAM_COPIA_DEPRECIACAO_BPS_MESMO_DADOS_INCORPORACAO_EM_LOTE]
(
	@AssetStartId int,
	@AssetStartIdACopiar int
) AS
BEGIN

	insert into MonthlyDepreciation
	(
       [AssetStartId]
      ,[ManagerUnitId]
      ,[MaterialItemCode]
      ,[InitialId]
      ,[AcquisitionDate]
      ,[CurrentDate]
      ,[DateIncorporation]
      ,[LifeCycle]
      ,[CurrentMonth]
      ,[ValueAcquisition]
      ,[CurrentValue]
      ,[ResidualValue]
      ,[RateDepreciationMonthly]
      ,[AccumulatedDepreciation]
      ,[UnfoldingValue]
      ,[Decree]
      ,[ManagerUnitTransferId]
      ,[MonthlyDepreciationId]
      ,[MesAbertoDoAceite]
      ,[QtdLinhaRepetida]
	)
	select 
	   @AssetStartId
      ,[ManagerUnitId]
      ,[MaterialItemCode]
      ,[InitialId]
      ,[AcquisitionDate]
      ,[CurrentDate]
      ,[DateIncorporation]
      ,[LifeCycle]
      ,[CurrentMonth]
      ,[ValueAcquisition]
      ,[CurrentValue]
      ,[ResidualValue]
      ,[RateDepreciationMonthly]
      ,[AccumulatedDepreciation]
      ,[UnfoldingValue]
      ,[Decree]
      ,[ManagerUnitTransferId]
      ,[MonthlyDepreciationId]
      ,[MesAbertoDoAceite]
      ,[QtdLinhaRepetida]
	  from MonthlyDepreciation
	  where AssetStartId = @AssetStartIdACopiar;
END

GO
/****** Object:  StoredProcedure [dbo].[SAM_DELETA_REGISTROS_DEPRECIACAO_REABERTURA]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SAM_DELETA_REGISTROS_DEPRECIACAO_REABERTURA]
(
	 @ManagerUnitId INT
) AS
BEGIN
	
	Declare @ClosingYearMonthReference varchar(6) = (select ManagmentUnit_YearMonthReference from ManagerUnit where Id = @managerUnitId);  
  Declare @DataReferencia datetime;  
  Declare @AnoMesReferencia varchar(8);  
       
  
  --Define os nomes a ser reaberto ---------------------------------------------------------------------------------------------------------------------------------------  
  SET @DataReferencia = DATEADD(month, 0, SUBSTRING(@ClosingYearMonthReference, 1, 4) + SUBSTRING(@ClosingYearMonthReference, 5, 2) + '01')  
  SET @AnoMesReferencia = RIGHT('0000' + CONVERT(varchar(4), DATEPART(year, @DataReferencia)), 4) + RIGHT('00' + CONVERT(varchar(2), DATEPART(month, @DataReferencia)), 2)  
  
  SET DATEFORMAT YMD;
  DELETE FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = @managerUnitId AND [CurrentDate] >= @DataReferencia
END

GO
/****** Object:  StoredProcedure [dbo].[SAM_DEPRECIACAO_UGE]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SAM_DEPRECIACAO_UGE]
	@ManagerUnitId INT,
	@MaterialItemCode INT,
	@AssetStartId INT,
	@DataFinal AS DATETIME,
	@Fechamento AS BIT= 0,
	@Retorno AS NVARCHAR(1000) OUTPUT,
	@Erro AS BIT OUTPUT
AS
---PRINT(@DataFinal)
SET DATEFORMAT YMD;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  

DECLARE @COUNT INT =0;
DECLARE @CONTADOR INT = 0;
DECLARE @TABLEID INT = 0;

DECLARE @COUNTBEM INT =0;
DECLARE @CONTADORBEM INT = 0;
DECLARE @TABLEBEMID INT  = 0;

DECLARE @VIDA_UTIL SMALLINT = 0;
DECLARE @CONTADORVIDAUTIL INT = 0;
DECLARE @CONTADORDECREE INT = 0;
DECLARE @DATATRANSFERENCIA AS DATETIME;
DECLARE @DATAORIGEM AS DATETIME;
DECLARE @BEMJADEPRECIADO AS BIGINT = 0
DECLARE @VIDA_UTIL_DECREE INT = NULL;
DECLARE @ULTIMADEPRECIACAO AS BIT = 0;
DECLARE @CURRENTDATE_MANAGERUNIT DATETIME;
DECLARE @BEMDEPRECIADONAOTOTAL AS BIT = 0;
DECLARE @COUNT_PRIMEIRO_FECHAMENTO INT = 0;
DECLARE  @FINAL BIT = 0;
DECLARE @TRANSFERENCIA_BP DATETIME = NULL;

----DEPRECIAÇÃO
DECLARE @AcquisitionDate DATETIME;
DECLARE @MovimentDate DATETIME
DECLARE @ValueAcquisition DECIMAL(18,2);
DECLARE @RateDepreciationMonthlyGroup AS DECIMAL(18,2);
DECLARE @RateDepreciationMonthly AS DECIMAL(18,2);
DECLARE @AccumulatedDepreciation AS DECIMAL(18,2) = 0;
DECLARE @ResidualValue AS DECIMAL(18,2)
DECLARE @ResidualPorcetagemValue AS DECIMAL(18,2)
DECLARE @CurrentValue DECIMAL(18,2);
DECLARE @Decree BIT =0;
DECLARE @UnfoldingValue DECIMAL(18,2)
DECLARE @CurrentDate AS DATETIME;
DECLARE @CurrentDateInitial AS DATETIME;
DECLARE @LifeCicle INT = 0;
DECLARE @DateDecree DATETIME;
DECLARE @DecreeValue AS DECIMAL(18,2) = 0;
DECLARE @CurrentMonth INT = 0;
DECLARE @QuantidadeDepreciado INT=0;
DECLARE @AssetIdNaOrigem INT = 0;
DECLARE @ManagerUnitDestino INT = NULL;
DECLARE @ManagerUnitOrigem INT  = NULL;
DECLARE @AssetId INT = NULL;
DECLARE @BemTransferidoJaDepreciadoQuantidade INT = NULL;
DECLARE @MesesFaltanteDepreciarDecreto SMALLINT = 0;
DECLARE @DateReverse DATETIME;
DECLARE @ValueAcquisitionCalcDecree DECIMAL(18,2)= NULL;
--Alteração Aqui
Declare @MaximoMesAtual int = 0;
--para BPs incoporados em um mês, e no mesmo mês terem sidos transferidos (sem ter feito qualquer fechamento)
Declare @AssetStartIdNaOrigem int;

---INFORMAÇÕES
DECLARE @InitialId INT = 0
DECLARE @DepreciationAmountStart DECIMAL(18,2) = 0;

DECLARE @TableTransfencia AS TABLE(
	ManagerUnitId INT NOT NULL,
	AssetId INT NOT NULL,
	MovimentDate DATETIME NULL
)

DECLARE @TABLEBEM AS TABLE(
	[Id] INT NOT NULL IDENTITY(1,1),
	[InitialId] INT NOT NULL,
	[AssetStartId] INT NOT NULL,
	[DataAquisicao] DATETIME NOT NULL,
	[DataIncorporacao] DATETIME NOT NULL,
	[ValorAquisicao] DECIMAL(18,2) NOT NULL
)


SET @COUNT = 1;
SET @DATAFINAL =(SELECT DATEADD(MONTH,1,@DATAFINAL));
SET @Retorno ='Depreciação realizada com sucesso!';
SET @Erro = 0;
---PRINT('DATA FINAL 01')

---PRINT(@DataFinal)
BEGIN TRY
WHILE(@CONTADOR < @COUNT )
BEGIN
	BEGIN TRY
		INSERT INTO @TABLEBEM([AssetStartId],[InitialId],[DataAquisicao],[DataIncorporacao],[ValorAquisicao])
		SELECT TOP 1 @AssetStartId,[bem].[InitialId], [bem].[AcquisitionDate],[bem].[MovimentDate],[bem].[ValueAcquisition] 
		   FROM [dbo].[Asset] [bem] 
		   INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [bem].[Id] 
		   WHERE [bem].[AssetStartId] =@AssetStartId 
		     AND [bem].[InitialId] IS NOT NULL
			 AND [bem].[flagVerificado] IS NULL
			 AND [bem].[flagDepreciaAcumulada] = 1 
			 AND [bem].[AssetStartId] IS NOT NULL
			 AND ([bem].[flagAcervo]  IS NULL OR [bem].[flagAcervo] = 0)
			 AND ([bem].[flagTerceiro] = 0 OR  [bem].[flagTerceiro] IS NULL)
			 AND ([bem].[FlagAnimalNaoServico] IS NULL OR [bem].[FlagAnimalNaoServico] =0)
			 AND ([mov].[AssetTransferenciaId] IS NULL OR [mov].[AssetTransferenciaId] = 0)
			 AND ([mov].[FlagEstorno] IS NULL OR [mov].[FlagEstorno] =0)
		   ORDER BY [bem].[Id] ASC
		     
		SET @COUNTBEM = (SELECT COUNT(*) FROM @TABLEBEM);
		WHILE(@CONTADORBEM < @COUNTBEM)
		BEGIN
			BEGIN TRY
				SET @COUNT_PRIMEIRO_FECHAMENTO = (SELECT COUNT(*) FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)


				IF(@COUNT_PRIMEIRO_FECHAMENTO < 1 AND @Fechamento = 1)
				BEGIN
					SET @Fechamento = 0;
				END;

			SET @LifeCicle = (SELECT TOP 1 [gr].[LifeCycle] 
								FROM [dbo].[MaterialGroup] [gr]
								INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
								WHERE [ast].[MaterialItemCode] = @MaterialItemCode);

			SET @AssetId = (SELECT TOP 1 [AssetId] FROM [AssetMovements] WHERE [ManagerUnitId] = @ManagerUnitId AND [MonthlyDepreciationId] = @AssetStartId ORDER BY [Id] DESC);
			IF(@AssetId IS NOT NULL)
			BEGIN
				SET @AssetIdNaOrigem = (SELECT [AssetId] FROM [dbo].[AssetMovements] WHERE AssetTransferenciaId  = @AssetId)
				---PRINT('PASSOU')
				IF(@AssetIdNaOrigem IS NOT NULL)
				BEGIN
					---PRINT('PASSOU 1')
					---PRINT(@LifeCicle)
					SET @QuantidadeDepreciado = (SELECT COUNT(*) FROM [dbo].[MonthlyDepreciation] WHERE AssetStartId = @AssetStartId);
					---PRINT('TESTE 20')
					---PRINT(@AssetStartId)
					---PRINT(@QuantidadeDepreciado)
					
					IF(ISNULL(@QuantidadeDepreciado,0) >= @LifeCicle)
					BEGIN
						---PRINT('PASSOU 2')
						SET @ManagerUnitDestino = (SELECT TOP 1 [ManagerUnitId] FROM [dbo].[MonthlyDepreciation] WHERE AssetStartId = @AssetStartId ORDER BY [Id] DESC);
						IF(@ManagerUnitDestino <> @ManagerUnitId)
						BEGIN
							---PRINT('PASSOU 3')
							SET @BemTransferidoJaDepreciadoQuantidade = (SELECT COUNT(*) [ManagerUnitId] FROM [dbo].[MonthlyDepreciation] WHERE AssetStartId = @AssetStartId AND [ManagerUnitId] = @ManagerUnitDestino);
							IF(ISNULL(@BemTransferidoJaDepreciadoQuantidade,0) < 1)
							BEGIN
								SET @FINAL = 1;
								GOTO TRANSFERENCIA_FINAL
							END;
							ELSE
								BEGIN
									---PRINT('PAROU');
									RETURN 0;
								END;
						END;
						ELSE
							BEGIN
								---PRINT('AQUI');
								RETURN 0;
							END;
					END;
					
				END;
								
			END;

			---PRINT('PASSOU 10');

			SET @TABLEBEMID =(SELECT TOP 1 [Id] FROM @TABLEBEM);

			SET @AcquisitionDate = (SELECT  [DataAquisicao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			SET @MovimentDate = (SELECT  [DataIncorporacao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			SET @ValueAcquisition  = (SELECT  [ValorAquisicao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			SET @InitialId  = (SELECT  [InitialId] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			

			SET @QuantidadeDepreciado = (SELECT COUNT(*) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)
			
			IF(@QuantidadeDepreciado >= @LifeCicle)
			BEGIN
				SET @Retorno ='Bem Material já éstá totalmente depreciado.'
				RETURN 0;
			END
			
			---PRINT('PASSOU 11');

			SET @VIDA_UTIL = @LifeCicle;

			SET @DateDecree =(SELECT TOP 1 [ast].[MovimentDate] 
									FROM [dbo].[Asset] [ast]
									INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
									WHERE [ast].[flagDecreto]  = 1
									AND [ast].[Id] =@AssetId --@AssetStartId
									AND [ast].[ManagerUnitId] = @ManagerUnitId 
									ORDER BY [ast].[Id] DESC);

			SET @UnfoldingValue =0;
			SET @AccumulatedDepreciation = 0;

				
			SET @ResidualValue = (SELECT TOP 1 [gr].[ResidualValue] 
											FROM [dbo].[MaterialGroup] [gr]
											INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
										WHERE [ast].[MaterialItemCode] = @MaterialItemCode); 
				
			SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] 
			                      WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)

			SET @CurrentDate = (SELECT MAX(CurrentDate) FROM [MonthlyDepreciation] 
			                     WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)

			IF(@CurrentDate IS NULL)
			BEGIN
			SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
										FROM [dbo].[AssetMovements]  
										WHERE [AssetId] = @AssetStartId
											AND [AssetTransferenciaId] IS NOT NULL
										ORDER BY [MovimentDate] ASC, [Id] DESC
											);
			END
			ELSE
				BEGIN
					SET @DATATRANSFERENCIA = (SELECT TOP 1 [mov].[MovimentDate] 
														FROM [dbo].[AssetMovements] [mov]  
														INNER JOIN [dbo].[Asset] [bem] ON [bem].[Id] = [mov].[AssetId] 
														WHERE [bem].[AssetStartId] = @AssetStartId 
															AND [mov].[AssetTransferenciaId] IS NOT NULL
															AND YEAR([mov].[MovimentDate]) = YEAR(@CurrentDate)
															AND MONTH([mov].[MovimentDate]) = MONTH(@CurrentDate)
														ORDER BY [mov].[MovimentDate] ASC, [mov].[Id] DESC
															);
					END

			IF(@DATATRANSFERENCIA IS NOT NULL)
			BEGIN
				select top 1 @AssetStartIdNaOrigem = AssetStartId
				from [dbo].[Asset] where [Id] = @AssetStartId;

				IF(@AssetStartIdNaOrigem IS NULL)
				BEGIN
					SET @ManagerUnitId = (SELECT TOP 1 am.[ManagerUnitId]
											FROM [dbo].[AssetMovements] am
											inner join [dbo].[Asset] a
											ON a.[Id] = am.[AssetId]
											WHERE a.[AssetStartId] = @AssetStartId
											  AND YEAR(am.[MovimentDate]) = YEAR(@DATATRANSFERENCIA)
											  AND MONTH(am.[MovimentDate]) = MONTH(@DATATRANSFERENCIA)
											ORDER BY am.[MovimentDate] ASC, am.[Id] DESC
											);
				END
				ELSE
				BEGIN
					SET @ManagerUnitId = (SELECT TOP 1 [ManagerUnitId] 
											FROM [dbo].[AssetMovements]  
											WHERE [AssetId] = @AssetStartId
											  AND YEAR([MovimentDate]) = YEAR(@DATATRANSFERENCIA)
											  AND MONTH([MovimentDate]) = MONTH(@DATATRANSFERENCIA)
											ORDER BY [MovimentDate] ASC, [Id] DESC
												);
				END
			END;

			SET @DATAORIGEM = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] = @AssetId
													ORDER BY [Id] DESC
													  );

			SET @DateReverse  = (SELECT TOP 1 [DataEstorno]
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] IS NOT NULL
													  AND ([FlagEstorno] IS NULL OR [FlagEstorno] = 0)
													ORDER BY [Id] DESC
													  );	
			

			SET @CURRENTDATE_MANAGERUNIT = (SELECT MAX(CurrentDate) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode AND [ManagerUnitId] = @ManagerUnitId)
			SET @CONTADORVIDAUTIL = 0;
			
			IF(@CurrentDate IS NOT NULL)
			BEGIN
				--SET @VIDA_UTIL = @VIDA_UTIL - DATEDIFF(MONTH,@AcquisitionDate,@CurrentDate);
				SET @CONTADORVIDAUTIL =(SELECT MAX(CurrentMonth) + 1 FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode); --(@LifeCicle - (@VIDA_UTIL - DATEDIFF(MONTH,@AcquisitionDate,@CurrentDate)) + 1 );
				SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));

			
							
				
			END
			
			IF(@CURRENTDATE_MANAGERUNIT IS NOT NULL)
			BEGIN

				IF((@LifeCicle -  DATEDIFF(MONTH,@AcquisitionDate,@CURRENTDATE_MANAGERUNIT)) < 1)
				BEGIN
					SET @FINAL = 1
					GOTO TRANSFERENCIA_FINAL;
				END


			END;
			
			---PRINT('PASSOU 12');
			--Calcula o valor a ser depreciano ao mês
			SET @ResidualPorcetagemValue =ROUND((@ValueAcquisition * ROUND(@ResidualValue,2,1)) / 100,2,1);
			SET @RateDepreciationMonthly = ROUND((@ValueAcquisition - @ResidualPorcetagemValue) / @VIDA_UTIL,2,1);

			IF(@CurrentDate IS NOT NULL)
			BEGIN
				SET @ResidualPorcetagemValue =  (SELECT TOP 1 ResidualValue FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
												
				SET @RateDepreciationMonthly =  (SELECT TOP 1 RateDepreciationMonthly FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
				SET @AccumulatedDepreciation =  (SELECT MAX(AccumulatedDepreciation) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) + @RateDepreciationMonthly;
				SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) - @RateDepreciationMonthly;
				SET @BEMDEPRECIADONAOTOTAL = 1
			END;

			---Executa sempre em penultimo
			IF(@CurrentValue IS NULL)
			BEGIN
				SET @CurrentValue = @ValueAcquisition;
			END;

						
			IF(@CurrentDate IS NULL)
			BEGIN
				---Alterar o dia da data atual para 10, assim padronizando o dia.
				SET @CurrentDate = CAST(YEAR(@AcquisitionDate) AS VARCHAR(4)) + '-' + CAST(MONTH(@AcquisitionDate) AS VARCHAR(2)) + '-' + '10';
			END
			
			---Executa sempre em ultimo
			IF(@DateDecree IS NOT NULL)
			BEGIN
					SET @MesesFaltanteDepreciarDecreto =  DATEDIFF(MONTH,@AcquisitionDate,@DateDecree);
					SET @Decree  = 1;
					SET @MesesFaltanteDepreciarDecreto = IIF(@MesesFaltanteDepreciarDecreto < 0, 0,@MesesFaltanteDepreciarDecreto);
		
					IF(@VIDA_UTIL - @MesesFaltanteDepreciarDecreto > 0)
					BEGIN
						SET @DecreeValue =  (SELECT TOP 1 [ValueAcquisition] 
									FROM [dbo].[Asset] [ast] 
									INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
									WHERE [AssetStartId] = @AssetStartId
										AND [ast].[MovimentDate] = @DateDecree
									ORDER BY [mov].[Id] DESC
									);


					
						SET @ValueAcquisitionCalcDecree = @DecreeValue -- @ResidualValue;
						SET @CurrentValue = @ValueAcquisitionCalcDecree;

						SET @CurrentDate = @DateDecree;
						SET @CONTADORVIDAUTIL =  @MesesFaltanteDepreciarDecreto + 1;;

						SET @ResidualPorcetagemValue =ROUND((@ValueAcquisitionCalcDecree * ROUND(@ResidualValue,2,1)) / 100,2,1);
						SET @RateDepreciationMonthly = ROUND((@ValueAcquisitionCalcDecree - @ResidualPorcetagemValue)  / (@VIDA_UTIL- @MesesFaltanteDepreciarDecreto),2,1);

						SET @AccumulatedDepreciation = @RateDepreciationMonthly + @AccumulatedDepreciation;
						SET @VIDA_UTIL_DECREE = @VIDA_UTIL;

						SET @TRANSFERENCIA_BP = (SELECT [MovimentDate] FROM [AssetMovements] WHERE [MonthlyDepreciationId] = @AssetStartId AND [AssetTransferenciaId] IS NOT NULL)

						IF(@TRANSFERENCIA_BP IS NOT NULL)
						BEGIN
							SET @CurrentDate = @TRANSFERENCIA_BP;
						END;
						
					END;
					ELSE
						BEGIN
							
							---- Decreto realizado com o bem totalmente depreciado, então seta @VIDA_UTIL = -1 para não entrar no loop de calacular
							
							SET @CurrentValue =  (SELECT TOP 1 [ValueAcquisition] 
									FROM [dbo].[Asset] [ast] 
									INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
									WHERE [AssetStartId] = @AssetStartId
										AND [ast].[MovimentDate] = @DateDecree
									ORDER BY [ast].[Id] DESC
									);	
										
							SET @CurrentDate = @DateDecree;

							SET @ResidualPorcetagemValue =ROUND((@CurrentValue * ROUND(@ResidualValue,2,1)) / 100,2,1);
							SET @RateDepreciationMonthly = ROUND((@CurrentValue - @ResidualPorcetagemValue) ,2,1) / @VIDA_UTIL;

							SET @AccumulatedDepreciation =(SELECT TOP 1 [AccumulatedDepreciation] 
											 FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] DESC)
							SET @CONTADORVIDAUTIL = @VIDA_UTIL;
							SET @FINAL = 1;
							
							---PRINT('PASSOU 20');
							INSERT INTO MonthlyDepreciation
								([AssetStartId]
								,[ManagerUnitId]
								,[InitialId]
								,[MaterialItemCode]
								,[AcquisitionDate]
								,[CurrentDate]
								,[DateIncorporation]
								,[LifeCycle]
								,[CurrentMonth]
								,[ValueAcquisition]
								,[CurrentValue]
								,[ResidualValue]
								,[RateDepreciationMonthly]
								,[AccumulatedDepreciation]
								,[UnfoldingValue]
								,[Decree]
								,[ManagerUnitTransferId]
								,[MonthlyDepreciationId])
							VALUES(@AssetStartId
									,@ManagerUnitId 
									,@InitialId
									,@MaterialItemCode
									,@AcquisitionDate
									,@CurrentDate
									,@MovimentDate
									,@LifeCicle
									,@LifeCicle + 1
									,@ValueAcquisition
									,@CurrentValue
									,@ResidualPorcetagemValue
									,0.0
									,0.0
									,0.0
									,1
									,NULL
									,@AssetStartId)

									SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
										FROM [dbo].[AssetMovements]  
										WHERE [AssetId] = @AssetStartId
											AND [ManagerUnitId] = @ManagerUnitId

											AND [AssetTransferenciaId] IS NOT NULL
										ORDER BY [Id] DESC
											);

									IF(@DATATRANSFERENCIA IS NOT NULL)
									BEGIN
										SET @ManagerUnitDestino = (SELECT TOP 1 SourceDestiny_ManagerUnitId 
																	FROM [dbo].[AssetMovements]  
																	WHERE [AssetId] = @AssetStartId
																		AND [ManagerUnitId] = @ManagerUnitId
																		AND [AssetTransferenciaId] IS NOT NULL
																	ORDER BY [Id] DESC
																		);
										
										INSERT INTO [dbo].[MonthlyDepreciation]
													([AssetStartId]
													,[ManagerUnitId]
													,[InitialId]
													,[MaterialItemCode]
													,[AcquisitionDate]
													,[CurrentDate]
													,[DateIncorporation]
													,[LifeCycle]
													,[CurrentMonth]
													,[ValueAcquisition]
													,[CurrentValue]
													,[ResidualValue]
													,[RateDepreciationMonthly]
													,[AccumulatedDepreciation]
													,[UnfoldingValue]
													,[Decree]
													,[ManagerUnitTransferId]
													,[MonthlyDepreciationId])
											  SELECT TOP 1 [AssetStartId]
													,@ManagerUnitDestino
													,[InitialId]
													,[MaterialItemCode]
													,[AcquisitionDate]
													,[CurrentDate]
													,[DateIncorporation]
													,[LifeCycle]
													,[CurrentMonth]
													,[ValueAcquisition]
													,[CurrentValue]
													,[ResidualValue]
													,[RateDepreciationMonthly]
													,[AccumulatedDepreciation]
													,[UnfoldingValue]
													,[Decree]
													,[ManagerUnitTransferId]
													,[MonthlyDepreciationId]
											  FROM [dbo].[MonthlyDepreciation]
											  WHERE [AssetStartId] = @AssetStartId
											  ORDER BY [Id] DESC
									END;
									ELSE
										BEGIN
											SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
														AND SourceDestiny_ManagerUnitId = @ManagerUnitId
														AND [AssetTransferenciaId] IS NOT NULL
													ORDER BY [Id] DESC);
											IF(@DATATRANSFERENCIA IS NOT NULL)
											BEGIN
												SET @ManagerUnitDestino = (SELECT TOP 1 [ManagerUnitId] 
																		FROM [dbo].[AssetMovements]  
																		WHERE [AssetId] = @AssetStartId
																			AND SourceDestiny_ManagerUnitId = @ManagerUnitId
																			AND [AssetTransferenciaId] IS NOT NULL
																		ORDER BY [Id] DESC
																			);
										
												INSERT INTO [dbo].[MonthlyDepreciation]
															([AssetStartId]
															,[ManagerUnitId]
															,[InitialId]
															,[MaterialItemCode]
															,[AcquisitionDate]
															,[CurrentDate]
															,[DateIncorporation]
															,[LifeCycle]
															,[CurrentMonth]
															,[ValueAcquisition]
															,[CurrentValue]
															,[ResidualValue]
															,[RateDepreciationMonthly]
															,[AccumulatedDepreciation]
															,[UnfoldingValue]
															,[Decree]
															,[ManagerUnitTransferId]
															,[MonthlyDepreciationId])
													  SELECT TOP 1 [AssetStartId]
															,@ManagerUnitDestino
															,[InitialId]
															,[MaterialItemCode]
															,[AcquisitionDate]
															,[CurrentDate]
															,[DateIncorporation]
															,[LifeCycle]
															,[CurrentMonth]
															,[ValueAcquisition]
															,[CurrentValue]
															,[ResidualValue]
															,[RateDepreciationMonthly]
															,[AccumulatedDepreciation]
															,[UnfoldingValue]
															,[Decree]
															,[ManagerUnitTransferId]
															,[MonthlyDepreciationId]
													  FROM [dbo].[MonthlyDepreciation]
													  WHERE [AssetStartId] = @AssetStartId
													  ORDER BY [Id] DESC
											END;
										END;
					END;
				
						   
			END
			
			---PRINT('PASSOU 14');
			SET @DepreciationAmountStart = @CurrentValue;
			IF(@Fechamento =1)
			BEGIN
				DECLARE @VidaUtilManagerUnit INT = 0;
				SET @VIDA_UTIL = @CONTADORVIDAUTIL;
				SET @RateDepreciationMonthly =  (SELECT TOP 1 RateDepreciationMonthly FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
				SET @AccumulatedDepreciation =  (SELECT MAX(AccumulatedDepreciation) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) + @RateDepreciationMonthly;
				SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) - @RateDepreciationMonthly;

				IF(@ValueAcquisitionCalcDecree IS NOT NULL)
					BEGIN
						SET @ResidualPorcetagemValue =  (SELECT TOP 1 ResidualValue FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
				
						SET @CurrentDate =  (SELECT TOP 1 CurrentDate FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
						SET @CONTADORVIDAUTIL =  (SELECT TOP 1 CurrentMonth + 1 FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);

						SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
						SET @VIDA_UTIL = @VIDA_UTIL_DECREE;
			
					END;


				SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] IS NOT NULL
													ORDER BY [Id] DESC
													  );

				SET @DATAORIGEM = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] = @AssetId
													ORDER BY [Id] DESC
													  );
				---DESTINO
				IF(@DATAORIGEM IS NOT NULL)
				BEGIN
					---PRINT('DESTINO');

					SET @VidaUtilManagerUnit = DATEDIFF(MONTH,@AcquisitionDate,@DATAORIGEM) + 1;

					SET @QuantidadeDepreciado = (SELECT COUNT(*) FROM [dbo].[MonthlyDepreciation] WHERE AssetStartId = @AssetStartId AND [ManagerUnitId] = @ManagerUnitId);
					IF(@QuantidadeDepreciado >= @VidaUtilManagerUnit)
					BEGIN
						RETURN 0;
					END;
					ELSE
						BEGIN
							SET @VIDA_UTIL = @CONTADORVIDAUTIL;
							SET @RateDepreciationMonthly =  (SELECT TOP 1 RateDepreciationMonthly 
															  FROM [MonthlyDepreciation] 
															  WHERE [AssetStartId] = @AssetStartId 
															    AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
							---PRINT('RateDepreciationMonthly');
							---PRINT(@RateDepreciationMonthly)
							SET @AccumulatedDepreciation =  (SELECT MAX(AccumulatedDepreciation) 
							                                   FROM [MonthlyDepreciation] 
															   WHERE [AssetStartId] = @AssetStartId 
															     AND [MaterialItemCode] = @MaterialItemCode) + @RateDepreciationMonthly;
							SET @CurrentValue = (SELECT MIN([CurrentValue]) 
												   FROM [MonthlyDepreciation] 
												   WHERE [AssetStartId] = @AssetStartId 
												     AND [MaterialItemCode] = @MaterialItemCode) - @RateDepreciationMonthly;

						END;
				END;
				---ORIGEM
				IF(@DATATRANSFERENCIA IS NOT NULL)
				BEGIN
					---PRINT('ORIGEM');

					SET @VidaUtilManagerUnit = DATEDIFF(MONTH,@AcquisitionDate,@DATATRANSFERENCIA) + 1;
					SET @QuantidadeDepreciado = (SELECT COUNT(*) FROM [dbo].[MonthlyDepreciation] WHERE AssetStartId = @AssetStartId AND [ManagerUnitId] = @ManagerUnitId);
					---PRINT(@QuantidadeDepreciado)
					---PRINT(@VidaUtilManagerUnit)
					IF(@QuantidadeDepreciado >= @VidaUtilManagerUnit)
					BEGIN
						RETURN 0;
					END;
					ELSE
						BEGIN
							SET @VIDA_UTIL = @CONTADORVIDAUTIL;
							SET @RateDepreciationMonthly =  (SELECT TOP 1 RateDepreciationMonthly 
															  FROM [MonthlyDepreciation] 
															  WHERE [AssetStartId] = @AssetStartId 
															    AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);

							SET @AccumulatedDepreciation =  (SELECT MAX(AccumulatedDepreciation) 
							                                   FROM [MonthlyDepreciation] 
															   WHERE [AssetStartId] = @AssetStartId 
															     AND [MaterialItemCode] = @MaterialItemCode) + @RateDepreciationMonthly;
							SET @CurrentValue = (SELECT MIN([CurrentValue]) 
												   FROM [MonthlyDepreciation] 
												   WHERE [AssetStartId] = @AssetStartId 
												     AND [MaterialItemCode] = @MaterialItemCode) - @RateDepreciationMonthly;

						END;

				END;
			END;
				
			
			---PRINT('VIDA UTIL');
			---PRINT(@VIDA_UTIL);
			WHILE(@CONTADORVIDAUTIL <= @VIDA_UTIL)
			BEGIN

				--IF(@CONTADORVIDAUTIL = 119)
				--BEGIN 
				--	---PRINT 'OK';
				--END
				
				IF(YEAR(@DataFinal) = YEAR(@CurrentDate) AND MONTH(@DataFinal) = MONTH(@CurrentDate))
				BEGIN
					---PRINT('DATA FINAL');
					BREAK;
				END
			
				---Verifica se na data atual foi realizado uma transferência
				IF(@DATATRANSFERENCIA IS NOT NULL AND @CONTADORVIDAUTIL > 0)
				BEGIN
					--IF(YEAR(DATEADD(MONTH,1,@DATATRANSFERENCIA))  = YEAR(@CurrentDate) AND MONTH(DATEADD(MONTH,1,@DATATRANSFERENCIA)) = MONTH(@CurrentDate))
					---PRINT('PASSOU TRANSFERENCIA')
					---PRINT(@DATATRANSFERENCIA)
					IF(YEAR(@DATATRANSFERENCIA) = YEAR(@CurrentDate) AND MONTH(@DATATRANSFERENCIA) = MONTH(@CurrentDate))
					BEGIN
						---PRINT('TRANSFERÊNCIA PASSOU')
						---PRINT(@DATATRANSFERENCIA)
						---PRINT(@CurrentDate)
						---BREAK;
						---Altera Id da UGE para o destino
						SET @ManagerUnitId = (SELECT TOP 1 [SourceDestiny_ManagerUnitId] 
						                       FROM [dbo].[AssetMovements] [mov]  
												INNER JOIN [dbo].[Asset] [bem] ON [bem].[Id] = [mov].[AssetId] 
												WHERE [bem].[AssetStartId] = @AssetStartId 
													AND [mov].[AssetTransferenciaId] IS NOT NULL
													AND YEAR([mov].[MovimentDate]) = YEAR(@CurrentDate)
													AND MONTH([mov].[MovimentDate]) = MONTH(@CurrentDate)
												ORDER BY [mov].[MovimentDate] ASC, [mov].[Id] DESC)
						---PRINT(@ManagerUnitId);
						SET @DATATRANSFERENCIA = NULL;
					END
					
				END;
				---PRINT('DATA FINAL')
				---PRINT(@DataFinal);
				-----verifica se o bp foi incoprporado por transferência, caso ok depreciar a alterar Id da Uge para a origem
				--IF(@DATAORIGEM IS NOT NULL AND @CONTADORVIDAUTIL > 0)
				--BEGIN
				--	SET @ManagerUnitId = (SELECT TOP 1 [ManagerUnitId] FROM [dbo].[AssetMovements] 
				--	     WHERE [SourceDestiny_ManagerUnitId] = @ManagerUnitId AND [MonthlyDepreciationId] = @AssetStartId AND [AssetTransferenciaId] IS NOT NULL ORDER BY [Id] DESC)
					
				--	SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
				--									FROM [dbo].[AssetMovements]  
				--									WHERE [AssetId] = @AssetStartId
				--									  AND [ManagerUnitId] = @ManagerUnitId
				--									  AND [AssetTransferenciaId] IS NOT NULL
				--									ORDER BY [Id] DESC
				--									  );
				--	SET @DATAORIGEM = NULL;
				--END
			
				---Verifica se na data atual foi realizado um estorno
				IF(@DateReverse IS NOT NULL)
				BEGIN
					IF(YEAR(@DateReverse)  = YEAR(@CurrentDate) AND MONTH(@DateReverse) = MONTH(@CurrentDate))
					BEGIN
						BREAK;
					END
					
				END;

				
				SET @BEMJADEPRECIADO = (SELECT TOP 1 [Id] FROM [dbo].[MonthlyDepreciation] 
													WHERE [AssetStartId] = @AssetStartId
														AND [CurrentDate]= @CurrentDate
														AND [ManagerUnitId] = @ManagerUnitId
														AND [MaterialItemCode] = @MaterialItemCode)

				IF(@BEMJADEPRECIADO IS NULL)
				
				IF(@CONTADORVIDAUTIL = 0)
				BEGIN
					---Verifica se o bem para a data atual já foi depreciado, caso sim não é realizado o insert, assim evitando error de inclusão
				
					IF(ISNULL(@BEMJADEPRECIADO,0) < 1)
					BEGIN
						
							---Alterar o dia do primeiro registro "linha 0".
							SET @CurrentDateInitial =(SELECT DATEADD(DAY,-1,@CurrentDate));
							
							UPDATE [dbo].[Asset] SET [DepreciationDateStart] = @CurrentDate, [DepreciationAmountStart] = @DepreciationAmountStart
												WHERE [AssetStartId] = @AssetStartId AND [DepreciationDateStart] IS NULL;	
							

							INSERT INTO [dbo].[MonthlyDepreciation]
										([AssetStartId]
										,[ManagerUnitId]
										,[InitialId]
										,[MaterialItemCode]
										,[AcquisitionDate]
										,[CurrentDate]
										,[DateIncorporation]
										,[LifeCycle]
										,[CurrentMonth]
										,[ValueAcquisition]
										,[CurrentValue]
										,[ResidualValue]
										,[RateDepreciationMonthly]
										,[AccumulatedDepreciation]
										,[UnfoldingValue]
										,[Decree]
										,[ManagerUnitTransferId]
										,[MonthlyDepreciationId])
									VALUES(@AssetStartId
											,@ManagerUnitId 
											,@InitialId
											,@MaterialItemCode
											,@AcquisitionDate
											,@CurrentDateInitial
											,@MovimentDate
											,@LifeCicle
											,@CONTADORVIDAUTIL
											,@ValueAcquisition
											,@CurrentValue
											,@ResidualPorcetagemValue
											,@RateDepreciationMonthly
											,@AccumulatedDepreciation
											,@UnfoldingValue
											,@Decree
											,NULL
											,@AssetStartId)
					END;
			

				END;
				ELSE
					BEGIN
						---PRINT('CURRENT MONTH');
						---PRINT(@CONTADORVIDAUTIL);

						IF(@CONTADORVIDAUTIL = @VIDA_UTIL)
						BEGIN
							IF(@BEMDEPRECIADONAOTOTAL = 1)
							BEGIN
								SET @CurrentMonth =(SELECT COUNT(*) + @CONTADORVIDAUTIL
											FROM MonthlyDepreciation 
											WHERE [AssetStartId] = @AssetStartId
											);
							END;
							ELSE
								BEGIN
									SET @CurrentMonth =(SELECT COUNT(*) 
											FROM MonthlyDepreciation 
											WHERE [AssetStartId] = @AssetStartId
										);
								END;
						
							IF(@CurrentMonth >= @LifeCicle AND @Fechamento = 0)
							BEGIN
								SET @UnfoldingValue = @CurrentValue - @ResidualPorcetagemValue;
								SET @CurrentValue = @CurrentValue - @UnfoldingValue;
								SET @AccumulatedDepreciation = @AccumulatedDepreciation + @UnfoldingValue;
								SET @RateDepreciationMonthly = @RateDepreciationMonthly + @UnfoldingValue;
								SET @ULTIMADEPRECIACAO = 1;
							END;

							IF(@DateDecree IS NOT NULL AND (@MesesFaltanteDepreciarDecreto + @CurrentMonth) >= @LifeCicle AND @Fechamento = 0)
							BEGIN
								SET @UnfoldingValue = @CurrentValue - @ResidualPorcetagemValue;
								SET @CurrentValue = @CurrentValue - @UnfoldingValue;
								SET @AccumulatedDepreciation = @AccumulatedDepreciation + @UnfoldingValue;
								SET @RateDepreciationMonthly = @RateDepreciationMonthly + @UnfoldingValue;
								SET @ULTIMADEPRECIACAO = 1;
							END;
							
						END;
						
						IF(ISNULL(@BEMJADEPRECIADO,0) < 1)
						BEGIN
							---PRINT('RateDepreciationMonthly')
							---PRINT(@RateDepreciationMonthly)
							INSERT INTO [dbo].[MonthlyDepreciation]
										([AssetStartId]
										,[ManagerUnitId]
										,[InitialId]
										,[MaterialItemCode]
										,[AcquisitionDate]
										,[CurrentDate]
										,[DateIncorporation]
										,[LifeCycle]
										,[CurrentMonth]
										,[ValueAcquisition]
										,[CurrentValue]
										,[ResidualValue]
										,[RateDepreciationMonthly]
										,[AccumulatedDepreciation]
										,[UnfoldingValue]
										,[Decree]
										,[ManagerUnitTransferId]
										,[MonthlyDepreciationId])
									VALUES(@AssetStartId
											,@ManagerUnitId 
											,@InitialId
											,@MaterialItemCode
											,@AcquisitionDate
											,@CurrentDate
											,@MovimentDate
											,@LifeCicle
											,@CONTADORVIDAUTIL
											,@ValueAcquisition
											,@CurrentValue
											,@ResidualPorcetagemValue
											,@RateDepreciationMonthly
											,@AccumulatedDepreciation
											,@UnfoldingValue
											,@Decree
											,NULL
											,@AssetStartId);

								
						END;
				

					END;
			
				--Alteração Aqui
				SET @MaximoMesAtual = @CONTADORVIDAUTIL;

				IF(@ULTIMADEPRECIACAO  = 1)
				BEGIN
					SET @MaximoMesAtual = @LifeCicle;
					BREAK;
				END;
			
				IF(@CONTADORVIDAUTIL =0)
				BEGIN
					SET @CurrentDate =(SELECT DATEADD(DAY,1,@CurrentDate));
				END;
				ELSE
					BEGIN
						SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
						
					END;

				SET @DATATRANSFERENCIA = (SELECT TOP 1 [mov].[MovimentDate] 
													FROM [dbo].[AssetMovements] [mov]  
													INNER JOIN [dbo].[Asset] [bem] ON [bem].[Id] = [mov].[AssetId] 
													WHERE [bem].[AssetStartId] = @AssetStartId 
														AND [mov].[AssetTransferenciaId] IS NOT NULL
														AND YEAR([mov].[MovimentDate]) = YEAR(@CurrentDate)
														AND MONTH([mov].[MovimentDate]) = MONTH(@CurrentDate)
													ORDER BY [mov].[MovimentDate] ASC, [mov].[Id] DESC
													  );
				
				IF(@ULTIMADEPRECIACAO =0)
				BEGIN
					SET @AccumulatedDepreciation = @RateDepreciationMonthly + @AccumulatedDepreciation;
					SET @CurrentValue = (@CurrentValue - @RateDepreciationMonthly) - @UnfoldingValue;
				END;
				
				
				SET @CONTADORVIDAUTIL = @CONTADORVIDAUTIL + 1;
				
			END;

			DELETE FROM @TABLEBEM WHERE [Id] = @TABLEBEMID;
			
			---PRINT('FINALIZADO 2')
			---PRINT(@CurrentDate)
			SET @CONTADORBEM = @CONTADORBEM + 1;

		END TRY
		BEGIN CATCH
			SET @Retorno =ERROR_MESSAGE();
			SET @Erro = 1;
			SET @CONTADORBEM = @CONTADORBEM + 1;
			PRINT(ERROR_MESSAGE());
		END CATCH
	END;

	SET @CONTADOR = @CONTADOR + 1;
	END TRY
	
	BEGIN CATCH
		SET @Retorno =ERROR_MESSAGE();
		SET @Erro = 1;
		SET @CONTADOR = @CONTADOR + 1;
		PRINT(ERROR_MESSAGE());
	END CATCH
END;

--Alteração Aqui
---PRINT(@Decree);
---PRINT(@VIDA_UTIL);
---PRINT(@CONTADORVIDAUTIL);
---PRINT(@LifeCicle);
---PRINT(@MaximoMesAtual);

IF(@Decree = 0 AND @VIDA_UTIL > 0 AND @CONTADORVIDAUTIL >= @LifeCicle and @MaximoMesAtual = @LifeCicle)
---Ultimo registro bem sem decreto
BEGIN
	---PRINT('ULTIMO')
	INSERT INTO [dbo].[MonthlyDepreciation]
					([AssetStartId]
					,[ManagerUnitId]
					,[InitialId]
					,[MaterialItemCode]
					,[AcquisitionDate]
					,[CurrentDate]
					,[DateIncorporation]
					,[LifeCycle]
					,[CurrentMonth]
					,[ValueAcquisition]
					,[CurrentValue]
					,[ResidualValue]
					,[RateDepreciationMonthly]
					,[AccumulatedDepreciation]
					,[UnfoldingValue]
					,[Decree]
					,[ManagerUnitTransferId]
					,[MonthlyDepreciationId])
				VALUES(@AssetStartId
						,@ManagerUnitId 
						,@InitialId
						,@MaterialItemCode
						,@AcquisitionDate
						,(SELECT DATEADD(DAY,1,@CurrentDate))
						,@MovimentDate
						,@LifeCicle
						,@LifeCicle + 1
						,@ValueAcquisition
						,@CurrentValue
						,@ResidualPorcetagemValue
						,0.0
						,@AccumulatedDepreciation
						,0.0
						,@Decree
						,NULL
						,@AssetStartId)

				SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [AssetTransferenciaId] IS NOT NULL
													ORDER BY [Id] DESC
													  );
				---PRINT(@AssetId);

							
				---PRINT('DESTINO 4');
				---PRINT(@AssetStartId);
				---PRINT(@ManagerUnitId);
				---PRINT(@AssetId);
				---ORIGEM
				IF(@DATATRANSFERENCIA IS NOT NULL)
				BEGIN
					DECLARE @CountTransfere INT=0;
					DECLARE @ContadorTransfere INT = 0;
					DECLARE @MovimentDateTransfere DATETIME = NULL;


					INSERT INTO @TableTransfencia(ManagerUnitId,AssetId,MovimentDate)
					  SELECT [SourceDestiny_ManagerUnitId],[AssetTransferenciaId],[MovimentDate]
			          FROM [dbo].[AssetMovements]  
					  WHERE [MonthlyDepreciationId] IN(SELECT [MonthlyDepreciationId] FROM [dbo].[AssetMovements]
														 WHERE  [AssetId] = @AssetStartId
														   AND [AssetTransferenciaId] IS NOT NULL)
					  AND [AssetTransferenciaId] IS NOT NULL
					  GROUP BY [SourceDestiny_ManagerUnitId],[AssetTransferenciaId],[MovimentDate]


					SET @CountTransfere = (SELECT COUNT(*) FROM @TableTransfencia)
					---PRINT('QUANTIDADE');
					---PRINT(@CountTransfere);
					WHILE(@ContadorTransfere <= @CountTransfere)
					BEGIN
						---PRINT('PASSOU TRANSFERENCIA')
						SET @ManagerUnitDestino = (SELECT TOP 1 ManagerUnitId 
												  FROM @TableTransfencia);

						SET @AssetId = (SELECT TOP 1 AssetId 
												  FROM @TableTransfencia WHERE ManagerUnitId = @ManagerUnitDestino);
						
						SET @MovimentDateTransfere = (SELECT TOP 1 MovimentDate 
												  FROM @TableTransfencia WHERE ManagerUnitId = @ManagerUnitDestino);
						---PRINT(@ManagerUnitDestino);

						SET @QuantidadeDepreciado = (SELECT COUNT(*) FROM [dbo].[MonthlyDepreciation] WHERE AssetStartId = @AssetStartId AND [ManagerUnitId] = @ManagerUnitDestino);

						IF(ISNULL(@QuantidadeDepreciado,0) < 1)
						BEGIN
							---PRINT('TRANSFERENCIA 20')
							INSERT INTO MonthlyDepreciation
									([AssetStartId]
									,[ManagerUnitId]
									,[InitialId]
									,[MaterialItemCode]
									,[AcquisitionDate]
									,[CurrentDate]
									,[DateIncorporation]
									,[LifeCycle]
									,[CurrentMonth]
									,[ValueAcquisition]
									,[CurrentValue]
									,[ResidualValue]
									,[RateDepreciationMonthly]
									,[AccumulatedDepreciation]
									,[UnfoldingValue]
									,[Decree]
									,[ManagerUnitTransferId]
									,[MonthlyDepreciationId])
								SELECT TOP 1 [AssetStartId]
										,@ManagerUnitDestino
										,[InitialId]
										,[MaterialItemCode]
										,[AcquisitionDate]
										,@MovimentDateTransfere
										,[DateIncorporation]
										,[LifeCycle]
										,[CurrentMonth]
										,[ValueAcquisition]
										,[CurrentValue]
										,[ResidualValue]
										,0.0
										,@AccumulatedDepreciation
										,0.0
										,[Decree]
										,[ManagerUnitTransferId]
										,[MonthlyDepreciationId]
									FROM [dbo].[MonthlyDepreciation]
									WHERE [AssetStartId] = @AssetStartId
									ORDER BY [Id] DESC

									SET @CurrentValue = (SELECT TOP 1 [CurrentValue] FROM [dbo].[MonthlyDepreciation] 
														  WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] = @ManagerUnitId ORDER BY [Id] DESC)
										END
									
						DELETE @TableTransfencia WHERE [ManagerUnitId] = @ManagerUnitDestino;
						SET @ContadorTransfere = @ContadorTransfere + 1;
						END;
	
				END;
		END;
ELSE
	---Ultimo registro bem com decreto
	--Alteração Aquo
	IF(@Decree = 1 AND @CONTADORVIDAUTIL >= @LifeCicle and @MaximoMesAtual = @LifeCicle)
	BEGIN
	INSERT INTO [dbo].[MonthlyDepreciation]
			([AssetStartId]
			,[ManagerUnitId]
			,[InitialId]
			,[MaterialItemCode]
			,[AcquisitionDate]
			,[CurrentDate]
			,[DateIncorporation]
			,[LifeCycle]
			,[CurrentMonth]
			,[ValueAcquisition]
			,[CurrentValue]
			,[ResidualValue]
			,[RateDepreciationMonthly]
			,[AccumulatedDepreciation]
			,[UnfoldingValue]
			,[Decree]
			,[ManagerUnitTransferId]
			,[MonthlyDepreciationId])
		VALUES(@AssetStartId
				,@ManagerUnitId 
				,@InitialId
				,@MaterialItemCode
				,@AcquisitionDate
				,@CurrentDate
				,@MovimentDate
				,@LifeCicle
				,@LifeCicle + 1
				,@ValueAcquisition
				,@CurrentValue
				,@ResidualPorcetagemValue
				,0.0
				,@AccumulatedDepreciation
				,0.0
				,1
				,NULL
				,@AssetStartId)

	END;

	DECREE_FINAL:
		---Ultimo registro bem com decreto
		IF(@Decree = 1 AND @FINAL = 1)
		BEGIN
			
			INSERT INTO MonthlyDepreciation
					([AssetStartId]
					,[ManagerUnitId]
					,[InitialId]
					,[MaterialItemCode]
					,[AcquisitionDate]
					,[CurrentDate]
					,[DateIncorporation]
					,[LifeCycle]
					,[CurrentMonth]
					,[ValueAcquisition]
					,[CurrentValue]
					,[ResidualValue]
					,[RateDepreciationMonthly]
					,[AccumulatedDepreciation]
					,[UnfoldingValue]
					,[Decree]
					,[ManagerUnitTransferId]
					,[MonthlyDepreciationId])
				VALUES(@AssetStartId
						,@ManagerUnitId 
						,@InitialId
						,@MaterialItemCode
						,@AcquisitionDate
						,@CurrentDate
						,@MovimentDate
						,@LifeCicle
						,@LifeCicle + 1
						,@ValueAcquisition
						,@CurrentValue
						,@ResidualPorcetagemValue
						,0.0
						,@AccumulatedDepreciation
						,0.0
						,1
						,NULL
						,@AssetStartId)

			END;
TRANSFERENCIA_FINAL:
	BEGIN
		SET @DateDecree =(SELECT TOP 1 [ast].[MovimentDate] 
							FROM [dbo].[Asset] [ast]
							INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
							WHERE [ast].[flagDecreto]  = 1
								AND [ast].[Id] = @AssetStartId
							ORDER BY [ast].[Id] DESC);
		IF(@DateDecree IS NULL)
		BEGIN
			SET @AccumulatedDepreciation =(SELECT TOP 1 [AccumulatedDepreciation] 
											 FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] DESC)
			SET @CurrentDate =(SELECT TOP 1 [CurrentDate]
											 FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] DESC)

		END;
		ELSE
			BEGIN
				SET @RateDepreciationMonthly = 0.0;
				SET @AccumulatedDepreciation =0.0;
				SET @CurrentDate = (SELECT TOP 1 [MovimentDate] FROM [dbo].[AssetMovements] 
					   WHERE [MonthlyDepreciationId] = @AssetStartId AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId ORDER BY [Id] DESC)

				IF(@CurrentDate IS NULL)
				BEGIN
					SET @CurrentDate =(SELECT TOP 1 [CurrentDate]
											 FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] DESC)

				END;
			END;
		IF(@FINAL = 1)
		BEGIN
---Transferência com depreciação total na origem
		INSERT INTO MonthlyDepreciation
				([AssetStartId]
				,[ManagerUnitId]
				,[InitialId]
				,[MaterialItemCode]
				,[AcquisitionDate]
				,[CurrentDate]
				,[DateIncorporation]
				,[LifeCycle]
				,[CurrentMonth]
				,[ValueAcquisition]
				,[CurrentValue]
				,[ResidualValue]
				,[RateDepreciationMonthly]
				,[AccumulatedDepreciation]
				,[UnfoldingValue]
				,[Decree]
				,[ManagerUnitTransferId]
				,[MonthlyDepreciationId])
			SELECT TOP 1 [AssetStartId]
					,@ManagerUnitId
					,[InitialId]
					,[MaterialItemCode]
					,[AcquisitionDate]
					,(SELECT TOP 1 [MovimentDate] FROM [dbo].[AssetMovements] 
					   WHERE [MonthlyDepreciationId] = @AssetStartId AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId ORDER BY [Id] DESC)
					,[DateIncorporation]
					,[LifeCycle]
					,[CurrentMonth]
					,[ValueAcquisition]
					,[CurrentValue]
					,[ResidualValue]
					,0.0
					,@AccumulatedDepreciation
					,0.0
					,[Decree]
					,[ManagerUnitTransferId]
					,[MonthlyDepreciationId]
				FROM [dbo].[MonthlyDepreciation]
				WHERE [AssetStartId] = @AssetStartId
				ORDER BY [Id] DESC

				SET @CurrentValue = (SELECT TOP 1 [CurrentValue] FROM [dbo].[MonthlyDepreciation] 
				                      WHERE [AssetStartId] = @AssetStartId AND [ManagerUnitId] = @ManagerUnitId ORDER BY [Id] DESC)

		END;
	END;

	UPDATE [dbo].[Asset] SET ValueUpdate = @CurrentValue 
						WHERE [AssetStartId] = @AssetStartId 
						  AND [ManagerUnitId] = @ManagerUnitId 
						  AND [MaterialItemCode] = @MaterialItemCode 
	RETURN @Contador;
END TRY
BEGIN CATCH
	SET @Retorno =ERROR_MESSAGE();
END CATCH

GO
/****** Object:  StoredProcedure [dbo].[SAM_DEPRECIACAO_UGE_SIMULAR]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SAM_DEPRECIACAO_UGE_SIMULAR]
	@ManagerUnitId INT,
	@MaterialItemCode INT,
	@AssetStartId INT,
	@DataFinal AS DATETIME,
	@Fechamento AS BIT= 0,
	@Retorno AS NVARCHAR(1000) OUTPUT  
AS

SET DATEFORMAT YMD;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  

DECLARE @COUNT INT =0;
DECLARE @CONTADOR INT = 0;
DECLARE @TABLEID INT = 0;

DECLARE @COUNTBEM INT =0;
DECLARE @CONTADORBEM INT = 0;
DECLARE @TABLEBEMID INT  = 0;

DECLARE @VIDA_UTIL SMALLINT = 0;
DECLARE @CONTADORVIDAUTIL INT = 0;
DECLARE @CONTADORDECREE INT = 0;
DECLARE @DATATRANSFERENCIA AS DATETIME;
DECLARE @BEMJADEPRECIADO AS BIGINT = 0
DECLARE @VIDA_UTIL_DECREE INT = NULL;
DECLARE @ULTIMADEPRECIACAO AS BIT = 0;
DECLARE @CURRENTDATE_MANAGERUNIT DATETIME;
DECLARE @BEMDEPRECIADONAOTOTAL AS BIT = 0;
DECLARE @FINAL BIT = 0;
DECLARE @TRANSFERENCIA_BP  DATETIME = NULL;

----DEPRECIAÇÃO
DECLARE @AcquisitionDate DATETIME;
DECLARE @MovimentDate DATETIME
DECLARE @ValueAcquisition DECIMAL(18,2);
DECLARE @RateDepreciationMonthlyGroup AS DECIMAL(18,2);
DECLARE @RateDepreciationMonthly AS DECIMAL(18,2);
DECLARE @AccumulatedDepreciation AS DECIMAL(18,2) = 0;
DECLARE @ResidualValue AS DECIMAL(18,2)
DECLARE @ResidualPorcetagemValue AS DECIMAL(18,2)
DECLARE @CurrentValue DECIMAL(18,2);
DECLARE @Decree BIT =0;
DECLARE @UnfoldingValue DECIMAL(18,2)
DECLARE @CurrentDate AS DATETIME;
DECLARE @CurrentDateInitial AS DATETIME;
DECLARE @LifeCicle INT = 0;
DECLARE @DateDecree DATETIME;
DECLARE @DecreeValue AS DECIMAL(18,2) = 0;
DECLARE @CurrentMonth INT = 0;
DECLARE @QuantidadeDepreciado INT=0;
DECLARE @BemNaoDepreciadoNaOrigem INT = 0;
DECLARE @AssetId INT = NULL;
DECLARE @BemTransferidoJaDepreciadoQuantidade INT = NULL;
DECLARE @MesesFaltanteDepreciarDecreto SMALLINT = 0;
DECLARE @DateReverse DATETIME;
DECLARE @ValueAcquisitionCalcDecree DECIMAL(18,2)= NULL;


---INFORMAÇÕES
DECLARE @InitialId INT = 0
DECLARE @DepreciationAmountStart DECIMAL(18,2) = 0;
DECLARE @ManagerUnitIdInitial AS INT = @ManagerUnitId;

DECLARE @TABLE_ERRO AS TABLE(
 ErrorNumber INT NOT NULL, 
 ErrorMessage VARCHAR(2000) NOT NULL,
 MaterialItemCode INT NOT NULL,
 AssetStartId INT NOT NULL,
 AssetId INT NOT NULL
)

DECLARE @TABLEBEM AS TABLE(
	[Id] INT NOT NULL IDENTITY(1,1),
	[InitialId] INT NOT NULL,
	[AssetStartId] INT NOT NULL,
	[DataAquisicao] DATETIME NOT NULL,
	[DataIncorporacao] DATETIME NOT NULL,
	[ValorAquisicao] DECIMAL(18,2) NOT NULL
)

DECLARE @MonthlyDepreciation AS TABLE (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssetStartId] [int] NULL,
	[NumberIdentification] [varchar](20) NULL,
	[ManagerUnitId] [int] NOT NULL,
	[MaterialItemCode] [int] NOT NULL,
	[InitialId] [int] NOT NULL,
	[AcquisitionDate] [datetime] NOT NULL,
	[CurrentDate] [datetime] NOT NULL,
	[DateIncorporation] [datetime] NOT NULL,
	[LifeCycle] [smallint] NOT NULL,
	[CurrentMonth] [smallint] NOT NULL,
	[ValueAcquisition] [decimal](18, 2) NOT NULL,
	[CurrentValue] [decimal](18, 2) NOT NULL,
	[ResidualValue] [decimal](18, 2) NOT NULL,
	[RateDepreciationMonthly] [decimal](18, 2) NOT NULL,
	[AccumulatedDepreciation] [decimal](18, 2) NOT NULL,
	[UnfoldingValue] [decimal](18, 2) NULL,
	[Decree] [bit] NOT NULL,
	[ManagerUnitTransferId] [int] NULL,
	[MonthlyDepreciationId] [int] NULL)

SET @COUNT = 1;
SET @DATAFINAL =(SELECT DATEADD(MONTH,1,@DATAFINAL));
SET @Retorno = 'Depreciação realizada com sucesso!';

BEGIN TRY
WHILE(@CONTADOR < @COUNT )
BEGIN
	BEGIN TRY
		INSERT INTO @TABLEBEM([AssetStartId],[InitialId],[DataAquisicao],[DataIncorporacao],[ValorAquisicao])
		SELECT TOP 1 @AssetStartId,[bem].[InitialId], [bem].[AcquisitionDate],[bem].[MovimentDate],[bem].[ValueAcquisition] 
		   FROM [dbo].[Asset] [bem] 
		   WHERE [bem].[AssetStartId] =@AssetStartId 
		     AND [bem].[InitialId] IS NOT NULL
			 AND [bem].[flagVerificado] IS NULL
			 AND [bem].[flagDepreciaAcumulada] = 1 
			 AND [bem].[AssetStartId] IS NOT NULL
			 AND ([bem].[flagAcervo]  IS NULL OR [bem].[flagAcervo] = 0)
			 AND ([bem].[flagTerceiro] = 0 OR  [bem].[flagTerceiro] IS NULL)
			 AND ([bem].[FlagAnimalNaoServico] IS NULL OR [bem].[FlagAnimalNaoServico] =0)
		   ORDER BY [bem].[Id] ASC

		SET @COUNTBEM = (SELECT COUNT(*) FROM @TABLEBEM);
		PRINT(@COUNTBEM);
		WHILE(@CONTADORBEM < @COUNTBEM)
		BEGIN
			BEGIN TRY
			SET @LifeCicle = (SELECT TOP 1 [gr].[LifeCycle] 
								FROM [dbo].[MaterialGroup] [gr]
								INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
								WHERE [ast].[MaterialItemCode] = @MaterialItemCode);
			SET @TABLEBEMID =(SELECT TOP 1 [Id] FROM @TABLEBEM);

			SET @AssetId = (SELECT TOP 1 [AssetId] FROM [AssetMovements] WHERE [ManagerUnitId] = @ManagerUnitId AND [MonthlyDepreciationId] = @AssetStartId ORDER BY [Id] DESC);
			IF(@AssetId IS NOT NULL)
			BEGIN
				SET @BemNaoDepreciadoNaOrigem = (SELECT [AssetId] FROM [dbo].[AssetMovements] WHERE AssetTransferenciaId  = @AssetId)
				IF(@BemNaoDepreciadoNaOrigem IS NOT NULL)
				BEGIN
					SET @BemTransferidoJaDepreciadoQuantidade = (SELECT COUNT(*) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @BemNaoDepreciadoNaOrigem AND [MaterialItemCode] = @MaterialItemCode)
					IF(@BemTransferidoJaDepreciadoQuantidade IS NOT NULL)
					BEGIN
						IF(@BemTransferidoJaDepreciadoQuantidade >= @LifeCicle)
						BEGIN
							PRINT('ERRO 1');

							INSERT INTO @TABLE_ERRO(ErrorNumber,ErrorMessage,MaterialItemCode,AssetStartId,AssetId)
							SELECT 0,'Bem Material já está totalmente depreciado, o mesmo foi realizado na origem da transferência.',@MaterialItemCode,@AssetStartId, (SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = @AssetStartId AND  [MaterialItemCode] = @MaterialItemCode)

							SET @FINAL = 1
							GOTO TRANSFERENCIA_FINAL;
						END
						ELSE
							IF(ISNULL(@BemTransferidoJaDepreciadoQuantidade,0) = 0)
							BEGIN
								SET @AcquisitionDate = (SELECT  [DataAquisicao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
								SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] IS NOT NULL
													ORDER BY [Id] DESC
													  );
				

								IF(YEAR(@AcquisitionDate) <= YEAR(@DATATRANSFERENCIA) AND (MONTH(@AcquisitionDate) < MONTH(@DATATRANSFERENCIA)))
								BEGIN

									INSERT INTO @TABLE_ERRO(ErrorNumber,ErrorMessage,MaterialItemCode,AssetStartId,AssetId)
									SELECT 0,'Bem Material ainda não foi depreciado na origem, por favor accese a página de depreciação e realize a depreciação item material',@MaterialItemCode,@AssetStartId,(SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = @AssetStartId AND  [MaterialItemCode] = @MaterialItemCode)
									GOTO GOTO_ERRO;
								END;
								--ELSE
								--	BEGIN
								--		---Atribui o Id da ManagerUnit de destino para que seja depreciado o bem no destino
								--		SET @ManagerUnitId=(SELECT TOP 1 [ManagerUnitId]
								--					FROM [dbo].[AssetMovements]  
								--					WHERE [AssetId] = @AssetStartId
								--					  AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId
								--					  AND [AssetTransferenciaId] IS NOT NULL
								--					ORDER BY [Id] DESC
								--					  );
								--	END;
								SET @AcquisitionDate = NULL;
								SET @DATATRANSFERENCIA = NULL;

								
							END; 
						
					END
				END;
			END;


			

			SET @AcquisitionDate = (SELECT  [DataAquisicao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			SET @MovimentDate = (SELECT  [DataIncorporacao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			SET @ValueAcquisition  = (SELECT  [ValorAquisicao] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			SET @InitialId  = (SELECT  [InitialId] FROM @TABLEBEM WHERE [Id] = @TABLEBEMID);
			

			SET @QuantidadeDepreciado = (SELECT COUNT(*) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)
			
			IF(@QuantidadeDepreciado >= @LifeCicle)
			BEGIN

				INSERT INTO @TABLE_ERRO(ErrorNumber,ErrorMessage,MaterialItemCode,AssetStartId,AssetId)
				SELECT 0,'Bem Material já éstá totalmente depreciado.',@MaterialItemCode,@AssetStartId,(SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = @AssetStartId AND  [MaterialItemCode] = @MaterialItemCode)
				GOTO GOTO_ERRO
			END

			SET @VIDA_UTIL = @LifeCicle;
			PRINT('VIDA UTIL 01')
			PRINT(@VIDA_UTIL);

			SET @DateDecree =(SELECT TOP 1 [ast].[MovimentDate] 
									FROM [dbo].[Asset] [ast]
									INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
									WHERE [ast].[flagDecreto]  = 1
									  AND [ast].[Id] = @AssetId--@AssetStartId
									  AND [ast].[ManagerUnitId] = @ManagerUnitId 
									ORDER BY [ast].[Id] DESC);

			SET @UnfoldingValue =0;
			SET @AccumulatedDepreciation = 0;

				
			SET @ResidualValue = (SELECT TOP 1 [gr].[ResidualValue] 
											FROM [dbo].[MaterialGroup] [gr]
											INNER JOIN [dbo].[Asset] [ast] ON [ast].[MaterialGroupCode] = [gr].[Code]
										WHERE [ast].[MaterialItemCode] = @MaterialItemCode); 
				
						
			SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] IS NOT NULL
													ORDER BY [Id] DESC
													  );
			SET @DateReverse  = (SELECT TOP 1 [DataEstorno]
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] IS NOT NULL
													  AND ([FlagEstorno] IS NULL OR [FlagEstorno] = 0)
													ORDER BY [Id] DESC
													  );	
			

			SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)
			SET @CurrentDate = (SELECT MAX(CurrentDate) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)
			SET @CURRENTDATE_MANAGERUNIT = (SELECT MAX(CurrentDate) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode AND [ManagerUnitId] = @ManagerUnitId)

			SET @CONTADORVIDAUTIL = 0;


			IF(@CurrentDate IS NOT NULL)
			BEGIN
		
				SET @CONTADORVIDAUTIL =(SELECT MAX(CurrentMonth) + 1 FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode)  ---(@LifeCicle - (@VIDA_UTIL - DATEDIFF(MONTH,@AcquisitionDate,@CurrentDate)) + 1) +1;

				SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
				
		
			END
			
			IF(@CURRENTDATE_MANAGERUNIT IS NOT NULL)
			BEGIN
				SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] 
										WHERE [AssetStartId] = @AssetStartId 
										  AND [MaterialItemCode] = @MaterialItemCode 
										  AND [ManagerUnitId] = @ManagerUnitId)


				SET @VIDA_UTIL = @LifeCicle -  DATEDIFF(MONTH,@AcquisitionDate,@CURRENTDATE_MANAGERUNIT);
				SET @CONTADORVIDAUTIL =  (SELECT MAX([CurrentMonth]) + 1 FROM [MonthlyDepreciation] 
										WHERE [AssetStartId] = @AssetStartId 
										  AND [MaterialItemCode] = @MaterialItemCode 
										  AND [ManagerUnitId] = @ManagerUnitId)

				PRINT('VIDA UTIL 02')
				PRINT(@VIDA_UTIL);
				IF(@VIDA_UTIL < 1)
				BEGIN
					SET @FINAL = 1
					GOTO TRANSFERENCIA_FINAL;
				END
			END;

			--Calcula o valor a ser depreciano ao mês
			SET @ResidualPorcetagemValue =ROUND((@ValueAcquisition * ROUND(@ResidualValue,2,1)) / 100,2,1);
			SET @RateDepreciationMonthly = ROUND((@ValueAcquisition - @ResidualPorcetagemValue) / @VIDA_UTIL,2,1);

			IF(@CurrentDate IS NOT NULL)
			BEGIN

				SET @ResidualPorcetagemValue =  (SELECT TOP 1 ResidualValue FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
												
				SET @RateDepreciationMonthly =  (SELECT TOP 1 RateDepreciationMonthly FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
				SET @AccumulatedDepreciation =  (SELECT MAX(AccumulatedDepreciation) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) + @RateDepreciationMonthly;
				SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) - @RateDepreciationMonthly;

				SET @BEMDEPRECIADONAOTOTAL = 1
				PRINT('CURRENT DATE')
				PRINT(@CurrentDate)
				PRINT(@CurrentMonth)
			END;

			---Executa sempre em penultimo
			IF(@CurrentValue IS NULL)
			BEGIN
				SET @CurrentValue = @ValueAcquisition;
			END;
			
			IF(@CurrentDate IS NULL)
			BEGIN
				---Alterar o dia da data atual para 10, assim padronizando o dia.
				SET @CurrentDate = CAST(YEAR(@AcquisitionDate) AS VARCHAR(4)) + '-' + CAST(MONTH(@AcquisitionDate) AS VARCHAR(2)) + '-' + '10';
				print(@CurrentDate);
			END
			---Executa sempre em ultimo
			IF(@DateDecree IS NOT NULL)
			BEGIN
					SET @MesesFaltanteDepreciarDecreto =  DATEDIFF(MONTH,@AcquisitionDate,@DateDecree);
					SET @Decree  = 1;
					SET @MesesFaltanteDepreciarDecreto = IIF(@MesesFaltanteDepreciarDecreto < 0, 0,@MesesFaltanteDepreciarDecreto);

					IF(@VIDA_UTIL - @MesesFaltanteDepreciarDecreto > 0)
					BEGIN
						---(Valor de Decreto - valor residual) / Vida util - Inicio de depreciação(450-45)/120-36

						SET @DecreeValue =  (SELECT TOP 1 [ValueAcquisition] 
									FROM [dbo].[Asset] [ast] 
									INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
									WHERE [AssetStartId] = @AssetStartId
										AND [ast].[MovimentDate] = @DateDecree
									ORDER BY [mov].[Id] DESC
									);

				    
					--SELECT  DATEDIFF(MONTH,'2015-12-01','2018-12-13');
					--SELECT ROUND((421.08 * ROUND(10.00,2,1)) / 100,2,1);
					--SELECT ROUND((421.08- 42.10) / (120-36),2,1)
					---10.00
						SET @ValueAcquisitionCalcDecree = @DecreeValue -- @ResidualValue;
						SET @CurrentValue = @ValueAcquisitionCalcDecree;
						PRINT('DECRETO VIDA UTIL')
						PRINT(@VIDA_UTIL)
						PRINT(@LIFECICLE)

						SET @CurrentDate = @DateDecree;
						SET @CONTADORVIDAUTIL = @MesesFaltanteDepreciarDecreto + 1;

						SET @ResidualPorcetagemValue =ROUND((@ValueAcquisitionCalcDecree * ROUND(@ResidualValue,2,1)) / 100,2,1);
						SET @RateDepreciationMonthly = ROUND((@ValueAcquisitionCalcDecree - @ResidualPorcetagemValue)  / (@VIDA_UTIL- @MesesFaltanteDepreciarDecreto),2,1);

						--select ROUND((@ValueAcquisitionCalcDecree - @ResidualPorcetagemValue) ,2,1) / (@VIDA_UTIL- @MesesFaltanteDepreciarDecreto);

						SET @AccumulatedDepreciation = @RateDepreciationMonthly + @AccumulatedDepreciation;
						SET @VIDA_UTIL_DECREE = @VIDA_UTIL;

						SET @TRANSFERENCIA_BP = (SELECT [MovimentDate] FROM [AssetMovements] WHERE [MonthlyDepreciationId] = @AssetStartId AND [AssetTransferenciaId] IS NOT NULL)

						IF(@TRANSFERENCIA_BP IS NOT NULL)
						BEGIN
							SET @CurrentDate = @TRANSFERENCIA_BP;
						END;
					

						PRINT('VIDA UTIL 5555')
						PRINT('Decreto')
						PRINT(@VIDA_UTIL)
						PRINT(@CONTADORVIDAUTIL)
						PRINT(@MesesFaltanteDepreciarDecreto)
						PRINT(@CurrentDate)
						print(@RateDepreciationMonthly)

						
					END;
					ELSE
						BEGIN
							
							---- Decreto realizado com o bem totalmente depreciado, então seta @VIDA_UTIL = -1 para não entrar no loop de calcular
							
							SET @CurrentValue =  (SELECT TOP 1 [ValueAcquisition] 
									FROM [dbo].[Asset] [ast] 
									INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
									WHERE [AssetStartId] = @AssetStartId
										AND [ast].[MovimentDate] = @DateDecree
									ORDER BY [ast].[Id] DESC
									);	
									
							SET @CurrentDate = @DateDecree;

							SET @ResidualPorcetagemValue =ROUND((@CurrentValue * ROUND(@ResidualValue,2,1)) / 100,2,1);
							SET @RateDepreciationMonthly = ROUND((@CurrentValue - @ResidualPorcetagemValue) ,2,1) / @VIDA_UTIL;

							SET  @AccumulatedDepreciation =(SELECT TOP 1 [AccumulatedDepreciation] 
											 FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] DESC);
							SET @CONTADORVIDAUTIL = @VIDA_UTIL;
							SET @FINAL = 1
							GOTO DECREE_FINAL;
						END;
				
						   
			END

			SET @DepreciationAmountStart = @CurrentValue;

			

			IF(@Fechamento =1)
			BEGIN
				SET @VIDA_UTIL = @CONTADORVIDAUTIL;
				SET @RateDepreciationMonthly =  (SELECT TOP 1 RateDepreciationMonthly FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
				SET @AccumulatedDepreciation =  (SELECT MAX(AccumulatedDepreciation) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) + @RateDepreciationMonthly;
				SET @CurrentValue = (SELECT MIN([CurrentValue]) FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode) - @RateDepreciationMonthly;

				
				PRINT('FECHAMENTO')
				IF(@CurrentValue IS NULL)
				BEGIN
					SET @VIDA_UTIL = @LifeCicle;
					SET @CurrentValue = @ValueAcquisition;
					--Calcula o valor a ser depreciano ao mês
					SET @ResidualPorcetagemValue =ROUND((@ValueAcquisition * ROUND(@ResidualValue,2,1)) / 100,2,1);
					SET @RateDepreciationMonthly = ROUND((@ValueAcquisition - @ResidualPorcetagemValue) / @VIDA_UTIL,2,1);
					SET @AccumulatedDepreciation = @RateDepreciationMonthly;
					
					SET @CONTADORVIDAUTIL = 0;
					PRINT('PASSOU 1');

				END
				ELSE
					IF(@ValueAcquisitionCalcDecree IS NOT NULL)
						BEGIN
							SET @ResidualPorcetagemValue =  (SELECT TOP 1 ResidualValue FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
				
							SET @CurrentDate =  (SELECT TOP 1 CurrentDate FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);
							SET @CONTADORVIDAUTIL =  (SELECT TOP 1 CurrentMonth + 1 FROM [MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId AND [MaterialItemCode] = @MaterialItemCode ORDER BY [Id] DESC);

							SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
							SET @VIDA_UTIL = @VIDA_UTIL_DECREE;
							
							PRINT('DECREE FECHAMENTO')
							PRINT(@VIDA_UTIL)
							PRINT(@CONTADORVIDAUTIL)
						END;
				

				SET @DATATRANSFERENCIA = (SELECT TOP 1 MovimentDate 
													FROM [dbo].[AssetMovements]  
													WHERE [AssetId] = @AssetStartId
													  AND [ManagerUnitId] = @ManagerUnitId
													  AND [AssetTransferenciaId] IS NOT NULL
													ORDER BY [Id] DESC
													  );
			END;
			SET @ValueAcquisitionCalcDecree = NULL;
			PRINT('CONTADOR VIDA UTIL')
			PRINT(@CONTADORVIDAUTIL)
			PRINT(@VIDA_UTIL)
			PRINT('DATA ATUAL INICIO')
			PRINT(CONVERT(VARCHAR(12),@CurrentDate,110))
			WHILE(@CONTADORVIDAUTIL <= @VIDA_UTIL)
			BEGIN
				PRINT('INICIO');
				IF(YEAR(@DataFinal) = YEAR(@CurrentDate) AND MONTH(@DataFinal) = MONTH(@CurrentDate))
				BEGIN
					
					BREAK;
				END
				---Verifica se na data atual foi realizado uma transferência

				IF(@DATATRANSFERENCIA IS NOT NULL)
				BEGIN


					IF(YEAR(@DATATRANSFERENCIA)  <= YEAR(@CurrentDate) AND MONTH(@DATATRANSFERENCIA) <= MONTH(@CurrentDate))
					BEGIN
						PRINT('Transferencia')
						PRINT(CONVERT(VARCHAR(12),@DATATRANSFERENCIA,110))
						PRINT(CONVERT(VARCHAR(12),@CurrentDate,110))
						BREAK;
					END
					
				END;
				---Verifica se na data atual foi realizado um estorno
				IF(@DateReverse IS NOT NULL)
				BEGIN
					IF(YEAR(@DateReverse)  = YEAR(@CurrentDate) AND MONTH(@DateReverse) = MONTH(@CurrentDate))
					BEGIN
						PRINT('Estorno')
						BREAK;
					END
					
				END;

				--IF(@ValueAcquisitionCalcDecree IS NULL AND YEAR(@DateDecree) = YEAR(@CurrentDate) AND MONTH(@DateDecree) = MONTH(@CurrentDate))
				--BEGIN

				--	SET @MesesFaltanteDepreciarDecreto =  DATEDIFF(MONTH,@AcquisitionDate,@DateDecree);
				--	SET @Decree  = 1;
			
				--	IF(@VIDA_UTIL - @MesesFaltanteDepreciarDecreto > 0)
				--	BEGIN
				--		SET @DecreeValue =  (SELECT TOP 1 [ValueUpdate] 
				--					FROM [dbo].[Asset] [ast] 
				--					INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
				--					WHERE [AssetStartId] = @AssetStartId
				--						AND [ast].[MovimentDate] = @DateDecree
				--					ORDER BY [mov].[Id] DESC
				--					);


						
				--		SET @ValueAcquisitionCalcDecree = @DecreeValue;
				--		SET @CurrentValue = @ValueAcquisitionCalcDecree;

				--		SET @CurrentDate = @DateDecree;
				--		SET @CONTADORVIDAUTIL = @MesesFaltanteDepreciarDecreto;

				--		SET @ResidualPorcetagemValue =ROUND((@ValueAcquisitionCalcDecree * ROUND(@ResidualValue,2,1)) / 100,2,1);
				--		SET @RateDepreciationMonthly = ROUND((@ValueAcquisitionCalcDecree - @ResidualPorcetagemValue) ,2,1) / (@VIDA_UTIL - (DATEDIFF(MONTH,@AcquisitionDate,@DateDecree)));
				--	END;
				--END;

				SET @BEMJADEPRECIADO = (SELECT TOP 1 [Id] FROM @MonthlyDepreciation 
													WHERE [AssetStartId] = @AssetStartId
														AND [CurrentDate]= @CurrentDate
														AND [ManagerUnitId] = @ManagerUnitId
														AND [MaterialItemCode] = @MaterialItemCode)
				PRINT('CONTADOR VIDA UTIL + 1')
				PRINT(@CONTADORVIDAUTIL)
				IF(@CONTADORVIDAUTIL = 0)
				BEGIN
					---Verifica se o bem para a data atual já foi depreciado, caso sim não é realizado o insert, assim evitando error de inclusão

					IF(ISNULL(@BEMJADEPRECIADO,0) < 1)
					BEGIN
						
							---Alterar o dia do primeiro registro "linha 0".
							SET @CurrentDateInitial =(SELECT DATEADD(DAY,-1,@CurrentDate));


							
							UPDATE [dbo].[Asset] SET [DepreciationDateStart] = @CurrentDate, [DepreciationAmountStart] = @DepreciationAmountStart
												WHERE [AssetStartId] = @AssetStartId AND [DepreciationDateStart] IS NULL;	
							
							PRINT('DEPRECIAÇÃO');
							PRINT(@CURRENTVALUE);
							INSERT INTO @MonthlyDepreciation
										([AssetStartId]
										,[ManagerUnitId]
										,[InitialId]
										,[MaterialItemCode]
										,[AcquisitionDate]
										,[CurrentDate]
										,[DateIncorporation]
										,[LifeCycle]
										,[CurrentMonth]
										,[ValueAcquisition]
										,[CurrentValue]
										,[ResidualValue]
										,[RateDepreciationMonthly]
										,[AccumulatedDepreciation]
										,[UnfoldingValue]
										,[Decree]
										,[ManagerUnitTransferId]
										,[MonthlyDepreciationId])
									VALUES(@AssetStartId
											,@ManagerUnitId 
											,@InitialId
											,@MaterialItemCode
											,@AcquisitionDate
											,@CurrentDateInitial
											,@MovimentDate
											,@LifeCicle
											,@CONTADORVIDAUTIL
											,@ValueAcquisition
											,@CurrentValue
											,@ResidualPorcetagemValue
											,@RateDepreciationMonthly
											,@AccumulatedDepreciation
											,@UnfoldingValue
											,@Decree
											,NULL
											,@AssetStartId)
					END;
			

				END;
				ELSE
					BEGIN
					
						IF(@CONTADORVIDAUTIL = @VIDA_UTIL)
						BEGIN
							IF(@BEMDEPRECIADONAOTOTAL = 1)
							BEGIN
								SET @CurrentMonth =(SELECT COUNT(*) + @CONTADORVIDAUTIL
											FROM @MonthlyDepreciation 
											WHERE [AssetStartId] = @AssetStartId
											);
							END;
							ELSE
								BEGIN
									SET @CurrentMonth =(SELECT COUNT(*) 
											FROM @MonthlyDepreciation 
											WHERE [AssetStartId] = @AssetStartId
										);
								END;
							
							PRINT('FINAL');
							PRINT(@CurrentMonth)
							PRINT(@LifeCicle)

							IF(@CurrentMonth >= @LifeCicle)
							BEGIN
								print('Final 1');
								SET @UnfoldingValue = @CurrentValue - @ResidualPorcetagemValue;
								SET @CurrentValue = @CurrentValue - @UnfoldingValue;
								SET @AccumulatedDepreciation = @AccumulatedDepreciation + @UnfoldingValue;
								SET @RateDepreciationMonthly = @RateDepreciationMonthly + @UnfoldingValue;
								SET @ULTIMADEPRECIACAO = 1;
							END;

							IF(@DateDecree IS NOT NULL AND (@MesesFaltanteDepreciarDecreto + @CurrentMonth) >= @LifeCicle)
							BEGIN
								print('Final 1');
								SET @UnfoldingValue = @CurrentValue - @ResidualPorcetagemValue;
								SET @CurrentValue = @CurrentValue - @UnfoldingValue;
								SET @AccumulatedDepreciation = @AccumulatedDepreciation + @UnfoldingValue;
								SET @RateDepreciationMonthly = @RateDepreciationMonthly + @UnfoldingValue;
								SET @ULTIMADEPRECIACAO = 1;
							END;
							
						END;
						PRINT('BEM DEPRECIADO');
						PRINT(@BEMJADEPRECIADO);

						IF(ISNULL(@BEMJADEPRECIADO,0) < 1)
						BEGIN
							INSERT INTO @MonthlyDepreciation
										([AssetStartId]
										,[ManagerUnitId]
										,[InitialId]
										,[MaterialItemCode]
										,[AcquisitionDate]
										,[CurrentDate]
										,[DateIncorporation]
										,[LifeCycle]
										,[CurrentMonth]
										,[ValueAcquisition]
										,[CurrentValue]
										,[ResidualValue]
										,[RateDepreciationMonthly]
										,[AccumulatedDepreciation]
										,[UnfoldingValue]
										,[Decree]
										,[ManagerUnitTransferId]
										,[MonthlyDepreciationId])
									VALUES(@AssetStartId
											,@ManagerUnitId 
											,@InitialId
											,@MaterialItemCode
											,@AcquisitionDate
											,@CurrentDate
											,@MovimentDate
											,@LifeCicle
											,@CONTADORVIDAUTIL
											,@ValueAcquisition
											,@CurrentValue
											,@ResidualPorcetagemValue
											,@RateDepreciationMonthly
											,@AccumulatedDepreciation
											,@UnfoldingValue
											,@Decree
											,NULL
											,@AssetStartId)
						END;
				

					END;
			
		
			
				IF(@CONTADORVIDAUTIL =0)
				BEGIN
					SET @CurrentDate =(SELECT DATEADD(DAY,1,@CurrentDate));
				END;
				ELSE
					BEGIN
						SET @CurrentDate =(SELECT DATEADD(MONTH,1,@CurrentDate));
						PRINT('DATA ATUAL + 1');
						PRINT(CONVERT(VARCHAR(12),@CurrentDate,110))
						PRINT(@CONTADORVIDAUTIL);
					END;

				IF(@ULTIMADEPRECIACAO =0)
				BEGIN
					SET @AccumulatedDepreciation = @RateDepreciationMonthly + @AccumulatedDepreciation;
					SET @CurrentValue = (@CurrentValue - @RateDepreciationMonthly) - @UnfoldingValue;
				END;
				

				SET @CONTADORVIDAUTIL = @CONTADORVIDAUTIL + 1;
			END;

			DELETE FROM @TABLEBEM WHERE [Id] = @TABLEBEMID;
			SET @CONTADORBEM = @CONTADORBEM + 1;
		END TRY
		BEGIN CATCH
			PRINT('ERRO 4');

			INSERT INTO @TABLE_ERRO(ErrorNumber,ErrorMessage,MaterialItemCode,AssetStartId,AssetId)
			SELECT ERROR_NUMBER(),ERROR_MESSAGE(),@MaterialItemCode,@AssetStartId,(SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = @AssetStartId AND  [MaterialItemCode] = @MaterialItemCode)

			SET @CONTADORBEM = @CONTADORBEM + 1;
		END CATCH
		END;

		SET @CONTADOR = @CONTADOR + 1;
	END TRY
	
	BEGIN CATCH
		PRINT('ERRO 5');

		INSERT INTO @TABLE_ERRO(ErrorNumber,ErrorMessage,MaterialItemCode,AssetStartId,AssetId)
		SELECT ERROR_NUMBER(),ERROR_MESSAGE(),@MaterialItemCode,@AssetStartId,(SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = @AssetStartId AND  [MaterialItemCode] = @MaterialItemCode)

		SET @CONTADOR = @CONTADOR + 1;
	END CATCH
	SET @ManagerUnitId = @ManagerUnitIdInitial;
END;

IF(@Decree = 0 AND @VIDA_UTIL > 0 AND @CONTADORVIDAUTIL >= @LifeCicle)
---Ultimo registro bem sem decreto
BEGIN

	INSERT INTO @MonthlyDepreciation
					([AssetStartId]
					,[ManagerUnitId]
					,[InitialId]
					,[MaterialItemCode]
					,[AcquisitionDate]
					,[CurrentDate]
					,[DateIncorporation]
					,[LifeCycle]
					,[CurrentMonth]
					,[ValueAcquisition]
					,[CurrentValue]
					,[ResidualValue]
					,[RateDepreciationMonthly]
					,[AccumulatedDepreciation]
					,[UnfoldingValue]
					,[Decree]
					,[ManagerUnitTransferId]
					,[MonthlyDepreciationId])
				VALUES(@AssetStartId
						,@ManagerUnitId 
						,@InitialId
						,@MaterialItemCode
						,@AcquisitionDate
						,@CurrentDate
						,@MovimentDate
						,@LifeCicle
						,@LifeCicle + 1
						,@ValueAcquisition
						,@CurrentValue
						,@ResidualPorcetagemValue
						,0.0
						,@AccumulatedDepreciation
						,0.0
						,@Decree
						,NULL
						,@AssetStartId)

END;
ELSE
	---Ultimo registro bem com decreto
	IF(@Decree = 1 AND @CONTADORVIDAUTIL >= @LifeCicle)
	BEGIN
	PRINT('Ultima com decreto');
	INSERT INTO @MonthlyDepreciation
			([AssetStartId]
			,[ManagerUnitId]
			,[InitialId]
			,[MaterialItemCode]
			,[AcquisitionDate]
			,[CurrentDate]
			,[DateIncorporation]
			,[LifeCycle]
			,[CurrentMonth]
			,[ValueAcquisition]
			,[CurrentValue]
			,[ResidualValue]
			,[RateDepreciationMonthly]
			,[AccumulatedDepreciation]
			,[UnfoldingValue]
			,[Decree]
			,[ManagerUnitTransferId]
			,[MonthlyDepreciationId])
		VALUES(@AssetStartId
				,@ManagerUnitId 
				,@InitialId
				,@MaterialItemCode
				,@AcquisitionDate
				,@CurrentDate
				,@MovimentDate
				,@LifeCicle
				,@LifeCicle + 1
				,@ValueAcquisition
				,@CurrentValue
				,@ResidualPorcetagemValue
				,0.0
				,@AccumulatedDepreciation
				,0.0
				,1
				,NULL
				,@AssetStartId)

	END;

	DECREE_FINAL:
		---Ultimo registro bem com decreto
		IF(@Decree = 1 AND @FINAL = 1)
		BEGIN
			

		PRINT('Ultima com decreto');
		INSERT INTO @MonthlyDepreciation
				([AssetStartId]
				,[ManagerUnitId]
				,[InitialId]
				,[MaterialItemCode]
				,[AcquisitionDate]
				,[CurrentDate]
				,[DateIncorporation]
				,[LifeCycle]
				,[CurrentMonth]
				,[ValueAcquisition]
				,[CurrentValue]
				,[ResidualValue]
				,[RateDepreciationMonthly]
				,[AccumulatedDepreciation]
				,[UnfoldingValue]
				,[Decree]
				,[ManagerUnitTransferId]
				,[MonthlyDepreciationId])
			VALUES(@AssetStartId
					,@ManagerUnitId 
					,@InitialId
					,@MaterialItemCode
					,@AcquisitionDate
					,@CurrentDate
					,@MovimentDate
					,@LifeCicle
					,@LifeCicle + 1
					,@ValueAcquisition
					,@CurrentValue
					,@ResidualPorcetagemValue
					,0.0
					,@AccumulatedDepreciation
					,0.0
					,1
					,NULL
					,@AssetStartId)

		END;
TRANSFERENCIA_FINAL:
	BEGIN
		SET @DateDecree =(SELECT TOP 1 [ast].[MovimentDate] 
							FROM [dbo].[Asset] [ast]
							INNER JOIN [dbo].[AssetMovements] [mov] ON [mov].[AssetId] = [ast].[Id]
							WHERE [ast].[flagDecreto]  = 1
								AND [ast].[Id] = @AssetStartId
							ORDER BY [ast].[Id] DESC);
		IF(@DateDecree IS NULL)
		BEGIN
			SET @AccumulatedDepreciation =(SELECT TOP 1 [AccumulatedDepreciation] 
											 FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] DESC)
			SET @CurrentDate =(SELECT TOP 1 [CurrentDate]
											 FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] DESC)

		END;
		ELSE
			BEGIN
				SET @RateDepreciationMonthly = 0.0;
				SET @AccumulatedDepreciation =0.0;
				SET @CurrentDate = (SELECT TOP 1 [MovimentDate] FROM [dbo].[AssetMovements] 
					   WHERE [MonthlyDepreciationId] = @AssetStartId AND [SourceDestiny_ManagerUnitId] = @ManagerUnitId ORDER BY [Id] DESC)

				IF(@CurrentDate IS NULL)
				BEGIN
					SET @CurrentDate =(SELECT TOP 1 [CurrentDate]
											 FROM [dbo].[MonthlyDepreciation] WHERE [AssetStartId] = @AssetStartId ORDER BY [Id] DESC)

				END;
			END;
		IF(@FINAL = 1)
		BEGIN
---Transferência com depreciação total na origem
		PRINT('depreciação total na origem');
		PRINT(@DateDecree)
		PRINT(@CurrentDate)
		INSERT INTO @MonthlyDepreciation
				([AssetStartId]
				,[ManagerUnitId]
				,[InitialId]
				,[MaterialItemCode]
				,[AcquisitionDate]
				,[CurrentDate]
				,[DateIncorporation]
				,[LifeCycle]
				,[CurrentMonth]
				,[ValueAcquisition]
				,[CurrentValue]
				,[ResidualValue]
				,[RateDepreciationMonthly]
				,[AccumulatedDepreciation]
				,[UnfoldingValue]
				,[Decree]
				,[ManagerUnitTransferId]
				,[MonthlyDepreciationId])
			SELECT TOP 1 [AssetStartId]
					,@ManagerUnitId
					,[InitialId]
					,[MaterialItemCode]
					,[AcquisitionDate]
					,@CurrentDate
					,[DateIncorporation]
					,[LifeCycle]
					,[CurrentMonth]
					,[ValueAcquisition]
					,[CurrentValue]
					,[ResidualValue]
					,0.0
					,@AccumulatedDepreciation 
					,0.0
					,[Decree]
					,[ManagerUnitTransferId]
					,[MonthlyDepreciationId]
				FROM [dbo].[MonthlyDepreciation]
				WHERE [AssetStartId] = @AssetStartId
				ORDER BY [Id] DESC

			PRINT('Vida util por transferencia finalizado');
		END;
	END;

END TRY
BEGIN CATCH
	PRINT('ERRO 6');
	PRINT(@ManagerUnitId);
	PRINT(@AssetStartId);
	PRINT(@MaterialItemCode);

	INSERT INTO @TABLE_ERRO(ErrorNumber,ErrorMessage,MaterialItemCode,AssetStartId,AssetId )
	SELECT ERROR_NUMBER(),ERROR_MESSAGE(),@MaterialItemCode,@AssetStartId,(SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [ManagerUnitId] = @ManagerUnitId AND [AssetStartId] = @AssetStartId AND  [MaterialItemCode] = @MaterialItemCode)

	GOTO GOTO_ERRO;
END CATCH


GOTO_ERRO:
	SELECT * FROM @TABLE_ERRO;

SELECT * FROM @MonthlyDepreciation

GO
/****** Object:  StoredProcedure [dbo].[SAM_PATRIMONIO_CONSULTA_INVENTARIO_MANUAL]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SAM_PATRIMONIO_CONSULTA_INVENTARIO_MANUAL] --6, 22, 205, 2060, 9612, 217673
(            
 @idOrgao  INT = NULL,
 @idUo   INT = NULL,
 @idUge   INT = NULL,
 @idUa   INT = NULL,
 @idResponsavel INT = NULL,
 @idDivisao INT = NULL
)
AS             
BEGIN            
 SELECT
  CONVERT(VARCHAR(MAX), [divisaoUA].[Code]) +' - '+ [divisaoUA].[Description] as [Dados_Divisao],
  ISNULL(itemInventario.InitialName, '') +'-'+ itemInventario.Code as [Sigla_Chapa],
  CAST(itemInventario.item AS VARCHAR(20)) +' - '+ [sdi].[Description] as [Dados_BP],
  itemInventario.AssetId as [AssetID],
  itemInventario.Estado as [SituacaoFisicaBP]
 FROM             
  ItemInventario itemInventario WITH (NOLOCK)
  INNER JOIN Inventario as inventario with(nolock) on inventario.Id = itemInventario.InventarioId 
 INNER JOIN Asset as a WITH (NOLOCK) on a.Id = itemInventario.AssetId
 INNER JOIN AssetMovements as am WITH (NOLOCK) on a.Id = am.AssetId
 LEFT JOIN Section as divisaoUA WITH (NOLOCK) on divisaoUA.Id = am.SectionId
 INNER JOIN Responsible as d WITH (NOLOCK) on d.id = am.ResponsibleId
 INNER JOIN Institution as e WITH (NOLOCK) on e.id = am.InstitutionId
 INNER JOIN BudgetUnit as f WITH (NOLOCK) on f.id = am.BudgetUnitId
 INNER JOIN ManagerUnit as g WITH (NOLOCK) on g.id = am.ManagerUnitId 
 LEFT JOIN AdministrativeUnit as h WITH (NOLOCK) on h.id = am.AdministrativeUnitId
 INNER JOIN Initial as i WITH (NOLOCK) on i.Id = a.InitialId
 LEFT JOIN ShortDescriptionItem as sdi WITH (NOLOCK) on a.ShortDescriptionItemId = sdi.Id
 WHERE
     (am.InstitutionId = @idOrgao OR @idOrgao IS NULL)
 AND (am.BudgetUnitId = @idUo OR @idUo IS NULL)
 AND (am.ManagerUnitId = @idUge OR @idUge IS NULL)
 AND (am.AdministrativeUnitId = @idUa OR @idUa IS NULL)
 AND (am.ResponsibleId = @idResponsavel OR @idResponsavel IS NULL)
 AND (am.SectionId = @idDivisao OR @idDivisao IS NULL)
 AND a.Status = 1 AND am.Status = 1
 AND a.flagVerificado IS NULL
 AND a.flagDepreciaAcumulada = 1
 AND inventario.Status = '0'
  GROUP BY 
	[divisaoUA].[id],[divisaoUA].[Code], [divisaoUA].[Description], itemInventario.InitialName, itemInventario.Code, itemInventario.Item, [sdi].[Description], itemInventario.Estado, itemInventario.AssetId
 ORDER BY
 divisaoUA.Code ASC, itemInventario.Code ASC
END

GO
/****** Object:  StoredProcedure [dbo].[SAM_PATRIMONIO_GERA_DADOS_INVENTARIO_MANUAL]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SAM_PATRIMONIO_GERA_DADOS_INVENTARIO_MANUAL] --6, 22, 205, 2060, 9612
(            
 @idOrgao  INT = NULL,
 @idUo   INT = NULL,
 @idUge   INT = NULL,
 @idUa   INT = NULL,
 @idResponsavel INT = NULL,
 @idDivisao INT = NULL
)
AS             
BEGIN            
 SELECT
 [divisaoUA].[id],
  CONVERT(VARCHAR(MAX), [divisaoUA].[Code]) +' - '+ [divisaoUA].[Description] as [Dados_Divisao],
  ISNULL(i.Name, '') +'-'+ a.NumberIdentification as [Sigla_Chapa],
  CAST(a.MaterialItemCode AS VARCHAR(20)) +' - '+ [sdi].[Description] as [Dados_BP],
  a.Id as [AssetID],
  '3' as [TipoInventario]
 FROM             
  Asset as a WITH (NOLOCK)              
 INNER JOIN AssetMovements as am WITH (NOLOCK) on a.Id = am.AssetId
 LEFT JOIN Section as divisaoUA WITH (NOLOCK) on divisaoUA.Id = am.SectionId
 INNER JOIN Responsible as d WITH (NOLOCK) on d.id = am.ResponsibleId
 INNER JOIN Institution as e WITH (NOLOCK) on e.id = am.InstitutionId
 INNER JOIN BudgetUnit as f WITH (NOLOCK) on f.id = am.BudgetUnitId
 INNER JOIN ManagerUnit as g WITH (NOLOCK) on g.id = am.ManagerUnitId 
 LEFT JOIN AdministrativeUnit as h WITH (NOLOCK) on h.id = am.AdministrativeUnitId
 INNER JOIN Initial as i WITH (NOLOCK) on i.Id = a.InitialId
 LEFT JOIN ShortDescriptionItem as sdi WITH (NOLOCK) on a.ShortDescriptionItemId = sdi.Id
 WHERE
     (am.InstitutionId = @idOrgao OR @idOrgao IS NULL)
 AND (am.BudgetUnitId = @idUo OR @idUo IS NULL)
 AND (am.ManagerUnitId = @idUge OR @idUge IS NULL)
 AND (am.AdministrativeUnitId = @idUa OR @idUa IS NULL)
 AND (am.ResponsibleId = @idResponsavel OR @idResponsavel IS NULL)
 AND (am.SectionId = @idDivisao OR @idDivisao IS NULL)
 AND a.Status = 1 AND am.Status = 1
 AND a.flagVerificado IS NULL
 AND a.flagDepreciaAcumulada = 1
 --AND divisaoUA.Code = 677
 GROUP BY 
	[divisaoUA].[id],[divisaoUA].[Code], [divisaoUA].[Description], i.Name, a.NumberIdentification, a.MaterialItemCode, [sdi].[Description], a.Id
 ORDER BY
 -- a.Id, i.Name, a.NumberIdentification
	divisaoUA.Code ASC,a.NumberIdentification ASC
END



GO
/****** Object:  StoredProcedure [dbo].[SAM_VISAO_GERAL_ATIVO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------[SAM_VISAO_GERAL_ATIVO] @Ativo = 1, @Filtro='0.63',@InstitutionId = 6,@Campo ='15',@Size = 10,@PageNumber = 0
CREATE PROCEDURE [dbo].[SAM_VISAO_GERAL_ATIVO]
	@AssetId AS VARCHAR(2000) = NULL,
	@InstitutionId INT =NULL,
	@BudgetUnitId INT = NULL,
	@ManagerUnitId INT = NULL,
	@AdministrativeUnitId INT = NULL,
	@Size SMALLINT = NULL,
	@PageNumber INT = NULL,
	@Estado TINYINT = NULL,
	@Filtro VARCHAR(2000) = NULL,
	@Campo VARCHAR(50) = NULL,
	@CPF varchar(11) = NULL
AS

DECLARE @SectionId INT = NULL;
DECLARE @SectionDescription VARCHAR(200) = NULL;
DECLARE @ResposibleName VARCHAR(100) = NULL;
DECLARE @ValueAcquisition DECIMAL(18,2);
DECLARE @AccumulatedDepreciation DECIMAL(18,2);
DECLARE @RateDepreciationMonthly DECIMAL(18,2);
DECLARE @CurrentValue DECIMAL(18,2) = NULL;
DECLARE @BookAccount INT = NULL;
DECLARE @AuxiliaryAccountDescription VARCHAR(100) = NULL;
DECLARE @Empenho VARCHAR(20) = NULL;
DECLARE @NumberDoc VARCHAR(20) = NULL;
DECLARE	@NumberIdentification VARCHAR(20) = NULL;
DECLARE @MaterialItemCode INT =NULL;
DECLARE @MaterialItemDescription VARCHAR(200) = NULL;
DECLARE @ValueUpdate DECIMAL(18,2) = NULL;
DECLARE @GrupCode AS VARCHAR(120) = NULL;
DECLARE @LifeCycle INT = NULL;
DECLARE @AuxiliaryAccountId INT = NULL;
DECLARE @InitialName VARCHAR(10) = NULL;
DECLARE @FiltroAnd AS VARCHAR(8000) = '';
DECLARE @FiltroOr AS VARCHAR(2000) = '';
DECLARE @NumberIdentificationRetorno VARCHAR(20)= NULL;
DECLARE @ChapaCompleta VARCHAR(20)= NULL;
DECLARE @SQL AS VARCHAR(8000) = '';
DECLARE @BuscaGeral AS BIT = 0;


	IF(@AssetId IS NOT NULL)
	BEGIN
		SET @SQL = 'DECLARE @AssetId AS VARCHAR(2000); SET @AssetId =''' + @AssetId + '''';
	END;
	IF(@Campo IS NOT NULL AND @Filtro IS NULL)
	BEGIN
		SET @Campo = NULL;
	END;

	IF(@Campo IS NULL)
	BEGIN
		IF(@Filtro IS NOT NULL AND ISNUMERIC(@Filtro) = 1)
		BEGIN
			SET @BuscaGeral = 1
			SET @NumberIdentificationRetorno = (SELECT TOP 1 [NumberIdentification] FROM [dbo].[Asset] WHERE [NumberIdentification] LIKE '%' + @Filtro + '%')
			
			SET @MaterialItemCode = (SELECT TOP 1 [MaterialItemCode] FROM [dbo].[Asset] WHERE [MaterialItemCode]  LIKE '%' + @Filtro + '%')
			
			SET @RateDepreciationMonthly = (SELECT TOP 1 [RateDepreciationMonthly] FROM [dbo].[MonthlyDepreciation] WHERE [RateDepreciationMonthly] = CAST(@Filtro AS DECIMAL(18,2)))
			
			SET @ValueUpdate = (SELECT TOP 1 [ValueUpdate] FROM [dbo].[Asset] WHERE [ValueUpdate] = CAST(@Filtro AS DECIMAL(28,2)))
			
			SET @NumberDoc  = (SELECT TOP 1 [aset].[NumberDoc] 
								   FROM [dbo].[Asset] [aset] 
							       inner join [dbo].[AssetMovements] [asetMovement]
								   ON [aset].[Id] = [asetMovement].[AssetId]
								   WHERE [asetMovement].[NumberDoc] = @Filtro 
								   OR ([aset].[NumberDoc] = @Filtro 
								   AND [asetMovement].[NumberDoc] IS NULL));
								    
			SET @LifeCycle = (SELECT TOP 1 [LifeCycle] FROM [dbo].[MaterialGroup] WHERE [LifeCycle] = CAST(@Filtro AS BIGINT))

		END;
		ELSE
	            BEGIN		
	            IF(@Filtro IS NOT NULL)
	            BEGIN
	            SET @BuscaGeral = 1
	            
				SET @ChapaCompleta = (SELECT TOP 1 [ChapaCompleta] FROM [dbo].[Asset] WHERE [ChapaCompleta] LIKE '%' + @Filtro + '%')
				IF(@ChapaCompleta IS NOT NULL)
	            BEGIN
	            	SET @FiltroOr = @FiltroOr + ' OR [Extent1].[ChapaCompleta] LIKE ''%' + @Filtro + '%'''
	            END

	            SET @InitialName = (SELECT TOP 1 [InitialName] FROM [dbo].[Asset] WHERE [InitialName] LIKE '%' +  @Filtro + '%')
	            IF(@InitialName IS NOT NULL)
	            BEGIN
	            	SET @FiltroOr = @FiltroOr + ' OR [Extent1].[InitialName] LIKE ''%' + @Filtro + '%'''
	            END
	            
	            SET @GrupCode =(SELECT TOP 1 [ma].[Code] 
	            				  FROM [dbo].[MaterialGroup] [ma]  WHERE [ma].[Description] LIKE '%' + @Filtro + '%');
	            IF(@GrupCode IS NOT NULL)
	            BEGIN
	            	SET @FiltroOr = @FiltroOr + ' OR [Extent1].[MaterialGroupCode] IN(SELECT [Code] FROM [dbo].[MaterialGroup] [ma] WHERE [ma].[Description] =''%' + @Filtro + '%'')';
	            END;
	            	
	            IF(UPPER(@Filtro) <> 'DECRETO' AND UPPER(@Filtro) <> 'ACERVO' AND UPPER(@Filtro) <> 'TERCEIRO')
	            BEGIN
	            	DECLARE @Id AS INT = (SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [MaterialItemDescription] LIKE '%' + @Filtro + '%')
	            
	            	IF(@Id IS NOT NULL)
	            	BEGIN
	            		SET @FiltroOr = @FiltroOr + ' OR [Extent1].[Id] =' + CAST(@Id AS VARCHAR(20));
	            	END;
	            END
	            		
	            IF(UPPER(@Filtro) <> 'DECRETO' AND UPPER(@Filtro) <> 'ACERVO' AND UPPER(@Filtro) <> 'TERCEIRO')
	            BEGIN
	            	SET @SectionDescription = (SELECT TOP 1 [sec].[Id] 
	            			  FROM [dbo].[Section] [sec] WHERE [sec].[Description] LIKE '%' + @Filtro + '%');
	            	IF(@SectionDescription IS NOT NULL)
	            	BEGIN
	            		SET @FiltroOr = @FiltroOr + ' OR [Extent9].[Description] LIKE ''%' +  @Filtro + '%''';
	                   END;
	            END;
	            				
	            SET @ResposibleName = (SELECT TOP 1 [res].[Id] FROM [dbo].[Responsible] [res] WHERE [res].[Name] LIKE '%' + @Filtro + '%');
	            
	            IF(@ResposibleName IS NOT NULL)
	            BEGIN
	            	SET @FiltroOr = @FiltroOr + ' OR [Extent10].[Name] LIKE ''%' +  @Filtro + '%''';
	            END;
	            				
	            SET @AuxiliaryAccountDescription  = (SELECT TOP 1 [aux].[Id] FROM [dbo].[AuxiliaryAccount] [aux] WHERE [aux].[Description] LIKE '%' + @Filtro + '%');
	            IF(@AuxiliaryAccountDescription IS NOT NULL)
	            BEGIN
	            	SET @FiltroOr = @FiltroOr + ' OR [Extent11].[Description] LIKE ''%' +  @Filtro + '%''';
	            END		
			END;
		END;
	END;
	ELSE
	BEGIN
		IF(@Campo ='1')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent1].[InitialName] LIKE ''%' +  @Filtro + '%'')';
		END
		IF(@Campo ='2')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent1].[ChapaCompleta] LIKE ''%' +  @Filtro + '%'')';
		END
		IF(@Campo ='3')
		BEGIN
			IF(ISNUMERIC(@Filtro) = 1)
			BEGIN
				SET @FiltroAnd = ' AND ([Extent1].[MaterialGroupCode] =' +  @Filtro + ')';
			END
			ELSE
			BEGIN
				SET @FiltroAnd = ' AND 1 = 0';
			END
		END
		IF(@Campo ='4')
		BEGIN
			IF(ISNUMERIC(@Filtro) = 1)
			BEGIN
				SET @FiltroAnd = ' AND ([Extent1].[MaterialItemCode] =' +  @Filtro + ')';
			END
			ELSE
			BEGIN
				SET @FiltroAnd = ' AND 1 = 0';
			END
		END
		IF(@Campo ='5')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent1].[MaterialItemDescription] LIKE ''%' +  @Filtro + '%'')';
		END
		IF(@Campo ='6')
		BEGIN
			SET @InstitutionId = (SELECT TOP 1 [orgao].[Id] FROM [dbo].[Institution] [orgao] WHERE [orgao].[Code] = '' + @Filtro + '' OR [orgao].[Description] LIKE '%' + @Filtro + '%')
			IF(@InstitutionId IS NOT NULL)
				SET @FiltroAnd = ' AND ([Extent2].[InstitutionId] =' + CAST(@InstitutionId AS VARCHAR(20)) + ')';

		END
		IF(@Campo ='7')
		BEGIN
			SET @BudgetUnitId = (SELECT TOP 1 [uo].[Id] FROM [dbo].[BudgetUnit] [uo] WHERE [uo].[Code] = '' + @Filtro + '' OR [uo].[Description] LIKE '%' + @Filtro + '%');

			IF(@BudgetUnitId IS NOT NULL)
				SET @FiltroAnd = ' AND ([Extent2].[BudgetUnitId] =' + CAST(@BudgetUnitId AS VARCHAR(20)) + ')';
		END
		IF(@Campo ='8')
		BEGIN
			SET @ManagerUnitId = (SELECT TOP 1 [uge].[Id] FROM [dbo].[ManagerUnit] [uge] WHERE [uge].[Code] = '' + @Filtro + '' OR [uge].[Description] LIKE '%' + @Filtro + '%');
			IF(@ManagerUnitId IS NOT NULL)
				SET @FiltroAnd = ' AND ([Extent2].[ManagerUnitId] =' + CAST(@ManagerUnitId AS VARCHAR(20)) + ')';
		END
		IF(@Campo ='9')
		BEGIN
			IF(ISNUMERIC(@Filtro) = 1)
			BEGIN
				SET @AdministrativeUnitId = (SELECT TOP 1 [ua].[Id] FROM [dbo].[AdministrativeUnit] [ua] WHERE [ua].[Code] =  CAST(@Filtro AS BIGINT));
				IF(@AdministrativeUnitId IS NOT NULL)
					SET @FiltroAnd = ' AND ([Extent2].[AdministrativeUnitId] =' + CAST(@AdministrativeUnitId AS VARCHAR(20)) + ')';
				END
			ELSE
			BEGIN
				SET @FiltroAnd = ' AND 1 = 0';
			END
		END
		IF(@Campo ='10')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent2].[SectionId] IN(SELECT [id] FROM [dbo].[Section] WHERE [Description] LIKE ''%' + @Filtro + '%''))'
		END
		IF(@Campo ='11')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent2].[ResponsibleId] IN(SELECT [id] FROM [dbo].[Responsible] WHERE [Name] LIKE ''%' + @Filtro + '%''))'
		END
		IF(@Campo ='12')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[ValueAcquisition] =CAST(' + REPLACE(@Filtro,',','.') + ' AS DECIMAL(18,2)))' 
		END
		IF(@Campo ='13')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[AssetStartId] IN(SELECT [AssetStartId] FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode]' + 
							 +' AND Convert(int,Convert(varchar(6),[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])' + 
							 +' AND [AccumulatedDepreciation] LIKE ''%' + REPLACE(@Filtro,',','.') + '%''))'
		END
		IF(@Campo ='14')
		BEGIN
			SET @FiltroAnd = 'AND CAST(''' + REPLACE(@Filtro,',','.') + ''' AS DECIMAL(18,2)) = CASE '+
						     +' WHEN [Extent1].[flagAcervo] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition]) '+
							 +' WHEN [Extent1].[flagTerceiro] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition]) '+
							 +' ELSE ISNULL((SELECT top 1 CurrentValue FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode] AND Convert(int,Convert(varchar(6),[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId]) AND [AssetStartId] = [Extent1].[AssetStartId]), ' +
						     + 'CASE WHEN [Extent1].[flagDecreto] = 1 THEN [Extent1].[ValueUpdate] ELSE [Extent1].[ValueAcquisition] END) END '
		END
		IF(@Campo ='15')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[AssetStartId] IN(SELECT [AssetStartId] FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode]' +
							 +' AND Convert(int,Convert(varchar(6),[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])' + 
							 +' AND [RateDepreciationMonthly] =' + REPLACE(@Filtro,',','.')  +'))'
		END
		IF(@Campo ='16')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[MaterialGroupCode] IN(SELECT [Code] FROM [dbo].[MaterialGroup] WHERE [Description] =''%' + @Filtro  +'%''))';
		END

		IF(@Campo ='17')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent11].[ContaContabilApresentacao] LIKE ''%' + @Filtro + '%'')';
			
		END
		IF(@Campo ='18')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent11].[Description] LIKE ''%' + @Filtro  +'%'')';
		END
		IF(@Campo ='19')
		BEGIN
			SET @FiltroAnd = 'AND (CONVERT(varchar,[Extent1].[AcquisitionDate],103) LIKE ''%' + @Filtro + '%'')' ;
		END
		IF(@Campo ='20')
		BEGIN
			SET @FiltroAnd = 'AND (CONVERT(varchar,[Extent1].[MovimentDate],103) LIKE ''%' + @Filtro + '%'')' ;
		END
		IF(@Campo ='21')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[Empenho] LIKE ''%' + @Filtro  +'%'')';
		END
		IF(@Campo ='22')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent2].[NumberDoc] =''' + @Filtro  +''' OR ([Extent1].[NumberDoc] =''' + @Filtro  +''' AND [Extent2].[NumberDoc] IS NULL))';
		END
		IF(@Campo ='23')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent14].[Description] LIKE ''%' + @Filtro  +'%'')';
		END
		IF(@Campo ='24')
		BEGIN
			SET @FiltroAnd = 'AND (CONVERT(varchar,[Extent2].[MovimentDate],103) LIKE ''%' + @Filtro + '%'')' ;
		END
		IF(@CAMPO ='25')
		BEGIN
			IF(@Filtro ='Acervo')
				SET @FiltroAnd = ' AND [Extent1].[flagAcervo] = 1';
			ELSE
				IF(@Filtro ='Terceiro')
					SET @FiltroAnd = ' AND [Extent1].[flagTerceiro] = 1';
					ELSE
						IF(@Filtro ='Decreto')
							SET @FiltroAnd = ' AND [Extent1].[flagDecreto] = 1';
		END;
	END;

	
	IF(@Size IS NOT NULL)
	BEGIN 
		IF(@PageNumber IS NULL)
		BEGIN
			SET @PageNumber = 10;
		END
		SET @SQL = @SQL + '; WITH VISAO AS( SELECT  [Extent1].[Id] AS ''Id'',';
	END
	ELSE
	BEGIN 
		SET @SQL = @SQL + '; SELECT  [Extent1].[Id] AS ''Id'','

	END
		SET @SQL = @SQL + ' [Extent1].[ChapaCompleta] AS ''Chapa'',  
		[Extent1].[MaterialItemCode] AS ''Item'',  
		CAST([Extent1].[MaterialGroupCode] AS VARCHAR(20)) ''GrupoItem'',
		[Extent1].[ValueAcquisition] ''ValorDeAquisicao'',
		[Extent1].[Empenho],
		ISNULL([Extent2].[NumberDoc], [Extent1].[NumberDoc]) AS ''NumeroDocumento'',
		(CASE WHEN [Extent1].[flagAcervo] = 1 THEN ''Acervo'' 
		      ELSE 
			     CASE WHEN [Extent1].[flagTerceiro] = 1 THEN ''Terceiro'' 
			          ELSE  CASE WHEN [Extent1].[flagDecreto] = 1 THEN ''Decreto'' ELSE '''' END
			      END
		  END) AS ''TipoBp'',
		[Extent1].[flagAcervo] ''flagAcervo'',
		[Extent1].[flagTerceiro] ''flagTerceiro'',
		[Extent2].[MovementTypeId], 
		[Extent2].[Status],  
		[Extent2].[NumberDoc] AS ''NumeroDocumentoAtual'', 
		[Extent3].[Name] ''Sigla'', 
		[Extent4].[Description] AS ''DescricaoDoItem'', 
		[Extent5].[NameManagerReduced] AS ''Orgao'', 
		[Extent6].[Code] AS ''UO'', 
		[Extent7].[Code] AS ''UGE'', 
		[Extent8].[Code] AS ''UA'', 
		[Extent9].[Code] AS ''DivisaoCode'', 
		[Extent9].[Description] AS ''DescricaoDaDivisao'', 
		[Extent10].[Name] AS ''Responsavel'', 
		[Extent11].[ContaContabilApresentacao] AS ''ContaContabil'',
		[Extent11].[Description] AS ''DescricaoContaContabil'',
		CASE
			WHEN [Extent1].[flagAcervo] = 1 THEN 0
			WHEN [Extent1].[flagTerceiro] = 1 THEN 0
			ELSE
		(SELECT MAX([Extent12].[AccumulatedDepreciation])
		  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
		  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
		    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
		    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
			AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
		) END AS ''DepreciacaoAcumulada'',
		CASE
			WHEN [Extent1].[flagAcervo] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition])
			WHEN [Extent1].[flagTerceiro] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition])
			WHEN [Extent1].[flagDecreto] = 1 
		    THEN 
				ISNULL((SELECT MIN([Extent12].[CurrentValue])
			  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
			  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
			    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
			    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
				AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
			), [Extent1].[ValueUpdate])
			ELSE
			ISNULL((SELECT MIN([Extent12].[CurrentValue])
			  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
			  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
			    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
			    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
				AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
		), [Extent1].[ValueAcquisition]) 
		END AS ''ValorAtual'',
		CASE
			WHEN [Extent1].[flagAcervo] = 1 THEN 0
			WHEN [Extent1].[flagTerceiro] = 1 THEN 0
			ELSE
		(SELECT TOP 1 [Extent12].[RateDepreciationMonthly]
		  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
		  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
		    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
		    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
			AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
		  ORDER BY [Extent12].[Id] DESC
		) END AS ''DepreciacaoMensal'',
		(SELECT MAX([Extent12].[CurrentMonth])
		  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
		  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
		    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
		    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
			AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
		) AS ''VidaUtil'',
		(SELECT MAX([Extent13].[LifeCycle])
		  FROM [dbo].[MaterialGroup] [Extent13] WITH(NOLOCK) 
		  WHERE [Extent13].[Code] = [Extent1].[MaterialGroupCode] 
		) AS ''LifeCycle'',
		CONVERT(varchar,[Extent2].[MovimentDate],103) AS ''DataUltimoHistorico'',
		CONVERT(varchar,[Extent1].[AcquisitionDate],103) AS ''DataAquisicao'',
		CONVERT(varchar,[Extent1].[MovimentDate],103) AS ''DataIncorporacao'',
		[Extent14].[Description] AS ''UltimoHistorico'', 
		[Extent15].[Id] ''IdAReclassificar'','

		IF(@Estado != 0)
		BEGIN
			SET @SQL = @SQL + 'ROW_NUMBER() OVER(ORDER BY  
						   CASE WHEN(11 = [Extent2].[MovementTypeId]) THEN cast(1 as bit)                           
						        WHEN(11 <> [Extent2].[MovementTypeId]) THEN cast(0 as bit)                       
						   END DESC,                      
						   CASE WHEN(12 = [Extent2].[MovementTypeId])  THEN cast(1 as bit)                          
						        WHEN(12 <> [Extent2].[MovementTypeId]) THEN cast(0 as bit) 
						    END DESC, [Extent1].[NumberIdentification] ASC, [InitialName] ASC) AS RowNum '  

		END;
		ELSE
		BEGIN
			SET @SQL = @SQL + 'ROW_NUMBER() OVER(ORDER BY [Extent2].[Id] DESC, [Extent2].[MovimentDate] DESC) AS RowNum '
		END;


		 
		SET @SQL = @SQL + 'FROM [dbo].[Asset] AS [Extent1] WITH(NOLOCK)    
		INNER JOIN [dbo].[Initial] AS [Extent3] WITH(NOLOCK) ON [Extent3].[Id] = [Extent1].[InitialId]    
		INNER JOIN [dbo].[ShortDescriptionItem] AS [Extent4] WITH(NOLOCK) ON [Extent4].[Id] = [Extent1].[ShortDescriptionItemId]    
		INNER JOIN [dbo].[AssetMovements] AS [Extent2] WITH(NOLOCK) ON[Extent1].[Id] = [Extent2].[AssetId]    
		INNER JOIN [dbo].[Institution] AS [Extent5] WITH(NOLOCK) ON [Extent5].[Id] = [Extent2].[InstitutionId]    
		INNER JOIN [dbo].[BudgetUnit] AS [Extent6] WITH(NOLOCK) ON [Extent6].[Id] = [Extent2].[BudgetUnitId]    
		INNER JOIN [dbo].[ManagerUnit] AS [Extent7] WITH(NOLOCK) ON [Extent7].[Id] = [Extent2].[ManagerUnitId]
		INNER JOIN [dbo].[MovementType] AS [Extent14] WITH(NOLOCK) ON [Extent14].[Id] = [Extent2].[MovementTypeId]
		LEFT OUTER JOIN [dbo].[AdministrativeUnit] AS[Extent8] WITH(NOLOCK) ON [Extent8].[Id] = [Extent2].[AdministrativeUnitId]    
		LEFT OUTER JOIN [dbo].[Section] AS [Extent9] WITH(NOLOCK) ON [Extent9].[Id] = [Extent2].[SectionId]    
		LEFT OUTER JOIN [dbo].[Responsible] AS [Extent10]  WITH(NOLOCK) ON [Extent10].[Id] = [Extent2].[ResponsibleId]  
		LEFT OUTER JOIN [dbo].[AuxiliaryAccount] AS [Extent11] WITH(NOLOCK) ON [Extent11].[Id] = [Extent2].[AuxiliaryAccountId] '
		
		IF(@Estado = 3)
		BEGIN
			SET @SQL = @SQL + ' INNER JOIN [dbo].[BPsARealizaremReclassificacaoContabil] AS [Extent15] WITH(NOLOCK) ON [Extent15].[AssetId] = [Extent1].[Id]';
		END
		ELSE
		BEGIN
			SET @SQL = @SQL + ' LEFT OUTER JOIN [dbo].[BPsARealizaremReclassificacaoContabil] AS [Extent15] WITH(NOLOCK) ON [Extent15].[AssetId] = [Extent1].[Id]';
		END

		SET @SQL = @SQL + ' WHERE  ([Extent1].[flagVerificado] IS NULL)        
		  AND ([Extent1].[flagDepreciaAcumulada] = 1)
		  AND ([Extent2].[flagEstorno] IS NULL)
		  AND ([Extent2].[DataEstorno] IS NULL) '
		  
		  IF(@Estado IS NOT NULL)
		  BEGIN
			IF(@Estado = 1 OR @Estado = 3)
			BEGIN
				SET @SQL = @SQL + ' AND ([Extent2].[Status] = 1)';
			END
			ELSE IF(@Estado = 2)
			BEGIN
				SET @SQL = @SQL + ' AND ([Extent2].[Status] = 1) AND ([Extent2].[MovementTypeId] IN(11,12,47,56,57))'
			END
			ELSE
			BEGIN
				SET @SQL = @SQL + ' AND ([Extent1].[Status] = 0) AND ([Extent2].[Status] = 0) AND ([Extent2].[MovementTypeId] IN(11,12,13,14,15,16,17,18,42,43,45,46,47,48,49,50,51,52,53,55,56,57,58,59,95))';
			END
			
		  END
		  ELSE
		  BEGIN
			SET @SQL = @SQL + ' AND (0 = 1)';
		  END

	IF(@BuscaGeral = 1)
	BEGIN
		SET @SQL = @SQL + ' AND ( 0=1 '
	END	  

	IF(@NumberIdentificationRetorno IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' OR [Extent1].[NumberIdentification] LIKE ''%' + @Filtro + '%''' 
	END

	IF(@MaterialItemCode IS NOT  NULL)
	BEGIN
		SET @SQL = @SQL + ' OR [Extent1].[MaterialItemCode] LIKE ''%' + @Filtro +'%''' 
	END

	IF(@RateDepreciationMonthly IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' OR [Extent1].[AssetStartId] IN(SELECT [AssetStartId] FROM [dbo].[MonthlyDepreciation] WHERE  [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode] AND [RateDepreciationMonthly] =' + CAST(@RateDepreciationMonthly AS VARCHAR(20)) + ')' 
	END 

	IF(@ValueUpdate IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' OR [Extent1].[ValueUpdate] =' + CAST(@ValueUpdate AS VARCHAR(20)) + '' 
	END
	IF(@LifeCycle IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' OR [Extent1].[MaterialGroupCode] IN(SELECT [Code] FROM [dbo].[MaterialGroup] WHERE LifeCycle =' + CAST(@LifeCycle AS VARCHAR(20))+ ')' 
	END	  
	
	IF(@AssetId IS NOT NULL)
	BEGIN
		SET @SQL = @SQL +  ' AND ([Extent1].[Id] IN(SELECT * FROM [dbo].[fnSplitString](@AssetId,'','')))'
	END;
	IF(@NumberDoc IS NOT NULL)
	BEGIN
		SET @SQL = @SQL +  ' OR ([Extent2].[NumberDoc] =''' + @Filtro  +''' OR ([Extent1].[NumberDoc] =''' + @Filtro  + ''' AND [Extent2].[NumberDoc] IS NULL)) OR [Extent8].Code LIKE ''%' + @Filtro +'%'''
	END;

	IF(@FiltroOr IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + @FiltroOr
	END;

	IF(@BuscaGeral = 1)
	BEGIN
		SET @SQL = @SQL + ')'
	END	

	IF(@InstitutionId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[InstitutionId] =' + CAST(@InstitutionId AS VARCHAR(20)) + ')';
	END
	IF(@BudgetUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[BudgetUnitId] =' + CAST(@BudgetUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@ManagerUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[ManagerUnitId] =' + CAST(@ManagerUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@AdministrativeUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[AdministrativeUnitId] =' + CAST(@AdministrativeUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@CPF IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[ResponsibleId] IN (select Id from Responsible where cpf = ''' + @CPF+'''))';
	END;
	SET @SQL = @SQL + @FiltroAnd;
	IF(@Size IS NOT NULL)
	BEGIN 
		IF(@PageNumber IS NULL)
		BEGIN
			SET @PageNumber = 10;
		END
		SET @SQL = @SQL + ') SELECT TOP(' + CAST(@Size AS VARCHAR(20)) +') [VISAO].* FROM [VISAO] WHERE (RowNum > ' + CAST(@PageNumber AS VARCHAR(20)) + ' AND RowNum <=' + CAST((@Size + @PageNumber) AS VARCHAR(20)) + ') ORDER BY RowNum ';
END
	
	EXEC(@SQL);
	--PRINT(@SQL)
	--PRINT(@FiltroAnd)
	--PRINT(@FiltroOr)

GO
/****** Object:  StoredProcedure [dbo].[SAM_VISAO_GERAL_COUNT]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SAM_VISAO_GERAL_COUNT]
	@AssetId AS VARCHAR(2000) = NULL,
	@InstitutionId INT =NULL,
	@BudgetUnitId INT = NULL,
	@ManagerUnitId INT = NULL,
	@AdministrativeUnitId INT = NULL,
	@Size SMALLINT = NULL,
	@PageNumber INT = NULL,
	@Estado TINYINT = NULL,
	@Filtro VARCHAR(2000) = NULL,
	@Campo VARCHAR(50) = NULL,
	@CPF varchar(11) = NULL
AS

DECLARE @SectionId INT = NULL;
DECLARE @SectionDescription VARCHAR(200) = NULL;
DECLARE @ResposibleName VARCHAR(100) = NULL;
DECLARE @ValueAcquisition DECIMAL(18,2);
DECLARE @AccumulatedDepreciation DECIMAL(18,2);
DECLARE @RateDepreciationMonthly DECIMAL(18,2);
DECLARE @CurrentValue DECIMAL(18,2) = NULL;
DECLARE @BookAccount INT = NULL;
DECLARE @AuxiliaryAccountDescription VARCHAR(100) = NULL;
DECLARE @Empenho VARCHAR(20) = NULL;
DECLARE @NumberDoc VARCHAR(20) = NULL;
DECLARE	@NumberIdentification VARCHAR(20) = NULL;
DECLARE @MaterialItemCode INT =NULL;
DECLARE @MaterialItemDescription VARCHAR(200) = NULL;
DECLARE @ValueUpdate DECIMAL(18,2) = NULL;
DECLARE @GrupCode AS VARCHAR(120) = NULL;
DECLARE @LifeCycle INT = NULL;
DECLARE @AuxiliaryAccountId INT = NULL;
DECLARE @InitialName VARCHAR(10) = NULL;
DECLARE @FiltroAnd AS VARCHAR(8000) = '';
DECLARE @FiltroOr AS VARCHAR(2000) = '';
DECLARE @NumberIdentificationRetorno VARCHAR(20)= NULL;
DECLARE @ChapaCompleta VARCHAR(20)= NULL;
DECLARE @BuscaGeral AS BIT = 0;

DECLARE @SQL AS VARCHAR(8000) = '';

	IF(@AssetId IS NOT NULL)
	BEGIN
		SET @SQL = 'DECLARE @AssetId AS VARCHAR(2000); SET @AssetId =''' + @AssetId + '''';
	END;
	IF(@Campo IS NOT NULL AND @Filtro IS NULL)
	BEGIN
		SET @Campo = NULL;
	END;
	IF(@Campo IS NULL)
	BEGIN
		IF(@Filtro IS NOT NULL AND ISNUMERIC(@Filtro) = 1)
			BEGIN
			SET @BuscaGeral = 1;
			SET @NumberIdentificationRetorno = (SELECT TOP 1 [NumberIdentification] FROM [dbo].[Asset] WHERE [NumberIdentification] LIKE '%' + @Filtro + '%')
			
			SET @RateDepreciationMonthly = (SELECT TOP 1 [RateDepreciationMonthly] FROM [dbo].[MonthlyDepreciation] WHERE [RateDepreciationMonthly] = CAST(@Filtro AS DECIMAL(28,2)))
			
			SET @ValueUpdate = (SELECT TOP 1 [ValueUpdate] FROM [dbo].[Asset] WHERE [ValueUpdate] = CAST(@Filtro AS DECIMAL(28,2)))
			
			SET @NumberDoc  = (SELECT TOP 1 [aset].[NumberDoc] 
							   FROM [dbo].[Asset] [aset] 
						       inner join [dbo].[AssetMovements] [asetMovement]
							   ON [aset].[Id] = [asetMovement].[AssetId]
							   WHERE [asetMovement].[NumberDoc] = @Filtro OR ([aset].[NumberDoc] = @Filtro AND [asetMovement].[NumberDoc] IS NULL));
			
			SET @LifeCycle = (SELECT TOP 1 [LifeCycle] FROM [dbo].[MaterialGroup] WHERE [LifeCycle] = CAST(@Filtro AS BIGINT))

				
		END;
		ELSE
		BEGIN
			IF(@Filtro IS NOT NULL)
			BEGIN
				SET @BuscaGeral = 1;

				SET @ChapaCompleta = (SELECT TOP 1 [ChapaCompleta] FROM [dbo].[Asset] WHERE [ChapaCompleta] LIKE '%' + @Filtro + '%')
				IF(@ChapaCompleta IS NOT NULL)
	            BEGIN
	            	SET @FiltroOr = @FiltroOr + ' OR [Extent1].[ChapaCompleta] LIKE ''%' + @Filtro + '%'''
	            END

				SET @InitialName = (SELECT TOP 1 [InitialName] FROM [dbo].[Asset] WHERE [InitialName] LIKE '%' +  @Filtro + '%')
				IF(@InitialName IS NOT NULL)
				BEGIN
					SET @FiltroOr = @FiltroOr + ' OR [Extent1].[InitialName] LIKE ''%' + @Filtro + '%'''
				END
				
				SET @GrupCode =(SELECT TOP 1 [ma].[Code] 
								  FROM [dbo].[MaterialGroup] [ma]  WHERE [ma].[Description] LIKE '%' + @Filtro + '%');
				IF(@GrupCode IS NOT NULL)
				BEGIN
					SET @FiltroOr = @FiltroOr + ' OR [Extent1].[MaterialGroupCode] IN(SELECT [Code] FROM [dbo].[MaterialGroup] [ma] WHERE [ma].[Description] =''%' + @Filtro + '%'')';
				END;
				
				IF(UPPER(@Filtro) <> 'DECRETO' AND UPPER(@Filtro) <> 'ACERVO' AND UPPER(@Filtro) <> 'TERCEIRO')
				BEGIN
					DECLARE @Id AS INT = (SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [MaterialItemDescription] LIKE '%' + @Filtro + '%')
					IF(@Id IS NOT NULL)
					BEGIN
						SET @FiltroOr = @FiltroOr + ' OR [Extent1].[Id] =' + CAST(@Id AS VARCHAR(20));
					END;
				END
			
				IF(UPPER(@Filtro) <> 'DECRETO' AND UPPER(@Filtro) <> 'ACERVO' AND UPPER(@Filtro) <> 'TERCEIRO')
				BEGIN
					SET @SectionDescription = (SELECT TOP 1 [sec].[Id] 
							  FROM [dbo].[Section] [sec] WHERE [sec].[Description] LIKE '%' + @Filtro + '%');
					IF(@SectionDescription IS NOT NULL)
					BEGIN
						SET @FiltroOr = @FiltroOr + ' OR [Extent9].[Description] LIKE ''%' +  @Filtro + '%''';
					END;
				END;
				
				SET @ResposibleName = (SELECT TOP 1 [res].[Id] FROM [dbo].[Responsible] [res] WHERE [res].[Name] LIKE '%' + @Filtro + '%');
				IF(@ResposibleName IS NOT NULL)
				BEGIN
					SET @FiltroOr = @FiltroOr + ' OR [Extent10].[Name] LIKE ''%' +  @Filtro + '%''';
				END;
					
				SET @AuxiliaryAccountDescription  = (SELECT TOP 1 [aux].[Id] FROM [dbo].[AuxiliaryAccount] [aux] WHERE [aux].[Description] LIKE '%' + @Filtro + '%');
				IF(@AuxiliaryAccountDescription IS NOT NULL)
				BEGIN
					SET @FiltroOr = @FiltroOr + ' OR [Extent11].[Description] LIKE ''%' +  @Filtro + '%''';
				END
				ELSE
				BEGIN
				IF(@Filtro ='Acervo')
					SET @FiltroAnd = ' AND [Extent1].[flagAcervo] = 1';
				ELSE
					IF(@Filtro ='Terceiro')
						SET @FiltroAnd = ' AND [Extent1].[flagTerceiro] = 1';
						ELSE
							IF(@Filtro ='Decreto')
								SET @FiltroAnd = ' AND [Extent1].[flagDecreto] = 1';
							END;
						END;
					END;
		END;
	ELSE
	BEGIN
		IF(@Campo ='1')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent1].[InitialName] LIKE ''%' +  @Filtro + '%'')';
		END
		IF(@Campo ='2')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent1].[ChapaCompleta] LIKE ''%' +  @Filtro + '%'')';
		END
		IF(@Campo ='3')
		BEGIN
			IF(ISNUMERIC(@Filtro) = 1)
			BEGIN
				SET @FiltroAnd = ' AND ([Extent1].[MaterialGroupCode] =' +  @Filtro + ')';
			END
			ELSE
			BEGIN
				SET @FiltroAnd = ' AND 1 = 0';
			END
		END
		IF(@Campo ='4')
		BEGIN
			IF(ISNUMERIC(@Filtro) = 1)
			BEGIN
				SET @FiltroAnd = ' AND ([Extent1].[MaterialItemCode] =' +  @Filtro + ')';
			END
			ELSE
			BEGIN
				SET @FiltroAnd = ' AND 1 = 0';
			END
		END
		IF(@Campo ='5')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent1].[MaterialItemDescription] LIKE ''%' +  @Filtro + '%'')';
		END
		IF(@Campo ='6')
		BEGIN
			SET @InstitutionId = (SELECT TOP 1 [orgao].[Id] FROM [dbo].[Institution] [orgao] WHERE [orgao].[Code] = '' + @Filtro + '' OR [orgao].[Description] LIKE '%' + @Filtro + '%')
			IF(@InstitutionId IS NOT NULL)
				SET @FiltroAnd = ' AND ([Extent2].[InstitutionId] =' + CAST(@InstitutionId AS VARCHAR(20)) + ')';
		END
		IF(@Campo ='7')
		BEGIN
			SET @BudgetUnitId = (SELECT TOP 1 [uo].[Id] FROM [dbo].[BudgetUnit] [uo] WHERE [uo].[Code] = '' + @Filtro + '' OR [uo].[Description] LIKE '%' + @Filtro + '%');

			IF(@BudgetUnitId IS NOT NULL)
				SET @FiltroAnd = ' AND ([Extent2].[BudgetUnitId] =' + CAST(@BudgetUnitId AS VARCHAR(20)) + ')';
		END
		IF(@Campo ='8')
		BEGIN
			SET @ManagerUnitId = (SELECT TOP 1 [uge].[Id] FROM [dbo].[ManagerUnit] [uge] WHERE [uge].[Code] = '' + @Filtro + '' OR [uge].[Description] LIKE '%' + @Filtro + '%');
			IF(@ManagerUnitId IS NOT NULL)
				SET @FiltroAnd = ' AND ([Extent2].[ManagerUnitId] =' + CAST(@ManagerUnitId AS VARCHAR(20)) + ')';
		END
		IF(@Campo ='9')
		BEGIN
			IF(ISNUMERIC(@Filtro) = 1)
			BEGIN
				SET @AdministrativeUnitId = (SELECT TOP 1 [ua].[Id] FROM [dbo].[AdministrativeUnit] [ua] WHERE [ua].[Code] =  CAST(@Filtro AS BIGINT));
				IF(@AdministrativeUnitId IS NOT NULL)
					SET @FiltroAnd = ' AND ([Extent2].[AdministrativeUnitId] =' + CAST(@AdministrativeUnitId AS VARCHAR(20)) + ')';
			END
			ELSE
			BEGIN
				SET @FiltroAnd = ' AND 1 = 0';
			END
		END
		IF(@Campo ='10')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent2].[SectionId] IN(SELECT [id] FROM [dbo].[Section] WHERE [Description] LIKE ''%' + @Filtro + '%''))'
		END
		IF(@Campo ='11')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent2].[ResponsibleId] IN(SELECT [id] FROM [dbo].[Responsible] WHERE [Name] LIKE ''%' + @Filtro + '%''))'
		END
		IF(@Campo ='12')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[ValueAcquisition] =CAST(' + REPLACE(@Filtro,',','.') + ' AS DECIMAL(18,2)))' 
		END
		IF(@Campo ='13')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[AssetStartId] IN(SELECT [AssetStartId] FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode]' + 
							 +' AND Convert(int,Convert(varchar(6),[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])' + 
							 +' AND [AccumulatedDepreciation] LIKE ''%' + REPLACE(@Filtro,',','.') + '%''))'
		END
	
		IF(@Campo ='14')
		BEGIN
			SET @FiltroAnd = 'AND CAST(''' + REPLACE(@Filtro,',','.') + ''' AS DECIMAL(18,2)) = CASE '+
						     +' WHEN [Extent1].[flagAcervo] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition]) '+
							 +' WHEN [Extent1].[flagTerceiro] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition]) '+
							 +' ELSE ISNULL((SELECT top 1 CurrentValue FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode] AND Convert(int,Convert(varchar(6),[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId]) AND [AssetStartId] = [Extent1].[AssetStartId]), ' +
						     + 'CASE WHEN [Extent1].[flagDecreto] = 1 THEN [Extent1].[ValueUpdate] ELSE [Extent1].[ValueAcquisition] END) END '
		END
		IF(@Campo ='15')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[AssetStartId] IN(SELECT [AssetStartId] FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode]' +
							 +' AND Convert(int,Convert(varchar(6),[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])' + 
							 +' AND [RateDepreciationMonthly] =' + REPLACE(@Filtro,',','.')  +'))'
		END
		IF(@Campo ='16')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[MaterialGroupCode] IN(SELECT [Code] FROM [dbo].[MaterialGroup] WHERE [Description] =''%' + @Filtro  +'%''))';
		END

		IF(@Campo ='17')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent11].[ContaContabilApresentacao] LIKE ''%' + @Filtro + '%'')';
			
		END
		IF(@Campo ='18')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent11].[Description] LIKE ''%' + @Filtro  +'%'')';

		END
		IF(@Campo ='19')
		BEGIN
			SET @FiltroAnd = 'AND (CONVERT(varchar,[Extent1].[AcquisitionDate],103) LIKE ''%' + @Filtro + '%'')' ;
		END
		IF(@Campo ='20')
		BEGIN
			SET @FiltroAnd = 'AND (CONVERT(varchar,[Extent1].[MovimentDate],103) LIKE ''%' + @Filtro + '%'')' ;
		END
		IF(@Campo ='21')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[Empenho]  LIKE ''%' + @Filtro  +'%'')';
		END
		IF(@Campo ='22')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent2].[NumberDoc] =''' + @Filtro  +''' OR ([Extent1].[NumberDoc] =''' + @Filtro  +''' AND [Extent2].[NumberDoc] IS NULL))';
		END
		IF(@Campo ='23')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent14].[Description] LIKE ''%' + @Filtro  +'%'')';
		END
		IF(@Campo ='24')
		BEGIN
			SET @FiltroAnd = 'AND (CONVERT(varchar,[Extent2].[MovimentDate],103) LIKE ''%' + @Filtro + '%'')' ;
		END
		IF(@CAMPO ='25')
		BEGIN
			IF(@Filtro ='Acervo')
				SET @FiltroAnd = ' AND [Extent1].[flagAcervo] = 1';
			ELSE
				IF(@Filtro ='Terceiro')
					SET @FiltroAnd = ' AND [Extent1].[flagTerceiro] = 1';
					ELSE
						IF(@Filtro ='Decreto')
							SET @FiltroAnd = ' AND [Extent1].[flagDecreto] = 1';
		END;
	END;



	

		SET @SQL = @SQL + 'SELECT COUNT(*) FROM [dbo].[Asset] AS [Extent1] WITH(NOLOCK)    
		INNER JOIN [dbo].[Initial] AS [Extent3] WITH(NOLOCK) ON [Extent3].[Id] = [Extent1].[InitialId]    
		INNER JOIN [dbo].[ShortDescriptionItem] AS [Extent4] WITH(NOLOCK) ON [Extent4].[Id] = [Extent1].[ShortDescriptionItemId]    
		INNER JOIN [dbo].[AssetMovements] AS [Extent2] WITH(NOLOCK) ON[Extent1].[Id] = [Extent2].[AssetId]    
		INNER JOIN [dbo].[Institution] AS [Extent5] WITH(NOLOCK) ON [Extent5].[Id] = [Extent2].[InstitutionId]    
		INNER JOIN [dbo].[BudgetUnit] AS [Extent6] WITH(NOLOCK) ON [Extent6].[Id] = [Extent2].[BudgetUnitId]    
		INNER JOIN [dbo].[ManagerUnit] AS [Extent7] WITH(NOLOCK) ON [Extent7].[Id] = [Extent2].[ManagerUnitId]    
		INNER JOIN [dbo].[MovementType] AS [Extent14] WITH(NOLOCK) ON [Extent14].[Id] = [Extent2].[MovementTypeId]
		LEFT OUTER JOIN [dbo].[AdministrativeUnit] AS[Extent8] WITH(NOLOCK) ON [Extent8].[Id] = [Extent2].[AdministrativeUnitId]    
		LEFT OUTER JOIN [dbo].[Section] AS [Extent9] WITH(NOLOCK) ON [Extent9].[Id] = [Extent2].[SectionId]    
		LEFT OUTER JOIN [dbo].[Responsible] AS [Extent10]  WITH(NOLOCK) ON [Extent10].[Id] = [Extent2].[ResponsibleId]  
		LEFT OUTER JOIN [dbo].[AuxiliaryAccount] AS [Extent11] WITH(NOLOCK) ON [Extent11].[Id] = [Extent2].[AuxiliaryAccountId]'
		
		IF(@Estado = 3)
		BEGIN
			SET @SQL = @SQL + ' INNER JOIN [dbo].[BPsARealizaremReclassificacaoContabil] AS [Extent15] WITH(NOLOCK) ON [Extent15].[AssetId] = [Extent1].[Id] ';
		END

		SET @SQL = @SQL + 'WHERE  ([Extent1].[flagVerificado] IS NULL)        
		  AND ([Extent1].[flagDepreciaAcumulada] = 1)
		  AND ([Extent2].[flagEstorno] IS NULL)
		  AND ([Extent2].[DataEstorno] IS NULL) '
		  IF(@Estado IS NOT NULL)
		  BEGIN
			IF(@Estado = 1 OR @Estado = 3)
			BEGIN
				SET @SQL = @SQL + ' AND ([Extent2].[Status] = 1)';
			END
			ELSE IF(@Estado = 2)
			BEGIN
				SET @SQL = @SQL + ' AND ([Extent2].[Status] = 1) AND ([Extent2].[MovementTypeId] IN(11,12,47,56,57))'
			END
			ELSE
			BEGIN
				SET @SQL = @SQL + ' AND ([Extent1].[Status] = 0) AND ([Extent2].[Status] = 0) AND ([Extent2].[MovementTypeId] IN(11,12,13,14,15,16,17,18,42,43,45,46,47,48,49,50,51,52,53,55,56,57,58,59,95))';
			END
		  END
		  ELSE
		  BEGIN
			SET @SQL = @SQL + ' AND (0 = 1)';
		  END


	IF(@BuscaGeral = 1)
	BEGIN
		SET @SQL = @SQL + ' AND ( 0=1 '
	END	  	 
	IF(@NumberIdentificationRetorno IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' OR [Extent1].[NumberIdentification] LIKE ''%' + @Filtro + '%''' 
	END

	IF(@MaterialItemCode IS NOT  NULL)
	BEGIN
		SET @SQL = @SQL + ' OR [Extent1].[MaterialItemCode] LIKE''%' + CAST(@MaterialItemCode AS VARCHAR(20)) + '%''' 
	END

	IF(@RateDepreciationMonthly IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' OR [Extent1].[AssetStartId] IN(SELECT [AssetStartId] FROM [dbo].[MonthlyDepreciation] WHERE  [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode] AND [RateDepreciationMonthly] =' + CAST(@RateDepreciationMonthly AS VARCHAR(20)) + ')' 
	END

	IF(@ValueUpdate IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' OR [Extent1].[ValueUpdate] =' + CAST(@ValueUpdate AS VARCHAR(20)) + '' 
	END
	IF(@LifeCycle IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' OR [Extent1].[MaterialGroupCode] IN(SELECT [Code] FROM [dbo].[MaterialGroup] WHERE LifeCycle =' + CAST(@LifeCycle AS VARCHAR(20))+ ')' 
	END	  
	IF(@NumberDoc IS NOT NULL)
	BEGIN
		SET @SQL = @SQL +  ' OR ([Extent2].[NumberDoc] =''' + @Filtro  +''' OR ([Extent1].[NumberDoc] =''' + @Filtro  + ''' AND [Extent2].[NumberDoc] IS NULL)) OR [Extent8].Code LIKE ''%' + @Filtro +'%'''
	END;
	IF(@AssetId IS NOT NULL)
	BEGIN
		SET @SQL = @SQL +  ' OR ([Extent1].[Id] IN(SELECT * FROM [dbo].[fnSplitString](@AssetId,'','')))'
	END;
		IF(@FiltroOr IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + @FiltroOr
	END;

	IF(@BuscaGeral = 1)
	BEGIN
		SET @SQL = @SQL + ')'
	END
	
	IF(@InstitutionId IS NOT NULL)
	BEGIN
		SET @FiltroAnd =@FiltroAnd +  ' AND ([Extent2].[InstitutionId] =' + CAST(@InstitutionId AS VARCHAR(20)) + ')';
	END
	IF(@BudgetUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[BudgetUnitId] =' + CAST(@BudgetUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@ManagerUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[ManagerUnitId] =' + CAST(@ManagerUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@AdministrativeUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[AdministrativeUnitId] =' + CAST(@AdministrativeUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@CPF IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[ResponsibleId] IN (select Id from Responsible where cpf = ''' + @CPF+'''))';
	END;


	SET @SQL = @SQL + @FiltroAnd;
	
	EXEC(@SQL);
	--PRINT(@SQL)
	

GO
/****** Object:  StoredProcedure [dbo].[SAM_VISAO_GERAL_EXCEL]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------[SAM_VISAO_GERAL_EXCEL] @Ativo = 1, @Filtro='0.63',@InstitutionId = 6,@Campo ='15',@Size = 10,@PageNumber = 0
CREATE PROCEDURE [dbo].[SAM_VISAO_GERAL_EXCEL]
	@AssetId AS VARCHAR(2000) = NULL,
	@InstitutionId INT =NULL,
	@BudgetUnitId INT = NULL,
	@ManagerUnitId INT = NULL,
	@AdministrativeUnitId INT = NULL,
	@Size SMALLINT = NULL,
	@PageNumber INT = NULL,
	@Estado TINYINT = NULL,
	@Filtro VARCHAR(2000) = NULL,
	@Campo VARCHAR(50) = NULL,
	@CPF varchar(11) = NULL
AS

DECLARE @SectionId INT = NULL;
DECLARE @SectionDescription VARCHAR(200) = NULL;
DECLARE @ResposibleName VARCHAR(100) = NULL;
DECLARE @ValueAcquisition DECIMAL(18,2);
DECLARE @AccumulatedDepreciation DECIMAL(18,2);
DECLARE @RateDepreciationMonthly DECIMAL(18,2);
DECLARE @CurrentValue DECIMAL(18,2) = NULL;
DECLARE @BookAccount INT = NULL;
DECLARE @AuxiliaryAccountDescription VARCHAR(100) = NULL;
DECLARE @Empenho VARCHAR(20) = NULL;
DECLARE @NumberDoc VARCHAR(20) = NULL;
DECLARE	@NumberIdentification VARCHAR(20) = NULL;
DECLARE @MaterialItemCode INT =NULL;
DECLARE @MaterialItemDescription VARCHAR(200) = NULL;
DECLARE @ValueUpdate DECIMAL(18,2) = NULL;
DECLARE @GrupCode AS VARCHAR(120) = NULL;
DECLARE @LifeCycle INT = NULL;
DECLARE @AuxiliaryAccountId INT = NULL;
DECLARE @InitialName VARCHAR(10) = NULL;
DECLARE @FiltroAnd AS VARCHAR(8000) = '';
DECLARE @FiltroOr AS VARCHAR(2000) = '';
DECLARE @NumberIdentificationRetorno VARCHAR(20)= NULL;
DECLARE @ChapaCompleta VARCHAR(20)= NULL;
DECLARE @BuscaGeral VARCHAR(2000) =' ';


DECLARE @SQL AS VARCHAR(8000) = '';

	IF(@AssetId IS NOT NULL)
	BEGIN
		SET @SQL = 'DECLARE @AssetId AS VARCHAR(2000); SET @AssetId =''' + @AssetId + '''';
	END;
	IF(@Campo IS NOT NULL AND @Filtro IS NULL)
	BEGIN
		SET @Campo = NULL;
	END;
	IF(@Campo IS NULL)
	BEGIN
		IF(@Filtro IS NOT NULL AND ISNUMERIC(@Filtro) = 1)
		BEGIN
			SET @BuscaGeral = 1

			SET @NumberIdentificationRetorno = (SELECT TOP 1 [NumberIdentification] FROM [dbo].[Asset] WHERE [NumberIdentification] LIKE '%' + @Filtro + '%')
		
			SET @MaterialItemCode = (SELECT TOP 1 [MaterialItemCode] FROM [dbo].[Asset] WHERE [MaterialItemCode] LIKE '%' + @Filtro + '%')
		
			SET @RateDepreciationMonthly = (SELECT TOP 1 [RateDepreciationMonthly] FROM [dbo].[MonthlyDepreciation] WHERE [RateDepreciationMonthly] = CAST(@Filtro AS DECIMAL(18,2)))
			
			SET @ValueUpdate = (SELECT TOP 1 [ValueUpdate] FROM [dbo].[Asset] WHERE [ValueUpdate] = CAST(@Filtro AS DECIMAL(28,2)))
		
			SET @NumberDoc  = (SELECT TOP 1 [aset].[NumberDoc] 
							   FROM [dbo].[Asset] [aset] 
						       inner join [dbo].[AssetMovements] [asetMovement]
							   ON [aset].[Id] = [asetMovement].[AssetId]
							   WHERE [asetMovement].[NumberDoc] = @Filtro OR ([aset].[NumberDoc] = @Filtro AND [asetMovement].[NumberDoc] IS NULL));
			
			SET @LifeCycle = (SELECT TOP 1 [LifeCycle] FROM [dbo].[MaterialGroup] WHERE [LifeCycle] = CAST(@Filtro AS BIGINT))

		END;
		ELSE
		BEGIN
		
			IF(@Filtro IS NOT NULL)
			BEGIN
				SET @BuscaGeral = 1

				SET @ChapaCompleta = (SELECT TOP 1 [ChapaCompleta] FROM [dbo].[Asset] WHERE [ChapaCompleta] LIKE '%' + @Filtro + '%')
				IF(@ChapaCompleta IS NOT NULL)
	            BEGIN
	            	SET @FiltroOr = @FiltroOr + ' OR [Extent1].[ChapaCompleta] LIKE ''%' + @Filtro + '%'''
	            END

				SET @InitialName = (SELECT TOP 1 [InitialName] FROM [dbo].[Asset] WHERE [InitialName] LIKE '%' +  @Filtro + '%')
				IF(@InitialName IS NOT NULL)
				BEGIN
					SET @FiltroOr = @FiltroOr + '  [Extent1].[InitialName] LIKE ''%' + @Filtro + '%'''

				END
				
				SET @GrupCode =(SELECT TOP 1 [ma].[Code] 
								  FROM [dbo].[MaterialGroup] [ma]  WHERE [ma].[Description] LIKE '%' + @Filtro + '%');
				IF(@GrupCode IS NOT NULL)
				BEGIN
					SET @FiltroOr = @FiltroOr + ' OR [Extent1].[MaterialGroupCode] IN(SELECT [Code] FROM [dbo].[MaterialGroup] [ma] WHERE [ma].[Description] =''%' + @Filtro + '%'')';
				END;
						
				IF(UPPER(@Filtro) <> 'DECRETO' AND UPPER(@Filtro) <> 'ACERVO' AND UPPER(@Filtro) <> 'TERCEIRO')
				BEGIN
					DECLARE @Id AS INT = (SELECT TOP 1 [Id] FROM [dbo].[Asset] WHERE [MaterialItemDescription] LIKE '%' + @Filtro + '%')

					IF(@Id IS NOT NULL)
					BEGIN
						SET @FiltroOr = @FiltroOr + ' OR [Extent1].[Id] =' + CAST(@Id AS VARCHAR(20));
					END;
				END
							
				IF(UPPER(@Filtro) <> 'DECRETO' AND UPPER(@Filtro) <> 'ACERVO' AND UPPER(@Filtro) <> 'TERCEIRO')
				BEGIN
					SET @SectionDescription = (SELECT TOP 1 [sec].[Id] 
							  FROM [dbo].[Section] [sec] WHERE [sec].[Description] LIKE '%' + @Filtro + '%');
					IF(@SectionDescription IS NOT NULL)
					BEGIN
						SET @FiltroOr = @FiltroOr + ' OR [Extent9].[Description] LIKE ''%' +  @Filtro + '%''';
					END;
				END;
								
				SET @ResposibleName = (SELECT TOP 1 [res].[Id] FROM [dbo].[Responsible] [res] WHERE [res].[Name] LIKE '%' + @Filtro + '%');
				
				IF(@ResposibleName IS NOT NULL)
				BEGIN
					SET @FiltroOr = @FiltroOr + ' OR [Extent10].[Name] LIKE ''%' +  @Filtro + '%''';
				END;
									
				SET @AuxiliaryAccountDescription  = (SELECT TOP 1 [aux].[Id] FROM [dbo].[AuxiliaryAccount] [aux] WHERE [aux].[Description] LIKE '%' + @Filtro + '%');
				IF(@AuxiliaryAccountDescription IS NOT NULL)
				BEGIN
					SET @FiltroAnd = ' AND [Extent11].[Description] LIKE ''%' +  @Filtro + '%''';
				END
			END;
		END;
	END;
	ELSE
	BEGIN
		IF(@Campo ='1')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent1].[InitialName] LIKE ''%' +  @Filtro + '%'')';
		END
		IF(@Campo ='2')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent1].[ChapaCompleta] LIKE ''%' +  @Filtro + '%'')';
		END
		IF(@Campo ='3')
		BEGIN
			IF(ISNUMERIC(@Filtro) = 1)
			BEGIN
				SET @FiltroAnd = ' AND ([Extent1].[MaterialGroupCode] =' +  @Filtro + ')';
			END
			ELSE
			BEGIN
				SET @FiltroAnd = ' AND 1 = 0';
			END
		END
		IF(@Campo ='4')
		BEGIN
			IF(ISNUMERIC(@Filtro) = 1)
			BEGIN
				SET @FiltroAnd = ' AND ([Extent1].[MaterialItemCode] =' +  @Filtro + ')';
			END
			ELSE
			BEGIN
				SET @FiltroAnd = ' AND 1 = 0';
			END
		END
		IF(@Campo ='5')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent1].[MaterialItemDescription] LIKE ''%' +  @Filtro + '%'')';
		END
		IF(@Campo ='6')
		BEGIN
			SET @InstitutionId = (SELECT TOP 1 [orgao].[Id] FROM [dbo].[Institution] [orgao] WHERE [orgao].[Code] = '' + @Filtro + '' OR [orgao].[Description] LIKE '%' + @Filtro + '%')
			IF(@InstitutionId IS NOT NULL)
				SET @FiltroAnd = ' AND ([Extent2].[InstitutionId] =' + CAST(@InstitutionId AS VARCHAR(20)) + ')';

		END
		IF(@Campo ='7')
		BEGIN
			SET @BudgetUnitId = (SELECT TOP 1 [uo].[Id] FROM [dbo].[BudgetUnit] [uo] WHERE [uo].[Code] = '' + @Filtro + '' OR [uo].[Description] LIKE '%' + @Filtro + '%');

			IF(@BudgetUnitId IS NOT NULL)
				SET @FiltroAnd = ' AND ([Extent2].[BudgetUnitId] =' + CAST(@BudgetUnitId AS VARCHAR(20)) + ')';
		END
		IF(@Campo ='8')
		BEGIN
			SET @ManagerUnitId = (SELECT TOP 1 [uge].[Id] FROM [dbo].[ManagerUnit] [uge] WHERE [uge].[Code] = '' + @Filtro + '' OR [uge].[Description] LIKE '%' + @Filtro + '%');
			IF(@ManagerUnitId IS NOT NULL)
				SET @FiltroAnd = ' AND ([Extent2].[ManagerUnitId] =' + CAST(@ManagerUnitId AS VARCHAR(20)) + ')';
		END
		IF(@Campo ='9')
		BEGIN
			IF(ISNUMERIC(@Filtro) = 1)
			BEGIN
				SET @AdministrativeUnitId = (SELECT TOP 1 [ua].[Id] FROM [dbo].[AdministrativeUnit] [ua] WHERE [ua].[Code] =  CAST(@Filtro AS BIGINT));
				IF(@AdministrativeUnitId IS NOT NULL)
					SET @FiltroAnd = ' AND ([Extent2].[AdministrativeUnitId] =' + CAST(@AdministrativeUnitId AS VARCHAR(20)) + ')';
			END
			ELSE
			BEGIN
				SET @FiltroAnd = ' AND 1 = 0';
			END
		END
		IF(@Campo ='10')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent2].[SectionId] IN(SELECT [id] FROM [dbo].[Section] WHERE [Description] LIKE ''%' + @Filtro + '%''))'
		END
		IF(@Campo ='11')
		BEGIN
			SET @FiltroAnd = ' AND ([Extent2].[ResponsibleId] IN(SELECT [id] FROM [dbo].[Responsible] WHERE [Name] LIKE ''%' + @Filtro + '%''))'
		END
		IF(@Campo ='12')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[ValueAcquisition] =CAST(' + REPLACE(@Filtro,',','.') + ' AS DECIMAL(18,2)))' 
		END
		IF(@Campo ='13')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[AssetStartId] IN(SELECT [AssetStartId] FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode]' + 
							 +' AND Convert(int,Convert(varchar(6),[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])' + 
							 +' AND [AccumulatedDepreciation] LIKE ''%' + REPLACE(@Filtro,',','.') + '%''))'
		END
		IF(@Campo ='14')
		BEGIN
			SET @FiltroAnd = 'AND CAST(''' + REPLACE(@Filtro,',','.') + ''' AS DECIMAL(18,2)) = CASE '+
						     +' WHEN [Extent1].[flagAcervo] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition]) '+
							 +' WHEN [Extent1].[flagTerceiro] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition]) '+
							 +' ELSE ISNULL((SELECT top 1 CurrentValue FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode] AND Convert(int,Convert(varchar(6),[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId]) AND [AssetStartId] = [Extent1].[AssetStartId]), ' +
						     + 'CASE WHEN [Extent1].[flagDecreto] = 1 THEN [Extent1].[ValueUpdate] ELSE [Extent1].[ValueAcquisition] END) END '
		END
		IF(@Campo ='15')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[AssetStartId] IN(SELECT [AssetStartId] FROM [dbo].[MonthlyDepreciation] WHERE [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode]' +
							 +' AND Convert(int,Convert(varchar(6),[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])' + 
							 +' AND [RateDepreciationMonthly] =' + REPLACE(@Filtro,',','.')  +'))'
		END
		IF(@Campo ='16')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[MaterialGroupCode] IN(SELECT [Code] FROM [dbo].[MaterialGroup] WHERE [Description] =''%' + @Filtro  +'%''))';
		END

		IF(@Campo ='17')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent11].[ContaContabilApresentacao] LIKE ''%' + @Filtro + '%'')';
			
		END
		IF(@Campo ='18')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent11].[Description] LIKE ''%' + @Filtro  +'%'')';

		END
		IF(@Campo ='19')
		BEGIN
			SET @FiltroAnd = 'AND (CONVERT(varchar,[Extent1].[AcquisitionDate],103) LIKE ''%' + @Filtro + '%'')' ;
		END
		IF(@Campo ='20')
		BEGIN
			SET @FiltroAnd = 'AND (CONVERT(varchar,[Extent1].[MovimentDate],103) LIKE ''%' + @Filtro + '%'')' ;
		END
		IF(@Campo ='21')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent1].[Empenho] LIKE ''%' + @Filtro  +'%'')';
		END
		IF(@Campo ='22')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent2].[NumberDoc] =''' + @Filtro  +''' OR ([Extent1].[NumberDoc] =''' + @Filtro  +''' AND [Extent2].[NumberDoc] IS NULL))';
		END
		IF(@Campo ='23')
		BEGIN
			SET @FiltroAnd = 'AND ([Extent14].[Description] LIKE ''%' + @Filtro  +'%'')';
		END
		IF(@Campo ='24')
		BEGIN
			SET @FiltroAnd = 'AND (CONVERT(varchar,[Extent2].[MovimentDate],103) LIKE ''%' + @Filtro + '%'')' ;
		END
		IF(@CAMPO ='25')
		BEGIN
			IF(@Filtro ='Acervo')
				SET @FiltroAnd = ' AND [Extent1].[flagAcervo] = 1';
			ELSE
				IF(@Filtro ='Terceiro')
					SET @FiltroAnd = ' AND [Extent1].[flagTerceiro] = 1';
					ELSE
						IF(@Filtro ='Decreto')
							SET @FiltroAnd = ' AND [Extent1].[flagDecreto] = 1';
		END;
	END;

	

		SET @SQL = @SQL + ' SELECT '


		SET @SQL = @SQL + ' [Extent3].[Name] ''Sigla''
		,[Extent1].[ChapaCompleta] AS ''Chapa'',  
		CAST([Extent1].[MaterialGroupCode] AS VARCHAR(20)) ''GrupoItem'',
		[Extent1].[MaterialItemCode] AS ''Item'',  
		[Extent4].[Description] AS ''DescricaoDoItem'', 
		[Extent5].[NameManagerReduced] AS ''Orgao'', 
		[Extent6].[Code] AS ''UO'', 
		[Extent7].[Code] AS ''UGE'', 
		[Extent8].[Code] AS ''UA'', 
		[Extent9].[Description] AS ''DescricaoDaDivisao'', 
		[Extent10].[Name] AS ''Responsavel'', 
		[Extent11].[ContaContabilApresentacao] AS ''ContaContabil'',
		[Extent1].[ValueAcquisition] ''ValorDeAquisicao'',
		CASE
			WHEN [Extent1].[flagAcervo] = 1 THEN 0
			WHEN [Extent1].[flagTerceiro] = 1 THEN 0
			ELSE
		(SELECT MAX([Extent12].[AccumulatedDepreciation])
		  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
		  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
		    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
		    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
			AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
		) END AS ''DepreciacaoAcumulada'',
		CASE
			WHEN [Extent1].[flagAcervo] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition])
			WHEN [Extent1].[flagTerceiro] = 1 THEN ISNULL([Extent1].[ValueUpdate],[Extent1].[ValueAcquisition])
			WHEN [Extent1].[flagDecreto] = 1 
		    THEN 
				ISNULL((SELECT MIN([Extent12].[CurrentValue])
			  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
			  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
			    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
			    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
				AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
			), [Extent1].[ValueUpdate]) 
			ELSE
			ISNULL((SELECT MIN([Extent12].[CurrentValue])
			  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
			  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
			    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
			    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
				AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
		), [Extent1].[ValueAcquisition]) 
		END AS ''ValorAtual'',
		CASE
			WHEN [Extent1].[flagAcervo] = 1 THEN 0
			WHEN [Extent1].[flagTerceiro] = 1 THEN 0
			ELSE
		(SELECT TOP 1 [Extent12].[RateDepreciationMonthly]
		  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
		  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
		    AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
		    AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
			AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId])
		  ORDER BY [Extent12].[Id] DESC
		) END AS ''DepreciacaoMensal'',
		(
			SELECT (SELECT CAST(ISNULL(MAX([Extent12].[CurrentMonth]),0) AS VARCHAR(10))
																		  FROM [dbo].[MonthlyDepreciation] [Extent12] WITH(NOLOCK) 
																		  WHERE [Extent12].[ManagerUnitId] = [Extent1].[ManagerUnitId] 
																			AND [Extent12].[AssetStartId] = [Extent1].[AssetStartId] 
																			AND [Extent12].[MaterialItemCode] = [Extent1].[MaterialItemCode] 
																			AND Convert(int,Convert(varchar(6),[Extent12].[CurrentDate],112)) <= (select CONVERT(int,ManagmentUnit_YearMonthReference) from ManagerUnit where id = [Extent1].[ManagerUnitId]))
																			 + ''/'' + 
			(SELECT CAST(ISNULL(MAX([Extent13].[LifeCycle]),0) AS VARCHAR(10))  
				FROM [dbo].[MaterialGroup] [Extent13] WITH(NOLOCK) 
            WHERE [Extent13].[Code] = [Extent1].[MaterialGroupCode]) 
																		 
		) AS ''VidaUtil'',
		CONVERT(varchar,[Extent1].[AcquisitionDate],103) AS ''DataAquisicao'',
		CONVERT(varchar,[Extent1].[MovimentDate],103) AS ''DataIncorporacao'',
		[Extent1].[Empenho],
		ISNULL([Extent2].[NumberDoc], [Extent1].[NumberDoc]) AS ''NumeroDocumento'',
		[Extent14].[Description] AS ''UltimoHistorico'',
		CONVERT(varchar,[Extent2].[MovimentDate],103) AS ''DataUltimoHistorico'',
		(CASE WHEN [Extent1].[flagAcervo] = 1 THEN ''Acervo'' 
		      ELSE 
			     CASE WHEN [Extent1].[flagTerceiro] = 1 THEN ''Terceiro'' 
			          ELSE  CASE WHEN [Extent1].[flagDecreto] = 1 THEN ''Decreto'' ELSE '''' END
			      END
		  END) AS ''TipoBp'''
	
		SET @SQL = @SQL + ' FROM [dbo].[Asset] AS [Extent1] WITH(NOLOCK)    
		INNER JOIN [dbo].[Initial] AS [Extent3] WITH(NOLOCK) ON [Extent3].[Id] = [Extent1].[InitialId]    
		INNER JOIN [dbo].[ShortDescriptionItem] AS [Extent4] WITH(NOLOCK) ON [Extent4].[Id] = [Extent1].[ShortDescriptionItemId]    
		INNER JOIN [dbo].[AssetMovements] AS [Extent2] WITH(NOLOCK) ON[Extent1].[Id] = [Extent2].[AssetId]    
		INNER JOIN [dbo].[Institution] AS [Extent5] WITH(NOLOCK) ON [Extent5].[Id] = [Extent2].[InstitutionId]    
		INNER JOIN [dbo].[BudgetUnit] AS [Extent6] WITH(NOLOCK) ON [Extent6].[Id] = [Extent2].[BudgetUnitId]    
		INNER JOIN [dbo].[ManagerUnit] AS [Extent7] WITH(NOLOCK) ON [Extent7].[Id] = [Extent2].[ManagerUnitId]    
		INNER JOIN [dbo].[MovementType] AS [Extent14] WITH(NOLOCK) ON [Extent14].[Id] = [Extent2].[MovementTypeId]
		LEFT OUTER JOIN [dbo].[AdministrativeUnit] AS[Extent8] WITH(NOLOCK) ON [Extent8].[Id] = [Extent2].[AdministrativeUnitId]    
		LEFT OUTER JOIN [dbo].[Section] AS [Extent9] WITH(NOLOCK) ON [Extent9].[Id] = [Extent2].[SectionId]    
		LEFT OUTER JOIN [dbo].[Responsible] AS [Extent10]  WITH(NOLOCK) ON [Extent10].[Id] = [Extent2].[ResponsibleId]  
		LEFT OUTER JOIN [dbo].[AuxiliaryAccount] AS [Extent11] WITH(NOLOCK) ON [Extent11].[Id] = [Extent2].[AuxiliaryAccountId] '
		
		IF(@Estado = 3)
		BEGIN
			SET @SQL = @SQL + ' INNER JOIN [dbo].[BPsARealizaremReclassificacaoContabil] AS [Extent15] WITH(NOLOCK) ON [Extent15].[AssetId] = [Extent1].[Id]';
		END

		SET @SQL = @SQL + 'WHERE  ([Extent1].[flagVerificado] IS NULL)        
		  AND ([Extent1].[flagDepreciaAcumulada] = 1)
		  AND ([Extent2].[flagEstorno] IS NULL)
		  AND ([Extent2].[DataEstorno] IS NULL) '
		IF(@Estado IS NOT NULL)
		BEGIN
			IF(@Estado = 1 OR @Estado = 3)
			BEGIN
				SET @SQL = @SQL + ' AND ([Extent2].[Status] = 1)';
			END
			ELSE IF(@Estado = 2)
			BEGIN
				SET @SQL = @SQL + ' AND ([Extent2].[Status] = 1) AND ([Extent2].[MovementTypeId] IN(11,12,47,56,57))'
			END
			ELSE
			BEGIN
				SET @SQL = @SQL + ' AND ([Extent1].[Status] = 0) AND ([Extent2].[Status] = 0) AND ([Extent2].[MovementTypeId] IN(11,12,13,14,15,16,17,18,42,43,45,46,47,48,49,50,51,52,53,55,56,57,58,59,95))';
			END
		END
		ELSE
		BEGIN
		SET @SQL = @SQL + ' AND (0 = 1)';
		END
	
	IF(@BuscaGeral = 1)
	BEGIN
		SET @SQL = @SQL + ' AND ( 0=1 '
	END	
	IF(@NumberIdentificationRetorno IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'OR [Extent1].[NumberIdentification]  LIKE''%' + @Filtro + '%''' 
	END
	IF(@MaterialItemCode IS NOT  NULL)
	BEGIN
		SET @SQL = @SQL + ' OR [Extent1].[MaterialItemCode] =' + CAST(@MaterialItemCode AS VARCHAR(20)) + '' 
	END
	IF(@RateDepreciationMonthly IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'OR [Extent1].[AssetStartId] IN(SELECT [AssetStartId] FROM [dbo].[MonthlyDepreciation] WHERE  [ManagerUnitId] = [Extent1].[ManagerUnitId] AND [MaterialItemCode] = [Extent1].[MaterialItemCode] AND [RateDepreciationMonthly] =' + CAST(@RateDepreciationMonthly AS VARCHAR(20)) + ')' 
	END
	IF(@ValueUpdate IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'OR [Extent1].[ValueUpdate] =' + CAST(@ValueUpdate AS VARCHAR(20)) + '' 
	END
	IF(@LifeCycle IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'OR [Extent1].[MaterialGroupCode] IN(SELECT [Code] FROM [dbo].[MaterialGroup] WHERE LifeCycle =' + CAST(@LifeCycle AS VARCHAR(20))+ ')' 
	END	  
	
	IF(@AssetId IS NOT NULL)
	BEGIN
		SET @SQL = @SQL +  ' AND ([Extent1].[Id] IN(SELECT * FROM [dbo].[fnSplitString](@AssetId,'','')))'
	END;

	IF(@NumberDoc IS NOT NULL)
	BEGIN
		SET @SQL = @SQL +  ' OR ([Extent2].[NumberDoc] =''' + @Filtro  +''' OR ([Extent1].[NumberDoc] =''' + @Filtro  +''' AND [Extent2].[NumberDoc] IS NULL))OR [Extent8].Code LIKE ''%' + @Filtro +'%'''
	END;
		IF(@FiltroOr IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + @FiltroOr
	END;
	IF(@BuscaGeral = 1)
	BEGIN
		SET @SQL = @SQL + ')'
	END	
	IF(@InstitutionId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + ' AND ([Extent2].[InstitutionId] =' + CAST(@InstitutionId AS VARCHAR(20)) + ')';
	END
	IF(@BudgetUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[BudgetUnitId] =' + CAST(@BudgetUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@ManagerUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[ManagerUnitId] =' + CAST(@ManagerUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@AdministrativeUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[AdministrativeUnitId] =' + CAST(@AdministrativeUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@CPF IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[ResponsibleId] IN (select Id from Responsible where cpf = ''' + @CPF+'''))';
	END;
	SET @SQL = @SQL + @FiltroAnd;
	
	EXEC(@SQL);
	PRINT(@SQL)
	--PRINT(@FiltroAnd)
	--PRINT(@FiltroOr)

GO
/****** Object:  StoredProcedure [dbo].[SAM_VISAO_GERAL_NOTIFICATION]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

------EXEC [SAM_VISAO_GERAL_NOTIFICATION] @InstitutionId =43,@BudgetUnitId =217,@ManagerUnitId =1578
CREATE PROCEDURE [dbo].[SAM_VISAO_GERAL_NOTIFICATION]
	@InstitutionId INT =NULL,
	@BudgetUnitId INT = NULL,
	@ManagerUnitId INT = NULL
AS

DECLARE @FiltroAnd AS VARCHAR(150) = '';
DECLARE @SQL AS VARCHAR(2000) = '';	
DECLARE @SQLAuxiliar AS VARCHAR(500) = '';

DECLARE @QtdBolsaSecretaria int;

	SET @SQL = 'DECLARE @Retorno table(qts int);'
	
	SET @SQLAuxiliar = ' count(distinct [Extent1].[Id])
						FROM [dbo].[Asset] AS [Extent1] WITH(NOLOCK)       
						INNER JOIN [dbo].[AssetMovements] AS [Extent2] WITH(NOLOCK) ON [Extent1].[Id] = [Extent2].[AssetId] 	
						INNER JOIN [dbo].[Exchange] AS [Extent3] WITH (NOLOCK) ON [Extent2].[AssetId] = [Extent3].[AssetId]
						WHERE  ([Extent1].[flagVerificado] IS NULL)        
						AND ([Extent1].[flagDepreciaAcumulada] = 1) 
						AND [Extent2].[Status] = 1
						AND [Extent3].[Status] = 1 '

	IF(@InstitutionId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + ' AND ([Extent2].[InstitutionId] =' + CAST(@InstitutionId AS VARCHAR(20)) + ')';
	END
	IF(@BudgetUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[BudgetUnitId] =' + CAST(@BudgetUnitId  AS VARCHAR(20)) + ')';
	END;
	IF(@ManagerUnitId IS NOT NULL)
	BEGIN
		SET @FiltroAnd = @FiltroAnd + '  AND ([Extent2].[ManagerUnitId] =' + CAST(@ManagerUnitId  AS VARCHAR(20)) + ')';
	END;
	
	SET @SQL = @SQL + 'INSERT INTO @Retorno SELECT ' + @SQLAuxiliar + ' AND [Extent2].[MovementTypeId] = 20 '+ @FiltroAnd + ';';
	
	SET @SQL = @SQL + 'INSERT INTO @Retorno SELECT ' + @SQLAuxiliar + ' AND [Extent2].[MovementTypeId] = 21 '+ @FiltroAnd + ';';
	
	SET @SQL = @SQL + 'INSERT INTO @Retorno SELECT count(distinct AssetId) from [dbo].[AssetMovements] WITH(NOLOCK) where (([Status] = 1 and [MovementTypeId] IN (11,12,47,56,57)) OR ([Status] = 0 and ([FlagEstorno] IS NULL OR [FlagEstorno] = 0) and AssetTransferenciaId IS NULL and [MovementTypeId] IN (42,43))) and [SourceDestiny_ManagerUnitId] = ' + CAST(@ManagerUnitId  AS VARCHAR(20)) + ';select * from @Retorno;';
	
	EXEC(@SQL);

	--PRINT(@SQL)
	--PRINT(@FiltroAnd)

GO
/****** Object:  StoredProcedure [dbo].[TRANSFERE_RESPONSAVEL_DIVISAO]    Script Date: 26/04/2021 23:33:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[TRANSFERE_RESPONSAVEL_DIVISAO] 
(
	@AdministrativeUnitId int,
	@SectionId int,
	@DataParaMovimento date,
	@ResponsibleId int,
	@Login varchar(11)
)
AS    
BEGIN 

BEGIN TRANSACTION
BEGIN TRY


SELECT am.* INTO #temp
FROM 
	AssetMovements as am
inner join Asset as a on am.AssetId = a.Id 
where 
	a.flagVerificado is null and
	a.flagDepreciaAcumulada = 1 and
	am.[Status] = 1 and
	a.[Status] = 1 and
	am.AdministrativeUnitId = @AdministrativeUnitId and
	am.SectionId = @SectionId


UPDATE 
	am
SET
	am.[Status] = 0
from 
	AssetMovements as am
inner join Asset as a on am.AssetId = a.Id 
where 
	a.flagVerificado is null and
	a.flagDepreciaAcumulada = 1 and
	am.Status = 1 and
	a.Status = 1 and
	am.AdministrativeUnitId = @AdministrativeUnitId and
	am.SectionId = @SectionId


INSERT INTO AssetMovements(
							AssetId, 
							[Status],
							MovimentDate,
							MovementTypeId,
							StateConservationId,
							NumberPurchaseProcess,
							InstitutionId,
							BudgetUnitId,
							ManagerUnitId,
							AdministrativeUnitId,
							SectionId,
							AuxiliaryAccountId,
							ResponsibleId,
							[Login],
							DataLogin,
							NumberDoc
						 )
SELECT					AssetId,
						1,
						@DataParaMovimento,
						10,
						StateConservationId,
						NumberPurchaseProcess,
						InstitutionId,
						BudgetUnitId,
						ManagerUnitId,
						AdministrativeUnitId,
						SectionId,
						AuxiliaryAccountId,
						@ResponsibleId,
						@Login,
						GETDATE(),
						NumberDoc						
FROM #temp


COMMIT TRANSACTION 
END TRY                    
                    
BEGIN CATCH      
 SELECT        
         ERROR_PROCEDURE() AS ErrorProcedure        
        ,ERROR_LINE() AS ErrorLine        
        ,ERROR_MESSAGE() AS ErrorMessage;       
 ROLLBACK TRANSACTION      
END CATCH  
                            
END
GO
