using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.CsvTools.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using FolkerKinzel.CsvTools.Helpers.Converters.Intls;

namespace FolkerKinzel.CsvTools.Helpers.Tests
{
    [TestClass()]
    public class CsvIndexPropertyTests
    {
        [TestMethod()]
        public void CsvIndexPropertyTest()
        {
            const string propertyName = "myProp";
            var prop = new CsvIndexProperty(propertyName, 0, new StringConverter(true, false));

            Assert.IsNotNull(prop);
            Assert.AreEqual(prop.PropertyName, propertyName);
            Assert.IsInstanceOfType(prop.Converter, typeof(StringConverter));
        }

        [TestMethod()]
        public void CloneTest()
        {
            Assert.Fail();
        }
    }
}