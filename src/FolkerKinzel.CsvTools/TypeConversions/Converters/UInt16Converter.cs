using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

[CLSCompliant(false)]
public sealed class UInt16Converter : CsvTypeConverter<ushort>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Any;


    public UInt16Converter(IFormatProvider? formatProvider = null,
                           bool throwsOnParseErrors = true,
                           NumberStyles styles = DEFAULT_NUMBER_STYLE,
                           ushort fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = styles.HasFlag(NumberStyles.AllowHexSpecifier) ? "X" : null;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options,
                                              IFormatProvider? formatProvider,
                                              bool hexConverter)
        => new UInt16Converter(formatProvider,
                               options.HasFlag(CsvConverterOptions.Throwing),
                               hexConverter ? NumberStyles.HexNumber : DEFAULT_NUMBER_STYLE)
            .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(ushort value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, out ushort result) => ushort.TryParse(value, _styles, _formatProvider, out result);
}
