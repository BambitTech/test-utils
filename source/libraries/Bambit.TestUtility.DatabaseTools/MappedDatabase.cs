namespace Bambit.TestUtility.DatabaseTools;


/// <summary>
/// Represents a mapping of a connection string to a <see cref="DatabaseCatalog"/> by name
/// </summary>
/// <param name="ConnectionString">The ConnectionString</param>
/// <param name="DatabaseCatalog">The name of the <see cref="DatabaseCatalog"/></param>
public record MappedDatabase(string ConnectionString, string DatabaseCatalog);
