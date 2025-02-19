using System.Data;
using Bambit.TestUtility.DatabaseTools.Reqnroll;
using Bambit.TestUtility.DatabaseTools.Reqnroll.Configuration;
using Bambit.TestUtility.DatabaseTools.Reqnroll.Tools;
using Microsoft.Extensions.Configuration;
using Reqnroll;

namespace Bambit.TestUtility.DatabaseTools.Reqnroll
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Reqnroll hooks </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    [Binding]
    public class Hooks(ScenarioContext context, IReqnrollOutputHelper outputHelper)
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
                .AddJsonFile("bambit.utilities.json", true)
                .AddJsonFile("bambit.utilities.local.json", true)
                .Build();
            ReqnrollUtilitiesConfiguration? reqnrollConfig =
                configuration.GetSection("reqnroll").Get<ReqnrollUtilitiesConfiguration>();

            TestDatabaseFactoryOptions testDatabaseFactoryOptions = configuration.GetSection("databaseFactory")
                .Get<TestDatabaseFactoryOptions>()!;
            Config.Initialize(testDatabaseFactoryOptions, reqnrollConfig ?? new ReqnrollUtilitiesConfiguration());
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
        public void BeforeScenarioRegisterContextVariables(ScenarioContext context, IReqnrollOutputHelper outputHelper)
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
                ReqnrollDatabaseStateManager stateManager = new ReqnrollDatabaseStateManager(context);
                ITestDbConnection testDbConnection =
                    stateManager .OpenConnectionForName(mockedObject.ConnectionName);
                using IDbCommand command = testDbConnection.CreateCommand();

                foreach (string restoreScript in mockedObject.RestoreScripts)
                {
                    command.CommandText = restoreScript;
                    command.ExecuteNonQuery();
                }


                if (mockedObject.DatabaseObjectType == DatabseObjectType.Table)
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
                outputHelper.WriteLine($"\tConnection: {mockedObject.ConnectionName}': ");
                outputHelper.WriteLine($"\tRestore scripts: {mockedObject.RestoreScripts?.Count}': ");
                outputHelper.WriteLine($"\tError: {ex.Message}'");
                if (ex.InnerException != null)
                {
                    outputHelper.WriteLine($"\t\t'Inner error: {ex.InnerException.Message}'");
                }
                
                outputHelper.WriteLine($"\tScripts: ");
                foreach (string restoreScript in mockedObject.RestoreScripts)
                {
                    outputHelper.WriteLine($"\t\t {restoreScript} ");
                }
                
            }
        }

    }
}
