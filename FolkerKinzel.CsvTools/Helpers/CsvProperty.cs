using FolkerKinzel.CsvTools.Helpers.Converters;
using FolkerKinzel.CsvTools.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// <param name="csvColumnAliases">Spaltennamen der CSV-Datei, auf die <see cref="CsvProperty"/> zugreifen kann. Beim lesenden
        /// Zugriff auf <see cref="CsvProperty"/> wird das Ergebnis der ersten Übereinstimmung eines strings aus <paramref name="csvColumnAliases"/>
        /// mit einem Spaltennamen der CSV-Datei zurückgegeben, bei Setzen des Wertes von <see cref="CsvProperty"/> werden hingegen alle
        /// Spalten von <see cref="CsvRecord"/>, die eine Übereinstimmung mit <paramref name="csvColumnAliases"/> haben, auf den neuen Wert
        /// gesetzt. Die Alias-Strings dürfen die Wildcard-Zeichen * und ? enthalten.</param>
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


        internal object? GetValue(CsvRecord record)
        {
            ColumnAliasesLookup lookup = GetLookup(record);

            return lookup.Aliases.Count == 0 ? Converter.FallbackValue : Converter.Parse(record[lookup.Aliases[0]]);
        }


        internal void SetValue(CsvRecord record, object? value)
        {
            ColumnAliasesLookup lookup = GetLookup(record);

            string? s = Converter.ConvertToString(value);
            var aliases = lookup.Aliases;

            for (int i = 0; i < aliases.Count; i++)
            {
                record[aliases[i]] = s;
            }
        }


        private ColumnAliasesLookup GetLookup(CsvRecord record)
        {
            if (this._lookup is null || record.Identifier != _lookup.CsvRecordIdentifier)
            {
                this._lookup = new ColumnAliasesLookup(record, this.CsvColumnAliases);
            }

            return _lookup;
        }


        /////////////////////////////////////////////////////////////////////

        private class ColumnAliasesLookup
        {
            public int CsvRecordIdentifier { get; }

            public List<string> Aliases { get; } = new List<string>();

            public ColumnAliasesLookup(CsvRecord record, ReadOnlyCollection<string> aliases)
            {
                this.CsvRecordIdentifier = record.Identifier;

                var comparer = record.Comparer;
                var keys = record.Keys;

                //this.Aliases = aliases.Intersect(record.Keys, comparer).Distinct(comparer).ToList();


                for (int i = 0; i < aliases.Count; i++)
                {
                    string alias = aliases[i];

                    if (alias is null) continue;
                    
                    if (HasWildcard(alias)) continue;

                    for (int j = 0; j < keys.Count; j++)
                    {
                        string key = keys[j];

                        if (comparer.Equals(key, alias)) // Es kann in keys keine 2 strings geben, auf die das zutrifft.
                        {
                            if (!Aliases.Contains(key, comparer))
                            {
                                Aliases.Add(key);
                            }

                            break; 
                        }
                    }
                }

                bool HasWildcard(string alias)
                {
                    for (int j = 0; j < alias.Length; j++)
                    {
                        char c = alias[j];

                        // Wildcard behandeln:
                        if (c == '*' || c == '?')
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

                            // Da das Regex nicht wiederverwendbar ist, wird die Instanzmethode
                            // verwendet.
                            var regex = new Regex(pattern, options);

                            for (int k = 0; k < keys.Count; k++) // Die Wildcard könnte auf alle keys passen.
                            {
                                string key = keys[k];

                                if (regex.IsMatch(key) && !Aliases.Contains(key, comparer))
                                {
                                    Aliases.Add(key);
                                }
                            }

                            return true;
                        }
                    }//for

                    return false;
                }//HasWildcard

            }//ctor ColumnAliasesLookup

        }//class ColumnAliasesLookup


    }
}
