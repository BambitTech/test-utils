
using System.Diagnostics.CodeAnalysis;
using Bambit.TestUtility.TestHelper;
using FluentAssertions;

namespace Bambit.TestUtility.DataGeneration.Tests;

[TestClass]
[ExcludeFromCodeCoverage]
[TestCategory("Unit")]
public class RandomDataGeneratorTest
{
    private const string PresetString="A constant string";
    #region Helper Methods

    public void TestNoDupes<T>(Func<T> generator, int numberToGenerate)
    {
        List<T> values = new List<T>();
        for (int x = 0; x < numberToGenerate; x++)
        {
            values.Add(generator());
        }

        ;
        while (values.Count > 1)
        {
            T value = values[0];
            values.RemoveAt(0);
            values.IndexOf(value).Should().Be(-1, $"value '{value}' found multiple times");
        }
    }

    protected class TestGenerator : RandomDataGenerator
    {
    }

    #endregion Helper Methods

    #region GenerateString

    [DataTestMethod]
    [DataRow(-1, DisplayName = "Negative 1")]
    [DataRow(0, DisplayName = "Zero")]
    [DataRow(-1000, DisplayName = "negative 1000")]
    [DataRow(int.MinValue, DisplayName = "int min")]
    public void GenerateString_InvalidLength_ThrowsException(int testNumber)
    {
        RandomDataGenerator.Instance.Invoking(r => r.GenerateString(testNumber)).Should()
            .Throw<ArgumentException>();
    }


    [DataTestMethod]
    [DataRow(10, DisplayName = "10")]
    [DataRow(1, DisplayName = "1")]
    [DataRow(1000, DisplayName = "1000")]
    [DataRow(10000, DisplayName = "10000")]
    public void GenerateString_ValidLength_GeneratesCorrectLength(int testNumber)
    {
        RandomDataGenerator.Instance.GenerateString(testNumber).Length.Should()
            .Be(testNumber);
    }

    [TestMethod]
    public void GenerateString_MultipleRuns_DoNotMatch()
    {
        TestNoDupes(() => RandomDataGenerator.Instance.GenerateString(100), 5);

    }

        

    #endregion GenerateString

    #region GetListEntry

    [TestMethod]

    public void GetListEntry_MultipleRuns_DoNotMatch()
    {
        List<int> list = [..new[] { 1, 2, 3, 5, 6, 7, 8, 9, 10 }];
        Dictionary<int, int> generated = new Dictionary<int, int>();
        for (int x = 0; x < list.Count * 100; x++)
        {
            int gv = RandomDataGenerator.Instance.GetListEntry(list);
            if (!generated.ContainsKey(gv))
                generated.Add(gv, 0);
            generated[gv]++;
        }

        foreach ((int key, int value) in generated)
        {
            value.Should().BeGreaterThan(5, because: $"key {key} only appeared {value} time(s)");
        }
    }

    [TestMethod]
    public void GetListEntry_OnlyReturnsItemsFromList()
    {
        List<string> list = [..new[] { "alpha", "bravo", "Charlie", "Delta", "Echo"}];
        for (int x = 0; x < 100; x++)
        {
            string gv = RandomDataGenerator.Instance.GetListEntry(list);
            list.Should().Contain(gv, $"Value '{gv}' should be in list");
        }

    }
    #endregion GetListEntry
        
    #region GetListEntry
    [TestMethod]

    public void CoinToss_MultipleRuns_DoNotMatch()
    {
        Dictionary<int, int> generated = new Dictionary<int, int>
        {
            { 5, 0 },
            { 10, 0 }
        };
        for (int x = 0; x < 100; x++)
        {
            int gv=RandomDataGenerator.Instance.CoinToss(5, 10);
            if(!generated.ContainsKey(gv))
                generated.Add(gv,0);
            generated[gv]++;
        }
            
        foreach ((int key, int value) in generated)
        {
            value.Should().BeGreaterThan(5, because:$"key {key} only appeared {value} time(s)");
        }
    }

