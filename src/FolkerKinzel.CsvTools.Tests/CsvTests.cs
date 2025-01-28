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
        (CsvAnalyzerResult _, Encoding encoding) = Csv.AnalyzeFile(TestFiles.AnalyzerTestCsv, textEncoding: TextEncodingConverter.GetEncoding("iso-8859-1"));
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
    public void OpenReadTest5(char delimiter)
    {
        _ = Csv.Parse("Hi", delimiter: delimiter);
    }

    [DataTestMethod]
    [DataRow('\"')]
    [DataRow('\r')]
    [DataRow('\n')]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void OpenReadTest6(char delimiter)
    {
        _ = Csv.Parse("Hi", delimiter: delimiter);
    }


    [TestMethod]
    public void OpenWriteTest1()
    {
        string fileName = Path.Combine(TestContext.TestRunResultsDirectory!, "OpenWriteTest1.csv");
        using var writer = Csv.OpenWrite(fileName, 2);
        Assert.AreEqual(',', writer.Delimiter);
    }

    [TestMethod]
    public void OpenWriteTest2()
    {
        string fileName = Path.Combine(TestContext.TestRunResultsDirectory!, "OpenWriteTest1.csv");
        using var writer = Csv.OpenWrite(fileName, []);
        Assert.AreEqual(',', writer.Delimiter);
    }

    [TestMethod]
    public void OpenWriteTest3()
    {
        using var stringWriter = new StringWriter();
        using var writer = Csv.OpenWrite(stringWriter, []);
        Assert.AreEqual(',', writer.Delimiter);
    }

    [TestMethod]
    public void OpenWriteTest4()
    {
        using var stringWriter = new StringWriter();
        using var writer = Csv.OpenWrite(stringWriter, 2);
        Assert.AreEqual(',', writer.Delimiter);
    }
}
