
namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Tools;

/// <summary>
/// State manager of databases
/// </summary>
public interface IDatabaseStateManager
{
    /// <summary>
    /// Gets the last used connection name
    /// </summary>
    string LastDatabaseConnectionName { get; set; }

    IDatabaseCatalogRecord GetCurrentConnector();
    ITestDbConnection OpenConnectionForName(string connectionName);

    DatabaseClassFactory DatabaseClassFactory { get; }
    public bool IsDebugModeOn { get; }
    Dictionary<string, object> Variables { get; }
    

}