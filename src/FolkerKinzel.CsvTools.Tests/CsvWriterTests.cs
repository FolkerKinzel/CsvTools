using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace FolkerKinzel.CsvTools.Tests
{
    [TestClass()]
    public class CsvWriterTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod()]
        public void CsvWriterTest()
        {
            using var writer = new CsvWriter("Test", 0);
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
            using var writer = new CsvWriter("Test", ["1", "2"]);
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
            using var writer = new CsvWriter(textWriter, ["1", "2"]);

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
            using var writer = new CsvWriter(textWriter, 0);

            Assert.IsNotNull(writer);
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
            using var reader = new CsvEnumerator(FILENAME_STANDARD);

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

            using var reader = new CsvEnumerator(FILENAME_STANDARD, hasHeaderRow: false);

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
    }
}