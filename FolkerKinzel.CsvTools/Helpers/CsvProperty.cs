using FolkerKinzel.CsvTools.Helpers.Converters;
using FolkerKinzel.CsvTools.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Helpers
{
    /// <summary>
    /// Repräsentiert eine Eigenschaft, die von <see cref="CsvRecordWrapper"/> dynamisch zur Laufzeit implementiert wird ("späte Bindung").
    /// <see cref="CsvProperty"/> kapselt Informationen über Zugriff und die Typkonvertierung, die <see cref="CsvRecordWrapper"/> benötigt,
    /// um auf die Daten des ihm zugrundeliegenden <see cref="CsvRecord"/>-Objekts zuzugreifen.
    /// </summary>
    /// <remarks>
    /// <note type="important">
    /// Es ist nicht empfehlenswert, dasselbe <see cref="CsvProperty"/>-Objekt zum Lesen von 
    /// CSV-Dateien mit verschiedenen Kopfzeilen zu verwenden - selbst wenn die zugewiesenen Spalten-Aliase dies ermöglichen würden.
    /// Der Grund dafür ist, dass beim ersten Lesen der geeignetste Alias ausgesucht und dann für alle
    /// nachfolgenden Lesevorgänge verwendet wird. Die Methode <see cref="Clone"/> erstellt eine frische
    /// Kopie des <see cref="CsvProperty"/>-Objekts, die zum Lesen einer anderen CSV-Datei verwendet werden kann.
    /// </note>
    /// </remarks>
    public class CsvProperty : ICloneable
    {
        /// <summary>
        /// Maximale Zeit (in Millisekunden) die für das Auflösen eines Spaltennamen-Aliases aufgewendet werden kann.
        /// </summary>
        public const int MaxWildcardTimeout = 500;

        private ColumnAliasesLookup? _lookup;
        private readonly int _wildcardTimeout;

        /// <summary>
        /// Kopierkonstruktor
        /// </summary>
        /// <param name="source"><see cref="CsvProperty"/>-Objekt, das kopiert wird.</param>
        protected CsvProperty(CsvProperty source)
        {
            this.PropertyName = source.PropertyName;
            this.ColumnNameAliases = source.ColumnNameAliases;
            this.Converter = source.Converter;
            this._wildcardTimeout = source._wildcardTimeout;
        }

        /// <summary>
        /// Initialisiert ein <see cref="CsvProperty"/>-Objekt.
        /// </summary>
        /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für Bezeichner
        /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
        /// <param name="columnNameAliases">Spaltennamen der CSV-Datei, auf die <see cref="CsvProperty"/> zugreifen kann. Für den
        /// Zugriff auf <see cref="CsvProperty"/> wird der erste Alias verwendet, der eine Übereinstimmung 
        /// mit einem Spaltennamen der CSV-Datei hat. Die Alias-Strings dürfen die Wildcard-Zeichen * und ? enthalten. Wenn ein 
        /// Wildcard-Alias mit mehreren Spalten der CSV-Datei eine Übereinstimmung hat, wird die Spalte mit dem niedrigsten Index referenziert.</param>
        /// <param name="converter">Der <see cref="ICsvTypeConverter"/>, der die Typkonvertierung übernimmt.</param>
        /// <param name="wildcardTimeout">Timeout-Wert in Millisekunden. Wenn ein Alias in <paramref name="columnNameAliases"/> ein
        /// Wildcard-Zeichen enthält, wird innerhalb
        /// dieses Timeouts versucht, den Alias aufzulösen. Gelingt dies nicht, reagiert <see cref="CsvProperty"/> so, als hätte sie
        /// kein Ziel in den Spalten der CSV-Datei. (In .Net-Framework 4.0 wird kein Timeout angewendet.)</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder <paramref name="columnNameAliases"/> oder 
        /// <paramref name="converter"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
        /// ASCII-Zeichen).</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="wildcardTimeout"/> ist kleiner als 1 oder größer als
        /// <see cref="MaxWildcardTimeout"/>.</exception>
        public CsvProperty(
            string propertyName, IEnumerable<string> columnNameAliases, ICsvTypeConverter converter, int wildcardTimeout = 10)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (!Regex.IsMatch(propertyName, "^[A-Za-z_][A-Za-z0-9_]*$"))
            {
                throw new ArgumentException(Res.BadIdentifier, nameof(propertyName));
            }


            if (columnNameAliases is null)
            {
                throw new ArgumentNullException(nameof(columnNameAliases));
            }



            if (converter is null)
            {
                throw new ArgumentNullException(nameof(converter));
            }


            if(wildcardTimeout > MaxWildcardTimeout || wildcardTimeout < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(wildcardTimeout));
            }
            


            this.PropertyName = propertyName;
            this.ColumnNameAliases = new ReadOnlyCollection<string>(columnNameAliases.Where(x => x != null).ToArray());
            this.Converter = converter;
            this._wildcardTimeout = wildcardTimeout;
        }


        /// <summary>
        /// Bezeichner der Eigenschaft
        /// </summary>
        public string PropertyName { get; }


        /// <summary>
        /// Sammlung von alternativen Spaltennamen der CSV-Datei, die <see cref="CsvRecordWrapper"/> für den Zugriff auf
        /// auf eine Spalte von <see cref="CsvRecord"/> verwendet.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Um mit verschiedenen CSV-Dateien kompatibel zu sein, können z.B. unterschiedliche Schreibweisen desselben Spaltennamens
        /// oder Übersetzungen des Spaltennamens in andere Sprachen angegeben werden. Ein Alias kann auch die Wildcard-Zeichen * und ?
        /// verwenden.
        /// </para>
        /// <para>
        /// Da beim Setzen des Wertes von <see cref="CsvProperty"/> alle Aliase berücksichtigt werden, empfiehlt es sich nicht, mehreren
        /// <see cref="CsvRecord"/>-Objekten denselben Alias zuzuweisen.</para>
        /// </remarks>
        public ReadOnlyCollection<string> ColumnNameAliases { get; }


        /// <summary>
        /// Ein <see cref="ICsvTypeConverter"/>-Objekt, das die Typkonvertierung durchführt.
        /// </summary>
        public ICsvTypeConverter Converter { get; }


        /// <summary>
        /// Extrahiert mit Hilfe von <see cref="Converter"/> Daten eines bestimmten Typs aus einer Spalte von <see cref="CsvRecord"/>.
        /// </summary>
        /// <param name="record">Das <see cref="CsvRecord"/>-Objekt, aus dem gelesen wird.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="record"/> ist null.</exception>
        /// <exception cref="InvalidCastException"><see cref="ICsvTypeConverter.ThrowsOnParseErrors"/> war <c>true</c> und der Wert konnte
        /// nicht erfolgreich aus <paramref name="record"/> gelesen werden.</exception>
        internal object? GetValue(CsvRecord? record)
        {
            Debug.Assert(record != null);
            
            string? columnName = GetColumnName(record);

            try
            {
                return columnName is null ? Converter.FallbackValue : Converter.Parse(record[columnName]);
            }
            catch(Exception e)
            {
                throw new InvalidCastException(e.Message, e);
            }
        }


        /// <summary>
        /// Speichert mit Hilfe von <see cref="Converter"/> Daten eines bestimmten Typs als <see cref="string"/> in einer Spalte von <see cref="CsvRecord"/>.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"><paramref name="record"/> ist null.</exception>
        /// <exception cref="InvalidCastException"><paramref name="value"/> entsprach nicht dem Datentyp den <see cref="Converter"/> umwandeln kann.</exception>
        internal void SetValue(CsvRecord? record, object? value)
        {
            Debug.Assert(record != null);

            string? columnName = GetColumnName(record);

            if (columnName != null)
            {
                string? s = Converter.ConvertToString(value);
                record[columnName] = s;
            }
            
        }


        private string? GetColumnName(CsvRecord record)
        {
            if (this._lookup is null || record.Identifier != _lookup.CsvRecordIdentifier)
            {
                this._lookup = new ColumnAliasesLookup(record, this.ColumnNameAliases, this._wildcardTimeout);
            }

            return _lookup.ColumnName;
        }


        /// <inheritdoc/>
        /// <remarks>
        /// Es ist nicht empfehlenswert, dasselbe <see cref="CsvProperty"/>-Objekt zum Lesen von
        /// CSV-Dateien mit verschiedenen Kopfzeilen zu verwenden - selbst wenn die zugewiesenen Spalten-Aliase dies ermöglichen würden.
        /// Der Grund dafür ist, dass beim ersten Lesen der geeignetste Alias ausgesucht und dann für alle
        /// nachfolgenden Lesevorgänge verwendet wird. Die Methode <see cref="Clone"/> erstellt eine frische
        /// Kopie des <see cref="CsvProperty"/>-Objekts, die zum Lesen einer anderen CSV-Datei verwendet werden kann.
        /// </remarks>
        public virtual object Clone() => new CsvProperty(this);
        


        /////////////////////////////////////////////////////////////////////

        private class ColumnAliasesLookup
        {
            public int CsvRecordIdentifier { get; }

            public string? ColumnName { get; private set; }

            public ColumnAliasesLookup(CsvRecord record, ReadOnlyCollection<string> aliases, int wildcardTimeout)
            {
                this.CsvRecordIdentifier = record.Identifier;

                var comparer = record.Comparer;
                var columnNames = record.ColumnNames;

                //this.Aliases = aliases.Intersect(record.Keys, comparer).Distinct(comparer).ToList();


                for (int i = 0; i < aliases.Count; i++)
                {
                    string alias = aliases[i];

                    if (alias is null) continue;

                    if (HasWildcard(alias))
                    {
                        Regex regex = InitRegex(comparer, alias, wildcardTimeout);

                        for (int k = 0; k < columnNames.Count; k++) // Die Wildcard könnte auf alle keys passen.
                        {
                            string columnName = columnNames[k];

                            try
                            {
                                if (regex.IsMatch(columnName))
                                {
                                    this.ColumnName = columnName;
                                    return;
                                }
                            }
                            catch(TimeoutException)
                            {
#if !NET40
                                Debug.WriteLine(nameof(RegexMatchTimeoutException));
#endif
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < columnNames.Count; j++)
                        {
                            string columnName = columnNames[j];

                            if (comparer.Equals(columnName, alias)) // Es kann in columnNames keine 2 Strings geben, auf die das zutrifft.
                            {
                                this.ColumnName = columnName;
                                return;
                            }
                        }
                    }
                }

                static bool HasWildcard(string alias)
                {
                    // Suche Wildcardzeichen im alias
                    for (int j = 0; j < alias.Length; j++)
                    {
                        char c = alias[j];

                        if (c == '*' || c == '?')
                        {
                            return true;
                        }
                    }//for

                    return false; // keine Wildcard-Zeichen
                }//HasWildcard

            }//ctor ColumnAliasesLookup


            //[SuppressMessage("Microsoft.Performance", "CA1801:ReviewUnusedParameters", MessageId = "isChecked")]
            private static Regex InitRegex(IEqualityComparer<string> comparer, string alias, int wildcardTimeout)
            {
#if NET40
                string pattern = "^" +
                    Regex
                    .Escape(alias)
                    .Replace("\\?", ".")
                    .Replace("\\*", ".*?") + "$";
#else
                string pattern = "^" +
                    Regex
                    .Escape(alias)
                    .Replace("\\?", ".", StringComparison.Ordinal)
                    .Replace("\\*", ".*?", StringComparison.Ordinal) + "$";
#endif

                RegexOptions options = comparer.Equals(StringComparer.OrdinalIgnoreCase) ?
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline :
                                              RegexOptions.CultureInvariant | RegexOptions.Singleline;


#if NET40
                return new Regex(pattern, options);
#else
                // Da das Regex nicht wiederverwendbar ist, wird die Instanzmethode
                // verwendet.
                return new Regex(pattern, options, TimeSpan.FromMilliseconds(wildcardTimeout));
#endif

            }
        }//class ColumnAliasesLookup

    }
}
