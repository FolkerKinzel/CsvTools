using System.Text;

namespace FolkerKinzel.CsvTools;

public static class Csv
{
    /// <summary>The newline characters to use in CSV files ("\r\n").</summary>
    public const string NewLine = "\r\n";


    public static (Encoding, CsvAnalyzerResult) Analyze(string fileName, int analizedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        Encoding? encoding = TextEncodingConverter.TryGetEncoding(GetCodePage(fileName), out encoding)
                                  ? encoding
                                  : Encoding.UTF8;

        CsvAnalyzerResult results = CsvAnalyzer.Analyze(fileName, analizedLines, encoding);

        return (encoding, results);
    }

    public static CsvEnumerator OpenReadAnalyzed(string fileName,
                                                 int analizedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        (Encoding encoding, CsvAnalyzerResult results) = Analyze(fileName, analizedLines);
        return new(fileName, results.HasHeaderRow, results.Options, results.Delimiter, encoding);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvEnumerator OpenRead(string fileName,
                                         bool hasHeaderRow = true,
                                         CsvOpts options = CsvOpts.Default,
                                         char delimiter = ',',
                                         Encoding? textEncoding = null)
        => new(fileName, hasHeaderRow, options, delimiter, textEncoding);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvEnumerator OpenRead(TextReader reader,
                                         bool hasHeaderRow = true,
                                         CsvOpts options = CsvOpts.Default,
                                         char delimiter = ',')
        => new(reader, hasHeaderRow, options, delimiter);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter OpenWrite(string fileName,
                                      IEnumerable<string?> columnNames,
                                      CsvOpts options = CsvOpts.Default,
                                      Encoding? textEncoding = null,
                                      char delimiter = ',')
        => new(fileName, columnNames, options, textEncoding, delimiter);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter OpenWrite(TextWriter writer,
                                      IEnumerable<string?> columnNames,
                                      CsvOpts options = CsvOpts.Default,
                                      char delimiter = ',')
        => new(writer, columnNames, options, delimiter);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter OpenWrite(string fileName,
                                      int columnsCount,
                                      CsvOpts options = CsvOpts.Default,
                                      Encoding? textEncoding = null,
                                      char delimiter = ',')
        => new(fileName, columnsCount, options, textEncoding, delimiter);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter OpenWrite(TextWriter writer,
                                      int columnsCount,
                                      CsvOpts options = CsvOpts.Default,
                                      char delimiter = ',')
        => new(writer, columnsCount, options, delimiter);

    public static void Write(TextWriter writer,
                             CsvData data,
                             IEnumerable<string?> columnNames,
                             CsvOpts options = CsvOpts.Default,
                             char delimiter = ',')
    {
        using CsvWriter csvWriter = new(writer, columnNames, options, delimiter);
        DoWrite(csvWriter, data);
    }

    public static void Write(string fileName,
                             CsvData data,
                             IEnumerable<string?> columnNames,
                             CsvOpts options = CsvOpts.Default,
                             Encoding? textEncoding = null,
                             char delimiter = ',')
    {
        using CsvWriter writer = new(fileName, columnNames, options, textEncoding, delimiter);
        DoWrite(writer, data);
    }

    public static void Write(string fileName,
                             CsvData data,
                             int columnsCount,
                             CsvOpts options = CsvOpts.Default,
                             Encoding? textEncoding = null,
                             char delimiter = ',')
    {
        using CsvWriter writer = new(fileName, columnsCount, options, textEncoding, delimiter);
        DoWrite(writer, data);
    }

    public static void Write(TextWriter writer,
                             CsvData data,
                             int columnsCount,
                             CsvOpts options = CsvOpts.Default,
                             char delimiter = ',')
    {
        using CsvWriter csvWriter = new(writer, columnsCount, options, delimiter);
        DoWrite(csvWriter, data);
    }

    private static int GetCodePage(string fileName)
    {
        const int BUF_LENGTH = 4;

        try
        {
            using FileStream fs = File.OpenRead(fileName);

#if NET8_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            Span<byte> buf = stackalloc byte[BUF_LENGTH];
            fs.Read(buf);
#else
            var buf = new byte[BUF_LENGTH];
            fs.Read(buf, 0, buf.Length);
#endif
            return TextEncodingConverter.GetCodePage(buf, out _);
        }
        catch
        {
            return Encoding.UTF8.CodePage;
        }
    }

    private static void DoWrite(CsvWriter writer, CsvData values)
    {
        foreach (IEnumerable<ReadOnlyMemory<char>> record in values)
        {
            writer.Record.DoFill(record);
            writer.Write();
        }
    }
}
