using FolkerKinzel.CsvTools.Helpers.Converters;
using FolkerKinzel.CsvTools.Intls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FolkerKinzel.CsvTools.Helpers
{
    /// <summary>
    /// Spezialisierung der <see cref="CsvProperty"/>-Klasse zur Bearbeitung von CSV-Dateien ohne Kopfzeile.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    [Obsolete("This class will be removed.", true)]
    public sealed class CsvIndexProperty : CsvProperty
    {
        [Obsolete("This constructor will be removed.", true)]
        private CsvIndexProperty(CsvIndexProperty source) : base(source)
        {

        }


        /// <summary>
        /// Initialisiert ein neues <see cref="CsvIndexProperty"/>-Objekt.
        /// </summary>
        /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für C#-Bezeichner
        /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
        /// <param name="csvColumnIndex">Nullbasierter Index der Spalte der CSV-Datei.</param>
        /// <param name="converter">Der <see cref="ICsvTypeConverter"/>, der die Typkonvertierung übernimmt.</param>
        /// 
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
        /// ASCII-Zeichen).</exception>
        /// 
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder 
        /// <paramref name="converter"/> ist <c>null</c>.</exception>
        /// 
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="csvColumnIndex"/> ist kleiner als 0.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("This ctor will be removed.", true)]
        public CsvIndexProperty(
            string propertyName, int csvColumnIndex, ICsvTypeConverter converter) :
            base(propertyName, csvColumnIndex, converter)
        {

        }


        /// <summary>
        /// Initialisiert ein neues <see cref="CsvIndexProperty"/>-Objekt.
        /// </summary>
        /// <param name="propertyName">Der Bezeichner unter dem die Eigenschaft angesprochen wird. Er muss den Regeln für C#-Bezeichner
        /// entsprechen. Es werden nur ASCII-Zeichen akzeptiert.</param>
        /// <param name="csvColumnIndex">Nullbasierter Index der Spalte der CSV-Datei.</param>
        /// <param name="converter">Der <see cref="ICsvTypeConverter"/>, der die Typkonvertierung übernimmt.</param>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> entspricht nicht den Regeln für C#-Bezeichner (nur
        /// ASCII-Zeichen).</exception>
        /// <param name="wildcardTimeout">Der Parameter hat keine Funktion.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder 
        /// <paramref name="converter"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="csvColumnIndex"/> ist kleiner als 0.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("Remove the argument for wildcardTimeout!", true)]
        public CsvIndexProperty(
            string propertyName, int csvColumnIndex, ICsvTypeConverter converter, int wildcardTimeout) :
            this(propertyName, csvColumnIndex, converter)
        {
        }



        /// <inheritdoc cref="ICloneable.Clone" />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        [Obsolete("This method will be removed.", true)]
        public override object Clone() => new CsvIndexProperty(this);

    }
}
