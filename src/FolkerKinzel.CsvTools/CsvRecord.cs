using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary>Represents a record of the CSV file (row). The column order corresponds
/// to that of the CSV file and all columns are of the <see cref="Type"/>&#160;
/// <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;char&gt;</see>. The content of the
/// columns can be accessed using a zero-based column index
/// or the column name.</summary>
public sealed class CsvRecord : IEnumerable<KeyValuePair<string, ReadOnlyMemory<char>>>
{
    private readonly ReadOnlyMemory<char>[] _values = [];
    private readonly Dictionary<string, int> _lookupDictionary;

    /// <summary> Initialisiert aus <paramref name="source" />, das als Vorlage dient,
    /// ein neues <see cref="CsvRecord" />-Objekt, das die unveränderlichen Teile der
    /// Vorlage referenziert bzw. kopiert. (Ctor, der von <see cref="CsvReader" /> verwendet
    /// wird.) </summary>
    /// <param name="source"> <see cref="CsvRecord" />-Objekt, das als Vorlage dient.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CsvRecord(CsvRecord source)
    {
        _lookupDictionary = source._lookupDictionary;
        Identifier = source.Identifier;
        ColumnNames = source.ColumnNames;
        _values = new ReadOnlyMemory<char>[Count];
    }

    /// <summary> Initialisiert ein <see cref="CsvRecord" />-Objekt mit Standardnamen
    /// für die Spalten ("Column1", "Column2" etc). (Geeignet für CSV-Dateien ohne Kopfzeile.)
    /// </summary>
    /// <param name="columnsCount">Anzahl der Spalten.</param>
    internal CsvRecord(int columnsCount)
    {
        IEqualityComparer<string> comparer = StringComparer.OrdinalIgnoreCase;

        this._lookupDictionary = new Dictionary<string, int>(columnsCount, comparer);
        Identifier = _lookupDictionary.GetHashCode();

        var columnNames = new string[columnsCount];

        for (int i = 0; i < columnsCount; i++)
        {
            string keyName = AutoColumnName.Create(i);
            this._lookupDictionary.Add(keyName, i);
            columnNames[i] = keyName;
        }

        ColumnNames = new ReadOnlyCollection<string>(columnNames);
        _values = new ReadOnlyMemory<char>[columnsCount];
    }

    /// <summary> Initialisiert ein neues <see cref="CsvRecord" />-Objekt mit Spaltennamen.
    /// (Geeignet für CSV-Dateien mit Kopfzeile.) </summary>
    /// <param name="keys">Spaltennamen. Die Auflistung kann <c>null</c>-Werte enthalten:
    /// Diese werden dann durch Standardnamen ersetzt.</param>
    /// <param name="caseSensitive">Wenn <c>true</c>, werden die Spaltennamen case-sensitiv
    /// behandelt.</param>
    /// <param name="trimColumns">Wenn <c>true</c>, werden alle Spaltennamen mit der
    /// Methode <see cref="string.Trim()" /> behandelt.</param>
    /// <param name="initArr">Wenn <c>false</c>, wird das Datenarray nicht initialisiert.
    /// Das Objekt taugt dann nur als Kopierschablone für weitere <see cref="CsvRecord"
    /// />-Objekte. (Wird von <see cref="CsvReader" /> verwendet.</param>
    /// <param name="throwException">Wenn <c>true</c>, wird eine <see cref="ArgumentException"
    /// /> geworfen, wenn <paramref name="keys" /> 2 identische Spaltennamen enthält.
    /// Beim Lesen einer Datei sollte der Parameter auf <c>false</c> gesetzt werden,
    /// um die Spaltennamen automatisch so abzuwandeln, dass sie eindeutig sind.</param>
    /// <exception cref="ArgumentException">Ein Spaltenname war bereits im Dictionary
    /// enthalten. Die Exception wird nur dann geworfen, wenn <paramref name="throwException"
    /// /><c>true</c> ist.</exception>
    internal CsvRecord(string?[] keys, bool caseSensitive, bool trimColumns, bool initArr, bool throwException)
    {
        Debug.Assert(keys != null);

        IEqualityComparer<string> comparer =
            caseSensitive ?
            StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

        this._lookupDictionary = new Dictionary<string, int>(keys.Length, comparer);
        Identifier = _lookupDictionary.GetHashCode();

        int defaultNameCounter = 0;

        for (int i = 0; i < keys.Length; i++)
        {
            string? key = keys[i];

            if (key is null)
            {
                key = GetDefaultName();
            }
            else
            {
                if (trimColumns)
                {
                    key = key.Trim();
                }

                if (!throwException && _lookupDictionary.ContainsKey(key))
                {
                    key = MakeUnique(key);
                }
            }

            _lookupDictionary.Add(key, i);
            keys[i] = key;
        }//for

        ColumnNames = keys!;

        if (initArr)
        {
            _values = new ReadOnlyMemory<char>[Count];
        }

        ///////////////////////////////////////////////////

        string GetDefaultName()
        {
            string key;

            do
            {
                key = AutoColumnName.Create(defaultNameCounter++);
            } while (_lookupDictionary.ContainsKey(key));

            return key;
        }

        //////////////////////////////////////////////////////

        string MakeUnique(string key)
        {
            string unique;
            int cnt = 1;

            do
            {
                unique = key + (++cnt).ToString(CultureInfo.InvariantCulture);
            } while (_lookupDictionary.ContainsKey(unique));

            return unique;
        }
    }

