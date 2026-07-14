using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using FolkerKinzel.Strings;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass]
public class CsvReaderTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void ReadTest1()
    {
        const string testCsv =
            "Spalte \"1\",," + "\r\n" +
            ",Spalte \"2\",";

        CsvRecord[] csv = Csv.Parse(testCsv, isHeaderPresent: false);
        Assert.HasCount(2, csv);
    }

    [TestMethod]
    public void ReadTest2()
    {
        string outDir = Path.Combine(TestContext.TestRunResultsDirectory!, "CsvFilesAnalyzed");
        _ = Directory.CreateDirectory(outDir);

        string file = TestFiles.GoogleCsv;
        using CsvReader csv = Csv.OpenRead(file, options: CsvOpts.None);

        foreach (CsvRecord record in csv)
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
    public void ReadTest3()
    {
        const string testCsv =
            "Spalte \"1\",," + "\r\n" +
            ",Spalte \"2\",";

        using var stringReader = new StringReader(testCsv);
        using CsvReader csv = Csv.OpenRead(stringReader, isHeaderPresent: false);

        stringReader.Dispose();

        _ = Assert.ThrowsExactly<ObjectDisposedException>(() => { foreach (CsvRecord _ in csv) { } });
    }


    [TestMethod]
    public void ReadTest4()
    {
        const string testCsv =
            "Spalte \"1\",," + "\r\n" +
            ",Spalte \"2\",";

        using var stringReader = new StringReader(testCsv);
        using var csv = new CsvReader(stringReader, isHeaderPresent: false);

        _ = csv.FirstOrDefault();
        _ = Assert.ThrowsExactly<ObjectDisposedException>(() => csv.FirstOrDefault());
    }

    [TestMethod]
    public void ReadTest5()
    {
        const string testCsv =
            "Spalte \"1\",," + "\r\n" +
            ",Spalte \"2\"," + "\r\n";

        using var stringReader = new StringReader(testCsv);
        CsvRecord[] csv = Csv.Parse(testCsv, isHeaderPresent: false);

        Assert.HasCount(2, csv);
    }

    [TestMethod]
    public void CsvReaderTest3()
        => Assert.ThrowsExactly<ArgumentNullException>(() => new CsvReader((string?)null!));

    [TestMethod]
    public void CsvReaderTest4()
        => _ = Assert.ThrowsExactly<ArgumentException>(() => new CsvReader("   "));

    [TestMethod]
    public void CsvReaderTest5()
        => Assert.ThrowsExactly<ArgumentNullException>(() => new CsvReader((StreamReader?)null!));

    [TestMethod]
    public void CsvReaderTest6()
    {
        const string csv = """
                Name,City
                Ingrid,Berlin
                Joyce,New York
                Horst,Hamburg
                John,New York
                """;

        using var csvReader = new CsvReader(new StringReader(csv));

        int cnt = 0;

        foreach (CsvRecord record in csvReader)
        {
            cnt++;
        }

        Assert.AreEqual(4, cnt);

        _ = Assert.ThrowsExactly<ObjectDisposedException>(
            () => { foreach (CsvRecord _ in csvReader) { } });
    }

    [TestMethod]
    public void CsvReaderTest7()
    {
        const string csv = """
                Name,City
                Ingrid,Berlin
                Joyce,New York
                Horst,Hamburg
                John,New York
                """;

        using var csvReader = new CsvReader(new StringReader(csv));

        Assert.HasCount(4, csvReader);

        _ = Assert.ThrowsExactly<ObjectDisposedException>(() => csvReader.Count());
    }

    [TestMethod]
    public void MyTestMethod()
    {
        const string csv = """
                Name,City
                Ingrid,Berlin
                Joyce,New York
                Horst,Hamburg
                John,New York
                """;

        using var csvReader = new CsvReader(new StringReader(csv));

        Assert.HasCount(4, csvReader);
    }

    [TestMethod]
    public void ResetTest1()
    {
        using var reader = new CsvReader(new StringReader("Test"));
        _ = Assert.ThrowsExactly<NotSupportedException>(() => ((IEnumerator)reader).Reset());
    }

    [TestMethod]
    public void IEnumerableTest()
    {
        const string csv = "a,b,c";
        using var stringReader = new StringReader(csv);
        using var reader = new CsvReader(stringReader, isHeaderPresent: false);

        IEnumerator enumerator = ((IEnumerable)reader).GetEnumerator();
        Assert.AreSame(reader, enumerator);
        Assert.IsTrue(enumerator.MoveNext());
        Assert.IsNotNull(enumerator.Current);
        Assert.AreEqual(',', reader.Delimiter);
    }

    [TestMethod]
    public void TooMuchFieldsTest()
    {
        const string csv = """
            a,b
            1,2,3
            """;
        _ = Assert.ThrowsExactly<CsvFormatException>(() => Csv.Parse(csv));
    }

    [TestMethod]
    public void EmptyLineTest1()
    {
        const string csv = """
            a,b

            1,2
            """;
        _ = Assert.ThrowsExactly<CsvFormatException>(() => Csv.Parse(csv));
    }

    [TestMethod]
    public void EmptyLineTest2()
    {
        const string csv = """

            a,b

            1,2
            """;

        CsvRecord[] result = Csv.Parse(csv, options: CsvOpts.Default.Unset(CsvOpts.ThrowOnEmptyLines));
        Assert.HasCount(2, result);
    }

    [TestMethod]
    public void TooFewFieldsTest()
    {
        const string csv = """
            a,b,c
            1,2
            """;
        _ = Assert.ThrowsExactly<CsvFormatException>(() => Csv.Parse(csv));
    }

    [TestMethod]
    public void FileTruncatedTest1()
    {
        const string csv = """
            a,b
            1,"2
            """;
        _ = Assert.ThrowsExactly<CsvFormatException>(() => Csv.Parse(csv));
    }

    [TestMethod]
    public void FileTruncatedTest2()
    {
        const string csv = """
            a,b
            1,"2
            """;
        Assert.HasCount(1, Csv.ParseAnalyzed(csv));
    }

    [TestMethod]
    public void FileTruncatedTest3()
    {
        const string csv = """
            a,b
            1, "2


            """;
        CsvRecord[] result = Csv.ParseAnalyzed(csv);
        Assert.HasCount(1, result);
    }
}
