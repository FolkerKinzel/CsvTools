using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using FolkerKinzel.Strings;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass]
public class CsvReaderTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod()]
    public void ReadTest1()
    {
        const string testCsv =
            "Spalte \"1\",," + "\r\n" +
            ",Spalte \"2\",";

        CsvRecord[] csv = Csv.Parse(testCsv, isHeaderPresent: false);
        Assert.AreEqual(2, csv.Length);
    }

    [TestMethod()]
    public void ReadTest2()
    {
        string outDir = Path.Combine(TestContext.TestRunResultsDirectory!, "CsvFilesAnalyzed");
        _ = Directory.CreateDirectory(outDir);

        string file = TestFiles.GoogleCsv;
        using CsvReader csv = Csv.OpenRead(file, options: CsvOpts.None);

        foreach (CsvRecord record in csv)
        {
            var sb = new StringBuilder();

            foreach (KeyValuePair<string, ReadOnlyMemory<char>> item in record)
            {
                _ = sb.Append(item.Key.PadRight(20)).Append(": ").Append(item.Value.Span).AppendLine();
            }

            File.WriteAllText(Path.Combine(outDir, Path.GetFileName(file) + ".txt"), sb.ToString());

            break;
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void ReadTest3()
    {
        const string testCsv =
            "Spalte \"1\",," + "\r\n" +
            ",Spalte \"2\",";

        using var stringReader = new StringReader(testCsv);
        using CsvReader csv = Csv.OpenRead(stringReader, isHeaderPresent: false);

        stringReader.Dispose();

        foreach (CsvRecord _ in csv)
        {

        }
    }


    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void ReadTest4()
    {
        const string testCsv =
            "Spalte \"1\",," + "\r\n" +
            ",Spalte \"2\",";

        using var stringReader = new StringReader(testCsv);
        using var csv = new CsvReader(stringReader, isHeaderPresent: false);

        _ = csv.FirstOrDefault();
        _ = csv.FirstOrDefault();
    }

    [TestMethod()]
    public void ReadTest5()
    {
        const string testCsv =
            "Spalte \"1\",," + "\r\n" +
            ",Spalte \"2\"," + "\r\n";

        using var stringReader = new StringReader(testCsv);
        CsvRecord[] csv = Csv.Parse(testCsv, isHeaderPresent: false);

        Assert.AreEqual(2, csv.Length);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CsvReaderTest3()
    {
        using var _ = new CsvReader((string?)null!);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvReaderTest4()
    {
        using var _ = new CsvReader("   ");
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CsvReaderTest5()
    {
        using var _ = new CsvReader((StreamReader?)null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void CsvReaderTest6()
    {
        const string csv = """
                Name,City
                Ingrid,Berlin
                Joyce,New York
                Horst,Hamburg
                John,New York
                """;

        using var csvReader = new CsvReader(new StringReader(csv));

        int cnt = 0;

        foreach (CsvRecord record in csvReader)
        {
            cnt++;
        }

        Assert.AreEqual(4, cnt);

        foreach (CsvRecord _ in csvReader)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void CsvReaderTest7()
    {
        const string csv = """
                Name,City
                Ingrid,Berlin
                Joyce,New York
                Horst,Hamburg
                John,New York
                """;

        using var csvReader = new CsvReader(new StringReader(csv));

        Assert.AreEqual(4, csvReader.Count());

        _ = csvReader.Count();
    }

    [TestMethod]
    public void MyTestMethod()
    {
        const string csv = """
                Name,City
                Ingrid,Berlin
                Joyce,New York
                Horst,Hamburg
                John,New York
                """;

        using var csvReader = new CsvReader(new StringReader(csv));

        Assert.AreEqual(4, csvReader.Count());
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void ResetTest1()
    {
        using var reader = new CsvReader("Test");
        ((IEnumerator)reader).Reset();
    }

    [TestMethod]
    public void IEnumerableTest()
    {
        const string csv = "a,b,c";
        using var stringReader = new StringReader(csv);
        using var reader = new CsvReader(stringReader, isHeaderPresent: false);

        IEnumerator enumerator = ((IEnumerable)reader).GetEnumerator();
        Assert.AreSame(reader, enumerator);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.IsNotNull(enumerator.Current);
        Assert.AreEqual(',', reader.Delimiter);
    }

    [TestMethod]
    [ExpectedException(typeof(CsvFormatException))]
    public void TooMuchFieldsTest()
    {
        const string csv = """
            a,b
            1,2,3
            """;
        _ = Csv.Parse(csv);
    }

    [TestMethod]
    [ExpectedException(typeof(CsvFormatException))]
    public void EmptyLineTest1()
    {
        const string csv = """
            a,b

            1,2
            """;
        _ = Csv.Parse(csv);
    }

    [TestMethod]
    public void EmptyLineTest2()
    {
        const string csv = """
            a,b

            1,2
            """;

        CsvRecord[] result = Csv.Parse(csv, options: CsvOpts.Default.Unset(CsvOpts.ThrowOnEmptyLines));
        Assert.AreEqual(2, result.Length);
    }

    [TestMethod]
    [ExpectedException(typeof(CsvFormatException))]
    public void TooFewFieldsTest()
    {
        const string csv = """
            a,b,c
            1,2
            """;
        _ = Csv.Parse(csv);
    }
}
