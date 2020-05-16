# FolkerKinzel.CsvTools
[![NuGet](https://img.shields.io/nuget/v/FolkerKinzel.CsvTools)](https://www.nuget.org/packages/FolkerKinzel.CsvTools/)


.NET-library to read and write CSV-files.

The library contains
* `CsvReader`: Reads CSV from files and streams. It enables you to perform Linq-Queries on CSV files.
* `CsvWriter`: Writes CSV to files and stream.
* `CsvRecordWrapper`: Maps the columns of the CSV file to the columns of your `DataTable` or to the 
properties of an `object` and performs type conversions. `CsvRecordWrapper` supports aliases with wildcards
for CSV column names and ships with a lot of 
supporting classes - most are data type converters for the most needed data types and an easy to implement
interface to create converters of whatever data type you need.
* `CsvAnalyzer`: Analyzes CSV files to get the right parameters for proper reading. This enables you, to read
CSV files, that don't fit the standard.

```
nuget Package Manager:
PM> Install-Package FolkerKinzel.CsvTools -Version 1.3.0

.NET CLI:
> dotnet add package FolkerKinzel.CsvTools --version 1.3.0

PackageReference (Visual Studio Project File):
<PackageReference Include="FolkerKinzel.CsvTools" Version="1.3.0" />

Paket CLI:
> paket add FolkerKinzel.CsvTools --version 1.3.0
```

* [Download Reference (English)](https://github.com/FolkerKinzel/CsvTools/blob/master/FolkerKinzel.CsvTools.Reference.en/Help/FolkerKinzel.CsvTools.en.chm)

* [Projektdokumentation (Deutsch) herunterladen](https://github.com/FolkerKinzel/CsvTools/blob/master/FolkerKinzel.CsvTools.Doku.de/Help/FolkerKinzel.CsvTools.de.chm)

> IMPORTANT: On some systems, the content of the CHM file is blocked. Before extracting it,
>  right click on the file, select Properties, and check the "Allow" checkbox - if it 
> is present - in the lower right corner of the General tab in the Properties dialog.

## Example Code
_(For better readability exception handling is ommitted in the following examples.)_

#### Linq-Query on a CSV file:
```csharp
using FolkerKinzel.CsvTools;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Examples
{
    static class LinqOnCsvFile
    {
        public static void TestLinqOnCsvFile()
        {
            const string csvFileName = "LinqTest.csv";

            File.WriteAllText(csvFileName, new StringBuilder()
                .AppendLine("Name,City")
                .AppendLine("Ingrid,Berlin")
                .AppendLine("Joyce,New York")
                .AppendLine("Horst,Hamburg")
                .AppendLine("John,New York")
                .ToString());

            using var csvReader = new CsvReader(csvFileName);
            Console.Write("How many people live in New York?: ");
            Console.WriteLine(csvReader.Read().Where(x => x["City"] == "New York").Count());

            // Console Output: How many people live in New York?: 2
        }
    }
}
```

#### CSV from and to DataTable:

```csharp
using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Helpers;
using FolkerKinzel.CsvTools.Helpers.Converters;
using System;
using System.Data;
using System.Globalization;
using System.IO;

namespace Examples
{
    static class CsvToDataTable
    {
        const string PupilsName = "Name";
        const string Subject = "Subject";
        const string LessonDay = "Day";
        const string LessonBegin = "Begin";

        const string fileName = "DataTable.csv";

        public static void TestCsvToDataTable()
        {
            using DataTable dataTable = InitDataTable();

            CsvRecordWrapper wrapper = InitCsvRecordWrapper();

            // Write the CSV-file:
            // (We can sort the columns of the csv file differently than those of the DataTable - 
            // CsvRecordWrapper will reorder that.)
            string[] columns = new string[] { Subject, LessonBegin, PupilsName, LessonDay };
            using (CsvWriter writer = new CsvWriter(fileName, columns))
            {
                // (CsvWriter reuses the same record.)
                wrapper.Record = writer.Record;

                foreach (DataRow? obj in dataTable.Rows)
                {
                    if (obj is DataRow dataRow)
                    {
                        // The properties of the CsvRecordWrapper match the columns of
                        // the DataTable in data type and order (but not the columns of the CSV-file).
                        for (int i = 0; i < wrapper.Count; i++)
                        {
                            wrapper[i] = dataRow[i];
                        }

                        writer.WriteRecord();
                    }
                }
            }

            dataTable.Clear();

            // Refill the DataTable from the CSV-file:
            using (CsvReader reader = new CsvReader(fileName))
            {
                foreach (CsvRecord record in reader.Read())
                {
                    wrapper.Record = record;
                    var dataRow = dataTable.NewRow();
                    dataTable.Rows.Add(dataRow);

                    // It doesn't matter that the columns in the csv-file have a
                    // different order than the columns of the DataTable:
                    // CsvRecordWrapper reorders that for us.
                    for (int i = 0; i < wrapper.Count; i++)
                    {
                        dataRow[i] = wrapper[i];
                    }
                }
            }

            WriteConsole(dataTable);

            // Console output:
            // Csv file:
            //
            // Subject,Begin,Name,Day
            // Piano,14:30:00,Susi Meyer, Wednesday
            // Piano,15:15:00,Carl Czerny, Thursday
            // ,, Frederic Chopin,
            //
            //
            // Content of the refilled DataTable:
            // Susi Meyer      Piano           3               14:30:00
            // Carl Czerny     Piano           4               15:15:00
            // Frederic Chopin <DBNull>        <DBNull>        <DBNull>
        }


        private static CsvRecordWrapper InitCsvRecordWrapper()
        {
            CsvRecordWrapper wrapper = new CsvRecordWrapper();

            // Store the stringConverter because you can reuse the same converter for more than one property
            // in CsvRecordWrapper.
            var stringConverter = CsvConverterFactory.CreateConverter(CsvTypeCode.String, maybeDBNull: true);

            wrapper.AddProperty(new CsvProperty(PupilsName, new string[] { PupilsName }, stringConverter));
            wrapper.AddProperty(new CsvProperty(Subject, new string[] { Subject }, stringConverter));
            wrapper.AddProperty(
                new CsvProperty(
                        LessonDay,
                        new string[] { LessonDay },
                        CsvConverterFactory.CreateEnumConverter<DayOfWeek>("G", maybeDBNull: true)));
            wrapper.AddProperty(
                new CsvProperty(
                        LessonBegin,
                        new string[] { LessonBegin },
                        CsvConverterFactory.CreateConverter(CsvTypeCode.TimeSpan, maybeDBNull: true)));

            return wrapper;
        }


        private static DataTable InitDataTable()
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add(new DataColumn(PupilsName));
            dataTable.Columns.Add(new DataColumn(Subject));
            dataTable.Columns.Add(new DataColumn(LessonDay, typeof(DayOfWeek)));
            dataTable.Columns.Add(new DataColumn(LessonBegin, typeof(TimeSpan)));

            dataTable.Rows.Add(new object[] { "Susi Meyer", "Piano", DayOfWeek.Wednesday, new TimeSpan(14, 30, 0) });
            dataTable.Rows.Add(new object[] { "Carl Czerny", "Piano", DayOfWeek.Thursday, new TimeSpan(15, 15, 0) });
            dataTable.Rows.Add(new object[] { "Frederic Chopin" });

            return dataTable;
        }


        private static void WriteConsole(DataTable dataTable)
        {
            Console.WriteLine("Csv file:");
            Console.WriteLine();
            Console.WriteLine(File.ReadAllText(fileName));

            Console.WriteLine();
            Console.WriteLine("Content of the refilled DataTable:");

            foreach (DataRow? dataRow in dataTable.Rows)
            {
                if (dataRow is null)
                {
                    continue;
                }

                const int padding = 15;

                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    object o = dataRow[i];
                    Console.Write(o switch
                    {
                        null => "<null>".PadRight(padding),
                        object dbNull when dbNull == DBNull.Value => "<DBNull>".PadRight(padding),
                        string s when s.Length == 0 => "\"\"".PadRight(padding),
                        TimeSpan ts => ts.ToString("g", CultureInfo.InvariantCulture).PadRight(padding),
                        _ => o.ToString()?.PadRight(padding)
                    });
                    Console.Write(' ');
                }

                Console.WriteLine();
            }
        }
    }
}
```


#### Deserialize object from CSV:

```csharp
using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Helpers;
using FolkerKinzel.CsvTools.Helpers.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Examples
{
    class Pupil
    {
        public string? Name { get; set; }
        public string? Subject { get; set; }
        public DayOfWeek? LessonDay { get; set; }
        public TimeSpan? LessonBegin { get; set; }

        public override string ToString()
        {
            const string NULL = "<null>";
            return new StringBuilder()
                .Append("Name:        ").AppendLine(Name ?? NULL)
                .Append("Subject:     ").AppendLine(Subject ?? NULL)
                .Append("LessonDay:   ").AppendLine(LessonDay.HasValue ? $"{nameof(DayOfWeek)}.{LessonDay.Value}" : NULL)
                .Append("LessonBegin: ").AppendLine(LessonBegin.HasValue ? LessonBegin.Value.ToString() : NULL)
                .ToString();
        }
    }


    static class ObjectFromCsv
    {
        public static void TestObjectFromCsv()
        {
            const string csvFileName = "Objects.csv";

            // Create a nonstandard CSV-File
            File.WriteAllText(csvFileName, new StringBuilder()
                .AppendLine("Unterrichtstag;Unterrichtsbeginn;Vollständiger Name;Unterrichtsfach;")
                .AppendLine("Wednesday;14:30;Susi Meyer;Piano")
                .AppendLine("Thursday;15:15;Carl Czerny;Piano;")
                .AppendLine(";;Frederic Chopin")
                .ToString());

            // Initialize a CsvRecordWrapper that retrieves the data from
            // the CSV-Columns and converts it to the right data type
            // Aliases with wildcards can be used to match the column-headers
            // of the csv-file.
            CsvRecordWrapper wrapper = new CsvRecordWrapper();

            // reuse a converter for more than one property
            var stringConverter = CsvConverterFactory.CreateConverter(CsvTypeCode.String, nullable: true);

            wrapper.AddProperty(new CsvProperty("Name", new string[] { "*name" }, stringConverter));
            wrapper.AddProperty(new CsvProperty("Subject", new string[] { "*subject", "*fach" }, stringConverter));
            wrapper.AddProperty(
                new CsvProperty(
                    "LessonDay",
                    new string[] { "*day", "*tag" },
                    CsvConverterFactory.CreateEnumConverter<DayOfWeek>(nullable: true)));
            wrapper.AddProperty(
                new CsvProperty(
                    "LessonBegin",
                    new string[] { "*begin?" },
                    CsvConverterFactory.CreateConverter(CsvTypeCode.TimeSpan, nullable: true)));

            // Analyze the CSV-file to determine the right parameters
            // for proper reading:
            CsvAnalyzer analyzer = new CsvAnalyzer();
            analyzer.Analyze(csvFileName);

            // Read the CSV-file:
            using CsvReader reader = 
                    new CsvReader(csvFileName, analyzer.HasHeaderRow, analyzer.Options, analyzer.FieldSeparator);

            List<Pupil> pupilsList = new List<Pupil>();

            foreach (CsvRecord record in reader.Read())
            {
                wrapper.Record = record;

                // Using a dynamic variable enables you, to assign
                // the properties without having to exlicitely cast them
                // to the target data type:
                dynamic dynWrapper = wrapper;

                pupilsList.Add(new Pupil
                {
                    Name = dynWrapper.Name,
                    LessonBegin = dynWrapper.LessonBegin,
                    LessonDay = dynWrapper.LessonDay,
                    Subject = dynWrapper.Subject
                });
            }

            // Write the results to Console:
            foreach (var pupil in pupilsList)
            {
                Console.WriteLine(pupil);
                Console.WriteLine();
            }
        }
    }
}

/*
Console output: 

Name:        Susi Meyer
Subject:     Piano
LessonDay:   DayOfWeek.Wednesday
LessonBegin: 14:30:00


Name:        Carl Czerny
Subject:     Piano
LessonDay:   DayOfWeek.Thursday
LessonBegin: 15:15:00


Name:        Frederic Chopin
Subject:     <null>
LessonDay:   <null>
LessonBegin: <null>
*/

```

