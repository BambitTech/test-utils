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
        string sourceType = RandomDataGenerator.Instance.GenerateString(10);
        FieldSourceAttribute attribute = new(testName,sourceType);
        attribute.Name.Should().Be(testName);
    }
    
    [TestMethod]
    public void Ctor_SetsSourceTypeAsExpected()
    {
        string testName = RandomDataGenerator.Instance.GenerateString(10);
        string sourceType = RandomDataGenerator.Instance.GenerateString(10);
        FieldSourceAttribute attribute = new(testName,sourceType);
        attribute.SourceType.Should().Be(sourceType);
    }
    

}