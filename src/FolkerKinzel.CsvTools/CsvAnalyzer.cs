using System.Collections.ObjectModel;
using System.Text;
using FolkerKinzel.CsvTools.Intls;

#if NET462 || NETSTANDARD2_0 || NETSTANDARD2_1
using FolkerKinzel.Strings;
#endif

namespace FolkerKinzel.CsvTools;

/// <summary> <see cref="CsvAnalyzer" /> can perform a statistical analysis on a 
/// CSV file and provide the results as its properties.</summary>
public partial class CsvAnalyzer
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

        new FieldSeparatorAnalyzer(this).InitFieldSeparator(fileName);
        //InitFieldSeparator(fileName, analyzedLinesCount, textEncoding);
        InitProperties(fileName, textEncoding, analyzedLinesCount);
    }

    private void InitProperties(string fileName, Encoding? textEncoding, int maxCount)
    {
        int analyzedLinesCount = 0;
        int firstLineCount = 0;
        List<ReadOnlyMemory<char>>? row;

        using StreamReader reader = StreamReaderHelper.InitializeStreamReader(fileName, textEncoding);
        using var csvStringReader = new CsvStringReader(reader, FieldSeparator, !Options.HasFlag(CsvOptions.ThrowOnEmptyLines));

        while ((row = csvStringReader.Read()) is not null && analyzedLinesCount < maxCount)
        {
            analyzedLinesCount++;

            if (analyzedLinesCount == 1)
            {
                firstLineCount = ParseFirstLine(row);
            }
            else
            {
                SetOptions(firstLineCount, row);
            }
        }
    }

    private int ParseFirstLine(List<ReadOnlyMemory<char>> row)
    {
        int firstLineCount;
        bool hasHeader = true;
        bool hasMaybeNoHeader = false;
        firstLineCount = row.Count;

        for (int i = 0; i < row.Count; i++)
        {
            ReadOnlyMemory<char> mem = row[i];

            if (hasHeader)
            {
                // RFC 4180 says: "The last field in the
                // record must not be followed by a comma."
                // Bad implementations - like Thunderbird - do other.
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
            ColumnNames = row.Where(x => !x.IsEmpty).Select(x => x.ToString()).ToArray();

            if (ColumnNames.Count == ColumnNames.Distinct(StringComparer.Ordinal).Count())
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

        return firstLineCount;
    }

    private void SetOptions(int firstLineCount, IList<ReadOnlyMemory<char>> row)
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
                ? currentLineCount == 1 && firstString.IsEmpty
                    ? this.Options.Unset(CsvOptions.ThrowOnEmptyLines)
                    : this.Options.Unset(CsvOptions.ThrowOnTooFewFields)
                : this.Options.Unset(CsvOptions.ThrowOnTooMuchFields);
        }
    }

    /// <summary>The field separator char used in the CSV file.</summary>
    public char FieldSeparator { get; private set; } = ',';

    /// <summary>Options for reading the CSV file.</summary>
    public CsvOptions Options { get; private set; } = CsvOptions.Default;

    /// <summary> <c>true</c>, if the CSV file has a header with column names.</summary>
    public bool HasHeaderRow => ColumnNames != null;

    /// <summary>The column names of the CSV file.</summary>
    public IReadOnlyList<string>? ColumnNames { get; private set; }
}//class
