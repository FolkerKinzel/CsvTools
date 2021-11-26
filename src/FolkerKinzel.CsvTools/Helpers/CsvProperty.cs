using FolkerKinzel.CsvTools.Resources;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

# if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.CsvTools.Helpers;

/// <summary>
/// Repräsentiert eine Eigenschaft, die von <see cref="CsvRecordWrapper"/> dynamisch zur Laufzeit implementiert wird ("späte Bindung").
/// <see cref="CsvProperty"/> kapselt Informationen über Zugriff und die Typkonvertierung, die <see cref="CsvRecordWrapper"/> benötigt,
/// um auf die Daten des ihm zugrundeliegenden <see cref="CsvRecord"/>-Objekts zuzugreifen.
/// </summary>
/// <threadsafety static="true" instance="false"/>
public sealed class CsvProperty
{
    /// <summary>
    /// Maximale Zeit (in Millisekunden) die für das Auflösen eines Spaltennamen-Aliases aufgewendet werden kann.
    /// </summary>
    public const int MaxWildcardTimeout = 500;

    private ColumnAliasesLookup? _lookup;
    private readonly int _wildcardTimeout;

    /// <summary>
    /// Initialisiert ein neues <see cref="CsvProperty"/>-Objekt.
    /// </summary>
    /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für C#-Bezeichner
    /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
    /// <param name="desiredCsvColumnIndex">Nullbasierter Index der Spalte der CSV-Datei in die <see cref="CsvProperty"/> schreiben soll bzw.
    /// aus der <see cref="CsvProperty"/> lesen soll. Wenn es den Index in der CSV-Datei nicht
    /// gibt, wird die <see cref="CsvProperty"/> beim Schreiben ignoriert. Beim Lesen wird in diesem Fall
    /// <see cref="ICsvTypeConverter.FallbackValue"/> zurückgegeben.</param>
    /// <param name="converter">Der <see cref="ICsvTypeConverter"/>, der die Typkonvertierung übernimmt.</param>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
    /// ASCII-Zeichen).</exception>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder 
    /// <paramref name="converter"/> ist <c>null</c>.</exception>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="desiredCsvColumnIndex"/> ist kleiner als 0.</exception>
    public CsvProperty(
        string propertyName, int desiredCsvColumnIndex, ICsvTypeConverter converter)
    {
        ValidatePropertyName(propertyName);

        if (desiredCsvColumnIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(desiredCsvColumnIndex));
        }

        if (converter is null)
        {
            throw new ArgumentNullException(nameof(converter));
        }

        this.PropertyName = propertyName;
        this.DesiredCsvColumnIndex = desiredCsvColumnIndex;
        this.Converter = converter;
    }

