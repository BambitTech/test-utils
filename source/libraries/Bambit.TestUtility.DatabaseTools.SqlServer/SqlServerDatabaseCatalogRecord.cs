using System.Data;
using System.Data.SqlClient;

namespace Bambit.TestUtility.DatabaseTools.SqlServer
{
    /// <inheritdoc />
    public class SqlServerDatabaseCatalogRecord : DatabaseCatalogRecord
    {
        /// <inheritdoc />
        public override  TestDbConnection GetConnection(string connectionString)
        {
            return new SqlServerTestDbConnection(new SqlConnection(connectionString));
        }
        /// <inheritdoc />
        public override char[] Qualifiers => ['[', ']', ' '];
        /// <inheritdoc />
        public override string EscapeToken(string token)
        {
            return $"[{token}]";
        }
    }
}