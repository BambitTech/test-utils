namespace Bambit.TestUtility.DatabaseTools.Attributes;

/// <summary>
/// Describes the source field name and type for a generated class
/// </summary>
/// <param name="sourceName">The name of the field in the database</param>
/// <param name="sourceType">The type of field in the database</param>
[AttributeUsage(AttributeTargets.Property)]
public class FieldSourceAttribute(string sourceName,string sourceType) : Attribute
{
    /// <summary>
    /// The name of the field in the database
    /// </summary>
    public string Name { get;  } = sourceName;
    /// <summary>
    /// The (mapped) data type of the field in the database
    /// </summary>
    public string SourceType { get;  } = sourceType;
}