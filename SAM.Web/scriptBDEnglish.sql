USE [master]
GO
/****** Object:  Database [SAM]    Script Date: 12/08/2015 17:36:40 ******/
CREATE DATABASE [SAM] ON  PRIMARY 
( NAME = N'SAM', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS2008R2\MSSQL\DATA\SAM.mdf' , SIZE = 25856KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'SAM_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS2008R2\MSSQL\DATA\SAM_log.LDF' , SIZE = 152384KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [SAM] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SAM].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SAM] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SAM] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SAM] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SAM] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SAM] SET ARITHABORT OFF 
GO
ALTER DATABASE [SAM] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SAM] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SAM] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SAM] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SAM] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SAM] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SAM] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SAM] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SAM] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SAM] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SAM] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SAM] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SAM] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SAM] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SAM] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SAM] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SAM] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SAM] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SAM] SET  MULTI_USER 
GO
ALTER DATABASE [SAM] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SAM] SET DB_CHAINING OFF 
GO
USE [SAM]
GO
/****** Object:  User [samw_user]    Script Date: 12/08/2015 17:36:40 ******/
CREATE USER [samw_user] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [samw_user]
GO
GRANT CONNECT TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[AdditionalAssetsOut]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[AdditionalAssetsOut](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MeetingDate] [datetime] NULL,
	[PresidentId] [int] NULL,
	[FirstMemberId] [int] NULL,
	[SecondMemberId] [int] NULL,
	[ThirdMemberId] [int] NULL,
	[FourthMemberID] [int] NULL,
	[DirectorID] [int] NULL,
	[SectionChiefId] [int] NULL,
	[HistoricId] [int] NULL,
	[RecordDate] [varchar](8) NULL,
	[DocumentRecordNumber] [varchar](15) NULL,
	[RecordDirectorId] [int] NULL,
	[FirstWitnessId] [int] NULL,
	[SecondWitnessId] [int] NULL,
	[ThirdWitnessId] [int] NULL,
	[FourthWitnessId] [int] NULL,
	[AddresseeId] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[AdditionalAssetsOut] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[AdditionalAssetsOut] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[AdditionalAssetsOut] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[AdditionalAssetsOut] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Address]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
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
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Address] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Address] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Address] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Address] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Addressee]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Addressee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](60) NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_Addressee_Status]  DEFAULT ((1))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Addressee] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Addressee] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Addressee] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Addressee] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[AdministrativeUnit]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AdministrativeUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManagerUnitId] [int] NOT NULL,
	[ResponsibleId] [int] NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[RelationshipAdministrativeUnitId] [int] NULL,
	[Status] [bit] NOT NULL,
	[CostCenterId] [int] NOT NULL,
 CONSTRAINT [PK_AdministrativeUnit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[AdministrativeUnit] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[AdministrativeUnit] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[AdministrativeUnit] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[AdministrativeUnit] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Asset]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Asset](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InitialId] [int] NOT NULL,
	[NumberIdentification] [varchar](6) NOT NULL,
	[PartNumberIdentification] [varchar](2) NULL,
	[InstitutionId] [int] NOT NULL,
	[AdministrativeUnitId] [int] NOT NULL,
	[ManagerID] [int] NOT NULL,
	[BudgetUnitId] [int] NOT NULL,
	[ManagementUnitId] [int] NOT NULL,
	[SectionId] [int] NOT NULL,
	[ResponsibleId] [int] NOT NULL,
	[MaterialItemId] [int] NOT NULL,
	[AuxiliaryAccountId] [int] NULL,
	[OutSourcedId] [int] NULL,
	[TypeIncorporationId] [int] NOT NULL,
	[StateConservationId] [int] NOT NULL,
	[MovementTypeId] [int] NOT NULL,
	[SupplierId] [int] NULL,
	[InclusionDate] [datetime] NULL,
	[AcquisitionDate] [datetime] NULL,
	[ValueAcquisition] [money] NULL,
	[ValueUpdate] [money] NULL,
	[NumberPurchaseProcess] [varchar](25) NULL,
	[NGPB] [varchar](15) NULL,
	[Invoice] [varchar](15) NULL,
	[SerialNumber] [varchar](50) NULL,
	[Manufacture] [varchar](15) NULL,
	[DateGuarantee] [datetime] NULL,
	[ChassiNumber] [varchar](20) NULL,
	[Brand] [varchar](20) NULL,
	[Model] [varchar](20) NULL,
	[NumberPlate] [varchar](8) NULL,
	[AdditionalDescription] [varchar](20) NULL,
	[HistoricId] [int] NULL,
	[Condition] [bit] NOT NULL,
	[Rating] [bit] NOT NULL,
	[BagDate] [datetime] NULL,
	[OldInitial] [int] NULL,
	[OldNumberIdentification] [varchar](6) NULL,
	[OldPartNumberIdentification] [varchar](2) NULL,
	[ReceiptTermDate] [datetime] NULL,
	[InventoryDate] [datetime] NULL,
	[NoteGranting] [varchar](11) NULL,
	[AssigningAmountDate] [datetime] NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Asset_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Asset] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Asset] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Asset] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Asset] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[AssetOutReason]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AssetOutReason](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](60) NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_AssetsOutReason_Status]  DEFAULT ((1))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[AssetOutReason] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[AssetOutReason] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[AssetOutReason] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[AssetOutReason] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[AssetsOut]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AssetsOut](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssetId] [int] NOT NULL,
	[AssetsOutListingId] [int] NULL,
	[AssetsOutComplementId] [int] NULL,
	[InitialId] [int] NOT NULL,
	[Numberidentification] [varchar](6) NOT NULL,
	[PartNumberIdentification] [varchar](2) NOT NULL,
	[DateOut] [datetime] NULL,
	[TypeOutId] [int] NOT NULL,
	[ProcessNumber] [varchar](25) NULL,
	[TypeDocumentOutId] [int] NULL,
	[DocumentNumberOut] [varchar](30) NULL,
	[NGPB] [varchar](30) NULL,
	[OldInitial] [varchar](10) NULL,
	[OldNumberIdentification] [varchar](6) NULL,
	[OldPartNumberIdentification] [varchar](2) NULL,
	[ValueOut] [decimal](18, 0) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[AssetsOut] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[AssetsOut] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[AssetsOut] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[AssetsOut] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[AssetsOutListing]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[AssetsOutListing](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateAssetsOutListing] [datetime] NULL,
	[NumberDocumentAssetsOutListing] [varchar](15) NULL,
	[AssetsOutListingIronMaterial] [bit] NULL,
	[UseConditions] [bit] NULL,
	[WeightIronMaterial] [bit] NULL,
	[PesoMaterialFerroArrolamento] [decimal](18, 0) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[AssetsOutListing] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[AssetsOutListing] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[AssetsOutListing] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[AssetsOutListing] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[AuxiliaryAccount]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AuxiliaryAccount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManagerId] [int] NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](60) NOT NULL,
	[BookAccount] [int] NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_AuxiliaryAccount_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_ContaAuxiliar] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[AuxiliaryAccount] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[AuxiliaryAccount] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[AuxiliaryAccount] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[AuxiliaryAccount] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[BudgetUnit]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BudgetUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManagerId] [int] NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_BudgetUnit_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_UO] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[BudgetUnit] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[BudgetUnit] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[BudgetUnit] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[BudgetUnit] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Closing]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Closing](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManagmentUnitId] [int] NOT NULL,
	[MaterialItemId] [int] NOT NULL,
	[MaterialSubItemId] [int] NULL,
	[PriceMovementIn] [decimal](28, 10) NULL,
	[PriceMovementOut] [decimal](28, 10) NULL,
	[BalanceAmount] [decimal](18, 4) NULL,
	[BalancePrice] [decimal](28, 10) NULL,
	[AmountMovementIn] [decimal](18, 4) NULL,
	[AmountMovementOut] [decimal](18, 4) NULL,
 CONSTRAINT [PK_Fechamento] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
GRANT DELETE ON [dbo].[Closing] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Closing] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Closing] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Closing] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Configuration]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuration](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[ManagerId] [int] NOT NULL,
	[BudgetUnitId] [int] NOT NULL,
	[ManagmentUnitId] [int] NOT NULL,
	[InitialYearMonthAsset] [date] NULL,
	[ReferenceYearMonthAsset] [date] NULL,
 CONSTRAINT [PK_Configuracoes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO
GRANT DELETE ON [dbo].[Configuration] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Configuration] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Configuration] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Configuration] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[CostCenter]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[CostCenter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](22) NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[ManagerId] [int] NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_CostCenter_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_CentroCusto] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[CostCenter] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[CostCenter] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[CostCenter] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[CostCenter] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[ELMAH_Error]    Script Date: 12/08/2015 17:36:40 ******/
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
	[AllXml] [ntext] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
GRANT DELETE ON [dbo].[ELMAH_Error] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[ELMAH_Error] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[ELMAH_Error] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[ELMAH_Error] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Historic]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
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
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Historic] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Historic] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Historic] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Historic] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[IncorporationType]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[IncorporationType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](60) NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_IncorporationType_Status]  DEFAULT ((1))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[IncorporationType] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[IncorporationType] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[IncorporationType] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[IncorporationType] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Initial]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Initial](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](10) NOT NULL,
	[Description] [varchar](60) NULL,
	[BarCode] [varchar](2) NULL,
	[ManagerId] [int] NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_Initial_Status]  DEFAULT ((1))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Initial] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Initial] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Initial] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Initial] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Institution]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Institution](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_Institution_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_Orgao] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Institution] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Institution] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Institution] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Institution] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Level]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [dbo].[Level](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NULL DEFAULT (NULL),
	[Description] [varchar](30) NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_Level_Status]  DEFAULT ((1)),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Level] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Level] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Level] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Level] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[ManagedSystem]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ManagedSystem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Status] [bit] NOT NULL,
	[Name] [varchar](20) NOT NULL,
	[Description] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[ManagedSystem] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[ManagedSystem] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[ManagedSystem] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[ManagedSystem] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Manager]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Manager](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[Name] [varchar](120) NOT NULL,
	[ShortName] [varchar](25) NOT NULL,
	[AddressId] [int] NOT NULL,
	[Telephone] [varchar](20) NULL,
	[Code] [int] NOT NULL,
	[Image] [image] NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_Manager_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_Gestor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Manager] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Manager] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Manager] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Manager] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[ManagerUnit]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ManagerUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BudgetUnitId] [int] NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_ManagerUnit_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_UGE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[ManagerUnit] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[ManagerUnit] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[ManagerUnit] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[ManagerUnit] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Material]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Material] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Material] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Material] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Material] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[MaterialClass]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[MaterialClass] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[MaterialClass] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[MaterialClass] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[MaterialClass] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[MaterialGroup]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MaterialGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](120) NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[MaterialGroup] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[MaterialGroup] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[MaterialGroup] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[MaterialGroup] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[MaterialItem]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[MaterialItem] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[MaterialItem] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[MaterialItem] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[MaterialItem] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[MaterialSubItem]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[MaterialSubItem] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[MaterialSubItem] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[MaterialSubItem] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[MaterialSubItem] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Module]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
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
	[Sequence] [int] NULL CONSTRAINT [DF_Module_Sequence]  DEFAULT ((0)),
 CONSTRAINT [PK__Modulo__3214EC0703317E3D] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Module] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Module] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Module] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Module] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[MovementType]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MovementType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_MovementType_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_TipoMovimento] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[MovementType] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[MovementType] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[MovementType] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[MovementType] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[OutSourced]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OutSourced](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](60) NULL,
	[CPFCNPJ] [varchar](14) NULL,
	[AddressId] [int] NULL,
	[Telephone] [varchar](11) NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_OutSourced_Status]  DEFAULT ((1)),
 CONSTRAINT [PK__Terceiro__3214EC072CF2ADDF] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[OutSourced] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[OutSourced] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[OutSourced] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[OutSourced] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Profile]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Profile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Status] [bit] NOT NULL,
	[Description] [varchar](100) NOT NULL,
 CONSTRAINT [PK__Perfil__3214EC0729572725] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Profile] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Profile] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Profile] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Profile] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[RelationshipAdministrativeUnitSection]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipAdministrativeUnitSection](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RelationshipManagerUnitAdministrativeUnit] [int] NOT NULL,
	[SectionId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_RelationshipAdministrativeUnitSection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
GRANT DELETE ON [dbo].[RelationshipAdministrativeUnitSection] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[RelationshipAdministrativeUnitSection] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[RelationshipAdministrativeUnitSection] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[RelationshipAdministrativeUnitSection] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[RelationshipBudgetUnitManagerUnit]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipBudgetUnitManagerUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RelationshipManagerBudgetUnitId] [int] NOT NULL,
	[ManagerUnitId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_RelationshipBudgetUnitManagerUnit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
GRANT DELETE ON [dbo].[RelationshipBudgetUnitManagerUnit] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[RelationshipBudgetUnitManagerUnit] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[RelationshipBudgetUnitManagerUnit] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[RelationshipBudgetUnitManagerUnit] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[RelationshipInstitutionManager]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipInstitutionManager](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RelationshipUserProfileInstitutionId] [int] NOT NULL,
	[ManagerId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_RelationshipInstitutionManager] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
