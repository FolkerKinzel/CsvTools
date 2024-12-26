using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using FolkerKinzel.CsvTools.Intls;
using FolkerKinzel.CsvTools.Resources;
using System.Collections;
using System.Numerics;

#if NET462 || NETSTANDARD2_0 || NETSTANDARD2_1
using FolkerKinzel.Strings;
#endif

namespace FolkerKinzel.CsvTools;

/// <summary>Provides read-only forward access to the records of a CSV file. (This
/// means that the <see cref="CsvReader" /> can only read the file forward once.)</summary>
/// <remarks>
/// The class implements <see cref="IEnumerable{T}">IEnumerable&lt;CsvRecord&gt;</see>. A 
/// <see cref="CsvReader"/> instance can be iterated with <c>foreach</c> or queried using 
/// Linq methods.
/// </remarks>
/// <example>
/// <note type="note">
/// In the following code examples - for easier readability - exception handling
/// has been omitted.
/// </note>
/// <para>
/// Linq query on a CSV file:
/// </para>
/// <code language="cs" source="..\Examples\LinqOnCsvFile.cs" />
/// </example>
public sealed class CsvReader : IDisposable, IEnumerable<CsvRecord>, IEnumerator<CsvRecord>
{
    private readonly CsvStringReader _reader;
    private readonly CsvOptions _options;
    private readonly bool _hasHeaderRow;

    private CsvRecord? _record = null; // Schablone für weitere CsvRecord-Objekte
    private CsvRecord _current;

