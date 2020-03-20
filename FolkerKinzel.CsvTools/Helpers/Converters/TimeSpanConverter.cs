using FolkerKinzel.CsvTools.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools.Helpers.Converters
{
    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="TimeSpan"/>-Datentyps.
    /// </summary>
    public sealed class TimeSpanConverter : ICsvTypeConverter
    {
        private readonly Converter<string?, object?> _parser;
        private readonly Converter<object?, string?> _toStringConverter;

        /// <summary>
        /// Initialisiert ein <see cref="TimeSpanConverter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;TimeSpan&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="TimeSpan"/>.</param>
        /// <param name="maybeDBNull">Wenn true, wird DBNull.Value als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
        /// <remarks>
        /// <para>
        /// Sie sollten diesen Konstruktor nicht direkt aufrufen, sondern über 
        /// <see cref="CsvConverterFactory.CreateConverter(CsvTypeCode, bool, bool, IFormatProvider, bool)"/> (für Konsistenz in Ihrem Code).
        /// </para>
        /// <para>
        /// Diese Überladung des Konstruktors ist wesentlich performanter als
        /// <see cref="TimeSpanConverter.TimeSpanConverter(string, bool, bool, IFormatProvider, bool, TimeSpanStyles, bool)"/>, bietet
        /// aber weniger Einstellmöglichkeiten: Bei der <see cref="string"/>-Ausgabe wird das Standardformat "g" verwendet. Beim Parsen kommt
        /// <see cref="TimeSpan.Parse(string, IFormatProvider)"/> zum Einsatz.
        /// </para></remarks>
        public TimeSpanConverter(
            bool nullable = false,
            bool maybeDBNull = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false)
        {
            this.ThrowsOnParseErrors = throwOnParseErrors;
            provider ??= CultureInfo.InvariantCulture;

            this.Type = nullable ? typeof(TimeSpan?) : typeof(TimeSpan);
            this.FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(TimeSpan?) : default(TimeSpan));

            const string format = "g";

            if (nullable)
            {
                // Cast nach T um InvalidCastException auszulösen bei falschem Typ:
                _toStringConverter = new Converter<object?, string?>(
                o =>
                {
                    if (o is null) return null;
                    if (Convert.IsDBNull(o) && maybeDBNull) return null;

                    return ((TimeSpan)o).ToString(format, provider);
                });

                _parser = new Converter<string?, object?>(
                s =>
                {
                    if (s is null) return null;

                    try
                    {
                        return TimeSpan.Parse(s, provider);
                    }
                    catch
                    {
                        if (throwOnParseErrors) throw;

                        return FallbackValue;
                    }
                });
            }
            else
            {
                _toStringConverter = new Converter<object?, string?>(
                o =>
                {
                    if (Convert.IsDBNull(o) && maybeDBNull) return null;
                    if (o is null) throw new InvalidCastException(Res.InvalidCastNullToValueType);

                    return ((TimeSpan)o).ToString(format, provider);
                });

                
                _parser = new Converter<string?, object?>(
                s =>
                {
                    if (s is null) return FallbackValue;

                    try
                    {
                        return TimeSpan.Parse(s, provider);
                    }
                    catch
                    {
                        if (throwOnParseErrors) throw;

                        return FallbackValue;
                    }
                });
            }
        }


        /// <summary>
        /// Initialisiert ein <see cref="TimeSpanConverter"/>-Objekt.
        /// </summary>
        /// <param name="format">Ein Formatstring, der für die <see cref="string"/>-Ausgabe von <see cref="TimeSpan"/>-Werten verwendet wird.
        /// Wenn die Option <paramref name="parseExact"/> gewählt ist, wird dieser Formatstring auch für das Parsen verwendet.</param>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;TimeSpan&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="TimeSpan"/>.</param>
        /// <param name="maybeDBNull">Wenn true, wird DBNull.Value als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
        /// <param name="styles">Ein Wert der <see cref="TimeSpanStyles"/>-Enum, der zusätzliche Informationen für das Parsen bereitstellt. Wird
        /// nur ausgewertet, wenn <paramref name="parseExact"/> true ist.</param>
        /// <param name="parseExact">Wenn true, muss der Text in der CSV-Datei exakt dem mit <paramref name="format"/> angegebenen
        /// Formatstring entsprechen.</param>
        /// <exception cref="ArgumentException"><paramref name="format"/> ist kein gültiger Formatstring.</exception>
        public TimeSpanConverter(
            string? format,
            bool nullable = false,
            bool maybeDBNull = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false,
            TimeSpanStyles styles = TimeSpanStyles.None,
            bool parseExact = false)
        {
            this.ThrowsOnParseErrors = throwOnParseErrors;
            provider ??= CultureInfo.InvariantCulture;

            this.Type = nullable ? typeof(TimeSpan?) : typeof(TimeSpan);
            this.FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(TimeSpan?) : default(TimeSpan));


            try
            {
                _ = TimeSpan.Zero.ToString(format, provider);
            }
            catch (FormatException e)
            {
                throw new ArgumentException(e.Message, nameof(format), e);
            }



            if (nullable)
            {
                // Cast nach T um InvalidCastException auszulösen bei falschem Typ:
                _toStringConverter = new Converter<object?, string?>(
                o =>
                {
                    if (o is null) return null;
                    if (Convert.IsDBNull(o) && maybeDBNull) return null;

                    return ((TimeSpan)o).ToString(format, provider);
                });


                if (parseExact)
                {
                    _parser = new Converter<string?, object?>(
                    s =>
                    {
                        if (s is null) return null;

                        try
                        {
                            return TimeSpan.ParseExact(s, format, provider, styles);
                        }
                        catch
                        {
                            if (throwOnParseErrors) throw;

                            return FallbackValue;
                        }
                    });
                }
                else
                {
                    _parser = new Converter<string?, object?>(
                   s =>
                   {
                       if (s is null) return null;

                       try
                       {
                           return TimeSpan.Parse(s, provider);
                       }
                       catch
                       {
                           if (throwOnParseErrors) throw;

                           return FallbackValue;
                       }
                   });
                }
            }
            else
            {
                _toStringConverter = new Converter<object?, string?>(
                o =>
                {
                    if (Convert.IsDBNull(o) && maybeDBNull) return null;
                    if (o is null) throw new InvalidCastException(Res.InvalidCastNullToValueType);

                    return ((TimeSpan)o).ToString(format, provider);
                });

                if (parseExact)
                {
                    _parser = new Converter<string?, object?>(
                    s =>
                    {
                        if (s is null) return FallbackValue;

                        try
                        {
                            return TimeSpan.ParseExact(s, format, provider, styles);
                        }
                        catch
                        {
                            if (throwOnParseErrors) throw;

                            return FallbackValue;
                        }
                    });
                }
                else
                {
                    _parser = new Converter<string?, object?>(
                    s =>
                    {
                        if (s is null) return FallbackValue;

                        try
                        {
                            return TimeSpan.Parse(s, provider);
                        }
                        catch
                        {
                            if (throwOnParseErrors) throw;

                            return FallbackValue;
                        }
                    });
                }
            }
        }


        /// <summary>
        /// Wert, der zurückgegeben wird, wenn <see cref="CsvProperty"/> keine Daten
        /// in den Spalten der CSV-Datei (repräsentiert duch <see cref="CsvRecord"/>) findet oder wenn
        /// von <see cref="Parse(string)"/> scheitert.
        /// </summary>
        public object? FallbackValue { get; }

        /// <summary>
        /// Der Datentyp, in den <see cref="TimeSpanConverter"/> konvertieren kann, bzw. den <see cref="TimeSpanConverter"/>
        /// in einen <see cref="string"/> umwandeln kann. (<c>typeof(TimeSpan)</c> oder <c>typeof(TimeSpan?)</c>)
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// True gibt an, dass eine Ausnahme geworfen wird, wenn <see cref="Parse(string)"/>
        /// scheitert. Anderenfalls wird in diesem Fall <see cref="FallbackValue"/> zurückgegeben.
        /// </summary>
        public bool ThrowsOnParseErrors { get; }

        /// <summary>
        /// Parst <paramref name="value"/> als <see cref="TimeSpan"/> oder <see cref="Nullable{T}">Nullable&lt;TimeSpan&gt;</see> - je
        /// nachdem, welche Option im Konstruktor gewählt wurde.
        /// </summary>
        /// <param name="value">Der zu parsende <see cref="string"/>.</param>
        /// <returns>Ein <see cref="TimeSpan"/> bzw. <see cref="Nullable{T}">Nullable&lt;TimeSpan&gt;</see> als Ergebnis
        /// des Parsens.</returns>
        /// <exception cref="FormatException"><paramref name="value"/> weist kein kompatibles Format auf. Die Ausnahme wird
        /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
        /// <exception cref="OverflowException"><paramref name="value"/> stellt eine Zahl außerhalb des Bereichs von <see cref="TimeSpan"/> dar.
        /// Die Ausnahme wird
        /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
        public object? Parse(string? value) => _parser(value);


        /// <summary>
        /// Erzeugt die Zeichenfolgendarstellung von <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Ein <see cref="TimeSpan"/> bzw. <see cref="Nullable{T}">Nullable&lt;TimeSpan&gt;</see>.</param>
        /// <returns>Eine Zeichenfolgendarstellung von <paramref name="value"/>.</returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> ist kein <see cref="TimeSpan"/> 
        /// bzw. <see cref="Nullable{T}">Nullable&lt;TimeSpan&gt;</see>.</exception>
        public string? ConvertToString(object? value) => _toStringConverter(value);

    }
}
