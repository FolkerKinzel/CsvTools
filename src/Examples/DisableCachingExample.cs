using FolkerKinzel.CsvTools;

namespace Examples;

internal static class DisableCachingExample
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
        // Caching is enabled by default.
        using (CsvReader csv = Csv.OpenRead(fileName))
        {
            foreach (CsvRecord record in
                // ToArray() caches the results:
                csv.Where(x => x["City"].Span.Equals("New York", StringComparison.Ordinal)).ToArray())
            {
                Console.Write(record["Name"]);
                Console.Write(' ');
            }

            Console.WriteLine();
        }

        Console.Write("  Determine with cache disabled: ");
        // If you set the flag CsvOpts.DisableCaching and then try to cache the data,
        // you will get the wrong results:
        using (CsvReader csv = Csv.OpenRead(fileName, 
                                                options: CsvOpts.Default.Set(CsvOpts.DisableCaching)))
        {
            foreach (CsvRecord record in
                // NOTICE: Removing ".ToArray()" would cause the correct results.
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
