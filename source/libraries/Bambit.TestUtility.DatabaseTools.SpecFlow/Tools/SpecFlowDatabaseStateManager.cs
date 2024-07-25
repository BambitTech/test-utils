using System.Configuration;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Configuration;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;
using TechTalk.SpecFlow;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Tools
{
    public class SpecFlowDatabaseStateManager : IDatabaseStateManager
    {
        
        public static readonly string LastProcDatabaseKey = nameof(LastProcDatabaseKey);
        public static readonly string LastResultsKey = nameof(LastResultsKey);
        public static readonly string IsDebuggingKey = nameof(IsDebuggingKey);
        public static readonly string VariablesKey = nameof(VariablesKey);

        protected ITestDatabaseFactory TestDatabaseFactory => Context.Get<ITestDatabaseFactory>();

        protected readonly ScenarioContext Context;

        public SpecFlowDatabaseStateManager(ScenarioContext context)
        {
            Context = context;
            Variables = new();
        }

        public SpecFlowUtilitiesConfiguration Configuration => Context.Get<SpecFlowUtilitiesConfiguration>();
        public ITestDbConnection OpenConnectionForName(string connectionName)
        {
            LastDatabaseConnectionName = connectionName;
            ITestDbConnection dbConnection = TestDatabaseFactory.GetConnection(connectionName);
            dbConnection.CommandTimeout=Configuration.TimeoutSeconds!.Value;
            dbConnection.Open();
            
            return dbConnection;
        }
        
        public DatabaseClassFactory DatabaseClassFactory => Context.Get<DatabaseClassFactory>();

        public IDatabaseCatalogRecord GetCurrentConnector()
        {
            
            return TestDatabaseFactory.GetGenerator(LastDatabaseConnectionName);
        }
        
        public Dictionary<string, object> Variables
        {
            get => Context.Get<Dictionary<string, object>>(VariablesKey);
            private init => Context[VariablesKey] = value;
        }

        public MappedTable LastResultSet
        {
            get =>
                Context.Get<MappedTable>(LastResultsKey);
            set
            {
                if (Context.ContainsKey(LastResultsKey))
                    Context[LastResultsKey] = value;
                else
                    Context.Add(LastResultsKey, value);
            }
        }

        
        public string LastDatabaseConnectionName
        {
            get =>
                Context.Get<string>(LastProcDatabaseKey) ??
                ConfigurationManager.ConnectionStrings[0].Name;
            set => Context[LastProcDatabaseKey]= value;
        }

        
        public bool IsDebugModeOn =>Context.ContainsKey(IsDebuggingKey) 
                                    && (Context.Get<bool?>(IsDebuggingKey) ?? false);
    }
}
