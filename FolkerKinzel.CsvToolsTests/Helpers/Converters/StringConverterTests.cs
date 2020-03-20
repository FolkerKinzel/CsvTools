using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.CsvTools.Helpers.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Tests
{
    [TestClass()]
    public class StringConverterTests
    {
        [TestMethod()]
        public void StringConverterTest()
        {
            var conv = new StringConverter(false);

            Assert.IsNotNull(conv.FallbackValue);
            Assert.AreEqual(typeof(string), conv.Type);
            Assert.IsFalse(conv.ThrowsOnParseErrors);
        }

        [TestMethod()]
        public void ParseTest()
        {
            var conv = new StringConverter();

            Assert.IsNull(conv.Parse(null));

            conv = new StringConverter(false);

            Assert.IsNotNull(conv.Parse(null));

            const string test = "Test";

            Assert.AreEqual(test, conv.Parse(test));

        }

        [TestMethod()]
        public void ConvertToStringTest()
        {
            var conv = new StringConverter();

            Assert.IsNull(conv.ConvertToString(DBNull.Value));

            Assert.IsNull(conv.ConvertToString(null));

            const string test = "Test";

            Assert.AreEqual(test, conv.ConvertToString(test));
        }

        [ExpectedException(typeof(InvalidCastException))]
        [TestMethod()]
        public void ConvertToStringTest_ThrowOnInvalidType()
        {
            var conv = new StringConverter();

            conv.ConvertToString(4711);

        }
    }
}