namespace FolkerKinzel.CsvTools.Intls;

internal class CsvRow : List<ReadOnlyMemory<char>>
{
    internal CsvRow(int capacity) : base(capacity) { }

    internal bool IsEmpty => Count == 1 && this[0].IsEmpty;
}