    [TestMethod]
    public void CoinToss_OnlyReturnsItemsFromList()
    {
        string gv=RandomDataGenerator.Instance.CoinToss("First", "Second");
        (gv is "First" or "Second").Should().BeTrue();

    }
    #endregion GetListEntry
    #region GenerateDouble

    [TestMethod]
    public void GenerateDouble_MultipleRuns_DoNotMatch()
    {
        TestNoDupes(() => RandomDataGenerator.Instance.GenerateDouble(0, 100000), 5);
    }



    [TestMethod]
    [DataRow((byte)1, (byte)1, DisplayName = "1,1")]
    [DataRow((byte)1, (byte)2, DisplayName = "1,2")]
    [DataRow((byte)1, (byte)3, DisplayName = "1,3")]
    [DataRow((byte)1, (byte)5, DisplayName = "1,5")]
    [DataRow((byte)1, (byte)7, DisplayName = "1,7")]
    [DataRow((byte)2, (byte)1, DisplayName = "2,1")]
    [DataRow((byte)2, (byte)2, DisplayName = "2,2")]
    [DataRow((byte)2, (byte)3, DisplayName = "2,3")]
    [DataRow((byte)2, (byte)5, DisplayName = "2,5")]
    [DataRow((byte)3, (byte)7, DisplayName = "2,7")]
    [DataRow((byte)3, (byte)1, DisplayName = "3,1")]
    [DataRow((byte)3, (byte)2, DisplayName = "3,2")]
    [DataRow((byte)3, (byte)3, DisplayName = "3,3")]
    [DataRow((byte)3, (byte)5, DisplayName = "3,5")]
    [DataRow((byte)3, (byte)7, DisplayName = "3,7")]
    [DataRow((byte)4, (byte)1, DisplayName = "4,1")]
    [DataRow((byte)4, (byte)2, DisplayName = "4,2")]
    [DataRow((byte)4, (byte)3, DisplayName = "4,3")]
    [DataRow((byte)4, (byte)5, DisplayName = "4,5")]
    [DataRow((byte)4, (byte)7, DisplayName = "4,7")]
    [DataRow((byte)5, (byte)1, DisplayName = "5,1")]
    [DataRow((byte)5, (byte)2, DisplayName = "5,2")]
    [DataRow((byte)5, (byte)3, DisplayName = "5,3")]
    [DataRow((byte)5, (byte)5, DisplayName = "5,5")]
    [DataRow((byte)5, (byte)7, DisplayName = "5,7")]
    public void GenerateDouble_WithPrecisionAndScale_ShouldBeInRange(byte precision, byte scale)
    {
        for (int x = 0; x < 10; x++)
        {
            RandomDataGenerator.Instance.GenerateDouble(precision, scale).Should().BeInRange(0,Math.Pow(10, precision )/Math.Pow(10,scale));
        }
    }
    [TestMethod]
    public void GenerateDouble_PrecisionMoreThan10TimesScale_ShouldBeInRange()
    {
        byte precision = 12;
        byte scale = 1;
        for (int x = 0; x < 100; x++)
            RandomDataGenerator.Instance.GenerateDouble(precision, scale).Should().BeInRange(0,Math.Pow(10, 10)/Math.Pow(10,scale));
            
    }

    [TestMethod]
    [DataRow((byte)7, (byte)1, DisplayName = "7,1")]
    [DataRow((byte)7, (byte)3, DisplayName = "7,2")]
    [DataRow((byte)7, (byte)3, DisplayName = "7,3")]
    [DataRow((byte)7, (byte)4, DisplayName = "7,4")]
    [DataRow((byte)7, (byte)5, DisplayName = "7,5")]
    public void GenerateDouble_WithPrecisionAndScale_NoDupes(byte precision, byte scale)
    {
         
        TestNoDupes(()=> RandomDataGenerator.Instance.GenerateDouble(precision, scale), 10);

    }

    #endregion GenerateDouble

    #region  GetFirstDayOfMonth


