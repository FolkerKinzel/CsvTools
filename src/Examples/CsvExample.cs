﻿using System.Text;
using FolkerKinzel.CsvTools;

namespace Examples;

internal static class CsvExample
{
    public static void HandleForeignCsvFile(string filePath)
    {
        const string nonStandardCsv = """


            First # "Second # Column"   
            1,"2",3 # Too much get's # LOST

            too few


            """;

        File.WriteAllText(filePath, nonStandardCsv, Encoding.Unicode);

        using CsvEnumerator csv = Csv.OpenReadAnalyzed(filePath);
        CsvRecord[] data = [.. csv];

        Csv.Write(data.Select(x => x.Values), filePath, data[0].ColumnNames);

        Console.WriteLine(File.ReadAllText(filePath));
    }
}

/*
 Console output:

First,Second # Column
"1,""2"",3",Too much get's
too few,

*/