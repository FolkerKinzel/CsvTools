using FolkerKinzel.CsvTools;
using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FolkerKinzel.CsvTools.Tests
{
    [TestClass()]
    public class CsvRecordTests
    {
        [TestMethod()]
        public void FillClearTest()
        {
            var data = new ReadOnlyMemory<char>[] { "eins".AsMemory(), "zwei".AsMemory() };
            var rec = new CsvRecord(2);

            Assert.AreEqual(2, rec.Count);

            rec.FillWith(data);

            CollectionAssert.AreEquivalent(data.Select(x => x.ToString()).ToArray(), rec.ToDictionary().Values.Select(x => x.ToString()).ToArray());

            rec.Clear();

            Assert.IsTrue(rec.ToDictionary().Values.All(x => x.IsEmpty));

            Assert.AreEqual(2, rec.Count);
        }

        [TestMethod()]
        public void FillTest2()
        {
            ReadOnlyMemory<char>[] data = ["eins".AsMemory(), "zwei".AsMemory()];
            var rec = new CsvRecord(2);

            Assert.AreEqual(2, rec.Count);

            rec.FillWith(data);
            CollectionAssert.AreEquivalent(data.Select(x => x.ToString()).ToArray(), rec.ToDictionary().Values.Select(x => x.ToString()).ToArray());

            rec.FillWith(["sieben".AsMemory()]);

            CollectionAssert.AreEquivalent(new string?[] { "sieben", "" }, rec.ToDictionary().Values.Select(x => x.ToString()).ToArray());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FillTest3()
        {
            var rec = new CsvRecord(2);

            rec.FillWith(["1".AsMemory(), "2".AsMemory(), "3".AsMemory()]);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void ItemTest1()
        {
            var rec = new CsvRecord(1);

            _ = rec[1];
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void ItemTest2()
        {
            var rec = new CsvRecord(1);

            rec[-1] = default;
        }


        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void FillTest4()
        //{
        //    var rec = new CsvRecord(2);

        //    rec.Fill((IEnumerable<string?>?)null!);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void FillTest5()
        //{
        //    var rec = new CsvRecord(2);

        //    rec.Fill((IEnumerable<ReadOnlyMemory<char>>?)null!);
        //}


        [TestMethod()]
        public void TryGetValueTest1a()
        {
            const string col1 = "col1";
            const string col2 = "col2";

            var rec = new CsvRecord([col1, col2], false, true, false);
            rec[col1] = "1".AsMemory();
            rec[col2] = "2".AsMemory();

            Assert.IsTrue(rec.TryGetValue(col1, out ReadOnlyMemory<char> val1));
            Assert.AreEqual("1", val1.ToString());

            Assert.IsTrue(rec.TryGetValue(col2, out ReadOnlyMemory<char> val2));
            Assert.AreEqual("2", val2.ToString());

            Assert.IsFalse(rec.TryGetValue("bla", out ReadOnlyMemory<char> val3));
            Assert.IsTrue(val3.IsEmpty);

            //Assert.IsFalse(rec.TryGetValue(-1, out ReadOnlyMemory<char> val4));
            //Assert.IsTrue(val4.IsEmpty);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetValueTest1b()
        {
            const string col1 = "col1";
            const string col2 = "col2";

            var rec = new CsvRecord([col1, col2], false, true, false);

            _ = rec.TryGetValue(null!, out ReadOnlyMemory<char> _);
        }


        //[TestMethod()]
        //public void TryGetValueTest2()
        //{
        //    var rec = new CsvRecord(2);
        //    rec.Values[0] = "1".AsMemory();
        //    rec.Values[1] = "2".AsMemory();

        //    Assert.IsTrue(rec.TryGetValue(0, out ReadOnlyMemory<char> val1));
        //    Assert.AreEqual("1", val1.ToString());

        //    Assert.IsTrue(rec.TryGetValue(1, out ReadOnlyMemory<char> val2));
        //    Assert.AreEqual("2", val2.ToString());

        //    Assert.IsFalse(rec.TryGetValue(2, out ReadOnlyMemory<char> val3));
        //    Assert.IsTrue(val3.IsEmpty);

        //    Assert.IsFalse(rec.TryGetValue(-1, out ReadOnlyMemory<char> val4));
        //    Assert.IsTrue(val4.IsEmpty);
        //}


        [TestMethod()]
        public void GetEnumeratorTest1()
        {
            var rec = new CsvRecord(2);
            rec.Values[0] = "1".AsMemory();
            rec.Values[1] = "2".AsMemory();

            Assert.AreEqual(3, rec.Select(x => int.Parse(x.Value!.ToString(), CultureInfo.InvariantCulture)).Sum());
        }

        [TestMethod()]
        public void ToStringTest()
        {
            var rec = new CsvRecord(2);
            rec.Values[0] = "1".AsMemory();
            rec.Values[1] = "2".AsMemory();

            string s = rec.ToString();

            Assert.IsNotNull(s);
            Assert.AreNotEqual(0, s.Length);
        }

        [TestMethod()]
        public void ToDictionaryTest()
        {
            const string col1 = "col1";
            const string col2 = "col2";

            var rec = new CsvRecord([col1, col2], false, true, false);
            rec[col1] = "1".AsMemory();
            rec[col2] = "2".AsMemory();

            var dic = rec.ToDictionary();

            Assert.AreEqual(dic.Comparer, rec.Comparer);

            Assert.AreEqual(dic.Count, rec.Count);

            foreach (KeyValuePair<string, ReadOnlyMemory<char>> kvp in dic)
            {
                Assert.AreEqual(kvp.Value.ToString(), rec[kvp.Key].ToString());
            }
        }

        [TestMethod()]
        public void ContainsColumnTest1()
        {
            const string col1 = "col1";

            var rec = new CsvRecord([col1], false, true, false);

            Assert.IsTrue(rec.ContainsColumn(col1));
            Assert.IsTrue(rec.ContainsColumn(col1.ToUpperInvariant()));

            Assert.IsFalse(rec.ContainsColumn("bla"));
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainsColumnTest2()
        {
            var rec = new CsvRecord([], false, true, false);

            _ = rec.ContainsColumn(null!);
        }


        [TestMethod()]
        public void IndexOfColumnTest()
        {
            const string col1 = "col1";
            const string col2 = "col2";

            var rec = new CsvRecord([col1, col2], false, true, false);

            Assert.AreEqual(0, rec.IndexOfColumn(col1));
            Assert.AreEqual(1, rec.IndexOfColumn(col2));

            Assert.AreEqual(-1, rec.IndexOfColumn("bla"));
            Assert.AreEqual(-1, rec.IndexOfColumn(null));
        }
    }
}