namespace FolkerKinzel.CsvTools.Intls;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
internal static class _ArgumentOutOfRangeException
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ThrowIfNegative(int argument, string? paramName)
#if NET462 || NET5_0 || NETSTANDARD2_0 || NETSTANDARD2_1
    { if (argument < 0) { throw new ArgumentOutOfRangeException(paramName); } }
#else
        => ArgumentOutOfRangeException.ThrowIfNegative(argument, paramName);
#endif

    internal static void ValidateDelimiter(char delimiter)
    {
        if (delimiter == '\"' || delimiter == '\r' || delimiter == '\n')
        {
            throw new ArgumentOutOfRangeException(nameof(delimiter));
        }
    }
}

