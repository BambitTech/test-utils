SET ANSI_NULLS, QUOTED_IDENTIFIER ON;
GO
drop table if exists [test].[testTableNullable];
drop table if exists [test].[testTableNotNullable];
drop table if exists [test].[TestTableAlpha];
drop table if exists [Test].[TestTableBigInt];
drop table if exists [Test].[TestTableEpsilon];
drop table if exists [Test].[TestTableForeignKeyHolder];
drop table if exists [Test].[TestTableForeignKeyReference];
drop SCHEMA if exists [test];

go
create schema [Test];
go
create table [Test].[TestTableForeignKeyReference] (
   id int not null primary key
)
go
create table  [Test].[TestTableForeignKeyHolder] (
   id int not null,
   fkid int not null
)
alter table  [Test].[TestTableForeignKeyHolder]
   add constraint FK_TestTableForeignKeyHolder_TestTableForeignKeyReference_fkid foreign key (fkid)
      references [Test].[TestTableForeignKeyReference]  (id)
go 

create table [test].[testTableNullable] (
   bigIntField bigint,
   TimeStampField timestamp,
   imageField image,
   binaryField binary,	
   varbinaryField varbinary,
   bitField bit,
   dateField date,
   dateTimeField datetime,
   dateTime2Field datetime2,
   smallDateTimeField smallDateTime,
   dateTimeOffsetField datetimeoffset,
   moneyField money,
   numericField numeric,
   smallMoneyField smallmoney,
   decimalField decimal(6,2),
   floatField float,
   intField int,
   ncharField nchar(255),
   ntextField ntext,
   nvarcharField nvarchar(255),
   charField char(255),
   textField text,
   varcharField varchar(255),
   xmlField xml,
   smallIntField smallint,
   timeField time,
   tinyintfield tinyint,
   uniqueidentifierField uniqueidentifier
)
GO 
create table [test].[testTableNotNullable] (
   bigIntField bigint not null,
   TimeStampField timestamp not null,
   imageField image not null,
   binaryField binary not null,
   varbinaryField varbinary not null,
   bitField bit not null,
   dateField date not null,
   dateTimeField datetime not null,
   dateTime2Field datetime2 not null,
   smallDateTimeField smallDateTime not null,
   dateTimeOffsetField datetimeoffset not null,
   moneyField money not null,
   numericField numeric not null,
   smallMoneyField smallmoney not null,
   decimalField decimal(6, 2) not null,
   floatField float not null,
   intField int not null,
   ncharField nchar(255) not null,
   ntextField ntext not null,
   nvarcharField nvarchar(255) not null,
   charField char(255) not null,
   textField text not null,
   varcharField varchar(255) not null,
   xmlField xml not null,
   smallIntField smallint not null,
   timeField time not null,
   tinyintfield tinyint not null,
   uniqueidentifierField uniqueidentifier not null
)

go

CREATE TABLE [test].[TestTableAlpha]
(
	[TestTableAlphaId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NULL, 
    [DateOfBirth] DATETIME NULL, 
    [Score] INT NULL,
    [CheckSmall] smallint null,
    [CheckTiny] tinyint null,
    [MyComputed] as (isnull(Score,100)+10) persisted,
    [MadMummyMoney] money,
    [TightDecimal] decimal(5,2),
    [NullableBit] bit,
)


GO
CREATE TABLE [Test].[TestTableBigInt]
(
	[Id] INT NOT NULL PRIMARY KEY,
	Big Bigint NULL
)



CREATE TABLE [Test].[TestTableEpsilon]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Name] nvarchar(255) unique,
)
GO
Create unique index UIX_dbo_TestTableEpsilon_Name on [Test].[TestTableEpsilon] (Name)