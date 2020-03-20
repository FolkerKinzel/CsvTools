using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace FolkerKinzel.CsvTools
{
    /// <summary>
    /// Statische Klasse zum Lesen von CSV-Dateien.
    /// </summary>
    public static class CsvReader
    {
        
        /// <summary>
        /// Lädt die Daten einer CSV-Datei vom Datenträger. 
        /// </summary>
        /// <param name="fileName">Dateipfad der CSV-Datei.</param>
        /// <param name="fieldSeparator">Das Feldtrennzeichen, das in der CSV-Datei Verwendung findet.</param>
        /// <param name="hasHeaderRow">True gibt an, dass die CSV-Datei eine Kopfzeile hat.</param>
        /// <param name="enc">Die zum Einlesen der CSV-Datei zu verwendende Textenkodierung oder null, um diese automatisch
        /// bestimmen zu lassen.</param>
        /// <returns>Eine <see cref="DataTable"/> mit den Daten der CSV-Datei. Alle Spalten sind vom
        /// Datentyp <see cref="string"/> und können <see cref="DBNull.Value"/> enthalten.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> ist null.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> ist kein gültiger Dateipfad.</exception>
        /// <exception cref="IOException">Es kann nicht auf den Datenträger zugegriffen werden.</exception>
        public static DataTable Load(
            string fileName, char fieldSeparator = ';', bool hasHeaderRow = true, Encoding? enc = null)
        {
            try
            {
                using var reader = new StreamReader(fileName, enc ?? Encoding.UTF8, true);
                return CsvReader.Read(reader, fieldSeparator, hasHeaderRow);
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


        /// <summary>
        /// Liest die CSV-Daten aus einem <see cref="TextReader"/>.
        /// </summary>
        /// <param name="reader">Der <see cref="TextReader"/>, mit dem die CSV-Daten gelesen werden.</param>
        /// <param name="fieldSeparator">Das Feldtrennzeichen, das in der CSV-Datei Verwendung findet.</param>
        /// <param name="hasHeaderRow">True gibt an, dass die CSV-Datei eine Kopfzeile hat.</param>
        /// <returns>Eine <see cref="DataTable"/> mit den Daten der CSV-Datei. Alle Spalten sind vom
        /// Datentyp <see cref="string"/> und können <see cref="DBNull.Value"/> enthalten.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> ist null.</exception>
        /// <exception cref="OutOfMemoryException">Der verfügbare Arbeitsspeicher reicht nicht aus.</exception>
        /// <exception cref="IOException">Fehler beim Zugriff auf den Datenträger.</exception>
        /// <exception cref="ObjectDisposedException">Die Ressourcen des Objekts sind bereits freigegeben.</exception>
        public static DataTable Read(TextReader reader, char fieldSeparator = ';', bool hasHeaderRow = true)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            var sb = new StringBuilder();

            const int RECORD_END = -1;
            
            string? currentLine;
            string? s;
            
            DataTable dt = InitDataTable();

            while ((currentLine = reader.ReadLine()) != null)
            {
                AddRecord();
            }

            return dt;

            ///////////////////////////////////////////////////

            DataTable InitDataTable()
            {
                if (hasHeaderRow)
                {
                    var colNames = new List<string?>();

                    currentLine = reader.ReadLine();

                    int stringIndex = 0;

                    do
                    {
                        stringIndex = GetField(stringIndex, ref currentLine, out s);
                        colNames.Add(s);
                    }
                    while (stringIndex != RECORD_END);

                    return CsvHelper.CreateDataTable(colNames);
                }
                else
                {
                    return new DataTable();
                }
            }


            void AddRecord()
            {
                DataRow row = dt.NewRow();
                dt.Rows.Add(row);

                int stringIndex = 0;
                int dataColumnIndex = 0;
                
                do
                {
                    stringIndex = GetField(stringIndex, ref currentLine, out s);

                    if (dataColumnIndex == dt.Columns.Count)
                    {
                        dt.Columns.Add();
                    }

                    row[dataColumnIndex++] = s;
                }
                while (stringIndex != RECORD_END);
            }


            int GetField(int startIndex, ref string? currentLine, out string? field)
            {
                int index = startIndex;
                sb.Clear();
                bool isQuoted = false;
                bool isMaskedDoubleQuote = false;

                while (true)
                {
                    if (currentLine is null) // Dateiende
                    {
                        field = InitField();
                        return RECORD_END;
                    }

                    if (isQuoted && currentLine.Length == 0) // Leerzeile
                    {
                        sb.AppendLine();
                        currentLine = reader.ReadLine();
                        index = 0;
                        continue;
                    }

                    if (index >= currentLine.Length)
                    {
                        field = InitField();
                        return RECORD_END;
                    }

                    char c = currentLine[index];

                    if (index == currentLine.Length - 1)
                    {
                        if (c == '\"') // Feld beginnt mit Leerzeile oder maskiertes Feld endet
                        {
                            isQuoted = !isQuoted;
                        }

                        if (isQuoted)
                        {
                            if (c != '\"')
                            {
                                sb.Append(c).AppendLine();
                            }
                            currentLine = reader.ReadLine();
                            index = 0;
                            continue;
                        }
                        else
                        {
                            // wenn die Datenzeile mit einem leeren Feld endet,
                            // wird dieses nicht gelesen
                            if (c != fieldSeparator)
                            {
                                sb.Append(c);
                            }

                            field = InitField();
                            return RECORD_END;
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
                                    sb.Append(c);
                                }
                                else
                                {
                                    char next = currentLine[index + 1];

                                    if (next == fieldSeparator) // Feldende
                                    {
                                        field = InitField();
                                        return index + 2;
                                    }
                                    else if (next == '\"')
                                    {
                                        isMaskedDoubleQuote = true;
                                    }
                                }
                            }
                            else
                            {
                                sb.Append(c);
                            }
                        }
                        else
                        {
                            if (index == startIndex && c == '\"')
                            {
                                isQuoted = true;
                            }
                            else if (c == fieldSeparator)
                            {
                                field = InitField();
                                return index + 1;
                            }
                            else
                            {
                                sb.Append(c);
                            }
                        }

                        index++;
                    }


                }// while


                string? InitField()
                {
                    string field = sb.ToString();

                    return (field.Length == 0) ?  null : field;
                }

            }

        }

    }
}
