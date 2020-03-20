using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FolkerKinzel.Csv.Exceptions;
using FolkerKinzel.Csv.Utility;
using System.Diagnostics;
using FolkerKinzel.Csv.Properties;
using System.Globalization;

namespace FolkerKinzel.Csv
{
    /// <summary>
    /// Statische Klasse, die Methoden zum Lesen und Schreiben von csv-Dateien enthält.
    /// </summary>
    public static partial class CsvIO
    {
        private const char SEPARATOR_REPLACEMENT = '\u0080'; // <CONTROL>
        private const char CARRIAGE_RETURN_REPLACEMENT = '\u0081'; // <CONTROL>
        private const char LINEFEED_REPLACEMENT = '\u0085'; // NEL - Nächste Zeile

        ///// <summary>
        ///// Liest eine csv-Datei und gibt ihren Inhalt als DataTable zurück. Zeilenwechselzeichen werden
        ///// auf das auf dem ausführenden System übliche Zeilenwechselzeichen normalisiert. Als Textencodierung
        ///// wird Encoding.Default angenommen.
        ///// </summary>
        ///// <param name="filename">Der Pfad zur csv-Datei.</param>
        ///// <param name="separator">Das Spaltentrennzeichen.</param>
        ///// <param name="hasHeaderRow">Gibt an, ob die Datei eine Kopfzeile hat.</param>
        ///// <param name="trimColumns">Wenn True, werden Anführungszeichen sowie führender und nachgestellter 
        ///// Leerraum aller Felder entfernt (auch der Spaltenbezeichner der Kopfzeile).</param>
        ///// <param name="csvMode">Die Eingabe von CsvModeEnum.NonStandard bewirkt, dass nicht 
        ///// standardgerechte Csv-Dateien, in denen in maskierten Bereichen die Gänsefüßchen nicht maskiert worden sind,
        ///// besser gelesen werden können.</param>
        ///// <param name="normalizeNewLines">True bedeutet, dass Zeilenwechselzeichen auf <see cref="Environment.NewLine"/>
        ///// normalisiert werden (Standard). Wenn in der Datei keine systemfremden Zeilenwechselzeichen enthalten sind, kann hier 
        ///// false eingegeben werden, um die Performance zu verbessern.</param>
        ///// <returns>Eine DataTable, die die Daten der csv-Datei enthält. Alle Felder der Tabelle sind vom Typ <see cref="string"/>.
        ///// Die Tabelle kann zwar NULL-Werte (DBNull.Value) enthalten, aber bei normaler Programmausführung haben leere Felder den
        ///// Wert string.Empty.</returns>
        ///// <exception cref="ArgumentException"><paramref name="filename"/> ist kein gültiger Dateiname.</exception>
        ///// <exception cref="CsvReaderException">Es gibt Probleme beim Laden der csv-Datei.</exception>
        ///// <exception cref="InvalidCsvException">Tritt auf, wenn die csv-Datei nicht auflösbare Fehler enthält.</exception>
        //public static DataTable ReadCsv(
        //    string filename,
        //    char separator = ';',
        //    bool hasHeaderRow = true, 
        //    bool trimColumns = true,
        //    CsvMode csvMode = CsvMode.Standard,
        //    bool normalizeNewLines = true)
        //{
        //    return ReadCsv(filename, Encoding.Default, separator, hasHeaderRow, trimColumns, csvMode, normalizeNewLines);
        //}


