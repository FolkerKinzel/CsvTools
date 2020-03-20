using FolkerKinzel.CsvTools.Properties;
using System;
using System.Globalization;

namespace FolkerKinzel.CsvTools.Helpers.Converters
{
    /// <summary>
    /// Übernimmt Konvertierungsvorgänge für den Datentyp <see cref="int"/> 
    /// bzw. <see cref="Nullable{T}">Nullable&lt;Int32&gt;</see>.
    /// </summary>
    public class Int32Converter : ICsvTypeConverter
    {
        private readonly Converter<string?, object?> _converter;
        private readonly Converter<object?, string?> _toStringConverter;
        private readonly bool _nullable;

        /// <summary>
        /// Initialisiert ein neues <see cref="Int32Converter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;Int32&gt;</see> akzeptiert und zurückgegeben, sonst
        /// <see cref="int"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="style">Ein einfacher oder kombinierter Wert der <see cref="NumberStyles"/>-Enum, der das Parsen steuert.</param>
        /// <param name="format">Eine standardmäßige oder benutzerdefinierte numerische Formatierungszeichenfolge für die Umwandlung von
        /// <see cref="int"/> in einen <see cref="string"/>.</param>
        /// <param name="throwOnDecodingErrors">Wenn true, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen mißlingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
        /// <exception cref="ArgumentException"><paramref name="style"/> ist kein gültiger <see cref="NumberStyles"/>-Wert - oder - 
        /// <paramref name="format"/> ist ungültig oder wird nicht unterstützt.</exception>
        public Int32Converter(
            bool nullable = false,
            IFormatProvider? provider = null,
            NumberStyles style = NumberStyles.Any,
            string format = "G",
            bool throwOnDecodingErrors = false)
        {

            try
            {
                // testet die Gültigkeit von style:
                _ = int.Parse("1", style, provider);
            }
            catch(ArgumentException e)
            {
                throw new ArgumentException(e.Message, nameof(style), e);
            }

            try
            {
                // Wirft FormatException, wenn format ungültig ist:
                _ = ((int)17).ToString(format, provider);
            }
            catch(FormatException e)
            {
                throw new ArgumentException(e.Message, nameof(format), e);
            }

            this._nullable = nullable;
            provider ??= CultureInfo.InvariantCulture;

           
            if (nullable)
            {
                _toStringConverter = new Converter<object?, string?>(
                o => (o is null) ? (string?)null : ((int)o).ToString(format, provider));

                if (throwOnDecodingErrors)
                {
                    _converter = new Converter<string?, object?>(
                        s => (s is null) ? (int?)null : int.Parse(s, style, provider));
                }
                else
                {
                    _converter = new Converter<string?, object?>(
                        s =>
                        {
                            if (s is null)
                            {
                                return (int?)null;
                            }
                            else if (int.TryParse(s, style, provider, out int res))
                            {
                                return res;
                            }
                            else
                            {
                                return FallbackValue;
                            }
                        });
                }
            }
            else
            {
                _toStringConverter = new Converter<object?, string?>(
                o => o is null ? throw new InvalidCastException(Resources.InvalidCastNullToInt32) : ((int)o).ToString(format, provider));

                if (throwOnDecodingErrors)
                {
                    _converter = new Converter<string?, object?>(
                        s => int.Parse(s, style, provider));
                }
                else
                {
                    _converter = new Converter<string?, object?>(
                        s =>
                        {
                            if (int.TryParse(s, style, provider, out int res))
                            {
                                return res;
                            }
                            else
                            {
                                return FallbackValue;
                            }
                        });
                }
            }
        }

        /// <summary>
        /// Wert, der zurückgegeben wird, wenn <see cref="CsvProperty"/> kein Zugriffsziel
        /// in den Spalten der CSV-Datei (repräsentiert duch <see cref="CsvRecord"/>) findet oder wenn
        /// von <see cref="Parse(string)"/> scheitert. (Je nach Kofiguration des Konstruktors <c>default(int)</c>
        /// oder <c>default(int?)</c>.)
        /// </summary>
        public object? FallbackValue => _nullable ? default(int?) : default(int);

        /// <summary>
        /// Der Datentyp, in den <see cref="Int32Converter"/> konvertieren kann, bzw. den <see cref="Int32Converter"/>
        /// in einen <see cref="string"/> verwandeln kann.
        /// </summary>
        public Type Type => _nullable ? typeof(int?) : typeof(int);

        /// <summary>
        /// Parst <paramref name="value"/> als <see cref="int"/> oder <see cref="Nullable{T}">Nullable&lt;Int32&gt;</see> - je
        /// nachdem, welche Option im Konstruktor gewählt wurde.
        /// </summary>
        /// <param name="value">Der zu parsende <see cref="string"/>.</param>
        /// <returns>Ein <see cref="int"/> bzw. <see cref="Nullable{T}">Nullable&lt;Int32&gt;</see> als Ergebnis
        /// des Parsens.</returns>
        /// <exception cref="FormatException"><paramref name="value"/> weist kein kompatibles Format auf. Die Ausnahme wird
        /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
        /// <exception cref="OverflowException"><paramref name="value"/> stellt eine Zahl dar, die kleiner als <see cref="int.MinValue"/>
        /// oder größer als <see cref="int.MaxValue"/> ist - oder - <paramref name="value"/> enthält Dezimalstellen.
        /// Die Ausnahme wird
        /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
        public object? Parse(string? value) => _converter(value);


        /// <summary>
        /// Gibt die Zeichenfolgendarstellung von <paramref name="value"/> zurück, wenn <paramref name="value"/> ein <see cref="int"/> 
        /// bzw. <see cref="Nullable{T}">Nullable&lt;Int32&gt;</see> ist.
        /// </summary>
        /// <param name="value">Ein <see cref="int"/> bzw. <see cref="Nullable{T}">Nullable&lt;Int32&gt;</see>.</param>
        /// <returns>Eine Zeichenfolgendarstellung von <paramref name="value"/>.</returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> lässt sich nicht in den Datentyp <see cref="int"/> 
        /// bzw. <see cref="Nullable{T}">Nullable&lt;Int32&gt;</see> umwandeln.</exception>
        public string? ConvertToString(object? value) => _toStringConverter(value);

    }
}
