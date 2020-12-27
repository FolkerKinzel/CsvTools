using FolkerKinzel.CsvTools.Helpers.Converters;
using System;
using System.Collections.Generic;
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

#pragma warning disable 1573
        /// <inheritdoc cref="CsvProperty.CsvProperty(string, IEnumerable{string}, ICsvTypeConverter, int)" 
        /// path="param[@name='propertyName']|param[@name='converter']|param[@name='wildcardTimeout']|exception"/>
        /// <summary>
        /// Initialisiert ein <see cref="CsvIndexProperty"/>-Objekt.
        /// </summary>
        /// <param name="csvColumnIndex">Nullbasierter Index der Spalte der CSV-Datei.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> oder 
        /// <paramref name="converter"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="csvColumnIndex"/> ist kleiner als 0 - oder - 
        /// <paramref name="wildcardTimeout"/> ist kleiner als 1 oder größer als
        /// <see cref="CsvProperty.MaxWildcardTimeout"/>.</exception>
        public CsvIndexProperty(
            string propertyName, int csvColumnIndex, ICsvTypeConverter converter, int wildcardTimeout = 10) : 
            base(propertyName, new string[] { AutoColumnName.Create(csvColumnIndex) }, converter, wildcardTimeout)
        {
            if(csvColumnIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(csvColumnIndex));
            }
        }

#pragma warning restore 1573

        
        /// <inheritdoc path="summary|returns"/>
        public override object Clone() => new CsvIndexProperty(this);
        
    }
}
