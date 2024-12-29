using FolkerKinzel.CsvTools;

namespace Examples;

internal static class EnableCachingExample
{
    public static void DisableCachingSideEffects()
    {
        const string fileName = "LinqTest.csv";

        File.WriteAllText(fileName, """
            Name,City
            Ingrid,Berlin
            Joyce,New York
            Horst,Hamburg
            John,New York
            """);

        Console.WriteLine("Which people live in New York?: ");

        Console.Write("  Determine with cache enabled:  ");
        using (CsvEnumerator csv = Csv.OpenRead(fileName, options: CsvOpts.Default.Set(CsvOpts.EnableCaching)))
        {
            foreach (CsvRecord record in
                // ToArray() caches the results.
                csv.Where(x => x["City"].Span.Equals("New York", StringComparison.Ordinal)).ToArray())
            {
                Console.Write(record["Name"]);
                Console.Write(' ');
            }

            Console.WriteLine();
        }

        // The caching is disabled by default. If you still try to cache the results
        // you will get wrong results.
        // NOTICE: Removing ".ToArray()" would cause the correct results.
        Console.Write("  Determine with cache disabled: ");
        using (CsvEnumerator csv = Csv.OpenRead(fileName))
        {
            foreach (CsvRecord record in
                csv.Where(x => x["City"].Span.Equals("New York", StringComparison.Ordinal)).ToArray())
            {
                Console.Write(record["Name"]);
                Console.Write(' ');
            }
        }

        Console.WriteLine();

        // Console Output: 
        // Which people live in New York?:
        //   Determine with cache enabled:  Joyce John
        //   Determine with cache disabled: John John
    }
}
