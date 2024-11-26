using System.Data;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;
using Bambit.TestUtility.DataGeneration;
using FluentAssertions;
using NSubstitute;
using TechTalk.SpecFlow;


namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Tests.Mapping
{
    [TestClass]
    public class MappedTableTest
    {

        public const string FirstFieldName = "ColumnAlpha";
        public const string SecondFieldName = "ColumnBeta";
        public const string ThirdFieldName = "ColumnGamma";
        public const string FourthFieldName = "ColumnDelta";
        public const string FifthFieldName = "ColumnEpsilon";
        public const string LastFieldName = "ColumnOmega";
        public const string InvalidFieldName = "NotFound";

        public static readonly string[] FirstRow = ["Row1 Alpha", "Row1 Beta", "Row1 Gamma", "Row1 Delta", "Row1 Epsilon", "Row1 Omega"];
        public static readonly string[] SecondRow = ["Row2 Alpha", "Row2 Beta", "Row2 Gamma", "Row2 Delta", "Row1 Epsilon", "Row2 Omega"];
        public static readonly object[] SpecialRow = ["null", DBNull.Value, "Row2 Gamma", DBNull.Value, "Row1 Epsilon", "Row2 Omega"];

        public static readonly string[] StandardColumns = [FirstFieldName, SecondFieldName, ThirdFieldName, FourthFieldName, FifthFieldName, LastFieldName];

        public static void InitializeSpecialRow(IDataReader mockReader)
        {
            mockReader.Read().Returns(true, false);
            mockReader.GetValue(0).Returns(SpecialRow[0]);
            mockReader.GetValue(1).Returns(SpecialRow[1]);
            mockReader.GetValue(2).Returns(SpecialRow[2]);
            mockReader.GetValue(3).Returns(SpecialRow[3]);
            mockReader.GetValue(4).Returns(SpecialRow[4]);
            mockReader.GetValue(5).Returns(SpecialRow[5]);
        }
        public static void InitializeStandardRows(IDataReader mockReader)
        {
            mockReader.Read().Returns(true, true, false);
            mockReader.GetValue(0).Returns(FirstRow[0], SecondRow[0]);
            mockReader.GetValue(1).Returns(FirstRow[1], SecondRow[1]);
            mockReader.GetValue(2).Returns(FirstRow[2], SecondRow[2]);
            mockReader.GetValue(3).Returns(FirstRow[3], SecondRow[3]);
            mockReader.GetValue(4).Returns(FirstRow[4], SecondRow[4]);
            mockReader.GetValue(5).Returns(FirstRow[5], SecondRow[5]);
        }
        public static void InitializeStandardTableHeader(IDataReader mockReader)
        {
            mockReader.FieldCount.Returns(6);
            PrepareMockReaderColumn(mockReader, 0, "date", FirstFieldName);
            PrepareMockReaderColumn(mockReader, 1, "datetime", SecondFieldName);
            PrepareMockReaderColumn(mockReader, 2, "bit", ThirdFieldName);
            PrepareMockReaderColumn(mockReader, 3, "Invalid", FourthFieldName);
            PrepareMockReaderColumn(mockReader, 4, null, FifthFieldName);
            PrepareMockReaderColumn(mockReader, 5, "boolean", LastFieldName);

        }
        public static void PrepareMockReaderColumn(IDataReader mockReader, int index, string? columnType, string rawName)
        {
            mockReader.GetDataTypeName(Arg.Is(index)).Returns(columnType);
            mockReader.GetName(Arg.Is(index)).Returns(rawName);
        }
        #region DataReader Ctor


        [TestMethod]
        public void IndexOfHeader_DataReaderCtor_HasCorrectColumns()
        {

            IDataReader mockReader = Substitute.For<IDataReader>();
            InitializeStandardTableHeader(mockReader);
            mockReader.Read().Returns(false);
            new MappedTable(mockReader).Columns.Should().BeEquivalentTo(StandardColumns.Select(a => a.ToLower()));

        }

        [TestMethod]
        public void Rows_DataReaderCtor_HasCorrectRows()
        {

            IDataReader mockReader = Substitute.For<IDataReader>();
            InitializeStandardTableHeader(mockReader);
            InitializeStandardRows(mockReader);
            MappedTable mappedTable = new(mockReader);
            mappedTable.Rows.Count.Should().Be(2);
            foreach (MappedRow mappedTableRow in mappedTable.Rows)
            {

                for (int x = 0; x < StandardColumns.Length; x++)
                {
                    mappedTableRow.ColumnIndex(StandardColumns[x]).Should().Be(x);
                }
            }

        }

        #endregion DataReader Ctor

        #region Table Ctor


        [TestMethod]
        public void IndexOfHeader_TableCtor_HasCorrectColumns()
        {

            Table table = new(StandardColumns);
            new MappedTable(table).Columns.Should().BeEquivalentTo(StandardColumns.Select(a => a.ToLower()));

        }

        [TestMethod]
        public void Rows_TableCtor_HasCorrectRows()
        {

            Table table = new(StandardColumns);
            table.AddRow(FirstRow);
            table.AddRow(SecondRow);
            MappedTable mappedTable = new(table);

            mappedTable.Rows.Count.Should().Be(2);
            foreach (MappedRow mappedTableRow in mappedTable.Rows)
            {

                for (int x = 0; x < StandardColumns.Length; x++)
                {
                    mappedTableRow.ColumnIndex(StandardColumns[x]).Should().Be(x);
                }
            }

        }
        #endregion DataReader Ctor

        #region CleanRegex

        [DataTestMethod]
        [DataRow("Abcd", "Abcd")]
        [DataRow("Abcd1", "Abcd1")]
        [DataRow("Abcd@", "Abcd@")]
        [DataRow("Abcd#", "Abcd")]
        [DataRow("Abcd#Abcd", "AbcdAbcd")]
        [DataRow("Abcd+Abcd", "AbcdAbcd")]
        [DataRow("Abcd-Abcd", "AbcdAbcd")]
        [DataRow("Abcd&Abcd", "AbcdAbcd")]
        public void CleanRegex_Replace(string input, string expected)
        {
            MappedTable.CleanRegex.Replace(input, "").Should().Be(expected);
        }

        #endregion

        [DataTestMethod]
        [DataRow(FirstFieldName, 0)]
        [DataRow(SecondFieldName, 1)]
        [DataRow(ThirdFieldName, 2)]
        [DataRow(FourthFieldName, 3)]
        [DataRow(FifthFieldName, 4)]
        [DataRow(LastFieldName, 5)]
        [DataRow(InvalidFieldName, -1)]
        public void IndexOfHeader_NameExists_ReturnsProperIndex(string name, int expectedIndex)
        {

            IDataReader mockReader = Substitute.For<IDataReader>();
            InitializeStandardTableHeader(mockReader);
            mockReader.Read().Returns(false);
            MappedTable mappedTable = new(mockReader);
            mappedTable.IndexOfHeader(name).Should().Be(expectedIndex);

        }

        [DataTestMethod]
        [DataRow(FirstFieldName, FirstFieldName, FirstFieldName, "date", 0)]
        [DataRow(SecondFieldName, SecondFieldName, SecondFieldName, "date", 1)]
        [DataRow(ThirdFieldName, ThirdFieldName, ThirdFieldName, "boolean", 2)]
        [DataRow(FourthFieldName, FourthFieldName, FourthFieldName, "Invalid", 3)]
        [DataRow(FifthFieldName, FifthFieldName, FifthFieldName, null, 4)]
        [DataRow(LastFieldName, LastFieldName, LastFieldName, "boolean", 5)]
        public void GetColumnDescription_ReturnsExpected(string name, string columnName, string cleanedName, string columnType, int index)
        {

            IDataReader mockReader = Substitute.For<IDataReader>();
            InitializeStandardTableHeader(mockReader);
            mockReader.Read().Returns(false);
            MappedTable mappedTable = new(mockReader);
            ColumnDescription expected = new(columnName.ToLower(), cleanedName.ToLower(), columnType, index);
            mappedTable.GetColumnDescription(name).Should().BeEquivalentTo(expected);

        }


        [TestMethod]
        public void GetColumnDescription_UnknownColumn_ReturnsNull()
        {

            IDataReader mockReader = Substitute.For<IDataReader>();
            InitializeStandardTableHeader(mockReader);
            mockReader.Read().Returns(false);
            MappedTable mappedTable = new(mockReader);
            mappedTable.GetColumnDescription(RandomDataGenerator.Instance.GenerateString(10)).Should().BeNull();

        }


        [DataTestMethod]
        [DataRow(SecondFieldName, "date", SecondFieldName, SecondFieldName, "date", 1)]
        [DataRow(ThirdFieldName, "boolean", ThirdFieldName, ThirdFieldName, "boolean", 2)]
        public void GetColumnDescription_TableHasTypes_ReturnsExpected(string name, string type, string columnName, string cleanedName, string columnType, int index)
        {


            Table table = new(StandardColumns.Select(a => $"{a} @{type}").ToArray());
            MappedTable mappedTable = new(table);
            ColumnDescription expected = new(columnName.ToLower(), cleanedName.ToLower(), columnType, index);
            mappedTable.GetColumnDescription(name).Should().BeEquivalentTo(expected);

        }

    }
}
