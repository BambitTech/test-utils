using Bambit.TestUtility.DatabaseTools.Reqnroll.Mapping;
using Reqnroll;

namespace Bambit.TestUtility.DatabaseTools.Reqnroll.Transforms;

/// <summary>
/// Extensions for transforming objects
/// </summary>
[Binding]
public static class Transformations
{
   
    /// <summary>
    /// Initializes a <see cref="DatabaseMappedClass"/> with random values and then applies any applicable fields from the <see cref="MappedRow"/>
    /// </summary>
    /// <param name="tableRow">The <see cref="MappedRow"/> containg the values to apply</param>
    /// <param name="mappedClass">The <see cref="DatabaseMappedClass"/> to apply the values to</param>
    /// <returns>An array of field names that were applied</returns>
    public static string[] AssignToObject(this MappedRow tableRow, DatabaseMappedClass mappedClass)
    {
        Config.Randomizer.InitializeObject(mappedClass);
        return tableRow.AssignValuesIfDefined(mappedClass);
            
    }
        

    /// <summary>
    /// Modifies the value in a <see cref="MappedRow"/>, setting the actual value to <c>null</c> if the string is the nullIndicator
    /// </summary>
    /// <param name="mappedRow">The mapped row to modify</param>
    /// <param name="nullIndicator">The string used to match nulls</param>
    public static void TransformForNull(this MappedRow mappedRow, string nullIndicator)
    {
        foreach (KeyValuePair<string, string?> pair in mappedRow)
        {
            mappedRow[pair.Key] = mappedRow.GetString(pair.Key, nullIndicator);
        }
    }
        
    /// <summary>
    /// Applies a function to transforms the values in a dictionary 
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IDictionary{TKey,TValue}"/> this is an extension of</typeparam>
    /// <param name="items">The <see cref="IDictionary{TKey,TValue}"/> to apply the transforms to</param>
    /// <param name="replaceVariable">The <see cref="Func{TKey,TResult}"/> to apply</param>
    /// <returns>The transformed items (allows chaining)</returns>
    public static T ApplyTransformValues<T>(this T items, Func<string?, string?> replaceVariable)
        where T:IDictionary<string,string?> 
    {
        foreach (KeyValuePair<string, string?> pair in items)
        {
            items[pair.Key] = pair.Value==null?null:replaceVariable(pair.Value);
                
        }
        return items;
    }
}