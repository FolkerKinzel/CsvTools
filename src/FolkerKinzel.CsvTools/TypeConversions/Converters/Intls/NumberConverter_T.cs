using System.Globalization;
using FolkerKinzel.CsvTools.Resources;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

/// <summary>
/// Generische Klasse, die Konvertierungsvorgänge für Datentypen übernimmt,
/// die das Interface <see cref="IConvertible"/> implementieren.
/// </summary>
/// <typeparam name="T">Eine struct, die <see cref="IConvertible"/> implementiert.</typeparam>
internal class NumberConverter<T> : ICsvTypeConverter where T : struct, IConvertible
{
    private readonly Converter<string?, object?> _converter;
    private readonly Converter<object?, string?> _toStringConverter;


    /// <summary>
    /// Initialisiert ein neues <see cref="NumberConverter{T}"/>-Objekt.
    /// </summary>
    /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}"/> akzeptiert und zurückgegeben, sonst
    /// <typeparamref name="T"/>.</param>
    /// <param name="maybeDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value">DBNull.Value</see> als Eingabe akzeptiert und bildet auch den
    /// Rückgabewert von <see cref="FallbackValue"/>.</param>
    /// <param name="formatProvider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
    /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
    /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
    /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
    public NumberConverter(
        bool nullable,
        bool maybeDBNull,
        IFormatProvider? formatProvider,
        bool throwOnParseErrors)
    {
        this.Type = nullable ? typeof(T?) : typeof(T);
        this.FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(T?) : default(T));

        formatProvider ??= CultureInfo.InvariantCulture;
        this.ThrowsOnParseErrors = throwOnParseErrors;

        _toStringConverter = nullable
            ? new Converter<object?, string?>
              (
                    o =>
                    {
                        if (o is null || ((o == DBNull.Value) && maybeDBNull))
                        {
                            return null;
                        }

                        try
                        {
                                // Cast nach T um InvalidCastException auszulösen bei falschem Typ:
                                return Convert.ToString((T)o, formatProvider);
                        }
                        catch (Exception e)
                        {
                            throw new InvalidCastException(e.Message, e);
                        }
                    }
              )
            : new Converter<object?, string?>
              (
                    o =>
                    {
                        if (o is null)
                        {
                            throw new InvalidCastException(Res.InvalidCastNullToValueType);
                        }

                        if ((o == DBNull.Value) && maybeDBNull)
                        {
                            return null;
                        }

                        try
                        {
                            return Convert.ToString((T)o, formatProvider);
                        }
                        catch (Exception e)
                        {
                            throw new InvalidCastException(e.Message, e);
                        }
                    }
             );

        _converter = new Converter<string?, object?>(
            s =>
            {
                if (s is null)
                {
                    return FallbackValue;
                }

                try
                {
                    return Convert.ChangeType(s, typeof(T), formatProvider);
                }
                catch
                {
                    if (throwOnParseErrors)
                    {
                        throw;
                    }

                    return FallbackValue;
                }
            });
    }

    /// <summary>
    /// Wert, der zurückgegeben wird, wenn <see cref="CsvColumnNameProperty"/> keine Daten
    /// in den Spalten der CSV-Datei (repräsentiert duch <see cref="CsvRecord"/>) findet oder wenn
    /// <see cref="Parse(string)"/> scheitert.
    /// </summary>
    public object? FallbackValue { get; }

    /// <summary>
    /// Der Datentyp, in den <see cref="NumberConverter{T}"/> konvertieren kann, bzw. den <see cref="NumberConverter{T}"/>
    /// in einen <see cref="string"/> umwandeln kann.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// <c>true</c> gibt an, dass eine Ausnahme geworfen wird, wenn <see cref="Parse(string)"/>
    /// scheitert. Anderenfalls wird in diesem Fall <see cref="FallbackValue"/> zurückgegeben.
    /// </summary>
    public bool ThrowsOnParseErrors { get; }

    /// <summary>
    /// Parst <paramref name="value"/> als <typeparamref name="T"/> oder <see cref="Nullable{T}"/> - je
    /// nachdem, welche Option im Konstruktor gewählt wurde.
    /// </summary>
    /// <param name="value">Der zu parsende <see cref="string"/>.</param>
    /// <returns>Ein <typeparamref name="T"/> bzw. <see cref="Nullable{T}"/> als Ergebnis
    /// des Parsens.</returns>
    /// <exception cref="FormatException"><paramref name="value"/> weist kein kompatibles Format auf. Die Ausnahme wird
    /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
    /// <exception cref="OverflowException"><paramref name="value"/> stellt eine Zahl außerhalb des Bereichs von <typeparamref name="T"/> dar.
    /// Die Ausnahme wird
    /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
    public object? Parse(string? value) => _converter(value);


    /// <summary>
    /// Gibt die Zeichenfolgendarstellung von <paramref name="value"/> zurück, wenn <paramref name="value"/> ein <typeparamref name="T"/> 
    /// bzw. <see cref="Nullable{T}"/> ist.
    /// </summary>
    /// <param name="value">Ein <typeparamref name="T"/> bzw. <see cref="Nullable{T}"/>. Der Typ von <paramref name="value"/> muss
    /// <see cref="Type"/> exakt entsprechen.</param>
    /// <returns>Eine Zeichenfolgendarstellung von <paramref name="value"/>.</returns>
    /// <exception cref="InvalidCastException"><paramref name="value"/> lässt sich nicht in den Datentyp <typeparamref name="T"/> 
    /// bzw. <see cref="Nullable{T}"/> umwandeln.</exception>
    public string? ConvertToString(object? value) => _toStringConverter(value);

}
