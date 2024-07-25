# Bambit.TestUtility.DatabaseTools

Provides a set of tools for interfacing with a database for test purposes.  Includes class generation from tables and verifying data.

## Getting started

Install:
```shell
PM> Install-Package Bambit.TestUtility.DatabaseTools

```



## Usage

For the following examples, we are going to use a SqlServer connection, with a database named ***TestDb***
We'll use a single table defined as:

```sql
create table dbo.[SampleTest](
   [ID] int not null,
   [Name] varchar(255) not null,
   [DateOfBirth] Date not null

)

```

We also will assume the following `appSettings.json` config:

```json
{
  "databaseFactory": {
   
    "databaseCatalogRecordMap": {
      "sqlServer": "Bambit.TestUtility.DatabaseTools.SqlServer.SqlServerDatabaseCatalogRecord,  Bambit.TestUtility.DatabaseTools.SqlServer"
    },
    "mappedDatabases": {
      "Test1": {
        "connectionString": "Server=localhost\\MMXXII; Database=IntegrationTests;Trusted_Connection=true",
        "databaseCatalog": "SqlServer"
      }
    },
  },
},
```
In the above json, the `databaseCatalogRecordMap` section defines a Dictionary of `Type` definitions for any type of database needed.
There are in separate libraries (such as Bambit.TestUtility.DatabaseTools.SqlServer which implements methods to access a SqlServer database).

The `mappedDatabases` section contains a dictionary of `MappedDatabase` values.  The key of this dictionary will be used as the `ConnectionName` in calls to ITestDatabaseFactory methods.  The `connecitonString` is the string that will be used to instantiate the connection, while the `databaseCatalog` is the lookup value in the `databaseCatalogRecordMap`

### ITestDatabaseFactory

The `ITestDatabaseFactory` provides access to configured `ITestDbConnection`s
 and `IDatabaseCatalogRecord`s, along with shortcuts to various methods on a given `ITestDbConnection`. 
 The concrete implemenation, `TestDbConnection`, requires a `TestDatabaseFactoryOptions` injected.
 

```csharp
// Using IBuilder and values from a config

IConfiguration configuration=new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .Build();
 TestDatabaseFactoryOptions options = Configuration.GetSection("databaseFactory").Get<TestDatabaseFactoryOptions>()!;
ITestDatabaseFactory factory=new TestDatabaseFactory(options);

```
### TableToClassBuilder



The `TableToClassBuilder` requires an `ITestDatabaseFactory` to be injected.


```csharp

TableToClassBuilder classBuilder = new(factory);
         


```

From here, you can generate `Types` or  objects:
```csharp
dynamic dynamicOption = classBuilder.GenerateObjectFromTable("Test1", "dbo", "SampleTest");
// It will have 3 fields:
dynamicOption.ID = 123;
dynamicOption.Name = "Mr. Hyde"
dynamicOption.DateOfBirth =null;

// The returned element is always a DatabaseMappedClass
DatabaseMappedClass mappedClass = classBuilder.GenerateObjectFromTable("Test1", "dbo", "SampleTest");

// This exposes a few methods:

// We can assign the values from a dictionary
Dictionary<string, object?> values = new {
   {"ID", 123},
   {"Name", "Mr. Hyde"},
   {"DateOfBirth", new Date(2000,1,1)},
};

// We can get a property
Assert.AreEqual(mappedClass.Get<int>("ID"), 123);

```

### IDatabaseCatalogRecord

`IDatabaseCatalogRecord` imeplementations provide methods to generate `ITestDbConnection` connections, along with some fields that are used in generating `DatabaseMappedClass` instances.
Implementation of this class are left for other packages as they are database specific.

### ITestDbConnection
The `ITestDbConnection` imeplementations are wrappers around specific [IDbConnection](https://learn.microsoft.com/en-us/dotnet/api/system.data.idbconnection?view=net-8.0) objects.
They expose some helper methods such as:

  * `CompareTableToDataset`: Compares a collecton of object arrays to values in a database table
  * `Persist`: Saves a `DatabaseMappedClass` to the database
  * `TrackInfoMessages`: Enables tracking (where possible) or messages from the connection (such as `print` statements)
  
