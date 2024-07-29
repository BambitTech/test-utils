using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Bambit.TestUtility.DatabaseTools;

/// <summary>
/// Base class for <see cref="ITestDbConnection"/> implemention
/// </summary>
/// <param name="connection">The <see cref="IDbConnection"/> that is wrapped</param>
public abstract class TestDbConnection(IDbConnection connection) : ITestDbConnection
{
    /// <summary>
    /// The <see cref="IDbConnection"/> that is wrapped
    /// </summary>
    protected IDbConnection Connection { get; } = connection;
    /// <inheritdoc />
    public int? CommandTimeout { get; set; }
    
    /// <summary>
    /// Holds the buffer of info messages if <see cref="TrackInfoMessages"/> is activated
    /// </summary>
    protected List<string> OutputMessagesStore { get; set; } = new();

    /// <inheritdoc />
    public string[] OutputMessages => OutputMessagesStore.ToArray();

    /// <inheritdoc />
    public void Dispose()
    {
        Connection.Dispose();
        Connection.Dispose();
    }

    /// <inheritdoc />
    public virtual IDbTransaction BeginTransaction()
    {
        return Connection.BeginTransaction();
    }

    /// <inheritdoc />
    public virtual IDbTransaction BeginTransaction(IsolationLevel il)
    {
        return Connection.BeginTransaction(il);
    }

    /// <inheritdoc />
    public virtual void ChangeDatabase(string databaseName)
    {
        Connection.ChangeDatabase(databaseName);
    }

    /// <inheritdoc />
    public virtual void Close()
    {
        Connection.Close();
    }

    /// <inheritdoc />
    public virtual IDbCommand CreateCommand()
    {
        IDbCommand dbCommand =Connection.CreateCommand();
        if(CommandTimeout.HasValue)
            dbCommand.CommandTimeout= CommandTimeout.Value;
        return dbCommand;
    }

    /// <inheritdoc />
    public virtual void Open()
    {
        try
        {

            Connection.Open();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error connection to {ConnectionString}", ex);
        }
    }
        
    /// <inheritdoc />
    [AllowNull]
    public virtual string ConnectionString
    {
        get => Connection.ConnectionString;
        set => Connection.ConnectionString = value;
    }
    /// <inheritdoc />
    public virtual int ConnectionTimeout => Connection.ConnectionTimeout;
    /// <inheritdoc />
    public virtual string Database => Connection.Database;
    /// <inheritdoc />
    public virtual ConnectionState State => Connection.State;

    /// <inheritdoc />
    public abstract bool TrackInfoMessages();
    /// <inheritdoc />
    public abstract void UntrackInfoMessages();

    /// <inheritdoc />
    public virtual void ClearInfoMessages()
    {
        OutputMessagesStore.Clear();
    }
    /// <inheritdoc />
    public event EventHandler<string>? MessageReceived;

    /// <inheritdoc />
    public abstract DataComparisonResults CompareTableToDataset(string schema, string tableName,
        string[] columns, IEnumerable<object?[]> rows, bool allowUnexpectedRows = false);
    /// <inheritdoc />
    public abstract DataComparisonResults CompareResults(

        string[] columns,
        string?[] expectedColumnTypes,
        IEnumerable<object?[]> expectedRows,
        IEnumerable<object?[]> compareRows,
        bool allowUnexpectedRows = false);
    /// <inheritdoc />
    public abstract void Persist(DatabaseMappedClass mappedClass);

    /// <summary>
    /// Processes received messages and fires off the <see cref="MessageReceived"/> event
    /// </summary>
    /// <param name="message"></param>
    protected void AddReceivedMessage(string message)
    {
        EventHandler<string>? handler = MessageReceived;
        if (handler != null)
        {
            handler.Invoke(this, message);
        }
    }

    /// <inheritdoc />
    public virtual T ExecuteScalar<T>(string query)
    {
        using IDbCommand command = CreateCommand();

        command.CommandText = query;
        return (T)command.ExecuteScalar()!;

    }
    /// <inheritdoc />
    public virtual void ExecuteQuery(string query)
    {
        using IDbCommand command = CreateCommand();

        command.CommandText = query;
        command.ExecuteNonQuery();

    }
    
    /// <inheritdoc />
    public IDataReader ExecuteReader(string query)
    {
        using IDbCommand command = CreateCommand();
        command.CommandText = query;
        command.CommandType = CommandType.Text;
        return command.ExecuteReader();
    }
}