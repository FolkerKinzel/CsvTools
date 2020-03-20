using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FolkerKinzel.Csv.Exceptions;
using FolkerKinzel.Csv.Properties;

namespace FolkerKinzel.Csv
{
    public static partial class CsvIO
    {
        /// <summary>
        /// Schreibt den Inhalt einer DataTable in eine csv-Datei. Als Trennzeichen wird ';' verwendet.
        /// Die Textencodierung entspricht <see cref="Encoding.UTF8"/>. Das Zeilenwechselzeichen ist <see cref="Environment.NewLine"/>.
        /// </summary>
        /// <remarks>Die Methode ist threadsicher.</remarks>
        /// <param name="data">Die DataTable. Der Datentyp ist beliebig: Zur Umwandlung wird object.ToString() angewendet. 
        /// Die DataTable darf NULL-Werte enthalten.</param>
        /// <param name="filename">Der Pfad der zu erzeugenden csv-Datei. 
        /// Existiert die Datei schon, wird sie überschrieben.</param>
        /// <param name="newLineReplacement">Das (bzw. die) Zeichen durch durch welche(s) evtl. 
        /// innerhalb einer Datenzeile vorkommende NewLine-Zeichen ersetzt werden sollen.
        /// Bei null findet keine Ersetzung statt.</param>
        /// <param name="trimColumns">Wenn true, wird führender und nachgestellter Leerraum in allen 
        /// Feldern der csv-Datei entfernt - auch in der Kopfzeile.</param>
        /// <param name="writeHeaderRow">Bestimmt, ob eine Kopfzeile in die csv-Datei geschrieben 
        /// werden soll, deren Bezeichner aus der DataColumn.Caption-Eigenschaft der DataTable
        /// gewonnen werden.</param>
        /// <param name="csvMode">Eingabe von CsvModeEnum.NonStandard bewirkt, das Gänsefüßchen in maskierten
        /// Bereichen durch \"'\"-Zeichen ersetzt werden (statt durch doppelte Gänsefüßchen).</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> oder <paramref name="filename"/> ist null.</exception>
        /// <exception cref="ArgumentException"><paramref name="filename"/> ist kein gültiger Dateiname.</exception>
        /// <exception cref="CsvWriterException">Tritt auf, wenn beim Schreiben einer csv-Datei
        /// ein Fehler auftritt.</exception>
        /// <threadsafety static="true" instance="false" />
        public static void WriteCsv(
            DataTable data,
            string filename,
            string? newLineReplacement = null,
            bool trimColumns = false,
            bool writeHeaderRow = true,
            CsvMode csvMode = CsvMode.Standard)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            
            DoWriteCsv(data, filename, Encoding.UTF8, NewLineConst.Default, ';', newLineReplacement, trimColumns, writeHeaderRow, csvMode);
            
        }


