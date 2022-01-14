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
            Assert.IsInstanceOfType(writer, typeof(CsvWriter));
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
            using var writer = new CsvWriter("Test", new string[] { "1", "2" });
            Assert.IsInstanceOfType(writer, typeof(CsvWriter));
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CsvWriterTest4()
        {
            using var _ = new CsvWriter("  ", new string[] { "1", "2" });
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CsvWriterTest5()
        {
            using var _ = new CsvWriter("Test", new string[] { "1", "1" });
        }

        [TestMethod()]
        public void CsvWriterTest6()
        {
            using var textWriter = new StringWriter();
            using var writer = new CsvWriter(textWriter, new string[] { "1", "2" });

            Assert.IsInstanceOfType(writer, typeof(CsvWriter));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CsvWriterTest7()
        {
            using var textWriter = new StringWriter();
            using var _ = new CsvWriter(textWriter, new string[] { "1", "1" });
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvWriterTest8()
        {
            using var _ = new CsvWriter((TextWriter?)null!, new string[] { "1", "2" });
        }

        [TestMethod()]
        public void CsvWriterTest9()
        {
            using var textWriter = new StringWriter();
            using var writer = new CsvWriter(textWriter, 0);

            Assert.IsInstanceOfType(writer, typeof(CsvWriter));
        }


        [TestMethod()]
        public void WriteRecordTest1()
        {
            string VALUE1 = "Ein \"schönes\" Wochenende;" + Environment.NewLine + Environment.NewLine + "Zeile 3";
            string FILENAME_STANDARD = Path.Combine(TestContext.TestRunResultsDirectory, @"StandardWithHeader.csv");

            const string Key1 = "Key1";
            const string Key2 = "Key2";

            using (var writer = new CsvWriter(FILENAME_STANDARD, new string[] { Key1, Key2 }))
            {
                writer.Record[Key1] = VALUE1;

                writer.WriteRecord();

                writer.Record[Key1] = "Value1";
                writer.Record[Key2] = "Value2";

                writer.WriteRecord();
            }

            using var reader = new CsvReader(FILENAME_STANDARD);

            Assert.AreEqual(VALUE1, reader.First()[Key1]);
        }


        /// <summary>
        /// Write CSV without Header.
        /// </summary>
        [TestMethod()]
        public void WriteRecordTest2()
        {
            string VALUE1 = "Ein \"schönes\" Wochenende;" + Environment.NewLine + Environment.NewLine + "Zeile 3";
            string FILENAME_STANDARD = Path.Combine(TestContext.TestRunResultsDirectory, @"NoHeader.csv");

            using (var writer = new CsvWriter(FILENAME_STANDARD, 2))
            {
                writer.Record[0] = VALUE1;

                writer.WriteRecord();

                writer.Record[0] = "Value1";
                writer.Record[1] = "Value2";

                writer.WriteRecord();
            }

            using var reader = new CsvReader(FILENAME_STANDARD, hasHeaderRow: false);

            Assert.AreEqual(VALUE1, reader.First()[0]);
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