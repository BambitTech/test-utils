using System.Collections;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using Bambit.TestUtility.DatabaseTools.Attributes;
using Npgsql;
using NpgsqlTypes;

namespace Bambit.TestUtility.DatabaseTools.Postgres;

/// <summary>
/// Implementation of <see cref="ITestDbConnection"/> for SQL Server
/// </summary>
/// <param name="connection">The <see cref="IDbConnection"/> that is wrapped</param>
public class PostgreSqlTestDbConnection(IDbConnection connection) : TestDbConnection(connection)
{

    private const string PostgreSqlTableDefinitionQuery = """
                                                          select
                                                                    column_name,
                                                                    data_type,
                                                                    case
                                                                        when is_nullable = 'YES' and data_type in ('bigint', 'bit', 'date', 'datetime', 'datetime2', 'datetimeoffset', 'decimal', 'float', 'int', 'money', 'numeric', 'real', 'smalldatetime', 'smallint', 'smallmoney', 'time', 'tinyint', 'uniqueidentifier')
                                                                        then 1
                                                                        else 0
                                                                    end NullableSign,
                                                                    coalesce(character_maximum_length, 0) max_length,
                                                                     coalesce(numeric_precision, 0) numeric_scale,
                                                                    coalesce(numeric_scale, 0) numeric_scale,
                                                                    case
                                                                       when IS_GENERATED = 'YES' then 1
                                                               --        when is_computed =1 then 1
                                                                       --when enerated_always_type <> 0 then 1
                                                                        when is_identity = 'YES' then 1
                                                          			 else 0 end Is_Computed
                                                                from
                                                                   information_schema.columns
                                                                    
                                                          WHERE table_name= @tableName and table_schema = @tableSchema
                                                          """;

    /// <summary>
    /// Query string that will be used to map a table to the C# classes
    /// </summary>
    /// <remarks>
    /// </remarks>
    protected virtual string TableDefinitionQuery => PostgreSqlTableDefinitionQuery;

    /// <summary>   (Immutable) name of the expected table. </summary>
    protected const string ExpectedTableName = "__TestingUtilities_expected";

    /// <summary>   (Immutable) name of the existing table. </summary>
    protected const string ExistingTableName = "__TestingUtilities_existing";

    /// <summary>   (Immutable) name of the results table. </summary>
    protected const string ResultsTableName = "__TestingUtilities_results";


    /// <summary>
    /// Handler for <see cref="NpgsqlConnection.Notification"/> events
    /// </summary>
    /// <param name="sender">The object that created the event</param>
    /// <param name="infoEvent">The event data</param>
    protected void InfoTrackerHandler(object sender, NpgsqlNotificationEventArgs infoEvent)
    {
        string message = $"{infoEvent.Channel}|{infoEvent.Payload}";
        OutputMessagesStore.Add(message);
        AddReceivedMessage(message);
    }

    /// <inheritdoc />
    public override bool TrackInfoMessages()
    {
        (Connection as NpgsqlConnection)!.Notification += InfoTrackerHandler;
        return true;
    }


