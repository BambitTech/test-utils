using Npgsql;

namespace Bambit.TestUtility.DatabaseTools.Postgres;

/// <inheritdoc />
public class PostgeSqlDatabaseCatalogRecord : DatabaseCatalogRecord
{

        
   
    /// <inheritdoc />
    public override  TestDbConnection GetConnection(string connectionString)
    {
        return new PostgreSqlTestDbConnection(new NpgsqlConnection(connectionString));
        
    }
    /// <inheritdoc />
    public override char[] Qualifiers => ['[', ']', ' '];
    /// <inheritdoc />
    public override string EscapeToken(string token)
    {
        return $"[{token}]";
    }

    
}