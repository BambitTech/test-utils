namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;

public class ColumnDescription(string name, string cleanedName, string? type, int index)
{
    public string ColumnName { get; } = name;
    public string? ColumnType { get; private set; } = type;
    public string CleanedName { get; private set; } = cleanedName;
    public int ColumnIndex { get; private set; } = index;

    public override string ToString()
    {
        return ColumnName;
    }
}