using FolkerKinzel.CsvTools.Resources;
using System;
using System.Globalization;

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
        /// <param name="maybeDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value">DBNull.Value</see> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="formatProvider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt, oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
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
        internal TimeSpanConverter(
            bool nullable,
            bool maybeDBNull,
            IFormatProvider? formatProvider,
            bool throwOnParseErrors)
        {
            this.ThrowsOnParseErrors = throwOnParseErrors;
            formatProvider ??= CultureInfo.InvariantCulture;

            this.Type = nullable ? typeof(TimeSpan?) : typeof(TimeSpan);
            this.FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(TimeSpan?) : default(TimeSpan));

            const string format = "g";

            _toStringConverter = nullable
                ? new Converter<object?, string?>
                    (
                        o => o is null || ((o == DBNull.Value) && maybeDBNull)
                                ? null
                                : ((TimeSpan)o).ToString(format, formatProvider)
                     )
                : new Converter<object?, string?>
                    (
                        o => (o == DBNull.Value) && maybeDBNull
                                ? null
                                : o is null ? throw new InvalidCastException(Res.InvalidCastNullToValueType)
                                            : ((TimeSpan)o).ToString(format, formatProvider)
                    );


            _parser = new Converter<string?, object?>
                (
                    s =>
                    {
                        if (s is null)
                        {
                            return FallbackValue;
                        }

                        try
                        {
                            return TimeSpan.Parse(s, formatProvider);
                        }
                        catch
                        {
                            if (throwOnParseErrors)
                            {
                                throw;
                            }

                            return FallbackValue;
                        }
                    }
                );
        }


        /// <summary>
        /// Initialisiert ein neues <see cref="TimeSpanConverter"/>-Objekt.
        /// </summary>
        /// <param name="format">Ein Formatstring, der für die <see cref="string"/>-Ausgabe von <see cref="TimeSpan"/>-Werten verwendet wird.
        /// Wenn die Option <paramref name="parseExact"/> gewählt ist, wird dieser Formatstring auch für das Parsen verwendet.</param>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;TimeSpan&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="TimeSpan"/>.</param>
        /// <param name="maybeDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value">DBNull.Value</see> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="formatProvider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
        /// <param name="styles">Ein Wert der <see cref="TimeSpanStyles"/>-Enum, der zusätzliche Informationen für das Parsen bereitstellt. Wird
        /// nur ausgewertet, wenn <paramref name="parseExact"/>&#160;<c>true</c> ist.</param>
        /// <param name="parseExact">Wenn <c>true</c>, muss der Text in der CSV-Datei exakt dem mit <paramref name="format"/> angegebenen
        /// Formatstring entsprechen.</param>
        /// 
        /// <exception cref="ArgumentException"><paramref name="format"/> ist kein gültiger Formatstring - oder - <paramref name="styles"/>
        /// hat einen ungültigen Wert.</exception>
        /// 
        /// <remarks>Wenn es genügt, dass bei der <see cref="string"/>-Ausgabe wird das <see cref="TimeSpan"/>-Standardformat "g" verwendet wird,
        /// sollten Sie aus Performancegründen das <see cref="TimeSpanConverter"/>-Objekt mit der Methode 
        /// <see cref="CsvConverterFactory.CreateConverter(CsvTypeCode, bool, bool, IFormatProvider?, bool)"/> initialisieren.</remarks>
        public TimeSpanConverter(
            string? format,
            bool nullable = false,
            bool maybeDBNull = false,
            IFormatProvider? formatProvider = null,
            bool throwOnParseErrors = false,
            TimeSpanStyles styles = TimeSpanStyles.None,
            bool parseExact = false)
        {
            this.ThrowsOnParseErrors = throwOnParseErrors;
            formatProvider ??= CultureInfo.InvariantCulture;

            this.Type = nullable ? typeof(TimeSpan?) : typeof(TimeSpan);
            this.FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(TimeSpan?) : default(TimeSpan));

            format ??= string.Empty;

            try
            {
                string tmp = TimeSpan.Zero.ToString(format, formatProvider);

                if(parseExact)
                {
                    _ = TimeSpan.ParseExact(tmp, format, formatProvider, styles);
                }
            }
            catch (FormatException e)
            {
                throw new ArgumentException(e.Message, e);
            }

            _toStringConverter = nullable
                ? new Converter<object?, string?>
                  (
                    o => o is null || ((o == DBNull.Value) && maybeDBNull)
                            ? null
                            : ((TimeSpan)o).ToString(format, formatProvider)
                   )
                : new Converter<object?, string?>
                  (
                    o => (o == DBNull.Value) && maybeDBNull
                            ? null
                            : o is null ? throw new InvalidCastException(Res.InvalidCastNullToValueType)
                                        : ((TimeSpan)o).ToString(format, formatProvider)
                   );

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
                                return TimeSpan.ParseExact(s, format, formatProvider, styles);
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
                               return TimeSpan.Parse(s, formatProvider);
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

        
        /// <inheritdoc/>
        public object? FallbackValue { get; }

        /// <summary>
        /// Der Datentyp, den <see cref="TimeSpanConverter"/> parsen kann, bzw. den <see cref="TimeSpanConverter"/>
        /// in einen <see cref="string"/> umwandeln kann. (<c>typeof(TimeSpan)</c> oder <c>typeof(TimeSpan?)</c>)
        /// </summary>
        public Type Type { get; }

        
        /// <inheritdoc/>
        public bool ThrowsOnParseErrors { get; }

        
        /// <inheritdoc/>
        /// <exception cref="FormatException"><paramref name="value"/> weist kein kompatibles Format auf. Die Ausnahme wird
        /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
        /// <exception cref="OverflowException"><paramref name="value"/> stellt eine Zahl außerhalb des Bereichs von <see cref="TimeSpan"/> dar.
        /// Die Ausnahme wird
        /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
        public object? Parse(string? value) => _parser(value);


        
        /// <inheritdoc/>
        public string? ConvertToString(object? value) => _toStringConverter(value);

    }
}
