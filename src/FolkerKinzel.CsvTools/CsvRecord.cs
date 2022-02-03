using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary>
/// Repräsentiert einen Datensatz der CSV-Datei (Zeile). Die Spaltenreihenfolge entspricht der der CSV-Datei und alle Spalten sind vom Datentyp
/// <see cref="string"/>. Auf den Inhalt der Spalten kann per nullbasiertem Spaltenindex oder über die Spaltennamen zugegriffen werden.
/// </summary>
/// <remarks>
/// Mit der Klasse <see cref="CsvRecordWrapper"/> kann die Reihenfolge der Datenspalten zur Laufzeit auf die evtl. andere Spaltenreihenfolge 
/// einer <see cref="DataTable"/> gemappt werden und es können damit auch Typkonvertierungen durchgeführt werden.
/// </remarks>
public sealed class CsvRecord : IEnumerable<KeyValuePair<string, ReadOnlyMemory<char>>>
{
    #region fields

    private readonly ReadOnlyMemory<char>[] _values = Array.Empty<ReadOnlyMemory<char>>();

    private readonly Dictionary<string, int> _lookupDictionary;


    #endregion


    #region ctors

    /// <summary>
    /// Initialisiert aus <paramref name="source"/>, das als Vorlage dient, ein neues <see cref="CsvRecord"/>-Objekt,
    /// das die unveränderlichen Teile der Vorlage referenziert bzw. kopiert. (Ctor der von <see cref="CsvReader"/> verwendet wird.)
    /// </summary>
    /// <param name="source"><see cref="CsvRecord"/>-Objekt, das als Vorlage dient.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CsvRecord(CsvRecord source)
    {
        _lookupDictionary = source._lookupDictionary;
        Identifier = source.Identifier;
        ColumnNames = source.ColumnNames;
        _values = new ReadOnlyMemory<char>[Count];
    }


    /// <summary>
    /// Initialisiert ein <see cref="CsvRecord"/>-Objekt mit Standardnamen für die Spalten ("Column1", "Column2" etc). (Geeignet für
    /// CSV-Dateien ohne Kopfzeile.)
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


    /// <summary>
    /// Initialisiert ein neues <see cref="CsvRecord"/>-Objekt mit Spaltennamen. (Geeignet für
    /// CSV-Dateien mit Kopfzeile.)
    /// </summary>
    /// <param name="keys">Spaltennamen. Die Auflistung kann <c>null</c>-Werte enthalten: Diese werden dann durch 
    /// Standardnamen ersetzt.</param>
    /// <param name="caseSensitive">Wenn <c>true</c>, werden die Spaltennamen case-sensitiv behandelt.</param>
    /// <param name="trimColumns">Wenn <c>true</c>, werden alle Spaltennamen mit der Methode <see cref="string.Trim()"/> behandelt.</param>
    /// <param name="initArr">Wenn <c>false</c>, wird das Datenarray nicht initialisiert. Das Objekt taugt dann nur als Kopierschablone
    /// für weitere <see cref="CsvRecord"/>-Objekte. (Wird von <see cref="CsvReader"/> verwendet.</param>
    /// <param name="throwException">Wenn <c>true</c>, wird eine <see cref="ArgumentException"/> geworfen,
    /// wenn <paramref name="keys"/> 2 identische Spaltennamen enthält. Beim Lesen einer Datei sollte der 
    /// Parameter auf <c>false</c> gesetzt werden, um die Spaltennamen automatisch so abzuwandeln, dass sie eindeutig sind.</param>
    /// <exception cref="ArgumentException">Ein Spaltenname war bereits im Dictionary enthalten. Die Exception wird nur dann
    /// geworfen, wenn <paramref name="throwException"/> <c>true</c> ist.</exception>
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

        ColumnNames = new ReadOnlyCollection<string>(keys!);

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

    #endregion


    #region Properties

    /// <summary>
    /// Ruft den Wert der Spalte der CSV-Datei, die sich am angegebenen Index befindet, ab oder legt diesen fest.
    /// </summary>
    /// <param name="index">Der nullbasierte Index der Spalte der CSV-Datei, deren Wert abgerufen oder festgelegt wird.</param>
    /// <returns>Das Element am angegebenen Index.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder größer oder gleich <see cref="Count"/>.</exception>
    public ReadOnlyMemory<char> this[int index]
    {
        get => _values[index];
        set => _values[index] = value;
    }

