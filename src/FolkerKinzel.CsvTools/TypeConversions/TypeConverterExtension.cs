using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

namespace FolkerKinzel.CsvTools.TypeConversions
{
    public static class TypeConverterExtension
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ICsvTypeConverter2 AddDBNullAcceptance(this ICsvTypeConverter2 converter) => new DBNullConverter(converter);


#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static CsvTypeConverter<Nullable<T>> MakeNullable<T>(this CsvTypeConverter<T> converter) where T : struct => new NullableStructConverter<T>(converter);



        internal static ICsvTypeConverter2 HandleNullableAndDBNullAcceptance<T>(this CsvTypeConverter<T> converter, bool nullable, bool acceptsDBNull) where T : struct
        {
            if (nullable)
            {
                CsvTypeConverter<T?> nullableConv = converter.MakeNullable();

                return acceptsDBNull ? nullableConv.AddDBNullAcceptance() : nullableConv;
            }

            return acceptsDBNull ? converter.AddDBNullAcceptance() : converter;
        }
        
    }
}
