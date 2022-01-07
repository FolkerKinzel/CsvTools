using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class DateTimeOffsetConverter : CsvTypeConverter<DateTimeOffset>
{
    private readonly IFormatProvider _formatProvider;
    private readonly string _format;
    private readonly bool _parseExact;

    private const DateTimeStyles STYLE = DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;

    public DateTimeOffsetConverter(bool throwing = true, IFormatProvider? formatProvider = null) : base(throwing)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = "O";
    }

    internal static ICsvTypeConverter Create(CsvConverterOptions options, IFormatProvider? formatProvider)
        => new DateTimeOffsetConverter(options.HasFlag(CsvConverterOptions.Throwing), formatProvider)
        .HandleNullableAndDBNullAcceptance(options);


    /// <summary>
    /// Initialisiert ein neues <see cref="DateTimeOffsetConverter"/>-Objekt.
    /// </summary>
    /// <param name="format">Eine Formatzeichenfolge, die für die <see cref="string"/>-Ausgabe von <see cref="DateTimeOffset"/>-Werten verwendet 
    /// wird.
    /// Wenn die Option <paramref name="parseExact"/> gewählt ist, wird diese Formatzeichenfolge auch für das Parsen verwendet.</param>
    /// <param name="throwing">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
    /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
    /// <param name="formatProvider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
    /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
    /// <param name="parseExact">Wenn <c>true</c> als Argument übergeben wird, muss der Text in der CSV-Datei exakt mit der mit 
    /// <paramref name="format"/> angegebenen
    /// Formatzeichenfolge übereinstimmen.</param>
    /// <exception cref="ArgumentNullException"><paramref name="format"/> ist <c>null</c> und <paramref name="parseExact"/> ist <c>true</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="format"/> ist keine gültige Formatzeichenfolge.</exception>
    public DateTimeOffsetConverter(
        string format,
        bool throwing = true,
        IFormatProvider? formatProvider = null,
        bool parseExact = false) : base(throwing)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = format;
        _parseExact = parseExact;
        ExamineFormat(nameof(format));
    }


    protected override string? DoConvertToString(DateTimeOffset value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(string value, [NotNullWhen(true)] out DateTimeOffset result)
        => _parseExact
            ? DateTimeOffset.TryParseExact(value, _format, _formatProvider, STYLE, out result)
            : DateTimeOffset.TryParse(value, _formatProvider, STYLE, out result);


    private void ExamineFormat(string paramName)
    {
        try
        {
            string tmp = DateTimeOffset.Now.ToString(_format, _formatProvider);
            if (_parseExact)
            {
                _ = DateTimeOffset.ParseExact(tmp, _format, _formatProvider, STYLE);
            }
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, paramName, e);
        }
    }
}
