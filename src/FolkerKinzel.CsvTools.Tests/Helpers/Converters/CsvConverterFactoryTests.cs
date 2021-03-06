﻿using System;
using System.Globalization;
using FolkerKinzel.CsvTools.Helpers.Converters.Intls;
using FolkerKinzel.CsvTools.Helpers.Converters.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Tests
{
    [TestClass()]
    public class CsvConverterFactoryTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateConverterTest1() => _ = CsvConverterFactory.CreateConverter((CsvTypeCode)4711);

        [DataTestMethod]
        [DataRow(CsvTypeCode.Byte, false, false, typeof(byte), default(byte))]
        [DataRow(CsvTypeCode.UInt16, false, false, typeof(ushort), default(ushort))]
        [DataRow(CsvTypeCode.UInt32, false, false, typeof(uint), default(uint))]
        [DataRow(CsvTypeCode.UInt64, false, false, typeof(ulong), default(ulong))]
        [DataRow(CsvTypeCode.SByte, false, false, typeof(sbyte), default(sbyte))]
        [DataRow(CsvTypeCode.Int16, false, false, typeof(short), default(short))]
        [DataRow(CsvTypeCode.Int32, false, false, typeof(int), default(int))]
        [DataRow(CsvTypeCode.Int64, false, false, typeof(long), default(long))]

        [DataRow(CsvTypeCode.Boolean, false, false, typeof(bool), default(bool))]
        [DataRow(CsvTypeCode.String, false, false, typeof(string), "")]
        [DataRow(CsvTypeCode.Double, false, false, typeof(double), default(double))]
        [DataRow(CsvTypeCode.Single, false, false, typeof(float), default(float))]
        [DataRow(CsvTypeCode.Char, false, false, typeof(char), default(char))]


        [DataRow(CsvTypeCode.Byte, true, false, typeof(byte?), null)]
        [DataRow(CsvTypeCode.UInt16, true, false, typeof(ushort?), null)]
        [DataRow(CsvTypeCode.UInt32, true, false, typeof(uint?), null)]
        [DataRow(CsvTypeCode.UInt64, true, false, typeof(ulong?), null)]
        [DataRow(CsvTypeCode.SByte, true, false, typeof(sbyte?), null)]
        [DataRow(CsvTypeCode.Int16, true, false, typeof(short?), null)]
        [DataRow(CsvTypeCode.Int32, true, false, typeof(int?), null)]
        [DataRow(CsvTypeCode.Int64, true, false, typeof(long?), null)]

        [DataRow(CsvTypeCode.Boolean, true, false, typeof(bool?), null)]
        [DataRow(CsvTypeCode.String, true, false, typeof(string), null)]
        [DataRow(CsvTypeCode.Double, true, false, typeof(double?), null)]
        [DataRow(CsvTypeCode.Single, true, false, typeof(float?), null)]
        [DataRow(CsvTypeCode.Char, true, false, typeof(char?), null)]

        [DataRow(CsvTypeCode.Decimal, true, false, typeof(decimal?), null)]
        [DataRow(CsvTypeCode.DateTime, true, false, typeof(DateTime?), null)]
        [DataRow(CsvTypeCode.TimeSpan, true, false, typeof(TimeSpan?), null)]
        [DataRow(CsvTypeCode.Date, true, false, typeof(DateTime?), null)]
        [DataRow(CsvTypeCode.ByteArray, true, false, typeof(byte[]), null)]
        [DataRow(CsvTypeCode.Guid, true, false, typeof(Guid?), null)]
        [DataRow(CsvTypeCode.DateTimeOffset, true, false, typeof(DateTimeOffset?), null)]

        [DataRow(CsvTypeCode.Byte, false, true, typeof(byte), default(byte))]
        [DataRow(CsvTypeCode.UInt16, false, true, typeof(ushort), default(ushort))]
        [DataRow(CsvTypeCode.UInt32, false, true, typeof(uint), default(uint))]
        [DataRow(CsvTypeCode.UInt64, false, true, typeof(ulong), default(ulong))]
        [DataRow(CsvTypeCode.SByte, false, true, typeof(sbyte), default(sbyte))]
        [DataRow(CsvTypeCode.Int16, false, true, typeof(short), default(short))]
        [DataRow(CsvTypeCode.Int32, false, true, typeof(int), default(int))]
        [DataRow(CsvTypeCode.Int64, false, true, typeof(long), default(long))]

        [DataRow(CsvTypeCode.Boolean, false, true, typeof(bool), default(bool))]
        [DataRow(CsvTypeCode.String, false, true, typeof(string), "")]
        [DataRow(CsvTypeCode.Double, false, true, typeof(double), default(double))]
        [DataRow(CsvTypeCode.Single, false, true, typeof(float), default(float))]
        [DataRow(CsvTypeCode.Char, false, true, typeof(char), default(char))]

        [DataRow(CsvTypeCode.Byte, true, true, typeof(byte?), null)]
        [DataRow(CsvTypeCode.UInt16, true, true, typeof(ushort?), null)]
        [DataRow(CsvTypeCode.UInt32, true, true, typeof(uint?), null)]
        [DataRow(CsvTypeCode.UInt64, true, true, typeof(ulong?), null)]
        [DataRow(CsvTypeCode.SByte, true, true, typeof(sbyte?), null)]
        [DataRow(CsvTypeCode.Int16, true, true, typeof(short?), null)]
        [DataRow(CsvTypeCode.Int32, true, true, typeof(int?), null)]
        [DataRow(CsvTypeCode.Int64, true, true, typeof(long?), null)]

        [DataRow(CsvTypeCode.Boolean, true, true, typeof(bool?), null)]
        [DataRow(CsvTypeCode.String, true, true, typeof(string), null)]
        [DataRow(CsvTypeCode.Double, true, true, typeof(double?), null)]
        [DataRow(CsvTypeCode.Single, true, true, typeof(float?), null)]
        [DataRow(CsvTypeCode.Char, true, true, typeof(char?), null)]

        [DataRow(CsvTypeCode.Decimal, true, true, typeof(decimal?), null)]
        [DataRow(CsvTypeCode.DateTime, true, true, typeof(DateTime?), null)]
        [DataRow(CsvTypeCode.Date, true, true, typeof(DateTime?), null)]
        [DataRow(CsvTypeCode.ByteArray, true, true, typeof(byte[]), null)]
        [DataRow(CsvTypeCode.Guid, true, true, typeof(Guid?), null)]
        [DataRow(CsvTypeCode.DateTimeOffset, true, true, typeof(DateTimeOffset?), null)]
        public void CreateConverterTest2(
            CsvTypeCode typeCode,
            bool nullable,
            bool throwOnParseErrors,
            Type converterType,
            object? fallBackValue)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(typeCode, nullable, false, throwOnParseErrors: throwOnParseErrors);

            Assert.AreEqual(converterType, conv.Type);
            Assert.AreEqual(fallBackValue, conv.FallbackValue);
            Assert.AreEqual(throwOnParseErrors, conv.ThrowsOnParseErrors);
        }


        [DataTestMethod]
        [DataRow(CsvTypeCode.Byte, false, false, typeof(byte))]
        [DataRow(CsvTypeCode.UInt16, false, false, typeof(ushort))]
        [DataRow(CsvTypeCode.UInt32, false, false, typeof(uint))]
        [DataRow(CsvTypeCode.UInt64, false, false, typeof(ulong))]
        [DataRow(CsvTypeCode.SByte, false, false, typeof(sbyte))]
        [DataRow(CsvTypeCode.Int16, false, false, typeof(short))]
        [DataRow(CsvTypeCode.Int32, false, false, typeof(int))]
        [DataRow(CsvTypeCode.Int64, false, false, typeof(long))]

        [DataRow(CsvTypeCode.Boolean, false, false, typeof(bool))]
        [DataRow(CsvTypeCode.String, false, false, typeof(string))]
        [DataRow(CsvTypeCode.Double, false, false, typeof(double))]
        [DataRow(CsvTypeCode.Single, false, false, typeof(float))]
        [DataRow(CsvTypeCode.Char, false, false, typeof(char))]
        [DataRow(CsvTypeCode.Decimal,  false, false, typeof(decimal))]
        [DataRow(CsvTypeCode.DateTime, false, false, typeof(DateTime))]
        [DataRow(CsvTypeCode.TimeSpan, false, false, typeof(TimeSpan))]
        [DataRow(CsvTypeCode.Date,     false, false, typeof(DateTime))]
        [DataRow(CsvTypeCode.ByteArray, false, false, typeof(byte[]))]
        [DataRow(CsvTypeCode.Guid,      false, false, typeof(Guid))]
        [DataRow(CsvTypeCode.DateTimeOffset, false, false, typeof(DateTimeOffset))]

        [DataRow(CsvTypeCode.Byte, true, false, typeof(byte?))]
        [DataRow(CsvTypeCode.UInt16, true, false, typeof(ushort?))]
        [DataRow(CsvTypeCode.UInt32, true, false, typeof(uint?))]
        [DataRow(CsvTypeCode.UInt64, true, false, typeof(ulong?))]
        [DataRow(CsvTypeCode.SByte, true, false, typeof(sbyte?))]
        [DataRow(CsvTypeCode.Int16, true, false, typeof(short?))]
        [DataRow(CsvTypeCode.Int32, true, false, typeof(int?))]
        [DataRow(CsvTypeCode.Int64, true, false, typeof(long?))]

        [DataRow(CsvTypeCode.Boolean, true, false, typeof(bool?))]
        [DataRow(CsvTypeCode.String,  true, false, typeof(string))]
        [DataRow(CsvTypeCode.Double,  true, false, typeof(double?))]
        [DataRow(CsvTypeCode.Single,  true, false, typeof(float?))]
        [DataRow(CsvTypeCode.Char,    true, false, typeof(char?))]
        [DataRow(CsvTypeCode.Decimal, true, false, typeof(decimal?))]
        [DataRow(CsvTypeCode.DateTime, true, false, typeof(DateTime?))]
        [DataRow(CsvTypeCode.TimeSpan, true, false, typeof(TimeSpan?))]
        [DataRow(CsvTypeCode.Date,     true, false, typeof(DateTime?))]
        [DataRow(CsvTypeCode.ByteArray, true, false, typeof(byte[]))]
        [DataRow(CsvTypeCode.Guid,      true, false, typeof(Guid?))]
        [DataRow(CsvTypeCode.DateTimeOffset, true, false, typeof(DateTimeOffset?))]

        [DataRow(CsvTypeCode.Byte, false, true, typeof(byte))]
        [DataRow(CsvTypeCode.UInt16, false, true, typeof(ushort))]
        [DataRow(CsvTypeCode.UInt32, false, true, typeof(uint))]
        [DataRow(CsvTypeCode.UInt64, false, true, typeof(ulong))]
        [DataRow(CsvTypeCode.SByte, false, true, typeof(sbyte))]
        [DataRow(CsvTypeCode.Int16, false, true, typeof(short))]
        [DataRow(CsvTypeCode.Int32, false, true, typeof(int))]
        [DataRow(CsvTypeCode.Int64, false, true, typeof(long))]

        [DataRow(CsvTypeCode.Boolean, false, true, typeof(bool))]
        [DataRow(CsvTypeCode.String, false,  true, typeof(string))]
        [DataRow(CsvTypeCode.Double, false,  true, typeof(double))]
        [DataRow(CsvTypeCode.Single, false,  true, typeof(float))]
        [DataRow(CsvTypeCode.Char, false,    true, typeof(char))]
        [DataRow(CsvTypeCode.Decimal,  false, true, typeof(decimal))]
        [DataRow(CsvTypeCode.DateTime, false, true, typeof(DateTime))]
        [DataRow(CsvTypeCode.TimeSpan, false, true, typeof(TimeSpan))]
        [DataRow(CsvTypeCode.Date,     false, true, typeof(DateTime))]
        [DataRow(CsvTypeCode.ByteArray, false, true, typeof(byte[]))]
        [DataRow(CsvTypeCode.Guid,      false, true, typeof(Guid))]
        [DataRow(CsvTypeCode.DateTimeOffset, false, true, typeof(DateTimeOffset))]

        [DataRow(CsvTypeCode.Byte, true, true, typeof(byte?))]
        [DataRow(CsvTypeCode.UInt16, true, true, typeof(ushort?))]
        [DataRow(CsvTypeCode.UInt32, true, true, typeof(uint?))]
        [DataRow(CsvTypeCode.UInt64, true, true, typeof(ulong?))]
        [DataRow(CsvTypeCode.SByte, true, true, typeof(sbyte?))]
        [DataRow(CsvTypeCode.Int16, true, true, typeof(short?))]
        [DataRow(CsvTypeCode.Int32, true, true, typeof(int?))]
        [DataRow(CsvTypeCode.Int64, true, true, typeof(long?))]

        [DataRow(CsvTypeCode.Boolean, true, true, typeof(bool?))]
        [DataRow(CsvTypeCode.String,  true, true, typeof(string))]
        [DataRow(CsvTypeCode.Double,  true, true, typeof(double?))]
        [DataRow(CsvTypeCode.Single,  true, true, typeof(float?))]
        [DataRow(CsvTypeCode.Char,    true, true, typeof(char?))]
        [DataRow(CsvTypeCode.Decimal, true, true, typeof(decimal?))]
        [DataRow(CsvTypeCode.DateTime, true, true, typeof(DateTime?))]
        [DataRow(CsvTypeCode.TimeSpan, true, true, typeof(TimeSpan?))]
        [DataRow(CsvTypeCode.Date,     true, true, typeof(DateTime?))]
        [DataRow(CsvTypeCode.ByteArray, true, true, typeof(byte[]))]
        [DataRow(CsvTypeCode.Guid,      true, true, typeof(Guid?))]
        [DataRow(CsvTypeCode.DateTimeOffset, true, true, typeof(DateTimeOffset?))]
        public void CreateConverterTest3(
            CsvTypeCode typeCode,
            bool nullable,
            bool throwOnParseErrors,
            Type converterType)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(typeCode, nullable, true, throwOnParseErrors: throwOnParseErrors);

            Assert.AreEqual(converterType, conv.Type);
            Assert.IsTrue(Convert.IsDBNull(conv.FallbackValue));
            Assert.AreEqual(throwOnParseErrors, conv.ThrowsOnParseErrors);
        }


        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest4(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.Decimal, false, false, throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(NumberConverter<decimal>));
            Assert.AreEqual(typeof(decimal), conv.Type);
            Assert.AreEqual(default(decimal), conv.FallbackValue);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            decimal test = 57.839m;
            string? s = conv.ConvertToString(test);

            Assert.IsNotNull(s);

            decimal test2 = (decimal)conv.Parse(s)!;

            Assert.AreEqual(test, test2);
        }


        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest5(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.Decimal, false, false, null, throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(NumberConverter<decimal>));
            Assert.AreEqual(typeof(decimal), conv.Type);
            Assert.AreEqual(default(decimal), conv.FallbackValue);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            decimal test = 57.839m;
            string? s = conv.ConvertToString(test);

            Assert.IsNotNull(s);

            decimal? test2 = (decimal?)conv.Parse(s);

            Assert.AreEqual(test, test2);
        }


        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest6(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.Decimal, false, false, CultureInfo.InvariantCulture, throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(NumberConverter<decimal>));
            Assert.AreEqual(typeof(decimal), conv.Type);
            Assert.AreEqual(default(decimal), conv.FallbackValue);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            decimal test = 57.839m;
            string? s = conv.ConvertToString(test);

            Assert.IsNotNull(s);

            decimal? test2 = (decimal?)conv.Parse(s);

            Assert.AreEqual(test, test2);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest7(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.Decimal, false, false, CultureInfo.CreateSpecificCulture("de-DE"), throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(NumberConverter<decimal>));
            Assert.AreEqual(typeof(decimal), conv.Type);
            Assert.AreEqual(default(decimal), conv.FallbackValue);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            decimal test = 57.839m;
            string? s = conv.ConvertToString(test);

            Assert.IsNotNull(s);

            decimal? test2 = (decimal?)conv.Parse(s);

            Assert.AreEqual(test, test2);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest8(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.ByteArray, false, false, throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(Base64Converter));
            Assert.AreEqual(typeof(byte[]), conv.Type);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            if (conv.FallbackValue is byte[] bts)
            {
                Assert.AreEqual(0, bts.Length);
            }
            else
            {
                Assert.Fail();
            }

            var test1 = new byte[] { 17, 24, 42 };

            string? s = conv.ConvertToString(test1);

            byte[]? test2 = (byte[]?)conv.Parse(s);

            CollectionAssert.AreEqual(test1, test2);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest9(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.Guid, false, false, throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(GuidConverter));
            Assert.AreEqual(typeof(Guid), conv.Type);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);
            Assert.AreEqual(conv.FallbackValue, default(Guid));

            var test1 = Guid.NewGuid();

            string? s = conv.ConvertToString(test1);

            var test2 = (Guid?)conv.Parse(s);

            Assert.AreEqual(test1, test2);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest10(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.Date, false, false, CultureInfo.InvariantCulture, throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(DateTimeConverter));
            Assert.AreEqual(typeof(DateTime), conv.Type);
            Assert.AreEqual(default(DateTime), conv.FallbackValue);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            var test = new DateTime(1956, 12, 24);
            string? s = conv.ConvertToString(test);

            Assert.IsNotNull(s);

            var test2 = (DateTime?)conv.Parse(s);

            Assert.AreEqual(test, test2);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest11(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.Date, false, false, CultureInfo.CreateSpecificCulture("de-DE"), throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(DateTimeConverter));
            Assert.AreEqual(typeof(DateTime), conv.Type);
            Assert.AreEqual(default(DateTime), conv.FallbackValue);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            var test = new DateTime(1956, 12, 24);
            string? s = conv.ConvertToString(test);

            Assert.IsNotNull(s);

            var test2 = (DateTime?)conv.Parse(s);

            Assert.AreEqual(test, test2);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest12(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTime, false, false, CultureInfo.InvariantCulture, throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(DateTimeConverter));
            Assert.AreEqual(typeof(DateTime), conv.Type);
            Assert.AreEqual(default(DateTime), conv.FallbackValue);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            var test = new DateTime(1956, 12, 24, 14, 27, 32);
            string? s = conv.ConvertToString(test);

            Assert.IsNotNull(s);

            var test2 = (DateTime?)conv.Parse(s);

            Assert.AreEqual(test, test2);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest13(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTime, false, false, CultureInfo.CreateSpecificCulture("de-DE"), throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(DateTimeConverter));
            Assert.AreEqual(typeof(DateTime), conv.Type);
            Assert.AreEqual(default(DateTime), conv.FallbackValue);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            var test = new DateTime(1956, 12, 24, 14, 27, 32);
            string? s = conv.ConvertToString(test);

            Assert.IsNotNull(s);

            var test2 = (DateTime?)conv.Parse(s);

            Assert.AreEqual(test, test2);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest14(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTimeOffset, false, false, CultureInfo.InvariantCulture, throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(DateTimeOffsetConverter));
            Assert.AreEqual(typeof(DateTimeOffset), conv.Type);
            Assert.AreEqual(default(DateTimeOffset), conv.FallbackValue);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            var test = new DateTimeOffset(1956, 12, 24, 14, 27, 32, TimeSpan.FromHours(3));
            string? s = conv.ConvertToString(test);

            Assert.IsNotNull(s);

            var test2 = (DateTimeOffset?)conv.Parse(s);

            Assert.AreEqual(test, test2);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest15(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.DateTimeOffset, false, false, CultureInfo.CreateSpecificCulture("de-DE"), throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(DateTimeOffsetConverter));
            Assert.AreEqual(typeof(DateTimeOffset), conv.Type);
            Assert.AreEqual(default(DateTimeOffset), conv.FallbackValue);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            var test = new DateTimeOffset(1956, 12, 24, 14, 27, 32, TimeSpan.FromHours(3));
            string? s = conv.ConvertToString(test);

            Assert.IsNotNull(s);

            var test2 = (DateTimeOffset?)conv.Parse(s);

            Assert.AreEqual(test, test2);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest16(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.TimeSpan, false, false, CultureInfo.InvariantCulture, throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(TimeSpanConverter));
            Assert.AreEqual(typeof(TimeSpan), conv.Type);
            Assert.AreEqual(default(TimeSpan), conv.FallbackValue);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            var test = new TimeSpan(16, 24, 53);
            string? s = conv.ConvertToString(test);

            Assert.IsNotNull(s);

            var test2 = (TimeSpan?)conv.Parse(s);

            Assert.AreEqual(test, test2);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CreateConverterTest17(bool throwsOnParseErrors)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.TimeSpan, false, false, CultureInfo.CreateSpecificCulture("de-DE"), throwOnParseErrors: throwsOnParseErrors);

            Assert.IsInstanceOfType(conv, typeof(TimeSpanConverter));
            Assert.AreEqual(typeof(TimeSpan), conv.Type);
            Assert.AreEqual(default(TimeSpan), conv.FallbackValue);
            Assert.AreEqual(throwsOnParseErrors, conv.ThrowsOnParseErrors);

            var test = new TimeSpan(16, 24, 53);
            string? s = conv.ConvertToString(test);

            Assert.IsNotNull(s);

            var test2 = (TimeSpan?)conv.Parse(s);

            Assert.AreEqual(test, test2);
        }

        [DataTestMethod]
        [DataRow(CsvTypeCode.Byte, false, false, typeof(byte), default(byte))]
        [DataRow(CsvTypeCode.UInt16, false, false, typeof(ushort), default(ushort))]
        [DataRow(CsvTypeCode.UInt32, false, false, typeof(uint), default(uint))]
        [DataRow(CsvTypeCode.UInt64, false, false, typeof(ulong), default(ulong))]
        [DataRow(CsvTypeCode.SByte, false, false, typeof(sbyte), default(sbyte))]
        [DataRow(CsvTypeCode.Int16, false, false, typeof(short), default(short))]
        [DataRow(CsvTypeCode.Int32, false, false, typeof(int), default(int))]
        [DataRow(CsvTypeCode.Int64, false, false, typeof(long), default(long))]

        [DataRow(CsvTypeCode.Byte, true, false, typeof(byte?), null)]
        [DataRow(CsvTypeCode.UInt16, true, false, typeof(ushort?), null)]
        [DataRow(CsvTypeCode.UInt32, true, false, typeof(uint?), null)]
        [DataRow(CsvTypeCode.UInt64, true, false, typeof(ulong?), null)]
        [DataRow(CsvTypeCode.SByte, true, false, typeof(sbyte?), null)]
        [DataRow(CsvTypeCode.Int16, true, false, typeof(short?), null)]
        [DataRow(CsvTypeCode.Int32, true, false, typeof(int?), null)]
        [DataRow(CsvTypeCode.Int64, true, false, typeof(long?), null)]

        [DataRow(CsvTypeCode.Byte, false, true, typeof(byte), default(byte))]
        [DataRow(CsvTypeCode.UInt16, false, true, typeof(ushort), default(ushort))]
        [DataRow(CsvTypeCode.UInt32, false, true, typeof(uint), default(uint))]
        [DataRow(CsvTypeCode.UInt64, false, true, typeof(ulong), default(ulong))]
        [DataRow(CsvTypeCode.SByte, false, true, typeof(sbyte), default(sbyte))]
        [DataRow(CsvTypeCode.Int16, false, true, typeof(short), default(short))]
        [DataRow(CsvTypeCode.Int32, false, true, typeof(int), default(int))]
        [DataRow(CsvTypeCode.Int64, false, true, typeof(long), default(long))]

        [DataRow(CsvTypeCode.Byte, true, true, typeof(byte?), null)]
        [DataRow(CsvTypeCode.UInt16, true, true, typeof(ushort?), null)]
        [DataRow(CsvTypeCode.UInt32, true, true, typeof(uint?), null)]
        [DataRow(CsvTypeCode.UInt64, true, true, typeof(ulong?), null)]
        [DataRow(CsvTypeCode.SByte, true, true, typeof(sbyte?), null)]
        [DataRow(CsvTypeCode.Int16, true, true, typeof(short?), null)]
        [DataRow(CsvTypeCode.Int32, true, true, typeof(int?), null)]
        [DataRow(CsvTypeCode.Int64, true, true, typeof(long?), null)]
        public void CreateHexConverterTest1(
            CsvTypeCode typeCode,
            bool nullable,
            bool throwOnParseErrors,
            Type converterType,
            object? fallBackValue)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateHexConverter(typeCode, nullable, false, throwOnParseErrors);

            Assert.AreEqual(converterType, conv.Type);
            Assert.AreEqual(fallBackValue, conv.FallbackValue);
            Assert.AreEqual(throwOnParseErrors, conv.ThrowsOnParseErrors);
        }


        [DataTestMethod]
        [DataRow(CsvTypeCode.Byte, false, false, typeof(byte))]
        [DataRow(CsvTypeCode.UInt16, false, false, typeof(ushort))]
        [DataRow(CsvTypeCode.UInt32, false, false, typeof(uint))]
        [DataRow(CsvTypeCode.UInt64, false, false, typeof(ulong))]
        [DataRow(CsvTypeCode.SByte, false, false, typeof(sbyte))]
        [DataRow(CsvTypeCode.Int16, false, false, typeof(short))]
        [DataRow(CsvTypeCode.Int32, false, false, typeof(int))]
        [DataRow(CsvTypeCode.Int64, false, false, typeof(long))]

        [DataRow(CsvTypeCode.Byte, true, false, typeof(byte?))]
        [DataRow(CsvTypeCode.UInt16, true, false, typeof(ushort?))]
        [DataRow(CsvTypeCode.UInt32, true, false, typeof(uint?))]
        [DataRow(CsvTypeCode.UInt64, true, false, typeof(ulong?))]
        [DataRow(CsvTypeCode.SByte, true, false, typeof(sbyte?))]
        [DataRow(CsvTypeCode.Int16, true, false, typeof(short?))]
        [DataRow(CsvTypeCode.Int32, true, false, typeof(int?))]
        [DataRow(CsvTypeCode.Int64, true, false, typeof(long?))]

        [DataRow(CsvTypeCode.Byte, false, true, typeof(byte))]
        [DataRow(CsvTypeCode.UInt16, false, true, typeof(ushort))]
        [DataRow(CsvTypeCode.UInt32, false, true, typeof(uint))]
        [DataRow(CsvTypeCode.UInt64, false, true, typeof(ulong))]
        [DataRow(CsvTypeCode.SByte, false, true, typeof(sbyte))]
        [DataRow(CsvTypeCode.Int16, false, true, typeof(short))]
        [DataRow(CsvTypeCode.Int32, false, true, typeof(int))]
        [DataRow(CsvTypeCode.Int64, false, true, typeof(long))]

        [DataRow(CsvTypeCode.Byte, true, true, typeof(byte?))]
        [DataRow(CsvTypeCode.UInt16, true, true, typeof(ushort?))]
        [DataRow(CsvTypeCode.UInt32, true, true, typeof(uint?))]
        [DataRow(CsvTypeCode.UInt64, true, true, typeof(ulong?))]
        [DataRow(CsvTypeCode.SByte, true, true, typeof(sbyte?))]
        [DataRow(CsvTypeCode.Int16, true, true, typeof(short?))]
        [DataRow(CsvTypeCode.Int32, true, true, typeof(int?))]
        [DataRow(CsvTypeCode.Int64, true, true, typeof(long?))]
        public void CreateHexConverterTest2(
            CsvTypeCode typeCode,
            bool nullable,
            bool throwOnParseErrors,
            Type converterType)
        {
            ICsvTypeConverter conv = CsvConverterFactory.CreateHexConverter(typeCode, nullable, true, throwOnParseErrors);

            Assert.AreEqual(converterType, conv.Type);
            Assert.IsTrue(Convert.IsDBNull(conv.FallbackValue));
            Assert.AreEqual(throwOnParseErrors, conv.ThrowsOnParseErrors);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateHexConverterTest3() => _ = CsvConverterFactory.CreateHexConverter(CsvTypeCode.Double);

    }
}