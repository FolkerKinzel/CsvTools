using System;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Intls
{
    /// <summary>
    /// Übernimmt Konvertierungsvorgänge für den Datentyp <see cref="string"/>. Führt eine Typüberprüfung durch und kann Nullable-Strings in
    /// Non-Nullable-Strings umwandeln. (Akzeptiert auch <see cref="DBNull.Value"/> als Eingabe.)
    /// </summary>
    internal sealed class StringConverter : ICsvTypeConverter
    {
        /// <summary>
        /// Initialisiert ein neues <see cref="StringConverter"/>-Objekt.
        /// </summary>
        /// <param name="allowNull">Wenn false, wird von <see cref="Parse(string)"/> immer ein Non-Nullable-<see cref="string"/> 
        /// zurückgegeben: Der Rückgabewert von 
        /// <see cref="FallbackValue"/>ist dann <see cref="string.Empty"/> (sonst <c>null</c>).</param>
        /// <remarks>
        /// Aus Gründen der Konsistenz sollten Sie diesen Konstruktor nicht direkt aufrufen, sondern die Methode
        /// <see cref="CsvConverterFactory.CreateConverter(CsvTypeCode, bool, bool, IFormatProvider, bool)"/> verwenden.
        /// </remarks>
        public StringConverter(bool allowNull)
        {
            this.FallbackValue = allowNull ? null : string.Empty;
        }


        /// <summary>
        /// <see cref="string"/>-Wert, der zurückgegeben wird, wenn <see cref="CsvProperty"/> kein Zugriffsziel
        /// in den Spalten der CSV-Datei (repräsentiert duch <see cref="CsvRecord"/>) findet oder wenn der Eingabewert
        /// von <see cref="Parse(string)"/>&#160;<c>null</c> ist.
        /// </summary>
        public object? FallbackValue { get; }

        /// <summary>
        /// Der Datentyp, den die <see cref="StringConverter"/>-Klasse umwandeln kann (<see cref="string"/>).
        /// </summary>
        public Type Type => typeof(string);

        /// <summary>
        /// Gibt an, ob eine Ausnahme geworfen wird, wenn <see cref="Parse(string)"/>
        /// scheitert. (Immer <c>false</c>.)
        /// </summary>
        public bool ThrowsOnParseErrors => false;


        /// <summary>
        /// Gibt <paramref name="value"/> zurück.
        /// </summary>
        /// <param name="value">Der <see cref="string"/>, der die Eingabe des <see cref="StringConverter"/>-Objekts darstellt.</param>
        /// <returns><paramref name="value"/> oder <see cref="FallbackValue"/>, wenn <paramref name="value"/>&#160;<c>null</c> ist.</returns>
        public object? Parse(string? value) => value is null ? FallbackValue : value;


        /// <summary>
        /// Gibt <paramref name="value"/> zurück, wenn <paramref name="value"/> ein <see cref="string"/>, <c>null</c> oder <see cref="DBNull.Value"/> ist.
        /// </summary>
        /// <param name="value">Ein <see cref="string"/>, <c>null</c> oder <see cref="DBNull.Value"/>.</param>
        /// <returns><paramref name="value"/>, wenn <paramref name="value"/> ein <see cref="string"/> oder <c>null</c> ist.</returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> ist ncht <c>null</c> oder <see cref="DBNull.Value"/> und auch kein <see cref="string"/>.</exception>
        public string? ConvertToString(object? value) => Convert.IsDBNull(value) ? null : (string?)value;


    }
}
