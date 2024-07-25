using Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;
using TechTalk.SpecFlow;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Transforms
{ 
    [Binding]
    public static class TableTransformations
    {
        [StepArgumentTransformation]
        public static MappedTable DatabaseMappedTableTransform(Table sourceTable)
        {
            return  new MappedTable(sourceTable);
        }

        public static string[] AssignToObject(this MappedRow tableRow, object result)
        {
            Config.Randomizer.InitializeObject(result);
            return tableRow.AssignValuesIfDefined(result);
            
        }
        
        public static object?[] ExtractValues(this MappedRow tableRow, string nullStringIdentifier, Func<string?, string?> variableReplaceFunc)
        {
            return tableRow.GetDbValues(nullStringIdentifier);
            
        }

        public static void TransformForNull(this MappedRow mappedRow, string nullIndicator)
        {
            foreach (KeyValuePair<string, string?> pair in mappedRow)
            {
                mappedRow[pair.Key] = mappedRow.GetString(pair.Key, nullIndicator);
            }
        }

    }
}
