using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.CsvTools.Helpers.Converters.Specialized;
using System;
using System.Collections.Generic;
using System.Text;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Specialized.Tests
{
    [TestClass()]
    public class DateTimeConverterTests
    {
        [TestMethod()]
        public void DateTimeConverterTest()
        {
            Assert.Fail();
        }

        [DataTestMethod()]
        [DataRow("1972-01-31")]
        [DataRow("1972/01/31")]
        public void ParseTest(string s)
        {
            var conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTime);

            var dt = conv.Parse(s);

            Assert.AreEqual(new DateTime(1972, 01, 31), dt);
        }

        [TestMethod()]
        public void ConvertToStringTest1()
        {
            var dt = new DateTime(1972, 01, 31);
            var conv = CsvConverterFactory.CreateConverter(CsvTypeCode.Date);

            //dt = dt.ToLocalTime();

            string? s = conv.ConvertToString(dt);

            Assert.IsNotNull(s);
        }


        [TestMethod()]
        public void ConvertToStringTest2()
        {
            var dt = new DateTime(1972, 01, 31);
            var conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTime);

            //dt = dt.ToLocalTime();

            string? s = conv.ConvertToString(dt);

            Assert.IsNotNull(s);
        }
    }
}