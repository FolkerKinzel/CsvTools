using FolkerKinzel.CsvTools.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FolkerKinzel.CsvTools
{
    /// <summary>
    /// Benannte Konstanten, um Optionen für das Lesen und Schreiben von CSV-Dateien anzugeben. Die Flags
    /// können kombiniert werden.
    /// </summary>
    /// <note type="tip">
    /// Verwenden Sie zum sicheren und bequemen Arbeiten mit der <see cref="CsvOptions"/>-Enum  die Erweiterungsmethoden der 
    /// Klasse <see cref="CsvOptionsExtensions"/>.
    /// </note>
    [Flags]
    public enum CsvOptions
    {
        /// <summary>
        /// Kein Flag ist gesetzt. Das erzeugt einen sehr nachsichtigen Parser, der kaum Ausnahmen wirft.
        /// </summary>
        None = 0,

        /// <summary>
        /// Wenn gesetzt, wirft <see cref="CsvReader"/> eine <see cref="InvalidCsvException"/>, wenn in einer Datenzeile mehr
        /// Felder enthalten sind, als in der ersten Datenzeile.
        /// <note>
        /// Wenn in einer Datenzeile mehr
        /// Felder enthalten sind, als in der ersten Datenzeile, ist das ein starkes Indiz für einen Lesefehler. Das Flag sollte
        /// deshalb i.d.R. gesetzt sein.
        /// </note>
        /// </summary>
        ThrowOnTooMuchFields = 1,


        /// <summary>
        /// Wenn gesetzt, wirft <see cref="CsvReader"/> eine <see cref="InvalidCsvException"/>, wenn in einer Datenzeile weniger
        /// Felder enthalten sind, als in der ersten Datenzeile.
        /// <note>
        /// Andere Sotware könnte darauf verzichten, leere Felder am Zeilenende durch Feldtrennzeichen zu markieren. Das Fehlen
        /// von Feldern am Zeilenende kann aber auch ein Indiz für einen Lesefehler sein.
        /// </note>
        /// </summary>
        ThrowOnTooFewFields = 1 << 1,

        /// <summary>
        /// Wenn gesetzt, wirft <see cref="CsvReader"/> eine <see cref="InvalidCsvException"/>, wenn in der CSV-Datei Leerzeilen
        /// vorkommen, die nicht Teil eines mit Gänsefüßchen maskierten Datenfeldes sind.
        /// <note>
        /// Leerzeilen, die nicht Teil eines maskierten Feldes sind, können in keinem Fall als Teil der zu lesenden Daten interpretiert
        /// werden. Sie können aber durch Entfernen des Fags erreichen, dass <see cref="CsvReader"/> solche Leerzeilen ignoriert.
        /// </note>
        /// </summary>
        ThrowOnEmptyLines = 1 << 2,

        /// <summary>
        /// Wenn das Flag nicht gesetzt ist, werden von der Klasse <see cref="CsvRecord"/> die Spaltennamen der CSV-Datei, die zum Zugriff
        /// auf die Daten dienen, nicht case-sensitiv interpretiert. Das ist dasselbe Verhalten, das <see cref="DataColumnCollection"/>
        /// zeigt.
        /// </summary>
        CaseSensitiveKeys = 1 << 3,

        /// <summary>
        /// Wenn das Flag gesetzt ist, werden von <see cref="CsvReader"/> und <see cref="CsvWriter"/> sämtliche Daten-Tokens und auch die 
        /// Spaltennamen mit der Methode <see cref="string.Trim()"/> behandelt. Das bedeutet einen starken Performanceverlust und verändert
        /// auch Daten, bei denen der vor- und nachgestellte Leerraum eine Bedeutung hat. Setzen Sie das Flag nur, um nicht standardkonforme
        /// CSV-Dateien zu lesen, die zusätzliches Padding einführen.
        /// </summary>
        TrimColumns = 1 << 4,

        /// <summary>
        /// Standardeinstellung. Dies ist ein kombinierter Wert, der <see cref="CsvReader"/> zum Werfen einer <see cref="InvalidCsvException"/> zwingt,
        /// wenn die zu lesende Datei nicht dem Standard RFC 4180 entspricht. (Abweichende Spaltentrennzeichen und Zeilenwechselzeichen werden immer toleriert.)
        /// </summary>
        Default = ThrowOnTooMuchFields | ThrowOnTooFewFields | ThrowOnEmptyLines,


        /// <summary>
        /// Wenn das Flag gesetzt wird, wird beim Lesen der CSV-Datei für jede Datenzeile dasselbe <see cref="CsvRecord"/>-Objekt verwendet (gefüllt mit neuen Daten).
        /// Das bringt bei sehr großen
        /// CSV-Dateien leichte Performancevorteile, macht es aber unmöglich, auf <see cref="CsvReader.Read"/> eine
        /// Linq-Abfrage durchzuführen.
        /// </summary>
        DisableCaching = 1 << 5
    }
}
