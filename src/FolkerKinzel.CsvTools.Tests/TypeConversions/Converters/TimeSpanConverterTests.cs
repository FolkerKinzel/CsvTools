using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Tests
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
        public void TimeSpanConverterTest3(string? format) => _ = new TimeSpanConverter(format, parseExact: true);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TimeSpanConverterTest4() => _ = new TimeSpanConverter("bla");

        [TestMethod()]
        public void TimeSpanConverterTest5()
        {
            var conv = new TimeSpanConverter("G");
            Assert.IsInstanceOfType(conv, typeof(TimeSpanConverter));
        }

        [TestMethod()]
        public void Roundtrip1()
        {
            TimeSpan now = DateTime.UtcNow.TimeOfDay;

            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.TimeSpan);

            string? tmp = conv.ConvertToString(now);

            Assert.IsNotNull(tmp);

            var now2 = (TimeSpan?)conv.Parse(tmp);

            Assert.AreEqual(now, now2);
        }

        [TestMethod()]
        public void Roundtrip2()
        {
            TimeSpan now = DateTime.UtcNow.TimeOfDay;

            var conv = new TimeSpanConverter("G");

            string? tmp = conv.ConvertToString(now);

            Assert.IsNotNull(tmp);

            var now2 = (TimeSpan?)conv.Parse(tmp);

            Assert.AreEqual(now, now2);
        }
    }
}