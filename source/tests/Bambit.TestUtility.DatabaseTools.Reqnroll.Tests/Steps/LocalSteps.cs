using System.Data.SqlClient;
using Bambit.TestUtility.DatabaseTools.Reqnroll.Mapping;
using Npgsql;
using Reqnroll;


namespace Bambit.TestUtility.DatabaseTools.Reqnroll.Tests.Steps;

[Binding]
public class LocalSteps(ScenarioContext context, IReqnrollOutputHelper outputHelper) : DatabaseSteps(context, outputHelper)
{
    [When(@"the following query run in the (?<connectionName>.*) database should timeout:")]
    public void WhenTheFollowingQueryRunInTheTestUtilitiesDatabaseShouldTimeout(string connectionName, string query)
    {
        using ITestDbConnection connection = OpenConnectionForName(connectionName);
        try
        {
            connection.ExecuteQuery(query);
        }
        catch (NpgsqlException npgsqlException )
        {
            if(npgsqlException.InnerException is TimeoutException)
                return;
            throw;
        }
        catch (SqlException sqlException)
        {
            if (sqlException.Message.Contains("Timeout expired"))
                return;
            throw;
        }
        Assert.Fail("Did not timeout as expected");
    }

    [Then(@"only the following records should exist in the (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database should fail:")]
    public void ThenOnlyTheFollowingRecordsShouldExistInTheDbo_TestTableAlphaTableInTheTestUtilitiesDatabaseShouldFail(string schema, string tableName,
        string connectionName, MappedTable data)
    {
        
        try
        {
            CompareTableToDataset(schema, tableName, connectionName,
                data, false);
        }
        catch 
        {
            return;
        }
        Assert.Fail("Expected fail did not occur");
    }

    [Then(@"Inserting the following records in the (?<schema>.*)\.(?<tableName>.*) table will throw an error \(override\):")]
    public void ThenInsertingTheFollowingRecordsInTheTest_TestTableEpsilonTableWillThrowAnErrorOverride(string schema, string tableName,
        MappedTable data)
    {
        TableSteps tableSteps = new (Context, OutputHelper);
        try
        {
            tableSteps.VerifyRecordsThrowError(schema, tableName, data);
        }
        catch 
        {
            return;
        }
        Assert.Fail("Exception was not thrown");
        
    }
    
        
    [Given(@"only the following records exist in the (?<schema>.*)\.(?<tableName>.*) table \(override\):")]
    public void GivenOnlyTheFollowingRecordsExistInTheTable(string schema, string tableName, MappedTable data)
    {
        TableSteps tableSteps = new (Context, OutputHelper);
        try
        {
            tableSteps.EmptyTableAndAddRows(schema, tableName, StateManager.LastDatabaseConnectionName , data);
        }
        catch
        {
            return;
        }
        Assert.Fail("Exception was not thrown");
    }
}