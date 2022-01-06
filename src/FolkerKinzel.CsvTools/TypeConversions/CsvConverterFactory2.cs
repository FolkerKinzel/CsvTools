using System.Globalization;
using FolkerKinzel.CsvTools.TypeConversions.Converters;
using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Statische Klasse, die Methoden veröffentlicht, um <see cref="ICsvTypeConverter"/>-Objekte für alle elementaren
/// Datentypen zu instanziieren.
/// </summary>
public static class CsvConverterFactory2
{
    /// <summary>
    /// Initialisiert einen neues <see cref="ICsvTypeConverter"/>-Objekt für den angegebenen Datentyp.
    /// </summary>
    /// <param name="type">Der Datentyp, den der <see cref="ICsvTypeConverter"/> konvertiert.</param>
    /// <param name="nullable">Wenn <c>true</c>, wird ein <see cref="ICsvTypeConverter"/>-Objekt erstellt, das <c>null</c> als Eingabe akzeptiert
    /// und auch zurückzugeben vermag (<see cref="Nullable{T}"/>).</param>
    /// <param name="acceptsDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value">DBNull.Value</see> als Eingabe akzeptiert und bildet auch den
    /// Rückgabewert von <see cref="ICsvTypeConverter.FallbackValue"/>.</param>
    /// <param name="formatProvider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
    /// bereitstellt, oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
    /// <param name="throwsOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="ICsvTypeConverter.Parse(string)"/> des erzeugten 
    /// <see cref="ICsvTypeConverter"/>-Objekts eine Ausnahme, wenn das Parsen
    /// misslingt,
    /// anderenfalls gibt sie in diesem Fall <see cref="ICsvTypeConverter.FallbackValue"/> zurück.</param>
    /// <returns>Ein <see cref="ICsvTypeConverter"/>-Objekt zur Umwandlung des gewünschten Datentyps.</returns>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/> ist keiner der definierten Werte der 
    /// <see cref="CsvTypeCode"/>-Enum.</exception>
    public static ICsvTypeConverter2 CreateConverter(
        CsvTypeCode type,
        bool nullable = false,
        bool acceptsDBNull = false,
        IFormatProvider? formatProvider = null,
        bool throwsOnParseErrors = false)
        => type switch
        {
            CsvTypeCode.Boolean => NumberConverter2<bool>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.Byte => NumberConverter2<byte>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.Char => NumberConverter2<char>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.Date => DateTimeConverter2.Create(true, nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.DateTime => DateTimeConverter2.Create(false, nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.Decimal => NumberConverter2<decimal>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.Double => NumberConverter2<double>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.Int16 => NumberConverter2<short>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.Int32 => NumberConverter2<int>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.Int64 => NumberConverter2<long>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.SByte => NumberConverter2<sbyte>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.Single => NumberConverter2<float>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.String => StringConverter2.Create(nullable, acceptsDBNull),
            CsvTypeCode.UInt16 => NumberConverter2<ushort>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.UInt32 => NumberConverter2<uint>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.UInt64 => NumberConverter2<ulong>.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.DateTimeOffset => DateTimeOffsetConverter2.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.TimeSpan => TimeSpanConverter2.Create(nullable, acceptsDBNull, formatProvider, throwsOnParseErrors),
            CsvTypeCode.ByteArray => Base64Converter2.Create(nullable, acceptsDBNull, throwsOnParseErrors),
            CsvTypeCode.Guid => GuidConverter2.Create(nullable, acceptsDBNull, throwsOnParseErrors),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };



    /// <summary>
    /// Initialisiert ein neues <see cref="ICsvTypeConverter"/>-Objekt, das einen Enum-Typ in seine
    /// Zahlendarstellung wandeln und aus dieser sowie auch aus Wort-Darstellungen der Enum-Bezeichner wieder parsen kann.
    /// </summary>
    /// <typeparam name="TEnum">Ein beliebiger Enum-Typ.</typeparam>
    /// <param name="nullable">Wenn <c>true</c>, wird ein <see cref="ICsvTypeConverter"/>-Objekt erstellt, das <c>null</c> als Eingabe akzeptiert
    /// und auch zurückzugeben vermag (<see cref="Nullable{T}"/>).</param>
    /// <param name="acceptsDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value">DBNull.Value</see> als Eingabe akzeptiert und bildet auch den
    /// Rückgabewert von <see cref="ICsvTypeConverter.FallbackValue"/>.</param>
    /// <param name="throwsOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="ICsvTypeConverter.Parse(string)"/> des erzeugten 
    /// <see cref="ICsvTypeConverter"/>-Objekts eine Ausnahme, wenn das Parsen
    /// misslingt,
    /// anderenfalls gibt sie in diesem Fall <see cref="ICsvTypeConverter.FallbackValue"/> zurück.</param>
    /// <param name="ignoreCase">Wenn <c>true</c>, wird die Groß- und Kleinschreibung von Enum-Bezeichnern beim Parsen ignoriert.</param>
    /// <param name="format">Ein Formatstring, der für die <see cref="string"/>-Ausgabe von <typeparamref name="TEnum"/> verwendet wird.</param>
    /// <returns>Ein <see cref="ICsvTypeConverter"/>-Objekt zur Umwandlung des Enum-Datentyps <typeparamref name="TEnum"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="format"/> ist kein gültiger Formatstring für Enum-Datentypen.</exception>
    public static ICsvTypeConverter2 CreateEnumConverter<TEnum>(
        bool nullable = false,
        bool acceptsDBNull = false,
        TEnum fallbackValue = default,
        bool throwsOnParseErrors = false,
        bool ignoreCase = true,
        string? format = "D") where TEnum : struct, Enum => EnumConverter2<TEnum>.Create(nullable, acceptsDBNull, throwsOnParseErrors, ignoreCase, format, fallbackValue);


    ///// <summary>
    ///// Initialisiert ein neues <see cref="ICsvTypeConverter"/>-Objekt, das einen Enum-Typ in seine
    ///// Zahlendarstellung wandeln und aus dieser sowie auch aus Wort-Darstellungen der Enum-Bezeichner wieder parsen kann.
    ///// </summary>
    ///// <typeparam name="TEnum">Ein beliebiger Enum-Typ.</typeparam>
    ///// <param name="format">Ein Formatstring, der für die <see cref="string"/>-Ausgabe von <typeparamref name="TEnum"/> verwendet wird.</param>
    ///// <param name="nullable">Wenn <c>true</c>, wird ein <see cref="ICsvTypeConverter"/>-Objekt erstellt, das <c>null</c> als Eingabe akzeptiert
    ///// und auch zurückzugeben vermag (<see cref="Nullable{T}"/>).</param>
    ///// <param name="maybeDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value">DBNull.Value</see> als Eingabe akzeptiert und bildet auch den
    ///// Rückgabewert von <see cref="ICsvTypeConverter.FallbackValue"/>.</param>
    ///// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="ICsvTypeConverter.Parse(string)"/> des erzeugten 
    ///// <see cref="ICsvTypeConverter"/>-Objekts eine Ausnahme, wenn das Parsen
    ///// misslingt,
    ///// anderenfalls gibt sie in diesem Fall <see cref="ICsvTypeConverter.FallbackValue"/> zurück.</param>
    ///// <returns>Ein <see cref="ICsvTypeConverter"/>-Objekt zur Umwandlung des Enum-Datentyps <typeparamref name="TEnum"/>.</returns>
    ///// <param name="ignoreCase">Wenn <c>true</c>, wird die Groß- und Kleinschreibung von Enum-Bezeichnern beim Parsen ignoriert.</param>
    ///// <exception cref="ArgumentException"><paramref name="format"/> ist kein gültiger Formatstring für Enum-Datentypen.</exception>
    //public static ICsvTypeConverter CreateEnumConverter<TEnum>(
    //    string? format,
    //    bool nullable = false,
    //    bool maybeDBNull = false,
    //    bool throwOnParseErrors = false,
    //    bool ignoreCase = true) where TEnum : struct, Enum => new EnumConverter<TEnum>(format, nullable, maybeDBNull, throwOnParseErrors, ignoreCase);


    /// <summary>
    /// Initialisiert ein neues <see cref="ICsvTypeConverter"/>-Objekt, das einen ganzzahligen Datentyp in seine
    /// hexadezimale Darstellung wandeln und aus dieser wieder parsen kann.
    /// </summary>
    /// <param name="type">Konstante für einen ganzzahligen Datentyp.</param>
    /// <param name="nullable">Wenn <c>true</c>, wird ein <see cref="ICsvTypeConverter"/>-Objekt erstellt, das <c>null</c> als Eingabe akzeptiert
    /// und auch zurückzugeben vermag (<see cref="Nullable{T}"/>).</param>
    /// <param name="acceptsDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value">DBNull.Value</see> als Eingabe akzeptiert und bildet auch den
    /// Rückgabewert von <see cref="ICsvTypeConverter.FallbackValue"/>.</param>
    /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="ICsvTypeConverter.Parse(string)"/> des erzeugten 
    /// <see cref="ICsvTypeConverter"/>-Objekts eine Ausnahme, wenn das Parsen
    /// misslingt,
    /// anderenfalls gibt sie in diesem Fall <see cref="ICsvTypeConverter.FallbackValue"/> zurück.</param>
    /// <returns>Ein <see cref="ICsvTypeConverter"/>-Objekt zur Umwandlung des gewünschten ganzzahligen Datentyps in hexadezimale Darstellung 
    /// und umgekehrt.</returns>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/> benennt keinen ganzzahligen Datentyp. (Unterstützt werden
    /// <see cref="CsvTypeCode.Byte"/>, <see cref="CsvTypeCode.UInt16"/>, <see cref=" CsvTypeCode.UInt32"/>, <see cref="CsvTypeCode.UInt64"/>,
    /// <see cref="CsvTypeCode.SByte"/>, <see cref="CsvTypeCode.Int16"/>, <see cref=" CsvTypeCode.Int32"/> und <see cref="CsvTypeCode.Int64"/>.)
    /// </exception>
    public static ICsvTypeConverter2 CreateHexConverter(
        CsvTypeCode type,
        bool nullable = false,
        bool acceptsDBNull = false,
        bool throwOnParseErrors = false) => type switch
        {
            CsvTypeCode.Byte => HexConverter2<byte>.Create(false, nullable, acceptsDBNull, throwOnParseErrors),
            CsvTypeCode.UInt16 => HexConverter2<ushort>.Create(false, nullable, acceptsDBNull, throwOnParseErrors),
            CsvTypeCode.UInt32 => HexConverter2<uint>.Create(false, nullable, acceptsDBNull, throwOnParseErrors),
            CsvTypeCode.UInt64 => HexConverter2<ulong>.Create(false, nullable, acceptsDBNull, throwOnParseErrors),
            CsvTypeCode.SByte => HexConverter2<sbyte>.Create(true, nullable, acceptsDBNull, throwOnParseErrors),
            CsvTypeCode.Int16 => HexConverter2<short>.Create(true, nullable, acceptsDBNull, throwOnParseErrors),
            CsvTypeCode.Int32 => HexConverter2<int>.Create(true, nullable, acceptsDBNull, throwOnParseErrors),
            CsvTypeCode.Int64 => HexConverter2<long>.Create(true, nullable, acceptsDBNull, throwOnParseErrors),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };



}
