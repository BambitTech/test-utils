namespace Bambit.TestUtility.DatabaseTools;

/// <summary>
/// Provides methods to dynamically generate a class from a table definition
/// </summary>
public interface ITableToClassBuilder
{
    /// <summary>
    /// Creates a <see cref="Type"/> for a specified database object
    /// </summary>
    /// <param name="catalogRecordName">The name of the <see cref="IDatabaseCatalogRecord"/> to use</param>
    /// <param name="schemaName">The name of the schema the table belongs to</param>
    /// <param name="tableName">The table to create a class for</param>
    /// <returns>A <see cref="Type"/> of a class that maps to the given table</returns>
    Type GenerateClassTypeFromTable(string catalogRecordName, string schemaName, string tableName);

    /// <summary>
    /// Creates a new, empty object (default property values) for the specified database object;
    /// </summary>
    /// <param name="catalogRecordName">The name of the <see cref="IDatabaseCatalogRecord"/> to use</param>
    /// <param name="schemaName">The name of the schema the table belongs to</param>
    /// <param name="tableName">The table to create a class for</param>
    /// <returns>A new object representing the supplied database object</returns>
    DatabaseMappedClass? GenerateObjectFromTable(string catalogRecordName, string schemaName, string tableName);
}