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
            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateConverter(CsvTypeCode.TimeSpan);
            Assert.IsInstanceOfType(conv, typeof(TimeSpanConverter2));
        }

        [TestMethod()]
        public void TimeSpanConverterTest2()
        {
            var conv = new TimeSpanConverter2("");
            Assert.IsInstanceOfType(conv, typeof(TimeSpanConverter2));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TimeSpanConverterTest3() => _ = new TimeSpanConverter2("", parseExact: true);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void TimeSpanConverterTest4() => _ = new TimeSpanConverter2("bla");

        [TestMethod()]
        public void TimeSpanConverterTest5()
        {
            var conv = new TimeSpanConverter2("G");
            Assert.IsInstanceOfType(conv, typeof(TimeSpanConverter2));
        }

        [TestMethod()]
        public void Roundtrip1()
        {
            TimeSpan now = DateTime.UtcNow.TimeOfDay;

            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateConverter(CsvTypeCode.TimeSpan);

            string? tmp = conv.ConvertToString(now);

            Assert.IsNotNull(tmp);

            var now2 = (TimeSpan?)conv.Parse(tmp);

            Assert.AreEqual(now, now2);
        }

        [TestMethod()]
        public void Roundtrip2()
        {
            TimeSpan now = DateTime.UtcNow.TimeOfDay;

            var conv = new TimeSpanConverter2("G");

            string? tmp = conv.ConvertToString(now);

            Assert.IsNotNull(tmp);

            var now2 = (TimeSpan?)conv.Parse(tmp);

            Assert.AreEqual(now, now2);
        }
    }
}