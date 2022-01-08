using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class Int32Converter : CsvTypeConverter<int>
{
    private const NumberStyles DEFAULT_STYLE = NumberStyles.Any;
    private const NumberStyles HEX_STYLE = NumberStyles.HexNumber;
    private const string HEX_FORMAT = "X";
    private const string? DEFAULT_FORMAT = null;

    private readonly IFormatProvider? _formatProvider;
    private NumberStyles _styles = DEFAULT_STYLE;
    private string? _format = DEFAULT_FORMAT;

    

    public Int32Converter(bool throwing = true, IFormatProvider? formatProvider = null)
        : base(throwing) => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
    

    public Int32Converter AsHexConverter()
    {
        _styles = HEX_STYLE;
        _format = HEX_FORMAT;
        return this;
    }

    //internal static ICsvTypeConverter Create(CsvConverterOptions options, IFormatProvider? formatProvider, bool hexConverter)
    //    => new Int32Converter(options.HasFlag(CsvConverterOptions.Throwing),
    //                         formatProvider)
    //       .HandleNullableAndDBNullAcceptance(options);

    protected override string? DoConvertToString(int value) => value.ToString(_format, _formatProvider);

    public override bool TryParseValue(string value, out int result) => int.TryParse(value, _styles, _formatProvider, out result);

}
