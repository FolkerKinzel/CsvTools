using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Intls.Tests;

[TestClass]
public class CsvStringReaderTests
{
    [TestMethod]
    public void FieldStartsWithEmptyLineTest()
    {
        string input = Environment.NewLine + "Hello";

        string csv = "\"" + input + "\"";

        using var stringReader = new StringReader(csv);
        using var reader = new CsvStringReader(stringReader, ',', CsvOpts.Default);

        List<ReadOnlyMemory<char>>? list = reader.Read();
        Assert.IsNotNull(list);
        Assert.AreEqual(1, list!.Count);
        string result = list[0].ToString();
        Assert.AreEqual(result, input);
    }

    [TestMethod]
    public void MaskedFieldTest1()
    {
        const string csv = " \"1\"\"2\"  ,3, ";
        CsvRecord[] result = Csv.Parse(csv, isHeaderPresent: false);

        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(3, result[0].Count);
        Assert.AreEqual("1\"2", result[0][0].ToString());
        Assert.AreEqual("3", result[0][1].ToString());
        Assert.AreEqual(" ", result[0][2].ToString());
    }

    [TestMethod]
    [ExpectedException(typeof(CsvFormatException))]
    public void InvalidMaskingTest1()
    {
        const string csv = """
                    "1"bla,3
                    a,b
                    """;
        _ = Csv.Parse(csv, isHeaderPresent: false);
    }

    [TestMethod]
    public void MaskedFieldWithoutAllocationTest()
    {
        const string csv = "\"a,b\",c";
        var result = Csv.Parse(csv, isHeaderPresent: false);

        Assert.AreEqual(1, result.Length);
        Assert.AreEqual(2, result[0].Count);
        Assert.AreEqual("a,b", result[0][0].ToString());
        Assert.AreEqual("c", result[0][1].ToString());
    }
}
