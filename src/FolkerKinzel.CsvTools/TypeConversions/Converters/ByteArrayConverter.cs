using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class ByteArrayConverter : CsvTypeConverter<byte[]?>
{
    public ByteArrayConverter(bool throwing = true, bool nullable = true) 
        : base(throwing, nullable ? null : Array.Empty<byte>()) { }
    

//    internal static ICsvTypeConverter Create(CsvConverterOptions options)
//    {
////#if NET40
////            byte[]? fallbackValue = options.HasFlag(CsvConverterOptions.Nullable) ? null : new byte[0];
////#else
////        byte[]? fallbackValue = options.HasFlag(CsvConverterOptions.Nullable) ? null : Array.Empty<byte>();
////#endif

//        var conv = new ByteArrayConverter(options.HasFlag(CsvConverterOptions.Throwing), options.HasFlag(CsvConverterOptions.Nullable));

//        return options.HasFlag(CsvConverterOptions.DBNullEnabled) ? conv.AsDBNullEnabled() : conv;
//    }

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
