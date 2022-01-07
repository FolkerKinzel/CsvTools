using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class DoubleConverter : CsvTypeConverter<double>
{
    private readonly IFormatProvider? _formatProvider;

    private const string FORMAT = "G17";
    private const NumberStyles STYLE = NumberStyles.Any;


    public DoubleConverter(bool throwing = true, IFormatProvider? formatProvider = null)
        : base(throwing) => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;

    internal static ICsvTypeConverter Create(CsvConverterOptions options, IFormatProvider? formatProvider)
        => new DoubleConverter(options.HasFlag(CsvConverterOptions.Throwing), formatProvider).HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(double value) => value.ToString(FORMAT, _formatProvider);


    public override bool TryParseValue(string value, out double result) => double.TryParse(value, STYLE, _formatProvider, out result);
}
