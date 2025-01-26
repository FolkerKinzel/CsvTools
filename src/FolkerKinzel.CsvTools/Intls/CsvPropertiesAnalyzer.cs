using System.Runtime.InteropServices;

namespace FolkerKinzel.CsvTools.Intls;

internal static class CsvPropertiesAnalyzer
{
    internal static void InitProperties(CsvStringReader csvStringReader,
                                        int maxLines,
                                        Header header,
                                        CsvAnalyzerResult result)
    {
        int analyzedLinesCount = 0;
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
                    result.Options = result.Options.Unset(CsvOpts.ThrowOnEmptyLines);
                }

                analyzedLinesCount++;

                if (analyzedLinesCount == 1)
                {
                    result.RowLength = row.Count;

                    if (header != Header.Absent)
                    {
                        ParseHeader(row, header, result);
                    }
                }
                else if (row.Count != result.RowLength)
                {
                    if (row.Count < result.RowLength)
                    {
                        result.Options = result.Options.Unset(CsvOpts.ThrowOnTooFewFields);
                    }
                    else
                    {
                        if (result.IsHeaderPresent)
                        {
                            result.Options = result.Options.Unset(CsvOpts.ThrowOnTooMuchFields);
                        }
                        else
                        {
                            result.Options = result.Options.Unset(CsvOpts.ThrowOnTooFewFields);
                            result.RowLength = row.Count;
                        }
                    }
                }
            }
        }
        catch (CsvFormatException e)
        {
            if (e.Error == CsvError.FileTruncated)
            {
                // This can only happen at EOF.
                // A repeated parsing is not required.
                result.Options = result.Options.Unset(CsvOpts.ThrowOnTruncatedFiles);
            }
        }
    }

    private static void ParseHeader(CsvRow csvRow, Header header, CsvAnalyzerResult results)
    {
        Debug.Assert(header != Header.Absent);
#if NET8_0_OR_GREATER
        Span<ReadOnlyMemory<char>> row = CollectionsMarshal.AsSpan(csvRow);
#else
        CsvRow row = csvRow;
#endif
        for (int i = 0; i < csvRow.Count; i++)
        {
            ReadOnlyMemory<char> mem = row[i];

            if (header == Header.ProbablyPresent && ((mem.Span.IsWhiteSpace() && i != csvRow.Count - 1) || mem.Span.ContainsAny([results.Delimiter, '\"', '\r', '\n'])))
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

        results.ColumnNames = csvRow.Select(x => x.Span.IsWhiteSpace() ? null : x.ToString()).ToArray();

        string[] columnNamesWithoutWS = results.ColumnNames.OfType<string>().ToArray();

        if (columnNamesWithoutWS.Length == columnNamesWithoutWS.Distinct(StringComparer.Ordinal).Count())
        {
            if (columnNamesWithoutWS.Length != columnNamesWithoutWS.Distinct(StringComparer.OrdinalIgnoreCase).Count())
            {
                results.Options = results.Options.Set(CsvOpts.CaseSensitiveKeys);
            }
        }
        else if (header == Header.ProbablyPresent) // duplicate column names: no header
        {
            results.ColumnNames = null;
            results.Options = results.Options.Unset(CsvOpts.TrimColumns);
        }
    }
}