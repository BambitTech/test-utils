using Bambit.TestUtility.DatabaseTools.SpecFlow.Configuration;
using TechTalk.SpecFlow;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    public static class Config
    {
        private static readonly object InstanceLock = new();
        internal class InstanceConfiguration
        {
            public InstanceConfiguration(TestDatabaseFactoryOptions testDatabaseFactoryOptions, SpecFlowUtilitiesConfiguration specFlowUtilitiesConfiguration)
            {
                TestDatabaseFactoryOptions = testDatabaseFactoryOptions;

                TestDatabaseFactory = new TestDatabaseFactory(testDatabaseFactoryOptions);
                DatabaseClassFactory = new DatabaseClassFactory(new TableToClassBuilder(TestDatabaseFactory));
                DatabaseRandomizer = DatabaseRandomizer = new DatabaseRandomizer(TestDatabaseFactory);
                SpecFlowUtilitiesConfiguration= specFlowUtilitiesConfiguration;
            }

            public SpecFlowUtilitiesConfiguration SpecFlowUtilitiesConfiguration { get; init; }
            public TestDatabaseFactoryOptions TestDatabaseFactoryOptions { get; init; }
            public TestDatabaseFactory  TestDatabaseFactory {get;  init; }
            public DatabaseClassFactory DatabaseClassFactory{get;  init; }
            public DatabaseRandomizer DatabaseRandomizer{get;  init; }

            
        }

        internal static InstanceConfiguration Configuration{
            get
            {
                VerifyConfigIsNotNull();
                return ConfigurationBacker!;
            }
        } 
        private static InstanceConfiguration? ConfigurationBacker { get; set; }

        internal static InstanceConfiguration Initialize(TestDatabaseFactoryOptions testDatabaseFactoryOptions, 
            SpecFlowUtilitiesConfiguration specFlowUtilitiesConfiguration)
        {
            lock (InstanceLock)
            {
                ConfigurationBacker ??= new InstanceConfiguration(testDatabaseFactoryOptions, specFlowUtilitiesConfiguration);
            }

            return Configuration;

        }

        private static void VerifyConfigIsNotNull()
        {
            if (ConfigurationBacker == null)
            {
                throw new NullReferenceException(
                    "Configuration has not been initialized, you must call Initialize before registering context variables");
            }
        }
        public static DatabaseRandomizer Randomizer => Configuration.DatabaseRandomizer;


        public static void RegisterContextVariables(ScenarioContext context)
        {
            VerifyConfigIsNotNull();
            context.Set<ITestDatabaseFactory>(ConfigurationBacker!.TestDatabaseFactory);
            context.Set(ConfigurationBacker.DatabaseClassFactory);
            context.Set(ConfigurationBacker.DatabaseRandomizer);
            context.Set(ConfigurationBacker.SpecFlowUtilitiesConfiguration.Clone() );
        }
    }
}
