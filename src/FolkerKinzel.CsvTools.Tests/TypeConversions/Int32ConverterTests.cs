using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.CsvTools.TypeConversions.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Tests
{
    [TestClass()]
    public class Int32ConverterTests
    {
        [TestMethod()]
        public void Int32ConverterTest()
        {
            const int val = -4711;
            var conv = new Int32Converter();

            string? s = conv.ConvertToString(val);

            Assert.IsNotNull(s);

            int res = conv.Parse(s);

            Assert.AreEqual(val, res);
        }


        [TestMethod()]
        public void Int32ConverterTest_Hex()
        {
            const int val = -4711;
            var conv = new Int32Converter(formatProvider: CultureInfo.CreateSpecificCulture("de-DE"));

            string? s = conv.ConvertToString(val);

            Assert.IsNotNull(s);

            int res = conv.Parse(s);

            Assert.AreEqual(val, res);
        }

        
    }
}