    /// <summary>
    /// Ruft den Wert ab, der dem angegebenen Spaltennamen der CSV-Datei zugeordnet ist, oder legt diesen fest.
    /// </summary>
    /// <param name="columnName">Der Spaltenname der CSV-Datei.</param>
    /// <returns>Der dem angegebenen Schlüssel zugeordnete Wert.</returns>
    /// <exception cref="KeyNotFoundException">Der mit <paramref name="columnName"/> angegebene Spaltenname existiert nicht.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="columnName"/> ist <c>null</c>.</exception>
    public ReadOnlyMemory<char> this[string columnName]
    {
        get => _values[_lookupDictionary[columnName]];
        set => _values[_lookupDictionary[columnName]] = value;
    }


    /// <summary>
    /// Gibt die Anzahl der Spalten in <see cref="CsvRecord"/> zurück.
    /// </summary>
    public int Count => _lookupDictionary.Count;

    /// <summary>
    /// Gibt <c>true</c> zurück, wenn <see cref="Count"/> 0 ist oder wenn alle
    /// Felder den Wert <c>null</c> haben.
    /// </summary>
    public bool IsEmpty => Count == 0 || _values.All(x => x.IsEmpty);



    /// <summary>
    /// Gibt die in <see cref="CsvRecord"/> gespeicherten Spaltennamen zurück.
    /// </summary>
    /// <remarks>Wenn die CSV-Datei keine Kopfzeile hatte, werden automatisch Spaltennamen der Art "Column1", "Column2" etc. vergeben.</remarks>
    public ReadOnlyCollection<string> ColumnNames { get; }



    /// <summary>
    /// Gibt die <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see>-Collection der in <see cref="CsvRecord"/> gespeicherten Daten zurück. Die 
    /// Werte können verändert werden.
    /// </summary>
    public IList<ReadOnlyMemory<char>> Values => _values;



