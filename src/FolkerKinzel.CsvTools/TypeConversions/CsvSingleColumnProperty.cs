using System.Diagnostics;

# if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.CsvTools.TypeConversions;

public abstract class CsvSingleColumnProperty : CsvPropertyBase
{
    public CsvSingleColumnProperty(string propertyName, ICsvTypeConverter converter) : base(propertyName)
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
    public ICsvTypeConverter Converter { get; }

    

    /// <summary>
    /// Der Index der Spalte der CSV-Datei, auf die <see cref="CsvColumnNameProperty"/> zugreift oder <c>null</c>,
    /// wenn <see cref="CsvColumnNameProperty"/> kein Ziel in der CSV-Datei findet. Die Eigenschaft wird beim
    /// ersten Lese- oder Schreibzugriff aktualisiert.
    /// </summary>
    public int? ReferredCsvColumnIndex { get; protected set; }

    protected abstract void UpdateReferredCsvColumnIndex(CsvRecord record);

    /// <summary>
    /// Extrahiert Daten eines bestimmten Typs aus <see cref="CsvRecord"/>.
    /// </summary>
    /// <param name="record">Das <see cref="CsvRecord"/>-Objekt, aus dem gelesen wird.</param>
    /// <returns></returns>
    /// <exception cref="InvalidCastException"><see cref="ICsvTypeConverter.ThrowsOnParseErrors"/> war <c>true</c> und der Wert konnte
    /// nicht erfolgreich aus <paramref name="record"/> gelesen werden.</exception>
    internal override object? GetValue(CsvRecord record)
    {
        Debug.Assert(record != null);
        UpdateReferredCsvColumnIndex(record);

        try
        {
            return ReferredCsvColumnIndex.HasValue ? Converter.Parse(record[ReferredCsvColumnIndex.Value]) : Converter.FallbackValue;
        }
        catch (Exception e)
        {
            throw new InvalidCastException(e.Message, e);
        }
    }


    /// <summary>
    /// Speichert Daten eines bestimmten Typs in der CSV-Datei./>.
    /// </summary>
    /// <param name="record"></param>
    /// <param name="value"></param>
    /// <exception cref="InvalidCastException"><paramref name="value"/> entsprach nicht dem Datentyp den <see cref="Converter"/> umwandeln kann.</exception>
    internal override void SetValue(CsvRecord record, object? value)
    {
        Debug.Assert(record != null);
        UpdateReferredCsvColumnIndex(record);

        if (ReferredCsvColumnIndex.HasValue)
        {
            string? s = Converter.ConvertToString(value);
            record[ReferredCsvColumnIndex.Value] = s;
        }
    }

}