    [TestMethod]
    public void GetFirstDayOfMonth_ReturnsFirstDayOfEachMonth()
    {
        int year = DateTime.Today.Year;
        for (int x = -5; x < 6; x++)
        {
            for (int month = 1; month <= 12; month++)
            {
                int day = RandomDataGenerator.Instance.GenerateInt(1, 27);
                DateTime testDate = new(year - x, month, day);
                DateTime result = RandomDataGenerator.Instance.GetFirstDayOfMonth(testDate);
                result.Year.Should().Be(testDate.Year);
                result.Month.Should().Be(testDate.Month);
                result.Day.Should().Be(1);
            }
        }
    }

    

    [TestMethod]
    public void GetFirstDayOfMonth_NoParameters_ReturnsFirstDayOfCurrentMonth()
    {
                DateTime result = RandomDataGenerator.Instance.GetFirstDayOfMonth();
                result.Year.Should().Be(DateTime.Today.Year);
                result.Month.Should().Be(DateTime.Today.Month);
                result.Day.Should().Be(1);
    }


    #endregion GetFirstDayOfMonth
    #region  GetLastDayOfMonth


    [TestMethod]
    public void GetLastDayOfMonth_ReturnsLastDayOfEachMonth()
    {
        int year = DateTime.Today.Year;
        for (int x = -5; x < 6; x++)
        {
            for (int month = 1; month <= 12; month++)
            {
                int day = RandomDataGenerator.Instance.GenerateInt(1, 27);
                DateTime testDate = new(year - x, month, day);
                DateTime result = RandomDataGenerator.Instance.GetLastDayOfMonth(testDate).AddDays(1);
                result.Day.Should().Be(1);
                if (month == 12)
                {
                    result.Year.Should().Be(testDate.Year+1);
                    result.Month.Should().Be(1);
                }
                else
                {
                    result.Year.Should().Be(testDate.Year);
                    result.Month.Should().Be(testDate.Month+1);
                }
            }
        }
    }

    [TestMethod]
    public void GetLastDayOfMonth_NoParameter_ReturnsLasstDayOfCurrentMonth()
    {

        DateTime result = RandomDataGenerator.Instance.GetLastDayOfMonth().AddDays(1);
        result.Day.Should().Be(1);
        if (DateTime.Today.Month == 12)
        {
            result.Year.Should().Be(DateTime.Today.Year + 1);
            result.Month.Should().Be(1);
        }
        else
        {
            result.Year.Should().Be(DateTime.Today.Year);
            result.Month.Should().Be(DateTime.Today.Month + 1);
        }
    }

    #endregion GetFirstDayOfMonth

    #region  GenerateDateTime


    [TestMethod]
    public void GenerateDateTime_GeneratedNoDupes()
    {
        TestNoDupes(()=> RandomDataGenerator.Instance.GenerateDateTime(-5, 5),100);
    }

    #endregion GenerateDateTime
    #region GenerateEnum

    [TestMethod]
    public void GenerateEnum_GeneratesRandomNumbers()
    {
        Dictionary<TestEnum,int> counts= new Dictionary<TestEnum,int>();

        Array array = Enum.GetValuesAsUnderlyingType<TestEnum>();
        foreach (TestEnum o in array)
        {
            counts.Add(o, 0);
        }

        for (int x = 0; x < array.Length * 100; x++)
        {
            TestEnum test = RandomDataGenerator.Instance.GenerateEnum<TestEnum>();
            counts[test] += 1;
        }
        foreach ((TestEnum key, int value) in counts)
        {
            value.Should().BeGreaterThan(5, because:$"Enum {key} only appeared {value} time(s)");
        }
    }
    #endregion GenerateEnum

    #region GenerateFirstName

    [TestMethod]
    public void GenerateFirstName_GeneratesRandomNames()
    {
        Dictionary<string,int> counts= new Dictionary<string,int>();

            
        foreach (string name in RandomDataGenerator.FirstNames)
        {
            counts.Add(name, 0);
        }

        for (int x = 0; x <  RandomDataGenerator.FirstNames.Length * 100; x++)
        {
            string test = RandomDataGenerator.Instance.GenerateFirstName();
            counts[test] += 1;
        }
        foreach ((string key, int value) in counts)
        {
            value.Should().BeGreaterThan(5, because:$"name {key} only appeared {value} time(s)");
        }
    }
    #endregion GenerateFirstName

