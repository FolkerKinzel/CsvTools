namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class CharConverter : CsvTypeConverter<char>
{
    public CharConverter(bool throwing = true, char fallbackValue = default)
        : base(throwing, fallbackValue) { }



    protected override string? DoConvertToString(char value) => value.ToString();


    public override bool TryParseValue(string value, out char result) => char.TryParse(value, out result);
}
