using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Tests
{
    [TestClass()]
    public class DateTimeOffsetConverterTests
    {
        [TestMethod()]
        public void DateTimeOffsetConverterTest0()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTimeOffset);

            Assert.IsInstanceOfType(conv, typeof(DateTimeOffsetConverter));

            var dt = new DateTime(1975, 07, 14);

            string? s = conv.ConvertToString(new DateTimeOffset(dt));

            Assert.IsNotNull(s);

            var dto = (DateTimeOffset)conv.Parse(s)!;

            Assert.AreEqual(dt, dto.DateTime);
        }


        [DataTestMethod()]
        [DataRow(null)]
        [DataRow("")]
        public void DateTimeOffsetConverterTest3(string? format)
        {
            var conv = new DateTimeOffsetConverter(format);
            Assert.IsInstanceOfType(conv, typeof(DateTimeOffsetConverter));
        }

        [DataTestMethod()]
        [DataRow(null)]
        [DataRow("")]
        [ExpectedException(typeof(ArgumentException))]
        public void DateTimeOffsetConverterTest4(string? format) => _ = new DateTimeOffsetConverter(format, parseExact: true);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void DateTimeOffsetConverterTest5()
            => _ = new DateTimeOffsetConverter("Ä");

        [TestMethod()]
        public void DateTimeOffsetConverterTest6()
        {
            var conv = new DateTimeOffsetConverter("D");
            Assert.IsInstanceOfType(conv, typeof(DateTimeOffsetConverter));
        }
        

        [TestMethod()]
        public void Roundtrip1()
        {
            DateTimeOffset now = new DateTime(2021, 3, 1, 17, 25, 38, DateTimeKind.Local);

            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTimeOffset);

            string? tmp = conv.ConvertToString(now);

            Assert.IsNotNull(tmp);

            var now2 = (DateTimeOffset?)conv.Parse(tmp);


            Assert.AreEqual(now, now2);
        }


        [TestMethod()]
        public void Roundtrip2()
        {
            DateTime tmp = DateTime.UtcNow;
            DateTimeOffset now = new DateTime(tmp.Year, tmp.Month, tmp.Day, tmp.Hour, tmp.Minute, tmp.Second, DateTimeKind.Utc);


            var conv = new DateTimeOffsetConverter("R");

            string? tmp1 = conv.ConvertToString(now);

            Assert.IsNotNull(tmp1);

            var now2 = (DateTimeOffset?)conv.Parse(tmp1);

            Assert.AreEqual(now, now2);
        }
    }
}