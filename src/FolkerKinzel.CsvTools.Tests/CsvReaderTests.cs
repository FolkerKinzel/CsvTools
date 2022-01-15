using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Collections;

using FolkerKinzel.Strings;

#if !NETCOREAPP3_1
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.CsvTools.Tests
{
    //public class CsvToolsTests<T>
    //{
    //    public virtual T? Test(T? val = default )
    //    {
    //        return val;
    //    }
    //}

    //public class Foo : CsvToolsTests<int>
    //{
    //    public Foo()
    //    {
    //        this.Val = Test();
    //    }

    //    public int Val { get; }

    //    public override int Test(int val = 0) => base.Test(val);
    //}

    public class CsvReaderTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod()]
        public void ReadTest1()
        {
            const string testCsv =
                "Spalte \"1\",," + "\r\n" +
                ",Spalte \"2\",";

            using var stringReader = new StringReader(testCsv);
            using var csvReader = new CsvReader(stringReader, hasHeaderRow: false);

            int counter = 0;
            foreach (CsvRecord record in csvReader)
            {
                counter++;
            }

            Assert.AreEqual(2, counter);
        }


        [TestMethod()]
        public void ReadTest2()
        {
            string outDir = Path.Combine(TestContext.TestRunResultsDirectory, "CsvFilesAnalyzed");
            _ = Directory.CreateDirectory(outDir);

            string file = TestFiles.GoogleCsv;
            using var Reader = new CsvReader(file, options: CsvOptions.None);

            foreach (CsvRecord record in Reader)
            {
                var sb = new StringBuilder();

                foreach (KeyValuePair<string, ReadOnlyMemory<char>> item in record)
                {
                    _ = sb.Append(item.Key.PadRight(20)).Append(": ").Append(item.Value.Span).AppendLine();
                }

                File.WriteAllText(Path.Combine(outDir, Path.GetFileName(file) + ".txt"), sb.ToString());

                break;
            }
        }


        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ReadTest3()
        {
            const string testCsv =
                "Spalte \"1\",," + "\r\n" +
                ",Spalte \"2\",";

            using var stringReader = new StringReader(testCsv);
            using var csvReader = new CsvReader(stringReader, hasHeaderRow: false);

            stringReader.Dispose();

            foreach (CsvRecord _ in csvReader)
            {

            }
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadTest4()
        {
            const string testCsv =
                "Spalte \"1\",," + "\r\n" +
                ",Spalte \"2\",";

            using var stringReader = new StringReader(testCsv);
            using var csvReader = new CsvReader(stringReader, hasHeaderRow: false);

            _ = csvReader.GetEnumerator();
            _ = csvReader.GetEnumerator();
        }


        [TestMethod()]
        public void ReadTest5()
        {
            const string testCsv =
                "Spalte \"1\",," + "\r\n" +
                ",Spalte \"2\",";

            using var stringReader = new StringReader(testCsv);
            using var csvReader = new CsvReader(stringReader, hasHeaderRow: false);

            IEnumerable numerable = csvReader;

            int counter = 0;
            foreach (object? record in numerable)
            {
                counter++;
            }

            Assert.AreEqual(2, counter);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvReaderTest3()
        {
            using var _ = new CsvReader((string?)null!);
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void CsvReaderTest4()
        {
            using var _ = new CsvReader("   ");
        }


        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CsvReaderTest5()
        {
            using var _ = new CsvReader((StreamReader?)null!);
        }

        
    }
}