#if NET40
#pragma warning disable CS1574 // XML-Kommentar weist ein cref-Attribut auf, das nicht aufgelöst werden konnte.
#endif
    /// <summary>
    /// Initialisiert ein neues <see cref="CsvProperty"/>-Objekt.
    /// </summary>
    /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für C#-Bezeichner
    /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
    /// <param name="columnNameAliases">Spaltennamen der CSV-Datei, auf die <see cref="CsvProperty"/> zugreifen kann. Für den
    /// Zugriff auf <see cref="CsvProperty"/> wird der erste Alias verwendet, der eine Übereinstimmung 
    /// mit einem Spaltennamen der CSV-Datei hat. Die Alias-Strings dürfen die Wildcard-Zeichen * und ? enthalten. Wenn ein 
    /// Wildcard-Alias mit mehreren Spalten der CSV-Datei eine Übereinstimmung hat, wird die Spalte mit dem niedrigsten Index referenziert.</param>
    /// <param name="converter">Der <see cref="ICsvTypeConverter"/>, der die Typkonvertierung übernimmt.</param>
    /// <param name="wildcardTimeout">Timeout-Wert in Millisekunden oder 0, für <see cref="Regex.InfiniteMatchTimeout"/>. 
    /// Ist der Wert größer als <see cref="MaxWildcardTimeout"/> wird er auf diesen Wert normalisiert.
    /// Wenn ein Alias in <paramref name="columnNameAliases"/> ein
    /// Wildcard-Zeichen enthält, wird innerhalb
    /// dieses Timeouts versucht, den Alias aufzulösen. Gelingt dies nicht, reagiert <see cref="CsvProperty"/> so, als hätte sie
    /// kein Ziel in den Spalten der CSV-Datei. (In .Net-Framework 4.0 wird kein Timeout angewendet.)</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder <paramref name="columnNameAliases"/> oder 
    /// <paramref name="converter"/> ist <c>null</c>.</exception>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
    /// ASCII-Zeichen).</exception>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="wildcardTimeout"/> ist kleiner als 0.</exception>
    public CsvProperty(
#if NET40
#pragma warning restore CS1574 // XML-Kommentar weist ein cref-Attribut auf, das nicht aufgelöst werden konnte.
#endif
            string propertyName, IEnumerable<string> columnNameAliases, ICsvTypeConverter converter, int wildcardTimeout = 10)
    {
        ValidatePropertyName(propertyName);

        if (columnNameAliases is null)
        {
            throw new ArgumentNullException(nameof(columnNameAliases));
        }


        if (converter is null)
        {
            throw new ArgumentNullException(nameof(converter));
        }


        if (wildcardTimeout > MaxWildcardTimeout)
        {
            wildcardTimeout = MaxWildcardTimeout;
        }
        else if (wildcardTimeout < 0)
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
    /// auf eine Spalte von <see cref="CsvRecord"/> verwendet oder <c>null</c>, wenn im Konstruktor stattdessen ein Index
    /// angegeben wurde.
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
    public ReadOnlyCollection<string>? ColumnNameAliases { get; }

    /// <summary>
    /// Der Index der Spalte der CSV-Datei, auf die <see cref="CsvProperty"/> zugreifen soll oder
    /// <c>null</c>, wenn dieser im Konstruktor nicht angegeben wurde.
    /// </summary>
    public int? DesiredCsvColumnIndex { get; }


    /// <summary>
    /// Der Index der Spalte der CSV-Datei, auf die <see cref="CsvProperty"/> zugreift oder <c>null</c>,
    /// wenn <see cref="CsvProperty"/> kein Ziel in der CSV-Datei findet. Die Eigenschaft wird beim
    /// ersten Lese- oder Schreibzugriff aktualisiert.
    /// </summary>
    public int? ReferredCsvColumnIndex => _lookup?.ColumnIndex;



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
        int? columnIndex = GetColumnIndex(record);

        try
        {
            return columnIndex.HasValue ? Converter.Parse(record[columnIndex.Value]) : Converter.FallbackValue;
        }
        catch (Exception e)
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
        int? columnIndex = GetColumnIndex(record);

        if (columnIndex.HasValue)
        {
            string? s = Converter.ConvertToString(value);
            record[columnIndex.Value] = s;
        }
    }


    private int? GetColumnIndex(CsvRecord record)
    {
        if (this._lookup is null || record.Identifier != _lookup.CsvRecordIdentifier)
        {
            this._lookup = ColumnNameAliases is null
                ? new ColumnAliasesLookup(record, this.DesiredCsvColumnIndex!.Value)
                : new ColumnAliasesLookup(record, this.ColumnNameAliases, this._wildcardTimeout);
        }

        return _lookup.ColumnIndex;
    }

    /// <summary>
    /// Validates the propertyName.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is not a valid C# identifier.</exception>
    private static void ValidatePropertyName(string propertyName)
    {
        if (propertyName is null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        if (!Regex.IsMatch(propertyName, "^[A-Za-z_][A-Za-z0-9_]*$"))
        {
            throw new ArgumentException(Res.BadIdentifier, nameof(propertyName));
        }
    }


    /////////////////////////////////////////////////////////////////////

    private class ColumnAliasesLookup
    {
        public int CsvRecordIdentifier { get; }
        public int? ColumnIndex { get; }

        public ColumnAliasesLookup(CsvRecord record, int estimatedIndex)
        {
            this.CsvRecordIdentifier = record.Identifier;

            if (estimatedIndex < record.ColumnNames.Count)
            {
                ColumnIndex = estimatedIndex;
            }
        }

#if NET40
            [SuppressMessage("Usage", "CA1801:Nicht verwendete Parameter überprüfen", Justification = "<Ausstehend>")]
#endif
        public ColumnAliasesLookup(CsvRecord record, ReadOnlyCollection<string> aliases, int wildcardTimeout)
        {
            this.CsvRecordIdentifier = record.Identifier;

            IEqualityComparer<string>? comparer = record.Comparer;
            ReadOnlyCollection<string>? columnNames = record.ColumnNames;

            for (int i = 0; i < aliases.Count; i++)
            {
                string alias = aliases[i];

                if (alias is null)
                {
                    continue;
                }

                if (HasWildcard(alias))
                {
#if NET40
                        Regex regex = InitRegex(comparer, alias);
#else
                    Regex regex = InitRegex(comparer, alias, wildcardTimeout);
#endif

                    for (int k = 0; k < columnNames.Count; k++) // Die Wildcard könnte auf alle keys passen.
                    {
                        string columnName = columnNames[k];

                        try
                        {
                            if (regex.IsMatch(columnName))
                            {
                                this.ColumnIndex = k;
                                return;
                            }
                        }
                        catch (TimeoutException)
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
                            this.ColumnIndex = j;
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


#if NET40
            private static Regex InitRegex(IEqualityComparer<string> comparer, string alias)
            {
                string pattern = "^" +
                    Regex
                    .Escape(alias)
                    .Replace("\\?", ".")
                    .Replace("\\*", ".*?") + "$";

                RegexOptions options = comparer.Equals(StringComparer.OrdinalIgnoreCase) ?
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline :
                                              RegexOptions.CultureInvariant | RegexOptions.Singleline;

                // Da das Regex nicht wiederverwendbar ist, wird die Instanzmethode
                // verwendet.
                return new Regex(pattern, options);
            }

#else

        private static Regex InitRegex(IEqualityComparer<string> comparer, string alias, int wildcardTimeout)
        {
            string pattern = "^" +
                Regex
                .Escape(alias)
                .Replace("\\?", ".", StringComparison.Ordinal)
                .Replace("\\*", ".*?", StringComparison.Ordinal) + "$";

            RegexOptions options = comparer.Equals(StringComparer.OrdinalIgnoreCase) ?
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline :
                                          RegexOptions.CultureInvariant | RegexOptions.Singleline;

            // Da das Regex nicht wiederverwendbar ist, wird die Instanzmethode
            // verwendet.
            return new Regex(pattern,
                             options,
                             wildcardTimeout == 0 ? Regex.InfiniteMatchTimeout
                                                  : TimeSpan.FromMilliseconds(wildcardTimeout));
        }
#endif


    }//class ColumnAliasesLookup

}
