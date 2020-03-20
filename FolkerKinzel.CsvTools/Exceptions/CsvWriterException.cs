using System;
using System.Runtime.Serialization;

namespace FolkerKinzel.Csv.Exceptions
{
    /// <summary>
    /// Ausnahmen, die beim Schreiben von Csv-Dokumenten auftreten können.
    /// </summary>
    [Serializable]
    public class CsvWriterException : CsvIOException, ISerializable
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public CsvWriterException() { }


        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Eine Textmeldung, die den Fehler beschreibt.</param>
        public CsvWriterException(string message) : base(message) { }


        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Eine Textmeldung, die den Fehler beschreibt.</param>
        /// <param name="innerException">Die Ausnahme, die die gegenwärtige Ausnahme ausgelöst hat.
        /// Wenn innerException ungleich null ist, wurde die gegenwärtige Ausnahme in einem catch-Block
        /// ausgelöst, in dem die innerException behandelt wurde.</param>
        public CsvWriterException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="CsvWriterException"/>-Klasse mit serialisierten Daten.
        /// </summary>
        /// <param name="serializationInfo">Die <see cref="SerializationInfo"/>, die die serialisierten Objektdaten für die ausgelöste Ausnahme enthält.</param>
        /// <param name="streamingContext">Der <see cref="StreamingContext"/>, der die Kontextinformationen über die Quelle oder das Ziel enthält.</param>
        protected CsvWriterException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {

        }
    }

}