using System.Data;
using System.Globalization;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary>
/// Extension methods for the <see cref="CsvRecord"/> class.
/// </summary>
public static class CsvRecordExtension
{
    /// <summary> Converts a <see cref="CsvRecord"/> instance to a 
    /// <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, ReadOnlyMemory&lt;char&gt;&gt;</see>.
    /// 
    /// </summary>
    /// <param name="record">The <see cref="CsvRecord"/> instance to convert.</param>
    /// <returns>A copy of the data stored in <paramref name="record"/> as 
    /// <see cref="Dictionary{TKey, TValue}">Dictionary&lt;string, ReadOnlyMemory&lt;char&gt;&gt;</see>.</returns>
    /// <remarks>
    /// The method copies the data stored in <paramref name="record"/> into a newly created <see cref="Dictionary{TKey,
    /// TValue}">Dictionary&lt;string, ReadOnlyMemory&lt;char&gt;&gt;</see> instance, 
    /// which uses the same <see cref="StringComparer" /> for key comparison that was used to create 
    /// <paramref name="record"/>.
    /// </remarks>
    public static Dictionary<string, ReadOnlyMemory<char>> ToDictionary(this CsvRecord record)
    {
#if NETSTANDARD2_0 || NET462
        var dic = new Dictionary<string, ReadOnlyMemory<char>>(record.Count, record.Comparer);

        foreach (KeyValuePair<string, ReadOnlyMemory<char>> kvp in record)
        {
            dic.Add(kvp.Key, kvp.Value);
        }

        return dic;
#else
        return new Dictionary<string, ReadOnlyMemory<char>>(record, record.Comparer);
#endif
    }

    /// <summary> Fills <paramref name="record"/> with the items of an 
    /// <see cref="object"/> collection.
    /// </summary>
    /// <param name="record">The <see cref="CsvRecord" /> instance to be filled.</param>
    /// <param name="data">The <see cref="object"/>s with which to fill <paramref name="record"/>.
    /// The argument may be <c>null</c> or may contain <c>null</c> values.</param>
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
    /// <param name="resetExcess">
    /// <para>
    /// If <paramref name="data"/> has fewer items than <paramref name="record"/> fields and this
    /// parameter is <c>true</c>, the  surplus fields in <paramref name="record"/> will be reset 
    /// to the default value .
    /// </para>
    /// <para>
    /// For performance reasons this parameter can be set to <c>false</c> when writing CSV because 
    /// <see cref = "CsvWriter.WriteRecord" /> resets all fields in <paramref name = "record" />.
    /// </para>
    /// </param>
    /// 
    /// <remarks>
    /// For serialization <see cref="IFormattable.ToString(string, IFormatProvider)"/> is used if the
    /// item implements <see cref="IFormattable"/>, otherwise <see cref="object.ToString"/>.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="record"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more items than <paramref name="record"/>.</exception>
    public static void FillWith(this CsvRecord record,
                                IEnumerable<object?>? data,
                                IFormatProvider? formatProvider = null,
                                string? format = null,
                                bool resetExcess = true)
    {
        _ArgumentNullException.ThrowIfNull(record, nameof(record));

        int i = 0;

        Span<ReadOnlyMemory<char>> span = record.Values;

        if (data is not null)
        {
            foreach (object? item in data)
            {
                if (i >= span.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(data));
                }

                span[i++] = item switch
                {
                    null => default,
                    string s => s.AsMemory(),
                    IFormattable formattable => formattable.ToString(format, formatProvider).AsMemory(),
                    _ => item.ToString().AsMemory()
                };
            }
        }

