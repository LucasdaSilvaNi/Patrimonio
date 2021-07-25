DROP PROCEDURE CONSOLIDACAO
GO

--CREATE PROCEDURE [dbo].[CONSOLIDACAO]

--@CODINSTITUTION int

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
--END

--    --
   			
--			SELECT ROW_NUMBER() OVER(ORDER BY A.AssetTransferenciaId ASC) AS Contador, a.AssetId ID_origem, 
--			A.AssetTransferenciaId ID_destino,SourceDestiny_ManagerUnitId DESTINO, B.ManagerUnitId ORIGEM,
--			'' LINHA INTO #tempTRANSF_OK FROM AssetMovements A
--			INNER JOIN Asset B ON B.ID = A.AssetId
--			WHERE AssetTransferenciaId IS NOT NULL 
--			ORDER BY A.AssetTransferenciaId

--			SELECT DISTINCT ROW_NUMBER() OVER(ORDER BY A.ID ASC) AS Contador, A.Code, A.ID INTO #tempTRANSF FROM ManagerUnit A
--			INNER JOIN BudgetUnit B ON B.Id  = A.BudgetUnitId
--			INNER JOIN Institution C ON C.ID = B.InstitutionId
--			WHERE C.Code =  @CODINSTITUTION





--			update Asset set flagVerificado = null   , flagDepreciaAcumulada = null 
--			where ManagerUnitId  IN (SELECT ID FROM #tempTRANSF) AND ID NOT IN (SELECT ID_origem FROM #tempTRANSF_OK) 
--			AND ID NOT IN (SELECT ID_destino FROM #tempTRANSF_OK) 



--			  DECLARE @Counter INT = 1;
--			  DECLARE @TotalLoops INT = (select count(1) from #tempTRANSF)
--			  DECLARE @Code INT;
			 

--			   DECLARE @ManagerUnitId INT;


            
--			 WHILE (@Counter <= @TotalLoops)
--				BEGIN


--					SELECT @Code = Code FROM #tempTRANSF
--					WHERE  CONTADOR =  @Counter 

				

--					Exec CONSOLIDACAO_IMPORTACAO_UGE  @CodManagerUnit = @Code
				
--					EXEC FECHAMENTO_RECALCULO @CodManagerUnit = @Code
	


--				SET @Counter = @Counter+1

--				END

--		DROP TABLE #tempTRANSF
--		DROP TABLE #tempTRANSF_OK
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