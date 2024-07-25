namespace Bambit.TestUtility.DatabaseTools;

/// <summary>
/// Base implementation of the <see cref="IDatabaseCatalogRecord"/>
/// </summary>
public abstract class DatabaseCatalogRecord : IDatabaseCatalogRecord
{
    #region Properties

    /// <inheritdoc />
    public abstract string TableDefinitionQuery { get; }

    /// <inheritdoc />
    public abstract char[] Qualifiers { get;  }

    #endregion Properties

    #region Methods

    /// <inheritdoc />
    public string CleanQualifiers(string input)
    {
        return  input.Trim(Qualifiers);
    }
    /// <inheritdoc />
    public virtual string CleanAndEscapeToken(string token)
    {
        return EscapeToken(CleanQualifiers(token));
    }

    /// <inheritdoc />
    public abstract string EscapeToken(string token);
    
    
    /// <inheritdoc />
    public abstract ITestDbConnection GetConnection(string connectionString);

    #endregion Methods
}