using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A database general steps. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="context">      The context. </param>
    /// <param name="outputHelper"> The output helper. </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    [Binding]
    public class DatabaseGeneralSteps(ScenarioContext context, ISpecFlowOutputHelper outputHelper) : DatabaseSteps(context, outputHelper)
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets current database. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="connectionName">   Name of the connection. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Given(@"I am working in the (?<connectionName>.*) database")]
        public void SetCurrentDatabase(string connectionName)
        {
            StateManager.LastDatabaseConnectionName = connectionName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Define variable. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="variableType"> Type of the variable. </param>
        /// <param name="variableName"> Name of the variable. </param>
        /// <param name="query">        The query. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [Given(@"I have a (string|date) variable named '(\$.*)' defined from:")]
        public void DefineVariable(string variableType, string variableName, string query)
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
