using System.Runtime.Serialization;

namespace FolkerKinzel.CsvTools;

/// <summary>Exception, thrown parsing invalid CSV documents.</summary>
[Serializable]
public class InvalidCsvException : Exception, ISerializable
{
    /// <summary>Line number of the CSV file where the error occurred (1-based index).</summary>
    public int CsvLineNumber { get; } = 1;

    /// <summary>Index of the character in the line of the CSV file where the error
    /// occurred (Zero-based index).</summary>
    public int CsvCharIndex { get; }

    /// <inheritdoc />
    public InvalidCsvException() { }

    /// <inheritdoc />
    public InvalidCsvException(string message) : base(message) { }

    /// <inheritdoc cref="InvalidCsvException(string)" />
    /// <param name="csvLineNumber">Line number of the CSV file where the error occurred
    /// (1-based index).</param>
    /// <param name="csvCharIndex">Index of the character in the line of the CSV file
    /// where the error occurred (0-based index).</param>
#pragma warning disable CS1573 // Parameter besitzt kein übereinstimmendes param-Tag im XML-Kommentar (andere Parameter jedoch schon)
    public InvalidCsvException(string message, int csvLineNumber, int csvCharIndex) : base(message)
#pragma warning restore CS1573 // Parameter besitzt kein übereinstimmendes param-Tag im XML-Kommentar (andere Parameter jedoch schon)
    {
        this.CsvLineNumber = csvLineNumber;
        this.CsvCharIndex = csvCharIndex;
    }

    /// <inheritdoc />
    public InvalidCsvException(string message, Exception innerException) : base(message, innerException) { }

    /// <inheritdoc />
    public override string ToString() =>
        base.ToString() + $" ({nameof(CsvLineNumber)}: {CsvLineNumber}, {nameof(CsvCharIndex)}: {CsvCharIndex})";
}
