using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Examples
{
    /// <summary>
    /// This example shows, that setting the flag <see cref="CsvOptions.DisableCaching"/> 
    /// may cause unexpected results,
    /// if you try to cache the result set in further processing.
    /// </summary>
    static class DisableCachingAndLinq
    {
        public static void TestDisableCachingAndLinq()
        {
            const string csvFileName = "LinqTest.csv";

            File.WriteAllText(csvFileName, new StringBuilder()
                .AppendLine("Name,City")
                .AppendLine("Ingrid,Berlin")
                .AppendLine("Joyce,New York")
                .AppendLine("Horst,Hamburg")
                .AppendLine("John,New York")
                .ToString());


            Console.WriteLine("How many people live in New York?: ");

            Console.Write("Determine with cache enabled:  ");
            using (var csvReader = new CsvReader(csvFileName))
            {

                foreach (var record in csvReader.Read().Where(x => x["City"] == "New York").ToArray())
                {
                    Console.Write(record["Name"]);
                    Console.Write(' ');
                }

                Console.WriteLine();
            }


            Console.Write("Determine with cache disabled: ");
            using (var csvReader = new CsvReader(csvFileName, options: CsvOptions.Default.Set(CsvOptions.DisableCaching)))
            {
                // NOTICE: Removing ".ToArray()" would cause the correct results:
                foreach (var record in csvReader.Read().Where(x => x["City"] == "New York").ToArray())
                {
                    Console.Write(record["Name"]);
                    Console.Write(' ');
                }
            }

            // Console Output: 
            // How many people live in New York?:
            // Determine with cache enabled:  Joyce John
            // Determine with cache disabled: John John
        }
    }
}
