using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class StringConverter : CsvTypeConverter<string?>
{
    public StringConverter(bool nullable = true) : base(false, nullable ? null : string.Empty) { }


    protected override string? DoConvertToString(string? value) => value;

    protected override bool CsvHasValue([NotNullWhen(true)] string? csvInput) => csvInput is not null;

    public override bool TryParseValue(string value, out string? result)
    {
        result = value;
        return true;
    }
}
