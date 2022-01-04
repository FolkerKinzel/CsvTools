using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters
{
    public sealed class IEnumerableTConverter : IEnumerableTConverterBase
    {

        public IEnumerableTConverter(ICsvTypeConverter itemsConverter,
                                     char fieldSeparator = ',',
                                     bool nullable = true,
                                     bool maybeDBNull = false,
                                     bool throwOnParseErrors = false)
            : base(itemsConverter, nullable, maybeDBNull, throwOnParseErrors)
        {
            this.FieldSeparator = fieldSeparator;
        }

        public char FieldSeparator { get; }


        public override object? Parse(string? value)
        {
            if(value is null)
            {
                return FallbackValue;
            }

            using var reader = new StringReader(value);
            using var csv = new CsvReader(reader, false, CsvOptions.None, FieldSeparator);

            CsvRecord record = csv.Read().First();

            //if(record is null)
            //{
            //    return FallbackValue;
            //}

            var arr = Array.CreateInstance(ItemsConverter.Type, record.Count);

            for (int i = 0; i < record.Count; i++)
            {
                arr.SetValue(ItemsConverter.Parse(record[i]), i);
            }

            //var result = Convert.ChangeType(ParseItems(record), Type);

            return arr;
        }


        public override string? ConvertToString(object? value)
        {
            if (value is null)
            {
                return null;
            }

            if (value is DBNull)
            {
                if(FallbackValue == DBNull.Value)
                {
                    return null;
                }
                else
                {
                    throw new InvalidCastException();
                }
            }

            Type valueType = value.GetType();
            if (!this.Type.IsAssignableFrom(valueType))
            {
                throw new InvalidCastException();
            }

            var values = new List<object>();
            var numerable = (IEnumerable)value;

            foreach (var item in numerable)
            {
                values.Add(item);
            }

            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, values.Count, CsvOptions.None, FieldSeparator);

            for (int i = 0; i < values.Count; i++)
            {
                csv.Record[i] = ItemsConverter.ConvertToString(values[i]);
            }
            csv.WriteRecord();

            return writer.ToString();
        }


    }

    public abstract class IEnumerableTConverterBase : ICsvTypeConverter
    {
        public IEnumerableTConverterBase(ICsvTypeConverter tConverter, bool nullable, bool maybeDBNull, bool throwOnParseErrors)
        {
            if (tConverter is null)
            {
                throw new ArgumentNullException(nameof(tConverter));
            }

            this.ItemsConverter = tConverter;

            ThrowsOnParseErrors = throwOnParseErrors;
            Type = typeof(IEnumerable<>).MakeGenericType(tConverter.Type);
            FallbackValue = maybeDBNull ? DBNull.Value : nullable ? null : Array.CreateInstance(tConverter.Type, 0);
        }

        public object? FallbackValue { get; }

        public Type Type { get; }
        public bool ThrowsOnParseErrors { get; }

        protected ICsvTypeConverter ItemsConverter { get; }


        public abstract string? ConvertToString(object? value); 
        public abstract object? Parse(string? value);

        
    }
}
