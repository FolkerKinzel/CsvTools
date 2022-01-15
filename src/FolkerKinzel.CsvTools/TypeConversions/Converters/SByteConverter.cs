using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

[CLSCompliant(false)]
public sealed class SByteConverter : CsvTypeConverter<sbyte>
{
    private const NumberStyles DEFAULT_STYLE = NumberStyles.Any;
    private const NumberStyles HEX_STYLE = NumberStyles.HexNumber;
    private const string HEX_FORMAT = "X";

    private readonly IFormatProvider? _formatProvider;
    private NumberStyles _styles = DEFAULT_STYLE;
    private string? _format = DEFAULT_FORMAT;

    
    private const string? DEFAULT_FORMAT = null;

    public SByteConverter(bool throwing = true, IFormatProvider? formatProvider = null)
        : base(throwing) => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
    

    public SByteConverter AsHexConverter()
    {
        _styles = HEX_STYLE;
        _format = HEX_FORMAT;
        return this;
    }


    protected override string? DoConvertToString(sbyte value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(ReadOnlySpan<char> value, out sbyte result)
#if NET461 || NETSTANDARD2_0
        => sbyte.TryParse(value.ToString(), _styles, _formatProvider, out result);
#else
        => sbyte.TryParse(value, _styles, _formatProvider, out result);
#endif
}
