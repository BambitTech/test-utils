using System.Data;
using System.Data.SqlClient;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    [Binding]
    public class TableSteps(ScenarioContext context, ISpecFlowOutputHelper outputHelper)
        : DatabaseSteps(context, outputHelper)
    {
        
        [Given(
            @"[Oo]nly the following records exist in the (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database:")]
        public void GivenOnlyTheFollowingRecordsExistInTheTableInTheDatabase(string schema, string tableName,
            string connectionName,
            MappedTable data)
        {
            GivenTheTableInTheDatabaseIsEmpty(schema, tableName, connectionName);
            GivenTheFollowingRecordsExistInTheDatabase(schema, tableName, connectionName, data);
        }

       
        [Given(@"[Tt]he table (?<schema>.*)\.(?<tableName>.*) is empty in the (?<connectionName>.*) database")]
        public void GivenTheTableInTheDatabaseIsEmpty(string schema, string tableName, string connectionName)
        {

            using IDbConnection connection = OpenConnectionForName(connectionName);
            IDatabaseCatalogRecord databaseCatalogRecord = GetCurrentConnector();

            ExecuteQuery(connection,
                $"delete from {databaseCatalogRecord.CleanAndEscapeToken(schema)}.{databaseCatalogRecord.CleanAndEscapeToken(tableName)}"
            );
        }

        [Given(@"[Tt]he table (?<schema>.*)\.(?<tableName>.*) is empty")]
        public void GivenTheTableIsEmpty(string schema, string tableName)
        {
            GivenTheTableInTheDatabaseIsEmpty(schema, tableName, LastDatabaseConnectionName);
        }

        
        [Given(@"[Tt]he following records exist in the (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database:")]
        public void GivenTheFollowingRecordsExistInTheDatabase(string schema, string tableName, string connectionName,
            MappedTable data)
        {
            GenerateAndPersistDatabaseTableObjects(schema, tableName, connectionName, data);
        }
        
        [Given(@"[Oo]nly the following records exist in the (?<schema>.*)\.(?<tableName>.*) table:")]
        public void GivenOnlyTheFollowingRecordsExistInTheTable(string schema, string tableName, MappedTable data)
        {
            GivenOnlyTheFollowingRecordsExistInTheTableInTheDatabase(schema, tableName, StateManager.LastDatabaseConnectionName , data);
        }
        
        [Then(@"[Oo]nly the following records should exist in the (?<schema>.*)\.(?<tableName>.*) (?:table|view):")]
        public void ThenOnlyTheFollowingRecordsShouldExistInTheTable(string schema, string tableName,
            MappedTable data)
        {
            CompareTableToDataset(schema, tableName, StateManager.LastDatabaseConnectionName, data, false);
        }

        [Then(@"[Oo]nly the following records should exist in the (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database:")]
        public void ThenOnlyTheFollowingRecordsShouldExistInTheTableInTheDatabase(string schema, string tableName,
            string connectionName, MappedTable data)
        {
            CompareTableToDataset(schema, tableName, connectionName, data, false);
        }
        
        [Then(@"[Tt]he following records should exist in the (?<schema>.*)\.(?<tableName>.*) (?:table|view) in the (?<connectionName>.*) database:")]
        public void ThenTheFollowingRecordsShouldExistInTheTableInTheDatabase(string schema, string tableName,
            string connectionName, MappedTable data)
        {
            CompareTableToDataset(schema, tableName, connectionName, data, true);
        }

        [Then(@"[Ii]nserting the following records in the (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database will throw an error:")]
        public void ThenInsertingTheFollowingRecordsInTheTableInTheDatabaseWillThrowAnError(string schema, string tableName,
            string connectionName,
            MappedTable data)
        {
            StateManager.LastDatabaseConnectionName = connectionName;
            try
            {
                GivenTheFollowingRecordsExistInTheDatabase(schema, tableName, StateManager.LastDatabaseConnectionName, data);
            }
            catch (SqlException exc)
            {
                OutputHelper.WriteLine($"Sql Exception caught: {exc.Message}");
                Context.Set(exc);
                return;
            }
            OutputHelper.WriteLine("Expected exception did not occur");
            Assert.Fail();
        }
        
        [Then(@"[Ii]nserting the following records in the (?<schema>.*)\.(?<tableName>.*) table will throw an error:")]
        public void ThenInsertingTheFollowingRecordsInTheTableInTheDatabaseWillThrowAnError(string schema, string tableName,
            MappedTable data)
        {
            ThenInsertingTheFollowingRecordsInTheTableInTheDatabaseWillThrowAnError(schema, tableName,StateManager.LastDatabaseConnectionName, data);
        }

     
        [Then(@"[Tt]he last SQL exception will contain the phrase ""(?<expectedPhrase>.*)""")]
        public void ThenTheLastExceptionWillContainThePhrase(string expectedPhrase)
        {
            SqlException sqlException = Context.Get<SqlException>();
            Assert.IsNotNull(sqlException, "No Sql Exception found");
            string phrase = sqlException.Message;
            
            Assert.IsTrue(phrase.IndexOf(expectedPhrase, StringComparison.CurrentCultureIgnoreCase) >= 0, 
                $"Exception '{sqlException.Message}' did not contain string '{expectedPhrase}");
        }
        
        [Then(@"[Tt]he (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database will have (?<expectedRows>\d.*) rows")]
        public void ThenTheTableInTheDatabaseWillHaveRows(string schema,
            string tableName, string connectionName, int expectedRows)
        {
            
            using ITestDbConnection connection = OpenConnectionForName(connectionName);
            {
                IDatabaseCatalogRecord databaseCatalogRecord = GetCurrentConnector();
                int results =
                    connection.ExecuteScalar<int>(
                        $"select count(1) from {databaseCatalogRecord.CleanAndEscapeToken(schema)}.{databaseCatalogRecord .CleanAndEscapeToken(tableName)}");
                Assert.AreEqual(expectedRows, results);
            }
        }
        
        [Then(@"[Tt]he (?<schema>.*)\.(?<tableName>.*) table in the (?<connectionName>.*) database will have no rows")]
        public void ThenTheTableInTheDatabaseWillHaveNoRows(string schema,
            string tableName, string connectionName)
        {
            ThenTheTableInTheDatabaseWillHaveRows(schema, tableName, connectionName, 0);
        }

    }
}
