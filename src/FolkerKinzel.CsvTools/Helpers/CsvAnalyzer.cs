using System.Collections.ObjectModel;
using System.Text;
using FolkerKinzel.CsvTools.Extensions;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools.Helpers;

/// <summary>
/// <see cref="CsvAnalyzer"/> kann eine statistische Analyse auf einer CSV-Datei ausführen und
/// die Ergebnisse als Objekteigenschaften zur Verfügung stellen.
/// </summary>
/// <example>
/// <note type="note">Im folgenden Code-Beispiel wurde - der leichteren Lesbarkeit wegen - auf Ausnahmebehandlung verzichtet.</note>
/// <para>Deserialisieren beliebiger Objekte aus CSV-Dateien:</para>
/// <code language="cs" source="..\Examples\DeserializingClassesFromCsv.cs"/>
/// </example>
public class CsvAnalyzer
{
    /// <summary>
    /// Mindestanzahl der zu untersuchenden Zeilen der CSV-Datei.
    /// </summary>
    public const int AnalyzedLinesMinCount = 5;

    /// <summary>
    /// Initialisiert ein neues <see cref="CsvAnalyzer"/>-Objekt.
    /// </summary>
    public CsvAnalyzer()
    {

    }

    /// <summary>
    /// Analysiert die CSV-Datei, auf die <paramref name="fileName"/> verweist, und füllt die Eigenschaften des <see cref="CsvAnalyzer"/>-Objekts mit den
    /// Ergebnissen der Analyse.
    /// </summary>
    /// <param name="fileName">Dateipfad der CSV-Datei.</param>
    /// <param name="analyzedLinesCount">Höchstanzahl der in der CSV-Datei zu analysierenden Zeilen. Der Mindestwert
    /// ist <see cref="AnalyzedLinesMinCount"/>. Wenn die Datei weniger Zeilen hat als <paramref name="analyzedLinesCount"/>
    /// wird sie komplett analysiert. (Sie können <see cref="int.MaxValue">Int32.MaxValue</see> angeben, um in jedem Fall die gesamte Datei zu
    /// analysieren.)</param>
    /// <param name="textEncoding">Die zum Einlesen der CSV-Datei zu verwendende Textkodierung oder <c>null</c> für <see cref="Encoding.UTF8"/>.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> ist kein gültiger Dateipfad.</exception>
    /// <exception cref="IOException">Es kann nicht auf den Datenträger zugegriffen werden.</exception>
    /// <remarks><para><see cref="CsvAnalyzer"/> führt auf der CSV-Datei eine statistische Analyse durch, um die geeigneten
    /// Parameter für das Lesen der Datei zu finden. Das Ergebnis der Analyse ist also immer nur eine Schätzung, deren
    /// Treffsicherheit mit der Zahl der analysierten Zeilen steigt.</para>
    /// <para>Die Analyse ist zeitaufwändig, da auf die CSV-Datei lesend zugegriffen werden muss.</para></remarks>
    public void Analyze(string fileName, int analyzedLinesCount = AnalyzedLinesMinCount, Encoding? textEncoding = null)
    {
        if (analyzedLinesCount < AnalyzedLinesMinCount)
        {
            analyzedLinesCount = AnalyzedLinesMinCount;
        }

        // Suche Feldtrennzeichen:
        using (StreamReader? reader = CsvReader.InitializeStreamReader(fileName, textEncoding))
        {
            const int COMMA_INDEX = 0;
            const int SEMICOLON_INDEX = 1;
            const int HASH_INDEX = 2;

            bool firstLine = true;

#if NET40
                char[] sepChars = new char[] { ',', ';', '#',  '\t', ' '};

                int sepCharsLength = sepChars.Length;

                int[] firstLineOccurrence = new int[sepCharsLength];
                int[] sameOccurrence = new int[sepCharsLength];
                int[] currentLineOccurrence = new int[sepCharsLength];
#else
            ReadOnlySpan<char> sepChars = stackalloc[] { ',', ';', '#', '\t', ' ' };
            int sepCharsLength = sepChars.Length;
            Span<int> firstLineOccurrence = stackalloc int[sepCharsLength];
            firstLineOccurrence.Clear();
            Span<int> sameOccurrence = stackalloc int[sepCharsLength];
            sameOccurrence.Clear();
            Span<int> currentLineOccurrence = stackalloc int[sepCharsLength];
            currentLineOccurrence.Clear();
#endif

            for (int i = 0; i < analyzedLinesCount; i++)
            {
                string? line = reader.ReadLine();

                if (line is null)
                {
                    break;
                }

                if (firstLine)
                {
                    // Skip empty lines before the first line:
                    if (line.Length == 0)
                    {
                        Options = Options.Unset(CsvOptions.ThrowOnEmptyLines);
                        i--;
                        continue;
                    }

                    firstLine = false;

                    // Vergleich für Kopfzeile:
                    for (int charIndex = 0; charIndex < line.Length; charIndex++)
                    {
                        char ch = line[charIndex];

                        for (int sepCharIndex = 0; sepCharIndex < sepCharsLength; sepCharIndex++)
                        {
                            if (ch.Equals(sepChars[sepCharIndex]))
                            {
                                firstLineOccurrence[sepCharIndex]++;
                            }
                        }
                    }

                    // wenn in der Kopfzeile Komma, Semikolon oder Raute auftauchen, werden Tabulator und Leerzeichen nicht mehr ausgewertet
                    if (firstLineOccurrence[COMMA_INDEX] != 0 || firstLineOccurrence[SEMICOLON_INDEX] != 0 || firstLineOccurrence[HASH_INDEX] != 0)
                    {
                        // lösche Whitespace-Werte
                        for (int j = 3; j < sepCharsLength; j++)
                        {
                            firstLineOccurrence[j] = 0;
                        }

                        sepCharsLength = 3;
                    }

                    continue;
                }



                // Vergleich für Datenzeile:
                for (int charIndex = 0; charIndex < line.Length; charIndex++)
                {
                    char ch = line[charIndex];

                    for (int sepCharIndex = 0; sepCharIndex < sepCharsLength; sepCharIndex++)
                    {
                        if (ch.Equals(sepChars[sepCharIndex]))
                        {
                            currentLineOccurrence[sepCharIndex]++;
                        }
                    }
                }

                for (int sepCharIndex = 0; sepCharIndex < sepCharsLength; sepCharIndex++)
                {
                    if (currentLineOccurrence[sepCharIndex] == firstLineOccurrence[sepCharIndex])
                    {
                        sameOccurrence[sepCharIndex]++;
                    }
                }

                // Clear currentLineOccurrence
                for (int sepCharIndex = 0; sepCharIndex < sepCharsLength; sepCharIndex++)
                {
                    currentLineOccurrence[sepCharIndex] = 0;
                }
            }//for

            this.FieldSeparator = sepChars[0];

            int sameOcc = sameOccurrence[0];
            int probability = firstLineOccurrence[0] * (1 + sameOcc * sameOcc * sameOcc);

            // Formel für statistische Wahrscheinlichkeit:
            for (int sepCharIndex = 1; sepCharIndex < sepCharsLength; sepCharIndex++)
            {
                sameOcc = sameOccurrence[sepCharIndex];

                int newProbability = firstLineOccurrence[sepCharIndex] * (1 + sameOcc * sameOcc * sameOcc);

                if (newProbability > probability)
                {
                    this.FieldSeparator = sepChars[sepCharIndex];
                    probability = newProbability;
                }
            }

        }//using

        using (StreamReader? reader = CsvReader.InitializeStreamReader(fileName, textEncoding))
        {
            bool firstLine = true;

            int firstLineCount = 0;

            using var csvStringReader = new CsvStringReader(reader, FieldSeparator, !Options.IsSet(CsvOptions.ThrowOnEmptyLines));

            foreach (IEnumerable<string?>? row in csvStringReader)
            {
                if (firstLine)
                {
                    firstLine = false;

                    var firstLineFields = new List<string>();

                    bool hasHeader = true;
                    bool hasMaybeNoHeader = false;

                    foreach (string? s in row)
                    {
                        firstLineCount++;

                        if (hasHeader)
                        {
                            // Nach RFC 4180 darf das letzte Feld einer Datenzeile hinter sich kein
                            // FieldSeparatorChar mehr haben: "The last field in the
                            // record must not be followed by a comma."
                            // Schlechte Implementierungen - wie Thunderbird - halten sich aber nicht daran.
                            if (hasMaybeNoHeader)
                            {
                                hasHeader = false;
                                this.Options = this.Options.Unset(CsvOptions.TrimColumns);
                            }

                            if (s is null)
                            {
                                hasMaybeNoHeader = true;
                                continue;
                            }

                            string trimmed = s.Trim();

                            firstLineFields.Add(trimmed);

                            if (trimmed.Length != s.Length)
                            {
                                this.Options = this.Options.Set(CsvOptions.TrimColumns);
                            }
                        }
                    }//foreach

                    if (hasHeader)
                    {
                        this.ColumnNames = new ReadOnlyCollection<string>(firstLineFields);
                    }

                    // Prüfe, ob sich zwei Spaltennamen nur durch Groß- und Kleinschreibung unterscheiden:
                    if (hasHeader)
                    {
                        // estimatedLength unterscheidet sich von firstLineCount, wenn nach dem letzten Feld
                        // ein FieldSeparatorChar stand (s.o.)
                        int estimatedLength = hasMaybeNoHeader ? firstLineCount - 1 : firstLineCount;

                        if (estimatedLength != firstLineFields.Distinct(StringComparer.OrdinalIgnoreCase).Count())
                        {
                            this.Options = this.Options.Set(CsvOptions.CaseSensitiveKeys);
                        }
                    }
                }
                else
                {
                    int currentLineCount = 0;
                    string? firstString = null;

                    foreach (string? s in row)
                    {
                        if (currentLineCount == 0)
                        {
                            firstString = s;
                        }

                        currentLineCount++;
                    }

                    if (currentLineCount != firstLineCount)
                    {
                        this.Options = currentLineCount < firstLineCount
                            ? currentLineCount == 1 && firstString is null ? this.Options.Unset(CsvOptions.ThrowOnEmptyLines) : this.Options.Unset(CsvOptions.ThrowOnTooFewFields)
                            : this.Options.Unset(CsvOptions.ThrowOnTooMuchFields);
                    }
                }
            }
        }//using
    }


    /// <summary>
    /// Das Feldtrennzeichen der CSV-Datei.
    /// </summary>
    public char FieldSeparator { get; private set; } = ',';

    /// <summary>
    /// Optionen für das Lesen der CSV-Datei.
    /// </summary>
    public CsvOptions Options { get; private set; } = CsvOptions.Default;

    /// <summary>
    /// <c>true</c>, wenn die CSV-Datei eine Kopfzeile hat.
    /// </summary>
    public bool HasHeaderRow => ColumnNames != null;

    /// <summary>
    /// Spaltennamen der CSV-Datei.
    /// </summary>
    public ReadOnlyCollection<string>? ColumnNames { get; private set; }
}//class
