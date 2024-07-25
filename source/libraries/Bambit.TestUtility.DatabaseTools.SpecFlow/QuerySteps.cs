using System.Data;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Extensions;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Transforms;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    [Binding]
    public class QuerySteps(ScenarioContext context, ISpecFlowOutputHelper outputHelper)
        : DatabaseSteps(context, outputHelper)
    {

        #region Givens
        [Given(@"the query '(?<query>.*)' is run in the (?<connectionName>.*) database")]
        public void GivenTheQueryIsRunInTheDatabase(string query, string connectionName)
        {
            using ITestDbConnection connection = OpenConnectionForName(connectionName);

            connection.ExecuteQuery(query);

        }
        
        [Given(@"the following query is run:")]
        public void GivenTheFollowingQueryIsRun(string query)
        {
            GivenTheQueryIsRunInTheDatabase(query, LastDatabaseConnectionName);
        }
        #endregion Givens

        #region Whens

        [When(@"the following query is run in the (?<connectionName>.*) database:")]
        public void WhenTheFollowingQueryIsRun(string connectionName, string query)
        {
            WhenTheQueryIsRunInTheDatabase(query, connectionName);
        }
        
        [When(@"the query ""(?<query>.*)"" is run in the (?<connectionName>.*) database")]
        public void WhenTheQueryIsRunInTheDatabase(string query, string connectionName)
        {
            using ITestDbConnection connection = OpenConnectionForName(connectionName);

            LastResultSet = ExecuteQueryForResults(connection,query);
        }

        #endregion Whens

        #region Thens
        [Then(@"the query ""(?<query>.*)"" returns:")]
        public void ThenTheQueryReturns(string query, MappedTable table)
        {
            ThenTheQueryReturns(query, LastDatabaseConnectionName, table);
        }

        
        [Then(@"the query ""(?<query>.*)"" in database {?<connectionName>..*} returns:")]
        public void ThenTheQueryReturns(string query, string connectionName, MappedTable table)
        {
           
            using ITestDbConnection connection = OpenConnectionForName(connectionName);
            MappedTable existingTable = ExecuteQueryForResults(connection, query);
            object?[][] expectedValue = table.Rows.Select(
                r => r.ApplyTransformValues(ReplaceVariable).ExtractValues(StateManager.Configuration.NullStringIdentifier, ReplaceVariable)
            ).ToArray();
            object?[][] existingValues = existingTable.Rows.Select(
                r => r.ApplyTransformValues(ReplaceVariable).ExtractValues(StateManager.Configuration.NullStringIdentifier, ReplaceVariable)
            ).ToArray();
            CompareTables(table, existingTable);
            connection.CompareResults(table.Columns, table.TableColumns.Select(a => a.ColumnType).ToArray(),
                expectedValue, existingValues, true);

        }

        
        [Then(@"the results(?: of the procedure)? will be(?: the following)?:")]
        public void ThenTheResultsOfTheProcedureWillBeTheFollowing(MappedTable data)
        {
            CompareTables(data, LastResultSet);

        }
        #endregion Whens

        #region Helpers

        protected void CompareTables(MappedTable expected, MappedTable actual)
        {
            using ITestDbConnection connection = OpenConnectionForName(LastDatabaseConnectionName);
            object?[][] expectedValue = expected.Rows.Select(
                r => r.ApplyTransformValues(ReplaceVariable).ExtractValues(StateManager.Configuration.NullStringIdentifier, ReplaceVariable)
            ).ToArray();
            object?[][] existingValues = actual.Rows.Select(
                r => r.ApplyTransformValues(ReplaceVariable).ExtractValues(StateManager.Configuration.NullStringIdentifier, ReplaceVariable)
            ).ToArray();
            connection.CompareResults(expected.Columns, expected.TableColumns.Select(a => a.ColumnType).ToArray(),
                expectedValue, existingValues, true);
        }
        protected MappedTable ExecuteQueryForResults(ITestDbConnection connection, string query)
        {
            using IDataReader reader= connection.ExecuteReader(query);
            MappedTable table = new(reader);
            reader.Close();
            return table;
        }

        #endregion Helpers
    }
}
