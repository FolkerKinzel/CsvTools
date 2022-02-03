using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class SingleConverter : CsvTypeConverter<float>
{
    private readonly IFormatProvider? _formatProvider;

    private const string FORMAT = "G9";
    private const NumberStyles STYLE = NumberStyles.Any;


    public SingleConverter(bool throwing = true, IFormatProvider? formatProvider = null)
        : base(throwing) => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;

    public override bool AcceptsNull => false;


    protected override string? DoConvertToString(float value) => value.ToString(FORMAT, _formatProvider);


    public override bool TryParseValue(ReadOnlySpan<char> value, out float result)
#if NET461 || NETSTANDARD2_0
        => float.TryParse(value.ToString(), STYLE, _formatProvider, out result);
#else
        => float.TryParse(value, STYLE, _formatProvider, out result);
#endif
}
