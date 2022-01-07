using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class DoubleConverter : CsvTypeConverter<double>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;

    public DoubleConverter(IFormatProvider? formatProvider = null, bool throwing = true, NumberStyles styles = NumberStyles.Any, double fallbackValue = default)
        : base(throwing, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
    }

    internal static ICsvTypeConverter Create(CsvConverterOptions options, IFormatProvider? formatProvider)
        => new DoubleConverter(formatProvider, options.HasFlag(CsvConverterOptions.Throwing)).HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(double value) => value.ToString(_formatProvider);


    public override bool TryParseValue(string value, out double result) => double.TryParse(value, _styles, _formatProvider, out result);
}
