using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class ByteConverter : CsvTypeConverter<byte>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_STYLE = NumberStyles.Any;
    private const NumberStyles HEX_STYLE = NumberStyles.HexNumber;
    private const string HEX_FORMAT = "X";

    public ByteConverter(bool hexConverter = false, bool throwing = true, IFormatProvider? formatProvider = null)
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
            _format = null;
            _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        }
    }

    internal static ICsvTypeConverter Create(CsvConverterOptions options, IFormatProvider? formatProvider, bool hexConverter)
        => new ByteConverter(hexConverter,
                             options.HasFlag(CsvConverterOptions.Throwing),
                             formatProvider)
           .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(byte value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, out byte result) => byte.TryParse(value, _styles, _formatProvider, out result);
}
