using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using FolkerKinzel.CsvTools.Extensions;
using FolkerKinzel.CsvTools.Resources;

namespace FolkerKinzel.CsvTools.TypeConversions;

/// <summary>
/// Die Klasse bildet einen Wrapper um Objekte der Klasse <see cref="CsvRecord"/>, der es ermöglicht, die Daten des <see cref="CsvRecord"/>-Objekts
/// in einer selbst gewählten Reihenfolge zu indexieren, auf die Daten mit dynamisch zur Laufzeit implementierten Eigenschaften zuzugreifen sowie
/// Typkonvertierungen durchzuführen. Um die dynamisch implementierten Eigenschaften der Klasse <see cref="CsvRecordWrapper"/> ("späte Bindung") direkt nutzen zu können 
/// und um die Rückgabewerte der Indexer ohne Cast zuzuweisen, muss die Instanz einer Variablen zugewiesen sein, die mit dem Schlüsselwort <c>dynamic</c>
/// deklariert ist.
/// </summary>
/// <remarks>
/// <para>Nachdem ein <see cref="CsvRecordWrapper"/>-Objekt initialisiert wurde, müssen bei ihm mit der Methode 
/// <see cref="AddProperty(CsvColumnNameProperty)"/>&#160;<see cref="CsvColumnNameProperty"/>-Objekte registriert werden, die dynamische Eigenschaften
/// des <see cref="CsvRecordWrapper"/>-Objekts darstellen und Typkonvertierungen für die im zugrundeliegenden <see cref="CsvRecord"/>-Objekt
/// gespeicherten <see cref="string"/>s übernehmen. Die Reihenfolge, in der die Eigenschaften registriert sind, bestimmt die
/// Reihenfolge, in der die Rückgabewerte dieser Eigenschaften zurückgegeben werden, wenn das <see cref="CsvRecordWrapper"/>-Objekt
/// mit einer <c>foreach</c>-Schleife durchlaufen wird oder wenn mit dem Indexer <see cref="this[int]"/> darauf zugegriffen wird. Da
/// die <see cref="CsvColumnNameProperty"/>-Objekte über den CSV-Spaltenname auf das zugundeliegende <see cref="CsvRecord"/>-Objekt zugreifen,
/// müssen Anzahl und Reihenfolge der im <see cref="CsvRecordWrapper"/>-Objekt registrierten <see cref="CsvColumnNameProperty"/>-Objekte nicht
/// mit den Spalten des zugrundeliegenden <see cref="CsvRecord"/>-Objekts übereinstimmen.
/// </para>
/// <para>Mit den folgenden Methoden kann jederzeit auf die Reihenfolge der registrierten Eigenschaften Einfluss genommen werden:</para>
/// <list type="bullet">
/// <item><see cref="InsertProperty(int, CsvColumnNameProperty)"/></item>
/// <item><see cref="ReplaceProperty(string, CsvColumnNameProperty)"/></item>
/// <item><see cref="ReplacePropertyAt(int, CsvColumnNameProperty)"/></item>
/// <item><see cref="RemoveProperty(string)"/></item>
/// <item><see cref="RemovePropertyAt(int)"/></item>
/// </list>
/// <para>Mit <see cref="Contains(string)"/> können Sie überprüfen, ob ein <see cref="CsvColumnNameProperty"/>-Objekt mit dem angegebenen Namen
/// bereits registriert ist.</para>
/// <para>
/// Auf die Indexer <see cref="this[int]"/> und <see cref="this[string]"/> kann zwar auch zugegriffen werden, wenn die 
/// <see cref="CsvRecordWrapper"/>-Instanz einer normalen Variable zugewiesen ist, jedoch müssen die Rückgabewerte der Indexer dann
/// möglicherweise mit dem Cast-Operator in den eigentlichen Datentyp gecastet werden, da sie vom Typ <see cref="object"/> sind.
/// </para>
/// <para>Wenn die <see cref="CsvRecordWrapper"/>-Instanz einer Variablen zugewiesen ist, die mit dem Schlüsselwort <c>dynamic</c>
/// deklariert wurde, übernimmt die Runtime die notwendigen Casts. Überdies ist es dann möglich, die registrierten 
/// <see cref="CsvColumnNameProperty"/>-Objekte wie normale Eigenschaften über ihren Namen anzusprechen (<see cref="CsvColumnNameProperty.PropertyName"/>).
/// Der Nachteil ist, dass Visual Studio auf dynamischen Variablen keinerlei "IntelliSense" bieten kann.
/// </para>
/// <para>Wenn eine CSV-Datei mit <see cref="CsvWriter"/> geschrieben wird, genügt es, <see cref="CsvRecordWrapper"/> das <see cref="CsvRecord"/>-Objekt,
/// das es mit Daten zu füllen gilt, nur einmal zuzuweisen, da beim Schreiben immer dasselbe
/// <see cref="CsvRecord"/>-Objekt verwendet wird.
/// </para>
/// <para>
/// Beim Lesen einer CSV-Datei mit <see cref="CsvReader"/> verhält es sich anders: Dem
/// <see cref="CsvRecordWrapper"/>-Objekt muss bei jeder Iteration des Readers zuerst das aktuelle <see cref="CsvRecord"/>-Objekt zugewiesen
/// werden, bevor auf die Eigenschaften des <see cref="CsvRecordWrapper"/>-Objekts zugegriffen wird, denn <see cref="CsvReader"/> gibt
/// bei jeder Iteration ein neues <see cref="CsvRecord"/>-Objekt zurück. (Das gilt nicht, wenn der <see cref="CsvReader"/> mit <see cref="CsvOptions.DisableCaching"/>
/// initialisiert wurde.)
/// </para>
/// <para>
/// Wenn <see cref="CsvMultiColumnProperty"/>-Objekte in <see cref="CsvRecordWrapper"/> eingefügt werden, erhalten die <see cref="CsvRecordWrapper"/>-Objekte
/// ihrer <see cref="CsvMultiColumnTypeConverter"/> das aktuelle <see cref="CsvRecord"/>-Objekt automatisch über den Mutter-Wrapper, in den sie eingefügt sind.
/// </para>
/// </remarks>
/// <example>
/// <note type="note">In den folgenden Code-Beispielen wurde - der leichteren Lesbarkeit wegen - auf Ausnahmebehandlung verzichtet.</note>
/// <para>Speichern des Inhalts einer <see cref="DataTable"/> als CSV-Datei und Einlesen von Daten einer CSV-Datei in
/// eine <see cref="DataTable"/>:</para>
/// <code language="cs" source="..\Examples\CsvToDataTable.cs"/>
/// <para>Deserialisieren beliebiger Objekte aus CSV-Dateien:</para>
/// <code language="cs" source="..\Examples\DeserializingClassesFromCsv.cs"/>
/// </example>
public sealed class CsvRecordWrapper : DynamicObject, IEnumerable<KeyValuePair<string, object?>>
{
    private readonly PropertyCollection _dynProps = new();
    private CsvRecord? _record;

