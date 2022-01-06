using System;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class Base64Converter2 : CsvTypeConverter<byte[]?>
{
    public Base64Converter2(bool throwsOnParseErrors, byte[]? fallbackValue) : base(throwsOnParseErrors, fallbackValue) { }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options)
    {
#if NET40
            byte[]? fallbackValue = options.HasFlag(CsvConverterOptions.Nullable) ? null : new byte[0];
#else
        byte[]? fallbackValue = options.HasFlag(CsvConverterOptions.Nullable) ? null : Array.Empty<byte>();
#endif

        var conv = new Base64Converter2(options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors), fallbackValue);

        return options.HasFlag(CsvConverterOptions.AcceptsDBNull) ? conv.AddDBNullAcceptance() : conv;
    }

    protected override string? DoConvertToString(byte[]? value) => value is null ? null : Convert.ToBase64String(value, Base64FormattingOptions.None);

    public override bool TryParseValue(string value, [NotNullWhen(true)] out byte[]? result)
    {
        try
        {
            result = (byte[])Convert.FromBase64String(value);
            return true;
        }
        catch (FormatException)
        {
            result = null;
            return false;
        }
    }
}


public sealed class EnumConverter2<TEnum> : CsvTypeConverter<TEnum> where TEnum : struct, Enum
{
    public EnumConverter2(
        bool ignoreCase = true,
        string? format = "D",
        bool throwsOnParseErrors = true,
        TEnum fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        ValidateFormat(format);
        this.IgnoreCase = ignoreCase;
        this.Format = format;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options, bool ignoreCase, string? format, TEnum fallbackValue)
        => new EnumConverter2<TEnum>(ignoreCase, format, options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors), fallbackValue)
        .HandleNullableAndDBNullAcceptance(options);

    private static void ValidateFormat(string? format)
    {
        if (format is null || format.Length == 0)
        {
            return;
        }

        if (format.Length == 1)
        {
            switch (char.ToUpperInvariant(format[0]))
            {
                case 'G':
                case 'D':
                case 'X':
                case 'F':
                    return;
            }
        }

        throw new ArgumentException("Invalid format string.", nameof(format));
    }

    internal bool IgnoreCase { get; }
    internal string? Format { get; }

    protected override string? DoConvertToString(TEnum value) => value.ToString(Format);

    public override bool TryParseValue(string value, out TEnum result) => Enum.TryParse<TEnum>(value, IgnoreCase, out result);
}


//public sealed class NumberConverter2<T> : CsvTypeConverter<T> where T : struct, IConvertible
//{
//    public NumberConverter2(IFormatProvider? formatProvider, bool throwsOnParseErrors, T fallbackValue = default)
//        : base(throwsOnParseErrors, fallbackValue) => FormatProvider = formatProvider;

//    internal static ICsvTypeConverter2 Create(bool nullable, bool acceptsDBNull, IFormatProvider? formatProvider, bool throwsOnParseErrors)
//        => new NumberConverter2<T>(formatProvider, throwsOnParseErrors).HandleNullableAndDBNullAcceptance(nullable, acceptsDBNull);

//    internal IFormatProvider? FormatProvider { get; }

//    protected override string? DoConvertToString(T value) => Convert.ToString(value, FormatProvider);

//    public override bool TryParseValue(string value, [NotNullWhen(true)] out T result)
//    {
//        try
//        {
//            result = (T)Convert.ChangeType(value, typeof(T), FormatProvider);
//            return true;
//        }
//        catch
//        {
//            result = default;
//            return false;
//        }
//    }
//}

public sealed class Int64Converter : CsvTypeConverter<long>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Any;


    public Int64Converter(IFormatProvider? formatProvider = null,
                          bool throwsOnParseErrors = true,
                          NumberStyles styles = DEFAULT_NUMBER_STYLE,
                          long fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = styles.HasFlag(NumberStyles.AllowHexSpecifier) ? "X" : null;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options,
                                              IFormatProvider? formatProvider,
                                              bool hexConverter)
        => new Int64Converter(formatProvider,
                              options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors),
                              hexConverter ? NumberStyles.HexNumber : DEFAULT_NUMBER_STYLE)
           .HandleNullableAndDBNullAcceptance(options);

    protected override string? DoConvertToString(long value) => value.ToString(_format, _formatProvider);

    public override bool TryParseValue(string value, out long result) => long.TryParse(value, _styles, _formatProvider, out result);

}

