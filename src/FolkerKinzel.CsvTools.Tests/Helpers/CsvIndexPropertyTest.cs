using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.CsvTools.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using FolkerKinzel.CsvTools.Helpers.Converters.Intls;
using FolkerKinzel.CsvTools.Helpers.Converters;

namespace FolkerKinzel.CsvTools.Helpers.Tests
{
    [TestClass()]
    public class CsvIndexPropertyTests
    {
        [TestMethod()]
        public void CsvIndexPropertyTest1()
        {
            const string propertyName = "myProp";
            var prop = new CsvProperty(propertyName, 0, new StringConverter(true, false, false));

            Assert.IsNotNull(prop);
            Assert.AreEqual(prop.PropertyName, propertyName);
            Assert.IsInstanceOfType(prop.Converter, typeof(StringConverter));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CsvIndexPropertyTest2() => _ = new CsvProperty("propertyName", -1, new StringConverter(true, false, false));

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvIndexPropertyTest3() => _ = new CsvProperty(null!, 17, CsvConverterFactory.CreateConverter(CsvTypeCode.String));


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvIndexPropertyTest4() => _ = new CsvProperty("Prop", 17, null!);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CsvIndexPropertyTest5() => _ = new CsvProperty("Ähh", 17, CsvConverterFactory.CreateConverter(CsvTypeCode.String));
       

        //[TestMethod()]
        //[Obsolete("Obsolete")]
        //public void CloneTest()
        //{
        //    const string propName = "Prop";
        //    ICsvTypeConverter conv = CsvConverterFactory.CreateConverter(CsvTypeCode.String);


        //    var prop = new CsvIndexProperty(propName, 42, conv);

        //    Assert.IsInstanceOfType(prop, typeof(CsvIndexProperty));

        //    var clone = (CsvIndexProperty)prop.Clone();

        //    Assert.AreNotSame(prop, clone);
        //    Assert.AreEqual(propName, prop.PropertyName, clone.PropertyName);
        //    CollectionAssert.AreEqual(prop.ColumnNameAliases, clone.ColumnNameAliases);
        //    Assert.AreSame(prop.Converter, clone.Converter);
        //}
    }
}