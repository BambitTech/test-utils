namespace Bambit.TestUtility.DatabaseTools;

/// <summary>
/// 
/// </summary>
/// <param name="Connection">The firing <see cref="ITestDbConnection"/></param>
/// <param name="Message">The message received</param>
public record TestDbConnectionMessageReceivedEvent(ITestDbConnection Connection, string Message) ;