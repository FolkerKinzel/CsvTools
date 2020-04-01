using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Specialized.Tests
{
    [TestClass()]
    public class DateTimeOffsetConverterTests
    {
        [TestMethod()]
        public void DateTimeOffsetConverterTest0()
        {
            var conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTimeOffset);

            var dt = new DateTime(1972, 01, 31);

            string? s = conv.ConvertToString(new DateTimeOffset(dt));

            Assert.IsNotNull(s);

            var dto = (DateTimeOffset)conv.Parse(s)!;

            Assert.AreEqual(dt, dto.DateTime);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DateTimeOffsetConverterTest1()
        {
            _ = new DateTimeOffsetConverter(null!);
        }

        [TestMethod()]
        public void DateTimeOffsetConverterTest2()
        {
            _ = new DateTimeOffsetConverter("");
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