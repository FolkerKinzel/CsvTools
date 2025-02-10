using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass]
public class CsvExtensionTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void SaveCsvTest1()
    {
        string path = Path.Combine(TestContext.TestRunResultsDirectory!, "SaveCsvTest1.csv");
        new string[][] { [] }.SaveCsv(path, null);
        Assert.IsTrue(File.Exists(path));

        using CsvReader reader = Csv.OpenRead(path);
        CsvRecord[] results = [.. reader];

        Assert.AreEqual(0, results.Length);
    }

    [TestMethod]
    public void SaveCsvTest2()
    {
        string path = Path.Combine(TestContext.TestRunResultsDirectory!, "SaveCsvTest1.csv");
        new string?[][] { [null] }.SaveCsv(path, null);
        Assert.IsTrue(File.Exists(path));

        using CsvReader reader = Csv.OpenRead(path);
        CsvRecord[] results = [.. reader];

        Assert.AreEqual(0, results.Length);
    }

    [TestMethod]
    public void SaveCsvTest3()
    {
        string path = Path.Combine(TestContext.TestRunResultsDirectory!, "SaveCsvTest1.csv");
        new string?[]?[] { null }.SaveCsv(path, null);
        Assert.IsTrue(File.Exists(path));

        using CsvReader reader = Csv.OpenRead(path, isHeaderPresent: false);
        CsvRecord[] results = [.. reader];

        Assert.AreEqual(0, results.Length);
    }

    [TestMethod]
    public void ToCsvTest1()
    {
        string csv = new string[][] { [] }.ToCsv(null);
        Assert.IsNotNull(csv);
        Assert.AreEqual(0, csv.Length);
    }

    [TestMethod]
    public void ToCsvTest2()
    {
        string csv = new string?[][] { [null] }.ToCsv(null);
        Assert.IsNotNull(csv);
        Assert.AreEqual(0, csv.Length);
    }

    [TestMethod]
    public void ToCsvTest3()
    {
        string csv = new object?[]?[] { null, [7] }.ToCsv(null);
        Assert.IsNotNull(csv);
        Assert.AreEqual("\r\n7", csv);
    }

    [TestMethod]
    public void ToCsvTest4()
    {
        string csv = new string[][] { ["a", "b", "c"] }.ToCsv(CultureInfo.InvariantCulture);
        Assert.IsNotNull(csv);
        Assert.AreEqual("a,b,c", csv);
    }

    [TestMethod]
    public void ToCsvTest5()
    {
        string csv = new string?[][] { ["a", "b", null, "c"] }.ToCsv(null);
        Assert.IsNotNull(csv);
        Assert.AreEqual("a,b,,c", csv);
    }

    [TestMethod]
    public void ToCsvTest6()
    {
        string csv = new object[][] { [3.14] }.ToCsv(CultureInfo.CreateSpecificCulture("de-DE"));
        Assert.IsNotNull(csv);
        Assert.AreEqual("\"3,14\"", csv);
    }

    [TestMethod]
    public void ToCsvTest7()
    {
        string csv = new Version[][] { [new(2, 1, 7)] }.ToCsv(null);
        Assert.IsNotNull(csv);
        Assert.AreEqual("2.1.7", csv);
    }

    [TestMethod]
    public void ToCsvTest8()
    {
        string csv = new object[][] { [7] }.ToCsv(null, "000");
        Assert.IsNotNull(csv);
        Assert.AreEqual("007", csv);
    }

    [TestMethod]
    public void ToCsvTest9()
    {
        using var stringWriter = new StringWriter();
        new string[][] { ["a", "b", "c"] }.WriteCsv(stringWriter, CultureInfo.InvariantCulture);
        string csv = stringWriter.ToString();
        Assert.IsNotNull(csv);
        Assert.AreEqual("a,b,c", csv);
    }

}
