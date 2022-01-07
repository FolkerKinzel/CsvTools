using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class TimeSpanConverter : CsvTypeConverter<TimeSpan>
{
    private readonly IFormatProvider _formatProvider;
    private readonly string _format;
    private readonly bool _parseExact;
    private readonly TimeSpanStyles _styles;

    public TimeSpanConverter(IFormatProvider? formatProvider = null, bool throwing = true) : base(throwing)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = "g";
    }

    internal static ICsvTypeConverter Create(CsvConverterOptions options, IFormatProvider? formatProvider)
       => new TimeSpanConverter(formatProvider, options.HasFlag(CsvConverterOptions.Throwing))
        .HandleNullableAndDBNullAcceptance(options);


    public TimeSpanConverter(
        string format,
        IFormatProvider? formatProvider = null,
        TimeSpan fallbackValue = default,
        bool throwOnParseErrors = false,
        TimeSpanStyles styles = TimeSpanStyles.None,
        bool parseExact = false) : base(throwOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = format ?? throw new ArgumentNullException(nameof(format));
        _parseExact = parseExact;
        ExamineFormat();
    }


    protected override string? DoConvertToString(TimeSpan value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, [NotNullWhen(true)] out TimeSpan result)
        => _parseExact
            ? TimeSpan.TryParseExact(value, _format, _formatProvider, _styles, out result)
            : TimeSpan.TryParse(value, _formatProvider, out result);


    private void ExamineFormat()
    {
        try
        {
            string tmp = TimeSpan.Zero.ToString(_format, _formatProvider);

            if (_parseExact)
            {
                _ = TimeSpan.ParseExact(tmp, _format, _formatProvider, _styles);
            }
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }
}
