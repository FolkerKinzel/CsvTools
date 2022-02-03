﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters;

public sealed class TimeSpanConverter : CsvTypeConverter<TimeSpan>
{
    private const string DEFAULT_FORMAT = "g";

    private readonly IFormatProvider _formatProvider;
    private readonly string _format = DEFAULT_FORMAT;
    private readonly bool _parseExact;
    private readonly TimeSpanStyles _styles;

    public TimeSpanConverter(bool throwing = true, IFormatProvider? formatProvider = null) : base(throwing)
        => _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;


    /// <summary>
    /// Initialisiert ein neues <see cref="TimeSpanConverter"/>-Objekt.
    /// </summary>
    /// <param name="format">Eine Formatzeichenfolge, die für die <see cref="string"/>-Ausgabe von <see cref="TimeSpan"/>-Werten verwendet 
    /// wird.
    /// Wenn die Option <paramref name="parseExact"/> gewählt ist, wird diese Formatzeichenfolge auch für das Parsen verwendet.</param>
    /// <param name="throwing">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
    /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
    /// <param name="formatProvider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
    /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
    /// <param name="parseExact">Wenn <c>true</c> als Argument übergeben wird, muss der Text in der CSV-Datei exakt mit der mit 
    /// <paramref name="format"/> angegebenen
    /// Formatzeichenfolge übereinstimmen.</param>
    /// <param name="styles">Ein Wert der <see cref="TimeSpanStyles"/>-Enumeration, der zusätzliche Informationen für das Parsen bereitstellt. Wird
    /// nur ausgewertet, wenn <paramref name="parseExact"/>&#160;<c>true</c> ist.</param>
    /// <exception cref="ArgumentNullException"><paramref name="format"/> ist <c>null</c> und <paramref name="parseExact"/> ist <c>true</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="format"/> ist keine gültige Formatzeichenfolge.</exception>
    public TimeSpanConverter(
        string format,
        bool throwing = true,
        IFormatProvider? formatProvider = null,
        bool parseExact = false,
        TimeSpanStyles styles = TimeSpanStyles.None) : base(throwing)
    {
        _formatProvider = formatProvider ?? CultureInfo.InvariantCulture;
        _styles = styles;
        _format = format;
        _parseExact = parseExact;
        ExamineFormat();
    }

    public override bool AcceptsNull => false;


    protected override string? DoConvertToString(TimeSpan value) => value.ToString(_format, _formatProvider);


    public override bool TryParseValue(ReadOnlySpan<char> value, [NotNullWhen(true)] out TimeSpan result)
#if NET461 || NETSTANDARD2_0
        => _parseExact
            ? TimeSpan.TryParseExact(value.ToString(), _format, _formatProvider, _styles, out result)
            : TimeSpan.TryParse(value.ToString(), _formatProvider, out result);
#else
        => _parseExact
            ? TimeSpan.TryParseExact(value, _format, _formatProvider, _styles, out result)
            : TimeSpan.TryParse(value, _formatProvider, out result);
#endif


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
