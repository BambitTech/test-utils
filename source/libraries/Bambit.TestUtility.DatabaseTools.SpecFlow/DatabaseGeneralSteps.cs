using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    [Binding]
    public class DatabaseGeneralSteps(ScenarioContext context, ISpecFlowOutputHelper outputHelper) : DatabaseSteps(context, outputHelper)
    {

        [Given(@"I am working in the (?<connectionName>.*) database")]
        public void GivenIAmWorkingInTheDatabase(string connectionName)
        {
            StateManager.LastDatabaseConnectionName = connectionName;
        }

        
        [Given(@"I have a (string|date) variable named '(\$.*)' defined from:")]
        public void WhenIHaveAStringVariableNamedDefinedFrom(string variableType, string variableName, string query)
        {
            using ITestDbConnection connection= StateManager.OpenConnectionForName(LastDatabaseConnectionName);

            switch (variableType.ToLower())
            {
                case "date":
                    Variables[variableName] = connection.ExecuteScalar<DateTime>(query);
                    break;
                default:
                    Variables[variableName] = connection.ExecuteScalar<string>(query);
                    break;

            }
            
        }

        
    }
}
