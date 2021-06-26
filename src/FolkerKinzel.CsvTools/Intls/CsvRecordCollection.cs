using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FolkerKinzel.CsvTools.Intls
{
    internal readonly struct CsvRecordCollection : IEnumerable<CsvRecord>
    {
        private readonly CsvReader _reader;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal CsvRecordCollection(CsvReader reader) => this._reader = reader;


#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public IEnumerator<CsvRecord> GetEnumerator() => _reader.GetEnumerator();

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
