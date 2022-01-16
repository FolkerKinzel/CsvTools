using System.Collections;
using System.Diagnostics;
using System.Text;

namespace FolkerKinzel.CsvTools.Intls;

/// <summary>
/// Liest eine Csv-Datei vorwärts und gibt ihre Datenzeilen als <c>IList&lt;ReadOnlyMemory&lt;char&gt;&gt;</c>" zurück.
/// </summary>
internal sealed class CsvStringReader : IDisposable
{
    private readonly TextReader _reader;
    private readonly char _fieldSeparator;

    private const int INITIAL_COLUMNS_COUNT = 32;
    private readonly List<ReadOnlyMemory<char>> _row = new(INITIAL_COLUMNS_COUNT);

    private readonly StringBuilder _sb = new();
    private readonly bool _skipEmptyLines;

    private string? _currentLine;

    internal int LineNumber { get; private set; }

    internal int LineIndex { get; private set; }



    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="reader">Der <see cref="TextReader"/>, mit dem die CSV-Datei gelesen wird.</param>
    /// <param name="fieldSeparator">Das Feldtrennzeichen.</param>
    /// <param name="skipEmptyLines">Wenn <c>true</c>, werden unmaskierte Leerzeilen in der CSV-Datei übersprungen.</param>
#if NET461 || NETSTANDARD2_0
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
#endif
    internal CsvStringReader(TextReader reader, char fieldSeparator, bool skipEmptyLines)
#if NET461 || NETSTANDARD2_0
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
#endif
    {
        Debug.Assert(reader != null);

        this._skipEmptyLines = skipEmptyLines;
        this._reader = reader;
        this._fieldSeparator = fieldSeparator;
    }


    internal IList<ReadOnlyMemory<char>>? Read()
    {
        while ((_currentLine = _reader.ReadLine()) != null)
        {
            LineNumber++;

            if (_currentLine.Length == 0 && _skipEmptyLines)
            {
                continue;
            }

            return GetNextRecord();
        }

        return null;
    }


    /// <summary>
    /// Liest die nächste Datenzeile als <see cref="List{T}">List&lt;ReadOnlyMemory&lt;char&gt;&gt;</see>.
    /// </summary>
    /// <returns>Die nächste Datenzeile als <see cref="List{T}">List&lt;ReadOnlyMemory&lt;char&gt;&gt;</see>.</returns>
    /// <remarks>Die Methode liest sämtliche Felder, die in der Datei enthalten sind und wirft keine <see cref="Exception"/>,
    /// wenn es in einer Zeile zu viele oder zu wenige sind.</remarks>
    private List<ReadOnlyMemory<char>> GetNextRecord()
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

        return _row;

        //////////////////////////////////////////////////

        ReadOnlyMemory<char> GetField()
        {
            int startIndex = LineIndex;
            bool isQuoted = false;
            bool isMaskedDoubleQuote = false;

            _ = _sb.Clear();

            while (true)
            {
                if (_currentLine is null) // Dateiende
                {
                    LineIndex = 0;
                    return InitField();
                }

                if (isQuoted && _currentLine.Length == 0) // Leerzeile
                {
                    _ = _sb.AppendLine();
                    _currentLine = _reader.ReadLine();
                    LineNumber++;
                    LineIndex = 0;
                    continue;
                }

                if (LineIndex >= _currentLine.Length)
                {
                    LineIndex = _currentLine.Length;

                    return InitField();
                }

                char c = _currentLine[LineIndex];

                if (LineIndex == _currentLine.Length - 1)
                {
                    if (c == '\"') // Feld beginnt mit Leerzeile oder maskiertes Feld endet
                    {
                        isQuoted = !isQuoted;
                    }

                    if (isQuoted)
                    {
                        if (c != '\"')
                        {
                            _ = _sb.Append(c).AppendLine();
                        }
                        _currentLine = _reader.ReadLine();
                        LineIndex = 0;
                        LineNumber++;
                        continue;
                    }
                    else
                    {
                        // wenn die Datenzeile mit einem leeren Feld endet,
                        // wird dieses nicht gelesen, aber von GetNextRecord() als default(ReadOnlyMemory<char>) ergänzt
                        if (c != _fieldSeparator)
                        {
                            _ = _sb.Append(c);
                        }


                        LineIndex = _currentLine.Length;
                        return InitField();
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
                                    return InitField();
                                }
                                else if (next == '\"')
                                {
                                    isMaskedDoubleQuote = true;
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
                        }
                        else if (c == _fieldSeparator)
                        {
                            LineIndex++;
                            return InitField();
                        }
                        else
                        {
                            _ = _sb.Append(c);
                        }
                    }

                    LineIndex++;
                }


            }// while

            //////////////////////////////////////////
            
            ReadOnlyMemory<char> InitField() => _sb.ToString().AsMemory();
        }
    }


    /// <summary>
    /// Gibt die Resourcen frei. (Schließt den <see cref="TextReader"/>.)
    /// </summary>
    public void Dispose() => _reader.Dispose();
}
