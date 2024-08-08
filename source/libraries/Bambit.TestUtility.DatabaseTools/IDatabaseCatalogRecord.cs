namespace Bambit.TestUtility.DatabaseTools;

/// <summary>
/// Exposes methods for a particular <see cref="ITestDbConnection"/>
/// </summary>
public interface IDatabaseCatalogRecord
{
    #region Properties
    
    
    /// <summary>
    /// An array of characters that can be used as qualifiers for database objects
    /// </summary>
    char[] Qualifiers { get; }
    #endregion Properties

    #region Methods
    
    
    /// <summary>
    /// Removes all qualifiers from the front and back of the supplied input string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    string CleanQualifiers(string input);

    /// <summary>
    /// Wraps a token with qualifiers
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    string EscapeToken(string token);
    /// <summary>
    /// Cleans then escapes the supplied token
    /// </summary>
    /// <param name="token">The token to </param>
    /// <returns></returns>
    string CleanAndEscapeToken(string token);
    /// <summary>
    /// Returns an <see cref="ITestDbConnection"/> for the supplied connection string
    /// </summary>
    /// <param name="connectionString">The connection string</param> to create the connection from .
    /// <returns>A <see cref="ITestDbConnection"/></returns>
    ITestDbConnection GetConnection(string connectionString);

    #endregion Methods

    


}