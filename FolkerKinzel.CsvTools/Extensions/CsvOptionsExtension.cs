using System.Runtime.CompilerServices;

namespace FolkerKinzel.CsvTools.Extensions
{
    /// <summary>
    /// Erweiterungsmethoden, die die Arbeit mit der <see cref="CsvOptions"/>-Enum erleichtern.
    /// </summary>
    public static class CsvOptionsExtension
    {
        /// <summary>
        /// Setzt sämtliche in <paramref name="flags"/> gesetzten Flags in <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Der <see cref="CsvOptions"/>?-Wert, auf dem die in <paramref name="flags"/> gesetzten Flags gesetzt werden.</param>
        /// <param name="flags">Ein einzelner <see cref="CsvOptions"/>-Wert oder eine Kombination aus mehreren 
        /// <see cref="CsvOptions"/>-Werten.</param>
        /// <returns>Ein <see cref="CsvOptions"/>-Wert, auf dem sämtliche in <paramref name="value"/> und <paramref name="flags"/>
        /// gesetzten Flags gesetzt sind.</returns>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static CsvOptions Set(this CsvOptions value, CsvOptions flags)
        {
            return (value | flags);
        }

        /// <summary>
        /// Untersucht, ob sämtliche in <paramref name="flags"/> gesetzten Flags auch in <paramref name="value"/>
        /// gesetzt sind.
        /// </summary>
        /// <param name="value">Der <see cref="CsvOptions"/>?-Wert, der daraufhin überprüft wird, ob sämtliche in <paramref name="flags"/> gesetzten 
        /// Flags auf ihm gesetzt sind.</param>
        /// <param name="flags">Ein einzelner <see cref="CsvOptions"/>-Wert oder eine Kombination aus mehreren 
        /// <see cref="CsvOptions"/>-Werten.</param>
        /// <returns><c>true</c>, wenn sämtliche in <paramref name="flags"/> gesetzten Flags auch in <paramref name="value"/>
        /// gesetzt sind. (Wenn <paramref name="flags"/>&#160;<see cref="CsvOptions.None"/> ist, wird nur dann <c>true</c> zurückgegeben,
        /// wenn auch <paramref name="value"/>&#160;<see cref="CsvOptions.None"/> ist.</returns>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsSet(this CsvOptions value, CsvOptions flags)
        {
            return flags == CsvOptions.None ? value == flags : (value & flags) == flags;
        }

        /// <summary>
        /// Entfernt sämtliche in <paramref name="flags"/> gesetzten Flags aus <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Der <see cref="CsvOptions"/>?-Wert, aus dem die in <paramref name="flags"/> gesetzten Flags entfernt werden.</param>
        /// <param name="flags">Ein einzelner <see cref="CsvOptions"/>-Wert oder eine Kombination aus mehreren 
        /// <see cref="CsvOptions"/>-Werten.</param>
        /// <returns>Ein <see cref="CsvOptions"/>-Wert, auf dem sämtliche in <paramref name="flags"/>
        /// gesetzten Flags entfernt sind.</returns>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static CsvOptions Unset(this CsvOptions value, CsvOptions flags)
        {
            return value & ~flags;
        }
    }
}
