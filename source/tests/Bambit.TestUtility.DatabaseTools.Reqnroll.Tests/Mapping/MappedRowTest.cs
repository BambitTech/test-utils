using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Bambit.TestUtility.DatabaseTools.Reqnroll.Mapping;
using Bambit.TestUtility.DatabaseTools.SqlServer;
using Bambit.TestUtility.DataGeneration;
using Bambit.TestUtility.TestHelper;
using FluentAssertions;
using NSubstitute;
using Reqnroll;

namespace Bambit.TestUtility.DatabaseTools.Reqnroll.Tests.Mapping;

[TestClass]
[TestCategory("Unit")]
public class MappedRowTest
{
  
    #region Ctors


    [TestMethod]
    public void Ctor_TableRow_SetsHeadersAsExpected()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new(mappedTable, row);
        for (int x = 0; x < MappedTableTest.StandardColumns.Length; x++)
        {
            mappedRow.HasColumn(MappedTableTest.StandardColumns[x]).Should().BeTrue();
            mappedRow.ColumnIndex(MappedTableTest.StandardColumns[x]).Should().Be(x);
        }
    }


    [TestMethod]
    public void Ctor_TableRow_SetsValuesAsExpected()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new(mappedTable, row);
        for (int x = 0; x < MappedTableTest.FirstRow.Length; x++)
        {
            mappedRow.GetString(MappedTableTest.StandardColumns[x],"null").Should().Be(MappedTableTest.FirstRow[x]);
        }
    }


    [TestMethod]
    public void Ctor_IDataReader_SetsHeaderAsExpected()
    {


        IDataReader mockReader = Substitute.For<IDataReader>();
        MappedTableTest.InitializeStandardTableHeader(mockReader);
        MappedTableTest.InitializeStandardRows(mockReader);
        MappedTable mappedTable = new(mockReader);
        MappedRow mappedRow = new(mappedTable, mockReader);

        for (int x = 0; x < MappedTableTest.StandardColumns.Length; x++)
        {
            mappedRow.HasColumn(MappedTableTest.StandardColumns[x]).Should().BeTrue();
            mappedRow.ColumnIndex(MappedTableTest.StandardColumns[x]).Should().Be(x);
        }
    }


    [TestMethod]
    public void Ctor_IDataReader_SetsValuesAsExpected()
    {


        Table table = new(MappedTableTest.StandardColumns);
        IDataReader mockReader = Substitute.For<IDataReader>();
        MappedTableTest.InitializeStandardTableHeader(mockReader);
        MappedTableTest.InitializeStandardRows(mockReader);
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new(mappedTable, mockReader);

        for (int x = 0; x < MappedTableTest.FirstRow.Length; x++)
        {
            mappedRow.GetString(MappedTableTest.StandardColumns[x],"null").Should().Be(MappedTableTest.FirstRow[x]);
        }
    }
    #endregion Ctors

    #region IDictionary that are verboten

    [TestMethod]
    public void Add_KeyAndValue_ThrowsNotImplemented()
    {
        
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        mappedRow.Invoking(m => m.Add("a", "b"))
            .Should()
            .Throw<NotImplementedException>();
    }
    
    [TestMethod]
    public void Add_KeyValuePair_ThrowsNotImplemented()
    {
        
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        mappedRow.Invoking(m => m.Add(new( "a", "b")))
            .Should()
            .Throw<NotImplementedException>();
    }
    
    [TestMethod]
    public void Clear_ThrowsNotImplemented()
    {
        
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        mappedRow.Invoking(m => m.Clear())
            .Should()
            .Throw<NotImplementedException>();
    }
     
    [TestMethod]
    public void Remove_KeyValuePair_ThrowsNotImplemented()
    {
        
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        mappedRow.Invoking(m => m.Remove(new KeyValuePair<string, string?>( "a", "b")))
            .Should()
            .Throw<NotImplementedException>();
    }
    
    [TestMethod]
    public void Remove_ThrowsNotImplemented()
    {
        
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        mappedRow.Invoking(m => m.Remove("a"))
            .Should()
            .Throw<NotImplementedException>();
    }
    
    [TestMethod]
    public void CopyTo_ThrowsNotImplemented()
    {
        
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        KeyValuePair<string, string?>[] array = new KeyValuePair<string, string?>[1];
        mappedRow.Invoking(m => m.CopyTo(array,0))
            .Should()
            .Throw<NotImplementedException>();
    }
    

    #endregion IDictionary that are verboten


    #region Contains
    
    [TestMethod]
    public void Contains_KeyValueExists_ReturnsFalse()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        KeyValuePair<string, string?> searchKeyValuePair = new(MappedTableTest.StandardColumns[0], table.Rows[0][0]);
        mappedRow.Contains(searchKeyValuePair).Should().BeTrue();
    }

    [TestMethod]
    public void Contains_KeyDoesNotExists_ReturnsFalse()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        KeyValuePair<string, string?> searchKeyValuePair = new("Moo", table.Rows[0][0]);
        mappedRow.Contains(searchKeyValuePair).Should().BeFalse();
    }
    [TestMethod]
    public void Contains_KeyExists_ValueDoesNotMatch_ReturnsFalse()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        KeyValuePair<string, string?> searchKeyValuePair = new(MappedTableTest.StandardColumns[0], "Moo");
        mappedRow.Contains(searchKeyValuePair).Should().BeFalse();
    }

    #endregion Contains

    #region Contains Key
    
    [TestMethod]
    public void ContainsKey_KeyValueExists_ReturnsTrue()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        
        mappedRow.ContainsKey(MappedTableTest.StandardColumns[0]).Should().BeTrue();
    }

    [TestMethod]
    public void ContainsKey_KeyDoesNotExists_ReturnsFalse()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        mappedRow.ContainsKey("moo").Should().BeFalse();
    }
    #endregion Contains

    #region GetEnumerator
    
    [TestMethod]
    public void GetEnumerator_AbleToIterateThroughData()
    {
        
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        List<string> matchedColumns = [];
        List<string?> matchedFields= [];

        foreach (KeyValuePair<string, string?> keyValuePair in mappedRow)
        { 
            matchedColumns.Add(keyValuePair.Key);
            matchedFields.Add(keyValuePair.Value);
        }

        matchedFields.Should().BeEquivalentTo(MappedTableTest.FirstRow);
        matchedColumns.Should().BeEquivalentTo(MappedTableTest.StandardColumns.Select(s=>s.ToLower()));
    }
    [TestMethod]
    public void GetEnumerator_FromIEnumerable_AbleToIterateThroughData()
    {
        
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        List<string> matchedColumns = [];
        List<string?> matchedFields= [];

        IEnumerator enumerator = ((IEnumerable)mappedRow).GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, string?> keyValuePair = (KeyValuePair<string, string?>)enumerator.Current ;
            matchedColumns.Add(keyValuePair.Key);
            matchedFields.Add(keyValuePair.Value);
        }
        (enumerator as IDisposable)?.Dispose();
        matchedFields.Should().BeEquivalentTo(MappedTableTest.FirstRow);
        matchedColumns.Should().BeEquivalentTo(MappedTableTest.StandardColumns.Select(s=>s.ToLower()));
    }

    #endregion GetEnumerator
    #region Indexer
    
    [TestMethod]
    public void Index_Integer_GetsExpectedValue()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        for (int x = 0; x < MappedTableTest.FirstRow.Length; x++)
        {
            mappedRow[x].Should().Be(MappedTableTest.FirstRow[x]);
        }
    }

    
    [TestMethod]
    public void Index_String_GetsExpectedValue()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        for (int x = 0; x < MappedTableTest.FirstRow.Length; x++)
        {
            mappedRow[MappedTableTest.StandardColumns[x]].Should().Be(MappedTableTest.FirstRow[x]);
        }
    }
 [TestMethod]
    public void Index_Integer_SetsValue()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        for (int x = 0; x < MappedTableTest.FirstRow.Length; x++)
        {
            string newValue = RandomDataGenerator.Instance.GenerateString(10);
            mappedRow[x] = newValue;
            mappedRow[x].Should().Be(newValue);
        }
    }

    
    [TestMethod]
    public void Index_String_SetsExpectedValue()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        for (int x = 0; x < MappedTableTest.FirstRow.Length; x++)
        {
            string newValue = RandomDataGenerator.Instance.GenerateString(10);
            mappedRow[x] = newValue;
            mappedRow[MappedTableTest.StandardColumns[x]].Should().Be(newValue);
        }
    }
    
    [TestMethod]
    public void Index_String_Get_KeyNotFound_ThrowsOutOfRange()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        mappedRow.Invoking(m => m[RandomDataGenerator.Instance.GenerateString(10)]).Should()
            .Throw<IndexOutOfRangeException>();
    }

    [TestMethod]
    public void Index_String_Get_SetNotFound_ThrowsOutOfRange()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        mappedRow.Invoking(m => m[RandomDataGenerator.Instance.GenerateString(10)]="Q").Should()
            .Throw<IndexOutOfRangeException>();
    }

    #endregion Indexer

    #region  Keys
    
    
    [TestMethod]
    public void Keys_ReturnsColumns()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        mappedRow.Keys.Should().BeEquivalentTo(MappedTableTest.StandardColumns.Select(c=>c.ToLower()));
    }
    

    #endregion Keys
    #region  Values
    
    
    [TestMethod]
    public void Values_ReturnsColumns()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        mappedRow.Values.Should().BeEquivalentTo(MappedTableTest.FirstRow);
    }
    
    [TestMethod]
    public void IDictionary_Values_ReturnsColumns()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new (mappedTable, row);
        ((IDictionary<string, string?>)mappedRow).Values.Should().BeEquivalentTo(MappedTableTest.FirstRow);
    }
    

    #endregion Values
    #region IsReadOnly
    
    [TestMethod]
    public void IsReadOnly_ReturnsFalse()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        new MappedRow(mappedTable, row).IsReadOnly.Should().BeFalse();
    }
    #endregion IsReadOnly
    #region Count
    
    [TestMethod]
    public void Count_ReturnsNumberColumns()
    {
        Table table = new(MappedTableTest.StandardColumns);
        table.AddRow(MappedTableTest.FirstRow);
        DataTableRow row = table.Rows[0];
        MappedTable mappedTable = new(table);
        new MappedRow(mappedTable, row).Count.Should().Be(MappedTableTest.StandardColumns.Length);
    }
    #endregion IsReadOnly

    #region GetString


    [TestMethod]
    public void GetString_RowHasNullValue_ReturnsNull()
    {

        Table table = new(MappedTableTest.StandardColumns);
        IDataReader mockReader = Substitute.For<IDataReader>();
        MappedTableTest.InitializeStandardTableHeader(mockReader);
        MappedTableTest.InitializeSpecialRow(mockReader);
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new(mappedTable, mockReader);

        for (int x = 0; x < MappedTableTest.SpecialRow.Length; x++)
        {
            object value = MappedTableTest.SpecialRow[x];
            if (value == DBNull.Value)
                mappedRow.GetString(MappedTableTest.StandardColumns[x],"null").Should().BeNull();
        }
    }

    #endregion GetString

    #region GetDbValue



    private static List<object[]> GetDbValueData()
    {
        DateTime randomDate = RandomDataGenerator.Instance.GenerateDate();
        int randomInt = RandomDataGenerator.Instance.GenerateInt();
        return
        [
            ["stringField", "Hello", "Hello"],
            ["emptyStringField", "", ""],
            ["quotedSingle @quoted", "' Hello '", " Hello "],
            ["quotedSingle @quoted", " Hello ", " Hello "],
            ["dbNullField", "null", DBNull.Value],
            ["dateField @date", randomDate.ToString("yyyy-MM-dd"), randomDate],
            ["intField", randomInt.ToString(), randomInt.ToString()],
            ["quotedSingle @quoted", "'", "'"]

        ];
    }
    [DataTestMethod]
    [DynamicData(nameof(GetDbValueData), DynamicDataSourceType.Method)]
    public void GetDbValue_ByIndex_ReturnsValue(string header, string cellValue, object expectedValue)
    {
        string[] headers = [header];
        Table table = new(headers);
        table.AddRow([cellValue]);
        MappedTable mappedTable = new(table);

        MappedRow mappedRow = new(mappedTable, table.Rows.First());
        mappedRow.GetDbValue(0,"null",
            new SqlServerTestDbConnection(new SqlConnection())
            ).Should().BeEquivalentTo(expectedValue);
    }


    [DataTestMethod]
    [DynamicData(nameof(GetDbValueData), DynamicDataSourceType.Method)]
    public void GetDbValue_ByName_ReturnsValue(string header, string cellValue, object expectedValue)
    {
        string[] headers = [header];
        Table table = new(headers);
        table.AddRow([cellValue]);
        MappedTable mappedTable = new(table);
        string fieldName = header.Contains('@') ? header[..(header.IndexOf("@", StringComparison.CurrentCultureIgnoreCase) - 1)].Trim() : header;
        MappedRow mappedRow = new(mappedTable, table.Rows.First());
        mappedRow.GetDbValue(fieldName,"null",new SqlServerTestDbConnection(new SqlConnection())).Should().BeEquivalentTo(expectedValue);
    }


    [TestMethod]
    public void GetDbValue_ColumnDoesNotExist_ReturnsDbNull()
    {
        string[] headers = ["ExistingColumn"];
        Table table = new(headers);
        table.AddRow(["TargetValue"]);
        MappedTable mappedTable = new(table);
        string fieldName = "MissingField";
        MappedRow mappedRow = new(mappedTable, table.Rows.First());
        mappedRow.GetDbValue(fieldName,"null",new SqlServerTestDbConnection(new SqlConnection())).Should().Be(DBNull.Value);
    }

    #endregion GetDbValue

    #region  AssignValuesIfDefined


    private static IEnumerable<object[]> GetAssignValues()
    {
        DateTime randomDate =
            DateTime.Parse(RandomDataGenerator.Instance.GenerateDate().ToString(CultureInfo.InvariantCulture));
        TimeOnly randomTime = TimeOnly.Parse(RandomDataGenerator.Instance.GenerateTimeOnly().ToString(CultureInfo.InvariantCulture));
        string firstName = RandomDataGenerator.Instance.GenerateFirstName();
        float randomFloat =
            (float)(RandomDataGenerator.Instance.GenerateDouble(6, 4) * (float.MaxValue - float.MinValue)) +
            float.MinValue;
        return
        [
            ["NullableFirstName", null!, (TestClass e)=>e.NullableFirstName],
            ["FirstName", firstName, (TestClass e)=>e.FirstName],
            ["IntField", RandomDataGenerator.Instance.GenerateInt() , (TestClass e)=>(object)e.IntField],
            ["DoubleField", RandomDataGenerator.Instance.GenerateDouble(4,3) , (TestClass e)=>(object)e.DoubleField],
            ["FloatField", randomFloat , (TestClass e)=>(object)e.FloatField],
            ["ShortField", RandomDataGenerator.Instance.GenerateInt(0,100) , (TestClass e)=>(object)e.ShortField],
            ["DateTimeField", randomDate  , (TestClass e)=>(object)e.DateTimeField],
            ["LongField", RandomDataGenerator.Instance.GenerateInt() , (TestClass e)=>(object)e.LongField],
            ["DecimalField", RandomDataGenerator.Instance.GenerateDecimal() , (TestClass e)=>(object)e.DecimalField],
            ["GuidField", RandomDataGenerator.Instance.GenerateGuid() , (TestClass e)=>(object)e.GuidField],
            ["BoolField", RandomDataGenerator.Instance.GenerateBoolean() , (TestClass e)=>(object)e.BoolField],
            ["ByteField", RandomDataGenerator.Instance.GenerateByte() , (TestClass e)=>(object)e.ByteField],

            ["DateOnlyField", RandomDataGenerator.Instance.GenerateDateOnly() , (TestClass e)=>(object)e.DateOnlyField],
            ["TimeOnlyField", randomTime , (TestClass e)=>(object)e.TimeOnlyField],


            ["NullableFirstName", firstName, (TestClass e)=>e.NullableFirstName],
            ["NullableIntField", RandomDataGenerator.Instance.GenerateInt() , (TestClass e)=>(object?)e.NullableIntField
            ],
            ["NullableDoubleField", RandomDataGenerator.Instance.GenerateDouble(4,3) , (TestClass e)=>(object?)e.NullableDoubleField
            ],
            ["NullableFloatField", randomFloat , (TestClass e)=>(object?)e.NullableFloatField],
            ["NullableShortField", RandomDataGenerator.Instance.GenerateInt(0,100) , (TestClass e)=>(object?)e.NullableShortField
            ],
            ["NullableDateTimeField", randomDate  , (TestClass e)=>(object?)e.NullableDateTimeField],
            ["NullableLongField", RandomDataGenerator.Instance.GenerateInt() , (TestClass e)=>(object?)e.NullableLongField
            ],
            ["NullableDecimalField", RandomDataGenerator.Instance.GenerateDecimal() , (TestClass e)=>(object?)e.NullableDecimalField
            ],
            ["NullableGuidField", RandomDataGenerator.Instance.GenerateGuid() , (TestClass e)=>(object?)e.NullableGuidField
            ],
            ["NullableBoolField", RandomDataGenerator.Instance.GenerateBoolean() , (TestClass e)=>(object?)e.NullableBoolField
            ],
            ["NullableByteField", RandomDataGenerator.Instance.GenerateByte() , (TestClass e)=>(object?)e.NullableByteField
            ],


            ["NullableIntField", null!, (TestClass e)=>(object?)e.NullableIntField],
            ["NullableDoubleField", null! , (TestClass e)=>(object?)e.NullableDoubleField],
            ["NullableShortField", null!, (TestClass e)=>(object?)e.NullableShortField],
            ["NullableFloatField", null!, (TestClass e)=>(object?)e.NullableFloatField],
            ["NullableDateTimeField", null!, (TestClass e)=>(object?)e.NullableDateTimeField],
            ["NullableLongField", null!, (TestClass e)=>(object?)e.NullableLongField],
            ["NullableDecimalField", null!, (TestClass e)=>(object?)e.NullableDecimalField],
            ["NullableGuidField", null!, (TestClass e)=>(object?)e.NullableGuidField],
            ["NullableBoolField", null! , (TestClass e)=>(object?)e.NullableBoolField],
            ["NullableByteField", null! , (TestClass e)=>(object?)e.NullableByteField],
        ];
    }

    [DataTestMethod]
    [DynamicData(nameof(GetAssignValues), DynamicDataSourceType.Method)]
    public void AssignValuesIfDefined_AllValuesSupplied_AssignsAllValues(string header, object? cellValue,
        Func<TestClass, object?> retrieverFunction)
    {
        TestClass testClass = RandomDataGenerator.Instance.InitializeObject<TestClass>();



        string[] headers = [header];
        Table table = new(headers);
        table.AddRow([cellValue?.ToString()]);
        MappedTable mappedTable = new(table);

        MappedRow mappedRow = new(mappedTable, table.Rows.First());
        string[] results = mappedRow.AssignValuesIfDefined(testClass);
        object? result = retrieverFunction(testClass);
        result.Should().BeEquivalentTo(cellValue);
        results.Should().BeEquivalentTo(headers);
    }

    [TestMethod]
    public void AssignValuesIfDefined_FieldHasComputedColumn_IgnoresComputedColumn()
    {
        MappedClassWithComputedColumn testClass = RandomDataGenerator.Instance.InitializeObject<MappedClassWithComputedColumn>();

        string[] headers = ["WeekDay"];
        Table table = new(headers);
        table.AddRow(["Monday"]);
        MappedTable mappedTable = new(table);
        string initialValue = testClass.WeekDay!;
        MappedRow mappedRow = new(mappedTable, table.Rows.First());
        string[] results = mappedRow.AssignValuesIfDefined(testClass);
        string? result = testClass.WeekDay;
        result.Should().BeEquivalentTo(initialValue);
        results.Should().BeEmpty();
    }

    [TestMethod]
    public void AssignValuesIfDefined_TryToAssignNullToNonNullField_ThrowsException()
    {
        TestClass testClass = RandomDataGenerator.Instance.InitializeObject<TestClass>();

        string[] headers = ["FirstName"];
        Table table = new(headers);
        table.AddRow([null]);
        MappedTable mappedTable = new(table);
        MappedRow mappedRow = new(mappedTable, table.Rows.First());
        mappedRow.Invoking(m => m.AssignValuesIfDefined(testClass))
            .Should().Throw<NullReferenceException>();
    }

    #endregion AssignValuesIfDefined
}