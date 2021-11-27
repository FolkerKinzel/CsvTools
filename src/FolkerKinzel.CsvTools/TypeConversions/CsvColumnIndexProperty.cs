# if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.CsvTools.TypeConversions;

public sealed class CsvColumnIndexProperty : CsvSingleColumnProperty
{
    /// <summary>
    /// Initialisiert ein neues <see cref="CsvColumnIndexProperty"/>-Objekt.
    /// </summary>
    /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für C#-Bezeichner
    /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
    /// <param name="desiredCsvColumnIndex">Nullbasierter Index der Spalte der CSV-Datei in die <see cref="CsvColumnNameProperty"/> schreiben soll bzw.
    /// aus der <see cref="CsvColumnNameProperty"/> lesen soll. Wenn es den Index in der CSV-Datei nicht
    /// gibt, wird die <see cref="CsvColumnNameProperty"/> beim Schreiben ignoriert. Beim Lesen wird in diesem Fall
    /// <see cref="ICsvTypeConverter.FallbackValue"/> zurückgegeben.</param>
    /// <param name="converter">Der <see cref="ICsvTypeConverter"/>, der die Typkonvertierung übernimmt.</param>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
    /// ASCII-Zeichen).</exception>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder 
    /// <paramref name="converter"/> ist <c>null</c>.</exception>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="desiredCsvColumnIndex"/> ist kleiner als 0.</exception>
    public CsvColumnIndexProperty(
        string propertyName, int desiredCsvColumnIndex, ICsvTypeConverter converter) : base(propertyName, converter)
    {
        if (desiredCsvColumnIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(desiredCsvColumnIndex));
        }

        this.DesiredCsvColumnIndex = desiredCsvColumnIndex;
    }

    /// <summary>
    /// Der Index der Spalte der CSV-Datei, auf die <see cref="CsvColumnNameProperty"/> zugreifen soll oder
    /// <c>null</c>, wenn dieser im Konstruktor nicht angegeben wurde.
    /// </summary>
    public int DesiredCsvColumnIndex { get; }

    protected override void UpdateReferredCsvColumnIndex(CsvRecord record)
        => ReferredCsvColumnIndex = DesiredCsvColumnIndex < record.Count ? DesiredCsvColumnIndex : null;
}
