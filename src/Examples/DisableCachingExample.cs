using FolkerKinzel.CsvTools;

namespace Examples;

internal static class DisableCachingExample
{
    public static void DisableCachingSideEffects(string filePath)
    {
        string[][] friends = [
                                ["Name", "City"],
                                ["Ingrid","Berlin"],
                                ["Joyce","New York"],
                                ["Horst","Hamburg"],
                                ["John","New York"],
                             ];
        friends.SaveCsv(filePath);

        Console.WriteLine("Which friends live in New York?: ");
        Console.Write("  Determine with cache enabled:  ");

        // Caching is enabled by default.
        using (CsvReader csv = Csv.OpenRead(filePath))
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
        using (CsvReader csv = Csv.OpenRead(filePath, 
                                            options: CsvOpts.Default.Set(CsvOpts.DisableCaching)))
        {
            foreach (CsvRecord record in
                // NOTE: Removing ".ToArray()" would cause the correct results.
                csv.Where(x => x["City"].Span.Equals("New York", StringComparison.Ordinal)).ToArray())
            {
                Console.Write(record["Name"]);
                Console.Write(' ');
            }
        }
    }
}
/*
Console Output:

Which friends live in New York?:
  Determine with cache enabled:  Joyce John
  Determine with cache disabled: John John
*/
