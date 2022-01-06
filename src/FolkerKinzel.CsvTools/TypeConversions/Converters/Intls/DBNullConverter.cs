namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

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
