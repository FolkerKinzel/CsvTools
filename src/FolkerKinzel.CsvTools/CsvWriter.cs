using FolkerKinzel.CsvTools.TypeConversions;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.CsvTools;

/// <summary>
/// Die Klasse ermöglicht es, Daten in eine CSV-Datei zu schreiben.
/// </summary>
/// <remarks><see cref="CsvWriter"/> stellt in der Eigenschaft <see cref="Record"/> ein <see cref="CsvRecord"/>-Objekt zur Verfügung, das einen
/// Puffer für einen Datensatz (Zeile) der CSV-Datei repräsentiert. Füllen Sie das <see cref="CsvRecord"/>-Objekt mit <see cref="string"/>-Daten und schreiben
/// Sie es anschließend mit der Methode <see cref="WriteRecord"/> in die Datei. Der Aufruf von <see cref="WriteRecord"/> setzt alle Felder von
/// <see cref="Record"/> wieder auf <c>null</c>-Werte, so dass das <see cref="CsvRecord"/>-Objekt erneut befüllt werden kann. Wenn andere Datentypen als <see cref="string"/>
/// geschrieben werden sollen, bietet sich die Verwendung der Klasse <see cref="CsvRecordWrapper"/> an, die einen komfortablen Adapter zwischen den
/// Daten der Anwendung und der CSV-Datei darstellt.</remarks>
/// <example>
/// <note type="note">In den folgenden Code-Beispielen wurde - der leichteren Lesbarkeit wegen - auf Ausnahmebehandlung verzichtet.</note>
/// <para>Speichern des Inhalts einer <see cref="DataTable"/> als CSV-Datei und Einlesen von Daten einer CSV-Datei in
/// eine <see cref="DataTable"/>:</para>
/// <code language="cs" source="..\Examples\CsvToDataTable.cs"/>
/// </example>
public sealed class CsvWriter : IDisposable
{
    /// <summary>
    /// Das beim Schreiben von CSV-Dateien zu verwendende Newline-Zeichen ("\r\n").
    /// </summary>
    public const string NewLine = "\r\n";

    private bool _isHeaderRowWritten;

    private readonly char _fieldSeparator;
    private readonly bool _trimColumns;

    //[NotNull]
    private readonly TextWriter _writer;


    #region ctors


    /// <summary>
    /// Initialisiert ein neues <see cref="CsvWriter"/>-Objekt mit den Spaltennamen für die zu schreibende Kopfzeile.
    /// </summary>
    /// <param name="fileName">Der Dateipfad der zu schreibenden CSV-Datei. Wenn die Datei existiert, wird sie überschrieben.</param>
    /// <param name="columnNames">Ein Array von Spaltennamen für die zu schreibende Kopfzeile. Wenn das Array <c>null</c>-Werte
    /// enthält, werden diese durch automatisch erzeugte Spaltennamen ersetzt. Spaltennamen dürfen nicht doppelt vorkommen. Dabei ist zu
    /// beachten, dass der Vergleich der Spaltennamen nicht case-sensitiv erfolgt - es sei denn, dass diese Option in <paramref name="options"/> ausdrücklich
    /// gewählt wurde.</param>
    /// <param name="options">Optionen für die zu schreibende CSV-Datei.</param>
    /// <param name="textEncoding">Die zu verwendende Textkodierung oder <c>null</c> für UTF-8 mit BOM. (<see cref="Encoding.UTF8"/>)</param>
    /// <param name="fieldSeparator">Das in der CSV-Datei zu verwendende Feldtrennzeichen.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para><paramref name="fileName"/> ist kein gültiger Dateipfad</para>
    /// <para>- oder -</para>
    /// <para>ein Spaltenname in <paramref name="columnNames"/> kommt doppelt vor. In <paramref name="options"/> kann
    /// gewählt werden, ob der Vergleich der Spaltennamen case-sensitiv erfolgt.</para></exception>
    /// <exception cref="IOException">E/A-Fehler.</exception>
    public CsvWriter(
        string fileName, string[] columnNames, CsvOptions options = CsvOptions.Default, Encoding? textEncoding = null, char fieldSeparator = ',')
         : this(columnNames, fieldSeparator, options) => _writer = InitStreamWriter(fileName, textEncoding);


