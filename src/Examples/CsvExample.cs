using System.Text;
using FolkerKinzel.CsvTools;

namespace Examples;

internal static class CsvExample
{
    public static void HandleForeignCsvFile(string filePath)
    {
        const string nonStandardCsv = """


            First # "Second # Column"   
            1,"2",3 # "Get's
            too much" # LOST?

            too few


            """;

        File.WriteAllText(filePath, nonStandardCsv, Encoding.Unicode);

        using CsvReader csv = Csv.OpenReadAnalyzed(filePath);
        CsvRecord[] data = [.. csv];

        using (CsvWriter writer = Csv.OpenWrite(filePath, data[0].ColumnNames))
        {
            foreach (CsvRecord record in data)
            {
                writer.Record.FillWith(record.Values);
                writer.WriteRecord();
            }
        }

        Console.WriteLine(File.ReadAllText(filePath));
    }
}

/*
 Console output:

Column1,Column2,Column3
First ,Second # Column,
"1,""2"",3 ","Get's
too much", LOST?
too few,,

*/
