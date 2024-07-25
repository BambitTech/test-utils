namespace Bambit.TestUtility.DatabaseTools.Exceptions
{
    /// <summary>
    /// Exception thrown if a generation fails
    /// </summary>
    public class TableGenerationException : Exception
    {

        /// <summary>
        /// The name of the table that was attempting to be generated against
        /// </summary>
        public string? TableName { get; }

        /// <summary>
        /// The name of the property that was trying to be generated
        /// </summary>
        public string? PropertyName { get; }

        /// <summary>
        /// The name of the connection that was being generated against
        /// </summary>
        public string? ConnectionName { get; }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public TableGenerationException()
        {
        }

        /// <summary>
        /// Initializes a new instance with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        public TableGenerationException(string message) : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance for an inner exception and message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception that was thrown.</param>
        public TableGenerationException(string message, Exception innerException) : base(message, innerException)
        {

        }
        
        /// <summary>
        /// Initializes a new instance for a given connection, table and property
        /// </summary>
        /// <param name="connectionName">The name of the connection that was being generated against.</param>
        /// <param name="tableName">The name of the table that was being generated against</param>
        /// <param name="propertyName">The name of the property that was being generated against</param>
        public TableGenerationException(string? connectionName, string? tableName, string? propertyName) :
            base(
                $"An exception was thrown for property '{propertyName}' in table '{tableName}' in connection '{connectionName}'")
        {
            TableName = tableName;
            ConnectionName = connectionName;
            PropertyName = propertyName;
        }
        /// <summary>
        /// Initializes a new instance for a given connection, table and property along with a custom message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="connectionName">The name of the connection that was being generated against.</param>
        /// <param name="tableName">The name of the table that was being generated against</param>
        /// <param name="propertyName">The name of the property that was being generated against</param>
        public TableGenerationException(string message, string? connectionName, string? tableName,
            string? propertyName) :
            base(message)
        {
            TableName = tableName;
            ConnectionName = connectionName;
            PropertyName = propertyName;
        }

        /// <summary>
        /// Initializes a new instance for a given connection, table and property and inner exception, along with a custom message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="connectionName">The name of the connection that was being generated against.</param>
        /// <param name="tableName">The name of the table that was being generated against</param>
        /// <param name="propertyName">The name of the property that was being generated against</param>
        /// <param name="innerException">The inner exception that was thrown.</param>
        public TableGenerationException(string message, string? connectionName, string? tableName, string? propertyName,
            Exception innerException) :
            base(message, innerException)
        {
            TableName = tableName;
            ConnectionName = connectionName;
            PropertyName = propertyName;
        }


        /// <summary>
        /// Initializes a new instance for a given connection, table and property, along with an inner exception
        /// </summary>
        /// <param name="connectionName">The name of the connection that was being generated against.</param>
        /// <param name="tableName">The name of the table that was being generated against</param>
        /// <param name="propertyName">The name of the property that was being generated against</param>
        /// <param name="innerException">The inner exception that was thrown.</param>
        public TableGenerationException(string? connectionName, string? tableName, string? propertyName,
            Exception innerException) :
            base(
                $"An exception was thrown for property '{propertyName}' in table '{tableName}' in connection '{connectionName}'",
                innerException)
        {
            TableName = tableName;
            ConnectionName = connectionName;
            PropertyName = propertyName;
        }


    }
}
