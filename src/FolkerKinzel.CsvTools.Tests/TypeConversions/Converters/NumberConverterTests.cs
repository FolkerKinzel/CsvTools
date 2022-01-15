using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Intls.Tests
{
    [TestClass()]
    public class NumberConverterTests
    {
        [TestMethod]
        public void NumberConverterTest1()
        {
            ICsvTypeConverter conv = new DoubleConverter();
            Assert.IsInstanceOfType(conv, typeof(DoubleConverter));
        }

        [TestMethod]
        public void RoundtripTest1()
        {
            double d = 72.81;

            ICsvTypeConverter conv = new DoubleConverter();

            string? s = conv.ConvertToString(d);
            Assert.IsNotNull(s);

            double? d2 = (double?)conv.Parse(s.AsSpan());
            Assert.AreEqual(d, d2);
        }

    }
}