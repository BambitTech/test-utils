using System.Data;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Configuration;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Tools;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Specflow hooks </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    [Binding]
    public class Hooks(ScenarioContext context, ISpecFlowOutputHelper outputHelper)
    {
        private const string HandlerKey = nameof(HandlerKey);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Before test run. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [BeforeTestRun(Order = 50)]
        public static void BeforeTestRun()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("bambit.utilities.json")
                .AddJsonFile("bambit.utilities.local.json", true)
                .Build();
            SpecFlowUtilitiesConfiguration? specFlowConfig =
                configuration.GetSection("specFlow").Get<SpecFlowUtilitiesConfiguration>();

            TestDatabaseFactoryOptions testDatabaseFactoryOptions = configuration.GetSection("databaseFactory").Get<TestDatabaseFactoryOptions>()!;
            Config.Initialize(testDatabaseFactoryOptions, specFlowConfig ?? new SpecFlowUtilitiesConfiguration());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Before scenario register context variables. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="context">      The context. </param>
        /// <param name="outputHelper"> The output helper. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [BeforeScenario]
        public void BeforeScenarioRegisterContextVariables(ScenarioContext context, ISpecFlowOutputHelper outputHelper)
        {
            Config.RegisterContextVariables(context);
            void Handler(object? _, TestDbConnectionMessageReceivedEvent packet) =>
                outputHelper.WriteLine(packet.Message);

            Config.Configuration.TestDatabaseFactory.MessageReceived += Handler;
            context.Set((EventHandler<TestDbConnectionMessageReceivedEvent>)Handler, HandlerKey);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Handler, called when the after scenario unregister message. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="context">      The context. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [AfterScenario]
#pragma warning disable CA1822 // Mark members as static
        public void AfterScenarioUnregisterMessageHandler(ScenarioContext context)
#pragma warning restore CA1822 // Mark members as static
        {
            EventHandler<TestDbConnectionMessageReceivedEvent> handler =
                context.Get<EventHandler<TestDbConnectionMessageReceivedEvent>>(HandlerKey);
            if (handler != null)
            {
                Config.Configuration.TestDatabaseFactory.MessageReceived -= handler;
            }


        }


        [AfterScenario]
        public void RestoreMocks()
        {

            Stack<MockedObject> mocks;
            if (context.TryGetValue(out mocks))
            {
                while (mocks.Count > 0)
                {
                    RestoreMockedObject(mocks.Pop());
                }
            }
            
        }


        private void RestoreMockedObject(MockedObject mockedObject)
        {
            try
            {
                SpecFlowDatabaseStateManager stateManager = new SpecFlowDatabaseStateManager(context);
                ITestDbConnection testDbConnection =
                    stateManager .OpenConnectionForName(mockedObject.ConnectionName);
                using IDbCommand command = testDbConnection.CreateCommand();

                foreach (string restoreScript in mockedObject.RestoreScripts)
                {
                    command.CommandText = restoreScript;
                    command.ExecuteNonQuery();
                }


                if (mockedObject.DatabseObjectType == DatabseObjectType.Table)
                    stateManager.DatabaseClassFactory.PurgeTableDefinition(mockedObject.ConnectionName,
                        mockedObject.OriginalSchema, mockedObject.OriginalName);

            }
            catch (Exception ex)
            {
                if (mockedObject == null)
                {
                    outputHelper.WriteLine("Attempt to restore null object");
                    throw;
                }
                outputHelper.WriteLine($"An error was thrown trying to restore object '{mockedObject.OriginalName}': ");
                outputHelper.WriteLine($"\t'Connection: {mockedObject.ConnectionName}': ");
                outputHelper.WriteLine($"\t'Restore scripts: {mockedObject.RestoreScripts?.Count}': ");
            }
        }

    }
}
