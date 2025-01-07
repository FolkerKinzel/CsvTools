using System.Text;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary>Writes data as a CSV file (RFC 4180).</summary>
/// <remarks> <see cref="CsvWriter" /> provides a <see cref="CsvRecord" /> object in its 
/// <see cref="Record" /> property that represents a buffer for a data record (row) of the 
/// CSV file. Fill the <see cref="CsvRecord" /> object with data and then write it to the 
/// file using the <see cref="WriteRecord" /> method! After the <see cref="WriteRecord" /> 
/// returns all fields of <see cref="Record" /> are reset to <see cref="ReadOnlyMemory{T}.Empty"/>
/// so that the <see cref="CsvRecord" /> object can be filled again.</remarks>
public sealed class CsvWriter : IDisposable
{
    private bool _isHeaderRowWritten;
    private bool _isDataWritten;
    private readonly SearchValuesPolyfill<char> _reservedChars;

    private readonly char _fieldSeparator;

    private readonly TextWriter _writer;

    /// <summary>Initializes a new <see cref="CsvWriter" /> object with the column names
    /// for the header row to be written.</summary>
    /// <param name="filePath">The file path of the CSV file to be written. If the file
    /// exists, it will be overwritten.</param>
    /// <param name="columnNames">A collection of column names for the header to be written.
    /// The collection will be copied. If the collection contains <c>null</c> values, these 
    /// are replaced by automatically generated column names. Column names cannot appear twice. 
    /// With <paramref name="caseSensitive"/>
    /// can be chosen whether the comparison is case-sensitive or not.</param>
    ///<param name="caseSensitive">If <c>true</c>, column names that differ only in 
    /// upper and lower case are also accepted, otherwise <c>false</c>.</param>
    /// <param name="textEncoding">The text encoding to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// <param name="delimiter">The field separator character.</param>
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
    /// <exception cref="IOException">I/O-Error</exception>
    public CsvWriter(string filePath,
                     IEnumerable<string?> columnNames,
                     bool caseSensitive = false,
                     Encoding? textEncoding = null,
                     char delimiter = ',')
         : this(InitStreamWriter(filePath, textEncoding), columnNames, caseSensitive, delimiter) { }

    /// <summary>Initializes a new <see cref="CsvWriter" /> object to write a CSV file
    /// without a header row.</summary>
    /// <param name="filePath">The file path of the CSV file to be written. If the file
    /// exists, it will be overwritten.</param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="textEncoding">The text encoding to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="filePath" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">I/O-Error</exception>
    public CsvWriter(string filePath,
                     int columnsCount,
                     Encoding? textEncoding = null,
                     char delimiter = ',')
         : this(InitStreamWriter(filePath, textEncoding), columnsCount, delimiter) { }

    /// <summary>Initializes a new <see cref="CsvWriter" /> object with the column names
    /// for the header row to be written.</summary>
    /// <param name="writer">The <see cref="TextWriter" /> used for writing.</param>
    /// <param name="columnNames">A collection of column names for the header to be written.
    /// The collection will be copied. If the collection contains <c>null</c> values, these 
    /// are replaced with automatically
    /// generated column names. Column names cannot appear twice. With <paramref name="caseSensitive"/>
    /// can be chosen whether the comparison is case-sensitive or not.</param>
    /// <param name="caseSensitive">If <c>true</c>, column names that differ only in 
    /// upper and lower case are also accepted, otherwise <c>false</c>.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="writer" /> or <paramref
    /// name="columnNames" /> is <c>null.</c></exception>
    /// <exception cref="ArgumentException">A column name in <paramref name="columnNames"
    /// /> occurs twice. With <paramref name="caseSensitive"/> can be chosen whether 
    /// the comparison is case-sensitive or not.</exception>
    public CsvWriter(TextWriter writer,
                     IEnumerable<string?> columnNames,
                     bool caseSensitive = false,
                     char delimiter = ',')
    {
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        _ArgumentNullException.ThrowIfNull(columnNames, nameof(columnNames));

        _writer = writer;
        _writer.NewLine = Csv.NewLine;

        _fieldSeparator = delimiter;
        _reservedChars = CreateReservedChars(delimiter);

        this.Record = new CsvRecord(
            columnNames.ToArray(),
            caseSensitive,
            initArr: true,
            throwException: true);
    }

