using System.Text;
using FolkerKinzel.CsvTools.Resources;

namespace FolkerKinzel.CsvTools.Intls;

/// <summary> Liest eine Csv-Datei vorwärts und gibt ihre Datenzeilen als <c>IList&lt;ReadOnlyMemory&lt;char&gt;&gt;</c>"
/// zurück. </summary>
internal sealed class CsvStringReader : IDisposable
{
    private const int INITIAL_COLUMNS_COUNT = 32;
    private const int INITIAL_STRINGBUILDER_CAPACITY = 64;
    private readonly CsvOptions _options;
    private readonly TextReader _reader;
    private readonly char _fieldSeparator;
    private readonly CsvRow _row = new(INITIAL_COLUMNS_COUNT);
    private StringBuilder? _sb;
    private string? _currentLine;
    private bool _mustAllocate;

    internal int LineNumber { get; private set; }

    internal int LineIndex { get; private set; }

    /// <summary> ctor </summary>
    /// <param name="reader">The <see cref="TextReader" /> with which the CSV file is
    /// read.</param>
    /// <param name="options">The parser options.</param>
    /// <param name="skipEmptyLines">Wenn <c>true</c>, werden unmaskierte Leerzeilen
    /// in der CSV-Datei übersprungen.</param>
    internal CsvStringReader(TextReader reader, char fieldSeparator, CsvOptions options)
    {
        Debug.Assert(reader != null);

        this._options = options;
        this._reader = reader;
        this._fieldSeparator = fieldSeparator;
    }

    /// <summary>Releases the resources. (Closes the <see cref="TextReader" />.)</summary>
    public void Dispose() => _reader.Dispose();

    internal CsvRow? Read()
    {
        while ((_currentLine = _reader.ReadLine()) != null)
        {
            LineNumber++;

            if (_currentLine.Length == 0 && !_options.HasFlag(CsvOptions.ThrowOnEmptyLines))
            {
                continue;
            }

            ReadNextRecord();
            return _row;
        }

        return null;
    }

    private void ReadNextRecord()
    {
        _row.Clear();
        LineIndex = 0;

        do
        {
            AddField();
        }
        while (LineIndex < _currentLine?.Length);

        if (_currentLine != null)
        {
            int length = _currentLine.Length;

            if (length != 0 && _currentLine[length - 1] == _fieldSeparator)
            {
                // adds the missing last field if the line ends with the
                // field separator:
                _row.Add(default);
            }
        }
    }

    private void AddField()
    {
        Debug.Assert(_currentLine != null);

        ReadOnlySpan<char> span = _currentLine.AsSpan();

        if (LineIndex < span.Length && span[LineIndex] == '\"')
        {
            AddAllocatedField();
            return;
        }

        for (int idx = LineIndex; idx < span.Length; idx++)
        {
            if (span[idx] == _fieldSeparator)
            {
                _row.Add(_currentLine.AsMemory(LineIndex, idx - LineIndex));
                LineIndex = idx + 1;
                return;
            }
        }

        // field at the end of a line
        _row.Add(_currentLine.AsMemory(LineIndex));
        LineIndex = _currentLine.Length;
    }

    private void AddAllocatedField()
    {
        Debug.Assert(_currentLine != null);
        Debug.Assert(_currentLine.Length > 0);

        int startIndex = LineIndex;
        bool insideQuotes = false;
        ReadOnlySpan<char> span = _currentLine.AsSpan();
        _mustAllocate = false; // if masked Double Quotes or new lines are inside of a field this must be true

        _sb ??= new StringBuilder(INITIAL_STRINGBUILDER_CAPACITY);
        _ = _sb.Clear();

        while (true)
        {
            if (insideQuotes && span.Length == 0) // empty line
            {
                _mustAllocate = true;
                _ = _sb.AppendLine();

                if (!LoadNextLine())
                {
                    return;
                }

                span = _currentLine.AsSpan();
                continue;
            }

            char c = span[LineIndex];

            if (c == '\"')
            {
                insideQuotes = !insideQuotes;
            }

            if (LineIndex == span.Length - 1)
            {
                if (insideQuotes)
                {
                    _mustAllocate = true; // empty line inside of a field

                    _ = _sb.Append(c).AppendLine();

                    if (!LoadNextLine())
                    {
                        return;
                    }

                    span = _currentLine.AsSpan();
                    continue;
                }
                else
                {
                    // If the line ends with an empty field this one is ignored here
                    // but supplemented by GetNextRecord() as default(ReadOnlyMemory<char>).
                    LineIndex = span.Length;
                    DoAddAllocatedField(startIndex);
                    return;
                }
            }
            else // LineIndex < span.Length - 1
            {
                if (insideQuotes)
                {
                    _ = _sb.Append(c);
                }
                else
                {
                    Debug.Assert(c == '\"');

                    char next = span[LineIndex + 1];

                    if (next == _fieldSeparator)
                    {
                        LineIndex += 2;
                        DoAddAllocatedField(startIndex);
                        return;
                    }
                    else if (next == '\"')
                    {
                        // masked double quote
                        _mustAllocate = true;
                    }
                    else
                    {
                        throw new CsvFormatException(Res.InvalidMasking, CsvError.InvalidMasking, LineNumber, LineIndex);
                    }
                }

                LineIndex++;
            }
        }// while
    }

    private bool LoadNextLine()
    {
        _currentLine = _reader.ReadLine();
        LineNumber++;
        LineIndex = 0;

        if (_currentLine is null) // EOF
        {
            if (_options.HasFlag(CsvOptions.ThrowOnTruncatedFiles))
            {
                throw new CsvFormatException(Res.FileTruncated, CsvError.FileTruncated, LineNumber, LineIndex);
            }

            return false;
        }

        return true;
    }

    private void DoAddAllocatedField(int startIndex)
    {
        Debug.Assert(_sb is not null);

        if (_mustAllocate)
        {
            _row.Add(_sb.ToString().AsMemory(1));
        }
        else
        {
            _row.Add(_currentLine.AsMemory(startIndex + 1, _sb.Length));
        }
    }
}
