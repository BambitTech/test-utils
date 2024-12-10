using Bambit.TestUtility.DatabaseTools.Attributes;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace Bambit.TestUtility.DatabaseTools.SqlServer;

/// <summary>
/// Implementation of <see cref="ITestDbConnection"/> for SQL Server
/// </summary>
/// <param name="connection">The <see cref="IDbConnection"/> that is wrapped</param>
public class SqlServerTestDbConnection(IDbConnection connection) : TestDbConnection(connection)
{
      
        private const string SqlServerTableDefinitionQuery = """
                                                                      select
                                                                              col.name,
                                                                              typ.name,
                                                                              case
                                                                                  when col.is_nullable = 1 --and typ.name in ('bigint', 'bit', 'date', 'datetime', 'datetime2', 'datetimeoffset', 'decimal', 'float', 'int', 'money', 'numeric', 'real', 'smalldatetime', 'smallint', 'smallmoney', 'time', 'tinyint', 'uniqueidentifier')
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
    protected virtual string TableDefinitionQuery => SqlServerTableDefinitionQuery;
        
    /// <summary>   (Immutable) name of the expected table. </summary>
    protected const string ExpectedTableName = "#__TestingUtilities_expected";

    /// <summary>   (Immutable) name of the existing table. </summary>
    protected const string ExistingTableName = "#__TestingUtilities_existing";

    /// <summary>   (Immutable) name of the results table. </summary>
    protected const string ResultsTableName = "#__TestingUtilities_results";

    
   
    /// <summary>
    /// Handler for <see cref="SqlConnection.InfoMessage"/> events
    /// </summary>
    /// <param name="sender">The object that created the event</param>
    /// <param name="infoEvent">The event data</param>
    protected void InfoTrackerHandler(object sender, SqlInfoMessageEventArgs infoEvent)
    {
        OutputMessagesStore.Add(infoEvent.Message);
        AddReceivedMessage(infoEvent.Message);
    }
    /// <inheritdoc />
    public override bool TrackInfoMessages()
    {
        (Connection as SqlConnection)!.InfoMessage += InfoTrackerHandler;
        return true;
    }
    /// <inheritdoc />
    public override void UntrackInfoMessages()
    {
        (Connection as SqlConnection)!.InfoMessage -= InfoTrackerHandler;
    }

    /// <summary>
    /// Provides a method to clean an identifier, removing qualifiers
    /// </summary>
    /// <param name="input">The string to clean</param>
    /// <returns>A cleaned string</returns>
    protected static string? CleanIdentifier(string? input)
    {
        return input?.Trim("[] ".ToCharArray());
    }
    /// <inheritdoc />
   public override DataComparisonResults CompareResults( 
        
        string[] columns, 
        string?[] expectedColumnTypes, 
        IEnumerable<object?[]> expectedRows, 
        IEnumerable<object?[]> compareRows, 
        bool allowUnexpectedRows = false)
    {
        
        string columnList = string.Join(",", columns);
        
        
        using (IDbCommand command = CreateCommand())
        {
            // Create a temp table
            StringBuilder fieldsQuery = new("[  CHECK SUM COLUMN  ] int ");
            for(int x=0;x < columns.Length;x++)
            {
                string? columnType = expectedColumnTypes[x];
                string sqlType = "nvarchar(max)";
                switch (columnType)
                {
                    case "date":
                        sqlType = "DateTime";
                        break;
                    case "boolean":
                    case "bit":
                        sqlType = "bit";
                        break;
                }
                fieldsQuery.Append($"\r\n\t,  [{ CleanIdentifier(columns[x])}] {sqlType}");
            }

            string createQuery = $@"drop table if exists {ExpectedTableName}
drop table if exists {ExistingTableName}
create table {ExpectedTableName} (
{fieldsQuery}
)
create table {ExistingTableName} (
{fieldsQuery}
)

drop table if exists {ResultsTableName}
";
            command.CommandText = createQuery;
            command.ExecuteNonQuery();
        }

        InsertRecords(ExistingTableName, columns, compareRows);
        InsertRecords(ExpectedTableName, columns, expectedRows);
        return CompareExistingAndExpected(columnList, allowUnexpectedRows);
    }
    /// <inheritdoc />
    public override DataComparisonResults CompareTableToDataset(string schema, string tableName,
        string[] columns, IEnumerable<object?[]> rows, bool allowUnexpectedRows = false)
    {

        string columnList = string.Join(",", columns);
        if (Connection.State != ConnectionState.Open)
            Connection.Open();


        // 1st: Create a temp table, inserting our target fields
        string existingFieldsQuery = $@"
drop table if exists {ExistingTableName}
drop table if exists {ExpectedTableName}
drop table if exists {ResultsTableName}
-- Existing
select 
        {columnList},
        CHECKSUM({columnList}) [  CHECK SUM COLUMN  ]
    into {ExistingTableName} 
    from    [{CleanIdentifier(schema)}].[{CleanIdentifier(tableName)}]
-- Expected
select 
        {columnList},
        CHECKSUM({columnList}) [  CHECK SUM COLUMN  ]
    into {ExpectedTableName} 
    from [{CleanIdentifier(schema)}].[{CleanIdentifier(tableName)}] 
    where 1  = 0
union all 
select 
        {columnList},
        CHECKSUM({columnList}) [  CHECK SUM COLUMN  ]
    from [{CleanIdentifier(schema)}].[{CleanIdentifier(tableName)}] 
    where 1  = 0

";
        using (IDbCommand command =CreateCommand())
        {
            command.CommandText = existingFieldsQuery;
            command.ExecuteNonQuery();
        }

        InsertRecords(ExpectedTableName, columns, rows);

        return CompareExistingAndExpected(columnList, allowUnexpectedRows);
    }

    private void InsertRecords(string tableName, string[] columns, IEnumerable<object?[]> rows)
    {
        string columnList = string.Join(",", columns);
        List<string> cleanedColumnList = [];
        for (int x = 0; x < columns.Length; x++)
            cleanedColumnList.Add($"p{x}");
        string variableList = $"@{string.Join(",@", cleanedColumnList)}";

        
        string query = $"insert into {tableName} ({columnList}) values ({variableList})";
        foreach (object?[] row in rows)
        {
            if (row.Length < columns.Length)
                throw new ArgumentOutOfRangeException( nameof(rows),
                    $"Row is missing columns (Expected {columns.Length}, actual {row.Length} ");
            using IDbCommand insertCommand = CreateCommand();
            

            insertCommand.CommandText = query;
            for (int x = 0; x < cleanedColumnList.Count; x++)
            {

                object? value = row[x];
                IDbDataParameter dataParameter = insertCommand.CreateParameter();
                dataParameter.ParameterName = $"@{cleanedColumnList[x]}";
                dataParameter.Value = value ?? DBNull.Value;
                insertCommand.Parameters.Add(dataParameter);
            }

            insertCommand.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Generates a human readable result table string for the given data table
    /// </summary>
    /// <param name="dataTable"></param>
    /// <returns></returns>
    protected static string GeneratePrintableResults(DataTable dataTable)
    {
        List<string> formats = [];
        StringBuilder results = new("\r\n");
        for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
        {
            string columnName = dataTable.Columns[colIndex].ColumnName;
            if (columnName == " __Indicator Column __ ")
            {
                columnName = "(M)atch / Un(E)xpected /Equals (=)";
            }

            int longestString = dataTable.Rows.OfType<DataRow>()
                .Select(m => m.Field<object>(colIndex)
                    ?.ToString()
                    ?.Length 
                             ?? 0
                )
                .Max(c => c);

            if (columnName.Length > longestString)
            {
                longestString = columnName.Length;

            }

            string formatString = $"| {{0,-{longestString}}} ";
            formats.Add(formatString);

            results.AppendFormat(formatString, columnName);
        }

        results.Append("|\r\n");

        foreach (DataRow row in dataTable.Rows)
        {
            for (int index = 0; index < formats.Count; index++)
            {
                string value = row[index] ==
                               DBNull.Value ? "null" 
                    : row[index].ToString() 
                      ?? "null";
               
                results.AppendFormat(formats[index], value);
            }

            results.Append("|\r\n");
        }

        return results.ToString();
    }

    /// <summary>
    /// Gets teh mapped DB type
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    protected static SqlDbType? GetDbType(PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetCustomAttributes(typeof(FieldSourceAttribute)).FirstOrDefault() is not FieldSourceAttribute attribute)
        {
            return null;
        }

        return attribute.SourceType.ToLower() switch
        {
            "image" => SqlDbType.Image,
            "binary" => SqlDbType.Binary,
            "varbinary" => SqlDbType.VarBinary,
            _ => null
        };
    }
    /// <inheritdoc />
    public override void Persist(DatabaseMappedClass mappedClass)
    {
        Type generatedType = mappedClass.GetType();
        if (Attribute.GetCustomAttribute(generatedType, typeof(TableSourceAttribute)) is not TableSourceAttribute tableSourceAttribute)
        {
            throw new ArgumentException("Supplied argument is not a table generated class", nameof(mappedClass));
        }

        StringBuilder builder =
            new($"Insert into [{tableSourceAttribute.SchemaName}].[{tableSourceAttribute.TableName}] (");

        StringBuilder args = new(" values (");
        PropertyInfo[] propertyInfos = generatedType.GetProperties(BindingFlags.Instance | BindingFlags.GetField |
                                                                   BindingFlags.SetField | BindingFlags.Public);
        int argNumber = 0;

        if (Connection.State != ConnectionState.Open)
        {
            Connection.Open();
        }

        using SqlCommand command = (CreateCommand() as SqlCommand)!;
        
        {
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetCustomAttributes(typeof(FieldSourceAttribute)).FirstOrDefault() is not FieldSourceAttribute fieldSource)
                    continue;
                if (propertyInfo.GetCustomAttributes(typeof(ComputedColumnAttribute)).Any())
                    continue;
                string argName = $"@{propertyInfo.Name}";
                string spacer = argNumber % 10 == 0 ? "\n" : " ";
                object? value = propertyInfo.GetValue(mappedClass);
                builder.Append($"{spacer}[{fieldSource.Name}] /* {value ?? DBNull.Value} {argName} */,");

                args.Append($"{spacer}{argName},");
                SqlDbType? dbType = GetDbType(propertyInfo);
                object sqlValue = value ?? DBNull.Value;
                SqlParameter parameter;
                if (dbType.HasValue)
                {
                    parameter = new SqlParameter(argName, dbType.Value)
                    {
                        Value = sqlValue
                    };
                }
                else 
                    parameter=new SqlParameter(argName,sqlValue );
                command.Parameters.Add(parameter);

                argNumber++;
            }

            string commandText = $"{builder.ToString().TrimEnd(',')} )\n {args.ToString().TrimEnd(',')} )";
            command.CommandText = commandText;
            
            command.ExecuteNonQuery();
        }

    }

    /// <inheritdoc />
    public override string GenerateRenameTableScript(string schema, string oldName, string newName)
    {
        return $"sp_Rename '{ schema}.{oldName}', '{newName}'";
    }

    /// <summary>
    /// Runs a comparison query against the 2 temp tables, <see cref="ExistingTableName"/> and <see cref="ExpectedTableName"/>
    /// </summary>
    /// <param name="columnList">The columns to compare</param>
    /// <param name="allowUnexpectedRows">Whether to treat unexpected rows in the <see cref="ExistingTableName"/> as an error or not</param>
    /// <returns></returns>
    protected DataComparisonResults CompareExistingAndExpected(string columnList, bool allowUnexpectedRows = false)
    {
        // Now we match
        // NOTE: This logic was borrowed from tSqlt (https://github.com/tSQLt-org/tSQLt/blob/main/Source/tSQLt.Private_CompareTables.ssp.sql) which is licensed under apache 2.0

        string matchQuery = $@"

select '=' [ __Indicator Column __ ], {columnList} into {ResultsTableName}
    from {ExistingTableName}  where 1 = 0
INSERT INTO  {ResultsTableName}  ([ __Indicator Column __ ] , {columnList}) 
    SELECT 
      CASE 
        WHEN RestoredRowIndex.[ __Indicator Column __ ] <= CASE WHEN [ _Expected Source_ ]<[ _ActualSource_ ] THEN [ _Expected Source_ ] ELSE [ _ActualSource_ ] END
         THEN '=' 
        WHEN RestoredRowIndex.[ __Indicator Column __ ] <= [ _Expected Source_ ] 
         THEN 'M' 
        ELSE 'X' 
      END AS ind,{columnList}
    FROM(
      SELECT SUM([ _Expected Source_ ]) AS [ _Expected Source_ ], 
             SUM([ _ActualSource_ ]) AS [ _ActualSource_ ], 
             {columnList}
      FROM (
        SELECT 1 AS [ _Expected Source_ ], 0[ _ActualSource_ ], {columnList}
          FROM {ExpectedTableName} 
        UNION ALL 
        SELECT 0[ _Expected Source_ ], 1 AS [ _ActualSource_ ], {columnList}
          FROM {ExistingTableName}
      ) AS X 
      GROUP BY {columnList}
    ) AS CollapsedRows
    CROSS APPLY (
       SELECT TOP(CASE WHEN [ _Expected Source_ ]>[ _ActualSource_ ] THEN [ _Expected Source_ ] 
                       ELSE [ _ActualSource_ ] END) 
              ROW_NUMBER() OVER(ORDER BY(SELECT 1)) 
         FROM (SELECT 1 
                 FROM {ExistingTableName} UNION ALL SELECT 1 FROM {ExpectedTableName} ) X(X)
              ) AS RestoredRowIndex([ __Indicator Column __ ]);


select sum(case when [ __Indicator Column __ ] = '=' then 1 else 0 end) [Matching],
    sum(case when [ __Indicator Column __ ] = 'M' then 1 else 0 end) [Missing],
sum(case when [ __Indicator Column __ ] = 'X' then 1 else 0 end) [extra]
from  {ResultsTableName}

                select * from {ResultsTableName}
"
            ;

        using IDbCommand command = CreateCommand();
        command.CommandText = matchQuery;
        using IDataReader reader = command.ExecuteReader();
        reader.Read();
        if (reader.RecordsAffected == 0)
            return ITestDbConnection.Success;
        int missing = reader.GetInt32(1);
        int extra = reader.GetInt32(2);
        //int notAtAll = reader.GetInt32(2);
        if (allowUnexpectedRows && missing == 0)
            return ITestDbConnection.Success;
        if (missing <= 0 && extra <= 0) return ITestDbConnection.Success;
        reader.NextResult();
        DataTable results = new();
        results.Load(reader);
        string printableResults = GeneratePrintableResults(results);
        AddReceivedMessage("Result Table:");
        AddReceivedMessage(printableResults);
        return new DataComparisonResults(false, missing, extra);


    }

    /// <inheritdoc />
    public override IList<DatabaseMappedClassPropertyDefinition> GetProperties( string schema, string tableName)
    {

        List<DatabaseMappedClassPropertyDefinition> properties = [];
        Connection.Open();
        using IDbCommand command = Connection.CreateCommand();

        command.CommandText = TableDefinitionQuery;
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = "@tableName";
        parameter.Value = $"{schema}.{tableName}";
        command.Parameters.Add(parameter);
        {
            using IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                DatabaseMappedClassPropertyDefinition definition = new()
                {
                    Name = reader.GetString(0),
                    SourceType=reader.GetString(1),
                    IsNullable = reader.GetInt32(2) == 1,
                    MaxSize = reader.GetInt16(3),
                    Precision = reader.GetByte(4),
                    Scale = reader.GetByte(5),
                    IsComputed = reader.GetInt32(6) > 0
                };
                definition.MappedType = GetPropertyType(definition.SourceType, definition.IsNullable);
                properties.Add(
                    definition
                );
            }
        }



        return properties;

    }

    

    private static Type? GetPropertyType(string sourceType, bool isNullable)
    {
        Type? type;
        switch (sourceType)
        {
            case "bigint":
            case "timestamp":
            case "long":
                type = isNullable ? typeof(long?) : typeof(long);
                break;
            case "image":
            case "binary":
            case "varbinary":
            case "bytearray":
                type = typeof(byte[]);
                break;
            case "bit":
            case "boolean":
            case "bool":
                type = isNullable ? typeof(bool?) : typeof(bool);
                break;
            case "date":
            case "datetime":
            case "datetime2":
            case "smalldatetime":
                type = isNullable ? typeof(DateTime?) : typeof(DateTime);
                break;
            case "datetimeoffset":
                type = isNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
                break;
            case "money":
            case "numeric":
            case "smallmoney":
            case "decimal":
                type = isNullable ? typeof(decimal?) : typeof(decimal);
                break;
            case "real":
            case "float":
            case "double":
                type = isNullable ? typeof(double?) : typeof(double);
                break;
            case "int":
                type = isNullable ? typeof(int?) : typeof(int);
                break;
            case "nchar":
            case "ntext":
            case "nvarchar":
            case "varchar":
            case "text":
            case "char":
            case "xml":
                type = typeof(string);
                break;
            case "smallint":
            case "short":
                type = isNullable ? typeof(short?) : typeof(short);
                break;
            case "time":
            case "timespan":
                type = isNullable ? typeof(TimeSpan?) : typeof(TimeSpan);
                break;
            case "tinyint":
            case "byte":
                type = isNullable ? typeof(byte?) : typeof(byte);
                break;
            case "uniqueidentifier":
            case "guid":
                type = isNullable ? typeof(Guid?) : typeof(Guid);
                break;
            default:
                type = null;
                break;
        }

        return type;
    }

}
