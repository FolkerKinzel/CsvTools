using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.TypeConversions;
using FolkerKinzel.CsvTools.TypeConversions.Converters;

namespace Examples
{
    public class Pupil
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
                .Append("LessonDay:   ").AppendLine(LessonDay.HasValue
                                                    ? $"{nameof(DayOfWeek)}.{LessonDay.Value}"
                                                    : NULL)
                .Append("LessonBegin: ").AppendLine(LessonBegin.HasValue
                                                    ? LessonBegin.Value.ToString()
                                                    : NULL)
                .ToString();
        }
    }


    public static class DeserializingClassesFromCsv
    {
        public static void TestDeserializingClassesFromCsv()
        {
            const string csvFileName = "Objects.csv";

            // Create a nonstandard CSV-File
            File.WriteAllText(csvFileName, new StringBuilder()
                .AppendLine(
                    "Unterrichtstag;Unterrichtsbeginn;Vollständiger Name;Unterrichtsfach;")
                .AppendLine(
                    "Wednesday;14:30;Susi Meyer;Piano")
                .AppendLine(
                    "Thursday;15:15;Carl Czerny;Piano;")
                .AppendLine(
                    ";;Frederic Chopin")
                .ToString());

            // Initialize a CsvRecordWrapper which retrieves the data from
            // the CSV-Columns and converts it to the right data type.
            // Aliases with wildcards can be used to match the column-headers
            // of the CSV file.
            var wrapper = new CsvRecordWrapper();

            // Reuse a converter for more than one property:
            ICsvTypeConverter2 stringConverter =
                new StringConverter2();

            wrapper.AddProperty
                (
                    new CsvColumnNameProperty("Name",
                                    new string[] { "*name" },
                                    stringConverter)
                );
            wrapper.AddProperty
                (
                    new CsvColumnNameProperty("Subject",
                                    new string[] { "*subject", "*fach" },
                                    stringConverter)
                );
            wrapper.AddProperty
                (
                    new CsvColumnNameProperty("LessonDay",
                                    new string[] { "*day", "*tag" },
                                    new EnumConverter2<DayOfWeek>().MakeNullableStructConverter())
                );
            wrapper.AddProperty
                (
                    new CsvColumnNameProperty("LessonBegin",
                                    new string[] { "*begin?" },
                                    CsvConverterFactory
                                        .CreateConverter(CsvTypeCode.TimeSpan, nullable: true))
                );

            // Analyze the CSV file to determine the right parameters
            // for proper reading:
            var analyzer = new CsvAnalyzer();
            analyzer.Analyze(csvFileName);

            // Read the CSV file:
            using var reader =
                new CsvReader(csvFileName,
                              analyzer.HasHeaderRow,
                              analyzer.Options,
                              analyzer.FieldSeparator);

            var pupilsList = new List<Pupil>();

            foreach (CsvRecord record in reader.Read())
            {
                wrapper.Record = record;

                // Using a dynamic variable enables you to assign
                // the properties without having to explicitely cast them
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
            foreach (Pupil pupil in pupilsList)
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
.
*/
