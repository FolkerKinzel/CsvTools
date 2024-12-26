using System.Text;

#if NET462 || NETSTANDARD2_0 || NETSTANDARD2_1
#endif

namespace FolkerKinzel.CsvTools;

public partial class CsvAnalyzer
{
    private ref struct FieldSeparatorAnalyzer(CsvAnalyzer analyzer)
    {
        private const int EOF = -1;
        private const int MAX_LINES = 5;

        private readonly CsvAnalyzer _analyzer = analyzer;
        private int _last = '\0';
        private int _current;
        private int _rowLength = 0;
        private bool _inQuotes = false;
        private RowSeparatorFinds _finds = new();

        private class RowSeparatorFinds
        {
            public int Comma { get; set; }
            public int Semicolon { get; set; }
            public int Hash { get; set; }
            public int Tab { get; set; }
            public int Space { get; set; }
        }

        public void InitFieldSeparator(string fileName)
        {
            List<RowSeparatorFinds> findsList = [];

            try
            {
                using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                while ((_current = fileStream.ReadByte()) != EOF && findsList.Count < MAX_LINES)
                {
                    if(_current == 0) 
                    {
                        // ignore Unicode
                        continue; 
                    }

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
                            HandleNewLine(findsList);

                            if (_current is '\n' or '\r')
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
                            HandleNewLine(findsList);
                            continue;
                        }

                        _last = _current;

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
                }// while
            }
            catch { }

            if (_last != '\n')
            {
                HandleNewLine(findsList);
            }

            if (findsList.Count == 0)
            {
                return; // FieldSeparator already set to ','
            }

            if (findsList[0].Comma > 0)
            {
                if (!findsList.Skip(1).Any(f => f.Comma != findsList[0].Comma))
                {
                    _analyzer.FieldSeparator = ',';
                    return;
                }
            }

            if (findsList[0].Semicolon > 0)
            {
                if (!findsList.Skip(1).Any(f => f.Semicolon != findsList[0].Semicolon))
                {
                    _analyzer.FieldSeparator = ';';
                    return;
                }
            }

            if (findsList[0].Hash > 0)
            {
                if (!findsList.Skip(1).Any(f => f.Hash != findsList[0].Hash))
                {
                    _analyzer.FieldSeparator = '#';
                    return;
                }
            }

            if (findsList[0].Tab > 0)
            {
                if (!findsList.Skip(1).Any(f => f.Tab != findsList[0].Tab))
                {
                    _analyzer.FieldSeparator = '\t';
                    return;
                }
            }

            if (findsList[0].Space > 0)
            {
                if (!findsList.Skip(1).Any(f => f.Space != findsList[0].Space))
                {
                    _analyzer.FieldSeparator = ' ';
                }
            }

            if (findsList[0].Comma == 0)
            {
                if (findsList[0].Semicolon > 0)
                {
                    _analyzer.FieldSeparator = ';';
                    return;
                }

                if (findsList[0].Hash > 0)
                {
                    _analyzer.FieldSeparator = '#';
                    return;
                }

                if (findsList[0].Tab > 0)
                {
                    _analyzer.FieldSeparator = '\t';
                    return;
                }

                if (findsList[0].Space > 0)
                {
                    _analyzer.FieldSeparator = ' ';
                }
            }
        }

        private void HandleNewLine(List<RowSeparatorFinds> findsList)
        {
            _last = _current;

            if (_rowLength == 1)
            {
                // Empty line
                _analyzer.Options = _analyzer.Options.Unset(CsvOptions.ThrowOnEmptyLines);
                return;
            }

            _rowLength = 0;
            findsList.Add(_finds);
            _finds = new RowSeparatorFinds();
        }
    }
}