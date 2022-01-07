using System;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

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
