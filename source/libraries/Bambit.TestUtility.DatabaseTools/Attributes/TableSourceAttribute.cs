namespace Bambit.TestUtility.DatabaseTools.Attributes;

/// <summary>
/// Maps a generated class to the source database and table
/// </summary>
/// <param name="sourceConnection">The name of the connection this class is mapped to.</param>
/// <param name="sourceSchema">The schema this class is mapped to.</param>
/// <param name="sourceTable">The table this class is mapped to.</param>
[AttributeUsage(AttributeTargets.Class)]
public class TableSourceAttribute(string sourceConnection, string sourceSchema, string sourceTable)
    : Attribute
{
    /// <summary>
    /// Initializes a new instance, trimming all qualifiers from the source connection, schema and table parameters
    /// </summary>
    /// <param name="sourceConnection">The name of the connection this class is mapped to.</param>
    /// <param name="sourceSchema">The schema this class is mapped to.</param>
    /// <param name="sourceTable">The table this class is mapped to.</param>
    /// <param name="qualifers">An array of characters that should be trimmed from any of the other arguments.</param>
    public TableSourceAttribute(string sourceConnection, string sourceSchema, string sourceTable, char[] qualifers)
    : this(sourceConnection.Trim(qualifers), sourceSchema.Trim(qualifers),sourceTable.Trim(qualifers))
    {
    }
    /// <summary>
    /// The source connection name.
    /// </summary>
    public string ConnectionName { get; } = sourceConnection;
    /// <summary>
    /// The source table name.
    /// </summary>
    public string TableName { get;  } = sourceTable;
    /// <summary>
    /// The source schema name.
    /// </summary>
    public string SchemaName { get;  } = sourceSchema;
}