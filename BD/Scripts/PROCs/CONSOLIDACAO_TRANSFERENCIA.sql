DROP PROCEDURE CONSOLIDACAO_TRANSFERENCIA
GO

--CREATE PROCEDURE [dbo].[CONSOLIDACAO_TRANSFERENCIA]

--AS
--BEGIN 
--	BEGIN TRY
--    BEGIN TRANSACTION
    

	 
--DECLARE @retorno int

--SELECT @retorno = COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS
--WHERE TABLE_NAME = 'Asset' AND  COLUMN_NAME = 'Recalculo'

----SELECT @retorno

--IF (@retorno = 0)
--BEGIN
--      ALTER TABLE Asset ADD Recalculo BIT NULL; 
--END


	 
--DECLARE @retorno2 int

--SELECT @retorno2 = COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS
--WHERE TABLE_NAME = 'Closing' AND  COLUMN_NAME = 'Recalculo'

----SELECT @retorno2

--IF (@retorno2 = 0)
--BEGIN
--      ALTER TABLE Closing ADD Recalculo BIT NULL; 
--END    --
   			
--			SELECT ROW_NUMBER() OVER(ORDER BY A.AssetTransferenciaId ASC) AS Contador, a.AssetId ID_origem, 
--			A.AssetTransferenciaId ID_destino,SourceDestiny_ManagerUnitId DESTINO, B.ManagerUnitId ORIGEM,
--			'' LINHA INTO #tempTRANSF FROM AssetMovements A
--			INNER JOIN Asset B ON B.ID = A.AssetId
--			WHERE AssetTransferenciaId IS NOT NULL 
--			ORDER BY A.AssetTransferenciaId

--			update Asset set flagVerificado = null   , flagDepreciaAcumulada = null 
--			where ID IN (SELECT ID_origem FROM #tempTRANSF)


--			update Asset set flagVerificado = null   , flagDepreciaAcumulada = null 
--			where ID IN (SELECT ID_destino FROM #tempTRANSF)


--			  DECLARE @Counter INT = 1;
--			  DECLARE @TotalLoops INT = (select count(1) from #tempTRANSF)
--			  DECLARE @DESTINO INT;
--			  DECLARE @ORIGEM INT;
--			  DECLARE @ID_DESTINO INT;
--			  DECLARE @ID_ORIGEM INT;

--			   DECLARE @ManagerUnitId INT;


            
--			 WHILE (@Counter <= @TotalLoops)
--				BEGIN


--					SELECT @DESTINO= DESTINO, @ORIGEM = ORIGEM,@ID_DESTINO= ID_DESTINO, @ID_ORIGEM = ID_ORIGEM FROM #tempTRANSF
--					WHERE  CONTADOR =  @Counter 

--					UPDATE #tempTRANSF
--					SET LINHA ='1'
--					WHERE CONTADOR = @Counter

--					Exec CONSOLIDACAO_IMPORTACAO_UGE_IDASSET @AssetId = @ID_ORIGEM, @ManagerUnitId = @ORIGEM
				
--					EXEC FECHAMENTO_RECALCULO_ASSETID @AssetId = @ID_ORIGEM
		
--					Exec CONSOLIDACAO_IMPORTACAO_UGE_IDASSET @AssetId = @ID_DESTINO, @ManagerUnitId = @DESTINO
		
--					EXEC FECHAMENTO_RECALCULO_ASSETID  @AssetId = @ID_DESTINO




--				SET @Counter = @Counter+1

--				END

--		DROP TABLE #tempTRANSF

--		select 'ok'
--	--
--		COMMIT TRANSACTION
--	END TRY
--	BEGIN CATCH        
--		IF @@TRANCOUNT > 0
--		BEGIN
--			ROLLBACK TRANSACTION 
--		END
		
--		-- Raise an error with the details of the exception
--		DECLARE @ErrMsg nvarchar(4000), @ErrSeverity int
--		SELECT @ErrMsg = ERROR_MESSAGE(),
--			 @ErrSeverity = ERROR_SEVERITY()

--		RAISERROR(@ErrMsg, @ErrSeverity, 1)
--	END CATCH
	
--	RETURN @@ERROR
--END
