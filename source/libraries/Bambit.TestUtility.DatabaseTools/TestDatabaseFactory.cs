namespace Bambit.TestUtility.DatabaseTools;

/// <summary>
/// Basic implementation of the <see cref="ITestDatabaseFactory"/> interface
/// </summary>
public class TestDatabaseFactory : ITestDatabaseFactory
{
    ///<inheritdoc />
    public event EventHandler<TestDbConnectionMessageReceivedEvent>? MessageReceived;

    #region Private Properties

    private readonly Dictionary<string, IDatabaseCatalogRecord> InformationFactories = new (StringComparer.CurrentCultureIgnoreCase);
    private Dictionary<string, MappedDatabase> ConnectionMap { get; } 
    

    #endregion Private Properties

    #region ITestDatabaseFactory Implementation

    

    /// <summary>
    /// Creates a new instances, based on the supplied options
    /// </summary>
    /// <param name="options">A <see cref="TestDatabaseFactoryOptions"/></param> object to initialize from
    public TestDatabaseFactory( TestDatabaseFactoryOptions options)
    {
        ConnectionMap = options.MappedDatabases.ToDictionary(StringComparer.CurrentCultureIgnoreCase);
        foreach(KeyValuePair<string, string> kvp in options.DatabaseCatalogRecordMap)
        {
            Type type = Type.GetType(kvp.Value)!;
            if(Activator.CreateInstance(type) is IDatabaseCatalogRecord instance)
            {
                RegisterDatabaseInformation(kvp.Key, instance);   
            }
        }
    }

    /// <summary>
    /// Adds or updates a mapped database
    /// </summary>
    /// <param name="name">the name of the database connection</param>
    /// <param name="database">The <see cref="MappedDatabase"/> information</param>
    public void AddOrUpdateMappedDatabase(string name, MappedDatabase database)
    {
        ConnectionMap[name] = database;
    }
    /// <inheritdoc />
    public ITestDbConnection GetConnection(string name)
    {
        MappedDatabase mappedDatabase = ConnectionMap[name];

        IDatabaseCatalogRecord catalogRecord =GetGenerator(name);
        ITestDbConnection testDbConnection = catalogRecord.GetConnection(mappedDatabase.ConnectionString);
        testDbConnection.MessageReceived +=
            (sender, message) => OnMessageReceived((sender as ITestDbConnection)!, message);
        return testDbConnection;
    }
    
    /// <inheritdoc />
    public IDatabaseCatalogRecord GetGenerator(string name)
    {
      
        MappedDatabase mappedDatabase = ConnectionMap[name];
        return InformationFactories[mappedDatabase.DatabaseCatalog];
    }
    /// <inheritdoc />
    public string GetConnectionString(string name)
    {
        return ConnectionMap[name].ConnectionString;
    }
    
    /// <inheritdoc />
    public void RegisterDatabaseInformation(string name,IDatabaseCatalogRecord catalogRecord)
    {
        InformationFactories.Add( name, catalogRecord);
    }

    /// <inheritdoc />
    public Func<string, string> GetCleanIdentifierFunc(string connectionName)
    {
        return GetGenerator(connectionName).CleanQualifiers;
    }

    /// <inheritdoc />
    public Func<string, string> GetEscapeIdentifierFunc(string connectionName)
    {
        return GetGenerator(connectionName).EscapeToken;
    }
    

    #endregion ITestDatabaseFactory Implementation

    #region Protected Methods
    /// <summary>
    /// Handles <see cref="TestDbConnection.MessageReceived"/> events
    /// </summary>
    /// <param name="source">The <see cref="ITestDatabaseFactory"/> that generated the even</param>
    /// <param name="message">The received message</param>
    /// <remarks>Bubbles the event up with new arguments</remarks>
    protected void OnMessageReceived(ITestDbConnection source,string message)
    {
        EventHandler<TestDbConnectionMessageReceivedEvent>? handler = MessageReceived;
        handler?.Invoke(this, new TestDbConnectionMessageReceivedEvent(source, message));
    }

    #endregion Protected Methods
}