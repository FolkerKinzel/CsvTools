using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

[CLSCompliant(false)]
public sealed class UInt32Converter : CsvTypeConverter<uint>
{
    private const NumberStyles DEFAULT_STYLE = NumberStyles.Any;
    private const NumberStyles HEX_STYLE = NumberStyles.HexNumber;
    private const string HEX_FORMAT = "X";
    private const string? DEFAULT_FORMAT = null;

    private readonly IFormatProvider? _formatProvider;
    private  NumberStyles _styles = DEFAULT_STYLE;
    private  string? _format = DEFAULT_FORMAT;


    public UInt32Converter(bool throwing = true, IFormatProvider? formatProvider = null)
        : base(throwing) => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;


    public UInt32Converter AsHexConverter()
    {
        _styles = HEX_STYLE;
        _format = HEX_FORMAT;
        return this;
    }


    protected override string? DoConvertToString(uint value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(ReadOnlySpan<char> value, out uint result)
#if NET461 || NETSTANDARD2_0
        => uint.TryParse(value.ToString(), _styles, _formatProvider, out result);
#else
        => uint.TryParse(value, _styles, _formatProvider, out result);
#endif
}
