using FolkerKinzel.CsvTools.Resources;
using System;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Specialized
{
    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="Guid"/>-Datentyps.
    /// </summary>
    public sealed class GuidConverter : ICsvTypeConverter
    {
        private readonly Converter<string?, object?> _parser;
        private readonly Converter<object?, string?> _toStringConverter;

        /// <summary>
        /// Initialisiert ein <see cref="GuidConverter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;Guid&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="Guid"/>.</param>
        /// <param name="maybeDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value">DBNull.Value</see> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
        /// <remarks>
        /// <para>
        /// Sie sollten diesen Konstruktor nicht direkt aufrufen, sondern über 
        /// <see cref="CsvConverterFactory.CreateConverter(CsvTypeCode, bool, bool, IFormatProvider, bool)"/> (für Konsistenz in Ihrem Code).
        /// </para>
        /// <para>
        /// Diese Überladung des Konstruktors ist wesentlich performanter als
        /// <see cref="GuidConverter.GuidConverter(string, bool, bool, bool)"/>, bietet
        /// aber weniger Einstellmöglichkeiten: Bei der <see cref="string"/>-Ausgabe wird das Standardformat "D" verwendet. Beim Parsen kommt
        /// <see cref="Guid.Parse(string)"/> zum Einsatz.
        /// </para>
        /// </remarks>
        internal GuidConverter(
            bool nullable,
            bool maybeDBNull,
            bool throwOnParseErrors)
        {
            this.ThrowsOnParseErrors = throwOnParseErrors;
            this.Type = nullable ? typeof(Guid?) : typeof(Guid);
            this.FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(Guid?) : default(Guid));

            const string format = "D";

            
            if (nullable)
            {
                // Cast nach Guid um InvalidCastException auszulösen bei falschem Typ:
                _toStringConverter = new Converter<object?, string?>(
                    o => o is null || (Convert.IsDBNull(o) && maybeDBNull) 
                            ? null 
                            : ((Guid)o).ToString(format, CultureInfo.InvariantCulture));


                _parser = new Converter<string?, object?>(
                     s =>
                     {
                         if (s is null)
                         {
                             return null;
                         }

                         try
                         {
                             return Guid.Parse(s);
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
            else
            {
                _toStringConverter = new Converter<object?, string?>(
                o =>
                {
                    if (Convert.IsDBNull(o) && maybeDBNull)
                    {
                        return null;
                    }

                    if (o is null)
                    {
                        throw new InvalidCastException(Res.InvalidCastNullToValueType);
                    }

                    return ((Guid)o).ToString(format, CultureInfo.InvariantCulture);
                });

                _parser = new Converter<string?, object?>(
                    s =>
                    {
                        if (s is null)
                        {
                            return FallbackValue;
                        }

                        try
                        {
                            return Guid.Parse(s);
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
        }



        /// <summary>
        /// Initialisiert ein neues <see cref="GuidConverter"/>-Objekt.
        /// </summary>
        /// <param name="format">Ein Formatstring, der für die <see cref="string"/>-Ausgabe von <see cref="Guid"/>-Werten verwendet wird.</param>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;Guid&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="Guid"/>.</param>
        /// <param name="maybeDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value">DBNull.Value</see> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
        /// 
        /// <exception cref="ArgumentException"><paramref name="format"/> ist kein gültiger Formatstring.</exception>
        /// 
        /// <remarks>Wenn lediglich das <see cref="Guid"/>-Standardformat "D" ausgegeben werden soll, sollte ein <see cref="GuidConverter"/> mit 
        /// <see cref="CsvConverterFactory.CreateConverter(CsvTypeCode, bool, bool, IFormatProvider?, bool)"/> erzeugt werden: Das ist wesentlich performanter!</remarks>
        public GuidConverter(
            string? format,
            bool nullable = false,
            bool maybeDBNull = false,
            bool throwOnParseErrors = false)
        {
            this.ThrowsOnParseErrors = throwOnParseErrors;
            this.Type = nullable ? typeof(Guid?) : typeof(Guid);
            this.FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(Guid?) : default(Guid));

            try
            {
                _ = Guid.Empty.ToString(format, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                throw new ArgumentException(e.Message, nameof(format), e);
            }



            if (nullable)
            {
                // Cast nach Guid um InvalidCastException auszulösen bei falschem Typ:
                _toStringConverter = new Converter<object?, string?>(
                    o => o is null || (Convert.IsDBNull(o) && maybeDBNull) 
                         ? null 
                         : ((Guid)o).ToString(format, CultureInfo.InvariantCulture));


                _parser = new Converter<string?, object?>(
                     s =>
                     {
                         if (s is null)
                         {
                             return null;
                         }

                         try
                         {
                             return Guid.Parse(s);
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
            else
            {
                _toStringConverter = new Converter<object?, string?>(
                o =>
                {
                    if (Convert.IsDBNull(o) && maybeDBNull)
                    {
                        return null;
                    }

                    if (o is null)
                    {
                        throw new InvalidCastException(Res.InvalidCastNullToValueType);
                    }

                    return ((Guid)o).ToString(format, CultureInfo.InvariantCulture);
                });

                _parser = new Converter<string?, object?>(
                    s =>
                    {
                        if (s is null)
                        {
                            return FallbackValue;
                        }

                        try
                        {
                            return Guid.Parse(s);
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
        }

        
        /// <inheritdoc />
        public object? FallbackValue { get; }

        /// <summary>
        /// Der Datentyp, in den <see cref="GuidConverter"/> parsen bzw.
        /// den <see cref="GuidConverter"/> in einen <see cref="string"/> umwandeln kann. (<c>typeof(Guid)</c> oder <c>typeof(Guid?)</c>)
        /// </summary>
        public Type Type { get; }

        
        /// <inheritdoc/>
        public bool ThrowsOnParseErrors { get; }

        
        /// <inheritdoc/>
        /// <exception cref="FormatException"><paramref name="value"/> weist kein kompatibles Format auf. Die Ausnahme wird
        /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
        public object? Parse(string? value) => _parser(value);


        
        /// <inheritdoc/>
        public string? ConvertToString(object? value) => _toStringConverter(value);

    }
}
