using System;
using FolkerKinzel.CsvTools.Helpers.Converters;
using FolkerKinzel.CsvTools.Helpers.Converters.Intls;
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
        public void CsvPropertyTest7() => _ = new CsvProperty("Prop", new string[] { "Col1" }, CsvConverterFactory.CreateConverter(CsvTypeCode.String), CsvProperty.MaxWildcardTimeout + 1);

        [TestMethod()]
        public void CsvPropertyTest8() => _ = new CsvProperty("Prop", new string[] { "Col1" }, CsvConverterFactory.CreateConverter(CsvTypeCode.String), 0);


        [TestMethod()]
        public void CsvIndexPropertyTest9()
        {
            const string propertyName = "myProp";
            var prop = new CsvProperty(propertyName, 0, new StringConverter(true, false, false));

            Assert.IsNotNull(prop);
            Assert.AreEqual(prop.PropertyName, propertyName);
            Assert.IsInstanceOfType(prop.Converter, typeof(StringConverter));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CsvPropertyTest10() => _ = new CsvProperty("propertyName", -1, new StringConverter(true, false, false));

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvPropertyTest11() => _ = new CsvProperty(null!, 17, CsvConverterFactory.CreateConverter(CsvTypeCode.String));


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvPropertyTest12() => _ = new CsvProperty("Prop", 17, null!);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CsvPropertyTest13() => _ = new CsvProperty("Ähh", 17, CsvConverterFactory.CreateConverter(CsvTypeCode.String));
       


        //[TestMethod()]
        //[Obsolete("Obsolete")]
        //public void CloneTest1()
        //{
        //    const string propName = "Prop";
        //    string[] aliases = new string[] { "Col1", "Other" };
        //    ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.String);


        //    var prop = new CsvProperty(propName, aliases, conv);

        //    Assert.IsInstanceOfType(prop, typeof(CsvProperty));

        //    var clone = (CsvProperty)prop.Clone();

        //    Assert.AreNotSame(prop, clone);
        //    Assert.AreEqual(propName, prop.PropertyName, clone.PropertyName);
        //    CollectionAssert.AreEqual(prop.ColumnNameAliases, clone.ColumnNameAliases);
        //    Assert.AreSame(prop.Converter, clone.Converter);
        //}
    }
}