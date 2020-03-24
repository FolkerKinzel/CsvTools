using FolkerKinzel.CsvTools.Extensions;
using FolkerKinzel.CsvTools.Helpers;
using FolkerKinzel.CsvTools.Helpers.Converters;
using FolkerKinzel.CsvTools.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FolkerKinzel.CsvTools.Helpers
{
    /// <summary>
    /// Die Klasse bildet einen Wrapper um Objekte der Klasse <see cref="CsvRecord"/>, der es ermöglicht, die Daten des <see cref="CsvRecord"/>-Objekts
    /// in einer selbst gewählten Reihenfolge zu indexieren, auf die Daten mit dynamisch zur Laufzeit implementierten Eigenschaften zuzugreifen sowie
    /// Typkonvertierungen durchzuführen. Um die dynamisch implementierten Eigenschaften der Klasse <see cref="CsvRecordWrapper"/> ("späte Bindung") direkt nutzen zu können 
    /// und um die Rückgabewerte der Indexer ohne Cast zuzuweisen, muss die Instanz einer Variablen zugewiesen sein, die mit dem Schlüsselwort <c>dynamic</c>
    /// deklariert ist.
    /// </summary>
    /// <remarks>
    /// <para>Nachdem ein <see cref="CsvRecordWrapper"/>-Objekt initialisiert wurde, müssen bei ihm mit der Methode 
    /// <see cref="AddProperty(CsvProperty)"/>&#160;<see cref="CsvProperty"/>-Objekte registriert werden, die dynamische Eigenschaften
    /// des <see cref="CsvRecordWrapper"/>-Objekts darstellen und Typkonvertierungen für die im zugrundeliegenden <see cref="CsvRecord"/>-Objekt
    /// gespeicherten <see cref="string"/>s übernehmen. Die Reihenfolge, in der die Eigenschaften registriert sind, bestimmt die
    /// Reihenfolge, in der die Rückgabewerte dieser Eigenschaften zurückgegeben werden, wenn das <see cref="CsvRecordWrapper"/>-Objekt
    /// mit einer <c>foreach</c>-Schleife durchlaufen wird oder wenn mit dem Indexer <see cref="this[int]"/> darauf zugegriffen wird. Da
    /// die <see cref="CsvProperty"/>-Objekte über den CSV-Spaltenname auf das zugundeliegende <see cref="CsvRecord"/>-Objekt zugreifen,
    /// müssen Anzahl und Reihenfolge der im <see cref="CsvRecordWrapper"/>-Objekt registrierten <see cref="CsvProperty"/>-Objekte nicht
    /// mit den Spalten des zugrundeliegenden <see cref="CsvRecord"/>-Objekts übereinstimmen.
    /// </para>
    /// <para>Mit den folgenden Methoden kann auch nachträglich auf die Reihenfolge der registrierten Eigenschaften Einfluss genommen werden:</para>
    /// <list type="bullet">
    /// <item><see cref="InsertProperty(int, CsvProperty)"/></item>
    /// <item><see cref="ReplaceProperty(string, CsvProperty)"/></item>
    /// <item><see cref="ReplacePropertyAt(int, CsvProperty)"/></item>
    /// <item><see cref="RemoveProperty(string)"/></item>
    /// <item><see cref="RemovePropertyAt(int)"/></item>
    /// </list>
    /// <para>Mit <see cref="HasProperty(string)"/> können Sie überprüfen, ob ein <see cref="CsvProperty"/>-Objekt mit dem angegebenen Namen
    /// bereits registriert ist.</para>
    /// <para>
    /// Auf die Indexer <see cref="this[int]"/> und <see cref="this[string]"/> kann zwar auch zugegriffen werden, wenn die 
    /// <see cref="CsvRecordWrapper"/>-Instanz einer normalen Variable zugewiesen ist, jedoch müssen die Rückgabewerte der Indexer dann
    /// möglicherweise mit dem Cast-Operator in den eigentlichen Datentyp gecastet werden, da sie vom Typ <see cref="object"/> sind.
    /// </para>
    /// <para>Wenn die <see cref="CsvRecordWrapper"/>-Instanz einer Variablen zugewiesen ist, die mit dem Schlüsselwort <c>dynamic</c>
    /// deklariert wurde, übernimmt die Runtime die notwendigen Casts. Überdies ist es dann möglich, die registrierten 
    /// <see cref="CsvProperty"/>-Objekte wie normale Eigenschaften über ihren Namen anzusprechen (<see cref="CsvProperty.PropertyName"/>).
    /// Der Nachteil ist, dass Visual Studio auf dynamischen Variablen keinerlei "Intellisense" bieten kann.
    /// </para>
    /// <para>Wenn eine CSV-Datei mit <see cref="CsvWriter"/> geschrieben wird, ist es möglich den Konstruktor <see cref="CsvRecordWrapper(CsvRecord)"/>
    /// zu verwenden, da das <see cref="CsvRecord"/>-Objekt, das es mit Daten zu füllen gilt, bereits vor dem ersten Schreibvorgang zur Verfügung
    /// steht. Weitere Zuweisungen mit <see cref="SetRecord(CsvRecord)"/> sind nicht notwendig, da <see cref="CsvWriter"/> immer dasselbe
    /// <see cref="CsvRecord"/>-Objekt verwendet.
    /// </para>
    /// <para>
    /// Beim Lesen einer CSV-Datei mit <see cref="CsvReader"/> verhält es sich anders: Ein <see cref="CsvRecord"/>-Objekt steht erst nach der 
    /// ersten Iteration des Readers zur Verfügung. Deshalb kann nur der Konstruktor <see cref="CsvRecordWrapper()"/> benutzt werden und dem
    /// <see cref="CsvRecordWrapper"/>-Objekt muss bei jeder Iteration des Readers zuerst das aktuelle <see cref="CsvRecord"/>-Objekt mit der 
    /// Methode <see cref="SetRecord(CsvRecord)"/> zugewiesen
    /// werden, bevor auf die Eigenschaften des <see cref="CsvRecordWrapper"/>-Objekts zugegriffen wird, denn <see cref="CsvReader"/> gibt
    /// bei jeder Iteration ein neues <see cref="CsvRecord"/>-Objekt zurück.
    /// </para>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Bezeichner müssen ein korrektes Suffix aufweisen", Justification = "<Ausstehend>")]
    public sealed class CsvRecordWrapper : DynamicObject, IList<object?>
    {
        private CsvRecord? _record;

        private readonly PropertyCollection _dynProps = new PropertyCollection();


        /// <summary>
        /// Initialisiert ein <see cref="CsvRecordWrapper"/>-Objekt zum Lesen einer CSV-Datei. Vor dem Zugriff auf die Eigenschaften muss die
        /// Methode <see cref="SetRecord(CsvRecord)"/> aufgerufen werden!
        /// </summary>
        /// <remarks>
        /// <note type="important">
        /// Bei Verwendung dieses Konstruktors müssen Sie die Methode <see cref="CsvRecordWrapper.SetRecord(CsvRecord)"/> aufrufen, bevor Sie auf
        /// Eigenschaften von <see cref="CsvRecordWrapper"/> zugreifen können. Verwenden Sie diesen Konstruktor nur zum Lesen einer CSV-Datei!
        /// </note>
        /// </remarks>
        public CsvRecordWrapper() { }


        /// <summary>
        /// Initialisiert ein <see cref="CsvRecordWrapper"/>-Objekt und weist diesem <paramref name="record"/> zu.
        /// </summary>
        /// <remarks>
        /// Beim Lesen einer CSV-Datei mit <see cref="CsvReader"/> steht ein <see cref="CsvRecord"/>-Objekt erst nach der 
        /// ersten Iteration des Readers zur Verfügung. Deshalb kann nur der Konstruktor <see cref="CsvRecordWrapper()"/> benutzt werden und dem
        /// <see cref="CsvRecordWrapper"/>-Objekt muss bei jeder Iteration des Readers zuerst das aktuelle <see cref="CsvRecord"/>-Objekt 
        /// mit der Methode <see cref="SetRecord(CsvRecord)"/> zugewiesen
        /// werden, bevor auf die Eigenschaften des <see cref="CsvRecordWrapper"/>-Objekts zugegriffen wird, denn <see cref="CsvReader"/> gibt
        /// bei jeder Iteration ein neues <see cref="CsvRecord"/>-Objekt zurück.
        /// </remarks>
        /// <param name="record">Das <see cref="CsvRecord"/>-Objekt, auf dessen Daten <see cref="CsvRecordWrapper"/> zugreift.</param>
        /// <exception cref="ArgumentNullException"><paramref name="record"/> ist <c>null</c>.</exception>
        public CsvRecordWrapper(CsvRecord record)
        {
            this.SetRecord(record);
        }


        /// <summary>
        /// Weist dem <see cref="CsvRecordWrapper"/> ein <see cref="CsvRecord"/>-Objekt zu, 
        /// auf dessen Daten es zugreift.
        /// </summary>
        /// <param name="record">Das <see cref="CsvRecord"/>-Objekt, dessen Daten vom 
        /// <see cref="CsvRecordWrapper"/> gelesen oder manipuliert werden.</param>
        /// <exception cref="ArgumentNullException"><paramref name="record"/> ist <c>null</c>.</exception>
        public void SetRecord(CsvRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this._record = record;
        }


        /// <summary>
        /// Registriert eine <see cref="CsvProperty"/> am Ende der Auflistung der registrierten Eigenschaften.
        /// </summary>
        /// <param name="property">Die anzufügende <see cref="CsvProperty"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Es ist bereits eine <see cref="CsvProperty"/> mit demselben
        /// <see cref="CsvProperty.PropertyName"/> enthalten.</exception>
        public void AddProperty(CsvProperty property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            this._dynProps.Add(property);
        }


        /// <summary>
        /// Entfernt die <see cref="CsvProperty"/> mit dem angegebenen <see cref="CsvProperty.PropertyName"/>
        /// aus der Auflistung der registrierten Eigenschaften.
        /// </summary>
        /// <param name="propertyName">Der <see cref="CsvProperty.PropertyName"/> der zu entfernenden
        /// <see cref="CsvProperty"/>.</param>
        /// <returns>True, wenn die gesuchte <see cref="CsvProperty"/> in der Auflistung enthalten war
        /// und entfernt werden konnte.</returns>
        public bool RemoveProperty(string? propertyName)
            => propertyName is null ? false : _dynProps.Remove(propertyName);


        /// <summary>
        /// Entfernt die <see cref="CsvProperty"/> am angegebenen <paramref name="index"/> aus der
        /// Auflistung der registrierten Eigenschaften.
        /// </summary>
        /// <param name="index">Der nullbasierte Index, an dem die <see cref="CsvProperty"/> entfernt werden soll.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder 
        /// größer oder gleich <see cref="Count"/>.</exception>
        public void RemovePropertyAt(int index) => _dynProps.RemoveAt(index);


        /// <summary>
        /// Fügt <paramref name="property"/> am <paramref name="index"/> der registrierten Eigenschaften ein.
        /// </summary>
        /// <param name="index">Der nullbasierte Index, an dem <paramref name="property"/> eingefügt werden soll.</param>
        /// <param name="property">Die einzufügende <see cref="CsvProperty"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder 
        /// größer als <see cref="Count"/>.</exception>
        /// <exception cref="ArgumentException">Es ist bereits eine <see cref="CsvProperty"/> mit demselben
        /// <see cref="CsvProperty.PropertyName"/> enthalten.</exception>
        public void InsertProperty(int index, CsvProperty property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }


            _dynProps.Insert(index, property);
        }


        /// <summary>
        /// Ersetzt die <see cref="CsvProperty"/> am angegebenen Index der Auflistung der registrierten Eigenschaften
        /// durch <paramref name="property"/>.
        /// </summary>
        /// <param name="index">Der nullbasierte Index, an dem das in der Auflistung vorhandene
        /// <see cref="CsvProperty"/>-Objekt durch <paramref name="property"/> ersetzt wird.</param>
        /// <param name="property">Ein <see cref="CsvProperty"/>-Objekt.</param>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder 
        /// größer oder gleich <see cref="Count"/>.</exception>
        /// <exception cref="ArgumentException">Es ist bereits eine <see cref="CsvProperty"/> mit demselben
        /// <see cref="CsvProperty.PropertyName"/> enthalten.</exception>
        public void ReplacePropertyAt(int index, CsvProperty property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            _dynProps[index] = property;
        }


        /// <summary>
        /// Ersetzt die registrierte Eigenschaft, deren <see cref="CsvProperty.PropertyName"/> gleich <paramref name="propertyName"/>
        /// ist durch <paramref name="property"/>.
        /// </summary>
        /// <param name="propertyName">Bezeichner der registrierten Eigenschaft, die ersetzt werden soll.</param>
        /// <param name="property"><see cref="CsvProperty"/>-Objekt, mit dem ersetzt werden soll.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder <paramref name="property"/>
        /// ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Es ist keine Eigenschaft unter der Bezeichnung <paramref name="propertyName"/>
        /// registriert - oder - in der Auflistung der registrierten Eigenschaften befindet sich bereits ein 
        /// <see cref="CsvProperty"/>-Objekt dessen <see cref="CsvProperty.PropertyName"/>-Eigenschaft identisch
        /// mit der von <paramref name="property"/> ist.</exception>
        public void ReplaceProperty(string propertyName, CsvProperty property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            

            if (HasProperty(propertyName))
            {
                int index = _dynProps.IndexOf(_dynProps[propertyName]);
                _dynProps[index] = property;
            }
            else
            {
                throw new ArgumentException(Res.PropertyNotFound, nameof(propertyName));
            }
        }

        /// <summary>
        /// Untersucht, ob ein <see cref="CsvProperty"/>-Objekt im <see cref="CsvRecordWrapper"/> unter dem mit
        /// <paramref name="propertyName"/> angegebenen Namen registriert ist.
        /// </summary>
        /// <param name="propertyName">Der <see cref="CsvProperty.PropertyName"/> der zu suchenden
        /// <see cref="CsvProperty"/>.</param>
        /// <returns>True, wenn ein <see cref="CsvProperty"/>-Objekt unter dem mit <paramref name="propertyName"/>
        /// angegebenen Namen registriert ist.</returns>
        public bool HasProperty(string propertyName)
        {
            if(propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            return _dynProps.Contains(propertyName);
        }


        /// <summary>
        /// Die registrierten <see cref="CsvProperty"/>-Objekte als Sammlung, die mit einem Iterator durchlaufen werden kann.
        /// </summary>
        public IEnumerable<CsvProperty> Properties => this._dynProps;


        /// <summary>
        /// Wird automatisch aufgerufen, wenn auf den Set-Accessor einer dynamisch implementierten Eigenschaft
        /// zugegriffen wird. (Nicht zur direkten Verwendung in eigenem Code bestimmt.)
        /// </summary>
        /// <param name="binder">Informationen über die aufrufende dynamische Eigenschaft.</param>
        /// <param name="value">Das Objekt, das der dynamisch implementierten Eigenschaft zugewiesen wird.</param>
        /// <returns>True, wenn auf eine Eigenschaft zugegriffen wurde, die zuvor als <see cref="CsvProperty"/>
        /// im <see cref="CsvRecordWrapper"/>-Objekt registriert wurde.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="binder"/> ist <c>null</c>. (Das kann nur passieren,
        /// wenn die Methode direkt aus eigenem Code aufgerufen wird.)</exception>
        /// <exception cref="InvalidOperationException">Es wurde auf eine dynamische Eigenschaft des 
        /// <see cref="CsvRecordWrapper"/>-Objekts zugegriffen, bevor diesem ein <see cref="CsvRecord"/>-Objekt 
        /// zugewiesen wurde.</exception>
        /// <exception cref="InvalidCastException"><paramref name="value"/> ist nicht vom Datentyp der registrierten
        /// Property.</exception>
        /// <exception cref="Exception">Es wurde versucht, auf eine nicht registrierte Property zuzugreifen.</exception>
        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            if (binder is null)
            {
                throw new ArgumentNullException(nameof(binder));
            }

            if (this._record is null)
            {
                throw new InvalidOperationException(Res.NoCsvRecord);
            }


            if (this._dynProps.TryGetValue(binder.Name, out CsvProperty? prop))
            {
                prop.SetValue(this._record, value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Wird automatisch aufgerufen, wenn auf den Get-Accessor einer dynamisch implementierten Eigenschaft
        /// zugegriffen wird. (Nicht zur direkten Verwendung in eigenem Code bestimmt.)
        /// </summary>
        /// <param name="binder">Informationen über die aufrufende dynamische Eigenschaft.</param>
        /// <param name="result">Das Objekt, das den Rückgabewert der dynamisch implementierten Eigenschaft darstellt.</param>
        /// <returns>True, wenn auf eine Eigenschaft zugegriffen wurde, die zuvor als <see cref="CsvProperty"/>
        /// im <see cref="CsvRecordWrapper"/>-Objekt registriert wurde.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="binder"/> ist <c>null</c>. (Das kann nur passieren,
        /// wenn die Methode direkt aus eigenem Code aufgerufen wird.)</exception>
        /// <exception cref="InvalidOperationException">Es wurde auf eine dynamische Eigenschaft des 
        /// <see cref="CsvRecordWrapper"/>-Objekts zugegriffen, bevor diesem ein <see cref="CsvRecord"/>-Objekt
        /// zugewiesen wurde.</exception>
        /// <exception cref="Exception">Der Rückgabewert der indexierten <see cref="CsvProperty"/> konnte nicht erfolgreich geparst werden und 
        /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvProperty"/> war so konfiguriert, dass er in diesem Fall eine
        /// Ausnahme wirft.</exception>
        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            if (binder is null)
            {
                throw new ArgumentNullException(nameof(binder));
            }

            if (this._record is null)
            {
                throw new InvalidOperationException(Res.NoCsvRecord);
            }


            if (this._dynProps.TryGetValue(binder.Name, out CsvProperty? prop))
            {
                result = prop.GetValue(this._record);
                return true;
            }

            result = null;
            return false;
        }


        /// <summary>
        /// Setzt sämtliche Spalten des zugrundeliegenden <see cref="CsvRecord"/>-Objekts auf <c>null</c>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Die Methode wurde aufgerufen, bevor dem <see cref="CsvRecordWrapper"/>
        /// ein <see cref="CsvRecord"/>-Objekt zugewiesen wurde.</exception>
        public void Clear()
        {
            if (this._record is null)
            {
                throw new InvalidOperationException(Res.NoCsvRecord);
            }

            _record.Clear();
        }

        /// <summary>
        /// Gibt die Anzahl der im <see cref="CsvRecordWrapper"/> registrierten <see cref="CsvProperty"/>-Objekte
        /// zurück.
        /// </summary>
        public int Count => _dynProps.Count;

        /// <summary>
        /// Gibt an, ob das <see cref="CsvRecordWrapper"/>-Objekt schreibgeschützt ist. (Immer false.)
        /// </summary>
        bool ICollection<object?>.IsReadOnly => false;


        /// <summary>
        /// Ermöglicht den Zugriff auf die im <see cref="CsvRecordWrapper"/> registrierten Eigenschaften
        /// über einen nullbasierten Index. Der Index entspricht der Reihenfolge, in der die
        /// <see cref="CsvProperty"/>-Objekte im <see cref="CsvRecordWrapper"/> registriert sind.
        /// </summary>
        /// <param name="index">Nullbasierter Index der registrierten <see cref="CsvProperty"/>-Objekte.</param>
        /// <returns>Rückgabewert der bei <paramref name="index"/> registrierten <see cref="CsvProperty"/>.</returns>
        /// <exception cref = "InvalidOperationException" > Der Indexer wurde aufgerufen, bevor dem <see cref= "CsvRecordWrapper" />
        /// ein <see cref="CsvRecord"/>-Objekt zugewiesen wurde.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> ist kleiner als 0 oder größer oder gleich <see cref="Count"/>.</exception>
        /// <exception cref="InvalidCastException">Einem Index wurde ein Objekt zugewiesen, dessen Datentyp nicht dem Datentyp der an
        /// diesem Index registrierten Property entspricht.</exception>
        /// <exception cref="Exception">Der Rückgabewert der indexierten <see cref="CsvProperty"/> konnte nicht erfolgreich geparst werden und 
        /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvProperty"/> war so konfiguriert, dass er in diesem Fall eine
        /// Ausnahme wirft.</exception>
        public object? this[int index]
        {
            get
            {
                if (this._record is null)
                {
                    throw new InvalidOperationException(Res.NoCsvRecord);
                }

                return _dynProps[index].GetValue(this._record);
            }
            set
            {
                if (this._record is null)
                {
                    throw new InvalidOperationException(Res.NoCsvRecord);
                }

                 _dynProps[index].SetValue(this._record, value);
            }
        }

        /// <summary>
        /// Ermöglicht den Zugriff auf die im <see cref="CsvRecordWrapper"/> registrierten Eigenschaften
        /// über den Wert der Eigenschaft <see cref="CsvProperty.PropertyName"/>.
        /// </summary>
        /// <param name="propertyName">Name der registrierten Eigenschaft. (Entspricht <see cref="CsvProperty.PropertyName"/>. Der
        /// Vergleich erfolgt case-sensitiv.</param>
        /// <returns>Rückgabewert der registrierten <see cref="CsvProperty"/>, deren Eigenschaft <see cref="CsvProperty.PropertyName"/>&#160;<paramref name="propertyName"/>
        /// entspricht. Der Vergleich ist case-sensitiv.</returns>
        /// <exception cref = "InvalidOperationException" > Der Indexer wurde aufgerufen, bevor dem <see cref= "CsvRecordWrapper"/> ein <see cref="CsvRecord"/>-Objekt
        /// zugewiesen wurde.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Es wurde unter den bei <see cref="CsvRecordWrapper"/> registrierten <see cref="CsvProperty"/>-Eigenschaften kein
        /// <see cref="CsvProperty"/>-Objekt gefunden, dessen Eigenschaft <see cref="CsvProperty.PropertyName"/>&#160;<paramref name="propertyName"/> entspricht.</exception>
        /// <exception cref="InvalidCastException">Einem Index wurde ein Objekt zugewiesen, dessen Datentyp nicht dem Datentyp der an
        /// diesem Index registrierten Property entspricht.</exception>
        /// <exception cref="Exception">Der Rückgabewert der indexierten <see cref="CsvProperty"/> konnte nicht erfolgreich geparst werden und 
        /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvProperty"/> war so konfiguriert, dass er in diesem Fall eine
        /// Ausnahme wirft.</exception>
        public object? this[string propertyName]
        {
            get
            {
                if (this._record is null)
                {
                    throw new InvalidOperationException(Res.NoCsvRecord);
                }

                if (propertyName is null)
                {
                    throw new ArgumentNullException(nameof(propertyName));
                }

                if (this._dynProps.TryGetValue(propertyName, out CsvProperty? prop))
                {
                    return prop.GetValue(this._record);
                }
                else
                {
                    throw new ArgumentException(Res.PropertyNotFound, nameof(propertyName));
                }
            }

            set
            {
                if (this._record is null)
                {
                    throw new InvalidOperationException(Res.NoCsvRecord);
                }

                if (propertyName is null)
                {
                    throw new ArgumentNullException(nameof(propertyName));
                }


                if (this._dynProps.TryGetValue(propertyName, out CsvProperty? prop))
                {
                    prop.SetValue(this._record, value);
                }
                else
                {
                    throw new ArgumentException(Res.PropertyNotFound, nameof(propertyName));
                }
            }
        }


        /// <summary>
        /// Gibt einen <see cref="IEnumerator{T}">IEnumerator&lt;object?&gt;</see> zurück, mit dem die Rückgabewerte der 
        /// dynamisch implementierten Eigenschaften durchlaufen werden können. Die Reihenfolge
        /// entspricht der Reihenfolge, in der die
        /// <see cref="CsvProperty"/>-Objekte im <see cref="CsvRecordWrapper"/> registriert sind.
        /// </summary>
        /// <returns>Ein <see cref="IEnumerator{T}">IEnumerator&lt;object?&gt;</see>.</returns>
        /// <exception cref="InvalidOperationException">Die Methode wurde aufgerufen, bevor dem <see cref="CsvRecordWrapper"/>
        /// ein <see cref="CsvRecord"/>-Objekt zugewiesen wurde.</exception>
        /// <exception cref="Exception">Der Rückgabewert der indexierten <see cref="CsvProperty"/> konnte nicht erfolgreich geparst werden und 
        /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvProperty"/> war so konfiguriert, dass er in diesem Fall eine
        /// Ausnahme wirft.</exception>
        public IEnumerator<object?> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }


        /// <summary>
        /// Gibt einen <see cref="IEnumerator"/> zurück, mit dem die Rückgabewerte der 
        /// dynamisch implementierten Eigenschaften durchlaufen werden können. Die Reihenfolge
        /// entspricht der Reihenfolge, in der die
        /// <see cref="CsvProperty"/>-Objekte im <see cref="CsvRecordWrapper"/> registriert sind.
        /// </summary>
        /// <returns>Ein <see cref="IEnumerator"/>.</returns>
        /// <exception cref="InvalidOperationException">Die Methode wurde aufgerufen, bevor dem <see cref="CsvRecordWrapper"/>
        /// ein <see cref="CsvRecord"/>-Objekt zugewiesen wurde.</exception>
        /// <exception cref="Exception">Der Rückgabewert der indexierten <see cref="CsvProperty"/> konnte nicht erfolgreich geparst werden und 
        /// der <see cref="ICsvTypeConverter"/> dieser <see cref="CsvProperty"/> war so konfiguriert, dass er in diesem Fall eine
        /// Ausnahme wirft.</exception>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <summary>
        /// Gibt den nullbasierten Index des ersten Vorkommens von <paramref name="item"/> unter den Rückgabewerten
        /// der im <see cref="CsvRecordWrapper"/>-Wrapper registrierten dynamischen Eigenschaften zurück oder
        /// -1, wenn <paramref name="item"/> nicht gefunden wurde.
        /// </summary>
        /// <param name="item">Das Objekt, dessen Index ermittelt wird.</param>
        /// <returns>Der nullbasierte Index, an dem <paramref name="item"/> unter den Rückgabewerten
        /// der im <see cref="CsvRecordWrapper"/>-Wrapper registrierten dynamischen Eigenschaften gefunden wurde
        /// oder -1, wenn <paramref name="item"/> nicht gefunden wurde.</returns>
        /// <exception cref="InvalidOperationException">Die Methode wurde aufgerufen, bevor dem <see cref="CsvRecordWrapper"/>
        /// ein <see cref="CsvRecord"/>-Objekt zugewiesen wurde.</exception>
        public int IndexOf(object? item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] == item)
                {
                    return i;
                }
            }

            return -1;
        }


        /// <summary>
        /// Untersucht, ob <paramref name="item"/> unter den Rückgabewerten
        /// der im <see cref="CsvRecordWrapper"/>-Wrapper registrierten dynamischen Eigenschaften existiert.
        /// </summary>
        /// <param name="item">Das Objekt, dessen Vorhandensein überprüft wird.</param>
        /// <returns>True, wenn <paramref name="item"/> unter den Rückgabewerten
        /// der im <see cref="CsvRecordWrapper"/>-Wrapper registrierten dynamischen Eigenschaften gefunden wurde.</returns>
        /// <exception cref="InvalidOperationException">Die Methode wurde aufgerufen, bevor dem <see cref="CsvRecordWrapper"/>
        /// ein <see cref="CsvRecord"/>-Objekt zugewiesen wurde.</exception>
        public bool Contains(object? item) => IndexOf(item) != -1;


        /// <summary>
        /// Kopiert die Rückgabewerte
        /// der im <see cref="CsvRecordWrapper"/>-Wrapper registrierten dynamischen Eigenschaften in ein <see cref="object"/>-Array,
        /// beginnend bei <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array">Ein <see cref="object"/>-Array, in das hineinkopiert wird.</param>
        /// <param name="arrayIndex">Der nullbasierte Index in <paramref name="array"/>, an dem der 
        /// Kopiervorgang startet.</param>
        /// <exception cref="InvalidOperationException">Die Methode wurde aufgerufen, bevor dem <see cref="CsvRecordWrapper"/>
        /// ein <see cref="CsvRecord"/>-Objekt zugewiesen wurde.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> ist kleiner als 0 oder die Anzahl der zu 
        /// kopierenden Elemente ist größer als die verfügbare Anzahl der Elemente von <paramref name="arrayIndex"/> bis zum Ende des Zielarrays.</exception>
        public void CopyTo(object?[] array, int arrayIndex)
        {
            if(array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            for (int i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = this[i];
            }
        }


        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="index">-</param>
        /// <param name="item">-</param>
        /// <exception cref="NotSupportedException">Not Supported.</exception>
        void IList<object?>.Insert(int index, object? item) => throw new NotSupportedException();


        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="index">-</param>
        /// <exception cref="NotSupportedException">Not Supported.</exception>
        void IList<object?>.RemoveAt(int index) => throw new NotSupportedException();


        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="item">-</param>
        /// <exception cref="NotSupportedException">Not Supported.</exception>
        void ICollection<object?>.Add(object? item) => throw new NotSupportedException();


        /// <summary>
        /// Not Supported.
        /// </summary>
        /// <param name="item">-</param>
        /// <returns>-</returns>
        /// <exception cref="NotSupportedException">Not Supported.</exception>
        bool ICollection<object?>.Remove(object? item) => throw new NotSupportedException();






        /// ////////////////////////////////////////////////////////////////////////

        private class PropertyCollection : KeyedCollection<string, CsvProperty>
        {
            protected override string GetKeyForItem(CsvProperty item) => item.PropertyName;

            public PropertyCollection() : base(StringComparer.Ordinal) { }
        }
    }
}
