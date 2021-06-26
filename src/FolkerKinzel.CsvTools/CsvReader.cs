using FolkerKinzel.CsvTools.Helpers;
using FolkerKinzel.CsvTools.Intls;
using FolkerKinzel.CsvTools.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace FolkerKinzel.CsvTools
{
    /// <summary>
    /// Bietet schreibgeschützten Vorwärtszugriff auf die Datensätze einer CSV-Datei. (Das bedeutet, dass der <see cref="CsvReader"/> die Datei nur einmal vorwärts
    /// lesen kann.)
    /// </summary>
    /// <remarks>
    /// <para>
    /// Die Methode <see cref="Read"/> gibt einen <see cref="IEnumerator{T}">IEnumerator&lt;CsvRecord&gt;</see> zurück, mit dem Sie über die Datensätze der CSV-Datei iterieren
    /// können, die in Form von <see cref="CsvRecord"/>-Objekten zurückgegeben werden.
    /// </para>
    /// <para>Die Klasse <see cref="CsvRecordWrapper"/> bietet die
    /// Möglichkeit, die Reihenfolge der Datenspalten der <see cref="CsvRecord"/>-Objekte zur Laufzeit auf die Spaltenreihenfolge einer <see cref="DataTable"/>
    /// zu mappen und Typkonvertierungen durchzuführen.
    /// </para>
    /// <para>Beim Lesen einer unbekannten CSV-Datei können die geeigneten Parameter für den Konstruktor von <see cref="CsvReader"/> mit Hilfe der Klasse
    /// <see cref="CsvAnalyzer"/> ermittelt werden.</para>
    /// </remarks>
    /// <example>
    /// <note type="note">In den folgenden Code-Beispielen wurde - der leichteren Lesbarkeit wegen - auf Ausnahmebehandlung verzichtet.</note>
    /// <para>Linq-Abfrage auf einer CSV-Datei:</para>
    /// <code language="cs" source="..\Examples\LinqOnCsvFile.cs"/>
    /// <para>Speichern des Inhalts einer <see cref="DataTable"/> als CSV-Datei und Einlesen von Daten einer CSV-Datei in
    /// eine <see cref="DataTable"/>:</para>
    /// <code language="cs" source="..\Examples\CsvToDataTable.cs"/>
    /// <para>Deserialisieren beliebiger Objekte aus CSV-Dateien:</para>
    /// <code language="cs" source="..\Examples\DeserializingClassesFromCsv.cs"/>
    /// </example>
    public sealed class CsvReader : IDisposable
    {
        private readonly CsvStringReader _reader;
        private readonly CsvOptions _options;
        private readonly bool _hasHeaderRow;
        private bool _firstRun = true;

        #region ctors

        /// <summary>
        /// Initialisiert ein neues <see cref="CsvReader"/>-Objekt.
        /// </summary>
        /// <param name="fileName">Dateipfad der CSV-Datei.</param>
        /// <param name="hasHeaderRow"><c>true</c>, wenn die CSV-Datei eine Kopfzeile mit den Spaltennamen hat.</param>
        /// <param name="options">Optionen für das Lesen der CSV-Datei.</param>
        /// <param name="fieldSeparator">Das Feldtrennzeichen, das in der CSV-Datei Verwendung findet.</param>
        /// <param name="textEncoding">Die zum Einlesen der CSV-Datei zu verwendende Textenkodierung oder <c>null</c> für <see cref="Encoding.UTF8"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> ist kein gültiger Dateipfad.</exception>
        /// <exception cref="IOException">Es kann nicht auf den Datenträger zugegriffen werden.</exception>
        public CsvReader(
            string fileName,
            bool hasHeaderRow = true,
            CsvOptions options = CsvOptions.Default,
            char fieldSeparator = ',',
            Encoding? textEncoding = null)
        {
            StreamReader streamReader = InitializeStreamReader(fileName, textEncoding);

            this._reader = new CsvStringReader(streamReader, fieldSeparator, (_options & CsvOptions.ThrowOnEmptyLines) != CsvOptions.ThrowOnEmptyLines);
            this._options = options;
            this._hasHeaderRow = hasHeaderRow;
        }


        /// <summary>
        /// Initialisiert ein neues <see cref="CsvReader"/>-Objekt.
        /// </summary>
        /// <param name="reader">Der <see cref="TextReader"/>, mit dem die CSV-Datei gelesen wird.</param>
        /// <param name="hasHeaderRow"><c>true</c>, wenn die CSV-Datei eine Kopfzeile mit den Spaltennamen hat.</param>
        /// <param name="options">Optionen für das Lesen der CSV-Datei.</param>
        /// <param name="fieldSeparator">Das Feldtrennzeichen.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> ist <c>null</c>.</exception>
        public CsvReader(
            TextReader reader,
            bool hasHeaderRow = true,
            CsvOptions options = CsvOptions.Default,
            char fieldSeparator = ',')
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            this._reader = new CsvStringReader(reader, fieldSeparator, (_options & CsvOptions.ThrowOnEmptyLines) != CsvOptions.ThrowOnEmptyLines);
            this._options = options;
            this._hasHeaderRow = hasHeaderRow;
        }

        #endregion


        #region public Methods

        /// <summary>
        /// Gibt ein <see cref="IEnumerable{T}">IEnumerable&lt;CsvRecord&gt;</see>-Objekt zurück, mit dem über die Datensätze der CSV-Datei
        /// iteriert werden kann.
        /// </summary>
        /// <returns>Ein <see cref="IEnumerable{T}">IEnumerable&lt;CsvRecord&gt;</see>, mit dem über die Datensätze der CSV-Datei
        /// iteriert werden kann.</returns>
        /// <exception cref="InvalidOperationException">Die Methode wurde mehr als einmal aufgerufen.</exception>
        /// <exception cref="ObjectDisposedException">Der <see cref="Stream"/> war bereits geschlossen.</exception>
        /// <exception cref="IOException">Fehler beim Zugriff auf den Datenträger.</exception>
        /// <exception cref="InvalidCsvException">Ungültige CSV-Datei. Die Interpretation ist abhängig vom <see cref="CsvOptions"/>-Wert,
        /// der im Konstruktor angegeben wurde.</exception>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public IEnumerable<CsvRecord> Read()
        {
            if (!_firstRun)
            {
                ThrowInvalidOperationException();
            }

            _firstRun = false;

            return new CsvRecordCollection(this);
        }


        /// <summary>
        /// Gibt die Resourcen frei. (Schließt den <see cref="TextReader"/>.)
        /// </summary>
        public void Dispose() => _reader.Dispose();

        #endregion


        #region internal

        internal IEnumerator<CsvRecord> GetEnumerator()
        {
            CsvRecord? record = null; // Schablone für weitere CsvRecord-Objekte
            CsvRecord? clone = null;

            foreach (IEnumerable<string?> row in _reader)
            {
                if (record is null)
                {
                    bool caseSensitiveColumns = (_options & CsvOptions.CaseSensitiveKeys) == CsvOptions.CaseSensitiveKeys;

                    if (_hasHeaderRow)
                    {
                        record = new CsvRecord(row.ToArray(),
                            caseSensitiveColumns,
                            (_options & CsvOptions.TrimColumns) == CsvOptions.TrimColumns,
                            false, false);
                        continue;
                    }
                    else
                    {
                        string?[]? arr = row.ToArray();

                        if (arr.Length == 0)
                        {
                            continue; // Leerzeile am Anfang
                        }

                        record = new CsvRecord(arr.Length, caseSensitiveColumns, false);
                        clone = new CsvRecord(record);
                        Fill(clone, arr, _reader);
                    }
                }
                else
                {
                    if ((_options & CsvOptions.DisableCaching) == CsvOptions.DisableCaching)
                    {
                        clone ??= new CsvRecord(record);
                    }
                    else
                    {
                        clone = new CsvRecord(record);
                    }
                    
                    Fill(clone, row, _reader);
                }

                yield return clone;
            }

            /////////////////////////////////////////////////////////////////////

            void Fill(CsvRecord clone, IEnumerable<string?> data, CsvStringReader reader)
            {
                int dataIndex = 0;

                foreach (string? item in data)
                {
                    if (dataIndex >= clone.Count)
                    {
                        if ((_options & CsvOptions.ThrowOnTooMuchFields) == CsvOptions.ThrowOnTooMuchFields)
                        {
                            throw new InvalidCsvException("Too much fields in a record.", reader.LineNumber, reader.LineIndex);
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (item != null && (_options & CsvOptions.TrimColumns) == CsvOptions.TrimColumns)
                    {
                        string? trimmed = item.Trim();

                        if (trimmed.Length == 0)
                        {
                            trimmed = null;
                        }

                        clone[dataIndex++] = trimmed;
                    }
                    else
                    {
                        clone[dataIndex++] = item;
                    }
                }


                if (dataIndex < clone.Count)
                {
                    if (dataIndex == 0 && (_options & CsvOptions.ThrowOnEmptyLines) == CsvOptions.ThrowOnEmptyLines)
                    {
                        throw new InvalidCsvException("Unmasked empty line.", reader.LineNumber, 0);
                    }

                    if ((_options & CsvOptions.ThrowOnTooFewFields) == CsvOptions.ThrowOnTooFewFields)
                    {
                        throw new InvalidCsvException("Too few fields in a record.", reader.LineNumber, reader.LineIndex);
                    }

                    if ((_options & CsvOptions.DisableCaching) == CsvOptions.DisableCaching)
                    {
                        for (int i = dataIndex; i < clone.Count; i++)
                        {
                            clone[i] = null;
                        }
                    }
                }
            }// Fill()

        }// GetEnumerator()


        /// <summary>
        /// Initialisiert einen <see cref="StreamReader"/>.
        /// </summary>
        /// <param name="fileName">Dateipfad.</param>
        /// <param name="textEncoding">Die zum Einlesen der CSV-Datei zu verwendende Textenkodierung oder <c>null</c> für <see cref="Encoding.UTF8"/>.</param>
        /// <returns>Ein <see cref="StreamReader"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/> ist kein gültiger Dateipfad.</exception>
        /// <exception cref="IOException">Es kann nicht auf den Datenträger zugegriffen werden.</exception>
        [ExcludeFromCodeCoverage]
        internal static StreamReader InitializeStreamReader(string fileName, Encoding? textEncoding)
        {
            try
            {
                return new StreamReader(fileName, textEncoding ?? Encoding.UTF8, true);
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

        #endregion


        #region private

        [DoesNotReturn]
        private static void ThrowInvalidOperationException() => throw new InvalidOperationException(Res.NotTwice);

        #endregion
    }
}
