using System.Globalization;

namespace FolkerKinzel.CsvTools.Intls;

internal static class AutoColumnName
{
    private const string DEFAULT_KEY_NAME = "Column";

    internal static string Create(int index) => DEFAULT_KEY_NAME + (index + 1).ToString(CultureInfo.InvariantCulture);
}
