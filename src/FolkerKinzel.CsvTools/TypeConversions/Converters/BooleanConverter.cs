namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class BooleanConverter : CsvTypeConverter<bool>
{
    public BooleanConverter(bool throwing = true, bool fallbackValue = default)
        : base(throwing, fallbackValue) { }
    

    protected override string? DoConvertToString(bool value) => value.ToString();


    public override bool TryParseValue(ReadOnlySpan<char> value, out bool result)
#if NET461 || NETSTANDARD2_0
        => bool.TryParse(value.ToString(), out result);
#else
        => bool.TryParse(value, out result);
#endif
}
