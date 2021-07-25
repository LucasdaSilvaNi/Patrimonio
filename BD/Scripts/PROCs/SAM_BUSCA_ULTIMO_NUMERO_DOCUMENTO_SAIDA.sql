SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SAM_BUSCA_ULTIMO_NUMERO_DOCUMENTO_SAIDA')
	DROP PROCEDURE [dbo].[SAM_BUSCA_ULTIMO_NUMERO_DOCUMENTO_SAIDA]  
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

GRANT EXECUTE ON [dbo].[SAM_BUSCA_ULTIMO_NUMERO_DOCUMENTO_SAIDA] TO [ususam]
GO