using System.Diagnostics.CodeAnalysis;
using FolkerKinzel.CsvTools.Intls;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass()]
public class InvalidCsvExceptionTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod()]
    public void InvalidCsvExceptionTest4()
    {
        string message = "Message";

        int lineNumber = 4711;
        int charIndex = 42;


        var e = new CsvFormatException(message, CsvError.FileTruncated, lineNumber, charIndex);

        Assert.IsNotNull(e);

        Assert.AreEqual(message, e.Message);
        Assert.AreEqual(CsvError.FileTruncated, e.Error);

        Assert.AreEqual(lineNumber, e.CsvLineNumber);
        Assert.AreEqual(charIndex, e.CsvCharIndex);
    }

    [TestMethod()]
    public void ToStringTest()
    {
        string message = "Message";

        int lineNumber = 4711;
        int charIndex = 42;

        var e = new CsvFormatException(message, CsvError.InvalidMasking, lineNumber, charIndex);

        string s = e.ToString();


        Assert.IsNotNull(s);

        TestContext.WriteLine(s);
    }
}