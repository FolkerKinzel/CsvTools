using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using FolkerKinzel.CsvTools.TypeConversions;
using FolkerKinzel.CsvTools.TypeConversions.Converters;

namespace FolkerKinzel.CsvTools.Extensions.Tests
{
#if !NET45
    internal class TestCollection : KeyedCollection<string, CsvPropertyBase>
    {
        protected override string GetKeyForItem(CsvPropertyBase item) => item.PropertyName;
    }


    [TestClass()]
    public class PropertyCollectionExtensionsTests
    {
        [TestMethod()]
        public void TryGetValueTest()
        {
            KeyedCollection<string, CsvPropertyBase> kColl = new TestCollection();

            var prop1 = new CsvColumnNameProperty("Test", new string[0], CsvConverterFactory.CreateConverter(CsvTypeCode.Boolean));

            kColl.Add(prop1); 

            Assert.IsTrue(kColl.TryGetValue("Test", out CsvPropertyBase? prop2));

            Assert.AreEqual(prop1, prop2);
        }
    }

#endif
}
