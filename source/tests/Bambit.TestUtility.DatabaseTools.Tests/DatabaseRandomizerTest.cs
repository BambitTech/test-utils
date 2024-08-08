using Bambit.TestUtility.DatabaseTools.Attributes;
using Bambit.TestUtility.DataGeneration;
using FluentAssertions;
using NSubstitute;

namespace Bambit.TestUtility.DatabaseTools.Tests;

[TestClass]
public class DatabaseRandomizerTest
{
    private const string ConnectionName = "dummyConnection";
    private const string TableName = "DummyTable";
    private const string Schema= "dbo";
    private const string FieldName = "TestField";
    [TableSource(ConnectionName, Schema,TableName)]
    public class RegisteredTestClass
    {
        [FieldSource(FieldName,typeof(string), "string")] public string TestField { get; set; } = null!;
    }

    public class UnregisteredTestClass
    {
        public string TestField { get; set; }=null!;
    }
    readonly ITestDatabaseFactory MockFactory = Substitute.For<ITestDatabaseFactory>();
    
    [TestMethod]
    public void RegisterTableFieldGenerator_FieldIsIntializedAsPrescribed()
    {
        MockFactory.GetCleanIdentifierFunc(Arg.Any<string>()).ReturnsForAnyArgs(x => (y)=>y);
        MockFactory.GetEscapeIdentifierFunc(Arg.Any<string>()).ReturnsForAnyArgs(x => (y=>y));

        string expected = RandomDataGenerator.Instance.GenerateString(10);
        DatabaseRandomizer randomizer = new(MockFactory);
        RegisteredTestClass registeredTestClass=new();
        randomizer.RegisterTableFieldGenerator(ConnectionName,  Schema, TableName,FieldName,
            (rdg) => expected);
        randomizer.InitializeObject(registeredTestClass);
        registeredTestClass.TestField.Should().Be(expected);
    }

    
    [TestMethod]
    public void RegisterTableFieldGenerator_NonRegisteredClass_CreatesRandom()
    {
        MockFactory.GetCleanIdentifierFunc(Arg.Any<string>()).ReturnsForAnyArgs(x => (y)=>y);
        MockFactory.GetEscapeIdentifierFunc(Arg.Any<string>()).ReturnsForAnyArgs(x => (y=>y));

        string expected = RandomDataGenerator.Instance.GenerateString(10);
        DatabaseRandomizer randomizer = new(MockFactory);
        UnregisteredTestClass registeredTestClass=new();
        randomizer.RegisterTableFieldGenerator(ConnectionName,  Schema, TableName,FieldName,
            (rdg) => expected);
        randomizer.InitializeObject(registeredTestClass);
        registeredTestClass.TestField.Should().NotBeNullOrWhiteSpace().And.NotBe(expected);
    }
    [TestMethod]
    public void RegisterTableFieldGenerator_FieldIsNotMatched_InitializesRandom()
    {
        MockFactory.GetCleanIdentifierFunc(Arg.Any<string>()).ReturnsForAnyArgs(x => (y)=>y);
        MockFactory.GetEscapeIdentifierFunc(Arg.Any<string>()).ReturnsForAnyArgs(x => (y=>y));

        string expected = RandomDataGenerator.Instance.GenerateString(10);
        DatabaseRandomizer randomizer = new(MockFactory);
        RegisteredTestClass registeredTestClass=new();
        randomizer.RegisterTableFieldGenerator(ConnectionName,  Schema, TableName,"Invalid",
            (rdg) => expected);
        randomizer.InitializeObject(registeredTestClass);
        registeredTestClass.TestField.Should().NotBeNullOrWhiteSpace().And.NotBe(expected);
    }
}