    #region ctors

    /// <summary>
    /// Initialisiert ein neues <see cref="CsvRecordWrapper"/>-Objekt. 
    /// </summary>
    /// <remarks>Vor dem Zugriff auf die Eigenschaften muss <see cref="Record"/>
    /// ein <see cref="CsvRecord"/>-Objekt zugewiesen werden.</remarks>
    public CsvRecordWrapper() { }

    #endregion

    #region Properties

    /// <summary>
    /// Das <see cref="CsvRecord"/>-Objekt, 
    /// auf dessen Daten <see cref="CsvRecordWrapper"/> zugreift.
    /// </summary>
    public CsvRecord? Record
    {
        get => _record;

        set
        {
            _record = value;

            for (int i = 0; i < _dynProps.Count; i++)
            {
                _dynProps[i].Record = value;
            }
        }
    }


    /// <summary>
    /// Gibt die Anzahl der im <see cref="CsvRecordWrapper"/> registrierten <see cref="CsvColumnNameProperty"/>-Objekte
    /// zurück.
    /// </summary>
    public int Count => _dynProps.Count;


    /// <summary>
    /// Gibt eine Kopie der in <see cref="CsvRecordWrapper"/> registrierten Eigenschaftsnamen zurück.
    /// </summary>
    public IList<string> PropertyNames => _dynProps.Select(x => x.PropertyName).ToArray();