public sealed class Int32Converter : CsvTypeConverter<int>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Any;


    public Int32Converter(IFormatProvider? formatProvider = null,
                          bool throwsOnParseErrors = true,
                          NumberStyles styles = DEFAULT_NUMBER_STYLE,
                          int fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = styles.HasFlag(NumberStyles.AllowHexSpecifier) ? "X" : null;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options,
                                              IFormatProvider? formatProvider,
                                              bool hexConverter)
        => new Int32Converter(formatProvider,
                              options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors),
                              hexConverter ? NumberStyles.HexNumber : DEFAULT_NUMBER_STYLE)
           .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(int value) => value.ToString(_format, _formatProvider);

    public override bool TryParseValue(string value, out int result) => int.TryParse(value, _styles, _formatProvider, out result);

}

public sealed class Int16Converter : CsvTypeConverter<int>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Any;


    public Int16Converter(IFormatProvider? formatProvider = null,
                          bool throwsOnParseErrors = true,
                          NumberStyles styles = DEFAULT_NUMBER_STYLE,
                          int fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = styles.HasFlag(NumberStyles.AllowHexSpecifier) ? "X" : null;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options,
                                              IFormatProvider? formatProvider,
                                              bool hexConverter)
        => new Int16Converter(formatProvider,
                              options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors),
                              hexConverter ? NumberStyles.HexNumber : DEFAULT_NUMBER_STYLE)
           .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(int value) => value.ToString(_format, _formatProvider);

    public override bool TryParseValue(string value, out int result) => int.TryParse(value, _styles, _formatProvider, out result);
}


[CLSCompliant(false)]
public sealed class SByteConverter : CsvTypeConverter<sbyte>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Any;


    public SByteConverter(IFormatProvider? formatProvider = null,
                          bool throwsOnParseErrors = true,
                          NumberStyles styles = DEFAULT_NUMBER_STYLE,
                          sbyte fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = styles.HasFlag(NumberStyles.AllowHexSpecifier) ? "X" : null;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options,
                                              IFormatProvider? formatProvider,
                                              bool hexConverter)
        => new SByteConverter(formatProvider,
                              options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors),
                              hexConverter ? NumberStyles.HexNumber : DEFAULT_NUMBER_STYLE)
          .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(sbyte value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, out sbyte result) => sbyte.TryParse(value, _styles, _formatProvider, out result);
}


[CLSCompliant(false)]
public sealed class UInt64Converter : CsvTypeConverter<ulong>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Any;


    public UInt64Converter(IFormatProvider? formatProvider = null, bool throwsOnParseErrors = true, NumberStyles styles = DEFAULT_NUMBER_STYLE, ulong fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = styles.HasFlag(NumberStyles.AllowHexSpecifier) ? "X" : null;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options, IFormatProvider? formatProvider, bool hexConverter)
        => new UInt64Converter(formatProvider,
                               options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors),
                               hexConverter ? NumberStyles.HexNumber : DEFAULT_NUMBER_STYLE)
           .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(ulong value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, out ulong result) => ulong.TryParse(value, _styles, _formatProvider, out result);
}

[CLSCompliant(false)]
public sealed class UInt32Converter : CsvTypeConverter<uint>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Any;


    public UInt32Converter(IFormatProvider? formatProvider = null,
                           bool throwsOnParseErrors = true,
                           NumberStyles styles = DEFAULT_NUMBER_STYLE,
                           uint fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = styles.HasFlag(NumberStyles.AllowHexSpecifier) ? "X" : null;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options,
                                              IFormatProvider? formatProvider,
                                              bool hexConverter)
        => new UInt32Converter(formatProvider,
                               options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors),
                               hexConverter ? NumberStyles.HexNumber : DEFAULT_NUMBER_STYLE)
           .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(uint value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, out uint result) => uint.TryParse(value, _styles, _formatProvider, out result);
}

