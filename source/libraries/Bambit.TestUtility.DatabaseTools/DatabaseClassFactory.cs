namespace Bambit.TestUtility.DatabaseTools;

/// <summary>
/// Factory to generate classes from a database
/// </summary>
/// <param name="tableToClassBuilder">The <see cref="ITableToClassBuilder"/> that will be used to generate the class</param>
public class DatabaseClassFactory( ITableToClassBuilder tableToClassBuilder )
{
    
    /// <summary>
    /// A <see cref="Dictionary{TKey,TValue}"/> used to cached table definitions
    /// </summary>

    private readonly Dictionary<string, Dictionary<string, TableDefinition>> TableDefinitions =
        new(StringComparer.CurrentCultureIgnoreCase);

    /// <summary>
    /// The <see cref="ITableToClassBuilder"/> that will be used to generate the objate
    /// </summary>
    protected ITableToClassBuilder TableToClassBuilder { get; } = tableToClassBuilder;
    
    class TableDefinition(Type objectTypeDefinition)
    {
        public  Type ObjectTypeDefinition { get; init; } = objectTypeDefinition;
    }

    /// <summary>
    /// Removes the cached table definition 
    /// </summary>
    /// <param name="connectionName">The name of the connection this table is generated for</param>
    /// <param name="schemaName">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    public void PurgeTableDefinition(string connectionName, string schemaName, string tableName)
    {
        lock (TableDefinitions)
        {
            if (!TableDefinitions.TryGetValue(connectionName, out var dbDictionary))
                return;
            string tableKey = $"{schemaName}.{tableName}";
            if (!dbDictionary.ContainsKey(tableKey))

                return;
            dbDictionary.Remove(tableKey);
        }
    }

    /// <summary>
    /// Gets the cached type for the table, otherwise creates a new one
    /// </summary>
    /// <param name="connectionName">The name of the connection this table is generated for</param>
    /// <param name="schemaName">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <returns></returns>
    public Type GetClassTypeFromTable(string connectionName, string schemaName, string tableName)
    {
        lock (TableDefinitions)
        {
            if (!TableDefinitions.ContainsKey(connectionName))
            {
                TableDefinitions.Add(connectionName,
                    new Dictionary<string, TableDefinition>(StringComparer.CurrentCultureIgnoreCase));
            }
        }

        Dictionary<string, TableDefinition> dbDictionary = TableDefinitions[connectionName];
        string tableKey = $"{schemaName}.{tableName}";
        if (!dbDictionary.ContainsKey(tableKey))
        {
            dbDictionary.Add(tableKey, CreateTableDefinition(connectionName, schemaName, tableName));
        }

        return dbDictionary[tableKey].ObjectTypeDefinition;

    }

    /// <summary>
    /// Initializes a new object for the supplied table
    /// </summary>
    /// <param name="connectionName">The name of the connection this table is generated for</param>
    /// <param name="schemaName">The schema of the table</param>
    /// <param name="tableName">The name of the table</param>
    /// <returns>A new object for the supplied table</returns>
    public DatabaseMappedClass GenerateObjectFromTable(string connectionName, string schemaName, string tableName)
    {

        Type type = GetClassTypeFromTable(connectionName, schemaName, tableName);
        return
            (Activator.CreateInstance(type) as DatabaseMappedClass)!;
    }

    private  TableDefinition CreateTableDefinition(string connectionName, string schemaName, string tableName)
    {
        return new TableDefinition(
                TableToClassBuilder.GenerateClassTypeFromTable(connectionName, schemaName, tableName)
        );
    }
}