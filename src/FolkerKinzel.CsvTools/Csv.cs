using System.Data;
using System.Globalization;
using System.Text;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary>Static class that provides methods for reading, writing and analyzing CSV data.</summary>
public static class Csv
{
    /// <summary>The newline characters to use in CSV files ("\r\n").</summary>
    public const string NewLine = "\r\n";

    /// <summary>
    /// Gets the appropriate method arguments for exchanging CSV data with Excel.
    /// </summary>
    /// <returns>A <see cref="ValueTuple{T1, T2}"/> containing the delimiter character, 
    /// the <see cref="IFormatProvider"/> and the <see cref="Encoding"/> to use when exchanging 
    /// CSV data with Excel.</returns>
    /// <remarks>
    /// <para>Excel formats numbers and dates depending on the culture in its "Regional Settings".
    /// Also the CSV field delimiter character used by Excel, and the <see cref="Encoding"/> that
    /// Excel uses to export CSV depends on these settings.</para>
    /// <para>
    /// This method uses <see cref="CultureInfo.CurrentCulture"/> to retrieve the required 
    /// informations. <see cref="CultureInfo.CurrentCulture"/> and the "Regional Settings"
    /// in Excel have to match to exchange CSV data successfully.
    /// </para>
    /// <para>
    /// When exporting CSV to Excel the <c>TextEncoding</c> argument doesn't need to be used because 
    /// Excel accepts <see cref="Encoding.UTF8"/> as input (which is the default of this library).
    /// </para>
    /// <para>
    /// When importing CSV from Excel the arguments retrieved with this method are a good guess 
    /// (including <c>TextEncoding</c>).
    /// To be on the safe side, <see cref="OpenReadAnalyzed(string, Encoding?, Header, bool, int)"/>
    /// should be used with the <c>TextEncoding</c> argument as <c>fallbackEncoding</c>.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">
    /// In the following code examples - for easier readability - exception handling has been omitted.
    /// </note>
    /// 
    /// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\DataTableExample.cs" />
    /// </example>
    public static
        (char Delimiter, IFormatProvider FormatProvider, Encoding TextEncoding) GetExcelArguments()
    {
        CultureInfo culture = CultureInfo.CurrentCulture;
        TextInfo textInfo = culture.TextInfo;
        string listSeparator = textInfo.ListSeparator;
        return (listSeparator.Length != 1 ? ',' : listSeparator[0], 
                culture,
                TextEncodingConverter.GetEncoding(textInfo.ANSICodePage));
    }

    /// <summary> Analyzes the CSV file referenced by <paramref name="filePath" /> to get the appropriate 
    /// method arguments for parsing.
    /// </summary>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="fallbackEncoding">
    /// The text <see cref="Encoding"/> to be used as a fallback if the CSV file has no byte order mark 
    /// (BOM), or <c>null</c> to use <see cref="Encoding.UTF8"/> as fallback encoding. Use 
    /// <see cref="GetExcelArguments"/> to get the appropriate argument for this parameter when importing
    /// CSV data from Excel.
    /// </param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)</param>
    /// 
    /// <returns>A <see cref="ValueTuple{T1, T2, T3}"/> containing the results of the analysis.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// The method performs a statistical analysis on the CSV file to find the appropriate 
    /// parameters for reading the file. The result of the analysis is therefore always only an estimate, 
    /// the accuracy of which increases with the number of lines analyzed. The analysis is time-consuming 
    /// because the CSV file has to be accessed for reading.
    /// </para>
    /// <para>
    /// This method also tries to determine the <see cref="Encoding"/> of the CSV file from the
    /// byte order mark (BOM). If no byte order mark can be found, <paramref name="fallbackEncoding"/> is
    /// used.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), 
    /// HASH (<c>'#'</c>, %x23), TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized 
    /// automatically.
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
    /// <exception cref="IOException">I/O error.</exception>
    public static (CsvAnalyzerResult AnalyzerResult, Encoding Encoding)
        AnalyzeFile(string filePath,
                    Encoding? fallbackEncoding = null,
                    Header header = Header.ProbablyPresent,
                    int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        fallbackEncoding ??= Encoding.UTF8;

        if (TryGetCodePageFromBom(filePath, out var codePage))
        {
            // Unicode encodings are always successful:
            _ = TextEncodingConverter.TryGetEncoding(codePage, out fallbackEncoding);
        }

        CsvAnalyzerResult results =
            CsvAnalyzer.AnalyzeFile(filePath, fallbackEncoding, header, analyzedLines);
        return (results, fallbackEncoding!);
    }

