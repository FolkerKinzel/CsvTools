using System;

namespace FolkerKinzel.CsvTools.Helpers
{
    /// <summary>
    /// Definiert eine Schnittstelle zur Umwandlung eines <see cref="string"/>-Objekts in einen anderen Datentyp und 
    /// zurück. Objekte, die die Schnittstelle implementieren, werden von der Klasse <see cref="CsvProperty"/> benötigt.
    /// </summary>
    /// <remarks>
    /// Mit Klasse <see cref="CsvConverterFactory"/> können bereits fertige Implementierungen für die wichtigsten Datentypen instanziiert 
    /// werden. Wenn der Funktionsumfang der im Namespace <see cref="FolkerKinzel.CsvTools.Helpers.Converters"/> zur Verfügung stehenden
    /// Implementierungen des Interfaces nicht Ihre Anforderungen erfüllt 
    /// oder
    /// wenn Sie komplexe Datentypen umwandeln müssen, können Sie leicht selbst eine Klasse erstellen, die <see cref="ICsvTypeConverter"/>
    /// implementiert.</remarks>
    public interface ICsvTypeConverter
    {
        /// <summary>
        /// Wandelt einen <see cref="string"/> in den von <see cref="Type"/> zurückgegebenen Datentyp um.
        /// </summary>
        /// <param name="value">Der zu konvertierende <see cref="string"/> oder <c>null</c>.</param>
        /// <returns><paramref name="value"/>, in den Datentyp von <see cref="Type"/> konvertiert.</returns>
        object? Parse(string? value);


        /// <summary>
        /// Wandelt <paramref name="value"/> in einen <see cref="string"/> um.
        /// </summary>
        /// <param name="value">Ein <see cref="object"/>, dessen Datentyp dem Rückgabewert von <see cref="Type"/> entspricht.</param>
        /// <returns><paramref name="value"/>, in einen <see cref="string"/> umgewandelt.</returns>
        /// <exception cref="InvalidCastException">Der Datentyp von 
        /// <paramref name="value"/> stimmt nicht mit dem Rückgabewert von <see cref="Type"/> überein.</exception>
        /// <remarks>
        /// <note type="implement">
        /// Die Methode sollte eine 
        /// <see cref="InvalidCastException"/> werfen, wenn der Datentyp von <paramref name="value"/> nicht dem Rückgabewert von 
        /// <see cref="Type"/> entspricht.
        /// </note>
        /// </remarks>
        string? ConvertToString(object? value);


        /// <summary>
        /// Wert, der zurückgegeben wird, wenn <see cref="CsvProperty"/> keine Daten
        /// in den Spalten der CSV-Datei (repräsentiert duch <see cref="CsvRecord"/>) findet oder wenn
        /// <see cref="Parse(string)"/> scheitert und keine <see cref="Exception"/> geworfen wird.
        /// </summary>
        /// <value>Ein Objekt des von <see cref="Type"/> zurückgegebenen Datentyps.</value>
        object? FallbackValue { get; }


        /// <summary>
        /// Der Datentyp, den <see cref="ICsvTypeConverter"/> parsen kann, bzw. den <see cref="ICsvTypeConverter"/>
        /// in einen <see cref="string"/> umwandeln kann.
        /// </summary>
        Type Type { get; }


        /// <summary>
        /// <c>true</c> gibt an, dass das <see cref="ICsvTypeConverter"/>-Objekt eine Ausnahme wirft, wenn <see cref="Parse(string)"/>
        /// scheitert. Anderenfalls wird in diesem Fall <see cref="FallbackValue"/> zurückgegeben.
        /// </summary>
        bool ThrowsOnParseErrors { get; }
        
    }
}