using System.Data;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;

public class MappedTable
{
    #region Static and private props
    public static Regex CleanRegex = new("[^a-zA-Z@]");
    private MappedRow[] TableRows { get; }

    #endregion Static and private props

    #region Ctors
    public MappedTable(IDataReader dataReader)
    {

        TableColumns = Enumerable.Range(0, dataReader.FieldCount).Select(c =>
        {

            string? columnType = ParseDatabaseType(dataReader.GetDataTypeName(c));
            string rawName = dataReader.GetName(c);
            string columnName = rawName.ToLower();
            string cleanedName = NormalizeName(rawName);

            ColumnDescription cd = new(columnName, cleanedName, columnType, c);
            return cd;
        }).ToArray();
        List<MappedRow> row = new List<MappedRow>();
        while (dataReader.Read())
        {
            row.Add(new MappedRow(this, dataReader));
        }

        TableRows = row.ToArray();

    }
    public MappedTable(Table sourceTable)
    {
        int index = 0;
        TableColumns = sourceTable.Header.Select(c =>
        {
            c = c.Trim();
            string? columnType = ParseType(c);
            if (columnType != null)
            {
                c = c.Substring(0, c.IndexOf("@", StringComparison.Ordinal));

                c = c.Trim();
            }

            string columnName = c.ToLower();
            string cleanedName = NormalizeName(c);

            ColumnDescription cd = new(columnName, cleanedName, columnType, index);
            index++;
            return cd;
        }).ToArray();
        TableRows = sourceTable.Rows.Select(r => new MappedRow(this, r)).ToArray();


    }
    #endregion Ctors

    #region Properties

    public string[] Columns => TableColumns.Select(a => a.ColumnName).ToArray();

    public ColumnDescription[] TableColumns { get; }

    public ICollection<MappedRow> Rows => TableRows;

    #endregion Properties

    #region Public methods

    public ColumnDescription? GetColumnDescription(string name)
    {
        return TableColumns.FirstOrDefault(d =>
            string.Compare(d.ColumnName, name, StringComparison.CurrentCultureIgnoreCase) == 0);
    }
    public int IndexOfHeader(string name)
    {
        string normalizedName = NormalizeName(name);
        for (int x = 0; x < TableColumns.Length; x++)
        {
            if (TableColumns[x].ColumnName == normalizedName || TableColumns[x].CleanedName == normalizedName)
                return x;
        }

        return -1;
    }
    #endregion Public methods

    #region Private Methods

    private string NormalizeName(string input)
    {
        return CleanRegex.Replace(input.ToLower(), "");
    }

    private string? ParseType(string input)
    {
        int indexOf = input.IndexOf("@", StringComparison.Ordinal);
        return indexOf > 0 ? input.Substring(indexOf + 1).Trim().ToLower() : null;
    }
    private string? ParseDatabaseType(string input)
    {
        switch (input)
        {
            case "datetime":
            case "date":
                return "date";
            case "bit":
            case "boolean":
                return "boolean";
        }

        return null;
    }

    #endregion Private Methods

}