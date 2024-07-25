using Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Transforms;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A query steps. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="context">      The context. </param>
    /// <param name="outputHelper"> The output helper. </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    [Binding]
    public class QuerySteps(ScenarioContext context, ISpecFlowOutputHelper outputHelper)
        : DatabaseSteps(context, outputHelper)
    {

        #region Givens

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="query">            The query. </param>
        /// <param name="connectionName">   Name of the connection. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Given(@"the query '(?<query>.*)' is run in the (?<connectionName>.*) database")]
        public void RunQuery(string query, string connectionName)
        {
            using ITestDbConnection connection = OpenConnectionForName(connectionName);

            connection.ExecuteQuery(query);

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="query">    The query. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Given(@"the following query is run:")]
        public void RunQuery(string query)
        {
            RunQuery(query, LastDatabaseConnectionName);
        }
        #endregion Givens

        #region Whens

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'multi line query' action. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connectionName">   Name of the connection. </param>
        /// <param name="query">            The query. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [When(@"the following query is run in the (?<connectionName>.*) database:")]
        public void RunMultiLineQueryAction(string connectionName, string query)
        {
            RunInlineQueryAction(query, connectionName);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'inline query' action. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="query">            The query. </param>
        /// <param name="connectionName">   Name of the connection. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [When(@"the query ""(?<query>.*)"" is run in the (?<connectionName>.*) database")]
        public void RunInlineQueryAction(string query, string connectionName)
        {
            using ITestDbConnection connection = OpenConnectionForName(connectionName);

            LastResultSet = ExecuteQueryForResults(connection,query);
        }

        #endregion Whens

        #region Thens

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query and verify results' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="query">    The query. </param>
        /// <param name="table">    The table. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Then(@"the query ""(?<query>.*)"" returns:")]
        public void RunQueryAndVerifyResults(string query, MappedTable table)
        {
            RunQueryAndVerifyResults(query, LastDatabaseConnectionName, table);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query and verify results' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="query">            The query. </param>
        /// <param name="connectionName">   Name of the connection. </param>
        /// <param name="table">            The table. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Then(@"the query ""(?<query>.*)"" in database {?<connectionName>..*} returns:")]
        public void RunQueryAndVerifyResults(string query, string connectionName, MappedTable table)
        {
           
            using ITestDbConnection connection = OpenConnectionForName(connectionName);
            MappedTable existingTable = ExecuteQueryForResults(connection, query);
            object?[][] expectedValue = table.Rows.Select(
                r => r.ApplyTransformValues(ReplaceVariable).GetDbValues(StateManager.Configuration.NullStringIdentifier)
            ).ToArray();
            object?[][] existingValues = existingTable.Rows.Select(
                r => r.ApplyTransformValues(ReplaceVariable).GetDbValues(StateManager.Configuration.NullStringIdentifier)
            ).ToArray();
            CompareTables(table, existingTable);
            connection.CompareResults(table.Columns, table.TableColumns.Select(a => a.ColumnType).ToArray(),
                expectedValue, existingValues, true);

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Verify last results. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="data"> The data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Then(@"the results(?: of the procedure)? will be(?: the following)?:")]
        public void VerifyLastResults(MappedTable data)
        {
            CompareTables(data, LastResultSet);

        }
        #endregion Whens

    }
}
