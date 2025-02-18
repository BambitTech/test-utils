using Reqnroll;

namespace Bambit.TestUtility.DatabaseTools.Reqnroll;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   A database general steps. </summary>
///
/// <remarks>   Law Metzler, 7/25/2024. </remarks>
///
/// <param name="context">      The context. </param>
/// <param name="outputHelper"> The output helper. </param>
////////////////////////////////////////////////////////////////////////////////////////////////////
[Binding]
public class DatabaseGeneralSteps(ScenarioContext context, IReqnrollOutputHelper outputHelper) : DatabaseSteps(context, outputHelper)
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Sets current database. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="connectionName">   Name of the connection. </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    [Given(@"^I am working in the (?<connectionName>.*) database$")]
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

    [Given(@"^I have a (?<variableType>string|date) variable named '(?<variableName>\$.*)' defined from:$")]
    public void SetVariableFromQuery(string variableType, string variableName, string query)
    {
        using ITestDbConnection connection= StateManager.OpenConnectionForName(LastDatabaseConnectionName);

        Variables[variableName] = variableType.ToLower() switch
        {
            "date" => connection.ExecuteScalar<DateTime>(query),
            _ => connection.ExecuteScalar<string>(query)
        };
    }

    /// <summary>
    /// Sets a variable with a value
    /// </summary>
    /// <param name="variableType"> Type of the variable. Either "date" or "string" </param>
    /// <param name="variableName"> Name of the variable. </param>
    /// <param name="value">        The value. </param>
    [Given(@"^I have a (?<variableType>string|date) variable named '(?<variableName>\$.*)' with a value of '(?<value>.*)'$")]
    public void SetVariable(string variableType, string variableName, string value) {
        Variables[variableName] = variableType.ToLower() switch
        {
            "date" =>DateTime.Parse(value),
            _ => value
        };
    }
}