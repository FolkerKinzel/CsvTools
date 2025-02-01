using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass]
public class CsvRecordExtensionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FillWithTest1() => new CsvRecord(1).FillWith(Enumerable.Repeat("a", 2));

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FillWithTest2() => ((CsvRecord)null!).FillWith(Enumerable.Repeat("a", 2));

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FillWithTest3() => new CsvRecord(1).FillWith(new string[] { "a", "a" });

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FillWithTest4() => ((CsvRecord)null!).FillWith(new string[] { "a", "a" });


    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FillWithTest5() => new CsvRecord(1).FillWith(new object[] { "a", -42}, CultureInfo.InvariantCulture);

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FillWithTest6() => ((CsvRecord)null!).FillWith(new object[] { "a", -42 }, CultureInfo.InvariantCulture);


    [TestMethod()]
    public void FillWithTest7()
    {
        ReadOnlyMemory<char>[] data = ["eins".AsMemory(), "zwei".AsMemory()];
        var rec = new CsvRecord(2);

        Assert.AreEqual(2, rec.Count);

        rec.FillWith(data);
        CollectionAssert.AreEquivalent(data.Select(x => x.ToString()).ToArray(), rec.ToDictionary().Values.Select(x => x.ToString()).ToArray());

        rec.FillWith(["sieben".AsMemory()], resetExcess: false);

        CollectionAssert.AreEquivalent(new string?[] { "sieben", "zwei" }, rec.ToDictionary().Values.Select(x => x.ToString()).ToArray());

        rec.FillWith(["sechs".AsMemory()], resetExcess: true);

        CollectionAssert.AreEquivalent(new string?[] { "sechs", "" }, rec.ToDictionary().Values.Select(x => x.ToString()).ToArray());
    }

    [TestMethod()]
    public void FillWithTest8()
    {
        string[] data = ["eins", "zwei"];
        var rec = new CsvRecord(2);

        Assert.AreEqual(2, rec.Count);

        rec.FillWith(data);
        CollectionAssert.AreEqual(data, rec.Values.Select(x => x.ToString()).ToArray());

        rec.FillWith(new string[] { "sieben" }, resetExcess: false);

        CollectionAssert.AreEquivalent(new string[] { "sieben", "zwei" }, rec.Values.Select(x => x.ToString()).ToArray());

        rec.FillWith(new string[] { "sechs" }, resetExcess: true);

        CollectionAssert.AreEquivalent(new string?[] { "sechs", "" }, rec.Values.Select(x => x.ToString()).ToArray());
    }

    [TestMethod()]
    public void FillWithTest9()
    {
        IEnumerable<string> data = ["eins", "zwei"];
        var rec = new CsvRecord(2);

        Assert.AreEqual(2, rec.Count);

        rec.FillWith(data);
        CollectionAssert.AreEqual(data.ToArray(), rec.Values.Select(x => x.ToString()).ToArray());

        rec.FillWith(Enumerable.Repeat("sieben", 1), resetExcess: false);

        CollectionAssert.AreEquivalent(new string[] { "sieben", "zwei" }, rec.Values.Select(x => x.ToString()).ToArray());

        rec.FillWith(Enumerable.Repeat("sechs", 1), resetExcess: true);

        CollectionAssert.AreEquivalent(new string?[] { "sechs", "" }, rec.Values.Select(x => x.ToString()).ToArray());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void FillWithTest10() => new CsvRecord(1).FillWith(new ReadOnlyMemory<char>[] { "a".AsMemory(), "a".AsMemory() });

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FillWithTest11() => ((CsvRecord?)null!).FillWith(new ReadOnlyMemory<char>[] { "a".AsMemory(), "a".AsMemory() });

    [TestMethod]
    public void ResetExcessTest1()
    {
        var rec = new CsvRecord(2);
        rec.FillWith(new string[] { "a", "b" });
        Assert.IsFalse(rec[1].IsEmpty);

        rec.FillWith([1], CultureInfo.InvariantCulture);
        Assert.AreEqual("1", rec[0].ToString());
        Assert.IsTrue(rec[1].IsEmpty);
    }
}