    /// <inheritdoc />
    public override void UntrackInfoMessages()
    {
        (Connection as NpgsqlConnection)!.Notification -= InfoTrackerHandler;
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
            StringBuilder fieldsQuery = new("CHECKSUMCOLUMN varchar ");
            for (int x = 0; x < columns.Length; x++)
            {
                string? columnType = expectedColumnTypes[x];
                string sqlType = "varchar";
                switch (columnType)
                {
                    case "date":
                        sqlType = "timestamp";
                        break;
                    case "boolean":
                    case "bit":
                        sqlType = "boolean";
                        break;
                }

                fieldsQuery.Append($"\r\n\t,  {CleanIdentifier(columns[x])} {sqlType}");
            }

            string createQuery = $@"drop table if exists {ExpectedTableName};
drop table if exists {ExistingTableName};
create  table {ExpectedTableName} (
{fieldsQuery}
);
create  table {ExistingTableName} (
{fieldsQuery}
);

drop table if exists {ResultsTableName};
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
drop table if exists {ExistingTableName};
drop table if exists {ExpectedTableName};
drop table if exists {ResultsTableName};


CREATE FUNCTION pg_temp.__testingutilities_agg_sfunc(text, anyelement) 
       RETURNS text
       LANGUAGE sql
 AS
$$
   SELECT md5($1 || $2::text);
$$
;
CREATE  AGGREGATE pg_temp.__testingutilities_agg (ORDER BY anyelement)
(
  STYPE = text,
  SFUNC = pg_temp.__testingutilities_agg_sfunc,
  INITCOND = ''
);

-- Existing
select * 
    into {ExistingTableName} 
   from    {CleanIdentifier(schema)}.{CleanIdentifier(tableName)}
/*    from (
        SELECT    {columnList}, pg_temp.__testingutilities_agg()  WITHIN GROUP  (ORDER BY __testingutilities_expected) as CHECKSUMCOLUMN
            from    {CleanIdentifier(schema)}.{CleanIdentifier(tableName)}
    )*/
    ;

-- Expected
select 
        {columnList}
--,        CHECKSUM({columnList}) CHECKSUMCOLUMN
    into {ExpectedTableName} 
    from {CleanIdentifier(schema)}.{CleanIdentifier(tableName)}
    where 1  = 0
union all 
select 
        {columnList}
--,        CHECKSUM({columnList}) CHECKSUMCOLUMN
    from {CleanIdentifier(schema)}.{CleanIdentifier(tableName)}
    where 1  = 0;

";
        using (IDbCommand command = CreateCommand())
        {
            command.CommandText = existingFieldsQuery;
            command.ExecuteNonQuery();
        }

        InsertRecords(ExpectedTableName, columns, rows);

        return CompareExistingAndExpected(columnList, allowUnexpectedRows);
    }

