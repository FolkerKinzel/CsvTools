using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Tests
{
    [TestClass()]
    public class GuidConverterTests
    {
        [TestMethod()]
        public void GuidConverterTest1()
        {
            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateConverter(CsvTypeCode.Guid);
            Assert.IsInstanceOfType(conv, typeof(GuidConverter2));
        }

        [DataTestMethod()]
        [DataRow(null)]
        [DataRow("")]
        public void GuidConverterTest2(string? format)
        {
            var conv = new GuidConverter2(format);
            Assert.IsInstanceOfType(conv, typeof(GuidConverter2));
        }

        

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GuidConverterTest4() => _ = new GuidConverter2("bla");


        [TestMethod()]
        public void GuidConverterTest5()
        {
            var conv = new GuidConverter2("B");
            Assert.IsInstanceOfType(conv, typeof(GuidConverter2));
        }

        [TestMethod()]
        public void Roundtrip1()
        {
            var guid = Guid.NewGuid();

            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateConverter(CsvTypeCode.Guid);

            string? tmp = conv.ConvertToString(guid);

            Assert.IsNotNull(tmp);

            var now2 = (Guid?)conv.Parse(tmp);

            Assert.AreEqual(guid, now2);
        }

        [TestMethod()]
        public void Roundtrip2()
        {
            var guid = Guid.NewGuid();

            var conv = new GuidConverter2("B");

            string? tmp = conv.ConvertToString(guid);

            Assert.IsNotNull(tmp);

            var now2 = (Guid?)conv.Parse(tmp);

            Assert.AreEqual(guid, now2);
        }
    }
}