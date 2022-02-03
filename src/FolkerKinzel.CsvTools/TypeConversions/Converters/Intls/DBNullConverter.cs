namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

internal sealed class DBNullConverter<T> : CsvTypeConverter<object>
{
    private readonly CsvTypeConverter<T> _valueConverter;

    public override bool AcceptsNull => _valueConverter.AcceptsNull;

    internal DBNullConverter(CsvTypeConverter<T> converter)
        : base((converter ?? throw new ArgumentNullException(nameof(converter))).Throwing, DBNull.Value)
        => _valueConverter = converter;

    protected override string? DoConvertToString(object value) => value == DBNull.Value ? null : _valueConverter.ConvertToString(value);

    public override bool TryParseValue(ReadOnlySpan<char> value, out object result)
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
