using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using FolkerKinzel.CsvTools.TypeConversions.Converters;

namespace FolkerKinzel.CsvTools.TypeConversions;

//[Flags]
//public enum CsvConverterOptions
//{
//    None = 0,
//    Nullable = 1,
//    DBNullEnabled = 2,
//    Throwing = 4,
//    Default = Throwing
//}

/// <summary>
/// Statische Klasse, die Methoden veröffentlicht, um <see cref="ICsvTypeConverter"/>-Objekte für alle elementaren
/// Datentypen zu instanziieren.
/// </summary>
public static class CsvConverterFactory
{
    /// <summary>
    /// Initialisiert einen neues <see cref="ICsvTypeConverter"/>-Objekt für den angegebenen Datentyp.
    /// </summary>
    /// <param name="typeCode">Der Datentyp, den der <see cref="ICsvTypeConverter"/> konvertiert.</param>
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
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="typeCode"/> ist keiner der definierten Werte der 
    /// <see cref="CsvTypeCode"/>-Enum.</exception>
    public static ICsvTypeConverter CreateConverter(
        CsvTypeCode typeCode,
        bool nullable = false,
        bool dbNullEnabled = false,
        bool throwing = true,
        IFormatProvider? formatProvider = null)
        => typeCode switch
        {
            CsvTypeCode.Boolean => new BooleanConverter(throwing).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.Byte => new ByteConverter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.Char => new CharConverter(throwing).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.Date => new DateTimeConverter(throwing, formatProvider).AsDateConverter().HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.DateTime => new DateTimeConverter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.Decimal => new DecimalConverter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.Double => new DoubleConverter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.Int16 => new Int16Converter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.Int32 => new Int32Converter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.Int64 => new Int64Converter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.SByte => new SByteConverter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.Single => new SingleConverter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.String => dbNullEnabled ? new StringConverter(nullable).AsDBNullEnabled() : new StringConverter(nullable),
            CsvTypeCode.UInt16 => new UInt16Converter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.UInt32 => new UInt32Converter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.UInt64 => new UInt64Converter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.DateTimeOffset => new DateTimeOffsetConverter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.TimeSpan => new TimeSpanConverter(throwing, formatProvider).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.ByteArray => dbNullEnabled ? new ByteArrayConverter(throwing, nullable).AsDBNullEnabled() : new ByteArrayConverter(throwing, nullable),
            CsvTypeCode.Guid => new GuidConverter(throwing).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            _ => throw new ArgumentOutOfRangeException(nameof(typeCode))
        };
        

    //private static CsvTypeConverter<T> CreateConverter<T>(CsvTypeCode typeCode, bool throwing, IFormatProvider? formatProvider)
    //    => typeCode switch
    //    {
    //        CsvTypeCode.Boolean => new BooleanConverter(throwing),
    //        CsvTypeCode.Byte => new ByteConverter(throwing, formatProvider),
    //        CsvTypeCode.Char => new CharConverter(throwing),
    //        //CsvTypeCode.Date => new DateTimeConverter.Create(isDate: true, options: options, formatProvider: formatProvider),
    //        CsvTypeCode.DateTime => new DateTimeConverter(throwing, formatProvider),
    //        CsvTypeCode.Decimal => new DecimalConverter(throwing, formatProvider),
    //        CsvTypeCode.Double => new DoubleConverter(throwing, formatProvider),
    //        CsvTypeCode.Int16 => new Int16Converter(throwing, formatProvider),
    //        CsvTypeCode.Int32 => new Int32Converter(throwing, formatProvider),
    //        CsvTypeCode.Int64 => new Int64Converter(throwing, formatProvider),
    //        CsvTypeCode.SByte => new SByteConverter(throwing, formatProvider),
    //        CsvTypeCode.Single => new SingleConverter(throwing, formatProvider),
    //        //CsvTypeCode.String => new StringConverter(nullable),
    //        CsvTypeCode.UInt16 => new UInt16Converter(throwing, formatProvider),
    //        CsvTypeCode.UInt32 => new UInt32Converter(throwing, formatProvider),
    //        CsvTypeCode.UInt64 => new UInt64Converter(throwing, formatProvider),
    //        CsvTypeCode.DateTimeOffset => new DateTimeOffsetConverter(throwing, formatProvider),
    //        CsvTypeCode.TimeSpan => new TimeSpanConverter(throwing, formatProvider),
    //        //CsvTypeCode.ByteArray => new ByteArrayConverter() .Create(options: options),
    //        CsvTypeCode.Guid => new GuidConverter(throwing),
    //        _ => throw new ArgumentOutOfRangeException(nameof(typeCode))
    //    };


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
    /// <returns>Ein <see cref="ICsvTypeConverter"/>-Objekt zur Umwandlung des Enum-Datentyps <typeparamref name="TEnum"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="format"/> ist kein gültiger Formatstring für Enum-Datentypen.</exception>
    public static ICsvTypeConverter CreateEnumConverter<TEnum>(
        bool nullable = false,
        bool dbNullEnabled = false,
        bool throwing = true,
        TEnum fallbackValue = default,
        bool ignoreCase = true) where TEnum : struct, Enum
        => new EnumConverter<TEnum>(throwing, fallbackValue, ignoreCase).HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled);


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
    public static ICsvTypeConverter CreateHexConverter(
        CsvTypeCode type,
        bool nullable = false,
        bool dbNullEnabled = false,
        bool throwing = true) => type switch
        {
            CsvTypeCode.Byte => new ByteConverter(throwing).AsHexConverter().HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.UInt16 => new UInt16Converter(throwing).AsHexConverter().HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.UInt32 => new UInt32Converter(throwing).AsHexConverter().HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.UInt64 => new UInt64Converter(throwing).AsHexConverter().HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.Int16 => new Int16Converter(throwing).AsHexConverter().HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.Int32 => new Int32Converter(throwing).AsHexConverter().HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.Int64 => new Int64Converter(throwing).AsHexConverter().HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            CsvTypeCode.SByte => new SByteConverter(throwing).AsHexConverter().HandleNullableAndDBNullAcceptance(nullable, dbNullEnabled),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };



}
