using System.Runtime.InteropServices;
using System.Text;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary> <see cref="CsvAnalyzer" /> can perform a statistical analysis on a 
/// CSV file and provide the results as its properties.</summary>
public partial class CsvAnalyzer
{
    /// <summary>Minimum number of lines in the CSV file to be analyzed.</summary>
    public const int AnalyzedLinesMinCount = 5;

    /// <summary> Initializes a new <see cref="CsvAnalyzer" /> instance. </summary>
    public CsvAnalyzer() { }

    /// <summary>The field separator char used in the CSV file.</summary>
    public char FieldSeparator { get; private set; } = ',';

    /// <summary>Options for reading the CSV file.</summary>
    public CsvOptions Options { get; private set; } = CsvOptions.Default;

    /// <summary> <c>true</c>, if the CSV file has a header with column names.</summary>
    public bool HasHeaderRow => ColumnNames != null;

    /// <summary>The column names of the CSV file.</summary>
    public IReadOnlyList<string>? ColumnNames { get; private set; }

    /// <summary> Parses the CSV file referenced by <paramref name="fileName" /> and populates 
    /// the properties of the <see cref="CsvAnalyzer" /> object with the results of the analysis.
    /// </summary>
    /// <param name="fileName">File path of the CSV file.</param>
    /// <param name="analyzedLinesCount">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLinesCount" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)</param>
    /// <param name="textEncoding">
    /// <para>
    /// The text encoding to be used to read the CSV file
    /// or <c>null</c> for <see cref="Encoding.UTF8" />.
    /// </para>
    /// <note type="tip">
    /// Use <see cref="FolkerKinzel.Strings.TextEncodingConverter.GetCodePage(ReadOnlySpan{byte}, out int)"/>
    /// to determine the code page automatically from the byte order mark (BOM). (It comes with the dependencies.)
    /// </note>
    /// </param>
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

        FieldSeparator = new FieldSeparatorAnalyzer().InitFieldSeparator(fileName);

repeat:
        try
        {
            InitProperties(fileName, textEncoding, analyzedLinesCount, Options);
        }
        catch (CsvFormatException e)
        {
            if (e.Error == CsvError.FileTruncated)
            {
                Options = Options.Unset(CsvOptions.ThrowOnTruncatedFiles);
                goto repeat;
            }

            return;
        }
    }

    private void InitProperties(string fileName, Encoding? textEncoding, int maxLines, CsvOptions options)
    {
        int analyzedLinesCount = 0;
        int firstLineCount = 0;
        CsvRow? row;

        using StreamReader reader = StreamReaderHelper.InitializeStreamReader(fileName, textEncoding);
        using var csvStringReader = new CsvStringReader(reader, FieldSeparator, options);
        bool hasEmptyLine = false;

        while ((row = csvStringReader.Read()) is not null && analyzedLinesCount < maxLines)
        {
            if (row.IsEmpty)
            {
                // Empty lines are not part of the data and should not be counted.
                // If all lines are empty, the file can be read with CsvOptions.Default.
                hasEmptyLine = true;
                continue;
            }

            if (hasEmptyLine)
            {
                Options = Options.Unset(CsvOptions.ThrowOnEmptyLines);
            }

            analyzedLinesCount++;

            if (analyzedLinesCount == 1)
            {
                firstLineCount = row.Count;
                ParseFirstLine(row);
            }
            else
            {
                SetOptions(firstLineCount, row);
            }
        }
    }

    private void ParseFirstLine(CsvRow csvRow)
    {
#if NET8_0_OR_GREATER
        Span<ReadOnlyMemory<char>> row = CollectionsMarshal.AsSpan(csvRow);
#else
        CsvRow row = csvRow;
#endif
        for (int i = 0; i < csvRow.Count; i++)
        {
            ReadOnlyMemory<char> mem = row[i];

            if (mem.IsEmpty && i != csvRow.Count - 1)
            {
                // Has no header if the empty field is not the
                // last field in the record.
                // RFC 4180 says: "The last field in the
                // record must not be followed by a comma."
                // Bad implementations - like Thunderbird - do other.
                ColumnNames = null;
                Options = Options.Unset(CsvOptions.TrimColumns);
                return;
            }

            ReadOnlyMemory<char> trimmed = mem.Trim();

            row[i] = trimmed;

            if (trimmed.Length != mem.Length)
            {
                Options = Options.Set(CsvOptions.TrimColumns);
            }
        }//for

        ColumnNames = csvRow.Where(x => !x.IsEmpty).Select(x => x.ToString()).ToArray();

        if (ColumnNames.Count == ColumnNames.Distinct(StringComparer.Ordinal).Count())
        {
            if (ColumnNames.Count != ColumnNames.Distinct(StringComparer.OrdinalIgnoreCase).Count())
            {
                this.Options = this.Options.Set(CsvOptions.CaseSensitiveKeys);
            }
        }
        else // duplicate column names: no header
        {
            ColumnNames = null;
            Options = Options.Unset(CsvOptions.TrimColumns);
        }
    }

    private void SetOptions(int firstLineCount, CsvRow row)
    {
        if (row.Count != firstLineCount)
        {
            this.Options = row.Count < firstLineCount
                ? this.Options.Unset(CsvOptions.ThrowOnTooFewFields)
                : this.Options.Unset(CsvOptions.ThrowOnTooMuchFields);
        }
    }
}