[CLSCompliant(false)]
public sealed class UInt16Converter : CsvTypeConverter<ushort>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Any;


    public UInt16Converter(IFormatProvider? formatProvider = null,
                           bool throwsOnParseErrors = true,
                           NumberStyles styles = DEFAULT_NUMBER_STYLE,
                           ushort fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = styles.HasFlag(NumberStyles.AllowHexSpecifier) ? "X" : null;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options,
                                              IFormatProvider? formatProvider,
                                              bool hexConverter)
        => new UInt16Converter(formatProvider,
                               options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors),
                               hexConverter ? NumberStyles.HexNumber : DEFAULT_NUMBER_STYLE)
            .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(ushort value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, out ushort result) => ushort.TryParse(value, _styles, _formatProvider, out result);
}

public sealed class ByteConverter : CsvTypeConverter<byte>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Any;

    public ByteConverter(IFormatProvider? formatProvider = null, bool throwsOnParseErrors = true, NumberStyles styles = DEFAULT_NUMBER_STYLE, byte fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = styles.HasFlag(NumberStyles.AllowHexSpecifier) ? "X" : null;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options, IFormatProvider? formatProvider, bool hexConverter)
        => new ByteConverter(formatProvider,
                             options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors),
                             hexConverter ? NumberStyles.HexNumber : DEFAULT_NUMBER_STYLE)
           .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(byte value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, out byte result) => byte.TryParse(value, _styles, _formatProvider, out result);
}

public sealed class DecimalConverter : CsvTypeConverter<decimal>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;

    public DecimalConverter(IFormatProvider? formatProvider = null, bool throwsOnParseErrors = true, NumberStyles styles = NumberStyles.Any, decimal fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options, IFormatProvider? formatProvider)
        => new DecimalConverter(formatProvider, options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors))
          .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(decimal value) => value.ToString(_formatProvider);


    public override bool TryParseValue(string value, out decimal result) => decimal.TryParse(value, _styles, _formatProvider, out result);
}

public sealed class DoubleConverter : CsvTypeConverter<double>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;

    public DoubleConverter(IFormatProvider? formatProvider = null, bool throwsOnParseErrors = true, NumberStyles styles = NumberStyles.Any, double fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options, IFormatProvider? formatProvider)
        => new DoubleConverter(formatProvider, options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors)).HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(double value) => value.ToString(_formatProvider);


    public override bool TryParseValue(string value, out double result) => double.TryParse(value, _styles, _formatProvider, out result);
}


public sealed class FloatConverter : CsvTypeConverter<float>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;

    public FloatConverter(IFormatProvider? formatProvider = null, bool throwsOnParseErrors = true, NumberStyles styles = NumberStyles.Any, float fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options, IFormatProvider? formatProvider)
        => new FloatConverter(formatProvider, options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors))
        .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(float value) => value.ToString(_formatProvider);


    public override bool TryParseValue(string value, out float result) => float.TryParse(value, _styles, _formatProvider, out result);
}

public sealed class BooleanConverter : CsvTypeConverter<bool>
{
    public BooleanConverter(bool throwsOnParseErrors = true, bool fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue) { }
    

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options)
        => new BooleanConverter(options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors))
           .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(bool value) => value.ToString();


    public override bool TryParseValue(string value, out bool result) => bool.TryParse(value, out result);
}


public sealed class CharConverter : CsvTypeConverter<char>
{
    public CharConverter(bool throwsOnParseErrors = true, char fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue) { }


    internal static ICsvTypeConverter2 Create(CsvConverterOptions options)
        => new CharConverter(options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors))
            .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(char value) => value.ToString();


    public override bool TryParseValue(string value, out char result) => char.TryParse(value, out result);
}


public sealed class StringConverter2 : CsvTypeConverter<string?>
{
    public StringConverter2(string? fallbackValue = null) : base(false, fallbackValue) { }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options)
    {
        string? fallbackValue = options.HasFlag(CsvConverterOptions.Nullable) ? null : string.Empty;

        var conv = new StringConverter2(fallbackValue);

        return options.HasFlag(CsvConverterOptions.AcceptsDBNull) ? conv.AddDBNullAcceptance() : conv;
    }

    protected override string? DoConvertToString(string? value) => value;
    public override bool TryParseValue(string value, out string? result)
    {
        result = value;
        return true;
    }
}


//public sealed class HexConverter2<T> : CsvTypeConverter<T> where T : struct
//{
//    private readonly bool _signed;

