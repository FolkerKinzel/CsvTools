using System.Collections;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using FolkerKinzel.CsvTools.Intls;
using FolkerKinzel.CsvTools.Resources;

namespace FolkerKinzel.CsvTools;

/// <summary>Provides read-only forward access to the records of a CSV file. (This
/// means that the <see cref="CsvReader" /> can only read the file forward once.)</summary>
/// <remarks>
/// <para>
/// The class implements <see cref="IEnumerable{T}">IEnumerable&lt;CsvRecord&gt;</see>. A 
/// <see cref="CsvReader"/> instance can be iterated with <c>foreach</c> or queried using 
/// Linq methods. Note that an instance can only be iterated once; if an attempt is made to
/// iterate it twice, an <see cref="ObjectDisposedException"/> is thrown.
/// </para>
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
    private CsvRecord? _current;

    /// <summary>Initializes a new <see cref="CsvReader" /> instance.</summary>
    /// <param name="fileName">File path of the CSV file to read.</param>
    /// <param name="hasHeaderRow"> <c>true</c>, if the CSV file has a header with column
    /// names.</param>
    /// <param name="options">Options for reading the CSV file.</param>
    /// <param name="fieldSeparator">The field separator char used in the CSV file.</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file
    /// or <c>null</c> for <see cref="Encoding.UTF8" />.</param>
    /// 
    /// <remarks>
    /// <note type="tip">
    /// The optimal constructor parameters can be determined automatically with <see cref="CsvAnalyzer"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="fileName" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="fileName" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">Error accessing the disk.</exception>
    public CsvReader(
        string fileName,
        bool hasHeaderRow = true,
        CsvOptions options = CsvOptions.Default,
        char fieldSeparator = ',',
        Encoding? textEncoding = null)
    {
        StreamReader streamReader = StreamReaderHelper.InitializeStreamReader(fileName, textEncoding);

        this._options = options;
        this._reader = new CsvStringReader(streamReader, fieldSeparator, options);
        this._hasHeaderRow = hasHeaderRow;
    }

    /// <summary>Initializes a new <see cref="CsvReader" /> object.</summary>
    /// <param name="reader">The <see cref="TextReader" /> with which the CSV file is
    /// read.</param>
    /// <param name="hasHeaderRow"> <c>true</c>, if the CSV file has a header with column
    /// names.</param>
    /// <param name="options">Options for reading the CSV file.</param>
    /// <param name="fieldSeparator">The field separator char used in the CSV file.</param>
    /// 
    /// <remarks>
    /// <note type="tip">
    /// The optimal constructor parameters can be determined automatically with <see cref="CsvAnalyzer"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="reader" /> is <c>null</c>.</exception>
    public CsvReader(
        TextReader reader,
        bool hasHeaderRow = true,
        CsvOptions options = CsvOptions.Default,
        char fieldSeparator = ',')
    {
        _ArgumentNullException.ThrowIfNull(reader, nameof(reader));

        this._options = options;
        this._reader = new CsvStringReader(reader, fieldSeparator, options);
        this._hasHeaderRow = hasHeaderRow;
    }

    /// <inheritdoc/>
    CsvRecord IEnumerator<CsvRecord>.Current => _current!;

    /// <inheritdoc/>
    object IEnumerator.Current => ((IEnumerator<CsvRecord>)this).Current;

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public IEnumerator<CsvRecord> GetEnumerator() => this;

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">The CSV file was already
    /// closed.</exception>
    /// <exception cref="IOException">Error accessing the file.</exception>
    /// <exception cref="CsvFormatException">Invalid CSV file. The interpretation depends
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
    /// <exception cref="CsvFormatException">Invalid CSV file. The interpretation depends
    /// on the <see cref="CsvOptions" /> value, specified in the constructor.</exception>
    private CsvRecord? Read()
    {
        CsvRow? row = _reader.Read();

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
                                        initArr: _options.HasFlag(CsvOptions.DisableCaching),
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

        void Fill(CsvRow data, CsvStringReader reader)
        {
            if (data.Count > _record.Count && _options.HasFlag(CsvOptions.ThrowOnTooMuchFields))
            {
                throw new CsvFormatException(Res.TooMuchFields,
                                             CsvError.TooMuchFields,
                                             reader.LineNumber,
                                             reader.LineIndex);
            }

            Span<ReadOnlyMemory<char>> recordSpan = _record.Span;
            int i;
#if NET8_0_OR_GREATER
            Span<ReadOnlyMemory<char>> dataSpan = CollectionsMarshal.AsSpan(data);

            for (i = 0; i < data.Count; i++)
            {
                ReadOnlyMemory<char> item = dataSpan[i];
#else
            for (i = 0; i < data.Count; i++)
            {
                ReadOnlyMemory<char> item = data[i];
#endif
                recordSpan[i] = item.Length != 0 && _options.HasFlag(CsvOptions.TrimColumns)
                             ? item.Trim()
                             : item;
            }

            if (i < _record.Count)
            {
                if (row.IsEmpty && _options.HasFlag(CsvOptions.ThrowOnEmptyLines))
                {
                    throw new CsvFormatException(Res.EmptyLine, CsvError.EmptyLine, reader.LineNumber, 0);
                }

                if (_options.HasFlag(CsvOptions.ThrowOnTooFewFields))
                {
                    throw new CsvFormatException(Res.TooFewFields, CsvError.TooFewFields, reader.LineNumber, reader.LineIndex);
                }

                if (_options.HasFlag(CsvOptions.DisableCaching))
                {
                    for (int j = i; j < _record.Count; j++)
                    {
                        recordSpan[j] = default;
                    }
                }
            }
        }// Fill()
    }

    #endregion
}
