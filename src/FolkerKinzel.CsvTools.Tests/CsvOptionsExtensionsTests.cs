using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Tests
{
    [TestClass()]
    public class CsvOptionsExtensionsTests
    {
        [TestMethod()]
        public void SetTest()
        {
            CsvOpts options = CsvOpts.None;

            Assert.IsFalse(options.HasFlag(CsvOpts.EnableCaching));

            options = options.Set(CsvOpts.EnableCaching);

            Assert.IsTrue(options.HasFlag(CsvOpts.EnableCaching));
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
            CsvOpts options = CsvOpts.EnableCaching | CsvOpts.ThrowOnEmptyLines;

            Assert.IsTrue(options.HasFlag(CsvOpts.EnableCaching));

            options = options.Unset(CsvOpts.EnableCaching);

            Assert.IsFalse(options.HasFlag(CsvOpts.EnableCaching));
        }
    }
}