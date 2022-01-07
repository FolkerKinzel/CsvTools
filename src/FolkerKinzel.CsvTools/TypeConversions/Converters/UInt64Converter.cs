using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

[CLSCompliant(false)]
public sealed class UInt64Converter : CsvTypeConverter<ulong>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Any;


    public UInt64Converter(IFormatProvider? formatProvider = null, bool throwing = true, NumberStyles styles = DEFAULT_NUMBER_STYLE, ulong fallbackValue = default)
        : base(throwing, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = styles.HasFlag(NumberStyles.AllowHexSpecifier) ? "X" : null;
    }

    internal static ICsvTypeConverter Create(CsvConverterOptions options, IFormatProvider? formatProvider, bool hexConverter)
        => new UInt64Converter(formatProvider,
                               options.HasFlag(CsvConverterOptions.Throwing),
                               hexConverter ? NumberStyles.HexNumber : DEFAULT_NUMBER_STYLE)
           .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(ulong value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, out ulong result) => ulong.TryParse(value, _styles, _formatProvider, out result);
}
