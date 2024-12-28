using System.Text;

namespace FolkerKinzel.CsvTools;

public static class Csv
{
    /// <summary>The newline characters to use in CSV files ("\r\n").</summary>
    public const string NewLine = "\r\n";

    /// <summary> Analyzes the CSV file referenced by <paramref name="fileName" />.
    /// </summary>
    /// <param name="fileName">File path of the CSV file.</param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)</param>
    /// 
    /// <returns>The results of the analysis.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// The method performs a statistical analysis on the CSV file to find the appropriate 
    /// parameters for reading the file. The result of the analysis is therefore always only an estimate, 
    /// the accuracy of which increases with the number of lines analyzed.
    /// </para>
    /// <para>
    /// The analysis is time-consuming because the CSV file has to be accessed for reading.
    /// </para>
    /// <para>
    /// This method also automatically determines the <see cref="Encoding"/> of the CSV file.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="fileName" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="fileName" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">Error accessing the file.</exception>
    public static (Encoding, CsvAnalyzerResult) Analyze(string fileName, int analizedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        Encoding? encoding = TextEncodingConverter.TryGetEncoding(GetCodePage(fileName), out encoding)
                                  ? encoding
                                  : Encoding.UTF8;

        CsvAnalyzerResult results = CsvAnalyzer.Analyze(fileName, analizedLines, encoding);

        return (encoding, results);
    }

    /// <summary>This method analyzes the CSV file referenced by <paramref name="fileName" />
    /// first and then opens a <see cref="CsvEnumerator"/> to read its content.
    /// </summary>
    /// <param name="fileName">File path of the CSV file.</param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)</param>
    /// 
    /// <returns>An <see cref="CsvEnumerator"/> that allows to iterate the data.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// The method performs a statistical analysis on the CSV file to find the appropriate 
    /// parameters for reading the file. The result of the analysis is therefore always only an estimate, 
    /// the accuracy of which increases with the number of lines analyzed.
    /// </para>
    /// <para>
    /// The analysis is time-consuming because the CSV file has to be accessed for reading.
    /// </para>
    /// <para>
    /// This method also automatically determines the <see cref="Encoding"/> of the CSV file.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="fileName" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="fileName" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">Error accessing the file.</exception>
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
