SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'SAM_CALCULA_DEPRECIACAO_CARGA_DE_DADOS')
BEGIN
    DROP PROCEDURE [dbo].[SAM_CALCULA_DEPRECIACAO_CARGA_DE_DADOS]
END
GO

CREATE PROCEDURE [SAM_CALCULA_DEPRECIACAO_CARGA_DE_DADOS]
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

GRANT EXECUTE ON [dbo].[SAM_CALCULA_DEPRECIACAO_CARGA_DE_DADOS] TO [ususam]
GO