using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Specialized.Tests
{
    [TestClass()]
    public class TimeSpanConverterTests
    {
        [TestMethod()]
        public void TimeSpanConverterTest1()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.TimeSpan);
            Assert.IsInstanceOfType(conv, typeof(TimeSpanConverter));
        }

        [DataTestMethod()]
        [DataRow(null)]
        [DataRow("")]
        public void TimeSpanConverterTest2(string? format)
        {
            var conv = new TimeSpanConverter(format);
            Assert.IsInstanceOfType(conv, typeof(TimeSpanConverter));
        }

        [DataTestMethod()]
        [DataRow(null)]
        [DataRow("")]
        [ExpectedException(typeof(ArgumentException))]
        public void TimeSpanConverterTest3(string? format)
        {
            _ = new TimeSpanConverter(format, parseExact: true);
        }

        [TestMethod()]
        public void ParseTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ConvertToStringTest()
        {
            Assert.Fail();
        }
    }
}