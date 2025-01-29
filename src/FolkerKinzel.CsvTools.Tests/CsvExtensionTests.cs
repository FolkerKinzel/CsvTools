using System.Diagnostics.CodeAnalysis;
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
        Array.Empty<string>().SaveCsv(path);
        Assert.IsTrue(File.Exists(path));

        using CsvReader reader = Csv.OpenRead(path);
        CsvRecord[] results = [.. reader];

        Assert.AreEqual(0, results.Length);
    }

    [TestMethod]
    public void SaveCsvTest2()
    {
        string path = Path.Combine(TestContext.TestRunResultsDirectory!, "SaveCsvTest1.csv");
        new string?[] {null}.SaveCsv(path);
        Assert.IsTrue(File.Exists(path));

        using CsvReader reader = Csv.OpenRead(path);
        CsvRecord[] results = [.. reader];

        Assert.AreEqual(0, results.Length);
    }

    [TestMethod]
    public void SaveCsvTest3()
    {
        string path = Path.Combine(TestContext.TestRunResultsDirectory!, "SaveCsvTest1.csv");
        new string?[]?[] { null }.SaveCsv(path);
        Assert.IsTrue(File.Exists(path));

        using CsvReader reader = Csv.OpenRead(path, isHeaderPresent: false);
        CsvRecord[] results = [.. reader];

        Assert.AreEqual(0, results.Length);
    }

    [TestMethod]
    public void ToCsvTest1()
    {
        string csv = Array.Empty<string>().ToCsv();
        Assert.IsNotNull(csv);
        Assert.AreEqual(0, Csv.ParseAnalyzed(csv).Length);
    }

    [TestMethod]
    public void ToCsvTest2()
    {
        string csv = new string?[] { null }.ToCsv();
        Assert.IsNotNull(csv);
        Assert.AreEqual(0, Csv.ParseAnalyzed(csv).Length);
    }

    [TestMethod]
    public void ToCsvTest3()
    {
        string csv = new object?[]?[] { null, [7] }.ToCsv();
        Assert.IsNotNull(csv);
        Assert.AreEqual(0, Csv.ParseAnalyzed(csv).Length);
    }

    [TestMethod]
    public void ToCsvTest4()
    {
        string csv = new string?[]?[] { ["a", "b", "c"] }.ToCsv();
        Assert.IsNotNull(csv);
        Assert.AreEqual(0, Csv.ParseAnalyzed(csv).Length);
    }

    [TestMethod]
    public void ToCsvTest5()
    {
        string csv = new string?[]?[] { ["a", "b", null, "c"] }.ToCsv();
        Assert.IsNotNull(csv);
        Assert.AreEqual(1, Csv.ParseAnalyzed(csv, header: Header.Absent).Length);
    }
}
