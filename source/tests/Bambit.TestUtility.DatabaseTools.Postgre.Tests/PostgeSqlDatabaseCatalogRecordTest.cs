using System.Data;
using Bambit.TestUtility.DataGeneration;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Bambit.TestUtility.DatabaseTools.Postgres.Tests
{
    [TestClass]
    [TestCategory("Integration")]
    public class PostgeSqlDatabaseCatalogRecordTest
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
        
        [TestMethod]
        public void GetConnection_ReturnsConnection()
        {
            PostgeSqlDatabaseCatalogRecord catalogRecord = new();
            IDbConnection dbConnection = catalogRecord.GetConnection(Configuration.GetConnectionString("IntegrationTestDatabase")!);
            dbConnection.Should().NotBeNull();
            dbConnection.Open();
            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = "select 1 + 1";
            int res=(int )dbCommand.ExecuteScalar()!;
            res.Should().Be(2);
        }

        [TestMethod]
        public void EscapeToken_WrapsToken()
        {
            
            PostgeSqlDatabaseCatalogRecord catalogRecord = new();
            string token = RandomDataGenerator.Instance.GenerateString(10);
            catalogRecord.EscapeToken(token).Should().Be($"[{token}]");
        }
    }
}
