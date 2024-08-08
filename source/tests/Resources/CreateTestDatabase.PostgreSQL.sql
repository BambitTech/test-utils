
drop table if exists test.testTableNullable;
drop table if exists test.testTableNotNullable;
drop table if exists test.TestTableAlpha;
drop table if exists Test.TestTableBigInt;
drop table if exists Test.TestTableEpsilon;
drop SCHEMA if exists test;


create schema test;

create table test.testTableNullable (
   bigIntField bigint,
   TimeStampField timestamp,
   binaryField bytea,	
   bitField bit,
   dateField date,
   dateTimeField timestamp ,
   timeField time ,
   moneyField money,
   numericField numeric,
   decimalField decimal(6,2),
   floatField float,
   intField int,
   ncharField nchar(255),
   charField char(255),
   textField text,
   varcharField varchar(255),
   xmlField xml,
   smallIntField smallint,
   booleanField boolean,
   uniqueidentifierField uuid
)
;
create table test.testTableNotNullable (
   bigIntField bigint not null ,
   TimeStampField timestamp not null,
   binaryField bytea not null,	
   bitField bit not null,
   dateField date not null,
   dateTimeField timestamp  not null,
   timeField time  not null,
   moneyField money not null,
   numericField numeric not null,
   decimalField decimal(6,2) not null,
   floatField float not null,
   intField int not null,
   ncharField nchar(255) not null,
   charField char(255) not null,
   textField text not null,
   varcharField varchar(255) not null,
   xmlField xml not null,
   smallIntField smallint not null,
   booleanField boolean not null,
   uniqueidentifierField uuid not null
)

;

CREATE TABLE test.TestTableAlpha
(
	TestTableAlphaId uuid NOT NULL PRIMARY KEY, 
    Name VARCHAR(50) NULL, 
    DateOfBirth timestamp NULL, 
    Score INT NULL,
    CheckSmall smallint null,
    CheckTiny char  null,
    MyComputed INT GENERATED ALWAYS AS  (coalesce(Score,100)+10) STORED,
    MadMummyMoney money,
    TightDecimal decimal(5,2),
    NullableBit bit
)


;
CREATE TABLE Test.TestTableBigInt
(
	Id INT NOT NULL PRIMARY KEY,
	Big Bigint NULL
)
;


CREATE TABLE Test.TestTableEpsilon
(
	Id INT NOT NULL PRIMARY KEY,
	Name varchar(255) unique
)
;
Create unique index UIX_dbo_TestTableEpsilon_Name on Test.TestTableEpsilon (Name)