using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class SingleConverter : CsvTypeConverter<float>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;

    public SingleConverter(IFormatProvider? formatProvider = null, bool throwing = true, NumberStyles styles = NumberStyles.Any, float fallbackValue = default)
        : base(throwing, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
    }

    internal static ICsvTypeConverter Create(CsvConverterOptions options, IFormatProvider? formatProvider)
        => new SingleConverter(formatProvider, options.HasFlag(CsvConverterOptions.Throwing))
        .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(float value) => value.ToString(_formatProvider);


    public override bool TryParseValue(string value, out float result) => float.TryParse(value, _styles, _formatProvider, out result);
}
