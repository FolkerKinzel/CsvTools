using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.CsvTools.TypeConversions.Converters;
using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

namespace FolkerKinzel.CsvTools.TypeConversions
{
    public static class CsvTypeConverterExtension
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static CsvTypeConverter<object> AsDBNullEnabled<T>(this CsvTypeConverter<T> converter)
        {
            if (converter is CsvTypeConverter<object> result && Convert.IsDBNull(result.FallbackValue))
            {
                return result;
            }

            return new DBNullConverter<T>(converter);
        }


#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static CsvTypeConverter<Nullable<T>> AsNullable<T>(this CsvTypeConverter<T> converter) 
            where T : struct => new NullableStructConverter<T>(converter);


#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static CsvTypeConverter<IEnumerable<TItem?>?> MakeIEnumerableConverter<TItem>(this CsvTypeConverter<TItem?> itemsConverter,
                                                                                     char fieldSeparator = ',',
                                                                                     IEnumerable<TItem?>? fallbackValue = null)
            => new IEnumerableConverter<TItem>(itemsConverter, fieldSeparator, fallbackValue);



        internal static ICsvTypeConverter2 HandleNullableAndDBNullAcceptance<T>(this CsvTypeConverter<T> converter, CsvConverterOptions options) where T : struct
        {
            if (options.HasFlag(CsvConverterOptions.Nullable))
            {
                CsvTypeConverter<T?> nullableConv = converter.AsNullable();

                return options.HasFlag(CsvConverterOptions.AcceptsDBNull) ? nullableConv.AsDBNullEnabled() : nullableConv;
            }

            return options.HasFlag(CsvConverterOptions.AcceptsDBNull) ? converter.AsDBNullEnabled() : converter;
        }
        
    }
}
