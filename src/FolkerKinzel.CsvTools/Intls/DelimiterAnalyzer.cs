﻿using System.Text;

namespace FolkerKinzel.CsvTools.Intls;

internal ref struct DelimiterAnalyzer()
{
    private const int EOF = -1;
    private const int MAX_LINES = 5;

    private class RowSeparatorFinds
    {
        internal int Comma { get; set; }
        internal int Semicolon { get; set; }
        internal int Hash { get; set; }
        internal int Tab { get; set; }
        internal int Space { get; set; }

        internal bool IsEmpty => Comma == 0 && Semicolon == 0 && Hash == 0 && Tab == 0 && Space == 0;
    }

    private int _last = '\0';
    private int _current;
    private int _rowLength = 0;
    private bool _inQuotes = false;
    private RowSeparatorFinds _finds = new();
    private readonly List<RowSeparatorFinds> _findsList = [];

    public char GetFieldSeparator(TextReader reader)
    {
        while ((_current = reader.Read()) != EOF && _findsList.Count < MAX_LINES) // Skips BOM
        {
            _rowLength++;

            if (_current == '"')
            {
                _inQuotes = !_inQuotes;
                _last = _current;
                continue;
            }

            if (!_inQuotes)
            {
                if (_last == '\r')
                {
                    _rowLength--; // current is \n or the first character of the next line
                    HandleNewLine();

                    if (_current == '\n' || _current == '\r')
                    {
                        // line ending is \r\n,
                        // or line ending is \r and two empty lines follow each other
                        continue;
                    }
                    else
                    {
                        // line ending is \r
                        _rowLength = 1;
                    }
                }
                else if (_current == '\n')
                {
                    // line ending is \n
                    HandleNewLine();
                    continue;
                }

                _last = _current;
                CountCurrent();
            }
        }// while

        if (_last != '\n')
        {
            // Last line does not end with a new line
            HandleNewLine();
        }

        return SelectSeparator(_findsList);
    }

    private void HandleNewLine()
    {
        _last = _current;

        if (_rowLength == 1)
        {
            // Empty line
            return;
        }

        _rowLength = 0;

        if (!_finds.IsEmpty)
        {
            _findsList.Add(_finds);
            _finds = new RowSeparatorFinds();
        }
    }

    private readonly void CountCurrent()
    {
        switch (_current)
        {
            case ',':
                _finds.Comma++;
                break;
            case ';':
                _finds.Semicolon++;
                break;
            case '#':
                _finds.Hash++;
                break;
            case '\t':
                _finds.Tab++;
                break;
            case ' ':
                _finds.Space++;
                break;
        }
    }

    private static char SelectSeparator(List<RowSeparatorFinds> findsList)
    {
        if (findsList.Count == 0)
        {
            return ','; // default
        }

        if (findsList[0].Comma > 0)
        {
            if (!findsList.Skip(1).Any(f => f.Comma != findsList[0].Comma))
            {
                return ',';
            }
        }

        if (findsList[0].Semicolon > 0)
        {
            if (!findsList.Skip(1).Any(f => f.Semicolon != findsList[0].Semicolon))
            {
                return ';';
            }
        }

        if (findsList[0].Hash > 0)
        {
            if (!findsList.Skip(1).Any(f => f.Hash != findsList[0].Hash))
            {
                return '#';
            }
        }

        if (findsList[0].Tab > 0)
        {
            if (!findsList.Skip(1).Any(f => f.Tab != findsList[0].Tab))
            {
                return '\t';
            }
        }

        if (findsList[0].Space > 0)
        {
            if (!findsList.Skip(1).Any(f => f.Space != findsList[0].Space))
            {
                return ' ';
            }
        }

        if (findsList[0].Comma != 0)
        {
            return ',';
        }

        if (findsList[0].Semicolon > 0)
        {
            return ';';
        }

        if (findsList[0].Hash > 0)
        {
            return '#';
        }

        if (findsList[0].Tab > 0)
        {
            return '\t';
        }

        return ' ';
    }
}
