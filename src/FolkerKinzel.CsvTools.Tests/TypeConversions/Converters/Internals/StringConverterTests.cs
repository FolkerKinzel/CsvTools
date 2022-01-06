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
            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateConverter(CsvTypeCode.String);

            Assert.IsNotNull(conv.Parse(null));
            Assert.AreEqual(typeof(StringConverter2), conv.GetType());
        }

        [TestMethod()]
        public void ParseTest()
        {
            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateConverter(CsvTypeCode.String, CsvConverterOptions.Nullable);

            Assert.IsNull(conv.Parse(null));

            conv = CsvConverterFactory2.CreateConverter(CsvTypeCode.String);

            Assert.IsNotNull(conv.Parse(null));

            const string test = "Test";

            Assert.AreEqual(test, conv.Parse(test));

        }

        [TestMethod()]
        public void ConvertToStringTest()
        {
            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateConverter(CsvTypeCode.String, CsvConverterOptions.Nullable | CsvConverterOptions.AcceptsDBNull);

            Assert.IsNull(conv.ConvertToString(DBNull.Value));

            Assert.IsNull(conv.ConvertToString(null));

            const string test = "Test";

            Assert.AreEqual(test, conv.ConvertToString(test));
        }

        [ExpectedException(typeof(InvalidCastException))]
        [TestMethod()]
        public void ConvertToStringTest_ThrowOnInvalidType()
        {
            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateConverter(CsvTypeCode.String);

            _ = conv.ConvertToString(4711);

        }


        [ExpectedException(typeof(InvalidCastException))]
        [TestMethod()]
        public void ConvertToStringTest_ThrowOnDBNull()
        {
            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateConverter(CsvTypeCode.String);

            _ = conv.ConvertToString(DBNull.Value);
        }
    }
}