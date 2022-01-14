using System.Text;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

internal sealed class IEnumerableConverter<TItem> : CsvTypeConverter<IEnumerable<TItem?>?>
{
    private readonly char _separatorChar;
    private readonly CsvTypeConverter<TItem?> _itemsConverter;


    internal IEnumerableConverter(CsvTypeConverter<TItem?> itemsConverter, bool nullable, char fieldSeparator)
        : base(false, nullable ? null : Array.Empty<TItem>())
    {
        _itemsConverter = itemsConverter ?? throw new ArgumentNullException(nameof(itemsConverter));
        _separatorChar = fieldSeparator;
    }

    protected override string? DoConvertToString(IEnumerable<TItem?>? value)
    {
        if (value is null || !value.Any())
        {
            return null;
        }
        var sb = new StringBuilder();
        using var writer = new StringWriter(sb);
        using (var csvWriter = new CsvWriter(writer, value.Count(), fieldSeparator: _separatorChar))
        {
            csvWriter.Record.Fill(value.Select(x => _itemsConverter.ConvertToString(x)));
            csvWriter.WriteRecord();
        }
        return writer.ToString();
    }

    public override bool TryParseValue(string value, out IEnumerable<TItem?>? result)
    {
        var list = new List<TItem?>();

        using var reader = new StringReader(value);
        using var csvReader = new CsvReader(reader, false, fieldSeparator: _separatorChar);

        CsvRecord? record = csvReader.FirstOrDefault();

        if (record is null || record.Count == 0)
        {
            result = list!;
            return true;
        }

        for (int i = 0; i < record.Count; i++)
        {
            list.Add(_itemsConverter.Parse(record[i]));
        }

        result = list;
        return true;
    }
}
