using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

internal class Base64Converter2 : CsvTypeConverter<byte[]>
{
    public Base64Converter2(object? fallbackValue, bool throwOnParseErrors) : base(fallbackValue, throwOnParseErrors) { }

    protected override string? DoConvertToString(byte[] value) => Convert.ToBase64String(value, Base64FormattingOptions.None);

    protected override bool TryParseValue(string value, [NotNullWhen(true)] out byte[]? result)
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

internal class EnumConverter2<TEnum> : CsvTypeConverter<TEnum> where TEnum : struct, Enum
{
    internal EnumConverter2(
        bool ignoreCase,
        string? format,
        object? fallbackValue,
        bool throwOnParseErrors)
        : base(fallbackValue, throwOnParseErrors)
    {
        ValidateFormat(format);
        this.IgnoreCase = ignoreCase;
        this.Format = format;
    }

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

        throw new ArgumentException("Invalid format.", nameof(format));
    }

    internal bool IgnoreCase { get; }
    internal string? Format { get; }

    protected override string? DoConvertToString(TEnum value) => value.ToString(Format);

    protected override bool TryParseValue(string value, [NotNullWhen(true)] out TEnum result) => Enum.TryParse<TEnum>(value, IgnoreCase, out result);
}


internal sealed class NumberConverter2<T> : CsvTypeConverter<T> where T : struct, IConvertible
{
    internal NumberConverter2(object? fallbackValue, IFormatProvider? formatProvider, bool throwOnParseErrors)
        : base(fallbackValue, throwOnParseErrors) => FormatProvider = formatProvider;

    internal IFormatProvider? FormatProvider { get; }

    protected override string? DoConvertToString(T value) => Convert.ToString(value, FormatProvider);

