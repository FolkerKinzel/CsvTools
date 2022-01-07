using System;
using System.Collections.Generic;
using System.Globalization;
using FolkerKinzel.CsvTools.TypeConversions;
using FolkerKinzel.CsvTools.TypeConversions.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.TypeConversions.Tests
{
    [TestClass()]
    public class CsvRecordWrapperTests
    {
        [TestMethod()]
        public void CsvRecordWrapperTest()
        {
            var wrapper = new CsvRecordWrapper();
            Assert.IsInstanceOfType(wrapper, typeof(CsvRecordWrapper));
        }


        [TestMethod()]
        public void InsertPropertyTest1()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";
            const string prop2Name = "Prop2";

            var conv = new StringConverter2();
            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                conv);

            var prop2 =
                new CsvColumnNameProperty(prop2Name, new string[] { "Hallo2" },
                conv);

            wrapper.AddProperty(prop2);
            Assert.AreEqual(1, wrapper.Count);
            Assert.AreEqual(prop2Name, wrapper.PropertyNames[0]);

            wrapper.InsertProperty(0, prop1);

            Assert.AreEqual(2, wrapper.Count);
            Assert.AreEqual(prop1Name, wrapper.PropertyNames[0]);
            Assert.AreEqual(prop2Name, wrapper.PropertyNames[1]);
        }

        [TestMethod()]
        public void InsertPropertyTest2()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";


            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                new StringConverter2());


            wrapper.InsertProperty(0, prop1);
            Assert.AreEqual(1, wrapper.Count);
            Assert.AreEqual(prop1Name, wrapper.PropertyNames[0]);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InsertPropertyTest3()
        {
            var wrapper = new CsvRecordWrapper();

            wrapper.InsertProperty(0, null!);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertPropertyTest4()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";


            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                new StringConverter2());


            wrapper.InsertProperty(4711, prop1);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void InsertPropertyTest5()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";

            var conv = new StringConverter2();

            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                conv);

            var prop2 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo2" },
                conv);

            wrapper.AddProperty(prop2);
            wrapper.InsertProperty(0, prop1);
        }


        [TestMethod()]
        public void ReplacePropertyAtTest1()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";
            const string prop2Name = "Prop2";
            const string prop3Name = "Prop3";

            var conv = new StringConverter2();


            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                conv);

            var prop2 =
                new CsvColumnNameProperty(prop2Name, new string[] { "Hallo2" },
                conv);

            var prop3 =
                new CsvColumnNameProperty(prop3Name, new string[] { "Hallo3" },
                conv);

            wrapper.AddProperty(prop1);
            wrapper.AddProperty(prop2);

            Assert.AreEqual(2, wrapper.Count);
            Assert.AreEqual(prop1Name, wrapper.PropertyNames[0]);
            Assert.AreEqual(prop2Name, wrapper.PropertyNames[1]);

            wrapper.ReplacePropertyAt(0, prop3);

            Assert.AreEqual(2, wrapper.Count);
            Assert.AreEqual(prop3Name, wrapper.PropertyNames[0]);
            Assert.AreEqual(prop2Name, wrapper.PropertyNames[1]);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ReplacePropertyAtTest2()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";
            const string prop2Name = "Prop2";

            var conv = new StringConverter2();


            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                conv);

            var prop2 =
                new CsvColumnNameProperty(prop2Name, new string[] { "Hallo2" },
                conv);

            var prop3 =
                new CsvColumnNameProperty(prop2Name, new string[] { "Hallo3" },
                conv);

            wrapper.AddProperty(prop1);
            wrapper.AddProperty(prop2);

            wrapper.ReplacePropertyAt(0, prop3);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ReplacePropertyAtTest3()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";
            const string prop2Name = "Prop2";
            const string prop3Name = "Prop3";

            var conv = new StringConverter2();

            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                conv);

            var prop2 =
                new CsvColumnNameProperty(prop2Name, new string[] { "Hallo2" },
                conv);

            var prop3 =
                new CsvColumnNameProperty(prop3Name, new string[] { "Hallo3" },
                conv);

            wrapper.AddProperty(prop1);
            wrapper.AddProperty(prop2);

            wrapper.ReplacePropertyAt(4711, prop3);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplacePropertyAtTest4()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";

            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                new StringConverter2());


            wrapper.AddProperty(prop1);

            wrapper.ReplacePropertyAt(0, null!);
        }

        [TestMethod()]
        public void ReplacePropertyTest1()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";
            const string prop2Name = "Prop2";
            const string prop3Name = "Prop3";

            var conv = new StringConverter2();


            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                conv);

            var prop2 =
                new CsvColumnNameProperty(prop2Name, new string[] { "Hallo2" },
                conv);

            var prop3 =
                new CsvColumnNameProperty(prop3Name, new string[] { "Hallo3" },
                conv);

            wrapper.AddProperty(prop1);
            wrapper.AddProperty(prop2);

            Assert.AreEqual(2, wrapper.Count);
            Assert.AreEqual(prop1Name, wrapper.PropertyNames[0]);
            Assert.AreEqual(prop2Name, wrapper.PropertyNames[1]);

            wrapper.ReplaceProperty(prop1Name, prop3);

            Assert.AreEqual(2, wrapper.Count);
            Assert.AreEqual(prop3Name, wrapper.PropertyNames[0]);
            Assert.AreEqual(prop2Name, wrapper.PropertyNames[1]);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ReplacePropertyTest2a()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";
            const string prop2Name = "Prop2";

            var conv = new StringConverter2();


            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                conv);

            var prop2 =
                new CsvColumnNameProperty(prop2Name, new string[] { "Hallo2" },
                conv);

            var prop3 =
                new CsvColumnNameProperty(prop2Name, new string[] { "Hallo3" },
                conv);

            wrapper.AddProperty(prop1);
            wrapper.AddProperty(prop2);

            wrapper.ReplaceProperty(prop1Name, prop3);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ReplacePropertyTest2b()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";

            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                new StringConverter2());

            wrapper.ReplaceProperty("bla", prop1);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplacePropertyTest3()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";

            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                new StringConverter2());


            wrapper.AddProperty(prop1);

            wrapper.ReplaceProperty(null!, prop1);
        }



        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplacePropertyTest4()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";

            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                new StringConverter2());


            wrapper.AddProperty(prop1);

            wrapper.ReplaceProperty(prop1Name, null!);
        }




        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TrySetMemberTest()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";

            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                new Int32Converter().AsNullable());

            wrapper.AddProperty(prop1);

            dynamic dyn = wrapper;

            dyn.Prop1 = 42;
        }


        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryGetMemberTest()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";

            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                new Int32Converter().AsNullable());

            wrapper.AddProperty(prop1);

            dynamic dyn = wrapper;

            _ = dyn.Prop1;
        }


        [DataTestMethod]
        [DataRow(0)]
        [DataRow(CsvColumnNameProperty.MaxWildcardTimeout)]
        [DataRow(CsvColumnNameProperty.MaxWildcardTimeout + 1)]
        public void DynPropTest(int wildcardTimeout)
        {
            var rec = new CsvRecord(new string[] { "Hallo1", "Blabla" }, false, false, true, false);

            var wrapper = new CsvRecordWrapper
            {
                Record = rec
            };

            const string prop1Name = "Prop1";
            const string prop2Name = "Prop2";


            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] {"Hallo1" },
                new Int32Converter().AsNullable());

            wrapper.AddProperty(prop1);

            var prop2 =
                new CsvColumnNameProperty(prop2Name, new string[] {"Blub", null!, "Bla*" },
                new StringConverter2(),
                wildcardTimeout);

            wrapper.AddProperty(prop2);

            dynamic dyn = wrapper;

            const int val = 42;

            dyn.Prop1 = val;
            int i = dyn.Prop1;

            Assert.AreEqual(val, i);

            const string prop2Value = "HullyGully";
            dyn.Prop2 = prop2Value;
            string? s = dyn.Prop2;

            Assert.AreEqual(prop2Value, s);


        }


        [TestMethod()]
        public void GetEnumeratorTest1()
        {
            var rec = new CsvRecord(3, false, true);

            var wrapper = new CsvRecordWrapper
            {
                Record = rec
            };

            const string prop1Name = "Prop1";
            const string prop2Name = "Prop2";
            const string prop3Name = "Prop3";

            ICsvTypeConverter2 conv = new Int32Converter().AsNullable();

            var prop1 =
                new CsvColumnIndexProperty(prop1Name, 0, conv);

            var prop2 =
                new CsvColumnIndexProperty(prop2Name, 1, conv);

            var prop3 =
                new CsvColumnIndexProperty(prop3Name, 2, conv);

            wrapper.AddProperty(prop1);
            wrapper.AddProperty(prop2);
            wrapper.AddProperty(prop3);

            dynamic dyn = wrapper;

            dyn.Prop1 = 1;
            dyn.Prop2 = 2;
            dyn.Prop3 = 3;


            foreach (KeyValuePair<string, object> kvp in dyn)
            {
                switch (kvp.Key)
                {
                    case prop1Name:
                        Assert.AreEqual(1, (int)kvp.Value);
                        break;
                    case prop2Name:
                        Assert.AreEqual(2, (int)kvp.Value);
                        break;
                    case prop3Name:
                        Assert.AreEqual(3, (int)kvp.Value);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }


        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetEnumeratorTest2()
        {
            var wrapper = new CsvRecordWrapper();

            foreach (KeyValuePair<string, object?> _ in wrapper)
            {

            }
        }


        [TestMethod()]
        public void IndexOfTest()
        {
            var wrapper = new CsvRecordWrapper();

            wrapper.AddProperty(new CsvColumnNameProperty("Hallo", new string[] { "Hallo" }, new StringConverter2()));

            Assert.AreEqual(0, wrapper.IndexOf("Hallo"));
            Assert.AreEqual(-1, wrapper.IndexOf("Wolli"));
            Assert.AreEqual(-1, wrapper.IndexOf(null));
            Assert.AreEqual(-1, wrapper.IndexOf(string.Empty));
        }

        [TestMethod()]
        public void ContainsTest()
        {
            var wrapper = new CsvRecordWrapper();

            wrapper.AddProperty(new CsvColumnNameProperty("Hallo", new string[] { "Hallo" }, new StringConverter2()));

            Assert.IsTrue(wrapper.Contains("Hallo"));
            Assert.IsFalse(wrapper.Contains("Wolli"));
            Assert.IsFalse(wrapper.Contains(null));
            Assert.IsFalse(wrapper.Contains(string.Empty));
        }


        [TestMethod()]
        public void AddPropertyTest1()
        {
            var wrapper = new CsvRecordWrapper();

            Assert.AreEqual(0, wrapper.Count);

            var prop =
                new CsvColumnNameProperty("Hallo", new string[] { "Hallo" },
                new StringConverter2());

            wrapper.AddProperty(prop);

            Assert.AreEqual(1, wrapper.Count);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddPropertyTest2()
        {
            var wrapper = new CsvRecordWrapper();

            wrapper.AddProperty(null!);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AddPropertyTest3()
        {
            var wrapper = new CsvRecordWrapper();

            var conv = new StringConverter2();

            var prop1 =
                new CsvColumnNameProperty("Hallo", new string[] { "Hallo" },
                conv);

            var prop2 =
                new CsvColumnNameProperty("Hallo", new string[] { "Hallo" },
                conv);

            wrapper.AddProperty(prop1);
            wrapper.AddProperty(prop2);
        }


        [TestMethod()]
        public void RemovePropertyTest1()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";
            const string prop2Name = "Prop2";

            var conv = new StringConverter2();


            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                conv);

            var prop2 =
                new CsvColumnNameProperty(prop2Name, new string[] { "Hallo2" },
                conv);

            wrapper.AddProperty(prop1);
            wrapper.AddProperty(prop2);

            Assert.AreEqual(2, wrapper.Count);
            Assert.AreEqual(prop1Name, wrapper.PropertyNames[0]);
            Assert.AreEqual(prop2Name, wrapper.PropertyNames[1]);

            Assert.IsTrue(wrapper.RemoveProperty(prop1Name));

            Assert.AreEqual(1, wrapper.Count);
            Assert.AreEqual(prop2Name, wrapper.PropertyNames[0]);
        }


        [TestMethod()]
        public void RemovePropertyTest2()
        {
            var wrapper = new CsvRecordWrapper();
            Assert.IsFalse(wrapper.RemoveProperty("bla"));
        }


        [TestMethod()]
        public void RemovePropertyAtTest1()
        {
            var wrapper = new CsvRecordWrapper();

            const string prop1Name = "Prop1";
            const string prop2Name = "Prop2";

            var conv = new StringConverter2();

            var prop1 =
                new CsvColumnNameProperty(prop1Name, new string[] { "Hallo1" },
                conv);

            var prop2 =
                new CsvColumnNameProperty(prop2Name, new string[] { "Hallo2" },
                conv);

            wrapper.AddProperty(prop1);
            wrapper.AddProperty(prop2);

            Assert.AreEqual(2, wrapper.Count);
            Assert.AreEqual(prop1Name, wrapper.PropertyNames[0]);
            Assert.AreEqual(prop2Name, wrapper.PropertyNames[1]);

            wrapper.RemovePropertyAt(0);

            Assert.AreEqual(1, wrapper.Count);
            Assert.AreEqual(prop2Name, wrapper.PropertyNames[0]);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RemovePropertyAtTest2()
        {
            var wrapper = new CsvRecordWrapper();
            wrapper.RemovePropertyAt(42);
        }


        [TestMethod()]
        public void IndexerTest()
        {
            var record = new CsvRecord(2, false, true);
            record[0] = "42";
            record[1] = "43";

            var wrapper = new CsvRecordWrapper();

            ICsvTypeConverter2 intConverter = CsvConverterFactory2.CreateConverter(CsvTypeCode.Int32);

            wrapper.AddProperty(new CsvColumnNameProperty(record.ColumnNames[0], new string[] { record.ColumnNames[0] }, intConverter));
            wrapper.AddProperty(new CsvColumnNameProperty(record.ColumnNames[1], new string[] { record.ColumnNames[1] }, intConverter));

            wrapper.Record = record;

            Assert.AreEqual(42, wrapper[0]);
            Assert.AreEqual(43, wrapper[1]);


            dynamic dyn = wrapper;

            Assert.AreEqual(42, dyn[0]);
            Assert.AreEqual(43, dyn[1]);

            int test = dyn[0];
            Assert.AreEqual(42, test);

            test = dyn["Column1"];
            Assert.AreEqual(42, test);

            dyn["Column2"] = 7;
            Assert.AreEqual(7, dyn["Column2"]);

            dyn[0] = 3;
            Assert.AreEqual(3, dyn[0]);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            var rec = new CsvRecord(3, false, true);

            var wrapper = new CsvRecordWrapper();

            string s = wrapper.ToString();
            Assert.IsNotNull(s);
            Assert.AreNotEqual(0, s.Length);

            wrapper.Record = rec;

            s = wrapper.ToString();
            Assert.IsNotNull(s);
            Assert.AreNotEqual(0, s.Length);

            const string prop1Name = "Prop1";
            const string prop2Name = "Prop2";
            const string prop3Name = "Prop3";

            ICsvTypeConverter2 conv = CsvConverterFactory2.CreateConverter(CsvTypeCode.Int32,
                                                                           CsvConverterOptions.ThrowsOnParseErrors | CsvConverterOptions.AcceptsDBNull);
                                                                                   

            var prop1 =
                new CsvColumnIndexProperty(prop1Name, 0, conv);

            var prop2 =
                new CsvColumnIndexProperty(prop2Name, 1, conv);

            var prop3 =
                new CsvColumnIndexProperty(prop3Name, 2, conv);

            wrapper.AddProperty(prop1);
            wrapper.AddProperty(prop2);
            wrapper.AddProperty(prop3);

            s = wrapper.ToString();
            Assert.IsNotNull(s);
            Assert.AreNotEqual(0, s.Length);

            dynamic dyn = wrapper;

            dyn.Prop1 = 1;
            dyn.Prop2 = 2;
            dyn.Prop3 = 3;

            s = wrapper.ToString();
            Assert.IsNotNull(s);
            Assert.AreNotEqual(0, s.Length);

            rec[0] = "bla";

            s = wrapper.ToString();
            Assert.IsNotNull(s);
            Assert.AreNotEqual(0, s.Length);
        }
    }
}