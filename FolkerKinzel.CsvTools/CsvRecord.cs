using FolkerKinzel.CsvTools.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace FolkerKinzel.CsvTools
{
    /// <summary>
    /// Repräsentiert einen Datensatz der CSV-Datei (Zeile). Die Spaltenreihenfolge entspricht der der CSV-Datei und alle Spalten sind vom Datentyp
    /// <see cref="string"/>. Auf den Inhalt der Spalten kann per nullbasiertem Spaltenindex oder über die Spaltennamen zugegriffen werden.
    /// </summary>
    /// <remarks>
    /// Mit der Klasse <see cref="CsvRecordWrapper"/> kann die Reihenfolge der Datenspalten zur Laufzeit auf die evtl. andere Spaltenreihenfolge 
    /// einer <see cref="DataTable"/> gemappt werden und es können damit auch Typkonvertierungen durchgeführt werden.
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Bezeichner müssen ein korrektes Suffix aufweisen", Justification = "<Ausstehend>")]
    public sealed class CsvRecord : IList<string?>, IDictionary<string, string?>
    {
        #region fields

        private const string DefaultKeyName = "Column";
#if NET40
        private static readonly string?[] DefaultArr = new string?[0];
        private readonly string?[] _arr = DefaultArr;
#else
        private readonly string?[] _arr = Array.Empty<string?>();
#endif

        private readonly Dictionary<string, int> _dic;
        private readonly int _count;
        private readonly ReadOnlyCollection<string> _keys;

        #endregion


        #region ctors

        /// <summary>
        /// Initialisiert aus <paramref name="source"/>, das als Vorlage dient ein neues <see cref="CsvRecord"/>-Objekt,
        /// das die unveränderlichen Teile der Vorlage referenziert bzw. kopiert. (Ctor der von <see cref="CsvReader"/> verwendet wird.)
        /// </summary>
        /// <param name="source"><see cref="CsvRecord"/>-Objekt, das als Vorlage dient.</param>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal CsvRecord(CsvRecord source)
        {
            _dic = source._dic;
            _keys = source._keys;
            _count = source._count;
            this.Identifier = source.Identifier;
            _arr = new string?[_count];
        }


        /// <summary>
        /// Initialisiert ein neues <see cref="CsvRecord"/>-Objekt mit Standardnamen für die Spalten. (Geeignet für
        /// CSV-Dateien ohne Kopfzeile.)
        /// </summary>
        /// <param name="columnsCount">Anzahl der Spalten.</param>
        /// <param name="caseSensitive">Wenn true, werden die Spaltennamen case-sensitiv behandelt.</param>
        /// <param name="initArr">Wenn false, wird das Datenarray nicht initialisiert. Das Objekt taugt dann nur als Kopierschablone
        /// für weitere <see cref="CsvRecord"/>-Objekte. (Wird von <see cref="CsvReader"/> verwendet.</param>
        internal CsvRecord(int columnsCount, bool caseSensitive, bool initArr)
        {
            IEqualityComparer<string> comparer =
                caseSensitive ?
                StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

            this._dic = new Dictionary<string, int>(columnsCount, comparer);
            this._count = columnsCount;
            this.Identifier = _dic.GetHashCode();

            var keyArr = new string[_count];


            for (int i = 0; i < _count; i++)
            {
                string keyName = DefaultKeyName + (i + 1).ToString(CultureInfo.InvariantCulture);

                keyArr[i] = keyName;
                this._dic.Add(keyName, i);
            }

            this._keys = new ReadOnlyCollection<string>(keyArr);

            if (initArr)
            {
                _arr = new string?[_count];
            }
        }

        /// <summary>
        /// Initialisiert ein neues <see cref="CsvRecord"/>-Objekt mit Spaltennamen. (Geeignet für
        /// CSV-Dateien mit Kopfzeile.)
        /// </summary>
        /// <param name="keys">Spaltennamen. Die Auflistung kann <c>null</c>-Werte enthalten: Diese werden dann durch 
        /// Standardnamen ersetzt.</param>
        /// <param name="caseSensitive">Wenn true, werden die Spaltennamen case-sensitiv behandelt.</param>
        /// <param name="trimColumns">Wenn true, werden alle Spaltennamen mit der Methode <see cref="string.Trim()"/> behandelt.</param>
        /// <param name="initArr">Wenn false, wird das Datenarray nicht initialisiert. Das Objekt taugt dann nur als Kopierschablone
        /// für weitere <see cref="CsvRecord"/>-Objekte. (Wird von <see cref="CsvReader"/> verwendet.</param>
        /// <param name="throwException">Wenn true, wird eine <see cref="ArgumentException"/> geworfen,
        /// wenn <paramref name="keys"/> 2 identische Spaltennamen enthält. Beim Lesen einer Datei sollte der 
        /// Parameter auf false gesetzt werden, um die Spaltennamen automatisch so abzuwandeln, dass sie eindeutig sind.</param>
        /// <exception cref="ArgumentException">Ein Spaltenname war bereits im Dictionary enthalten. Die Exception wird nur dann
        /// geworfen, wenn <paramref name="throwException"/> true ist.</exception>
        internal CsvRecord(string?[] keys, bool caseSensitive, bool trimColumns, bool initArr, bool throwException)
        {
            Debug.Assert(keys != null);

            IEqualityComparer<string> comparer =
                caseSensitive ?
                StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

            this._dic = new Dictionary<string, int>(keys.Length, comparer);
            this._count = keys.Length;
            this.Identifier = _dic.GetHashCode();

            int defaultNameCounter = 0;

            for (int i = 0; i < _count; i++)
            {
                string? key = keys[i];

                if (key is null)
                {
                    key = GetDefaultName();
                    _dic.Add(key, i);
                    keys[i] = key;
                    continue;
                }

                if (trimColumns)
                {
                    key = key.Trim();
                }

                if (!throwException && _dic.ContainsKey(key))
                {
                    key = MakeUnique(key);
                }

                _dic.Add(key, i);
                keys[i] = key;

            }

            _keys = new ReadOnlyCollection<string>(keys!);

            if (initArr)
            {
                _arr = new string?[_count];
            }



            ///////////////////////////////////////////////////

            string GetDefaultName()
            {
                string key;

                do
                {
                    key = DefaultKeyName + (++defaultNameCounter).ToString(CultureInfo.InvariantCulture);
                } while (_dic.ContainsKey(key));

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
                } while (_dic.ContainsKey(unique));

                return unique;
            }

        }

        #endregion


        #region Properties

        /// <summary>
        /// Ruft das Element am angegebenen Index ab oder legt dieses fest.
        /// </summary>
        /// <param name="index">Der nullbasierte Index des abzurufenden oder festzulegenden Elements.</param>
        /// <returns>Das Element am angegebenen Index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder größer oder gleich <see cref="Count"/>.</exception>
        public string? this[int index]
        {
            get => _arr[index];
            set => _arr[index] = value;
        }

        /// <summary>
        /// Ruft den Wert ab, der dem angegebenen Schlüssel zugeordnet ist, oder legt diesen fest.
        /// </summary>
        /// <param name="key">Der Schlüssel des abzurufenden oder festzulegenden Werts.</param>
        /// <returns>Der dem angegebenen Schlüssel zugeordnete Wert.</returns>
        /// <exception cref="KeyNotFoundException">Der mit <paramref name="key"/> angegebene Schlüssel existiert nicht.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> ist <c>null</c>.</exception>
        public string? this[string key]
        {
            get => _arr[_dic[key]];
            set => _arr[_dic[key]] = value;
        }


        /// <summary>
        /// Gibt die Anzahl der Spalten in <see cref="CsvRecord"/> zurück.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Gibt true zurück, wenn <see cref="Count"/> 0 ist oder wenn alle
        /// Felder den Wert <c>null</c> haben.
        /// </summary>
        public bool IsEmpty => _count == 0 || _arr.All(x => x is null);

        /// <summary>
        /// Gibt an, ob <see cref="CsvRecord"/> schreibgeschützt ist. (Immer false.)
        /// </summary>
        bool ICollection<string?>.IsReadOnly => false;


        /// <summary>
        /// Gibt an, ob <see cref="CsvRecord"/> schreibgeschützt ist. (Immer false.)
        /// </summary>
        bool ICollection<KeyValuePair<string, string?>>.IsReadOnly => false;

        /// <summary>
        /// Gibt die in <see cref="CsvRecord"/> gespeicherten Spaltennamen zurück.
        /// </summary>
        public ReadOnlyCollection<string> Keys => _keys;

        /// <summary>
        /// Gibt die in <see cref="CsvRecord"/> gespeicherten Spaltennamen zurück. Die 
        /// Collection ist readonly.
        /// </summary>
        ICollection<string> IDictionary<string, string?>.Keys => Keys;

        /// <summary>
        /// Gibt die in <see cref="CsvRecord"/> gespeicherten Daten als Collection zurück. Die 
        /// Werte können verändert werden.
        /// </summary>
        public ICollection<string?> Values => _arr;

        /// <summary>
        /// Ein Hashcode, der für alle <see cref="CsvRecord"/>-Objekte, die zu selben CSV-Datei
        /// gehören, identisch ist.
        /// </summary>
        internal int Identifier { get; }

        /// <summary>
        /// Der zur Auswahl der Schlüssel verwendete Comparer.
        /// </summary>
        internal IEqualityComparer<string> Comparer => _dic.Comparer;

        #endregion

        /// <summary>
        /// Setzt alle Datenfelder von <see cref="CsvRecord"/> auf <c>null</c>.
        /// </summary>
        public void Clear() => Array.Clear(_arr, 0, _arr.Length);

        /// <summary>
        /// Ruft den dem angegebenen Schlüssel zugeordneten Wert ab.
        /// </summary>
        /// <param name="key">Der Schlüssel des abzurufenden Werts.</param>
        /// <param name="value">Enthält nach dem Beenden dieser Methode den Wert, der dem angegebenen Schlüssel 
        /// zugeordnet ist, wenn der Schlüssel gefunden wurde, oder andernfalls <c>null</c>. Dieser Parameter wird nicht
        /// initialisiert übergeben.</param>
        /// <returns>True, wenn ein Schlüssel mit dem Wert von <paramref name="key"/> enthalten ist.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> ist <c>null</c>.</exception>
        public bool TryGetValue(string key, out string? value)
        {
            if (_dic.TryGetValue(key, out int index))
            {
                value = _arr[index];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }



        /// <summary>
        /// Füllt <see cref="CsvRecord"/> mit den Inhalten einer <see cref="string"/>-Collection.
        /// </summary>
        /// <param name="data">Eine <see cref="string"/>-Collection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="data"/> enthält mehr Einträge
        /// als <see cref="CsvRecord.Count"/>.</exception>
        /// <remarks>Wenn <paramref name="data"/> weniger Einträge als <see cref="CsvRecord.Count"/> hat,
        /// werden die restlichen Felder von <see cref="CsvRecord"/> mit <c>null</c>-Werten gefüllt.</remarks>
        public void Fill(IEnumerable<string?> data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            int dataIndex = 0;

            foreach (string? item in data)
            {
                if (dataIndex >= _arr.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(data));
                }

                _arr[dataIndex++] = item;
            }

            for (int i = dataIndex; i < _arr.Length; i++)
            {
                _arr[i] = null;
            }
        }

        //public void Fill(IEnumerable<object?> data)
        //{
        //    if (data is null)
        //    {
        //        throw new ArgumentNullException(nameof(data));
        //    }

        //    int dataIndex = 0;

        //    foreach (object? item in data)
        //    {
        //        if (dataIndex >= _arr.Length)
        //        {
        //            throw new ArgumentOutOfRangeException(nameof(data));
        //        }

        //        _arr[dataIndex++] = ObjectToStringConverter.ConvertToString(item);
        //    }

        //    for (int i = dataIndex; i < _arr.Length; i++)
        //    {
        //        _arr[i] = null;
        //    }
        //}


        /// <summary>
        /// Untersucht, ob der <see cref="string"/>&#160;<paramref name="item"/> den Inhalt einer Datenspalte in <see cref="CsvRecord"/> darstellt.
        /// </summary>
        /// <param name="item">Der zu suchende <see cref="string"/>.</param>
        /// <returns>True, wenn <paramref name="item"/> den Inhalt einer Datenspalte in <see cref="CsvRecord"/> darstellt.</returns>
        public bool Contains(string? item) => Array.IndexOf(_arr, item) >= 0;


        /// <summary>
        /// Untersucht, ob der Inhalt der Datenspalte mit dem Namen von <paramref name="key"/> dem Wert von <paramref name="value"/>
        /// entspricht.
        /// </summary>
        /// <param name="key">Der Spaltenname der zu vergelichenden Datenspalte.</param>
        /// <param name="value">Der zu vergleichende <see cref="string"/>.</param>
        /// <returns>True, wenn der Inhalt der Datenspalte mit dem Namen von <paramref name="key"/> dem Wert von <paramref name="value"/>
        /// entspricht.</returns>
        /// <exception cref="KeyNotFoundException">Der mit <paramref name="key"/> angegebene Schlüssel existiert nicht.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> ist <c>null</c>.</exception>
        public bool Contains(string key, string? value) => this[key] == value;


        /// <summary>
        /// Untersucht, ob der Inhalt der Datenspalte mit dem Namen <see cref="KeyValuePair{TKey, TValue}.Key"/>&#160;<see cref="KeyValuePair{TKey, TValue}.Value"/>
        /// entspricht.
        /// </summary>
        /// <param name="item">Ein <see cref="KeyValuePair{TKey, TValue}"/>, das den Namen der zu überprüfenden Datenspalte und den zu vergleichenden Wert
        /// enthält.</param>
        /// <returns>True, wenn der Inhalt der Datenspalte mit dem Namen <see cref="KeyValuePair{TKey, TValue}.Key"/>&#160;<see cref="KeyValuePair{TKey, TValue}.Value"/>
        /// entspricht.</returns>
        /// <exception cref="KeyNotFoundException">Der mit <see cref="KeyValuePair{TKey, TValue}.Key"/> angegebene Schlüssel existiert nicht.</exception>
        bool ICollection<KeyValuePair<string, string?>>.Contains(KeyValuePair<string, string?> item) => Contains(item.Key, item.Value);


        /// <summary>
        /// Bestimmt, ob das <see cref="CsvRecord"/>-Objekt den angegebenen Schlüssel (Spaltenname) enthält.
        /// </summary>
        /// <param name="key">Der zu suchende Schlüssel ("Spaltenname").</param>
        /// <returns>True, wenn <paramref name="key"/> zu den Spaltennamen des <see cref="CsvRecord"/>-Objekts gehört.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> ist <c>null</c>.</exception>
        public bool ContainsKey(string key) => _dic.ContainsKey(key);


        /// <summary>
        /// Kopiert den Inhalt sämtlicher Spalten des <see cref="CsvRecord"/>-Objekts in ein <see cref="string"/>-Array,
        /// beginnend bei dem nullbasierten Index <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array">Das Array, in das hineinkopiert wird.</param>
        /// <param name="arrayIndex">Der Index in <paramref name="array"/>, bei dem der Kopiervorgang startet.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> ist kleiner als 0 oder die Anzahl der zu 
        /// kopierenden Elemente ist größer als die verfügbare Anzahl der Elemente von <paramref name="arrayIndex"/> bis zum Ende
        /// des Zielarrays.</exception>
        /// <exception cref="ArgumentException"><paramref name="array"/> ist mehrdimensional.</exception>
        public void CopyTo(string?[] array, int arrayIndex) => _arr.CopyTo(array, arrayIndex);


        /// <summary>
        /// Kopiert den Inhalt sämtlicher Spalten des <see cref="CsvRecord"/>-Objekts in ein <see cref="KeyValuePair{TKey, TValue}"/>-Array,
        /// beginnend bei dem nullbasierten Index <paramref name="arrayIndex"/>. Ein <see cref="KeyValuePair{TKey, TValue}"/> enthält dabei
        /// den Spaltennamen als <see cref="KeyValuePair{TKey, TValue}.Key"/> und den Inhalt der Spalte als <see cref="KeyValuePair{TKey, TValue}.Value"/>.
        /// </summary>
        /// <param name="array">Das Array, in das hineinkopiert wird.</param>
        /// <param name="arrayIndex">Der Index in <paramref name="array"/>, bei dem der Kopiervorgang startet.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> ist kleiner als 0 oder die Anzahl der zu 
        /// kopierenden Elemente ist größer als die verfügbare Anzahl der Elemente von <paramref name="arrayIndex"/> bis zum Ende
        /// des Zielarrays.</exception>
        public void CopyTo(KeyValuePair<string, string?>[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }


            for (int i = 0; i < _keys.Count; i++)
            {
                array[arrayIndex + i] = new KeyValuePair<string, string?>(_keys[i], _arr[i]);
            }
        }

        /// <summary>
        /// Gibt den nullbasierten Index des ersten Vorkommens von <paramref name="item"/> unter 
        /// den in <see cref="CsvRecord"/> gespeicherten Daten zurück oder -1, wenn <paramref name="item"/>
        /// dort nicht gefunden wird.
        /// </summary>
        /// <param name="item">Der zu suchende <see cref="string"/> oder <c>null</c>.</param>
        /// <returns>Der nullbasierte Index des ersten Vorkommens von <paramref name="item"/> unter den gespeicherten Daten
        /// oder -1, wenn <paramref name="item"/> dort nicht existiert.</returns>
        public int IndexOf(string? item) => Array.IndexOf(_arr, item);


        /// <summary>
        /// Gibt den nullbasierten Index zurück, auf den der Spaltenname <paramref name="key"/> verweist
        /// oder -1, wenn <paramref name="key"/> nicht zu den in <see cref="CsvRecord"/> registrierten
        /// Spaltennamen gehört (die i.d.R. den Spaltennamen der CSV-Datei entsprechen).
        /// </summary>
        /// <param name="key">Der Spaltenname, für den überprüft werden soll, auf welchen Index von <see cref="CsvRecord"/> er verweist.</param>
        /// <returns>Der Index der Datenspalte in <see cref="CsvRecord"/>, auf die der Spaltenname <paramref name="key"/> verweist oder
        /// -1, wenn <paramref name="key"/> kein Spaltenname ist.</returns>
        public int IndexOfKey(string? key) => key != null && _dic.TryGetValue(key, out int i) ? i : -1;



        /// <summary>
        /// Gibt einen <see cref="IEnumerator{T}">IEnumerator&lt;string?&gt;</see> zurück, mit dem die Felder des <see cref="CsvRecord"/>-Objekts
        /// durchlaufen werden.
        /// </summary>
        /// <returns>Ein <see cref="IEnumerator{T}">IEnumerator&lt;string?&gt;</see>.</returns>
        public IEnumerator<string?> GetEnumerator() => ((IList<string?>)_arr).GetEnumerator();


        /// <summary>
        /// Gibt einen <see cref="IEnumerator"/> zurück, mit dem die Felder des <see cref="CsvRecord"/>-Objekts
        /// durchlaufen werden.
        /// </summary>
        /// <returns>Ein <see cref="IEnumerator"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _arr.GetEnumerator();


        /// <summary>
        /// Gibt einen <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string, string?&gt;&gt;</see> zurück, mit dem die Felder des 
        /// <see cref="CsvRecord"/>-Objekts durchlaufen werden. Ein <see cref="KeyValuePair{TKey, TValue}"/> enthält dabei
        /// den Spaltennamen als <see cref="KeyValuePair{TKey, TValue}.Key"/> und den Inhalt der Spalte als <see cref="KeyValuePair{TKey, TValue}.Value"/>.
        /// </summary>
        /// <returns>Ein <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string, string?&gt;&gt;</see>.</returns>
        IEnumerator<KeyValuePair<string, string?>> IEnumerable<KeyValuePair<string, string?>>.GetEnumerator()
        {
            foreach (var key in _keys)
            {
                yield return new KeyValuePair<string, string?>(key, _arr[_dic[key]]);
            }
        }




        /// <summary>
        /// Erstellt eine <see cref="String"/>-Repräsentation des <see cref="CsvRecord"/>-Objekts. (Gut für's Debugging.)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Count == 0)
            {
                return base.ToString();
            }

            StringBuilder sb = new StringBuilder();

            foreach (var key in _keys)
            {
                sb.Append(key).Append(": ").Append(this[key] ?? "<null>").Append(", ");
            }

            if (sb.Length >= 2)
            {
                sb.Length -= 2;
            }

            return sb.ToString();
        }


        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="item">-</param>
        /// <returns>-</returns>
        /// <exception cref="NotSupportedException">Not Supported.</exception>
        bool ICollection<string?>.Remove(string? item) => throw new NotSupportedException();

        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="key">-</param>
        /// <returns>-</returns>
        /// <exception cref="NotSupportedException">Not Supported.</exception>
        bool IDictionary<string, string?>.Remove(string key) => throw new NotSupportedException();

        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="item">-</param>
        /// <returns>-</returns>
        /// <exception cref="NotSupportedException">Not Supported.</exception>
        bool ICollection<KeyValuePair<string, string?>>.Remove(KeyValuePair<string, string?> item) => throw new NotSupportedException();

        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="index">-</param>
        /// <exception cref="NotSupportedException">Not Supported.</exception>
        void IList<string?>.RemoveAt(int index) => throw new NotSupportedException();

        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="item">-</param>
        /// <exception cref="NotSupportedException">Not Supported.</exception>
        void ICollection<string?>.Add(string? item) => throw new NotSupportedException();

        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="key">-</param>
        /// <param name="value">-</param>
        /// <exception cref="NotSupportedException">Not Supported.</exception>
        void IDictionary<string, string?>.Add(string key, string? value) => throw new NotSupportedException();

        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="item">-</param>
        /// <exception cref="NotSupportedException">Not Supported.</exception>
        void ICollection<KeyValuePair<string, string?>>.Add(KeyValuePair<string, string?> item) => throw new NotSupportedException();

        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="index">-</param>
        /// <param name="item">-</param>
        /// <exception cref="NotSupportedException">Not Supported.</exception>
        void IList<string?>.Insert(int index, string? item) => throw new NotSupportedException();












    }
}
