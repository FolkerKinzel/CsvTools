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
    public sealed class CsvIndexProperty : CsvProperty, ICloneable
    {
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
        public CsvIndexProperty(
            string propertyName, int csvColumnIndex, ICsvTypeConverter converter) : 
            base(propertyName, new string[] { AutoColumnName.Create(csvColumnIndex) }, converter)
        {
            if(csvColumnIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(csvColumnIndex));
            }
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
        [Obsolete("Use CsvIndexProperty(string propertyName, int csvColumnIndex, ICsvTypeConverter converter) instead!")]
        public CsvIndexProperty(
            string propertyName, int csvColumnIndex, ICsvTypeConverter converter, int wildcardTimeout) :
            this(propertyName, csvColumnIndex, converter)
        {
        }



        /// <inheritdoc cref="ICloneable.Clone" />
        public override object Clone() => new CsvIndexProperty(this);
        
    }
}
