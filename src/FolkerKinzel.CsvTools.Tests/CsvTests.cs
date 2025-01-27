using System.Globalization;
using System.Text;
using FolkerKinzel.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass]
public class CsvTests
{
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
}
