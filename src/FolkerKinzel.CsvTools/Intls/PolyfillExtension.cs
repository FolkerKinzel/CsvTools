namespace FolkerKinzel.CsvTools.Intls;

internal static class PolyfillExtension
{
#if NET461 || NETSTANDARD2_0
        internal static void Write(this TextWriter writer, ReadOnlySpan<char> value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                writer.Write(value[i]);
            }
        }
#endif
}
