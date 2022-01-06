namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class StringConverter2 : CsvTypeConverter<string?>
{
    public StringConverter2(string? fallbackValue = null) : base(false, fallbackValue) { }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options)
    {
        string? fallbackValue = options.HasFlag(CsvConverterOptions.Nullable) ? null : string.Empty;

        var conv = new StringConverter2(fallbackValue);

        return options.HasFlag(CsvConverterOptions.AcceptsDBNull) ? conv.AddDBNullAcceptance() : conv;
    }

    protected override string? DoConvertToString(string? value) => value;
    public override bool TryParseValue(string value, out string? result)
    {
        result = value;
        return true;
    }
}
