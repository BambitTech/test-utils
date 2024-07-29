using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Bambit.TestUtility.DatabaseTools.SqlServer.Tests;

[TestClass]
public class TableToClassBuilderTest
{
    protected IConfiguration Configuration { get; set; } = null!;

    [TestInitialize]
    public void TestInitialize()
    {   
        Configuration=new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .AddJsonFile("appSettings.local.json", true)
            .Build();
    }

    
    protected TableToClassBuilder GetClassBuilder()
    {
        TestDatabaseFactoryOptions options = Configuration.GetSection("DatabaseFactory").Get<TestDatabaseFactoryOptions>()!;

        TestDatabaseFactory testDatabaseFactory = new(options);
        TableToClassBuilder builder = new(testDatabaseFactory);
        return builder;
    }

    [TestMethod]
    public void GenerateClassTypeFromTable_NullableValues_ReturnsAllExpectedFields()
    {
        TableToClassBuilder classBuilder = GetClassBuilder();
        Type generateClassType = classBuilder.GenerateClassTypeFromTable("Test1", "Test", "[testTableNullable]");

        generateClassType.Should().NotBeNull();
        ValidatePropertyExistsAsExpected(generateClassType, "bigIntField", typeof(long?));
        ValidatePropertyExistsAsExpected(generateClassType, "TimeStampField", typeof(long));
        ValidatePropertyExistsAsExpected(generateClassType, "imageField", typeof(byte[]));
        ValidatePropertyExistsAsExpected(generateClassType, "binaryField", typeof(byte[]));
        ValidatePropertyExistsAsExpected(generateClassType, "varbinaryField", typeof(byte[]));
        ValidatePropertyExistsAsExpected(generateClassType, "bitField", typeof(bool?));
        ValidatePropertyExistsAsExpected(generateClassType, "dateField", typeof(DateTime?));
        ValidatePropertyExistsAsExpected(generateClassType, "dateTimeField", typeof(DateTime?));
        ValidatePropertyExistsAsExpected(generateClassType, "dateTime2Field", typeof(DateTime?));
        ValidatePropertyExistsAsExpected(generateClassType, "smallDateTimeField", typeof(DateTime?));
        ValidatePropertyExistsAsExpected(generateClassType, "smallDateTimeField", typeof(DateTime?));
        ValidatePropertyExistsAsExpected(generateClassType, "dateTimeOffsetField", typeof(DateTimeOffset?));
        ValidatePropertyExistsAsExpected(generateClassType, "moneyField", typeof(decimal?));
        ValidatePropertyExistsAsExpected(generateClassType, "numericField", typeof(decimal?));
        ValidatePropertyExistsAsExpected(generateClassType, "smallMoneyField", typeof(decimal?));
        ValidatePropertyExistsAsExpected(generateClassType, "decimalField", typeof(decimal?));
        ValidatePropertyExistsAsExpected(generateClassType, "floatField", typeof(double?));
        ValidatePropertyExistsAsExpected(generateClassType, "intField", typeof(int?));
        ValidatePropertyExistsAsExpected(generateClassType, "ncharField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "ntextField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "ncharField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "nvarcharField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "charField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "textField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "varcharField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "xmlField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "smallIntField", typeof(short?));
        ValidatePropertyExistsAsExpected(generateClassType, "timeField", typeof(TimeSpan?));
        ValidatePropertyExistsAsExpected(generateClassType, "tinyintfield", typeof(byte?));
        ValidatePropertyExistsAsExpected(generateClassType, "uniqueidentifierField", typeof(Guid?));
    }
    
    [TestMethod]
    public void GenerateClassTypeFromTable_NotNullableValues_ReturnsAllExpectedFields()
    {
        TableToClassBuilder classBuilder = GetClassBuilder();
        Type generateClassType = classBuilder.GenerateClassTypeFromTable("Test1", "Test", "[testTableNotNullable]");

        generateClassType.Should().NotBeNull();
        ValidatePropertyExistsAsExpected(generateClassType, "bigIntField", typeof(long));
        ValidatePropertyExistsAsExpected(generateClassType, "TimeStampField", typeof(long));
        ValidatePropertyExistsAsExpected(generateClassType, "imageField", typeof(byte[]));
        ValidatePropertyExistsAsExpected(generateClassType, "binaryField", typeof(byte[]));
        ValidatePropertyExistsAsExpected(generateClassType, "varbinaryField", typeof(byte[]));
        ValidatePropertyExistsAsExpected(generateClassType, "bitField", typeof(bool));
        ValidatePropertyExistsAsExpected(generateClassType, "dateField", typeof(DateTime));
        ValidatePropertyExistsAsExpected(generateClassType, "dateTimeField", typeof(DateTime));
        ValidatePropertyExistsAsExpected(generateClassType, "dateTime2Field", typeof(DateTime));
        ValidatePropertyExistsAsExpected(generateClassType, "smallDateTimeField", typeof(DateTime));
        ValidatePropertyExistsAsExpected(generateClassType, "smallDateTimeField", typeof(DateTime));
        ValidatePropertyExistsAsExpected(generateClassType, "dateTimeOffsetField", typeof(DateTimeOffset));
        ValidatePropertyExistsAsExpected(generateClassType, "moneyField", typeof(decimal));
        ValidatePropertyExistsAsExpected(generateClassType, "numericField", typeof(decimal));
        ValidatePropertyExistsAsExpected(generateClassType, "smallMoneyField", typeof(decimal));
        ValidatePropertyExistsAsExpected(generateClassType, "decimalField", typeof(decimal));
        ValidatePropertyExistsAsExpected(generateClassType, "floatField", typeof(double));
        ValidatePropertyExistsAsExpected(generateClassType, "intField", typeof(int));
        ValidatePropertyExistsAsExpected(generateClassType, "ncharField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "ntextField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "ncharField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "nvarcharField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "charField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "textField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "varcharField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "xmlField", typeof(string));
        ValidatePropertyExistsAsExpected(generateClassType, "smallIntField", typeof(short));
        ValidatePropertyExistsAsExpected(generateClassType, "timeField", typeof(TimeSpan));
        ValidatePropertyExistsAsExpected(generateClassType, "tinyintfield", typeof(byte));
        ValidatePropertyExistsAsExpected(generateClassType, "uniqueidentifierField", typeof(Guid));
    }
    
    public static void ValidatePropertyExistsAsExpected(Type testType, string propertyName, Type expectedType)
    {
        PropertyInfo? info = testType.GetProperties().FirstOrDefault(p=>p.Name==propertyName);
        info.Should().NotBeNull();
        info!.PropertyType.Should().Be(expectedType);
    }
}