using FolkerKinzel.CsvTools;

namespace Examples;

internal static class DisableCachingExamples
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
        using (CsvEnumerator csv = Csv.OpenRead(fileName))
        {
            foreach (CsvRecord record in
                csv.Where(x => x["City"].Span.Equals("New York", StringComparison.Ordinal)).ToArray())
            {
                Console.Write(record["Name"]);
                Console.Write(' ');
            }

            Console.WriteLine();
        }

        Console.Write("  Determine with cache disabled: ");
        using (CsvEnumerator csv = Csv.OpenRead(fileName,
                                                options: CsvOpts.Default.Set(CsvOpts.DisableCaching)))
        {
            // NOTICE: Removing ".ToArray()" would cause the correct results:
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
