using System.Globalization;
using System.Text;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary> Analyzes a CSV file.</summary>
public static class CsvAnalyzer
{
    /// <summary>Minimum number of lines in the CSV file to be analyzed.</summary>
    public const int AnalyzedLinesMinCount = 5;

    /// <summary> Analyzes the CSV file referenced by <paramref name="filePath" />.</summary>
    /// 
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="textEncoding">
    /// <para>
    /// The text encoding to be used to read the CSV file, or <c>null</c> for 
    /// <see cref="Encoding.UTF8" />.
    /// </para>
    /// <note type="tip">
    /// Use <see cref="Csv.AnalyzeFile(string, Header, Encoding?, int)"/> to also automatically 
    /// determine the <see cref="Encoding"/>.
    /// </note>
    /// </param>
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)</param>
    /// 
    /// <returns>The results of the analysis.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// <see cref="CsvAnalyzer" /> performs a statistical analysis on the CSV file to find the appropriate 
    /// parameters for reading the file. The result of the analysis is therefore always only an estimate, 
    /// the accuracy of which increases with the number of lines analyzed. The analysis is time-consuming 
    /// because the CSV file has to be accessed for reading.
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
    public static CsvAnalyzerResult AnalyzeFile(string filePath,
                                                Encoding? textEncoding = null,
                                                Header header = Header.ProbablyPresent,
                                                int analyzedLines = AnalyzedLinesMinCount)
    {
        Validate(header);
        analyzedLines = Normalize(analyzedLines);
        CsvAnalyzerResult result = new();

        using (StreamReader reader1 = StreamHelper.InitStreamReader(filePath, textEncoding))
        {
            result.Delimiter = new DelimiterAnalyzer().GetFieldSeparator(reader1);
        }

        using StreamReader reader2 = StreamHelper.InitStreamReader(filePath, textEncoding);
        using var csvStringReader = new CsvStringReader(reader2, result.Delimiter, result.Options);

        CsvPropertiesAnalyzer.InitProperties(csvStringReader,
                                             analyzedLines,
                                             header,
                                             result);
        return result;
    }

    /// <summary> Analyzes a <see cref="string"/> that contains CSV data to get the 
    /// appropriate parameters for parsing.</summary>
    /// 
    /// <param name="csv">The CSV-<see cref="string"/> to analyze.</param>
    /// 
    /// <param name="header">A supposition that is made about the presence of a header row.</param>
    /// <param name="analyzedLines">Maximum number of lines to analyze in <paramref name="csv"/>. The 
    /// minimum value is <see cref="AnalyzedLinesMinCount" />. If <paramref name="csv"/> has fewer lines
    /// than <paramref name="analyzedLines" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire <see cref="string"/> in any 
    /// case.)</param>
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
    /// The field delimiters COMMA (<c>','</c>, %x2C), SEMICOLON  (<c>';'</c>, %x3B), 
    /// HASH (<c>'#'</c>, %x23), TAB (<c>'\t'</c>, %x09), and SPACE (<c>' '</c>, %x20) are recognized 
    /// automatically.
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
    public static CsvAnalyzerResult AnalyzeString(string csv,
                                                  Header header = Header.ProbablyPresent,
                                                  int analyzedLines = AnalyzedLinesMinCount)
    {
        _ArgumentNullException.ThrowIfNull(csv, nameof(csv));

        Validate(header);
        analyzedLines = Normalize(analyzedLines);
        CsvAnalyzerResult result = new();

        using (StringReader reader1 = new(csv))
        {
            result.Delimiter = new DelimiterAnalyzer().GetFieldSeparator(reader1);
        }

        using StringReader reader2 = new(csv);
        using var csvStringReader = new CsvStringReader(reader2, result.Delimiter, result.Options);

        CsvPropertiesAnalyzer.InitProperties(csvStringReader,
                                             analyzedLines,
                                             header,
                                             result);
        return result;
    }

    private static int Normalize(int analyzedLinesCount)
    {
        if (analyzedLinesCount < AnalyzedLinesMinCount)
        {
            analyzedLinesCount = AnalyzedLinesMinCount;
        }

        return analyzedLinesCount;
    }

    private static void Validate(Header header)
    {
        if (!(header is Header.ProbablyPresent or Header.Present or Header.Absent))
        {
            throw new ArgumentOutOfRangeException(nameof(header));
        }
    }
}