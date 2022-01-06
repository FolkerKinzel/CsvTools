using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class DoubleConverter : CsvTypeConverter<double>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;

    public DoubleConverter(IFormatProvider? formatProvider = null, bool throwsOnParseErrors = true, NumberStyles styles = NumberStyles.Any, double fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options, IFormatProvider? formatProvider)
        => new DoubleConverter(formatProvider, options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors)).HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(double value) => value.ToString(_formatProvider);


    public override bool TryParseValue(string value, out double result) => double.TryParse(value, _styles, _formatProvider, out result);
}
