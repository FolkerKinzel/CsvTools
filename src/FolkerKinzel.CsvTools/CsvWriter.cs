using System.Text;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary>Writes string data as CSV (RFC 4180).</summary>
/// 
/// <remarks> <see cref="CsvWriter" /> provides a <see cref="CsvRecord" /> object in its 
/// <see cref="Record" /> property that represents a buffer for a data record (row) of the 
/// CSV file. Fill the <see cref="CsvRecord" /> object with data and then write it to the 
/// file using the <see cref="WriteRecord" /> method! After the <see cref="WriteRecord" /> 
/// returns all fields of <see cref="Record" /> are reset to <see cref="ReadOnlyMemory{T}.Empty"/>
/// so that the <see cref="CsvRecord" /> object can be filled again.</remarks>
/// 
/// <example>
/// <note type="note">
/// In the following code examples - for easier readability - exception handling has been omitted.
/// </note>
/// <para>
/// Saving a CSV file:
/// </para>
/// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\CsvAnalyzerExample.cs" />
/// </example>
public sealed class CsvWriter : IDisposable
{
    private static readonly SearchValuesPolyfill<char> _reservedCharsDefault
       = SearchValuesPolyfill.Create([',', '\"', '\r', '\n']);

    private bool _isHeaderRowWritten;
    private bool _isDataWritten;
    private bool _disposed;
    private readonly SearchValuesPolyfill<char> _reservedChars;
    private readonly char _delimiter;
    private readonly TextWriter _writer;

