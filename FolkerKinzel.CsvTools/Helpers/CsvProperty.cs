using FolkerKinzel.CsvTools.Helpers.Converters;
using FolkerKinzel.CsvTools.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Helpers
{
    /// <summary>
    /// Repräsentiert eine Eigenschaft, die von <see cref="CsvRecordWrapper"/> dynamisch zur Laufzeit implementiert wird ("späte Bindung").
    /// <see cref="CsvProperty"/> kapselt Informationen über Zugriff und die Typkonvertierung, die <see cref="CsvRecordWrapper"/> benötigt,
    /// um auf die Daten des ihm zugrundeliegenden <see cref="CsvRecord"/>-Objekts zuzugreifen.
    /// </summary>
    public sealed class CsvProperty
    {
        private ColumnAliasesLookup? _lookup;


        /// <summary>
        /// Initialisiert ein neues <see cref="CsvProperty"/>-Objekt.
        /// </summary>
        /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für Bezeichner
        /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
        /// <param name="csvColumnAliases">Spaltennamen der CSV-Datei, auf die <see cref="CsvProperty"/> zugreifen kann. Für den
        /// Zugriff auf <see cref="CsvProperty"/> wird der erste Alias verwendet, der eine Übereinstimmung 
        /// mit einem Spaltennamen der CSV-Datei hat. Die Alias-Strings dürfen die Wildcard-Zeichen * und ? enthalten. Wenn ein 
        /// Wildcard-Alias mit mehreren Spalten der CSV-Datei eine Übereinstimmung hat, wird die Spalte mit dem niedrigsten Index referenziert.</param>
        /// <param name="converter">Der <see cref="ICsvTypeConverter"/>, der die Typkonvertierung übernimmt.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder <paramref name="csvColumnAliases"/> oder 
        /// <paramref name="converter"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
        /// ASCII-Zeichen).</exception>
        public CsvProperty(
            string propertyName, IEnumerable<string> csvColumnAliases, ICsvTypeConverter converter)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (!Regex.IsMatch(propertyName, "^[A-Za-z_][A-Za-z0-9_]*$"))
            {
                throw new ArgumentException(Res.BadIdentifier, nameof(propertyName));
            }


            if (csvColumnAliases is null)
            {
                throw new ArgumentNullException(nameof(csvColumnAliases));
            }



            if (converter is null)
            {
                throw new ArgumentNullException(nameof(converter));
            }


            this.PropertyName = propertyName;
            this.CsvColumnAliases = new ReadOnlyCollection<string>(csvColumnAliases.Where(x => x != null).ToArray());
            this.Converter = converter;
        }


        /// <summary>
        /// Bezeichner der Eigenschaft
        /// </summary>
        public string PropertyName { get; }


        /// <summary>
        /// Sammlung von alternativen Spaltennamen einer CSV-Datei, die <see cref="CsvRecordWrapper"/> für den Zugriff auf
        /// <see cref="CsvRecord"/> verwendet.
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
        public ReadOnlyCollection<string> CsvColumnAliases { get; }


        /// <summary>
        /// Ein <see cref="ICsvTypeConverter"/>-Objekt, das die Typkonvertierung durchführt.
        /// </summary>
        public ICsvTypeConverter Converter { get; }


        /// <summary>
        /// Extrahiert mit Hilfe von <see cref="Converter"/> Daten eines bestimmten Typs aus einer Spalte von <see cref="CsvRecord"/>.
        /// </summary>
        /// <param name="record"></param>
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
                this._lookup = new ColumnAliasesLookup(record, this.CsvColumnAliases);
            }

            return _lookup.ColumnName;
        }


        /////////////////////////////////////////////////////////////////////

        private class ColumnAliasesLookup
        {
            public int CsvRecordIdentifier { get; }

            public string? ColumnName { get; private set; }

            public ColumnAliasesLookup(CsvRecord record, ReadOnlyCollection<string> aliases)
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
                        Regex regex = InitRegex(comparer, alias);

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




            private static Regex InitRegex(IEqualityComparer<string> comparer, string alias)
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
                return new Regex(pattern, options, TimeSpan.FromSeconds(1));
#endif

            }
        }//class ColumnAliasesLookup


    }
}
