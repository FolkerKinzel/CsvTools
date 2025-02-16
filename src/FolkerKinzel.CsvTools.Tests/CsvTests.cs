using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using FolkerKinzel.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass]
public class CsvTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    [ExpectedException(typeof(IOException))]
    public void OpenReadAnalyzedTest1()
    {
        using CsvReader reader = Csv.OpenReadAnalyzed("DoesNotExist");
    }

    [TestMethod]
    public void OpenReadAnalyzedTest2()
    {
        using CsvReader reader = Csv.OpenReadAnalyzed(TestFiles.AnalyzerTestCsv);
        CsvRecord[] arr = [.. reader];
        Assert.AreEqual(4, arr.Length);
        Assert.AreEqual(4, arr.Distinct().Count());
    }

    [TestMethod]
    public void OpenReadAnalyzedTest3()
    {
        using CsvReader reader = Csv.OpenReadAnalyzed(TestFiles.AnalyzerTestCsv, disableCaching: true);
        CsvRecord[] arr = [.. reader];
        Assert.AreEqual(4, arr.Length);
        Assert.AreEqual(1, arr.Distinct().Count());
    }

    [TestMethod]
    public void OpenReadAnalyzedTest4()
    {
        using CsvReader reader = Csv.OpenReadAnalyzed(TestFiles.AnalyzerTestCsv, header: Header.Absent, disableCaching: true);
        CsvRecord[] arr = [.. reader];
        Assert.AreEqual(5, arr.Length);
        Assert.AreEqual(8, arr[0].Count);
        Assert.AreEqual(1, arr.Distinct().Count());
    }

    [TestMethod]
    public void OpenReadAnalyzedTest5()
    {
        using CsvReader reader = Csv.OpenReadAnalyzed(TestFiles.AnalyzerTestCsv, header: Header.Absent);
        CsvRecord[] arr = [.. reader];
        Assert.AreEqual(5, arr.Length);
        Assert.AreEqual(5, arr.Distinct().Count());
        Assert.AreEqual(8, arr[0].Count);
    }

    [TestMethod]
    public void AnalyzeTest1()
    {
        (CsvAnalyzerResult _, Encoding encoding) = Csv.AnalyzeFile(TestFiles.AnalyzerTestCsv, fallbackEncoding: TextEncodingConverter.GetEncoding("iso-8859-1"));
        Assert.AreEqual("iso-8859-1", encoding.WebName, true, CultureInfo.InvariantCulture);
    }

    [DataTestMethod]
    [DataRow(',')]
    [DataRow(';')]
    [DataRow('#')]
    [DataRow('\t')]
    [DataRow(' ')]
    public void OpenReadTest1(char delimiter)
    {
        using CsvReader reader = Csv.OpenRead(TestFiles.AnalyzerTestCsv, delimiter: delimiter);
    }

    [DataTestMethod]
    [DataRow('\"')]
    [DataRow('\r')]
    [DataRow('\n')]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OpenReadTest2(char delimiter)
    {
        using CsvReader reader = Csv.OpenRead(TestFiles.AnalyzerTestCsv, delimiter: delimiter);
    }

    [DataTestMethod]
    [DataRow(',')]
    [DataRow(';')]
    [DataRow('#')]
    [DataRow('\t')]
    [DataRow(' ')]
    public void OpenReadTest3(char delimiter)
    {
        using StringReader stringReader = new("Hi");
        using CsvReader reader = Csv.OpenRead(stringReader, delimiter: delimiter);
    }

    [DataTestMethod]
    [DataRow('\"')]
    [DataRow('\r')]
    [DataRow('\n')]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OpenReadTest4(char delimiter)
    {
        using StringReader stringReader = new("Hi");
        using CsvReader reader = Csv.OpenRead(stringReader, delimiter: delimiter);
    }

    [DataTestMethod]
    [DataRow(',')]
    [DataRow(';')]
    [DataRow('#')]
    [DataRow('\t')]
    [DataRow(' ')]
    public void OpenReadTest5(char delimiter) => _ = Csv.Parse("Hi", delimiter: delimiter);

    [DataTestMethod]
    [DataRow('\"')]
    [DataRow('\r')]
    [DataRow('\n')]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OpenReadTest6(char delimiter) => _ = Csv.Parse("Hi", delimiter: delimiter);

    [TestMethod]
    public void OpenWriteTest1()
    {
        string fileName = Path.Combine(TestContext.TestRunResultsDirectory!, "OpenWriteTest1.csv");
        using CsvWriter writer = Csv.OpenWrite(fileName, 2);
        Assert.AreEqual(',', writer.Delimiter);
    }

    [TestMethod]
    public void OpenWriteTest2()
    {
        string fileName = Path.Combine(TestContext.TestRunResultsDirectory!, "OpenWriteTest1.csv");
        using CsvWriter writer = Csv.OpenWrite(fileName, []);
        Assert.AreEqual(',', writer.Delimiter);
    }

    [TestMethod]
    public void OpenWriteTest3()
    {
        using var stringWriter = new StringWriter();
        using CsvWriter writer = Csv.OpenWrite(stringWriter, []);
        Assert.AreEqual(',', writer.Delimiter);
    }

    [TestMethod]
    public void OpenWriteTest4()
    {
        using var stringWriter = new StringWriter();
        using CsvWriter writer = Csv.OpenWrite(stringWriter, 2);
        Assert.AreEqual(',', writer.Delimiter);
    }

    [TestMethod]
    public void OpenWriteTest5()
    {
        using var stringWriter = new StringWriter();
        using CsvWriter writer = Csv.OpenWrite(stringWriter, ["NAME", "name"]);
        Assert.AreEqual(',', writer.Delimiter);
        Assert.IsFalse(writer.Record.Comparer.Equals("A", "a"));
    }

    [TestMethod]
    public void ParseTest1()
    {
        CsvRecord[] result = Csv.Parse("""
            A,A,A
            1,2,3
            """);

        Assert.AreEqual(1, result.Length);
        Assert.AreEqual("A", result[0].ColumnNames[0]);
        Assert.AreEqual("A2", result[0].ColumnNames[1]);
        Assert.AreEqual("A3", result[0].ColumnNames[2]);
    }

    [TestMethod]
    public void ParseAnalyzedTest1()
    {
        var result = Csv.ParseAnalyzed("");
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void MyTest()
    {
        const string fileName = "C:\\Users\\fkinz\\source\\repos\\FolkerKinzel.CsvTools\\src\\FolkerKinzel.CsvTools.Tests\\TestFiles\\UTF8 (2).csv";
        var encoding = Encoding.GetEncoding("Latin1");
        encoding = Encoding.UTF8;

        int latin1CodePage = encoding.CodePage;
        int ansiCodePage = CultureInfo.CurrentCulture.TextInfo.ANSICodePage;
        int ebcdicCodePage = CultureInfo.CurrentCulture.TextInfo.EBCDICCodePage;
        int macCodePage = CultureInfo.CurrentCulture.TextInfo.MacCodePage;
        int oemCodePage = CultureInfo.CurrentCulture.TextInfo.OEMCodePage;

        var enc2 = TextEncodingConverter.GetEncoding(20273);

        (char Delimiter, IFormatProvider FormatProvider) = Csv.GetExcelArguments();

        var ansiEncoding = TextEncodingConverter.GetEncoding(ansiCodePage);
        using CsvReader reader = Csv.OpenRead(fileName,
            Delimiter, textEncoding: ansiEncoding, isHeaderPresent: true);
        CsvRecord[] resAnsi = [.. reader];

        var ebcdicEncoding = TextEncodingConverter.GetEncoding(ebcdicCodePage);
        using CsvReader reader2 = Csv.OpenRead(fileName,
            Delimiter, textEncoding: ebcdicEncoding, isHeaderPresent: true);
        CsvRecord[] resEbcdic = [.. reader2];

        var macEncoding = TextEncodingConverter.GetEncoding(macCodePage);
        using CsvReader reader3 = Csv.OpenRead(fileName,
           Delimiter, textEncoding: macEncoding, isHeaderPresent: true);
        CsvRecord[] resMac = [.. reader3];

        var oemEncoding = TextEncodingConverter.GetEncoding(oemCodePage);
        using CsvReader reader4 = Csv.OpenRead(fileName,
           Delimiter, textEncoding: oemEncoding, isHeaderPresent: true);
        CsvRecord[] resOem = [.. reader4];

        new string[][] { ["A", "B"],
                         ["ÄÖÜäöüß€µ簾", "ÄÖÜäöüß€µ簾"]}.SaveCsv("ExportedAsUtf8.csv", ';', CultureInfo.CurrentCulture);
    }
}
