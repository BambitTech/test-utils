using Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;
using Bambit.TestUtility.DataGeneration;
using FluentAssertions;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Tests.Mapping;

[TestClass]
public class ColumnDescriptionTest
{
    [TestMethod]
    public void ColumnName_SetFromCtor()
    {
        string name=RandomDataGenerator.Instance.GenerateString(10);
        string type=RandomDataGenerator.Instance.GenerateString(10);
        string cleanedName=RandomDataGenerator.Instance.GenerateString(10);
        int index = RandomDataGenerator.Instance.GenerateInt();
        ColumnDescription columnDescription = new(name, cleanedName, type, index);
        columnDescription.ColumnName.Should().Be(name);
    }
    [TestMethod]
    public void ColumnIndex_SetFromCtor()
    {
        string name=RandomDataGenerator.Instance.GenerateString(10);
        string type=RandomDataGenerator.Instance.GenerateString(10);
        string cleanedName=RandomDataGenerator.Instance.GenerateString(10);
        int index = RandomDataGenerator.Instance.GenerateInt();
        ColumnDescription columnDescription = new(name, cleanedName, type, index);
        columnDescription.ColumnIndex.Should().Be(index);
    }
    [TestMethod]
    public void ColumnType_SetFromCtor()
    {
        string name=RandomDataGenerator.Instance.GenerateString(10);
        string type=RandomDataGenerator.Instance.GenerateString(10);
        string cleanedName=RandomDataGenerator.Instance.GenerateString(10);
        int index = RandomDataGenerator.Instance.GenerateInt();
        ColumnDescription columnDescription = new(name, cleanedName, type, index);
        columnDescription.ColumnType.Should().Be(type);
    }

    [TestMethod]
    public void CleanedName_SetFromCtor()
    {
        string name=RandomDataGenerator.Instance.GenerateString(10);
        string type=RandomDataGenerator.Instance.GenerateString(10);
        string cleanedName=RandomDataGenerator.Instance.GenerateString(10);
        int index = RandomDataGenerator.Instance.GenerateInt();
        ColumnDescription columnDescription = new(name, cleanedName, type, index);
        columnDescription.CleanedName.Should().Be(cleanedName);
    }

    [TestMethod]
    public void ToString_ReturnsColumnName()
    {
        string name=RandomDataGenerator.Instance.GenerateString(10);
        string type=RandomDataGenerator.Instance.GenerateString(10);
        string cleanedName=RandomDataGenerator.Instance.GenerateString(10);
        int index = RandomDataGenerator.Instance.GenerateInt();
        ColumnDescription columnDescription = new(name, cleanedName, type, index);
        columnDescription.ToString().Should().Be(name);
    }

}