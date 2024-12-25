using System;
using System.IO;
using System.Linq;
using System.Text;
using FolkerKinzel.CsvTools;

namespace Examples
{
    internal static class LinqOnCsvFile
    {
        public static void TestLinqOnCsvFile()
        {
            const string csvFileName = "LinqTest.csv";

            File.WriteAllText(csvFileName, """
                Name,City
                Ingrid,Berlin
                Joyce,New York
                Horst,Hamburg
                John,New York
                """);

            using var csvReader = new CsvReader(csvFileName);
            Console.Write("How many people live in New York?: ");
            Console.WriteLine(
                csvReader.Where(x => x["City"].Span.Equals("New York", StringComparison.Ordinal)).Count());

            // Console Output: How many people live in New York?: 2
        }
    }
}
