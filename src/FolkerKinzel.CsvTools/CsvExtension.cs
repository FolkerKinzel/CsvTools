using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools;

/// <summary>
/// Extension methods for serializing CSV data.
/// </summary>
public static class CsvExtension
{
    /// <summary>
    /// Converts the contents of <paramref name="data"/> to a comma-separated values <see cref="string"/> 
    /// (CSV, RFC 4180).
    /// </summary>
    /// <param name="data">The data to convert.</param>
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
    /// <para>A format <see cref="string"/> to use for all items that implement <see cref="IFormattable"/>.</para>
    /// <para>- or -</para>
    /// <para>A <c>null</c> reference to use the default format for each item.</para>
    /// </param>
    /// 
    /// <returns>A CSV-<see cref="string"/> containing the contents of <paramref name="data"/>.</returns>
    /// 
    /// <remarks>
    /// <para>
    /// The CSV that this method creates uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the constructor of
    /// <see cref="CsvWriter"/> directly."
    /// </para>
    /// <para>
    /// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used if the
    /// item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">
    /// In the following code examples - for easier readability - exception handling
    /// has been omitted.
    /// </note>
    /// 
    /// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\CsvStringExample.cs" />
    /// </example>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToCsv(this IEnumerable<IEnumerable<object?>?> data,
                               IFormatProvider? formatProvider = null,
                               string? format = null)
        => Csv.AsString(data, formatProvider, format);

    ///// <summary>
    ///// Converts the contents of <paramref name="data"/> to a comma-separated values <see cref="string"/> 
    ///// (CSV, RFC 4180).
    ///// </summary>
    ///// <param name="data">The data to convert.</param>
    ///// <param name="formatProvider">
    ///// <para>
    ///// The provider to use to format the value.
    ///// </para>
    ///// <para>
    ///// - or -
    ///// </para>
    ///// <para>
    ///// A <c>null</c> reference for <see cref="CultureInfo.InvariantCulture"/>.
    ///// </para>
    ///// </param>
    ///// <param name="format">
    ///// <para>A format <see cref="string"/> to use for all items that implement <see cref="IFormattable"/>.</para>
    ///// <para>- or -</para>
    ///// <para>A <c>null</c> reference to use the default format for each item.</para>
    ///// </param>
    ///// 
    ///// <returns>A CSV-<see cref="string"/> containing the content of <paramref name="data"/>.</returns>
    ///// 
    ///// <remarks>
    ///// <para>
    ///// The CSV that this method creates uses the comma ',' (%x2C) as field delimiter.
    ///// This complies with the RFC 4180 standard. If another delimiter is required, use the constructor of
    ///// <see cref="CsvWriter"/> directly."
    ///// </para>
    ///// <para>
    ///// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used if the
    ///// item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    ///// </para>
    ///// </remarks>
    ///// 
    ///// <exception cref="ArgumentNullException"> <paramref name="data" /> is <c>null</c>.</exception>
    //public static string ToCsv(this IEnumerable<object?> data,
    //                           IFormatProvider? formatProvider = null,
    //                           string? format = null)
    //{
    //    _ArgumentNullException.ThrowIfNull(data, nameof(data));
    //    return Csv.AsString(Enumerable.Repeat(data, 1), formatProvider, format);
    //}

    /// <summary>
    /// Saves the contents of <paramref name="data"/> as a CSV file.
    /// </summary>
    /// <param name="data">The data to save.</param>
    /// <param name="filePath">The file path of the CSV file to be written.</param>
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
    /// <para>A format <see cref="string"/> to use for all items that implement <see cref="IFormattable"/>.</para>
    /// <para>- or -</para>
    /// <para>A <c>null</c> reference to use the default format for each item.</para>
    /// </param>
    /// <param name="textEncoding">The <see cref="Encoding"/> to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// 
    /// <remarks>
    /// <para>Creates a new CSV file. If the target file already exists, it is 
    /// truncated and overwritten.
    /// </para>
    /// <para>
    /// The CSV file that this method creates uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the constructor of
    /// <see cref="CsvWriter"/> directly."
    /// </para>
    /// <para>
    /// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used if the
    /// item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> or 
    /// <paramref name="filePath"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a 
    /// valid file path.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SaveCsv(this IEnumerable<IEnumerable<object?>?> data,
                               string filePath,
                               IFormatProvider? formatProvider = null,
                               string? format = null,
                               Encoding? textEncoding = null)
        => Csv.Save(data, filePath, formatProvider, format, textEncoding);

    /// <summary>
    /// Writes the contents of <paramref name="data"/> as CSV.
    /// </summary>
    /// <param name="data">The data to write.</param>
    /// <param name="textWriter">The <see cref="TextWriter"/> to be used.</param>
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
    /// <para>A format <see cref="string"/> to use for all items that implement <see cref="IFormattable"/>.</para>
    /// <para>- or -</para>
    /// <para>A <c>null</c> reference to use the default format for each item.</para>
    /// </param>
    /// 
    /// <remarks>
    /// <para>
    /// The CSV file that this method creates uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the constructor of
    /// <see cref="CsvWriter"/> directly."
    /// </para>
    /// <para>
    /// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used if the
    /// item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> or 
    /// <paramref name="textWriter"/> is <c>null</c>.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteCsv(this IEnumerable<IEnumerable<object?>?> data,
                                TextWriter textWriter,
                                IFormatProvider? formatProvider = null,
                                string? format = null)
        => Csv.Write(data, textWriter, formatProvider, format);

    ///// <summary>
    ///// Saves the contents of <paramref name="data"/> as a CSV file.
    ///// </summary>
    ///// <param name="data">The data to save.</param>
    ///// <param name="filePath">The file path of the CSV file to be written.</param>
    ///// <param name="formatProvider">
    ///// <para>
    ///// The provider to use to format the value.
    ///// </para>
    ///// <para>
    ///// - or -
    ///// </para>
    ///// <para>
    ///// A <c>null</c> reference for <see cref="CultureInfo.InvariantCulture"/>.
    ///// </para>
    ///// </param>
    ///// <param name="format">
    ///// <para>A format <see cref="string"/> to use for all items that implement <see cref="IFormattable"/>.</para>
    ///// <para>- or -</para>
    ///// <para>A <c>null</c> reference to use the default format for each item.</para>
    ///// </param>
    ///// <param name="textEncoding">The <see cref="Encoding"/> to be used or <c>null</c> for <see
    ///// cref="Encoding.UTF8" />.</param>
    ///// 
    ///// <remarks>
    ///// <para>Creates a new CSV file. If the target file already exists, it is 
    ///// truncated and overwritten.
    ///// </para>
    ///// <para>
    ///// The CSV file that this method creates uses the comma ',' (%x2C) as field delimiter.
    ///// This complies with the RFC 4180 standard. If another delimiter is required, use the constructor of
    ///// <see cref="CsvWriter"/> directly."
    ///// </para>
    ///// <para>
    ///// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used if the
    ///// item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    ///// </para>
    ///// </remarks>
    ///// 
    ///// <exception cref="ArgumentNullException"> <paramref name="data" /> or 
    ///// <paramref name="filePath"/> is <c>null</c>.</exception>
    ///// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a 
    ///// valid file path.</exception>
    ///// <exception cref="IOException">I/O error.</exception>
    //public static void SaveCsv(this IEnumerable<object?> data,
    //                           string filePath,
    //                           IFormatProvider? formatProvider = null,
    //                           string? format = null,
    //                           Encoding? textEncoding = null)
    //{
    //    _ArgumentNullException.ThrowIfNull(data, nameof(data));
    //    Csv.Save(Enumerable.Repeat(data, 1), filePath, formatProvider, format, textEncoding);
    //}
}
