using Bambit.TestUtility.DatabaseTools.Attributes;
using Bambit.TestUtility.DataGeneration;
using FluentAssertions;

namespace Bambit.TestUtility.DatabaseTools.Tests.Attributes;

[TestClass]
[TestCategory("Unit")]
public class FieldSourceAttributeTest
{
    [TestMethod]
    public void Ctor_SetsNameAsExpected()
    {
        string testName = RandomDataGenerator.Instance.GenerateString(10);
        Type testType = typeof(string);
        string sourceType = RandomDataGenerator.Instance.GenerateString(10);
        FieldSourceAttribute attribute = new(testName,testType, sourceType);
        attribute.Name.Should().Be(testName);
    }
    
    [TestMethod]
    public void Ctor_SetsSourceTypeAsExpected()
    {
        string testName = RandomDataGenerator.Instance.GenerateString(10);
        string sourceType = RandomDataGenerator.Instance.GenerateString(10);
        Type testType = typeof(string);
        FieldSourceAttribute attribute = new(testName,testType, sourceType);
        attribute.SourceType.Should().Be(sourceType);
    }
    
    
    [TestMethod]
    public void Ctor_SetsMappedTypeAsExpected()
    {
        string testName = RandomDataGenerator.Instance.GenerateString(10);
        string sourceType = RandomDataGenerator.Instance.GenerateString(10);
        Type testType = typeof(string);
        FieldSourceAttribute attribute = new(testName,testType, sourceType);
        attribute.MappedType.Should().Be(testType);
        Type testType2 = typeof(DateTime);
         attribute = new(testName,testType2, sourceType);
        attribute.MappedType.Should().Be(testType2);
    }

}