using System.Reflection;
using Bambit.TestUtility.DatabaseTools.Attributes;
using Bambit.TestUtility.DataGeneration;

namespace Bambit.TestUtility.DatabaseTools
{
    /// <summary>
    /// Specialized <see cref="IRandomDataGenerator"/> implementation, providing to methods to generate random data from a table
    /// </summary>
    /// <param name="testDatabaseFactory"></param>
    public class DatabaseRandomizer(ITestDatabaseFactory testDatabaseFactory) : RandomDataGenerator
    {
        private readonly Dictionary<string, Func<IRandomDataGenerator, object>> TableFieldSpecificGenerators = new(StringComparer.CurrentCultureIgnoreCase);
        private ITestDatabaseFactory TestDatabaseFactory { get; } = testDatabaseFactory;

        private string BuildFieldKey(string connectionName, string schema, string tableName, string fieldName)
        {
            Func<string, string> cleaner = TestDatabaseFactory.GetCleanIdentifierFunc(connectionName);
            Func<string, string> escaper = TestDatabaseFactory.GetEscapeIdentifierFunc(connectionName);
            return
                $"{escaper(cleaner(connectionName))}.{escaper(cleaner(schema))}.{escaper(cleaner(tableName))}.{escaper(cleaner(fieldName))}"
                    .ToLower();
        }

        /// <summary>
        /// Registers a specialized function to use when generating the targeted field
        /// </summary>
        /// <param name="connectionName">The <see cref="IDatabaseCatalogRecord"/> name</param>
        /// <param name="schema">The schema the table belongs to</param>
        /// <param name="tableName">The table the field belong to</param>
        /// <param name="fieldName">The field to generate</param>
        /// <param name="generator">The generator to use.</param>
        public void RegisterTableFieldGenerator(string connectionName, string schema, string tableName,
            string fieldName, Func<IRandomDataGenerator,object> generator)
        {
            string key = BuildFieldKey(connectionName, schema, tableName, fieldName);
            TableFieldSpecificGenerators[key] = generator;
        }

        /// <summary>
        /// Retrieves a <see cref="Func{T1, TResult}"/> if it was previously registered by <see cref="RegisterTableFieldGenerator"/>
        /// </summary>
        /// <param name="connectionName">The <see cref="IDatabaseCatalogRecord"/> name</param>
        /// <param name="schema">The schema the table belongs to</param>
        /// <param name="tableName">The table the field belong to</param>
        /// <param name="fieldName">The field to generate</param>
        /// <returns></returns>
        public Func<IRandomDataGenerator,object>? GetTableFieldGenerator(string connectionName, string schema, string tableName,
            string fieldName)
        {
            if (
                string.IsNullOrWhiteSpace(connectionName) ||
                string.IsNullOrWhiteSpace(schema) ||
                string.IsNullOrWhiteSpace(tableName) ||
                string.IsNullOrWhiteSpace(fieldName)
            )
                return null;

            string key = BuildFieldKey(connectionName, schema, tableName, fieldName);
            return TableFieldSpecificGenerators.GetValueOrDefault(key);
        }

        ///<inheritdoc />
        public override T InitializeObject<T>(T objectToInitialize) 
        {
            Type objectType = objectToInitialize.GetType();
            PropertyInfo[] propertyInfos =
                objectType.GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public)
                    .Where(p => p is { CanRead: true, CanWrite: true }).ToArray();
            if (objectType.GetCustomAttributes(typeof(TableSourceAttribute)).FirstOrDefault() is not TableSourceAttribute tableSourceAttribute)
            {
                return base.InitializeObject(objectToInitialize);
            }

            string connectionName = tableSourceAttribute.ConnectionName;
            string tableName = tableSourceAttribute.TableName;
            string schema = tableSourceAttribute.SchemaName;

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                Func<IRandomDataGenerator, object>? fieldGenerator = null;
                if (propertyInfo.GetCustomAttributes(typeof(FieldSourceAttribute)).FirstOrDefault() is FieldSourceAttribute fieldSourceAttribute)

                    fieldGenerator =
                        GetTableFieldGenerator(connectionName, schema, tableName, fieldSourceAttribute.Name);
                if (fieldGenerator != null)
                {
                    propertyInfo.SetValue(objectToInitialize, fieldGenerator(this));
                }
                else
                {
                    AssignValueToProperty(objectToInitialize, propertyInfo, MaxRecursion);
                }

            }

            return objectToInitialize;
        }
    }
}