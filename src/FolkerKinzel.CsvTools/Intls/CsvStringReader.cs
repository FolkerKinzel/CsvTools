using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FolkerKinzel.CsvTools.Intls
{
    /// <summary>
    /// Liest eine Csv-Datei vorwärts und ermöglicht es, über die enthaltenen <see cref="string"/>s zu iterieren.
    /// Da <see cref="CsvStringReader"/>&#160;<see cref="IEnumerable"/> direkt implementiert, kann über das
    /// <see cref="CsvStringReader"/>-Objekt mit einer doppelten foreach-Schleife iteriert werden. (Eine foreach-Schleife 
    /// für die Datensätze (Zeilen) und eine innere foreach-Schleife für die Felder der einzelnen Datensätze.)
    /// </summary>
    internal sealed class CsvStringReader : IEnumerable<IEnumerable<string?>>, IDisposable
    {
        private readonly TextReader _reader;
        private readonly char _fieldSeparator;
       
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
        internal CsvStringReader(TextReader reader, char fieldSeparator, bool skipEmptyLines)
        {
            Debug.Assert(reader != null);

            this._skipEmptyLines = skipEmptyLines;
            this._reader = reader;
            this._fieldSeparator = fieldSeparator;
        }


        /// <summary>
        /// Gibt einen Iterator zurück, mit dem über die Zeilen der csv-Datei iteriert werden kann.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IEnumerable<string?>> GetEnumerator()
        {
            while ((_currentLine = _reader.ReadLine()) != null)
            {
                if(_currentLine.Length == 0 && _skipEmptyLines)
                {
                    continue;
                }

                yield return GetNextRecord();
            }
        }


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <summary>
        /// Liest die nächste Datenzeile als <see cref="IEnumerable{T}">IEnumerable&lt;string?&gt;</see> und ermöglicht es damit,
        /// über die Felder der Datenzeile zu iterieren.
        /// </summary>
        /// <returns>Die nächste Datenzeile als <see cref="IEnumerable{T}">IEnumerable&lt;string?&gt;</see>.</returns>
        /// <remarks>Die Methode liest sämtliche Felder, die in der Datei enthalten sind und wirft keine <see cref="Exception"/>,
        /// wenn es in einer Zeile zu viele oder zu wenige sind.</remarks>
        private IEnumerable<string?> GetNextRecord()
        {
            LineNumber++;
            LineIndex = 0;
            
            do
            {
                yield return GetField();
            }
            while (LineIndex < _currentLine?.Length);


            if (_currentLine != null)
            {
                int length = _currentLine.Length;
                if (length != 0 && _currentLine[length - 1] == _fieldSeparator)
                {
                    // ergänzt das fehlende letzte Feld, wenn die Zeile mit dem
                    // Feldtrennzeichen endet:
                    yield return null;
                }

                _currentLine = null;
            }

            //////////////////////////////////////////////////

            string? GetField()
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
                            // wird dieses nicht gelesen, aber von GetNextRecord() als null-Wert ergänzt
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


                string? InitField()
                {
                    string field = _sb.ToString();
                    return (field.Length == 0) ? null : field;
                }
            }
        }


        /// <summary>
        /// Gibt die Resourcen frei. (Schließt den <see cref="TextReader"/>.)
        /// </summary>
        public void Dispose() => _reader.Dispose();
    }
}
