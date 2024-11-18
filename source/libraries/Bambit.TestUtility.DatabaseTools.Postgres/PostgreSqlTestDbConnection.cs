using System.Collections;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
    private static readonly Dictionary<string, NpgsqlDbType> DatabasePropertyTypeMap =
            new(StringComparer.CurrentCultureIgnoreCase)
            {
                { "boolean", NpgsqlDbType.Boolean },
                { "bool", NpgsqlDbType.Boolean },
                { "smallint", NpgsqlDbType.Smallint },
                { "integer", NpgsqlDbType.Integer },
                { "int", NpgsqlDbType.Integer },
                { "bigint", NpgsqlDbType.Bigint },
                { "real", NpgsqlDbType.Real },
                { "double precision", NpgsqlDbType.Double },
                { "numeric", NpgsqlDbType.Money },
                { "money", NpgsqlDbType.Money },
                { "text", NpgsqlDbType.Varchar },
                { "character varying", NpgsqlDbType.Varchar },
                { "character", NpgsqlDbType.Varchar },
                { "citext", NpgsqlDbType.Varchar },
                { "json", NpgsqlDbType.Varchar },
                { "jsonb", NpgsqlDbType.Varchar },
                { "(internal) char", NpgsqlDbType.Varchar },
                { "name", NpgsqlDbType.Varchar },
                { "xml", NpgsqlDbType.Xml },
                { "uuid", NpgsqlDbType.Uuid },
                { "bytea", NpgsqlDbType.Bytea },
                { "timestamp without time zone", NpgsqlDbType.Timestamp },
                { "timestamp with time zone", NpgsqlDbType.TimestampTz },
                { "date", NpgsqlDbType.Date },
                { "time without time zone", NpgsqlDbType.Time },
                { "time with time zone", NpgsqlDbType.TimeTz },
                { "interval", NpgsqlDbType.Interval },
                { "cidr", NpgsqlDbType.Cidr },
                { "inet", NpgsqlDbType.Inet },
                { "macaddr", NpgsqlDbType.MacAddr },
                { "tsquery", NpgsqlDbType.TsQuery },
                { "tsvector", NpgsqlDbType.TsVector },
                { "bit", NpgsqlDbType.Bit },
                { "bit varying", NpgsqlDbType.Bit },
                { "point", NpgsqlDbType.Point },
                { "lseg", NpgsqlDbType.LSeg },
                { "path", NpgsqlDbType.Path },
                { "polygon", NpgsqlDbType.Polygon },
                { "line", NpgsqlDbType.Line },
                { "circle", NpgsqlDbType.Circle },
                { "box", NpgsqlDbType.Box },
                { "hstore", NpgsqlDbType.Hstore },
                { "oid", NpgsqlDbType.Oid },
                { "cid", NpgsqlDbType.Cid },
                { "oidvector", NpgsqlDbType.Oidvector },
            };
    private static readonly 
        Dictionary<string,Type> PropertyTypes =new(StringComparer.CurrentCultureIgnoreCase)
        {
            {"byte",  typeof(byte)},
            {"boolean" ,   typeof(bool)},
            {"bool" ,   typeof(bool)},
            {"smallint" ,   typeof(short)},
            {"short" ,   typeof(short)},
            {"integer" ,   typeof(int)},
            {"int" ,   typeof(int)},
            {"bigint" ,   typeof(long)},
            {"long" ,   typeof(long)},
            {"real" ,   typeof(float)},
            {"float" ,   typeof(float)},
            {"double precision" ,   typeof(double)},
            {"double" ,   typeof(double)},
            {"numeric" ,   typeof(decimal)},
            {"money" ,   typeof(decimal)},
            {"decimal" ,   typeof(decimal)},
            {"text" , typeof(string)},
            {"character varying" , typeof(string)},
            {"character" , typeof(string)},
            {"citext" , typeof(string)},
            {"json", typeof(string)},
            {"jsonb", typeof(string)},
            {"xml", typeof(string)},
            {"(internal) char", typeof(string)},
            {"name", typeof(string)},
            {"string" , typeof(string)},
            {"uuid"  ,   typeof(Guid)},
            {"guid" ,   typeof(Guid)},
            {"bytea" , typeof(byte[])},
            {"timestamp without time zone" ,   typeof(DateTime)},
                {"datetime" ,   typeof(DateTime)},
            {"timestamp with time zone" ,   typeof(DateTimeOffset)},
            {"date" ,   typeof(DateOnly)},
            {"time without time zone",   typeof(TimeSpan)},
                {"interval"  ,   typeof(TimeSpan)},
            {"time with time zone" ,   typeof(DateTimeOffset)},
            {"cidr" ,   typeof(NpgsqlCidr)},
            {"inet" , typeof(IPAddress)},
            {"macaddr" , typeof(PhysicalAddress)},
            {"bit",    typeof(bool)},
            {"bit(1)",   typeof(bool)},
            {"bit varying" , typeof(BitArray)},
            {"point" ,   typeof(NpgsqlPoint)},
            {"path" ,   typeof(NpgsqlPath)},
            {"polygon" ,   typeof(NpgsqlPolygon)},
            {"line" ,   typeof(NpgsqlLine)},
            {"circle" ,   typeof(NpgsqlCircle)},
            {"box" ,   typeof(NpgsqlBox)},
            {"hstore" , typeof(Dictionary<string, string>)},
            {"oid" ,   typeof(uint)},
            {"cid" ,   typeof(uint)},
            {"oidvector" , typeof(uint[])},
            {"record" , typeof(object[])}
        };
    private static readonly Dictionary<string, Type> NullablePropertyTypes =
            new(StringComparer.CurrentCultureIgnoreCase)
            {
                { "byte", typeof(byte?) },
                { "boolean", typeof(bool?) },
                { "bool", typeof(bool?) },
                { "smallint", typeof(short?) },
                { "short", typeof(short?) },
                { "integer", typeof(int?) },
                { "int", typeof(int?) },
                { "bigint", typeof(long?) },
                { "long", typeof(long?) },
                { "real", typeof(float?) },
                { "float", typeof(float?) },
                { "double precision", typeof(double?) },
                { "double", typeof(double?) },
                { "numeric", typeof(decimal?) },
                { "money", typeof(decimal?) },
                { "decimal", typeof(decimal?) },
                { "text", typeof(string) },
                { "character varying", typeof(string) },
                { "character", typeof(string) },
                { "citext", typeof(string) },
                { "json", typeof(string) },
                { "jsonb", typeof(string) },
                { "xml", typeof(string) },
                { "(internal?) char", typeof(string) },
                { "name", typeof(string) },
                { "string", typeof(string) },
                { "uuid", typeof(Guid?) },
                { "guid", typeof(Guid?) },
                { "bytea", typeof(byte[]) },
                { "timestamp without time zone", typeof(DateTime?) },
                { "datetime", typeof(DateTime?) },
                { "timestamp with time zone", typeof(DateTimeOffset?) },
                { "date", typeof(DateOnly?) },
                { "time without time zone", typeof(TimeSpan?) },
                { "interval", typeof(TimeSpan?) },
                { "time with time zone", typeof(DateTimeOffset?) },
                { "cidr", typeof(NpgsqlCidr?) },
                { "inet", typeof(IPAddress) },
                { "macaddr", typeof(PhysicalAddress) },
                { "bit", typeof(bool?) },
                { "bit(1?)", typeof(bool?) },
                { "bit varying", typeof(BitArray) },
                { "point", typeof(NpgsqlPoint?) },
                { "path", typeof(NpgsqlPath?) },
                { "polygon", typeof(NpgsqlPolygon?) },
                { "line", typeof(NpgsqlLine?) },
                { "circle", typeof(NpgsqlCircle?) },
                { "box", typeof(NpgsqlBox?) },
                { "hstore", typeof(Dictionary<string, string>) },
                { "oid", typeof(uint?) },
                { "cid", typeof(uint?) },
                { "oidvector", typeof(uint[]) },
                { "record", typeof(object[]) }
            };

    private const string PostgreSqlTableDefinitionQuery = """
                                                          select
                                                                    column_name,
                                                                    data_type,
                                                                    case
                                                                        when is_nullable = 'YES' -- and data_type in ('bigint', 'bit', 'date', 'datetime', 'datetime2', 'datetimeoffset', 'decimal', 'float', 'int', 'money', 'numeric', 'real', 'smalldatetime', 'smallint', 'smallmoney', 'time', 'tinyint', 'uniqueidentifier')
                                                                        then 1
                                                                        else 0
                                                                    end NullableSign,
                                                                    coalesce(character_maximum_length, 0) max_length,
                                                                     coalesce(numeric_precision, 0) numeric_scale,
                                                                    coalesce(numeric_scale, 0) numeric_scale,
                                                                    case
                                                                       when IS_GENERATED = 'ALWAYS' then 1
                                                               --        when is_computed =1 then 1
                                                                       --when enerated_always_type <> 0 then 1
                                                                        when is_identity = 'YES' then 1
                                                          			 else 0 end Is_Computed
                                                                from
                                                                   information_schema.columns
                                                                    
                                                          WHERE table_name= @tableName and table_schema = @tableSchema
                                                          """;

    
    
    private static readonly Dictionary<Type, Func<string, object>> LocalConverters =
        new()
        {
            {typeof(NpgsqlCidr), input =>
                {
                    string[] strings = input.Split("/");
                    IPAddress address = IPAddress.Parse(strings[0]);
                    byte mask = byte.Parse(strings[1]);
                    return new NpgsqlCidr(address, mask);
                }
            },
            {typeof(NpgsqlPoint), i=>ParsePoint(i) },
            {typeof(NpgsqlPath), i=>ParsePath(i) },
            {typeof(NpgsqlLine), i=>ParseLine(i)},
            {typeof(NpgsqlCircle), i=>ParseCircle(i)},
            {typeof(NpgsqlBox), i=>ParseBox(i)}
        };

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
                string sqlType = columnType??"varchar";
                switch (columnType)
                {
                    case "date": sqlType = "timestamp";
                        break;
                    case "boolean":
                    case "bit": sqlType = "boolean";
                        break;
                    case "void":     sqlType = "varchar";
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
select {columnList} 
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

    private string[] GetDatabaseFieldTypes(string tableName, string[] columns)
    {
        IList<DatabaseMappedClassPropertyDefinition> properties = GetProperties("public", tableName);
        return columns.Select(c => properties.First(p =>
            string.Compare(p.Name, c, StringComparison.CurrentCultureIgnoreCase) == 0
        ).SourceType).ToArray();
    }

    private void InsertRecords(string tableName, string[] columns, IEnumerable<object?[]> rows)
    {
        string columnList = string.Join(",", columns);
        List<string> cleanedColumnList = [];
        for (int x = 0; x < columns.Length; x++)
            cleanedColumnList.Add($"p{x}");
        string variableList = $"@{string.Join(",@", cleanedColumnList)}";
        string[] expectedColumnTypes =GetDatabaseFieldTypes(tableName, columns);
        

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
                    dbType  =GetDatabasePropertyType(expectedColumnTypes[x]);
                
                if(dbType.HasValue)
                {
                    if (value is string s)
                        value = ConverterToType(expectedColumnTypes![x], s);
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
            if (columnName == "__IndicatorColumn__")
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
drop table if exists  {ResultsTableName};	
-- Create the temp table
select '=' AS __IndicatorColumn__ , {columnList} into   {ResultsTableName}
    from {ExistingTableName}  where 1 = 0;

-- drop collapsed rows if they exist (shouldn't this is a temp table)
drop table if exists CollapsedRows;
-- Match rows as we can
SELECT
		SUM( _ExpectedSource_ ) AS  _ExpectedSource_ , 
		SUM( _ActualSource_ ) AS  _ActualSource_ , 
		{columnList}
	into   CollapsedRows
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
            string v = dataRow[0].ToString()!;
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

    /// <summary>
    /// Parse a string and returns a point
    /// </summary>
    /// <param name="input">    The string to parse. </param>
    /// <returns>
    /// A NpgsqlPoint.
    /// </returns>
    /// <remarks>The point is expected to be in the format of (x,y). </remarks>
    public static NpgsqlPoint ParsePoint(string input)
    {
        
            string[] split = input.Trim(['(', ')']).Split(",");
            double x = double.Parse(split[0]);
            double y = double.Parse(split[1]);
            return new NpgsqlPoint(x, y);
        
    }

    /// <summary>
    /// Parse a string and returns an NpgsqlPath object
    /// </summary>
    /// <param name="input">    The string to parse. </param>
    /// <returns>
    /// A NpgsqlPath.
    /// </returns>
    /// <remarks>
    /// The string is expected to be in the format of [(x1,y1), (x2, y2)...(xn,yn)]
    /// </remarks>
    public static NpgsqlPath ParsePath(string input)
    {
        bool open = input[0] == '[';
        string trim = input.Trim(['[', ']']);
        Regex matcher = new(@"\((.*?)\)");
        MatchCollection matchCollection = matcher.Matches(trim);
        IEnumerable<NpgsqlPoint> npgsqlPoints = matchCollection.Select(m => ParsePoint(m.Groups[0].Value));
        return new NpgsqlPath(npgsqlPoints, open);
    }

    /// <summary>
    /// Parse a string and returns an NpgsqlBox object
    /// </summary>
    /// <param name="input">    The string to parse. </param>
    /// <returns>
    /// A NpgsqlBox.
    /// </returns>
    /// <remarks>The string is expected to be in the format of [(T,R), (B,L)] where (T,R) is the top right coordinate and (B,L) is the bottom left.</remarks>
    public static NpgsqlBox ParseBox(string input)
    {
        Regex matcher = new(@"\((.*?)\)");
        MatchCollection matchCollection = matcher.Matches(input);
        NpgsqlPoint[] npgsqlPoints = matchCollection.Select(m => ParsePoint(m.Groups[0].Value)).ToArray();
        return new NpgsqlBox(npgsqlPoints[0], npgsqlPoints[1]);
    }

    /// <summary>
    /// Parse a string and retunrs an NpgsqlLine
    /// </summary>
    /// <param name="input">    The string to parse. </param>
    /// <returns>
    /// An NpgsqlLine.
    /// </returns>
    /// <remarks>The string is expected to be in the format of {A,B,C} where A, B and C are represented  in the formula Ax + By = C = 0 </remarks>
    public static NpgsqlLine ParseLine(string input)
    {
        string[] split = input.Trim(['{', '}']).Split(",");
        double a = double.Parse(split[0]);
        double b = double.Parse(split[1]);
        double c = double.Parse(split[2]);
        return new NpgsqlLine(a, b, c);
    }

    /// <summary>
    /// Parse a string to return an NpgsqlCircle
    /// </summary>
    /// <param name="input">    The string to parse. </param>
    /// <returns>
    /// An NpgsqlCircle.
    /// </returns>
    /// <remarks>The string is expected to be in the format of &lt;(Cx,Cy),R&gt; where (Cx,Cy) is the center of the circle and R is the radius.</remarks>
    public static NpgsqlCircle ParseCircle(string input)
    {
        string[] split = input.Replace("(","")
            .Replace(")","")
            .Replace("<","")
            .Replace(">","")
            .Split(",");
        double a = double.Parse(split[0]);
        double b = double.Parse(split[1]);
        double c = double.Parse(split[2]);
        return new NpgsqlCircle(a, b, c);
    }
    /// <inheritdoc />
    public override IList<DatabaseMappedClassPropertyDefinition> GetProperties(string schema, string tableName)
    {

        List<DatabaseMappedClassPropertyDefinition> properties = [];
        if(Connection.State!= ConnectionState.Open)
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
                definition.MappedType = GetPropertyType(definition.SourceType, definition.IsNullable/*, definition.MaxSize*/);
                properties.Add(
                    definition
                );
            }
        }



        return properties;

    }

    /// <inheritdoc/>
    protected override object ConvertForDatabaseValue(string value, string targetType)
    {
        return ConverterToType(targetType, value);
    }

    /// <summary>
    /// Converter to specified type
    /// </summary>
    /// <param name="typeName"> Name of the type. </param>
    /// <param name="input">    The string to parse/convert. </param>
    /// <returns>
    /// An object of the specified type, if a converter is defined; otherwise the input
    /// </returns>
    public static object ConverterToType(string typeName, string input)
    {
        Type? converterType = GetPropertyType(typeName, false/*, -1*/);
        if (converterType != null)
        {
            if (LocalConverters.TryGetValue(converterType, out var localConverter))
                return localConverter(input);
            if (AutoAssigner.Converters.TryGetValue(converterType, out var converter))
                return converter(input);
        }

        return input;
    }
    

    private static NpgsqlDbType? GetDatabasePropertyType(string sourceType)
    {
        
        if (DatabasePropertyTypeMap.TryGetValue(sourceType, out NpgsqlDbType dbType))
            return dbType;
        return null;
    }
    private static Type? GetPropertyType(string sourceType, bool isNullable/*, int maxLength*/)
    {
        if(isNullable)
        {if (NullablePropertyTypes.TryGetValue(sourceType, out Type? pt))
                return pt;

        }
        else {if (PropertyTypes.TryGetValue(sourceType, out Type? pt))
            return pt;
        }

        return null;
    }

}
