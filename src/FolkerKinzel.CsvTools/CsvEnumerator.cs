using System.Collections;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using FolkerKinzel.CsvTools.Intls;
using FolkerKinzel.CsvTools.Resources;

namespace FolkerKinzel.CsvTools;

/// <summary>Provides read-only forward access to the records of a CSV file. (This
/// means that the <see cref="CsvEnumerator" /> can only read the file forward once.)</summary>
/// <remarks>
/// <para>
/// The class implements <see cref="IEnumerable{T}">IEnumerable&lt;CsvRecord&gt;</see>. A 
/// <see cref="CsvEnumerator"/> instance can be iterated with <c>foreach</c> or queried using 
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
public sealed class CsvEnumerator : IDisposable, IEnumerable<CsvRecord>, IEnumerator<CsvRecord>
{
    private readonly CsvStringReader _reader;
    private readonly bool _hasHeaderRow;

    private CsvRecord? _record = null; // Template for additional CsvRecord objects
    private CsvRecord? _current;

    /// <summary>Initializes a new <see cref="CsvEnumerator" /> instance.</summary>
    /// <param name="filePath">File path of the CSV file to read.</param>
    /// <param name="hasHeaderRow"> <c>true</c>, if the CSV file has a header with column
    /// names.</param>
    /// <param name="options">Options for reading the CSV file.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file
    /// or <c>null</c> for <see cref="Encoding.UTF8" />.</param>
    /// 
    /// <remarks>
    /// <note type="tip">
    /// The optimal constructor parameters can be determined automatically with <see cref="CsvAnalyzer"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">Error accessing the disk.</exception>
    public CsvEnumerator(
        string filePath,
        bool hasHeaderRow = true,
        CsvOpts options = CsvOpts.Default,
        char delimiter = ',',
        Encoding? textEncoding = null)
    {
        StreamReader streamReader = StreamReaderHelper.InitializeStreamReader(filePath, textEncoding);

        this._reader = new CsvStringReader(streamReader, delimiter, options);
        this._hasHeaderRow = hasHeaderRow;
    }

    /// <summary>Initializes a new <see cref="CsvEnumerator" /> instance.</summary>
    /// <param name="reader">The <see cref="TextReader" /> with which the CSV data is
    /// read.</param>
    /// <param name="hasHeaderRow"> <c>true</c>, if the CSV file has a header with column
    /// names, otherwise <c>false</c>.</param>
    /// <param name="options">Options for reading CSV.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="reader" /> is <c>null</c>.</exception>
    public CsvEnumerator(
        TextReader reader,
        bool hasHeaderRow = true,
        CsvOpts options = CsvOpts.Default,
        char delimiter = ',')
    {
        _ArgumentNullException.ThrowIfNull(reader, nameof(reader));

        this._reader = new CsvStringReader(reader, delimiter, options);
        this._hasHeaderRow = hasHeaderRow;
    }

    public CsvOpts Options => _reader.Options;

    public char Delimiter => _reader.Delimiter;

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
    /// on the <see cref="CsvOpts" /> value, specified in the constructor.</exception>
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
    /// on the <see cref="CsvOpts" /> value, specified in the constructor.</exception>
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
                bool trimColumns = Options.HasFlag(CsvOpts.TrimColumns);
                string?[] columnNames = row.Select(x => x.IsEmpty ? null : x.ToString()).ToArray();

                _record = new CsvRecord(columnNames,
                                        Options.HasFlag(CsvOpts.CaseSensitiveKeys),
                                        initArr: Options.HasFlag(CsvOpts.DisableCaching),
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

        if (!Options.HasFlag(CsvOpts.DisableCaching))
        {
            _record = new CsvRecord(_record);
        }

        Fill(row, _reader);

        return _record;

        /////////////////////////////////////////////////////////////////////

        void Fill(CsvRow data, CsvStringReader reader)
        {
            if (data.Count > _record.Count && Options.HasFlag(CsvOpts.ThrowOnTooMuchFields))
            {
                throw new CsvFormatException(Res.TooMuchFields,
                                             CsvError.TooMuchFields,
                                             reader.LineNumber,
                                             reader.LineIndex);
            }

            Span<ReadOnlyMemory<char>> recordSpan = _record.Span;
            int i;
            int count = Math.Min(data.Count, _record.Count);
#if NET8_0_OR_GREATER
            Span<ReadOnlyMemory<char>> dataSpan = CollectionsMarshal.AsSpan(data);

            for (i = 0; i < count; i++)
            {
                ReadOnlyMemory<char> item = dataSpan[i];
#else
            for (i = 0; i < data.Count; i++)
            {
                ReadOnlyMemory<char> item = data[i];
#endif
                recordSpan[i] = item;
            }

            if (i < _record.Count)
            {
                if (row.IsEmpty && Options.HasFlag(CsvOpts.ThrowOnEmptyLines))
                {
                    throw new CsvFormatException(Res.EmptyLine, CsvError.EmptyLine, reader.LineNumber, 0);
                }

                if (Options.HasFlag(CsvOpts.ThrowOnTooFewFields))
                {
                    throw new CsvFormatException(Res.TooFewFields, CsvError.TooFewFields, reader.LineNumber, reader.LineIndex);
                }

                if (Options.HasFlag(CsvOpts.DisableCaching))
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
