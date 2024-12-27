using System.Collections;
using System.Diagnostics;
using System.Text;

namespace FolkerKinzel.CsvTools.Intls;

/// <summary> Liest eine Csv-Datei vorwärts und gibt ihre Datenzeilen als <c>IList&lt;ReadOnlyMemory&lt;char&gt;&gt;</c>"
/// zurück. </summary>
internal sealed class CsvStringReader : IDisposable
{
    private const int INITIAL_COLUMNS_COUNT = 32;
    private const int INITIAL_STRINGBUILDER_CAPACITY = 64;

    private readonly TextReader _reader;
    private readonly char _fieldSeparator;
    private readonly CsvRow _row = new(INITIAL_COLUMNS_COUNT);
    private StringBuilder? _sb;
    private readonly bool _skipEmptyLines;
    private string? _currentLine;
    private bool _mustAllocate;

    internal int LineNumber { get; private set; }

    internal int LineIndex { get; private set; }

    /// <summary> ctor </summary>
    /// <param name="reader">The <see cref="TextReader" /> with which the CSV file is
    /// read.</param>
    /// <param name="fieldSeparator">The field separator char used in the CSV file.</param>
    /// <param name="skipEmptyLines">Wenn <c>true</c>, werden unmaskierte Leerzeilen
    /// in der CSV-Datei übersprungen.</param>
#if NET462 || NETSTANDARD2_0
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
#endif
    internal CsvStringReader(TextReader reader, char fieldSeparator, bool skipEmptyLines)
#if NET462 || NETSTANDARD2_0
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
#endif
    {
        Debug.Assert(reader != null);

        this._skipEmptyLines = skipEmptyLines;
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

            if (_currentLine.Length == 0 && _skipEmptyLines)
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

            _currentLine = null;
        }
    }

    private void AddField()
    {
        Debug.Assert(_currentLine != null);

        ReadOnlySpan<char> span = _currentLine.AsSpan();

        for (int endIndex = LineIndex; endIndex < span.Length; endIndex++)
        {
            char c = span[endIndex];

            if (c == _fieldSeparator)
            {
                _row.Add(_currentLine.AsMemory(LineIndex, endIndex - LineIndex));
                LineIndex = endIndex + 1;
                return;
            }

            if (c == '\"')
            {
                AddAllocatedField();
                return;
            }
        }

        // separator at the end of the line
        _row.Add(_currentLine.AsMemory(LineIndex));
        LineIndex = _currentLine.Length;
    }

    private void AddAllocatedField()
    {
        Debug.Assert(_currentLine != null);
        Debug.Assert(_currentLine.Length > 0);

        int startIndex = LineIndex;
        bool insideQuotes = false;
        bool isMaskedDoubleQuote = false;
        ReadOnlySpan<char> span = _currentLine.AsSpan();
        _mustAllocate = false; // if masked Double Quotes or new lines are inside of a field this must be true

        _sb ??= new StringBuilder(INITIAL_STRINGBUILDER_CAPACITY);
        _ = _sb.Clear();

        while (true)
        {
            if (_currentLine is null) // EOF
            {
                LineIndex = 0;
                DoAddAllocatedField(startIndex);
                return;
            }

            if (insideQuotes && span.Length == 0) // empty line
            {
                _mustAllocate = true;
                _ = _sb.AppendLine();
                _currentLine = _reader.ReadLine();
                span = _currentLine.AsSpan();
                LineNumber++;
                LineIndex = 0;
                continue;
            }

            if (LineIndex >= _currentLine.Length)
            {
                LineIndex = _currentLine.Length;
                DoAddAllocatedField(startIndex);
                return;
            }

            char c = span[LineIndex];

            if (LineIndex == span.Length - 1)
            {
                if (c == '\"') // a field starts with an empty line or a masked field ends
                {
                    insideQuotes = !insideQuotes;
                }

                if (insideQuotes)
                {
                    _mustAllocate = true; // empty line inside of a field

                    _ = c == '\"' ? _sb.AppendLine() : _sb.Append(c).AppendLine();

                    _currentLine = _reader.ReadLine();
                    span = _currentLine.AsSpan();
                    LineIndex = 0;
                    LineNumber++;
                    continue;
                }
                else
                {
                    // If the line ends with an empty field this one is ignored here
                    // but supplemented by GetNextRecord() as default(ReadOnlyMemory<char>).
                    if (c != _fieldSeparator && c != '\"')
                    {
                        _ = _sb.Append(c);
                    }

                    LineIndex = span.Length;
                    DoAddAllocatedField(startIndex);
                    return;
                }
            }
            else // LineIndex < span.Length - 1
            {
                if (insideQuotes)
                {
                    if (c == '\"')
                    {
                        if (isMaskedDoubleQuote)
                        {
                            isMaskedDoubleQuote = false;
                            _ = _sb.Append(c);
                        }
                        else
                        {
                            char next = span[LineIndex + 1];

                            if (next == _fieldSeparator) // Feldende
                            {
                                LineIndex += 2;
                                DoAddAllocatedField(startIndex);
                                return;
                            }
                            else if (next == '\"')
                            {
                                isMaskedDoubleQuote = true;
                                _mustAllocate = true;
                            }
                            else
                            {
                                insideQuotes = false;
                            }
                        }
                    }
                    else
                    {
                        _ = _sb.Append(c);
                    }
                }
                else
                {
                    if (LineIndex == startIndex && c == '\"')
                    {
                        insideQuotes = true;
                    }
                    // The remaining cases can only happen in invalid CSV:
                    else if (c == _fieldSeparator)
                    {
                        LineIndex++;
                        DoAddAllocatedField(startIndex);
                        return;
                    }
                    else
                    {
                        _ = _sb.Append(c);
                    }
                }

                LineIndex++;
            }

        }// while
    }

    private void DoAddAllocatedField(int startIndex)
    {
        Debug.Assert(_sb is not null);

        if(_mustAllocate)
        {
            _row.Add(_sb.ToString().AsMemory());
        }
        else
        {
            _row.Add(_currentLine.AsMemory(startIndex + 1, _sb.Length));
        }
    }
}
