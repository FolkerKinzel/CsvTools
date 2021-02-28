using FolkerKinzel.CsvTools.Helpers;
using FolkerKinzel.CsvTools.Helpers.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Helpers.Tests
{
    [TestClass()]
    public class CsvRecordWrapperTests
    {
        [TestMethod()]
        public void CsvRecordWrapperTest()
        {
            Assert.Fail();
        }


        [TestMethod()]
        public void InsertPropertyTest()
        {
            Assert.Fail();
        }


        [TestMethod()]
        public void ReplacePropertyAtTest()
        {
            Assert.Fail();
        }


        [TestMethod()]
        public void ReplacePropertyTest()
        {
            Assert.Fail();
        }


        [TestMethod()]
        public void TrySetMemberTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void TryGetMemberTest()
        {
            Assert.Fail();
        }



        [TestMethod()]
        public void GetEnumeratorTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IndexOfTest()
        {
            var wrapper = new CsvRecordWrapper();

            wrapper.AddProperty(new CsvProperty("Hallo", new string[] { "Hallo" }, Converters.CsvConverterFactory.CreateConverter(Converters.CsvTypeCode.String, true)));

            Assert.AreEqual(0, wrapper.IndexOf("Hallo"));
            Assert.AreEqual(-1, wrapper.IndexOf("Wolli"));
            Assert.AreEqual(-1, wrapper.IndexOf(null));
            Assert.AreEqual(-1, wrapper.IndexOf(string.Empty));
        }

        [TestMethod()]
        public void ContainsTest()
        {
            var wrapper = new CsvRecordWrapper();

            wrapper.AddProperty(new CsvProperty("Hallo", new string[] { "Hallo" }, Converters.CsvConverterFactory.CreateConverter(Converters.CsvTypeCode.String, true)));

            Assert.IsTrue(wrapper.Contains("Hallo"));
            Assert.IsFalse(wrapper.Contains("Wolli"));
            Assert.IsFalse(wrapper.Contains(null));
            Assert.IsFalse(wrapper.Contains(string.Empty));
        }

        [TestMethod()]
        public void AddPropertyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemovePropertyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemovePropertyAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IndexerTest()
        {
            var record = new CsvRecord(2, false, true);
            record[0] = "42";
            record[1] = "43";

            var wrapper = new CsvRecordWrapper();

            ICsvTypeConverter intConverter = CsvConverterFactory.CreateConverter(CsvTypeCode.Int32);

            wrapper.AddProperty(new CsvProperty(record.ColumnNames[0], new string[] { record.ColumnNames[0] }, intConverter));
            wrapper.AddProperty(new CsvProperty(record.ColumnNames[1], new string[] { record.ColumnNames[1] }, intConverter));

            wrapper.Record = record;

            Assert.AreEqual(42, wrapper[0]);
            Assert.AreEqual(43, wrapper[1]);


            dynamic dyn = wrapper;

            _ = Assert.AreEqual(42, dyn[0]);
            _ = Assert.AreEqual(43, dyn[1]);

            int test = dyn[0];
            Assert.AreEqual(42, test);

            test = dyn["Column1"];
            Assert.AreEqual(42, test);

            dyn["Column2"] = 7;
            _ = Assert.AreEqual(7, dyn["Column2"]);

            dyn[0] = 3;
            _ = Assert.AreEqual(3, dyn[0]);
        }
    }
}