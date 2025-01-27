using System.Text;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary>Static class that provides methods for reading and writing CSV files.</summary>
public static class Csv
{
    /// <summary>The newline characters to use in CSV files ("\r\n").</summary>
    public const string NewLine = "\r\n";

    /// <summary> Analyzes the CSV file referenced by <paramref name="filePath" />.
    /// </summary>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// <param name="textEncoding">
    /// The text encoding to be used to read the CSV file, or <c>null</c> to determine the <see cref="Encoding"/>
    /// automatically from the byte order mark (BOM).
    /// </param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If the file has fewer lines than 
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
    /// If the argument of the <paramref name="textEncoding"/> parameter is <c>null</c>, this method 
    /// also automatically 
    /// determines the <see cref="Encoding"/> of the CSV file from the byte order mark (BOM).
    /// <see cref="Encoding.UTF8"/> is used as fallback value if the <see cref="Encoding"/> cannot be 
    /// determined automatically.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="header"/> is not a defined value of 
    /// the <see cref="Header"/> enum.</para>
    /// <para> - or -</para>
    /// <para><paramref name="header"/> is a combination of <see cref="Header"/> values.</para>
    /// </exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">Error accessing the file.</exception>
    public static (CsvAnalyzerResult, Encoding) AnalyzeFile(string filePath,
                                                            Header header = Header.ProbablyPresent,
                                                            Encoding? textEncoding = null,
                                                            int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        if (textEncoding is null)
        {
            _ = TextEncodingConverter.TryGetEncoding(GetCodePage(filePath), out Encoding? encoding);
            textEncoding = encoding;
        }

        CsvAnalyzerResult results = CsvAnalyzer.AnalyzeFile(filePath, textEncoding, header, analyzedLines);
        return (results, textEncoding!);
    }

    /// <summary> Analyzes a <see cref="string"/> that contains CSV data to get the 
    /// appropriate parameters for parsing.</summary>
    /// 
    /// <param name="csv">The CSV-<see cref="string"/> to analyze.</param>
    /// 
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in <paramref name="csv"/>. The minimum 
    /// value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If <paramref name="csv"/> has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire <see cref="string"/> in any case.)</param>
    /// 
    /// <returns>The results of the analysis.</returns>
    /// 
    /// <remarks>
    /// <see cref="CsvAnalyzer" /> performs a statistical analysis on the <see cref="string"/>. The result 
    /// of the analysis is therefore always only an estimate, 
    /// the accuracy of which increases with the number of lines analyzed.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="header"/> is not a defined value of 
    /// the <see cref="Header"/> enum.</para>
    /// <para> - or -</para>
    /// <para><paramref name="header"/> is a combination of <see cref="Header"/> values.</para>
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvAnalyzerResult AnalyzeString(string csv,
                                                  Header header = Header.ProbablyPresent,
                                                  int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount) 
        => CsvAnalyzer.AnalyzeString(csv, header, analyzedLines);

    /// <summary>Analyzes the CSV file referenced by <paramref name="filePath" />
    /// first and then opens a <see cref="CsvReader"/> to read its content.
    /// </summary>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// <param name="textEncoding">
    /// The text encoding to be used to read the CSV file, or <c>null</c> to determine the <see cref="Encoding"/>
    /// automatically from the byte order mark (BOM).
    /// </param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)</param>
    /// <param name="disableCaching"><c>true</c> to set the <see cref="CsvOpts.DisableCaching"/> flag, 
    /// otherwise <c>false</c>.</param>
    /// 
    /// <returns>A <see cref="CsvReader"/> that allows to iterate the data.</returns>
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
    /// This method also automatically determines the <see cref="Encoding"/> of the CSV file from the
    /// byte order mark (BOM) if the argument of the <paramref name="textEncoding"/> parameter is <c>null</c>.
    /// If the <see cref="Encoding"/> cannot be determined automatically, <see cref="Encoding.UTF8"/> is used.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">
    /// In the following code examples - for easier readability - exception handling
    /// has been omitted.
    /// </note>
    /// 
    /// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\CsvExample.cs" />
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="header"/> is not a defined value of 
    /// the <see cref="Header"/> enum.</para>
    /// <para> - or -</para>
    /// <para><paramref name="header"/> is a combination of <see cref="Header"/> values.</para>
    /// </exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">Error accessing the file.</exception>
    public static CsvReader OpenReadAnalyzed(string filePath,
                                             Header header = Header.ProbablyPresent,
                                             Encoding? textEncoding = null,
                                             int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount,
                                             bool disableCaching = false)
    {
        (CsvAnalyzerResult result, Encoding encoding) = AnalyzeFile(filePath, header, textEncoding, analyzedLines);
        result.Options = disableCaching ? result.Options | CsvOpts.DisableCaching : result.Options;
        return result.IsHeaderPresent
            ? new(filePath, isHeaderPresent: true, result.Options, result.Delimiter, encoding)
            : new(filePath, result, encoding);
    }

