using FolkerKinzel.CsvTools;

namespace Examples;

internal static class LinqOnCsvExample
{
    public static void LinqOnCsvFile(string filePath)
    {
        File.WriteAllText(filePath, """
            Name,City
            Ingrid,Berlin
            Joyce,New York
            Horst,Hamburg
            John,New York
            """);

        using CsvReader csv = Csv.OpenRead(filePath);
        Console.Write("How many people live in New York?: ");
        Console.WriteLine(
            csv.Where(x => x["City"].Span.Equals("New York", StringComparison.Ordinal)).Count());

        // Console Output: How many people live in New York?: 2
    }
}
