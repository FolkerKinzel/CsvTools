using System.Globalization;
using FolkerKinzel.CsvTools.TypeConversions.Converters;

namespace FolkerKinzel.CsvTools.TypeConversions;

[Flags]
public enum CsvConverterOptions
{
    None = 0,
    Nullable = 1,
    AcceptsDBNull = 2,
    ThrowsOnParseErrors = 4,
    Default = ThrowsOnParseErrors
}

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
        CsvConverterOptions options = default,
        IFormatProvider? formatProvider = null)
        => type switch
        {
            CsvTypeCode.Boolean => BooleanConverter.Create(options: options),
            CsvTypeCode.Byte => ByteConverter.Create(options: options, formatProvider: formatProvider, hexConverter: false),
            CsvTypeCode.Char => CharConverter.Create(options: options),
            CsvTypeCode.Date => DateTimeConverter2.Create(isDate: true, options: options, formatProvider: formatProvider),
            CsvTypeCode.DateTime => DateTimeConverter2.Create(isDate: false, options: options, formatProvider: formatProvider),
            CsvTypeCode.Decimal => DecimalConverter.Create(options: options, formatProvider: formatProvider),
            CsvTypeCode.Double => DoubleConverter.Create(options: options, formatProvider: formatProvider),
            CsvTypeCode.Int16 => Int16Converter.Create(options: options, formatProvider: formatProvider, hexConverter: false),
            CsvTypeCode.Int32 => Int32Converter.Create(options: options, formatProvider: formatProvider, hexConverter: false),
            CsvTypeCode.Int64 => Int64Converter.Create(options: options, formatProvider: formatProvider, hexConverter: false),
            CsvTypeCode.SByte => SByteConverter.Create(options: options, formatProvider: formatProvider,  hexConverter: false),
            CsvTypeCode.Single => FloatConverter.Create(options: options, formatProvider: formatProvider),
            CsvTypeCode.String => StringConverter2.Create(options: options),
            CsvTypeCode.UInt16 => UInt16Converter.Create(options: options, formatProvider: formatProvider, hexConverter: false),
            CsvTypeCode.UInt32 => UInt32Converter.Create(options: options, formatProvider: formatProvider, hexConverter: false),
            CsvTypeCode.UInt64 => UInt64Converter.Create(options: options, formatProvider: formatProvider,  hexConverter: false),
            CsvTypeCode.DateTimeOffset => DateTimeOffsetConverter2.Create(options: options, formatProvider: formatProvider),
            CsvTypeCode.TimeSpan => TimeSpanConverter2.Create(options: options, formatProvider: formatProvider),
            CsvTypeCode.ByteArray => Base64Converter2.Create(options: options),
            CsvTypeCode.Guid => GuidConverter2.Create(options: options),
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
        CsvConverterOptions options = default,
        TEnum fallbackValue = default,
        bool ignoreCase = true,
        string? format = "D") where TEnum : struct, Enum 
        => EnumConverter2<TEnum>.Create(options: options, ignoreCase: ignoreCase, format, fallbackValue: fallbackValue);


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
    /// <param name="throwsOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="ICsvTypeConverter.Parse(string)"/> des erzeugten 
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
        CsvConverterOptions options = CsvConverterOptions.Default) => type switch
        {
            CsvTypeCode.Byte => ByteConverter.Create(options: options, formatProvider: null, hexConverter: true),
            CsvTypeCode.UInt16 => UInt16Converter.Create(options: options, formatProvider: null, hexConverter: true),
            CsvTypeCode.UInt32 => UInt32Converter.Create(options: options, formatProvider: null, hexConverter: true),
            CsvTypeCode.UInt64 => UInt64Converter.Create(options: options, formatProvider: null, hexConverter: true),
            CsvTypeCode.SByte => SByteConverter.Create(options: options, formatProvider: null, hexConverter: true),
            CsvTypeCode.Int16 => Int16Converter.Create(options: options, formatProvider: null, hexConverter: true),
            CsvTypeCode.Int32 => Int32Converter.Create(options: options, formatProvider: null, hexConverter: true),
            CsvTypeCode.Int64 => Int64Converter.Create(options: options, formatProvider: null, hexConverter: true),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };



}
