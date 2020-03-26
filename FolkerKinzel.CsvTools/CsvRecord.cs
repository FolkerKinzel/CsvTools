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
    public sealed class CsvRecord : IEnumerable<KeyValuePair<string, string?>>
    {
        #region fields

        private const string DefaultKeyName = "Column";
#if NET40
        private static readonly string?[] DefaultArr = new string?[0];
        private readonly string?[] _values = DefaultArr;
#else
        private readonly string?[] _values = Array.Empty<string?>();
#endif

        private readonly Dictionary<string, int> _lookupDictionary;
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
            _lookupDictionary = source._lookupDictionary;
            _keys = source._keys;
            _values = new string?[Count];
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

            this._lookupDictionary = new Dictionary<string, int>(columnsCount, comparer);
            

            var keyArr = new string[columnsCount];


            for (int i = 0; i < columnsCount; i++)
            {
                string keyName = DefaultKeyName + (i + 1).ToString(CultureInfo.InvariantCulture);

                keyArr[i] = keyName;
                this._lookupDictionary.Add(keyName, i);
            }

            this._keys = new ReadOnlyCollection<string>(keyArr);

            if (initArr)
            {
                _values = new string?[columnsCount];
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

            this._lookupDictionary = new Dictionary<string, int>(keys.Length, comparer);

            int defaultNameCounter = 0;

            for (int i = 0; i < keys.Length; i++)
            {
                string? key = keys[i];

                if (key is null)
                {
                    key = GetDefaultName();
                    _lookupDictionary.Add(key, i);
                    keys[i] = key;
                    continue;
                }

                if (trimColumns)
                {
                    key = key.Trim();
                }

                if (!throwException && _lookupDictionary.ContainsKey(key))
                {
                    key = MakeUnique(key);
                }

                _lookupDictionary.Add(key, i);
                keys[i] = key;

            }

            this._keys = new ReadOnlyCollection<string>(keys!);

            if (initArr)
            {
                _values = new string?[Count];
            }



            ///////////////////////////////////////////////////

            string GetDefaultName()
            {
                string key;

                do
                {
                    key = DefaultKeyName + (++defaultNameCounter).ToString(CultureInfo.InvariantCulture);
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
        /// Ruft das Element am angegebenen Index ab oder legt dieses fest.
        /// </summary>
        /// <param name="index">Der nullbasierte Index des abzurufenden oder festzulegenden Elements.</param>
        /// <returns>Das Element am angegebenen Index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder größer oder gleich <see cref="Count"/>.</exception>
        public string? this[int index]
        {
            get => _values[index];
            set => _values[index] = value;
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
            get => _values[_lookupDictionary[key]];
            set => _values[_lookupDictionary[key]] = value;
        }


        /// <summary>
        /// Gibt die Anzahl der Spalten in <see cref="CsvRecord"/> zurück.
        /// </summary>
        public int Count => _keys.Count;

        /// <summary>
        /// Gibt true zurück, wenn <see cref="Count"/> 0 ist oder wenn alle
        /// Felder den Wert <c>null</c> haben.
        /// </summary>
        public bool IsEmpty => Count == 0 || _values.All(x => x is null);

        

        /// <summary>
        /// Gibt die in <see cref="CsvRecord"/> gespeicherten Spaltennamen zurück.
        /// </summary>
        public ReadOnlyCollection<string> Keys => _keys;

        

        /// <summary>
        /// Gibt die in <see cref="CsvRecord"/> gespeicherten Daten als Collection zurück. Die 
        /// Werte können verändert werden.
        /// </summary>
        public IList<string?> Values => _values;


        /// <summary>
        /// Gibt eine Kopie der in <see cref="CsvRecord"/> gespeicherten Daten als <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string?&gt;</see> zurück, das
        /// für den Schlüsselvergleich denselben <see cref="StringComparer"/> verwendet, mit dem <see cref="CsvRecord"/> erstellt wurde.
        /// </summary>
        /// <returns>Eine Kopie der in <see cref="CsvRecord"/> gespeicherten Daten als <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, string?&gt;</see>.</returns>
        public Dictionary<string, string?> ToDictionary()
        {
#if NET40
            var dic = new Dictionary<string, string?>(this.Count, this._lookupDictionary.Comparer);

            foreach (var kvp in this)
            {
                dic.Add(kvp.Key, kvp.Value);
            }

            return dic;
#else
            return new Dictionary<string, string?>(this, this._lookupDictionary.Comparer);
#endif
        }

        /// <summary>
        /// Ein Hashcode, der für alle <see cref="CsvRecord"/>-Objekte, die zu selben CSV-Datei
        /// gehören, identisch ist. (Wird von <see cref="CsvProperty"/> verwendet, um festzustellen,
        /// ob das aktuelle <see cref="CsvRecord"/>-Objekt zur selben CSV-Datei gehört, mit der das
        /// Alias-Lookup erstellt wurde.)
        /// </summary>
        internal int Identifier => _lookupDictionary.GetHashCode();


        /// <summary>
        /// Der zur Auswahl der Schlüssel verwendete Comparer.
        /// </summary>
        internal IEqualityComparer<string> Comparer => _lookupDictionary.Comparer;

#endregion

        /// <summary>
        /// Setzt alle Datenfelder von <see cref="CsvRecord"/> auf <c>null</c>.
        /// </summary>
        public void Clear() => Array.Clear(_values, 0, _values.Length);

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
            if (_lookupDictionary.TryGetValue(key, out int index))
            {
                value = _values[index];
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
                if (dataIndex >= _values.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(data));
                }

                _values[dataIndex++] = item;
            }

            for (int i = dataIndex; i < _values.Length; i++)
            {
                _values[i] = null;
            }
        }

        


        /// <summary>
        /// Untersucht, ob der <see cref="string"/>&#160;<paramref name="item"/> den Inhalt einer Datenspalte in <see cref="CsvRecord"/> darstellt.
        /// </summary>
        /// <param name="item">Der zu suchende <see cref="string"/>.</param>
        /// <returns>True, wenn <paramref name="item"/> den Inhalt einer Datenspalte in <see cref="CsvRecord"/> darstellt.</returns>
        public bool Contains(string? item) => Array.IndexOf(_values, item) >= 0;


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
        /// Bestimmt, ob das <see cref="CsvRecord"/>-Objekt den angegebenen Schlüssel (Spaltenname) enthält.
        /// </summary>
        /// <param name="key">Der zu suchende Schlüssel ("Spaltenname").</param>
        /// <returns>True, wenn <paramref name="key"/> zu den Spaltennamen des <see cref="CsvRecord"/>-Objekts gehört.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> ist <c>null</c>.</exception>
        public bool ContainsKey(string key) => _lookupDictionary.ContainsKey(key);


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
        public void CopyTo(string?[] array, int arrayIndex) => _values.CopyTo(array, arrayIndex);


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
                array[arrayIndex + i] = new KeyValuePair<string, string?>(_keys[i], _values[i]);
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
        public int IndexOf(string? item) => Array.IndexOf(_values, item);


        /// <summary>
        /// Gibt den nullbasierten Index zurück, auf den der Spaltenname <paramref name="key"/> verweist
        /// oder -1, wenn <paramref name="key"/> nicht zu den in <see cref="CsvRecord"/> registrierten
        /// Spaltennamen gehört (die i.d.R. den Spaltennamen der CSV-Datei entsprechen).
        /// </summary>
        /// <param name="key">Der Spaltenname, für den überprüft werden soll, auf welchen Index von <see cref="CsvRecord"/> er verweist.</param>
        /// <returns>Der Index der Datenspalte in <see cref="CsvRecord"/>, auf die der Spaltenname <paramref name="key"/> verweist oder
        /// -1, wenn <paramref name="key"/> kein Spaltenname ist.</returns>
        public int IndexOfKey(string? key) => key != null && _lookupDictionary.TryGetValue(key, out int i) ? i : -1;



        


        /// <summary>
        /// Gibt einen <see cref="IEnumerator"/> zurück, mit dem das <see cref="CsvRecord"/>-Objekt
        /// durchlaufen wird.
        /// </summary>
        /// <returns>Ein <see cref="IEnumerator"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <summary>
        /// Gibt einen <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string, string?&gt;&gt;</see> zurück, mit das 
        /// <see cref="CsvRecord"/>-Objekts durchlaufen wird. Ein <see cref="KeyValuePair{TKey, TValue}"/> enthält dabei
        /// den Spaltennamen als <see cref="KeyValuePair{TKey, TValue}.Key"/> und den Inhalt der Spalte als <see cref="KeyValuePair{TKey, TValue}.Value"/>.
        /// </summary>
        /// <returns>Ein <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string, string?&gt;&gt;</see>.</returns>
        public IEnumerator<KeyValuePair<string, string?>> GetEnumerator()
        {

            for (int i = 0; i < Count; i++)
            {
                yield return new KeyValuePair<string, string?>(Keys[i], Values[i]);
            }
            
        }




        /// <summary>
        /// Erstellt eine <see cref="String"/>-Repräsentation des <see cref="CsvRecord"/>-Objekts. (Gut für's Debugging.)
        /// </summary>
        /// <returns>Eine <see cref="String"/>-Repräsentation des <see cref="CsvRecord"/>-Objekts.</returns>
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


        












    }
}