    #region GenerateLastName

    [TestMethod]
    public void GenerateLastName_GeneratesRandomNames()
    {
        Dictionary<string,int> counts= new Dictionary<string,int>();

            
        foreach (string name in RandomDataGenerator.LastNames)
        {
            counts.Add(name, 0);
        }

        for (int x = 0; x <  RandomDataGenerator.LastNames.Length * 100; x++)
        {
            string test = RandomDataGenerator.Instance.GenerateLastName();
            counts[test] += 1;
        }
        foreach ((string key, int value) in counts)
        {
            value.Should().BeGreaterThan(5, because:$"name {key} only appeared {value} time(s)");
        }
    }
    #endregion GenerateLastName
    #region GenerateName

    [TestMethod]
    public void GenerateName_GeneratesRandomNames()
    {
        Dictionary<string,int> counts= new Dictionary<string,int>();

        for (int x = 0; x < 100; x++)
        {
            string test = RandomDataGenerator.Instance.GenerateName();
            if(counts.ContainsKey(test))
                counts[test] += 1;
            else 
                counts.Add(test, 1);
        }
        foreach ((string key, int value) in counts)
        {
            value.Should().BeLessThan(3, because:$"name {key} appeared {value} time(s)");
        }
    }
    #endregion GenerateLastName

    #region GenerateInt

    [TestMethod]
    public void GenerateInt_MultipleRuns_DoNotMatch()
    {
        TestNoDupes(() => RandomDataGenerator.Instance.GenerateInt(-100000, 100000), 5);
    }

    #endregion GenerateInt

    
    #region GenerateByte

    [TestMethod]
    public void GenerateByte_MultipleRuns_DoNotMatch()
    {
        TestNoDupes(() => RandomDataGenerator.Instance.GenerateByte(), 5);
    }

    #endregion GenerateByte
    #region GenerateDecimal

    [TestMethod]
    public void GenerateDecimal_MultipleRuns_DoNotMatch()
    {
        TestNoDupes(() => RandomDataGenerator.Instance.GenerateDecimal(0, 100000), 5);
    }



    [TestMethod]
    [DataRow((byte)1, (byte)1, DisplayName = "1,1")]
    [DataRow((byte)1, (byte)2, DisplayName = "1,2")]
    [DataRow((byte)1, (byte)3, DisplayName = "1,3")]
    [DataRow((byte)1, (byte)5, DisplayName = "1,5")]
    [DataRow((byte)1, (byte)7, DisplayName = "1,7")]
    [DataRow((byte)2, (byte)1, DisplayName = "2,1")]
    [DataRow((byte)2, (byte)2, DisplayName = "2,2")]
    [DataRow((byte)2, (byte)3, DisplayName = "2,3")]
    [DataRow((byte)2, (byte)5, DisplayName = "2,5")]
    [DataRow((byte)3, (byte)7, DisplayName = "2,7")]
    [DataRow((byte)3, (byte)1, DisplayName = "3,1")]
    [DataRow((byte)3, (byte)2, DisplayName = "3,2")]
    [DataRow((byte)3, (byte)3, DisplayName = "3,3")]
    [DataRow((byte)3, (byte)5, DisplayName = "3,5")]
    [DataRow((byte)3, (byte)7, DisplayName = "3,7")]
    [DataRow((byte)4, (byte)1, DisplayName = "4,1")]
    [DataRow((byte)4, (byte)2, DisplayName = "4,2")]
    [DataRow((byte)4, (byte)3, DisplayName = "4,3")]
    [DataRow((byte)4, (byte)5, DisplayName = "4,5")]
    [DataRow((byte)4, (byte)7, DisplayName = "4,7")]
    [DataRow((byte)5, (byte)1, DisplayName = "5,1")]
    [DataRow((byte)5, (byte)2, DisplayName = "5,2")]
    [DataRow((byte)5, (byte)3, DisplayName = "5,3")]
    [DataRow((byte)5, (byte)5, DisplayName = "5,5")]
    [DataRow((byte)5, (byte)7, DisplayName = "5,7")]
    public void GenerateDecimal_WithPrecisionAndScale_ShouldBeInRange(byte precision, byte scale)
    {
        for (int x = 0; x < 10; x++)
        {
            RandomDataGenerator.Instance.GenerateDecimal(precision, scale).Should().BeInRange(0,Convert.ToDecimal( Math.Pow(10, precision)));
        }
    }
        
