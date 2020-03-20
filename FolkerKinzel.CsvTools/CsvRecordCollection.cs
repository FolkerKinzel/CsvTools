using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FolkerKinzel.CsvTools
{
    internal readonly struct CsvRecordCollection : IEnumerable<CsvRecord>
    {
        private readonly CsvReader _reader;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal CsvRecordCollection(CsvReader reader)
        {
            this._reader = reader;
        }


#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public IEnumerator<CsvRecord> GetEnumerator()
        {
            return _reader.GetEnumerator();
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
