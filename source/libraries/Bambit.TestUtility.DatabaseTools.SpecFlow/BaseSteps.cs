using Bambit.TestUtility.DatabaseTools.SpecFlow.Configuration;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;
using Bambit.TestUtility.DatabaseTools.SpecFlow.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    /// <summary>
    /// Base class for all steps, provides state management and simplified access to variables on the <see cref="ScenarioContext"/>
    /// </summary>
    /// <param name="context"></param>
    public class BaseSteps(ScenarioContext context)
    {
        #region Properties
        
        /// <summary>
        /// An instance of the <see cref="SpecFlowDatabaseStateManager"/>
        /// </summary>
        protected readonly SpecFlowDatabaseStateManager StateManager = new(context);

        /// <summary>
        /// Shortcut reference to the current <see cref="SpecFlowUtilitiesConfiguration"/>
        /// </summary>
        protected SpecFlowUtilitiesConfiguration Configuration => StateManager.Configuration;

        /// <summary>
        /// Holds the current <see cref="ScenarioContext"/>
        /// </summary>
        protected readonly ScenarioContext Context = context;

        /// <summary>
        /// Stores the last results as a <see cref="MappedTable"/>
        /// </summary>
        protected MappedTable LastResultSet
        {
            get => StateManager.LastResultSet;
            set=>StateManager.LastResultSet=value;
        }
        /// <summary>
        /// Stores scenario defined replacement variables
        /// </summary>
        protected Dictionary<string, object> Variables => StateManager.Variables;


     

        #endregion Properties
        /// <summary>
        /// Returns a variable, if defined in <see cref="VerifyCompareResults"/>, with its value
        /// </summary>
        /// <param name="token">The variable to replace</param>
        /// <returns>A mapped string if found; otherwise the original variable</returns>
        protected string? ReplaceVariable(string? token)
        {
            if (!string.IsNullOrWhiteSpace(token) && StateManager.Variables.TryGetValue(token, out object? value))
            {
                return (value is DateTime dateTime
                    ? dateTime.ToString("yyyy-MM-dd HH:mm:ss.FFFFFF")
                    : value as string);
            }

            return token;
        }

        /// <summary>
        /// Provides assertions and readable results from a <see cref="DataComparisonResults"/>
        /// </summary>
        /// <param name="results">The results to compare</param>
        protected void VerifyCompareResults(DataComparisonResults results)
        {
            if (results.IsSuccess)
                return;
            if (results.NumberRowsMissing > 0)
            {
                if (results.NumberRowsNotExpected > 0)
                {
                    Assert.Fail(
                        $"{results.NumberRowsMissing} rows were not found, while {results.NumberRowsNotExpected} rows were not expected");

                }

                Assert.Fail($"{results.NumberRowsMissing} rows were not found");
            }

            Assert.Fail(
                $"{results.NumberRowsMissing} rows were not found, while {results.NumberRowsNotExpected} rows were not expected");
        }
    }
}
