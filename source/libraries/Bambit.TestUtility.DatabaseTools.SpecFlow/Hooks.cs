using Bambit.TestUtility.DatabaseTools.SpecFlow.Configuration;
using Microsoft.Extensions.Configuration;
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
    public class Hooks
    {
        private const string HandlerKey = nameof(HandlerKey);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Before test run. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("bambit.utilities.json")
                .AddJsonFile("bambit.utilities.local.json", true)
                .Build();
            SpecFlowUtilitiesConfiguration? specFlowConfig =
                configuration.GetSection("specFlow").Get<SpecFlowUtilitiesConfiguration>();

            TestDatabaseFactoryOptions testDatabaseFactoryOptions = configuration.GetSection("databaseFactory").Get<TestDatabaseFactoryOptions>()!;
            Config.Initialize(testDatabaseFactoryOptions,specFlowConfig??new SpecFlowUtilitiesConfiguration());
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
            EventHandler<TestDbConnectionMessageReceivedEvent> handler = (source, packet) =>
                outputHelper.WriteLine(packet.Message);

            Config.Configuration.TestDatabaseFactory.MessageReceived += handler;
            context.Set( handler, HandlerKey);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Handler, called when the after scenario unregister message. </summary>
        ///
        /// <remarks>   Law Metzler, 7/25/2024. </remarks>
        ///
        /// <param name="context">      The context. </param>
        /// <param name="outputHelper"> The output helper. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [AfterScenario]
        public void AfterScenarioUnregisterMessageHandler(ScenarioContext context, ISpecFlowOutputHelper outputHelper)
        {
            EventHandler<TestDbConnectionMessageReceivedEvent> handler =
                context.Get<EventHandler<TestDbConnectionMessageReceivedEvent>>(HandlerKey);
            if (handler != null)
            {
                Config.Configuration.TestDatabaseFactory.MessageReceived -= handler;
            }

            
        }

    }
}
