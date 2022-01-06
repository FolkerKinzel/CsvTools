using System.Diagnostics;
using FolkerKinzel.CsvTools.TypeConversions.Converters;

#if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Abstrakte Basisklasse für Klassen, die eine Eigenschaft von <see cref="CsvRecordWrapper"/> repräsentieren, die dynamisch zur Laufzeit
/// implementiert wird, und die ihre Daten aus einer einzelnen Spalte der CSV-Datei bezieht.
/// </summary>
public abstract class CsvSingleColumnProperty : CsvPropertyBase
{
    /// <summary>
    /// Initialisiert ein neues <see cref="CsvSingleColumnProperty"/>-Objekt.
    /// </summary>
    /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für C#-Bezeichner
    /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
    /// <param name="converter">Der <see cref="ICsvTypeConverter"/>, der die Typkonvertierung übernimmt.</param>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
    /// ASCII-Zeichen).</exception>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder 
    /// <paramref name="converter"/> ist <c>null</c>.</exception>
    protected CsvSingleColumnProperty(string propertyName, ICsvTypeConverter2 converter) : base(propertyName)
    {
        if (converter is null)
        {
            throw new ArgumentNullException(nameof(converter));
        }

        this.Converter = converter;
    }

    /// <summary>
    /// Ein <see cref="ICsvTypeConverter"/>-Objekt, das die Typkonvertierung durchführt.
    /// </summary>
    public ICsvTypeConverter2 Converter { get; }

    /// <summary>
    /// Das <see cref="CsvRecord"/>-Objekt, über das der Zugriff auf die CSV-Datei erfolgt.
    /// </summary>
    protected internal override CsvRecord? Record { get; internal set; }


    /// <summary>
    /// Der Index der Spalte der CSV-Datei, auf die <see cref="CsvColumnNameProperty"/> zugreift oder <c>null</c>,
    /// wenn <see cref="CsvColumnNameProperty"/> kein Ziel in der CSV-Datei findet. Die Eigenschaft wird beim
    /// ersten Lese- oder Schreibzugriff aktualisiert.
    /// </summary>
    public int? ReferredCsvColumnIndex { get; protected set; }

    /// <summary>
    /// Wird bei jedem Schreib- oder Lesevorgang aufgerufen, um zu überprüfen, ob <see cref="ReferredCsvColumnIndex"/> noch aktuell ist.
    /// </summary>
    protected abstract void UpdateReferredCsvColumnIndex();

    /// <inheritdoc/>
    protected internal override object? GetValue()
    {
        Debug.Assert(Record != null);
        UpdateReferredCsvColumnIndex();

        try
        {
            return ReferredCsvColumnIndex.HasValue ? Converter.Parse(Record[ReferredCsvColumnIndex.Value]) : Converter.Parse(null);
        }
        catch (Exception e)
        {
            throw new InvalidCastException(e.Message, e);
        }
    }


    /// <inheritdoc/>
    protected internal override void SetValue(object? value)
    {
        Debug.Assert(Record != null);
        UpdateReferredCsvColumnIndex();

        if (ReferredCsvColumnIndex.HasValue)
        {
            string? s = Converter.ConvertToString(value);
            Record[ReferredCsvColumnIndex.Value] = s;
        }
    }

}