        /// <summary>
        /// Schreibt den Inhalt einer DataTable in eine csv-Datei.
        /// </summary>
        /// <param name="data">Die DataTable. Der Datentyp ist beliebig: Zur Umwandlung wird object.ToString() angewendet. 
        /// Die DataTable darf NULL-Werte enthalten.</param>
        /// <param name="filename">Der Pfad der zu erzeugenden csv-Datei. 
        /// Existiert die Datei schon, wird sie überschrieben.</param>
        /// <param name="encoding">Die Textencodierung, die beim Schreiben der csv-Datei verwendet wird.</param>
        /// <param name="newLine">Das NewLine-Zeichen, das zum Trennen der Datenzeilen verwendet wird.</param>
        /// <param name="separator">Das Zeichen, das als Spalten-Trennzeichen in der 
        /// csv-Datei verwendet wird.</param>
        /// <param name="newLineReplacement">Das (bzw. die) Zeichen durch durch welche(s) evtl. 
        /// innerhalb einer Datenzeile vorkommende NewLine-Zeichen ersetzt werden sollen.
        /// Bei null findet keine Ersetzung statt.</param>
        /// <param name="trimColumns">Wenn true, wird führender und nachgestellter Leerraum in allen 
        /// Feldern der csv-Datei entfernt - auch in der Kopfzeile.</param>
        /// <param name="writeHeaderRow">Bestimmt, ob eine Kopfzeile in die csv-Datei geschrieben 
        /// werden soll, deren Bezeichner aus der DataColumn.Caption-Eigenschaft der DataTable
        /// gewonnen werden.</param>
        /// <param name="csvMode">Eingabe von CsvModeEnum.NonStandard bewirkt, das Gänsefüßchen in maskierten
        /// Bereichen durch \"'\"-Zeichen ersetzt werden (statt durch doppelte Gänsefüßchen).</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> oder oder <paramref name="filename"/> oder
        /// <paramref name="encoding"/> oder <paramref name="newLine"/> ist null.</exception>
        /// <exception cref="ArgumentException"><paramref name="filename"/> ist kein gültiger Dateiname.</exception>
        /// <exception cref="CsvWriterException">Tritt auf, wenn beim Schreiben einer csv-Datei
        /// ein Fehler auftritt.</exception>
        public static void WriteCsv(
            DataTable data,
            string filename,
            Encoding encoding,
            NewLineConst newLine,
            char separator = ';',
            string? newLineReplacement = null,
            bool trimColumns = false,
            bool writeHeaderRow = true,
            CsvMode csvMode = CsvMode.Standard)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            if (newLine == null)
            {
                throw new ArgumentNullException(nameof(newLine));
            }

            
            DoWriteCsv(data, filename, encoding, newLine, separator, newLineReplacement, trimColumns, writeHeaderRow, csvMode);
            
        }


        /// <summary>
        /// Schreibt den Inhalt einer <see cref="IEnumerable{T}">IEnumerable&lt;IDictionary&lt;string, object?>></see>/>-Collection in eine csv-Datei.
        /// Die Values eines Dictionaries entsprechen dabei einer Datenzeile. Die Kopfzeile wird automatisch
        /// aus den Keys aller Dictionaries in der Collection ermittelt. Als Trennzeichen wird ';' verwendet.
        /// Die Textencodierung entspricht <see cref="Encoding.UTF8"/>. Das Zeilenwechselzeichen ist <see cref="Environment.NewLine"/>.
        /// </summary>
        /// <param name="data"><see cref="IEnumerable{T}">IEnumerable&lt;IDictionary&lt;string, object?>></see>/>. <paramref name="data"/> 
        /// darf nicht null sein, aber null-Werte enthalten.</param>
        /// <param name="filename">Der Pfad der zu erzeugenden csv-Datei. 
        /// Existiert die Datei schon, wird sie überschrieben.</param>
        /// <param name="newLineReplacement">Das (bzw. die) Zeichen durch durch welche(s) evtl. 
        /// innerhalb einer Datenzeile vorkommende NewLine-Zeichen ersetzt werden sollen.
        /// Bei null findet keine Ersetzung statt.</param>
        /// <param name="trimColumns">Wenn true, wird führender und nachgestellter Leerraum in allen 
        /// Feldern der csv-Datei entfernt - auch in der Kopfzeile.</param>
        /// <param name="csvMode">Eingabe von CsvModeEnum.NonStandard bewirkt, das Gänsefüßchen in maskierten
        /// Bereichen durch \"'\"-Zeichen ersetzt werden (statt durch doppelte Gänsefüßchen).</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> oder <paramref name="filename"/> ist null.</exception>
        /// <exception cref="ArgumentException"><paramref name="filename"/> ist kein gültiger Dateiname.</exception>
        /// <exception cref="CsvWriterException">Tritt auf, wenn beim Schreiben einer csv-Datei
        /// ein Fehler auftritt.</exception>
        public static void WriteCsv(
            IEnumerable<IDictionary<string, object?>> data,
            string filename,
            string? newLineReplacement = null,
            bool trimColumns = false,
            CsvMode csvMode = CsvMode.Standard)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using DataTable? dt = InitializeDataTable(data);

            if (dt == null)
                throw new CsvWriterException(Resources.DataNotProcessed);

            
            DoWriteCsv(dt, filename, Encoding.UTF8, NewLineConst.Default, ';', newLineReplacement, trimColumns, true, csvMode);
        }


        /// <summary>
        /// Schreibt den Inhalt einer <see cref="IEnumerable{T}">IEnumerable&lt;IDictionary&lt;string, object?>></see>-Collection in eine csv-Datei.
        /// Die Values eines Dictionaries entsprechen dabei einer Datenzeile. Die Kopfzeile wird automatisch
        /// aus den Keys aller Dictionaries in der Collection ermittelt.
        /// </summary>
        /// <param name="data"><see cref="IEnumerable{T}">IEnumerable&lt;IDictionary&lt;string, object?>></see>/>. <paramref name="data"/> 
        /// darf nicht null sein, aber null-Werte enthalten.</param>
        /// <param name="filename">Der Pfad der zu erzeugenden csv-Datei. 
        /// Existiert die Datei schon, wird sie überschrieben.</param>
        /// <param name="encoding">Die Textencodierung, die beim Schreiben der csv-Datei verwendet wird.</param>
        /// <param name="newLine">Das NewLine-Zeichen, das zum Trennen der Datenzeilen verwendet wird.</param>
        /// <param name="separator">Das Zeichen, das als Spalten-Trennzeichen in der 
        /// csv-Datei verwendet wird.</param>
        /// <param name="newLineReplacement">Das (bzw. die) Zeichen durch durch welche(s) evtl. 
        /// innerhalb einer Datenzeile vorkommende NewLine-Zeichen ersetzt werden sollen.
        /// Bei null findet keine Ersetzung statt.</param>
        /// <param name="trimColumns">Wenn true, wird führender und nachgestellter Leerraum in allen 
        /// Feldern der csv-Datei entfernt - auch in der Kopfzeile.</param>
        /// <param name="csvMode">Eingabe von CsvModeEnum.NonStandard bewirkt, das Gänsefüßchen in maskierten
        /// Bereichen durch \"'\"-Zeichen ersetzt werden (statt durch doppelte Gänsefüßchen).</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> oder <paramref name="filename"/> oder 
        /// <paramref name="encoding"/> oder <paramref name="newLine"/> ist null.</exception>
        /// <exception cref="ArgumentException"><paramref name="filename"/> ist kein gültiger Dateiname.</exception>
        /// <exception cref="CsvWriterException">Tritt auf, wenn beim Schreiben einer csv-Datei
        /// ein Fehler auftritt.</exception>
        public static void WriteCsv(
            IEnumerable<IDictionary<string, object?>> data,
            string filename,
            Encoding encoding,
            NewLineConst newLine,
            char separator = ';',
            string? newLineReplacement = null,
            bool trimColumns = false,
            CsvMode csvMode = CsvMode.Standard)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            if (newLine == null)
            {
                throw new ArgumentNullException(nameof(newLine));
            }

            using DataTable? dt = InitializeDataTable(data);

            if (dt == null)
                throw new CsvWriterException(Resources.DataNotProcessed);

            DoWriteCsv(dt, filename, encoding, newLine, separator, newLineReplacement, trimColumns, true, csvMode);
        }


        #region private

        #region InitializeDataTable

        private static DataTable? InitializeDataTable(IEnumerable<IDictionary<string, object?>> daten)
        {
            DataTable? dt = null;
            try
            {
                dt = CsvHelper.CreateDataTable(daten.SelectMany(dic => dic == null ? (new Dictionary<string,object?>()).Keys : dic.Keys).Distinct());
            }
            catch(DuplicateNameException)
            {
                dt?.Dispose();
                return null;
            }

            return FillData(daten, dt);
        }


        private static DataTable FillData(IEnumerable<IDictionary<string, object?>> daten, DataTable dt)
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

        #endregion


        private static void DoWriteCsv(DataTable data, string filename, Encoding encoding, NewLineConst newLine, char separator, string? newLineReplacement, bool trimColumns, bool writeHeaderRow, CsvMode csvMode)
        {
            try
            {
                using StreamWriter sw = new StreamWriter(filename, false, encoding);
                if (data.Columns.Count == 0) return;

                sw.NewLine = newLine.Value;

                if (writeHeaderRow)
                {
                    WriteHeaderRow(data, sw, newLineReplacement, separator, trimColumns, csvMode);
                }

                WriteDataRows(data, sw, newLineReplacement, separator, trimColumns, csvMode);
            }
            catch (PathTooLongException e)
            {
                throw new ArgumentException(e.Message, nameof(filename), e);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException(nameof(filename));
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(e.Message, nameof(filename), e);
            }
            catch (IOException e)
            {
                throw new CsvWriterException(e.Message, e);
            }
            catch (SystemException e)
            {
                throw new CsvWriterException(e.Message, e);
            }
            catch (Exception e)
            {
                throw new CsvWriterException(e.Message, e);
            }
        }


        private static void WriteHeaderRow(
            DataTable daten,
            StreamWriter sw,
            string? newLineReplacement,
            char separator,
            bool trimColumns,
            CsvMode csvMode)
        {
            string[] zeile = new string[daten.Columns.Count];

            for (int i = 0; i < zeile.Length; i++)
            {
                zeile[i] = Check(daten.Columns[i].Caption, newLineReplacement, separator, trimColumns, csvMode);
            }
            string z = string.Join(separator.ToString(CultureInfo.InvariantCulture), zeile);

            sw.WriteLine(z);
        }

        

        private static void WriteDataRows(
            DataTable daten,
            StreamWriter sw,
            string? newLineReplacement,
            char separator,
            bool trimColumns,
            CsvMode csvMode)
        {
            string[] zeile = new string[daten.Columns.Count];

            foreach (DataRow row in daten.Rows)
            {
                for (int i = 0; i < zeile.Length; i++)
                {
                    if (!Convert.IsDBNull(row[i]))
                    {
                        zeile[i] = Check(row[i].ToString(), newLineReplacement, separator, trimColumns, csvMode);
                    }
                }

                sw.WriteLine(string.Join(separator.ToString(CultureInfo.InvariantCulture), zeile));

                Array.Clear(zeile, 0, zeile.Length);
            }
        }



        private static string Check(string s, string? newLineReplacement, char separator, bool trimColumns, CsvMode csvMode)
        {
            //Leerraum entfernen
            if (trimColumns) s = s.Trim();

            //Ersetzen und Maskieren von NewLine-Zeichen und Gänsefüßchen
            s = ReplaceNewLine(s, newLineReplacement, separator, csvMode);

            return s;
        }


        private static string ReplaceNewLine(string s, string? newLineReplacement, char separator, CsvMode csvMode)
        {
            if (newLineReplacement != null)
            {
#if NET40
                if (!newLineReplacement.Contains("\n") && !newLineReplacement.Contains("\r"))
                {
                    s = s.Replace("\r\n", newLineReplacement);
                    s = s.Replace("\n", newLineReplacement);
                    s = s.Replace("\r", newLineReplacement);
                }
                else
                {
                    s = s.Replace("\r\n", "\u0085");
                    s = s.Replace('\n', '\u0085');
                    s = s.Replace('\r', '\u0085');
                    s = s.Replace("\u0085", newLineReplacement);
                }
#else
                if (!newLineReplacement.Contains('\n', StringComparison.Ordinal) && !newLineReplacement.Contains('\r', StringComparison.Ordinal))
                {
                    s = s.Replace("\r\n", newLineReplacement, StringComparison.Ordinal);
                    s = s.Replace("\n", newLineReplacement, StringComparison.Ordinal);
                    s = s.Replace("\r", newLineReplacement, StringComparison.Ordinal);
                }
                else
                {
                    s = s.Replace("\r\n", "\u0085", StringComparison.Ordinal);
                    s = s.Replace('\n', '\u0085');
                    s = s.Replace('\r', '\u0085');
                    s = s.Replace("\u0085", newLineReplacement, StringComparison.Ordinal);
                }
#endif

            }

            //falls noch unerlaubte Zeichen enthalten sind,
            //string in Anführungszeichen setzen und Anführungszeichen
            //in der Mitte des Strings durch doppelte Anführungszeichen
            //maskieren
            if (s.LastIndexOf(separator) != -1 ||
                s.LastIndexOf('\n') != -1 ||
                s.LastIndexOf('\r') != -1)
            {
                if (csvMode == CsvMode.Standard)
                {
#if NET40
                    s = s.Replace("\"", "\"\"");
#else
                    s = s.Replace("\"", "\"\"", StringComparison.Ordinal);
#endif
                }

                s = "\"" + s + "\"";
            }
            return s;
        }

#endregion


        

    } //class
} //ns
