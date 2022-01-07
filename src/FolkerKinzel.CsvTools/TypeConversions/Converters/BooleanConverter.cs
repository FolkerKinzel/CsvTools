namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class BooleanConverter : CsvTypeConverter<bool>
{
    public BooleanConverter(bool throwsOnParseErrors = true, bool fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue) { }
    

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options)
        => new BooleanConverter(options.HasFlag(CsvConverterOptions.Throwing))
           .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(bool value) => value.ToString();


    public override bool TryParseValue(string value, out bool result) => bool.TryParse(value, out result);
}
