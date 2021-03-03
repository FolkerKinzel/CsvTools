using System;
using FolkerKinzel.CsvTools.Helpers.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Helpers.Tests
{
    [TestClass()]
    public class CsvPropertyTests
    {
        [TestMethod()]
        public void CsvPropertyTest1()
        {
            var prop = new CsvProperty("Prop", new string[] { "Col1" }, CsvConverterFactory.CreateConverter(CsvTypeCode.String));
            Assert.IsInstanceOfType(prop, typeof(CsvProperty));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvPropertyTest2() => _ = new CsvProperty(null!, new string[] { "Col1" }, CsvConverterFactory.CreateConverter(CsvTypeCode.String));


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvPropertyTest3() => _ = new CsvProperty("Prop", null!, CsvConverterFactory.CreateConverter(CsvTypeCode.String));


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvPropertyTest4() => _ = new CsvProperty("Prop", new string[] { "Col1" }, null!);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CsvPropertyTest5() => _ = new CsvProperty("Ähh", new string[] { "Col1" }, CsvConverterFactory.CreateConverter(CsvTypeCode.String));

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CsvPropertyTest6() => _ = new CsvProperty("Prop", new string[] { "Col1" }, CsvConverterFactory.CreateConverter(CsvTypeCode.String), -7);


        [TestMethod()]
        public void CloneTest1()
        {
            const string propName = "Prop";
            string[] aliases = new string[] { "Col1", "Other" };
            ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.String);


            var prop = new CsvProperty(propName, aliases, conv);

            Assert.IsInstanceOfType(prop, typeof(CsvProperty));

            var clone = (CsvProperty)prop.Clone();

            Assert.AreNotSame(prop, clone);
            Assert.AreEqual(propName, prop.PropertyName, clone.PropertyName);
            CollectionAssert.AreEqual(prop.ColumnNameAliases, clone.ColumnNameAliases);
            Assert.AreSame(prop.Converter, clone.Converter);
        }
    }
}