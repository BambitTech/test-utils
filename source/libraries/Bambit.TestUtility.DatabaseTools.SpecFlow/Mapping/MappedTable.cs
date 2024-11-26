
using System.Data;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   A mapped table. </summary>
///
/// <remarks>   Law Metzler, 7/25/2024. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public partial class MappedTable
{
    #region Static and private props
    /// <summary>   The clean RegEx. </summary>
    public static readonly Regex CleanRegex = AlphaRegEx();

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets the table rows. </summary>
    ///
    /// <value> The table rows. </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private MappedRow[] TableRows { get; }

    #endregion Static and private props

    #region Ctors

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Constructor. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="dataReader">   The data reader. </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

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
        List<MappedRow> row = [];
        while (dataReader.Read())
        {
            row.Add(new MappedRow(this, dataReader));
        }

        TableRows = [.. row];

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Constructor. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="sourceTable">  Source table. </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public MappedTable(Table sourceTable)
    {
        int index = 0;
        TableColumns = sourceTable.Header.Select(c =>
        {
            c = c.Trim();
            string? columnType = ParseType(c);
            if (columnType != null)
            {
                c = c[..c.IndexOf('@', StringComparison.Ordinal)];

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets the columns. </summary>
    ///
    /// <value> The columns. </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public string[] Columns => TableColumns.Select(a => a.ColumnName).ToArray();

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets the table columns. </summary>
    ///
    /// <value> The table columns. </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public ColumnDescription[] TableColumns { get; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets the rows. </summary>
    ///
    /// <value> The rows. </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public ICollection<MappedRow> Rows => TableRows;

    #endregion Properties

    #region Public methods

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets column description. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="name"> The name. </param>
    ///
    /// <returns>   The column description. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public ColumnDescription? GetColumnDescription(string name)
    {
        return TableColumns.FirstOrDefault(d =>
            string.Compare(d.ColumnName, name, StringComparison.CurrentCultureIgnoreCase) == 0);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Searches for the first header. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="name"> The name. </param>
    ///
    /// <returns>   The zero-based index of the found header, or -1 if no match was found. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Normalize name. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="input">    The input. </param>
    ///
    /// <returns>   A string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private static string NormalizeName(string input)
    {
        return CleanRegex.Replace(input.ToLower(), "");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Parse type. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="input">    The input. </param>
    ///
    /// <returns>   A string? </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private static string? ParseType(string input)
    {
        int indexOf = input.IndexOf('@', StringComparison.Ordinal);
        return indexOf > 0 ? input[(indexOf + 1)..].Trim().ToLower() : null;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Parse database type. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="input">    The input. </param>
    ///
    /// <returns>   A string? </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private static string? ParseDatabaseType(string input)
    {
        return input switch
        {
            "datetime" or "date" => "date",
            "bit" or "boolean" => "boolean",
            "char" or "byte"=>"byte",
            _ => input
        };
    }

    [GeneratedRegex("[^a-zA-Z@0-9]")]
    private static partial Regex AlphaRegEx();

    #endregion Private Methods

}