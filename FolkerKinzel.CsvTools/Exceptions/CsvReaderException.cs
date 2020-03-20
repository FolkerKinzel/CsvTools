using System;
using System.Runtime.Serialization;

namespace FolkerKinzel.Csv.Exceptions
{

    /// <summary>
    /// Ausnahmen, die beim Lesen von Csv-Dateien auftreten können
    /// </summary>
    [Serializable]
    public class CsvReaderException : CsvIOException, ISerializable
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public CsvReaderException() { }


        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Eine Textmeldung, die den Fehler beschreibt.</param>
        public CsvReaderException(string message) : base(message) { }


        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Eine Textmeldung, die den Fehler beschreibt.</param>
        /// <param name="innerException">Die Ausnahme, die die gegenwärtige Ausnahme ausgelöst hat.
        /// Wenn innerException ungleich null ist, wurde die gegenwärtige Ausnahme in einem catch-Block
        /// ausgelöst, in dem die innerException behandelt wurde.</param>
        public CsvReaderException(string message, Exception innerException) : base(message, innerException) { }


        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="CsvReaderException"/>-Klasse mit serialisierten Daten.
        /// </summary>
        /// <param name="serializationInfo">Die <see cref="SerializationInfo"/>, die die serialisierten Objektdaten für die ausgelöste Ausnahme enthält.</param>
        /// <param name="streamingContext">Der <see cref="StreamingContext"/>, der die Kontextinformationen über die Quelle oder das Ziel enthält.</param>
        protected CsvReaderException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {

        }
    }
}
