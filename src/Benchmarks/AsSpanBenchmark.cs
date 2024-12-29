using BenchmarkDotNet.Attributes;

namespace Benchmarks;

public class AsSpanBenchmark
{
    private readonly int[] _array = new int[10000];

    public AsSpanBenchmark()
    {
        for (int i = 0; i < _array.Length; i++)
        {
            _array[i] = i;
        }
    }

    [Benchmark]
    public int Sum()
    {
        int sum = 0;

        for (int i = 0; i < _array.Length; i++)
        {
            sum += _array[i];
        }

        return sum;
    }

    [Benchmark]
    public int SumAsSpan()
    {
        int sum = 0;

        for (int i = 0; i < _array.Length; i++)
        {
            sum += _array.AsSpan()[i];
        }

        return sum;
    }
}
