using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using FolkerKinzel.CsvTools.Intls;

#if NETSTANDARD2_1 || NETSTANDARD2_0 || NET462
using FolkerKinzel.Strings;
#endif

namespace FolkerKinzel.CsvTools;

    /// <summary>Writes data to a CSV file.</summary>
    /// <remarks> <see cref="CsvWriter" /> stellt in der Eigenschaft <see cref="Record"
    /// /> ein <see cref="CsvRecord" />-Objekt zur Verfügung, das einen Puffer für einen
    /// Datensatz (Zeile) der CSV-Datei repräsentiert. Füllen Sie das <see cref="CsvRecord"
    /// />-Objekt mit <see cref="string" />-Daten und schreiben Sie es anschließend
    /// mit der Methode <see cref="WriteRecord" /> in die Datei. Der Aufruf von <see
    /// cref="WriteRecord" /> setzt alle Felder von <see cref="Record" /> wieder auf
    /// <c>null</c>-Werte, so dass das <see cref="CsvRecord" />-Objekt erneut befüllt
    /// werden kann. Wenn andere Datentypen als <see cref="string" /> geschrieben werden
    /// sollen, bietet sich die Verwendung der Klasse <see cref="CsvRecordWrapper" />
    /// an, die einen komfortablen Adapter zwischen den Daten der Anwendung und der
    /// CSV-Datei darstellt.</remarks>
    /// <example>
    /// <note type="note">
    /// In the following code examples - for easier readability - exception handling
    /// has been omitted.
    /// </note>
    /// <para>
    /// Saving the contents of a <see cref="DataTable" /> as a CSV file and importing
    /// data from a CSV file into a <see cref="DataTable" />:
    /// </para>
    /// <code language="cs" source="..\Examples\CsvToDataTable.cs" />
    /// </example>
public sealed class CsvWriter : IDisposable
{
    /// <summary>The newline character to use, when writing CSV files ("\r\n").</summary>
    public const string NewLine = "\r\n";

    private bool _isHeaderRowWritten;
    private bool _isDataWritten;

    private readonly char _fieldSeparator;
    private readonly bool _trimColumns;

    private readonly TextWriter _writer;

    /// <summary>Initializes a new <see cref="CsvWriter" /> object with the column names
    /// for the header row to be written.</summary>
    /// <param name="fileName">The file path of the CSV file to be written. If the file
    /// exists, it will be overwritten.</param>
    /// <param name="columnNames">An array of column names for the header to be written.
    /// If the array contains <c>null</c> values, these are replaced by automatically
    /// generated column names. Column names cannot appear twice. It is to note that
    /// the comparison is not case-sensitive - unless this option is explicitely chosen
    /// in <paramref name="options" />.</param>
    /// <param name="options">Options for the CSV file to be written.</param>
    /// <param name="textEncoding">The text encoding to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// <param name="fieldSeparator">The field separator char to use in the CSV file.</param>
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
    public CsvWriter(
        string fileName, string?[] columnNames, CsvOptions options = CsvOptions.Default, Encoding? textEncoding = null, char fieldSeparator = ',')
         : this(columnNames, fieldSeparator, options) => _writer = InitStreamWriter(fileName, textEncoding);

    /// <summary>Initializes a new <see cref="CsvWriter" /> object to write a CSV file
    /// without a header row.</summary>
    /// <param name="fileName">The file path of the CSV file to be written. If the file
    /// exists, it will be overwritten.</param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="options">Options for the CSV file to be written.</param>
    /// <param name="textEncoding">The text encoding to be used or <c>null</c> for <see
    /// cref="Encoding.UTF8" />.</param>
    /// <param name="fieldSeparator">The field separator char to use in the CSV file.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="fileName" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> <paramref name="fileName" /> is not a valid
    /// file path.</exception>
    /// <exception cref="IOException">I/O-Error</exception>
    public CsvWriter(
        string fileName, int columnsCount, CsvOptions options = CsvOptions.Default, Encoding? textEncoding = null, char fieldSeparator = ',')
         : this(columnsCount, fieldSeparator, options) => _writer = InitStreamWriter(fileName, textEncoding);

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
    /// <param name="fieldSeparator">The field separator char to use in the CSV file.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="writer" /> or <paramref
    /// name="columnNames" /> is <c>null.</c></exception>
    /// <exception cref="ArgumentException">A column name in <paramref name="columnNames"
    /// /> occurs twice. In <paramref name="options" /> can be chosen, whether the comparison
    /// is case-sensitive.</exception>
    public CsvWriter(
        TextWriter writer, IEnumerable<string?> columnNames, CsvOptions options = CsvOptions.Default, char fieldSeparator = ',')
        : this(columnNames, fieldSeparator, options)
    {
        _ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        _ArgumentNullException.ThrowIfNull(columnNames, nameof(columnNames));

        this._writer = writer;
        writer.NewLine = NewLine;
    }

