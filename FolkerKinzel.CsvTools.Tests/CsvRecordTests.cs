using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Tests
{
    [TestClass()]
    public class CsvRecordTests
    {
        [TestMethod()]
        public void FillClearTest()
        {
            string[] data = new string[] { "eins", "zwei" };
            var rec = new CsvRecord(2, false, true);

            Assert.AreEqual(2, rec.Count);

            rec.Fill(data);

            CollectionAssert.AreEquivalent(data, rec.ToDictionary().Values);

            rec.Clear();

            Assert.IsTrue(rec.ToDictionary().Values.All(x => x is null));

            Assert.AreEqual(2, rec.Count);
        }

        [TestMethod()]
        public void FillTest2()
        {
            string[] data = new string[] { "eins", "zwei" };
            var rec = new CsvRecord(2, false, true);

            Assert.AreEqual(2, rec.Count);

            rec.Fill(data);
            CollectionAssert.AreEquivalent(data, rec.ToDictionary().Values);

            rec.Fill(new string[] { "sieben" });

            CollectionAssert.AreEquivalent(new string?[] { "sieben", null }, rec.ToDictionary().Values);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FillTest3()
        {
            var rec = new CsvRecord(2, false, true);

            rec.Fill(new string[] { "1", "2", "3" });

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FillTest4()
        {
            var rec = new CsvRecord(2, false, true);

            rec.Fill(null!);

        }


        [TestMethod()]
        public void TryGetValueTest()
        {
            Assert.Fail();
        }

        

        [TestMethod()]
        public void ContainsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ContainsTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ContainsKeyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CopyToTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CopyToTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IndexOfTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IndexOfKeyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Assert.Fail();
        }
    }
}