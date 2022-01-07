using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class Int64Converter : CsvTypeConverter<long>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;
    private readonly string? _format;

    private const NumberStyles DEFAULT_NUMBER_STYLE = NumberStyles.Any;


    public Int64Converter(IFormatProvider? formatProvider = null,
                          bool throwing = true,
                          NumberStyles styles = DEFAULT_NUMBER_STYLE,
                          long fallbackValue = default)
        : base(throwing, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = styles.HasFlag(NumberStyles.AllowHexSpecifier) ? "X" : null;
    }

    internal static ICsvTypeConverter Create(CsvConverterOptions options,
                                              IFormatProvider? formatProvider,
                                              bool hexConverter)
        => new Int64Converter(formatProvider,
                              options.HasFlag(CsvConverterOptions.Throwing),
                              hexConverter ? NumberStyles.HexNumber : DEFAULT_NUMBER_STYLE)
           .HandleNullableAndDBNullAcceptance(options);

    protected override string? DoConvertToString(long value) => value.ToString(_format, _formatProvider);

    public override bool TryParseValue(string value, out long result) => long.TryParse(value, _styles, _formatProvider, out result);

}
