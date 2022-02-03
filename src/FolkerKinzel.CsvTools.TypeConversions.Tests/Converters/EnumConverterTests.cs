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
            ICsvTypeConverter conv = new EnumConverter<TypeCode>();
            Assert.IsInstanceOfType(conv, typeof(EnumConverter<TypeCode>));
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
            ICsvTypeConverter conv = new EnumConverter<TypeCode>(format: "bla");
            Assert.IsInstanceOfType(conv, typeof(EnumConverter<TypeCode>));
        }

        [TestMethod()]
        public void EnumConverterTest4()
        {
            ICsvTypeConverter conv = new EnumConverter<TypeCode>(format: "F");
            Assert.IsInstanceOfType(conv, typeof(EnumConverter<TypeCode>));
        }


        [TestMethod()]
        [ExpectedException(typeof(InvalidCastException))]
        public void EnumConverterTest5() => _ = new EnumConverter<DayOfWeek>().ConvertToString(null);


        [TestMethod]
        public void RoundtripTest1()
        {
            TypeCode val = TypeCode.DateTime;

            ICsvTypeConverter conv = new EnumConverter<TypeCode>();

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            var val2 = (TypeCode?)conv.Parse(s.AsSpan());

            Assert.AreEqual(val, val2);
        }

        [TestMethod]
        public void RoundtripTest2()
        {
            TypeCode val = TypeCode.DateTime;

            ICsvTypeConverter conv = new EnumConverter<TypeCode>(format: "F");

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            var val2 = (TypeCode?)conv.Parse(s.AsSpan());

            Assert.AreEqual(val, val2);
        }

        [TestMethod]
        public void RoundtripTest3()
        {
            TypeCode val = TypeCode.DateTime;

            ICsvTypeConverter conv = new EnumConverter<TypeCode>(ignoreCase: true);

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            s = s!.ToUpperInvariant();

            var val2 = (TypeCode?)conv.Parse(s.AsSpan());

            Assert.AreEqual(val, val2);
        }

        [TestMethod]
        public void RoundtripTest4()
        {
            TypeCode val = TypeCode.DateTime;

            ICsvTypeConverter conv = new EnumConverter<TypeCode>(format: "F", ignoreCase: true);

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            s = s!.ToUpperInvariant();


            var val2 = (TypeCode?)conv.Parse(s.AsSpan());

            Assert.AreEqual(val, val2);
        }


        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void RoundtripTest5()
        {
            ICsvTypeConverter conv = new EnumConverter<TypeCode>(ignoreCase: false);

            string s = TypeCode.DateTime.ToString().ToUpperInvariant();

            _ = (TypeCode?)conv.Parse(s.AsSpan());
        }


        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void RoundtripTest6()
        {
            TypeCode val = TypeCode.DateTime;

            ICsvTypeConverter conv = new EnumConverter<TypeCode>(format: "F", ignoreCase: false);

            string? s = conv.ConvertToString(val);
            Assert.IsNotNull(s);

            s = s!.ToUpperInvariant();
            _ = (TypeCode?)conv.Parse(s.AsSpan());
        }


        [DataTestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(true, true)]
        public void RoundtripTest7(bool throwOnParseErrors, bool ignoreCase)
        {
            TypeCode? val = null;

            ICsvTypeConverter conv = new EnumConverter<TypeCode>(format: "F", throwing: throwOnParseErrors, ignoreCase: ignoreCase).AsNullableConverter();

            string? s = conv.ConvertToString(val);
            Assert.IsNull(s);

            Assert.IsNull(conv.Parse(s.AsSpan()));
        }


        [DataTestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(true, true)]
        public void RoundtripTest8(bool throwOnParseErrors, bool ignoreCase)
        {
            TypeCode? val = null;

            ICsvTypeConverter conv = new EnumConverter<TypeCode>(throwOnParseErrors, ignoreCase: ignoreCase).AsNullableConverter();

            string? s = conv.ConvertToString(val);
            Assert.IsNull(s);

            Assert.IsNull(conv.Parse(s.AsSpan()));
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
            var enumConv = new EnumConverter<TypeCode>(format: "F", throwing: throwOnParseErrors, ignoreCase: ignoreCase);

            CsvTypeConverter<object> conv = nullable ? enumConv.AsNullableConverter().AsDBNullEnabled() : enumConv.AsDBNullEnabled();

            string? s = conv.ConvertToString(DBNull.Value);
            Assert.IsNull(s);

            Assert.IsTrue(Convert.IsDBNull(conv.Parse(s.AsSpan())));
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
            CsvTypeConverter<object> conv = nullable ? new EnumConverter<TypeCode>(throwOnParseErrors, ignoreCase: ignoreCase).AsNullableConverter().AsDBNullEnabled() :
                                  new EnumConverter<TypeCode>(throwOnParseErrors, ignoreCase: ignoreCase).AsDBNullEnabled();

            string? s = conv.ConvertToString(DBNull.Value);
            Assert.IsNull(s);

            Assert.IsTrue(Convert.IsDBNull(conv.Parse(s.AsSpan())));
        }

    }
}