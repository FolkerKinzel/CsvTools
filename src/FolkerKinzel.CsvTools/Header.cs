namespace FolkerKinzel.CsvTools;

/// <summary>
/// Named constants to describe the presence of the header row of a CSV file.
/// </summary>
public enum Header
{
    /// <summary>
    /// Indicates that the CSV file has no header row. (This information can 
    /// be taken from the <c>header</c> parameter of the <c>text/csv</c> MIME type.)
    /// </summary>
    Absent = -1,

    /// <summary>
    /// Indicates that the <see cref="CsvAnalyzer"/> should take into account that the 
    /// first row of the CSV file could be a header row.
    /// </summary>
    ProbablyPresent = 0,

    /// <summary>
    /// Indicates that the first row of the CSV file should be treated as a header row. (This information can 
    /// be taken from the <c>header</c> parameter of the <c>text/csv</c> MIME type.)
    /// </summary>
    Present = 1
}
