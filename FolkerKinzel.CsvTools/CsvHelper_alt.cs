using System;
using System.Collections.Generic;
using System.Data;
using FolkerKinzel.CsvTools.Properties;

namespace FolkerKinzel.CsvTools
{
    /// <summary>
    /// Die Klasse enthält statische Methoden, die beim Lesen und Schreiben von csv-Dateien hilfreich
    /// sind.
    /// </summary>
    public static class CsvHelper
    {
        /// <summary>
        /// Erzeugt eine DataTable, deren Spalten die Namen tragen, die der Methode in einer <see cref="IEnumerable{String}">IEnumerable&lt;String&gt;</see>/>-
        /// Collection übergeben werden. Die Spalten sind vom Datentyp <see cref="string"/> und dürfen <see cref="DBNull.Value"/> enthalten.
        /// </summary>
        /// <remarks>Die Methode ist threadsicher.</remarks>
        /// <param name="keys">Die Spaltennamen. Spaltennamen dürfen nicht doppelt vorkommen (Groß- und Kleinschreibung
        /// wird nicht beachtet). Wenn Spaltennamen null oder leer sind, wird automatisch ein eindeutiger Name vergeben.</param>
        /// <returns>Die initialisierte DataTable.</returns>
        /// <exception cref="DuplicateNameException"><paramref name="keys"/> enthält doppelte Spaltennamen.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="keys"/> ist null.</exception>
        public static DataTable CreateDataTable(IEnumerable<string?> keys)
        {
            if (keys is null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            DataTable datbl = new DataTable();

            foreach (string? columnName in keys)
            {
                datbl.Columns.Add(columnName);
            }

            return datbl;
        }

    }//class

}
