using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class DecimalConverter : CsvTypeConverter<decimal>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;

    public DecimalConverter(IFormatProvider? formatProvider = null, bool throwsOnParseErrors = true, NumberStyles styles = NumberStyles.Any, decimal fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
    }

    internal static ICsvTypeConverter Create(CsvConverterOptions options, IFormatProvider? formatProvider)
        => new DecimalConverter(formatProvider, options.HasFlag(CsvConverterOptions.Throwing))
          .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(decimal value) => value.ToString(_formatProvider);


    public override bool TryParseValue(string value, out decimal result) => decimal.TryParse(value, _styles, _formatProvider, out result);
}
