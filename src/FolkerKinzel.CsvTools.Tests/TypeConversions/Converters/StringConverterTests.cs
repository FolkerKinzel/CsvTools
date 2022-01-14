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
            ICsvTypeConverter conv = new StringConverter(false);

            Assert.IsNotNull(conv.Parse(null));
            Assert.AreEqual(typeof(StringConverter), conv.GetType());
        }

        [TestMethod()]
        public void ParseTest()
        {
            ICsvTypeConverter conv = new StringConverter();

            Assert.IsNull(conv.Parse(null));

            conv = new StringConverter(false);

            Assert.IsNotNull(conv.Parse(null));

            const string test = "Test";

            Assert.AreEqual(test, conv.Parse(test));

        }

        [TestMethod()]
        public void ConvertToStringTest()
        {
            ICsvTypeConverter conv = new StringConverter().AsDBNullEnabled();

            Assert.IsNull(conv.ConvertToString(DBNull.Value));

            Assert.IsNull(conv.ConvertToString(null));

            const string test = "Test";

            Assert.AreEqual(test, conv.ConvertToString(test));
        }

        [ExpectedException(typeof(InvalidCastException))]
        [TestMethod()]
        public void ConvertToStringTest_ThrowOnInvalidType()
        {
            ICsvTypeConverter conv = new StringConverter();

            _ = conv.ConvertToString(4711);

        }


        [ExpectedException(typeof(InvalidCastException))]
        [TestMethod()]
        public void ConvertToStringTest_ThrowOnDBNull() => _ = new StringConverter().ConvertToString(DBNull.Value);
    }
}