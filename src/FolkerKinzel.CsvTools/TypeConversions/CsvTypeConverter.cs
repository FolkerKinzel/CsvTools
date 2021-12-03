using System;
using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.TypeConversions;

internal class Base64Converter2 : CsvTypeConverter<byte[]>
{
    public Base64Converter2(object? fallbackValue, bool throwOnParseErrors) : base(fallbackValue, throwOnParseErrors)
    {
    }

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
            result=null;
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
        if (format is null || format.Length == 0) return;

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
            result = default(T);
            return false;
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

        if(tVal is null)
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
