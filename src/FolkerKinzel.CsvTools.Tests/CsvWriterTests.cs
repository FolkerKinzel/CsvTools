﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass()]
public class CsvWriterTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod()]
    public void CsvWriterTest()
    {
        using CsvWriter writer = Csv.OpenWrite("Test", 0);
        Assert.IsNotNull(writer);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CsvWriterTest1()
    {
        using var _ = new CsvWriter((string?)null!, 0);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvWriterTest2()
    {
        using var _ = new CsvWriter("  ", 0);
    }

    [TestMethod()]
    public void CsvWriterTest3()
    {
        using CsvWriter writer = Csv.OpenWrite("Test", ["1", "2"]);
        Assert.IsNotNull(writer);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvWriterTest4()
    {
        using var _ = new CsvWriter("  ", ["1", "2"]);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvWriterTest5()
    {
        using var _ = new CsvWriter("Test", ["1", "1"]);
    }

    [TestMethod()]
    public void CsvWriterTest6()
    {
        using var textWriter = new StringWriter();
        using CsvWriter writer = Csv.OpenWrite(textWriter, ["1", "2"]);

        Assert.IsNotNull(writer);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void CsvWriterTest7()
    {
        using var textWriter = new StringWriter();
        using var _ = new CsvWriter(textWriter, ["1", "1"]);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CsvWriterTest8()
    {
        using var _ = new CsvWriter((TextWriter?)null!, ["1", "2"]);
    }

    [TestMethod()]
    public void CsvWriterTest9()
    {
        using var textWriter = new StringWriter();
        using CsvWriter writer = Csv.OpenWrite(textWriter, 0);

        Assert.IsNotNull(writer);
    }

    [DataTestMethod]
    [DataRow(',')]
    [DataRow(';')]
    [DataRow('#')]
    [DataRow('\t')]
    [DataRow(' ')]
    public void CsvWriterTest10(char delimiter)
    {
        string fileName = Path.Combine(TestContext.TestRunResultsDirectory!, "CsvWriterTest10.csv");
        using var writer = new CsvWriter(fileName, [], delimiter: delimiter);
    }

    [DataTestMethod]
    [DataRow('\"')]
    [DataRow('\r')]
    [DataRow('\n')]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CsvWriterTest11(char delimiter)
    {
        string fileName = Path.Combine(TestContext.TestRunResultsDirectory!, "CsvWriterTest11.csv");
        using var writer = new CsvWriter(fileName, [], delimiter: delimiter);
    }

    [DataTestMethod]
    [DataRow(',')]
    [DataRow(';')]
    [DataRow('#')]
    [DataRow('\t')]
    [DataRow(' ')]
    public void CsvWriterTest12(char delimiter)
    {
        string fileName = Path.Combine(TestContext.TestRunResultsDirectory!, "CsvWriterTest12.csv");
        using var writer = new CsvWriter(fileName, 2, delimiter: delimiter);
    }

    [DataTestMethod]
    [DataRow('\"')]
    [DataRow('\r')]
    [DataRow('\n')]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CsvWriterTest13(char delimiter)
    {
        string fileName = Path.Combine(TestContext.TestRunResultsDirectory!, "CsvWriterTest13.csv");
        using var writer = new CsvWriter(fileName, 2, delimiter: delimiter);
    }

    [DataTestMethod]
    [DataRow(',')]
    [DataRow(';')]
    [DataRow('#')]
    [DataRow('\t')]
    [DataRow(' ')]
    public void CsvWriterTest14(char delimiter)
    {
        using var stringWriter = new StringWriter();
        using var writer = new CsvWriter(stringWriter, [], delimiter: delimiter);
    }

    [DataTestMethod]
    [DataRow('\"')]
    [DataRow('\r')]
    [DataRow('\n')]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CsvWriterTest15(char delimiter)
    {
        using var stringWriter = new StringWriter();
        using var writer = new CsvWriter(stringWriter, [], delimiter: delimiter);
    }

    [DataTestMethod]
    [DataRow(',')]
    [DataRow(';')]
    [DataRow('#')]
    [DataRow('\t')]
    [DataRow(' ')]
    public void CsvWriterTest16(char delimiter)
    {
        using var stringWriter = new StringWriter();
        using var writer = new CsvWriter(stringWriter, 2, delimiter: delimiter);
    }

    [DataTestMethod]
    [DataRow('\"')]
    [DataRow('\r')]
    [DataRow('\n')]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CsvWriterTest17(char delimiter)
    {
        using var stringWriter = new StringWriter();
        using var writer = new CsvWriter(stringWriter, 2, delimiter: delimiter);
    }

    [TestMethod()]
    public void WriteRecordTest1()
    {
        string VALUE1 = "Ein \"schönes\" Wochenende;" + Environment.NewLine + Environment.NewLine + "Zeile 3";
        string FILENAME_STANDARD = Path.Combine(TestContext.TestRunResultsDirectory!, @"StandardWithHeader.csv");

        const string Key1 = "Key1";
        const string Key2 = "Key2";

        using (var writer = new CsvWriter(FILENAME_STANDARD, [Key1, Key2]))
        {
            writer.Record[Key1] = VALUE1.AsMemory();

            writer.WriteRecord();

            writer.Record[Key1] = "Value1".AsMemory();
            writer.Record[Key2] = "Value2".AsMemory();

            writer.WriteRecord();
        }

        //string csv = File.ReadAllText(FILENAME_STANDARD);
        using var reader = new CsvReader(FILENAME_STANDARD);

        Assert.AreEqual(VALUE1, reader.First()[Key1].ToString());
    }

    /// <summary>
    /// Write CSV without Header.
    /// </summary>
    [TestMethod()]
    public void WriteRecordTest2()
    {
        string VALUE1 = "Ein \"schönes\" Wochenende;" + Environment.NewLine + Environment.NewLine + "Zeile 3";
        string FILENAME_STANDARD = Path.Combine(TestContext.TestRunResultsDirectory!, @"NoHeader.csv");

        using (var writer = new CsvWriter(FILENAME_STANDARD, 2))
        {
            writer.Record.Values[0] = VALUE1.AsMemory();

            writer.WriteRecord();

            writer.Record.Values[0] = "Value1".AsMemory();
            writer.Record.Values[1] = "Value2".AsMemory();

            writer.WriteRecord();
        }

        using var reader = new CsvReader(FILENAME_STANDARD, isHeaderPresent: false);

        Assert.AreEqual(VALUE1, reader.First().Values[0].ToString());
    }

    [TestMethod()]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void WriteRecordTest3()
    {
        using var writer = new CsvWriter("File", 2);

        writer.Dispose();
        writer.WriteRecord();
    }

    [TestMethod()]
    public void DisposeTest()
    {
        TestContext.WriteLine($"{nameof(TestContext.DeploymentDirectory)}:         {TestContext.DeploymentDirectory}");
        TestContext.WriteLine("");
        //TestContext.WriteLine($"{nameof(TestContext.TestDeploymentDir)}:           {TestContext.TestDeploymentDir}");
        //TestContext.WriteLine("");
        TestContext.WriteLine($"{nameof(TestContext.ResultsDirectory)}:            {TestContext.ResultsDirectory}");
        TestContext.WriteLine("");
        TestContext.WriteLine($"{nameof(TestContext.TestResultsDirectory)}:        {TestContext.TestResultsDirectory}");
        TestContext.WriteLine("");
        TestContext.WriteLine($"{nameof(TestContext.TestRunDirectory)}:            {TestContext.TestRunDirectory}");
        TestContext.WriteLine("");
        TestContext.WriteLine($"{nameof(TestContext.TestRunResultsDirectory)}:     {TestContext.TestRunResultsDirectory}");
    }

    [TestMethod]
    public void PullRequestTest()
    {
        const string VALUE1 = "1234";
        const string VALUE2 = "4567";
        const string VALUE3 = "\"DemoString\" Some more demo string";
        //string FILENAME_STANDARD = Path.Combine(TestContext.TestRunResultsDirectory!, @"NoHeader.csv");

        using var stringWriter = new StringWriter();
        using (var writer = new CsvWriter(stringWriter, 3, delimiter: '|'))
        {
            writer.Record[0] = VALUE1.AsMemory();
            writer.Record[1] = VALUE2.AsMemory();
            writer.Record[2] = VALUE3.AsMemory();

            writer.WriteRecord();
        }

        //const string csv = "1234|4567|\"DemoString\" Some more demo string|";

        using var stringReader = new StringReader(stringWriter.ToString());
        using var reader = new CsvReader(stringReader, delimiter: '|', isHeaderPresent: false);
        CsvRecord record = reader.First();
    }
}
