using System;
using System.Collections.Generic;
using System.Text;

namespace FolkerKinzel.CsvTools.Helpers.Converters
{
    /// <summary>
    /// Benannte Konstanten, um den Datentyp eines <see cref="ICsvTypeConverter"/>-Objekts auszuwählen.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Bezeichner enthält Typnamen", Justification = "<Ausstehend>")]
    public enum CsvTypeCode
    {
        /// <summary>
        /// <see cref="bool"/>
        /// </summary>
        Boolean,

        /// <summary>
        /// <see cref="byte"/>
        /// </summary>
        Byte,

        /// <summary>
        /// <see cref="sbyte"/>
        /// </summary>
        SByte,

        /// <summary>
        /// <see cref="char"/>
        /// </summary>
        Char,

        /// <summary>
        /// <see cref="ushort"/>
        /// </summary>
        UInt16,

        /// <summary>
        /// <see cref="short"/>
        /// </summary>
        Int16,

        /// <summary>
        /// <see cref="uint"/>
        /// </summary>
        UInt32,

        /// <summary>
        /// <see cref="int"/>
        /// </summary>
        Int32,

        /// <summary>
        /// <see cref="ulong"/>
        /// </summary>
        UInt64,

        /// <summary>
        /// <see cref="long"/>
        /// </summary>
        Int64,

        /// <summary>
        /// <see cref="Guid"/>
        /// </summary>
        Guid,

        /// <summary>
        /// <see cref="float"/>
        /// </summary>
        Single,


        /// <summary>
        /// <see cref="double"/>
        /// </summary>
        Double,


        /// <summary>
        /// <see cref="decimal"/>
        /// </summary>
        Decimal,

        /// <summary>
        /// <see cref="TimeSpan"/>
        /// </summary>
        TimeSpan,

        /// <summary>
        /// <see cref="DateTime"/>
        /// </summary>
        DateTime,


        /// <summary>
        /// <see cref="DateTimeOffset"/>
        /// </summary>
        DateTimeOffset,


        /// <summary>
        /// <see cref="string"/>
        /// </summary>
        String,

       
        /// <summary>
        /// <see cref="byte"/>-Array (Base64-codiert)
        /// </summary>
        ByteArray
        
    }
}
