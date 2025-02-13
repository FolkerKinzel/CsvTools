using System.Runtime.Serialization;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

/// <summary>Exception, thrown parsing invalid CSV documents.</summary>
[Serializable]
public sealed class CsvFormatException : Exception, ISerializable
{
    /// <summary>Line number of the CSV file where the error occurred (1-based index).
    /// </summary>
    public int CsvLineNumber { get; } = 1;

    /// <summary>Index of the character in the line of the CSV file where the error
    /// occurred (Zero-based index).</summary>
    public int CsvCharIndex { get; }

    internal CsvError Error { get; }

    /// <summary>Initializes a new instance of the <see cref="CsvFormatException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="error">The error that occurred.</param>
    /// <param name="csvLineNumber">Line number of the CSV file where the error occurred
    /// (1-based index).</param>
    /// <param name="csvCharIndex">Index of the character in the line of the CSV file
    /// where the error occurred (0-based index).</param>
    internal CsvFormatException(string message,
                                CsvError error,
                                int csvLineNumber,
                                int csvCharIndex) : base(message)
    {
        Error = error;
        CsvLineNumber = csvLineNumber;
        CsvCharIndex = csvCharIndex;
    }

    /// <inheritdoc />
    public override string ToString() =>
         $"{base.ToString()} ({nameof(CsvLineNumber)}: {CsvLineNumber}, {nameof(CsvCharIndex)}: {CsvCharIndex})";
}
