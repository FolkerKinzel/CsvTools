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
        /// <param name="maybeDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value">DBNull.Value</see> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="formatProvider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt, oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
        /// <remarks>
        /// <para>
        /// Sie können diesen Konstruktor nicht direkt aufrufen, sondern über 
        /// <see cref="CsvConverterFactory.CreateConverter(CsvTypeCode, bool, bool, IFormatProvider, bool)"/>.
        /// </para>
        /// <para>
        /// Diese Überladung des Konstruktors ist wesentlich performanter als
        /// <see cref="DateTimeConverter.DateTimeConverter(string, bool, bool, IFormatProvider, bool, DateTimeStyles, bool)"/>, bietet
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


            _toStringConverter = nullable
                ? new Converter<object?, string?>(
                    o => o is null || ((o == DBNull.Value) && maybeDBNull)
                          ? null
                          : ((DateTime)o).ToString(format, formatProvider)
                          )
                
                : new Converter<object?, string?>(
                    o => (o == DBNull.Value) && maybeDBNull
                        ? null
                        : o is null ? throw new InvalidCastException(Res.InvalidCastNullToValueType) 
                                    : ((DateTime)o).ToString(format, formatProvider));


            _parser = new Converter<string?, object?>(
                    s =>
                    {
                        if (s is null)
                        {
                            return FallbackValue;
                        }

                        try
                        {
                            return DateTime.Parse(s, formatProvider, styles);
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
        /// Initialisiert ein neues <see cref="DateTimeConverter"/>-Objekt.
        /// </summary>
        /// <param name="format">Ein Formatstring, der für die <see cref="string"/>-Ausgabe von <see cref="DateTime"/>-Werten verwendet wird.
        /// Wenn die Option <paramref name="parseExact"/> gewählt ist, wird dieser Formatstring auch für das Parsen verwendet.</param>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;DateTime&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="DateTime"/>.</param>
        /// <param name="maybeDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value">DBNull.Value</see> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="formatProvider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param> 
        /// <param name="styles">Ein Wert der <see cref="DateTimeStyles"/>-Enum, der zusätzliche Informationen für das Parsen bereitstellt. Wird
        /// nur ausgewertet, wenn <paramref name="parseExact"/>&#160;<c>true</c> ist.</param>
        /// <param name="parseExact">Wenn <c>true</c>, muss der Text in der CSV-Datei exakt dem mit <paramref name="format"/> angegebenen
        /// Formatstring entsprechen.</param>
        /// <exception cref="ArgumentException"><paramref name="format"/> ist kein gültiger Formatstring - oder - <paramref name="styles"/>
        /// hat einen ungültigen Wert.</exception>
        /// <remarks>Wenn kein spezielles Format gefordert ist, sollten Sie das <see cref="DateTimeConverter"/>-Objekt
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

            format ??= string.Empty;

            try
            {
                string tmp = DateTime.Now.ToString(format, formatProvider);

                if(parseExact)
                {
                    _ = DateTime.ParseExact(tmp, format, formatProvider, styles);
                }
            }
            catch (FormatException e)
            {
                throw new ArgumentException(e.Message, e);
            }


            _toStringConverter = nullable
                ? new Converter<object?, string?>(
                    o => o is null || ((o == DBNull.Value) && maybeDBNull)
                           ? null
                           : ((DateTime)o).ToString(format, formatProvider))
                : new Converter<object?, string?>(
                    o => (o == DBNull.Value) && maybeDBNull
                         ? null
                         : o is null ? throw new InvalidCastException(Res.InvalidCastNullToValueType)
                                     : ((DateTime)o).ToString(format, formatProvider));

            _parser = parseExact
                    ? new Converter<string?, object?>(
                        s =>
                        {
                            if (s is null)
                            {
                                return FallbackValue;
                            }

                            try
                            {
                                return DateTime.ParseExact(s, format, formatProvider, styles);
                            }
                            catch
                            {
                                if (throwOnParseErrors)
                                {
                                    throw;
                                }

                                return FallbackValue;
                            }
                        })
                    : new Converter<string?, object?>(
                        s =>
                        {
                            if (s is null)
                            {
                                return FallbackValue;
                            }

                            try
                            {
                                return DateTime.Parse(s, formatProvider, styles);
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


        /// <inheritdoc />
        public object? FallbackValue { get; }

        /// <summary>
        /// Der Datentyp, in den <see cref="DateTimeConverter"/> parsen bzw.
        /// den <see cref="DateTimeConverter"/> in einen <see cref="string"/> umwandeln kann. (<c>typeof(DateTime)</c> oder <c>typeof(DateTime?)</c>)
        /// </summary>
        public Type Type { get; }

        
        /// <inheritdoc/>
        public bool ThrowsOnParseErrors { get; }

        
        /// <inheritdoc/>
        /// <exception cref="FormatException"><paramref name="value"/> weist kein kompatibles Format auf. Die Ausnahme wird
        /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
        public object? Parse(string? value) => _parser(value);


        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">Datum und Uhrzeit liegen außerhalb des Bereichs von Datumsangaben, die 
        /// vom Kalender der im Konstruktor zugewiesenen <see cref="CultureInfo"/> unterstützt werden.</exception>
        public string? ConvertToString(object? value) => _toStringConverter(value);

    }
}
