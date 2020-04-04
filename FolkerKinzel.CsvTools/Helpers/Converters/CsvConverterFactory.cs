using FolkerKinzel.CsvTools.Helpers.Converters.Intls;
using FolkerKinzel.CsvTools.Helpers.Converters.Specialized;
using System;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Helpers.Converters
{
    /// <summary>
    /// Statische Klasse, die eine Methode veröffentlicht, um <see cref="ICsvTypeConverter"/>-Objekte für alle elementaren
    /// Datentypen (inkl. <see cref="DateTime"/>) zu instanziieren.
    /// </summary>
    public static class CsvConverterFactory
    {
        /// <summary>
        /// Erzeugt einen <see cref="ICsvTypeConverter"/> für den angegebenen Datentyp.
        /// </summary>
        /// <param name="type">Der Datentyp des Konverters.</param>
        /// <param name="nullable">Wenn <c>true</c>, wird ein <see cref="ICsvTypeConverter"/>-Objekt erstellt, das <c>null</c> als Eingabe akzeptiert
        /// und auch zurückzugeben vermag (<see cref="Nullable{T}"/>).</param>
        /// <param name="maybeDBNull">Wenn true, wird <see cref="DBNull.Value"/> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="ICsvTypeConverter.FallbackValue"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="ICsvTypeConverter.Parse(string)"/> des erzeugten 
        /// <see cref="ICsvTypeConverter"/>-Objekts eine Ausnahme, wenn das Parsen
        /// misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="ICsvTypeConverter.FallbackValue"/> zurück.</param>
        /// <returns>Ein <see cref="ICsvTypeConverter"/>-Objekt zur Umwandlung des gewünschten Datentyps.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/> ist keiner der definierten Werte der 
        /// <see cref="CsvTypeCode"/>-enum.</exception>
        public static ICsvTypeConverter CreateConverter(
            CsvTypeCode type,
            bool nullable = false,
            bool maybeDBNull = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) => type switch
            {
                CsvTypeCode.Boolean => new NumberConverter<bool>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.Byte => new NumberConverter<byte>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.Char => new NumberConverter<char>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.Date => new DateTimeConverter(true, nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.DateTime => new DateTimeConverter(false, nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.Decimal => new NumberConverter<decimal>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.Double => new NumberConverter<double>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.Int16 => new NumberConverter<short>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.Int32 => new NumberConverter<int>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.Int64  => new NumberConverter<long>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.SByte => new NumberConverter<sbyte>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.Single => new NumberConverter<float>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.String => new StringConverter(nullable),
                CsvTypeCode.UInt16 => new NumberConverter<ushort>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.UInt32 => new NumberConverter<uint>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.UInt64 => new NumberConverter<ulong>(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.DateTimeOffset => new DateTimeOffsetConverter(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.TimeSpan => new TimeSpanConverter(nullable, maybeDBNull, provider, throwOnParseErrors),
                CsvTypeCode.ByteArray => new Base64Converter(nullable, throwOnParseErrors),
                CsvTypeCode.Guid => new GuidConverter(nullable, maybeDBNull, throwOnParseErrors),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };


        /// <summary>
        /// Initialisiert ein <see cref="ICsvTypeConverter"/>-Objekt, das einen Enum-Typ in seine
        /// Zahlendarstellung wandeln und aus dieser sowie auch aus Wort-Darstellungen der Enum-Bezeichner wieder parsen kann.
        /// </summary>
        /// <typeparam name="TEnum">Ein beliebiger Enum-Typ.</typeparam>
        /// <param name="nullable">Wenn <c>true</c>, wird ein <see cref="ICsvTypeConverter"/>-Objekt erstellt, das <c>null</c> als Eingabe akzeptiert
        /// und auch zurückzugeben vermag (<see cref="Nullable{T}"/>).</param>
        /// <param name="maybeDBNull">Wenn true, wird <see cref="DBNull.Value"/> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="ICsvTypeConverter.FallbackValue"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="ICsvTypeConverter.Parse(string)"/> des erzeugten 
        /// <see cref="ICsvTypeConverter"/>-Objekts eine Ausnahme, wenn das Parsen
        /// misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="ICsvTypeConverter.FallbackValue"/> zurück.</param>
        /// <param name="ignoreCase">Wenn <c>true</c>, wird die Groß- und Kleinschreibung von Enum-Bezeichnern beim Parsen ignoriert.</param>
        /// <returns>Ein <see cref="ICsvTypeConverter"/>-Objekt zur Umwandlung des Enum-Datentyps <typeparamref name="TEnum"/>.</returns>
        public static ICsvTypeConverter CreateEnumConverter<TEnum>(
            bool nullable = false,
            bool maybeDBNull = false,
            bool throwOnParseErrors = false,
            bool ignoreCase = true) where TEnum: struct, Enum
        {
            return new EnumConverter<TEnum>(nullable, maybeDBNull, throwOnParseErrors, ignoreCase);
        }


        /// <summary>
        /// Initialisiert ein <see cref="ICsvTypeConverter"/>-Objekt, das einen Enum-Typ in seine
        /// Zahlendarstellung wandeln und aus dieser sowie auch aus Wort-Darstellungen der Enum-Bezeichner wieder parsen kann.
        /// </summary>
        /// <typeparam name="TEnum">Ein beliebiger Enum-Typ.</typeparam>
        /// <param name="format">Ein Formatstring, der für die <see cref="string"/>-Ausgabe von <typeparamref name="TEnum"/> verwendet wird.</param>
        /// <param name="nullable">Wenn <c>true</c>, wird ein <see cref="ICsvTypeConverter"/>-Objekt erstellt, das <c>null</c> als Eingabe akzeptiert
        /// und auch zurückzugeben vermag (<see cref="Nullable{T}"/>).</param>
        /// <param name="maybeDBNull">Wenn true, wird <see cref="DBNull.Value"/> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="ICsvTypeConverter.FallbackValue"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="ICsvTypeConverter.Parse(string)"/> des erzeugten 
        /// <see cref="ICsvTypeConverter"/>-Objekts eine Ausnahme, wenn das Parsen
        /// misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="ICsvTypeConverter.FallbackValue"/> zurück.</param>
        /// <returns>Ein <see cref="ICsvTypeConverter"/>-Objekt zur Umwandlung des Enum-Datentyps <typeparamref name="TEnum"/>.</returns>
        /// <param name="ignoreCase">Wenn <c>true</c>, wird die Groß- und Kleinschreibung von Enum-Bezeichnern beim Parsen ignoriert.</param>
        /// <exception cref="ArgumentException"><paramref name="format"/> ist kein gültiger Formatstring für Enum-Datentypen.</exception>
        public static ICsvTypeConverter CreateEnumConverter<TEnum>(
            string? format,
            bool nullable = false,
            bool maybeDBNull = false,
            bool throwOnParseErrors = false,
            bool ignoreCase = true) where TEnum : struct, Enum
        {
            return new EnumConverter<TEnum>(format, nullable, maybeDBNull, throwOnParseErrors, ignoreCase);
        }


        /// <summary>
        /// Initialisiert ein <see cref="ICsvTypeConverter"/>-Objekt, das einen ganzzahligen Datentyp in seine
        /// hexadezimale Darstellung wandeln und aus dieser wieder parsen kann.
        /// </summary>
        /// <param name="type">Konstante für einen ganzzahligen Datentyp.</param>
        /// <param name="nullable">Wenn <c>true</c>, wird ein <see cref="ICsvTypeConverter"/>-Objekt erstellt, das <c>null</c> als Eingabe akzeptiert
        /// und auch zurückzugeben vermag (<see cref="Nullable{T}"/>).</param>
        /// <param name="maybeDBNull">Wenn true, wird <see cref="DBNull.Value"/> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="ICsvTypeConverter.FallbackValue"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="ICsvTypeConverter.Parse(string)"/> des erzeugten 
        /// <see cref="ICsvTypeConverter"/>-Objekts eine Ausnahme, wenn das Parsen
        /// misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="ICsvTypeConverter.FallbackValue"/> zurück.</param>
        /// <returns>Ein <see cref="ICsvTypeConverter"/>-Objekt zur Umwandlung des gewünschten ganzzahligen Datentyps in hexadezimale Darstellung und umgekehrt.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/> benennt keinen ganzzahligen Datentyp. (Unterstützt werden
        /// <see cref="CsvTypeCode.Byte"/>, <see cref="CsvTypeCode.UInt16"/>, <see cref=" CsvTypeCode.UInt32"/>, <see cref="CsvTypeCode.UInt64"/>,
        /// <see cref="CsvTypeCode.SByte"/>, <see cref="CsvTypeCode.Int16"/>, <see cref=" CsvTypeCode.Int32"/> und <see cref="CsvTypeCode.Int64"/>.)
        /// </exception>
        public static ICsvTypeConverter CreateHexConverter(
            CsvTypeCode type,
            bool nullable = false,
            bool maybeDBNull = false,
            bool throwOnParseErrors = false) => type switch
            {
                CsvTypeCode.Byte => new HexConverter<byte>(true, nullable, maybeDBNull, throwOnParseErrors),
                CsvTypeCode.UInt16 => new HexConverter<ushort>(true, nullable, maybeDBNull, throwOnParseErrors),
                CsvTypeCode.UInt32 => new HexConverter<int>(true, nullable, maybeDBNull, throwOnParseErrors),
                CsvTypeCode.UInt64 => new HexConverter<ulong>(true, nullable, maybeDBNull, throwOnParseErrors),
                CsvTypeCode.SByte => new HexConverter<sbyte>(false, nullable, maybeDBNull, throwOnParseErrors),
                CsvTypeCode.Int16 => new HexConverter<short>(false, nullable, maybeDBNull, throwOnParseErrors),
                CsvTypeCode.Int32 => new HexConverter<int>(false, nullable, maybeDBNull, throwOnParseErrors),
                CsvTypeCode.Int64 => new HexConverter<long>(false, nullable, maybeDBNull, throwOnParseErrors),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };

        
        
    }
}
