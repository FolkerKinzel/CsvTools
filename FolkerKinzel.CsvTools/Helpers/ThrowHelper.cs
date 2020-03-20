using System;
using System.Collections.Generic;
using System.Text;

namespace FolkerKinzel.CsvTools.Helpers
{
    internal static class ThrowHelper
    {
        internal static void ThrowArgumentNullException(string propertyName)
            => throw new ArgumentNullException(propertyName);

        internal static void ThrowInvalidOperationException(string message)
            => throw new InvalidOperationException(message);
    }
}
