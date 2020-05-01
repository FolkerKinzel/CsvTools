using FolkerKinzel.CsvTools.Resources;
using System;
using System.Diagnostics;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Intls
{
    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// von Enums.
    /// </summary>
    /// <typeparam name="TEnum">Typ einer beliebigen Enum.</typeparam>
    internal class EnumConverter<TEnum> : ICsvTypeConverter where TEnum: struct, Enum
    {
        private readonly Converter<string?, object?> _parser;
        private readonly Converter<object?, string?> _toStringConverter;

        /// <summary>
        /// Initialisiert ein <see cref="EnumConverter{T}"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}"/> akzeptiert und zurückgegeben,
        /// sonst <typeparamref name="TEnum"/>.</param>
        /// <param name="maybeDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value"/> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
        /// <param name="ignoreCase"><c>true</c> gibt an, das beim Parsen die Groß- und Kleinschreibung von Enum-Bezeichnern ignoriert wird.</param>
        /// <remarks>
        /// <para>
        /// Sie sollten diesen Konstruktor nicht direkt aufrufen, sondern über 
        /// <see cref="CsvConverterFactory.CreateEnumConverter{T}(bool, bool, bool, bool)"/> (für Konsistenz in Ihrem Code).
        /// </para>
        /// <para>
        /// Diese Überladung des Konstruktors ist wesentlich performanter als
        /// <see cref="EnumConverter(string?, bool, bool, bool, bool)"/>, bietet
        /// aber weniger Einstellmöglichkeiten: Bei der <see cref="string"/>-Ausgabe wird das Standardformat "D" verwendet, das die kürzeste 
        /// Repräsentation des Enum-Werts ausgibt (also i.d.R den Zahlenwert). Beim Parsen kommt
        /// <see cref="Enum.Parse(Type, string, bool)"/> zum Einsatz.
        /// </para>
        /// </remarks>
        public EnumConverter(
            bool nullable,
            bool maybeDBNull,
            bool throwOnParseErrors,
            bool ignoreCase)
        {
            const string format = "D";
            this.ThrowsOnParseErrors = throwOnParseErrors;

            Type = nullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
            FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(TEnum?) : default(TEnum));

            

            if (nullable)
            {
                // Cast nach T um InvalidCastException auszulösen bei falschem Typ:
                _toStringConverter = new Converter<object?, string?>(o =>
                {
                    if (o is null) return null;
                    if (Convert.IsDBNull(o) && maybeDBNull) return null;

                    return ((TEnum)o).ToString(format);
                });


                _parser = new Converter<string?, object?>(
                s =>
                {
                    if (s is null) return null;

                    Debug.Assert(s.Length != 0);

                    try
                    {
                        return Enum.Parse(typeof(TEnum), s, ignoreCase);
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

                    return ((TEnum)o).ToString(format);
                });

                _parser = new Converter<string?, object?>(
                    s =>
                    {
                        if (s is null) return FallbackValue;

                        Debug.Assert(s.Length != 0);

                        try
                        {
                            return Enum.Parse(typeof(TEnum), s, ignoreCase);
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
        /// Initialisiert ein <see cref="EnumConverter{T}"/>-Objekt.
        /// </summary>
        /// <param name="format">Ein Formatstring, der für die <see cref="string"/>-Ausgabe von <typeparamref name="TEnum"/> verwendet wird.</param>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}"/> akzeptiert und zurückgegeben,
        /// sonst <typeparamref name="TEnum"/>.</param>
        /// <param name="maybeDBNull">Wenn <c>true</c>, wird <see cref="DBNull.Value"/> als Eingabe akzeptiert und bildet auch den
        /// Rückgabewert von <see cref="FallbackValue"/>.</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
        /// <param name="ignoreCase"><c>true</c> gibt an, das beim Parsen die Groß- und Kleinschreibung von Enum-Bezeichnern ignoriert wird.</param>
        /// <exception cref="ArgumentException"><paramref name="format"/> ist kein gültiger Formatstring.</exception>
        public EnumConverter(
            string? format,
            bool nullable,
            bool maybeDBNull,
            bool throwOnParseErrors,
            bool ignoreCase)
        {
            this.ThrowsOnParseErrors = throwOnParseErrors;

            Type = nullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
            FallbackValue = maybeDBNull ? DBNull.Value : (object?)(nullable ? default(TEnum?) : default(TEnum));

            try
            {
                _ = default(TEnum).ToString(format);
            }
            catch(FormatException e)
            {
                throw new ArgumentException(e.Message, nameof(format), e);
            }

            if(nullable)
            {
                // Cast nach T um InvalidCastException auszulösen bei falschem Typ:
                _toStringConverter = new Converter<object?, string?>(o =>
                {
                    if (o is null) return null;
                    if (Convert.IsDBNull(o) && maybeDBNull) return null;

                    return ((TEnum)o).ToString(format);
                });


                _parser = new Converter<string?, object?>(
                s =>
                {
                    if (s is null) return null;

                    Debug.Assert(s.Length != 0);

                    try
                    {
                        return Enum.Parse(typeof(TEnum), s, ignoreCase);
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

                    return ((TEnum)o).ToString(format);
                });

                _parser = new Converter<string?, object?>(
                    s =>
                    {
                        if (s is null) return FallbackValue;

                        Debug.Assert(s.Length != 0);

                        try
                        {
                            return Enum.Parse(typeof(TEnum), s, ignoreCase);
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
        /// Wert, der zurückgegeben wird, wenn <see cref="CsvProperty"/> keine Daten
        /// in den Spalten der CSV-Datei (repräsentiert duch <see cref="CsvRecord"/>) findet oder wenn
        /// von <see cref="Parse(string)"/> scheitert.
        /// </summary>
        public object? FallbackValue { get; }


        /// <summary>
        /// Der Datentyp, in den <see cref="EnumConverter{TEnum}"/> konvertieren kann, bzw. den <see cref="EnumConverter{TEnum}"/>
        /// in einen <see cref="string"/> umwandeln kann. (<c>typeof(TEnum)</c> oder <c>typeof(TEnum?)</c>)
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// <c>true</c> gibt an, dass eine Ausnahme geworfen wird, wenn <see cref="Parse(string)"/>
        /// scheitert. Anderenfalls wird in diesem Fall <see cref="FallbackValue"/> zurückgegeben.
        /// </summary>
        public bool ThrowsOnParseErrors { get; }

        /// <summary>
        /// Erzeugt die Zeichenfolgendarstellung von <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Ein <typeparamref name="TEnum"/> bzw. <see cref="Nullable{T}"/>.</param>
        /// <returns>Ein <typeparamref name="TEnum"/> bzw. <see cref="Nullable{T}"/> als Ergebnis
        /// des Parsens.</returns>
        /// <exception cref = "InvalidCastException" ><paramref name="value"/> ist nicht <typeparamref name="TEnum"/>
        /// bzw. <see cref="Nullable{T}"/>.</exception>
        public string? ConvertToString(object? value) => this._toStringConverter(value);

        /// <summary>
        /// Parst <paramref name="value"/> als <typeparamref name="TEnum"/> oder <see cref="Nullable{T}"/> - je
        /// nachdem, welche Option im Konstruktor gewählt wurde.
        /// </summary>
        /// <param name="value">Der zu parsende <see cref="string"/>.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="value"/> enthält nur Leerraum oder  <paramref name="value"/> ist ein Name, aber nicht der 
        /// Name einer der für die Enumeration definierten benannten Konstanten.</exception>
        /// <exception cref="OverflowException"><paramref name="value"/> liegt außerhalb des Bereichs des <typeparamref name="TEnum"/> zugrunde liegenden Typs.</exception>
        public object? Parse(string? value) => this._parser(value);
        
    }
}
