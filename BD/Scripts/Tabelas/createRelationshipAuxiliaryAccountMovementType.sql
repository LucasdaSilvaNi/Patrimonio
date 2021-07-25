Create Table RelationshipAuxiliaryAccountMovementType
(
	Id int not null  primary key identity (1,1), 
	AuxiliaryAccountId int Constraint FK_RelationshipAuxiliaryAccountMovementType_AuxiliaryAccount Foreign Key References AuxiliaryAccount(Id),
	MovementTypeId int Constraint FK_RelationshipAuxiliaryAccountMovementType_MovementType Foreign Key References MovementType(Id)
);

Alter Table RelationshipAuxiliaryAccountMovementType
Add Constraint UC_RelationshipAuxiliaryAccountMovementType UNIQUE (MovementTypeId, AuxiliaryAccountId);