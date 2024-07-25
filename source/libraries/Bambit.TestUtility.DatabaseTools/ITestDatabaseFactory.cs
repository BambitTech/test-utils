using System.Data;

namespace Bambit.TestUtility.DatabaseTools;

/// <summary>
/// Provides methods to get <see cref="ITestDbConnection"/> and <see cref="IDatabaseCatalogRecord"/>
/// </summary>
public interface ITestDatabaseFactory
{
    /// <summary>
    /// Event fire for any messages from the <see cref="ITestDbConnection"/>
    /// </summary>
    event EventHandler<TestDbConnectionMessageReceivedEvent>? MessageReceived;
    /// <summary>
    /// Gets a new <see cref="ITestDbConnection"/> for the specified name
    /// </summary>
    /// <param name="name">The name of the test generator to return</param>
    /// <returns></returns>
    ITestDbConnection GetConnection(string name);

    /// <summary>
    /// Returns the <see cref="IDatabaseCatalogRecord"/> for the given name
    /// </summary>
    /// <param name="name">The name of the <see cref="IDatabaseCatalogRecord"/> to retrieve</param>
    /// <returns>A <see cref="IDatabaseCatalogRecord"/> for the given name, <c>null</c> if not defined</returns>
    IDatabaseCatalogRecord GetGenerator(string name);

    /// <summary>
    /// Retrieved the defined connection string for the named <see cref="IDatabaseCatalogRecord"/>
    /// </summary>
    /// <param name="name">The name of the <see cref="IDatabaseCatalogRecord"/> to retrieve</param>
    /// <returns>A string representing the <see cref="IDbConnection.ConnectionString"/> of the <see cref="IDatabaseCatalogRecord"/></returns>
    string GetConnectionString(string name);

    /// <summary>
    /// Adds or replaces a <see cref="IDatabaseCatalogRecord"/> for the given name
    /// </summary>
    /// <param name="name">The name (identifier) of the <see cref="IDatabaseCatalogRecord"/></param>
    /// <param name="catalogRecord">The <see cref="IDatabaseCatalogRecord"/></param> to register
    void RegisterDatabaseInformation(string name, IDatabaseCatalogRecord catalogRecord);

    /// <summary>
    /// Returns a <see cref="Func{T1,TResult}"/> that can be used to clean a string, removing invalid characters
    /// </summary>
    /// <param name="name">The name of the <see cref="IDatabaseCatalogRecord"/> to that will provide the cleaning function</param>
    /// <returns>A <see cref="Func{T1,TResult}"/> that can clean a string for DB consumption</returns>
    Func<string,string> GetCleanIdentifierFunc(string name);
    
    /// <summary>
    /// Returns a <see cref="Func{T1,TResult}"/> that can be used to escape a string, removing invalid characters
    /// </summary>
    /// <param name="name">The name of the <see cref="IDatabaseCatalogRecord"/> to that will provide the escaping function</param>
    /// <returns>A <see cref="Func{T1,TResult}"/> that can clean escape for DB consumption</returns>
    Func<string,string> GetEscapeIdentifierFunc(string name);
}