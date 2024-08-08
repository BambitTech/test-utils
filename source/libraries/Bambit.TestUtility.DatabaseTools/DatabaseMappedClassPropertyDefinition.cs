namespace Bambit.TestUtility.DatabaseTools;

/// <summary>
/// Internal structure used when generating classes
/// </summary>
public struct DatabaseMappedClassPropertyDefinition
{
    /// <summary>
    /// Gets or sets the name of the field
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the type of the field
    /// </summary>
    /// <value>
    /// The type of the mapped.
    /// </value>
    public Type? MappedType { get; set; }

    /// <summary>
    /// Gets or sets the type of the field in the database
    /// </summary>
    /// <value>
    /// The type of field in the database
    /// </value>
    public string SourceType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this field is nullable.
    /// </summary>
    /// <value>
    /// True if this object is nullable, false if not.
    /// </value>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Gets or sets the maximum size.
    /// </summary>
    /// <value>
    /// The maximum size of the field
    /// </value>
    public int MaxSize { get; set; }

    /// <summary>
    /// Gets or sets the precision.
    /// </summary>
    /// <value>
    /// The precision of the field
    /// </value>
    public byte Precision { get; set; }

    /// <summary>
    /// Gets or sets the scale of the field
    /// </summary>
    /// <value>
    /// The scale.
    /// </value>
    public byte Scale { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this object is computed.
    /// </summary>
    /// <value>
    /// True if this object is computed, false if not.
    /// </value>
    public bool IsComputed { get; set; }


}