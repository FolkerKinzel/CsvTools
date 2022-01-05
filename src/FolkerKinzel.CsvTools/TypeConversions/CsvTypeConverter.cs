using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

internal class Base64Converter2 : CsvTypeConverter<byte[]?>
{
    public Base64Converter2(bool throwOnParseErrors, byte[]? fallbackValue) : base(throwOnParseErrors, fallbackValue) { }

    protected override string? DoConvertToString(byte[]? value) => value is null ? null : Convert.ToBase64String(value, Base64FormattingOptions.None);

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
        TEnum fallbackValue,
        bool throwOnParseErrors)
        : base(throwOnParseErrors, fallbackValue)
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

    protected override bool TryParseValue(string value, out TEnum result) => Enum.TryParse<TEnum>(value, IgnoreCase, out result);
}


internal sealed class NumberConverter2<T> : CsvTypeConverter<T> where T : struct, IConvertible
{
    internal NumberConverter2(T fallbackValue, IFormatProvider? formatProvider, bool throwOnParseErrors)
        : base(throwOnParseErrors, fallbackValue) => FormatProvider = formatProvider;

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


internal sealed class StringConverter2 : CsvTypeConverter<string?>
{
    public StringConverter2(string fallbackValue, bool throwOnParseErrors) : base(throwOnParseErrors, fallbackValue) { }

    protected override string? DoConvertToString(string? value) => value;
    protected override bool TryParseValue(string value, out string? result)
    {
        result = value;
        return true;
    }
}


internal sealed class HexConverter2<T> : CsvTypeConverter<T> where T : struct, IConvertible
{
    private readonly bool _signed;

    public HexConverter2(bool throwOnParseErrors) : base(throwOnParseErrors, default)
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

    internal DateTimeConverter2(bool isDate, IFormatProvider? formatProvider, bool throwOnParseErrors) : base(throwOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = isDate ? "d" : "s";
        _styles = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;
    }

    public DateTimeConverter2(
        string? format,
        IFormatProvider? formatProvider = null,
        DateTime fallbackValue = default,
        bool throwOnParseErrors = false,
        DateTimeStyles styles = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind,
        bool parseExact = false) : base(throwOnParseErrors, fallbackValue)
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

    internal DateTimeOffsetConverter2(IFormatProvider? formatProvider, bool throwOnParseErrors) : base(throwOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = "O";
        _styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;
    }

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

    internal TimeSpanConverter2(IFormatProvider? formatProvider, bool throwOnParseErrors) : base(throwOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = "g";
    }


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

    internal GuidConverter2(bool throwOnParseErrors) : base(throwOnParseErrors) => _format = "D";


    public GuidConverter2(
        string? format,
        bool throwOnParseErrors = false,
        TimeSpanStyles styles = TimeSpanStyles.None,
        bool parseExact = false) : base(throwOnParseErrors, default)
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

public class NullableStructConverter<T> : CsvTypeConverter<Nullable<T>> where T : struct
{
    private readonly CsvTypeConverter<T> _typeConverter;

    public NullableStructConverter(CsvTypeConverter<T> typeConverter) : base(false)
    {
        _typeConverter = typeConverter ?? throw new ArgumentNullException(nameof(typeConverter));
    }

    protected override string? DoConvertToString(T? value) => value.HasValue ? _typeConverter.ConvertToString(value.Value) : null;

    protected override bool TryParseValue(string value, [NotNullWhen(true)] out T? result)
    {
        result = _typeConverter.Parse(value);
        return true;
    }
}

public class EnumerableConverter<TItem> : CsvTypeConverter<IEnumerable<TItem?>?>
{
    private readonly char _separatorChar;

    public EnumerableConverter(CsvTypeConverter<TItem?> itemsConverter,
                                char fieldSeparator = ',',
                                bool nullable = true) : base(false)
    {
        ItemsConverter = itemsConverter ?? throw new ArgumentNullException(nameof(itemsConverter));

        _separatorChar = fieldSeparator;
    }

    protected CsvTypeConverter<TItem?> ItemsConverter { get; }

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
            csvWriter.Record.Fill(value.Select(x => ItemsConverter.ConvertToString(x)));
            csvWriter.WriteRecord();
        }
        return writer.ToString();
    }

    protected override bool TryParseValue(string value, out IEnumerable<TItem?>? result)
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
            list.Add(ItemsConverter.Parse(record[i]));
        }

        result = list;
        return true;
    }
}


public class DBNullConverter : CsvTypeConverter<object?>
{
    private readonly ICsvTypeConverter2 _valueConverter;

    public DBNullConverter(ICsvTypeConverter2 valueConverter) : base(false, DBNull.Value)
    {
        _valueConverter = valueConverter ?? throw new ArgumentNullException(nameof(valueConverter));
    }

    protected override string? DoConvertToString(object? value) => value == DBNull.Value ? null : _valueConverter.ConvertToString(value);
    protected override bool TryParseValue(string value, out object? result)
    {
        result = _valueConverter.Parse(value);
        return true;
    }
}


public abstract class CsvTypeConverter<T> : ICsvTypeConverter2
{
    protected CsvTypeConverter(bool throwOnParseErrors, T? fallbackValue = default)
    {
        ThrowsOnParseErrors = throwOnParseErrors;
        FallbackValue = fallbackValue;
    }

    public T? FallbackValue { get; }

    object? ICsvTypeConverter2.FallbackValue => FallbackValue;


    public bool ThrowsOnParseErrors { get; }


    protected abstract bool TryParseValue(string value, out T result);

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


    public string? ConvertToString(T value) => DoConvertToString(value);


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

    object? FallbackValue { get; }
}