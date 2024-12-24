using System.Runtime.Serialization;

namespace FolkerKinzel.CsvTools;

/// <summary>
/// Ausnahmen, die beim Lesen von CSV-Dokumenten durch ungültige CSV-Dokumente
/// ausgelöst werden.
/// </summary>
[Serializable]
public class InvalidCsvException : Exception, ISerializable
{
    /// <summary>
    /// Zeilennummer der CSV-Datei beim Auftreten des Fehlers (1-basierter Index).
    /// </summary>
    public int CsvLineNumber { get; } = 1;

    /// <summary>
    /// Zeichen in der Zeile der CSV-Datei, bei dem der Fehler auftrat (0-basierter Index).
    /// </summary>
    public int CsvCharIndex { get; }



    /// <inheritdoc/>
    public InvalidCsvException()
    {
    }



    ///<inheritdoc/>
    public InvalidCsvException(string message) : base(message)
    {
    }


    /// <inheritdoc cref="InvalidCsvException(string)" />
    /// <param name="csvLineNumber">Zeilennummer der CSV-Datei beim Auftreten des Fehlers (1-basierter Index).</param>
    /// <param name="csvCharIndex">Zeichen in der Zeile der CSV-Datei, bei dem der Fehler auftrat (0-basierter Index).</param>
#pragma warning disable CS1573 // Parameter besitzt kein übereinstimmendes param-Tag im XML-Kommentar (andere Parameter jedoch schon)
    public InvalidCsvException(string message, int csvLineNumber, int csvCharIndex) : base(message)
#pragma warning restore CS1573 // Parameter besitzt kein übereinstimmendes param-Tag im XML-Kommentar (andere Parameter jedoch schon)
    {
        this.CsvLineNumber = csvLineNumber;
        this.CsvCharIndex = csvCharIndex;
    }



    /// <inheritdoc/>
    public InvalidCsvException(string message, Exception innerException) : base(message, innerException) { }



    ///// <inheritdoc/>
    //protected InvalidCsvException(SerializationInfo info, StreamingContext context) : base(info, context)
    //{
    //    this.CsvLineNumber = info.GetInt32(nameof(CsvLineNumber));
    //    this.CsvCharIndex = info.GetInt32(nameof(CsvCharIndex));
    //}



    ///// <inheritdoc/>
    //[Obsolete("This ctor is deprecated. Use the ctor that has the parameters message, csvLineNumber and csvCharIndex instead.")]
    //public override void GetObjectData(SerializationInfo info, StreamingContext context)
    //{
    //    base.GetObjectData(info, context);

    //    info.AddValue(nameof(CsvLineNumber), CsvLineNumber);
    //    info.AddValue(nameof(CsvCharIndex), CsvCharIndex);
    //}



    /// <inheritdoc/>
    public override string ToString() =>
        base.ToString() + $" ({nameof(CsvLineNumber)}: {CsvLineNumber}, {nameof(CsvCharIndex)}: {CsvCharIndex})";

}
