using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.CsvTools;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FolkerKinzel.CsvTools.Tests
{
    [TestClass()]
    public class CsvReaderTests
    {
        [TestMethod()]
        public void CsvReaderTest()
        {
            const string testCsv =
                "Spalte \"1\",," + "\r\n" +
                ",Spalte \"2\",";

            using var stringReader = new StringReader(testCsv);
            using var csvReader = new CsvReader(stringReader, hasHeaderRow: false);

            int counter = 0;
            foreach (var record in csvReader.Read())
            {
                counter++;
            }

            Assert.AreEqual(2, counter);
        }


        [TestMethod()]
        public void CsvReaderTest1()
        {
            Assert.Fail();
        }
    }
}