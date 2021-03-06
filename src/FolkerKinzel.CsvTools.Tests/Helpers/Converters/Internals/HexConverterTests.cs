﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Intls.Tests
{
    [TestClass()]
    public class HexConverterTests
    {
        [TestMethod()]
        public void HexConverterTest1()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateHexConverter(CsvTypeCode.Int32);
            Assert.IsInstanceOfType(conv, typeof(HexConverter<int>));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void HexConverterTest2() => _ = CsvConverterFactory.CreateHexConverter(CsvTypeCode.Double);


        [TestMethod]
        public void RoundtripTest1()
        {
            int i = 123456789;

            ICsvTypeConverter conv = CsvConverterFactory.CreateHexConverter(CsvTypeCode.Int32);

            string? s = conv.ConvertToString(i);
            Assert.IsNotNull(s);

            var i2 = (int?)conv.Parse(s);

            Assert.AreEqual(i, i2);

        }


        [TestMethod]
        public void RoundtripTest2()
        {
            int i = 123456789;

            ICsvTypeConverter conv = CsvConverterFactory.CreateHexConverter(CsvTypeCode.Int32);

            string? s = conv.ConvertToString(i)?.ToLowerInvariant();
            Assert.IsNotNull(s);

            var i2 = (int?)conv.Parse(s);

            Assert.AreEqual(i, i2);

        }
    }
}