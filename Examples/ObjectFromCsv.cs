﻿using FolkerKinzel.CsvTools;
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

            wrapper.AddProperty(
                new CsvProperty("Name", new string[] { "*name" }, stringConverter)
                );
            wrapper.AddProperty(
                new CsvProperty("Subject", new string[] { "*subject", "*fach" }, stringConverter)
                );
            wrapper.AddProperty(
                new CsvProperty("LessonDay", new string[] { "*day", "*tag" }, CsvConverterFactory.CreateEnumConverter<DayOfWeek>(nullable: true))
                );
            wrapper.AddProperty(
                new CsvProperty("LessonBegin", new string[] { "*begin?" }, CsvConverterFactory.CreateConverter(CsvTypeCode.TimeSpan, nullable: true))
                );


            // Analyze the CSV-file to determine the right parameters
            // for proper reading:
            CsvAnalyzer analyzer = new CsvAnalyzer();
            analyzer.Analyze(csvFileName);


            // Read the CSV-file:
            using CsvReader reader = new CsvReader(csvFileName, analyzer.HasHeaderRow, analyzer.Options, analyzer.FieldSeparator);

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