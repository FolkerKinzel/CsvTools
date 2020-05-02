using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Extensions.Tests
{
    [TestClass()]
    public class CsvOptionsExtensionsTests
    {
        [TestMethod()]
        public void SetTest()
        {
            CsvOptions options = CsvOptions.None;

            Assert.IsFalse((options & CsvOptions.DisableCaching) == CsvOptions.DisableCaching);

            options = options.Set(CsvOptions.DisableCaching);

            Assert.IsTrue((options & CsvOptions.DisableCaching) == CsvOptions.DisableCaching);
        }

        [TestMethod()]
        public void IsSetTest()
        {
            CsvOptions options = CsvOptions.ThrowOnEmptyLines | CsvOptions.TrimColumns;

            Assert.IsTrue(options.IsSet(CsvOptions.ThrowOnEmptyLines));
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