using System;
using System.Resources;

namespace FolkerKinzel.CsvTools.Resources
{
    /// <summary>
    ///   Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1304:CultureInfo angeben", Justification = "<Ausstehend>")]
    internal static class Res
    {
        /// <summary>
        ///   Gibt bei jedem Aufruf eine neue <see cref="ResourceManager"/>-Instanz zurück. Das ist sinnvoll, weil die Instanz
        ///   nur für Ausnahmen verwendet wird.
        /// </summary>
        private static ResourceManager ResourceManager => new ResourceManager("FolkerKinzel.CsvTools.Resources.Res", typeof(Res).Assembly);

        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die "The identifier is not well-formed." ähnelt.
        /// </summary>
        internal static string BadIdentifier => ResourceManager.GetString("BadIdentifier");

        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die "The converters data type is not supported." ähnelt.
        /// </summary>
        internal static string DataTypeNotSupported => ResourceManager.GetString("DataTypeNotSupported");

        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die "Unable to cast null to value type." ähnelt.
        /// </summary>
        internal static string InvalidCastNullToValueType => ResourceManager.GetString("InvalidCastNullToValueType");

        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die "No CsvRecord is assigned. Call &apos;CsvRecordWrapper.SetRecord(CsvRecord)&apos; first!" ähnelt.
        /// </summary>
        internal static string NoCsvRecord => ResourceManager.GetString("NoCsvRecord");

        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die "The flag &apos;DateTimeStyles.NoCurrentDateDefault&apos; is not supported." ähnelt.
        /// </summary>
        internal static string NoNoCurrentDateDafault => ResourceManager.GetString("NoNoCurrentDateDafault");

        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die "You must not run this method twice." ähnelt.
        /// </summary>
        internal static string NotTwice => ResourceManager.GetString("NotTwice");


        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die "There is no property with this name!" ähnelt.
        /// </summary>
        internal static string PropertyNotFound => ResourceManager.GetString("PropertyNotFound");
    }
}
