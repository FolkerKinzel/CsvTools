using System.Runtime.InteropServices;
using System.Text;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary> <see cref="CsvAnalyzer" /> can perform an analysis on a 
/// CSV file.</summary>
public static class CsvAnalyzer
{
    /// <summary>Minimum number of lines in the CSV file to be analyzed.</summary>
    public const int AnalyzedLinesMinCount = 5;

    /// <summary> Analyzes the CSV file referenced by <paramref name="filePath" />.
    /// </summary>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)</param>
    /// <param name="textEncoding">
    /// <para>
    /// The text encoding to be used to read the CSV file or <c>null</c> for <see cref="Encoding.UTF8" />.
    /// </para>
    /// <note type="tip">
    /// Use <see cref="Csv.Analyze(string, int)"/> to also automatically determine the <see cref="Encoding"/>.
    /// </note>
    /// </param>
    /// <returns>The results of the analysis.</returns>
    /// 
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
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">Error accessing the file.</exception>
    public static CsvAnalyzerResult Analyze(string filePath,
                                            int analyzedLines = AnalyzedLinesMinCount,
                                            Encoding? textEncoding = null)
    {
        CsvAnalyzerResult results = new();

        if (analyzedLines < AnalyzedLinesMinCount)
        {
            analyzedLines = AnalyzedLinesMinCount;
        }

        results.Delimiter = new FieldSeparatorAnalyzer().GetFieldSeparator(filePath, textEncoding);

repeat:
        try
        {
            InitProperties(filePath,
                           textEncoding,
                           analyzedLines,
                           results);
        }
        catch (CsvFormatException e)
        {
            if (e.Error == CsvError.FileTruncated)
            {
                results.Options = results.Options.Unset(CsvOpts.ThrowOnTruncatedFiles);
                goto repeat;
            }
        }

        return results;
    }

    private static void InitProperties(string fileName,
                                       Encoding? textEncoding,
                                       int maxLines,
                                       CsvAnalyzerResult results)
    {
        int analyzedLinesCount = 0;
        int firstLineCount = 0;
        CsvRow? row;

        using StreamReader reader = StreamReaderHelper.InitializeStreamReader(fileName, textEncoding);
        using var csvStringReader = new CsvStringReader(reader, results.Delimiter, results.Options);

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
                results.Options = results.Options.Unset(CsvOpts.ThrowOnEmptyLines);
            }

            analyzedLinesCount++;

            if (analyzedLinesCount == 1)
            {
                firstLineCount = row.Count;
                ParseFirstLine(row, results);
            }
            else if (row.Count != firstLineCount)
            {
                results.Options = row.Count < firstLineCount
                    ? results.Options.Unset(CsvOpts.ThrowOnTooFewFields)
                    : results.Options.Unset(CsvOpts.ThrowOnTooMuchFields);
            }
        }
    }

    private static void ParseFirstLine(CsvRow csvRow, CsvAnalyzerResult results)
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
                results.ColumnNames = null;
                results.Options = results.Options.Unset(CsvOpts.TrimColumns);
                return;
            }

            ReadOnlyMemory<char> trimmed = mem.Trim();

            row[i] = trimmed;

            if (trimmed.Length != mem.Length && !results.Options.HasFlag(CsvOpts.TrimColumns))
            {
                results.Options = results.Options.Set(CsvOpts.TrimColumns);
            }
        }//for

        results.ColumnNames = csvRow.Where(x => !x.IsEmpty).Select(x => x.ToString()).ToArray();

        if (results.ColumnNames.Count == results.ColumnNames.Distinct(StringComparer.Ordinal).Count())
        {
            if (results.ColumnNames.Count != results.ColumnNames.Distinct(StringComparer.OrdinalIgnoreCase).Count())
            {
                results.Options = results.Options.Set(CsvOpts.CaseSensitiveKeys);
            }
        }
        else // duplicate column names: no header
        {
            results.ColumnNames = null;
            results.Options = results.Options.Unset(CsvOpts.TrimColumns);
        }
    }
}
