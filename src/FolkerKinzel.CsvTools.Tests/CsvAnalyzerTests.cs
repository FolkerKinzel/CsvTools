using System;
using System.Linq;
using System.Text;
using FolkerKinzel.CsvTools.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass()]
public class CsvAnalyzerTests
{
    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AnalyzeFileTest1() => Csv.AnalyzeFile(null!);

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void AnalyzeFileTest2() => Csv.AnalyzeFile("  ");

    [TestMethod()]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void AnalyzeFileTest2b() => Csv.AnalyzeFile("Test", header: (Header)4711);

    [TestMethod()]
    public void AnalyzeFileTest3()
    {
        (CsvAnalyzerResult results, Encoding enc) = Csv.AnalyzeFile(TestFiles.AnalyzerTestCsv);
        AssertAnalyzerTestCsv(results);
        Assert.IsNotNull(enc);
    }

    [TestMethod]
    public void AnalyzeFileTest4()
    {
        CsvAnalyzerResult result = CsvAnalyzer.AnalyzeFile(TestFiles.AnalyzerTestCsv, analyzedLines: -42);
        AssertAnalyzerTestCsv(result);
    }

    private static void AssertAnalyzerTestCsv(CsvAnalyzerResult result)
    {
        Assert.IsTrue(result.IsHeaderPresent);
        Assert.AreEqual(';', result.Delimiter);
        CollectionAssert.AreEqual(result.ColumnNames?.ToArray(), new string?[] { "Eins", "eins", "zwei", "drei", null });
        Assert.IsTrue(result.Options.HasFlag(CsvOpts.CaseSensitiveKeys));
        Assert.IsTrue(result.Options.HasFlag(CsvOpts.TrimColumns));
        Assert.IsFalse(result.Options.HasFlag(CsvOpts.ThrowOnTooFewFields));
        Assert.IsFalse(result.Options.HasFlag(CsvOpts.ThrowOnTooMuchFields));
        Assert.IsFalse(result.Options.HasFlag(CsvOpts.DisableCaching));
        Assert.IsFalse(result.Options.HasFlag(CsvOpts.ThrowOnEmptyLines));
    }

    [TestMethod()]
    public void AnalyzeFileTest5()
    {
        CsvAnalyzerResult result = CsvAnalyzer.AnalyzeFile(TestFiles.GoogleCsv);

        Assert.IsTrue(result.IsHeaderPresent);
        Assert.AreEqual(',', result.Delimiter);

        Assert.AreEqual(result.Options, CsvOpts.Default);
    }

    [TestMethod]
    public void AnalyzeStringTest1()
    {
        CsvAnalyzerResult result = Csv.AnalyzeString("a;b;c");
        Assert.IsTrue(result.IsHeaderPresent);
        Assert.AreEqual(';', result.Delimiter);
    }

    [TestMethod]
    public void HeaderTest1()
    {
        const string csv = """
            a,   ,b
            1,2,3
            """;

        CsvAnalyzerResult result = Csv.AnalyzeString(csv);
        Assert.IsFalse(result.IsHeaderPresent);
        Assert.AreEqual(CsvOpts.Default, result.Options);
    }

    [TestMethod]
    public void HeaderTest2()
    {
        const string csv = """
            a,"x,y",b
            1,2,3
            """;

        CsvAnalyzerResult result = Csv.AnalyzeString(csv);
        Assert.IsFalse(result.IsHeaderPresent);
        Assert.AreEqual(CsvOpts.Default, result.Options);
    }

    [TestMethod]
    public void HeaderTest3()
    {
        const string csv = """
            a,"x,y",b
            1,2,3
            """;

        CsvAnalyzerResult result = Csv.AnalyzeString(csv, Header.Present);
        Assert.IsTrue(result.IsHeaderPresent);
        Assert.AreEqual(CsvOpts.Default, result.Options);
    }

    [TestMethod]
    public void HeaderTest4()
    {
        const string csv = """
            a,b,
            1,2,3
            """;

        CsvAnalyzerResult result = Csv.AnalyzeString(csv);
        Assert.IsTrue(result.IsHeaderPresent);
        Assert.AreEqual(CsvOpts.Default, result.Options);
    }

    [TestMethod]
    public void HeaderTest5()
    {
        const string csv = """
            a,a,b
            1,2,3
            """;

        CsvAnalyzerResult result = Csv.AnalyzeString(csv, Header.Present);
        Assert.IsTrue(result.IsHeaderPresent);
        Assert.AreEqual(CsvOpts.Default, result.Options);
    }

    [TestMethod]
    public void HeaderTest6()
    {
        const string csv = """
            a,a,b
            1,2,3
            """;

        CsvAnalyzerResult result = Csv.AnalyzeString(csv);
        Assert.IsFalse(result.IsHeaderPresent);
        Assert.AreEqual(CsvOpts.Default, result.Options);
    }

    [TestMethod]
    public void DelimiterTest1()
    {
        const string csv = "a#b\r1#2";
        CsvAnalyzerResult result = Csv.AnalyzeString(csv);
        Assert.AreEqual('#', result.Delimiter);
    }

    [TestMethod]
    public void DelimiterTest2()
    {
        const string csv = "a\tb\r1\t2";
        CsvAnalyzerResult result = Csv.AnalyzeString(csv);
        Assert.AreEqual('\t', result.Delimiter);
    }

    [TestMethod]
    public void DelimiterTest3()
    {
        const string csv = """
                  a,b
                  1,2,3
                  """;
        CsvAnalyzerResult result = Csv.AnalyzeString(csv);
        Assert.AreEqual(',', result.Delimiter);
    }

    [TestMethod]
    public void DelimiterTest4()
    {
        const string csv = """
                  a#b
                  1#2#3
                  """;
        CsvAnalyzerResult result = Csv.AnalyzeString(csv);
        Assert.AreEqual('#', result.Delimiter);
    }

    [TestMethod]
    public void DelimiterTest5()
    {
        const string csv = "a\tb\n1\t2\t3";
        CsvAnalyzerResult result = Csv.AnalyzeString(csv);
        Assert.AreEqual('\t', result.Delimiter);
    }

    [TestMethod]
    public void DelimiterTest6()
    {
        const string csv = """
                  a b
                  1 2 3
                  """;
        CsvAnalyzerResult result = Csv.AnalyzeString(csv);
        Assert.AreEqual(' ', result.Delimiter);
    }

    [TestMethod]
    public void DelimiterTest7()
    {
        const string csv = """
                  a b c
                  1 2 3
                  """;
        CsvAnalyzerResult result = Csv.AnalyzeString(csv);
        Assert.AreEqual(' ', result.Delimiter);
    }
}