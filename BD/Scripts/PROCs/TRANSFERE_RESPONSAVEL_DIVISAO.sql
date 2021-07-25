SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS(SELECT 1 FROM sys.procedures 
          WHERE Name = 'TRANSFERE_RESPONSAVEL_DIVISAO')
BEGIN
    DROP PROCEDURE [dbo].[TRANSFERE_RESPONSAVEL_DIVISAO]
END
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