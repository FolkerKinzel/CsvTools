using FolkerKinzel.CsvTools.Intls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass()]
public class InvalidCsvExceptionTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }


    //[TestMethod()]
    //public void InvalidCsvExceptionTest1()
    //{
    //    var e = new CsvFormatException();

    //    Assert.IsNotNull(e);
    //    Assert.IsNotNull(e.Message);
    //}

    //[TestMethod()]
    //public void InvalidCsvExceptionTest2()
    //{
    //    string message = "Message";

    //    var e = new CsvFormatException(message);

    //    Assert.IsNotNull(e);
    //    Assert.AreEqual(message, e.Message);
    //}

    //[TestMethod()]
    //public void InvalidCsvExceptionTest3()
    //{
    //    string message = "Message";

    //    var inner = new Exception();

    //    var e = new CsvFormatException(message,  inner);

    //    Assert.IsNotNull(e);
    //    Assert.AreSame(inner, e.InnerException);
    //    Assert.AreEqual(message, e.Message);
    //}

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

//        [TestMethod()]
//        public void GetObjectDataTest()
//        {
//            string message = "Message";

//            int lineNumber = 4711;
//            int charIndex = 42;

//            var e = new InvalidCsvException(message, lineNumber, charIndex);


//            using var memStream = new MemoryStream();
//#pragma warning disable SYSLIB0011 // Type or member is obsolete
//            var bf = new BinaryFormatter();
//#pragma warning restore SYSLIB0011 // Type or member is obsolete
//            bf.Serialize(memStream, e);

//            memStream.Position = 0;

//            var e2 = (InvalidCsvException)bf.Deserialize(memStream);

//            Assert.AreEqual(e.Message, e2.Message);
//            Assert.AreEqual(e.CsvLineNumber, e2.CsvLineNumber);
//            Assert.AreEqual(e.CsvCharIndex, e2.CsvCharIndex);
//        }


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