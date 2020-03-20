using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FolkerKinzel.Csv
{
    /// <summary>
    /// Konstanten zum Einstellen des Lese- und Schreibmodus
    /// </summary>
    public enum CsvMode
    {
        /// <summary>
        /// Modus zum Lesen und Schreiben standardgemäßer Csv-Dateien.
        /// </summary>
        Standard,

        /// <summary>
        /// Modus zum Lesen und Schreiben nichtstandardgemäßer Csv-Dateien, 
        /// bei denen Gänsefüßchen in maskierten Abschnitten nicht durch doppelte
        /// Gänsefüßchen ersetzt worden sind bzw. bei denen beim Einlesen doppelte Gänsefüßchen
        /// nicht zurückgewandelt werden.
        /// </summary>
        NonStandard
    }
}
