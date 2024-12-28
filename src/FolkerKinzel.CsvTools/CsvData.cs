using System.Collections;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools;

//public class CsvDataRow : IEnumerable<ReadOnlyMemory<char>>
//{
//    private IEnumerable<ReadOnlyMemory<char>> _data;
//    public IEnumerator<ReadOnlyMemory<char>> GetEnumerator() => _data.GetEnumerator();
//    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

//    private CsvDataRow(IEnumerable<ReadOnlyMemory<char>> data) => this._data = data;

//    public static CsvDataRow Create(IEnumerable<ReadOnlyMemory<char>> row)
//    {
//        _ArgumentNullException.ThrowIfNull(row, nameof(row));
//        return new(row);
//    }

//    public static CsvDataRow Create(IEnumerable<string?> row)
//    {
//        _ArgumentNullException.ThrowIfNull(row, nameof(row));
//        return new(row.Select(x => x.AsMemory()));
//    }

//    [SuppressMessage("Style", "IDE0028:Simplify collection initialization",
//        Justification = "Performance: Should be a list")]
//    public static CsvDataRow Create() => new(new List<ReadOnlyMemory<char>>());

//    public CsvDataRow AddField(string? field)
//        => AddField(field.AsMemory());

//    public CsvDataRow AddField(ReadOnlyMemory<char> field)
//    {
//        if (_data is List<ReadOnlyMemory<char>> list) // Avoids copying the data
//        {
//            list.Add(field);
//        }
//        else
//        {
//            _data = new List<ReadOnlyMemory<char>>(_data)
//            {
//                field
//            };
//        }

//        return this;
//    }

//    public static implicit operator CsvDataRow?(CsvRecord? record)
//    {
//        if (record is null)
//        {
//            return null;
//        }
//        return Create(record.Values);
//    }
//}

public class CsvData : IEnumerable<IEnumerable<ReadOnlyMemory<char>>>
{
    private IEnumerable<IEnumerable<ReadOnlyMemory<char>>> _data;

    public IEnumerator<IEnumerable<ReadOnlyMemory<char>>> GetEnumerator() => _data.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private CsvData(IEnumerable<IEnumerable<ReadOnlyMemory<char>>> data) => this._data = data;

    public static CsvData Create(IEnumerable<IEnumerable<ReadOnlyMemory<char>>?> data)
    {
        _ArgumentNullException.ThrowIfNull(data, nameof(data));
        return new(data.OfType<IEnumerable<ReadOnlyMemory<char>>>());
    }

    public static CsvData Create(IEnumerable<IEnumerable<string?>?> data)
    {
        _ArgumentNullException.ThrowIfNull(data, nameof(data));
        return new(data.OfType<IEnumerable<string?>>().Select(x => x.Select(y => y.AsMemory())));
    }

    [SuppressMessage("Style", "IDE0028:Simplify collection initialization",
        Justification = "Performance: Should be a list")]
    public static CsvData Create() => new(new List<IEnumerable<ReadOnlyMemory<char>>>());

    public CsvData AddRow(IEnumerable<ReadOnlyMemory<char>> row)
    {
        _ArgumentNullException.ThrowIfNull(row, nameof(row));

        if (_data is List<IEnumerable<ReadOnlyMemory<char>>> list) // Avoids copying the data
        {
            list.Add(row);
        }
        else
        {
            _data = new List<IEnumerable<ReadOnlyMemory<char>>>(_data)
            {
                row
            };
        }

        return this;
    }
}
