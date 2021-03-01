using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Intls.Tests
{
    [TestClass()]
    public class Base64ConverterTests
    {
        [TestMethod()]
        public void Base64ConverterTest1()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray);
            Assert.IsInstanceOfType(conv, typeof(Base64Converter));
        }

        

        [TestMethod]
        public void RoundtripTest1()
        {
            var bytes = new byte[7];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(bytes);

            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray);

            string? s = conv.ConvertToString(bytes);
            Assert.IsNotNull(s);

            var bytes2 = (byte[]?)conv.Parse(s);

            CollectionAssert.AreEqual(bytes, bytes2);
        }

    }
}