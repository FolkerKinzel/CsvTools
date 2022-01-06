using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class Base64Converter2 : CsvTypeConverter<byte[]?>
{
    public Base64Converter2(bool throwsOnParseErrors, byte[]? fallbackValue) : base(throwsOnParseErrors, fallbackValue) { }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options)
    {
#if NET40
            byte[]? fallbackValue = options.HasFlag(CsvConverterOptions.Nullable) ? null : new byte[0];
#else
        byte[]? fallbackValue = options.HasFlag(CsvConverterOptions.Nullable) ? null : Array.Empty<byte>();
#endif

        var conv = new Base64Converter2(options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors), fallbackValue);

        return options.HasFlag(CsvConverterOptions.AcceptsDBNull) ? conv.AddDBNullAcceptance() : conv;
    }

    protected override string? DoConvertToString(byte[]? value) => value is null ? null : Convert.ToBase64String(value, Base64FormattingOptions.None);

    public override bool TryParseValue(string value, [NotNullWhen(true)] out byte[]? result)
    {
        try
        {
            result = (byte[])Convert.FromBase64String(value);
            return true;
        }
        catch (FormatException)
        {
            result = null;
            return false;
        }
    }
}
