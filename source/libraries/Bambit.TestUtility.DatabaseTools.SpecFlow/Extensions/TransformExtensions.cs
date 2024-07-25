namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Extensions
{
    public static class TransformExtensions
    {   
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
}
