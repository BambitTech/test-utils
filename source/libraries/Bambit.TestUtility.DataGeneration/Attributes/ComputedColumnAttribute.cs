using System;

namespace Bambit.TestUtility.DatabaseTools.Attributes;

/// <summary>
/// Marks a property as a computed column. A computed column will not be assigned a value.
/// </summary>
/// <remarks>
/// Primarily used when the value will be stored in a database and should not be assigned.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class ComputedColumnAttribute : Attribute
{
}