    /// <summary> Analyzes a <see cref="string"/> that contains CSV data to get the appropriate method
    /// arguments for parsing.</summary>
    /// 
    /// <param name="csv">The CSV-<see cref="string"/> to analyze.</param>
    /// 
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in <paramref name="csv"/>. The 
    /// minimum value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If <paramref name="csv"/> has 
    /// fewer lines than <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire <see cref="string"/> in any case.)
    /// </param>
    /// 
    /// <returns>The results of the analysis.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// <see cref="CsvAnalyzer" /> performs a statistical analysis on the <see cref="string"/>. The result 
    /// of the analysis is therefore always only an estimate, 
    /// the accuracy of which increases with the number of lines analyzed.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), HASH (<c>'#'</c>, %x23),
    /// TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized automatically.
    /// </para>
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

    /// <summary>Analyzes the CSV file referenced by <paramref name="filePath" /> first and then opens 
    /// a <see cref="CsvReader"/> to read its content.
    /// </summary>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="fallbackEncoding">
    /// The text <see cref="Encoding"/> to be used as a fallback if the CSV file has no byte order mark 
    /// (BOM), or <c>null</c> to use <see cref="Encoding.UTF8"/> as fallback encoding. Use 
    /// <see cref="GetExcelArguments"/> to get the appropriate argument for this parameter when importing
    /// CSV data from Excel.
    /// </param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// <param name="disableCaching"><c>true</c> to set the <see cref="CsvOpts.DisableCaching"/> flag, 
    /// otherwise <c>false</c>.</param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)</param>
    /// 
    /// <returns>A <see cref="CsvReader"/> that allows to iterate the data.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// The method performs a statistical analysis on the CSV file to find the appropriate 
    /// parameters for reading the file. The result of the analysis is therefore always only an estimate, 
    /// the accuracy of which increases with the number of lines analyzed. The analysis is time-consuming 
    /// because the CSV file has to be accessed for reading.
    /// </para>
    /// <para>
    /// This method also tries to determine the <see cref="Encoding"/> of the CSV file from the
    /// byte order mark (BOM). If no byte order mark can be found, <paramref name="fallbackEncoding"/> is
    /// used.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), HASH (<c>'#'</c>, %x23),
    /// TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized automatically.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">
    /// In the following code examples - for easier readability - exception handling
    /// has been omitted.
    /// </note>
    /// 
    /// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\CsvAnalyzerExample.cs" />
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
    /// <exception cref="IOException">I/O error.</exception>
    public static CsvReader OpenReadAnalyzed(string filePath,
                                             Encoding? fallbackEncoding = null,
                                             Header header = Header.ProbablyPresent,
                                             bool disableCaching = false,
                                             int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        (CsvAnalyzerResult result, Encoding encoding) =
            AnalyzeFile(filePath, fallbackEncoding, header, analyzedLines);
        result.Options = disableCaching ? result.Options | CsvOpts.DisableCaching : result.Options;
        return result.IsHeaderPresent
            ? new(filePath, result.Delimiter, encoding, true, result.Options)
            : new(filePath, result, encoding);
    }