    /// <summary>Initializes a new <see cref="CsvWriter" /> object with the column names for the header 
    /// row to be written.</summary>
    /// <param name="filePath">File path of the CSV file.</param>
    /// 
    /// <param name="columnNames">
    /// <para>
    /// A collection of column names for the header to be written.
    /// </para>
    /// <para>
    /// The collection determines the order in which the columns appear in the CSV file and their index in 
    /// <see cref="CsvRecord"/>. The <see cref="CsvRecord"/> instance, which the <see cref="Record"/> 
    /// property of the newly initialized <see cref="CsvWriter"/> gets, can be accessed with this column 
    /// names.
    /// </para>
    /// </param>
    ///<param name="caseSensitive">If <c>true</c>, column names that differ only in upper and lower case are 
    ///also accepted, otherwise <c>false</c>.</param>
    /// <param name="textEncoding">The <see cref="Encoding"/> to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// <param name="delimiter">The field separator character. It's not recommended to change the default
    /// value.</param>
    /// 
    /// <remarks>The constructor creates a new file at the specified <paramref name="filePath"/>. If the file
    /// already exists, it is truncated and overwritten.</remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="filePath" /> is not a valid file path
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// a column name in <paramref name="columnNames" /> occurs twice. With <paramref name="caseSensitive"/> 
    /// can be chosen whether the comparison is case-sensitive or not.
    /// </para>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the double quotes
    /// <c>"</c> or a line break character ('\r' or  '\n').</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public CsvWriter(string filePath,
                     IEnumerable<string?> columnNames,
                     bool caseSensitive = false,
                     Encoding? textEncoding = null,
                     char delimiter = ',')
    {
        _ArgumentNullException.ThrowIfNull(columnNames, nameof(columnNames));
        _ArgumentOutOfRangeException.ValidateDelimiter(delimiter);

        this.Record = new CsvRecord(
            columnNames.ToArray(),
            caseSensitive,
            initArr: true,
            throwException: true);

        // Don't change the order: _writer will not be disposed if an exception is thrown
        // after it had been initialized.
        _writer = StreamHelper.InitStreamWriter(filePath, textEncoding);
        _writer.NewLine = Csv.NewLine;

        _delimiter = delimiter;
        _reservedChars = CreateReservedChars(delimiter);
    }

    /// <summary>Initializes a new <see cref="CsvWriter" /> object to write a CSV file without a header row.
    /// </summary>
    /// <param name="filePath">File path of the CSV file.</param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="textEncoding">The <see cref="Encoding"/> to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// <param name="delimiter">The field separator character. It's not recommended to change the default 
    /// value.</param>
    /// 
    /// <remarks>The constructor creates a new file at the specified <paramref name="filePath"/>. If the file 
    /// already exists, it is truncated and overwritten.</remarks>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid file path.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para>
    /// <paramref name="columnsCount"/> is negative
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// <paramref name="delimiter"/> is either 
    /// the double quotes <c>"</c> or a line break character ('\r' or  '\n').
    /// </para>
    /// </exception>
    /// <exception cref="IOException">I/O error.</exception>
    public CsvWriter(string filePath,
                     int columnsCount,
                     Encoding? textEncoding = null,
                     char delimiter = ',')
    {
        _ArgumentOutOfRangeException.ValidateDelimiter(delimiter);

        Record = new CsvRecord(columnsCount);

        // Don't change the order: _writer will not be disposed if an exception is thrown
        // after it had been initialized.
        this._writer = StreamHelper.InitStreamWriter(filePath, textEncoding);
        _writer.NewLine = Csv.NewLine;

        _isHeaderRowWritten = true;
        _delimiter = delimiter;
        _reservedChars = CreateReservedChars(delimiter);
    }

    /// <summary>Initializes a new <see cref="CsvWriter" /> object with the column names for the header
    /// row to be written.</summary>
    /// <param name="writer">The <see cref="TextWriter" /> used for writing.</param>
    /// 
    /// <param name="columnNames">
    /// <para>
    /// A collection of column names for the header to be written.
    /// </para>
    /// <para>
    /// The collection determines the order in which the columns appear in the CSV and their index in 
    /// <see cref="CsvRecord"/>. The <see cref="CsvRecord"/> instance, which the <see cref="Record"/> 
    /// property of the newly initialized <see cref="CsvWriter"/> gets, can be accessed with this column 
    /// names.
    /// </para>
    /// </param>
    /// <param name="caseSensitive">If <c>true</c>, column names that differ only in upper and lower 
    /// case are also accepted, otherwise <c>false</c>.</param>
    /// <param name="delimiter">The field separator character. It's not recommended to change the default 
    /// value.</param>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="writer" /> or 
    /// <paramref name="columnNames" /> is <c>null.</c></exception>
    /// <exception cref="ArgumentException">A column name in <paramref name="columnNames" /> occurs twice.
    /// With <paramref name="caseSensitive"/> can be chosen whether the comparison is case-sensitive or 
    /// not.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="delimiter"/> is either the double 
    /// quotes <c>"</c> or a line break character ('\r' or  '\n').</exception>
    public CsvWriter(TextWriter writer,
                     IEnumerable<string?> columnNames,
                     bool caseSensitive = false,
                     char delimiter = ',')
    {
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        _ArgumentNullException.ThrowIfNull(columnNames, nameof(columnNames));
        _ArgumentOutOfRangeException.ValidateDelimiter(delimiter);

        this.Record = new CsvRecord(
            columnNames.ToArray(),
            caseSensitive,
            initArr: true,
            throwException: true);

        _writer = writer;
        _writer.NewLine = Csv.NewLine;

        _delimiter = delimiter;
        _reservedChars = CreateReservedChars(delimiter);
    }

    /// <summary>Initializes a new <see cref="CsvWriter" /> object to write CSV data without a 
    /// header row.</summary>
    /// <param name="writer">The <see cref="TextWriter" /> used for writing.</param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="delimiter">The field separator character. It's not recommended to change the 
    /// default value.</param>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="writer" /> is <c>null.</c>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para>
    /// <paramref name="columnsCount"/> is negative
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// <paramref name="delimiter"/> is either the double quotes <c>"</c> or a line break character 
    /// ('\r' or  '\n'). </para>
    /// </exception>
    public CsvWriter(TextWriter writer,
                     int columnsCount,
                     char delimiter = ',')
    {
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        _ArgumentOutOfRangeException.ValidateDelimiter(delimiter);

        Record = new CsvRecord(columnsCount);

        this._writer = writer;
        writer.NewLine = Csv.NewLine;

        _isHeaderRowWritten = true;
        _delimiter = delimiter;
        _reservedChars = CreateReservedChars(delimiter);
    }

    /// <summary>Gets the record to be written to the file. </summary>
    /// <remarks>
    /// <para>
    /// Fill the <see cref="CsvRecord" /> object with data and then call the <see cref="WriteRecord" />
    /// method to write this data to the file. 
    /// </para>
    /// <para>
    /// The <see cref="CsvRecord" /> is the same with each call. <see cref="CsvWriter" /> clears the 
    /// contents of this <see cref="CsvRecord" /> instance after each call of the 
    /// <see cref="WriteRecord" /> method.
    /// </para>
    /// </remarks>
    public CsvRecord Record { get; }

    /// <summary>
    /// Gets the field separator character.
    /// </summary>
    public char Delimiter => _delimiter;

    /// <summary> Writes the contents of <see cref="Record" /> to the CSV file and then sets all
    /// fields of <see cref="Record" /> to <see cref="ReadOnlyMemory{T}.Empty" />. (The first time it 
    /// is called, the header row may also be written.)</summary>
    /// 
    /// <example>
    /// <note type="note">
    /// In the following code examples - for easier readability - exception handling has been omitted.
    /// </note>
    /// 
    /// <code language="cs" source="..\..\..\FolkerKinzel.CsvTools\src\Examples\CsvAnalyzerExample.cs" />
    /// </example>
    /// 
    /// <exception cref="IOException">I/O error.</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public void WriteRecord()
    {
        int recordLength = Record.Count;

        if (recordLength == 0)
        {
            return;
        }

        if (!_isHeaderRowWritten)
        {
            Debug.Assert(Record.ColumnNames is string[]);
            ReadOnlySpan<string> columns = (string[])Record.ColumnNames;

            for (int i = 0; i < recordLength - 1; i++)
            {
                WriteField(columns[i].AsSpan());
                _writer.Write(_delimiter);
            }

            WriteField(columns[recordLength - 1].AsSpan());
            _isHeaderRowWritten = _isDataWritten = true;
        }

        if (_isDataWritten)
        {
            _writer.WriteLine();
        }

        _isDataWritten = true;

        Span<ReadOnlyMemory<char>> values = Record.Values;

        for (int j = 0; j < recordLength - 1; j++)
        {
            ReadOnlyMemory<char> mem = values[j];

            if (!mem.IsEmpty)
            {
                WriteField(mem.Span);
                values[j] = default;
            }

            _writer.Write(_delimiter);
        }

        ReadOnlyMemory<char> lastString = values[recordLength - 1];

        if (!lastString.IsEmpty)
        {
            WriteField(lastString.Span);
            values[recordLength - 1] = default;
        }

        /////////////////////////////////////////////

        void WriteField(ReadOnlySpan<char> field)
        {
            bool needsToBeQuoted = field.ContainsAny(_reservedChars);

            if (needsToBeQuoted)
            {
                _writer.Write('"');

                for (int j = 0; j < field.Length; j++)
                {
                    char c = field[j];

                    if (c == Environment.NewLine[0])
                    {
                        _writer.WriteLine();
                        j += Environment.NewLine.Length - 1;
                    }
                    else if (c == '\"')
                    {
                        _writer.Write('"');
                        _writer.Write(c);
                    }
                    else
                    {
                        _writer.Write(c);
                    }
                }

                _writer.Write('"');
            }
            else
            {
                _writer.Write(field);
            }
        }
    }

    /// <summary>Releases the resources. (Closes the CSV file.)</summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _writer.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    private static SearchValuesPolyfill<char> CreateReservedChars(char delimiter)
       => delimiter == ',' ? _reservedCharsDefault : SearchValuesPolyfill.Create([delimiter, '\"', '\r', '\n']);
}
