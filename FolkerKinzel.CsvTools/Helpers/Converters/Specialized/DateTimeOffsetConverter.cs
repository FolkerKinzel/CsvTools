using FolkerKinzel.CsvTools.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Specialized
{
    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="DateTimeOffset"/>-Datentyps.
    /// </summary>
    public sealed class DateTimeOffsetConverter : ICsvTypeConverter
    {
        private readonly Converter<string?, object?> _parser;
        private readonly Converter<object?, string?> _toStringConverter;

        /// <summary>
        /// Initialisiert ein <see cref="DateTimeOffsetConverter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;DateTimeOffset&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="DateTimeOffset"/>.</param>
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
        /// <see cref="DateTimeOffsetConverter.DateTimeOffsetConverter(string, bool, bool, IFormatProvider, bool, DateTimeStyles, bool)"/>, bietet
        /// aber weniger Einstellmöglichkeiten: Bei der <see cref="string"/>-Ausgabe wird das Standardformat "O" verwendet. Beim Parsen kommt
        /// <see cref="DateTimeOffset.Parse(string, IFormatProvider, DateTimeStyles)"/> zum Einsatz. Der <see cref="DateTimeStyles"/>-Wert ist so
        /// eingestellt, dass Leerraum ignoriert wird (<see cref="DateTimeStyles.AllowWhiteSpaces"/>).
        /// </para></remarks>
        internal DateTimeOffsetConverter(
            bool nullable,
            bool maybeDBNull,
            IFormatProvider? provider,
            bool throwOnParseErrors)
        {
            this.ThrowsOnParseErrors = throwOnParseErrors;

            provider ??= CultureInfo.InvariantCulture;

            Type = nullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
            FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(DateTimeOffset?) : default(DateTimeOffset));


            const string format = "O";
            const DateTimeStyles styles = DateTimeStyles.AllowWhiteSpaces;

            if (nullable)
            {
                _toStringConverter = new Converter<object?, string?>(
                    o =>
                    {
                        if (o is null) return null;
                        if (Convert.IsDBNull(o) && maybeDBNull) return null;

                        return ((DateTimeOffset)o).ToString(format, provider);
                    });


                _parser = new Converter<string?, object?>(
                    s =>
                    {
                        if (s is null) return null;

                        try
                        {
                            return DateTimeOffset.Parse(s, provider, styles);
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

                    return ((DateTimeOffset)o).ToString(format, provider);
                });

                _parser = new Converter<string?, object?>(
                    s =>
                    {
                        if (s is null) return FallbackValue;

                        try
                        {
                            return DateTimeOffset.Parse(s, provider, styles);
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
        /// Initialisiert ein <see cref="DateTimeOffsetConverter"/>-Objekt.
        /// </summary>
        /// <param name="format">Ein Formatstring, der für die <see cref="string"/>-Ausgabe von <see cref="DateTimeOffset"/>-Werten verwendet wird.
        /// Wenn die Option <paramref name="parseExact"/> gewählt ist, wird dieser Formatstring auch für das Parsen verwendet.</param>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;DateTimeOffset&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="DateTimeOffset"/>.</param>
        /// <param name="maybeDBNull">Wenn true, wird DBNull.Value als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param> 
        /// <param name="styles">Ein Wert der <see cref="DateTimeStyles"/>-Enum, der zusätzliche Informationen für das Parsen bereitstellt. Wird
        /// nur ausgewertet, wenn <paramref name="parseExact"/> true ist.</param>
        /// <param name="parseExact">Wenn true, muss der Text in der CSV-Datei exakt dem mit <paramref name="format"/> angegebenen
        /// Formatstring entsprechen.</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="format"/> ist kein gültiger Formatstring - oder - <paramref name="styles"/> 
        /// enthält das Flag <see cref="DateTimeStyles.NoCurrentDateDefault"/>.</exception>
        /// <remarks>Wenn es genügt, dass bei der <see cref="string"/>-Ausgabe das Standardformat "O" verwendet wird, sollten Sie das <see cref="DateTimeOffsetConverter"/>-Objekt
        /// über die Methode <see cref="CsvConverterFactory.CreateConverter(CsvTypeCode, bool, bool, IFormatProvider?, bool)"/> initialisieren: Das ist 
        /// wesentlich performanter.</remarks>
        public DateTimeOffsetConverter(
            string format,
            bool nullable = false,
            bool maybeDBNull = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false,
            DateTimeStyles styles = DateTimeStyles.AllowWhiteSpaces,
            bool parseExact = false)
        {
            this.ThrowsOnParseErrors = throwOnParseErrors;

            provider ??= CultureInfo.InvariantCulture;

            Type = nullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
            FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(DateTimeOffset?) : default(DateTimeOffset));


            if (format is null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            try
            {
                _ = DateTimeOffset.Now.ToString(format, provider);
            }
            catch (FormatException e)
            {
                throw new ArgumentException(e.Message, nameof(format), e);
            }


            if ((styles & DateTimeStyles.NoCurrentDateDefault) == DateTimeStyles.NoCurrentDateDefault)
            {
                throw new ArgumentException(Res.NoNoCurrentDateDafault, nameof(styles));
            }



            if (nullable)
            {
                _toStringConverter = new Converter<object?, string?>(
                    o =>
                    {
                        if (o is null) return null;
                        if (Convert.IsDBNull(o) && maybeDBNull) return null;

                        return ((DateTimeOffset)o).ToString(format, provider);
                    });


                if (parseExact)
                {
                    _parser = new Converter<string?, object?>(
                        s =>
                        {
                            if (s is null) return null;

                            try
                            {
                                return DateTimeOffset.ParseExact(s, format, provider, styles);
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
                                return DateTimeOffset.Parse(s, provider, styles);
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

                    return ((DateTimeOffset)o).ToString(format, provider);
                });



                if (parseExact)
                {
                    _parser = new Converter<string?, object?>(
                        s =>
                        {
                            if (s is null) return FallbackValue;

                            try
                            {
                                return DateTimeOffset.ParseExact(s, format, provider, styles);
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
                                return DateTimeOffset.Parse(s, provider, styles);
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
        /// Der Datentyp, in den <see cref="DateTimeOffsetConverter"/> parsen bzw.
        /// in einen <see cref="string"/> umwandeln kann. (<c>typeof(DateTimeOffset)</c> oder <c>typeof(DateTimeOffset?)</c>)
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// True gibt an, dass eine Ausnahme geworfen wird, wenn <see cref="Parse(string)"/>
        /// scheitert. Anderenfalls wird in diesem Fall <see cref="FallbackValue"/> zurückgegeben.
        /// </summary>
        public bool ThrowsOnParseErrors { get; }

        /// <summary>
        /// Parst <paramref name="value"/> als <see cref="DateTimeOffset"/> oder <see cref="Nullable{T}">Nullable&lt;DateTimeOffset&gt;</see> - je
        /// nachdem, welche Option im Konstruktor gewählt wurde.
        /// </summary>
        /// <param name="value">Der zu parsende <see cref="string"/>.</param>
        /// <returns>Ein <see cref="DateTimeOffset"/> bzw. <see cref="Nullable{T}">Nullable&lt;DateTimeOffset&gt;</see> als Ergebnis
        /// des Parsens.</returns>
        /// <exception cref="FormatException"><paramref name="value"/> weist kein kompatibles Format auf. Die Ausnahme wird
        /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
        /// <exception cref="ArgumentException">Der Offset ist größer als 14 Stunden oder kleiner als -14 Stunden.
        /// Die Ausnahme wird nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird 
        /// <see cref="FallbackValue"/> zurückgegeben.</exception>
        public object? Parse(string? value) => _parser(value);


        /// <summary>
        /// Gibt die Zeichenfolgendarstellung von <paramref name="value"/> zurück, wenn <paramref name="value"/> ein <see cref="DateTimeOffset"/> 
        /// bzw. <see cref="Nullable{T}">Nullable&lt;DateTimeOffset&gt;</see> ist.
        /// </summary>
        /// <param name="value">Ein <see cref="DateTimeOffset"/> bzw. <see cref="Nullable{T}">Nullable&lt;DateTimeOffset&gt;</see>.</param>
        /// <returns>Eine Zeichenfolgendarstellung von <paramref name="value"/>.</returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> lässt sich nicht in den Datentyp <see cref="DateTimeOffset"/> 
        /// bzw. <see cref="Nullable{T}">Nullable&lt;DateTimeOffset&gt;</see> umwandeln.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Datum und Uhrzeit liegen außerhalb des Bereichs von Datumsangaben, die 
        /// vom Kalender der im Konstruktor zugewiesenen <see cref="CultureInfo"/> unterstützt werden.</exception>
        public string? ConvertToString(object? value) => _toStringConverter(value);

    }
}
