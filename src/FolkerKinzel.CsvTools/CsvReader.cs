using System.Collections;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using FolkerKinzel.CsvTools.Intls;
using FolkerKinzel.CsvTools.Resources;

namespace FolkerKinzel.CsvTools;

/// <summary>Provides read-only forward access to the records (rows) of a CSV file.</summary>
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
/// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\LinqOnCsvExample.cs" />
/// </example>
public sealed class CsvReader : IDisposable, IEnumerable<CsvRecord>, IEnumerator<CsvRecord>
{
    private readonly CsvStringReader _reader;
    private readonly bool _hasHeaderRow;
    private readonly int? _rowLength;

    private CsvRecord? _record = null; // Template for additional CsvRecord objects
    private CsvRecord? _current;
    private bool _disposed;

    /// <summary>
    /// Initializes a new <see cref="CsvReader"/> instance to read a CSV file without header row.
    /// </summary>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="analyzerResult">The results of the analysis of the CSV file.</param>
    /// <param name="textEncoding">The <see cref="Encoding"/> to use, or <c>null</c> for 
    /// <see cref="Encoding.UTF8"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is not a valid file path.
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    internal CsvReader(string filePath,
                       CsvAnalyzerResult analyzerResult,
                       Encoding? textEncoding)
    {
        StreamReader streamReader = StreamHelper.InitStreamReader(filePath, textEncoding);
        this._reader = new CsvStringReader(streamReader, analyzerResult.Delimiter, analyzerResult.Options);
        _rowLength = analyzerResult.RowLength;
    }

    internal CsvReader(TextReader reader,
                       CsvAnalyzerResult result)
    {
        this._reader = new CsvStringReader(reader, result.Delimiter, result.Options);
        _rowLength = result.RowLength;
    }

    /// <summary>Initializes a new <see cref="CsvReader" /> instance.</summary>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="textEncoding">The text encoding to be used to read the CSV file or <c>null</c> for 
    /// <see cref="Encoding.UTF8" />.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, otherwise 
    /// <c>false</c>.</param>
    /// <param name="options">Options for reading the CSV file.</param>
    /// 
    /// <remarks>
    /// <note type="tip">
    /// The optimal constructor parameters can be determined automatically with <see cref="CsvAnalyzer"/>.
    /// </note>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid file path.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the double 
    /// quotes <c>"</c> or a line break character ('\r' or  '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public CsvReader(string filePath,
                     char delimiter = ',',
                     Encoding? textEncoding = null,
                     bool isHeaderPresent = true,
                     CsvOpts options = CsvOpts.Default)
    {
        _ArgumentOutOfRangeException.ValidateDelimiter(delimiter);

        StreamReader streamReader = StreamHelper.InitStreamReader(filePath, textEncoding);

        this._reader = new CsvStringReader(streamReader, delimiter, options);
        this._hasHeaderRow = isHeaderPresent;
    }

    /// <summary>Initializes a new <see cref="CsvReader" /> instance.</summary>
    /// <param name="reader">The <see cref="TextReader" /> with which the CSV data is read.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="isHeaderPresent"> <c>true</c>, to interpret the first line as a header, otherwise 
    /// <c>false</c>.</param>
    /// <param name="options">Options for reading CSV.</param>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="reader" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either 
    /// the double quotes <c>"</c> or a line break character ('\r' or  '\n').</exception>
    public CsvReader(TextReader reader,
                     char delimiter = ',',
                     bool isHeaderPresent = true,
                     CsvOpts options = CsvOpts.Default)
    {
        _ArgumentNullException.ThrowIfNull(reader, nameof(reader));
        _ArgumentOutOfRangeException.ValidateDelimiter(delimiter);

        this._reader = new CsvStringReader(reader, delimiter, options);
        this._hasHeaderRow = isHeaderPresent;
    }

    /// <summary>
    /// Gets the options for reading the CSV file.
    /// </summary>
    public CsvOpts Options => _reader.Options;

    /// <summary>
    /// Gets the field separator character.
    /// </summary>
    public char Delimiter => _reader.Delimiter;

    /// <inheritdoc/>
    CsvRecord IEnumerator<CsvRecord>.Current => _current!;

    /// <inheritdoc/>
    object IEnumerator.Current => ((IEnumerator<CsvRecord>)this).Current;

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this;

    /// <inheritdoc/>
    public IEnumerator<CsvRecord> GetEnumerator() => this;

    /// <inheritdoc/>
    /// <exception cref="ObjectDisposedException">The CSV file was already closed.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="CsvFormatException">Invalid CSV file. The interpretation depends on the 
    /// <see cref="CsvOpts" /> value, specified in the constructor.</exception>
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
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _reader.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>Returns the next <see cref="CsvRecord"/> in the CSV file or <c>null</c> 
    /// if the file has been read completely.</summary>
    /// <returns>The next <see cref="CsvRecord"/> in the CSV file or <c>null</c> 
    /// if the file has been read completely.</returns>
    /// 
    /// <exception cref="ObjectDisposedException">The CSV file was already
    /// closed.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="CsvFormatException">Invalid CSV file. The interpretation depends
    /// on the <see cref="CsvOpts" /> value, specified in the constructor.</exception>
    public CsvRecord? Read()
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
                string[] columnNames = [.. row.Select(x => x.ToString())];

                _record = new CsvRecord(columnNames,
                                        Options.HasFlag(CsvOpts.CaseSensitiveKeys),
                                        initArr: Options.HasFlag(CsvOpts.DisableCaching),
                                        throwException: false);
                return Read();
            }
            else
            {
                _record = new CsvRecord(_rowLength ?? row.Count);
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

            Span<ReadOnlyMemory<char>> recordSpan = _record.Values;
            int i;
            int count = Math.Min(data.Count, _record.Count);
#if NET8_0_OR_GREATER
            Span<ReadOnlyMemory<char>> dataSpan = CollectionsMarshal.AsSpan(data);

            for (i = 0; i < count; i++)
            {
                ReadOnlyMemory<char> item = dataSpan[i];
#else
            for (i = 0; i < count; i++)
            {
                ReadOnlyMemory<char> item = data[i];
#endif
                recordSpan[i] = item;
            }

            if (i < _record.Count)
            {
                if (row.IsEmpty)
                {
                    if (Options.HasFlag(CsvOpts.ThrowOnEmptyLines))
                    {
                        throw new CsvFormatException(Res.EmptyLine,
                                                     CsvError.EmptyLine,
                                                     reader.LineNumber,
                                                     0);
                    }
                }
                else if (Options.HasFlag(CsvOpts.ThrowOnTooFewFields))
                {
                    throw new CsvFormatException(Res.TooFewFields,
                                                 CsvError.TooFewFields,
                                                 reader.LineNumber,
                                                 reader.LineIndex);
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
}