    /// <summary>Gets or sets the value of the column in the CSV file that is at the
    /// specified index.</summary>
    /// <param name="index">The zero-based index of the column in the CSV file whose
    /// value is being obtained or set.</param>
    /// <returns>The item at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="index" /> is
    /// less than Zero or greater or equal than <see cref="Count" />.</exception>
    public ReadOnlyMemory<char> this[int index]
    {
        get => _values[index];
        set => _values[index] = value;
    }

    /// <summary>Gets or sets the value associated with the specified column name in
    /// the CSV file.</summary>
    /// <param name="columnName">The column name of the CSV file.</param>
    /// <returns>The value associated with the specified column name.</returns>
    /// <exception cref="KeyNotFoundException">The column name specified with <paramref
    /// name="columnName" /> does not exist.</exception>
    /// <exception cref="ArgumentNullException"> <paramref name="columnName" /> is <c>null</c>.</exception>
    public ReadOnlyMemory<char> this[string columnName]
    {
        get => _values[_lookupDictionary[columnName]];
        set => _values[_lookupDictionary[columnName]] = value;
    }

    /// <summary>Returns the number of columns in the <see cref="CsvRecord" /> instance.</summary>
    public int Count => _lookupDictionary.Count;

    /// <summary>Gets whether or not the instance contains usable data.</summary>
    /// <value>
    /// <c>true</c> if <see cref="Count" /> is Zero or if all fields are
    /// <see cref="ReadOnlyMemory{T}.Empty"/>, otherwise <c>false</c>.
    /// </value>
    public bool IsEmpty => Count == 0 || _values.All(x => x.IsEmpty);

    /// <summary>Gets the column names.</summary>
    /// <remarks>If the CSV file did not have a header, column names of the type "Column1",
    /// "Column2" etc. are automatically assigned.</remarks>
    public IReadOnlyList<string> ColumnNames { get; }

    /// <summary> Gets the list of data stored in <see cref="CsvRecord" />. The values 
    /// can be changed.</summary>
    public IList<ReadOnlyMemory<char>> Values => _values;

    /// <summary> Returns a copy of the data stored in <see cref="CsvRecord" /> as <see cref="Dictionary{TKey,
    /// TValue}">Dictionary&lt;string, ReadOnlyMemory&lt;char&gt;&gt;</see>, 
    /// which uses the same <see cref="StringComparer" /> for key comparison that was used to create the 
    /// <see cref="CsvRecord" /> instance.
    ///</summary>
    /// <returns>A copy of the data stored in <see cref="CsvRecord" />.</returns>
    public Dictionary<string, ReadOnlyMemory<char>> ToDictionary()
    {
#if NETSTANDARD2_0 || NET462
        var dic = new Dictionary<string, ReadOnlyMemory<char>>(this.Count, this._lookupDictionary.Comparer);

        foreach (KeyValuePair<string, ReadOnlyMemory<char>> kvp in this)
        {
            dic.Add(kvp.Key, kvp.Value);
        }

        return dic;
#else
        return new Dictionary<string, ReadOnlyMemory<char>>(this, this._lookupDictionary.Comparer);
#endif
    }

    /// <summary> A hash code that is identical for all <see cref="CsvRecord" /> objects belonging to 
    /// the same read or write operation. </summary>
    /// <remarks>Used by the package FolkerKinzel.CsvTools.TypeConversions.</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int Identifier { get; }

    /// <summary> Gets the comparer used to select the keys.</summary>
    /// <remarks>Used by the package FolkerKinzel.CsvTools.TypeConversions.</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public IEqualityComparer<string> Comparer => _lookupDictionary.Comparer;

    /// <summary>Sets all data fields of <see cref="CsvRecord" /> to <c>null</c>.</summary>
    public void Clear() => Array.Clear(_values, 0, _values.Length);

