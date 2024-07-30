
using System.Data;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Transforms;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Base class, providing functionality for database manipulation. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="context">      . </param>
    /// <param name="outputHelper"> . </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class DatabaseSteps(ScenarioContext context, ISpecFlowOutputHelper outputHelper) : BaseSteps(context)
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Current <see cref="ISpecFlowOutputHelper"/> </summary>
        ///
        /// <value> The output helper. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected  ISpecFlowOutputHelper OutputHelper { get; }=outputHelper;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The last used connection. </summary>
        ///
        /// <value> The name of the last database connection. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected string LastDatabaseConnectionName
        {
            get => StateManager.LastDatabaseConnectionName;
            set => StateManager.LastDatabaseConnectionName = value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Returns a <see cref="ITestDbConnection"/> for the supplied name. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connectionName">   Name of connection to return. </param>
        ///
        /// <returns>   An ITestDbConnection. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected ITestDbConnection OpenConnectionForName(string connectionName)
        {
            return StateManager.OpenConnectionForName(connectionName);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Returns the current <see cref="IDatabaseCatalogRecord"/> </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <returns>   A <see cref="IDatabaseCatalogRecord"/> </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected IDatabaseCatalogRecord GetCurrentConnector()
        {
            return StateManager.GetCurrentConnector();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Compare table to dataset. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="schema">               The schema. </param>
        /// <param name="tableName">            Name of the table. </param>
        /// <param name="connectionName">       Name of connection to return. </param>
        /// <param name="data">                 The data. </param>
        /// <param name="allowUnexpectedRows">  True to allow, false to suppress the unexpected rows. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CompareTableToDataset(string schema, string tableName,
            string connectionName, MappedTable data, bool allowUnexpectedRows)
        {
            
            using ITestDbConnection connection = OpenConnectionForName(connectionName);
            object?[][] values = data.Rows.Select(
                r => r.ApplyTransformValues(ReplaceVariable).GetDbValues(StateManager.Configuration.NullStringIdentifier)
            ).ToArray();
            VerifyCompareResults(connection.CompareTableToDataset(schema, tableName, data.Columns, values,
                allowUnexpectedRows));

            StateManager.LastDatabaseConnectionName = connectionName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Generates an and persist database table objects. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="schema">           The schema. </param>
        /// <param name="tableName">        Name of the table. </param>
        /// <param name="connectionName">   Name of connection to return. </param>
        /// <param name="data">             The data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void GenerateAndPersistDatabaseTableObjects(string schema, string tableName, string connectionName,
            MappedTable data)
        {
            DatabaseClassFactory databaseClassFactory = StateManager.DatabaseClassFactory;
            ITestDbConnection testDbConnection= StateManager.OpenConnectionForName(connectionName);
            
            
            foreach (MappedRow tableRow in data.Rows)
            {
                DatabaseMappedClass instance = databaseClassFactory.GenerateObjectFromTable(connectionName, schema, tableName);
                tableRow.TransformForNull(StateManager.Configuration.NullStringIdentifier);
                string[] assignedValues = tableRow.AssignToObject(instance);
                
                if (assignedValues.Length != data.Columns.Length)
                {
                    IEnumerable<string> missingFields = data.TableColumns
                        .Where(c => !assignedValues.Select(a => a.ToLower()).Contains(c.CleanedName)).Select(d => d.ColumnName);
                    Assert.Fail($"Not all supplied fields for table {schema}.{tableName} were assigned: '{string.Join("','", missingFields)}'");
                }
                
                testDbConnection.Persist(instance);
            }
            StateManager.LastDatabaseConnectionName = connectionName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a command. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connection">   The connection. </param>
        ///
        /// <returns>   The new command. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public IDbCommand CreateCommand(IDbConnection connection)
        {
            IDbCommand command = connection.CreateCommand();
            if (Configuration.TimeoutSeconds.HasValue)
                command.CommandTimeout = Configuration.TimeoutSeconds.Value;
            return command;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connection">   The connection. </param>
        /// <param name="query">        The query. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ExecuteQuery(IDbConnection connection, string query)
        {
            
            using IDbCommand command = CreateCommand(connection);
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connection">   The connection. </param>
        /// <param name="query">        The query. </param>
        /// <param name="parameters">A Table of parameter values</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ExecuteQuery(IDbConnection connection, string query, Table parameters)
        {
            
            using IDbCommand command = CreateCommand(connection);
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            
            TableRow tableRow = parameters.Rows.First();
            string[] headers = [.. parameters.Header];
            for (int x = 0; x < parameters.Header.Count; x++)
            {
                string name = headers[x];
                if (!name.StartsWith('@'))
                    name = $"@{name}";
                string value = tableRow[x];
                IDbDataParameter parameter = command.CreateParameter();
                parameter.ParameterName=name;
                if (value == Configuration.NullStringIdentifier)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    parameter.Value = value;
                }
                command.Parameters.Add(parameter);
            }

            command.ExecuteNonQuery();
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connection">   The connection. </param>
        /// <param name="query">        The query. </param>
        /// <param name="parameters">A Table of parameter values</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ExecuteQueryForResults(ITestDbConnection connection, string query, Table parameters)
        {
            
            using IDbCommand command = CreateCommand(connection);
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            
            TableRow tableRow = parameters.Rows.First();
            string[] headers = [.. parameters.Header];
            for (int x = 0; x < parameters.Header.Count; x++)
            {
                string name = headers[x];
                if (!name.StartsWith('@'))
                    name = $"@{name}";
                string value = tableRow[x];
                IDbDataParameter parameter = command.CreateParameter();
                parameter.ParameterName=name;
                if (value == Configuration.NullStringIdentifier)
                {
                    parameter.Value = DBNull.Value;
                }
                else
                {
                    parameter.Value = value;
                }
                command.Parameters.Add(parameter);
            }

            using IDataReader reader = command.ExecuteReader();
            MappedTable table = new(reader);
            reader.Close();

            LastResultSet = table;
            
        }

        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connectionName">   The name of the connection to use. </param>
        /// <param name="query">        The query. </param>
        /// <param name="parameters">A Table of parameter values</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ExecuteQuery(string connectionName, string query, Table parameters)
        {
            
            using ITestDbConnection connection = OpenConnectionForName(connectionName);
            ExecuteQuery(connection, query, parameters);
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connectionName">   The name of the connection to use. </param>
        /// <param name="query">        The query. </param>
        /// <param name="parameters">A Table of parameter values</param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ExecuteQueryForResults(string connectionName, string query, Table parameters)
        {
            
            using ITestDbConnection connection = OpenConnectionForName(connectionName);
            ExecuteQueryForResults(connection, query, parameters);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Compare tables. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="expected"> The expected. </param>
        /// <param name="actual">   The actual. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void CompareTables(MappedTable expected, MappedTable actual)
        {
            using ITestDbConnection connection = OpenConnectionForName(LastDatabaseConnectionName);
            object?[][] expectedValue = expected.Rows.Select(
                r => r.ApplyTransformValues(ReplaceVariable).GetDbValues(StateManager.Configuration.NullStringIdentifier)
            ).ToArray();
            object?[][] existingValues = actual.Rows.Select(
                r => r.ApplyTransformValues(ReplaceVariable).GetDbValues(StateManager.Configuration.NullStringIdentifier)
            ).ToArray();
            connection.CompareResults(expected.Columns, expected.TableColumns.Select(a => a.ColumnType).ToArray(),
                expectedValue, existingValues, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query for results' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connection">   The connection. </param>
        /// <param name="query">        The query. </param>
        ///
        /// <returns>   A MappedTable. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected MappedTable ExecuteQueryForResults(ITestDbConnection connection, string query)
        {
            using IDataReader reader= connection.ExecuteReader(query);
            MappedTable table = new(reader);
            reader.Close();

            LastResultSet = table;
            return table;
        }  
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the 'query for results' operation. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connectionName">   The name of the connection to run against. </param>
        /// <param name="query">        The query. </param>
        ///
        /// <returns>   A MappedTable. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected MappedTable ExecuteQueryForResults(string connectionName, string query)
        {

            using ITestDbConnection connection = OpenConnectionForName(connectionName);

            return ExecuteQueryForResults(connection, query);
        }

    }
}
