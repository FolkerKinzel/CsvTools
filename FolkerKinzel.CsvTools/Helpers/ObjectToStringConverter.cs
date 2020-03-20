using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FolkerKinzel.CsvTools.Helpers
{
    internal static class ObjectToStringConverter
    {
        internal static string? ConvertToString(object? o) => o switch
        {
            null => null,
            DBNull _ => null,
            string s => s,
            IEnumerable<byte> bytes => Convert.ToBase64String(bytes.ToArray()),
            _ => Convert.ToString(o, CultureInfo.InvariantCulture),
        };

    }
}
