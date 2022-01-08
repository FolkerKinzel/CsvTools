using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class StringConverter : CsvTypeConverter<string?>
{
    public StringConverter(bool nullable = true) : base(false, nullable ? null : string.Empty) { }

    //internal static ICsvTypeConverter Create(CsvConverterOptions options)
    //{
    //    var conv = new StringConverter(options.HasFlag(CsvConverterOptions.Nullable));
    //    return options.HasFlag(CsvConverterOptions.DBNullEnabled) ? conv.AsDBNullEnabled() : conv;
    //}

    protected override string? DoConvertToString(string? value) => value;

    protected override bool CsvHasValue([NotNullWhen(true)] string? csvInput) => csvInput is not null;

    public override bool TryParseValue(string value, out string? result)
    {
        result = value;
        return true;
    }
}
