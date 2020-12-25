using System;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Intls
{
    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// von Byte-Arrays. (Akzeptiert auch <c>null</c> und <see cref="DBNull.Value">DBNull.Value</see> als Eingabe.)
    /// </summary>
    internal class Base64Converter : ICsvTypeConverter
    {
        /// <summary>
        /// Initialisiert ein <see cref="Base64Converter"/>-Objekt.
        /// </summary>
        /// <param name="allowNull">Wenn <c>false</c>, wird von <see cref="Parse(string)"/> nie <c>null</c> zurückgegeben: Der Rückgabewert von 
        /// <see cref="FallbackValue"/>ist dann ein leeres Array (sonst <c>null</c>).</param>
        /// <param name="throwOnParseErrors">Wenn <c>true</c>, wirft die Methode <see cref="Parse"/> eine Ausnahme, wenn das Parsen misslingt,
        /// anderenfalls gibt sie in diesem Fall <see cref="FallbackValue"/> zurück.</param>
        /// <remarks>
        /// Aus Gründen der Konsistenz sollten Sie diesen Konstruktor nicht direkt aufrufen, sondern die Methode
        /// <see cref="CsvConverterFactory.CreateConverter(CsvTypeCode, bool, bool, IFormatProvider, bool)"/> verwenden.
        /// </remarks>
        public Base64Converter(bool allowNull, bool throwOnParseErrors)
        {
#if NET40
            FallbackValue = allowNull ? null : new byte[0];
#else
            FallbackValue = allowNull ? null : Array.Empty<byte>();
#endif


            this.ThrowsOnParseErrors = throwOnParseErrors;
        }

        /// <summary>
        /// Wert, der zurückgegeben wird, wenn <see cref="CsvProperty"/> kein Zugriffsziel
        /// in den Spalten der CSV-Datei (repräsentiert duch <see cref="CsvRecord"/>) findet oder wenn
        /// von <see cref="Parse(string)"/> scheitert.
        /// </summary>
        public object? FallbackValue { get; }

        /// <summary>
        /// Der Datentyp, in den <see cref="Base64Converter"/> parsen bzw.
        /// in einen Base64-kodierten <see cref="string"/> umwandeln kann: <c>byte[]</c>.
        /// </summary>
        public Type Type => typeof(byte[]);

        /// <summary>
        /// <c>true</c> gibt an, dass eine Ausnahme geworfen wird, wenn <see cref="Parse(string)"/>
        /// scheitert. Anderenfalls wird in diesem Fall <see cref="FallbackValue"/> zurückgegeben.
        /// </summary>
        public bool ThrowsOnParseErrors { get; }


        /// <summary>
        /// Gibt die Base64-Zeichenfolgendarstellung von <paramref name="value"/> zurück, wenn <paramref name="value"/> ein Byte-Array
        /// ist. Wenn <paramref name="value"/>&#160;<c>null</c> oder <see cref="DBNull.Value">DBNull.Value</see> ist, gibt die Methode
        /// <c>null</c> zurück.
        /// </summary>
        /// <param name="value">Ein Byte-Array oder <c>null</c> oder <see cref="DBNull.Value">DBNull.Value</see>.</param>
        /// <returns>Eine Base64-Zeichenfolgendarstellung von <paramref name="value"/> oder null.</returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> ist weder <c>null</c> noch <see cref="DBNull.Value">DBNull.Value</see>
        /// noch ein Byte-Array.</exception>
        public string? ConvertToString(object? value) 
            => (value is null || Convert.IsDBNull(value)) ? null : Convert.ToBase64String((byte[])value, Base64FormattingOptions.None);


        /// <summary>
        /// Parst einen Base64-codierten <see cref="string"/> als <see cref="byte"/>-Array.
        /// </summary>
        /// <param name="value">Ein Base64-codierter <see cref="string"/> oder <c>null</c>.</param>
        /// <returns>Ein Byte-Array oder <c>null</c>, wenn <paramref name="value"/><c>null</c> war, oder wenn beim Parsen ein Fehler
        /// passiert ist.</returns>
        /// <exception cref="FormatException">Das Format von <paramref name="value"/> ist ungültig. Die Ausnahme wird
        /// nur geworfen, wenn das im Konstruktor so konfiguriert wurde - anderenfalls wird <see cref="FallbackValue"/> zurückgegeben.</exception>
        public object? Parse(string? value)
        {
            if (value is null) return null;

            try
            {
                return Convert.FromBase64String(value);
            }
            catch (FormatException)
            {
                if (ThrowsOnParseErrors)
                {
                    throw;
                }
                else
                {
                    return FallbackValue;
                }
            }
        }
    }
}
