using System.Globalization;

namespace FolkerKinzel.CsvTools.Intls;

internal static class AutoColumnName
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string Create(int index) => $"Column{index + 1}";
}