    [TestMethod]
    public void GenerateDecimal_PrecisionMoreThan10TimesScale_ShouldBeInRange()
    {
        byte precision = 12;
        byte scale = 1;
        for (int x = 0; x < 100; x++)
            RandomDataGenerator.Instance.GenerateDecimal(precision, scale).Should()
                .BeInRange(0, Convert.ToDecimal(Math.Pow(10, 10) / Math.Pow(10, scale)));

    }
    [TestMethod]
    [DataRow((byte)7, (byte)1, DisplayName = "7,1")]
    [DataRow((byte)7, (byte)3, DisplayName = "7,2")]
    [DataRow((byte)7, (byte)3, DisplayName = "7,3")]
    [DataRow((byte)7, (byte)4, DisplayName = "7,4")]
    [DataRow((byte)7, (byte)5, DisplayName = "7,5")]
    public void GenerateDecimal_WithPrecisionAndScale_NoDupes(byte precision, byte scale)
    {

        TestNoDupes(() => RandomDataGenerator.Instance.GenerateDecimal(precision, scale), 10);

    }

    #endregion GenerateDecimal

    #region InitializeList

    [TestMethod]
    public void InitializeList_ReturnsExpectedNumber()
    {
        IList<TestClass> initializeList = RandomDataGenerator.Instance.InitializeList<TestClass>(4);
        initializeList.Should().HaveCount(4);
        foreach (TestClass testClass in initializeList)
        {
            
            testClass.Should().NotBeNull();
            testClass.FirstName.Should().NotBeNullOrWhiteSpace();
        }
            
    }

        
    [TestMethod]
    public void InitializeList_PostCreateAction_ReturnsUsersMethod()
    {
        DateTime date1 = new (2007, 1, 31);
        DateTimeOffset offset=new (2009, 11, 3, 10, 0, 0, TimeSpan.Zero);
        IList<TestClass> initializeList = RandomDataGenerator.Instance.InitializeList<TestClass>(4, f =>
        {
            f.DateTimeField = date1;
            f.DateTimeOffsetField = offset;
        });
        initializeList.Should().HaveCount(4);
        foreach (TestClass testClass in initializeList)
        {
            
            testClass.Should().NotBeNull();
            testClass.DateTimeField.Should().Be(date1);
            testClass.DateTimeOffsetField.Should().Be(offset);

        }
    }
        
    [TestMethod]
    public void InitializeList_GeneratorFunction_ReturnsUsersMethod()
    {
        DateTime date1 = new (2007, 1, 31);
        DateTimeOffset offset=new (2009, 11, 3, 10, 0, 0, TimeSpan.Zero);
        TestClass source = new()
        {
            DateTimeField = date1,
            DateTimeOffsetField = offset,
            DecimalField = 10.0m,
            IntField = 20
        };

        List<TestClass> initializeList =
            RandomDataGenerator.Instance.InitializeList<TestClass>(4, (_) => source);
        initializeList.Should().HaveCount(4);
        initializeList.ForEach(i=>
        {
            i.Should().NotBeNull().And.Be(source);

        });
    }

        
    [TestMethod]
    public void InitializeList_Strings_ReturnsExpectedNumber()
    {
        IList<string> initializeList = RandomDataGenerator.Instance.InitializeList(4,10);
        initializeList.Should().HaveCount(4);
        foreach (string testString in initializeList)
        {
            
            testString.Should().NotBeNullOrEmpty().And.HaveLength(10);
        }
    }

    #endregion

    #region  InitializeDictionary

        
    [TestMethod]
    public void InitializeDictionary_ReturnsExpectedNumber()
    {
        Dictionary<TestClass, TestChildClass> initializeList =
            RandomDataGenerator.Instance.InitializeDictionary<TestClass, TestChildClass>(4);
        initializeList.Should().HaveCount(4);
        foreach ((TestClass? key, TestChildClass? value) in initializeList)
        {
            key.Should().NotBeNull();
            key.FirstName.Should().NotBeNullOrWhiteSpace();
            value.Should().NotBeNull();
            value.SubName.Should().NotBeNullOrWhiteSpace();
        }
    }

