using System;
using System.Collections.Generic;
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
        using var reader = new CsvStringReader(stringReader, ',', true);

        List<ReadOnlyMemory<char>>? list = reader.Read();
        Assert.IsNotNull(list);
        Assert.AreEqual(1, list!.Count);
        Assert.AreEqual(list![0].ToString(), input);

    }
}