    /// <summary>Opens the CSV file referenced with <paramref name="filePath"/> for reading.</summary>
    /// <param name="filePath">File path of the CSV file to read.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading the CSV file.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file
    /// or <c>null</c> for <see cref="Encoding.UTF8" />.</param>
    /// 
    /// <returns>A <see cref="CsvReader"/> that allows you to iterate through the CSV data.</returns>
    /// 
    /// <remarks>
    /// <note type="tip">
    /// The optimal parameters can be determined automatically with <see cref="CsvAnalyzer"/> - or use
    /// <see cref="OpenReadAnalyzed(string, Header, Encoding?, int, bool)"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">Error accessing the disk.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvReader OpenRead(string filePath,
                                     bool isHeaderPresent = true,
                                     CsvOpts options = CsvOpts.Default,
                                     char delimiter = ',',
                                     Encoding? textEncoding = null)
        => new(filePath, isHeaderPresent, options, delimiter, textEncoding);

    /// <summary>Initializes a <see cref="CsvReader"/> instance to read data that is in the 
    /// CSV format.</summary>
    /// <param name="reader">The <see cref="TextReader" /> with which the CSV data is
    /// read.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading CSV.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// 
    /// <returns>A <see cref="CsvReader"/> that allows you to iterate through the CSV data.</returns>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="reader" /> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvReader OpenRead(TextReader reader,
                                     bool isHeaderPresent = true,
                                     CsvOpts options = CsvOpts.Default,
                                     char delimiter = ',')
        => new(reader, isHeaderPresent, options, delimiter);

    /// <summary>Creates a new CSV file with header row and initializes a <see cref="CsvWriter"/> instance
    /// to write data to it. If the target file already exists, it is truncated and overwritten.</summary>
    /// <param name="filePath">The file path of the CSV file to be written.</param>
    /// <param name="columnNames">A collection of column names for the header to be written.
    /// The collection will be copied. If the collection contains <c>null</c> values, these 
    /// are replaced by automatically generated column names. Column names cannot appear twice. 
    /// With <paramref name="caseSensitive"/> can be chosen whether the comparison is case-sensitive or not.</param>
    ///  <param name="caseSensitive">If <c>true</c>, column names that differ only in 
    /// upper and lower case are also accepted, otherwise <c>false</c>.</param>
    /// <param name="textEncoding">The text encoding to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// 
    /// <returns>A <see cref="CsvWriter"/> instance that allows you to write data as a CSV file.</returns>
    /// 
    /// <remarks>
    /// This method initializes a <see cref="CsvWriter"/> instance that uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the constructor of
    /// <see cref="CsvWriter"/> directly."/>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="filePath" /> is not a valid file path
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// a column name in <paramref name="columnNames" /> occurs twice. With <paramref name="caseSensitive"/>
    /// can be chosen whether the comparison is case-sensitive or not.
    /// </para>
    /// </exception>
    /// <exception cref="IOException">I/O-Error</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter OpenWrite(string filePath,
                                      IEnumerable<string?> columnNames,
                                      bool caseSensitive = false,
                                      Encoding? textEncoding = null)
        => new(filePath, columnNames, caseSensitive, textEncoding);

    /// <summary>
    /// Initializes a new <see cref="CsvWriter" /> object with the column names
    /// for the header row to be written.</summary>
    /// <param name="writer">The <see cref="TextWriter" /> used for writing.</param>
    /// <param name="columnNames">A collection of column names for the header to be written.
    /// The collection will be copied. If the collection contains <c>null</c> values, these 
    /// are replaced with automatically
    /// generated column names. Column names cannot appear twice. With <paramref name="caseSensitive"/>
    /// can be chosen whether the comparison is case-sensitive or not.</param>
    /// <param name="caseSensitive">If <c>true</c>, column names that differ only in 
    /// upper and lower case are also accepted, otherwise <c>false</c>.</param>
    /// 
    /// <returns>A <see cref="CsvWriter" /> instance that allows you to write CSV data with
    /// <paramref name="writer"/>.</returns>
    /// 
    /// <remarks>
    /// This method initializes a <see cref="CsvWriter"/> instance that uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the constructor of
    /// <see cref="CsvWriter"/> directly."/>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="writer" /> or <paramref
    /// name="columnNames" /> is <c>null.</c></exception>
    /// <exception cref="ArgumentException">A column name in <paramref name="columnNames"
    /// /> occurs twice. With <paramref name="caseSensitive"/> can be chosen whether 
    /// the comparison is case-sensitive or not.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter OpenWrite(TextWriter writer,
                                      IEnumerable<string?> columnNames,
                                      bool caseSensitive = false)
        => new(writer, columnNames, caseSensitive);

    /// <summary>Creates a new CSV file without a header row and initializes a <see cref="CsvWriter"/> 
    /// instance to write data to it. If the target file already exists, it is truncated and overwritten.</summary>
    /// <param name="filePath">The file path of the CSV file to be written.</param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="textEncoding">The text encoding to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// 
    /// <returns>A <see cref="CsvWriter"/> instance that allows you to write data as a CSV file.</returns>
    /// 
    /// <remarks>
    /// This method initializes a <see cref="CsvWriter"/> instance that uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the constructor of
    /// <see cref="CsvWriter"/> directly."/>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">I/O-Error</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter OpenWrite(string filePath,
                                      int columnsCount,
                                      Encoding? textEncoding = null)
        => new(filePath, columnsCount, textEncoding);

    /// <summary>Initializes a new <see cref="CsvWriter" /> object to write CSV data
    /// without a header row.</summary>
    /// <param name="writer">The <see cref="TextWriter" /> used for writing.</param>
    /// <param name="columnsCount">Number of columns in the CSV.</param>
    /// 
    /// <returns>A <see cref="CsvWriter" /> instance that allows you to write CSV data with
    /// the <see cref="TextWriter"/>.</returns>
    /// 
    /// <remarks>
    /// This method initializes a <see cref="CsvWriter"/> instance that uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the constructor of
    /// <see cref="CsvWriter"/> directly."/>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="writer" /> is <c>null.</c></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter OpenWrite(TextWriter writer, int columnsCount)
        => new(writer, columnsCount);

    /// <summary>Parses the specified CSV-<see cref="string"/> to make its content accessible.</summary>
    /// <param name="csv">The CSV-<see cref="string"/> to parse.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Parsing options. (The flag <see cref="CsvOpts.DisableCaching"/>
    /// will be ignored.)</param>
    /// <param name="delimiter">The field separator character.</param>
    /// 
    /// <returns>An array of <see cref="CsvRecord"/> objects containing the parsed data.</returns>
    /// 
    /// <remarks>
    /// <note type="tip">
    /// The optimal parameters can be determined automatically with <see cref="CsvAnalyzer"/> - or use
    /// <see cref="ParseAnalyzed(string, Header, int)"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" /> is <c>null</c>.</exception>
    public static CsvRecord[] Parse(string csv,
                                    bool isHeaderPresent = true,
                                    CsvOpts options = CsvOpts.Default,
                                    char delimiter = ',')
    {
        _ArgumentNullException.ThrowIfNull(csv, nameof(csv));

        using var stringReader = new StringReader(csv);
        using var reader = 
            new CsvReader(stringReader, isHeaderPresent, options.Unset(CsvOpts.DisableCaching), delimiter);

        return [.. reader];
    }

    /// <summary>Analyzes the specified CSV-<see cref="string"/>
    /// first and then parses it content.
    /// </summary>
    /// <param name="csv">The CSV-<see cref="string"/> to parse.</param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// 
    /// <param name="analyzedLines">Maximum number of lines to analyze in <paramref name="csv"/>. The minimum 
    /// value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If <paramref name="csv"/> has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire <see cref="string"/> in any case.)</param>
    /// 
    /// <returns>An array of <see cref="CsvRecord"/> objects containing the parsed data.</returns>
    /// 
    /// <remarks>
    /// <see cref="CsvAnalyzer" /> performs a statistical analysis on the <see cref="string"/>. The result 
    /// of the analysis is therefore always only an estimate, 
    /// the accuracy of which increases with the number of lines analyzed.
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">
    /// In the following code examples - for easier readability - exception handling
    /// has been omitted.
    /// </note>
    /// 
    /// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\StringExample.cs" />
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="header"/> is not a defined value of 
    /// the <see cref="Header"/> enum.</para>
    /// <para> - or -</para>
    /// <para><paramref name="header"/> is a combination of <see cref="Header"/> values.</para>
    /// </exception>
    /// <exception cref="CsvFormatException">Invalid CSV file. Try to increase the value of 
    /// <paramref name="analyzedLines"/>
    /// to get better analyzer results.</exception>
    public static CsvRecord[] ParseAnalyzed(string csv,
                                            Header header = Header.ProbablyPresent,
                                            int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        CsvAnalyzerResult result = CsvAnalyzer.AnalyzeString(csv, header, analyzedLines);

        using var stringReader = new StringReader(csv);
        using CsvReader reader = result.IsHeaderPresent
            ? new(stringReader, isHeaderPresent: true, result.Options, result.Delimiter)
            : new(stringReader, result);

        return [.. reader];
    }

    /// <summary>
    /// Converts the content of <paramref name="data"/> to a comma-separated values <see cref="string"/> (CSV, RFC 4180).
    /// </summary>
    /// <param name="data">The data to convert.</param>
    /// <returns>A CSV-<see cref="string"/> containing the content of <paramref name="data"/>.</returns>
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> is <c>null</c>.</exception>
    public static string AsString(IEnumerable<IEnumerable<string?>?> data)
    {
        _ArgumentNullException.ThrowIfNull(data, nameof(data));

        int maxLen = data.Max(x => x?.Count() ?? 0);

        if (maxLen == 0)
        {
            return string.Empty;
        }

        IEnumerable<string?>? header = data.FirstOrDefault(x => x?.Any(x => !string.IsNullOrWhiteSpace(x)) ?? false);

        using var writer = new StringWriter();
        using var csvWriter = new CsvWriter(writer, maxLen);

        foreach (IEnumerable<string?>? record in data)
        {
            csvWriter.Record.FillWith(record);
            csvWriter.WriteRecord();
        }

        return writer.ToString();
    }

    private static int GetCodePage(string filePath)
    {
        const int BUF_LENGTH = 4;

        try
        {
            using FileStream fs = File.OpenRead(filePath);

#if NET8_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            Span<byte> span = stackalloc byte[BUF_LENGTH];
            int bytesRead = fs.Read(span);
#else
            var buf = new byte[BUF_LENGTH];
            int bytesRead = fs.Read(buf, 0, buf.Length);
            ReadOnlySpan<byte> span = buf;
#endif
            return TextEncodingConverter.GetCodePage(span.Slice(0, bytesRead), out _);
        }
        catch
        {
            return Encoding.UTF8.CodePage;
        }
    }
}
