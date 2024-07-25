using Bambit.TestUtility.DataGeneration;
using FluentAssertions;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Tests;

[TestClass]
public class AutoAssignerTest
{  
    #region ParseDateExtended

    private static IEnumerable<object[]> ParseDateData()
    {
        DateTime randomDate = RandomDataGenerator.Instance.GenerateDate();
        DateTime firstOfMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);
        DateTime lastDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);
        DateTime lastDayOfLastMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
        int randomInt = RandomDataGenerator.Instance.GenerateInt(1, 99);

        return new List<object[]>
        {
            new object[] {randomDate.ToString("yyyy-MM-dd"),()=> randomDate},
            new object[] {"today", ()=>DateTime.Today},
            new object[] {"yesterday", ()=>DateTime.Today.AddDays(-1)},
            new object[] {"tomorrow", ()=>DateTime.Today.AddDays(1)},
            new object[] {"today",()=> DateTime.Today},
            new object[] {"now", ()=>DateTime.Now},
            new object[] {"firstdayofthismonth", ()=>firstOfMonth},
            new object[] {"first day of this month", ()=>firstOfMonth},
            new object[] {"firstdayofcurrentmonth", ()=>firstOfMonth},
            new object[] {"first day of current month",()=> firstOfMonth},
            new object[] {"firstdayofmonth", ()=>firstOfMonth},
            new object[] {"first day of month", ()=>firstOfMonth},
            new object[] {"lastdayofthismonth", ()=>lastDayOfMonth},
            new object[] {"last day of this month", ()=>lastDayOfMonth},
            new object[] {"lastdayofcurrentmonth", ()=>lastDayOfMonth},
            new object[] {"last day of current month", ()=>lastDayOfMonth},
            new object[] {"lastdayofmonth", ()=>lastDayOfMonth},
            new object[] {"last day of month", ()=>lastDayOfMonth},
            new object[] {"lastdayoflastmonth", ()=>lastDayOfLastMonth},
            new object[] {"endoflastmonth", ()=>lastDayOfLastMonth},
            new object[] {"end of last month", ()=>lastDayOfLastMonth},
            new object[] {"last day of last month",()=> lastDayOfLastMonth},
            new object[] {"lastdayofpreviousmonth", ()=>lastDayOfLastMonth},
            new object[] {"last day of previous month", ()=>lastDayOfLastMonth},
            new object[] {$"today + {randomInt}", ()=>DateTime.Today.AddDays(randomInt)},
            new object[] {$"today + {randomInt} Days",()=> DateTime.Today.AddDays(randomInt)},
            new object[] {$"today - {randomInt}", ()=>DateTime.Today.AddDays(-randomInt)},
            new object[] {$"today - {randomInt} Days",()=> DateTime.Today.AddDays(-randomInt)},
            new object[] {$"today + {randomInt} Months", ()=>DateTime.Today.AddMonths(randomInt)},
            new object[] {$"today - {randomInt} Months", ()=>DateTime.Today.AddMonths(-randomInt)},
            new object[] {$"today + {randomInt} Minutes", ()=>DateTime.Today.AddMinutes(randomInt)},
            new object[] {$"today - {randomInt} Minutes", ()=>DateTime.Today.AddMinutes(-randomInt)},
            new object[] {$"today + {randomInt} Seconds", ()=>DateTime.Today.AddSeconds(randomInt)},
            new object[] {$"today - {randomInt} Seconds", ()=>DateTime.Today.AddSeconds(-randomInt)},
            new object[] {$"today + {randomInt} Hours", ()=>DateTime.Today.AddHours(randomInt)},
            new object[] {$"today - {randomInt} Hours", ()=>DateTime.Today.AddHours(-randomInt)},
            new object[] {$"today + {randomInt} years", ()=>DateTime.Today.AddYears(randomInt)},
            new object[] {$"today - {randomInt} Years", ()=>DateTime.Today.AddYears(-randomInt)},

        };
    }

    [TestMethod]
    [DynamicData(nameof(ParseDateData), DynamicDataSourceType.Method)]
    public void ParseDateExtended(string value, Func<DateTime> expectedFunc)
    {
        DateTime dateExtended = AutoAssigner.ParseDateExtended(value);
        dateExtended.Should().BeCloseTo(expectedFunc(), TimeSpan.FromMicroseconds(500));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ParseDateExtended_InvalidDateDefinition_ThrowsException()
    {
        AutoAssigner.ParseDateExtended("nottoday");
    }


    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ParseDateExtended_InvalidDateAddDefinition_ThrowsException()
    {
        AutoAssigner.ParseDateExtended("today + 99999999999999999999today");
    }
    #endregion ParseDateExtended

}