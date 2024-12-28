﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
public class CsvEnumeratorTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod()]
    public void ReadTest1()
    {
        const string testCsv =
            "Spalte \"1\",," + "\r\n" +
            ",Spalte \"2\",";

        using var stringReader = new StringReader(testCsv);
        using var csv = new CsvEnumerator(stringReader, hasHeaderRow: false);

        int counter = 0;
        foreach (CsvRecord record in csv)
        {
            counter++;
        }

        Assert.AreEqual(2, counter);
    }


    [TestMethod()]
    public void ReadTest2()
    {
        string outDir = Path.Combine(TestContext.TestRunResultsDirectory!, "CsvFilesAnalyzed");
        _ = Directory.CreateDirectory(outDir);

        string file = TestFiles.GoogleCsv;
        using var csv = new CsvEnumerator(file, options: CsvOpts.None);

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
        using var csv = new CsvEnumerator(stringReader, hasHeaderRow: false);

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
        using var csv = new CsvEnumerator(stringReader, hasHeaderRow: false);

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
        using var csv = new CsvEnumerator(stringReader, hasHeaderRow: false);

        IEnumerable numerable = csv;
        int counter = 0;

        foreach (object? record in numerable)
        {
            counter++;
        }

        Assert.AreEqual(2, counter);
    }


    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CsvReaderTest3()
    {
        using var _ = new CsvEnumerator((string?)null!);
    }


    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvReaderTest4()
    {
        using var _ = new CsvEnumerator("   ");
    }


    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CsvReaderTest5()
    {
        using var _ = new CsvEnumerator((StreamReader?)null!);
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

        using var csvReader = new CsvEnumerator(new StringReader(csv));

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

        using var csvReader = new CsvEnumerator(new StringReader(csv));

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

        using var csvReader = new CsvEnumerator(new StringReader(csv));

        Assert.AreEqual(4, csvReader.Count());
    }
}