    /// <summary>
    /// Gibt eine Kopie der in <see cref="CsvRecord"/> gespeicherten Daten als <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string?&gt;</see> zurück, das
    /// für den Schlüsselvergleich denselben <see cref="StringComparer"/> verwendet, mit dem <see cref="CsvRecord"/> erstellt wurde.
    /// </summary>
    /// <returns>Eine Kopie der in <see cref="CsvRecord"/> gespeicherten Daten als <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string?&gt;</see>.</returns>
    public Dictionary<string, ReadOnlyMemory<char>> ToDictionary()
    {
#if NETSTANDARD2_0 || NET461
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



    /// <summary>
    /// Ein Hashcode, der für alle <see cref="CsvRecord"/>-Objekte, die zum selben Lese- oder Schreibvorgang
    /// gehören, identisch ist. (Wird von <see cref="CsvColumnNameProperty"/> verwendet, um festzustellen,
    /// ob der Zugriffsindex aktuell ist.)
    /// </summary>
    public int Identifier { get; }


    /// <summary>
    /// Der zur Auswahl der Schlüssel verwendete Comparer.
    /// </summary>
    public IEqualityComparer<string> Comparer => _lookupDictionary.Comparer;

    #endregion

    /// <summary>
    /// Setzt alle Datenfelder von <see cref="CsvRecord"/> auf <c>null</c>.
    /// </summary>
    public void Clear() => Array.Clear(_values, 0, _values.Length);



    /// <summary>
    /// Ruft den Wert ab, der dem angegebenen Spaltennamen der CSV-Datei zugeordnet ist.
    /// </summary>
    /// <param name="columnName">Der Name der Spalte der CSV-Datei, deren Wert abgerufen wird.</param>
    /// <param name="value">Enthält nach dem Beenden dieser Methode den Wert, der dem mit  <paramref name="columnName"/> angegebenen Spaltennamen
    /// zugeordnet ist, wenn der Schlüssel gefunden wurde, oder andernfalls <c>null</c>. Dieser Parameter wird nicht
    /// initialisiert übergeben.</param>
    /// <returns><c>true</c>, wenn ein Spaltenname mit dem Wert von <paramref name="columnName"/> enthalten ist.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="columnName"/> ist <c>null</c>.</exception>
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

    /// <summary>
    /// Ruft den Wert ab, der dem angegebenen Spaltenindex der CSV-Datei zugeordnet ist.
    /// </summary>
    /// <param name="columnIndex">Nullbasierter Index der Datenspalte der CSV-Datei.</param>
    /// <param name="value">Enthält nach dem Beenden dieser Methode den Wert, der dem mit  <paramref name="columnIndex"/> 
    /// angegebenen Spaltenindex
    /// zugeordnet ist, wenn der Spaltenindex existiert, oder andernfalls <c>null</c>. Dieser Parameter wird nicht
    /// initialisiert übergeben.</param>
    /// <returns><c>true</c>, wenn ein Spaltenindex mit dem Wert von <paramref name="columnIndex"/> in der CSV-Datei existiert.</returns>
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


    public void Fill(IEnumerable<string?> data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        Fill(data.Select(x => x.AsMemory()));
    }


    /// <summary>
    /// Füllt <see cref="CsvRecord"/> mit den Inhalten einer <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see>-Collection.
    /// </summary>
    /// <param name="data">Eine <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see>-Collection.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="data"/> enthält mehr Einträge
    /// als <see cref="CsvRecord.Count"/>.</exception>
    /// <remarks>Wenn <paramref name="data"/> weniger Einträge als <see cref="CsvRecord.Count"/> hat,
    /// werden die restlichen Felder von <see cref="CsvRecord"/> mit <see cref="ReadOnlyMemory{T}.Empty"/>-Werten gefüllt.</remarks>
    public void Fill(IEnumerable<ReadOnlyMemory<char>> data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

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


    /// <summary>
    /// Bestimmt, ob das <see cref="CsvRecord"/>-Objekt eine Spalte mit dem angegebenen Spaltennamen enthält.
    /// </summary>
    /// <param name="columnName">Der Spaltenname der zu suchenden Spalte der CSV-Datei.</param>
    /// <returns><c>true</c>, wenn <paramref name="columnName"/> zu den Spaltennamen des <see cref="CsvRecord"/>-Objekts gehört.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="columnName"/> ist <c>null</c>.</exception>
    public bool ContainsColumn(string columnName)
    {
        return columnName is null
            ? throw new ArgumentNullException(nameof(columnName))
            : _lookupDictionary.ContainsKey(columnName);
    }


    /// <summary>
    /// Gibt den nullbasierten Index der Spalte der CSV-Datei zurück, die den angegebenen Spaltennamen hat,
    /// oder -1, wenn <paramref name="columnName"/> nicht zu den Spaltennamen der CSV-Datei gehört.
    /// </summary>
    /// <param name="columnName">Der Spaltenname, für den überprüft werden soll, auf welchen Index von <see cref="CsvRecord"/> er verweist.</param>
    /// <returns>Der nullbasierte Index der Spalte der CSV-Datei mit dem Spaltennamen <paramref name="columnName"/>
    /// oder -1, wenn <paramref name="columnName"/> nicht zu den Spaltennamen der CSV-Datei gehört.</returns>
    public int IndexOfColumn(string? columnName) => columnName != null && _lookupDictionary.TryGetValue(columnName, out int i) ? i : -1;


    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    /// <summary>
    /// Gibt einen <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string, string?&gt;&gt;</see> zurück, mit dem das 
    /// <see cref="CsvRecord"/>-Objekts durchlaufen wird. Das <see cref="KeyValuePair{TKey, TValue}">KeyValuePair&lt;string, string?&gt;</see> enthält dabei
    /// den Spaltennamen als <see cref="KeyValuePair{TKey, TValue}.Key"/> und den Inhalt der Spalte als <see cref="KeyValuePair{TKey, TValue}.Value"/>.
    /// </summary>
    /// <returns>Ein <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string, string?&gt;&gt;</see>.</returns>
    public IEnumerator<KeyValuePair<string, ReadOnlyMemory<char>>> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return new KeyValuePair<string, ReadOnlyMemory<char>>(ColumnNames[i], Values[i]);
        }
    }


    /// <inheritdoc/>
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
