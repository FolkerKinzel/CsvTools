﻿using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

public static class CsvRecordExtension
{
    /// <summary> Returns a copy of the data stored in <paramref name="record"/> as <see cref="Dictionary{TKey,
    /// TValue}">Dictionary&lt;string, ReadOnlyMemory&lt;char&gt;&gt;</see>, 
    /// which uses the same <see cref="StringComparer" /> for key comparison that was used to create 
    /// <paramref name="record"/>.
    ///</summary>
    /// <returns>A copy of the data stored in <paramref name="record"/>.</returns>
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

    /// <summary> Fills <paramref name="record"/> with the contents of a 
    /// <see cref="string"/> collection.
    /// </summary>
    /// <param name="record">The <see cref="CsvRecord" /> instance to be filled.</param>
    /// <param name="data">The contents with which to populate <paramref name="record"/>.
    /// The argument may be <c>null</c> or may contain <c>null</c> values.</param>
    /// 
    /// <remarks>If <paramref name="data" /> has fewer entries than <paramref name="record"/>, 
    /// the remaining fields of <paramref name="record"/> will be set to <c>default</c>.</remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="record"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more items than <paramref name="record"/>.</exception>
    public static void FillWith(this CsvRecord record, IEnumerable<string?>? data)
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

        for (int j = i; j < span.Length; j++)
        {
            span[j] = default;
        }
    }

    /// <summary> Fills <paramref name="record"/> with the contents of a 
    /// <see cref="string"/> array.
    /// </summary>
    /// <param name="data">The contents with which to populate <paramref name="record"/>.
    /// The argument may be <c>null</c> or may contain <c>null</c> values.</param>
    /// 
    /// <remarks>If <paramref name="data" /> has fewer entries than <paramref name="record"/>, 
    /// the remaining fields of <paramref name="record"/> will be set to <c>default</c>.</remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="record"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more items than <paramref name="record"/>.</exception>
    public static void FillWith(this CsvRecord record, string?[]? data)
        => FillWith(record, data.AsSpan());

    /// <summary> Fills <paramref name="record"/> with the contents 
    /// of a read-only span of <see cref="string"/>s.
    /// </summary>
    /// <param name="record">The <see cref="CsvRecord" /> instance to be filled.</param>
    /// <param name="data">The contents with which to populate <paramref name="record"/>. 
    /// The span may contain <c>null</c> values.</param>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more entries than <paramref name="record"/>.</exception>
    /// <remarks>If <paramref name="data" /> has fewer entries than <paramref name="record"/>, 
    /// the remaining fields of <paramref name="record"/> will be set to <c>default</c>.</remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="record"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more entries than <paramref name="record"/>.</exception>
    public static void FillWith(this CsvRecord record, ReadOnlySpan<string?> data)
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

        for (int j = i; j < span.Length; j++)
        {
            span[j] = default;
        }
    }

    /// <summary> Fills <paramref name="record"/> with the contents 
    /// of a read-only span of
    /// <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> values.
    /// </summary>
    /// <param name="record">The <see cref="CsvRecord" /> instance to be filled.</param>
    /// <param name="data">The contents with which to populate <paramref name="record"/>.</param>
    /// 
    /// <remarks>If <paramref name="data" /> has fewer entries than <paramref name="record"/>, 
    /// the remaining fields of <paramref name="record"/> will be set to <c>default</c>.</remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="record"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="data" /> contains
    /// more entries than <paramref name="record"/>.</exception>
    public static void FillWith(this CsvRecord record, ReadOnlySpan<ReadOnlyMemory<char>> data)
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

        for (int j = i; j < span.Length; j++)
        {
            span[j] = default;
        }
    }
}