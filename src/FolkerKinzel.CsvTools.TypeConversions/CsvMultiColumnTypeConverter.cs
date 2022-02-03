namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Abstrakte Basisklasse zur Serialisierung und Deserialisierung von Objekten, deren Daten auf mehrere Spalten einer CSV-Datei verteilt sind.
/// Instanzen, die von dieser Klasse abgeleitet sind, werden von <see cref="CsvMultiColumnProperty" benötigt.
/// </summary>
public abstract class CsvMultiColumnTypeConverter
{
    protected CsvMultiColumnTypeConverter(CsvRecordWrapper wrapper)
    {
        if (wrapper is null)
        {
            throw new ArgumentNullException(nameof(wrapper));
        }

        this.Wrapper = wrapper;
    }

    public CsvRecordWrapper Wrapper { get; }


    /// <summary>
    /// Liest die Daten aus <see cref="Wrapper"/> und versucht, daraus eine Instanz des gewünschten Typs zu erzeugen.
    /// </summary>
    /// <returns>Eine Instanz des gewünschten Typs oder ein beliebiges FallbackValue (z.B. <c>null</c> oder <see cref="DBNull.Value"/>).</returns>
    public abstract object? Create();


    /// <summary>
    /// Schreibt <paramref name="value"/> mit Hilfe von <see cref="Wrapper"/> in die CSV-Datei.
    /// </summary>
    /// <param name="value">Das in die CSV-Datei zu schreibende Objekt.</param>
    /// <exception cref="InvalidCastException"><paramref name="value"/> hat einen unerwarteten Datentyp.</exception>
    /// <remarks>
    /// <note type="inherit">
    /// Die Methode sollte eine 
    /// <see cref="InvalidCastException"/> werfen, wenn <paramref name="value"/> nicht <typeparamref name="T"/> oder einem
    /// anderen erwarteten Datentyp (z.B. <see cref="DBNull.Value"/>) entspricht.
    /// </note>
    /// </remarks>
    public abstract void ToCsv(object? value);

}
