using System.Collections.ObjectModel;
using System.Text;
using FolkerKinzel.CsvTools.Intls;

#if NET462 || NETSTANDARD2_0 || NETSTANDARD2_1
using FolkerKinzel.Strings;
#endif

namespace FolkerKinzel.CsvTools;

/// <summary> <see cref="CsvAnalyzer" /> can perform a statistical analysis on a 
/// CSV file and provide the results as its properties.</summary>
/// <example>
/// <note type="note">
/// In the following code example - for easier readability - exception handling
/// has been omitted.
/// </note>
/// <para>
/// Deserialization of any objects from CSV files:
/// </para>
/// <code language="cs" source="..\Examples\DeserializingClassesFromCsv.cs" />
/// </example>
public class CsvAnalyzer
{
    /// <summary>Minimum number of lines in the CSV file to be analyzed.</summary>
    public const int AnalyzedLinesMinCount = 5;

    /// <summary> Initializes a new <see cref="CsvAnalyzer" /> instance. </summary>
    public CsvAnalyzer() { }

    /// <summary> Parses the CSV file referenced by <paramref name="fileName" /> and populates 
    /// the properties of the <see cref="CsvAnalyzer" /> object with the results of the analysis.
    /// </summary>
    /// <param name="fileName">File path of the CSV file.</param>
    /// <param name="analyzedLinesCount">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLinesCount" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file
    /// or <c>null</c> for <see cref="Encoding.UTF8" />.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileName" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="fileName" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">Error accessing the file.</exception>
    /// <remarks>
    /// <para>
    /// <see cref="CsvAnalyzer" /> performs a statistical analysis on the CSV file to find the appropriate 
    /// parameters for reading the file. The result of the analysis is therefore always only an estimate, 
    /// the accuracy of which increases with the number of lines analyzed.
    /// </para>
    /// <para>
    /// The analysis is time-consuming because the CSV file has to be accessed for reading.
    /// </para>
    /// </remarks>
    public void Analyze(string fileName, int analyzedLinesCount = AnalyzedLinesMinCount, Encoding? textEncoding = null)
    {
        if (analyzedLinesCount < AnalyzedLinesMinCount)
        {
            analyzedLinesCount = AnalyzedLinesMinCount;
        }

        // Suche Feldtrennzeichen:

/* Unmerged change from project 'FolkerKinzel.CsvTools (netstandard2.1)'
Before:
        using (StreamReader? reader = CsvReader.InitializeStreamReader(fileName, textEncoding))
        {
After:
        using (StreamReader? reader = CsvTools.StreamReaderHelper.InitializeStreamReader(fileName, textEncoding))
        {
*/
        using (StreamReader? reader = StreamReaderHelper.InitializeStreamReader(fileName, textEncoding))
        {
            const int COMMA_INDEX = 0;
            const int SEMICOLON_INDEX = 1;
            const int HASH_INDEX = 2;

            bool firstLine = true;

            const string sepChars = ",;#\t ";
            int sepCharsLength = sepChars.Length;
            Span<int> firstLineOccurrence = stackalloc int[sepCharsLength];
            firstLineOccurrence.Clear();
            Span<int> sameOccurrence = stackalloc int[sepCharsLength];
            sameOccurrence.Clear();
            Span<int> currentLineOccurrence = stackalloc int[sepCharsLength];
            currentLineOccurrence.Clear();

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


/* Unmerged change from project 'FolkerKinzel.CsvTools (netstandard2.1)'
Before:
        using (StreamReader? reader = CsvReader.InitializeStreamReader(fileName, textEncoding))
        {
After:
        using (StreamReader? reader = CsvTools.StreamReaderHelper.InitializeStreamReader(fileName, textEncoding))
        {
*/
        using (StreamReader? reader = StreamReaderHelper.InitializeStreamReader(fileName, textEncoding))
        {
            bool firstLine = true;

            int firstLineCount = 0;

            using var csvStringReader = new CsvStringReader(reader, FieldSeparator, !Options.HasFlag(CsvOptions.ThrowOnEmptyLines));

            IList<ReadOnlyMemory<char>>? row;
            while((row = csvStringReader.Read()) is not null)
            {
                if (firstLine)
                {
                    firstLine = false;

                    //var firstLineFields = new List<string>();

                    bool hasHeader = true;
                    bool hasMaybeNoHeader = false;
                    firstLineCount = row.Count;

                    for (int i = 0; i < row.Count; i++)
                    {
                        ReadOnlyMemory<char> mem = row[i];

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

                            if (mem.IsEmpty)
                            {
                                hasMaybeNoHeader = true;
                                continue;
                            }

                            ReadOnlyMemory<char> trimmed = mem.Trim();

                            row[i] = trimmed;

                            if (trimmed.Length != mem.Length)
                            {
                                Options = Options.Set(CsvOptions.TrimColumns);
                            }
                        }
                    }//for

                    if (hasHeader)
                    {
                        ColumnNames = new ReadOnlyCollection<string>(row.Where(x => !x.IsEmpty).Select(x => x.ToString()).ToArray());

                        if(ColumnNames.Count == ColumnNames.Distinct(StringComparer.Ordinal).Count())
                        {
                            if (ColumnNames.Count != ColumnNames.Distinct(StringComparer.OrdinalIgnoreCase).Count())
                            {
                                this.Options = this.Options.Set(CsvOptions.CaseSensitiveKeys);
                            }
                        }
                        else
                        {
                            hasHeader = false;
                            ColumnNames = null;
                            Options = Options.Unset(CsvOptions.TrimColumns);
                        }
                    }
                }
                else
                {
                    int currentLineCount = 0;
                    ReadOnlyMemory<char> firstString = default;

                    foreach (ReadOnlyMemory<char> mem in row)
                    {
                        if (currentLineCount == 0)
                        {
                            firstString = mem;
                        }

                        currentLineCount++;
                    }

                    if (currentLineCount != firstLineCount)
                    {
                        this.Options = currentLineCount < firstLineCount
                            ? currentLineCount == 1 && firstString.IsEmpty ? this.Options.Unset(CsvOptions.ThrowOnEmptyLines) : this.Options.Unset(CsvOptions.ThrowOnTooFewFields)
                            : this.Options.Unset(CsvOptions.ThrowOnTooMuchFields);
                    }
                }
            }
        }//using
    }

    /// <summary>The field separator char used in the CSV file.</summary>
    public char FieldSeparator { get; private set; } = ',';

    /// <summary>Options for reading the CSV file.</summary>
    public CsvOptions Options { get; private set; } = CsvOptions.Default;

    /// <summary> <c>true</c>, if the CSV file has a header with column names.</summary>
    public bool HasHeaderRow => ColumnNames != null;

    /// <summary>The column names of the CSV file.</summary>
    public ReadOnlyCollection<string>? ColumnNames { get; private set; }
}//class
