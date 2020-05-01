using System;

namespace FolkerKinzel.CsvTools.Helpers.Converters
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
        /// Kovertiert einen <see cref="string"/> in den von <see cref="Type"/> zurückgegebenen Datentyp.
        /// </summary>
        /// <param name="value">Der zu konvertierende <see cref="string"/> oder <c>null</c>.</param>
        /// <returns><paramref name="value"/>, in den Datentyp von <see cref="Type"/> konvertiert.</returns>
        object? Parse(string? value);


        /// <summary>
        /// Konvertiert <paramref name="value"/> in einen <see cref="string"/>.
        /// </summary>
        /// <param name="value">Ein <see cref="object"/> einer beliebigen Klasse oder <c>null</c>. Die Methode sollte aber eine 
        /// <see cref="InvalidCastException"/> werfen, wenn der Datentyp von <paramref name="value"/> nicht dem Rückgabewert von 
        /// <see cref="Type"/> entspricht.</param>
        /// <returns><paramref name="value"/>, in einen <see cref="string"/> umgewandelt.</returns>
        /// <exception cref="InvalidCastException">Diese Ausnahme sollte die Methode werfen, wenn der Datentyp von 
        /// <paramref name="value"/> nicht dem Rückgabewert von <see cref="Type"/> entspricht.</exception>
        string? ConvertToString(object? value);


        /// <summary>
        /// Ein Objekt des von <see cref="Type"/> zurückgegebenen Datentyps, das als Rückgabewert des 
        /// <see cref="ICsvTypeConverter"/>-Objekts verwendet wird, wenn <see cref="CsvProperty"/> kein Zugriffsziel
        /// in den Spalten der CSV-Datei (repräsentiert duch <see cref="CsvRecord"/>) findet. <see cref="FallbackValue"/>
        /// sollte auch dann zurückgegeben werden, wenn die Typkonvertierung in <see cref="Parse(string)"/> scheitert 
        /// und keine <see cref="Exception"/> geworfen wird.
        /// </summary>
        object? FallbackValue { get; }


        /// <summary>
        /// Der Datentyp, in den <see cref="ICsvTypeConverter"/> konvertieren kann, bzw. den <see cref="ICsvTypeConverter"/>
        /// in einen <see cref="string"/> verwandeln kann.
        /// </summary>
        Type Type { get; }


        /// <summary>
        /// <c>true</c> gibt an, dass das <see cref="ICsvTypeConverter"/>-Objekt eine Ausnahme wirft, wenn <see cref="Parse(string)"/>
        /// scheitert. Anderenfalls wird in diesem Fall <see cref="FallbackValue"/> zurückgegeben.
        /// </summary>
        bool ThrowsOnParseErrors { get; }
        
    }
}