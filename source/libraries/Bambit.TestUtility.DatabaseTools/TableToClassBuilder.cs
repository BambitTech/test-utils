using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection.Emit;
using System.Reflection;
using System.Text.RegularExpressions;
using Bambit.TestUtility.DatabaseTools.Attributes;
using Bambit.TestUtility.DatabaseTools.Exceptions;
using Bambit.TestUtility.DataGeneration.Attributes;

namespace Bambit.TestUtility.DatabaseTools;


/// <summary>
/// Provides methods to dynamically generate a class from a table definition
/// </summary>
/// <param name="testDatabaseFactory"></param>
public class TableToClassBuilder(ITestDatabaseFactory testDatabaseFactory) : ITableToClassBuilder
{

    private static readonly Regex RemoveNonAlphaNumericRegex = new("[^a-zA-Z0-9]");

    /// <summary>
    /// The <see cref="ITestDatabaseFactory"/> used to retrieve database information
    /// </summary>
    protected ITestDatabaseFactory TestDatabaseFactory { get; } = testDatabaseFactory;

    /// <summary>
    /// Internal structure used when generating classes
    /// </summary>
    struct PropertyDefinition
    {
        public string Name { get; init; }
        public string Type { get; init; }
        public bool IsNullable { get; init; }
        public int MaxSize { get; init; }
        public byte Precision { get; init; }
        public byte Scale { get; init; }
        public bool IsComputed { get; init; }


    }

    #region ITableToClassBuilder implementatioon

    /// <inheritdoc />
    public DatabaseMappedClass? GenerateObjectFromTable(string catalogRecordName, string schemaName, string tableName)
    {
        Type type = GenerateClassTypeFromTable(catalogRecordName, schemaName, tableName);
        return Activator.CreateInstance(type) as DatabaseMappedClass;
    }

    /// <inheritdoc />
    public Type GenerateClassTypeFromTable(string catalogRecordName, string schemaName, string tableName)
    {

        IList<PropertyDefinition> propertyDefinitions = GetProperties(catalogRecordName, schemaName, tableName);

        IDatabaseCatalogRecord databaseCatalogRecord = TestDatabaseFactory.GetGenerator(catalogRecordName);
        TypeBuilder builder =
            CreateTypeBuilder($"{Clean(catalogRecordName)}.{Clean(schemaName)}.{Clean(tableName)}");
        Type tableSourceAttributeType = typeof(TableSourceAttribute);
        ConstructorInfo ci =
            tableSourceAttributeType.GetConstructor([typeof(string), typeof(string), typeof(string), typeof(char[])])!;

        CustomAttributeBuilder cab = new(ci,
            [catalogRecordName, schemaName, tableName, databaseCatalogRecord.Qualifiers]);
        builder.SetCustomAttribute(cab);

        builder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName |
                                         MethodAttributes.RTSpecialName);

        // NOTE: assuming your list contains Field objects with fields FieldName(string) and FieldType(Type)
        foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
            try
            {
                CreateProperty(builder, propertyDefinition);
            }
            catch (Exception e)
            {
                throw new TableGenerationException(catalogRecordName, tableName, propertyDefinition.Name, e);
            }


