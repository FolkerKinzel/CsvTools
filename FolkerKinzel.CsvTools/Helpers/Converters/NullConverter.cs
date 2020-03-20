using System;
using System.Collections.Generic;
using System.Text;

namespace FolkerKinzel.CsvTools.Helpers.Converters
{
    /// <summary>
    /// Wandelt <c>null</c> und DBNull.Value in einen <c>null</c>-<see cref="string"/> um und führt dabei
    /// eine Typüberprüfung durch. Gibt beim Parsen immer <c>null</c> zurück.
    /// </summary>
    public class NullConverter : ICsvTypeConverter
    {
        private static readonly NullConverter _converter = new NullConverter();

        private NullConverter() { }

        /// <summary>
        /// Gibt ein <see cref="NullConverter"/>-Objekt zurück (Singleton).
        /// </summary>
        /// <returns>Ein <see cref="NullConverter"/>-Objekt.</returns>
        public static NullConverter GetConverter() => _converter;

        /// <summary>
        /// Wert, der zurückgegeben wird, wenn <see cref="CsvProperty"/> keine Daten
        /// in den Spalten der CSV-Datei (repräsentiert duch <see cref="CsvRecord"/>) findet. (<c>null</c>)
        /// </summary>
        public object? FallbackValue => null;


        /// <summary>
        /// Der Datentyp, den die <see cref="NullConverter"/>-Klasse umwandeln kann (<see cref="object"/>).
        /// </summary>
        public Type Type => typeof(object);

        /// <summary>
        /// Gibt an, ob eine Ausnahme geworfen wird, wenn <see cref="Parse(string)"/>
        /// scheitert. (Immer <c>false</c>.)
        /// </summary>
        public bool ThrowsOnParseErrors => false;


        /// <summary>
        /// Gibt <c>null</c> zurück, wenn <paramref name="value"/>&#160;<c>null</c> oder DBNull.Value ist.
        /// </summary>
        /// <param name="value"><c>null</c> oder DBNull.Value</param>
        /// <returns><c>null</c></returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> ist nicht <c>null</c> oder DBNull.Value.</exception>
        public string? ConvertToString(object? value) => value is null || value is DBNull ? (string?)null : throw new InvalidCastException();


        /// <summary>
        /// Gibt <c>null</c> zurück.
        /// </summary>
        /// <param name="value">Ein beliebiger <see cref="string"/> oder <c>null</c>.</param>
        /// <returns><c>null</c></returns>
        public object? Parse(string? value) => null;

    }
}
