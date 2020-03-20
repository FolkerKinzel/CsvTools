using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;
using FolkerKinzel.CsvTools.Properties;

namespace FolkerKinzel.CsvTools
{
    /// <summary>
    /// Statische Klasse zum Schreiben von CSV-Dateien.
    /// </summary>
    public static class CsvWriter
    {
        /// <summary>
        /// Schreibt den Inhalt einer <see cref="DataTable"/>, deren Spalten vom Typ <see cref="string"/> sind,
        /// in eine CSV-Datei.
        /// </summary>
        /// <param name="fileName">Dateipfad der zu schreibenden Datei. Wenn die Datei existiert, wird sie überschrieben.</param>
        /// <param name="data">Eine <see cref="DataTable"/>. Alle Spalten müssen vom Datentyp <see cref="string"/> sein, dürfen aber
        /// <see cref="DBNull"/> zulassen.</param>
        /// <param name="writeHeader">True, um eine Kopfzeile in die CSV-Datei zu schreiben. Als Spaltenbezeichner werden die Werte
        /// der <see cref="DataColumn.Caption"/>-Eigenschaft verwendet.</param>
        /// <param name="fieldSeparator">Das Feldtrennzeichen.</param>
        /// <param name="enc">Die zu verwendende Textenkodierung oder null, um die Datei als UTF-8 mit BOM zu schreiben.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> oder <paramref name="data"/> ist null.</exception>
        /// <exception cref="ArgumentException">Nicht alle Spalten von <paramref name="data"/> sind vom Datentyp <see cref="string"/> oder
        /// <paramref name="fileName"/> ist kein gültiger Dateipfad.</exception>
        /// <exception cref="IOException">Es konnte nicht auf den Datenträger geschrieben werden.</exception>
        public static void Save(
            string fileName,
            DataTable data,
            bool writeHeader = true,
            char fieldSeparator = ';',
            Encoding? enc = null)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));


            try
            {
                using var writer = new StreamWriter(fileName, false, enc ?? Encoding.UTF8); // UTF-8-Encoding mit BOM
                CsvWriter.Write(writer, data, writeHeader, fieldSeparator);
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
        /// Schreibt den Inhalt einer <see cref="IEnumerable{T}">IEnumerable&lt;IDictionary&lt;string, string?>?></see>-Collection in eine CSV-Datei.
        /// Die Values eines Dictionaries entsprechen dabei einer Datenzeile. Die Kopfzeile wird automatisch
        /// aus den Keys aller Dictionaries in der Collection ermittelt.
        /// </summary>
        /// <param name="fileName">Dateipfad der zu schreibenden Datei. Wenn die Datei existiert, wird sie überschrieben.</param>
        /// <param name="data">Ein <see cref="IEnumerable{T}">IEnumerable&lt;IDictionary&lt;string, string?>?></see>.</param>
        /// <param name="fieldSeparator">Das Feldtrennzeichen.</param>
        /// <param name="enc">Die zu verwendende Textenkodierung oder null, um die Datei als UTF-8 mit BOM zu schreiben.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> oder <paramref name="data"/> ist null.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> ist kein gültiger Dateipfad.</exception>
        /// <exception cref="IOException">Es konnte nicht auf den Datenträger geschrieben werden.</exception>
        public static void Save(
            string fileName,
            IEnumerable<IDictionary<string, string?>?> data,
            char fieldSeparator = ';',
            Encoding? enc = null)
        {
            try
            {
                using var writer = new StreamWriter(fileName, false, enc ?? Encoding.UTF8); // UTF-8-Encoding mit BOM
                CsvWriter.Write(writer, data, fieldSeparator);
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
        /// Schreibt den Inhalt einer <see cref="IEnumerable{T}">IEnumerable&lt;IDictionary&lt;string, string?>?></see>-Collection
        /// in einen <see cref="TextWriter"/>.
        /// Die Values eines Dictionaries entsprechen dabei einer Datenzeile. Die Kopfzeile wird automatisch
        /// aus den Keys aller Dictionaries in der Collection ermittelt.
        /// </summary>
        /// <param name="writer">Der <see cref="TextWriter"/>, mit dem die CSV-Daten geschrieben werden.</param>
        /// <param name="data">Ein <see cref="IEnumerable{T}">IEnumerable&lt;IDictionary&lt;string, string?>?></see>.</param>
        /// <param name="fieldSeparator">Das Feldtrennzeichen.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> oder <paramref name="data"/> ist null.</exception>
        /// <exception cref="IOException">Fehler beim Zugriff auf den Datenträger.</exception>
        /// <exception cref="ObjectDisposedException">Der <see cref="TextWriter"/> ist geschlossen.</exception>
        public static void Write(TextWriter writer, IEnumerable<IDictionary<string, string?>?> data, char fieldSeparator = ';')
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            using DataTable tmp = InitializeDataTable(data);
            CsvWriter.Write(writer, tmp, true, fieldSeparator);

            ///////////////////////////////////////////////////////////////////////

            static DataTable InitializeDataTable(IEnumerable<IDictionary<string, string?>?> daten)
            {
                DataTable dt = CsvHelper.CreateDataTable(daten.Where(x => x != null).SelectMany(dic => dic!.Keys).Distinct(StringComparer.OrdinalIgnoreCase));

                return FillData(daten, dt);

                static DataTable FillData(IEnumerable<IDictionary<string, string?>?> daten, DataTable dt)
                {
                    DataRow row;
                    foreach (var dictionary in daten)
                    {
                        if (dictionary == null) continue;

                        row = dt.NewRow();
                        foreach (var kvp in dictionary)
                        {
                            row[kvp.Key] = kvp.Value;
                        }
                        dt.Rows.Add(row);
                    }
                    return dt;
                }
            }

        }


        /// <summary>
        /// Schreibt <paramref name="data"/> im CSV-Format in einen <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">Der <see cref="TextWriter"/>, mit dem die CSV-Daten geschrieben werden.</param>
        /// <param name="data">Eine <see cref="DataTable"/>. Alle Spalten müssen vom Datentyp <see cref="string"/> sein, dürfen aber
        /// <see cref="DBNull"/> zulassen.</param>
        /// <param name="writeHeader">True, um eine Kopfzeile in die CSV-Datei zu schreiben. Als Spaltenbezeichner werden die Werte
        /// der <see cref="DataColumn.Caption"/>-Eigenschaft verwendet.</param>
        /// <param name="fieldSeparator">Das Feldtrennzeichen.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> oder <paramref name="data"/> ist null.</exception>
        /// <exception cref="ArgumentException">Nicht alle Spalten von <paramref name="data"/> sind vom Datentyp <see cref="string"/>.</exception>
        /// <exception cref="IOException">Fehler beim Zugriff auf den Datenträger.</exception>
        /// <exception cref="ObjectDisposedException">Der <see cref="TextWriter"/> ist geschlossen.</exception>
        public static void Write(TextWriter writer, DataTable data, bool writeHeader = true, char fieldSeparator = ';')
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            if (data is null)
                throw new ArgumentNullException(nameof(data));

            const string newLine = "\r\n";

#if NET40
        string fieldSeparatorString = new string(fieldSeparator, 1);
#endif

            for (int i = 0; i < data.Columns.Count; i++)
            {
                if (data.Columns[i].DataType != typeof(string))
                {
                    throw new ArgumentException(Resources.FalseDataType, nameof(data));
                }
            }

            writer.NewLine = newLine;
            int recordLength = data.Columns.Count;

            object currentField;

            if (writeHeader)
            {
                for (int i = 0; i < recordLength - 1; i++)
                {
                    WriteField(data.Columns[i].Caption);
                    writer.Write(fieldSeparator);
                }

                WriteField(data.Columns[recordLength - 1].Caption);
                writer.WriteLine();
            }

            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow currentRecord = data.Rows[i];

                for (int j = 0; j < recordLength - 1; j++)
                {
                    currentField = currentRecord[j];
                    if (currentField is string s)
                    {
                        WriteField(s);
                    }

                    writer.Write(fieldSeparator);
                }

                currentField = currentRecord[recordLength - 1];

                if (currentField is string lastString)
                {
                    WriteField(lastString);
                }
                writer.WriteLine();
            }

            void WriteField(string field)
            {
                bool needsToBeQuoted = NeedsToBeQuoted(field);

                if (needsToBeQuoted)
                {
                    writer.Write('"');

                    for (int j = 0; j < field.Length; j++)
                    {
                        char c = field[j];

                        if (c == Environment.NewLine[0])
                        {
                            writer.WriteLine();
                            j += Environment.NewLine.Length - 1;
                        }
                        else if (c == '\"')
                        {
                            writer.Write('"');
                            writer.Write(c);
                        }
                        else
                        {
                            writer.Write(c);
                        }
                    }

                    writer.Write('"');
                }
                else
                {
                    writer.Write(field);
                }
            }

#if NET40
            bool NeedsToBeQuoted(string s) => (s.Contains(fieldSeparatorString) ||
                                                  s.Contains("\"") ||
                                                  s.Contains(Environment.NewLine)); 
#else
            bool NeedsToBeQuoted(string s) => (s.Contains(fieldSeparator, StringComparison.Ordinal) ||
                                                      s.Contains('"', StringComparison.Ordinal) ||
                                                      s.Contains(Environment.NewLine, StringComparison.Ordinal));
#endif

        }

    }
}
