using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.CsvTools.Helpers.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Specialized.Tests
{
    [TestClass()]
    public class DateTimeOffsetConverterTests
    {
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