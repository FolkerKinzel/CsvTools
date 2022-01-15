using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls.Tests
{
    [TestClass()]
    public class ByteArrayConverterTests
    {
        [TestMethod()]
        public void Base64ConverterTest1()
        {
            ICsvTypeConverter conv = new ByteArrayConverter();
            Assert.IsInstanceOfType(conv, typeof(ByteArrayConverter));
        }


        [TestMethod]
        public void ParseTest1()
        {
            ICsvTypeConverter conv = new ByteArrayConverter();
            Assert.IsInstanceOfType(conv, typeof(ByteArrayConverter));

            Assert.IsNull(conv.Parse(null));
        }


        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseTest2()
        {
            ICsvTypeConverter conv = new ByteArrayConverter();
            Assert.IsInstanceOfType(conv, typeof(ByteArrayConverter));

            Assert.IsNull(conv.Parse(default));
            _ = conv.Parse("blabla".AsSpan());
        }


        [TestMethod]
        public void ParseTest3()
        {
            ICsvTypeConverter conv = new ByteArrayConverter(false);
            Assert.IsInstanceOfType(conv, typeof(ByteArrayConverter));

            Assert.IsNull(conv.Parse(default));
            Assert.IsNull(conv.Parse("blabla".AsSpan()));
        }
        

        [TestMethod]
        public void RoundtripTest1()
        {
            var bytes = new byte[7];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(bytes);

            ICsvTypeConverter conv = new ByteArrayConverter();

            string? s = conv.ConvertToString(bytes);
            Assert.IsNotNull(s);

            var bytes2 = (byte[]?)conv.Parse(s.AsSpan());

            CollectionAssert.AreEqual(bytes, bytes2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void MyTestMethod() => _ = new ByteArrayConverter().ConvertToString(4711);

    }
}