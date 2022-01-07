namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class BooleanConverter : CsvTypeConverter<bool>
{
    public BooleanConverter(bool throwing = true, bool fallbackValue = default)
        : base(throwing, fallbackValue) { }
    

    internal static ICsvTypeConverter Create(CsvConverterOptions options)
        => new BooleanConverter(options.HasFlag(CsvConverterOptions.Throwing))
           .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(bool value) => value.ToString();


    public override bool TryParseValue(string value, out bool result) => bool.TryParse(value, out result);
}
