using System.Data;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Extensions;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Transforms;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    public class DatabaseSteps(ScenarioContext context, ISpecFlowOutputHelper outputHelper) : BaseSteps(context)
    {
        protected  ISpecFlowOutputHelper OutputHelper { get; }=outputHelper;
        
        
        protected string LastDatabaseConnectionName
        {
            get => StateManager.LastDatabaseConnectionName;
            set => StateManager.LastDatabaseConnectionName = value;
        }
        protected ITestDbConnection OpenConnectionForName(string connectionName)
        {
            return StateManager.OpenConnectionForName(connectionName);
        }

        protected IDatabaseCatalogRecord GetCurrentConnector()
        {
            return StateManager.GetCurrentConnector();
        }
        
        public void CompareTableToDataset(string schema, string tableName,
            string connectionName, MappedTable data, bool allowUnexpectedRows)
        {
            
            using ITestDbConnection connection = OpenConnectionForName(connectionName);
            object?[][] values = data.Rows.Select(
                r => r.ApplyTransformValues(ReplaceVariable).ExtractValues(StateManager.Configuration.NullStringIdentifier, ReplaceVariable)
            ).ToArray();
            VerifyCompareResults(connection.CompareTableToDataset(schema, tableName, data.Columns, values,
                allowUnexpectedRows));

            StateManager.LastDatabaseConnectionName = connectionName;
        }

        
        
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

        
        public IDbCommand CreateCommand(IDbConnection connection)
        {
            IDbCommand command = connection.CreateCommand();
            if (Configuration.TimeoutSeconds.HasValue)
                command.CommandTimeout = Configuration.TimeoutSeconds.Value;
            return command;
        }

        public void ExecuteQuery(IDbConnection connection, string query)
        {
            
            using IDbCommand command = CreateCommand(connection);
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }

    }
}