        /// <summary>
        /// Liest eine csv-Datei und gibt ihren Inhalt als DataTable zurück. Zeilenwechselzeichen werden normalisiert.
        /// </summary>
        /// <param name="filename">Der Pfad zur csv-Datei.</param>
        /// <param name="encoding">Die Textencodierung der csv-Datei. Wird null angegeben, wird versucht, die Textencodierung
        /// automatisch zu bestimmen.</param>
        /// <param name="separator">Das Spaltentrennzeichen.</param>
        /// <param name="hasHeaderRow">Gibt an, ob die Datei eine Kopfzeile hat.</param>
        /// <param name="trimColumns">Wenn True, werden Anführungszeichen sowie führender und nachgestellter 
        /// Leerraum aller Felder entfernt (auch der Spaltenbezeichner der Kopfzeile).</param>
        /// <param name="csvMode">Die Eingabe von CsvModeEnum.NonStandard bewirkt, dass nicht 
        /// standardgerechte Csv-Dateien, in denen in maskierten Bereichen die Gänsefüßchen nicht maskiert worden sind,
        /// besser gelesen werden können.</param>
        /// <returns>Eine DataTable, die die Daten der csv-Datei enthält. Alle Felder der Tabelle sind vom Typ <see cref="string"/>.
        /// Die Tabelle kann NULL-Werte (DBNull.Value) enthalten.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="filename"/> ist null.</exception>
        /// <exception cref="ArgumentException"><paramref name="filename"/> ist kein gültiger Dateiname.</exception>
        /// <exception cref="CsvReaderException">Es gibt Probleme beim Laden
        /// der csv-Datei.</exception>
        /// <exception cref="InvalidCsvException">Tritt auf, wenn die csv-Datei
        /// nicht auflösbare Fehler enthält.</exception>
        public static DataTable ReadCsv(
            string filename,
            Encoding? encoding = null,
            char separator = ';',
            bool hasHeaderRow = true,
            bool trimColumns = true,
            CsvMode csvMode = CsvMode.Standard)
        {
            //string csvData;
            string[] zeilen;

            try
            {
                zeilen = (encoding is null) ? File.ReadAllLines(filename) : File.ReadAllLines(filename, encoding);
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
            catch (NotSupportedException e)
            {
                throw new ArgumentException(e.Message, nameof(filename), e);
            }
            catch(DirectoryNotFoundException e)
            {
                throw new ArgumentException(e.Message, nameof(filename), e);
            }
            catch (IOException e)
            {
                throw new CsvReaderException(e.Message, e);
            }
            catch (SystemException e)
            {
                throw new CsvReaderException(e.Message, e);
            }
            catch (Exception e)
            {
                throw new CsvReaderException(e.Message, e);
            }

            

            if (zeilen.Length == 0) return new DataTable();

            string csvText = string.Join(Environment.NewLine, zeilen);

            bool dirtyText = !ValidateFile(ref csvText, separator, csvMode);

            zeilen = csvText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (zeilen.Length == 0) return new DataTable();


            DataTable dattb;

            if (hasHeaderRow)
            {
                try
                {
                    dattb = InitDataTableWithHeader(zeilen[0], separator, dirtyText, trimColumns, csvMode);
                }
                catch
                {
                    // Es ist nicht zu 100% auszuschließen, dass die Methode eine Endlosschleife produziert:
                    throw new CsvReaderException(Resources.HeaderRecordNotInitialized);
                }
            }
            else
            {
                dattb = InitDataTableWithoutHeader(zeilen[0].Split(separator).Length);
            }

            FillDataTable(zeilen, dattb, dirtyText, hasHeaderRow, separator, trimColumns, csvMode);

            return dattb;
        }


        #region private

        ///// <summary>
        ///// Setzt alle Newline-Zeichen im Text auf Environment.NewLine.
        ///// </summary>
        ///// <param name="s">Der gesamte csv-Text.</param>
        ///// <returns>Der bereinigte csv-Text.</returns>
        //private static string NormalizeNewLine(string s)
        //{
        //    StringBuilder sb = new StringBuilder(s);

        //    if (Environment.NewLine == "\r\n")
        //    {
        //        sb.Replace("\r\n", "\n");
        //        sb.Replace("\r", "\n");
        //        sb.Replace("\n", Environment.NewLine);
        //    }

        //    else if (Environment.NewLine == "\n")
        //    {
        //        sb.Replace("\r\n", Environment.NewLine);
        //        sb.Replace("\r", Environment.NewLine);
        //    }
        //    return sb.ToString();
        //}


        #region ValidateFile

        /// <summary>
        /// Untersucht den csv-Text auf verbotene Zeichen und ersetzt diese temporär.
        /// </summary>
        /// <param name="csvData">Der csv-Text.</param>
        /// <param name="separator">Das Trennzeichen.</param>
        /// <param name="csvMode">Modus, nach dem maskierte Bereiche gesucht werden.</param>
        /// <returns>True, wenn keine verbotene Zeichen gefunden wurden.</returns>
        private static bool ValidateFile(ref string csvData, char separator, CsvMode csvMode)
        {
            List<int> matchlist = csvMode == CsvMode.Standard ? GetMatchListStandard(csvData) : GetMatchListNonStandard(csvData, separator.ToString(CultureInfo.InvariantCulture));

            if (matchlist.Count == 0) return true;

            //Ungültige Zeichen temporär ersetzen
            StringBuilder testsb = new StringBuilder(csvData.Length);
            testsb.Append(csvData);

            for (int i = matchlist.Count - 1; i > 0; i -= 2)
            {
                for (int j = matchlist[i] - 1; j > matchlist[i - 1]; j--)
                {
                    if (testsb[j] == separator)
                    {
                        testsb[j] = SEPARATOR_REPLACEMENT;
                        continue;
                    }
                    if (testsb[j] == '\r')
                    {
                        testsb[j] = CARRIAGE_RETURN_REPLACEMENT;
                        continue;
                    }
                    if (testsb[j] == '\n')
                    {
                        testsb[j] = LINEFEED_REPLACEMENT;
                        continue;
                    }
                }
            }
            csvData = testsb.ToString();
            return false;
        }


        private static List<int> GetMatchListStandard(string csvData)
        {
            List<int> matchlist = new List<int>();

            bool nextIsOpener = true;

            for (int i = 0; i < csvData.Length; i++)
            {
                if (csvData[i] == '\"')
                {
                    if (nextIsOpener)
                    {
                        nextIsOpener = false;
                        matchlist.Add(i);
                    }
                    else
                    {
                        if (i == csvData.Length - 1)
                        {
                            matchlist.Add(i);
                            break;
                        }

                        if (csvData[++i] != '\"')
                        {
                            nextIsOpener = true;
                            matchlist.Add(i);
                        }
                    }
                }
            }

            return matchlist;
        }

        private static List<int> GetMatchListNonStandard(string csvData, string separator)
        {
            List<int> matchlist = new List<int>();

            //Öffnende Anführungszeichen suchen
            MatchCollection mcStart =
                Regex.Matches(csvData, "\n\"|" + Regex.Escape(separator) + "\"");

            if (mcStart.Count == 0) return matchlist;

            //Schließende Anführungszeichen suchen
            MatchCollection mcEnd =
                Regex.Matches(csvData, "\"" + Regex.Escape(Environment.NewLine) + "|\"" + Regex.Escape(separator));

            //gemeinsame Liste aller Treffer erstellen
            List<int> startlist = new List<int>();

            foreach (Match match in mcStart)
            {
                if (match.Index < csvData.Length - 1)
                {
                    startlist.Add(match.Index + 1);
                }
            }

            for (int i = 0; i < startlist.Count; i++)
            {
                foreach (Match match in mcEnd)
                {
                    if (i < startlist.Count - 1)
                    {
                        //ein öffnendes Anführungszeichen zuviel
                        if (match.Index > startlist[i + 1]) break;
                    }

                    if (match.Index > startlist[i])
                    {
                        matchlist.Add(startlist[i]);
                        matchlist.Add(match.Index);
                        break;
                    }
                }//foreach
            }//for

            return matchlist;
        }


        #endregion


        #region InitDataTable

        /// <summary>
        /// Legt eine neue Datatable an, wenn die Datei eine Headerzeile hat.
        /// </summary>
        /// <param name="zeile">Die 1. Zeile der csv-Datei.</param>
        /// <param name="separator">Das Spaltentrennzeichen.</param>
        /// <param name="dirtyText">Gibt an, ob die csv-Datei maskierte verbotene Zeichen enthält,
        /// die zurückgewandelt werden müssen.</param>
        /// <param name="trimColumns">Wenn True, wird führender und nachgestellter 
        /// Leerraum aller Felder entfernt (auch der Spaltenbezeichner der Kopfzeile).</param>
        /// <param name="csvMode"></param>
        /// <returns>Die Datatable, deren Spaltennamen den 
        /// Feldern der Headerzeile entsprechen.</returns>
        private static DataTable InitDataTableWithHeader(string zeile, char separator, bool dirtyText , bool trimColumns, CsvMode csvMode)
        {
            string[] headerRow = zeile.Split(separator);

            CleanString(headerRow, separator, dirtyText, csvMode, trimColumns);

            return DoInitDataTableWithHeader(headerRow);
        }

        private static DataTable DoInitDataTableWithHeader(string?[] headerRow)
        {
            DataTable dattb;
            try
            {
                dattb = CsvHelper.CreateDataTable(headerRow);
            }
            catch (DuplicateNameException)
            {
                dattb = DoInitDataTableWithHeader(CleanColumnNames(headerRow));

                for (int i = 0; i < headerRow.Length; i++)
                {
                    dattb.Columns[i].Caption = headerRow[i];
                }
            }
            return dattb;
        }

        ///// <summary>
        ///// Entfernt Leerraum und Anführungszeichen am Anfang und am Ende jedes Eintrags der Kopfzeile.)
        ///// </summary>
        ///// <param name="headerRow">Die Kopfzeile.</param>
        //private static void TrimHeader(string[] headerRow)
        //{
        //    for (int i = 0; i < headerRow.Length; i++)
        //    {
        //        headerRow[i] = headerRow[i].Trim();
        //        headerRow[i] = headerRow[i].TrimStart('"');
        //        headerRow[i] = headerRow[i].TrimEnd('"');
        //    }
        //}

        /// <summary>
        /// Verhindert, dass in der DataTable unterschiedliche Spalten mit gleichem Namen angelegt werden.
        /// Wird nur aufgerufen, wenn beim Initialisieren der DataTable ein Fehler auftrat.
        /// </summary>
        /// <param name="sarr">Die ursprüngliche Headerzeile der csv-Datei.</param>
        /// <returns>Die geänderte Headerzeile.</returns>
        private static string?[] CleanColumnNames(string?[] sarr)
        {
            var newArr = new string?[sarr.Length];

            for (int i = 0; i < sarr.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(sarr[i]))
                {
                    sarr[i] = null;
                }
                else
                {
                    newArr[i] = sarr[i];
                }
            }

            int end = newArr.Length - 1;
            StringComparer comp = StringComparer.InvariantCultureIgnoreCase;

            for (int i = 0; i < end; i++)
            {
                if (newArr[i] == null) continue;

                int counter = 2;

                for (int j = i + 1; j < newArr.Length; j++)
                {
                    if (comp.Compare(newArr[i], newArr[j]) == 0)
                    {
                        newArr[j] = newArr[j] + counter++.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }

            return newArr;
        }


        /// <summary>
        /// Legt eine neue Datatable an, wenn die Datei keine Headerzeile hat.
        /// </summary>
        /// <param name="columnsCount">Anzahl der Felder.</param>
        /// <returns>Die Datatable, deren Spaltenanzahl der Zahl der Felder in der ersten Zeile
        /// der csv-Datei entspricht. Die Spaltennamen haben Standardwerte.</returns>
        private static DataTable InitDataTableWithoutHeader(int columnsCount)
        {
            DataTable dattb = new System.Data.DataTable();

            // wenn der ColumnName mit null initialisiert wird, erhalten die Spalten automatisch Namen ("Column1", "Column2" etc.)
            for (int i = 0; i < columnsCount; i++)
                dattb.Columns.Add(null, typeof(string));

            return dattb;
        }

        #endregion


        /// <summary>
        /// Füllt die DataTable mit den Daten.
        /// </summary>
        /// <param name="zeilen">Die in Zeilen gesplittete csv-Datei.</param>
        /// <param name="dt">Die DataTable, die gefüllt wird.</param>
        /// <param name="dirtyText">Gibt an, ob die csv-Datei maskierte verbotene Zeichen enthält,
        /// die zurückgewandelt werden müssen.</param>
        /// <param name="hasHeaderRow">Gibt an, ob die csv-Datei eine Header-Zeile enthält.</param>
        /// <param name="separator">Das Spaltentrennzeichen.</param>
        /// <param name="trimColumns">Wenn True, wird führender und nachgestellter 
        /// Leerraum aller Felder entfernt.</param>
        /// <param name="csvMode">Die Eingabe von CsvModeEnum.NonStandard bewirkt, dass nicht 
        /// standardgerechte Csv-Dateien, in denen in maskierten Bereichen die Gänsefüßchen nicht maskiert worden sind,
        /// besser gelesen werden können.</param>
        private static void FillDataTable(
            string[] zeilen,
            DataTable dt, 
            bool dirtyText, 
            bool hasHeaderRow,
            char separator,
            bool trimColumns,
            CsvMode csvMode)
        {
            string[] sarr;

            int startindex = 0;
            if (hasHeaderRow) startindex++;

            for (int i = startindex; i < zeilen.Length; i++)
            {
                sarr = zeilen[i].Split(separator);
                if (sarr.Length > dt.Columns.Count)
                {
                    throw new InvalidCsvException(Resources.TooManyFields);
                }
                
                CleanString(sarr, separator, dirtyText, csvMode, trimColumns);
                
                dt.LoadDataRow(sarr, false);
            }
        }


        private static void CleanString(string[] arr, char separator, bool dirtyText, CsvMode csvMode, bool trimColumns)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                Debug.Assert(arr[i] != null);

                if (dirtyText)
                {
                    arr[i] = arr[i].Replace(SEPARATOR_REPLACEMENT, separator);
                    arr[i] = arr[i].Replace(CARRIAGE_RETURN_REPLACEMENT, '\r');
                    arr[i] = arr[i].Replace(LINEFEED_REPLACEMENT, '\n');
                }

                if (csvMode == CsvMode.Standard)
                {
#if NET40
                    arr[i] = arr[i].Replace("\"\"", "\"");
#else
                    arr[i] = arr[i].Replace("\"\"", "\"", StringComparison.Ordinal);
#endif
                }

                if (trimColumns)
                {
                    arr[i] = arr[i].Trim();

                    int startIndex = arr[i].StartsWith("\"", StringComparison.Ordinal) ? 1 : 0;
                    int length = arr[i].EndsWith("\"", StringComparison.Ordinal) && arr[i].Length != 1 ? arr[i].Length - 1 : arr[i].Length;

                    length -= startIndex;
                    

                    if (length != arr[i].Length)
                        arr[i] = arr[i].Substring(startIndex, length);
                    
                }
            }

        }


        




#endregion

    }
    
}
