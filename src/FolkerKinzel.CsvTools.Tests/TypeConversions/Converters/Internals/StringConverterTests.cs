using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls.Tests
{
    [TestClass()]
    public class StringConverterTests
    {
        [TestMethod()]
        public void StringConverterTest()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.String, false);

            Assert.IsNotNull(conv.FallbackValue);
            Assert.AreEqual(typeof(string), conv.Type);
            Assert.IsFalse(conv.ThrowsOnParseErrors);
        }

        [TestMethod()]
        public void ParseTest()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.String, true);

            Assert.IsNull(conv.Parse(null));

            conv = CsvConverterFactory.CreateConverter(CsvTypeCode.String, false);

            Assert.IsNotNull(conv.Parse(null));

            const string test = "Test";

            Assert.AreEqual(test, conv.Parse(test));

        }

        [TestMethod()]
        public void ConvertToStringTest()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.String, true, true);

            Assert.IsNull(conv.ConvertToString(DBNull.Value));

            Assert.IsNull(conv.ConvertToString(null));

            const string test = "Test";

            Assert.AreEqual(test, conv.ConvertToString(test));
        }

        [ExpectedException(typeof(InvalidCastException))]
        [TestMethod()]
        public void ConvertToStringTest_ThrowOnInvalidType()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.String);

            _ = conv.ConvertToString(4711);

        }


        [ExpectedException(typeof(InvalidCastException))]
        [TestMethod()]
        public void ConvertToStringTest_ThrowOnDBNull()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.String);

            _ = conv.ConvertToString(DBNull.Value);
        }
    }
}