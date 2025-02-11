using FolkerKinzel.CsvTools;

namespace Examples;

internal static class LinqOnCsvExample
{
    public static void LinqOnCsv(string filePath)
    {
        object?[][] customers =
            [
                ["Customer", "PhoneNumbers", "Sales"],
                ["Detlef",    null,            3.85m],
                ["Susi",     """
                             0177-4711,
                             123-4567
                             """,         39_457.26m],
                ["Gabi",    "08-15",      28.70m]
            ];

        customers.SaveCsv(filePath);

        Console.WriteLine(File.ReadAllText(filePath));

        using CsvReader csv = Csv.OpenRead(filePath);

        Console.WriteLine();
        Console.WriteLine("What phone numbers does Susi have?:");
        Console.WriteLine(
            csv.FirstOrDefault(x => x["Customer"].Span.Equals("Susi", StringComparison.Ordinal))?["PhoneNumbers"]);
    }
}
/*
Console Output:

Customer,PhoneNumbers,Sales
Detlef,,3.85
Susi,"0177-4711,
123-4567",39457.26
Gabi,08-15,28.70

What phone numbers does Susi have?:
0177-4711,
123-4567
*/
