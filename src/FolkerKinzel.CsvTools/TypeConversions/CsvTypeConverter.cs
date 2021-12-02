using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.TypeConversions;

internal sealed class NumberConverter2<T> : CsvTypeConverter<T> where T : IConvertible
{
    public NumberConverter2(bool maybeDBNull, T? fallbackValue, IFormatProvider? formatProvider, bool throwOnParseErrors) 
        : base(maybeDBNull, fallbackValue, throwOnParseErrors)
    {

    }

    protected override string? DoConvertToString(T value)
    {

    }
    protected override bool TryParseValue(string value, [NotNullWhen(true)] out T? result)
    {
        result = default;
    }
}

public abstract class CsvTypeConverter<T>
{
    protected CsvTypeConverter(bool maybeDBNull, T? fallbackValue = default, bool throwOnParseErrors = false)
    {
        ThrowsOnParseErrors = throwOnParseErrors;
        FallbackValue = maybeDBNull ? DBNull.Value : fallbackValue;
        Type = fallbackValue is null ? typeof(T?) : typeof(T);
    }

    public object? FallbackValue { get; }

    public Type Type { get; }

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
            if(FallbackValue is null)
            {
                return null;
            }
        }

        return DoConvertToString((T)value!);
    }


    public object? Parse(string? value)
    {
        if(value is null)
        {
            return FallbackValue;
        }

        if(TryParseValue(value, out T? result))
        {
            return result;
        }

        if(ThrowsOnParseErrors)
        {
            throw new ArgumentException(string.Format("Cannot convert {0} to {1}", value is null ? "null" : $"\"value\"", typeof(T)), nameof(value));
        }

        return FallbackValue;
    }
}