    /// <summary>Initializes a new <see cref="CsvWriter" /> object to write a CSV file
    /// without a header row.</summary>
    /// <param name="writer">The <see cref="TextWriter" /> used for writing.</param>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="options">Options for the CSV file to be written.</param>
    /// <param name="fieldSeparator">The field separator char to use in the CSV file.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="writer" /> is <c>null.</c></exception>
    public CsvWriter(
        TextWriter writer, int columnsCount, CsvOptions options = CsvOptions.Default, char fieldSeparator = ',')
        : this(columnsCount, fieldSeparator, options)
    {
        if (writer is null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        this._writer = writer;
        writer.NewLine = NewLine;
    }

    /// <summary> Privater Konstruktor, der von den öffentlichen Konstruktoren aufgerufen
    /// wird, die ein <see cref="CsvWriter" />-Objekt mit Spaltennamen für die zu schreibende
    /// Kopfzeile initialisieren. (Diese Konstruktoren initialisieren auch <see cref="_writer"
    /// />.) </summary>
    /// <param name="columnNames">Eine Sammlung von Spaltennamen für die zu schreibende
    /// Kopfzeile. Die Sammlung wird kopiert. Wenn die Sammlung <c>null</c>-Werte enthält, werden diese durch automatisch
    /// erzeugte Spaltennamen ersetzt. Spaltennamen dürfen nicht doppelt vorkommen.
    /// Dabei ist zu beachten, dass der Vergleich nicht case-sensitiv erfolgt - es sei
    /// denn, dass diese Option in <paramref name="options" /> ausdrücklich gewählt
    /// wurde.</param>
    /// <param name="fieldSeparator">The field separator char to use in the CSV file.</param>
    /// <param name="options">Options for the CSV file to be written.</param>
    /// <exception cref="ArgumentException">A column name in <paramref name="columnNames"
    /// /> occurs twice. In <paramref name="options" /> can be chosen, whether the comparison
    /// is case-sensitive.</exception>
#pragma warning disable CS8618 // Das Non-Nullable-Feld _writer ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".
    private CsvWriter(IEnumerable<string?> columnNames, char fieldSeparator, CsvOptions options)
#pragma warning restore CS8618 // Das Non-Nullable-Feld ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".
    {
        this._fieldSeparator = fieldSeparator;
        this._trimColumns = options.HasFlag(CsvOptions.TrimColumns);

        this.Record = new CsvRecord(
            columnNames.ToArray(),
            options.HasFlag(CsvOptions.CaseSensitiveKeys),
            _trimColumns,
            initArr: true,
            throwException: true);
    }

    /// <summary> Privater Konstruktor, der von den öffentlichen Konstruktoren aufgerufen
    /// wird, die ein <see cref="CsvWriter" />-Objekt initialisieren, mit dem eine CSV-Datei
    /// ohne Kopfzeile schreiben. (Diese Konstruktoren initialisieren auch <see cref="_writer"
    /// />.) </summary>
    /// <param name="columnsCount">Number of columns in the CSV file.</param>
    /// <param name="fieldSeparator">The field separator char to use in the CSV file.</param>
    /// <param name="options">Options for the CSV file to be written.</param>
#pragma warning disable CS8618 // Das Non-Nullable-Feld _writer ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".
    private CsvWriter(int columnsCount, char fieldSeparator, CsvOptions options)
#pragma warning restore CS8618 // Das Non-Nullable-Feld ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".
    {
        this._isHeaderRowWritten = true;
        this._fieldSeparator = fieldSeparator;
        this._trimColumns = options.HasFlag(CsvOptions.TrimColumns);
        this.Record = new CsvRecord(columnsCount);
    }

    /// <summary>The record to be written to the file. Fill the <see cref="CsvRecord"
    /// /> object with data and call then <see cref="WriteRecord" /> to write this data
    /// to the file. <see cref="CsvWriter" /> clears the contents of <see cref="Record"
    /// /> after each call of the <see cref="WriteRecord" /> method.</summary>
    public CsvRecord Record { get; }

    /// <summary> Schreibt den Inhalt von <see cref="Record" /> in die CSV-Datei und
    /// setzt anschließend alle Spalten von <see cref="Record" /> auf <see cref="ReadOnlySpan{T}.Empty"
    /// />. (Beim ersten Aufruf wird ggf. auch die Kopfzeile geschrieben.) </summary>
    /// <exception cref="IOException">I/O-Error</exception>
    /// <exception cref="ObjectDisposedException">The resources were already released.</exception>
    public void WriteRecord()
    {
        int recordLength = Record.Count;

        if (!_isHeaderRowWritten)
        {
            ReadOnlyCollection<string>? columns = Record.ColumnNames;

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
        if (!Record[recordLength - 1].IsEmpty)
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


        bool NeedsToBeQuoted(ReadOnlySpan<char> s) => s.Contains(_fieldSeparator) ||
                                                  s.Contains('"') ||
                                                  s.Contains(Environment.NewLine, StringComparison.Ordinal);
    }

    /// <summary>Releases the resources. (Closes the <see cref="TextWriter" />.)</summary>
    public void Dispose() => _writer.Dispose();

    #region private

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
                NewLine = CsvWriter.NewLine
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

    #endregion
}
