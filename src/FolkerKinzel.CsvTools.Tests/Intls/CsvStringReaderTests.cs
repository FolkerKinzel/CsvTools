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
        Assert.HasCount(1, list);
        string result = list[0].ToString();
        Assert.AreEqual(result, input);
    }

    [TestMethod]
    public void MaskedFieldTest1()
    {
        const string csv = " \"1\"\"2\"  ,3, ";
        CsvRecord[] result = Csv.Parse(csv, isHeaderPresent: false);

        Assert.HasCount(1, result);
        Assert.HasCount(3, result[0]);
        Assert.AreEqual("1\"2", result[0][0].ToString());
        Assert.AreEqual("3", result[0][1].ToString());
        Assert.AreEqual(" ", result[0][2].ToString());
    }

    [TestMethod]
    public void InvalidMaskingTest1()
    {
        const string csv = """
                    "1"bla,3
                    a,b
                    """;
        _ = Assert.ThrowsExactly<CsvFormatException>(
            () => Csv.Parse(csv, isHeaderPresent: false));
    }

    [TestMethod]
    public void MaskedFieldWithoutAllocationTest()
    {
        const string csv = "\"a,b\",c";
        CsvRecord[] result = Csv.Parse(csv, isHeaderPresent: false);

        Assert.HasCount(1, result);
        Assert.HasCount(2, result[0]);
        Assert.AreEqual("a,b", result[0][0].ToString());
        Assert.AreEqual("c", result[0][1].ToString());
    }
}
