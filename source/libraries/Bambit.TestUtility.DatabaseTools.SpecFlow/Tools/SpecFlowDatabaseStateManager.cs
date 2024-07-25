using System.Configuration;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Configuration;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;
using TechTalk.SpecFlow;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Tools
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Manager for specifier flow database states. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SpecFlowDatabaseStateManager : IDatabaseStateManager
    {
        
        /// <summary>   (Immutable) the last procedure database key. </summary>
        public static readonly string LastProcDatabaseKey = nameof(LastProcDatabaseKey);
        /// <summary>   (Immutable) the last results key. </summary>
        public static readonly string LastResultsKey = nameof(LastResultsKey);
        /// <summary>   (Immutable) the is debugging key. </summary>
        public static readonly string IsDebuggingKey = nameof(IsDebuggingKey);
        /// <summary>   (Immutable) the variables key. </summary>
        public static readonly string VariablesKey = nameof(VariablesKey);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the test database factory. </summary>
        ///
        /// <value> The test database factory. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected ITestDatabaseFactory TestDatabaseFactory => Context.Get<ITestDatabaseFactory>();

        /// <summary>   (Immutable) the context. </summary>
        protected readonly ScenarioContext Context;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SpecFlowDatabaseStateManager(ScenarioContext context)
        {
            Context = context;
            Variables = new();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the configuration. </summary>
        ///
        /// <value> The configuration. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public SpecFlowUtilitiesConfiguration Configuration => Context.Get<SpecFlowUtilitiesConfiguration>();

        /// <inheritdoc />
        public ITestDbConnection OpenConnectionForName(string connectionName)
        {
            LastDatabaseConnectionName = connectionName;
            ITestDbConnection dbConnection = TestDatabaseFactory.GetConnection(connectionName);
            dbConnection.CommandTimeout=Configuration.TimeoutSeconds!.Value;
            dbConnection.Open();
            
            return dbConnection;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the database class factory. </summary>
        ///
        /// <value> The database class factory. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DatabaseClassFactory DatabaseClassFactory => Context.Get<DatabaseClassFactory>();

        /// <inheritdoc />
        
        public IDatabaseCatalogRecord GetCurrentConnector()
        {
            
            return TestDatabaseFactory.GetGenerator(LastDatabaseConnectionName);
        }
        
        /// <inheritdoc />
        public Dictionary<string, object> Variables
        {
            get => Context.Get<Dictionary<string, object>>(VariablesKey);
            private init => Context[VariablesKey] = value;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the set the last result belongs to. </summary>
        ///
        /// <value> The last result set. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

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

        
        /// <inheritdoc />
        public string LastDatabaseConnectionName
        {
            get =>
                Context.Get<string>(LastProcDatabaseKey) ??
                ConfigurationManager.ConnectionStrings[0].Name;
            set => Context[LastProcDatabaseKey]= value;
        }

        
        /// <inheritdoc />
        public bool IsDebugModeOn =>Context.ContainsKey(IsDebuggingKey) 
                                    && (Context.Get<bool?>(IsDebuggingKey) ?? false);
    }
}
