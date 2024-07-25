using System.Data;
using System.Data.SqlClient;

namespace Bambit.TestUtility.DatabaseTools.SqlServer
{
    /// <inheritdoc />
    public class SqlServerDatabaseCatalogRecord : DatabaseCatalogRecord
    {
        public static readonly string SqlServerTableDefinitionQuery = """
                                                             select
                                                                     col.name,
                                                                     typ.name,
                                                                     case
                                                                         when col.is_nullable = 1 and typ.name in ('bigint', 'bit', 'date', 'datetime', 'datetime2', 'datetimeoffset', 'decimal', 'float', 'int', 'money', 'numeric', 'real', 'smalldatetime', 'smallint', 'smallmoney', 'time', 'tinyint', 'uniqueidentifier')
                                                                         then 1
                                                                         else 0
                                                                     end NullableSign,
                                                                     case
                                                                            when typ.name in ('nchar', 'ntext', 'nvarchar') then cast(col.max_length/2 as smallint)
                                                                         else col.max_length
                                                                         end max_length,
                                                                     case
                                                                            when typ.name  = 'smallmoney' then cast(5  as tinyint)
                                                                            else col.precision end,
                                                                     case 
                                                                        when typ.name  = 'smallmoney' or typ.name='money' then cast(2  as tinyint)
                                                                        else col.scale end,
                                                                     case 
                                                                        when typ.name = 'timestamp' then 1 
                                                                        when col.is_computed =1 then 1
                                                                        when col.generated_always_type <> 0 then 1 
                                                                        else cast(col.is_identity as int) end [Is_Computed]
                                                                 from
                                                                     sys.columns col
                                                                     join sys.types typ
                                                                         on col.system_type_id = typ.system_type_id
                                                                             AND col.user_type_id = typ.user_type_id
                                                                     where
                                                                     object_id = object_id(@tableName)
                                                             """;
        /// <inheritdoc />
        public override string TableDefinitionQuery => SqlServerTableDefinitionQuery;
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