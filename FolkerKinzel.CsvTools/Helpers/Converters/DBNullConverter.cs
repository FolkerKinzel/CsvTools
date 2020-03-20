using System;
using System.Collections.Generic;
using System.Text;

namespace FolkerKinzel.CsvTools.Helpers.Converters
{
    /// <summary>
    /// Wandelt DBNull.Value in einen <c>null</c>-<see cref="string"/> um und führt dabei
    /// eine Typüberprüfung durch. Parst jeglichen <see cref="string"/> oder <c>null</c> als
    /// DBNull.Value.
    /// </summary>
    public class DBNullConverter : ICsvTypeConverter
    {
        private static readonly DBNullConverter _converter = new DBNullConverter();

        private DBNullConverter() { }

        /// <summary>
        /// Gibt ein <see cref="DBNullConverter"/>-Objekt zurück (Singleton).
        /// </summary>
        /// <returns>Ein <see cref="DBNullConverter"/>-Objekt.</returns>
        public static DBNullConverter GetConverter() => _converter;

        /// <summary>
        /// Wert, der zurückgegeben wird, wenn <see cref="CsvProperty"/> keine Daten
        /// in den Spalten der CSV-Datei (repräsentiert duch <see cref="CsvRecord"/>) findet. (DBNull.Value)
        /// </summary>
        public object? FallbackValue => DBNull.Value;


        /// <summary>
        /// Der Datentyp, den die <see cref="DBNullConverter"/>-Klasse umwandeln kann (<see cref="DBNull"/>).
        /// </summary>
        public Type Type => typeof(DBNull);

        /// <summary>
        /// Gibt an, ob eine Ausnahme geworfen wird, wenn <see cref="Parse(string)"/>
        /// scheitert. (Immer <c>false</c>.)
        /// </summary>
        public bool ThrowsOnParseErrors => false;


        /// <summary>
        /// Gibt <c>null</c> zurück, wenn <paramref name="value"/> DBNull.Value ist.
        /// </summary>
        /// <param name="value">DBNull.Value</param>
        /// <returns><c>null</c></returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> ist nicht DBNull.Value.</exception>
        public string? ConvertToString(object? value) => value is DBNull ? (string?)null : throw new InvalidCastException();

        /// <summary>
        /// Gibt DBNull.Value zurück.
        /// </summary>
        /// <param name="value">Ein beliebiger <see cref="string"/> oder <c>null</c>.</param>
        /// <returns>DBNull.Value</returns>
        public object? Parse(string? value) => DBNull.Value;
        
    }
}
