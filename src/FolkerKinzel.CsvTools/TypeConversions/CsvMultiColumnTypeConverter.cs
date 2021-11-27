namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Definiert eine Schnittstelle zur Umwandlung eines <see cref="string"/>-Objekts in einen anderen Datentyp und 
/// zurück. Objekte, die die Schnittstelle implementieren, werden von der Klasse <see cref="CsvProperty"/> benötigt.
/// </summary>
/// <remarks>
/// Mit Klasse <see cref="CsvConverterFactory"/> können bereits fertige Implementierungen für die wichtigsten Datentypen instanziiert 
/// werden. Wenn der Funktionsumfang der im Namespace <see cref="FolkerKinzel.CsvTools.TypeConversions.Converters"/> zur Verfügung stehenden
/// Implementierungen des Interfaces nicht Ihre Anforderungen erfüllt 
/// oder
/// wenn Sie komplexe Datentypen umwandeln müssen, können Sie leicht selbst eine Klasse erstellen, die <see cref="ICsvTypeConverter"/>
/// implementiert.</remarks>
public abstract class CsvMultiColumnTypeConverter<T>
{
    public CsvMultiColumnTypeConverter(CsvRecordWrapper wrapper)
    {
        if (wrapper is null)
        {
            throw new ArgumentNullException(nameof(wrapper));
        }

        this.Wrapper = wrapper;
    }

    public CsvRecordWrapper Wrapper { get; }


    /// <summary>
    /// Wandelt einen <see cref="string"/> in den von <see cref="Type"/> zurückgegebenen Datentyp um.
    /// </summary>
    /// <param name="value">Der zu konvertierende <see cref="string"/> oder <c>null</c>.</param>
    /// <returns><paramref name="value"/>, in den Datentyp von <see cref="Type"/> konvertiert.</returns>
    public abstract T Create();


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
    public abstract void ToCsv(T value);

}
