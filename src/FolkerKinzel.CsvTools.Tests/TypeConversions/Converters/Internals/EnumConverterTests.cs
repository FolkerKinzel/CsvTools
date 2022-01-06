using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls.Tests
{
    [TestClass()]
    public class EnumConverterTests
    {
        [TestMethod()]
        public void EnumConverterTest1()
        {
            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>();
            Assert.IsInstanceOfType(conv, typeof(EnumConverter2<CsvTypeCode>));
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
            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>(format: "bla");
            Assert.IsInstanceOfType(conv, typeof(EnumConverter2<CsvTypeCode>));
        }

        [TestMethod()]
        public void EnumConverterTest4()
        {
            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>(format: "F");
            Assert.IsInstanceOfType(conv, typeof(EnumConverter2<CsvTypeCode>));
        }


        [TestMethod]
        public void RoundtripTest1()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>();

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            var val2 = (CsvTypeCode?)conv.Parse(s);

            Assert.AreEqual(val, val2);
        }

        [TestMethod]
        public void RoundtripTest2()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>(format: "F");

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            var val2 = (CsvTypeCode?)conv.Parse(s);

            Assert.AreEqual(val, val2);
        }

        [TestMethod]
        public void RoundtripTest3()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>(ignoreCase: true);

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

            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>(format: "F", ignoreCase: true);

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
            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>(ignoreCase: false);

            string s = CsvTypeCode.DateTimeOffset.ToString().ToUpperInvariant();

            _ = (CsvTypeCode?)conv.Parse(s);

        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RoundtripTest6()
        {
            CsvTypeCode val = CsvTypeCode.DateTimeOffset;

            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>(format: "F", ignoreCase: false);

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

            CsvConverterOptions options = CsvConverterOptions.Nullable;

            if (throwOnParseErrors)
            {
                options |= CsvConverterOptions.ThrowsOnParseErrors;
            }

            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>(format: "F", options: options, ignoreCase: ignoreCase);

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
            CsvConverterOptions options = CsvConverterOptions.Nullable;

            if (throwOnParseErrors)
            {
                options |= CsvConverterOptions.ThrowsOnParseErrors;
            }

            CsvTypeCode? val = null;

            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>(options: options, ignoreCase: ignoreCase);

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
            CsvConverterOptions options = CsvConverterOptions.AcceptsDBNull;

            if (throwOnParseErrors)
            {
                options |= CsvConverterOptions.ThrowsOnParseErrors;
            }

            if (nullable)
            {
                options |= CsvConverterOptions.Nullable;
            }

            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>(format: "F", options: options, ignoreCase: ignoreCase);

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
            CsvConverterOptions options = CsvConverterOptions.AcceptsDBNull;

            if (throwOnParseErrors)
            {
                options |= CsvConverterOptions.ThrowsOnParseErrors;
            }

            if (nullable)
            {
                options |= CsvConverterOptions.Nullable;
            }

            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateEnumConverter<CsvTypeCode>(options: options, ignoreCase: ignoreCase);

            string? s = conv.ConvertToString(DBNull.Value);
            Assert.IsNull(s);

            Assert.IsTrue(Convert.IsDBNull(conv.Parse(s)));
        }

    }
}