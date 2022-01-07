namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class StringConverter : CsvTypeConverter<string?>
{
    public StringConverter(string? fallbackValue = null) : base(false, fallbackValue) { }

    internal static ICsvTypeConverter Create(CsvConverterOptions options)
    {
        string? fallbackValue = options.HasFlag(CsvConverterOptions.Nullable) ? null : string.Empty;

        var conv = new StringConverter(fallbackValue);

        return options.HasFlag(CsvConverterOptions.DBNullEnabled) ? conv.AsDBNullEnabled() : conv;
    }

    protected override string? DoConvertToString(string? value) => value;
    public override bool TryParseValue(string value, out string? result)
    {
        result = value;
        return true;
    }
}
