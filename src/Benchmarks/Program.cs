using System;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
    internal class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnötige Zuweisung eines Werts.", Justification = "<Ausstehend>")]
        private static void Main()
        {
            //Summary summary = BenchmarkRunner.Run<DBNullBenchmarks>();
            Summary summary = BenchmarkRunner.Run<AccessBenchmark>();

        }
    }
}