    [TestMethod]
    public void InitializeDictionary_StringDictionary_ReturnsExpectedNumber()
    {
        Dictionary<string, TestChildClass> initializeList =
            RandomDataGenerator.Instance.InitializeDictionary<TestChildClass>(4);
        initializeList.Should().HaveCount(4);
        foreach ((string key, TestChildClass? value) in initializeList)
        {
            key.Should().NotBeNullOrWhiteSpace();
            value.Should().NotBeNull();
            value.SubName.Should().NotBeNullOrWhiteSpace();
        }
    }


    #endregion InitializeDictionary

    #region InitializeObject

    [TestMethod]
    public void InitializeObject_PreCreated_SetsFields()
    {
        TestClass testClass=new();
        RandomDataGenerator.Instance.InitializeObject(testClass);
        testClass.Should().NotBeNull();
        testClass.ChildClass.Should().BeNull();
        testClass.Children.Should().BeEmpty();
        testClass.DoubleField.Should().NotBe(0);
        testClass.IntField.Should().NotBe(0);
        testClass.FirstName.Should().NotBeNull().And.NotBeEmpty();
        testClass.DecimalField.Should().NotBe(0);
        testClass.ShortField.Should().NotBe(0);
        testClass.LongField.Should().NotBe(0);
        testClass.GuidField.Should().NotBe(Guid.Empty);
        testClass.DateTimeField.Should().NotBe(DateTime.MinValue);
        testClass.DateTimeOffsetField.Should().NotBe(DateTimeOffset.MinValue);

        testClass.NullableDoubleField.Should().NotBeNull().And.NotBe(0);
        testClass.NullableIntField.Should().NotBeNull().And.NotBe(0);
        testClass.NullableFirstName.Should().NotBeNull().And.NotBeNull().And.NotBeEmpty();
        testClass.NullableDecimalField.Should().NotBeNull().And.NotBe(0);
        testClass.NullableShortField.Should().NotBeNull().And.NotBe(0);
        testClass.NullableLongField.Should().NotBeNull().And.NotBe(0);
        testClass.NullableGuidField.Should().NotBeNull().And.NotBe(Guid.Empty);
        testClass.NullableDateTimeField.Should().NotBeNull().And.NotBe(DateTime.MinValue);
        testClass.NullableDateTimeOffsetField.Should().NotBeNull().And.NotBe(DateTimeOffset.MinValue);
    }

        
    [TestMethod]
    public void InitializeObject_PreCreatedTwice_UniqueItems()
    {
        TestClass testClass1=new();
        RandomDataGenerator.Instance.InitializeObject(testClass1);
        TestClass testClass2=new();
        RandomDataGenerator.Instance.InitializeObject(testClass2);
        testClass1.DoubleField.Should().NotBe(testClass2.DoubleField);
        testClass1.IntField.Should().NotBe(testClass2.IntField);
        testClass1.FirstName.Should().NotBe(testClass2.FirstName);
    }
        
    [TestMethod]
    public void InitializeObject_PreCreated_PassInitializerFunction_UsesPassedFunction()
    {
        TestChildClass preSetChild = RandomDataGenerator.Instance.InitializeObject<TestChildClass>();
            
        TestClass testClass=new();
        RandomDataGenerator.Instance.InitializeObject(testClass,
            newObject =>
            {
                newObject.FirstName = PresetString;
                newObject.ChildClass = preSetChild;
            });
        testClass.Should().NotBeNull();
        testClass.ChildClass.Should().NotBeNull().And.Be(preSetChild);
        testClass.Children.Should().BeEmpty();
        testClass.FirstName.Should().BeEquivalentTo(PresetString);
    }

