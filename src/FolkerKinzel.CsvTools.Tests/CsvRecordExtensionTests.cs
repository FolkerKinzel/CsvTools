using System.Globalization;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass]
public class CsvRecordExtensionTests
{
    [TestMethod]
    public void FillWithTest1()
        => _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new CsvRecord(1).FillWith(Enumerable.Repeat("a", 2)));

    [TestMethod]
    public void FillWithTest2()
        => Assert.ThrowsExactly<ArgumentNullException>(
            () => ((CsvRecord)null!).FillWith(Enumerable.Repeat("a", 2)));

    [TestMethod]
    public void FillWithTest3()
        => _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new CsvRecord(1).FillWith(new string[] { "a", "a" }));

    [TestMethod]
    public void FillWithTest4()
        => Assert.ThrowsExactly<ArgumentNullException>(
            () => ((CsvRecord)null!).FillWith(new string[] { "a", "a" }));


    [TestMethod]
    public void FillWithTest5()
        => _ = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new CsvRecord(1).FillWith(["a", -42], CultureInfo.InvariantCulture));

    [TestMethod]
    public void FillWithTest6()
        => Assert.ThrowsExactly<ArgumentNullException>(
            () => ((CsvRecord)null!).FillWith(["a", -42], CultureInfo.InvariantCulture));


    [TestMethod]
    public void FillWithTest7()
    {
        ReadOnlyMemory<char>[] data = ["eins".AsMemory(), "zwei".AsMemory()];
        var rec = new CsvRecord(2);

        Assert.AreEqual(2, rec.Count);

        rec.FillWith(data);
        Assert.AreSequenceEqual([.. data.Select(x => x.ToString())],
                                [.. rec.ToDictionary().Values.Select(x => x.ToString())],
                                SequenceOrder.InAnyOrder);

        rec.FillWith(["sieben".AsMemory()], resetExcess: false);

        Assert.AreSequenceEqual(new string?[] { "sieben", "zwei" },
                                [.. rec.ToDictionary().Values.Select(x => x.ToString())],
                                SequenceOrder.InAnyOrder);

        rec.FillWith(["sechs".AsMemory()], resetExcess: true);

        Assert.AreSequenceEqual(new string?[] { "sechs", "" },
                                [.. rec.ToDictionary().Values.Select(x => x.ToString())],
                                SequenceOrder.InAnyOrder);
    }

    [TestMethod]
    public void FillWithTest8()
    {
        string[] data = ["eins", "zwei"];
        var rec = new CsvRecord(2);

        Assert.AreEqual(2, rec.Count);

        rec.FillWith(data);
        Assert.AreSequenceEqual(data, [.. rec.Values.Select(x => x.ToString())]);

        rec.FillWith(new string[] { "sieben" }, resetExcess: false);

        Assert.AreSequenceEqual(["sieben", "zwei"],
                                [.. rec.Values.Select(x => x.ToString())],
                                SequenceOrder.InAnyOrder);

        rec.FillWith(new string[] { "sechs" }, resetExcess: true);

        Assert.AreSequenceEqual(new string?[] { "sechs", "" },
                               [.. rec.Values.Select(x => x.ToString())],
                               SequenceOrder.InAnyOrder);
    }

    [TestMethod]
    public void FillWithTest9()
    {
        IEnumerable<string> data = ["eins", "zwei"];
        var rec = new CsvRecord(2);

        Assert.AreEqual(2, rec.Count);

        rec.FillWith(data);
        Assert.AreSequenceEqual([.. data], [.. rec.Values.Select(x => x.ToString())]);

        rec.FillWith(Enumerable.Repeat("sieben", 1), resetExcess: false);

        Assert.AreSequenceEqual(["sieben", "zwei"],
                                [.. rec.Values.Select(x => x.ToString())],
                                SequenceOrder.InAnyOrder);

        rec.FillWith(Enumerable.Repeat("sechs", 1), resetExcess: true);

        Assert.AreSequenceEqual(new string?[] { "sechs", "" },
                                [.. rec.Values.Select(x => x.ToString())],
                                SequenceOrder.InAnyOrder);
    }

    [TestMethod]
    public void FillWithTest10()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new CsvRecord(1).FillWith(["a".AsMemory(), "a".AsMemory()]));

    [TestMethod]
    public void FillWithTest11()
        => Assert.ThrowsExactly<ArgumentNullException>(
            () => ((CsvRecord?)null!).FillWith(["a".AsMemory(), "a".AsMemory()]));

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
