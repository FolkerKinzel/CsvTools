﻿using System;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public abstract class CsvTypeConverter<T> : ICsvTypeConverter
{
    protected CsvTypeConverter(bool throwsOnParseErrors, T? fallbackValue = default)
    {
        Throwing = throwsOnParseErrors;
        FallbackValue = fallbackValue;
    }

    public T? FallbackValue { get; }

    object? ICsvTypeConverter.FallbackValue => FallbackValue;


    public bool Throwing { get; }


    public abstract bool TryParseValue(ReadOnlySpan<char> value, out T result);

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


    protected virtual bool CsvHasValue(ReadOnlySpan<char> csvInput) => !csvInput.IsWhiteSpace();

    public T? Parse(ReadOnlySpan<char> value)
    {
        if (!CsvHasValue(value))
        {
            return FallbackValue;
        }

        if (TryParseValue(value, out T? result))
        {
            return result;
        }

        if (Throwing)
        {
            throw new FormatException(
                string.Format("Cannot convert {0} into {1}.",
                value.Length > 40 ? nameof(value) : $"\"{value.ToString()}\"",
                typeof(T)));
        }

        return FallbackValue;
    }

    object? ICsvTypeConverter.Parse(ReadOnlySpan<char> value) => Parse(value);
}
