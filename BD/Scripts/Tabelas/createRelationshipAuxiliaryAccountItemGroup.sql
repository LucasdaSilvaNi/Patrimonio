Create Table RelationshipAuxiliaryAccountItemGroup
(
 Id int not null  primary key identity (1,1), 
 MaterialGroupId  int Foreign Key References  MaterialGroup(Id),
 AuxiliaryAccountId int Foreign Key References AuxiliaryAccount(Id)
);

Alter Table RelationshipAuxiliaryAccountItemGroup
Add Constraint UC_RelationshipAuxiliaryAccountItemGroup UNIQUE (MaterialGroupId, AuxiliaryAccountId);