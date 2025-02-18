﻿
namespace Bambit.TestUtility.DatabaseTools.Reqnroll.Tools;

/// <summary>
/// State manager of databases
/// </summary>
public interface IDatabaseStateManager
{
    /// <summary>
    /// Gets the last used connection name
    /// </summary>
    string LastDatabaseConnectionName { get; set; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets current connector. </summary>
    ///
    /// <returns>   The current connector. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    IDatabaseCatalogRecord GetCurrentConnector();

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Opens connection for name. </summary>
    ///
    /// <param name="connectionName">   Name of the connection. </param>
    ///
    /// <returns>   An ITestDbConnection. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    ITestDbConnection OpenConnectionForName(string connectionName);

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets the database class factory. </summary>
    ///
    /// <value> The database class factory. </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    DatabaseClassFactory DatabaseClassFactory { get; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets a value indicating whether this object is debug mode on. </summary>
    ///
    /// <value> True if this object is debug mode on, false if not. </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public bool IsDebugModeOn { get; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets the variables. </summary>
    ///
    /// <value> The variables. </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    Dictionary<string, object> Variables { get; }
    

}