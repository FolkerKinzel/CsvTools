using System.Collections;
using System.Runtime.CompilerServices;

namespace FolkerKinzel.CsvTools.Intls;

internal readonly struct CsvRecordCollection : IEnumerable<CsvRecord>
{
    private readonly CsvReader _reader;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal CsvRecordCollection(CsvReader reader) => this._reader = reader;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<CsvRecord> GetEnumerator() => _reader.GetEnumerator();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
