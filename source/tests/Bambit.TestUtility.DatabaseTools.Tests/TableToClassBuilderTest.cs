using System.Data;
using System.Reflection;
using Bambit.TestUtility.DatabaseTools.Attributes;
using Bambit.TestUtility.DataGeneration;
using FluentAssertions;
using NSubstitute;


namespace Bambit.TestUtility.DatabaseTools.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public class TableToClassBuilderTest
    {
        readonly ITestDatabaseFactory MockTestDatabaseFactory = Substitute.For<ITestDatabaseFactory>();
        readonly IDbCommand MockCommand = Substitute.For<IDbCommand>();
        readonly IDataReader MockReader = Substitute.For<IDataReader>();
        readonly IDatabaseCatalogRecord MockCatalogRecord = Substitute.For<IDatabaseCatalogRecord>();

        public void InitializeStandardSetup(string connectionName)
        {
            string connectionString = RandomDataGenerator.Instance.GenerateString(10);
            string tableGenerationString = RandomDataGenerator.Instance.GenerateString(50);

            MockTestDatabaseFactory.GetGenerator(Arg.Any<string>()).Returns(MockCatalogRecord);
            MockTestDatabaseFactory.GetConnectionString(connectionName).Returns(connectionString);
            MockCatalogRecord.GetConnection(connectionString).CreateCommand().Returns(MockCommand);
            MockCatalogRecord.TableDefinitionQuery.Returns(tableGenerationString);
            MockCommand.ExecuteReader().Returns(MockReader);

        }

        [TestMethod]
        public void GenerateClassTypeFromTable_HappyPath_SetsAssemblyNamePrefix()
        {
            ITestDatabaseFactory testDatabaseFactory = Substitute.For<ITestDatabaseFactory>();
            TableToClassBuilder testClass = new(testDatabaseFactory);

            string connectionName=RandomDataGenerator.Instance.GenerateString(10);
            string schemaName=RandomDataGenerator.Instance.GenerateString(10);
            string tableName=RandomDataGenerator.Instance.GenerateString(10);
            Type generateClassType= testClass.GenerateClassTypeFromTable(connectionName, schemaName, tableName);
            generateClassType.Should().NotBeNull();
            generateClassType.Assembly.FullName.Should().StartWith("Bambit.Generated");
        }

        
        [DataTestMethod]
        [DataRow("float", 0, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("real", 0, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("double", 0, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("bigint", 0, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("timestamp", 0, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("long", 0, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("varbinary", 0, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("image", 0, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("binary", 0, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("bytearray", 0, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("boolean", 0, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("bool", 0, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("bit", 0, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("date", 0, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetime", 0, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("smalldatetime", 0, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetime2", 0, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetimeoffset", 0, (short)16,default(byte), default(byte),typeof(DateTimeOffset))]
        [DataRow("money", 0, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("numeric", 0, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("decimal", 0, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("smallmoney", 0, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("int", 0, (short)16,default(byte), default(byte),typeof(int))]
        [DataRow("nchar", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("ntext", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("nvarchar", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("varchar", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("text", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("char", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("xml", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("smallint", 0, (short)16,default(byte), default(byte),typeof(short))]
        [DataRow("short", 0, (short)16,default(byte), default(byte),typeof(short))]
        [DataRow("time", 0, (short)16,default(byte), default(byte),typeof(TimeSpan))]
        [DataRow("timespan", 0, (short)16,default(byte), default(byte),typeof(TimeSpan))]
        [DataRow("tinyint", 0, (short)16,default(byte), default(byte),typeof(byte))]
        [DataRow("byte", 0, (short)16,default(byte), default(byte),typeof(byte))]
        [DataRow("uniqueidentifier", 0, (short)16,default(byte), default(byte),typeof(Guid))]

        
        [DataRow("float", 1, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("real", 1, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("double", 1, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("bigint", 1, (short)16,default(byte), default(byte),typeof(long?))]
        [DataRow("timestamp", 1, (short)16,default(byte), default(byte),typeof(long?))]
        [DataRow("long", 1, (short)16,default(byte), default(byte),typeof(long?))]

        [DataRow("varbinary", 1, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("image", 1, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("binary", 1, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("bytearray", 1, (short)16,default(byte), default(byte),typeof(byte[]))]

        [DataRow("boolean", 1, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("bool", 1, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("bit", 1, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("date", 1, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetime", 1, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("smalldatetime", 1, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetime2", 1, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetimeoffset", 1, (short)16,default(byte), default(byte),typeof(DateTimeOffset?))]
        [DataRow("money", 1, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("numeric", 1, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("decimal", 1, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("smallmoney", 1, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("int", 1, (short)16,default(byte), default(byte),typeof(int?))]

        [DataRow("nchar", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("ntext", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("nvarchar", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("varchar", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("text", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("char", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("xml", 1, (short)16,default(byte), default(byte),typeof(string))]

        [DataRow("smallint", 1, (short)16,default(byte), default(byte),typeof(short?))]
        [DataRow("short", 1, (short)16,default(byte), default(byte),typeof(short?))]
        [DataRow("time", 1, (short)16,default(byte), default(byte),typeof(TimeSpan?))]
        [DataRow("timespan", 1, (short)16,default(byte), default(byte),typeof(TimeSpan?))]
        [DataRow("tinyint", 1, (short)16,default(byte), default(byte),typeof(byte?))]
        [DataRow("byte", 1, (short)16,default(byte), default(byte),typeof(byte?))]
        [DataRow("uniqueidentifier", 1, (short)16,default(byte), default(byte),typeof(Guid?))]
        public void GenerateClassTypeFromTable_NonGeneratedFields_CreatesProperty(string fieldName, int isNullable, short maxSize,
            byte precision,byte scale, Type expectedType)
        {
            string connectionName=RandomDataGenerator.Instance.GenerateString(10);

            string schemaName=RandomDataGenerator.Instance.GenerateString(10);
            string tableName=RandomDataGenerator.Instance.GenerateString(10);
            string stringFieldName = RandomDataGenerator.Instance.GenerateString(5);


            InitializeStandardSetup(connectionName);
            MockReader.Read().Returns(true, false);

            MockReader.GetString(0).Returns(stringFieldName);
            MockReader.GetString(1).Returns(fieldName);
            MockReader.GetInt32(2).Returns(isNullable);
            MockReader.GetInt16(3).Returns(maxSize);
            MockReader.GetByte(4).Returns(precision);
            MockReader.GetByte(5).Returns(scale);

            MockReader.GetBoolean(6).Returns(false);
            MockReader.GetInt32(7).Returns(0);
            MockReader.GetBoolean(88).Returns(false);
            TableToClassBuilder testClass = new(MockTestDatabaseFactory);

            Type generateClassType= testClass.GenerateClassTypeFromTable(connectionName, schemaName, tableName);
            generateClassType.Should().NotBeNull();
            PropertyInfo? info = generateClassType.GetProperties().FirstOrDefault(p=>p.Name==stringFieldName);
            info.Should().NotBeNull();
            info!.PropertyType.Should().Be(expectedType);
        }

        
        [TestMethod]
        public void GenerateClassTypeFromTable_UnknownType_DoesNotCreateProperty()
        {
            string connectionName=RandomDataGenerator.Instance.GenerateString(10);

            string schemaName=RandomDataGenerator.Instance.GenerateString(10);
            string tableName=RandomDataGenerator.Instance.GenerateString(10);
            string fieldName=RandomDataGenerator.Instance.GenerateString(10);
            string stringFieldName = RandomDataGenerator.Instance.GenerateString(5);


            InitializeStandardSetup(connectionName);
            MockReader.Read().Returns(true, false);

            MockReader.GetString(0).Returns(stringFieldName);
            MockReader.GetString(1).Returns(fieldName);
            MockReader.GetInt32(2).Returns(0);
            MockReader.GetInt16(3).Returns((short)16);
            MockReader.GetByte(4).Returns(default(byte));
            MockReader.GetByte(5).Returns(default(byte));

            MockReader.GetBoolean(6).Returns(false);
            MockReader.GetInt32(7).Returns(0);
            MockReader.GetBoolean(8).Returns(false);
            TableToClassBuilder testClass = new(MockTestDatabaseFactory);

            Type generateClassType= testClass.GenerateClassTypeFromTable(connectionName, schemaName, tableName);
            generateClassType.Should().NotBeNull();
            PropertyInfo? info = generateClassType.GetProperties().FirstOrDefault(p=>p.Name==stringFieldName);
            info.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("float", 0, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("real", 0, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("double", 0, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("bigint", 0, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("timestamp", 0, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("long", 0, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("varbinary", 0, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("image", 0, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("binary", 0, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("bytearray", 0, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("boolean", 0, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("bool", 0, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("bit", 0, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("date", 0, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetime", 0, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("smalldatetime", 0, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetime2", 0, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetimeoffset", 0, (short)16,default(byte), default(byte),typeof(DateTimeOffset))]
        [DataRow("money", 0, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("numeric", 0, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("decimal", 0, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("smallmoney", 0, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("int", 0, (short)16,default(byte), default(byte),typeof(int))]
        [DataRow("nchar", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("ntext", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("nvarchar", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("varchar", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("text", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("char", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("xml", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("smallint", 0, (short)16,default(byte), default(byte),typeof(short))]
        [DataRow("short", 0, (short)16,default(byte), default(byte),typeof(short))]
        [DataRow("time", 0, (short)16,default(byte), default(byte),typeof(TimeSpan))]
        [DataRow("timespan", 0, (short)16,default(byte), default(byte),typeof(TimeSpan))]
        [DataRow("tinyint", 0, (short)16,default(byte), default(byte),typeof(byte))]
        [DataRow("byte", 0, (short)16,default(byte), default(byte),typeof(byte))]
        [DataRow("uniqueidentifier", 0, (short)16,default(byte), default(byte),typeof(Guid))]

        
        [DataRow("float", 1, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("real", 1, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("double", 1, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("bigint", 1, (short)16,default(byte), default(byte),typeof(long?))]
        [DataRow("timestamp", 1, (short)16,default(byte), default(byte),typeof(long?))]
        [DataRow("long", 1, (short)16,default(byte), default(byte),typeof(long?))]

        [DataRow("varbinary", 1, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("image", 1, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("binary", 1, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("bytearray", 1, (short)16,default(byte), default(byte),typeof(byte[]))]

        [DataRow("boolean", 1, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("bool", 1, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("bit", 1, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("date", 1, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetime", 1, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("smalldatetime", 1, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetime2", 1, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetimeoffset", 1, (short)16,default(byte), default(byte),typeof(DateTimeOffset?))]
        [DataRow("money", 1, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("numeric", 1, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("decimal", 1, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("smallmoney", 1, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("int", 1, (short)16,default(byte), default(byte),typeof(int?))]

        [DataRow("nchar", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("ntext", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("nvarchar", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("varchar", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("text", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("char", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("xml", 1, (short)16,default(byte), default(byte),typeof(string))]

        [DataRow("smallint", 1, (short)16,default(byte), default(byte),typeof(short?))]
        [DataRow("short", 1, (short)16,default(byte), default(byte),typeof(short?))]
        [DataRow("time", 1, (short)16,default(byte), default(byte),typeof(TimeSpan?))]
        [DataRow("timespan", 1, (short)16,default(byte), default(byte),typeof(TimeSpan?))]
        [DataRow("tinyint", 1, (short)16,default(byte), default(byte),typeof(byte?))]
        [DataRow("byte", 1, (short)16,default(byte), default(byte),typeof(byte?))]
        [DataRow("uniqueidentifier", 1, (short)16,default(byte), default(byte),typeof(Guid?))]
        public void GenerateClassTypeFromTable_FieldIsGenerated_AddsComputedColumnAttribute(string fieldName, int isNullable, short maxSize,
            byte precision,byte scale, Type expectedType)
        {
            string connectionName=RandomDataGenerator.Instance.GenerateString(10);

            string schemaName=RandomDataGenerator.Instance.GenerateString(10);
            string tableName=RandomDataGenerator.Instance.GenerateString(10);
            string stringFieldName = RandomDataGenerator.Instance.GenerateString(5);


            InitializeStandardSetup(connectionName);
            MockReader.Read().Returns(true, false);

            MockReader.GetString(0).Returns(stringFieldName);
            MockReader.GetString(1).Returns(fieldName);
            MockReader.GetInt32(2).Returns(isNullable);
            MockReader.GetInt16(3).Returns(maxSize);
            MockReader.GetByte(4).Returns(precision);
            MockReader.GetByte(5).Returns(scale);

            MockReader.GetInt32(6).Returns(1);
            MockReader.GetInt32(7).Returns(0);
            MockReader.GetBoolean(8).Returns(false);
            TableToClassBuilder testClass = new(MockTestDatabaseFactory);

            Type generateClassType= testClass.GenerateClassTypeFromTable(connectionName, schemaName, tableName);
            generateClassType.Should().NotBeNull();
            PropertyInfo? info = generateClassType.GetProperties().FirstOrDefault(p=>p.Name==stringFieldName);
            info.Should().NotBeNull();
            Attribute? attribute = info!.GetCustomAttributes(typeof(ComputedColumnAttribute)).SingleOrDefault();
            attribute.Should().NotBeNull().And.BeOfType<ComputedColumnAttribute>();
        }


        
        [DataTestMethod]
        [DataRow("float", 0, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("real", 0, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("double", 0, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("bigint", 0, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("timestamp", 0, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("long", 0, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("varbinary", 0, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("image", 0, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("binary", 0, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("bytearray", 0, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("boolean", 0, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("bool", 0, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("bit", 0, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("date", 0, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetime", 0, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("smalldatetime", 0, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetime2", 0, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetimeoffset", 0, (short)16,default(byte), default(byte),typeof(DateTimeOffset))]
        [DataRow("money", 0, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("numeric", 0, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("decimal", 0, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("smallmoney", 0, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("int", 0, (short)16,default(byte), default(byte),typeof(int))]
        [DataRow("nchar", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("ntext", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("nvarchar", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("varchar", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("text", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("char", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("xml", 0, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("smallint", 0, (short)16,default(byte), default(byte),typeof(short))]
        [DataRow("short", 0, (short)16,default(byte), default(byte),typeof(short))]
        [DataRow("time", 0, (short)16,default(byte), default(byte),typeof(TimeSpan))]
        [DataRow("timespan", 0, (short)16,default(byte), default(byte),typeof(TimeSpan))]
        [DataRow("tinyint", 0, (short)16,default(byte), default(byte),typeof(byte))]
        [DataRow("byte", 0, (short)16,default(byte), default(byte),typeof(byte))]
        [DataRow("uniqueidentifier", 0, (short)16,default(byte), default(byte),typeof(Guid))]

        
        [DataRow("float", 1, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("real", 1, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("double", 1, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("bigint", 1, (short)16,default(byte), default(byte),typeof(long?))]
        [DataRow("timestamp", 1, (short)16,default(byte), default(byte),typeof(long?))]
        [DataRow("long", 1, (short)16,default(byte), default(byte),typeof(long?))]

        [DataRow("varbinary", 1, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("image", 1, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("binary", 1, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("bytearray", 1, (short)16,default(byte), default(byte),typeof(byte[]))]

        [DataRow("boolean", 1, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("bool", 1, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("bit", 1, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("date", 1, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetime", 1, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("smalldatetime", 1, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetime2", 1, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetimeoffset", 1, (short)16,default(byte), default(byte),typeof(DateTimeOffset?))]
        [DataRow("money", 1, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("numeric", 1, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("decimal", 1, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("smallmoney", 1, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("int", 1, (short)16,default(byte), default(byte),typeof(int?))]

        [DataRow("nchar", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("ntext", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("nvarchar", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("varchar", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("text", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("char", 1, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("xml", 1, (short)16,default(byte), default(byte),typeof(string))]

        [DataRow("smallint", 1, (short)16,default(byte), default(byte),typeof(short?))]
        [DataRow("short", 1, (short)16,default(byte), default(byte),typeof(short?))]
        [DataRow("time", 1, (short)16,default(byte), default(byte),typeof(TimeSpan?))]
        [DataRow("timespan", 1, (short)16,default(byte), default(byte),typeof(TimeSpan?))]
        [DataRow("tinyint", 1, (short)16,default(byte), default(byte),typeof(byte?))]
        [DataRow("byte", 1, (short)16,default(byte), default(byte),typeof(byte?))]
        [DataRow("uniqueidentifier", 1, (short)16,default(byte), default(byte),typeof(Guid?))]
        public void GenerateObjectFromTable_NonGeneratedFields_CreatesProperty(string fieldName, int isNullable, short maxSize,
            byte precision,byte scale, Type expectedType)
        {
            string connectionName=RandomDataGenerator.Instance.GenerateString(10);

            string schemaName=RandomDataGenerator.Instance.GenerateString(10);
            string tableName=RandomDataGenerator.Instance.GenerateString(10);
            string stringFieldName = RandomDataGenerator.Instance.GenerateString(5);


            InitializeStandardSetup(connectionName);
            MockReader.Read().Returns(true, false);

            MockReader.GetString(0).Returns(stringFieldName);
            MockReader.GetString(1).Returns(fieldName);
            MockReader.GetInt32(2).Returns(isNullable);
            MockReader.GetInt16(3).Returns(maxSize);
            MockReader.GetByte(4).Returns(precision);
            MockReader.GetByte(5).Returns(scale);

            MockReader.GetBoolean(6).Returns(false);
            MockReader.GetInt32(7).Returns(0);
            MockReader.GetBoolean(8).Returns(false);
            TableToClassBuilder testClass = new(MockTestDatabaseFactory);

            dynamic generateClass = testClass.GenerateObjectFromTable(connectionName, schemaName, tableName)!;
            Type generateClassType= generateClass.GetType();
            PropertyInfo? info = generateClassType.GetProperties().FirstOrDefault(p=>p.Name==stringFieldName);
            info.Should().NotBeNull();
            info!.PropertyType.Should().Be(expectedType);
        }

        [TestMethod]
        public void GenerateObjectFromTable_PropertiesAccessable()
        {
            string connectionName=RandomDataGenerator.Instance.GenerateString(10);

            string schemaName=RandomDataGenerator.Instance.GenerateString(10);
            string tableName=RandomDataGenerator.Instance.GenerateString(10);
            string stringFieldName =  "MyInt";

            string fieldtype = "int";
            int isNullable = 0;
            short maxSize = 4;
            byte precision = 0;
            byte scale = 0;
            int testValue = RandomDataGenerator.Instance.GenerateInt();
            InitializeStandardSetup(connectionName);
            MockReader.Read().Returns(true, false);

            MockReader.GetString(0).Returns(stringFieldName);
            MockReader.GetString(1).Returns(fieldtype);
            MockReader.GetInt32(2).Returns(isNullable);
            MockReader.GetInt16(3).Returns(maxSize);
            MockReader.GetByte(4).Returns(precision);
            MockReader.GetByte(5).Returns(scale);

            MockReader.GetBoolean(6).Returns(false);
            MockReader.GetInt32(7).Returns(0);
            MockReader.GetBoolean(8).Returns(false);
            TableToClassBuilder testClass = new(MockTestDatabaseFactory);

            dynamic generateClass = testClass.GenerateObjectFromTable(connectionName, schemaName, tableName)!;
            Type generateClassType= generateClass.GetType();
            PropertyInfo? info = generateClassType.GetProperties().FirstOrDefault(p=>p.Name==stringFieldName);
            info.Should().NotBeNull();
            ((int)generateClass.MyInt).Should().Be(0);
            generateClass.MyInt = testValue;
            ((int)generateClass.MyInt).Should().Be(testValue );
        }
    }   
}
