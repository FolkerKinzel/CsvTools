using System;
using System.Runtime.Serialization;

namespace FolkerKinzel.CsvTools
{
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


#pragma warning disable 1573
        /// <inheritdoc cref="InvalidCsvException(string)" />
        /// <param name="csvLineNumber">Zeilennummer der CSV-Datei beim Auftreten des Fehlers (1-basierter Index).</param>
        /// <param name="csvCharIndex">Zeichen in der Zeile der CSV-Datei, bei dem der Fehler auftrat (0-basierter Index).</param>
        public InvalidCsvException(string message, int csvLineNumber, int csvCharIndex) : base(message)
        {
            this.CsvLineNumber = csvLineNumber;
            this.CsvCharIndex = csvCharIndex;
        }
#pragma warning restore 1573


        
        /// <inheritdoc/>
        public InvalidCsvException(string message, Exception innerException) : base(message, innerException) { }


        
        /// <inheritdoc/>
        protected InvalidCsvException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.CsvLineNumber = info.GetInt32(nameof(CsvLineNumber));
            this.CsvCharIndex = info.GetInt32(nameof(CsvCharIndex));
        }


        
        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(CsvLineNumber), CsvLineNumber);
            info.AddValue(nameof(CsvCharIndex), CsvCharIndex);
        }



        /// <inheritdoc/>
        public override string ToString() =>
            base.ToString() + $" ({nameof(CsvLineNumber)}: {CsvLineNumber}, {nameof(CsvCharIndex)}: {CsvCharIndex})";

    }
}
