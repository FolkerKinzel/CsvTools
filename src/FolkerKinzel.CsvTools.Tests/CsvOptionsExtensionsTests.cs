using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Tests
{
    [TestClass()]
    public class CsvOptionsExtensionsTests
    {
        [TestMethod()]
        public void SetTest()
        {
            CsvOptions options = CsvOptions.None;

            Assert.IsFalse(options.HasFlag(CsvOptions.DisableCaching));

            options = options.Set(CsvOptions.DisableCaching);

            Assert.IsTrue(options.HasFlag(CsvOptions.DisableCaching));
        }

        [TestMethod()]
        public void IsSetTest1()
        {
            CsvOptions options = CsvOptions.ThrowOnEmptyLines | CsvOptions.TrimColumns;

            Assert.IsTrue(options.HasFlag(CsvOptions.ThrowOnEmptyLines));
        }

        

        [TestMethod()]
        public void UnsetTest()
        {
            CsvOptions options = CsvOptions.DisableCaching | CsvOptions.ThrowOnEmptyLines;

            Assert.IsTrue((options & CsvOptions.DisableCaching) == CsvOptions.DisableCaching);

            options = options.Unset(CsvOptions.DisableCaching);

            Assert.IsFalse((options & CsvOptions.DisableCaching) == CsvOptions.DisableCaching);
        }
    }
}