    private void InsertRecords(string tableName, string[] columns, IEnumerable<object?[]> rows,string?[]? expectedColumnTypes =null)
    {
        string columnList = string.Join(",", columns);
        List<string> cleanedColumnList = [];
        for (int x = 0; x < columns.Length; x++)
            cleanedColumnList.Add($"p{x}");
        string variableList = $"@{string.Join(",@", cleanedColumnList)}";

        

        string query = $"""

                        insert into {tableName} ({columnList}) values ({variableList})

                        """;
        foreach (object?[] row in rows)
        {
            if (row.Length < columns.Length)
                throw new ArgumentOutOfRangeException(nameof(rows),
                    $"Row is missing columns (Expected {columns.Length}, actual {row.Length} ");
            using NpgsqlCommand insertCommand = (CreateCommand() as NpgsqlCommand)!;


            insertCommand.CommandText = query;
            for (int x = 0; x < cleanedColumnList.Count; x++)
            {
                object? value =  row[x];
                NpgsqlParameter parameter;
                NpgsqlDbType? dbType=null;
                string argName=$"@{cleanedColumnList[x]}";
                if (expectedColumnTypes?[x]!=null)
                    dbType  =GetDatabasePropertyType(expectedColumnTypes[x]!);
                
                if(dbType.HasValue)
                {
                    if (value is string s)
                        value = ConverterToType(expectedColumnTypes![x]!, s);
                    parameter = new NpgsqlParameter(argName, dbType.Value)
                    {
                        Value =  value ?? DBNull.Value
                    };
                }
                else
                    parameter = new NpgsqlParameter(argName,  value ?? DBNull.Value);

                insertCommand.Parameters.Add(parameter);

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
            if (columnName == " __IndicatorColumn__ ")
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
                               DBNull.Value
                    ? "null"
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
    protected static NpgsqlDbType? GetDbType(PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetCustomAttributes(typeof(FieldSourceAttribute)).FirstOrDefault() is not FieldSourceAttribute
            attribute)
        {
            return null;
        }

        return GetDatabasePropertyType(attribute.SourceType);
    }

    

    /// <inheritdoc />
    public override void Persist(DatabaseMappedClass mappedClass)
    {
        Type generatedType = mappedClass.GetType();
        if (Attribute.GetCustomAttribute(generatedType, typeof(TableSourceAttribute)) is not TableSourceAttribute
            tableSourceAttribute)
        {
            throw new ArgumentException("Supplied argument is not a table generated class", nameof(mappedClass));
        }

        StringBuilder builder =
            new($"Insert into {tableSourceAttribute.SchemaName}.{tableSourceAttribute.TableName} (");

        StringBuilder args = new(" values (");
        PropertyInfo[] propertyInfos = generatedType.GetProperties(BindingFlags.Instance | BindingFlags.GetField |
                                                                   BindingFlags.SetField | BindingFlags.Public);
        int argNumber = 0;

        if (Connection.State != ConnectionState.Open)
        {
            Connection.Open();
        }

        using NpgsqlCommand command = (CreateCommand() as NpgsqlCommand)!;

        {
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetCustomAttributes(typeof(FieldSourceAttribute)).FirstOrDefault() is not
                    FieldSourceAttribute fieldSource)
                    continue;
                if (propertyInfo.GetCustomAttributes(typeof(ComputedColumnAttribute)).Any())
                    continue;
                string argName = $"@{propertyInfo.Name}";
                string spacer = argNumber % 10 == 0 ? "\n" : " ";
                object? value = propertyInfo.GetValue(mappedClass);
                builder.Append($"{spacer}{fieldSource.Name} /* {value ?? DBNull.Value} {argName} */,");

                args.Append($"{spacer}{argName},");
                NpgsqlDbType? dbType = GetDbType(propertyInfo);
                object sqlValue = value ?? DBNull.Value;
                NpgsqlParameter parameter;
                if (dbType.HasValue)
                {
                    parameter = new NpgsqlParameter(argName, dbType.Value)
                    {
                        Value = sqlValue
                    };
                }
                else
                    parameter = new NpgsqlParameter(argName, sqlValue);

                command.Parameters.Add(parameter);

                argNumber++;
            }

            string commandText = $"{builder.ToString().TrimEnd(',')} )\n {args.ToString().TrimEnd(',')} )";
            command.CommandText = commandText;

            command.ExecuteNonQuery();
        }

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
-- Create the temp table
select '=' AS __IndicatorColumn__ , {columnList} into TEMPORARY {ResultsTableName}
    from {ExistingTableName}  where 1 = 0;

-- drop collapsed rows if they exist (shouldn't this is a temp table)
drop table if exists CollapsedRows;
-- Match rows as we can
SELECT
		SUM( _ExpectedSource_ ) AS  _ExpectedSource_ , 
		SUM( _ActualSource_ ) AS  _ActualSource_ , 
		{columnList}
	into TEMPORARY CollapsedRows
    FROM (
		SELECT 
				1 AS  _ExpectedSource_ , 
				0 _ActualSource_ , 
				{columnList}
			FROM {ExpectedTableName} 
        UNION ALL 
        SELECT 
				0 _ExpectedSource_ , 
				1 AS  _ActualSource_ ,
				{columnList}
			FROM {ExistingTableName}
      ) AS X 
	GROUP BY {columnList};
	
	DO $$
		DECLARE  
			maxExpected integer;
			maxActual integer;
		BEGIN
			-- Get the matching rows
			select count(1) 
				into maxExpected
				from CollapsedRows
				where _ExpectedSource_>0 and _ActualSource_> 0 ;
	  
			-- return all matching
			while maxExpected> 0 LOOP
				INSERT INTO {ResultsTableName}
					SELECT '=', {columnList} 
						FROM CollapsedRows
						where _ExpectedSource_>0 and _ActualSource_ > 0;
				UPDATE CollapsedRows
					SET _ExpectedSource_ = _ExpectedSource_ - 1 , 
						_ActualSource_ = _ActualSource_ - 1
					where _ExpectedSource_ >0 
						and _ActualSource_ > 0;
				select count(1) into maxExpected
					from CollapsedRows
					where _ExpectedSource_> 0 and _ActualSource_> 0 ;
	  
	
			END LOOP;
			select count(1) 
				into maxExpected
				from CollapsedRows
				where _ExpectedSource_>0;
		
			while maxExpected> 0 LOOP
				INSERT INTO {ResultsTableName}
					SELECT 'M', {columnList} 
						FROM CollapsedRows
						where _ExpectedSource_>0;
				UPDATE CollapsedRows
					SET _ExpectedSource_ = _ExpectedSource_ - 1
					where _ExpectedSource_>0;
				select count(1) 
					into maxExpected
					from CollapsedRows
					where _ExpectedSource_>0;
			  END LOOP;
	  
			select count(1) 
				into maxExpected
				from CollapsedRows
				where _ActualSource_>0;
			
			while maxExpected> 0 LOOP
				INSERT INTO {ResultsTableName}
					SELECT 'X', {columnList} 
						FROM CollapsedRows
						where _ActualSource_>0;
				UPDATE CollapsedRows
					SET _ActualSource_ = _ActualSource_ - 1
					where _ActualSource_>0;
				select count(1) 
					into maxExpected
					from CollapsedRows
					where _ActualSource_>0;
			END LOOP;
	  
	  END $$
	  ;
--insert into {ResultsTableName}
--select     cast(
--sum(case when  __IndicatorColumn__  = '=' then 1 else 0 end) AS varchar
--) ||'|'||cast(
--sum(case when  __IndicatorColumn__  = 'M' then 1 else 0 end) AS varchar
--) ||'|'||cast(
--    sum(case when  __IndicatorColumn__  = 'X' then 1 else 0 end) AS varchar
--)	  
--FROM  {ResultsTableName};

SELECT 
* FROM  {ResultsTableName}

;

";

        using IDbCommand command = CreateCommand();
        command.CommandText = matchQuery;
        using IDataReader reader = command.ExecuteReader();
        DataTable results = new();
        results.Load(reader);
        
        int missing = 0;//reader.GetInt32(1);
        int extra = 0;//reader.GetInt32(2);
        foreach (DataRow dataRow in results.Rows)
        {
            string v = dataRow[0]!.ToString()!;
            switch (v)
            {
                case "M":
                    missing++;
                    break;
                case "X":
                    extra++; 
                    break;

            }
        }
        if (allowUnexpectedRows && missing == 0)
            return ITestDbConnection.Success;
        if (missing <= 0 && extra <= 0) return ITestDbConnection.Success;
        string printableResults = GeneratePrintableResults(results);
        AddReceivedMessage("Result Table:");
        AddReceivedMessage(printableResults);

        return new DataComparisonResults(false, missing, extra);


    }

    /// <inheritdoc />
    public override IList<DatabaseMappedClassPropertyDefinition> GetProperties(string schema, string tableName)
    {

        List<DatabaseMappedClassPropertyDefinition> properties = [];
        Connection.Open();
        using IDbCommand command = Connection.CreateCommand();

        command.CommandText = TableDefinitionQuery;
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = "@tableName";
        parameter.Value = tableName.ToLower();
        command.Parameters.Add(parameter);
        parameter = command.CreateParameter();
        parameter.ParameterName = "@tableSchema";
        parameter.Value = schema.ToLower();
        command.Parameters.Add(parameter);
        {
            using IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                DatabaseMappedClassPropertyDefinition definition = new()
                {
                    Name = reader.GetString(0),
                    SourceType = reader.GetString(1),
                    IsNullable = reader.GetInt32(2) == 1,
                    MaxSize = reader.GetInt16(3),
                    Precision = reader.GetByte(4),
                    Scale = reader.GetByte(5),
                    IsComputed = reader.GetInt32(6) > 0
                };
                definition.MappedType = GetPropertyType(definition.SourceType, definition.IsNullable, definition.MaxSize);
                properties.Add(
                    definition
                );
            }
        }



        return properties;

    }

    
    public static object ConverterToType(string typeName, string input)
    {
        return typeName.ToLower() switch
        {
            "int" or "integer" => AutoAssigner.Converters[typeof(int)](input),
            "long" => AutoAssigner.Converters[typeof(long)](input),
            "decimal" => AutoAssigner.Converters[typeof(decimal)](input),
            "double" => AutoAssigner.Converters[typeof(double)](input),
            "float" => AutoAssigner.Converters[typeof(float)](input),
            "byte" => AutoAssigner.Converters[typeof(byte)](input),
            "short" => AutoAssigner.Converters[typeof(short)](input),
            "string" => AutoAssigner.Converters[typeof(string)](input),
            "Guid" => AutoAssigner.Converters[typeof(Guid)](input),
            "DateTime" => AutoAssigner.Converters[typeof(DateTime)](input),
            "bool" => AutoAssigner.Converters[typeof(bool)](input),
            _ =>  input
        };
        
    }
    

