
using System.Collections;
using System.Data;
using TechTalk.SpecFlow;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Mapping;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   A mapped row. </summary>
///
/// <remarks>   Law Metzler, 7/25/2024. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public class MappedRow: IDictionary<string, string?>
{

    #region Private Fields

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets the owner. </summary>
    ///
    /// <value> The owner. </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private MappedTable Owner { get; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an
    /// element with the specified key.
    /// </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="key">  The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
    ///
    /// <returns>
    /// <see langword="true" /> if the <see cref="T:System.Collections.Generic.IDictionary`2" />
    /// contains an element with the key; otherwise, <see langword="false" />.
    /// </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public bool ContainsKey(string key)
    {
        return HasColumn(key);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in
    /// the <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </summary>
    ///
    /// <value>
    /// An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the
    /// object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    ICollection<string?> IDictionary<string, string?>.Values => Items;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets the items. </summary>
    ///
    /// <value> The items. </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private string?[] Items { get; } = null!;


    #endregion Private Fields

    #region Ctors

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Specialized constructor for use only by derived class. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="owner">    The owner. </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    protected MappedRow(MappedTable owner)
    {
        Owner = owner;
        
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Constructor. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="owner">    The owner. </param>
    /// <param name="row">      The row. </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public MappedRow(MappedTable owner, TableRow row) : this(owner)
    {
        Items = [.. row.Values];
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Constructor. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="owner">    The owner. </param>
    /// <param name="reader">   The reader. </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Column index. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="name"> The name. </param>
    ///
    /// <returns>   An int. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public int ColumnIndex(string name)
    {
        return Owner.IndexOfHeader(name);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets database value. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="index">                Zero-based index of the. </param>
    /// <param name="nullStringIdentifier"> Identifier for the null string. </param>
    ///
    /// <returns>   The database value. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public object GetDbValue(int index, string nullStringIdentifier, ITestDbConnection dbConnection)
    {

        ColumnDescription columnDescription = Owner.TableColumns[index];   
        return GetDbValue(columnDescription, nullStringIdentifier, dbConnection);


    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets database value. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="name">                 The name. </param>
    /// <param name="nullStringIdentifier"> Identifier for the null string. </param>
    /// <param name="dbConnection">         The <see cref="ITestDbConnection"/> to get the db value for</param>
    ///
    /// <returns>   The database value. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public object GetDbValue(string name, string nullStringIdentifier, ITestDbConnection dbConnection)
    {
        ColumnDescription? columnDescription = Owner.GetColumnDescription(name);
        if (columnDescription == null)
            return DBNull.Value;
        return GetDbValue(columnDescription, nullStringIdentifier, dbConnection);


    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets database value. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="columnDescription">    The <see cref="ColumnDescription"/> of the column to get the values for. </param>
    /// <param name="nullStringIdentifier"> Identifier for the null string. </param>
    /// <param name="dbConnection">         The <see cref="ITestDbConnection"/> to get the db value for</param>
    ///
    /// <returns>   The database value. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    protected object GetDbValue(ColumnDescription columnDescription , string nullStringIdentifier, ITestDbConnection dbConnection)
    {
        string? value = Items[columnDescription.ColumnIndex];
        if (value==null|| string.Compare(value, nullStringIdentifier,StringComparison.CurrentCultureIgnoreCase) == 0)
            return DBNull.Value;
        if(!string.IsNullOrEmpty( columnDescription.ColumnType))
            return dbConnection.ConvertValue( value,columnDescription.ColumnType);
        return value;
       

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets database values. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="nullStringIdentifier"> Identifier for the null string. </param>
    ///
    /// <returns>   An array of object? </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public object?[] GetDbValues(string nullStringIdentifier, ITestDbConnection dbConnection)
    {
        object?[] results = new object? [Values.Count];
        for (int x = 0; x < Values.Count; x++)
        {
            results[x]= GetDbValue(x, nullStringIdentifier,dbConnection);
        }
        return results;

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets a string. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="name">                 The name. </param>
    /// <param name="nullStringIdentifier"> Identifier for the null string. </param>
    ///
    /// <returns>   The string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public string? GetString(string name, string nullStringIdentifier)
    {
       
        int index = ColumnIndex(name);
        return GetString(index, nullStringIdentifier);

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets a string. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="index">                Zero-based index of the. </param>
    /// <param name="nullStringIdentifier"> Identifier for the null string. </param>
    ///
    /// <returns>   The string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public string? GetString(int index, string nullStringIdentifier)
    {
       
        string? value = index == -1 ? null : Items[index];
        if (string.Compare(value, nullStringIdentifier,StringComparison.CurrentCultureIgnoreCase) == 0)
            return null;
        return value;

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Query if 'name' has column. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="name"> The name. </param>
    ///
    /// <returns>   True if column, false if not. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public bool HasColumn(string name)
    {
        return ColumnIndex(name) > -1;
    }

    #endregion Public Methods

    #endregion Methods

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Returns an enumerator that iterates through the collection. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <returns>   An enumerator that can be used to iterate through the collection. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
    {
        int itemIndex = 0;
        foreach (string header in Owner.Columns)
        {
            yield return new KeyValuePair<string, string?>(header, Items[itemIndex]);
            itemIndex++;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Returns an enumerator that iterates through a collection. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through
    /// the collection.
    /// </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="item"> The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void Add(KeyValuePair<string, string?> item)
    {
        throw new NotImplementedException("Cannot modify table structure");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="key">      The object to use as the key of the element to add. </param>
    /// <param name="value">    The object to use as the value of the element to add. </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void Add(string key,string? value)
    {
        throw new NotImplementedException("Cannot modify table structure");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void Clear()
    {
        throw new NotImplementedException("Cannot modify table structure");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a
    /// specific value.
    /// </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="item"> The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    ///
    /// <returns>
    /// <see langword="true" /> if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />
    /// ; otherwise, <see langword="false" />.
    /// </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public bool Contains(KeyValuePair<string, string?> item)
    {
       
        int keyIndex = ColumnIndex(item.Key);
        if (keyIndex < 0)
            return false;
        return Items[keyIndex]?.Equals(item.Value)??false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />,
    /// starting at a particular <see cref="T:System.Array" /> index.
    /// </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="array">        The one-dimensional <see cref="T:System.Array" /> that is the
    ///                             destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />.
    ///                             The <see cref="T:System.Array" /> must have zero-based indexing. 
    /// </param>
    /// <param name="arrayIndex">   The zero-based index in <paramref name="array" /> at which
    ///                             copying begins. </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public void CopyTo(KeyValuePair<string, string?>[] array, int arrayIndex)
    {
        throw new NotImplementedException("Cannot modify table structure");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="item"> The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    ///
    /// <returns>
    /// <see langword="true" /> if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />
    /// ; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="item" />
    /// is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public bool Remove(KeyValuePair<string, string?> item)
    {
        throw new NotImplementedException("Cannot modify table structure");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </summary>
    ///
    /// <value>
    /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
    /// </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public int Count => Items.Length;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" />
    /// is read-only.
    /// </summary>
    ///
    /// <value>
    /// <see langword="true" /> if the <see cref="T:System.Collections.Generic.ICollection`1" /> is
    /// read-only; otherwise, <see langword="false" />.
    /// </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public bool IsReadOnly => false;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="key">  The key of the element to remove. </param>
    ///
    /// <returns>
    /// <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.
    /// This method also returns <see langword="false" /> if <paramref name="key" /> was not found in
    /// the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public bool Remove(string key)
    {
        throw new NotImplementedException("Cannot modify table structure");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets the value associated with the specified key. </summary>
    ///
    /// <remarks>   Law Metzler, 7/25/2024. </remarks>
    ///
    /// <param name="key">      The key whose value to get. </param>
    /// <param name="value">    [out] When this method returns, the value associated with the
    ///                         specified key, if the key is found; otherwise, the default value for the
    ///                         type of the <paramref name="value" /> parameter. This parameter is passed
    ///                         uninitialized. </param>
    ///
    /// <returns>
    /// <see langword="true" /> if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />
    /// contains an element with the specified key; otherwise, <see langword="false" />.
    /// </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets or sets the element with the specified key. </summary>
    ///
    /// <exception cref="IndexOutOfRangeException"> Thrown when the index is outside the required
    ///                                             range. </exception>
    ///
    /// <param name="header">   The key of the element to get or set. </param>
    ///
    /// <returns>   The element with the specified key. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets or sets the element with the specified key. </summary>
    ///
    /// <param name="index">    The key of the element to get or set. </param>
    ///
    /// <returns>   The element with the specified key. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public string? this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </summary>
    ///
    /// <value>
    /// An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the
    /// object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public ICollection<string> Keys => Owner.Columns;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in
    /// the <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </summary>
    ///
    /// <value>
    /// An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the
    /// object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </value>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public ICollection<string?> Values => Items;
}