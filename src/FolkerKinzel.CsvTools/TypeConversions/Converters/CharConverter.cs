namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class CharConverter : CsvTypeConverter<char>
{
    public CharConverter(bool throwsOnParseErrors = true, char fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue) { }


    internal static ICsvTypeConverter Create(CsvConverterOptions options)
        => new CharConverter(options.HasFlag(CsvConverterOptions.Throwing))
            .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(char value) => value.ToString();


    public override bool TryParseValue(string value, out char result) => char.TryParse(value, out result);
}
