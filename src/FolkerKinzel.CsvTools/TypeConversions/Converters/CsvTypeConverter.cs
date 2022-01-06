using System;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

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
