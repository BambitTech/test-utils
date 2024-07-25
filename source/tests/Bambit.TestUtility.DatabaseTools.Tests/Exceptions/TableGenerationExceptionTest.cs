using Bambit.TestUtility.DatabaseTools.Exceptions;
using Bambit.TestUtility.DataGeneration;
using FluentAssertions;

namespace Bambit.TestUtility.DatabaseTools.Tests.Exceptions;

[TestClass]
[TestCategory("Unit")]
public class TableGenerationExceptionTest
{
    [TestMethod]
    public void Ctor_Default_InitializesEmpty()
    {
        TableGenerationException exception=new();
        exception.InnerException.Should().BeNull();
        exception.ConnectionName.Should().BeNull();
        exception.PropertyName.Should().BeNull();
        exception.TableName.Should().BeNull();
    }
    [TestMethod]
    public void Ctor_WithMessage_AssignsAsExpected()
    {
        string message = RandomDataGenerator.Instance.GenerateString(50);
        TableGenerationException exception=new(message);
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
        exception.ConnectionName.Should().BeNull();
        exception.PropertyName.Should().BeNull();
        exception.TableName.Should().BeNull();
    }
    
    [TestMethod]
    public void Ctor_WithMessageAndException_AssignsAsExpected()
    {
        Exception subException = new();
        string message = RandomDataGenerator.Instance.GenerateString(50);
        TableGenerationException exception=new(message, subException);
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(subException);
        exception.ConnectionName.Should().BeNull();
        exception.PropertyName.Should().BeNull();
        exception.TableName.Should().BeNull();
    }

    
    
    [TestMethod]
    public void Ctor_NoMessage_AssignsAsExpected()
    {
        Exception subException = new();
        string connectionName = RandomDataGenerator.Instance.GenerateString(10);
        string propertyName=RandomDataGenerator.Instance.GenerateString(10);
        string tableName= RandomDataGenerator.Instance.GenerateString(10);
        TableGenerationException exception=new(connectionName, tableName,propertyName,  subException);
        exception.Message.Should().NotBeNull().And
            .Be(
                $"An exception was thrown for property '{propertyName}' in table '{tableName}' in connection '{connectionName}'");
        exception.InnerException.Should().Be(subException);
        exception.ConnectionName.Should().Be(connectionName);
        exception.PropertyName.Should().Be(propertyName);
        exception.TableName.Should().Be(tableName);
    }
}