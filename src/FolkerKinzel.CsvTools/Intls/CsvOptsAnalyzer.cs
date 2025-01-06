using System.Runtime.InteropServices;

namespace FolkerKinzel.CsvTools.Intls;

internal static class CsvOptsAnalyzer
{
    internal static void InitProperties(CsvStringReader csvStringReader,
                                        int maxLines,
                                        CsvAnalyzerResult results)
    {
        int analyzedLinesCount = 0;
        int firstLineCount = 0;
        CsvRow? row;

        bool hasEmptyLine = false;

        try
        {
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
        catch (CsvFormatException e)
        {
            if (e.Error == CsvError.FileTruncated)
            {
                // This can only happen at EOF.
                // A repeated parsing is not required.
                results.Options = results.Options.Unset(CsvOpts.ThrowOnTruncatedFiles);
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

            if ((mem.Span.IsWhiteSpace() && i != csvRow.Count - 1) || mem.Span.ContainsAny(results.Delimiter, '\r', '\n'))
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

            if (trimmed.Length != mem.Length)
            {
                results.Options = results.Options.Set(CsvOpts.TrimColumns);
            }
        }//for

        results.ColumnNames = csvRow.Where(x => !x.Span.IsWhiteSpace()).Select(x => x.ToString()).ToArray();

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