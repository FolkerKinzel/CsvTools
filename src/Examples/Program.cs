namespace Examples
{
    internal static class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0022:Ausdruckskörper für Methoden verwenden", Justification = "<Ausstehend>")]
        private static void Main()
        {
             //LinqOnCsvExamples.LinqOnCsvFile();
             //DisableCachingExamples.DisableCachingSideEffects();
            CsvExamples.HandleForeignCsvFile("foreign.csv");
        }
    }
}
