using Microsoft.Extensions.Configuration;

namespace Bambit.TestUtility.DatabaseTools.Postgres.Tests
{
    [TestClass]
    public class DatabaseClassFactoryTest
    {
        protected IConfiguration Configuration { get; set; } = null!;
        protected ITestDatabaseFactory TestDatabaseFactory { get; set; } = null!;
        protected ITableToClassBuilder TableToClassBuilder { get; set; } = null!;
        
        
        [TestInitialize]
        public void TestInitialize()
        {
            Configuration=new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .Build();
            
            TestDatabaseFactoryOptions options = Configuration.GetSection("DatabaseFactory").Get<TestDatabaseFactoryOptions>()!;
            TestDatabaseFactory = new TestDatabaseFactory(options);
            TableToClassBuilder = new TableToClassBuilder(TestDatabaseFactory);
            
        }
    }
}
