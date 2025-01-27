﻿using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary>
/// Extension methods for CSV data.
/// </summary>
public static class CsvExtension
{
    /// <summary>
    /// Converts the content of <paramref name="data"/> to a comma-separated values <see cref="string"/> 
    /// (CSV, RFC 4180).
    /// </summary>
    /// <param name="data">The data to convert.</param>
    /// <returns>A CSV-<see cref="string"/> containing the content of <paramref name="data"/>.</returns>
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
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToCsv(this IEnumerable<IEnumerable<string?>?> data)
        => Csv.AsString(data);

    /// <summary>
    /// Converts the content of <paramref name="data"/> to a comma-separated values <see cref="string"/> (CSV, RFC 4180).
    /// </summary>
    /// <param name="data">The data to convert.</param>
    /// <returns>A CSV-<see cref="string"/> containing the content of <paramref name="data"/>.</returns>
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> is <c>null</c>.</exception>
    public static string ToCsv(this IEnumerable<string?> data)
    {
        _ArgumentNullException.ThrowIfNull(data, nameof(data));
        return Csv.AsString(Enumerable.Repeat(data, 1));
    }

    /// <summary>
    /// Saves the content of <paramref name="data"/> as a CSV file.
    /// </summary>
    /// <param name="data">The data to save.</param>
    /// <param name="filePath">The file path of the CSV file to be written.</param>
    /// 
    /// <remarks>
    /// <para>Creates a new CSV file. If the target file already exists, it is 
    /// truncated and overwritten.
    /// </para>
    /// <para>
    /// The CSV file that this method creates uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the constructor of
    /// <see cref="CsvWriter"/> directly.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> or 
    /// <paramref name="filePath"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a 
    /// valid file path.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SaveCsv(this IEnumerable<IEnumerable<string?>?> data, 
                               string filePath) => Csv.Save(data, filePath);

    /// <summary>
    /// Saves the content of <paramref name="data"/> as a CSV file.
    /// </summary>
    /// <param name="data">The data to save.</param>
    /// <param name="filePath">The file path of the CSV file to be written.</param>
    /// 
    /// <remarks>
    /// <para>Creates a new CSV file. If the target file already exists, it is 
    /// truncated and overwritten.
    /// </para>
    /// <para>
    /// The CSV file that this method creates uses the comma ',' (%x2C) as field delimiter.
    /// This complies with the RFC 4180 standard. If another delimiter is required, use the constructor of
    /// <see cref="CsvWriter"/> directly.
    /// </para>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="data" /> or 
    /// <paramref name="filePath"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a 
    /// valid file path.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static void SaveCsv(this IEnumerable<string?> data,
                               string filePath)
    {
        _ArgumentNullException.ThrowIfNull(data, nameof(data));
        Csv.Save(Enumerable.Repeat(data, 1), filePath);
    }
}
