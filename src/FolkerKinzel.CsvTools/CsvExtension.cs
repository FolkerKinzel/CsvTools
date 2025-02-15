using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools;

/// <summary>
/// Extension methods for serializing CSV data.
/// </summary>
public static class CsvExtension
{
    /// <summary>
    /// Converts the contents of <paramref name="data"/> to a comma-separated values 
    /// <see cref="string"/> (CSV, RFC 4180).
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
    /// <para>A format <see cref="string"/> to use for all items that implement 
    /// <see cref="IFormattable"/>.</para>
    /// <para>- or -</para>
    /// <para>A <c>null</c> reference to use the default format for each item.</para>
    /// </param>
    /// 
    /// <returns>A CSV-<see cref="string"/> containing the contents of 
    /// <paramref name="data"/>.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is
    /// used if the item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\CsvStringExample.cs" />
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the 
    /// double quotes <c>"</c> or a line break character ('\r' or '\n').</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToCsv(this IEnumerable<IEnumerable<object?>?> data,
                               char delimiter = ',',
                               IFormatProvider? formatProvider = null,
                               string? format = null)
        => Csv.AsString(data, delimiter, formatProvider, format);

    /// <summary>
    /// Saves the contents of <paramref name="data"/> as a CSV file.
    /// </summary>
    /// <param name="data">The data to save.</param>
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
    /// <param name="textEncoding">The <see cref="Encoding"/> to be used or <c>null</c> for 
    /// <see cref="Encoding.UTF8" />.</param>
    /// <param name="format">
    /// <para>A format <see cref="string"/> to use for all items that implement 
    /// <see cref="IFormattable"/>.</para>
    /// <para>- or -</para>
    /// <para>A <c>null</c> reference to use the default format for each item.</para>
    /// </param>
    /// 
    /// <remarks>
    /// <para>Creates a new CSV file. If the target file already exists, it is 
    /// truncated and overwritten.
    /// </para>
    /// <para>
    /// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used
    /// if the item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate parameters can be determined with 
    /// <see cref="Csv.GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">
    /// In the following code examples - for easier readability - exception handling
    /// has been omitted.
    /// </note>
    /// 
    /// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\LinqOnCsvExample.cs" />
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> or <paramref name="filePath"/> 
    /// is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid file path.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the 
    /// double quotes <c>"</c> or a line break character ('\r' or '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SaveCsv(this IEnumerable<IEnumerable<object?>?> data,
                               string filePath,
                               char delimiter = ',',
                               IFormatProvider? formatProvider = null,
                               Encoding? textEncoding = null,
                               string? format = null)
        => Csv.Save(data, filePath, delimiter, formatProvider, textEncoding, format);

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
    /// <para>A format <see cref="string"/> to use for all items that implement 
    /// <see cref="IFormattable"/>.</para>
    /// <para>- or -</para>
    /// <para>A <c>null</c> reference to use the default format for each item.</para>
    /// </param>
    /// 
    /// <remarks>
    /// <para>
    /// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used 
    /// if the item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    /// </para>
    /// <para>
    /// When exchanging CSV data with Excel, the appropriate parameters can be determined with 
    /// <see cref="Csv.GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> or 
    /// <paramref name="textWriter"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the 
    /// double quotes <c>"</c> or a line break character ('\r' or '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteCsv(this IEnumerable<IEnumerable<object?>?> data,
                                TextWriter textWriter,
                                char delimiter = ',',
                                IFormatProvider? formatProvider = null,
                                string? format = null)
        => Csv.Write(data, textWriter, delimiter, formatProvider, format);
}
