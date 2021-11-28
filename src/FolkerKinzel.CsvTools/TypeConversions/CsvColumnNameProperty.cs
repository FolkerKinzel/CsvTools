using FolkerKinzel.CsvTools.Resources;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

# if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.CsvTools.TypeConversions;


/// <summary>
/// Repräsentiert eine Eigenschaft, die von <see cref="CsvRecordWrapper"/> dynamisch zur Laufzeit implementiert wird ("späte Bindung").
/// <see cref="CsvColumnNameProperty"/> kapselt Informationen über Zugriff und die Typkonvertierung, die <see cref="CsvRecordWrapper"/> benötigt,
/// um auf die Daten des ihm zugrundeliegenden <see cref="CsvRecord"/>-Objekts über den Spaltennamen zuzugreifen.
/// </summary>
/// <threadsafety static="true" instance="false"/>
public sealed class CsvColumnNameProperty : CsvSingleColumnProperty
{
    /// <summary>
    /// Maximale Zeit (in Millisekunden) die für das Auflösen eines Spaltennamen-Aliases aufgewendet werden kann.
    /// </summary>
    public const int MaxWildcardTimeout = 500;

    private readonly int _wildcardTimeout;

#if NET40
#pragma warning disable CS1574 // XML-Kommentar weist ein cref-Attribut auf, das nicht aufgelöst werden konnte.
#endif
    /// <summary>
    /// Initialisiert ein neues <see cref="CsvColumnNameProperty"/>-Objekt.
    /// </summary>
    /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für C#-Bezeichner
    /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
    /// <param name="columnNameAliases">Spaltennamen der CSV-Datei, auf die <see cref="CsvColumnNameProperty"/> zugreifen kann. Für den
    /// Zugriff wird der erste Alias verwendet, der eine Übereinstimmung 
    /// mit einem Spaltennamen der CSV-Datei hat. Die Alias-Strings dürfen die Wildcard-Zeichen * und ? enthalten. Wenn ein 
    /// Wildcard-Alias mit mehreren Spalten der CSV-Datei eine Übereinstimmung hat, wird die Spalte mit dem niedrigsten Index referenziert.</param>
    /// <param name="converter">Der <see cref="ICsvTypeConverter"/>, der die Typkonvertierung übernimmt.</param>
    /// <param name="wildcardTimeout">Timeout-Wert in Millisekunden oder 0, für <see cref="Regex.InfiniteMatchTimeout"/>. 
    /// Ist der Wert größer als <see cref="MaxWildcardTimeout"/>, wird er auf diesen Wert normalisiert.
    /// Wenn ein Alias in <paramref name="columnNameAliases"/> ein
    /// Wildcard-Zeichen enthält, wird innerhalb
    /// dieses Timeouts versucht, den Alias aufzulösen. Gelingt dies nicht, reagiert <see cref="CsvColumnNameProperty"/> so, als hätte sie
    /// kein Ziel in den Spalten der CSV-Datei. (In .Net-Framework 4.0 wird kein Timeout angewendet.)</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder <paramref name="columnNameAliases"/> oder 
    /// <paramref name="converter"/> ist <c>null</c>.</exception>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
    /// ASCII-Zeichen).</exception>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="wildcardTimeout"/> ist kleiner als 0.</exception>
    public CsvColumnNameProperty(
#if NET40
#pragma warning restore CS1574 // XML-Kommentar weist ein cref-Attribut auf, das nicht aufgelöst werden konnte.
#endif
            string propertyName, IEnumerable<string> columnNameAliases, ICsvTypeConverter converter, int wildcardTimeout = 10) : base(propertyName, converter)
    {

        if (columnNameAliases is null)
        {
            throw new ArgumentNullException(nameof(columnNameAliases));
        }


        if (wildcardTimeout > MaxWildcardTimeout)
        {
            wildcardTimeout = MaxWildcardTimeout;
        }
        else if (wildcardTimeout < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(wildcardTimeout));
        }

        this.ColumnNameAliases = new ReadOnlyCollection<string>(columnNameAliases.Where(x => x != null).ToArray());
        this._wildcardTimeout = wildcardTimeout;
    }


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
    /// Da beim Setzen des Wertes von <see cref="CsvColumnNameProperty"/> alle Aliase berücksichtigt werden, empfiehlt es sich nicht, mehreren
    /// <see cref="CsvRecord"/>-Objekten denselben Alias zuzuweisen.</para>
    /// </remarks>
    public ReadOnlyCollection<string> ColumnNameAliases { get; }

    /// <summary>
    /// Ein Hashcode, der für alle <see cref="CsvRecord"/>-Objekte, die zum selben Lese- oder Schreibvorgang
    /// gehören, identisch ist. (Wird von <see cref="CsvColumnNameProperty"/> verwendet, um festzustellen,
    /// ob der Zugriffsindex aktuell ist.)
    /// </summary>
    private int CsvRecordIdentifier { get; set; }

    
    /// <inheritdoc/>
    protected override void UpdateReferredCsvColumnIndex()
    {
        Debug.Assert(Record is not null);
        if (CsvRecordIdentifier != Record.Identifier)
        {
            CsvRecordIdentifier = Record.Identifier;
            ReferredCsvColumnIndex = GetReferredIndex(); ;
        }
    }

    #region private
    private int? GetReferredIndex()
    {
        Debug.Assert(Record is not null);

        IEqualityComparer<string>? comparer = Record.Comparer;
        ReadOnlyCollection<string>? columnNames = Record.ColumnNames;

        for (int i = 0; i < ColumnNameAliases.Count; i++)
        {
            string alias = ColumnNameAliases[i];

            if (alias is null)
            {
                continue;
            }

            if (HasWildcard(alias))
            {
#if NET40
                Regex regex = InitRegex(comparer, alias);
#else
                Regex regex = InitRegex(comparer, alias, _wildcardTimeout);
#endif

                for (int k = 0; k < columnNames.Count; k++) // Die Wildcard könnte auf alle keys passen.
                {
                    string columnName = columnNames[k];

                    try
                    {
                        if (regex.IsMatch(columnName))
                        {
                            return k;
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
                        return j;
                    }
                }
            }
        }//for
        return null;

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
    }


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


    #endregion
}
