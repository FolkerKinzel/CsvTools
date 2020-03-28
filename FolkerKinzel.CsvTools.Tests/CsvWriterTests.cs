using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.IO;

namespace FolkerKinzel.CsvTools.Tests
{
    [TestClass()]
    public class CsvWriterTests
    {

#pragma warning disable CS8618 // Das Non-Nullable-Feld ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".
        public TestContext TestContext {get; set;}
#pragma warning restore CS8618 // Das Non-Nullable-Feld ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".


        [TestMethod()]
        public void CsvWriterTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CsvWriterTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CsvWriterTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CsvWriterTest3()
        {
            Assert.Fail();
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


            Assert.AreEqual(VALUE1, reader.Read().First()["VALUE1"]);
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

            using var reader = new CsvReader(FILENAME_STANDARD);


            Assert.AreEqual(VALUE1, reader.Read().First()[0]);
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