    /// <summary>
    /// Initialisiert ein neues <see cref="CsvWriter"/>-Objekt, mit dem eine CSV-Datei ohne Kopfzeile geschrieben wird.
    /// </summary>
    /// <param name="fileName">Der Dateipfad der zu schreibenden CSV-Datei. Wenn die Datei existiert, wird sie überschrieben.</param>
    /// <param name="columnsCount">Anzahl der Spalten in der CSV-Datei.</param>
    /// <param name="options">Optionen für die zu schreibende CSV-Datei.</param>
    /// <param name="textEncoding">Die zu verwendende Textkodierung oder <c>null</c> für UTF-8 mit BOM.</param>
    /// <param name="fieldSeparator">Das in der CSV-Datei zu verwendende Feldtrennzeichen.</param>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> ist kein gültiger Dateipfad.</exception>
    /// <exception cref="IOException">E/A-Fehler.</exception>
    public CsvWriter(
        string fileName, int columnsCount, CsvOptions options = CsvOptions.Default, Encoding? textEncoding = null, char fieldSeparator = ',')
         : this(columnsCount, fieldSeparator, options) => _writer = InitStreamWriter(fileName, textEncoding);


    /// <summary>
    /// Initialisiert ein neues <see cref="CsvWriter"/>-Objekt, mit den Spaltennamen für die zu schreibende Kopfzeile.
    /// </summary>
    /// <param name="writer">Der <see cref="TextWriter"/>, mit dem geschrieben wird.</param>
    /// <param name="columnNames">Ein Array von Spaltennamen für die zu schreibende Kopfzeile. Wenn das Array <c>null</c>-Werte
    /// enthält, werden diese durch automatisch erzeugte Spaltennamen ersetzt. Spaltennamen dürfen nicht doppelt vorkommen. Dabei ist zu
    /// beachten, dass der Vergleich der Spaltennamen nicht case-sensitiv erfolgt - es sei denn, dass diese Option in <paramref name="options"/> ausdrücklich
    /// gewählt wurde.</param>
    /// <param name="options">Optionen für die zu schreibende CSV-Datei.</param>
    /// <param name="fieldSeparator">Das in der CSV-Datei zu verwendende Feldtrennzeichen.</param>
    /// <exception cref="ArgumentNullException"><paramref name="writer"/> oder <paramref name="columnNames"/> ist <c>null.</c></exception>
    /// <exception cref="ArgumentException">Ein Spaltenname in <paramref name="columnNames"/> kommt doppelt vor. In <paramref name="options"/> kann
    /// gewählt werden, ob der Vergleich case-sensitiv erfolgt.</exception>
    public CsvWriter(
        TextWriter writer, string[] columnNames, CsvOptions options = CsvOptions.Default, char fieldSeparator = ',')
        : this(columnNames, fieldSeparator, options)
    {
        if (writer is null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (columnNames is null)
        {
            throw new ArgumentNullException(nameof(columnNames));
        }

        this._writer = writer;
        writer.NewLine = NewLine;
    }


    /// <summary>
    /// Initialisiert ein neues <see cref="CsvWriter"/>-Objekt, mit dem eine CSV-Datei ohne Kopfzeile geschrieben wird.
    /// </summary>
    /// <param name="writer">Der <see cref="TextWriter"/>, mit dem geschrieben wird.</param>
    /// <param name="columnsCount">Anzahl der Spalten in der CSV-Datei.</param>
    /// <param name="options">Optionen für die zu schreibende CSV-Datei.</param>
    /// <param name="fieldSeparator">Das in der CSV-Datei zu verwendende Feldtrennzeichen.</param>
    /// <exception cref="ArgumentNullException"><paramref name="writer"/> ist <c>null.</c></exception>
    public CsvWriter(
        TextWriter writer, int columnsCount, CsvOptions options = CsvOptions.Default, char fieldSeparator = ',')
        : this(columnsCount, fieldSeparator, options)
    {
        if (writer is null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        this._writer = writer;
        writer.NewLine = NewLine;
    }


    /// <summary>
    /// Privater Konstruktor, der von den öffentlichen Konstruktoren aufgerufen wird, die ein <see cref="CsvWriter"/>-Objekt mit Spaltennamen 
    /// für die zu schreibende Kopfzeile initialisieren. (Diese Konstruktoren initialisieren auch <see cref="_writer"/>.)
    /// </summary>
    /// <param name="columnNames">Ein Array von Spaltennamen für die zu schreibende Kopfzeile. Wenn das Array <c>null</c>-Werte
    /// enthält, werden diese durch automatisch erzeugte Spaltennamen ersetzt. Spaltennamen dürfen nicht doppelt vorkommen. Dabei ist zu
    /// beachten, dass der Vergleich nicht case-sensitiv erfolgt - es sei denn, dass diese Option in <paramref name="options"/> ausdrücklich
    /// gewählt wurde.</param>
    /// <param name="fieldSeparator">Das in der CSV-Datei zu verwendende Feldtrennzeichen.</param>
    /// <param name="options">Optionen für die zu schreibende CSV-Datei.</param>
    /// <exception cref="ArgumentException">Ein Spaltenname in <paramref name="columnNames"/> kommt doppelt vor. In <paramref name="options"/> kann
    /// gewählt werden, ob der Vergleich case-sensitiv erfolgt.</exception>
#pragma warning disable CS8618 // Das Non-Nullable-Feld _writer ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".
    private CsvWriter(string?[] columnNames, char fieldSeparator, CsvOptions options)
#pragma warning restore CS8618 // Das Non-Nullable-Feld ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".
    {
        this._fieldSeparator = fieldSeparator;
        this._trimColumns = (options & CsvOptions.TrimColumns) == CsvOptions.TrimColumns;
        this.Record = new CsvRecord(
            columnNames,
            (options & CsvOptions.CaseSensitiveKeys) == CsvOptions.CaseSensitiveKeys,
            _trimColumns,
            true, true);
    }


    /// <summary>
    /// Privater Konstruktor, der von den öffentlichen Konstruktoren aufgerufen wird, die ein <see cref="CsvWriter"/>-Objekt initialisieren, mit dem
    /// eine CSV-Datei ohne Kopfzeile schreiben. (Diese Konstruktoren initialisieren auch <see cref="_writer"/>.)
    /// </summary>
    /// <param name="columnsCount">Anzahl der Spalten in der CSV-Datei.</param>
    /// <param name="fieldSeparator">Das in der CSV-Datei zu verwendende Feldtrennzeichen.</param>
    /// <param name="options">Optionen für die zu schreibende CSV-Datei.</param>
#pragma warning disable CS8618 // Das Non-Nullable-Feld _writer ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".
    private CsvWriter(int columnsCount, char fieldSeparator, CsvOptions options)
#pragma warning restore CS8618 // Das Non-Nullable-Feld ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".
    {
        this._isHeaderRowWritten = true;
        this._fieldSeparator = fieldSeparator;
        this._trimColumns = (options & CsvOptions.TrimColumns) == CsvOptions.TrimColumns;
        this.Record = new CsvRecord(
            columnsCount,
            (options & CsvOptions.CaseSensitiveKeys) == CsvOptions.CaseSensitiveKeys,
            true);

    }


    #endregion


    /// <summary>
    /// Der in die Datei zu schreibende Datensatz. Füllen Sie das <see cref="CsvRecord"/>-Objekt mit Daten und rufen Sie
    /// anschließend <see cref="WriteRecord"/> auf, um diese Daten in die Datei zu schreiben. <see cref="CsvWriter"/> leert
    /// <see cref="Record"/> nach jedem Schreibvorgang.
    /// </summary>
    public CsvRecord Record { get; }


    /// <summary>
    /// Schreibt den Inhalt von <see cref="Record"/> in die CSV-Datei und setzt anschließend alle Spalten von <see cref="Record"/> auf <c>null</c>. 
    /// (Beim ersten Aufruf wird ggf. auch die Kopfzeile geschrieben.)
    /// </summary>
    /// <exception cref="IOException">E/A-Fehler</exception>
    /// <exception cref="ObjectDisposedException">Die Resourcen waren bereits freigegeben.</exception>
    public void WriteRecord()
    {
#if NET40
        string fieldSeparatorString = new(_fieldSeparator, 1);
#endif

        int recordLength = Record.Count;

        if (!_isHeaderRowWritten)
        {
            ReadOnlyCollection<string>? columns = Record.ColumnNames;

            for (int i = 0; i < recordLength - 1; i++)
            {
                WriteField(columns[i]);
                _writer.Write(_fieldSeparator);
            }

            WriteField(columns[recordLength - 1]);
            _writer.WriteLine();

            _isHeaderRowWritten = true;
        }

        for (int j = 0; j < recordLength - 1; j++)
        {
            if (Record[j] is string s) // sonst null
            {
                WriteField(s);

                Record[j] = null;
            }

            _writer.Write(_fieldSeparator);
        }

        if (Record[recordLength - 1] is string lastString) // sonst null
        {
            WriteField(lastString);

            Record[recordLength - 1] = null;
        }
        _writer.WriteLine();

        /////////////////////////////////////////////

        void WriteField(string field)
        {
            if (_trimColumns)
            {
                field = field.Trim();
            }

            bool needsToBeQuoted = NeedsToBeQuoted(field);

            if (needsToBeQuoted)
            {
                _writer.Write('"');

                for (int j = 0; j < field.Length; j++)
                {
                    char c = field[j];

                    if (c == Environment.NewLine[0])
                    {
                        _writer.WriteLine();
                        j += Environment.NewLine.Length - 1;
                    }
                    else if (c == '\"')
                    {
                        _writer.Write('"');
                        _writer.Write(c);
                    }
                    else
                    {
                        _writer.Write(c);
                    }
                }

                _writer.Write('"');
            }
            else
            {
                _writer.Write(field);
            }
        }

#if NET40
            bool NeedsToBeQuoted(string s) => s.Contains(fieldSeparatorString) ||
                                                  s.Contains("\"") ||
                                                  s.Contains(Environment.NewLine); 
#else
        bool NeedsToBeQuoted(string s) => s.Contains(_fieldSeparator, StringComparison.Ordinal) ||
                                                  s.Contains('"', StringComparison.Ordinal) ||
                                                  s.Contains(Environment.NewLine, StringComparison.Ordinal);
#endif

    }


    /// <summary>
    /// Gibt die Ressourcen frei. (Schließt den <see cref="TextWriter"/>.)
    /// </summary>
    public void Dispose() => _writer.Dispose();


    #region private

    /// <summary>
    /// Initialisiert einen <see cref="StreamWriter"/> mit der angegebenen Textkodierung mit
    /// dem Namen der zu schreibenden Datei.
    /// </summary>
    /// <param name="fileName">Dateipfad.</param>
    /// <param name="textEncoding">Textkodierung oder <c>null</c> für UTF-8 mit BOM.</param>
    /// <returns><see cref="StreamWriter"/></returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> ist kein gültiger Dateipfad.</exception>
    /// <exception cref="IOException">E/A-Fehler.</exception>
    [ExcludeFromCodeCoverage]
    private static StreamWriter InitStreamWriter(string fileName, Encoding? textEncoding)
    {
        try
        {
            return new StreamWriter(fileName, false, textEncoding ?? Encoding.UTF8) // UTF-8-Encoding mit BOM
            {
                NewLine = CsvWriter.NewLine
            };
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
}
