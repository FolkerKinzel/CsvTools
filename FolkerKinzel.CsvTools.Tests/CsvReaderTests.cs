using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.CsvTools;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections;

namespace FolkerKinzel.CsvTools.Tests
{
    [TestClass()]
    public class CsvReaderTests
    {
#pragma warning disable CS8618 // Das Non-Nullable-Feld ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".
        public TestContext TestContext { get; set; }
#pragma warning restore CS8618 // Das Non-Nullable-Feld ist nicht initialisiert. Deklarieren Sie das Feld ggf. als "Nullable".

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


        [TestMethod()]
        public void CsvReaderTest2()
        {
            string outDir = Path.Combine(TestContext.TestRunResultsDirectory, "CsvFilesAnalyzed");
            Directory.CreateDirectory(outDir);

            foreach (var file in TestFiles.GetAll().Where(x => StringComparer.OrdinalIgnoreCase.Equals(Path.GetExtension(x), ".CSV")))
            {
                using var Reader = new CsvReader(file, options: CsvOptions.None);

                foreach (var record in Reader.Read())
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var item in record)
                    {
                        sb.Append(item.Key.PadRight(20)).Append(": ").AppendLine(item.Value);
                    }

                    File.WriteAllText(Path.Combine(outDir, Path.GetFileName(file) + ".txt"), sb.ToString());

                    break;
                }
            }
        }

    }
}