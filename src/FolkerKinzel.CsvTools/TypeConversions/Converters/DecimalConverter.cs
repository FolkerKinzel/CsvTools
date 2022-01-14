using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class DecimalConverter : CsvTypeConverter<decimal>
{
    private readonly IFormatProvider? _formatProvider;

    private const string FORMAT = "G";
    private const NumberStyles STYLE = NumberStyles.Any;


    public DecimalConverter(bool throwing = true, IFormatProvider? formatProvider = null)
        : base(throwing) => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;


    protected override string? DoConvertToString(decimal value) => value.ToString(FORMAT, _formatProvider);


    public override bool TryParseValue(string value, out decimal result) => decimal.TryParse(value, STYLE, _formatProvider, out result);
}
