using System;

namespace FolkerKinzel.CsvTools.Helpers.Converters
{
    /// <summary>
    /// Wandelt den Datentyp <see cref="object"/> in einen <see cref="string"/> um, indem auf ihm die <see cref="object.ToString"/>-Methode
    /// aufgerufen wird. Ist das Objekt <c>null</c>
    /// oder DBNull.Value wird <see cref="FallbackValue"/> zurückgegeben (d.h. <c>null</c>). Da
    /// der Typ des Objekts nicht bekannt ist, kann <see cref="ObjectConverter"/> einen Eingabestring nicht parsen, sondern
    /// gibt ihn unverändert zurück.
    /// </summary>
    public sealed class ObjectConverter : ICsvTypeConverter
    {
        private static readonly ObjectConverter _objectConverter = new ObjectConverter();

        /// <summary>
        /// Initialisiert ein neues <see cref="ObjectConverter"/>-Objekt.
        /// </summary>
        private ObjectConverter()
        {
        }

        /// <summary>
        /// Gibt ein <see cref="ObjectConverter"/>-Objekt zurück (Singleton).
        /// </summary>
        /// <returns>Ein <see cref="ObjectConverter"/>-Objekt.</returns>
        public static ObjectConverter GetConverter() => _objectConverter;


        /// <summary>
        /// Wert, der zurückgegeben wird, wenn <see cref="CsvProperty"/> keine Daten
        /// in den Spalten der CSV-Datei (repräsentiert duch <see cref="CsvRecord"/>) findet. (<c>null</c>)
        /// </summary>
        public object? FallbackValue => null;

        /// <summary>
        /// Der Datentyp, den die <see cref="ObjectConverter"/>-Klasse umwandeln kann (<see cref="object"/>).
        /// </summary>
        public Type Type => typeof(object);

        /// <summary>
        /// Gibt an, ob eine Ausnahme geworfen wird, wenn <see cref="Parse(string)"/>
        /// scheitert. (Immer <c>false</c>.)
        /// </summary>
        bool ICsvTypeConverter.ThrowsOnParseErrors => false;


        /// <summary>
        /// Gibt <paramref name="value"/> zurück.
        /// </summary>
        /// <param name="value">Der <see cref="string"/>, der die Eingabe des <see cref="ObjectConverter"/>-Objekts darstellt.</param>
        /// <returns><paramref name="value"/>.</returns>
        public object? Parse(string? value) => value;


        /// <summary>
        /// Gibt <c>null</c> zurück, wenn <paramref name="value"/> <c>null</c> oder DBNull.Value ist,
        /// sonst das Ergebnis der <see cref="object.ToString"/>-Methode, die auf <paramref name="value"/> aufgerufen
        /// wird.
        /// </summary>
        /// <param name="value">Ein <see cref="object"/>, DBNull.Value oder <c>null</c>.</param>
        /// <returns><c>null</c> zurück, wenn <paramref name="value"/> <c>null</c> oder DBNull.Value ist,
        /// sonst das Ergebnis der <see cref="object.ToString"/>-Methode, die auf <paramref name="value"/> aufgerufen
        /// wird.</returns>
        public string? ConvertToString(object? value) => Convert.IsDBNull(value) ? null : value?.ToString();


    }
}
