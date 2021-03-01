using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Helpers.Converters.Tests
{
    [TestClass()]
    public class CsvConverterFactoryTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateConverterTest() => _ = CsvConverterFactory.CreateConverter((CsvTypeCode)4711);

        
    }
}