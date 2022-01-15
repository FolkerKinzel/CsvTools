﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls.Tests
{
    [TestClass()]
    public class EnumConverterTests
    {
        [TestMethod()]
        public void EnumConverterTest1()
        {
            ICsvTypeConverter conv = new EnumConverter<CsvTypeCode>();
            Assert.IsInstanceOfType(conv, typeof(EnumConverter<CsvTypeCode>));
        }

        //[TestMethod()]
        //public void EnumConverterTest2()
        //{
        //    ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>();
        //    Assert.IsInstanceOfType(conv, typeof(EnumConverter2<CsvTypeCode>));
        //}


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void EnumConverterTest3()
        {
            ICsvTypeConverter conv = new EnumConverter<CsvTypeCode>(format: "bla");
            Assert.IsInstanceOfType(conv, typeof(EnumConverter<CsvTypeCode>));
        }

        [TestMethod()]
        public void EnumConverterTest4()
        {
            ICsvTypeConverter conv = new EnumConverter<CsvTypeCode>(format: "F");
            Assert.IsInstanceOfType(conv, typeof(EnumConverter<CsvTypeCode>));
        }


        [TestMethod]
        public void RoundtripTest1()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter conv = new EnumConverter<CsvTypeCode>();

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            var val2 = (CsvTypeCode?)conv.Parse(s);

            Assert.AreEqual(val, val2);
        }

        [TestMethod]
        public void RoundtripTest2()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter conv = new EnumConverter<CsvTypeCode>(format: "F");

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            var val2 = (CsvTypeCode?)conv.Parse(s);

            Assert.AreEqual(val, val2);
        }

        [TestMethod]
        public void RoundtripTest3()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter conv = new EnumConverter<CsvTypeCode>(ignoreCase: true);

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

            ICsvTypeConverter conv = new EnumConverter<CsvTypeCode>(format: "F", ignoreCase: true);

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            s = s!.ToUpperInvariant();


            var val2 = (CsvTypeCode?)conv.Parse(s);

            Assert.AreEqual(val, val2);
        }


        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void RoundtripTest5()
        {
            ICsvTypeConverter conv = new EnumConverter<CsvTypeCode>(ignoreCase: false);

            string s = CsvTypeCode.DateTimeOffset.ToString().ToUpperInvariant();

            _ = (CsvTypeCode?)conv.Parse(s);
        }


        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void RoundtripTest6()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter conv = new EnumConverter<CsvTypeCode>(format: "F", ignoreCase: false);

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            s = s!.ToUpperInvariant();
            _ = (CsvTypeCode?)conv.Parse(s);
        }


        [DataTestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(true, true)]
        public void RoundtripTest7(bool throwOnParseErrors, bool ignoreCase)
        {
            CsvTypeCode? val = null;

            ICsvTypeConverter conv = new EnumConverter<CsvTypeCode>(format: "F", throwing: throwOnParseErrors, ignoreCase: ignoreCase).AsNullableConverter();

            string? s = conv.ConvertToString(val);
            Assert.IsNull(s);

            Assert.IsNull(conv.Parse(s));
        }


        [DataTestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(true, true)]
        public void RoundtripTest8(bool throwOnParseErrors, bool ignoreCase)
        {
            CsvTypeCode? val = null;

            ICsvTypeConverter conv = new EnumConverter<CsvTypeCode>(throwOnParseErrors, ignoreCase: ignoreCase).AsNullableConverter();

            string? s = conv.ConvertToString(val);
            Assert.IsNull(s);

            Assert.IsNull(conv.Parse(s));
        }

        [DataTestMethod]
        [DataRow(false, false, false)]
        [DataRow(false, true, false)]
        [DataRow(true, false, false)]
        [DataRow(true, true, false)]
        [DataRow(false, false, true)]
        [DataRow(false, true, true)]
        [DataRow(true, false, true)]
        [DataRow(true, true, true)]
        public void RoundtripTest9(bool throwOnParseErrors, bool ignoreCase, bool nullable)
        {
            var enumConv = new EnumConverter<CsvTypeCode>(format: "F", throwing: throwOnParseErrors, ignoreCase: ignoreCase);

            CsvTypeConverter<object> conv = nullable ? enumConv.AsNullableConverter().AsDBNullEnabled() : enumConv.AsDBNullEnabled();

            string? s = conv.ConvertToString(DBNull.Value);
            Assert.IsNull(s);

            Assert.IsTrue(Convert.IsDBNull(conv.Parse(s)));
        }


        [DataTestMethod]
        [DataRow(false, false, false)]
        [DataRow(false, true, false)]
        [DataRow(true, false, false)]
        [DataRow(true, true, false)]
        [DataRow(false, false, true)]
        [DataRow(false, true, true)]
        [DataRow(true, false, true)]
        [DataRow(true, true, true)]
        public void RoundtripTest10(bool throwOnParseErrors, bool ignoreCase, bool nullable)
        {
            CsvTypeConverter<object> conv = nullable ? new EnumConverter<CsvTypeCode>(throwOnParseErrors, ignoreCase: ignoreCase).AsNullableConverter().AsDBNullEnabled() :
                                  new EnumConverter<CsvTypeCode>(throwOnParseErrors, ignoreCase: ignoreCase).AsDBNullEnabled();

            string? s = conv.ConvertToString(DBNull.Value);
            Assert.IsNull(s);

            Assert.IsTrue(Convert.IsDBNull(conv.Parse(s)));
        }

    }
}