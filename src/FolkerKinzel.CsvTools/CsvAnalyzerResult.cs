using System.Runtime.InteropServices;

namespace FolkerKinzel.CsvTools;

/// <summary>
/// Result of the analysis of a CSV file.
/// </summary>
public class CsvAnalyzerResult
{
    /// <summary>The column names of the CSV file, or <c>null</c> if the
    /// CSV file has no header row.</summary>
    public IReadOnlyList<string>? ColumnNames { get; internal set; }

    /// <summary>Options for reading the CSV file.</summary>
    public CsvOpts Options { get; internal set; } = CsvOpts.Default;

    /// <summary>The field separator character.</summary>
    public char Delimiter { get; internal set; } = ',';

    /// <summary>Gets a value that indicates whether the CSV file has a header row.</summary>
    /// <value><c>true</c> if the CSV file has a header with column names,
    /// otherwise <c>false</c>.</value>
    public bool HeaderPresent => ColumnNames != null;

    /// <summary>
    /// Gets the number of fields in a data row of the CSV file.
    /// </summary>
    /// <value>At least 1.</value>
    public int RowLength { get; internal set; } = 1;
}
