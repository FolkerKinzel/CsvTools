using System;
using System.Linq;
using FolkerKinzel.CsvTools.Extensions;
using FolkerKinzel.CsvTools.TypeConversions;
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
            Assert.IsInstanceOfType(analyzer, typeof(CsvAnalyzer));
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
            CollectionAssert.AreEqual(analyzer.ColumnNames, new string[] { "Eins", "eins", "zwei", "drei", "" });

            Assert.IsTrue(analyzer.Options.IsSet(CsvOptions.CaseSensitiveKeys));
            Assert.IsTrue(analyzer.Options.IsSet(CsvOptions.TrimColumns));
            Assert.IsFalse(analyzer.Options.IsSet(CsvOptions.ThrowOnTooFewFields));
            Assert.IsFalse(analyzer.Options.IsSet(CsvOptions.ThrowOnTooMuchFields));
            Assert.IsFalse(analyzer.Options.IsSet(CsvOptions.DisableCaching));
            Assert.IsFalse(analyzer.Options.IsSet(CsvOptions.ThrowOnEmptyLines));
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