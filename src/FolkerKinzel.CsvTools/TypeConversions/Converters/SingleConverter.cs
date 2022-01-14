using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class SingleConverter : CsvTypeConverter<float>
{
    private readonly IFormatProvider? _formatProvider;

    private const string FORMAT = "G9";
    private const NumberStyles STYLE = NumberStyles.Any;


    public SingleConverter(bool throwing = true, IFormatProvider? formatProvider = null)
        : base(throwing) => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;


    protected override string? DoConvertToString(float value) => value.ToString(FORMAT, _formatProvider);


    public override bool TryParseValue(string value, out float result) => float.TryParse(value, STYLE, _formatProvider, out result);
}
