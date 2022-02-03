using FolkerKinzel.CsvTools.TypeConversions.Resources;
using System.Text.RegularExpressions;

# if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Abstrakte Basisklasse für Klassen, die eine Eigenschaft von <see cref="CsvRecordWrapper"/> repräsentieren, die dynamisch zur Laufzeit
/// implementiert wird.
/// </summary>
/// <remarks>
/// <see cref="CsvPropertyBase"/> kapselt Informationen, die <see cref="CsvRecordWrapper"/> benötigt,
/// um auf die Daten des ihm zugrundeliegenden <see cref="CsvRecord"/>-Objekts zuzugreifen.
/// </remarks>
public abstract class CsvPropertyBase
{
    /// <summary>
    /// Initialisiert ein neues <see cref="CsvPropertyBase"/>-Objekt.
    /// </summary>
    /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für C#-Bezeichner
    /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
    /// 
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
    /// ASCII-Zeichen).</exception>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> ist <c>null</c>.
    protected CsvPropertyBase(string propertyName)
    {
        if (propertyName is null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        if (!Regex.IsMatch(propertyName, "^[A-Za-z_][A-Za-z0-9_]*$"))
        {
            throw new ArgumentException(Res.BadIdentifier, nameof(propertyName));
        }

        this.PropertyName = propertyName;
    }


    /// <summary>
    /// Bezeichner der Eigenschaft
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Das <see cref="CsvRecord"/>-Objekt, über das der Zugriff auf die CSV-Datei erfolgt.
    /// </summary>
    protected internal abstract CsvRecord? Record { get; internal set; }

    /// <summary>
    /// Extrahiert Daten eines bestimmten Typs aus <see cref="CsvRecord"/>.
    /// </summary>
    /// <param name="record">Das <see cref="CsvRecord"/>-Objekt, aus dem gelesen wird.</param>
    /// <returns>Die extrahierten Daten.</returns>
    protected internal abstract object? GetValue();

    /// <summary>
    /// Speichert Daten eines bestimmten Typs in der CSV-Datei./>.
    /// </summary>
    /// <param name="record">Das <see cref="CsvRecord"/>-Objekt, in das die Daten hineingeschrieben werden.</param>
    /// <param name="value">Das zu speichernde Objekt.</param>
    /// <exception cref="InvalidCastException"><paramref name="value"/> entspricht nicht dem erwarteten Datentyp.</exception>
    protected internal abstract void SetValue(object? value);
}