    [TestMethod]
    public void InitializeObject_New_SetsFields()
    {
        TestClass testClass=RandomDataGenerator.Instance.InitializeObject<TestClass >();
        testClass.Should().NotBeNull();
        testClass.ChildClass.Should().BeNull();
        testClass.Children.Should().BeEmpty();
        testClass.DoubleField.Should().NotBe(0);
        testClass.IntField.Should().NotBe(0);
        testClass.FirstName.Should().NotBeNull().And.NotBeEmpty();
    }
        
    [TestMethod]
    public void InitializeObject_PassInitializerFunction_UsesPassedFunction()
    {
        TestChildClass preSetChild = RandomDataGenerator.Instance.InitializeObject<TestChildClass>();
        TestClass testClass=RandomDataGenerator.Instance.InitializeObject<TestClass >(
            newObject =>
            {
                newObject.FirstName = PresetString;
                newObject.ChildClass = preSetChild;
            });
        testClass.Should().NotBeNull();
        testClass.ChildClass.Should().NotBeNull().And.Be(preSetChild);
        testClass.Children.Should().BeEmpty();
        testClass.FirstName.Should().BeEquivalentTo(PresetString);
    }

        
    [TestMethod]
    public void InitializeObject_NewTwice_UniqueItems()
    {
        TestClass testClass1=RandomDataGenerator.Instance.InitializeObject<TestClass >();
        TestClass testClass2 = RandomDataGenerator.Instance.InitializeObject<TestClass>();
        testClass1.DoubleField.Should().NotBe(testClass2.DoubleField);
        testClass1.IntField.Should().NotBe(testClass2.IntField);
        testClass1.FirstName.Should().NotBe(testClass2.FirstName);
    }

        

    [TestMethod]
    public void InitializeObject_FieldLengthOnString_SetsFieldLength()
    {
        TestChildClass testClass=RandomDataGenerator.Instance.InitializeObject<TestChildClass >();
        testClass.Should().NotBeNull();
        testClass.ChildName.Should().NotBeNullOrWhiteSpace();
        testClass.ChildName.Length.Should().Be(4);
        testClass.SubName.Should().NotBeNullOrWhiteSpace();
        testClass.SubName.Length.Should().Be(6);
    }
        

    #endregion InitializeObject
        

    #region Create Object 
        
    [TestMethod]
    public void CreateObject_New_SetsFields()
    {
        TestClass? testClass=RandomDataGenerator.Instance.CreateObject(typeof(TestClass)) as TestClass;
        testClass.Should().NotBeNull();
        testClass!.ChildClass.Should().BeNull();
        testClass.Children.Should().BeEmpty();
        testClass.DoubleField.Should().NotBe(0);
        testClass.IntField.Should().NotBe(0);
        testClass.FirstName.Should().NotBeNull().And.NotBeEmpty();
    }
    
    [TestMethod]
    public void CreateObject_NoDefaultCtor_ThrowsException()
    {

        RandomDataGenerator.Instance.Invoking(d => d.CreateObject(typeof(NoDefaultCtorClass))).Should()
            .Throw<MissingMethodException>();
    }

    
    [TestMethod]
    public void CreateObject_NullableType_ThrowsException()
    {
        RandomDataGenerator.Instance.Invoking(d => d.CreateObject(typeof(int?))).Should()
            .Throw<InvalidOperationException>();
    }

        
    #endregion

    #region   AddCustomObjectInitialization
        
    [TestMethod]
    public void AddCustomObjectInitialization_CreateObject_UsesMappedInitializer()
    {
        TestGenerator generator = new();
        TestClass initialClass=RandomDataGenerator.Instance.InitializeObject<TestClass >();

        generator.AddCustomObjectInitialization((_) => initialClass);

        TestClass? testClass=generator.CreateObject(typeof(TestClass)) as TestClass;
        testClass.Should().NotBeNull().And.Be(initialClass);
    }
    [TestMethod]
    public void AddCustomObjectInitialization_NewInitializer_UsesMappedInitializer()
    {
        TestGenerator generator = new();
        TestClass initialClass=RandomDataGenerator.Instance.InitializeObject<TestClass >();

        generator.AddCustomObjectInitialization((_) => initialClass);
        TestClass testClass = generator.InitializeObject<TestClass>();
        testClass.Should().NotBeNull().And.Be(initialClass);
    }

