using System.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    /// <summary>
    /// Provides step to mock objects
    /// </summary>
    /// <param name="context">The Scenario Context</param>
    /// <param name="outputHelper">A SpecFlow output helper</param>
    [Binding]
    public class MockObjectSteps(ScenarioContext context, ISpecFlowOutputHelper outputHelper) : DatabaseSteps(context, outputHelper)
    {
        /// <summary>
        /// Create a copy of a table w/o any constraints or foreign keys, with all fields nullable
        /// </summary>
        /// <param name="connectionName">The name of the conneciton that the table can be found in</param>
        /// <param name="schema">The schema the table belongs to </param>
        /// <param name="name">Tne name of the table</param>
        /// <returns>A <see cref="MockedObject"/> representign the table</returns>
        public MockedObject MockTable(string connectionName, string schema, string name)
        {

            MockedObject mockedObject = new()
            {
                DatabseObjectType = DatabseObjectType.Table, ConnectionName = connectionName, OriginalName = $"{name}",
                OriginalSchema = $"{schema}"
            };
            using ITestDbConnection connection = OpenConnectionForName(connectionName);
            using IDbCommand command = connection.CreateCommand();
            mockedObject.NewName= $"{name}_{DateTime.Now.Ticks}";
            mockedObject.RestoreScripts = new List<string>(new[]
            {
                $@"Drop table  if exists {schema}.{name}",
                connection.GenerateRenameTableScript(schema, mockedObject.NewName, name)// "sp_Rename '[{schema}].{mockedObject.NewName}', '{name}'"
            });
            
                string query = $@"SELECT count(1) FROM information_schema.columns WHERE table_schema = '{schema}' and table_name = '{name}'";
                command.CommandText = query;
                int numCols = (int )command.ExecuteScalar()!;

                command.CommandText = connection.GenerateRenameTableScript(schema, name, mockedObject.NewName);// $@"sp_Rename '{schema}.{name}', '{mockedObject.NewName}' ";
                command.ExecuteNonQuery();

                command.CommandText = $@"select top 1 *
                    into {schema}.{name}
                from {schema}.{mockedObject.NewName}
                where 1 = 0
union select {string.Join(",",Enumerable.Repeat("null",numCols))}
         from {schema}.{mockedObject.NewName}
                where 1 = 0           
";
                command.ExecuteNonQuery();

            context.Get<DatabaseClassFactory>().PurgeTableDefinition(connectionName, mockedObject.OriginalSchema, mockedObject.OriginalName);

            return mockedObject;
        }
        
        [Given(@"the table (?<schema>.*)\.(?<name>.*) is mocked in the (?<connectionName>.*) database without constraints")]
        public void GivenTheTableIsMockedWithoutConstraints(string schema, string name, string connectionName)
        {
            using ITestDbConnection connection = OpenConnectionForName(connectionName);

            AddMockedObject(MockTable(connectionName, schema, name));
        }
        

        [Given(@"the table (?<schema>.*)\.(?<name>.*) is mocked without constraints")]
        public void GivenTheTableIsMockedWithoutConstraints(string schema, string name)
        {
            GivenTheTableIsMockedWithoutConstraints(schema, name, LastDatabaseConnectionName);
        }

    }
}
