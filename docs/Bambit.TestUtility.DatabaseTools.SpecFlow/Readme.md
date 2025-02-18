# Bambit.TestUtility.DatabaseTools.Reqnroll Basic Concepts

## sage
Provides a set of step definitions ane hooks for manipulating and testing agasint a Database with SpecFlo syntax.

## Getting started

Install:
```shell
PM> Install-Package Bambit.TestUtility.DatabaseTools.Reqnroll

```

To use the defined steps in a StepFlow project, you will need to do the following:
 1. Add the assembly in the `stepAssemblies` section of your ***Reqnroll.json*** file (see https://docs.Reqnroll.org/projects/Reqnroll/en/latest/Bindings/Use-Bindings-from-External-Assemblies.html)
 2. Create a ***bambit.utilities.json*** config file.  It should look like the following:
```json
{

  "databaseFactory": {
    "mappedDatabases": {
      "sqlTestDb": {
        "connectionString": "Server=localhost; Database=TestDB;Trusted_Connection=true",
        "databaseCatalog": "SqlServer"
      }
   },
   "databaseCatalogRecordMap": {
        "sqlServer": "Bambit.TestUtility.DatabaseTools.SqlServer.SqlServerDatabaseCatalogRecord,  Bambit.TestUtility.DatabaseTools.SqlServer"
      }
    },
      
  },
  "Reqnroll": {
    "nullStringIdentifier": "null",
    "timeoutSeconds": 30
  }
}

```
See 


## Usage

Simply use the step definitions as yoou would any other class:

```gherkin

Scenario: Verify records exists in table
	Given I am working in the SqlTestDb database
	And only the following records exist in the [dbo].[TestTable] table:
	| ID                                   | Name                  |
	| 8BFAE7CC-EDEA-4326-B671-334D5FECDAEB | Count Dracula         |
   
Then only the following records should exist in the [dbo].[TestTable] table:
	| ID                                   | Name                  |
	| 8BFAE7CC-EDEA-4326-B671-334D5FECDAEB | Count Dracula         |
   
	
```
For a full list of available steps and usage, see [Documentation](https://github.com/BambitTech/test-utils/Overview.html)




## Usage

Simply use the step definitions as yoou would any other class:

```gherkin

Scenario: Verify records exists in table
	Given I am working in the SqlTestDb database
	And only the following records exist in the [dbo].[TestTable] table:
	| ID                                   | Name                  |
	| 8BFAE7CC-EDEA-4326-B671-334D5FECDAEB | Count Dracula         |
   
Then only the following records should exist in the [dbo].[TestTable] table:
	| ID                                   | Name                  |
	| 8BFAE7CC-EDEA-4326-B671-334D5FECDAEB | Count Dracula         |
   
	
```
For a full list of available steps and usage, see [Documentation](https://bambittech.github.io/test-utils/Bambit.TestUtility.DatabaseTools.Reqnroll/Overview.html) and [Steps](https://bambittech.github.io/test-utils/Bambit.TestUtility.DatabaseTools.Reqnroll/StepDefinitions.html)
