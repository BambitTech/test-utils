using Bambit.TestUtility.DatabaseTools.Attributes;
using Bambit.TestUtility.DataGeneration;
using FluentAssertions;

namespace Bambit.TestUtility.DatabaseTools.Tests.Attributes
{
    [TestClass]
    [TestCategory("Unit")]
    public class TableSourceAttributeTest
    {
        [TestMethod]
        public void Ctor_NoQualifiers_SetsFieldsAsExpected()
        {
            string sourceConnection = RandomDataGenerator.Instance.GenerateString(10);
            string sourceSchema= RandomDataGenerator.Instance.GenerateString(10);
            string sourceTable= RandomDataGenerator.Instance.GenerateString(10);
            TableSourceAttribute attribute = new(sourceConnection, sourceSchema, sourceTable);
            attribute.ConnectionName.Should().Be(sourceConnection);
            attribute.SchemaName.Should().Be(sourceSchema);
            attribute.TableName.Should().Be(sourceTable);
        }

        [TestMethod]
        public void Ctor_WithQualifiers_RemovesQualifiers()
        {
            string sourceConnection = RandomDataGenerator.Instance.GenerateString(10);
            string sourceSchema=RandomDataGenerator.Instance.GenerateString(10);
            string sourceTable= RandomDataGenerator.Instance.GenerateString(10);
            TableSourceAttribute attribute = new(
                $"[{sourceConnection}]",
                $"[{sourceSchema}]",
                $"[{sourceTable}]", ['[', ']']);
            attribute.ConnectionName.Should().Be(sourceConnection);
            attribute.SchemaName.Should().Be(sourceSchema);
            attribute.TableName.Should().Be(sourceTable);
        }
    }
}