//    public HexConverter2(bool throwsOnParseErrors, T fallbackValue = default) 
//        : base(throwsOnParseErrors, fallbackValue)
//    {
//        switch (Type.GetTypeCode(FallbackValue.GetType()))
//        {
//            case TypeCode.Byte:   
//            case TypeCode.UInt16: 
//            case TypeCode.UInt32: 
//            case TypeCode.UInt64:
//                //_signed = false;
//                break;            
//            case TypeCode.SByte:
//            case TypeCode.Int16:
//            case TypeCode.Int32:
//            case TypeCode.Int64:
//                _signed = true;
//                break;
//            default: throw new NotSupportedException(String.Format("The Type {0} is not supported.", FallbackValue.GetType().FullName));
//        }
//    }

//    private HexConverter2(bool signed, bool throwsOnParseErrors) : base(throwsOnParseErrors, default)
//        => _signed = signed;

//    internal static ICsvTypeConverter2 Create(bool signed, bool nullable, bool acceptsDBNull, bool throwOnParseErrors)
//        => new HexConverter2<T>(signed, throwOnParseErrors).HandleNullableAndDBNullAcceptance(nullable, acceptsDBNull);


//    protected override string? DoConvertToString(T value)
//    {
//        const string format = "X";

//        if (_signed)
//        {
//            long l = Convert.ToInt64(value, CultureInfo.InvariantCulture);
//            return l.ToString(format, CultureInfo.InvariantCulture);
//        }
//        else
//        {
//            ulong l = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
//            return l.ToString(format, CultureInfo.InvariantCulture);
//        }
//    }

//    public override bool TryParseValue(string value, [NotNullWhen(true)] out T result)
//    {
//        const NumberStyles styles = NumberStyles.HexNumber;

//        if (_signed)
//        {
//            if (long.TryParse(value, styles, CultureInfo.InvariantCulture, out long res))
//            {
//                result = (T)Convert.ChangeType(res, typeof(T), CultureInfo.InvariantCulture);
//                return true;
//            }
//        }
//        else
//        {
//            if (ulong.TryParse(value, styles, CultureInfo.InvariantCulture, out ulong res))
//            {
//                result = (T)Convert.ChangeType(res, typeof(T), CultureInfo.InvariantCulture);
//                return true;
//            }
//        }

//        result = default;
//        return false;
//    }
//}


internal sealed class NullableStructConverter<T> : CsvTypeConverter<Nullable<T>> where T : struct
{
    private readonly CsvTypeConverter<T> _typeConverter;

    internal NullableStructConverter(CsvTypeConverter<T> converter)
        : base((converter ?? throw new ArgumentNullException(nameof(converter))).ThrowsOnParseErrors) => _typeConverter = converter;

    protected override string? DoConvertToString(T? value) => value.HasValue ? _typeConverter.ConvertToString(value.Value) : null;

    public override bool TryParseValue(string value, [NotNullWhen(true)] out T? result)
    {
        if (_typeConverter.TryParseValue(value, out T tmp))
        {
            result = tmp;
            return true;
        }

        result = FallbackValue;
        return false;
    }
}


internal sealed class DBNullConverter<T> : CsvTypeConverter<object>
{
    private readonly CsvTypeConverter<T> _valueConverter;

    internal DBNullConverter(CsvTypeConverter<T> converter)
        : base((converter ?? throw new ArgumentNullException(nameof(converter))).ThrowsOnParseErrors, DBNull.Value)
        => _valueConverter = converter;

    protected override string? DoConvertToString(object? value) => value == DBNull.Value ? null : _valueConverter.ConvertToString(value);

    public override bool TryParseValue(string value, out object result)
    {
        if (_valueConverter.TryParseValue(value, out T tmp))
        {
            result = tmp ?? FallbackValue!;
            return true;
        }

        result = FallbackValue!;
        return false;
    }
}

public sealed class DateTimeConverter2 : CsvTypeConverter<DateTime>
{
    private readonly IFormatProvider _formatProvider;
    private readonly string _format;
    private readonly bool _parseExact;
    private readonly DateTimeStyles _styles;

    private DateTimeConverter2(bool isDate, IFormatProvider? formatProvider, bool throwsOnParseErrors) : base(throwsOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = isDate ? "d" : "s";
        _styles = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;
    }

