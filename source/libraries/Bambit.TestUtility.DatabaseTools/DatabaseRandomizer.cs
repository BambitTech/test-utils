using System.Reflection;
using Bambit.TestUtility.DatabaseTools.Attributes;
using Bambit.TestUtility.DataGeneration;

namespace Bambit.TestUtility.DatabaseTools
{
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

        public void RegisterTableFieldGenerator(string connectionName, string schema, string tableName,
            string fieldName, Func<IRandomDataGenerator,object> generator)
        {
            string key = BuildFieldKey(connectionName, schema, tableName, fieldName);
            TableFieldSpecificGenerators[key] = generator;
        }

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

        public override T InitializeObject<T>(T objectToInitialize) 
        {
            Type objectType = objectToInitialize.GetType();
            PropertyInfo[] propertyInfos =
                objectType.GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public)
                    .Where(p => p is { CanRead: true, CanWrite: true }).ToArray();
            TableSourceAttribute? tableSourceAttribute =
                objectType.GetCustomAttributes(typeof(TableSourceAttribute)).FirstOrDefault() as TableSourceAttribute;
            if (tableSourceAttribute == null)
            {
                return base.InitializeObject(objectToInitialize);
            }

            string connectionName = tableSourceAttribute.ConnectionName;
            string tableName = tableSourceAttribute.TableName;
            string schema = tableSourceAttribute.SchemaName;

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                FieldSourceAttribute? fieldSourceAttribute =
                    propertyInfo.GetCustomAttributes(typeof(FieldSourceAttribute)).FirstOrDefault() as
                        FieldSourceAttribute;
                Func<IRandomDataGenerator,object>? fieldGenerator = null;
                if (fieldSourceAttribute != null)

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