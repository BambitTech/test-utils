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

    }
}
