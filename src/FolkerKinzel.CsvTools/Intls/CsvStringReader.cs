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
    private readonly List<ReadOnlyMemory<char>> _row = new(INITIAL_COLUMNS_COUNT);
    private StringBuilder? _sb;
    private readonly bool _skipEmptyLines;
    private string? _currentLine;

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

    internal List<ReadOnlyMemory<char>>? Read()
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

    /// <summary> Liest die nächste Datenzeile in <see cref="_row"/> ein.
    /// </summary>
    ///
    /// <remarks>Die Methode liest sämtliche Felder, die in der Datei enthalten sind
    /// und wirft keine <see cref="Exception" />, wenn es in einer Zeile zu viele oder
    /// zu wenige sind.</remarks>
    private void ReadNextRecord()
    {
        _row.Clear();
        LineIndex = 0;

        do
        {
            _row.Add(GetField());
        }
        while (LineIndex < _currentLine?.Length);

        if (_currentLine != null)
        {
            int length = _currentLine.Length;
            if (length != 0 && _currentLine[length - 1] == _fieldSeparator)
            {
                // ergänzt das fehlende letzte Feld, wenn die Zeile mit dem
                // Feldtrennzeichen endet:
                _row.Add(default);
            }

            _currentLine = null;
        }
    }

    private ReadOnlyMemory<char> GetField()
    {
        Debug.Assert(_currentLine != null);

        int startIndex = LineIndex;

        for (int endIndex = startIndex; endIndex < _currentLine.Length; endIndex++)
        {
            char c = _currentLine[endIndex];

            if (c == _fieldSeparator)
            {
                LineIndex = endIndex + 1;
                return _currentLine.AsMemory(startIndex, endIndex - startIndex);
            }

            if (c == '\"')
            {
                LineIndex = startIndex;
                return GetAllocatedField();
            }
        }

        LineIndex = _currentLine.Length;
        return _currentLine.AsMemory(startIndex);
    }

    private ReadOnlyMemory<char> GetAllocatedField()
    {
        Debug.Assert(_currentLine != null);
        Debug.Assert(_currentLine.Length > 0);
        int startIndex = LineIndex;
        bool isQuoted = false;
        bool hadBeenQuoted = false;
        bool isMaskedDoubleQuote = false;
        bool mustAllocate = false; // if masked Double Quotes or new lines are inside of a field this must be true

        _sb ??= new StringBuilder(INITIAL_STRINGBUILDER_CAPACITY);
        _ = _sb.Clear();

        while (true)
        {
            if (_currentLine is null) // EOF
            {
                LineIndex = 0;
                return AllocateField();
            }

            if (isQuoted && _currentLine.Length == 0) // empty line
            {
                mustAllocate = true;
                _ = _sb.AppendLine();
                _currentLine = _reader.ReadLine();
                LineNumber++;
                LineIndex = 0;
                continue;
            }

            if (LineIndex >= _currentLine.Length)
            {
                LineIndex = _currentLine.Length;
                return AllocateField();
            }

            char c = _currentLine[LineIndex];

            if (LineIndex == _currentLine.Length - 1)
            {
                if (c == '\"') // a field starts with an empty line or a masked field ends
                {
                    isQuoted = !isQuoted;
                }

                if (isQuoted)
                {
                    mustAllocate = true; // empty line inside of a field

                    _ = c == '\"' ? _sb.AppendLine() : _sb.Append(c).AppendLine();

                    _currentLine = _reader.ReadLine();
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


                    LineIndex = _currentLine.Length;
                    return AllocateField();
                }
            }
            else
            {
                if (isQuoted)
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
                            char next = _currentLine[LineIndex + 1];

                            if (next == _fieldSeparator) // Feldende
                            {
                                LineIndex += 2;
                                return AllocateField();
                            }
                            else if (next == '\"')
                            {
                                isMaskedDoubleQuote = true;
                                mustAllocate = true;
                            }
                            else
                            {
                                isQuoted = false;
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
                        isQuoted = true;
                        hadBeenQuoted = true;
                    }
                    // The remaining cases can only happen in invalid CSV:
                    else if (c == _fieldSeparator)
                    {
                        LineIndex++;
                        return AllocateField();
                    }
                    else
                    {
                        _ = _sb.Append(c);
                    }
                }

                LineIndex++;
            }

        }// while

        ReadOnlyMemory<char> AllocateField()
        {
            Debug.Assert(_sb is not null);
            return mustAllocate ? _sb.ToString().AsMemory()
                                : _currentLine.AsMemory(hadBeenQuoted ? startIndex + 1 : startIndex, _sb.Length);
        }
    }


}
