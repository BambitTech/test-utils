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

        return
        [
            [randomDate.ToString("yyyy-MM-dd"),()=> randomDate],
            ["today", ()=>DateTime.Today],
            ["yesterday", ()=>DateTime.Today.AddDays(-1)],
            ["tomorrow", ()=>DateTime.Today.AddDays(1)],
            ["today",()=> DateTime.Today],
            ["now", ()=>DateTime.Now],
            ["firstdayofthismonth", ()=>firstOfMonth],
            ["first day of this month", ()=>firstOfMonth],
            ["firstdayofcurrentmonth", ()=>firstOfMonth],
            ["first day of current month",()=> firstOfMonth],
            ["firstdayofmonth", ()=>firstOfMonth],
            ["first day of month", ()=>firstOfMonth],
            ["lastdayofthismonth", ()=>lastDayOfMonth],
            ["last day of this month", ()=>lastDayOfMonth],
            ["lastdayofcurrentmonth", ()=>lastDayOfMonth],
            ["last day of current month", ()=>lastDayOfMonth],
            ["lastdayofmonth", ()=>lastDayOfMonth],
            ["last day of month", ()=>lastDayOfMonth],
            ["lastdayoflastmonth", ()=>lastDayOfLastMonth],
            ["endoflastmonth", ()=>lastDayOfLastMonth],
            ["end of last month", ()=>lastDayOfLastMonth],
            ["last day of last month",()=> lastDayOfLastMonth],
            ["lastdayofpreviousmonth", ()=>lastDayOfLastMonth],
            ["last day of previous month", ()=>lastDayOfLastMonth],
            [$"today + {randomInt}", ()=>DateTime.Today.AddDays(randomInt)],
            [$"today + {randomInt} Days",()=> DateTime.Today.AddDays(randomInt)],
            [$"today - {randomInt}", ()=>DateTime.Today.AddDays(-randomInt)],
            [$"today - {randomInt} Days",()=> DateTime.Today.AddDays(-randomInt)],
            [$"today + {randomInt} Months", ()=>DateTime.Today.AddMonths(randomInt)],
            [$"today - {randomInt} Months", ()=>DateTime.Today.AddMonths(-randomInt)],
            [$"today + {randomInt} Minutes", ()=>DateTime.Today.AddMinutes(randomInt)],
            [$"today - {randomInt} Minutes", ()=>DateTime.Today.AddMinutes(-randomInt)],
            [$"today + {randomInt} Seconds", ()=>DateTime.Today.AddSeconds(randomInt)],
            [$"today - {randomInt} Seconds", ()=>DateTime.Today.AddSeconds(-randomInt)],
            [$"today + {randomInt} Hours", ()=>DateTime.Today.AddHours(randomInt)],
            [$"today - {randomInt} Hours", ()=>DateTime.Today.AddHours(-randomInt)],
            [$"today + {randomInt} years", ()=>DateTime.Today.AddYears(randomInt)],
            [$"today - {randomInt} Years", ()=>DateTime.Today.AddYears(-randomInt)],

        ];
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