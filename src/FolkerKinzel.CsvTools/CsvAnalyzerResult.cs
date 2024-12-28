using System.Runtime.InteropServices;

namespace FolkerKinzel.CsvTools;

public class CsvAnalyzerResult
{
    /// <summary>The column names of the CSV file.</summary>
    public IReadOnlyList<string>? ColumnNames { get; internal set; }

    /// <summary>Options for reading the CSV file.</summary>
    public CsvOpts Options { get; internal set; } = CsvOpts.Default;

    /// <summary>The field delimiter character used in the CSV file.</summary>
    public char Delimiter { get; internal set; } = ',';

    /// <summary> <c>true</c>, if the CSV file has a header with column names.</summary>
    public bool HasHeaderRow => ColumnNames != null;
}