    internal static ICsvTypeConverter2 Create(bool isDate, CsvConverterOptions options, IFormatProvider? formatProvider)
        => new DateTimeConverter2(isDate, formatProvider, options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors))
            .HandleNullableAndDBNullAcceptance(options);

    public DateTimeConverter2(
        string? format,
        IFormatProvider? formatProvider = null,
        DateTime fallbackValue = default,
        bool throwsOnParseErrors = false,
        DateTimeStyles styles = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind,
        bool parseExact = false) : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = format ?? string.Empty;
        _parseExact = parseExact;
        ExamineFormat();
    }


    protected override string? DoConvertToString(DateTime value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, [NotNullWhen(true)] out DateTime result)
        => _parseExact
            ? DateTime.TryParseExact(value, _format, _formatProvider, _styles, out result)
            : DateTime.TryParse(value, _formatProvider, _styles, out result);


    private void ExamineFormat()
    {
        try
        {
            string tmp = DateTime.Now.ToString(_format, _formatProvider);

            if (_parseExact)
            {
                _ = DateTime.ParseExact(tmp, _format, _formatProvider, _styles);
            }
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }
}


public sealed class DateTimeOffsetConverter2 : CsvTypeConverter<DateTimeOffset>
{
    private readonly IFormatProvider _formatProvider;
    private readonly string _format;
    private readonly bool _parseExact;
    private readonly DateTimeStyles _styles;

    private DateTimeOffsetConverter2(IFormatProvider? formatProvider, bool throwsOnParseErrors) : base(throwsOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = "O";
        _styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options, IFormatProvider? formatProvider)
        => new DateTimeOffsetConverter2(formatProvider, options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors))
        .HandleNullableAndDBNullAcceptance(options);


    public DateTimeOffsetConverter2(
        string? format,

        IFormatProvider? formatProvider = null,
        DateTimeOffset fallbackValue = default,
        bool throwOnParseErrors = false,
        DateTimeStyles styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind,
        bool parseExact = false) : base(throwOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = format ?? string.Empty;
        _parseExact = parseExact;
        ExamineFormat();
    }


    protected override string? DoConvertToString(DateTimeOffset value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, [NotNullWhen(true)] out DateTimeOffset result)
        => _parseExact
            ? DateTimeOffset.TryParseExact(value, _format, _formatProvider, _styles, out result)
            : DateTimeOffset.TryParse(value, _formatProvider, _styles, out result);


    private void ExamineFormat()
    {
        try
        {
            string tmp = DateTimeOffset.Now.ToString(_format, _formatProvider);

            if (_parseExact)
            {
                _ = DateTimeOffset.ParseExact(tmp, _format, _formatProvider, _styles);
            }
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }
}


public sealed class TimeSpanConverter2 : CsvTypeConverter<TimeSpan>
{
    private readonly IFormatProvider _formatProvider;
    private readonly string _format;
    private readonly bool _parseExact;
    private readonly TimeSpanStyles _styles;

    private TimeSpanConverter2(IFormatProvider? formatProvider, bool throwsOnParseErrors) : base(throwsOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = "g";
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options, IFormatProvider? formatProvider)
       => new TimeSpanConverter2(formatProvider, options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors))
        .HandleNullableAndDBNullAcceptance(options);


    public TimeSpanConverter2(
        string? format,
        IFormatProvider? formatProvider = null,
        TimeSpan fallbackValue = default,
        bool throwOnParseErrors = false,
        TimeSpanStyles styles = TimeSpanStyles.None,
        bool parseExact = false) : base(throwOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = format ?? string.Empty;
        _parseExact = parseExact;
        ExamineFormat();
    }


    protected override string? DoConvertToString(TimeSpan value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, [NotNullWhen(true)] out TimeSpan result)
        => _parseExact
            ? TimeSpan.TryParseExact(value, _format, _formatProvider, _styles, out result)
            : TimeSpan.TryParse(value, _formatProvider, out result);


    private void ExamineFormat()
    {
        try
        {
            string tmp = TimeSpan.Zero.ToString(_format, _formatProvider);

            if (_parseExact)
            {
                _ = TimeSpan.ParseExact(tmp, _format, _formatProvider, _styles);
            }
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }
}



