If Exists (Select * From sysobjects Where name ='SiafemCalendar' And xtype='U')
	Drop Table SiafemCalendar;

IF Not Exists (Select * From sysobjects Where name ='SiafemCalendar' And xtype='U')
Begin
	Create Table SiafemCalendar
	(
	Id int primary key identity (1,1) not null,
	FiscalYear int not null,
	ReferenceMonth int unique not null,
	DateClosing date not null,
	Status bit not null
    )
End

Grant Select, Insert, Update, Delete On [dbo].[SiafemCalendar] To [ususamweb]

