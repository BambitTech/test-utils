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
        readonly ITestDbConnection MockTestConnection = Substitute.For<ITestDbConnection >();
        readonly IDbCommand MockCommand = Substitute.For<IDbCommand>();
        readonly IDataReader MockReader = Substitute.For<IDataReader>();
        readonly IDatabaseCatalogRecord MockCatalogRecord = Substitute.For<IDatabaseCatalogRecord>();

        public void InitializeStandardSetup(string connectionName)
        {
            string connectionString = RandomDataGenerator.Instance.GenerateString(10);

            MockTestDatabaseFactory.GetGenerator(Arg.Any<string>()).Returns(MockCatalogRecord);
            MockTestDatabaseFactory.GetConnectionString(connectionName).Returns(connectionString);
            MockTestDatabaseFactory.GetConnection(connectionName).Returns(MockTestConnection);
            MockCatalogRecord.GetConnection(connectionString).CreateCommand().Returns(MockCommand);
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
        [DataRow("float", false, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("real", false, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("double", false, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("bigint", false, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("timestamp", false, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("long", false, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("varbinary", false, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("image", false, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("binary", false, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("bytearray", false, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("boolean", false, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("bool", false, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("bit", false, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("date", false, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetime", false, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("smalldatetime", false, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetime2", false, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetimeoffset", false, (short)16,default(byte), default(byte),typeof(DateTimeOffset))]
        [DataRow("money", false, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("numeric", false, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("decimal", false, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("smallmoney", false, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("int", false, (short)16,default(byte), default(byte),typeof(int))]
        [DataRow("nchar", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("ntext", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("nvarchar", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("varchar", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("text", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("char", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("xml", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("smallint", false, (short)16,default(byte), default(byte),typeof(short))]
        [DataRow("short", false, (short)16,default(byte), default(byte),typeof(short))]
        [DataRow("time", false, (short)16,default(byte), default(byte),typeof(TimeSpan))]
        [DataRow("timespan", false, (short)16,default(byte), default(byte),typeof(TimeSpan))]
        [DataRow("tinyint", false, (short)16,default(byte), default(byte),typeof(byte))]
        [DataRow("byte", false, (short)16,default(byte), default(byte),typeof(byte))]
        [DataRow("uniqueidentifier", false, (short)16,default(byte), default(byte),typeof(Guid))]

        
        [DataRow("float", true, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("real", true, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("double", true, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("bigint", true, (short)16,default(byte), default(byte),typeof(long?))]
        [DataRow("timestamp", true, (short)16,default(byte), default(byte),typeof(long?))]
        [DataRow("long", true, (short)16,default(byte), default(byte),typeof(long?))]

        [DataRow("varbinary", true, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("image", true, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("binary", true, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("bytearray", true, (short)16,default(byte), default(byte),typeof(byte[]))]

        [DataRow("boolean", true, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("bool", true, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("bit", true, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("date", true, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetime", true, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("smalldatetime", true, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetime2", true, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetimeoffset", true, (short)16,default(byte), default(byte),typeof(DateTimeOffset?))]
        [DataRow("money", true, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("numeric", true, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("decimal", true, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("smallmoney", true, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("int", true, (short)16,default(byte), default(byte),typeof(int?))]

        [DataRow("nchar", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("ntext", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("nvarchar", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("varchar", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("text", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("char", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("xml", true, (short)16,default(byte), default(byte),typeof(string))]

        [DataRow("smallint", true, (short)16,default(byte), default(byte),typeof(short?))]
        [DataRow("short", true, (short)16,default(byte), default(byte),typeof(short?))]
        [DataRow("time", true, (short)16,default(byte), default(byte),typeof(TimeSpan?))]
        [DataRow("timespan", true, (short)16,default(byte), default(byte),typeof(TimeSpan?))]
        [DataRow("tinyint", true, (short)16,default(byte), default(byte),typeof(byte?))]
        [DataRow("byte", true, (short)16,default(byte), default(byte),typeof(byte?))]
        [DataRow("uniqueidentifier", true, (short)16,default(byte), default(byte),typeof(Guid?))]
        public void GenerateClassTypeFromTable_NonGeneratedFields_CreatesProperty(string fieldName, bool isNullable, short maxSize,
            byte precision,byte scale, Type expectedType)
        {
            string connectionName=RandomDataGenerator.Instance.GenerateString(10);

            string schemaName=RandomDataGenerator.Instance.GenerateString(10);
            string tableName=RandomDataGenerator.Instance.GenerateString(10);
            string stringFieldName = RandomDataGenerator.Instance.GenerateString(5);


            InitializeStandardSetup(connectionName);
            IList<DatabaseMappedClassPropertyDefinition> fieldList =
            [
                new DatabaseMappedClassPropertyDefinition
                {
                    IsComputed = true, IsNullable = isNullable, MappedType = expectedType, MaxSize = maxSize,
                    Name = stringFieldName, Scale = scale, Precision = precision, SourceType = fieldName
                }
            ];
            MockTestConnection.GetProperties(schemaName, tableName).Returns(fieldList);
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
        [DataRow("float", false, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("real", false, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("double", false, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("bigint", false, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("timestamp", false, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("long", false, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("varbinary", false, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("image", false, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("binary", false, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("bytearray", false, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("boolean", false, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("bool", false, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("bit", false, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("date", false, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetime", false, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("smalldatetime", false, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetime2", false, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetimeoffset", false, (short)16,default(byte), default(byte),typeof(DateTimeOffset))]
        [DataRow("money", false, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("numeric", false, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("decimal", false, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("smallmoney", false, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("int", false, (short)16,default(byte), default(byte),typeof(int))]
        [DataRow("nchar", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("ntext", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("nvarchar", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("varchar", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("text", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("char", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("xml", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("smallint", false, (short)16,default(byte), default(byte),typeof(short))]
        [DataRow("short", false, (short)16,default(byte), default(byte),typeof(short))]
        [DataRow("time", false, (short)16,default(byte), default(byte),typeof(TimeSpan))]
        [DataRow("timespan", false, (short)16,default(byte), default(byte),typeof(TimeSpan))]
        [DataRow("tinyint", false, (short)16,default(byte), default(byte),typeof(byte))]
        [DataRow("byte", false, (short)16,default(byte), default(byte),typeof(byte))]
        [DataRow("uniqueidentifier", false, (short)16,default(byte), default(byte),typeof(Guid))]

        
        [DataRow("float", true, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("real", true, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("double", true, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("bigint", true, (short)16,default(byte), default(byte),typeof(long?))]
        [DataRow("timestamp", true, (short)16,default(byte), default(byte),typeof(long?))]
        [DataRow("long", true, (short)16,default(byte), default(byte),typeof(long?))]

        [DataRow("varbinary", true, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("image", true, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("binary", true, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("bytearray", true, (short)16,default(byte), default(byte),typeof(byte[]))]

        [DataRow("boolean", true, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("bool", true, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("bit", true, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("date", true, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetime", true, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("smalldatetime", true, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetime2", true, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetimeoffset", true, (short)16,default(byte), default(byte),typeof(DateTimeOffset?))]
        [DataRow("money", true, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("numeric", true, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("decimal", true, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("smallmoney", true, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("int", true, (short)16,default(byte), default(byte),typeof(int?))]

        [DataRow("nchar", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("ntext", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("nvarchar", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("varchar", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("text", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("char", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("xml", true, (short)16,default(byte), default(byte),typeof(string))]

        [DataRow("smallint", true, (short)16,default(byte), default(byte),typeof(short?))]
        [DataRow("short", true, (short)16,default(byte), default(byte),typeof(short?))]
        [DataRow("time", true, (short)16,default(byte), default(byte),typeof(TimeSpan?))]
        [DataRow("timespan", true, (short)16,default(byte), default(byte),typeof(TimeSpan?))]
        [DataRow("tinyint", true, (short)16,default(byte), default(byte),typeof(byte?))]
        [DataRow("byte", true, (short)16,default(byte), default(byte),typeof(byte?))]
        [DataRow("uniqueidentifier", true, (short)16,default(byte), default(byte),typeof(Guid?))]
        public void GenerateClassTypeFromTable_FieldIsGenerated_AddsComputedColumnAttribute(string fieldName, bool isNullable, short maxSize,
            byte precision,byte scale, Type expectedType)
        {
            string connectionName=RandomDataGenerator.Instance.GenerateString(10);

            string schemaName=RandomDataGenerator.Instance.GenerateString(10);
            string tableName=RandomDataGenerator.Instance.GenerateString(10);
            string stringFieldName = RandomDataGenerator.Instance.GenerateString(5);


            InitializeStandardSetup(connectionName);
            IList<DatabaseMappedClassPropertyDefinition> fieldList =
            [
                new()
                {
                    IsComputed = true, IsNullable = isNullable, MappedType = expectedType, MaxSize = maxSize,
                    Name = stringFieldName, Scale = scale, Precision = precision, SourceType = fieldName
                }
            ];
            MockTestConnection.GetProperties(schemaName, tableName).Returns(fieldList);
            /*
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
            */
            TableToClassBuilder testClass = new(MockTestDatabaseFactory);

            Type generateClassType= testClass.GenerateClassTypeFromTable(connectionName, schemaName, tableName);
            generateClassType.Should().NotBeNull();
            PropertyInfo? info = generateClassType.GetProperties().FirstOrDefault(p=>p.Name==stringFieldName);
            info.Should().NotBeNull();
            Attribute? attribute = info!.GetCustomAttributes(typeof(ComputedColumnAttribute)).SingleOrDefault();
            attribute.Should().NotBeNull().And.BeOfType<ComputedColumnAttribute>();
        }


        
        [DataTestMethod]
        [DataRow("float", false, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("real", false, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("double", false, (short)16,default(byte), default(byte),typeof(double))]
        [DataRow("bigint", false, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("timestamp", false, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("long", false, (short)16,default(byte), default(byte),typeof(long))]
        [DataRow("varbinary", false, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("image", false, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("binary", false, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("bytearray", false, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("boolean", false, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("bool", false, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("bit", false, (short)16,default(byte), default(byte),typeof(bool))]
        [DataRow("date", false, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetime", false, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("smalldatetime", false, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetime2", false, (short)16,default(byte), default(byte),typeof(DateTime))]
        [DataRow("datetimeoffset", false, (short)16,default(byte), default(byte),typeof(DateTimeOffset))]
        [DataRow("money", false, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("numeric", false, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("decimal", false, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("smallmoney", false, (short)16,default(byte), default(byte),typeof(decimal))]
        [DataRow("int", false, (short)16,default(byte), default(byte),typeof(int))]
        [DataRow("nchar", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("ntext", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("nvarchar", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("varchar", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("text", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("char", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("xml", false, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("smallint", false, (short)16,default(byte), default(byte),typeof(short))]
        [DataRow("short", false, (short)16,default(byte), default(byte),typeof(short))]
        [DataRow("time", false, (short)16,default(byte), default(byte),typeof(TimeSpan))]
        [DataRow("timespan", false, (short)16,default(byte), default(byte),typeof(TimeSpan))]
        [DataRow("tinyint", false, (short)16,default(byte), default(byte),typeof(byte))]
        [DataRow("byte", false, (short)16,default(byte), default(byte),typeof(byte))]
        [DataRow("uniqueidentifier", false, (short)16,default(byte), default(byte),typeof(Guid))]

        
        [DataRow("float", true, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("real", true, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("double", true, (short)16,default(byte), default(byte),typeof(double?))]
        [DataRow("bigint", true, (short)16,default(byte), default(byte),typeof(long?))]
        [DataRow("timestamp", true, (short)16,default(byte), default(byte),typeof(long?))]
        [DataRow("long", true, (short)16,default(byte), default(byte),typeof(long?))]

        [DataRow("varbinary", true, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("image", true, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("binary", true, (short)16,default(byte), default(byte),typeof(byte[]))]
        [DataRow("bytearray", true, (short)16,default(byte), default(byte),typeof(byte[]))]

        [DataRow("boolean", true, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("bool", true, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("bit", true, (short)16,default(byte), default(byte),typeof(bool?))]
        [DataRow("date", true, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetime", true, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("smalldatetime", true, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetime2", true, (short)16,default(byte), default(byte),typeof(DateTime?))]
        [DataRow("datetimeoffset", true, (short)16,default(byte), default(byte),typeof(DateTimeOffset?))]
        [DataRow("money", true, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("numeric", true, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("decimal", true, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("smallmoney", true, (short)16,default(byte), default(byte),typeof(decimal?))]
        [DataRow("int", true, (short)16,default(byte), default(byte),typeof(int?))]

        [DataRow("nchar", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("ntext", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("nvarchar", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("varchar", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("text", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("char", true, (short)16,default(byte), default(byte),typeof(string))]
        [DataRow("xml", true, (short)16,default(byte), default(byte),typeof(string))]

        [DataRow("smallint", true, (short)16,default(byte), default(byte),typeof(short?))]
        [DataRow("short", true, (short)16,default(byte), default(byte),typeof(short?))]
        [DataRow("time", true, (short)16,default(byte), default(byte),typeof(TimeSpan?))]
        [DataRow("timespan", true, (short)16,default(byte), default(byte),typeof(TimeSpan?))]
        [DataRow("tinyint", true, (short)16,default(byte), default(byte),typeof(byte?))]
        [DataRow("byte", true, (short)16,default(byte), default(byte),typeof(byte?))]
        [DataRow("uniqueidentifier", true, (short)16,default(byte), default(byte),typeof(Guid?))]
        public void GenerateObjectFromTable_NonGeneratedFields_CreatesProperty(string fieldName, bool isNullable,  short maxSize,
            byte precision,byte scale, Type expectedType)
        {
            string connectionName=RandomDataGenerator.Instance.GenerateString(10);

            string schemaName=RandomDataGenerator.Instance.GenerateString(10);
            string tableName=RandomDataGenerator.Instance.GenerateString(10);
            string stringFieldName = RandomDataGenerator.Instance.GenerateString(5);


            InitializeStandardSetup(connectionName);
            IList<DatabaseMappedClassPropertyDefinition> fieldList =
            [
                new DatabaseMappedClassPropertyDefinition
                {
                    IsComputed = false, IsNullable = isNullable, MappedType = expectedType, MaxSize = maxSize,
                    Name = stringFieldName, Scale = scale, Precision = precision, SourceType = fieldName
                }
            ];
            MockTestConnection.GetProperties(schemaName, tableName).Returns(fieldList);
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
            MockTestConnection.GetProperties(schemaName, tableName).Returns(
                [
                    new()
                    {
                        IsComputed = false, IsNullable = false, Name = stringFieldName, SourceType = fieldtype,
                        MappedType = typeof(int), Scale = scale, Precision = precision, MaxSize = maxSize
                    }
                ]);
            dynamic generateClass = testClass.GenerateObjectFromTable(connectionName, schemaName, tableName)!;
            Type generateClassType= generateClass.GetType();
            PropertyInfo? info = generateClassType.GetProperties().FirstOrDefault(p=>
                string.Compare( p.Name,stringFieldName, StringComparison.CurrentCultureIgnoreCase)==0
                );
            info.Should().NotBeNull();
            ((int)generateClass.MyInt).Should().Be(0);
            generateClass.MyInt = testValue;
            ((int)generateClass.MyInt).Should().Be(testValue );
        }
    }   
}
