IF Not Exists (Select * From sysobjects Where name ='UGEDepreciaramAbrilDoisMilVinte' And xtype='U')
BEGIN

create table UGEDepreciaramAbrilDoisMilVinte(
	Id int primary key identity(1,1),
	ManagerUnitId int
);

ALTER TABLE [dbo].[UGEDepreciaramAbrilDoisMilVinte]  WITH CHECK ADD  CONSTRAINT [FK_UGEDepreciaramAbrilDoisMilVinte_ManagerUnit] FOREIGN KEY([ManagerUnitId])
	REFERENCES [dbo].[ManagerUnit] ([Id]);

Grant Select, Insert, Update, Delete On [dbo].[UGEDepreciaramAbrilDoisMilVinte] To [ususam];

END