        if (resetExcess)
        {
            for (; i < span.Length; i++)
            {
                span[i] = default;
            }
        }
    }

    /// <summary> Fills <paramref name="record"/> with the items of a 
    /// <see cref="string"/> collection.
    /// </summary>
    /// <param name="record">The <see cref="CsvRecord" /> instance to be filled.</param>
    /// <param name="data">The <see cref="string"/>s with which to fill <paramref name="record"/>.
    /// The argument may be <c>null</c> or may contain <c>null</c> values.</param>
    /// <param name="resetExcess">
    /// <para>
    /// If <paramref name="data"/> has fewer items than <paramref name="record"/> fields and this
    /// parameter is <c>true</c>, the  surplus fields in <paramref name="record"/> will be reset 
    /// to the default value .
    /// </para>
    /// <para>
    /// For performance reasons this parameter can be set to <c>false</c> when writing CSV because 
    /// <see cref = "CsvWriter.WriteRecord" /> resets all fields in <paramref name = "record" />.
    /// </para>
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="record"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more items than <paramref name="record"/>.</exception>
    public static void FillWith(this CsvRecord record,
                                IEnumerable<string?>? data,
                                bool resetExcess = true)
    {
        _ArgumentNullException.ThrowIfNull(record, nameof(record));

        int i = 0;

        Span<ReadOnlyMemory<char>> span = record.Values;

        if (data is not null)
        {
            foreach (string? item in data)
            {
                if (i >= span.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(data));
                }

                span[i++] = item.AsMemory();
            }
        }

        if (resetExcess)
        {
            for (; i < span.Length; i++)
            {
                span[i] = default;
            }
        }
    }

    /// <summary> Fills <paramref name="record"/> with the content of a 
    /// <see cref="string"/> array.
    /// </summary>
    /// <param name="record">The <see cref="CsvRecord" /> instance to be filled.</param>
    /// <param name="data">The <see cref="string"/>s with which to fill <paramref name="record"/>.
    /// The argument may be <c>null</c> or may contain <c>null</c> values.</param>
    /// <param name="resetExcess">
    /// <para>
    /// If <paramref name="data"/> has fewer items than <paramref name="record"/> fields and this
    /// parameter is <c>true</c>, the  surplus fields in <paramref name="record"/> will be reset 
    /// to the default value .
    /// </para>
    /// <para>
    /// For performance reasons this parameter can be set to <c>false</c> when writing CSV because 
    /// <see cref = "CsvWriter.WriteRecord" /> resets all fields in <paramref name = "record" />.
    /// </para>
    /// </param>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="record"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more items than <paramref name="record"/>.</exception>
    public static void FillWith(this CsvRecord record, string?[]? data, bool resetExcess = true)
        => FillWith(record, data.AsSpan(), resetExcess);

    /// <summary> Fills <paramref name="record"/> with the items
    /// of a read-only span of <see cref="string"/>s.
    /// </summary>
    /// <param name="record">The <see cref="CsvRecord" /> instance to be filled.</param>
    /// <param name="data">The <see cref="string"/>s with which to fill <paramref name="record"/>. 
    /// The span may contain <c>null</c> values.</param>
    /// <param name="resetExcess">
    /// <para>
    /// If <paramref name="data"/> has fewer items than <paramref name="record"/> fields and this
    /// parameter is <c>true</c>, the  surplus fields in <paramref name="record"/> will be reset 
    /// to the default value .
    /// </para>
    /// <para>
    /// For performance reasons this parameter can be set to <c>false</c> when writing CSV because 
    /// <see cref = "CsvWriter.WriteRecord" /> resets all fields in <paramref name = "record" />.
    /// </para>
    /// </param>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more items than <paramref name="record"/>.</exception>
    /// <exception cref="ArgumentNullException"> <paramref name="record"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more items than <paramref name="record"/>.</exception>
    public static void FillWith(this CsvRecord record,
                                ReadOnlySpan<string?> data,
                                bool resetExcess = true)
    {
        _ArgumentNullException.ThrowIfNull(record, nameof(record));

        if (data.Length > record.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(data));
        }

        int i = 0;
        Span<ReadOnlyMemory<char>> span = record.Values;

        foreach (string? item in data)
        {
            span[i++] = item.AsMemory();
        }

        if (resetExcess)
        {
            for (; i < span.Length; i++)
            {
                span[i] = default;
            }
        }
    }

    /// <summary> Fills <paramref name="record"/> with the items
    /// of a read-only span of
    /// <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> values.
    /// </summary>
    /// <param name="record">The <see cref="CsvRecord" /> instance to be filled.</param>
    /// <param name="data">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see>
    /// values with which to fill <paramref name="record"/>.
    /// </param>
    /// <param name="resetExcess">
    /// <para>
    /// If <paramref name="data"/> has fewer items than <paramref name="record"/> fields and this
    /// parameter is <c>true</c>, the  surplus fields in <paramref name="record"/> will be reset 
    /// to the default value .
    /// </para>
    /// <para>
    /// For performance reasons this parameter can be set to <c>false</c> when writing CSV because 
    /// <see cref = "CsvWriter.WriteRecord" /> resets all fields in <paramref name = "record" />.
    /// </para>
    /// </param>
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
    /// <exception cref="ArgumentNullException"> <paramref name="record"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more items than <paramref name="record"/>.</exception>
    public static void FillWith(this CsvRecord record,
                                ReadOnlySpan<ReadOnlyMemory<char>> data,
                                bool resetExcess = true)
    {
        _ArgumentNullException.ThrowIfNull(record, nameof(record));

        if (data.Length > record.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(data));
        }

        int i = 0;
        Span<ReadOnlyMemory<char>> span = record.Values;

        foreach (ReadOnlyMemory<char> item in data)
        {
            span[i++] = item;
        }

        if (resetExcess)
        {
            for (; i < span.Length; i++)
            {
                span[i] = default;
            }
        }
    }

    internal static void FillWith(this CsvRecord record,
                                  DataRow dataRow,
                                  IFormatProvider? formatProvider,
                                  string? format)
    {
        if (dataRow.RowState == DataRowState.Deleted)
        {
            return;
        }

        Debug.Assert(record.ColumnNames is string[]);

        ReadOnlySpan<string> columnNames = (string[])record.ColumnNames;
        Span<ReadOnlyMemory<char>> recordSpan = record.Values;

        for (int i = 0; i < recordSpan.Length; i++)
        {
            object item = dataRow[columnNames[i]];

            recordSpan[i] = item switch
            {
                DBNull => default,
                string s => s.AsMemory(),
                IFormattable formattable => formattable.ToString(format, formatProvider).AsMemory(),
                _ => item.ToString().AsMemory()
            };
        }
    }
}
