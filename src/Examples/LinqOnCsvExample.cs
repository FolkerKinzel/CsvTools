using FolkerKinzel.CsvTools;

namespace Examples;

internal static class LinqOnCsvExample
{
    public static void LinqOnCsvFile()
    {
        const string fileName = "LinqTest.csv";

        File.WriteAllText(fileName, """
            Name,City
            Ingrid,Berlin
            Joyce,New York
            Horst,Hamburg
            John,New York
            """);

        using CsvEnumerator csv = Csv.OpenRead(fileName);
        Console.Write("How many people live in New York?: ");
        Console.WriteLine(
            csv.Where(x => x["City"].Span.Equals("New York", StringComparison.Ordinal)).Count());

        // Console Output: How many people live in New York?: 2
    }
}
