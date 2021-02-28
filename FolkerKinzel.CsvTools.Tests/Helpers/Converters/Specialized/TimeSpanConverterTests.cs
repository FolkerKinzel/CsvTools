using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Specialized.Tests
{
    [TestClass()]
    public class TimeSpanConverterTests
    {
        [TestMethod()]
        public void TimeSpanConverterTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ParseTest()
        {
            //Assert.Fail();

            string s = DateTime.Now.TimeOfDay.ToString(null, CultureInfo.InvariantCulture);

            TimeSpan ts = TimeSpan.ParseExact(s, (string?)null!, CultureInfo.InvariantCulture);
        }

        [TestMethod()]
        public void ConvertToStringTest()
        {
            Assert.Fail();
        }
    }
}