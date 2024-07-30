using Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Tools;
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


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="query">            The query. </param>
        /// <param name="connectionName">   Name of the connection. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Given(@"[Tt]he query '(?<query>.*)' is run in the (?<connectionName>.*) database")]
        public void RunQuery(string query, string connectionName)
        {
            using ITestDbConnection connection = OpenConnectionForName(connectionName);

            connection.ExecuteQuery(query);
            LastDatabaseConnectionName=connectionName;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="query">    The query. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Given(@"[Tt]he following query is run:")]
        public void RunQuery(string query)
        {
            RunQuery(query, LastDatabaseConnectionName);
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the multi line query, saving the results. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connectionName">   Name of the connection. </param>
        /// <param name="query">            The query. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [When(@"[Tt]he following query is run in the (?<connectionName>.*) database:")]
        public void RunMultiLineQueryActionForResults(string connectionName, string query)
        {
            RunQueryForResults(query, connectionName);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'inline query' action. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="query">            The query. </param>
        /// <param name="connectionName">   Name of the connection. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [When(@"[Tt]he query ""(?<query>.*)"" is run in the (?<connectionName>.*) database")]
        public void RunQueryForResults(string query, string connectionName)
        {
            ExecuteQueryForResults(connectionName,query);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query and verify results' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/32024. </remarks>
        ///
        /// <param name="query">    The query. </param>
        /// <param name="table">    The table. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Then(@"[Tt]he query ""(?<query>.*)"" returns:")]
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

        [Then(@"[Tt]he query ""(?<query>.*)"" in database {?<connectionName>..*} returns:")]
        public void RunQueryAndVerifyResults(string query, string connectionName, MappedTable table)
        {
           
            using ITestDbConnection connection = OpenConnectionForName(connectionName);
            MappedTable existingTable = ExecuteQueryForResults(connectionName, query);
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

        [Then(@"[Tt]he results(?: of the procedure)? will be(?: the following)?:")]
        public void VerifyLastResults(MappedTable data)
        {
            CompareTables(data, LastResultSet);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the previously loaded query with the given name. </summary>
        ///
        /// <remarks>   Law Metzler, 7/30/2024. </remarks>
        ///
        /// <param name="queryName">    Name of the query. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Given(@"[Tt]he query named ""(?<queryName>.*)"" is run")]
        public void RunNamedQuery(string queryName)
        {
            RunNamedQuery(queryName, LastDatabaseConnectionName);
        }

        /// <summary>
        /// Executes the previously loaded query with the given name.
        /// </summary>
        /// <remarks>
        /// Law Metzler, 7/30/2024.
        /// </remarks>
        /// <param name="queryName">        Name of the query. </param>
        /// <param name="connectionName">   Name of the connection. </param>
        [Given(@"[Tt]he query named ""(?<queryName>.*)"" is run in the (?<connectionName>.*) database")]
        public void RunNamedQuery(string queryName, string connectionName)
        {
            string script = QueryHelpers.GetScript(queryName);
            RunQuery(script, connectionName);
        }

        /// <summary>
        /// Executes the named query, storing the results
        /// </summary>
        /// <param name="queryName">    Name of the query. </param>
        [When(@"[Tt]he query named ""(?<queryName>.*)"" is run")]
        public void RunNamedQueryForResults(string queryName)
        {
            RunNamedQueryForResults(queryName, LastDatabaseConnectionName);
        }

        /// <summary>
        /// Executes the named query, storing the results.
        /// </summary>
        /// <param name="queryName">        Name of the query. </param>
        /// <param name="connectionName">   Name of the connection. </param>
        [When(@"[Tt]he query named ""(?<queryName>.*)"" is run in the (?<connectionName>.*) database")]
        public void RunNamedQueryForResults(string queryName, string connectionName)
        {
            
            string script = QueryHelpers.GetScript(queryName);
            RunQueryForResults(script, connectionName);
        }

        /// <summary>
        /// Executes the named query and compare results against the expected
        /// </summary>
        /// <param name="queryName">    Name of the query. </param>
        /// <param name="results">      The results. </param>
        [Then(@"[Tt]he query named ""(?<queryName>.*)"" returns:")]
        public void RunNamedQueryAndCompareResults(string queryName, MappedTable results)
        {
            RunNamedQueryForResults(queryName);
            CompareTables(results, LastResultSet);
        }

        /// <summary>
        /// Executes the named query and compare results against the expected.
        /// </summary>
        /// <param name="queryName">        Name of the query. </param>
        /// <param name="connectionName">   Name of the connection. </param>
        /// <param name="results">          The results. </param>
        [Then(@"[Tt]he query named ""(?<queryName>.*)"" run in the (?<connectionName>.*) database returns:")]
        public void RunNamedQueryAndCompareResults(string queryName, string connectionName, MappedTable results)
        {
            RunNamedQueryForResults(queryName, connectionName);
            CompareTables(results,LastResultSet );
        }

        /// <summary>
        /// Executes the named query against the last connection, passing in the parameters
        /// </summary>
        /// <param name="queryName">    Name of the query. </param>
        /// <param name="parameters">   Options for controlling the operation. </param>
        [Given(@"[Tt]he query named ""(?<queryName>.*)"" is run with the (?:following )?parameters:")]
        public void RunNamedQueryWithParameters(string queryName, Table parameters)
        {
            RunNamedQueryWithParameters(queryName, LastDatabaseConnectionName, parameters);
        }

        /// <summary>
        /// Executes the named query against the last connection, passing in the parameters.
        /// </summary>
        /// <param name="queryName">        Name of the query. </param>
        /// <param name="connectionName">   Name of the connection. </param>
        /// <param name="parameters">       Options for controlling the operation. </param>
        [Given(@"[Tt]he query named ""(?<queryName>.*)"" is run in the (?<connectionName>.*) database with the (?:following ) parameters:")]
        public void RunNamedQueryWithParameters(string queryName, string connectionName, Table parameters)
        {
            string script = QueryHelpers.GetScript(queryName);
            RunQueryWithParameters(script, connectionName, parameters);
        }

        /// <summary>
        /// Executes the query, passing in the parameters.
        /// </summary>
        /// <param name="query">            The query. </param>
        /// <param name="connectionName">   Name of the connection. </param>
        /// <param name="parameters">       Options for controlling the operation. </param>
        [Given(@"[Tt]he query ""(?<query>.*)"" is run in the (?<connectionName>.*) database with the (?:following )? parameters:")]
        public void RunQueryWithParameters(string query, string connectionName, Table parameters)
        {
            ExecuteQuery(connectionName, query, parameters);
        }

        /// <summary>
        /// Executes the named query, passing in the parameters, and saves the results
        /// </summary>
        /// <param name="queryName">    Name of the query. </param>
        /// <param name="parameters">   Options for controlling the operation. </param>
        [When(@"the query named ""(?<queryName>.*)"" is run with the (?:following )?parameters:")]
        public void RunNamedQueryWithParametersForResults(string queryName, Table parameters)
        {
            RunNamedQueryWithParametersForResults(queryName, LastDatabaseConnectionName, parameters);
        }

        /// <summary>
        /// Executes the named query, passing in the parameters, and saves the results.
        /// </summary>
        /// <param name="queryName">        Name of the query. </param>
        /// <param name="connectionName">   Name of the connection. </param>
        /// <param name="parameters">       Options for controlling the operation. </param>
        [When(@"[Tt]he query named ""(?<queryName>.*)"" is run in the (?<connectionName>.*) database with the (?:following) parameters:")]
        public void RunNamedQueryWithParametersForResults(string queryName, string connectionName, Table parameters)
        {
            string script = QueryHelpers.GetScript(queryName);
            ExecuteQueryForResults(connectionName,script, parameters);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'multi line query' action. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connectionName">   Name of the connection. </param>
        /// <param name="query">            The query. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Given(@"[Tt]he following query is run in the (?<connectionName>.*) database:")]
        public void RunMultiLineQueryAction(string connectionName, string query)
        {
            RunQuery(query, connectionName);
        }

        /// <summary>
        /// Executes the query, saving the results 
        /// </summary>
        /// <param name="query">    The query. </param>
        [When(@"the following query is run:")]
        public void RunQueryForResults(string query)
        {
            RunQueryForResults(query, LastDatabaseConnectionName);
        }

        
        /// <summary>
        /// Executes the query, saving the results 
        /// </summary>
        /// <param name="query">    The query. </param>
        [When(@"the query ""(?<query>.*)"" is run")]
        public void RunInlineQueryForResults(string query)
        {
            RunQueryForResults(query, LastDatabaseConnectionName);
        }
    }
}