    protected override bool TryParseValue(string value, [NotNullWhen(true)] out T result)
    {
        try
        {
            result = (T)Convert.ChangeType(value, typeof(T), FormatProvider);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

}


internal sealed class StringConverter2 : CsvTypeConverter<string>
{
    public StringConverter2(object? fallbackValue, bool throwOnParseErrors) : base(fallbackValue, throwOnParseErrors) { }

    protected override string? DoConvertToString(string value) => value;
    protected override bool TryParseValue(string value, [NotNullWhen(true)] out string? result)
    {
        result = value;
        return true;
    }
}


internal sealed class HexConverter2<T> : CsvTypeConverter<T> where T : struct, IConvertible
{
    private readonly bool _signed;

    public HexConverter2(object? fallbackValue, bool throwOnParseErrors) : base(fallbackValue, throwOnParseErrors)
        => _signed = Convert.ToBoolean(typeof(T).GetField("MinValue")?.GetValue(null));

    protected override string? DoConvertToString(T value)
    {
        const string format = "X";

        if (_signed)
        {
            long l = Convert.ToInt64(value, CultureInfo.InvariantCulture);
            return l.ToString(format, CultureInfo.InvariantCulture);
        }
        else
        {
            ulong l = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            return l.ToString(format, CultureInfo.InvariantCulture);
        }
    }

    protected override bool TryParseValue(string value, [NotNullWhen(true)] out T result)
    {
        const NumberStyles styles = NumberStyles.HexNumber;

        if (_signed)
        {
            if (long.TryParse(value, styles, CultureInfo.InvariantCulture, out long res))
            {
                result = (T)Convert.ChangeType(res, typeof(T), CultureInfo.InvariantCulture);
                return true;
            }
        }
        else
        {
            if (ulong.TryParse(value, styles, CultureInfo.InvariantCulture, out ulong res))
            {
                result = (T)Convert.ChangeType(res, typeof(T), CultureInfo.InvariantCulture);
                return true;
            }
        }

        result = default;
        return false;
    }
}


public class DateTimeConverter2 : CsvTypeConverter<DateTime>
{
    private readonly IFormatProvider _formatProvider;
    private readonly string _format;
    private readonly bool _parseExact;
    private readonly DateTimeStyles _styles;

    internal DateTimeConverter2(bool isDate, IFormatProvider? formatProvider, object? fallbackValue, bool throwOnParseErrors) : base(fallbackValue, throwOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = isDate ? "d" : "s";
        _styles = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;
    }

    public DateTimeConverter2(
        string? format,
        bool nullable = false,
        bool maybeDBNull = false,
        IFormatProvider? formatProvider = null,
        bool throwOnParseErrors = false,
        DateTimeStyles styles = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind,
        bool parseExact = false) : base(fallbackValue: maybeDBNull ? DBNull.Value : nullable ? null : default(DateTime),
                                        throwOnParseErrors: throwOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = format ?? string.Empty;
        _parseExact = parseExact;
        ExamineFormat();
    }


    protected override string? DoConvertToString(DateTime value) => value.ToString(_format, _formatProvider);


    protected override bool TryParseValue(string value, [NotNullWhen(true)] out DateTime result)
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


public class DateTimeOffsetConverter2 : CsvTypeConverter<DateTimeOffset>
{
    private readonly IFormatProvider _formatProvider;
    private readonly string _format;
    private readonly bool _parseExact;
    private readonly DateTimeStyles _styles;

    internal DateTimeOffsetConverter2(IFormatProvider? formatProvider, object? fallbackValue, bool throwOnParseErrors) : base(fallbackValue, throwOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = "O";
        _styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;
    }

    public DateTimeOffsetConverter2(
        string? format,
        bool nullable = false,
        bool maybeDBNull = false,
        IFormatProvider? formatProvider = null,
        bool throwOnParseErrors = false,
        DateTimeStyles styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind,
        bool parseExact = false) : base(fallbackValue: maybeDBNull ? DBNull.Value : nullable ? null : default(DateTime),
                                        throwOnParseErrors: throwOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = format ?? string.Empty;
        _parseExact = parseExact;
        ExamineFormat();
    }


    protected override string? DoConvertToString(DateTimeOffset value) => value.ToString(_format, _formatProvider);


    protected override bool TryParseValue(string value, [NotNullWhen(true)] out DateTimeOffset result)
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


public class TimeSpanConverter2 : CsvTypeConverter<TimeSpan>
{
    private readonly IFormatProvider _formatProvider;
    private readonly string _format;
    private readonly bool _parseExact;
    private readonly TimeSpanStyles _styles;

    internal TimeSpanConverter2(IFormatProvider? formatProvider, object? fallbackValue, bool throwOnParseErrors) : base(fallbackValue, throwOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = "g";
    }


    public TimeSpanConverter2(
        string? format,
        bool nullable = false,
        bool maybeDBNull = false,
        IFormatProvider? formatProvider = null,
        bool throwOnParseErrors = false,
        TimeSpanStyles styles = TimeSpanStyles.None,
        bool parseExact = false) : base(fallbackValue: maybeDBNull ? DBNull.Value : nullable ? null : default(DateTime),
                                        throwOnParseErrors: throwOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = format ?? string.Empty;
        _parseExact = parseExact;
        ExamineFormat();
    }


    protected override string? DoConvertToString(TimeSpan value) => value.ToString(_format, _formatProvider);


    protected override bool TryParseValue(string value, [NotNullWhen(true)] out TimeSpan result)
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



public class GuidConverter2 : CsvTypeConverter<Guid>
{
    private readonly string _format;
    private readonly bool _parseExact;
    private readonly TimeSpanStyles _styles;

    internal GuidConverter2(object? fallbackValue, bool throwOnParseErrors) : base(fallbackValue, throwOnParseErrors) => _format = "D";


    public GuidConverter2(
        string? format,
        bool nullable = false,
        bool maybeDBNull = false,
        bool throwOnParseErrors = false,
        TimeSpanStyles styles = TimeSpanStyles.None,
        bool parseExact = false) : base(fallbackValue: maybeDBNull ? DBNull.Value : nullable ? null : default(DateTime),
                                        throwOnParseErrors: throwOnParseErrors)
    {
        _styles = styles;
        _format = format ?? string.Empty;
        _parseExact = parseExact;
        ExamineFormat(nameof(format));
    }


    protected override string? DoConvertToString(Guid value) => value.ToString(_format, CultureInfo.InvariantCulture);


    protected override bool TryParseValue(string value, [NotNullWhen(true)] out Guid result)
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





public abstract class CsvTypeConverter<T>
{
    protected CsvTypeConverter(object? fallbackValue, bool throwOnParseErrors)
    {
        ThrowsOnParseErrors = throwOnParseErrors;

        FallbackValue = fallbackValue is T or null or DBNull
            ? fallbackValue
            : throw new ArgumentException(string.Format("The Type of {0} is not compliant.", nameof(fallbackValue)), nameof(fallbackValue));
    }

    public object? FallbackValue { get; }

    public bool ThrowsOnParseErrors { get; }

    protected abstract bool TryParseValue(string value, [NotNullWhen(true)] out T? result);

    protected abstract string? DoConvertToString(T value);

    public string? ConvertToString(object? value)
    {
        if (value == DBNull.Value)
        {
            if (FallbackValue == DBNull.Value)
            {
                return null;
            }
        }

        if (value is null)
        {
            if (FallbackValue is null)
            {
                return null;
            }
        }

        T? tVal;
        try
        {
            tVal = (T?)value;
        }
        catch (Exception ex)
        {
            throw new InvalidCastException("Assignment of an incompliant Type.", ex);
        }

        if (tVal is null)
        {
            return null;
        }

        return DoConvertToString(tVal);
    }


    public object? Parse(string? value)
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
}
