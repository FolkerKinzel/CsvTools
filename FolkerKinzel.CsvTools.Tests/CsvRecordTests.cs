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
        public void TryGetValueTest1a()
        {
            const string col1 = "col1";
            const string col2 = "col2";

            var rec = new CsvRecord(new string[] { col1, col2 }, false, false, true, false);
            rec[col1] = "1";
            rec[col2] = "2";

            Assert.IsTrue(rec.TryGetValue(col1, out string? val1));
            Assert.AreEqual("1", val1);

            Assert.IsTrue(rec.TryGetValue(col2, out string? val2));
            Assert.AreEqual("2", val2);

            Assert.IsFalse(rec.TryGetValue("bla", out string? val3));
            Assert.IsNull(val3);

            Assert.IsFalse(rec.TryGetValue(-1, out string? val4));
            Assert.IsNull(val4);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetValueTest1b()
        {
            const string col1 = "col1";
            const string col2 = "col2";

            var rec = new CsvRecord(new string[] { col1, col2 }, false, false, true, false);

            _ = rec.TryGetValue(null!, out string? _);
        }


        [TestMethod()]
        public void TryGetValueTest2()
        {
            var rec = new CsvRecord(2, true, true);
            rec[0] = "1";
            rec[1] = "2";

            Assert.IsTrue(rec.TryGetValue(0, out string? val1));
            Assert.AreEqual("1", val1);

            Assert.IsTrue(rec.TryGetValue(1, out string? val2));
            Assert.AreEqual("2", val2);

            Assert.IsFalse(rec.TryGetValue(2, out string? val3));
            Assert.IsNull(val3);

            Assert.IsFalse(rec.TryGetValue(-1, out string? val4));
            Assert.IsNull(val4);
        }


        [TestMethod()]
        public void GetEnumeratorTest1()
        {
            var rec = new CsvRecord(2, true, true);
            rec[0] = "1";
            rec[1] = "2";

            Assert.AreEqual(3, rec.Select(x => int.Parse(x.Value!, CultureInfo.InvariantCulture)).Sum());
        }

        


        [TestMethod()]
        public void ToStringTest()
        {
            var rec = new CsvRecord(2, true, true);
            rec[0] = "1";
            rec[1] = "2";

            string s = rec.ToString();

            Assert.IsNotNull(s);
            Assert.AreNotEqual(0, s.Length);
        }


        [TestMethod()]
        public void ToDictionaryTest()
        {
            const string col1 = "col1";
            const string col2 = "col2";

            var rec = new CsvRecord(new string[] { col1, col2 }, false, false, true, false);
            rec[col1] = "1";
            rec[col2] = "2";

            var dic = rec.ToDictionary();

            Assert.AreEqual(dic.Comparer, rec.Comparer);

            Assert.AreEqual(dic.Count, rec.Count);

            foreach (KeyValuePair<string, string?> kvp in dic)
            {
                Assert.AreEqual(kvp.Value, rec[kvp.Key]);
            }
        }

        [TestMethod()]
        public void ContainsColumnTest1()
        {
            const string col1 = "col1";

            var rec = new CsvRecord(new string[] { col1 }, false, false, true, false);

            Assert.IsTrue(rec.ContainsColumn(col1));
            Assert.IsTrue(rec.ContainsColumn(col1.ToUpperInvariant()));

            Assert.IsFalse(rec.ContainsColumn("bla"));
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ContainsColumnTest2()
        {
            var rec = new CsvRecord(new string[0], false, false, true, false);

            _ = rec.ContainsColumn(null!);
        }


        [TestMethod()]
        public void IndexOfColumnTest()
        {
            const string col1 = "col1";
            const string col2 = "col2";

            var rec = new CsvRecord(new string[] { col1, col2 }, false, false, true, false);

            Assert.AreEqual(0, rec.IndexOfColumn(col1));
            Assert.AreEqual(1, rec.IndexOfColumn(col2));

            Assert.AreEqual(-1, rec.IndexOfColumn("bla"));
            Assert.AreEqual(-1, rec.IndexOfColumn(null));
        }
    }
}