    [TestMethod]
    public void AddCustomObjectInitialization_WIthAutoProperty_UsesMappedInitializer()
    {
        TestGenerator generator = new();
        TestChildClass initialClass=RandomDataGenerator.Instance.InitializeObject<TestChildClass>();

        generator.AddCustomObjectInitialization((_) => initialClass,true);
        TestClass testClass = generator.InitializeObject<TestClass>();
        testClass.ChildClass.Should().NotBeNull().And.Be(initialClass);
    }

    [TestMethod]
    public void AddCustomObjectInitialization_ReplaceInitializer_UsesMappedInitializer()
    {
        TestGenerator generator = new();
        TestClass initialClass=RandomDataGenerator.Instance.InitializeObject<TestClass >();

        generator.AddCustomObjectInitialization((_) => initialClass);
        TestClass testClass = generator.InitializeObject<TestClass>();
        testClass.Should().NotBeNull().And.Be(initialClass);
        generator.AddCustomObjectInitialization((_) => RandomDataGenerator.Instance.InitializeObject<TestClass>());
        TestClass testClass2 = generator.InitializeObject<TestClass>();
        testClass2.Should().NotBeNull().And.NotBeEquivalentTo(initialClass);

    }


    #endregion AddCustomObjectInitialization

    #region MaxRecursion

    [TestMethod]
    public void MaxRecursionSetLow_ThrowsRecursiveError()
    {
        TestGenerator generator = new()
        {
            MaxRecursion = 1
        };
        generator.AddAutoProperties<TestChildClass>();
        generator.AddAutoProperties<TestGrandChild>();
        generator.Invoking(g => g.InitializeObject<TestClass>())
            .Should().Throw<InvalidOperationException>();
    }


    [TestMethod]
    public void MaxRecursionSetHigh_DoesNotThrowsRecursiveError()
    {
        TestGenerator generator = new()
        {
            MaxRecursion = 4
        };
        generator.AddAutoProperties<TestChildClass>();
        generator.AddAutoProperties<TestGrandChild>();
        generator.Invoking(g => g.InitializeObject<TestClass>())
            .Should().NotThrow<Exception>();
    }

    #endregion MaxRecursion

    #region AddAutoProperties

    [TestMethod]
    public void AddAutoProperties_ConcreteType_SetsProperty()
    {
        TestGenerator generator = new();

        generator.AddAutoProperties<TestChildClass>();
        TestClass testClass = generator.InitializeObject<TestClass>();
        testClass.ChildClass.Should().NotBeNull();
        testClass.ChildClassInterface.Should().BeNull();
        testClass.ChildClass.ChildName.Should().NotBeNullOrWhiteSpace();
    }


    [TestMethod]
    public void AddAutoProperties_RecursiveClass_ThrowsAtDepth()
    {
        TestGenerator generator = new();

        generator.AddAutoProperties<RecursiveClass>();
        generator.Invoking(g => g.InitializeObject<RecursiveClass>())
            .Should().Throw<InvalidOperationException>();
    }


    [TestMethod]
    public void AddAutoProperties_Interface_SetsProperty()
    {
        TestGenerator generator = new();

        generator.AddAutoProperties<ITestChildClass, TestChildClass>();
        TestClass testClass = generator.InitializeObject<TestClass>();
        testClass.ChildClassInterface.Should().NotBeNull();
        testClass.ChildClass.Should().BeNull();
        testClass.ChildClassInterface.ChildName.Should().NotBeNullOrWhiteSpace();
    }

    #endregion AddAutoProperties

        

    #region AddStringFieldNameGenerator

    [TestMethod]
    public void AddStringFieldNameGenerator_ConcreteType_SetsProperty()
    {
        TestGenerator generator = new();
        const string generatedString = "John Jacob Jingleheimer Smith";
        generator.AddStringFieldNameGenerator("FirstName",()=> generatedString);
        TestClass testClass = generator.InitializeObject<TestClass>();
        testClass.FirstName.Should().Be(generatedString);
    }



    #endregion AddStringFieldNameGenerator
        
}