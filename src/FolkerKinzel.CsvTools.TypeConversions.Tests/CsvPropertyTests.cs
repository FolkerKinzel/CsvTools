using System;
using FolkerKinzel.CsvTools.TypeConversions.Converters;
using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.TypeConversions.Tests
{
    [TestClass()]
    public class CsvPropertyTests
    {
        [TestMethod()]
        public void CsvPropertyTest1()
        {
            var prop = new CsvColumnNameProperty("Prop", new string[] { "Col1" }, new StringConverter());
            Assert.IsInstanceOfType(prop, typeof(CsvColumnNameProperty));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvPropertyTest2() => _ = new CsvColumnNameProperty(null!, new string[] { "Col1" }, new StringConverter());


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvPropertyTest3() => _ = new CsvColumnNameProperty("Prop", null!, new StringConverter());


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvPropertyTest4() => _ = new CsvColumnNameProperty("Prop", new string[] { "Col1" }, null!);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CsvPropertyTest5()
            => _ = new CsvColumnNameProperty("Ähh", new string[] { "Col1" }, new StringConverter());

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CsvPropertyTest6() 
            => _ = new CsvColumnNameProperty("Prop", new string[] { "Col1" }, new StringConverter(), -7);


        [TestMethod()]
        public void CsvPropertyTest7()
            => _ = new CsvColumnNameProperty("Prop", new string[] { "Col1" }, new StringConverter(), CsvColumnNameProperty.MaxWildcardTimeout + 1);

        [TestMethod()]
        public void CsvPropertyTest8() 
            => _ = new CsvColumnNameProperty("Prop", new string[] { "Col1" }, new StringConverter(), 0);


        [TestMethod()]
        public void CsvIndexPropertyTest9()
        {
            const string propertyName = "myProp";
            var prop = new CsvColumnIndexProperty(propertyName, 0, new StringConverter());

            Assert.IsNotNull(prop);
            Assert.AreEqual(prop.PropertyName, propertyName);
            Assert.IsInstanceOfType(prop.Converter, typeof(StringConverter));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CsvPropertyTest10() => _ = new CsvColumnIndexProperty("propertyName", -1, new StringConverter());

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvPropertyTest11() => _ = new CsvColumnIndexProperty(null!, 17, new StringConverter());


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvPropertyTest12() => _ = new CsvColumnIndexProperty("Prop", 17, null!);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CsvPropertyTest13() => _ = new CsvColumnIndexProperty("Ähh", 17, new StringConverter());
       


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