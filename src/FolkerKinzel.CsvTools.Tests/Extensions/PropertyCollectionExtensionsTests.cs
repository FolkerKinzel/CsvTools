using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using FolkerKinzel.CsvTools.TypeConversions;
using FolkerKinzel.CsvTools.TypeConversions.Converters;

namespace FolkerKinzel.CsvTools.Extensions.Tests
{
#if !NET45
    internal class TestCollection : KeyedCollection<string, CsvProperty>
    {
        protected override string GetKeyForItem(CsvProperty item) => item.PropertyName;
    }


    [TestClass()]
    public class PropertyCollectionExtensionsTests
    {
        [TestMethod()]
        public void TryGetValueTest()
        {
            KeyedCollection<string, CsvProperty> kColl = new TestCollection();

            var prop1 = new CsvProperty("Test", new string[0], CsvConverterFactory.CreateConverter(CsvTypeCode.Boolean));

            kColl.Add(prop1); 

            Assert.IsTrue(kColl.TryGetValue("Test", out CsvProperty? prop2));

            Assert.AreEqual(prop1, prop2);
        }
    }

#endif
}
