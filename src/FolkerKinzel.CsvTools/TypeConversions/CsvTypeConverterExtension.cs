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
        public static CsvTypeConverter<Nullable<T>> AsNullableConverter<T>(this CsvTypeConverter<T> converter) 
            where T : struct => new NullableStructConverter<T>(converter);


#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static CsvTypeConverter<IEnumerable<TItem?>?> AsIEnumerableConverter<TItem>(this CsvTypeConverter<TItem?> itemsConverter,
                                                                                     bool nullable = true,
                                                                                     char fieldSeparator = ',')
            => new IEnumerableConverter<TItem>(itemsConverter, nullable, fieldSeparator);



        internal static ICsvTypeConverter HandleNullableAndDBNullAcceptance<T>(this CsvTypeConverter<T> converter, bool nullable, bool dbNullEnabled) where T : struct
        {
            if (nullable)
            {
                CsvTypeConverter<T?> nullableConv = converter.AsNullableConverter();

                return dbNullEnabled ? nullableConv.AsDBNullEnabled() : nullableConv;
            }

            return dbNullEnabled ? converter.AsDBNullEnabled() : converter;
        }
        
    }
}