    /// <summary>Opens the CSV file referenced with <paramref name="filePath"/> for reading.</summary>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file
    /// or <c>null</c> for <see cref="Encoding.UTF8" />.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading the CSV file.</param>
    /// 
    /// <returns>A <see cref="CsvReader"/> that allows you to iterate through the CSV data.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// The method arguments can be determined automatically with <see cref="CsvAnalyzer"/> - or use
    /// <see cref="OpenReadAnalyzed(string, Encoding?, Header, bool, int)"/>.
    /// </para>
    /// <para>
    /// When importing CSV data from Excel, the appropriate arguments can be determined 
    /// with <see cref="Csv.GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">
    /// In the following code examples - for easier readability - exception handling
    /// has been omitted.
    /// </note>
    /// 
    /// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\DisableCachingExample.cs" />
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either 
    /// the double quotes <c>"</c> or a line break character ('\r' or  '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvReader OpenRead(string filePath,
                                     char delimiter = ',',
                                     Encoding? textEncoding = null,
                                     bool isHeaderPresent = true,
                                     CsvOpts options = CsvOpts.Default)
        => new(filePath, delimiter, textEncoding, isHeaderPresent, options);

    /// <summary>Initializes a <see cref="CsvReader"/> instance to read CSV data.</summary>
    /// <param name="reader">The <see cref="TextReader" /> with which the CSV data is
    /// read.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="isHeaderPresent"> <c>true</c> to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading CSV.</param>
    /// 
    /// <returns>A <see cref="CsvReader"/> that allows you to iterate through the CSV data.</returns>
    /// 
    /// <remarks>
    /// When importing CSV data from Excel, the appropriate arguments can be determined with 
    /// <see cref="Csv.GetExcelArguments"/>.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="reader" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either 
    /// the double quotes <c>"</c> or a line break character ('\r' or  '\n').</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvReader OpenRead(TextReader reader,
                                     char delimiter = ',',
                                     bool isHeaderPresent = true,
                                     CsvOpts options = CsvOpts.Default)
        => new(reader, delimiter, isHeaderPresent, options);

    /// <summary>Creates a new CSV file with header row and initializes a <see cref="CsvWriter"/> 
    /// instance to write data to it.</summary>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="columnNames">
    /// <para>
    /// A collection of column names for the header to be written.
    /// </para>
    /// <para>
    /// The collection determines the order in which the columns appear in the CSV file and their index in 
    /// <see cref="CsvRecord"/>. The 
    /// <see cref="CsvWriter.Record"/>
    /// of the <see cref="CsvWriter"/> instance, which the method returns, can be accessed with this column 
    /// names.
    /// </para>
    /// <para>
    /// The collection will be copied. If the collection contains <c>null</c> values, empty strings or white
    /// space, these are replaced by automatically generated column names. Column names cannot appear twice. 
    /// By default the comparison is case-sensitive but it will be reset to a case-insensitive comparison if 
    /// the column names are also unique when treated case-insensitive.
    /// </para>
    /// </param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">The text encoding to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// 
    /// <returns>A <see cref="CsvWriter"/> instance that allows you to write data as a CSV file.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// If the target file already exists, it is truncated and overwritten.
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined with 
    /// <see cref="GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">
    /// In the following code examples - for easier readability - exception handling has been omitted.
    /// </note>
    /// 
    /// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\CsvAnalyzerExample.cs" />
    /// </example>
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
    /// a column name in <paramref name="columnNames" /> occurs twice.
    /// </para>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the double quotes
    /// <c>"</c> or a line break character ('\r' or  '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter OpenWrite(string filePath,
                                      IReadOnlyCollection<string?> columnNames,
                                      char delimiter = ',',
                                      Encoding? textEncoding = null)
        => new(filePath, columnNames, CaseSensitive(columnNames), delimiter, textEncoding);

    /// <summary>
    /// Initializes a new <see cref="CsvWriter" /> object with the column names for the header row 
    /// to be written.</summary>
    /// <param name="writer">The <see cref="TextWriter" /> used for writing.</param>
    /// 
    /// <param name="columnNames">
    /// <para>
    /// A collection of column names for the header to be written.
    /// </para>
    /// <para>
    /// The collection determines the order in which the columns appear in the CSV and their index in 
    /// <see cref="CsvRecord"/>. The 
    /// <see cref="CsvWriter.Record"/>
    /// of the <see cref="CsvWriter"/> instance, which the method returns, can be accessed with this 
    /// column names.
    /// </para>
    /// <para>
    /// The collection will be copied. If the collection contains <c>null</c> values, empty strings or 
    /// white space, these are replaced by automatically generated column names. Column names cannot appear
    /// twice. By default the comparison is case-sensitive but it will be reset to a case-insensitive 
    /// comparison if the column names are also unique when treated case-insensitive.
    /// </para>
    /// </param>
    /// <param name="delimiter">The field separator character.</param>
    /// 
    /// <returns>A <see cref="CsvWriter" /> instance that allows you to write CSV data with
    /// <paramref name="writer"/>.</returns>
    /// 
    /// <remarks>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined with 
    /// <see cref="GetExcelArguments"/>.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="writer" /> or <paramref name="columnNames" />
    /// is <c>null.</c></exception>
    /// <exception cref="ArgumentException">A column name in <paramref name="columnNames" /> occurs twice. 
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the double quotes
    /// <c>"</c> or a line break character ('\r' or  '\n').</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter OpenWrite(TextWriter writer,
                                      IReadOnlyCollection<string?> columnNames,
                                      char delimiter = ',')
        => new(writer, columnNames, CaseSensitive(columnNames), delimiter);

    /// <summary>Creates a new CSV file without a header row and initializes a <see cref="CsvWriter"/> 
    /// instance to write data to it.</summary>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">The text encoding to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// 
    /// <returns>A <see cref="CsvWriter"/> instance that allows you to write data as a CSV file.</returns>
    /// 
    /// <remarks>
    /// <para>Creates a new CSV file. If the target file already exists, it is truncated and overwritten.
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined with 
    /// <see cref="GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid file path.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="columnsCount"/> is negative.</para>
    /// <para>- or -</para>
    /// <para><paramref name="delimiter"/> is either the double quotes <c>"</c> or a line break character 
    /// ('\r' or  '\n').</para>
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter OpenWrite(string filePath,
                                      int columnsCount,
                                      char delimiter = ',',
                                      Encoding? textEncoding = null)
        => new(filePath, columnsCount, delimiter, textEncoding);

    /// <summary>Initializes a new <see cref="CsvWriter" /> object to write CSV data without a header row.
    /// </summary>
    /// <param name="writer">The <see cref="TextWriter" /> used for writing.</param>
    /// <param name="columnsCount">Number of columns in the CSV.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// 
    /// <returns>A <see cref="CsvWriter" /> instance that allows you to write CSV data with
    /// the <see cref="TextWriter"/>.</returns>
    /// 
    /// <remarks>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined with 
    /// <see cref="GetExcelArguments"/>.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="writer" /> is <c>null.</c></exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="columnsCount"/> is negative.</para>
    /// <para>- or -</para>
    /// <para><paramref name="delimiter"/> is either the double quotes <c>"</c> or a line break character 
    /// ('\r' or  '\n').</para>
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvWriter OpenWrite(TextWriter writer, int columnsCount, char delimiter = ',')
        => new(writer, columnsCount, delimiter);

    /// <summary>Parses the specified CSV-<see cref="string"/> to make its content accessible.</summary>
    /// <param name="csv">The CSV-<see cref="string"/> to parse.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, 
    /// otherwise <c>false</c>.</param>
    /// <param name="options">Parsing options. (The flag <see cref="CsvOpts.DisableCaching"/>
    /// will be ignored.)</param>
    /// 
    /// <returns>An array of <see cref="CsvRecord"/> objects containing the parsed data.</returns>
    /// 
    /// <remarks>
    /// <note type="tip">
    /// The optimal method arguments can be determined automatically with <see cref="CsvAnalyzer"/> - or 
    /// use <see cref="ParseAnalyzed(string, Header, int)"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" /> is <c>null</c>.</exception>
    /// <exception cref="CsvFormatException">Invalid CSV. The interpretation depends
    /// on <paramref name="options"/>.</exception>
    public static CsvRecord[] Parse(string csv,
                                    char delimiter = ',',
                                    bool isHeaderPresent = true,
                                    CsvOpts options = CsvOpts.Default)
    {
        _ArgumentNullException.ThrowIfNull(csv, nameof(csv));

        using var stringReader = new StringReader(csv);
        using var reader =
            new CsvReader(stringReader, delimiter, isHeaderPresent, options.Unset(CsvOpts.DisableCaching));

        return [.. reader];
    }

    /// <summary>Analyzes the specified CSV-<see cref="string"/> first and then parses it content.
    /// </summary>
    /// <param name="csv">The CSV-<see cref="string"/> to parse.</param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// 
    /// <param name="analyzedLines">Maximum number of lines to analyze in <paramref name="csv"/>. The minimum 
    /// value is <see cref="CsvAnalyzer.AnalyzedLinesMinCount" />. If <paramref name="csv"/> has fewer lines 
    /// than <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire <see cref="string"/> in any case.)
    /// </param>
    /// 
    /// <returns>An array of <see cref="CsvRecord"/> objects containing the parsed data.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// <see cref="CsvAnalyzer" /> performs a statistical analysis on the <see cref="string"/>. The result 
    /// of the analysis is therefore always only an estimate, the accuracy of which increases with the number 
    /// of lines analyzed.
    /// </para>
    /// <para>
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), HASH (<c>'#'</c>, %x23),
    /// TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized automatically.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">
    /// In the following code examples - for easier readability - exception handling has been omitted.
    /// </note>
    /// 
    /// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\CsvStringExample.cs" />
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="csv" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="header"/> is not a defined value of  the <see cref="Header"/> enum.</para>
    /// <para> - or -</para>
    /// <para><paramref name="header"/> is a combination of <see cref="Header"/> values.</para>
    /// </exception>
    /// <exception cref="CsvFormatException">Invalid CSV file. Try to increase the value of 
    /// <paramref name="analyzedLines"/> to get a better analyzer result!</exception>
    public static CsvRecord[] ParseAnalyzed(string csv,
                                            Header header = Header.ProbablyPresent,
                                            int analyzedLines = CsvAnalyzer.AnalyzedLinesMinCount)
    {
        CsvAnalyzerResult result = CsvAnalyzer.AnalyzeString(csv, header, analyzedLines);

        using var stringReader = new StringReader(csv);
        using CsvReader reader = result.IsHeaderPresent
            ? new(stringReader, delimiter: result.Delimiter, isHeaderPresent: true, options: result.Options)
            : new(stringReader, result);

        return [.. reader];
    }

    /// <summary>
    /// Converts the contents of <paramref name="data"/> to a comma-separated values <see cref="string"/> 
    /// (CSV, RFC 4180).
    /// </summary>
    /// <param name="data">The data to convert.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="formatProvider">
    /// <para>
    /// The provider to use to format the value.
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// A <c>null</c> reference for <see cref="CultureInfo.InvariantCulture"/>.
    /// </para>
    /// </param>
    /// <param name="format">
    /// <para>A format <see cref="string"/> to use for all items that implement <see cref="IFormattable"/>.
    /// </para>
    /// <para>- or -</para>
    /// <para>A <c>null</c> reference to use the default format for each item.</para>
    /// </param>
    /// 
    /// <returns>A CSV-<see cref="string"/> containing the contents of <paramref name="data"/>.</returns>
    /// 
    /// <remarks>
    /// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used if the
    /// item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the double quotes
    /// <c>"</c> or a line break character ('\r' or  '\n').</exception>
    public static string AsString(IEnumerable<IEnumerable<object?>?> data,
                                  char delimiter = ',',
                                  IFormatProvider? formatProvider = null,
                                  string? format = null)
    {
        using var writer = new StringWriter();
        WriteIntl(data, writer, delimiter, formatProvider, format);

        return writer.ToString();
    }

    /// <summary>
    /// Saves the contents of <paramref name="data"/> as a CSV file.
    /// </summary>
    /// <param name="data">The data to save.</param>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="formatProvider">
    /// <para>
    /// The provider to use to format the value.
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// A <c>null</c> reference for <see cref="CultureInfo.InvariantCulture"/>.
    /// </para>
    /// </param>
    /// <param name="textEncoding">The <see cref="Encoding"/> to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// <param name="format">
    /// <para>A format <see cref="string"/> to use for all items that implement <see cref="IFormattable"/>.
    /// </para>
    /// <para>- or -</para>
    /// <para>A <c>null</c> reference to use the default format for each item.</para>
    /// </param>
    /// 
    /// <remarks>
    /// <para>Creates a new CSV file. If the target file already exists, it is truncated and overwritten.
    /// </para>
    /// <para>
    /// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used if the
    /// item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined with 
    /// <see cref="GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> or 
    /// <paramref name="filePath"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a 
    /// valid file path.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the double quotes
    /// <c>"</c> or a line break character ('\r' or  '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static void Save(IEnumerable<IEnumerable<object?>?> data,
                            string filePath,
                            char delimiter = ',',
                            IFormatProvider? formatProvider = null,
                            Encoding? textEncoding = null,
                            string? format = null)
    {
        using StreamWriter streamWriter = StreamHelper.InitStreamWriter(filePath, textEncoding);
        WriteIntl(data, streamWriter, delimiter, formatProvider, format);
    }

    /// <summary>
    /// Saves the content of a <see cref="DataTable"/> as a CSV file with header row. 
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> to save.</param>
    /// <param name="filePath">The file path of the CSV file to be written.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="formatProvider">
    /// <para>
    /// The provider to use to format the value.
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// A <c>null</c> reference for <see cref="CultureInfo.InvariantCulture"/>.
    /// </para>
    /// </param>
    /// <param name="textEncoding">The <see cref="Encoding"/> to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// <param name="csvColumnNames">
    /// <para>
    /// A collection of <see cref="DataColumn.ColumnName"/>s from <paramref name="dataTable"/>
    /// that allows to select the <see cref="DataColumn"/>s to export and to determine their order
    /// in the CSV file, or <c>null</c> to save the whole <see cref="DataTable"/> using its current
    /// column order. 
    /// </para>
    /// <para>
    /// Each item in this collection MUST be a <see cref="DataColumn.ColumnName"/> in 
    /// <paramref name="dataTable"/>.
    /// </para>
    /// </param>
    /// <param name="format">
    /// <para>A format <see cref="string"/> to use for all items that implement <see cref="IFormattable"/>.
    /// </para>
    /// <para>- or -</para>
    /// <para>A <c>null</c> reference to use the default format for each item.</para>
    /// </param>
    /// 
    /// <remarks>
    /// <para>Creates a new CSV file. If the target file already exists, it is truncated and overwritten.
    /// </para>
    /// <para>
    /// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used if the
    /// item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined with 
    /// <see cref="GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="dataTable" /> or 
    /// <paramref name="filePath"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para><paramref name="filePath" /> is not a 
    /// valid file path.</para>
    /// <para>- or -</para>
    /// <para>
    /// <paramref name="csvColumnNames"/> contains an item that is not a <see cref="DataColumn.ColumnName"/>
    /// in <paramref name="dataTable"/>.
    /// </para>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the double quotes
    /// <c>"</c> or a line break character ('\r' or  '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static void Save(DataTable dataTable,
                            string filePath,
                            char delimiter = ',',
                            IFormatProvider? formatProvider = null,
                            Encoding? textEncoding = null,
                            IEnumerable<string>? csvColumnNames = null,
                            string? format = null)
    {
        using StreamWriter streamWriter = StreamHelper.InitStreamWriter(filePath, textEncoding);
        WriteIntl(dataTable, streamWriter, delimiter, formatProvider, csvColumnNames, format);
    }

    /// <summary>
    /// Writes the contents of a <see cref="DataTable"/> as CSV with header row.
    /// </summary>
    /// <param name="dataTable">The <see cref="DataTable"/> whose content is written.</param>
    /// <param name="textWriter">The <see cref="TextWriter"/> to be used.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="formatProvider">
    /// <para>
    /// The provider to use to format the value.
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// A <c>null</c> reference for <see cref="CultureInfo.InvariantCulture"/>.
    /// </para>
    /// </param>
    /// <param name="csvColumnNames">
    /// <para>
    /// A collection of <see cref="DataColumn.ColumnName"/>s from <paramref name="dataTable"/>
    /// that allows to select the <see cref="DataColumn"/>s to export and to determine their order
    /// in the CSV file, or <c>null</c> to save
    /// the whole <see cref="DataTable"/> with its current column order. 
    /// </para>
    /// <para>
    /// Each item in this collection MUST be a <see cref="DataColumn.ColumnName"/> in 
    /// <paramref name="dataTable"/>.
    /// </para>
    /// </param>
    /// <param name="format">
    /// <para>A format <see cref="string"/> to use for all items that implement <see cref="IFormattable"/>.
    /// </para>
    /// <para>- or -</para>
    /// <para>A <c>null</c> reference to use the default format for each item.</para>
    /// </param>
    /// 
    /// <remarks>
    /// <para>
    /// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used if the
    /// item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined with 
    /// <see cref="GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="dataTable" /> or 
    /// <paramref name="textWriter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="csvColumnNames"/> contains an item that is not a <see cref="DataColumn.ColumnName"/>
    /// in <paramref name="dataTable"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the double quotes
    /// <c>"</c> or a line break character ('\r' or  '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static void Write(DataTable dataTable,
                             TextWriter textWriter,
                             char delimiter = ',',
                             IFormatProvider? formatProvider = null,
                             IEnumerable<string>? csvColumnNames = null,
                             string? format = null)
    {
        _ArgumentNullException.ThrowIfNull(textWriter, nameof(textWriter));
        WriteIntl(dataTable, textWriter, delimiter, formatProvider, csvColumnNames, format);
    }

    /// <summary>
    /// Writes the contents of <paramref name="data"/> as CSV.
    /// </summary>
    /// <param name="data">The data to write.</param>
    /// <param name="textWriter">The <see cref="TextWriter"/> to be used.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="formatProvider">
    /// <para>
    /// The provider to use to format the value.
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// A <c>null</c> reference for <see cref="CultureInfo.InvariantCulture"/>.
    /// </para>
    /// </param>
    /// <param name="format">
    /// <para>A format <see cref="string"/> to use for all items that implement <see cref="IFormattable"/>.
    /// </para>
    /// <para>- or -</para>
    /// <para>A <c>null</c> reference to use the default format for each item.</para>
    /// </param>
    /// 
    /// <remarks>
    /// <para>
    /// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used if the
    /// item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate arguments can be determined with 
    /// <see cref="GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> or <paramref name="textWriter"/> 
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the double quotes
    /// <c>"</c> or a line break character ('\r' or '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static void Write(IEnumerable<IEnumerable<object?>?> data,
                             TextWriter textWriter,
                             char delimiter = ',',
                             IFormatProvider? formatProvider = null,
                             string? format = null)
    {
        _ArgumentNullException.ThrowIfNull(textWriter, nameof(textWriter));
        WriteIntl(data, textWriter, delimiter, formatProvider, format);
    }

    private static void WriteIntl(DataTable dataTable,
                                  TextWriter textWriter,
                                  char delimiter,
                                  IFormatProvider? formatProvider,
                                  IEnumerable<string?>? csvColumnNames,
                                  string? format)
    {
        _ArgumentNullException.ThrowIfNull(dataTable, nameof(dataTable));

        formatProvider ??= CultureInfo.InvariantCulture;

        csvColumnNames ??= dataTable.Columns
                                 .Cast<DataColumn>()
                                 .Select(x => x.ColumnName);

        // DataColumn.ColumnName is not case-sensitive
        using CsvWriter csvWriter = new(textWriter, csvColumnNames, caseSensitive: false, delimiter: delimiter);
        CsvRecord record = csvWriter.Record;

        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
            DataRow row = dataTable.Rows[i];

            if (row.RowState != DataRowState.Deleted)
            {
                record.FillWith(row, formatProvider, format);
                csvWriter.WriteRecord();
            }
        }
    }

    private static void WriteIntl(IEnumerable<IEnumerable<object?>?> data,
                                  TextWriter textWriter,
                                  char delimiter,
                                  IFormatProvider? formatProvider,
                                  string? format)
    {
        _ArgumentNullException.ThrowIfNull(data, nameof(data));

        int maxLen = data.Max(static x => x?.Count() ?? 0);

        using var csvWriter = new CsvWriter(textWriter, maxLen, delimiter);

        formatProvider ??= CultureInfo.InvariantCulture;
        CsvRecord record = csvWriter.Record;

        foreach (IEnumerable<object?>? coll in data)
        {
            record.FillWith(coll, formatProvider, format, resetExcess: false);
            csvWriter.WriteRecord();
        }
    }

    /// <summary>
    /// Determines whether <paramref name="columnNames"/> should be treated case-sensitive.
    /// </summary>
    /// <param name="columnNames">The column names to examine.</param>
    /// <returns><c>true</c> if the <paramref name="columnNames"/> should be treated 
    /// case-sensitive, otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="columnNames"/> is <c>null</c>.
    /// </exception>
    private static bool CaseSensitive(IReadOnlyCollection<string?> columnNames)
    {
        _ArgumentNullException.ThrowIfNull(columnNames, nameof(columnNames));
        return columnNames.Where(x => !string.IsNullOrWhiteSpace(x))
                          .Count()
            != columnNames.Where(x => !string.IsNullOrWhiteSpace(x))
                          .Distinct(StringComparer.OrdinalIgnoreCase)
                          .Count();
    }

    private static bool TryGetCodePageFromBom(string filePath, out int codePage)
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
            codePage = 
                TextEncodingConverter.GetCodePage(span.Slice(0, bytesRead), out int bomLength);
            return bomLength != 0 ;
        }
        catch
        {
            codePage = 0;
            return false;
        }
    }
}
