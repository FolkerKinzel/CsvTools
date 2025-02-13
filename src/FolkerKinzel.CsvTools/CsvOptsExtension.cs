namespace FolkerKinzel.CsvTools;

/// <summary>Extension methods for the <see cref="CsvOpts" /> enum.</summary>
public static class CsvOptsExtension
{
    /// <summary>Sets all flags set in <paramref name="flags" /> in 
    /// <paramref name="value" />.</summary>
    /// <param name="value">The <see cref="CsvOpts" /> Value to which the flags
    /// set in <paramref name="flags" /> are set.</param>
    /// <param name="flags">A single <see cref="CsvOpts" /> value or a combination
    /// of several <see cref="CsvOpts" /> values.</param>
    /// <returns>A <see cref="CsvOpts" /> value on which all in <paramref name="value"
    /// /> and <paramref name="flags" /> set flags are set.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvOpts Set(this CsvOpts value, CsvOpts flags) => value | flags;

    /// <summary>Removes all flags set in <paramref name="flags" /> from 
    /// <paramref name="value" />.</summary>
    /// <param name="value">The <see cref="CsvOpts" /> value from which the flags
    /// set in <paramref name="flags" /> are removed.</param>
    /// <param name="flags">A single <see cref="CsvOpts" /> value or a combination
    /// of several <see cref="CsvOpts" /> values.</param>
    /// <returns>A <see cref="CsvOpts" /> value on which everyone in 
    /// <paramref name="flags" /> set flags are removed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CsvOpts Unset(this CsvOpts value, CsvOpts flags) => value & ~flags;
}