        Type objectType = builder.CreateType();
        return objectType;

    }

    #endregion ITableToClassBuilder implementatioon

    #region Private Methods

    private static string Clean(string input)
    {
        return RemoveNonAlphaNumericRegex.Replace(input, string.Empty);
    }

    private static void CreateProperty(TypeBuilder builder, PropertyDefinition propertyDefinition)
    {
        Type? propertyType = GetPropertyType(propertyDefinition);
        if (propertyType == null)
        {
            return;
        }

        Type? underlyingType = Nullable.GetUnderlyingType(propertyType);
        string safeName = Clean(propertyDefinition.Name);
        FieldBuilder fieldBuilder = builder.DefineField($"_{safeName}", propertyType, FieldAttributes.Private);

        PropertyBuilder propertyBuilder = builder.DefineProperty(safeName, PropertyAttributes.HasDefault,
            propertyType, null);
        MethodBuilder getBuilder = builder.DefineMethod($"get_{safeName}",
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType,
            Type.EmptyTypes);
        ILGenerator getIl = getBuilder.GetILGenerator();

        getIl.Emit(OpCodes.Ldarg_0);
        getIl.Emit(OpCodes.Ldfld, fieldBuilder);
        getIl.Emit(OpCodes.Ret);

        MethodBuilder setBuilder =
            builder.DefineMethod($"set_{safeName}",
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig,
                null, new[] { propertyType });

        ILGenerator setIl = setBuilder.GetILGenerator();
        Label modifyProperty = setIl.DefineLabel();
        Label exitSet = setIl.DefineLabel();

        setIl.MarkLabel(modifyProperty);
        setIl.Emit(OpCodes.Ldarg_0);
        setIl.Emit(OpCodes.Ldarg_1);
        setIl.Emit(OpCodes.Stfld, fieldBuilder);

        setIl.Emit(OpCodes.Nop);
        setIl.MarkLabel(exitSet);
        setIl.Emit(OpCodes.Ret);

        propertyBuilder.SetGetMethod(getBuilder);
        propertyBuilder.SetSetMethod(setBuilder);

        Type fieldSourceAttributeType = typeof(FieldSourceAttribute);
        ConstructorInfo ci = fieldSourceAttributeType.GetConstructor([typeof(string), typeof(string)])!;
        CustomAttributeBuilder cab = new(ci, [propertyDefinition.Name, propertyDefinition.Type]);
        propertyBuilder.SetCustomAttribute(cab);

        if (propertyType == typeof(string) && propertyDefinition.MaxSize > 0)
        {

            Type maxLengthType = typeof(MaxLengthAttribute);
            ci = maxLengthType.GetConstructor([typeof(int)])!;
            cab = new CustomAttributeBuilder(ci, [propertyDefinition.MaxSize]);
            propertyBuilder.SetCustomAttribute(cab);
        }
        else if (propertyType == typeof(float) || propertyType == typeof(decimal)
                                               || propertyType == typeof(double)
                                               || (
                                                   underlyingType != null &&
                                                   (underlyingType == typeof(float) || underlyingType == typeof(decimal)
                                                       || underlyingType == typeof(double)
                                                   )))
        {
            Type precisionAttribute = typeof(DecimalPrecisionAttribute);
            ci = precisionAttribute.GetConstructor([typeof(byte), typeof(byte)])!;
            cab = new CustomAttributeBuilder(ci, [propertyDefinition.Precision, propertyDefinition.Scale]);

            propertyBuilder.SetCustomAttribute(cab);
        }

        if (propertyDefinition.IsComputed)
        {
            Type computerColumnType = typeof(ComputedColumnAttribute);
            ci = computerColumnType.GetConstructor(Type.EmptyTypes)!;
            cab = new CustomAttributeBuilder(ci, []);
            propertyBuilder.SetCustomAttribute(cab);
        }
    }

    private static Type? GetPropertyType(PropertyDefinition propertyDefinition)
    {
        Type? type;
        switch (propertyDefinition.Type)
        {
            case "bigint":
            case "timestamp":
            case "long":
                type = propertyDefinition.IsNullable ? typeof(long?) : typeof(long);
                break;
            case "image":
            case "binary":
            case "varbinary":
            case "bytearray":
                type = typeof(byte[]);
                break;
            case "bit":
            case "boolean":
            case "bool":
                type = propertyDefinition.IsNullable ? typeof(bool?) : typeof(bool);
                break;
            case "date":
            case "datetime":
            case "datetime2":
            case "smalldatetime":
                type = propertyDefinition.IsNullable ? typeof(DateTime?) : typeof(DateTime);
                break;
            case "datetimeoffset":
                type = propertyDefinition.IsNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
                break;
            case "money":
            case "numeric":
            case "smallmoney":
            case "decimal":
                type = propertyDefinition.IsNullable ? typeof(decimal?) : typeof(decimal);
                break;
            case "real":
            case "float":
            case "double":
                type = propertyDefinition.IsNullable ? typeof(double?) : typeof(double);
                break;
            case "int":
                type = propertyDefinition.IsNullable ? typeof(int?) : typeof(int);
                break;
            case "nchar":
            case "ntext":
            case "nvarchar":
            case "varchar":
            case "text":
            case "char":
            case "xml":
                type = typeof(string);
                break;
            case "smallint":
            case "short":
                type = propertyDefinition.IsNullable ? typeof(short?) : typeof(short);
                break;
            case "time":
            case "timespan":
                type = propertyDefinition.IsNullable ? typeof(TimeSpan?) : typeof(TimeSpan);
                break;
            case "tinyint":
            case "byte":
                type = propertyDefinition.IsNullable ? typeof(byte?) : typeof(byte);
                break;
            case "uniqueidentifier":
            case "guid":
                type = propertyDefinition.IsNullable ? typeof(Guid?) : typeof(Guid);
                break;
            default:
                type = null;
                break;
        }

        return type;
    }


    private static TypeBuilder CreateTypeBuilder(string signature)
    {
        AssemblyName assemblyName = new($"Bambit.Generated.{signature}");
        AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder moduleBuilder = builder.DefineDynamicModule("MainModule");
        return moduleBuilder.DefineType(signature,
            TypeAttributes.Public |
            TypeAttributes.Class |
            TypeAttributes.AutoClass |
            TypeAttributes.AnsiClass |
            TypeAttributes.BeforeFieldInit |
            TypeAttributes.AutoLayout,
            typeof(DatabaseMappedClass));
    }

    private IList<PropertyDefinition> GetProperties(string connectionName, string schema, string tableName)
    {

        IDatabaseCatalogRecord databaseCatalogRecord = TestDatabaseFactory.GetGenerator(connectionName);

        List<PropertyDefinition> properties = new List<PropertyDefinition>();
        using IDbConnection connection =
            databaseCatalogRecord.GetConnection(TestDatabaseFactory.GetConnectionString(connectionName));

        connection.Open();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = databaseCatalogRecord.TableDefinitionQuery;
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = "@tableName";
        parameter.Value = $"{schema}.{tableName}";
        command.Parameters.Add(parameter);
        {
            using IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                properties.Add(
                    new PropertyDefinition
                    {
                        Name = reader.GetString(0),
                        Type = reader.GetString(1),
                        IsNullable = reader.GetInt32(2) == 1,
                        MaxSize = reader.GetInt16(3),
                        Precision = reader.GetByte(4),
                        Scale = reader.GetByte(5),
                        IsComputed = reader.GetInt32(6) > 0
                    }
                );
            }
        }



        return properties;

    }

    #endregion Private Methods
}
