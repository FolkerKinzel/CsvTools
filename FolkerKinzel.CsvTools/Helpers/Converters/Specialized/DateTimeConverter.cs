using FolkerKinzel.CsvTools.Resources;
using System;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Specialized
{
    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="DateTime"/>-Datentyps.
    /// </summary>
    public sealed class DateTimeConverter : ICsvTypeConverter
    {
        private readonly Converter<string?, object?> _parser;
        private readonly Converter<object?, string?> _toStringConverter;


        /// <summary>
        /// Initialisiert ein <see cref="DateTimeConverter"/>-Objekt.
        /// </summary>
        /// <param name="isDate">Wenn <c>true</c>, wird nur der Datumsteil gelesen und ausgegeben.</param>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;DateTime&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="DateTime"/>.</param>
        /// <param name="maybeDBNull">Wenn true, wird <see cref="DBNull.Value"/> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="formatProvider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param> 
        /// <param name="styles">Ein Wert der <see cref="DateTimeStyles"/>-Enum, der zusätzliche Informationen für das Parsen bereitstellt. Wird
        /// nur ausgewertet, wenn <paramref name="parseExact"/> true ist.</param>
        /// <param name="parseExact">Wenn true, muss der Text in der CSV-Datei exakt dem mit <paramref name="format"/> angegebenen
        /// Formatstring entsprechen.</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="format"/> ist kein gültiger Formatstring.</exception>
        /// <remarks>
        /// <para>
        /// Sie können diesen Konstruktor nicht direkt aufrufen, sondern über 
        /// <see cref="CsvConverterFactory.CreateConverter(CsvTypeCode, bool, bool, IFormatProvider, bool)"/>.
        /// </para>
        /// <para>
        /// Diese Überladung des Konstruktors ist wesentlich performanter als
        /// <see cref="DateTimeOffsetConverter.DateTimeConverter(string, bool, bool, IFormatProvider, bool, DateTimeStyles, bool)"/>, bietet
        /// aber weniger Einstellmöglichkeiten. Beim Parsen kommt
        /// <see cref="DateTimeOffset.Parse(string, IFormatProvider, DateTimeStyles)"/> zum Einsatz. Der <see cref="DateTimeStyles"/>-Wert ist so
        /// eingestellt, dass Leerraum ignoriert wird (<see cref="DateTimeStyles.AllowWhiteSpaces"/>).
        /// </para></remarks>
        internal DateTimeConverter(
            bool isDate,
            bool nullable,
            bool maybeDBNull,
            IFormatProvider? formatProvider,
            bool throwOnParseErrors)
        {
            this.ThrowsOnParseErrors = throwOnParseErrors;

            formatProvider ??= CultureInfo.InvariantCulture;

            Type = nullable ? typeof(DateTime?) : typeof(DateTime);
            FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(DateTime?) : default(DateTime));

            string format = isDate ? "d" : "s";
            DateTimeStyles styles = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind;


            if (nullable)
            {
                _toStringConverter = new Converter<object?, string?>(
                    o =>
                    {
                        if (o is null) return null;
                        if (Convert.IsDBNull(o) && maybeDBNull) return null;

                        return ((DateTime)o).ToString(format, formatProvider);
                    });


                _parser = new Converter<string?, object?>(
                    s =>
                    {
                        if (s is null) return null;

                        try
                        {
                            return DateTime.Parse(s, formatProvider, styles);
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

                    return ((DateTime)o).ToString(format, formatProvider);
                });

                _parser = new Converter<string?, object?>(
                    s =>
                    {
                        if (s is null) return FallbackValue;

                        try
                        {
                            return DateTime.Parse(s, formatProvider, styles);
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
        /// Initialisiert ein <see cref="DateTimeConverter"/>-Objekt.
        /// </summary>
        /// <param name="format">Ein Formatstring, der für die <see cref="string"/>-Ausgabe von <see cref="DateTime"/>-Werten verwendet wird.
        /// Wenn die Option <paramref name="parseExact"/> gewählt ist, wird dieser Formatstring auch für das Parsen verwendet.</param>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;DateTime&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="DateTime"/>.</param>
        /// <param name="maybeDBNull">Wenn true, wird <see cref="DBNull.Value"/> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="formatProvider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param> 
        /// <param name="styles">Ein Wert der <see cref="DateTimeStyles"/>-Enum, der zusätzliche Informationen für das Parsen bereitstellt. Wird
        /// nur ausgewertet, wenn <paramref name="parseExact"/> true ist.</param>
        /// <param name="parseExact">Wenn true, muss der Text in der CSV-Datei exakt dem mit <paramref name="format"/> angegebenen
        /// Formatstring entsprechen.</param>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="format"/> ist kein gültiger Formatstring.</exception>
        /// <remarks>Wenn kein spezielles Format gefprdert ist, sollten Sie das <see cref="DateTimeConverter"/>-Objekt
        /// über die Methode <see cref="CsvConverterFactory.CreateConverter(CsvTypeCode, bool, bool, IFormatProvider?, bool)"/> initialisieren: Das ist 
        /// wesentlich performanter.</remarks>
        public DateTimeConverter(
            string? format,
            bool nullable = false,
            bool maybeDBNull = false,
            IFormatProvider? formatProvider = null,
            bool throwOnParseErrors = false,
            DateTimeStyles styles = DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind,
            bool parseExact = false)
        {
            this.ThrowsOnParseErrors = throwOnParseErrors;

            formatProvider ??= CultureInfo.InvariantCulture;

            Type = nullable ? typeof(DateTime?) : typeof(DateTime);
            FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(DateTime?) : default(DateTime));


            if (format is null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            try
            {
                _ = DateTime.Now.ToString(format, formatProvider);
            }
            catch (FormatException e)
            {
                throw new ArgumentException(e.Message, nameof(format), e);
            }


            //if ((styles & DateTimeStyles.NoCurrentDateDefault) == DateTimeStyles.NoCurrentDateDefault)
            //{
            //    throw new ArgumentException(Res.NoCurrentDateDafault, nameof(styles));
            //}



            if (nullable)
            {
                _toStringConverter = new Converter<object?, string?>(
                    o =>
                    {
                        if (o is null) return null;
                        if (Convert.IsDBNull(o) && maybeDBNull) return null;

                        return ((DateTime)o).ToString(format, formatProvider);
                    });


                if (parseExact)
                {
                    _parser = new Converter<string?, object?>(
                        s =>
                        {
                            if (s is null) return null;

                            try
                            {
                                return DateTime.ParseExact(s, format, formatProvider, styles);
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
                                return DateTime.Parse(s, formatProvider, styles);
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

                    return ((DateTime)o).ToString(format, formatProvider);
                });



                if (parseExact)
                {
                    _parser = new Converter<string?, object?>(
                        s =>
                        {
                            if (s is null) return FallbackValue;

                            try
                            {
                                return DateTime.ParseExact(s, format, formatProvider, styles);
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
                                return DateTime.Parse(s, formatProvider, styles);
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
        /// Der Datentyp, in den <see cref="DateTimeConverter"/> parsen bzw.
        /// in einen <see cref="string"/> umwandeln kann. (<c>typeof(DateTime)</c> oder <c>typeof(DateTime?)</c>)
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// True gibt an, dass eine Ausnahme geworfen wird, wenn <see cref="Parse(string)"/>
        /// scheitert. Anderenfalls wird in diesem Fall <see cref="FallbackValue"/> zurückgegeben.
        /// </summary>
        public bool ThrowsOnParseErrors { get; }

        /// <summary>
        /// Parst <paramref name="value"/> als <see cref="DateTime"/> oder <see cref="Nullable{T}">Nullable&lt;DateTime&gt;</see> - je
        /// nachdem, welche Option im Konstruktor gewählt wurde.
        /// </summary>
        /// <param name="value">Der zu parsende <see cref="string"/>.</param>
        /// <returns>Ein <see cref="DateTime"/> bzw. <see cref="Nullable{T}">Nullable&lt;DateTime&gt;</see> als Ergebnis
        /// des Parsens.</returns>
        /// <exception cref="FormatException"><paramref name="value"/> weist kein kompatibles Format auf. Die Ausnahme wird
        /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
        public object? Parse(string? value) => _parser(value);


        /// <summary>
        /// Gibt die Zeichenfolgendarstellung von <paramref name="value"/> zurück, wenn <paramref name="value"/> ein <see cref="DateTime"/> 
        /// bzw. <see cref="Nullable{T}">Nullable&lt;DateTime&gt;</see> ist.
        /// </summary>
        /// <param name="value">Ein <see cref="DateTime"/> bzw. <see cref="Nullable{T}">Nullable&lt;DateTime&gt;</see>.</param>
        /// <returns>Eine Zeichenfolgendarstellung von <paramref name="value"/>.</returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> lässt sich nicht in den Datentyp <see cref="DateTime"/> 
        /// bzw. <see cref="Nullable{T}">Nullable&lt;DateTime&gt;</see> umwandeln.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Datum und Uhrzeit liegen außerhalb des Bereichs von Datumsangaben, die 
        /// vom Kalender der im Konstruktor zugewiesenen <see cref="CultureInfo"/> unterstützt werden.</exception>
        public string? ConvertToString(object? value) => _toStringConverter(value);

    }
}