GRANT DELETE ON [dbo].[RelationshipInstitutionManager] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[RelationshipInstitutionManager] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[RelationshipInstitutionManager] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[RelationshipInstitutionManager] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[RelationshipManagerBudgetUnit]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipManagerBudgetUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RelationshipInstitutionManagerId] [int] NOT NULL,
	[BudgetUnitId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_RelationshipManagerBudgetUnit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
GRANT DELETE ON [dbo].[RelationshipManagerBudgetUnit] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[RelationshipManagerBudgetUnit] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[RelationshipManagerBudgetUnit] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[RelationshipManagerBudgetUnit] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[RelationshipManagerUnitAdministrativeUnit]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipManagerUnitAdministrativeUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RelationshipBudgetUnitManagerUnitId] [int] NOT NULL,
	[AdministrativeUnitId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_RelationshipManagerUnitAdministrativeUnit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
GRANT DELETE ON [dbo].[RelationshipManagerUnitAdministrativeUnit] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[RelationshipManagerUnitAdministrativeUnit] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[RelationshipManagerUnitAdministrativeUnit] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[RelationshipManagerUnitAdministrativeUnit] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[RelationshipProfileLevel]    Script Date: 12/08/2015 17:36:40 ******/
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
GRANT DELETE ON [dbo].[RelationshipProfileLevel] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[RelationshipProfileLevel] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[RelationshipProfileLevel] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[RelationshipProfileLevel] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[RelationshipTransactionProfile]    Script Date: 12/08/2015 17:36:40 ******/
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
GRANT DELETE ON [dbo].[RelationshipTransactionProfile] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[RelationshipTransactionProfile] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[RelationshipTransactionProfile] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[RelationshipTransactionProfile] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[RelationshipUserProfile]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipUserProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ProfileId] [int] NOT NULL,
	[DefaultProfile] [bit] NOT NULL,
 CONSTRAINT [PK_RelUsuarioPerfil_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
GRANT DELETE ON [dbo].[RelationshipUserProfile] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[RelationshipUserProfile] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[RelationshipUserProfile] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[RelationshipUserProfile] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[RelationshipUserProfileInstitution]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipUserProfileInstitution](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RelationshipUserProfileId] [int] NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_RelUsuarioPerfilOrgao] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
GRANT DELETE ON [dbo].[RelationshipUserProfileInstitution] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[RelationshipUserProfileInstitution] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[RelationshipUserProfileInstitution] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[RelationshipUserProfileInstitution] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Responsible]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Responsible](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Position] [varchar](60) NULL,
	[ManagerId] [int] NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_Responsible_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_Responsavel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Responsible] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Responsible] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Responsible] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Responsible] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Section]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Section](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdministrativeUnitId] [int] NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](120) NULL,
	[AddressId] [int] NULL,
	[Telephone] [varchar](20) NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Divisao] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Section] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Section] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Section] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Section] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Signature]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Signature](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[PositionCamex] [varchar](100) NULL,
	[Post] [varchar](50) NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_Signature_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_Assinatura] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Signature] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Signature] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Signature] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Signature] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[SpendingOrigin]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SpendingOrigin](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[ActivityIndicator] [bit] NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_SpendingOrigin_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_NaturezaDespesa] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[SpendingOrigin] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[SpendingOrigin] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[SpendingOrigin] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[SpendingOrigin] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[StateConservation]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StateConservation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](60) NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_StateConservation_Status]  DEFAULT ((1))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[StateConservation] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[StateConservation] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[StateConservation] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[StateConservation] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Supplier]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Supplier](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CPFCNPJ] [varchar](14) NOT NULL,
	[Name] [varchar](60) NOT NULL,
	[AddressId] [int] NULL,
	[Telephone] [varchar](20) NULL,
	[Email] [varchar](50) NULL,
	[AdditionalData] [varchar](255) NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_Supplier_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_Fornecedor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Supplier] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Supplier] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Supplier] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Supplier] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[SupplyUnit]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SupplyUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](12) NOT NULL,
	[Description] [varchar](30) NOT NULL,
	[ManagerId] [int] NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_SupplyUnit_Status]  DEFAULT ((1)),
 CONSTRAINT [PK_UnidadeFornecimento] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[SupplyUnit] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[SupplyUnit] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[SupplyUnit] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[SupplyUnit] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Ticket]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Ticket](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OpenDate] [datetime] NOT NULL,
	[ClosedDate] [datetime] NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[ManagedSystemId] [int] NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[OperatorRating] [int] NULL,
	[TicketRating] [int] NULL,
	[DescriptionRating] [varchar](max) NULL,
	[TicketTypeId] [int] NOT NULL,
	[TicketStatusId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_Ticket] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Ticket] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Ticket] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Ticket] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Ticket] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[TicketHistory]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TicketHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TicketId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[HistoryDate] [datetime] NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[TicketStatusId] [int] NOT NULL,
 CONSTRAINT [PK_TicketHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[TicketHistory] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[TicketHistory] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[TicketHistory] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[TicketHistory] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[TicketStatus]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TicketStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](200) NOT NULL,
 CONSTRAINT [PK_TicketStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[TicketStatus] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[TicketStatus] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[TicketStatus] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[TicketStatus] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[TicketType]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TicketType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](200) NOT NULL,
 CONSTRAINT [PK_TicketType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[TicketType] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[TicketType] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[TicketType] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[TicketType] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Transaction]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Transaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModuleId] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[Initial] [varchar](300) NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[Path] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Transacao] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Transaction] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Transaction] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Transaction] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Transaction] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[Transfer]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Transfer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InitialId] [int] NOT NULL,
	[NumberIdentification] [varchar](6) NOT NULL,
	[PartNumberIdentification] [varchar](2) NULL,
	[InstitutionId] [int] NOT NULL,
	[ManagerId] [int] NOT NULL,
	[BudgetUnitId] [int] NOT NULL,
	[ManagerUnitId] [int] NOT NULL,
	[AdministrativeUnitId] [int] NOT NULL,
	[SectionId] [int] NOT NULL,
	[ResponsibleId] [int] NOT NULL,
	[MaterialItemId] [int] NOT NULL,
	[AuxiliaryAccountId] [int] NULL,
	[OutSourcedId] [int] NULL,
	[TypeIncorporationId] [int] NOT NULL,
	[StateConservationId] [int] NOT NULL,
	[MovementTypeId] [int] NOT NULL,
	[SupplierId] [int] NULL,
	[InclusionDate] [datetime] NULL,
	[AcquisitionDate] [datetime] NULL,
	[ValueAcquisition] [money] NULL,
	[ValueUpdate] [money] NULL,
	[NumberPurchaseProcess] [varchar](25) NULL,
	[NGPB] [varchar](15) NULL,
	[Invoice] [varchar](15) NULL,
	[SerialNumber] [varchar](50) NULL,
	[Manufacture] [varchar](15) NULL,
	[DateGuarantee] [datetime] NULL,
	[ChassiNumber] [varchar](20) NULL,
	[Brand] [varchar](20) NULL,
	[Model] [varchar](20) NULL,
	[NumberPlate] [varchar](8) NULL,
	[AdditionalDescription] [varchar](20) NULL,
	[HistoricId] [int] NULL,
	[Condition] [bit] NOT NULL,
	[Rating] [bit] NOT NULL,
	[BagDate] [datetime] NULL,
	[OldInitial] [int] NULL,
	[OldNumberIdentification] [varchar](6) NULL,
	[OldPartNumberIdentification] [varchar](2) NULL,
	[ReceiptTermDate] [datetime] NULL,
	[InventoryDate] [datetime] NULL,
	[NoteGranting] [varchar](11) NULL,
	[AssigningAmountDate] [datetime] NULL,
	[DestinationInstituitionId] [int] NOT NULL,
	[DestinationManagerId] [int] NOT NULL,
	[DestinationBudgetUnitId] [int] NOT NULL,
	[DestinationManagmentUnitId] [int] NOT NULL,
	[DestinationAdministrativeUnitId] [int] NOT NULL,
	[DestinationSectionId] [int] NOT NULL,
	[DestinationResponsibleId] [int] NOT NULL,
	[DestinationAuxiliaryAccountId] [int] NULL,
	[DestinationUserId] [int] NOT NULL,
	[TransferDate] [datetime] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[Transfer] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[Transfer] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[Transfer] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[Transfer] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[TypeDocumentOut]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TypeDocumentOut](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](60) NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_TypeDocumentOut_Status]  DEFAULT ((1))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[TypeDocumentOut] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[TypeDocumentOut] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[TypeDocumentOut] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[TypeDocumentOut] TO [samw_user] AS [dbo]
