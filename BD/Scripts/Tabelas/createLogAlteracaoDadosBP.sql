IF Not Exists (Select * From sysobjects Where name ='LogAlteracaoDadosBP' And xtype='U')
BEGIN

create table [dbo].[LogAlteracaoDadosBP] (
	Id int primary key identity(1,1),
	AssetId int not null,
	AssetMovementId int null,
	Campo varchar(30),
	ValorAntigo varchar(30),
	ValorNovo varchar(30),
	UserId int NOT NULL,
	DataHora datetime NOT NULL,
)

alter table [dbo].[LogAlteracaoDadosBP] 
WITH CHECK ADD CONSTRAINT [FK_LogAlteracaoDadosBP_Asset] FOREIGN KEY (AssetId)
REFERENCES [dbo].[Asset]([Id])

alter table [dbo].[LogAlteracaoDadosBP] 
WITH CHECK ADD CONSTRAINT [FK_LogAlteracaoDadosBP_AssetMovements] FOREIGN KEY (AssetMovementId)
REFERENCES [dbo].[AssetMovements]([Id])

alter table [dbo].[LogAlteracaoDadosBP] 
WITH CHECK ADD CONSTRAINT [FK_LogAlteracaoDadosBP_User] FOREIGN KEY (UserId)
REFERENCES [dbo].[User]([Id])

Grant Select, Insert, Update, Delete On [dbo].[LogAlteracaoDadosBP] To [ususam]

END