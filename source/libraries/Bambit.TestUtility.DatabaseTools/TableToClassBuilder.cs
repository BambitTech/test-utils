using System.ComponentModel.DataAnnotations;
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

        IList<DatabaseMappedClassPropertyDefinition> propertyDefinitions = GetProperties(catalogRecordName, schemaName, tableName);

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
        foreach (DatabaseMappedClassPropertyDefinition propertyDefinition in propertyDefinitions)
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

    private static void CreateProperty(TypeBuilder builder, DatabaseMappedClassPropertyDefinition propertyDefinition)
    {
        if (propertyDefinition.MappedType == null)
        {
            return;
        }


        Type? underlyingType = Nullable.GetUnderlyingType(propertyDefinition.MappedType);
        string safeName = Clean(propertyDefinition.Name);
        FieldBuilder fieldBuilder = builder.DefineField($"_{safeName}", propertyDefinition.MappedType, FieldAttributes.Private);

        PropertyBuilder propertyBuilder = builder.DefineProperty(safeName, PropertyAttributes.HasDefault,
            propertyDefinition.MappedType, null);
        MethodBuilder getBuilder = builder.DefineMethod($"get_{safeName}",
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyDefinition.MappedType,
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
                null, [propertyDefinition.MappedType]);

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
        ConstructorInfo ci = fieldSourceAttributeType.GetConstructor([typeof(string), typeof(Type), typeof(string)])!;
        CustomAttributeBuilder cab = new(ci, [propertyDefinition.Name, propertyDefinition.MappedType, propertyDefinition.SourceType]);
        propertyBuilder.SetCustomAttribute(cab);

        if (propertyDefinition.MappedType == typeof(string))
        {
            if (propertyDefinition.MaxSize > 0)
            {

                Type maxLengthType = typeof(MaxLengthAttribute);
                ci = maxLengthType.GetConstructor([typeof(int)])!;
                cab = new CustomAttributeBuilder(ci, [propertyDefinition.MaxSize]);
                propertyBuilder.SetCustomAttribute(cab);
            }

            if (propertyDefinition.IsNullable)
            {
                Type nullableAttributeType = typeof(DatabaseNullableAttribute);
                ConstructorInfo nci = nullableAttributeType.GetConstructor([])!;
                CustomAttributeBuilder nullableAttribute = new(nci,[]);
                propertyBuilder.SetCustomAttribute(nullableAttribute);

            }
        }
        else if (propertyDefinition.MappedType == typeof(float) || propertyDefinition.MappedType == typeof(decimal)
                                                                || propertyDefinition.MappedType == typeof(double)
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

    private IList<DatabaseMappedClassPropertyDefinition> GetProperties(string connectionName, string schema, string tableName)
    {
        using ITestDbConnection connection = TestDatabaseFactory.GetConnection(connectionName);
        return connection.GetProperties(schema, tableName);

    }

    #endregion Private Methods
}