    private static NpgsqlDbType? GetDatabasePropertyType(string sourceType)
    {
        return sourceType.ToLower() switch
        {
            "boolean"  => NpgsqlDbType.Boolean,
            "smallint" => NpgsqlDbType.Smallint,
            "integer" => NpgsqlDbType.Integer,
            "bigint" => NpgsqlDbType.Bigint,
            "real" => NpgsqlDbType.Real,
            "double precision" => NpgsqlDbType.Double,
            "numeric" or "money" => NpgsqlDbType.Money,
            "text" or "character varying" or "character" or "citext" or "json" or "jsonb" or "(internal) char"
                or "name" => NpgsqlDbType.Varchar,
            "xml"=>NpgsqlDbType.Xml,
            "uuid" => NpgsqlDbType.Uuid,
            "bytea" => NpgsqlDbType.Bytea,
            "timestamp without time zone" => NpgsqlDbType.Timestamp,
            "timestamp with time zone" => NpgsqlDbType.TimestampTz,
            "date" => NpgsqlDbType.Date,
            "time without time zone" => NpgsqlDbType.Time,
            "time with time zone" => NpgsqlDbType.TimeTz,
            "interval" => NpgsqlDbType.Interval,
            "cidr" => NpgsqlDbType.Cidr,
            "inet" => NpgsqlDbType.Inet,
            "macaddr" => NpgsqlDbType.MacAddr,
            "tsquery" =>NpgsqlDbType.TsQuery,
            "tsvector" => NpgsqlDbType.TsVector,
            "bit" or "bit varying" => NpgsqlDbType.Bit,
            "point" => NpgsqlDbType.Point,
            "lseg" => NpgsqlDbType.LSeg,
            "path" => NpgsqlDbType.Path,
            "polygon" => NpgsqlDbType.Polygon,
            "line" => NpgsqlDbType.Line,
            "circle" => NpgsqlDbType.Circle,
            "box" => NpgsqlDbType.Box,
            "hstore" => NpgsqlDbType.Hstore,
            "oid" => NpgsqlDbType.Oid,
            "cid" => NpgsqlDbType.Cid,
            "oidvector" => NpgsqlDbType.Oidvector,
//            "record" => NpgsqlDbType.Rec
            _ => null
        };
    }
    private static Type? GetPropertyType(string sourceType, bool isNullable, int maxLength)
    {
        return sourceType switch
        {
            "boolean" => isNullable ? typeof(bool?) : typeof(bool),
            "smallint" => isNullable ? typeof(short?) : typeof(short),
            "integer" => isNullable ? typeof(int?) : typeof(int),
            "bigint" => isNullable ? typeof(long?) : typeof(long),
            "real" => isNullable ? typeof(float?) : typeof(float),
            "double precision" => isNullable ? typeof(double?) : typeof(double),
            "numeric" or "money" => isNullable ? typeof(decimal?) : typeof(decimal),
            "text" or "character varying" or "character" or "citext" or "json" or "jsonb" or "xml" or "(internal) char"
                or "name" => typeof(string),
            "uuid" => isNullable ? typeof(Guid?) : typeof(Guid),
            "bytea" => typeof(byte[]),
            "timestamp without time zone" => isNullable ? typeof(DateTime?) : typeof(DateTime),
            "timestamp with time zone" => isNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset),
            "date" => isNullable ? typeof(DateOnly?) : typeof(DateOnly),
            "time without time zone" or "interval"  => isNullable ? typeof(TimeSpan?) : typeof(TimeSpan),
            "time with time zone" => isNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset),
            "cidr" => isNullable ? typeof(NpgsqlCidr?) : typeof(NpgsqlCidr),
            "inet" => typeof(IPAddress),
            "macaddr" => typeof(PhysicalAddress),
            "tsquery" => typeof(NpgsqlTsQuery),
            "tsvector" => typeof(NpgsqlTsVector),
            "bit"=> maxLength== 1?( isNullable ? typeof(bool?) : typeof(bool)) :   isNullable ? typeof(bool?) : typeof(bool),
            "bit varying" => typeof(BitArray),
            "point" => isNullable ? typeof(NpgsqlPoint?) : typeof(NpgsqlPoint),
            "lseg" => isNullable ? typeof(NpgsqlLSeg?) : typeof(NpgsqlLSeg),
            "path" => isNullable ? typeof(NpgsqlPath?) : typeof(NpgsqlPath),
            "polygon" => isNullable ? typeof(NpgsqlPolygon?) : typeof(NpgsqlPolygon),
            "line" => isNullable ? typeof(NpgsqlLine?) : typeof(NpgsqlLine),
            "circle" => isNullable ? typeof(NpgsqlCircle?) : typeof(NpgsqlCircle),
            "box" => isNullable ? typeof(NpgsqlBox?) : typeof(NpgsqlBox),
            "hstore" => typeof(Dictionary<string, string>),
            "oid" => isNullable ? typeof(uint?) : typeof(uint),
            "cid" => isNullable ? typeof(uint?) : typeof(uint),
            "oidvector" => typeof(uint[]),
            "record" => typeof(object[]),
            _ => null
        };
    }

}
