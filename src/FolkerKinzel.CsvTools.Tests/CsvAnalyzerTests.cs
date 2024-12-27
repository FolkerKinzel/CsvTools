using System;
using System.Linq;
using FolkerKinzel.CsvTools.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Tests
{
    [TestClass()]
    public class CsvAnalyzerTests
    {
        [TestMethod()]
        public void CsvAnalyzerTest()
        {
            var analyzer = new CsvAnalyzer();
            Assert.IsNotNull(analyzer);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AnalyzeTest1()
        {
            var analyzer = new CsvAnalyzer();
            analyzer.Analyze(null!);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AnalyzeTest2()
        {
            var analyzer = new CsvAnalyzer();
            analyzer.Analyze("  ");
        }

        [TestMethod()]
        public void AnalyzeTest3()
        {
            var analyzer = new CsvAnalyzer();
            analyzer.Analyze(TestFiles.AnalyzerTestCsv);

            Assert.IsTrue(analyzer.HasHeaderRow);
            Assert.AreEqual(';', analyzer.FieldSeparator);
            CollectionAssert.AreEqual(analyzer.ColumnNames?.ToArray(), new string[] { "Eins", "eins", "zwei", "drei"});

            Assert.IsTrue(analyzer.Options.HasFlag(CsvOptions.CaseSensitiveKeys));
            Assert.IsTrue(analyzer.Options.HasFlag(CsvOptions.TrimColumns));
            Assert.IsFalse(analyzer.Options.HasFlag(CsvOptions.ThrowOnTooFewFields));
            Assert.IsFalse(analyzer.Options.HasFlag(CsvOptions.ThrowOnTooMuchFields));
            Assert.IsFalse(analyzer.Options.HasFlag(CsvOptions.DisableCaching));
            Assert.IsFalse(analyzer.Options.HasFlag(CsvOptions.ThrowOnEmptyLines));
        }

        [TestMethod()]
        public void AnalyzeTest4()
        {
            var analyzer = new CsvAnalyzer();
            analyzer.Analyze(TestFiles.GoogleCsv);

            Assert.IsTrue(analyzer.HasHeaderRow);
            Assert.AreEqual(',', analyzer.FieldSeparator);

            Assert.AreEqual(analyzer.Options, CsvOptions.Default);
        }
    }
}