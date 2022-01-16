using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Intls.Tests
{
    [TestClass]
    public class CsvStringReaderTests
    {
        [TestMethod]
        public void FieldStartsWithEmptyLineTest()
        {
            string input = Environment.NewLine + "Hello";

            string csv = "\"" + input + "\"";

            using var stringReader = new StringReader(csv);
            using var reader = new CsvStringReader(stringReader, ',', true);

            string? field = reader.First().First();

            Assert.IsNotNull(field);
            Assert.AreEqual(field, input);

        }
    }
}
