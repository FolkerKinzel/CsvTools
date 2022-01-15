using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class ByteArrayConverter : CsvTypeConverter<byte[]?>
{
    public ByteArrayConverter(bool throwing = true, bool nullable = true)
        : base(throwing, nullable ? null : Array.Empty<byte>()) { }
    
    protected override string? DoConvertToString(byte[]? value) => value is null ? null : Convert.ToBase64String(value, Base64FormattingOptions.None);

    public override bool TryParseValue(ReadOnlySpan<char> value, [NotNullWhen(true)] out byte[]? result)
    {
        try
        {
            result = (byte[])Convert.FromBase64String(value.ToString());
            return true;
        }
        catch (FormatException)
        {
            result = null;
            return false;
        }
    }
}