    /// <summary>
    /// Gibt eine Kopie der in <see cref="CsvRecord"/> gespeicherten Daten zurück.
    /// </summary>
    /// <exception cref="InvalidCastException">Der Rückgabewert einer indexierten <see cref="CsvColumnNameProperty"/> konnte nicht erfolgreich geparst werden und 
    /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvColumnNameProperty"/> war so konfiguriert, dass er in diesem Fall eine
    /// Ausnahme wirft.</exception>
    /// <exception cref="InvalidOperationException">Es wurde versucht, auf die Daten von <see cref="CsvRecordWrapper"/> zuzugreifen, ohne dass diesem
    /// ein <see cref="CsvRecord"/>-Objekt zugewiesen war.</exception>
    public IList<object?> Values => this.Select(x => x.Value).ToArray();



    /// <summary>
    /// Ermöglicht den Zugriff auf die im <see cref="CsvRecordWrapper"/> registrierten Eigenschaften
    /// über einen nullbasierten Index. Der Index entspricht der Reihenfolge, in der die
    /// <see cref="CsvColumnNameProperty"/>-Objekte im <see cref="CsvRecordWrapper"/> registriert sind.
    /// </summary>
    /// <param name="index">Nullbasierter Index der registrierten <see cref="CsvColumnNameProperty"/>-Objekte.</param>
    /// <returns>Rückgabewert der bei <paramref name="index"/> registrierten <see cref="CsvColumnNameProperty"/>.</returns>
    /// <exception cref = "InvalidOperationException" > Der Indexer wurde aufgerufen, bevor dem <see cref= "CsvRecordWrapper" />
    /// ein <see cref="CsvRecord"/>-Objekt zugewiesen wurde.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para></para>
    /// <paramref name="index"/> ist kleiner als 0 oder größer oder gleich <see cref="Count"/>.</exception>
    /// <exception cref="InvalidCastException">
    /// <para>
    /// Einem Index wurde ein Objekt zugewiesen, dessen Datentyp nicht dem Datentyp der an
    /// diesem Index registrierten Property entspricht
    /// </para>
    /// <para>- oder -</para>
    /// <para>
    /// der Rückgabewert der indexierten <see cref="CsvColumnNameProperty"/> konnte nicht erfolgreich geparst werden und 
    /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvColumnNameProperty"/> war so konfiguriert, dass er in diesem Fall eine
    /// Ausnahme wirft.
    /// </para></exception>
    /// <exception cref="InvalidOperationException">Es wurde versucht, auf die Daten von <see cref="CsvRecordWrapper"/> zuzugreifen, ohne dass diesem
    /// ein <see cref="CsvRecord"/>-Objekt zugewiesen war.</exception>
    public object? this[int index]
    {
        get
        {
            return Record is null
                ? throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)))
                : _dynProps[index].GetValue();
        }

        set
        {
            if (Record is null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
            }

            _dynProps[index].SetValue(value);
        }
    }


    /// <summary>
    /// Ermöglicht den Zugriff auf die im <see cref="CsvRecordWrapper"/> registrierten Eigenschaften
    /// über den Wert der Eigenschaft <see cref="CsvColumnNameProperty.PropertyName"/>.
    /// </summary>
    /// <param name="propertyName">Name der registrierten Eigenschaft. (Entspricht <see cref="CsvColumnNameProperty.PropertyName"/>.) Der
    /// Vergleich erfolgt case-sensitiv.</param>
    /// <returns>Rückgabewert der registrierten <see cref="CsvColumnNameProperty"/>, deren Eigenschaft <see cref="CsvColumnNameProperty.PropertyName"/>&#160;<paramref name="propertyName"/>
    /// entspricht. Der Vergleich ist case-sensitiv.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Es wurde unter den bei <see cref="CsvRecordWrapper"/> registrierten <see cref="CsvColumnNameProperty"/>-Eigenschaften kein
    /// <see cref="CsvColumnNameProperty"/>-Objekt gefunden, dessen Eigenschaft <see cref="CsvColumnNameProperty.PropertyName"/>&#160;<paramref name="propertyName"/> entspricht.</exception>
    /// <exception cref="InvalidCastException">
    /// <para>
    /// Einem Index wurde ein Objekt zugewiesen, dessen Datentyp nicht dem Datentyp der an
    /// diesem Index registrierten Property entspricht
    /// </para>
    /// <para>- oder -</para>
    /// <para>
    /// der Rückgabewert der indexierten <see cref="CsvColumnNameProperty"/> konnte nicht erfolgreich geparst werden und 
    /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvColumnNameProperty"/> war so konfiguriert, dass er in diesem Fall eine
    /// Ausnahme wirft.
    /// </para></exception>
    /// <exception cref="InvalidOperationException">Es wurde versucht, auf die Daten von <see cref="CsvRecordWrapper"/> zuzugreifen, ohne dass diesem
    /// ein <see cref="CsvRecord"/>-Objekt zugewiesen war.</exception>
    public object? this[string propertyName]
    {
        get
        {
            if (Record is null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
            }

            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            return this._dynProps.TryGetValue(propertyName, out CsvPropertyBase? prop)
                ? prop.GetValue()
                : throw new ArgumentException(Res.PropertyNotFound, nameof(propertyName));
        }

        set
        {
            if (Record is null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
            }

            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }


            if (this._dynProps.TryGetValue(propertyName, out CsvPropertyBase? prop))
            {
                prop.SetValue(value);
            }
            else
            {
                throw new ArgumentException(Res.PropertyNotFound, nameof(propertyName));
            }
        }
    }

    #endregion


    /// <summary>
    /// Registriert eine <see cref="CsvColumnNameProperty"/> am Ende der Auflistung der registrierten Eigenschaften.
    /// </summary>
    /// <param name="property">Die anzufügende <see cref="CsvColumnNameProperty"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Es ist bereits eine <see cref="CsvColumnNameProperty"/> mit demselben
    /// <see cref="CsvColumnNameProperty.PropertyName"/> enthalten. Prüfen Sie das vorher mit <see cref="Contains(string)"/>!</exception>
    public void AddProperty(CsvPropertyBase property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        this._dynProps.Add(property);
        property.Record = Record;
    }


    /// <summary>
    /// Entfernt die <see cref="CsvColumnNameProperty"/> mit dem angegebenen <see cref="CsvColumnNameProperty.PropertyName"/>
    /// aus der Auflistung der registrierten Eigenschaften.
    /// </summary>
    /// <param name="propertyName">Der <see cref="CsvColumnNameProperty.PropertyName"/> der zu entfernenden
    /// <see cref="CsvColumnNameProperty"/>.</param>
    /// <returns><c>true</c>, wenn die gesuchte <see cref="CsvColumnNameProperty"/> in der Auflistung enthalten war
    /// und entfernt werden konnte.</returns>
    public bool RemoveProperty(string? propertyName)
        => propertyName is not null && _dynProps.Remove(propertyName);



    /// <summary>
    /// Entfernt die <see cref="CsvColumnNameProperty"/> am angegebenen <paramref name="index"/> aus der
    /// Auflistung der registrierten Eigenschaften.
    /// </summary>
    /// <param name="index">Der nullbasierte Index, an dem die <see cref="CsvColumnNameProperty"/> entfernt werden soll.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder 
    /// größer oder gleich <see cref="Count"/>.</exception>
    public void RemovePropertyAt(int index)
    => _dynProps.RemoveAt(index);


    /// <summary>
    /// Fügt <paramref name="property"/> am <paramref name="index"/> der registrierten Eigenschaften ein.
    /// </summary>
    /// <param name="index">Der nullbasierte Index, an dem <paramref name="property"/> eingefügt werden soll.</param>
    /// <param name="property">Die einzufügende <see cref="CsvColumnNameProperty"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder 
    /// größer als <see cref="Count"/>.</exception>
    /// <exception cref="ArgumentException">Es ist bereits eine <see cref="CsvColumnNameProperty"/> mit demselben
    /// <see cref="CsvColumnNameProperty.PropertyName"/> enthalten. Prüfen Sie das vorher mit <see cref="Contains(string)"/>!</exception>
    public void InsertProperty(int index, CsvPropertyBase property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        _dynProps.Insert(index, property);
        property.Record = Record;
    }


    /// <summary>
    /// Ersetzt die <see cref="CsvColumnNameProperty"/> am angegebenen Index der Auflistung der registrierten Eigenschaften
    /// durch <paramref name="property"/>.
    /// </summary>
    /// <param name="index">Der nullbasierte Index, an dem das in der Auflistung vorhandene
    /// <see cref="CsvColumnNameProperty"/>-Objekt durch <paramref name="property"/> ersetzt wird.</param>
    /// <param name="property">Ein <see cref="CsvColumnNameProperty"/>-Objekt.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder 
    /// größer oder gleich <see cref="Count"/>.</exception>
    /// <exception cref="ArgumentException">Es ist bereits eine <see cref="CsvColumnNameProperty"/> mit demselben
    /// <see cref="CsvColumnNameProperty.PropertyName"/> enthalten. Prüfen Sie das vorher mit <see cref="Contains(string)"/>!</exception>
    public void ReplacePropertyAt(int index, CsvPropertyBase property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        _dynProps[index] = property;
    }




    /// <summary>
    /// Ersetzt die registrierte Eigenschaft, deren <see cref="CsvColumnNameProperty.PropertyName"/> gleich <paramref name="propertyName"/>
    /// ist durch <paramref name="property"/>.
    /// </summary>
    /// <param name="propertyName">Bezeichner der registrierten Eigenschaft, die ersetzt werden soll.</param>
    /// <param name="property"><see cref="CsvColumnNameProperty"/>-Objekt, mit dem ersetzt werden soll.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder <paramref name="property"/>
    /// ist <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Es ist keine Eigenschaft unter der Bezeichnung <paramref name="propertyName"/>
    /// registriert - oder - in der Auflistung der registrierten Eigenschaften befindet sich bereits ein 
    /// <see cref="CsvColumnNameProperty"/>-Objekt dessen <see cref="CsvColumnNameProperty.PropertyName"/>-Eigenschaft identisch
    /// mit der von <paramref name="property"/> ist. Prüfen Sie das vorher mit <see cref="Contains(string)"/>!</exception>
    public void ReplaceProperty(string propertyName, CsvPropertyBase property)
    {
        if (propertyName is null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }


        int index = _dynProps.IndexOf(_dynProps[propertyName]);

        _dynProps[index] = index >= 0 ? property : throw new ArgumentException(Res.PropertyNotFound, nameof(propertyName));
    }

    /// <summary>
    /// Untersucht, ob ein <see cref="CsvColumnNameProperty"/>-Objekt im <see cref="CsvRecordWrapper"/> unter dem mit
    /// <paramref name="propertyName"/> angegebenen Namen registriert ist.
    /// </summary>
    /// <param name="propertyName">Der <see cref="CsvColumnNameProperty.PropertyName"/> der zu suchenden
    /// <see cref="CsvColumnNameProperty"/>.</param>
    /// <returns><c>true</c>, wenn ein <see cref="CsvColumnNameProperty"/>-Objekt unter dem mit <paramref name="propertyName"/>
    /// angegebenen Namen registriert ist.</returns>
    public bool Contains(string? propertyName) => propertyName is not null && _dynProps.Contains(propertyName);


    /// <summary>
    /// Gibt den Index der Eigenschaft zurück, die den Eigenschaftsnamen (<see cref="CsvColumnNameProperty.PropertyName"/>) der 
    /// <paramref name="propertyName"/> entspricht oder -1, wenn eine solche Eigenschaft nicht in <see cref="CsvRecordWrapper"/> registriert ist.
    /// </summary>
    /// <param name="propertyName">Der Eigenschaftsname der zu suchenden <see cref="CsvColumnNameProperty"/>.</param>
    /// <returns>Der Index der Eigenschaft, die <paramref name="propertyName"/> als Eigenschaftsnamen (<see cref="CsvColumnNameProperty.PropertyName"/>) hat
    /// oder -1, wenn eine solche Eigenschaft nicht in <see cref="CsvRecordWrapper"/> registriert ist.</returns>
    public int IndexOf(string? propertyName) => propertyName is null ? -1 : _dynProps.Contains(propertyName) ? _dynProps.IndexOf(_dynProps[propertyName]) : -1;


    /// <summary>
    /// Wird automatisch aufgerufen, wenn einer dynamisch implementierten Eigenschaft ein Wert
    /// zugewiesen wird. (Nicht zur direkten Verwendung in eigenem Code bestimmt.)
    /// </summary>
    /// <param name="binder">Informationen über die aufrufende dynamische Eigenschaft.</param>
    /// <param name="value">Das Objekt, das der dynamisch implementierten Eigenschaft zugewiesen wird.</param>
    /// <returns><c>true</c>, wenn auf eine Eigenschaft zugegriffen wurde, die zuvor als <see cref="CsvColumnNameProperty"/>
    /// im <see cref="CsvRecordWrapper"/>-Objekt registriert wurde.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="binder"/> ist <c>null</c>. (Das kann nur passieren,
    /// wenn die Methode direkt aus eigenem Code aufgerufen wird.)</exception>
    /// <exception cref="InvalidCastException"><paramref name="value"/> ist nicht vom Datentyp der registrierten
    /// Property.</exception>
    /// <exception cref="Exception">Es wurde versucht, auf eine nicht registrierte Property zuzugreifen.</exception>
    ///  <exception cref="InvalidOperationException">Es wurde versucht, auf die Daten von <see cref="CsvRecordWrapper"/> zuzugreifen, ohne dass diesem
    /// ein <see cref="CsvRecord"/>-Objekt zugewiesen war.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if (binder is null)
        {
            throw new ArgumentNullException(nameof(binder));
        }

        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
        }

        if (this._dynProps.TryGetValue(binder.Name, out CsvPropertyBase? prop))
        {
            prop.SetValue(value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Wird automatisch aufgerufen, wenn der Wert einer dynamisch implementierten Eigenschaft
    /// einer Variablen zugewiesen wird. (Nicht zur direkten Verwendung in eigenem Code bestimmt.)
    /// </summary>
    /// <param name="binder">Informationen über die aufrufende dynamische Eigenschaft.</param>
    /// <param name="result">Das Objekt, das den Rückgabewert der dynamisch implementierten Eigenschaft darstellt.</param>
    /// <returns><c>true</c>, wenn auf eine Eigenschaft zugegriffen wurde, die zuvor als <see cref="CsvColumnNameProperty"/>
    /// im <see cref="CsvRecordWrapper"/>-Objekt registriert wurde.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="binder"/> ist <c>null</c>. (Das kann nur passieren,
    /// wenn die Methode direkt aus eigenem Code aufgerufen wird.)</exception>
    /// <exception cref="Exception">Der Rückgabewert der indexierten <see cref="CsvColumnNameProperty"/> konnte nicht erfolgreich geparst werden und 
    /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvColumnNameProperty"/> war so konfiguriert, dass er in diesem Fall eine
    /// Ausnahme wirft.</exception>
    /// <exception cref="InvalidOperationException">Es wurde versucht, auf die Daten von <see cref="CsvRecordWrapper"/> zuzugreifen, ohne dass diesem
    /// ein <see cref="CsvRecord"/>-Objekt zugewiesen war.</exception>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (binder is null)
        {
            throw new ArgumentNullException(nameof(binder));
        }

        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
        }

        if (this._dynProps.TryGetValue(binder.Name, out CsvPropertyBase? prop))
        {
            result = prop.GetValue();
            return true;
        }

        result = null;
        return false;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override IEnumerable<string> GetDynamicMemberNames() => base.GetDynamicMemberNames();

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override DynamicMetaObject GetMetaObject(Expression parameter) => base.GetMetaObject(parameter);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object? result) => base.TryBinaryOperation(binder, arg, out result);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryConvert(ConvertBinder binder, out object? result) => base.TryConvert(binder, out result);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryCreateInstance(CreateInstanceBinder binder, object?[]? args, [NotNullWhen(true)] out object? result) => base.TryCreateInstance(binder, args, out result);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes) => base.TryDeleteIndex(binder, indexes);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryDeleteMember(DeleteMemberBinder binder) => base.TryDeleteMember(binder);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result) => base.TryGetIndex(binder, indexes, out result);

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result) => base.TryInvoke(binder, args, out result);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result) => base.TryInvokeMember(binder, args, out result);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value) => base.TrySetIndex(binder, indexes, value);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool TryUnaryOperation(UnaryOperationBinder binder, out object? result) => base.TryUnaryOperation(binder, out result);

    ///// <summary>
    ///// Setzt den Wert sämtlicher Spalten des zugrundeliegenden <see cref="CsvRecord"/>-Objekts auf <c>null</c>.
    ///// </summary>
    ///// <exception cref="InvalidOperationException"><see cref="Record"/> ist beim Aufruf der Methode <c>null</c>.</exception>
    //public void Clear()
    //{
    //    if (Record is null)
    //    {
    //        throw new InvalidOperationException("Record is null.");
    //    }
    //    else
    //    {
    //        Record.Clear();
    //    };
    //}


    /// <summary>
    /// Gibt einen <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string, object?&gt;&gt;</see> zurück, mit dem die Rückgabewerte der 
    /// dynamisch implementierten Eigenschaften durchlaufen werden. Die Reihenfolge
    /// entspricht der Reihenfolge, in der die
    /// <see cref="CsvColumnNameProperty"/>-Objekte im <see cref="CsvRecordWrapper"/> registriert sind.
    /// </summary>
    /// <returns>Ein <see cref="IEnumerator{T}">IEnumerator&lt;KeyValuePair&lt;string, object?&gt;&gt;</see>.</returns>
    /// <exception cref="InvalidCastException">Der Rückgabewert einer indexierten <see cref="CsvColumnNameProperty"/> konnte nicht erfolgreich geparst werden und 
    /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvColumnNameProperty"/> war so konfiguriert, dass er in diesem Fall eine
    /// Ausnahme wirft.</exception>
    /// <exception cref="InvalidOperationException">Es wurde versucht, auf die Daten von <see cref="CsvRecordWrapper"/> zuzugreifen, ohne dass diesem
    /// ein <see cref="CsvRecord"/>-Objekt zugewiesen war.</exception>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        if (Record is null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.CsvRecordIsNull, nameof(Record)));
        }


        foreach (CsvPropertyBase? prop in this._dynProps)
        {
            yield return new KeyValuePair<string, object?>(prop.PropertyName, prop.GetValue());
        }
    }


    /// <summary>
    /// Gibt einen <see cref="IEnumerator"/> zurück, mit dem die Rückgabewerte der 
    /// dynamisch implementierten Eigenschaften durchlaufen werden können. Die Reihenfolge
    /// entspricht der Reihenfolge, in der die
    /// <see cref="CsvColumnNameProperty"/>-Objekte im <see cref="CsvRecordWrapper"/> registriert sind.
    /// </summary>
    /// <returns>Ein <see cref="IEnumerator"/>.</returns>
    /// <exception cref="InvalidCastException">Der Rückgabewert einer indexierten <see cref="CsvColumnNameProperty"/> konnte nicht erfolgreich geparst werden und 
    /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvColumnNameProperty"/> war so konfiguriert, dass er in diesem Fall eine
    /// Ausnahme wirft.</exception>
    /// <exception cref="InvalidOperationException">Es wurde versucht, auf die Daten von <see cref="CsvRecordWrapper"/> zuzugreifen, ohne dass diesem
    /// ein <see cref="CsvRecord"/>-Objekt zugewiesen war.</exception>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



    /// <inheritdoc/>
    [SuppressMessage("Design", "CA1031:Keine allgemeinen Ausnahmetypen abfangen", Justification = "<Ausstehend>")]
    public override string ToString()
    {
        if (Record is null || Count == 0)
        {
            return base.ToString() ?? string.Empty;
        }

        var sb = new StringBuilder();

        foreach (string propName in this.PropertyNames)
        {
            object? value;

            string valString;

            try
            {
                value = this[propName];
                valString = value is null ? "<null>" : value is DBNull ? "<DBNull>" : value.ToString() ?? string.Empty;
            }
            catch
            {
                valString = "<Exception>";
            }

            _ = sb.Append(propName).Append(": ").Append(valString).Append(", ");
        }

        if (sb.Length >= 2)
        {
            sb.Length -= 2;
        }

        return sb.ToString();
    }

    /// ////////////////////////////////////////////////////////////////////////

    private class PropertyCollection : KeyedCollection<string, CsvPropertyBase>
    {
        protected override string GetKeyForItem(CsvPropertyBase item) => item.PropertyName;

        public PropertyCollection() : base(StringComparer.Ordinal) { }
    }
}
