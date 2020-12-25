using System;
using System.Runtime.Serialization;

namespace FolkerKinzel.CsvTools
{
    /// <summary>
    /// Ausnahmen, die beim Lesen von Csv-Dokumenten durch ungültige CSV-Dokumente
    /// ausgelöst werden.
    /// </summary>
    [Serializable]
    public class InvalidCsvException : Exception, ISerializable
    {
        /// <summary>
        /// Zeilennummer der CSV-Datei beim Auftreten des Fehlers (1-basierter Index).
        /// </summary>
        public int CsvLineNumber { get; }

        /// <summary>
        /// Zeichen in der Zeile der CSV-Datei, bei dem der Fehler auftrat (0-basierter Index).
        /// </summary>
        public int CsvCharIndex { get; }


        /// <summary>
        /// Initialisiert ein neues <see cref="InvalidCsvException"/>-Objekt mit Standardwerten.
        /// </summary>
        public InvalidCsvException()
        {
        }

        /// <summary>
        /// Initialisiert ein neues <see cref="InvalidCsvException"/>-Objekt mit einer Fehlermeldung.
        /// </summary>
        /// <param name="message">Eine Textmeldung, die den Fehler beschreibt.</param>
        public InvalidCsvException(string message) : base(message)
        {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Eine Textmeldung, die den Fehler beschreibt.</param>
        /// <param name="csvLineNumber">Zeilennummer der CSV-Datei beim Auftreten des Fehlers (1-basierter Index).</param>
        /// <param name="csvCharIndex">Zeichen in der Zeile der CSV-Datei, bei dem der Fehler auftrat (0-basierter Index).</param>
        public InvalidCsvException(string message, int csvLineNumber, int csvCharIndex) : base(message)
        {
            this.CsvLineNumber = csvLineNumber;
            this.CsvCharIndex = csvCharIndex;
        }


        /// <summary>
        /// Initialisiert ein neues <see cref="InvalidCsvException"/>-Objekt.
        /// </summary>
        /// <param name="message">Eine Textmeldung, die den Fehler beschreibt.</param>
        /// <param name="innerException">Die Ausnahme, die die gegenwärtige Ausnahme ausgelöst hat.
        /// Wenn <paramref name="innerException"/> ungleich <c>null</c> ist, wurde die gegenwärtige Ausnahme in einem catch-Block
        /// ausgelöst, in dem <paramref name="innerException"/> behandelt wurde.</param>
        public InvalidCsvException(string message, Exception innerException) : base(message, innerException) { }


        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="InvalidCsvException"/>-Klasse mit serialisierten Daten.
        /// </summary>
        /// <param name="serializationInfo">Die <see cref="SerializationInfo"/>, die die serialisierten Objektdaten für die ausgelöste Ausnahme enthält.</param>
        /// <param name="streamingContext">Der <see cref="StreamingContext"/>, der die Kontextinformationen über die Quelle oder das Ziel enthält.</param>
        protected InvalidCsvException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
            this.CsvLineNumber = serializationInfo.GetInt32(nameof(CsvLineNumber));
            this.CsvCharIndex = serializationInfo.GetInt32(nameof(CsvCharIndex));
        }


        /// <summary>
        /// Serialisiert die Daten des Objekts.
        /// </summary>
        /// <param name="info">Die <see cref="SerializationInfo"/>, die die serialisierten Objektdaten für die ausgelöste Ausnahme enthält.</param>
        /// <param name="context">Der <see cref="StreamingContext"/>, der die Kontextinformationen über die Quelle oder das Ziel enthält.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(CsvLineNumber), CsvLineNumber);
            info.AddValue(nameof(CsvCharIndex), CsvCharIndex);
        }


        /// <summary>
        /// Erzeugt eine String-Repräsentation des <see cref="InvalidCsvException"/>-Objekts.
        /// </summary>
        /// <returns>Eine String-Repräsentation des <see cref="InvalidCsvException"/>-Objekts.</returns>
        public override string ToString()
        {
            return base.ToString() + $" ({nameof(CsvLineNumber)}: {CsvLineNumber}, {nameof(CsvCharIndex)}: {CsvCharIndex})";
        }

    }
}