    /// <summary>Initializes a new <see cref="CsvReader" /> instance.</summary>
    /// <param name="fileName">File path of the CSV file to read.</param>
    /// <param name="hasHeaderRow"> <c>true</c>, if the CSV file has a header with column
    /// names.</param>
    /// <param name="options">Options for reading the CSV file.</param>
    /// <param name="fieldSeparator">The field separator char used in the CSV file.</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file
    /// or <c>null</c> for <see cref="Encoding.UTF8" />.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileName" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="fileName" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">Error accessing the disk.</exception>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public CsvReader(
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        string fileName,
        bool hasHeaderRow = true,
        CsvOptions options = CsvOptions.Default,
        char fieldSeparator = ',',
        Encoding? textEncoding = null)
    {
        StreamReader streamReader = StreamReaderHelper.InitializeStreamReader(fileName, textEncoding);

        this._options = options;
        this._reader = new CsvStringReader(streamReader, fieldSeparator, !options.HasFlag(CsvOptions.ThrowOnEmptyLines));
        this._hasHeaderRow = hasHeaderRow;
    }

    /// <summary>Initializes a new <see cref="CsvReader" /> object.</summary>
    /// <param name="reader">The <see cref="TextReader" /> with which the CSV file is
    /// read.</param>
    /// <param name="hasHeaderRow"> <c>true</c>, if the CSV file has a header with column
    /// names.</param>
    /// <param name="options">Options for reading the CSV file.</param>
    /// <param name="fieldSeparator">The field separator char used in the CSV file.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="reader" /> is <c>null</c>.</exception>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public CsvReader(
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        TextReader reader,
        bool hasHeaderRow = true,
        CsvOptions options = CsvOptions.Default,
        char fieldSeparator = ',')
    {
        _ArgumentNullException.ThrowIfNull(reader, nameof(reader));

        this._options = options;
        this._reader = new CsvStringReader(reader, fieldSeparator, !options.HasFlag(CsvOptions.ThrowOnEmptyLines));
        this._hasHeaderRow = hasHeaderRow;
    }

    /// <inheritdoc/>
    CsvRecord IEnumerator<CsvRecord>.Current => _current;

    /// <inheritdoc/>
    object? IEnumerator.Current => ((IEnumerator<CsvRecord>)this).Current;

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public IEnumerator<CsvRecord> GetEnumerator() => this;

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">The CSV file was already
    /// closed.</exception>
    /// <exception cref="IOException">Error accessing the file.</exception>
    /// <exception cref="InvalidCsvException">Invalid CSV file. The interpretation depends
    /// on the <see cref="CsvOptions" /> value, specified in the constructor.</exception>
    bool IEnumerator.MoveNext()
    {
        CsvRecord? record = Read();

        if (record is null)
        {
            return false;
        }

        _current = record;
        return true;
    }

    /// <summary>
    /// Throws a <see cref="NotSupportedException"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">The method has been called.</exception>
    void IEnumerator.Reset() => throw new NotSupportedException();

    /// <summary>Closes the CSV file.</summary>
    public void Dispose() => _reader.Dispose();

    #region private

    /// <summary>Returns the next <see cref="CsvRecord"/> in the CSV file or <c>null</c> 
    /// if the file has been read completely.</summary>
    /// <returns>The next <see cref="CsvRecord"/> in the CSV file or <c>null</c> 
    /// if the file has been read completely.</returns>
    /// 
    /// <exception cref="ObjectDisposedException">The CSV file was already
    /// closed.</exception>
    /// <exception cref="IOException">Error accessing the file.</exception>
    /// <exception cref="InvalidCsvException">Invalid CSV file. The interpretation depends
    /// on the <see cref="CsvOptions" /> value, specified in the constructor.</exception>
    private CsvRecord? Read()
    {
        List<ReadOnlyMemory<char>>? row = _reader.Read();

        if (row is null)
        {
            Dispose();
            return null;
        }

        if (_record is null)
        {
            if (_hasHeaderRow)
            {
                bool trimColumns = _options.HasFlag(CsvOptions.TrimColumns);
                string?[] columnNames = trimColumns ? row.Select(TrimConvert).ToArray() : row.Select(x => x.IsEmpty ? null : x.ToString()).ToArray();

                _record = new CsvRecord(columnNames,
                                        _options.HasFlag(CsvOptions.CaseSensitiveKeys),
                                        trimColumns,
                                        initArr: false,
                                        throwException: false);
                return Read();
            }
            else
            {
                _record = new CsvRecord(row.Count);
                Fill(row, _reader);
                return _record;
            }
        }

        if (!_options.HasFlag(CsvOptions.DisableCaching))
        {
            _record = new CsvRecord(_record);
        }

        Fill(row, _reader);

        return _record;

        /////////////////////////////////////////////////////////////////////

        static string? TrimConvert(ReadOnlyMemory<char> value)
        {
            ReadOnlySpan<char> span = value.Span.Trim();
            return span.IsEmpty ? null : span.ToString();
        }

        void Fill(IList<ReadOnlyMemory<char>> data, CsvStringReader reader)
        {
            int i;
            for (i = 0; i < data.Count; i++)
            {
                if (i >= _record.Count)
                {
                    if (_options.HasFlag(CsvOptions.ThrowOnTooMuchFields))
                    {
                        throw new InvalidCsvException("Too much fields in a record.", reader.LineNumber, reader.LineIndex);
                    }
                    else
                    {
                        return;
                    }
                }

                ReadOnlyMemory<char> item = data[i];

                _record[i] = item.Length != 0 && _options.HasFlag(CsvOptions.TrimColumns)
                             ? item.Trim()
                             : item;
            }


            if (i < _record.Count)
            {
                if (i == 1 && _options.HasFlag(CsvOptions.ThrowOnEmptyLines) && data[0].IsEmpty)
                {
                    throw new InvalidCsvException("Unmasked empty line.", reader.LineNumber, 0);
                }

                if (_options.HasFlag(CsvOptions.ThrowOnTooFewFields))
                {
                    throw new InvalidCsvException("Too few fields in a record.", reader.LineNumber, reader.LineIndex);
                }

                if (_options.HasFlag(CsvOptions.DisableCaching))
                {
                    for (int j = i; j < _record.Count; j++)
                    {
                        _record[j] = default;
                    }
                }
            }
        }// Fill()
    }

    #endregion
}
