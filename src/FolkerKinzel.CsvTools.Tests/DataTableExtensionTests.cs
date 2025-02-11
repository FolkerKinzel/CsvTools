using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.CsvTools.Tests;

[TestClass]
public class DataTableExtensionTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void WriteCsvTest1()
    {
        using var dataTable = new DataTable();
        _ = dataTable.Columns.Add("A", typeof(int));
        _ = dataTable.Columns.Add("B", typeof(int));

        _ = dataTable.Rows.Add(1,2);
        _ = dataTable.Rows.Add(3);
        _ = dataTable.Rows.Add(4, 5);
        dataTable.AcceptChanges();

        dataTable.Rows[2].Delete();

        using var stringWriter = new StringWriter();
        dataTable.WriteCsv(stringWriter, null,  CultureInfo.InvariantCulture);

        string csv = stringWriter.ToString();
        Assert.AreEqual("""
            A,B
            1,2
            3,
            """, csv);
    }

    [TestMethod]
    public void WriteCsvTest2()
    {
        using var dataTable = new DataTable();
        _ = dataTable.Columns.Add("A");
        _ = dataTable.Columns.Add("B");

        _ = dataTable.Rows.Add("1", "2");
        _ = dataTable.Rows.Add("3");
        _ = dataTable.Rows.Add("4", "5");
        dataTable.AcceptChanges();

        dataTable.Rows[2].Delete();

        using var stringWriter = new StringWriter();
        dataTable.WriteCsv(stringWriter, null, CultureInfo.InvariantCulture);

        string csv = stringWriter.ToString();
        Assert.AreEqual("""
            A,B
            1,2
            3,
            """, csv);
    }

    [TestMethod]
    public void WriteCsvTest3()
    {
        using var dataTable = new DataTable();
        _ = dataTable.Columns.Add("A", typeof(char));
        _ = dataTable.Columns.Add("B", typeof(char));

        _ = dataTable.Rows.Add('1', '2');
        _ = dataTable.Rows.Add('3');
        _ = dataTable.Rows.Add('4', '5');
        dataTable.AcceptChanges();

        dataTable.Rows[2].Delete();

        using var stringWriter = new StringWriter();
        dataTable.WriteCsv(stringWriter, null, null);

        string csv = stringWriter.ToString();
        Assert.AreEqual("""
            A,B
            1,2
            3,
            """, csv);
    }

    [TestMethod]
    public void WriteCsvTest4()
    {
        using var dataTable = new DataTable();
        _ = dataTable.Columns.Add("A", typeof(int));
        _ = dataTable.Columns.Add("B", typeof(int));

        _ = dataTable.Rows.Add(1, 2);
        _ = dataTable.Rows.Add(3);
        _ = dataTable.Rows.Add(4, 5);
        dataTable.AcceptChanges();

        dataTable.Rows[2].Delete();

        string filePath = Path.Combine(TestContext.TestRunResultsDirectory!, "WriteCsvTest4.csv");
        dataTable.WriteCsv(filePath, ["B"], CultureInfo.InvariantCulture);

        string csv = File.ReadAllText(filePath);
        Assert.AreEqual("""
            B
            2
            
            """, csv);
    }
}
