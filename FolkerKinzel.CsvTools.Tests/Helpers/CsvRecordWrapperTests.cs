using FolkerKinzel.CsvTools.Helpers;
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
    }
}