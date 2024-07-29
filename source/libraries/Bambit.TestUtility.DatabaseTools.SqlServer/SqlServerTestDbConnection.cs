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
        List<string> cleanedColumnList = new List<string>();
        for (int x = 0; x < columns.Length; x++)
            cleanedColumnList.Add($"p{x}");
        string variableList = $"@{string.Join(",@", cleanedColumnList)}";

        
        string query = $"insert into {tableName} ({columnList}) values ({variableList})";
        foreach (object?[] row in rows)
        {
            if (row.Length < columns.Length)
                throw new ArgumentOutOfRangeException(
                    $"Row is missing columns (Expected {columns.Length}, actual {row.Length} ", nameof(rows));
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
        List<string> formats = new List<string>();
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
        FieldSourceAttribute? attribute =
            propertyInfo.GetCustomAttributes(typeof(FieldSourceAttribute)).FirstOrDefault() as FieldSourceAttribute;
        if (attribute == null)
        {
            return null;
        }

        switch (attribute.SourceType.ToLower())
        {
            case "image":
                return  SqlDbType.Image;
            case "binary":
                return  SqlDbType.Binary;
            case "varbinary":
                return  SqlDbType.VarBinary;
            default:
                return null;
        }
    }
    /// <inheritdoc />
    public override void Persist(DatabaseMappedClass mappedClass)
    {
        Type generatedType = mappedClass.GetType();
        TableSourceAttribute? tableSourceAttribute =
            Attribute.GetCustomAttribute(generatedType, typeof(TableSourceAttribute)) as
                TableSourceAttribute;
        if (tableSourceAttribute == null)
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
                FieldSourceAttribute? fieldSource =
                    propertyInfo.GetCustomAttributes(typeof(FieldSourceAttribute)).FirstOrDefault() as
                        FieldSourceAttribute;
                if (fieldSource == null)
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
}
