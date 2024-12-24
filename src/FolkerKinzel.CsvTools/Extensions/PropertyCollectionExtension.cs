using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Extensions;

#if NETSTANDARD2_0 || NET462
internal static class PropertyCollectionExtension
{
    internal static bool TryGetValue(
        this KeyedCollection<string, CsvPropertyBase> kColl, string key, [NotNullWhen(true)] out CsvPropertyBase? value)
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