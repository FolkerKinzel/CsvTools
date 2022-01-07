using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.CsvTools.TypeConversions.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;

namespace FolkerKinzel.CsvTools.TypeConversions.Converters.Tests
{
    [TestClass()]
    public class IEnumerableTConverterTests
    {
        [TestMethod]
        public void MyTestMethod()
        {

            var list = new List<int?>();
            list.Add(7);
            list.Add(9);
            list.Add(11);


            ICsvTypeConverter2 conv = new Int32Converter().AsNullable().MakeIEnumerableConverter();
            

            string? s = conv.ConvertToString(list);

            var result = (IEnumerable<int?>?)conv.Parse(s);
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(list, result!.ToList());
        }

        [TestMethod()]
        public void IEnumerableTConverterTest()
        {
            IEnumerable<int> arr1 = new int[] { 1, 2, 3 };
            var conv = new Int32Converter().MakeIEnumerableConverter();

            var wrapper = new CsvRecordWrapper();
            var prop = new CsvColumnIndexProperty("TestProp", 0, conv);
            wrapper.AddProperty(prop);

            using var writer = new StringWriter();
            using var csvWriter = new CsvWriter(writer, 1);

            wrapper.Record = csvWriter.Record;
            wrapper[0] = arr1;

            csvWriter.WriteRecord();

            string csv = writer.ToString();

            using var reader = new StringReader(csv);
            using var csvReader = new CsvReader(reader, false);

            wrapper.Record = csvReader.Read().First();

            dynamic dynWrapper = wrapper;

            IEnumerable<int> arr2 = dynWrapper.TestProp;

            CollectionAssert.AreEqual(arr1.ToArray(), arr2.ToArray());
        }

        [TestMethod()]
        public void ParseTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ConvertToStringTest()
        {
            Assert.Fail();
        }
    }
}