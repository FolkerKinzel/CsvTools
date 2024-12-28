using System.Text;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary>Writes data as a CSV file.</summary>
/// <remarks> <see cref="CsvWriter" /> provides a <see cref="CsvRecord" /> object in its 
/// <see cref="Record" /> property that represents a buffer for a data record (row) of the 
/// CSV file. Fill the <see cref="CsvRecord" /> object with data and then write it to the 
/// file using the <see cref="Write" /> method! After the <see cref="Write" /> 
/// returns all fields of <see cref="Record" /> are reset to <see cref="ReadOnlyMemory{T}.Empty"/>
/// so that the <see cref="CsvRecord" /> object can be filled again.</remarks>
public sealed class CsvWriter : IDisposable
{
    private bool _isHeaderRowWritten;
    private bool _isDataWritten;

    private readonly char _fieldSeparator;
    private readonly bool _trimColumns;

    private readonly TextWriter _writer;

    /// <summary>Initializes a new <see cref="CsvWriter" /> object with the column names
    /// for the header row to be written.</summary>
    /// <param name="fileName">The file path of the CSV file to be written. If the file
    /// exists, it will be overwritten.</param>
    /// <param name="columnNames">A collection of column names for the header to be written.
    /// The collection will be copied. If the collection contains <c>null</c> values, these 
    /// are replaced by automatically generated column names. Column names cannot appear twice. 
    /// It is to note that the comparison is not case-sensitive - unless this option is explicitely
    /// chosen in <paramref name="options" />.</param>
    /// <param name="options">Options for the CSV file to be written.</param>
    /// <param name="textEncoding">The text encoding to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// <param name="delimiter">The field separator char to use in the CSV file.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileName" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="fileName" /> is not a valid file path
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// a column name in <paramref name="columnNames" /> occurs twice. In <paramref
    /// name="options" /> you can choose, whether the comparison of column names is
    /// case-sensitive.
    /// </para>
    /// </exception>
    /// <exception cref="IOException">I/O-Error</exception>
    public CsvWriter(string fileName,
                     IEnumerable<string?> columnNames,
                     CsvOpts options = CsvOpts.Default,
                     Encoding? textEncoding = null,
                     char delimiter = ',')
         : this(InitStreamWriter(fileName, textEncoding), columnNames, options, delimiter) { }

    /// <summary>Initializes a new <see cref="CsvWriter" /> object to write a CSV file
    /// without a header row.</summary>
    /// <param name="fileName">The file path of the CSV file to be written. If the file
    /// exists, it will be overwritten.</param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="options">Options for the CSV file to be written.</param>
    /// <param name="textEncoding">The text encoding to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// <param name="delimiter">The field separator char to use in the CSV file.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileName" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="fileName" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">I/O-Error</exception>
    public CsvWriter(string fileName,
                     int columnsCount,
                     CsvOpts options = CsvOpts.Default,
                     Encoding? textEncoding = null,
                     char delimiter = ',')
         : this(InitStreamWriter(fileName, textEncoding), columnsCount, options, delimiter) { }

    /// <summary>Initializes a new <see cref="CsvWriter" /> object with the column names
    /// for the header row to be written.</summary>
    /// <param name="writer">The <see cref="TextWriter" /> used for writing.</param>
    /// <param name="columnNames">A colletion of column names for the header to be written.
    /// The collection will be copied. If the collection contains <c>null</c> values, these 
    /// are replaced with automatically
    /// generated column names. Column names cannot appear twice. It is to note that
    /// the comparison is not case-sensitive - unless this option is explicitely chosen
    /// in <paramref name="options" />.</param>
    /// <param name="options">Options for the CSV file to be written.</param>
    /// <param name="delimiter">The field separator char to use in the CSV file.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="writer" /> or <paramref
    /// name="columnNames" /> is <c>null.</c></exception>
    /// <exception cref="ArgumentException">A column name in <paramref name="columnNames"
    /// /> occurs twice. In <paramref name="options" /> can be chosen whether the comparison
    /// is case-sensitive.</exception>
    public CsvWriter(TextWriter writer,
                     IEnumerable<string?> columnNames,
                     CsvOpts options = CsvOpts.Default,
                     char delimiter = ',')
    {
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        _ArgumentNullException.ThrowIfNull(columnNames, nameof(columnNames));

        this._writer = writer;
        writer.NewLine = Csv.NewLine;

        this._fieldSeparator = delimiter;
        this._trimColumns = options.HasFlag(CsvOpts.TrimColumns);

        this.Record = new CsvRecord(
            columnNames.ToArray(),
            options.HasFlag(CsvOpts.CaseSensitiveKeys),
            initArr: true,
            throwException: true);
    }

    /// <summary>Initializes a new <see cref="CsvWriter" /> object to write a CSV file
    /// without a header row.</summary>
    /// <param name="writer">The <see cref="TextWriter" /> used for writing.</param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="options">Options for the CSV file to be written.</param>
    /// <param name="delimiter">The field separator char to use in the CSV file.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="writer" /> is <c>null.</c></exception>
    public CsvWriter(TextWriter writer,
                     int columnsCount,
                     CsvOpts options = CsvOpts.Default,
                     char delimiter = ',')
    {
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));

        this._writer = writer;
        writer.NewLine = Csv.NewLine;

        this._isHeaderRowWritten = true;
        this._fieldSeparator = delimiter;
        this._trimColumns = options.HasFlag(CsvOpts.TrimColumns);
        this.Record = new CsvRecord(columnsCount);
    }

    /// <summary>The record to be written to the file. Fill the <see cref="CsvRecord"
    /// /> object with data and call then <see cref="Write" /> to write this data
    /// to the file. <see cref="CsvWriter" /> clears the contents of <see cref="Record"
    /// /> after each call of the <see cref="Write" /> method.</summary>
    public CsvRecord Record { get; }

    /// <summary> Writes the contents of <see cref="Record" /> to the CSV file and then sets all
    /// fields of <see cref="Record" /> to <see cref="ReadOnlyMemory{T}.Empty" />. (The first 
    /// time it is called, the header row may also be written.)</summary>
    /// <exception cref="IOException">I/O-Error</exception>
    /// <exception cref="ObjectDisposedException">The file was already closed.</exception>
    public void Write()
    {
        int recordLength = Record.Count;

        if (!_isHeaderRowWritten)
        {
            IReadOnlyList<string>? columns = Record.ColumnNames;

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

        for (int j = 0; j < recordLength - 1; j++)
        {
            ReadOnlyMemory<char> mem = Record[j];

            if (!mem.IsEmpty)
            {
                WriteField(mem.Span);
                Record[j] = default;
            }

            _writer.Write(_fieldSeparator);
        }

        ReadOnlyMemory<char> lastString = Record[recordLength - 1];

        if (!lastString.IsEmpty)
        {
            WriteField(lastString.Span);
            Record[recordLength - 1] = default;
        }

        /////////////////////////////////////////////

        void WriteField(ReadOnlySpan<char> field)
        {
            if (_trimColumns)
            {
                field = field.Trim();
            }

            bool needsToBeQuoted = NeedsToBeQuoted(field);

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

        bool NeedsToBeQuoted(ReadOnlySpan<char> s)
            => s.Contains(_fieldSeparator) ||
               s.Contains('"') ||
               s.Contains(Environment.NewLine, StringComparison.Ordinal);
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
