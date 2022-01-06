using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class FloatConverter : CsvTypeConverter<float>
{
    private readonly IFormatProvider? _formatProvider;
    private readonly NumberStyles _styles;

    public FloatConverter(IFormatProvider? formatProvider = null, bool throwsOnParseErrors = true, NumberStyles styles = NumberStyles.Any, float fallbackValue = default)
        : base(throwsOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options, IFormatProvider? formatProvider)
        => new FloatConverter(formatProvider, options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors))
        .HandleNullableAndDBNullAcceptance(options);


    protected override string? DoConvertToString(float value) => value.ToString(_formatProvider);


    public override bool TryParseValue(string value, out float result) => float.TryParse(value, _styles, _formatProvider, out result);
}
