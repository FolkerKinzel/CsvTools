using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Intls.Tests
{
    [TestClass()]
    public class EnumConverterTests
    {
        [TestMethod()]
        public void EnumConverterTest1()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateEnumConverter<CsvTypeCode>();
            Assert.IsInstanceOfType(conv, typeof(EnumConverter<CsvTypeCode>));
        }

        [TestMethod()]
        public void EnumConverterTest2()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateEnumConverter<CsvTypeCode>(null);
            Assert.IsInstanceOfType(conv, typeof(EnumConverter<CsvTypeCode>));
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void EnumConverterTest3()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateEnumConverter<CsvTypeCode>("bla");
            Assert.IsInstanceOfType(conv, typeof(EnumConverter<CsvTypeCode>));
        }

        [TestMethod()]
        public void EnumConverterTest4()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateEnumConverter<CsvTypeCode>("F");
            Assert.IsInstanceOfType(conv, typeof(EnumConverter<CsvTypeCode>));
        }


        [TestMethod]
        public void RoundtripTest1()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter conv = CsvConverterFactory.CreateEnumConverter<CsvTypeCode>();

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            var val2 = (CsvTypeCode?)conv.Parse(s);

            Assert.AreEqual(val, val2);
        }

        [TestMethod]
        public void RoundtripTest2()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter conv = CsvConverterFactory.CreateEnumConverter<CsvTypeCode>("F");

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            var val2 = (CsvTypeCode?)conv.Parse(s);

            Assert.AreEqual(val, val2);
        }

        [TestMethod]
        public void RoundtripTest3()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter conv = CsvConverterFactory.CreateEnumConverter<CsvTypeCode>(ignoreCase: true);

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            s = s!.ToUpperInvariant();

            var val2 = (CsvTypeCode?)conv.Parse(s);

            Assert.AreEqual(val, val2);
        }

        [TestMethod]
        public void RoundtripTest4()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter conv = CsvConverterFactory.CreateEnumConverter<CsvTypeCode>("F", ignoreCase: true);

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            s = s!.ToUpperInvariant();


            var val2 = (CsvTypeCode?)conv.Parse(s);

            Assert.AreEqual(val, val2);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RoundtripTest5()
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateEnumConverter<CsvTypeCode>(ignoreCase: false, throwOnParseErrors: true);

            string s = CsvTypeCode.DateTimeOffset.ToString().ToUpperInvariant();

            _ = (CsvTypeCode?)conv.Parse(s);

        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RoundtripTest6()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter conv = CsvConverterFactory.CreateEnumConverter<CsvTypeCode>("F", ignoreCase: false, throwOnParseErrors: true);

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            s = s!.ToUpperInvariant();


            _ = (CsvTypeCode?)conv.Parse(s);

        }
    }
}