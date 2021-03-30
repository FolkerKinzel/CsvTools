using System;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class DBNullBenchmarks
    {
        private readonly object _o = DBNull.Value;

        [Benchmark]
        public bool ConvertIsDBNull() => Convert.IsDBNull(_o);

        [Benchmark]
        public bool IsDBNull() => _o is DBNull;

        [Benchmark]
        public bool EqualSign() => _o == DBNull.Value;

        [Benchmark]
        public bool EqualsMethod() => DBNull.Value.Equals(_o);

        [Benchmark]
        public bool ReferenceEquals() => object.ReferenceEquals(_o, DBNull.Value);

    }
}