    private static SearchValuesPolyfill<char> CreateReservedChars(char delimiter) 
        => SearchValuesPolyfill.Create([delimiter, '\"', '\r', 'n']);

    /// <summary>Initializes a new <see cref="CsvWriter" /> object to write CSV data
    /// without a header row.</summary>
    /// <param name="writer">The <see cref="TextWriter" /> used for writing.</param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// 
    /// <exception cref="ArgumentNullException"> <paramref name="writer" /> is <c>null.</c></exception>
    public CsvWriter(TextWriter writer,
                     int columnsCount,
                     char delimiter = ',')
    {
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        this._writer = writer;
        writer.NewLine = Csv.NewLine;

        _isHeaderRowWritten = true;
        _fieldSeparator = delimiter;
        _reservedChars = CreateReservedChars(delimiter);
        Record = new CsvRecord(columnsCount);
    }

    /// <summary>The record to be written to the file. Fill the <see cref="CsvRecord"
    /// /> object with data and call then <see cref="WriteRecord" /> to write this data
    /// to the file. <see cref="CsvWriter" /> clears the contents of <see cref="Record"
    /// /> after each call of the <see cref="WriteRecord" /> method.</summary>
    public CsvRecord Record { get; }

    /// <summary> Writes the contents of <see cref="Record" /> to the CSV file and then sets all
    /// fields of <see cref="Record" /> to <see cref="ReadOnlyMemory{T}.Empty" />. (The first 
    /// time it is called, the header row may also be written.)</summary>
    /// <exception cref="IOException">I/O-Error</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public void WriteRecord()
    {
        int recordLength = Record.Count;

        if (!_isHeaderRowWritten)
        {
            Debug.Assert(Record.ColumnNames is string[]);
            ReadOnlySpan<string> columns = (string[])Record.ColumnNames;

            for (int i = 0; i < recordLength - 1; i++)
            {
                WriteField(columns[i].AsSpan());
                _writer.Write(_fieldSeparator);
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

            _writer.Write(_fieldSeparator);
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
    public void Dispose() => _writer.Dispose();

    /// <summary> Initialisiert einen <see cref="StreamWriter" /> mit der angegebenen
    /// Textkodierung mit dem Namen der zu schreibenden Datei. </summary>
    /// <param name="fileName">Dateipfad.</param>
    /// <param name="textEncoding">Textkodierung oder <c>null</c> für UTF-8 mit BOM.</param>
    /// <returns> <see cref="StreamWriter" /> </returns>
    /// <exception cref="ArgumentNullException"> <paramref name="fileName" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="fileName" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">I/O-Error</exception>
    [ExcludeFromCodeCoverage]
    private static StreamWriter InitStreamWriter(string fileName, Encoding? textEncoding)
    {
        try
        {
            return new StreamWriter(fileName, false, textEncoding ?? Encoding.UTF8) // UTF-8-Encoding mit BOM
            {
                NewLine = Csv.NewLine
            };
        }
        catch (ArgumentNullException)
        {
            throw new ArgumentNullException(nameof(fileName));
        }
        catch (ArgumentException e)
        {
            throw new ArgumentException(e.Message, nameof(fileName), e);
        }
        catch (UnauthorizedAccessException e)
        {
            throw new IOException(e.Message, e);
        }
        catch (NotSupportedException e)
        {
            throw new ArgumentException(e.Message, nameof(fileName), e);
        }
        catch (System.Security.SecurityException e)
        {
            throw new IOException(e.Message, e);
        }
        catch (PathTooLongException e)
        {
            throw new ArgumentException(e.Message, nameof(fileName), e);
        }
        catch (Exception e)
        {
            throw new IOException(e.Message, e);
        }
    }
}
