# if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Abstrakte Basisklasse für Klassen, die eine Eigenschaft von <see cref="CsvRecordWrapper"/> repräsentieren, die dynamisch zur Laufzeit
/// implementiert wird, und deren Daten aus verschiedenen Spalten der CSV-Datei stammen.
/// </summary>
public class CsvMultiColumnProperty : CsvPropertyBase
{
    /// <summary>
    /// Initialisiert ein neues <see cref="CsvMultiColumnProperty"/>-Objekt.
    /// </summary>
    /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für C#-Bezeichner
    /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
    /// <param name="converter">Ein von <see cref="CsvMultiColumnTypeConverter"/> abgeleitetes Objekt, das die Typkonvertierung durchführt.</param>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
    /// ASCII-Zeichen).</exception>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder <paramref name="converter"/> ist <c>null</c>.
    public CsvMultiColumnProperty(string propertyName, CsvMultiColumnTypeConverter converter) : base(propertyName)
    {
        if (converter is null)
        {
            throw new ArgumentNullException(nameof(converter));
        }

        this.Converter = converter;
    }

    /// <summary>
    /// Ein von <see cref="CsvMultiColumnTypeConverter"/> abgeleitetes Objekt, das die Typkonvertierung durchführt.
    /// </summary>
    public CsvMultiColumnTypeConverter Converter { get; }

    /// <summary>
    /// Das <see cref="CsvRecord"/>-Objekt, über das der Zugriff auf die CSV-Datei erfolgt.
    /// </summary>
    protected internal override CsvRecord? Record { get => Converter.Wrapper.Record; internal set => Converter.Wrapper.Record = value; }

    /// <inheritdoc/>
    protected internal override object? GetValue() => Converter.Create();

    /// <inheritdoc/>
    protected internal override void SetValue(object? value) => Converter.ToCsv(value);
}
