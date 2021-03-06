﻿using System;
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
        public void ParseTest1()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray, true);
            Assert.IsInstanceOfType(conv, typeof(Base64Converter));

            Assert.IsNull(conv.FallbackValue);
            Assert.IsNull(conv.Parse(null));
        }


        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseTest2()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray, true, throwOnParseErrors:true);
            Assert.IsInstanceOfType(conv, typeof(Base64Converter));

            Assert.IsNull(conv.FallbackValue);
            _ = conv.Parse("blabla");
        }


        [TestMethod]
        public void ParseTest3()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray, true, throwOnParseErrors:false);
            Assert.IsInstanceOfType(conv, typeof(Base64Converter));

            Assert.IsNull(conv.FallbackValue);
            Assert.IsNull(conv.Parse("blabla"));
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

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void MyTestMethod()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray);

            _ = conv.ConvertToString(4711);
        }

    }
}