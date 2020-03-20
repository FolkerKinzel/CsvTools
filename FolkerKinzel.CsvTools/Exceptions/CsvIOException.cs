using System;
using System.Runtime.Serialization;

namespace FolkerKinzel.Csv.Exceptions
{

    /// <summary>
    /// Basisklasse aller Exceptions im namespace FK.Csv
    /// </summary>
    [Serializable]
    public class CsvIOException : Exception, ISerializable
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public CsvIOException() { }


        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Eine Textmeldung, die den Fehler beschreibt.</param>
        public CsvIOException(string message) : base(message) { }


        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="message">Eine Textmeldung, die den Fehler beschreibt.</param>
        /// <param name="innerException">Die Ausnahme, die die gegenwärtige Ausnahme ausgelöst hat.
        /// Wenn innerException ungleich null ist, wurde die gegenwärtige Ausnahme in einem catch-Block
        /// ausgelöst, in dem die innerException behandelt wurde.</param>
        public CsvIOException(string message, Exception innerException) : base(message, innerException) { }


        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="CsvIOException"/>-Klasse mit serialisierten Daten.
        /// </summary>
        /// <param name="serializationInfo">Die <see cref="SerializationInfo"/>, die die serialisierten Objektdaten für die ausgelöste Ausnahme enthält.</param>
        /// <param name="streamingContext">Der <see cref="StreamingContext"/>, der die Kontextinformationen über die Quelle oder das Ziel enthält.</param>
        protected CsvIOException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {

        }
    }

}
