using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls.Tests
{
    [TestClass()]
    public class Base64ConverterTests
    {
        [TestMethod()]
        public void Base64ConverterTest1()
        {
            ICsvTypeConverter2 conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray);
            Assert.IsInstanceOfType(conv, typeof(Base64Converter2));
        }


        [TestMethod]
        public void ParseTest1()
        {
            ICsvTypeConverter2 conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray, CsvConverterOptions.Nullable);
            Assert.IsInstanceOfType(conv, typeof(Base64Converter2));

            Assert.IsNull(conv.Parse(null));
        }


        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseTest2()
        {
            ICsvTypeConverter2 conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray, CsvConverterOptions.Nullable | CsvConverterOptions.Throwing);
            Assert.IsInstanceOfType(conv, typeof(Base64Converter2));

            Assert.IsNull(conv.Parse(null));
            _ = conv.Parse("blabla");
        }


        [TestMethod]
        public void ParseTest3()
        {
            ICsvTypeConverter2 conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray, CsvConverterOptions.Nullable);
            Assert.IsInstanceOfType(conv, typeof(Base64Converter2));

            Assert.IsNull(conv.Parse(null));
            Assert.IsNull(conv.Parse("blabla"));
        }
        

        [TestMethod]
        public void RoundtripTest1()
        {
            var bytes = new byte[7];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(bytes);

            ICsvTypeConverter2 conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray);

            string? s = conv.ConvertToString(bytes);
            Assert.IsNotNull(s);

            var bytes2 = (byte[]?)conv.Parse(s);

            CollectionAssert.AreEqual(bytes, bytes2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void MyTestMethod()
        {
            ICsvTypeConverter2 conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray);

            _ = conv.ConvertToString(4711);
        }

    }
}