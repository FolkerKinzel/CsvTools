using FolkerKinzel.CsvTools.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Extensions;

#if NET40 || NETSTANDARD2_0 || NET461
internal static class PropertyCollectionExtension
{
    internal static bool TryGetValue(
        this KeyedCollection<string, CsvProperty> kColl, string key, [NotNullWhen(true)] out CsvProperty? value)
    {
        Debug.Assert(kColl != null);
        Debug.Assert(key != null);

        if (kColl.Contains(key))
        {
            value = kColl[key];
            return true;
        }

        value = null;
        return false;
    }
}
#endif