namespace Examples;

internal static class Program
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0022:Ausdruckskörper für Methoden verwenden", Justification = "<Ausstehend>")]
    private static void Main()
    {
        LinqOnCsvExample.LinqOnCsvFile("LinqTest.csv");
        DisableCachingExample.DisableCachingSideEffects("DisableCachingTest.csv");
        CsvAnalyzerExample.ParseForeignCsvFile("foreign.csv");
        CsvStringExample.ConvertingCsvStrings();
    }
}
