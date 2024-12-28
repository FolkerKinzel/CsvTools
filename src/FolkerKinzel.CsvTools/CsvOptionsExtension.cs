namespace FolkerKinzel.CsvTools;

/// <summary>Extension methods that make working with the <see cref="CsvOpts" /> enum 
/// easier.</summary>
public static class CsvOptionsExtension
{
    /// <summary>Sets all flags set in <paramref name="flags" /> in <paramref name="value"
    /// />.</summary>
    /// <param name="value">The <see cref="CsvOpts" /> Value to which the flags
    /// set in <paramref name="flags" /> are set.</param>
    /// <param name="flags">A single <see cref="CsvOpts" /> value or a combination
    /// of several <see cref="CsvOpts" /> values.</param>
    /// <returns>A <see cref="CsvOpts" /> value on which all in <paramref name="value"
    /// /> and <paramref name="flags" /> set flags are set.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvOpts Set(this CsvOpts value, CsvOpts flags) => value | flags;

    ///// <summary>Examines whether all flags set in <paramref name="flags" /> also in
    ///// <paramref name="value" /> are set.</summary>
    ///// <param name="value">The <see cref="CsvOptions" /> value, which is checked to
    ///// see whether all flags set in <paramref name="flags" /> are set on it.</param>
    ///// <param name="flags">A single <see cref="CsvOptions" /> value or a combination
    ///// of several <see cref="CsvOptions" /> values.</param>
    ///// <returns> <c>true</c> if all flags set in <paramref name="flags" /> also in
    ///// <paramref name="value" /> are set. (If <paramref name="flags" /> is <see cref="CsvOptions.None"
    ///// />, <c>true</c> is only returned if <paramref name="value" /> is also <see
    ///// cref="CsvOptions.None" />.</returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static bool IsSet(this CsvOptions value, CsvOptions flags)
    //    => flags == CsvOptions.None ? value == flags : (value & flags) == flags;

    /// <summary>Removes all flags set in <paramref name="flags" /> from <paramref name="value"
    /// />.</summary>
    /// <param name="value">The <see cref="CsvOptions" /> value from which the flags
    /// set in <paramref name="flags" /> are removed.</param>
    /// <param name="flags">A single <see cref="CsvOptions" /> value or a combination
    /// of several <see cref="CsvOptions" /> values.</param>
    /// <returns>A <see cref="CsvOptions" /> value on which everyone in <paramref name="flags"
    /// /> set flags are removed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvOpts Unset(this CsvOpts value, CsvOpts flags) => value & ~flags;
}
