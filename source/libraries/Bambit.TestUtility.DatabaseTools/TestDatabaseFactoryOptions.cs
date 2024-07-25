namespace Bambit.TestUtility.DatabaseTools;

/// <summary>
/// Options for the <see cref="TestDatabaseFactory"/> class
/// </summary>
public class TestDatabaseFactoryOptions
{
    /// <summary>
    /// A nap of strings <see cref="Dictionary{TKey,TValue}"/> of <see cref="MappedDatabase"/> by Key
    /// </summary>
    public Dictionary<string, MappedDatabase> MappedDatabases { get; set; } = new();
    /// <summary>
    /// A map of names to <see cref="Type"/> strings that will instantiate the <see cref="MappedDatabase"/>
    /// </summary>
    public Dictionary<string, string> DatabaseCatalogRecordMap { get; set; } = new();
}