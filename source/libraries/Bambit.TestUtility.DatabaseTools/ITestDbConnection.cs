using System.Data;

namespace Bambit.TestUtility.DatabaseTools
{
    /// <summary>
    /// Contains results of a comparison
    /// </summary>
    /// <param name="IsSuccess">Bool indicating if the comparison was valid</param>
    /// <param name="NumberRowsMissing">Indicates the number of rows that were expected by not there</param>
    /// <param name="NumberRowsNotExpected">Indicates the number of rows that were not expected by were present</param>
    public record DataComparisonResults(bool IsSuccess, int NumberRowsMissing, int NumberRowsNotExpected);

    /// <summary>
    /// Defines a test database connection.  Extends <see cref="IDbConnection"/>
    /// </summary>
    public interface ITestDbConnection : IDbConnection
    {
        /// <summary>
        /// Static success result
        /// </summary>
        static readonly DataComparisonResults Success = new(true, 0, 0);
        
        /// <summary>
        /// Event fire for any messages that need conveying
        /// </summary>
        event EventHandler<string> MessageReceived;

        /// <summary>
        /// Compares a database table to a collection of objects, returning <c>null</c> if ti
        /// </summary>
        /// <param name="schema">The schema of the table to compare against</param>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="allowUnexpectedRows"></param>
        /// <returns></returns>
        DataComparisonResults CompareTableToDataset(string schema, string tableName,
             string[] columns, IEnumerable<object?[]> rows, bool allowUnexpectedRows = false);
        /// <summary>
        /// Writes a mapped class to the database
        /// </summary>
        /// <param name="mappedClass">The mapped class to persist</param>
        void Persist(DatabaseMappedClass mappedClass);
        /// <summary>
        /// A collection of messages received from the database, if enabled by <see cref="TrackInfoMessages"/>
        /// </summary>
        string[] OutputMessages { get; }
        /// <summary>
        /// Begins tracking output messages from the database
        /// </summary>
        /// <returns><c>true</c> if the database can track messages; otherwise <c>false</c></returns>
        bool TrackInfoMessages();
        /// <summary>
        /// stops tracking output messages from the database
        /// </summary>

        void UntrackInfoMessages();

        /// <summary>
        /// Clears all current output messages
        /// </summary>
        void ClearInfoMessages();
        /// <summary>
        /// Executes a query returning a scalar value
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="query">The query to execute</param>
        /// <returns>The result from the query.</returns>
        T ExecuteScalar<T>(string query);

        /// <summary>
        /// Executes a query, returning nothing
        /// </summary>
        /// <param name="query">The query to execute</param>
        void ExecuteQuery(string query);

        /// <summary>
        /// Executes a query, returning a <see cref="IDataReader"/>
        /// </summary>
        /// <param name="query">The query to execute</param>
        /// <returns>A <see cref="IDataReader"/></returns>
        IDataReader ExecuteReader(string query);


        /// <summary>
        /// The command timeout to use for executed commands.  If null, default value for the connection will be used
        /// </summary>
         int? CommandTimeout { get; set; }
        /// <summary>
        /// Compares a collection of objects to ensure they have the same values.
        /// </summary>
        /// <param name="columns">The columns in the data set to compare</param>
        /// <param name="expectedColumnTypes">The types of data in the columns</param>
        /// <param name="expectedRows">The rows that are expected</param>
        /// <param name="compareRows">The rows to compare</param>
        /// <param name="allowUnexpectedRows">Flag indicating if unexpected rows in the compares set are to be allowed</param>
        /// <returns>A <see cref="DataComparisonResults"/> object with the results of the comparison</returns>
         DataComparisonResults CompareResults(

            string[] columns,
            string?[] expectedColumnTypes,
            IEnumerable<object?[]> expectedRows,
            IEnumerable<object?[]> compareRows,
            bool allowUnexpectedRows = false);
    }
}
