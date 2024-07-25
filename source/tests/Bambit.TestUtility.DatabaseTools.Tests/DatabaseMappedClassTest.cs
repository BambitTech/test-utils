using Bambit.TestUtility.DataGeneration;
using Bambit.TestUtility.TestHelper;
using FluentAssertions;

namespace Bambit.TestUtility.DatabaseTools.Tests;

[TestClass]
public class DatabaseMappedClassTest
{
    [TestMethod]
    [TestCategory("Unit")]
    public void AssignValuesIfDefined_AllValuesSupplied_AssignsAllValues()
    {
        MappedClassInstance mappedClass=new();
        MappedClassInstance expected= RandomDataGenerator.Instance.InitializeObject<MappedClassInstance >();
        Dictionary<string, object?> fields = new Dictionary<string, object?>
        {
            { "Name", expected.Name! },
            { "Age",expected.Age },
            { "Height", expected.Height },
            { "DateOfBirth",  expected.DateOfBirth }
        };
        mappedClass.AssignValuesIfDefined(fields);
        mappedClass.Should().BeEquivalentTo(expected);

    }

    [TestMethod] public void AssignValuesIfDefined_ComputedColumnExists_DoesNotSetComputedColumn()
    {
        MappedClassWithComputedColumn mappedClass=new();
        MappedClassWithComputedColumn expected= RandomDataGenerator.Instance.InitializeObject<MappedClassWithComputedColumn >();
        Dictionary<string, object?> fields = new Dictionary<string, object?>
        {
            { "Name", expected.Name! },
            { "Age",expected.Age },
            { "Height", expected.Height },
            { "DateOfBirth",  expected.DateOfBirth },
            {"WeekDay", RandomDataGenerator.Instance.GenerateString(10)}
        };
        mappedClass.AssignValuesIfDefined(fields);
        mappedClass.Should().BeEquivalentTo(expected, (a) => a.Excluding(i => i.WeekDay));
        mappedClass.WeekDay.Should().BeNull();

    }

    
    [TestMethod]
    public void AssignValuesIfDefined_ExtraValuesSupplied_AssignsAllValues_NoError()
    {
        MappedClassInstance mappedClass=new();
        MappedClassInstance expected= RandomDataGenerator.Instance.InitializeObject<MappedClassInstance >();
        Dictionary<string, object?> fields = new Dictionary<string, object?>
        {
            { "Name", expected.Name! },
            { "Age",expected.Age },
            { "Height", expected.Height },
            { "DateOfBirth",  expected.DateOfBirth },
            {"ThisDoesNotExist", RandomDataGenerator.Instance.GenerateString(10)},
            {"NeitherDoesThis", RandomDataGenerator.Instance.GenerateDateTime()}
        };
        mappedClass.AssignValuesIfDefined(fields);
        mappedClass.Should().BeEquivalentTo(expected);

    }

    
    
    [TestMethod]
    public void AssignValuesIfDefined_SingleValueSupplied_AssignsValue_DoesNotAssignOthers()
    {
        MappedClassInstance mappedClass=new();
        int expectedAge = RandomDataGenerator.Instance.GenerateInt();
        Dictionary<string, object?> fields = new Dictionary<string, object?>
        {
            { "Age",expectedAge},
        };
        mappedClass.AssignValuesIfDefined(fields);
        mappedClass.Age.Should().Be(expectedAge);
        mappedClass.Name.Should().BeNull();
        mappedClass.DateOfBirth.Should().Be(default);
        mappedClass.Height.Should().Be(default);
        

    }

    
    [TestMethod]
    public void AssignValuesIfDefined_InvalidValueSuppliedForField_ThrowsInvalidFieldException()
    {
        MappedClassInstance mappedClass=new();
        Dictionary<string, object?> fields = new Dictionary<string, object?>
        {
            { "Age",RandomDataGenerator.Instance.GenerateString(10)},
        };

        mappedClass.Invoking(m => m.AssignValuesIfDefined(fields))
            .Should().Throw<InvalidDataException>();
        

    }
}
