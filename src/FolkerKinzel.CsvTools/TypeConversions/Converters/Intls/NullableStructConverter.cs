using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

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
        : base((converter ?? throw new ArgumentNullException(nameof(converter))).Throwing) => _typeConverter = converter;

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
