using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;


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
