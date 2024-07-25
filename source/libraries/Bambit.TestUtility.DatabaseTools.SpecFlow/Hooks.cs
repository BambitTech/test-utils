using Bambit.TestUtility.DatabaseTools.SpecFlow.Configuration;
using Microsoft.Extensions.Configuration;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;
using static Bambit.TestUtility.DatabaseTools.TestDatabaseFactory;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    [Binding]
    public class Hooks
    {
        private const string HandlerKey = nameof(HandlerKey);
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

        [BeforeScenario]
        public void BeforeScenarioRegisterContextVariables(ScenarioContext context, ISpecFlowOutputHelper outputHelper)
        {
            Config.RegisterContextVariables(context);
            EventHandler<TestDbConnectionMessageReceivedEvent> handler = (source, packet) =>
                outputHelper.WriteLine(packet.Message);

            Config.Configuration.TestDatabaseFactory.MessageReceived += handler;
            context.Set( handler, HandlerKey);
        }

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
