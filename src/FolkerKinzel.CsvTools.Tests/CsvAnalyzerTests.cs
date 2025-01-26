using System;
using System.Linq;
using System.Text;
using FolkerKinzel.CsvTools.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Tests
{
    [TestClass()]
    public class CsvAnalyzerTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AnalyzeTest1() => CsvAnalyzer.Analyze(null!);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AnalyzeTest2() => CsvAnalyzer.Analyze("  ");

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AnalyzeTest2b() => CsvAnalyzer.Analyze("Test", header: (Header)4711);

        [TestMethod()]
        public void AnalyzeTest3()
        {
            (CsvAnalyzerResult results, Encoding enc) = Csv.Analyze(TestFiles.AnalyzerTestCsv);
            AssertAnalyzerTestCsv(results);
            Assert.IsNotNull(enc);
        }

        [TestMethod]
        public void AnalyzeTest4()
        {
            CsvAnalyzerResult result = CsvAnalyzer.Analyze(TestFiles.AnalyzerTestCsv, analyzedLinesCount: -42);
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
        public void AnalyzeTest5()
        {
            CsvAnalyzerResult result = CsvAnalyzer.Analyze(TestFiles.GoogleCsv);

            Assert.IsTrue(result.IsHeaderPresent);
            Assert.AreEqual(',', result.Delimiter);

            Assert.AreEqual(result.Options, CsvOpts.Default);
        }
    }
}