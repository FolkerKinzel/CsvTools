using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class DateTimeOffsetConverter2 : CsvTypeConverter<DateTimeOffset>
{
    private readonly IFormatProvider _formatProvider;
    private readonly string _format;
    private readonly bool _parseExact;
    private readonly DateTimeStyles _styles;

    private DateTimeOffsetConverter2(IFormatProvider? formatProvider, bool throwsOnParseErrors) : base(throwsOnParseErrors)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = "O";
        _styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;
    }

    internal static ICsvTypeConverter2 Create(CsvConverterOptions options, IFormatProvider? formatProvider)
        => new DateTimeOffsetConverter2(formatProvider, options.HasFlag(CsvConverterOptions.ThrowsOnParseErrors))
        .HandleNullableAndDBNullAcceptance(options);


    public DateTimeOffsetConverter2(
        string? format,

        IFormatProvider? formatProvider = null,
        DateTimeOffset fallbackValue = default,
        bool throwOnParseErrors = false,
        DateTimeStyles styles = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind,
        bool parseExact = false) : base(throwOnParseErrors, fallbackValue)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = format ?? string.Empty;
        _parseExact = parseExact;
        ExamineFormat();
    }


    protected override string? DoConvertToString(DateTimeOffset value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, [NotNullWhen(true)] out DateTimeOffset result)
        => _parseExact
            ? DateTimeOffset.TryParseExact(value, _format, _formatProvider, _styles, out result)
            : DateTimeOffset.TryParse(value, _formatProvider, _styles, out result);


    private void ExamineFormat()
    {
        try
        {
            string tmp = DateTimeOffset.Now.ToString(_format, _formatProvider);

            if (_parseExact)
            {
                _ = DateTimeOffset.ParseExact(tmp, _format, _formatProvider, _styles);
            }
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }
}
