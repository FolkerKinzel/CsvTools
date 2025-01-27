using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass()]
public class CsvOptionsExtensionsTests
{
    [TestMethod()]
    public void SetTest()
    {
        CsvOpts options = CsvOpts.None;

        Assert.IsFalse(options.HasFlag(CsvOpts.DisableCaching));

        options = options.Set(CsvOpts.DisableCaching);

        Assert.IsTrue(options.HasFlag(CsvOpts.DisableCaching));
    }

    [TestMethod()]
    public void IsSetTest1()
    {
        CsvOpts options = CsvOpts.ThrowOnEmptyLines | CsvOpts.TrimColumns;

        Assert.IsTrue(options.HasFlag(CsvOpts.ThrowOnEmptyLines));
    }

    [TestMethod()]
    public void UnsetTest()
    {
        CsvOpts options = CsvOpts.DisableCaching | CsvOpts.ThrowOnEmptyLines;

        Assert.IsTrue(options.HasFlag(CsvOpts.DisableCaching));

        options = options.Unset(CsvOpts.DisableCaching);

        Assert.IsFalse(options.HasFlag(CsvOpts.DisableCaching));
    }
}