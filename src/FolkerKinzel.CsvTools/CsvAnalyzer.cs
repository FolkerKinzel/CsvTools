using System.Runtime.InteropServices;
using System.Text;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary> Analyzes a CSV file.</summary>
public static class CsvAnalyzer
{
    /// <summary>Minimum number of lines in the CSV file to be analyzed.</summary>
    public const int AnalyzedLinesMinCount = 5;

    /// <summary> Analyzes the CSV file referenced by <paramref name="filePath" />. </summary>
    /// 
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="textEncoding">
    /// <para>
    /// The text encoding to be used to read the CSV file, or <c>null</c> for <see cref="Encoding.UTF8" />.
    /// </para>
    /// <note type="tip">
    /// Use <see cref="Csv.Analyze(string, CsvSupposition, Encoding?, int)"/> to also automatically determine 
    /// the <see cref="Encoding"/>.
    /// </note>
    /// </param>
    /// <param name="supposition">A supposition that is made about the presence of a header row.</param>
    /// <param name="analyzedLinesCount">Maximum number of lines to analyze in the CSV file. The minimum 
    /// value is <see cref="AnalyzedLinesMinCount" />. If the file has fewer lines than 
    /// <paramref name="analyzedLinesCount" />, it will be analyzed completely. (You can specify 
    /// <see cref="int.MaxValue">Int32.MaxValue</see> to analyze the entire file in any case.)</param>
    /// 
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
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="supposition"/> is not a defined value of 
    /// the <see cref="CsvSupposition"/> enum.</para>
    /// <para> - or -</para>
    /// <para><paramref name="supposition"/> is a combination of <see cref="CsvSupposition"/> values.</para>
    /// </exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">Error accessing the file.</exception>
    public static CsvAnalyzerResult Analyze(string filePath,
                                            Encoding? textEncoding = null,
                                            CsvSupposition supposition = CsvSupposition.ProbablyHeaderPresent,
                                            int analyzedLinesCount = AnalyzedLinesMinCount)
    {
        ValidateSupposition(supposition);
        CsvAnalyzerResult result = new();

        if (analyzedLinesCount < AnalyzedLinesMinCount)
        {
            analyzedLinesCount = AnalyzedLinesMinCount;
        }

        using (StreamReader reader1 = StreamReaderHelper.InitializeStreamReader(filePath, textEncoding))
        {
            result.Delimiter = new FieldSeparatorAnalyzer().GetFieldSeparator(reader1);
        }

        using StreamReader reader2 = StreamReaderHelper.InitializeStreamReader(filePath, textEncoding);
        using var csvStringReader = new CsvStringReader(reader2, result.Delimiter, result.Options);

        CsvPropertiesAnalyzer.InitProperties(csvStringReader,
                                       analyzedLinesCount,
                                       supposition,
                                       result);
        return result;

        ///////////////////////////////////

        static void ValidateSupposition(CsvSupposition supposition)
        {
            if (!(supposition is CsvSupposition.ProbablyHeaderPresent or CsvSupposition.HeaderPresent or CsvSupposition.HeaderAbsent))
            {
                throw new ArgumentOutOfRangeException(nameof(supposition));
            }
        }
    }
}