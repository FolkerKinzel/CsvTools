using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class DateTimeConverter : CsvTypeConverter<DateTime>
{
    private const string DEFAULT_FORMAT = "s";
    private const string DATE_FORMAT = "d";

    private readonly IFormatProvider _formatProvider;
    private readonly string _format = DEFAULT_FORMAT;
    private readonly bool _parseExact;

    private const DateTimeStyles STYLE = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;


    public DateTimeConverter(bool throwing = true, IFormatProvider? formatProvider = null) : base(throwing)
        => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;


    /// <summary>
    /// Initialisiert ein neues <see cref="DateTimeConverter"/>-Objekt.
    /// </summary>
    /// <param name="format">Eine Formatzeichenfolge, die für die <see cref="string"/>-Ausgabe von <see cref="DateTime"/>-Werten verwendet 
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
    public DateTimeConverter(
        string format,
        bool throwing = true,
        IFormatProvider? formatProvider = null,
        bool parseExact = false) : base(throwing)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _format = format;
        _parseExact = parseExact;
        ExamineFormat();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeConverter CreateDateConverter(bool throwing = true, IFormatProvider? formatProvider = null, bool parseExact = false)
        => new(DATE_FORMAT, throwing, formatProvider, parseExact);


    protected override string? DoConvertToString(DateTime value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(ReadOnlySpan<char> value, [NotNullWhen(true)] out DateTime result)
#if NET461 || NETSTANDARD2_0
        => _parseExact
            ? DateTime.TryParseExact(value.ToString(), _format, _formatProvider, STYLE, out result)
            : DateTime.TryParse(value.ToString(), _formatProvider, STYLE, out result);
#else
        => _parseExact
            ? DateTime.TryParseExact(value, _format, _formatProvider, STYLE, out result)
            : DateTime.TryParse(value, _formatProvider, STYLE, out result);
#endif


    private void ExamineFormat()
    {
        try
        {
            string tmp = DateTime.Now.ToString(_format, _formatProvider);

            if (_parseExact)
            {
                _ = DateTime.ParseExact(tmp, _format, _formatProvider, STYLE);
            }
        }
        catch (FormatException e)
        {
            throw new ArgumentException(e.Message, e);
        }
    }
}
