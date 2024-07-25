using Bambit.TestUtility.DatabaseTools.SpecFlow.Configuration;
using TechTalk.SpecFlow;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    [Binding]
    public class ConfigurationSteps(ScenarioContext context) : BaseSteps(context)
    {
        
        protected string NullCode
        {
            set => Configuration.NullStringIdentifier = value;
        }

        protected int Timeout
        {
            set => Configuration.TimeoutSeconds = value;
        }

        [Given(@"I'm treating empty strings as null")]
        public void GivenImTreatingEmptyStringsAsNull()
        {
            NullCode = string.Empty;
        }
        

        [Given(@"I have a query timeout of (?<seconds>.*) seconds")]
        public void GivenIHaveAQueryTimeoutOfSeconds(int seconds)
        {
            Timeout = seconds;
        }


    }
}
