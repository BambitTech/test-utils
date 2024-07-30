# Usage

## General Commands
When executing a database step, it's important to be aware of which connection you are using, as multiple connecitons can exist for a given test project.
Most steps have a version with a parameter `connectionName`, which will specify the Test DB Connection to use.
Once called, that connectionString will be used for all future steps until explicitly changed by passing in a new `connectionName` to a step.

To explicilty select a database connection, you can also call the step `I am working in the (?:<connectionName>.*) database`

## Given, When, Then
Many steps will have versions for multiple types of executing.  The general differnece are as follow:
 * __Given__ These statements are executed without consideration of the results.  
Any results from a query or stored procedure will be completely ignored and not even iterated over.
 * __When__ These statements will, when applicable, store the results as the ___LastResults___ for future comparisons
 * __Then__ These statements either test immediate results or compare the __LastResults__ against the values.



## Connections

All database connections needs to be defined in the _databaseFactory:mappedDatabases_ section of the bambit.utilities.json_ file.
The keys of the mapped databases will be used when defining `connectionName` values for queries to run in.

## Populating tables
When populating tables, you only need to specify the columns you are concerned about; remaining columns will be filled with random data.
So if you have a table with 20 column, but for a given test you only need to specify data for three of them, you only need to define those three, even if the others are not nullable.

Where possible, data types are automatically derived; however, to force certian data types you can specify the type in the column header with "@<datatype>".
Currently, the only available forced types  "__@date__" and "__@boolean__".

Along with parsing dates, the following strings will be parsed out and applied to dates, as implemented in the [Bambit.TestUtility.DataGeneration] package:

| String | Value |
|--- |---|
| today	                 |	DateTime.Today                                                                    |
| yesterday               | DateTime.Today.AddDays(-1)                                                         |
| tomorrow                | DateTime.Today.AddDays(1)                                                          |
| now                    | DateTime.Now                                                                       |
| first day of this month     | new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)                         |
| first day of current month  | new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)                         |
| first day of month         | new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)                         |
| last day of this month    | new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1)|
| last day of current month  | new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1)|
| last day of month        | new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1)|
| end of last month        | new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1)             |
| last day of last month    | new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1)             |
| last day of previous month|new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1)              |


Futhermore, you can add or subtract years, months, days, hours, minutes or seconds from any value:

```
now + 1 hour
last day of previous month - 2 days
tomorrow - 1 year

```

##  <a name="namedQueries" ></a> Named Queries
Occasionally you may have a multi line or complex query you want to run multiple times, or want to pass variables to the query.
In these scenarios, you can load a script and assign it a name, after which you can reference the query by name.

To register a named query, make a call to one of the `QueryHelpers.RegisterNamedScript` methods in you test initialization:

```csharp
public static void BeforeTestRun()
{
   QueryHelpers.RegisterNamedScript("My Named Script", "select @var1 as [Variable1], @var2 as [Variable2]");
}
```

This uses a lazy loaded Dictionary to store the queries by key.

```csharp
/// <summary>
/// Registers a query with a name for later referencing.
/// </summary>
/// <param name="name">The name to use when referencing the query.  If this name has already been used, it will be overwritten with the new value.</param>
/// <param name="query">The query to register.</param>
QueryHelpers.RegisterNamedScript(string name, string query)

/// <summary>
/// Loads a file containing a query for later referencing, storing the contents of the file with the supplied name.
/// </summary>
/// <param name="name">The name to use when referencing the query.  If this name has already been used, it will be overwritten with the new value.</param>
/// <param name="filePath">Relative location (from the executing dll) of the file.</param>
QueryHelpers.RegisterNamedScriptFromFile(string name, string filePath)
 /// <summary>
/// Loads a file embedded inside a module for later referencing, storing the contents of the file with the supplied name.
/// </summary>
/// <param name="name">The name to use when referencing the query.  If this name has already been used, it will be overwritten with the new value.</param>
/// <param name="assembly">Reference to the assembly in which the file is embedded.</param>
/// <param name="filePath">Path of the file (manifest resource stream path) inside the assembly.</param>
public static void RegisterNamedScriptFromEmbeddedFile( string name, Assembly assembly, string filePath)
```

Once registered, you can use one of the steps to reference the query:
```gherkin
When the query named "My Named Script" is run with the following parameters:
| var1 | var2|
| Foo  | Bar |
Then the results will be the following:
| Variable1 | Variable2 |
| Foo       | Bar       |
```
## Validation

To validate result sets, the library does the following:

 1. Creates 2 temporary tables.
These tables have a column for each defined column in the results defined in the feature file, along with 1 more column "***[  CHECK SUM COLUMN  ]***" which contains a checksum to simplify the validation.
 2. The tables are populated with the values from the feature file, along with the source table or __LastResults__
 3. Only fields defined in the feature file results are included; any extra fields in the source table ot __LastResults__ are ignored completely.
 4. The ___[  CHECK SUM COLUMN  ]___ column is computed with the SQL function checksum
 5. The ___[  CHECK SUM COLUMN  ]___ values are checked against the 2 tables, and results are passed back and asserted against, depending on the step.
 
 
With this in mind, there are a couple limitations, primarily that columns that are not allowed in the **checksum** function (_xml_, _cursor_, _image_, _ntext_, _text_) cannnot be validated.
One work around is to use the `Then the query "(?<query>.*)" returns:` to select the values, which will then be treated as base types.