GO
/****** Object:  Table [dbo].[User]    Script Date: 12/08/2015 17:36:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CPF] [varchar](11) NOT NULL,
	[Status] [bit] NOT NULL CONSTRAINT [DF_Usuario_Status]  DEFAULT ((1)),
	[Name] [varchar](200) NOT NULL,
	[Email] [varchar](100) NULL,
	[Password] [varchar](255) NULL,
	[Phrase] [varchar](50) NOT NULL,
	[AddedDate] [datetime] NULL CONSTRAINT [DF__Usuario__DataCad__4D94879B]  DEFAULT (getdate()),
	[InvalidAttempts] [int] NULL CONSTRAINT [DF_Usuario_TentivasInvalidas]  DEFAULT ((0)),
	[Blocked] [bit] NOT NULL CONSTRAINT [DF__Usuario__Bloquea__4F7CD00D]  DEFAULT ((0)),
	[ChangePassword] [bit] NOT NULL CONSTRAINT [DF_Usuario_Bloqueado]  DEFAULT ((0)),
	[AddressId] [int] NOT NULL,
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
GRANT DELETE ON [dbo].[User] TO [samw_user] AS [dbo]
GO
GRANT INSERT ON [dbo].[User] TO [samw_user] AS [dbo]
GO
GRANT SELECT ON [dbo].[User] TO [samw_user] AS [dbo]
GO
GRANT UPDATE ON [dbo].[User] TO [samw_user] AS [dbo]
GO
SET IDENTITY_INSERT [dbo].[Address] ON 

INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (1, N'Sede', N's/n', NULL, N'Centro', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (2, N'q', NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (3, N'1', N'1', N'1', N'3', N'3', N'1', NULL)
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (4, N'Doutor Marinho de Andrade', N'131', N'Casa', N'Jardim Jamaica', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (5, N'Araraquara', N'93', NULL, N'Parque Paulista', N'Francisco Morato', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (6, N'Sede', N's/n', NULL, N'Centro', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (7, N'Sede - 01', N's/n', NULL, N'Centro', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (8, N'Centro', N's/n', NULL, N'Centro', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (9, N'Agueda Gonçalves', N'340', N'1ª Andar', N'Pedro Gonçalves', N'São Paulo', N'SP', N'02467140')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (10, N'Sede', N'123', NULL, N'Centro', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (11, N'Sede', N'123', NULL, N'Centro', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (12, N'Doutor Marinho de Andrade', N'456', N'Casa', N'Jardim Jamaica', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (14, N'Teste', N'123', N'teste', N'teste', N'teste', N'sp', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (15, N'Doutor Marinho de Andrade, 131', N'131', N'Sem Complemento', N'Centro', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (16, N'Sede', N's/n', NULL, N'Centro', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (17, N'Doutor Marinho de Andrade', N'131', NULL, N'Centro', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (18, N'Doutor Marinho de Andrade', N'131', NULL, N'Centro', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (19, N'Sede', N's/n', NULL, N'Centro', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (20, N'Doutor Marinho de Andrade, 131', N's/n', NULL, N'Centro', N'São Paulo', N'SP', N'05743170')
INSERT [dbo].[Address] ([Id], [Street], [Number], [ComplementAddress], [District], [City], [State], [PostalCode]) VALUES (21, N'Centro', N'32', NULL, N'Centro', N'São Paulo', N'SP', N'05743170')
SET IDENTITY_INSERT [dbo].[Address] OFF
SET IDENTITY_INSERT [dbo].[Addressee] ON 

INSERT [dbo].[Addressee] ([Id], [Description], [Status]) VALUES (1, N'Destinatário 01', 1)
INSERT [dbo].[Addressee] ([Id], [Description], [Status]) VALUES (2, N'Destinatário - 02', 0)
SET IDENTITY_INSERT [dbo].[Addressee] OFF
SET IDENTITY_INSERT [dbo].[AdministrativeUnit] ON 

INSERT [dbo].[AdministrativeUnit] ([Id], [ManagerUnitId], [ResponsibleId], [Code], [Description], [RelationshipAdministrativeUnitId], [Status], [CostCenterId]) VALUES (1, 1, 1, 1, N'UA - Homologação 01', NULL, 1, 1)
INSERT [dbo].[AdministrativeUnit] ([Id], [ManagerUnitId], [ResponsibleId], [Code], [Description], [RelationshipAdministrativeUnitId], [Status], [CostCenterId]) VALUES (4, 1, 1, 3, N'UA - 03', NULL, 0, 1)
SET IDENTITY_INSERT [dbo].[AdministrativeUnit] OFF
SET IDENTITY_INSERT [dbo].[AssetOutReason] ON 

INSERT [dbo].[AssetOutReason] ([Id], [Description], [Status]) VALUES (1, N'Motivo Baixa 01', 1)
INSERT [dbo].[AssetOutReason] ([Id], [Description], [Status]) VALUES (2, N'Motivo Baixa - 02', 0)
SET IDENTITY_INSERT [dbo].[AssetOutReason] OFF
SET IDENTITY_INSERT [dbo].[AuxiliaryAccount] ON 

INSERT [dbo].[AuxiliaryAccount] ([Id], [ManagerId], [Code], [Description], [BookAccount], [Status]) VALUES (1, 5, 1, N'Conta Auxiliar 01', 1, 1)
INSERT [dbo].[AuxiliaryAccount] ([Id], [ManagerId], [Code], [Description], [BookAccount], [Status]) VALUES (4, 5, 2, N'Conta Auxiliar - 02', 2, 0)
SET IDENTITY_INSERT [dbo].[AuxiliaryAccount] OFF
SET IDENTITY_INSERT [dbo].[BudgetUnit] ON 

INSERT [dbo].[BudgetUnit] ([Id], [ManagerId], [Code], [Description], [Status]) VALUES (2, 5, 1, N'UO Homologação 01', 1)
INSERT [dbo].[BudgetUnit] ([Id], [ManagerId], [Code], [Description], [Status]) VALUES (4, 7, 2, N'UO Apresentação', 1)
INSERT [dbo].[BudgetUnit] ([Id], [ManagerId], [Code], [Description], [Status]) VALUES (5, 5, 2, N'UO Homologação 02', 1)
SET IDENTITY_INSERT [dbo].[BudgetUnit] OFF
SET IDENTITY_INSERT [dbo].[Configuration] ON 

INSERT [dbo].[Configuration] ([Id], [InstitutionId], [ManagerId], [BudgetUnitId], [ManagmentUnitId], [InitialYearMonthAsset], [ReferenceYearMonthAsset]) VALUES (1, 1, 5, 2, 1, CAST(N'2015-06-29' AS Date), CAST(N'2015-07-28' AS Date))
SET IDENTITY_INSERT [dbo].[Configuration] OFF
SET IDENTITY_INSERT [dbo].[CostCenter] ON 

INSERT [dbo].[CostCenter] ([Id], [Code], [Description], [ManagerId], [Status]) VALUES (1, N'01', N'Informática', 5, 1)
INSERT [dbo].[CostCenter] ([Id], [Code], [Description], [ManagerId], [Status]) VALUES (2, N'05', N'Centro de Custo 05', 5, 0)
SET IDENTITY_INSERT [dbo].[CostCenter] OFF
SET IDENTITY_INSERT [dbo].[IncorporationType] ON 

INSERT [dbo].[IncorporationType] ([Id], [Description], [Status]) VALUES (1, N'Tipo Incorporação 01', 1)
INSERT [dbo].[IncorporationType] ([Id], [Description], [Status]) VALUES (2, N'Tipo Incorporação 02', 1)
INSERT [dbo].[IncorporationType] ([Id], [Description], [Status]) VALUES (3, N'Tipo Incorporação - 03', 0)
SET IDENTITY_INSERT [dbo].[IncorporationType] OFF
SET IDENTITY_INSERT [dbo].[Initial] ON 

INSERT [dbo].[Initial] ([Id], [Name], [Description], [BarCode], [ManagerId], [Status]) VALUES (1, N'BP', N'Bem Patrimonial', N'1', 5, 1)
INSERT [dbo].[Initial] ([Id], [Name], [Description], [BarCode], [ManagerId], [Status]) VALUES (2, N'BPH', N'Bem Patrimonial - Homologação', NULL, 5, 0)
SET IDENTITY_INSERT [dbo].[Initial] OFF
SET IDENTITY_INSERT [dbo].[Institution] ON 

INSERT [dbo].[Institution] ([Id], [Code], [Description], [Status]) VALUES (1, 1, N'Secretária de Homologação', 1)
INSERT [dbo].[Institution] ([Id], [Code], [Description], [Status]) VALUES (2, 2, N'Órgão 02', 0)
SET IDENTITY_INSERT [dbo].[Institution] OFF
SET IDENTITY_INSERT [dbo].[Level] ON 

INSERT [dbo].[Level] ([Id], [ParentId], [Description], [Status]) VALUES (1, NULL, N'Órgão', 1)
INSERT [dbo].[Level] ([Id], [ParentId], [Description], [Status]) VALUES (4, 5, N'UO', 1)
INSERT [dbo].[Level] ([Id], [ParentId], [Description], [Status]) VALUES (5, 1, N'Gestor', 1)
INSERT [dbo].[Level] ([Id], [ParentId], [Description], [Status]) VALUES (6, 4, N'UGE', 1)
INSERT [dbo].[Level] ([Id], [ParentId], [Description], [Status]) VALUES (7, 6, N'UA', 1)
INSERT [dbo].[Level] ([Id], [ParentId], [Description], [Status]) VALUES (8, 7, N'Divisão', 1)
INSERT [dbo].[Level] ([Id], [ParentId], [Description], [Status]) VALUES (9, 5, N'Almoxarifado', 1)
INSERT [dbo].[Level] ([Id], [ParentId], [Description], [Status]) VALUES (10, 9, N'teste01', 0)
SET IDENTITY_INSERT [dbo].[Level] OFF
SET IDENTITY_INSERT [dbo].[ManagedSystem] ON 

INSERT [dbo].[ManagedSystem] ([Id], [Status], [Name], [Description]) VALUES (1, 1, N'Segurança', N'Módulo de configurações do Estoque e Patrimônio')
INSERT [dbo].[ManagedSystem] ([Id], [Status], [Name], [Description]) VALUES (2, 1, N'Estoque', N'Módulo de gestão de estoque')
INSERT [dbo].[ManagedSystem] ([Id], [Status], [Name], [Description]) VALUES (3, 1, N'Patrimônio', N'Módulo de gestão de patrimônio')
INSERT [dbo].[ManagedSystem] ([Id], [Status], [Name], [Description]) VALUES (4, 1, N'Chamados', N'Módulo de gestão de chamados para Estoque e Patrimônio')
INSERT [dbo].[ManagedSystem] ([Id], [Status], [Name], [Description]) VALUES (12, 1, N'Apresentação', N'Apresentação')
SET IDENTITY_INSERT [dbo].[ManagedSystem] OFF
SET IDENTITY_INSERT [dbo].[Manager] ON 

INSERT [dbo].[Manager] ([Id], [InstitutionId], [Name], [ShortName], [AddressId], [Telephone], [Code], [Image], [Status]) VALUES (5, 1, N'Gestão de Homologação', N'GH', 6, N'(11) 28456-6403', 1, NULL, 1)
INSERT [dbo].[Manager] ([Id], [InstitutionId], [Name], [ShortName], [AddressId], [Telephone], [Code], [Image], [Status]) VALUES (6, 1, N'Gestor 02', N'G02', 14, N'(11) 11111-1111', 2, NULL, 0)
INSERT [dbo].[Manager] ([Id], [InstitutionId], [Name], [ShortName], [AddressId], [Telephone], [Code], [Image], [Status]) VALUES (7, 1, N'Gestão de Apresentação', N'GA', 16, N'(11) 2845-6403', 2, NULL, 1)
SET IDENTITY_INSERT [dbo].[Manager] OFF
SET IDENTITY_INSERT [dbo].[ManagerUnit] ON 

INSERT [dbo].[ManagerUnit] ([Id], [BudgetUnitId], [Code], [Description], [Status]) VALUES (1, 2, 1, N'UGE Homologação 01', 1)
INSERT [dbo].[ManagerUnit] ([Id], [BudgetUnitId], [Code], [Description], [Status]) VALUES (3, 2, 2, N'UGE - 02', 0)
SET IDENTITY_INSERT [dbo].[ManagerUnit] OFF
SET IDENTITY_INSERT [dbo].[Module] ON 

INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (1, 1, 1, N'Cadastros', NULL, N'', N'/', NULL, NULL)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (2, 1, 1, N'Gerenciamento Menu', NULL, N'', N'/Menu', 1, 1)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (5, 1, 1, N'Perfil', NULL, N'', N'/Profiles', 1, 4)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (6, 1, 1, N'Nível', NULL, N'', N'/Levels', 1, 6)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (7, 1, 1, N'Usuários', NULL, N'', N'/Users', 1, 5)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (8, 1, 1, N'Órgão', N'Estrutura Hierárquica', N'Configurações dos níveis de gestão dos módulos de Estoque e Patrimônio', N'/Institutions', 1, 11)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (9, 1, 1, N'Gestor', NULL, N'', N'/Managers', 1, 12)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (10, 1, 1, N'Unidade Orçamentária', NULL, N'', N'/BudgetUnits', 1, 13)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (11, 1, 1, N'Unidade Gestora', NULL, N'', N'/ManagerUnits', 1, 14)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (12, 1, 1, N'Unidade Administrativa', NULL, N'', N'/AdministrativeUnits', 1, 15)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (13, 1, 1, N'Divisão', NULL, N'', N'/Sections', 1, 16)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (14, 1, 1, N'Responsáveis', NULL, N'', N'/Responsibles', 1, 17)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (15, 3, 1, N'Centros de Custo', NULL, N'', N'/CostCenters', 18, 2)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (18, 3, 1, N'Cadastros', NULL, N'', N'/', NULL, NULL)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (19, 1, 1, N'Rel. Usuário Perfil Órgão', NULL, N'', N'/RelationshipUsersProfilesInstitutions', 1, 10)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (20, 1, 1, N'Rel. Perfil Nível', NULL, N'', N'/RelationshipProfilesLevels', 1, 9)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (21, 1, 1, N'Rel. Transação Perfil', NULL, N'', N'/RelationshipTransactionsProfiles', 1, 8)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (22, 1, 1, N'Rel. Usuário Perfil', N'Relacionamentos', N'Configurações dos Níveis de Acesso', N'/RelationshipUsersProfiles', 1, 7)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (32, 3, 1, N'Sub Item Material', NULL, N'', N'/MaterialSubItems', 18, 14)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (33, 3, 1, N'Natureza Despesa', NULL, N'', N'/SpendingOrigins', 18, 11)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (34, 3, 1, N'Unidade de Fornecimento', NULL, N'', N'/SupplyUnits', 18, 18)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (35, 3, 1, N'Conta Auxiliar', NULL, N'', N'/AuxiliaryAccounts', 18, 4)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (37, 3, 1, N'Terceiro', NULL, N'', N'/OutSourceds', 18, 15)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (38, 3, 1, N'Sigla', NULL, N'', N'/Initials', 18, 13)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (39, 3, 1, N'Tipos Documentos Baixa', NULL, N'', N'/TypesDocumentOut', 18, 17)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (40, 3, 1, N'Tipo Incorporação', NULL, N'', N'/IncorporationTypes', 18, 16)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (41, 3, 1, N'Motivo Baixa', NULL, N'', N'/AssetOutReasons', 18, 10)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (42, 3, 1, N'Fornecedor', NULL, N'', N'/Suppliers', 18, 6)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (43, 3, 1, N'Assinatura', NULL, N'', N'/Signatures', 18, 1)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (44, 3, 1, N'Destinatario', NULL, N'', N'/Addressees', 18, 5)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (45, 3, 1, N'Grupo Material', NULL, N'', N'/MaterialGroups', 18, 7)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (46, 3, 1, N'Classe Material', NULL, N'', N'/MaterialClasses', 18, 3)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (47, 3, 1, N'Material', NULL, N'', N'/Materials', 18, 9)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (48, 3, 1, N'Item Material', NULL, N'', N'/MaterialItems', 18, 8)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (52, 3, 1, N'Bem Patrimonial', NULL, NULL, N'/Assets', NULL, NULL)
INSERT [dbo].[Module] ([Id], [ManagedSystemId], [Status], [Name], [MenuName], [Description], [Path], [ParentId], [Sequence]) VALUES (53, 3, 1, N'Responsáveis', NULL, N'', N'/Responsibles', 18, 12)
SET IDENTITY_INSERT [dbo].[Module] OFF
SET IDENTITY_INSERT [dbo].[MovementType] ON 

INSERT [dbo].[MovementType] ([Id], [Code], [Description], [Status]) VALUES (1, 1, N'Tipo Movimento 01', 1)
SET IDENTITY_INSERT [dbo].[MovementType] OFF
SET IDENTITY_INSERT [dbo].[OutSourced] ON 

INSERT [dbo].[OutSourced] ([Id], [Name], [CPFCNPJ], [AddressId], [Telephone], [Status]) VALUES (1, N'Vinicius Nunes Macedo', N'30778432858', 10, N'11985811233', 1)
INSERT [dbo].[OutSourced] ([Id], [Name], [CPFCNPJ], [AddressId], [Telephone], [Status]) VALUES (2, N'Jorge - Franco', N'30778432858', 20, N'11985811233', 0)
SET IDENTITY_INSERT [dbo].[OutSourced] OFF
SET IDENTITY_INSERT [dbo].[Profile] ON 

INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (2, 1, N'Operador de Almoxarifado')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (3, 1, N'Requisitante')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (4, 1, N'Administrador do Gestor')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (5, 1, N'Administrador Geral')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (6, 1, N'Administrador de Órgão')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (7, 1, N'Consulta Relatório Almoxarifado')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (8, 1, N'Requisitante Geral')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (9, 1, N'Administrador do Patrimônio')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (10, 1, N'Núcleo do Patrimônio')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (11, 1, N'Almoxarifado')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (12, 1, N'Operador da UGE Patrimônio')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (13, 1, N'Consulta da UGE Patrimônio')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (14, 1, N'Operador da UA Patrimônio')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (15, 1, N'Consulta de UA Patrimônio')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (16, 1, N'Consulta Geral Patrimônio')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (17, 1, N'Operador Único Patrimônio')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (20, 1, N'Homologação')
INSERT [dbo].[Profile] ([Id], [Status], [Description]) VALUES (21, 1, N'Help Desk')
SET IDENTITY_INSERT [dbo].[Profile] OFF
SET IDENTITY_INSERT [dbo].[RelationshipAdministrativeUnitSection] ON 

INSERT [dbo].[RelationshipAdministrativeUnitSection] ([Id], [RelationshipManagerUnitAdministrativeUnit], [SectionId], [Status]) VALUES (1, 1, 3, 1)
SET IDENTITY_INSERT [dbo].[RelationshipAdministrativeUnitSection] OFF
SET IDENTITY_INSERT [dbo].[RelationshipBudgetUnitManagerUnit] ON 

INSERT [dbo].[RelationshipBudgetUnitManagerUnit] ([Id], [RelationshipManagerBudgetUnitId], [ManagerUnitId], [Status]) VALUES (1, 1, 1, 1)
SET IDENTITY_INSERT [dbo].[RelationshipBudgetUnitManagerUnit] OFF
SET IDENTITY_INSERT [dbo].[RelationshipInstitutionManager] ON 

INSERT [dbo].[RelationshipInstitutionManager] ([Id], [RelationshipUserProfileInstitutionId], [ManagerId], [Status]) VALUES (3, 2, 5, 1)
INSERT [dbo].[RelationshipInstitutionManager] ([Id], [RelationshipUserProfileInstitutionId], [ManagerId], [Status]) VALUES (4, 2, 7, 1)
SET IDENTITY_INSERT [dbo].[RelationshipInstitutionManager] OFF
SET IDENTITY_INSERT [dbo].[RelationshipManagerBudgetUnit] ON 

INSERT [dbo].[RelationshipManagerBudgetUnit] ([Id], [RelationshipInstitutionManagerId], [BudgetUnitId], [Status]) VALUES (1, 3, 2, 1)
SET IDENTITY_INSERT [dbo].[RelationshipManagerBudgetUnit] OFF
SET IDENTITY_INSERT [dbo].[RelationshipManagerUnitAdministrativeUnit] ON 

INSERT [dbo].[RelationshipManagerUnitAdministrativeUnit] ([Id], [RelationshipBudgetUnitManagerUnitId], [AdministrativeUnitId], [Status]) VALUES (1, 1, 1, 1)
SET IDENTITY_INSERT [dbo].[RelationshipManagerUnitAdministrativeUnit] OFF
SET IDENTITY_INSERT [dbo].[RelationshipProfileLevel] ON 

INSERT [dbo].[RelationshipProfileLevel] ([Id], [ProfileId], [LevelId]) VALUES (1, 5, 1)
SET IDENTITY_INSERT [dbo].[RelationshipProfileLevel] OFF
SET IDENTITY_INSERT [dbo].[RelationshipTransactionProfile] ON 

INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3828, 3, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3830, 9, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3831, 10, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3832, 11, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3833, 12, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3834, 26, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3835, 27, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3836, 28, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3837, 29, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3838, 30, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3839, 31, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3840, 32, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3841, 33, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3842, 34, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3843, 35, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3844, 36, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3845, 37, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3846, 38, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3847, 39, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3848, 40, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3849, 41, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3850, 42, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3851, 43, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3852, 44, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3853, 45, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3854, 46, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3855, 47, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3856, 48, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3857, 49, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3858, 52, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3859, 53, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3860, 54, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3861, 55, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3862, 56, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3863, 57, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3865, 60, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3866, 62, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3867, 63, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3868, 64, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3869, 65, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3870, 66, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3871, 67, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3872, 68, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3873, 69, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3874, 70, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3875, 71, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3876, 72, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3877, 73, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3878, 74, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3879, 75, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3880, 76, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3881, 77, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3882, 78, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3883, 79, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3884, 90, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3885, 91, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3886, 92, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3887, 93, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3888, 94, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3889, 95, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3890, 96, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3891, 97, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3892, 98, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3893, 99, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3894, 100, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3895, 101, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3896, 102, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3897, 103, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3898, 104, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3899, 105, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3900, 106, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3901, 107, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3902, 108, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3903, 109, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3904, 110, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3905, 131, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3908, 3, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3910, 9, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3911, 10, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3912, 11, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3913, 12, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3914, 26, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3915, 27, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3916, 28, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3917, 29, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3918, 30, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3919, 31, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3920, 32, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3921, 33, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3922, 34, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3923, 35, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3924, 36, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3925, 37, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3926, 38, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3927, 39, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3928, 40, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3929, 41, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3930, 42, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3931, 43, 3, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3932, 44, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3933, 45, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3934, 46, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3935, 47, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3936, 48, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3937, 49, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3938, 52, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3939, 53, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3940, 54, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3941, 55, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3942, 56, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3943, 57, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3945, 60, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3946, 62, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3947, 63, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3948, 64, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3949, 65, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3950, 66, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3951, 67, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3952, 68, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3953, 69, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3954, 70, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3955, 71, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3956, 72, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3957, 73, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3958, 74, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3959, 75, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3960, 76, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3961, 77, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3962, 78, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3963, 79, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3964, 90, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3965, 91, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3966, 92, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3967, 93, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3968, 94, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3969, 95, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3970, 96, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3971, 97, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3972, 98, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3973, 99, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3974, 100, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3975, 101, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3976, 102, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3977, 103, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3978, 104, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3979, 105, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3980, 106, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3981, 107, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3982, 108, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3983, 109, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3984, 110, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3985, 131, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3988, 3, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3990, 9, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3991, 10, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3992, 11, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3993, 12, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3994, 26, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3995, 27, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3996, 28, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3997, 29, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3998, 30, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (3999, 31, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4000, 32, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4001, 33, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4002, 34, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4003, 35, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4004, 36, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4005, 37, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4006, 38, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4007, 39, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4008, 40, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4009, 41, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4010, 42, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4011, 43, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4012, 44, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4013, 45, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4014, 46, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4015, 47, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4016, 48, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4017, 49, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4018, 52, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4019, 53, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4020, 54, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4021, 55, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4022, 56, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4023, 57, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4025, 60, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4026, 62, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4027, 63, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4028, 64, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4029, 65, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4030, 66, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4031, 67, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4032, 68, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4033, 69, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4034, 70, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4035, 71, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4036, 72, 4, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4037, 73, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4038, 74, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4039, 75, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4040, 76, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4041, 77, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4042, 78, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4043, 79, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4044, 90, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4045, 91, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4046, 92, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4047, 93, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4048, 94, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4049, 95, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4050, 96, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4051, 97, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4052, 98, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4053, 99, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4054, 100, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4055, 101, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4056, 102, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4057, 103, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4058, 104, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4059, 105, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4060, 106, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4061, 107, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4062, 108, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4063, 109, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4064, 110, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4065, 131, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4068, 3, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4070, 9, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4071, 10, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4072, 11, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4073, 12, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4074, 26, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4075, 27, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4076, 28, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4077, 29, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4078, 30, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4079, 31, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4080, 32, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4081, 33, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4082, 34, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4083, 35, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4084, 36, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4085, 37, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4086, 38, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4087, 39, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4088, 40, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4089, 41, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4090, 42, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4091, 43, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4092, 44, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4093, 45, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4094, 46, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4095, 47, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4096, 48, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4097, 49, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4098, 52, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4099, 53, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4100, 54, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4101, 55, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4102, 56, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4103, 57, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4105, 60, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4106, 62, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4107, 63, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4108, 64, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4109, 65, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4110, 66, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4111, 67, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4112, 68, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4113, 69, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4114, 70, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4115, 71, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4116, 72, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4117, 73, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4118, 74, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4119, 75, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4120, 76, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4121, 77, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4122, 78, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4123, 79, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4124, 90, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4125, 91, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4126, 92, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4127, 93, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4128, 94, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4129, 95, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4130, 96, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4131, 97, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4132, 98, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4133, 99, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4134, 100, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4135, 101, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4136, 102, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4137, 103, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4138, 104, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4139, 105, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4140, 106, 5, 1)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4141, 107, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4142, 108, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4143, 109, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4144, 110, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4145, 131, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4148, 3, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4150, 9, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4151, 10, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4152, 11, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4153, 12, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4154, 26, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4155, 27, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4156, 28, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4157, 29, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4158, 30, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4159, 31, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4160, 32, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4161, 33, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4162, 34, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4163, 35, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4164, 36, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4165, 37, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4166, 38, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4167, 39, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4168, 40, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4169, 41, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4170, 42, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4171, 43, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4172, 44, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4173, 45, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4174, 46, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4175, 47, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4176, 48, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4177, 49, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4178, 52, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4179, 53, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4180, 54, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4181, 55, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4182, 56, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4183, 57, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4185, 60, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4186, 62, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4187, 63, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4188, 64, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4189, 65, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4190, 66, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4191, 67, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4192, 68, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4193, 69, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4194, 70, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4195, 71, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4196, 72, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4197, 73, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4198, 74, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4199, 75, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4200, 76, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4201, 77, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4202, 78, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4203, 79, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4204, 90, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4205, 91, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4206, 92, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4207, 93, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4208, 94, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4209, 95, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4210, 96, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4211, 97, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4212, 98, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4213, 99, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4214, 100, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4215, 101, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4216, 102, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4217, 103, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4218, 104, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4219, 105, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4220, 106, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4221, 107, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4222, 108, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4223, 109, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4224, 110, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4225, 131, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4228, 3, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4230, 9, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4231, 10, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4232, 11, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4233, 12, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4234, 26, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4235, 27, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4236, 28, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4237, 29, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4238, 30, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4239, 31, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4240, 32, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4241, 33, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4242, 34, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4243, 35, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4244, 36, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4245, 37, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4246, 38, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4247, 39, 7, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4248, 40, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4249, 41, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4250, 42, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4251, 43, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4252, 44, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4253, 45, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4254, 46, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4255, 47, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4256, 48, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4257, 49, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4258, 52, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4259, 53, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4260, 54, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4261, 55, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4262, 56, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4263, 57, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4265, 60, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4266, 62, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4267, 63, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4268, 64, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4269, 65, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4270, 66, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4271, 67, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4272, 68, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4273, 69, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4274, 70, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4275, 71, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4276, 72, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4277, 73, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4278, 74, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4279, 75, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4280, 76, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4281, 77, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4282, 78, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4283, 79, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4284, 90, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4285, 91, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4286, 92, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4287, 93, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4288, 94, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4289, 95, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4290, 96, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4291, 97, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4292, 98, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4293, 99, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4294, 100, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4295, 101, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4296, 102, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4297, 103, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4298, 104, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4299, 105, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4300, 106, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4301, 107, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4302, 108, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4303, 109, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4304, 110, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4305, 131, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4308, 3, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4310, 9, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4311, 10, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4312, 11, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4313, 12, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4314, 26, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4315, 27, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4316, 28, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4317, 29, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4318, 30, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4319, 31, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4320, 32, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4321, 33, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4322, 34, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4323, 35, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4324, 36, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4325, 37, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4326, 38, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4327, 39, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4328, 40, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4329, 41, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4330, 42, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4331, 43, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4332, 44, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4333, 45, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4334, 46, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4335, 47, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4336, 48, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4337, 49, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4338, 52, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4339, 53, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4340, 54, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4341, 55, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4342, 56, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4343, 57, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4345, 60, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4346, 62, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4347, 63, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4348, 64, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4349, 65, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4350, 66, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4351, 67, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4352, 68, 8, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4353, 69, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4354, 70, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4355, 71, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4356, 72, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4357, 73, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4358, 74, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4359, 75, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4360, 76, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4361, 77, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4362, 78, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4363, 79, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4364, 90, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4365, 91, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4366, 92, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4367, 93, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4368, 94, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4369, 95, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4370, 96, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4371, 97, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4372, 98, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4373, 99, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4374, 100, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4375, 101, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4376, 102, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4377, 103, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4378, 104, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4379, 105, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4380, 106, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4381, 107, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4382, 108, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4383, 109, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4384, 110, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4385, 131, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4388, 3, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4390, 9, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4391, 10, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4392, 11, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4393, 12, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4394, 26, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4395, 27, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4396, 28, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4397, 29, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4398, 30, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4399, 31, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4400, 32, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4401, 33, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4402, 34, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4403, 35, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4404, 36, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4405, 37, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4406, 38, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4407, 39, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4408, 40, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4409, 41, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4410, 42, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4411, 43, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4412, 44, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4413, 45, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4414, 46, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4415, 47, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4416, 48, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4417, 49, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4418, 52, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4419, 53, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4420, 54, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4421, 55, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4422, 56, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4423, 57, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4425, 60, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4426, 62, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4427, 63, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4428, 64, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4429, 65, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4430, 66, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4431, 67, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4432, 68, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4433, 69, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4434, 70, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4435, 71, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4436, 72, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4437, 73, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4438, 74, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4439, 75, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4440, 76, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4441, 77, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4442, 78, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4443, 79, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4444, 90, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4445, 91, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4446, 92, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4447, 93, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4448, 94, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4449, 95, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4450, 96, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4451, 97, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4452, 98, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4453, 99, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4454, 100, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4455, 101, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4456, 102, 9, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4457, 103, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4458, 104, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4459, 105, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4460, 106, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4461, 107, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4462, 108, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4463, 109, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4464, 110, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4465, 131, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4468, 3, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4470, 9, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4471, 10, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4472, 11, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4473, 12, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4474, 26, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4475, 27, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4476, 28, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4477, 29, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4478, 30, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4479, 31, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4480, 32, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4481, 33, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4482, 34, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4483, 35, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4484, 36, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4485, 37, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4486, 38, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4487, 39, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4488, 40, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4489, 41, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4490, 42, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4491, 43, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4492, 44, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4493, 45, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4494, 46, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4495, 47, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4496, 48, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4497, 49, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4498, 52, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4499, 53, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4500, 54, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4501, 55, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4502, 56, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4503, 57, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4505, 60, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4506, 62, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4507, 63, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4508, 64, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4509, 65, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4510, 66, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4511, 67, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4512, 68, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4513, 69, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4514, 70, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4515, 71, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4516, 72, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4517, 73, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4518, 74, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4519, 75, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4520, 76, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4521, 77, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4522, 78, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4523, 79, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4524, 90, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4525, 91, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4526, 92, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4527, 93, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4528, 94, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4529, 95, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4530, 96, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4531, 97, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4532, 98, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4533, 99, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4534, 100, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4535, 101, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4536, 102, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4537, 103, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4538, 104, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4539, 105, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4540, 106, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4541, 107, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4542, 108, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4543, 109, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4544, 110, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4545, 131, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4548, 3, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4550, 9, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4551, 10, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4552, 11, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4553, 12, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4554, 26, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4555, 27, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4556, 28, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4557, 29, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4558, 30, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4559, 31, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4560, 32, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4561, 33, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4562, 34, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4563, 35, 11, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4564, 36, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4565, 37, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4566, 38, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4567, 39, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4568, 40, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4569, 41, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4570, 42, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4571, 43, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4572, 44, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4573, 45, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4574, 46, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4575, 47, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4576, 48, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4577, 49, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4578, 52, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4579, 53, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4580, 54, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4581, 55, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4582, 56, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4583, 57, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4585, 60, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4586, 62, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4587, 63, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4588, 64, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4589, 65, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4590, 66, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4591, 67, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4592, 68, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4593, 69, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4594, 70, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4595, 71, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4596, 72, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4597, 73, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4598, 74, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4599, 75, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4600, 76, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4601, 77, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4602, 78, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4603, 79, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4604, 90, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4605, 91, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4606, 92, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4607, 93, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4608, 94, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4609, 95, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4610, 96, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4611, 97, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4612, 98, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4613, 99, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4614, 100, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4615, 101, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4616, 102, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4617, 103, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4618, 104, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4619, 105, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4620, 106, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4621, 107, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4622, 108, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4623, 109, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4624, 110, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4625, 131, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4628, 3, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4630, 9, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4631, 10, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4632, 11, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4633, 12, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4634, 26, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4635, 27, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4636, 28, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4637, 29, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4638, 30, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4639, 31, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4640, 32, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4641, 33, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4642, 34, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4643, 35, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4644, 36, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4645, 37, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4646, 38, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4647, 39, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4648, 40, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4649, 41, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4650, 42, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4651, 43, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4652, 44, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4653, 45, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4654, 46, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4655, 47, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4656, 48, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4657, 49, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4658, 52, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4659, 53, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4660, 54, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4661, 55, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4662, 56, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4663, 57, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4665, 60, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4666, 62, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4667, 63, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4668, 64, 12, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4669, 65, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4670, 66, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4671, 67, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4672, 68, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4673, 69, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4674, 70, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4675, 71, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4676, 72, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4677, 73, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4678, 74, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4679, 75, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4680, 76, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4681, 77, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4682, 78, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4683, 79, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4684, 90, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4685, 91, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4686, 92, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4687, 93, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4688, 94, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4689, 95, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4690, 96, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4691, 97, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4692, 98, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4693, 99, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4694, 100, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4695, 101, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4696, 102, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4697, 103, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4698, 104, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4699, 105, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4700, 106, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4701, 107, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4702, 108, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4703, 109, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4704, 110, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4705, 131, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4708, 3, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4710, 9, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4711, 10, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4712, 11, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4713, 12, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4714, 26, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4715, 27, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4716, 28, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4717, 29, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4718, 30, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4719, 31, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4720, 32, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4721, 33, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4722, 34, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4723, 35, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4724, 36, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4725, 37, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4726, 38, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4727, 39, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4728, 40, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4729, 41, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4730, 42, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4731, 43, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4732, 44, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4733, 45, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4734, 46, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4735, 47, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4736, 48, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4737, 49, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4738, 52, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4739, 53, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4740, 54, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4741, 55, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4742, 56, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4743, 57, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4745, 60, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4746, 62, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4747, 63, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4748, 64, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4749, 65, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4750, 66, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4751, 67, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4752, 68, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4753, 69, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4754, 70, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4755, 71, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4756, 72, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4757, 73, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4758, 74, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4759, 75, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4760, 76, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4761, 77, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4762, 78, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4763, 79, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4764, 90, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4765, 91, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4766, 92, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4767, 93, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4768, 94, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4769, 95, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4770, 96, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4771, 97, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4772, 98, 13, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4773, 99, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4774, 100, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4775, 101, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4776, 102, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4777, 103, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4778, 104, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4779, 105, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4780, 106, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4781, 107, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4782, 108, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4783, 109, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4784, 110, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4785, 131, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4788, 3, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4790, 9, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4791, 10, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4792, 11, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4793, 12, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4794, 26, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4795, 27, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4796, 28, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4797, 29, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4798, 30, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4799, 31, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4800, 32, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4801, 33, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4802, 34, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4803, 35, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4804, 36, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4805, 37, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4806, 38, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4807, 39, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4808, 40, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4809, 41, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4810, 42, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4811, 43, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4812, 44, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4813, 45, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4814, 46, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4815, 47, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4816, 48, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4817, 49, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4818, 52, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4819, 53, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4820, 54, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4821, 55, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4822, 56, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4823, 57, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4825, 60, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4826, 62, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4827, 63, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4828, 64, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4829, 65, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4830, 66, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4831, 67, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4832, 68, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4833, 69, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4834, 70, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4835, 71, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4836, 72, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4837, 73, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4838, 74, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4839, 75, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4840, 76, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4841, 77, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4842, 78, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4843, 79, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4844, 90, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4845, 91, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4846, 92, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4847, 93, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4848, 94, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4849, 95, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4850, 96, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4851, 97, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4852, 98, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4853, 99, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4854, 100, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4855, 101, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4856, 102, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4857, 103, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4858, 104, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4859, 105, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4860, 106, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4861, 107, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4862, 108, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4863, 109, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4864, 110, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4865, 131, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4868, 3, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4870, 9, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4871, 10, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4872, 11, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4873, 12, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4874, 26, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4875, 27, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4876, 28, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4877, 29, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4878, 30, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4879, 31, 15, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4880, 32, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4881, 33, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4882, 34, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4883, 35, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4884, 36, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4885, 37, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4886, 38, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4887, 39, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4888, 40, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4889, 41, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4890, 42, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4891, 43, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4892, 44, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4893, 45, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4894, 46, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4895, 47, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4896, 48, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4897, 49, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4898, 52, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4899, 53, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4900, 54, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4901, 55, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4902, 56, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4903, 57, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4905, 60, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4906, 62, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4907, 63, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4908, 64, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4909, 65, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4910, 66, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4911, 67, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4912, 68, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4913, 69, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4914, 70, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4915, 71, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4916, 72, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4917, 73, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4918, 74, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4919, 75, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4920, 76, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4921, 77, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4922, 78, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4923, 79, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4924, 90, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4925, 91, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4926, 92, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4927, 93, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4928, 94, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4929, 95, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4930, 96, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4931, 97, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4932, 98, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4933, 99, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4934, 100, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4935, 101, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4936, 102, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4937, 103, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4938, 104, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4939, 105, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4940, 106, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4941, 107, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4942, 108, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4943, 109, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4944, 110, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4945, 131, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4948, 3, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4950, 9, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4951, 10, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4952, 11, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4953, 12, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4954, 26, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4955, 27, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4956, 28, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4957, 29, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4958, 30, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4959, 31, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4960, 32, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4961, 33, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4962, 34, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4963, 35, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4964, 36, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4965, 37, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4966, 38, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4967, 39, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4968, 40, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4969, 41, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4970, 42, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4971, 43, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4972, 44, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4973, 45, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4974, 46, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4975, 47, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4976, 48, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4977, 49, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4978, 52, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4979, 53, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4980, 54, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4981, 55, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4982, 56, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4983, 57, 16, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4985, 60, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4986, 62, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4987, 63, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4988, 64, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4989, 65, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4990, 66, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4991, 67, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4992, 68, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4993, 69, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4994, 70, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4995, 71, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4996, 72, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4997, 73, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4998, 74, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (4999, 75, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5000, 76, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5001, 77, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5002, 78, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5003, 79, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5004, 90, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5005, 91, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5006, 92, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5007, 93, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5008, 94, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5009, 95, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5010, 96, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5011, 97, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5012, 98, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5013, 99, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5014, 100, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5015, 101, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5016, 102, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5017, 103, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5018, 104, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5019, 105, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5020, 106, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5021, 107, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5022, 108, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5023, 109, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5024, 110, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5025, 131, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5028, 3, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5030, 9, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5031, 10, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5032, 11, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5033, 12, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5034, 26, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5035, 27, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5036, 28, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5037, 29, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5038, 30, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5039, 31, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5040, 32, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5041, 33, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5042, 34, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5043, 35, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5044, 36, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5045, 37, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5046, 38, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5047, 39, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5048, 40, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5049, 41, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5050, 42, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5051, 43, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5052, 44, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5053, 45, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5054, 46, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5055, 47, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5056, 48, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5057, 49, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5058, 52, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5059, 53, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5060, 54, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5061, 55, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5062, 56, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5063, 57, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5065, 60, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5066, 62, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5067, 63, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5068, 64, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5069, 65, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5070, 66, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5071, 67, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5072, 68, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5073, 69, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5074, 70, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5075, 71, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5076, 72, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5077, 73, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5078, 74, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5079, 75, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5080, 76, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5081, 77, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5082, 78, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5083, 79, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5084, 90, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5085, 91, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5086, 92, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5087, 93, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5088, 94, 17, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5089, 95, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5090, 96, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5091, 97, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5092, 98, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5093, 99, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5094, 100, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5095, 101, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5096, 102, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5097, 103, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5098, 104, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5099, 105, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5100, 106, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5101, 107, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5102, 108, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5103, 109, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5104, 110, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5105, 131, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5108, 3, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5110, 9, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5111, 10, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5112, 11, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5113, 12, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5114, 26, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5115, 27, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5116, 28, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5117, 29, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5118, 30, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5119, 31, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5120, 32, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5121, 33, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5122, 34, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5123, 35, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5124, 36, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5125, 37, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5126, 38, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5127, 39, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5128, 40, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5129, 41, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5130, 42, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5131, 43, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5132, 44, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5133, 45, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5134, 46, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5135, 47, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5136, 48, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5137, 49, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5138, 52, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5139, 53, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5140, 54, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5141, 55, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5142, 56, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5143, 57, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5145, 60, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5146, 62, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5147, 63, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5148, 64, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5149, 65, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5150, 66, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5151, 67, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5152, 68, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5153, 69, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5154, 70, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5155, 71, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5156, 72, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5157, 73, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5158, 74, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5159, 75, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5160, 76, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5161, 77, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5162, 78, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5163, 79, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5164, 90, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5165, 91, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5166, 92, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5167, 93, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5168, 94, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5169, 95, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5170, 96, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5171, 97, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5172, 98, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5173, 99, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5174, 100, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5175, 101, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5176, 102, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5177, 103, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5178, 104, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5179, 105, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5180, 106, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5181, 107, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5182, 108, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5183, 109, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5184, 110, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5185, 131, 20, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5186, 3, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5187, 3, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5188, 3, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5190, 3, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5191, 3, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5192, 3, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5193, 3, 9, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5194, 3, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5195, 3, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5196, 3, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5197, 3, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5198, 3, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5199, 3, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5200, 3, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5201, 3, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5202, 3, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5203, 3, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5204, 3, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5205, 3, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5207, 3, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5208, 3, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5209, 3, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5210, 3, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5211, 3, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5212, 3, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5213, 3, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5214, 3, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5215, 3, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5216, 3, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5217, 3, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5218, 3, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5219, 3, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5220, 134, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5221, 134, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5222, 134, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5223, 134, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5224, 134, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5225, 134, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5226, 134, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5227, 134, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5228, 134, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5229, 134, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5230, 134, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5231, 134, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5232, 134, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5233, 134, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5234, 134, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5235, 134, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5236, 134, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5237, 135, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5238, 135, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5239, 135, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5241, 135, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5242, 135, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5243, 135, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5244, 135, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5245, 135, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5246, 135, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5247, 135, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5248, 135, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5249, 135, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5250, 135, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5251, 135, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5252, 135, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5253, 135, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5254, 136, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5255, 136, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5256, 136, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5257, 136, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5258, 136, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5259, 136, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5260, 136, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5261, 136, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5262, 136, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5263, 136, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5264, 136, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5265, 136, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5266, 136, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5267, 136, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5268, 136, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5269, 136, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5270, 136, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5271, 134, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5272, 134, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5273, 134, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5275, 134, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5276, 134, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5277, 134, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5278, 134, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5279, 134, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5280, 134, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5281, 134, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5282, 134, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5283, 134, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5284, 134, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5285, 134, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5286, 134, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5287, 134, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5288, 135, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5289, 135, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5290, 135, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5292, 135, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5293, 135, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5294, 135, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5295, 135, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5296, 135, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5297, 135, 11, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5298, 135, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5299, 135, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5300, 135, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5301, 135, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5302, 135, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5303, 135, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5304, 135, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5305, 136, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5306, 136, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5307, 136, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5309, 136, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5310, 136, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5311, 136, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5312, 136, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5313, 136, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5314, 136, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5315, 136, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5316, 136, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5317, 136, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5318, 136, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5319, 136, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5320, 136, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5321, 136, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5322, 137, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5323, 137, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5324, 137, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5325, 137, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5326, 137, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5327, 137, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5328, 137, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5329, 137, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5330, 137, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5331, 137, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5332, 137, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5333, 137, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5334, 137, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5335, 137, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5336, 137, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5337, 137, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5338, 137, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5339, 135, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5340, 135, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5341, 135, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5342, 135, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5343, 135, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5344, 135, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5345, 135, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5346, 135, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5347, 135, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5348, 135, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5349, 135, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5350, 135, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5351, 135, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5352, 135, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5353, 135, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5354, 135, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5355, 135, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5356, 136, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5357, 136, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5358, 136, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5360, 136, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5361, 136, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5362, 136, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5363, 136, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5364, 136, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5365, 136, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5366, 136, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5367, 136, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5368, 136, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5369, 136, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5370, 136, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5371, 136, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5372, 136, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5373, 134, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5374, 134, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5375, 134, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5377, 134, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5378, 134, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5379, 134, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5380, 134, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5381, 134, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5382, 134, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5383, 134, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5384, 134, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5385, 134, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5386, 134, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5387, 134, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5388, 134, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5389, 134, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5390, 137, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5391, 137, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5392, 137, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5394, 137, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5395, 137, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5396, 137, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5397, 137, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5398, 137, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5399, 137, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5400, 137, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5401, 137, 13, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5402, 137, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5403, 137, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5404, 137, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5405, 137, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5406, 137, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5407, 132, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5408, 133, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5409, 138, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5410, 138, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5411, 138, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5412, 138, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5413, 138, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5414, 138, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5415, 138, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5416, 138, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5417, 138, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5418, 138, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5419, 138, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5420, 138, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5421, 138, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5422, 138, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5423, 138, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5424, 138, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5425, 138, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5426, 139, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5427, 139, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5428, 139, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5429, 139, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5430, 139, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5431, 139, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5432, 139, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5433, 139, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5434, 139, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5435, 139, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5436, 139, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5437, 139, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5438, 139, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5439, 139, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5440, 139, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5441, 139, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5442, 139, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5443, 140, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5444, 140, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5445, 140, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5446, 140, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5447, 140, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5448, 140, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5449, 140, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5450, 140, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5451, 140, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5452, 140, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5453, 140, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5454, 140, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5455, 140, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5456, 140, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5457, 140, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5458, 140, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5459, 140, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5460, 141, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5461, 141, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5462, 141, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5463, 141, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5464, 141, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5465, 141, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5466, 141, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5467, 141, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5468, 141, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5469, 141, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5470, 141, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5471, 141, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5472, 141, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5473, 141, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5474, 141, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5475, 141, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5476, 141, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5477, 142, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5478, 142, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5479, 142, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5480, 142, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5481, 142, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5482, 142, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5483, 142, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5484, 142, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5485, 142, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5486, 142, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5487, 142, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5488, 142, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5489, 142, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5490, 142, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5491, 142, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5492, 142, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5493, 142, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5494, 143, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5495, 143, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5496, 143, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5497, 143, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5498, 143, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5499, 143, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5500, 143, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5501, 143, 9, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5502, 143, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5503, 143, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5504, 143, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5505, 143, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5506, 143, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5507, 143, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5508, 143, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5509, 143, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5510, 143, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5511, 143, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5512, 143, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5513, 143, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5514, 143, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5515, 143, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5516, 143, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5517, 143, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5518, 143, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5519, 143, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5520, 143, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5521, 143, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5522, 143, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5523, 143, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5524, 143, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5525, 143, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5526, 143, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5527, 143, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5528, 144, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5529, 144, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5530, 144, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5531, 144, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5532, 144, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5533, 144, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5534, 144, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5535, 144, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5536, 144, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5537, 144, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5538, 144, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5539, 144, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5540, 144, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5541, 144, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5542, 144, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5543, 144, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5544, 144, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5545, 145, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5546, 145, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5547, 145, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5548, 145, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5549, 145, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5550, 145, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5551, 145, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5552, 145, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5553, 145, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5554, 145, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5555, 145, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5556, 145, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5557, 145, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5558, 145, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5559, 145, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5560, 145, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5561, 145, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5562, 146, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5563, 146, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5564, 146, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5565, 146, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5566, 146, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5567, 146, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5568, 146, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5569, 146, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5570, 146, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5571, 146, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5572, 146, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5573, 146, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5574, 146, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5575, 146, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5576, 146, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5577, 146, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5578, 146, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5579, 147, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5580, 147, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5581, 147, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5582, 147, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5583, 147, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5584, 147, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5585, 147, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5586, 147, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5587, 147, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5588, 147, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5589, 147, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5590, 147, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5591, 147, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5592, 147, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5593, 147, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5594, 147, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5595, 147, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5596, 148, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5597, 148, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5598, 148, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5599, 148, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5600, 148, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5601, 148, 7, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5602, 148, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5603, 148, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5604, 148, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5605, 148, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5606, 148, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5607, 148, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5608, 148, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5609, 148, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5610, 148, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5611, 148, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5612, 148, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5613, 149, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5614, 149, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5615, 149, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5616, 149, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5617, 149, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5618, 149, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5619, 149, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5620, 149, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5621, 149, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5622, 149, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5623, 149, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5624, 149, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5625, 149, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5626, 149, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5627, 149, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5628, 149, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5629, 149, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5630, 150, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5631, 150, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5632, 150, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5633, 150, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5634, 150, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5635, 150, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5636, 150, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5637, 150, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5638, 150, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5639, 150, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5640, 150, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5641, 150, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5642, 150, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5643, 150, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5644, 150, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5645, 150, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5646, 150, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5647, 151, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5648, 151, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5649, 151, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5650, 151, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5651, 151, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5652, 151, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5653, 151, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5654, 151, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5655, 151, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5656, 151, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5657, 151, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5658, 151, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5659, 151, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5660, 151, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5661, 151, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5662, 151, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5663, 151, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5664, 152, 2, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5665, 152, 3, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5666, 152, 4, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5667, 152, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5668, 152, 6, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5669, 152, 7, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5670, 152, 8, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5671, 152, 9, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5672, 152, 10, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5673, 152, 11, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5674, 152, 12, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5675, 152, 13, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5676, 152, 14, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5677, 152, 15, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5678, 152, 16, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5679, 152, 17, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5680, 152, 20, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5681, 153, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5682, 154, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5683, 155, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5684, 156, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5685, 157, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5686, 158, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5687, 159, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5688, 160, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5689, 161, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5690, 162, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5691, 163, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5692, 164, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5693, 165, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5694, 166, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5695, 167, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5696, 168, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5697, 169, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5698, 170, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5699, 171, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5700, 172, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5701, 173, 5, 1)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5702, 174, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5703, 175, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5704, 176, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5705, 177, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5706, 178, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5707, 179, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5708, 180, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5709, 181, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5710, 182, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5711, 183, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5712, 184, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5713, 185, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5714, 186, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5715, 187, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5716, 188, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5717, 189, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5718, 190, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5719, 191, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5720, 192, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5721, 193, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5722, 194, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5723, 195, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5724, 196, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5725, 197, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5726, 198, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5727, 199, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5728, 200, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5729, 201, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5730, 202, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5731, 203, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5732, 204, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5733, 205, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5734, 206, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5735, 207, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5736, 208, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5737, 209, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5738, 210, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5739, 211, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5740, 212, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5741, 213, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5742, 214, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5743, 215, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5744, 216, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5745, 217, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5746, 218, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5747, 219, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5748, 220, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5749, 221, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5750, 222, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5751, 223, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5752, 224, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5753, 225, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5754, 226, 5, 1)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5755, 3, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5756, 9, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5757, 10, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5758, 11, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5759, 12, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5760, 26, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5761, 27, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5762, 28, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5763, 29, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5764, 30, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5765, 31, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5766, 32, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5767, 33, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5768, 34, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5769, 35, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5770, 36, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5771, 37, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5772, 38, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5773, 39, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5774, 40, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5775, 41, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5776, 42, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5777, 43, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5778, 44, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5779, 45, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5780, 46, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5781, 47, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5782, 48, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5783, 49, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5784, 52, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5785, 53, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5786, 54, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5787, 55, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5788, 56, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5789, 57, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5790, 60, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5791, 62, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5792, 63, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5793, 64, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5794, 65, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5795, 66, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5796, 67, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5797, 68, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5798, 69, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5799, 70, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5800, 71, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5801, 72, 21, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5802, 73, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5803, 74, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5804, 75, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5805, 76, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5806, 77, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5807, 78, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5808, 79, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5809, 90, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5810, 91, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5811, 92, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5812, 93, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5813, 94, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5814, 95, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5815, 96, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5816, 97, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5817, 98, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5818, 99, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5819, 100, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5820, 101, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5821, 102, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5822, 103, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5823, 104, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5824, 105, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5825, 106, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5826, 107, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5827, 108, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5828, 109, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5829, 110, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5830, 131, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5831, 132, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5832, 133, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5833, 134, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5834, 135, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5835, 136, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5836, 137, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5837, 138, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5838, 139, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5839, 140, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5840, 141, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5841, 142, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5842, 143, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5843, 144, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5844, 145, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5845, 146, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5846, 147, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5847, 148, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5848, 149, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5849, 150, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5850, 151, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5851, 152, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5852, 153, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5853, 154, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5854, 155, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5855, 156, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5856, 157, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5857, 158, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5858, 159, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5859, 160, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5860, 161, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5861, 162, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5862, 163, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5863, 164, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5864, 165, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5865, 166, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5866, 167, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5867, 168, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5868, 169, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5869, 170, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5870, 171, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5871, 172, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5872, 173, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5873, 174, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5874, 175, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5875, 176, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5876, 177, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5877, 178, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5878, 179, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5879, 180, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5880, 181, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5881, 182, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5882, 183, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5883, 184, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5884, 185, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5885, 186, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5886, 187, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5887, 188, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5888, 189, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5889, 190, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5890, 191, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5891, 192, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5892, 193, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5893, 194, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5894, 195, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5895, 196, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5896, 197, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5897, 198, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5898, 199, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5899, 200, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5900, 201, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5901, 202, 21, 0)
GO
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5902, 203, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5903, 204, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5904, 205, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5905, 206, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5906, 207, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5907, 208, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5908, 209, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5909, 210, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5910, 211, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5911, 212, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5912, 213, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5913, 214, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5914, 215, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5915, 216, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5916, 217, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5917, 218, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5918, 219, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5919, 220, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5920, 221, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5921, 222, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5922, 223, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5923, 224, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5924, 225, 21, 0)
INSERT [dbo].[RelationshipTransactionProfile] ([Id], [TransactionId], [ProfileId], [Status]) VALUES (5925, 226, 21, 0)
SET IDENTITY_INSERT [dbo].[RelationshipTransactionProfile] OFF
SET IDENTITY_INSERT [dbo].[RelationshipUserProfile] ON 

INSERT [dbo].[RelationshipUserProfile] ([Id], [UserId], [ProfileId], [DefaultProfile]) VALUES (3, 1, 2, 0)
INSERT [dbo].[RelationshipUserProfile] ([Id], [UserId], [ProfileId], [DefaultProfile]) VALUES (4, 1, 5, 1)
INSERT [dbo].[RelationshipUserProfile] ([Id], [UserId], [ProfileId], [DefaultProfile]) VALUES (5, 1, 11, 0)
INSERT [dbo].[RelationshipUserProfile] ([Id], [UserId], [ProfileId], [DefaultProfile]) VALUES (8, 3, 5, 1)
INSERT [dbo].[RelationshipUserProfile] ([Id], [UserId], [ProfileId], [DefaultProfile]) VALUES (9, 4, 20, 1)
INSERT [dbo].[RelationshipUserProfile] ([Id], [UserId], [ProfileId], [DefaultProfile]) VALUES (10, 5, 5, 1)
SET IDENTITY_INSERT [dbo].[RelationshipUserProfile] OFF
SET IDENTITY_INSERT [dbo].[RelationshipUserProfileInstitution] ON 

INSERT [dbo].[RelationshipUserProfileInstitution] ([Id], [RelationshipUserProfileId], [InstitutionId], [Status]) VALUES (2, 4, 1, 1)
INSERT [dbo].[RelationshipUserProfileInstitution] ([Id], [RelationshipUserProfileId], [InstitutionId], [Status]) VALUES (6, 9, 1, 1)
SET IDENTITY_INSERT [dbo].[RelationshipUserProfileInstitution] OFF
SET IDENTITY_INSERT [dbo].[Responsible] ON 

INSERT [dbo].[Responsible] ([Id], [UserId], [Position], [ManagerId], [Status]) VALUES (1, 1, N'Administrador', 5, 1)
INSERT [dbo].[Responsible] ([Id], [UserId], [Position], [ManagerId], [Status]) VALUES (2, 1, N'Teste 05', 5, 0)
INSERT [dbo].[Responsible] ([Id], [UserId], [Position], [ManagerId], [Status]) VALUES (3, 4, N'Coordenador Almoxarifado', 7, 0)
SET IDENTITY_INSERT [dbo].[Responsible] OFF
SET IDENTITY_INSERT [dbo].[Section] ON 

INSERT [dbo].[Section] ([Id], [AdministrativeUnitId], [Code], [Description], [AddressId], [Telephone], [Status]) VALUES (1, 1, 1, N'1', NULL, N'1234', 0)
INSERT [dbo].[Section] ([Id], [AdministrativeUnitId], [Code], [Description], [AddressId], [Telephone], [Status]) VALUES (3, 1, 1, N'Divisão Homologação 01', 7, N'11985811233', 1)
INSERT [dbo].[Section] ([Id], [AdministrativeUnitId], [Code], [Description], [AddressId], [Telephone], [Status]) VALUES (4, 1, 2, N'Divisão Homologação 02', 8, NULL, 1)
INSERT [dbo].[Section] ([Id], [AdministrativeUnitId], [Code], [Description], [AddressId], [Telephone], [Status]) VALUES (5, 1, 4, N'Divisão 04', 15, N'11985811233', 0)
INSERT [dbo].[Section] ([Id], [AdministrativeUnitId], [Code], [Description], [AddressId], [Telephone], [Status]) VALUES (6, 1, 3, N'Divisão Homologação 03', 17, N'11985811233', 1)
SET IDENTITY_INSERT [dbo].[Section] OFF
SET IDENTITY_INSERT [dbo].[Signature] ON 

INSERT [dbo].[Signature] ([Id], [Name], [PositionCamex], [Post], [Status]) VALUES (1, N'Jorge Franco', N'Coordenador', N'Coordenador', 0)
INSERT [dbo].[Signature] ([Id], [Name], [PositionCamex], [Post], [Status]) VALUES (2, N'Vinicius Nunes Macedo', N'Analista de Sistemas', N'Analista de Sistemas', 1)
SET IDENTITY_INSERT [dbo].[Signature] OFF
SET IDENTITY_INSERT [dbo].[SpendingOrigin] ON 

INSERT [dbo].[SpendingOrigin] ([Id], [Code], [Description], [ActivityIndicator], [Status]) VALUES (1, 1, N'Natureza Despesa 01', 1, 1)
INSERT [dbo].[SpendingOrigin] ([Id], [Code], [Description], [ActivityIndicator], [Status]) VALUES (2, 2, N'Natureza Despesa - 02', 1, 0)
SET IDENTITY_INSERT [dbo].[SpendingOrigin] OFF
SET IDENTITY_INSERT [dbo].[StateConservation] ON 

INSERT [dbo].[StateConservation] ([Id], [Description], [Status]) VALUES (1, N'Bom', 1)
SET IDENTITY_INSERT [dbo].[StateConservation] OFF
SET IDENTITY_INSERT [dbo].[Supplier] ON 

INSERT [dbo].[Supplier] ([Id], [CPFCNPJ], [Name], [AddressId], [Telephone], [Email], [AdditionalData], [Status]) VALUES (2, N'30778432858', N'Vinicius Nunes Macedo', 11, N'11985811233', N'vnmacedo@gmail.com', N'Sem mais', 1)
INSERT [dbo].[Supplier] ([Id], [CPFCNPJ], [Name], [AddressId], [Telephone], [Email], [AdditionalData], [Status]) VALUES (3, N'10143333852', N'Ivanilda Pereira Nunes', 18, N'11985811233', N'ivi.nunes@gmail.com', NULL, 0)
SET IDENTITY_INSERT [dbo].[Supplier] OFF
SET IDENTITY_INSERT [dbo].[SupplyUnit] ON 

INSERT [dbo].[SupplyUnit] ([Id], [Code], [Description], [ManagerId], [Status]) VALUES (1, N'01', N'Unidade Fornecimento 01', 5, 1)
INSERT [dbo].[SupplyUnit] ([Id], [Code], [Description], [ManagerId], [Status]) VALUES (2, N'02', N'Unidade Fornecimento 02', 5, 0)
SET IDENTITY_INSERT [dbo].[SupplyUnit] OFF
SET IDENTITY_INSERT [dbo].[Ticket] ON 

INSERT [dbo].[Ticket] ([Id], [OpenDate], [ClosedDate], [InstitutionId], [ManagedSystemId], [Description], [OperatorRating], [TicketRating], [DescriptionRating], [TicketTypeId], [TicketStatusId], [UserId]) VALUES (7, CAST(N'2015-08-11 11:50:19.000' AS DateTime), CAST(N'2015-08-11 11:50:19.000' AS DateTime), 1, 1, N'teste', 0, 0, NULL, 1, 1, 1)
INSERT [dbo].[Ticket] ([Id], [OpenDate], [ClosedDate], [InstitutionId], [ManagedSystemId], [Description], [OperatorRating], [TicketRating], [DescriptionRating], [TicketTypeId], [TicketStatusId], [UserId]) VALUES (8, CAST(N'2015-08-11 19:48:21.680' AS DateTime), CAST(N'2015-08-11 19:48:21.680' AS DateTime), 1, 1, N'teste2', 0, 0, NULL, 1, 1, 1)
INSERT [dbo].[Ticket] ([Id], [OpenDate], [ClosedDate], [InstitutionId], [ManagedSystemId], [Description], [OperatorRating], [TicketRating], [DescriptionRating], [TicketTypeId], [TicketStatusId], [UserId]) VALUES (9, CAST(N'2015-08-12 09:46:29.000' AS DateTime), CAST(N'2015-08-12 09:46:29.000' AS DateTime), 1, 1, N'teste para o Jorge', 0, 0, NULL, 1, 1, 1)
SET IDENTITY_INSERT [dbo].[Ticket] OFF
SET IDENTITY_INSERT [dbo].[TicketHistory] ON 

INSERT [dbo].[TicketHistory] ([Id], [TicketId], [UserId], [HistoryDate], [Description], [TicketStatusId]) VALUES (2, 7, 1, CAST(N'2015-08-11 15:14:36.900' AS DateTime), N'teste', 1)
INSERT [dbo].[TicketHistory] ([Id], [TicketId], [UserId], [HistoryDate], [Description], [TicketStatusId]) VALUES (3, 7, 1, CAST(N'2015-08-11 16:11:11.133' AS DateTime), N'teste', 1)
INSERT [dbo].[TicketHistory] ([Id], [TicketId], [UserId], [HistoryDate], [Description], [TicketStatusId]) VALUES (4, 9, 1, CAST(N'2015-08-12 09:46:48.700' AS DateTime), N'Estou analisando', 2)
INSERT [dbo].[TicketHistory] ([Id], [TicketId], [UserId], [HistoryDate], [Description], [TicketStatusId]) VALUES (5, 9, 1, CAST(N'2015-08-12 09:47:04.190' AS DateTime), N'Continuo analisando', 2)
SET IDENTITY_INSERT [dbo].[TicketHistory] OFF
SET IDENTITY_INSERT [dbo].[TicketStatus] ON 

INSERT [dbo].[TicketStatus] ([Id], [Description]) VALUES (1, N'Aberto')
INSERT [dbo].[TicketStatus] ([Id], [Description]) VALUES (2, N'Em Atendimento')
INSERT [dbo].[TicketStatus] ([Id], [Description]) VALUES (3, N'Aguardando Usuário')
INSERT [dbo].[TicketStatus] ([Id], [Description]) VALUES (4, N'Concluido')
INSERT [dbo].[TicketStatus] ([Id], [Description]) VALUES (5, N'Finalizado')
INSERT [dbo].[TicketStatus] ([Id], [Description]) VALUES (6, N'Reaberto')
SET IDENTITY_INSERT [dbo].[TicketStatus] OFF
SET IDENTITY_INSERT [dbo].[TicketType] ON 

INSERT [dbo].[TicketType] ([Id], [Description]) VALUES (1, N'Erro')
INSERT [dbo].[TicketType] ([Id], [Description]) VALUES (2, N'Dúvida')
SET IDENTITY_INSERT [dbo].[TicketType] OFF
SET IDENTITY_INSERT [dbo].[Transaction] ON 

INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (3, 2, 1, N'Lista de Sistemas/Módulos/Transações Cadastradas', N'Lista de Sistemas/Módulos/Transações Cadastradas', N'/Menu')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (9, 5, 1, N'Cadastro de Perfil', N'Novo', N'/Profiles/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (10, 5, 1, N'Alteração de Perfil', N'Editar', N'/Profiles/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (11, 5, 1, N'Lista de Perfis Cadastrados', N'Lista de Perfis Cadastrados', N'/Profiles')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (12, 5, 1, N'Excluir Perfil', N'Excluir', N'/Profiles/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (26, 6, 1, N'Cadastro de Nível', N'Novo', N'/Levels/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (27, 6, 1, N'Alteração de Nível', N'Editar', N'/Levels/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (28, 6, 1, N'Lista de Níveis', N'Lista de Níveis Cadastrados', N'/Levels')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (29, 6, 1, N'Excluir Nível', N'Excluir', N'/Levels/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (30, 7, 1, N'Cadastro de Usuário', N'Novo', N'/Users/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (31, 7, 1, N'Alteração de Usuário', N'Editar', N'/Users/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (32, 7, 1, N'Lista de Usuários', N'Lista de Usuários Cadastrados', N'/Users')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (33, 7, 1, N'Excluir Usuário', N'Excluir', N'/Users/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (34, 8, 1, N'Cadastro de Órgão', N'Novo', N'/Institutions/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (35, 8, 1, N'Alteração de Órgão', N'Editar', N'/Institutions/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (36, 8, 1, N'Lista de Órgãos', N'Lista de Órgãos Cadastrados', N'/Institutions')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (37, 8, 1, N'Excluir Órgão', N'Excluir', N'/Institutions/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (38, 9, 1, N'Cadastro de Gestor', N'Novo', N'/Managers/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (39, 9, 1, N'Alteração de Gestor', N'Editar', N'/Managers/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (40, 9, 1, N'Lista de Gestores', N'Lista de Gestores Cadastrados', N'/Managers')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (41, 9, 1, N'Excluir Gestor', N'Excluir', N'/Managers/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (42, 10, 1, N'Cadastro de UO', N'Novo', N'/BudgetUnits/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (43, 10, 1, N'Alteração de UO', N'Editar', N'/BudgetUnits/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (44, 10, 1, N'Lista de UOs', N'Lista de Unidades Orçamentárias Cadastradas', N'/BudgetUnits')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (45, 10, 1, N'Excluir UO', N'Excluir', N'/BudgetUnits/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (46, 11, 1, N'Cadastro de UGE', N'Novo', N'/ManagerUnits/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (47, 11, 1, N'Alteração de UGE', N'Editar', N'/ManagerUnits/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (48, 11, 1, N'Lista de UGEs', N'Lista de Unidades Gestoras Cadastradas', N'/ManagerUnits')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (49, 11, 1, N'Excluir UGE', N'Excluir', N'/ManagerUnits/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (52, 12, 1, N'Lista de UAs', N'Lista de Unidades Administrativas Cadastradas', N'/AdministrativeUnits')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (53, 12, 1, N'Excluir UA', N'Excluir', N'/AdministrativeUnits/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (54, 13, 1, N'Cadastro de Divisão', N'Novo', N'/Sections/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (55, 13, 1, N'Alteração de Divisão', N'Editar', N'/Sections/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (56, 13, 1, N'Lista de Divisões', N'Lista de Divisões Cadastradas', N'/Sections')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (57, 13, 1, N'Excluir Divisão', N'Excluir', N'/Sections/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (60, 5, 1, N'Detalhes Perfil', N'Detalhes', N'/Profiles/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (62, 6, 1, N'Detalhes Nível', N'Detalhes', N'/Levels/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (63, 7, 1, N'Detalhes Usuário', N'Detalhes', N'/Users/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (64, 8, 1, N'Detalhes Órgão', N'Detalhes', N'/Institutions/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (65, 9, 1, N'Detalhes Gestor', N'Detalhes', N'/Managers/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (66, 10, 1, N'Detalhes UO', N'Detalhes', N'/BudgetUnits/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (67, 11, 1, N'Detalhes UGE', N'Detalhes', N'/ManagerUnits/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (68, 12, 1, N'Detalhes UA', N'Detalhes', N'/AdministrativeUnits/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (69, 13, 1, N'Detalhes Divisão', N'Detalhes', N'/Sections/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (70, 14, 1, N'Cadastro de Responsável', N'Novo', N'/Responsibles/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (71, 14, 1, N'Alteração de Responsável', N'Editar', N'/Responsibles/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (72, 14, 1, N'Lista de Responsáveis', N'Lista de Responsáveis Cadastrados', N'/Responsibles')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (73, 14, 1, N'Excluir Responsável', N'Excluir', N'/Responsibles/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (74, 14, 1, N'Detalhes Responsável', N'Detalhes', N'/Responsibles/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (75, 15, 1, N'Cadastro de Centro de Custo', N'Novo', N'/CostCenters/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (76, 15, 1, N'Alteração de Centro de Custo', N'Editar', N'/CostCenters/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (77, 15, 1, N'Lista de Centros de Custos', N'Lista de Centros de Custo Cadastrados', N'/CostCenters')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (78, 15, 1, N'Excluir Centro de Custo', N'Excluir', N'/CostCenters/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (79, 15, 1, N'Detalhes Centro de Custo', N'Detalhes', N'/CostCenters/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (90, 19, 1, N'Cadastro de Relacionamento Usuário Perfil Órgão', N'Novo', N'/RelationshipUsersProfilesInstitutions/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (91, 19, 1, N'Alteração de Relacionamento Usuário Perfil Órgão', N'Editar', N'/RelationshipUsersProfilesInstitutions/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (92, 19, 1, N'Lista de Relacionamentos Usuário Perfil Órgão', N'Lista de Órgãos Cadastrados', N'/RelationshipUsersProfilesInstitutions')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (93, 19, 1, N'Excluir Relacionamento Usuário Perfil Órgão', N'Excluir', N'/RelationshipUsersProfilesInstitutions/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (94, 19, 1, N'Detalhes Relacionamento Usuário Perfil Órgão', N'Detalhes', N'/RelationshipUsersProfilesInstitutions/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (95, 22, 1, N'Cadastro de Relacionamento Usuário Perfil ', N'Novo', N'/RelationshipUsersProfiles/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (96, 22, 1, N'Alteração de Relacionamento Usuário Perfil ', N'Editar', N'/RelationshipUsersProfiles/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (97, 22, 1, N'Lista de Relacionamentos Usuário Perfil ', N'Lista de Usuários X Perfis', N'/RelationshipUsersProfiles')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (98, 22, 1, N'Excluir Relacionamento Usuário Perfil ', N'Excluir', N'/RelationshipUsersProfiles/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (99, 22, 1, N'Detalhes Relacionamento Usuário Perfil ', N'Detalhes', N'/RelationshipUsersProfiles/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (100, 20, 1, N'Cadastro de Relacionamento Perfil Nível', N'Novo', N'/RelationshipProfilesLevels/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (101, 20, 1, N'Alteração de Relacionamento Perfil Nível ', N'Editar', N'/RelationshipProfilesLevels/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (102, 20, 1, N'Lista de Relacionamentos Perfil Nível ', N'Lista de Perfis X Níveis', N'/RelationshipProfilesLevels')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (103, 20, 1, N'Excluir Relacionamento Perfil Nível ', N'Excluir', N'/RelationshipProfilesLevels/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (104, 20, 1, N'Detalhes Relacionamento Perfil Nível ', N'Detalhes', N'/RelationshipProfilesLevels/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (105, 21, 1, N'Cadastro de Relacionamento Transação Perfil', N'Novo', N'/RelationshipTransactionsProfiles/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (106, 21, 1, N'Alteração de Relacionamento Transação Perfil ', N'Editar', N'/RelationshipTransactionsProfiles/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (107, 21, 1, N'Lista de Relacionamentos Transação Perfil ', N'Lista de Transações X Perfis', N'/RelationshipTransactionsProfiles')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (108, 21, 1, N'Excluir Relacionamento Transação Perfil ', N'Excluir', N'/RelationshipTransactionsProfiles/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (109, 21, 1, N'Detalhes Relacionamento Transação Perfil ', N'Detalhes', N'/RelationshipTransactionsProfiles/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (110, 7, 1, N'Troca Senha Usuário', N'Troca Senha', N'/Users/ChangePassword/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (131, 43, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/Signatures')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (132, 12, 1, N'Cadastro de UA', N'Novo', N'/AdministrativeUnits/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (133, 12, 1, N'Alteração de UA', N'Editar', N'/AdministrativeUnits/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (134, 2, 1, N'Salvar - Transações', N'Salvar - Transações', N'/Transactions/Save')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (135, 2, 1, N'Salvar - Módulos', N'Salvar - Módulos', N'/Modules/Save')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (136, 2, 1, N'Salvar - Sistemas', N'Salvar - Sistemas', N'/ManagedSystems/Save')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (137, 2, 1, N'Salvar - Transações por Perfil', N'Salvar - Transações por Perfil', N'/RelationshipTransactionsProfiles/Save')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (138, 46, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/MaterialClasses')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (139, 35, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/AuxiliaryAccounts')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (140, 44, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/Addressees')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (141, 42, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/Suppliers')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (142, 45, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/MaterialGroups')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (143, 41, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/AssetOutReasons')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (144, 47, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/Materials')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (145, 33, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/SpendingOrigins')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (146, 38, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/Initials')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (147, 32, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/MaterialSubItems')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (148, 52, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/Assets')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (149, 37, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/OutSourceds')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (150, 40, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/IncorporationTypes')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (151, 39, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/TypesDocumentOut')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (152, 34, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/SupplyUnits')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (153, 43, 1, N'Novo', N'Novo', N'/Signatures/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (154, 46, 1, N'Novo', N'Novo', N'/MaterialClasses/Create')
GO
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (155, 35, 1, N'Novo', N'Novo', N'/AuxiliaryAccounts/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (156, 44, 1, N'Novo', N'Novo', N'/Addressees/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (157, 42, 1, N'Novo', N'Novo', N'/Suppliers/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (158, 45, 1, N'Novo', N'Novo', N'/MaterialGroups/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (159, 41, 1, N'Novo', N'Novo', N'/AssetOutReasons/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (160, 47, 1, N'Novo', N'Novo', N'/Materials/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (161, 33, 1, N'Novo', N'Novo', N'/SpendingOrigins/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (162, 38, 1, N'Novo', N'Novo', N'/Initials/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (163, 32, 1, N'Novo', N'Novo', N'/MaterialSubItems/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (164, 52, 1, N'Novo', N'Novo', N'/Assets/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (165, 37, 1, N'Novo', N'Novo', N'/OutSourceds/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (166, 40, 1, N'Novo', N'Novo', N'/IncorporationTypes/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (167, 39, 1, N'Novo', N'Novo', N'/TypesDocumentOut/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (168, 34, 1, N'Novo', N'Novo', N'/SupplyUnits/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (169, 43, 1, N'Editar', N'Editar', N'/Signatures/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (170, 46, 1, N'Editar', N'Editar', N'/MaterialClasses/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (171, 35, 1, N'Editar', N'Editar', N'/AuxiliaryAccounts/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (172, 44, 1, N'Editar', N'Editar', N'/Addressees/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (173, 42, 1, N'Editar', N'Editar', N'/Suppliers/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (174, 45, 1, N'Editar', N'Editar', N'/MaterialGroups/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (175, 41, 1, N'Editar', N'Editar', N'/AssetOutReasons/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (176, 47, 1, N'Editar', N'Editar', N'/Materials/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (177, 33, 1, N'Editar', N'Editar', N'/SpendingOrigins/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (178, 38, 1, N'Editar', N'Editar', N'/Initials/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (179, 32, 1, N'Editar', N'Editar', N'/MaterialSubItems/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (180, 52, 1, N'Editar', N'Editar', N'/Assets/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (181, 37, 1, N'Editar', N'Editar', N'/OutSourceds/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (182, 40, 1, N'Editar', N'Editar', N'/IncorporationTypes/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (183, 39, 1, N'Editar', N'Editar', N'/TypesDocumentOut/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (184, 34, 1, N'Editar', N'Editar', N'/SupplyUnits/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (185, 43, 1, N'Excluir', N'Excluir', N'/Signatures/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (186, 46, 1, N'Excluir', N'Excluir', N'/MaterialClasses/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (187, 35, 1, N'Excluir', N'Excluir', N'/AuxiliaryAccounts/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (188, 44, 1, N'Excluir', N'Excluir', N'/Addressees/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (189, 42, 1, N'Excluir', N'Excluir', N'/Suppliers/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (190, 45, 1, N'Excluir', N'Excluir', N'/MaterialGroups/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (191, 41, 1, N'Excluir', N'Excluir', N'/AssetOutReasons/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (192, 47, 1, N'Excluir', N'Excluir', N'/Materials/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (193, 33, 1, N'Excluir', N'Excluir', N'/SpendingOrigins/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (194, 38, 1, N'Excluir', N'Excluir', N'/Initials/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (195, 32, 1, N'Excluir', N'Excluir', N'/MaterialSubItems/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (196, 52, 1, N'Excluir', N'Excluir', N'/Assets/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (197, 37, 1, N'Excluir', N'Excluir', N'/OutSourceds/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (198, 40, 1, N'Excluir', N'Excluir', N'/IncorporationTypes/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (199, 39, 1, N'Excluir', N'Excluir', N'/TypesDocumentOut/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (200, 34, 1, N'Excluir', N'Excluir', N'/SupplyUnits/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (201, 43, 1, N'Detalhes', N'Detalhes', N'/Signatures/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (202, 46, 1, N'Detalhes', N'Detalhes', N'/MaterialClasses/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (203, 35, 1, N'Detalhes', N'Detalhes', N'/AuxiliaryAccounts/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (204, 44, 1, N'Detalhes', N'Detalhes', N'/Addressees/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (205, 42, 1, N'Detalhes', N'Detalhes', N'/Suppliers/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (206, 45, 1, N'Detalhes', N'Detalhes', N'/MaterialGroups/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (207, 41, 1, N'Detalhes', N'Detalhes', N'/AssetOutReasons/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (208, 47, 1, N'Detalhes', N'Detalhes', N'/Materials/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (209, 33, 1, N'Detalhes', N'Detalhes', N'/SpendingOrigins/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (210, 38, 1, N'Detalhes', N'Detalhes', N'/Initials/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (211, 32, 1, N'Detalhes', N'Detalhes', N'/MaterialSubItems/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (212, 52, 1, N'Detalhes', N'Detalhes', N'/Assets/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (213, 37, 1, N'Detalhes', N'Detalhes', N'/OutSourceds/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (214, 40, 1, N'Detalhes', N'Detalhes', N'/IncorporationTypes/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (215, 39, 1, N'Detalhes', N'Detalhes', N'/TypesDocumentOut/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (216, 34, 1, N'Detalhes', N'Detalhes', N'/SupplyUnits/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (217, 53, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/Responsibles')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (218, 53, 1, N'Novo', N'Novo', N'/Responsibles/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (219, 53, 1, N'Editar', N'Editar', N'/Responsibles/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (220, 53, 1, N'Excluir', N'Excluir', N'/Responsibles/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (221, 53, 1, N'Detalhes', N'Detalhes', N'/Responsibles/Details/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (222, 48, 1, N'Lista de Cadastrados', N'Lista de Cadastrados', N'/MaterialItems')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (223, 48, 1, N'Novo', N'Novo', N'/MaterialItems/Create')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (224, 48, 1, N'Editar', N'Editar', N'/MaterialItems/Edit/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (225, 48, 1, N'Excluir', N'Excluir', N'/MaterialItems/Delete/')
INSERT [dbo].[Transaction] ([Id], [ModuleId], [Status], [Initial], [Description], [Path]) VALUES (226, 48, 1, N'Detalhes', N'Detalhes', N'/MaterialItems/Details/')
SET IDENTITY_INSERT [dbo].[Transaction] OFF
SET IDENTITY_INSERT [dbo].[TypeDocumentOut] ON 

INSERT [dbo].[TypeDocumentOut] ([Id], [Description], [Status]) VALUES (1, N'Tipo Documento Baixa 01', 1)
INSERT [dbo].[TypeDocumentOut] ([Id], [Description], [Status]) VALUES (2, N'Tipo Documento Baixa 02', 0)
SET IDENTITY_INSERT [dbo].[TypeDocumentOut] OFF
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([Id], [CPF], [Status], [Name], [Email], [Password], [Phrase], [AddedDate], [InvalidAttempts], [Blocked], [ChangePassword], [AddressId]) VALUES (1, N'30778432858', 1, N'Vinicius Nunes Macedo', N'vnmacedo@gmail.com', N'teste', N'Padrão', NULL, NULL, 0, 0, 4)
INSERT [dbo].[User] ([Id], [CPF], [Status], [Name], [Email], [Password], [Phrase], [AddedDate], [InvalidAttempts], [Blocked], [ChangePassword], [AddressId]) VALUES (3, N'27325354860', 1, N'Marcia Cristina Mucheroni', N'mmucheroni@sp.gov.br', N'123', N'teste', CAST(N'2015-06-17 16:50:01.000' AS DateTime), NULL, 0, 1, 9)
INSERT [dbo].[User] ([Id], [CPF], [Status], [Name], [Email], [Password], [Phrase], [AddedDate], [InvalidAttempts], [Blocked], [ChangePassword], [AddressId]) VALUES (4, N'10143333852', 1, N'Marcelo', N'vnmacedo@gmail.com', N'1234', N'Padrão', CAST(N'2015-06-28 21:24:11.000' AS DateTime), NULL, 0, 0, 12)
INSERT [dbo].[User] ([Id], [CPF], [Status], [Name], [Email], [Password], [Phrase], [AddedDate], [InvalidAttempts], [Blocked], [ChangePassword], [AddressId]) VALUES (5, N'64313921834', 0, N'Sonia M Pereira Antonio', N'santonio@saude.sp.gov.br', N'homologacao', N'Homologação', CAST(N'2015-07-23 22:44:57.000' AS DateTime), NULL, 0, 0, 19)
INSERT [dbo].[User] ([Id], [CPF], [Status], [Name], [Email], [Password], [Phrase], [AddedDate], [InvalidAttempts], [Blocked], [ChangePassword], [AddressId]) VALUES (6, N'88888888888', 1, N'V2', N'vnmacedo@gmail.com', N'teste', N'teste', CAST(N'2015-08-12 16:07:31.270' AS DateTime), NULL, 0, 1, 21)
SET IDENTITY_INSERT [dbo].[User] OFF
/****** Object:  Index [PK_AdditionalAssetsOut]    Script Date: 12/08/2015 17:36:40 ******/
ALTER TABLE [dbo].[AdditionalAssetsOut] ADD  CONSTRAINT [PK_AdditionalAssetsOut] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [PK_Addressee]    Script Date: 12/08/2015 17:36:40 ******/
ALTER TABLE [dbo].[Addressee] ADD  CONSTRAINT [PK_Addressee] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [PK_MotivoBaixa]    Script Date: 12/08/2015 17:36:40 ******/
ALTER TABLE [dbo].[AssetOutReason] ADD  CONSTRAINT [PK_MotivoBaixa] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [PK_Baixa]    Script Date: 12/08/2015 17:36:40 ******/
ALTER TABLE [dbo].[AssetsOut] ADD  CONSTRAINT [PK_Baixa] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [PK_ArrolamentoBaixa]    Script Date: 12/08/2015 17:36:40 ******/
ALTER TABLE [dbo].[AssetsOutListing] ADD  CONSTRAINT [PK_ArrolamentoBaixa] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [PK_ELMAH_Error]    Script Date: 12/08/2015 17:36:40 ******/
ALTER TABLE [dbo].[ELMAH_Error] ADD  CONSTRAINT [PK_ELMAH_Error] PRIMARY KEY NONCLUSTERED 
(
	[ErrorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ELMAH_Error_App_Time_Seq]    Script Date: 12/08/2015 17:36:40 ******/
CREATE NONCLUSTERED INDEX [IX_ELMAH_Error_App_Time_Seq] ON [dbo].[ELMAH_Error]
(
	[Application] ASC,
	[TimeUtc] DESC,
	[Sequence] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [PK_TipoIncorp]    Script Date: 12/08/2015 17:36:40 ******/
ALTER TABLE [dbo].[IncorporationType] ADD  CONSTRAINT [PK_TipoIncorp] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [aaaaaSigla_PK]    Script Date: 12/08/2015 17:36:40 ******/
ALTER TABLE [dbo].[Initial] ADD  CONSTRAINT [aaaaaSigla_PK] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [PK_EstadoConservacao]    Script Date: 12/08/2015 17:36:40 ******/
ALTER TABLE [dbo].[StateConservation] ADD  CONSTRAINT [PK_EstadoConservacao] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [PK_Transferencia]    Script Date: 12/08/2015 17:36:40 ******/
ALTER TABLE [dbo].[Transfer] ADD  CONSTRAINT [PK_Transferencia] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [PK_TipoDocumentoBaixa]    Script Date: 12/08/2015 17:36:40 ******/
ALTER TABLE [dbo].[TypeDocumentOut] ADD  CONSTRAINT [PK_TipoDocumentoBaixa] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Usuario]    Script Date: 12/08/2015 17:36:40 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [IX_Usuario] UNIQUE NONCLUSTERED 
(
	[CPF] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Asset] ADD  CONSTRAINT [DF_Asset_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[ELMAH_Error] ADD  CONSTRAINT [DF_ELMAH_Error_ErrorId]  DEFAULT (newid()) FOR [ErrorId]
GO
ALTER TABLE [dbo].[Historic] ADD  DEFAULT (getdate()) FOR [InsertDate]
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
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Destination] FOREIGN KEY([AddresseeId])
REFERENCES [dbo].[Addressee] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] CHECK CONSTRAINT [FK_AdditionalAssetsOut_Destination]
GO
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Historic] FOREIGN KEY([HistoricId])
REFERENCES [dbo].[Historic] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Historic]
GO
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed] FOREIGN KEY([FourthWitnessId])
REFERENCES [dbo].[Signature] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed]
GO
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed1] FOREIGN KEY([ThirdWitnessId])
REFERENCES [dbo].[Signature] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed1]
GO
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed10] FOREIGN KEY([PresidentId])
REFERENCES [dbo].[Signature] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed10]
GO
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed2] FOREIGN KEY([SecondWitnessId])
REFERENCES [dbo].[Signature] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed2]
GO
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed3] FOREIGN KEY([FirstWitnessId])
REFERENCES [dbo].[Signature] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed3]
GO
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed4] FOREIGN KEY([RecordDirectorId])
REFERENCES [dbo].[Signature] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed4]
GO
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed5] FOREIGN KEY([SectionChiefId])
REFERENCES [dbo].[Signature] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed5]
GO
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed6] FOREIGN KEY([FourthMemberID])
REFERENCES [dbo].[Signature] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed6]
GO
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed7] FOREIGN KEY([ThirdMemberId])
REFERENCES [dbo].[Signature] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed7]
GO
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed8] FOREIGN KEY([SecondMemberId])
REFERENCES [dbo].[Signature] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed8]
GO
ALTER TABLE [dbo].[AdditionalAssetsOut]  WITH NOCHECK ADD  CONSTRAINT [FK_AdditionalAssetsOut_Signed9] FOREIGN KEY([FirstMemberId])
REFERENCES [dbo].[Signature] ([Id])
GO
ALTER TABLE [dbo].[AdditionalAssetsOut] NOCHECK CONSTRAINT [FK_AdditionalAssetsOut_Signed9]
GO
ALTER TABLE [dbo].[AdministrativeUnit]  WITH CHECK ADD  CONSTRAINT [FK_AdministrativeUnit_CostCenter] FOREIGN KEY([CostCenterId])
REFERENCES [dbo].[CostCenter] ([Id])
GO
ALTER TABLE [dbo].[AdministrativeUnit] CHECK CONSTRAINT [FK_AdministrativeUnit_CostCenter]
GO
ALTER TABLE [dbo].[AdministrativeUnit]  WITH CHECK ADD  CONSTRAINT [FK_AdministrativeUnit_ManagerUnit] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[AdministrativeUnit] CHECK CONSTRAINT [FK_AdministrativeUnit_ManagerUnit]
GO
ALTER TABLE [dbo].[AdministrativeUnit]  WITH CHECK ADD  CONSTRAINT [FK_AdministrativeUnit_Responsible] FOREIGN KEY([ResponsibleId])
REFERENCES [dbo].[Responsible] ([Id])
GO
ALTER TABLE [dbo].[AdministrativeUnit] CHECK CONSTRAINT [FK_AdministrativeUnit_Responsible]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_AdministrativeUnit] FOREIGN KEY([AdministrativeUnitId])
REFERENCES [dbo].[AdministrativeUnit] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_AdministrativeUnit]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_AuxiliaryAccount] FOREIGN KEY([AuxiliaryAccountId])
REFERENCES [dbo].[AuxiliaryAccount] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_AuxiliaryAccount]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_BudgetUnit] FOREIGN KEY([BudgetUnitId])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_BudgetUnit]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Historic] FOREIGN KEY([HistoricId])
REFERENCES [dbo].[Historic] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_Historic]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_IncorporationType] FOREIGN KEY([TypeIncorporationId])
REFERENCES [dbo].[IncorporationType] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_IncorporationType]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Initial] FOREIGN KEY([InitialId])
REFERENCES [dbo].[Initial] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_Initial]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Institution] FOREIGN KEY([InstitutionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_Institution]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Manager] FOREIGN KEY([ManagerID])
REFERENCES [dbo].[Manager] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_Manager]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_ManagerUnit] FOREIGN KEY([ManagementUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_ManagerUnit]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_MaterialItem] FOREIGN KEY([MaterialItemId])
REFERENCES [dbo].[MaterialItem] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_MaterialItem]
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
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Responsible] FOREIGN KEY([ResponsibleId])
REFERENCES [dbo].[Responsible] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_Responsible]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Section] FOREIGN KEY([SectionId])
REFERENCES [dbo].[Section] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_Section]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_StateConservation] FOREIGN KEY([StateConservationId])
REFERENCES [dbo].[StateConservation] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_StateConservation]
GO
ALTER TABLE [dbo].[Asset]  WITH CHECK ADD  CONSTRAINT [FK_Asset_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[Asset] CHECK CONSTRAINT [FK_Asset_Supplier]
GO
ALTER TABLE [dbo].[AssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_AssetsOut_AdditionalAssetsOut] FOREIGN KEY([AssetsOutComplementId])
REFERENCES [dbo].[AdditionalAssetsOut] ([Id])
GO
ALTER TABLE [dbo].[AssetsOut] CHECK CONSTRAINT [FK_AssetsOut_AdditionalAssetsOut]
GO
ALTER TABLE [dbo].[AssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_Baixa_ArrolamentoBaixa] FOREIGN KEY([AssetsOutListingId])
REFERENCES [dbo].[AssetsOutListing] ([Id])
GO
ALTER TABLE [dbo].[AssetsOut] CHECK CONSTRAINT [FK_Baixa_ArrolamentoBaixa]
GO
ALTER TABLE [dbo].[AssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_Baixa_BemPatrimonial] FOREIGN KEY([AssetId])
REFERENCES [dbo].[Asset] ([Id])
GO
ALTER TABLE [dbo].[AssetsOut] CHECK CONSTRAINT [FK_Baixa_BemPatrimonial]
GO
ALTER TABLE [dbo].[AssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_Baixa_MotivoBaixa] FOREIGN KEY([TypeOutId])
REFERENCES [dbo].[AssetOutReason] ([Id])
GO
ALTER TABLE [dbo].[AssetsOut] CHECK CONSTRAINT [FK_Baixa_MotivoBaixa]
GO
ALTER TABLE [dbo].[AssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_Baixa_Sigla] FOREIGN KEY([InitialId])
REFERENCES [dbo].[Initial] ([Id])
GO
ALTER TABLE [dbo].[AssetsOut] CHECK CONSTRAINT [FK_Baixa_Sigla]
GO
ALTER TABLE [dbo].[AssetsOut]  WITH CHECK ADD  CONSTRAINT [FK_Baixa_TipoDocumentoBaixa] FOREIGN KEY([TypeDocumentOutId])
REFERENCES [dbo].[TypeDocumentOut] ([Id])
GO
ALTER TABLE [dbo].[AssetsOut] CHECK CONSTRAINT [FK_Baixa_TipoDocumentoBaixa]
GO
ALTER TABLE [dbo].[AuxiliaryAccount]  WITH CHECK ADD  CONSTRAINT [FK_ContaAuxiliar_Gestor] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Manager] ([Id])
GO
ALTER TABLE [dbo].[AuxiliaryAccount] CHECK CONSTRAINT [FK_ContaAuxiliar_Gestor]
GO
ALTER TABLE [dbo].[BudgetUnit]  WITH CHECK ADD  CONSTRAINT [FK_UO_Gestor] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Manager] ([Id])
GO
ALTER TABLE [dbo].[BudgetUnit] CHECK CONSTRAINT [FK_UO_Gestor]
GO
ALTER TABLE [dbo].[Closing]  WITH CHECK ADD  CONSTRAINT [FK_Fechamento_ItemMaterial] FOREIGN KEY([MaterialItemId])
REFERENCES [dbo].[MaterialItem] ([Id])
GO
ALTER TABLE [dbo].[Closing] CHECK CONSTRAINT [FK_Fechamento_ItemMaterial]
GO
ALTER TABLE [dbo].[Closing]  WITH CHECK ADD  CONSTRAINT [FK_Fechamento_UGE] FOREIGN KEY([ManagmentUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[Closing] CHECK CONSTRAINT [FK_Fechamento_UGE]
GO
ALTER TABLE [dbo].[Configuration]  WITH CHECK ADD  CONSTRAINT [FK_Configuracoes_Gestor] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Manager] ([Id])
GO
ALTER TABLE [dbo].[Configuration] CHECK CONSTRAINT [FK_Configuracoes_Gestor]
GO
ALTER TABLE [dbo].[Configuration]  WITH CHECK ADD  CONSTRAINT [FK_Configuracoes_Orgao] FOREIGN KEY([InstitutionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[Configuration] CHECK CONSTRAINT [FK_Configuracoes_Orgao]
GO
ALTER TABLE [dbo].[Configuration]  WITH CHECK ADD  CONSTRAINT [FK_Configuracoes_UGE] FOREIGN KEY([ManagmentUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[Configuration] CHECK CONSTRAINT [FK_Configuracoes_UGE]
GO
ALTER TABLE [dbo].[Configuration]  WITH CHECK ADD  CONSTRAINT [FK_Configuracoes_UO] FOREIGN KEY([BudgetUnitId])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[Configuration] CHECK CONSTRAINT [FK_Configuracoes_UO]
GO
ALTER TABLE [dbo].[CostCenter]  WITH CHECK ADD  CONSTRAINT [FK_CentroCusto_Gestor] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Manager] ([Id])
GO
ALTER TABLE [dbo].[CostCenter] CHECK CONSTRAINT [FK_CentroCusto_Gestor]
GO
ALTER TABLE [dbo].[Historic]  WITH CHECK ADD  CONSTRAINT [FK_Historico_Usuario] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Historic] CHECK CONSTRAINT [FK_Historico_Usuario]
GO
ALTER TABLE [dbo].[Initial]  WITH CHECK ADD  CONSTRAINT [FK_Sigla_Gestor] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Manager] ([Id])
GO
ALTER TABLE [dbo].[Initial] CHECK CONSTRAINT [FK_Sigla_Gestor]
GO
ALTER TABLE [dbo].[Level]  WITH CHECK ADD FOREIGN KEY([ParentId])
REFERENCES [dbo].[Level] ([Id])
GO
ALTER TABLE [dbo].[Manager]  WITH CHECK ADD  CONSTRAINT [FK_Gestor_Endereco] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[Manager] CHECK CONSTRAINT [FK_Gestor_Endereco]
GO
ALTER TABLE [dbo].[Manager]  WITH CHECK ADD  CONSTRAINT [FK_Gestor_Orgao] FOREIGN KEY([InstitutionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[Manager] CHECK CONSTRAINT [FK_Gestor_Orgao]
GO
ALTER TABLE [dbo].[ManagerUnit]  WITH CHECK ADD  CONSTRAINT [FK_UGE_UO1] FOREIGN KEY([BudgetUnitId])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[ManagerUnit] CHECK CONSTRAINT [FK_UGE_UO1]
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
ALTER TABLE [dbo].[OutSourced]  WITH CHECK ADD  CONSTRAINT [FK_Terceiro_Endereco] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[OutSourced] CHECK CONSTRAINT [FK_Terceiro_Endereco]
GO
ALTER TABLE [dbo].[RelationshipAdministrativeUnitSection]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipAdministrativeUnitSection_RelationshipManagerUnitAdministrativeUnit] FOREIGN KEY([RelationshipManagerUnitAdministrativeUnit])
REFERENCES [dbo].[RelationshipManagerUnitAdministrativeUnit] ([Id])
GO
ALTER TABLE [dbo].[RelationshipAdministrativeUnitSection] CHECK CONSTRAINT [FK_RelationshipAdministrativeUnitSection_RelationshipManagerUnitAdministrativeUnit]
GO
ALTER TABLE [dbo].[RelationshipAdministrativeUnitSection]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipAdministrativeUnitSection_Section] FOREIGN KEY([SectionId])
REFERENCES [dbo].[Section] ([Id])
GO
ALTER TABLE [dbo].[RelationshipAdministrativeUnitSection] CHECK CONSTRAINT [FK_RelationshipAdministrativeUnitSection_Section]
GO
ALTER TABLE [dbo].[RelationshipBudgetUnitManagerUnit]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipBudgetUnitManagerUnit_ManagerUnit] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[RelationshipBudgetUnitManagerUnit] CHECK CONSTRAINT [FK_RelationshipBudgetUnitManagerUnit_ManagerUnit]
GO
ALTER TABLE [dbo].[RelationshipBudgetUnitManagerUnit]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipBudgetUnitManagerUnit_RelationshipManagerBudgetUnit] FOREIGN KEY([RelationshipManagerBudgetUnitId])
REFERENCES [dbo].[RelationshipManagerBudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[RelationshipBudgetUnitManagerUnit] CHECK CONSTRAINT [FK_RelationshipBudgetUnitManagerUnit_RelationshipManagerBudgetUnit]
GO
ALTER TABLE [dbo].[RelationshipInstitutionManager]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipInstitutionManager_Manager] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Manager] ([Id])
GO
ALTER TABLE [dbo].[RelationshipInstitutionManager] CHECK CONSTRAINT [FK_RelationshipInstitutionManager_Manager]
GO
ALTER TABLE [dbo].[RelationshipInstitutionManager]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipInstitutionManager_RelationshipUserProfileInstitution] FOREIGN KEY([RelationshipUserProfileInstitutionId])
REFERENCES [dbo].[RelationshipUserProfileInstitution] ([Id])
GO
ALTER TABLE [dbo].[RelationshipInstitutionManager] CHECK CONSTRAINT [FK_RelationshipInstitutionManager_RelationshipUserProfileInstitution]
GO
ALTER TABLE [dbo].[RelationshipManagerBudgetUnit]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipManagerBudgetUnit_BudgetUnit] FOREIGN KEY([BudgetUnitId])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[RelationshipManagerBudgetUnit] CHECK CONSTRAINT [FK_RelationshipManagerBudgetUnit_BudgetUnit]
GO
ALTER TABLE [dbo].[RelationshipManagerBudgetUnit]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipManagerBudgetUnit_RelationshipInstitutionManager] FOREIGN KEY([RelationshipInstitutionManagerId])
REFERENCES [dbo].[RelationshipInstitutionManager] ([Id])
GO
ALTER TABLE [dbo].[RelationshipManagerBudgetUnit] CHECK CONSTRAINT [FK_RelationshipManagerBudgetUnit_RelationshipInstitutionManager]
GO
ALTER TABLE [dbo].[RelationshipManagerUnitAdministrativeUnit]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipManagerUnitAdministrativeUnit_AdministrativeUnit] FOREIGN KEY([AdministrativeUnitId])
REFERENCES [dbo].[AdministrativeUnit] ([Id])
GO
ALTER TABLE [dbo].[RelationshipManagerUnitAdministrativeUnit] CHECK CONSTRAINT [FK_RelationshipManagerUnitAdministrativeUnit_AdministrativeUnit]
GO
ALTER TABLE [dbo].[RelationshipManagerUnitAdministrativeUnit]  WITH CHECK ADD  CONSTRAINT [FK_RelationshipManagerUnitAdministrativeUnit_RelationshipBudgetUnitManagerUnit] FOREIGN KEY([RelationshipBudgetUnitManagerUnitId])
REFERENCES [dbo].[RelationshipBudgetUnitManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[RelationshipManagerUnitAdministrativeUnit] CHECK CONSTRAINT [FK_RelationshipManagerUnitAdministrativeUnit_RelationshipBudgetUnitManagerUnit]
GO
ALTER TABLE [dbo].[RelationshipProfileLevel]  WITH CHECK ADD FOREIGN KEY([LevelId])
REFERENCES [dbo].[Level] ([Id])
GO
ALTER TABLE [dbo].[RelationshipProfileLevel]  WITH CHECK ADD  CONSTRAINT [FK_RelPerfilNivel_Perfil] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profile] ([Id])
GO
ALTER TABLE [dbo].[RelationshipProfileLevel] CHECK CONSTRAINT [FK_RelPerfilNivel_Perfil]
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
ALTER TABLE [dbo].[RelationshipUserProfileInstitution]  WITH CHECK ADD  CONSTRAINT [FK_RelUsuarioPerfilOrgao_Orgao] FOREIGN KEY([InstitutionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[RelationshipUserProfileInstitution] CHECK CONSTRAINT [FK_RelUsuarioPerfilOrgao_Orgao]
GO
ALTER TABLE [dbo].[RelationshipUserProfileInstitution]  WITH CHECK ADD  CONSTRAINT [FK_RelUsuarioPerfilOrgao_RelUsuarioPerfil] FOREIGN KEY([RelationshipUserProfileId])
REFERENCES [dbo].[RelationshipUserProfile] ([Id])
GO
ALTER TABLE [dbo].[RelationshipUserProfileInstitution] CHECK CONSTRAINT [FK_RelUsuarioPerfilOrgao_RelUsuarioPerfil]
GO
ALTER TABLE [dbo].[Responsible]  WITH CHECK ADD  CONSTRAINT [FK_Responsavel_Gestor] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Manager] ([Id])
GO
ALTER TABLE [dbo].[Responsible] CHECK CONSTRAINT [FK_Responsavel_Gestor]
GO
ALTER TABLE [dbo].[Responsible]  WITH CHECK ADD  CONSTRAINT [FK_Responsavel_Usuario] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Responsible] CHECK CONSTRAINT [FK_Responsavel_Usuario]
GO
ALTER TABLE [dbo].[Section]  WITH CHECK ADD  CONSTRAINT [FK_Divisao_Endereco] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[Section] CHECK CONSTRAINT [FK_Divisao_Endereco]
GO
ALTER TABLE [dbo].[Section]  WITH CHECK ADD  CONSTRAINT [FK_Divisao_UA] FOREIGN KEY([AdministrativeUnitId])
REFERENCES [dbo].[AdministrativeUnit] ([Id])
GO
ALTER TABLE [dbo].[Section] CHECK CONSTRAINT [FK_Divisao_UA]
GO
ALTER TABLE [dbo].[Supplier]  WITH CHECK ADD  CONSTRAINT [FK_Fornecedor_Endereco] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[Supplier] CHECK CONSTRAINT [FK_Fornecedor_Endereco]
GO
ALTER TABLE [dbo].[SupplyUnit]  WITH CHECK ADD  CONSTRAINT [FK_UnidadeFornecimento_Gestor] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Manager] ([Id])
GO
ALTER TABLE [dbo].[SupplyUnit] CHECK CONSTRAINT [FK_UnidadeFornecimento_Gestor]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Institution] FOREIGN KEY([InstitutionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_Institution]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_ManagedSystem] FOREIGN KEY([ManagedSystemId])
REFERENCES [dbo].[ManagedSystem] ([Id])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_ManagedSystem]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_TicketStatus] FOREIGN KEY([TicketStatusId])
REFERENCES [dbo].[TicketStatus] ([Id])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_TicketStatus]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_TicketType] FOREIGN KEY([TicketTypeId])
REFERENCES [dbo].[TicketType] ([Id])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_TicketType]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_User]
GO
ALTER TABLE [dbo].[TicketHistory]  WITH CHECK ADD  CONSTRAINT [FK_TicketHistory_Ticket] FOREIGN KEY([TicketId])
REFERENCES [dbo].[Ticket] ([Id])
GO
ALTER TABLE [dbo].[TicketHistory] CHECK CONSTRAINT [FK_TicketHistory_Ticket]
GO
ALTER TABLE [dbo].[TicketHistory]  WITH CHECK ADD  CONSTRAINT [FK_TicketHistory_TicketStatus] FOREIGN KEY([TicketStatusId])
REFERENCES [dbo].[TicketStatus] ([Id])
GO
ALTER TABLE [dbo].[TicketHistory] CHECK CONSTRAINT [FK_TicketHistory_TicketStatus]
GO
ALTER TABLE [dbo].[TicketHistory]  WITH CHECK ADD  CONSTRAINT [FK_TicketHistory_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[TicketHistory] CHECK CONSTRAINT [FK_TicketHistory_User]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transacao__Modulo] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Module] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transacao__Modulo]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_ContaAuxiliar] FOREIGN KEY([AuxiliaryAccountId])
REFERENCES [dbo].[AuxiliaryAccount] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_ContaAuxiliar]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_ContaAuxiliar1] FOREIGN KEY([DestinationAuxiliaryAccountId])
REFERENCES [dbo].[AuxiliaryAccount] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_ContaAuxiliar1]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_Divisao] FOREIGN KEY([SectionId])
REFERENCES [dbo].[Section] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_Divisao]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_Divisao1] FOREIGN KEY([DestinationSectionId])
REFERENCES [dbo].[Section] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_Divisao1]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_EstadoConservacao] FOREIGN KEY([StateConservationId])
REFERENCES [dbo].[StateConservation] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_EstadoConservacao]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_Fornecedor] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_Fornecedor]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_Gestor] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Manager] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_Gestor]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_Gestor1] FOREIGN KEY([DestinationManagerId])
REFERENCES [dbo].[Manager] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_Gestor1]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_Historico] FOREIGN KEY([HistoricId])
REFERENCES [dbo].[Historic] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_Historico]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_ItemMaterial] FOREIGN KEY([MaterialItemId])
REFERENCES [dbo].[MaterialItem] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_ItemMaterial]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_Orgao] FOREIGN KEY([InstitutionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_Orgao]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_Orgao1] FOREIGN KEY([DestinationInstituitionId])
REFERENCES [dbo].[Institution] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_Orgao1]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_Responsavel] FOREIGN KEY([ResponsibleId])
REFERENCES [dbo].[Responsible] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_Responsavel]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_Responsavel1] FOREIGN KEY([DestinationResponsibleId])
REFERENCES [dbo].[Responsible] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_Responsavel1]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_Sigla] FOREIGN KEY([InitialId])
REFERENCES [dbo].[Initial] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_Sigla]
GO
ALTER TABLE [dbo].[Transfer]  WITH NOCHECK ADD  CONSTRAINT [FK_Transferencia_Terceiro] FOREIGN KEY([OutSourcedId])
REFERENCES [dbo].[OutSourced] ([Id])
GO
ALTER TABLE [dbo].[Transfer] NOCHECK CONSTRAINT [FK_Transferencia_Terceiro]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_TipoIncorporacao] FOREIGN KEY([TypeIncorporationId])
REFERENCES [dbo].[IncorporationType] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_TipoIncorporacao]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_TipoMovimento] FOREIGN KEY([MovementTypeId])
REFERENCES [dbo].[MovementType] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_TipoMovimento]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_UA] FOREIGN KEY([AdministrativeUnitId])
REFERENCES [dbo].[AdministrativeUnit] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_UA]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_UA1] FOREIGN KEY([DestinationAdministrativeUnitId])
REFERENCES [dbo].[AdministrativeUnit] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_UA1]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_UGE] FOREIGN KEY([ManagerUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_UGE]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_UGE1] FOREIGN KEY([DestinationManagmentUnitId])
REFERENCES [dbo].[ManagerUnit] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_UGE1]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_UO] FOREIGN KEY([BudgetUnitId])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_UO]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_UO1] FOREIGN KEY([DestinationBudgetUnitId])
REFERENCES [dbo].[BudgetUnit] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_UO1]
GO
ALTER TABLE [dbo].[Transfer]  WITH CHECK ADD  CONSTRAINT [FK_Transferencia_Usuario] FOREIGN KEY([DestinationUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Transfer] CHECK CONSTRAINT [FK_Transferencia_Usuario]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_Usuario_Endereco] FOREIGN KEY([AddressId])
REFERENCES [dbo].[Address] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_Usuario_Endereco]
GO
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorsXml]    Script Date: 12/08/2015 17:36:40 ******/
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
/****** Object:  StoredProcedure [dbo].[ELMAH_GetErrorXml]    Script Date: 12/08/2015 17:36:40 ******/
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
/****** Object:  StoredProcedure [dbo].[ELMAH_LogError]    Script Date: 12/08/2015 17:36:40 ******/
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
USE [master]
GO
ALTER DATABASE [SAM] SET  READ_WRITE 
GO
