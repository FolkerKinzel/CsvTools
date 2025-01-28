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


    [TestMethod()]
    public void FillWithTest5()
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
    public void FillWithTest7()
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
    public void FillWithTest8()
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
    public void FillWithTest9() => new CsvRecord(1).FillWith(new ReadOnlyMemory<char>[] { "a".AsMemory(), "a".AsMemory() });

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FillWithTest10() => ((CsvRecord?)null!).FillWith(new ReadOnlyMemory<char>[] { "a".AsMemory(), "a".AsMemory() });

}
