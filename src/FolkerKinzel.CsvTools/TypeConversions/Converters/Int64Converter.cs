using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class Int64Converter : CsvTypeConverter<long>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_STYLE = NumberStyles.Any;
    private const NumberStyles HEX_STYLE = NumberStyles.HexNumber;
    private const string HEX_FORMAT = "X";
    private const string? DEFAULT_FORMAT = null;

    public Int64Converter(bool throwing = true, bool hexConverter = false, IFormatProvider? formatProvider = null)
        : base(throwing)
    {
        if (hexConverter)
        {
            _styles = HEX_STYLE;
            _format = HEX_FORMAT;
            _formatProvider = CultureInfo.InvariantCulture;
        }
        else
        {
            _styles = DEFAULT_STYLE;
            _format = DEFAULT_FORMAT;
            _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        }
    }

    internal static ICsvTypeConverter Create(CsvConverterOptions options, IFormatProvider? formatProvider, bool hexConverter)
        => new Int64Converter(options.HasFlag(CsvConverterOptions.Throwing),
                             hexConverter,
                             formatProvider)
           .HandleNullableAndDBNullAcceptance(options);

    protected override string? DoConvertToString(long value) => value.ToString(_format, _formatProvider);

    public override bool TryParseValue(string value, out long result) => long.TryParse(value, _styles, _formatProvider, out result);

}
