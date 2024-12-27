using System;
using System.IO;
using System.Linq;
using System.Text;
using FolkerKinzel.CsvTools;

namespace Examples
{
    internal static class DisableCachingAndLinq
    {
        public static void TestDisableCachingAndLinq()
        {
            const string csvFileName = "LinqTest.csv";

            File.WriteAllText(csvFileName, """
                Name,City
                Ingrid,Berlin
                Joyce,New York
                Horst,Hamburg
                John,New York
                """);

            Console.WriteLine("Which people live in New York?: ");

            Console.Write("  Determine with cache enabled:  ");
            using (var csvReader = new CsvReader(csvFileName))
            {
                foreach (CsvRecord record in
                    csvReader.Where(x => x["City"].Span.Equals("New York", StringComparison.Ordinal)).ToArray())
                {
                    Console.Write(record["Name"]);
                    Console.Write(' ');
                }

                Console.WriteLine();
            }

            Console.Write("  Determine with cache disabled: ");
            using (var csvReader =
                new CsvReader(csvFileName,
                              options: CsvOptions.Default.Set(CsvOptions.DisableCaching)))
            {
                // NOTICE: Removing ".ToArray()" would cause the correct results:
                foreach (CsvRecord record in
                    csvReader.Where(x => x["City"].Span.Equals("New York", StringComparison.Ordinal)).ToArray())
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
}
