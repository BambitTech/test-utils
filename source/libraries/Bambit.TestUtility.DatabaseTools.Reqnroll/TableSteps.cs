using System.Data;
using Bambit.TestUtility.DatabaseTools.Reqnroll.Mapping;
using Bambit.TestUtility.DatabaseTools.Reqnroll;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace Bambit.TestUtility.DatabaseTools.Reqnroll;

/// <summary>
/// Steps that apply to a Database table
/// </summary>
/// <param name="context">The current <see cref="ScenarioContext"/></param>
/// <param name="outputHelper">The current <see cref="IReqnrollOutputHelper"/></param>
[Binding]
public class TableSteps(ScenarioContext context, IReqnrollOutputHelper outputHelper)
    : DatabaseSteps(context, outputHelper)
{
    /// <summary>
    /// Clears out a table and adds in the supplied rows
    /// </summary>
    /// <param name="schema">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="connectionName">The applied connection name</param>
    /// <param name="data">The data to insert</param>
    [Given(
        @"[Oo]nly the following records exist in the (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database:")]
    public void EmptyTableAndAddRows(string schema, string tableName,
        string connectionName,
        MappedTable data)
    {
        EmptyTable(schema, tableName, connectionName);
        AddRecordsToDatabase(schema, tableName, connectionName, data);
    }
    /// <summary>
    /// Clear our all records in a table
    /// </summary>
    /// <param name="schema">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="connectionName">The applied connection name</param>
       
    [Given(@"^[Tt]he table (?<schema>.*)\.(?<tableName>.*) is empty in the (?<connectionName>.*) database$")]
    public void EmptyTable(string schema, string tableName, string connectionName)
    {

        using IDbConnection connection = OpenConnectionForName(connectionName);
        IDatabaseCatalogRecord databaseCatalogRecord = GetCurrentConnector();

        ExecuteQuery(connection,
            $"delete from {databaseCatalogRecord.CleanAndEscapeToken(schema)}.{databaseCatalogRecord.CleanAndEscapeToken(tableName)}"
        );
    }

    /// <summary>
    /// Clear our all records in a table for the last connection used
    /// </summary>
    /// <param name="schema">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    [Given(@"^[Tt]he table (?<schema>.*)\.(?<tableName>.*) is empty$")]
    public void EmptyTable(string schema, string tableName)
    {
        EmptyTable(schema, tableName, LastDatabaseConnectionName);
    }

    /// <summary>
    /// Adds a collection of records to a table
    /// </summary>
    /// <param name="schema">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="connectionName">The applied connection name</param>
    /// <param name="data">The data to insert</param>
        
    [Given(@"^[Tt]he following records exist in the (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database:$")]
    public void AddRecordsToDatabase(string schema, string tableName, string connectionName,
        MappedTable data)
    {
        GenerateAndPersistDatabaseTableObjects(schema, tableName, connectionName, data);
    }
    /// <summary>
    /// Clears out a table and adds in the supplied rows
    /// </summary>
    /// <param name="schema">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="data">The data to insert</param>
    [Given(@"^[Oo]nly the following records exist in the (?<schema>.*)\.(?<tableName>.*) table:$")]
    public void EmptyTableAndAddRows(string schema, string tableName, MappedTable data)
    {
        EmptyTableAndAddRows(schema, tableName, StateManager.LastDatabaseConnectionName , data);
    }
    /// <summary>
    /// Verifies that only the supplied records exist in the table, using the most recent connection
    /// </summary>
    /// <param name="schema">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="data">The data to verify</param>
    [Then(@"^[Oo]nly the following records should exist in the (?<schema>.*)\.(?<tableName>.*) (?:table|view):$")]
    public void VerifyOnlyRecordsExistInTable(string schema, string tableName,
        MappedTable data)
    {
        CompareTableToDataset(schema, tableName, StateManager.LastDatabaseConnectionName, data, false);
    }
    /// <summary>
    /// Verifies that only the supplied records exist in the table
    /// </summary>
    /// <param name="schema">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="connectionName">The applied connection name</param>
    /// <param name="data">The data to verify</param>

    [Then(@"^[Oo]nly the following records should exist in the (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database:$")]
    public void VerifyOnlyRecordsExistInTable(string schema, string tableName,
        string connectionName, MappedTable data)
    {
        CompareTableToDataset(schema, tableName, connectionName, data, false);
    }
        
    /// <summary>
    /// Verifies the supplied records exist in the table, but other records may exist
    /// </summary>
    /// <param name="schema">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="connectionName">The applied connection name</param>
    /// <param name="data">The data to verify</param>
    [Then(@"^[Tt]he following records should exist in the (?<schema>.*)\.(?<tableName>.*) (?:table|view) in the (?<connectionName>.*) database:$")]
    public void VerifyRecordsExistInTable(string schema, string tableName,
        string connectionName, MappedTable data)
    {
        CompareTableToDataset(schema, tableName, connectionName, data, true);
    }
        
    /// <summary>
    /// Verifies the supplied records, when inserted into a table, will throw an exception
    /// </summary>
    /// <param name="schema">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="connectionName">The applied connection name</param>
    /// <param name="data">The data to verify</param>
    [Then(@"^[Ii]nserting the following records in the (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database will throw an error:$")]
    public void VerifyRecordsThrowError(string schema, string tableName,
        string connectionName,
        MappedTable data)
    {
        StateManager.LastDatabaseConnectionName = connectionName;
        try
        {
            AddRecordsToDatabase(schema, tableName, StateManager.LastDatabaseConnectionName, data);
        }
        catch (Exception exc)
        {
            OutputHelper.WriteLine($"Exception caught: {exc.Message}");
            Context.Set(exc);
            return;
        }
        OutputHelper.WriteLine("Expected exception did not occur");
        Assert.Fail();
    }
        
    /// <summary>
    /// Verifies the supplied records, when inserted into a table, will throw an exception.  Users last connection
    /// </summary>
    /// <param name="schema">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="data">The data to verify</param>
    [Then(@"^[Ii]nserting the following records in the (?<schema>.*)\.(?<tableName>.*) table will throw an error:$")]
    public void VerifyRecordsThrowError(string schema, string tableName,
        MappedTable data)
    {
        VerifyRecordsThrowError(schema, tableName,StateManager.LastDatabaseConnectionName, data);
    }

    /// <summary>
    /// Verifies that the last exception thrown contains a phrase
    /// </summary>
    /// <param name="expectedPhrase">The phrase expected</param>
     
    [Then(@"^[Tt]he last SQL exception will contain the phrase ""(?<expectedPhrase>.*)""$")]
    public void VerifyLastExceptionPhrase(string expectedPhrase)
    {
        Exception sqlException = Context.Get<Exception>();
        Assert.IsNotNull(sqlException, "No Sql Exception found");
        string phrase = sqlException.Message;
            
        Assert.IsTrue(phrase.Contains(expectedPhrase, StringComparison.CurrentCultureIgnoreCase), 
            $"Exception '{sqlException.Message}' did not contain string '{expectedPhrase}");
    }
        
    /// <summary>
    /// Verifies the table has the exact number of rows
    /// </summary>
    /// <param name="schema">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="connectionName">The applied connection name</param>
    /// <param name="expectedRows">Number of rows expected</param>
    [Then(@"^[Tt]he (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database will have (?<expectedRows>\d.*) rows$")]
    public void VerifyRowCountInTable(string schema,
        string tableName, string connectionName, int expectedRows)
    {
            
        using ITestDbConnection connection = OpenConnectionForName(connectionName);
        {
            IDatabaseCatalogRecord databaseCatalogRecord = GetCurrentConnector();
            int results =
                connection.ExecuteScalar<int>(
                    $"select cast(count(1) as integer) from {databaseCatalogRecord.CleanAndEscapeToken(schema)}.{databaseCatalogRecord .CleanAndEscapeToken(tableName)}");
            Assert.AreEqual(expectedRows, results);
        }
    }
        
        
    /// <summary>
    /// Verifies the table has no rows
    /// </summary>
    /// <param name="schema">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <param name="connectionName">The applied connection name</param>
    [Then(@"^[Tt]he (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database will have no rows$")]
    public void VerifyTableIsEmpty(string schema,
        string tableName, string connectionName)
    {
        VerifyRowCountInTable(schema, tableName, connectionName, 0);
    }

}