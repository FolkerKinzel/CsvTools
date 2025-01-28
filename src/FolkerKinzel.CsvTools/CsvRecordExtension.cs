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

    /// <summary> Fills <paramref name="record"/> with the items of a 
    /// <see cref="string"/> collection.
    /// </summary>
    /// <param name="record">The <see cref="CsvRecord" /> instance to be filled.</param>
    /// <param name="data">The <see cref="string"/>s with which to fill <paramref name="record"/>.
    /// The argument may be <c>null</c> or may contain <c>null</c> values.</param>
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

        if(resetExcess)
        {
            for (int j = i; j < span.Length; j++)
            {
                span[j] = default;
            }
        }
    }

    /// <summary> Fills <paramref name="record"/> with the content of a 
    /// <see cref="string"/> array.
    /// </summary>
    /// <param name="record">The <see cref="CsvRecord" /> instance to be filled.</param>
    /// <param name="data">The <see cref="string"/>s with which to fill <paramref name="record"/>.
    /// The argument may be <c>null</c> or may contain <c>null</c> values.</param>
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
            for (int j = i; j < span.Length; j++)
            {
                span[j] = default;
            }
        }
    }

    /// <summary> Fills <paramref name="record"/> with the items
    /// of a read-only span of
    /// <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> values.
    /// </summary>
    /// <param name="record">The <see cref="CsvRecord" /> instance to be filled.</param>
    /// <param name="data">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see>
    /// values with which to fill <paramref name="record"/>.</param>
    /// 
    /// 
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
            for (int j = i; j < span.Length; j++)
            {
                span[j] = default;
            }
        }
    }
}
