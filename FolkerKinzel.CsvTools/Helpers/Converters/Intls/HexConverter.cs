using FolkerKinzel.CsvTools.Resources;
using System;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Intls
{
    /// <summary>
    /// Ein <see cref="ICsvTypeConverter"/>-Objekt, das zur Umwandlung ganzzahliger Datentypen in hexadezimale Darstellung und
    /// zum Parsen dieser Datentypen aus hexadezimaler Darstellung dient.
    /// </summary>
    /// <typeparam name="T">Ein ganzzahliger Datentyp.</typeparam>
    internal class HexConverter<T> : ICsvTypeConverter where T : struct, IConvertible
    {
        private readonly Converter<string?, object?> _converter;
        private readonly Converter<object?, string?> _toStringConverter;

        /// <summary>
        /// Initialisiert ein <see cref="HexConverter{T}"/>-Objekt.
        /// </summary>
        /// <param name="unsigned"><c>true</c>, wenn <typeparamref name="T"/> ein vorzeichenloser Integer-Typ ist.</param>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}"/> akzeptiert und zurückgegeben, sonst
        /// <typeparamref name="T"/>.</param>
        /// <param name="maybeDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value">DBNull.Value</see> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
        internal HexConverter(
            bool unsigned,
            bool nullable,
            bool maybeDBNull,
            bool throwOnParseErrors)
        {
            this.Type = nullable ? typeof(T?) : typeof(T);
            this.FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(T?) : default(T));
            this.ThrowsOnParseErrors = throwOnParseErrors;

            const string format = "X";
            const NumberStyles styles = NumberStyles.HexNumber;


            _toStringConverter = nullable
                ? InitNullableToStringConverter(unsigned, maybeDBNull, format)
                : InitNonNullableToStringConverter(unsigned, maybeDBNull, format);

            _converter = InitConverter(unsigned, styles);
        }

        /// <summary>
        /// Wert, der zurückgegeben wird, wenn <see cref="CsvProperty"/> keine Daten
        /// in den Spalten der CSV-Datei (repräsentiert duch <see cref="CsvRecord"/>) findet oder wenn
        /// von <see cref="Parse(string)"/> scheitert.
        /// </summary>
        public object? FallbackValue { get; }


        /// <summary>
        /// Der Datentyp, in den <see cref="HexConverter{T}"/> parsen bzw.
        /// in einen <see cref="string"/> umwandeln kann.
        /// </summary>
        public Type Type { get; }


        /// <summary>
        /// <c>true</c> gibt an, dass eine Ausnahme geworfen wird, wenn <see cref="Parse(string)"/>
        /// scheitert. Anderenfalls wird in diesem Fall <see cref="FallbackValue"/> zurückgegeben.
        /// </summary>
        public bool ThrowsOnParseErrors { get; }


        /// <summary>
        /// Gibt die hexadezimale Zeichenfolgendarstellung von <paramref name="value"/> zurück.
        /// </summary>
        /// <param name="value">Ein ganzzahliger Datentyp <typeparamref name="T"/> oder null, wenn <see cref="Type"/>&#160;<see cref="Nullable{T}"/> ist.</param>
        /// <returns>Eine hexadezimale Zeichenfolgendarstellung von <paramref name="value"/> oder <c>null</c></returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> lässt sich nicht in <see cref="Type"/> umwandeln.</exception>
        public string? ConvertToString(object? value) => _toStringConverter(value);


        /// <summary>
        /// Parst <paramref name="value"/> als <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">Der zu parsende <see cref="string"/>.</param>
        /// <returns>Ein ganzzahliger Typ - <typeparamref name="T"/> oder <see cref="Nullable{T}"/>.</returns>
        /// <exception cref="FormatException"><paramref name="value"/> weist kein kompatibles Format auf. Die Ausnahme wird
        /// nur geworfen, wenn wenn <see cref="ThrowsOnParseErrors"/>&#160;<c>true</c> - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
        /// <exception cref="OverflowException"><paramref name="value"/> stellt eine Zahl außerhalb des Bereichs von <see cref="Type"/> dar.</exception>
        public object? Parse(string? value) => _converter(value);

        
        #region private

        private static Converter<object?, string?> InitNullableToStringConverter(bool unsigned, bool maybeDBNull, string format)
        => unsigned ? new Converter<object?, string?>
                        (
                            o =>
                            {
                                if (o is null || ((o == DBNull.Value) && maybeDBNull))
                                {
                                    return null;
                                }

                                ulong l = Convert.ToUInt64((T)o, CultureInfo.InvariantCulture);
                                return l.ToString(format, CultureInfo.InvariantCulture);
                            }
                        )
                    : new Converter<object?, string?>
                            (
                            o =>
                            {
                                if (o is null || ((o == DBNull.Value) && maybeDBNull))
                                {
                                    return null;
                                }

                                long l = Convert.ToInt64((T)o, CultureInfo.InvariantCulture);
                                return l.ToString(format, CultureInfo.InvariantCulture);
                            }
                            );


        private static Converter<object?, string?> InitNonNullableToStringConverter(bool unsigned, bool maybeDBNull, string format)
        => unsigned ? new Converter<object?, string?>
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

                                ulong l = Convert.ToUInt64((T)o, CultureInfo.InvariantCulture);
                                return l.ToString(format, CultureInfo.InvariantCulture);
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

                                long l = Convert.ToInt64((T)o, CultureInfo.InvariantCulture);
                                return l.ToString(format, CultureInfo.InvariantCulture);
                            }
                        );

        private Converter<string?, object?> InitConverter(bool unsigned, NumberStyles styles)
        => unsigned ? new Converter<string?, object?>
                        (
                            s =>
                            {
                                if (s is null)
                                {
                                    return FallbackValue;
                                }

                                try
                                {
                                    return Convert.ChangeType(ulong.Parse(s, styles, CultureInfo.InvariantCulture), typeof(T), CultureInfo.InvariantCulture);
                                }
                                catch
                                {
                                    if (ThrowsOnParseErrors)
                                    {
                                        throw;
                                    }

                                    return FallbackValue;
                                }
                            }
                        )
                    : new Converter<string?, object?>
                        (
                            s =>
                            {
                                if (s is null)
                                {
                                    return FallbackValue;
                                }

                                try
                                {
                                    return Convert.ChangeType(long.Parse(s, styles, CultureInfo.InvariantCulture), typeof(T), CultureInfo.InvariantCulture);
                                }
                                catch
                                {
                                    if (ThrowsOnParseErrors)
                                    {
                                        throw;
                                    }

                                    return FallbackValue;
                                }
                            }
                        );

        #endregion

    }
}
