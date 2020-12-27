using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools
{
    internal static class AutoColumnName
    {
        private const string DefaultKeyName = "Column";

        internal static string Create(int index) => DefaultKeyName + (index + 1).ToString(CultureInfo.InvariantCulture);

    }
}
