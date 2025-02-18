using Bambit.TestUtility.DatabaseTools.Reqnroll.Configuration;
using Reqnroll;

namespace Bambit.TestUtility.DatabaseTools.Reqnroll;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A configuration. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class Config
    {
        /// <summary>   (Immutable) the instance lock. </summary>
        private static readonly object InstanceLock = new();

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   An instance configuration. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal class InstanceConfiguration
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Constructor. </summary>
            ///
            /// <remarks>   Law Metzler, 7/25/2024. </remarks>
            ///
            /// <param name="testDatabaseFactoryOptions">       Options that control the test database
            ///                                                 factory. </param>
            /// <param name="reqnrollUtilitiesConfiguration">   The specifier flow utilities configuration. </param>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public InstanceConfiguration(TestDatabaseFactoryOptions testDatabaseFactoryOptions,
                ReqnrollUtilitiesConfiguration reqnrollUtilitiesConfiguration)
            {
                TestDatabaseFactoryOptions = testDatabaseFactoryOptions;

                TestDatabaseFactory = new TestDatabaseFactory(testDatabaseFactoryOptions);
                DatabaseClassFactory = new DatabaseClassFactory(new TableToClassBuilder(TestDatabaseFactory));
                DatabaseRandomizer = DatabaseRandomizer = new DatabaseRandomizer(TestDatabaseFactory);
                ReqnrollUtilitiesConfiguration= reqnrollUtilitiesConfiguration;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets or initializes the specifier flow utilities configuration. </summary>
            ///
            /// <value> The specifier flow utilities configuration. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public ReqnrollUtilitiesConfiguration ReqnrollUtilitiesConfiguration { get; init; }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets or initializes options for controlling the test database factory. </summary>
            ///
            /// <value> Options that control the test database factory. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public TestDatabaseFactoryOptions TestDatabaseFactoryOptions { get; init; }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets or initializes the test database factory. </summary>
            ///
            /// <value> The test database factory. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public TestDatabaseFactory  TestDatabaseFactory {get;  init; }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets or initializes the database class factory. </summary>
            ///
            /// <value> The database class factory. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public DatabaseClassFactory DatabaseClassFactory{get;  init; }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>   Gets or initializes the database randomizer. </summary>
            ///
            /// <value> The database randomizer. </value>
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public DatabaseRandomizer DatabaseRandomizer{get;  init; }

            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the configuration. </summary>
        ///
        /// <value> The configuration. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static InstanceConfiguration Configuration{
            get
            {
                VerifyConfigIsNotNull();
                return ConfigurationBacker!;
            }
        } 

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the configuration backer. </summary>
        ///
        /// <value> The configuration backer. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static InstanceConfiguration? ConfigurationBacker { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes this object. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="testDatabaseFactoryOptions">       Options for controlling the test database
        ///                                                 factory. </param>
        /// <param name="ReqnrollUtilitiesConfiguration">   The specifier flow utilities configuration. </param>
        ///
        /// <returns>   An InstanceConfiguration. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        internal static InstanceConfiguration Initialize(TestDatabaseFactoryOptions testDatabaseFactoryOptions, 
            ReqnrollUtilitiesConfiguration ReqnrollUtilitiesConfiguration)
        {
            lock (InstanceLock)
            {
                ConfigurationBacker ??= new InstanceConfiguration(testDatabaseFactoryOptions, ReqnrollUtilitiesConfiguration);
            }

            return Configuration;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Verify configuration is not null. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <exception cref="NullReferenceException">   Thrown when a value was unexpectedly null. </exception>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void VerifyConfigIsNotNull()
        {
            if (ConfigurationBacker == null)
            {
                throw new NullReferenceException(
                    "Configuration has not been initialized, you must call Initialize before registering context variables");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the randomizer. </summary>
        ///
        /// <value> The randomizer. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static DatabaseRandomizer Randomizer => Configuration.DatabaseRandomizer;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Registers the context variables described by context. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="context">  The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void RegisterContextVariables(ScenarioContext context)
        {
            VerifyConfigIsNotNull();
            context.Set<ITestDatabaseFactory>(ConfigurationBacker!.TestDatabaseFactory);
            context.Set(ConfigurationBacker.DatabaseClassFactory);
            context.Set(ConfigurationBacker.DatabaseRandomizer);
            context.Set(ConfigurationBacker.ReqnrollUtilitiesConfiguration.Clone() );
        }
    }
