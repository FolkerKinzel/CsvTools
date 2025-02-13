using System.Text;
using FolkerKinzel.CsvTools.Resources;

namespace FolkerKinzel.CsvTools.Intls;

/// <summary> Read a CSV file once forward and returns its contents as 
/// <see cref="CsvRow "/> objects. </summary>
internal sealed class CsvStringReader : IDisposable
{
    private const int INITIAL_COLUMNS_COUNT = 32;
    private const int INITIAL_STRINGBUILDER_CAPACITY = 64;
    private readonly TextReader _reader;
    private readonly CsvRow _row = new(INITIAL_COLUMNS_COUNT);
    private StringBuilder? _sb;
    private string? _currentLine;
    private bool _mustAllocate;
    private bool _firstLineFound;

    internal int LineNumber { get; private set; }

    internal int LineIndex { get; private set; }

    /// <summary> ctor </summary>
    /// <param name="reader">The <see cref="TextReader" /> with which the 
    /// CSV file is read.</param>
    /// <param name="delimiter">The field separator character.</param>
    /// <param name="options">The parser options.</param>
    internal CsvStringReader(TextReader reader, char delimiter, CsvOpts options)
    {
        Debug.Assert(reader != null);

        this.Options = options;
        this._reader = reader;
        this.Delimiter = delimiter;
    }

    internal CsvOpts Options { get; }

    internal char Delimiter { get; }

    /// <summary>Releases the resources. (Closes the <see cref="TextReader" />.)</summary>
    public void Dispose() => _reader.Dispose();

    internal CsvRow? Read()
    {
        while ((_currentLine = _reader.ReadLine()) != null)
        {
            LineNumber++;

            if (!_firstLineFound
                && _currentLine.Length == 0
                && !Options.HasFlag(CsvOpts.ThrowOnEmptyLines))
            {
                continue;
            }

            _firstLineFound = true;
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

            if (length != 0 && _currentLine[length - 1] == Delimiter)
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

        if (IsMaskedField(span))
        {
            AddAllocatedField();
            return;
        }

        ReadOnlyMemory<char> field;

        for (int idx = LineIndex; idx < span.Length; idx++)
        {
            if (span[idx] == Delimiter)
            {
                field = _currentLine.AsMemory(LineIndex, idx - LineIndex);
                _row.Add(Options.HasFlag(CsvOpts.TrimColumns) ? field.Trim() : field);
                LineIndex = idx + 1;
                return;
            }
        }

        // field at the end of a line
        field = _currentLine.AsMemory(LineIndex);
        _row.Add(Options.HasFlag(CsvOpts.TrimColumns) ? field.Trim() : field);
        LineIndex = _currentLine.Length;
    }

    private bool IsMaskedField(ReadOnlySpan<char> span)
    {
        if (LineIndex >= span.Length)
        {
            return false;
        }

        for (int i = LineIndex; i < span.Length; i++)
        {
            char c = span[i];

            if (c == ' ')
            {
                continue;
            }

            if (c == '\"')
            {
                LineIndex = i;
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
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
                    DoAddAllocatedField(startIndex);
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
                        DoAddAllocatedField(startIndex);
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
                    Debug.Assert(c == '\"' || c == ' ');

                    char next = span[LineIndex + 1];

                    if (next == Delimiter)
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
                    else if (next == ' ')
                    {
                        // tolerate spaces after the closing quote if the
                        // field separator is not a space
                    }
                    else
                    {
                        throw new CsvFormatException(Res.InvalidMasking,
                                                     CsvError.InvalidMasking,
                                                     LineNumber,
                                                     LineIndex);
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
            if (Options.HasFlag(CsvOpts.ThrowOnTruncatedFiles))
            {
                throw new CsvFormatException(Res.FileTruncated,
                                             CsvError.FileTruncated,
                                             LineNumber,
                                             LineIndex);
            }

            return false;
        }

        return true;
    }

    private void DoAddAllocatedField(int startIndex)
    {
        Debug.Assert(_sb is not null);
        Debug.Assert(_sb.Length >= 1);

        if (_mustAllocate)
        {
            _row.Add(_sb.ToString().AsMemory(1));
        }
        else
        {
            _row.Add(_currentLine.AsMemory(startIndex + 1, _sb.Length - 1));
        }
    }
}
