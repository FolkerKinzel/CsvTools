# if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.CsvTools.TypeConversions;

public class CsvMultiColumnProperty<T> : CsvPropertyBase
{
    protected CsvMultiColumnProperty(string propertyName, CsvMultiColumnTypeConverter<T> converter) : base(propertyName)
    {
        if (converter is null)
        {
            throw new ArgumentNullException(nameof(converter));
        }

        this.Converter = converter;
    }

    public CsvMultiColumnTypeConverter<T> Converter { get; }

    internal override object? GetValue(CsvRecord record)
    {
        Converter.Wrapper.Record = record;
        return Converter.Create();
    }

    internal override void SetValue(CsvRecord record, object? value)
    {
        Converter.Wrapper.Record = record;
        Converter.ToCsv((T?)value);
    }
}