    /// <summary>Tries to get the value associated with the specified column name in the CSV
    /// file.</summary>
    /// <param name="columnName">The column name in the CSV file whose value is being
    /// obtained.</param>
    /// <param name="value">After the method has finished, the argument contains the
    /// value associated with the column name specified with <paramref name="columnName"
    /// /> if the key was found, or <c>null</c> otherwise. The parameter is passed uninitialized.</param>
    /// <returns> <c>true</c> if it contains a column name with the value of <paramref
    /// name="columnName" />, otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"> <paramref name="columnName" /> is <c>null</c>.</exception>
    public bool TryGetValue(string columnName, out ReadOnlyMemory<char> value)
    {
        if (columnName is null)
        {
            throw new ArgumentNullException(columnName);
        }

        if (_lookupDictionary.TryGetValue(columnName, out int index))
        {
            value = _values[index];
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    /// <summary>Tries to get the value associated with the specified column index of the CSV
    /// file.</summary>
    /// <param name="columnIndex">Zero-based index of the data column of the CSV file.</param>
    /// <param name="value">When this method returns, the parameter contains the value
    /// assigned to the column index specified with <paramref name="columnIndex" />
    /// if the column index exists, or <c>null</c> otherwise. This parameter is passed
    /// uninitialized.</param>
    /// <returns> <c>true</c> if a column index with the value of <paramref name="columnIndex"
    /// /> exists in the CSV file, otherwise <c>false</c>.</returns>
    public bool TryGetValue(int columnIndex, out ReadOnlyMemory<char> value)
    {
        if (columnIndex >= 0 && columnIndex < Count)
        {
            value = _values[columnIndex];
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    /// <summary> Fills the <see cref="CsvRecord" /> instance with the contents of a 
    /// <see cref="string"/> collection. The collection may contain <c>null</c> values.
    /// </summary>
    /// <param name="data">The contents with which to populate the <see cref="CsvRecord" /> instance.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more entries than <see cref="CsvRecord.Count" />.</exception>
    /// <remarks>If <paramref name="data" /> has fewer entries than <see cref="CsvRecord.Count" />, 
    /// the remaining fields of <see cref="CsvRecord" /> are filled with 
    /// <see cref="ReadOnlyMemory{T}.Empty" /> values.</remarks>
    public void Fill(IEnumerable<string?> data)
    {
        _ArgumentNullException.ThrowIfNull(data, nameof(data));

        Fill(data.Select(x => x.AsMemory()));
    }

    /// <summary> Fills the <see cref="CsvRecord" /> instance with the contents of a 
    /// <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> collection.
    /// </summary>
    /// <param name="data">The contents with which to populate the <see cref="CsvRecord" /> instance.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more entries than <see cref="CsvRecord.Count" />.</exception>
    /// <remarks>If <paramref name="data" /> has fewer entries than <see cref="CsvRecord.Count" />, 
    /// the remaining fields of <see cref="CsvRecord" /> are filled with 
    /// <see cref="ReadOnlyMemory{T}.Empty" /> values.</remarks>
    public void Fill(IEnumerable<ReadOnlyMemory<char>> data)
    {
        _ArgumentNullException.ThrowIfNull(data, nameof(data));
        
        int i = 0;

        foreach (ReadOnlyMemory<char> item in data)
        {
            if (i >= _values.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            _values[i++] = item;
        }

        for (int j = i; j < _values.Length; j++)
        {
            _values[j] = default;
        }
    }

    /// <summary>Determines whether the <see cref="CsvRecord" /> object contains a column
    /// with the specified column name.</summary>
    /// <param name="columnName">The column name of the CSV file column to search for.</param>
    /// <returns> <c>true</c> if <paramref name="columnName" /> is one of the column
    /// names of the <see cref="CsvRecord" /> object.</returns>
    /// <exception cref="ArgumentNullException"> <paramref name="columnName" /> is <c>null</c>.</exception>
    public bool ContainsColumn(string columnName)
    {
        return columnName is null
            ? throw new ArgumentNullException(nameof(columnName))
            : _lookupDictionary.ContainsKey(columnName);
    }

    /// <summary>Returns the zero-based index of the column in the CSV file, that has
    /// the specified column name, or -1, if <paramref name="columnName" /> is not one
    /// of the column names in the CSV file.</summary>
    /// <param name="columnName">The column name to be checked for the index of <see
    /// cref="CsvRecord" /> that it refers to.</param>
    /// <returns>The zero-based index of the column in the CSV file with the column
    /// name <paramref name="columnName" /> or -1, if <paramref name="columnName" />
    /// is not one of the column names in the CSV file.</returns>
    public int IndexOfColumn(string? columnName) => columnName != null && _lookupDictionary.TryGetValue(columnName, out int i) ? i : -1;

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Returns an <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string,
    /// string?&gt;&gt;</see> to iterate over the <see cref="CsvRecord" /> object. The
    /// <see cref="KeyValuePair{TKey, TValue}">KeyValuePair&lt;string, string?&gt;</see>
    /// contains the column name as <see cref="KeyValuePair{TKey, TValue}.Key" /> and
    /// the content of the column as <see cref="KeyValuePair{TKey, TValue}.Value" />.</summary>
    /// <returns>An <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string,
    /// string?&gt;&gt;</see>.</returns>
    public IEnumerator<KeyValuePair<string, ReadOnlyMemory<char>>> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return new KeyValuePair<string, ReadOnlyMemory<char>>(ColumnNames[i], Values[i]);
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (this.Count == 0)
        {
            return base.ToString() ?? string.Empty;
        }

        var sb = new StringBuilder();

        foreach (var key in ColumnNames)
        {
            _ = sb.Append(key).Append(": ").Append(this[key]).Append(", ");
        }

        if (sb.Length >= 2)
        {
            sb.Length -= 2;
        }

        return sb.ToString();
    }
}
