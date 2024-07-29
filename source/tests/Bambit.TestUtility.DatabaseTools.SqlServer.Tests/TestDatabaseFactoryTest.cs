using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Data;
using Bambit.TestUtility.DataGeneration;

namespace Bambit.TestUtility.DatabaseTools.SqlServer.Tests;

[TestClass]
public class TestDatabaseFactoryTest
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

    protected TestDatabaseFactory GetDatabaseFactory()
    {
        TestDatabaseFactoryOptions options = Configuration.GetSection("DatabaseFactory").Get<TestDatabaseFactoryOptions>()!;
        return new TestDatabaseFactory(options);

    }
    [TestMethod]
    public void GetConnectionString_ValidName_ReturnsConnectionString()
    {
        TestDatabaseFactory factory=GetDatabaseFactory();
        
        string connectionString = factory.GetConnectionString("Test1");
        connectionString.Should().NotBeNull();
    }
    [TestMethod]
    public void GetGenerator_ValidName_ReturnsGenerator()
    {
        TestDatabaseFactory factory=GetDatabaseFactory();
        
        IDatabaseCatalogRecord generator= factory.GetGenerator("Test1");
        generator.Should().NotBeNull();
    }
    
    [TestMethod]
    public void GetConnection_ValidName_ReturnsGenerator()
    {
        TestDatabaseFactory factory=GetDatabaseFactory();
        
        IDbConnection dbConnection = factory.GetConnection("Test1");
        dbConnection.Should().NotBeNull();
        dbConnection.Open();
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "select 1 + 1";
        int res=(int )dbCommand.ExecuteScalar()!;
        res.Should().Be(2);
    }



    [TestMethod]
    public void TrackInfoMessages_DatabaseGeneratesMessage_BubblesMessage()
    {
        
        TestDatabaseFactory factory=GetDatabaseFactory();
        IList<string> printMessage1 = RandomDataGenerator.Instance.InitializeList(10, 10);
        ITestDbConnection dbConnection = factory.GetConnection("Test1");
        dbConnection.TrackInfoMessages();
        dbConnection.Open();
        foreach (string s in printMessage1)
        {
            dbConnection.ExecuteQuery($"print '{s}'");    
        }

        dbConnection.OutputMessages.Should().BeEquivalentTo(printMessage1);

    }


    

    [TestMethod]
    public void UntrackInfoMessages_DatabaseGeneratesMessage_MessagesNotBubbled()
    {
        
        TestDatabaseFactory factory=GetDatabaseFactory();
        ITestDbConnection dbConnection = factory.GetConnection("Test1");
        factory.MessageReceived += (_, eventArgs) =>
        {
            
            eventArgs.Connection.Should().Be(dbConnection);
        };
        IList<string> printMessage1 = RandomDataGenerator.Instance.InitializeList(10, 10);
        IList<string> printMessage2 = RandomDataGenerator.Instance.InitializeList(10, 10);
        dbConnection.TrackInfoMessages();
        dbConnection.Open();
        foreach (string s in printMessage1)
        {
            dbConnection.ExecuteQuery($"print '{s}'");    
        }
        dbConnection.UntrackInfoMessages();
        foreach (string s in printMessage2)
        {
            dbConnection.ExecuteQuery($"print '{s}'");    
        }
        dbConnection.OutputMessages.Should().BeEquivalentTo(printMessage1);

    }

    [TestMethod]
    public void GetCleanIdentifierFunc_ReturnsExpectedFunction()
    {
        TestDatabaseFactory factory=GetDatabaseFactory();
        IDatabaseCatalogRecord generator = factory.GetGenerator("Test1");
        Func<string, string> expected = generator.CleanQualifiers;
        factory.GetCleanIdentifierFunc("Test1").Should().Be(expected);
    }

    [TestMethod]
    public void GetEscapeIdentifierFunc_ReturnsExpectedFunction()
    {
        TestDatabaseFactory factory=GetDatabaseFactory();
        IDatabaseCatalogRecord generator = factory.GetGenerator("Test1");
        Func<string, string> expected = generator.EscapeToken;
        factory.GetEscapeIdentifierFunc("Test1").Should().Be(expected);
    }
}