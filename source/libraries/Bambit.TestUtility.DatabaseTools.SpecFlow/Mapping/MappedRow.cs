using System.Collections;
using System.Data;
using TechTalk.SpecFlow;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;

public class MappedRow: IDictionary<string, string?>
{

    #region Private Fields

    private MappedTable Owner { get; }
    
    public bool ContainsKey(string key)
    {
        return HasColumn(key);
    }


    ICollection<string?> IDictionary<string, string?>.Values => Items;

    private string?[] Items { get; } = null!;


    #endregion Private Fields

    #region Ctors

    protected MappedRow(MappedTable owner)
    {
        Owner = owner;
        
    }

    public MappedRow(MappedTable owner, TableRow row) : this(owner)
    {
        Items = row.Values.ToArray();
    }

    public MappedRow(MappedTable owner, IDataReader reader) : this(owner)
    {
        Items = Enumerable.Range(0, reader.FieldCount).Select(c =>
  
        {
            object value = reader.GetValue(c);
            return value == DBNull.Value
                ? null
                : value.ToString();
        }).ToArray();
    }

    #endregion Ctors

    #region Methods
  

    #region Public Methods

    

    public int ColumnIndex(string name)
    {
        return Owner.IndexOfHeader(name);
    }

    public object GetDbValue(int index, string nullStringIdentifier)
    {

        ColumnDescription columnDescription = Owner.TableColumns[index];
        string? value = Items[columnDescription.ColumnIndex];

        if (value==null|| string.Compare(value, nullStringIdentifier,StringComparison.CurrentCultureIgnoreCase) == 0)
            return DBNull.Value;

        switch (columnDescription.ColumnType)
        {
            case "date":
                return AutoAssigner.ParseDateExtended(value);
            case "quoted":
                int length = value.Length;
                if (length < 2)
                    break;
                if (value[0] == '\'' && value[length - 1] == '\'' ||
                    value[0] == '"' && value[length - 1] == '"')
                    return value.Substring(1, length - 2);
                break;
        }

        return value;

    }

    public object GetDbValue(string name, string nullStringIdentifier)
    {
        ColumnDescription? columnDescription = Owner.GetColumnDescription(name);
        if (columnDescription == null)
            return DBNull.Value;
        string? value = Items[columnDescription.ColumnIndex];
        if (value==null|| string.Compare(value, nullStringIdentifier,StringComparison.CurrentCultureIgnoreCase) == 0)
            return DBNull.Value;
        switch (columnDescription.ColumnType)
        {
            case "date":
                return AutoAssigner.ParseDateExtended(value);
            case "quoted":
                int length = value.Length;
                if (length < 2)
                    break;
                if (value[0] == '\'' && value[length - 1] == '\'' ||
                    value[0] == '"' && value[length - 1] == '"')
                    return value.Substring(1, length - 2);
                break;
        }

        return value;

    }
    
    public object?[] GetDbValues(string nullStringIdentifier)
    {
        object?[] results = new object? [Values.Count];
        for (int x = 0; x < Values.Count; x++)
        {
            results[x]= GetDbValue(x, nullStringIdentifier);
        }
        return results;

    }
    public string? GetString(string name, string nullStringIdentifier)
    {
       
        int index = ColumnIndex(name);
        return GetString(index, nullStringIdentifier);

    }
    public string? GetString(int index, string nullStringIdentifier)
    {
       
        string? value = index == -1 ? null : Items[index];
        if (string.Compare(value, nullStringIdentifier,StringComparison.CurrentCultureIgnoreCase) == 0)
            return null;
        return value;

    }
    public bool HasColumn(string name)
    {
        return ColumnIndex(name) > -1;
    }

    #endregion Public Methods

    #endregion Methods

    public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
    {
        int itemIndex = 0;
        foreach (string header in Owner.Columns)
        {
            yield return new KeyValuePair<string, string?>(header, Items[itemIndex]);
            itemIndex++;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<string, string?> item)
    {
        throw new NotImplementedException("Cannot modify table structure");
    }
    
    public void Add(string key,string? value)
    {
        throw new NotImplementedException("Cannot modify table structure");
    }

    public void Clear()
    {
        throw new NotImplementedException("Cannot modify table structure");
    }

    public bool Contains(KeyValuePair<string, string?> item)
    {
       
        int keyIndex = ColumnIndex(item.Key);
        if (keyIndex < 0)
            return false;
        return Items[keyIndex]?.Equals(item.Value)??false;
    }

    public void CopyTo(KeyValuePair<string, string?>[] array, int arrayIndex)
    {
        throw new NotImplementedException("Cannot modify table structure");
    }

    public bool Remove(KeyValuePair<string, string?> item)
    {
        throw new NotImplementedException("Cannot modify table structure");
    }
    
    public int Count => Items.Length;
    public bool IsReadOnly => false;

    public bool Remove(string key)
    {
        throw new NotImplementedException("Cannot modify table structure");
    }

    public bool TryGetValue(string key, out string? value)
    {
        int keyIndex = ColumnIndex(key);
        if (keyIndex < 0)
        {
            value = null!;
            return false;
        }

        value = Items[keyIndex];
        return true;
    }
    

    public string? this[string header]
    {
        get
        {
            int itemIndex = ColumnIndex(header);
            return Items[itemIndex];
        }
        set
        {
            int keyIndex = ColumnIndex(header);
            if(keyIndex < 0)
                throw new IndexOutOfRangeException($"Columns '{header}' not in collection, table structure can not be modified");
            Items[keyIndex] = value;
        }
    }
    public string? this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }

    public ICollection<string> Keys => Owner.Columns;

    public ICollection<string?> Values => Items;
}