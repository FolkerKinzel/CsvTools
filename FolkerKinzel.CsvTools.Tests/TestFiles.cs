using System.IO;

namespace FolkerKinzel.CsvTools.Tests
{
    internal static class TestFiles
    {
        private const string TEST_FILE_DIRECTORY_NAME = "TestFiles";

        static readonly string _testFileDirectory;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Statische Felder für Referenztyp inline initialisieren", Justification = "<Ausstehend>")]
        static TestFiles()
        {
            ProjectDirectory = Properties.Resources.ProjDir.Trim();
            _testFileDirectory = Path.Combine(ProjectDirectory, TEST_FILE_DIRECTORY_NAME);
        }


        internal static string[] GetAll()
        {
            return Directory.GetFiles(_testFileDirectory);
        }


        internal static string ProjectDirectory { get; }

        internal static string GoogleCsv => Path.Combine(_testFileDirectory, "Google.csv");

        internal static string GoogleCreatedOutlookCsv => Path.Combine(_testFileDirectory, "GoogleCreatedOutlook.csv");

        internal static string Outlook365Csv => Path.Combine(_testFileDirectory, "Outlook365.csv");

        internal static string ThunderbirdAnsiCsv => Path.Combine(_testFileDirectory, "ThunderbirdAnsi.csv");

        internal static string ThunderbirdUtf8Csv => Path.Combine(_testFileDirectory, "ThunderbirdUtf8.csv");


    }
}
