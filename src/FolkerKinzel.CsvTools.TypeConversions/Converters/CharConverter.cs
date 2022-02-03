namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class CharConverter : CsvTypeConverter<char>
{
    public CharConverter(bool throwing = true, char fallbackValue = default)
        : base(throwing, fallbackValue) { }


    protected override string? DoConvertToString(char value) => value.ToString();

    public override bool AcceptsNull => false;


    public override bool TryParseValue(ReadOnlySpan<char> value, out char result)
    {
        if (value.Trim().Length == 1)
        {
            result = value[0];
            return true;
        }

        result = default;
        return false;
    }

}
