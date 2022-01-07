using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Tests
{
    [TestClass()]
    public class DateTimeConverterTests
    {
        [TestMethod()]
        public void DateTimeConverterTest1()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTime);
            Assert.IsInstanceOfType(conv, typeof(DateTimeConverter));
        }

        [TestMethod()]
        public void DateTimeConverterTest2()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.Date);
            Assert.IsInstanceOfType(conv, typeof(DateTimeConverter));
        }

        [DataTestMethod()]
        [DataRow(null)]
        [DataRow("")]
        public void DateTimeConverterTest3(string? format)
        {
            var conv = new DateTimeConverter(format);
            Assert.IsInstanceOfType(conv, typeof(DateTimeConverter));
        }

        [DataTestMethod()]
        [DataRow(null)]
        [DataRow("")]
        [ExpectedException(typeof(ArgumentException))]
        public void DateTimeConverterTest4(string? format) => _ = new DateTimeConverter(format, parseExact: true);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void DateTimeConverterTest5()
            => _ = new DateTimeConverter("Ä");

        [TestMethod()]
        public void DateTimeConverterTest6()
        {
            var conv = new DateTimeConverter("D");
            Assert.IsInstanceOfType(conv, typeof(DateTimeConverter));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DateTimeConverterTest7() => _ = new DateTimeConverter(null!, parseExact: true);


        [TestMethod()]
        public void Roundtrip1()
        {
            var now = new DateTime(2021, 3, 1, 17, 25, 38, DateTimeKind.Unspecified);

            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTime);

            string? tmp = conv.ConvertToString(now);

            Assert.IsNotNull(tmp);

            var now2 = (DateTime?)conv.Parse(tmp);


            Assert.AreEqual(now, now2);
        }


        [TestMethod()]
        public void Roundtrip2()
        {
            DateTime now = DateTime.UtcNow;

            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);

            var conv = new DateTimeConverter("F");

            string? tmp = conv.ConvertToString(now);

            Assert.IsNotNull(tmp);

            var now2 = (DateTime?)conv.Parse(tmp);

            Assert.AreEqual(now, now2);
        }


        [DataTestMethod()]
        [DataRow("1972-01-31")]
        [DataRow("1972/01/31")]
        public void ParseTest(string s)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTime);

            var dt = conv.Parse(s);

            Assert.AreEqual(new DateTime(1972, 01, 31), dt);
        }

        [TestMethod()]
        public void ConvertToStringTest1()
        {
            var dt = new DateTime(1972, 01, 31);
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.Date);

            //dt = dt.ToLocalTime();

            string? s = conv.ConvertToString(dt);

            Assert.IsNotNull(s);
        }


        [TestMethod()]
        public void ConvertToStringTest2()
        {
            var dt = new DateTime(1972, 01, 31);
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTime);

            //dt = dt.ToLocalTime();

            string? s = conv.ConvertToString(dt);

            Assert.IsNotNull(s);
        }
    }
}