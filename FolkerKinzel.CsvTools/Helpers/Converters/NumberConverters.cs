using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FolkerKinzel.CsvTools.Helpers.Converters
{
    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="decimal"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DecimalConverter : NumberConverter<decimal>
    {
        /// <summary>
        /// Initialisiert ein <see cref="DecimalConverter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;Decimal&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="decimal"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public DecimalConverter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }


    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="double"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DoubleConverter : NumberConverter<double>
    {
        /// <summary>
        /// Initialisiert ein <see cref="DoubleConverter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;Double&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="double"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public DoubleConverter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }

    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="float"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class SingleConverter : NumberConverter<float>
    {
        /// <summary>
        /// Initialisiert ein <see cref="SingleConverter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;Single&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="float"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public SingleConverter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }


    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="ulong"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class UInt64Converter : NumberConverter<ulong> 
    {
        /// <summary>
        /// Initialisiert ein <see cref="UInt64Converter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;UInt64&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="ulong"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public UInt64Converter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }

    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="long"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class Int64Converter : NumberConverter<long>
    {
        /// <summary>
        /// Initialisiert ein <see cref="Int64Converter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;Int64&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="long"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public Int64Converter(
           bool nullable = false,
           IFormatProvider? provider = null,
           bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }


    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="uint"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class UInt32Converter : NumberConverter<uint> 
    {
        /// <summary>
        /// Initialisiert ein <see cref="UInt32Converter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;UInt32&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="uint"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public UInt32Converter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }

    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="int"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class Int32Converter : NumberConverter<int>
    {
        /// <summary>
        /// Initialisiert ein <see cref="Int32Converter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;Int32&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="int"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public Int32Converter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }


    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="ushort"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class UInt16Converter : NumberConverter<ushort>
    {
        /// <summary>
        /// Initialisiert ein <see cref="UInt16Converter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;UInt16&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="ushort"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public UInt16Converter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }

    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="short"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class Int16Converter : NumberConverter<short>
    {
        /// <summary>
        /// Initialisiert ein <see cref="Int16Converter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;Int16&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="short"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public Int16Converter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }

    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="byte"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class ByteConverter : NumberConverter<byte>
    {
        /// <summary>
        /// Initialisiert ein <see cref="ByteConverter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;Byte&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="byte"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public ByteConverter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }

    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="sbyte"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class SByteConverter : NumberConverter<sbyte>
    {
        /// <summary>
        /// Initialisiert ein <see cref="SByteConverter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;SByte&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="sbyte"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public SByteConverter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }

    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="char"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class CharConverter : NumberConverter<char> 
    {
        /// <summary>
        /// Initialisiert ein <see cref="CharConverter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;Char&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="char"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public CharConverter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }


    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="DateTime"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class DateTimeConverter : NumberConverter<DateTime>
    {
        /// <summary>
        /// Initialisiert ein <see cref="DateTimeConverter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;DateTime&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="DateTime"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public DateTimeConverter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }


    /// <summary>
    /// Implementiert das Interface <see cref="ICsvTypeConverter"/> für die Umwandlung
    /// des <see cref="bool"/>-Datentyps.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class BooleanConverter : NumberConverter<bool>
    {
        /// <summary>
        /// Initialisiert ein <see cref="BooleanConverter"/>-Objekt.
        /// </summary>
        /// <param name="nullable">Wenn <c>true</c>, wird <see cref="Nullable{T}">Nullable&lt;bool&gt;</see> akzeptiert und zurückgegeben,
        /// sonst <see cref="bool"/>.</param>
        /// <param name="provider">Ein <see cref="IFormatProvider"/>-Objekt, das kulturspezifische Formatierungsinformationen
        /// bereitstellt oder <c>null</c> für <see cref="CultureInfo.InvariantCulture"/>.</param>
        /// <param name="throwOnParseErrors">Wenn true, wirft die Methode <see cref="NumberConverter{T}.Parse"/> eine Ausnahme, wenn das Parsen 
        /// misslingt, anderenfalls gibt sie in diesem Fall <see cref="NumberConverter{T}.FallbackValue"/> zurück.</param>
        public BooleanConverter(
            bool nullable = false,
            IFormatProvider? provider = null,
            bool throwOnParseErrors = false) : base(nullable, provider, throwOnParseErrors) { }
    }

}
