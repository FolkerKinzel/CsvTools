using System.Data;
using System.Globalization;
using FolkerKinzel.CsvTools;

namespace Examples;

internal static class DataTableExample
{
    internal static void SerializingDataTablesAsCsv(string filePath)
    {
        using var table = new DataTable();
        _ = table.Columns.Add("id", typeof(int));
        _ = table.Columns.Add("name", typeof(string));
        _ = table.Columns.Add("sales", typeof(decimal));
        _ = table.Columns.Add("last_purchase", typeof(DateOnly));
        _ = table.Columns.Add("reserved", typeof(string));

        _ = table.Rows.Add(1, "Susi", 4_711m, new DateOnly(2004, 3, 14), "my comment");
        _ = table.Rows.Add(2, "Tom", 38_527.28m, new DateOnly(2006, 12, 24));
        _ = table.Rows.Add(3, "Rachel", 25.8m, new DateOnly(2011, 8, 27));

        string[] csvColumns = ["name", "last_purchase", "sales"];
        table.WriteCsv(filePath, csvColumns);

        Console.WriteLine(File.ReadAllText(filePath));


        // Write a CSV file that can be imported by Excel:
        Console.WriteLine();
        Console.WriteLine("Current culture: {0}", CultureInfo.CurrentCulture);
        Console.WriteLine();
        
        (IFormatProvider FormatProvider, char Delimiter) = CsvAnalyzer.GetExcelParameters();
        table.WriteCsv(filePath, csvColumns, FormatProvider, delimiter: Delimiter);

        Console.WriteLine(File.ReadAllText(filePath));
    }
}

/*
Console output:

name,last_purchase,sales
Susi,03/14/2004,4711
Tom,12/24/2006,38527.28
Rachel,08/27/2011,25.8

Current culture: de-DE

name;last_purchase;sales
Susi;14.03.2004;4711
Tom;24.12.2006;38527,28
Rachel;27.08.2011;25,8
*/