public sealed class GuidConverter2 : CsvTypeConverter<Guid>
{
    private readonly string _format;

    private GuidConverter2(bool throwsOnParseErrors) : base(throwsOnParseErrors) => _format = "D";

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options)
        => new GuidConverter2(options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors))
        .HandleNullableAndDBNullAcceptance(options);


    public GuidConverter2(
        string? format,
        bool throwOnParseErrors = false) : base(throwOnParseErrors, default)
    {
        _format = format ?? string.Empty;
        ExamineFormat(nameof(format));
    }


    protected override string? DoConvertToString(Guid value) => value.ToString(_format, CultureInfo.InvariantCulture);


    public override bool TryParseValue(string value, [NotNullWhen(true)] out Guid result)
        => Guid.TryParse(value, out result);


    private void ExamineFormat(string parameterName)
    {
        try
        {
            _ = Guid.Empty.ToString(_format, CultureInfo.InvariantCulture);
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, parameterName, e);
        }
    }
}


internal sealed class IEnumerableConverter<TItem> : CsvTypeConverter<IEnumerable<TItem?>?>
{
    private readonly char _separatorChar;
    private readonly CsvTypeConverter<TItem?> _itemsConverter;


    public IEnumerableConverter(CsvTypeConverter<TItem?> itemsConverter, char fieldSeparator, IEnumerable<TItem?>? fallbackValue)
        : base(false, fallbackValue)
    {
        _itemsConverter = itemsConverter ?? throw new ArgumentNullException(nameof(itemsConverter));
        _separatorChar = fieldSeparator;
    }

    protected override string? DoConvertToString(IEnumerable<TItem?>? value)
    {
        if (value is null || !value.Any())
        {
            return null;
        }
        var sb = new StringBuilder();
        using var writer = new StringWriter(sb);
        using (var csvWriter = new CsvWriter(writer, value.Count(), fieldSeparator: _separatorChar))
        {
            csvWriter.Record.Fill(value.Select(x => _itemsConverter.ConvertToString(x)));
            csvWriter.WriteRecord();
        }
        return writer.ToString();
    }

    public override bool TryParseValue(string value, out IEnumerable<TItem?>? result)
    {
        var list = new List<TItem?>();

        using var reader = new StringReader(value);
        using var csvReader = new CsvReader(reader, false, fieldSeparator: _separatorChar);

        CsvRecord? record = csvReader.Read().FirstOrDefault();

        if (record is null || record.Count == 0)
        {
            result = list!;
            return true;
        }

        for (int i = 0; i < record.Count; i++)
        {
            list.Add(_itemsConverter.Parse(record[i]));
        }

        result = list;
        return true;
    }
}



public abstract class CsvTypeConverter<T> : ICsvTypeConverter2
{
    protected CsvTypeConverter(bool throwsOnParseErrors, T? fallbackValue = default)
    {
        ThrowsOnParseErrors = throwsOnParseErrors;
        FallbackValue = fallbackValue;
    }

    public T? FallbackValue { get; }

    //object? ICsvTypeConverter2.FallbackValue => FallbackValue;


    public bool ThrowsOnParseErrors { get; }


    public abstract bool TryParseValue(string value, out T result);

    protected abstract string? DoConvertToString(T value);

    public string? ConvertToString(object? value)
    {
        if (value is null)
        {
            return null;
        }


        if (value is T t)
        {
            return DoConvertToString(t);
        }
        else
        {
            throw new InvalidCastException("Assignment of an incompliant Type.");
        }
    }


    public string? ConvertToString(T? value) => value is null ? null : DoConvertToString(value);


    public T? Parse(string? value)
    {
        if (value is null)
        {
            return FallbackValue;
        }

        if (TryParseValue(value, out T? result))
        {
            return result;
        }

        if (ThrowsOnParseErrors)
        {
            throw new ArgumentException(string.Format("Cannot convert {0} to {1}", value is null ? "null" : $"\"value\"", typeof(T)), nameof(value));
        }

        return FallbackValue;
    }

    object? ICsvTypeConverter2.Parse(string? value) => Parse(value);
}

public interface ICsvTypeConverter2
{
    object? Parse(string? value);

    string? ConvertToString(object? value);

    //bool ThrowsOnParseErrors { get; }

    //object? FallbackValue { get; }
}
