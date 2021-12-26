using System.Text;
using FolkerKinzel.CsvTools;

namespace Examples;

public static class LinqOnCsvFile
{
    public static void TestLinqOnCsvFile()
    {
        const string csvFileName = "LinqTest.csv";

        File.WriteAllText(csvFileName, new StringBuilder()
            .AppendLine("Name,City")
            .AppendLine("Ingrid,Berlin")
            .AppendLine("Joyce,New York")
            .AppendLine("Horst,Hamburg")
            .AppendLine("John,New York")
            .ToString());

        using var csvReader = new CsvReader(csvFileName);
        Console.Write("How many people live in New York?: ");
        Console.WriteLine(
            csvReader.Read().Where(x => x["City"] == "New York").Count());

        // Console Output: How many people live in New York?: 2
    }
}
