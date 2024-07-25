namespace Bambit.TestUtility.DatabaseTools;

/// <summary>
/// Exposes methods for a particular <see cref="ITestDbConnection"/>
/// </summary>
public interface IDatabaseCatalogRecord
{
    #region Properties
    /// <summary>
    /// Query string that will be used to map a table to the C# classes
    /// </summary>
    /// <remarks>
    /// The query will have a variable "@tableName" supplied when it is run.
    /// The query must have the following fields returned in order:
    /// 1: String field with the name of the column
    /// 2: The column type.  Must be any of the following: bigint, binary, bit, bool, boolean, byte, bytearray, char, date, datetime, datetime2, datetimeoffset, decimal, double, float, guid, image, int, long, money, nchar, ntext, numeric, nvarchar, real, short, smalldatetime, smallint, smallmoney, text, time, timespan, timestamp, tinyint, uniqueidentifier, varbinary, varchar, xml
    /// 3: An integer indicating nullablilty of the field.  0 is non-null, anything else is nullable
    /// 4: An integer representing the max size (numeric, byte, string)
    /// 5: A small int (byte) representing the precision for floating types
    /// 6: A small int(byte) representing the scale for floating types
    /// 7: An integer indicating if the column is computed. 0 is not computed, anything else is computed
    /// </remarks>
    string TableDefinitionQuery { get; }

    
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