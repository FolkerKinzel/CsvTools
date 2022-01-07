using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class DateTimeConverter : CsvTypeConverter<DateTime>
{
    private readonly IFormatProvider _formatProvider;
    private readonly string _format;
    private readonly bool _parseExact;
    private readonly DateTimeStyles _styles;

    private DateTimeConverter(bool isDate, IFormatProvider? formatProvider, bool throwing) : base(throwing)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = isDate ? "d" : "s";
        _styles = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;
    }

    internal static ICsvTypeConverter Create(bool isDate, CsvConverterOptions options, IFormatProvider? formatProvider)
        => new DateTimeConverter(isDate, formatProvider, options.HasFlag(CsvConverterOptions.Throwing))
            .HandleNullableAndDBNullAcceptance(options);

    public DateTimeConverter(
        string? format,
        IFormatProvider? formatProvider = null,
        DateTime fallbackValue = default,
        bool throwing = false,
        DateTimeStyles styles = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind,
        bool parseExact = false) : base(throwing, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = format ?? string.Empty;
        _parseExact = parseExact;
        ExamineFormat();
    }


    protected override string? DoConvertToString(DateTime value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, [NotNullWhen(true)] out DateTime result)
        => _parseExact
            ? DateTime.TryParseExact(value, _format, _formatProvider, _styles, out result)
            : DateTime.TryParse(value, _formatProvider, _styles, out result);


    private void ExamineFormat()
    {
        try
        {
            string tmp = DateTime.Now.ToString(_format, _formatProvider);

            if (_parseExact)
            {
                _ = DateTime.ParseExact(tmp, _format, _formatProvider, _styles);
            }
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }
}
