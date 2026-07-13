using System.Data;
using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools;

/// <summary>
/// Extension methods for the <see cref="DataTable"/> class.
/// </summary>
public static class DataTableExtension
{
    /// <summary>
    /// Writes the content of the <see cref="DataTable"/> as a CSV file with header.
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
    /// that allows to select the <see cref="DataColumn"/>s to export and to determine their 
    /// order in the CSV file, or <c>null</c> to save the whole <see cref="DataTable"/> using
    /// its current column order. 
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
    /// <see cref="Csv.GetExcelArguments"/>.
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <note type="note">
    /// In the following code examples - for easier readability - exception handling has been omitted.
    /// </note>
    /// <para>
    /// Simple DataTable serialization with CSV:
    /// </para>
    /// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\DataTableExample.cs" />
    /// </example>
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
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the double 
    /// quotes <c>"</c> or a line break character ('\r' or  '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteCsv(this DataTable dataTable,
                                string filePath,
                                char delimiter = ',',
                                IFormatProvider? formatProvider = null,
                                Encoding? textEncoding = null,
                                IEnumerable<string>? csvColumnNames = null,
                                string? format = null)
        => Csv.Save(dataTable, filePath, delimiter, formatProvider, textEncoding, csvColumnNames, format);

    /// <summary>
    /// Writes the content of the <see cref="DataTable"/> as CSV.
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
    /// <see cref="Csv.GetExcelArguments"/>.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteCsv(this DataTable dataTable,
                                TextWriter textWriter,
                                char delimiter = ',',
                                IFormatProvider? formatProvider = null,
                                IEnumerable<string>? csvColumnNames = null,
                                string? format = null)
        => Csv.Write(dataTable, textWriter, delimiter, formatProvider, csvColumnNames, format);
}
