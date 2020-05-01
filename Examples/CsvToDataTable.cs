using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Helpers;
using FolkerKinzel.CsvTools.Helpers.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

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
            // Frederic Chopin <DBNull>        < DBNull >      < DBNull >

        }

        

        private static CsvRecordWrapper InitCsvRecordWrapper()
        {
            CsvRecordWrapper wrapper = new CsvRecordWrapper();

            // Store the stringConverter because you can reuse the same converter for more than one property
            // in CsvRecordWrapper.
            var stringConverter = CsvConverterFactory.CreateConverter(CsvTypeCode.String, maybeDBNull: true);

            wrapper.AddProperty(new CsvProperty(PupilsName, new string[] { PupilsName }, stringConverter));
            wrapper.AddProperty(new CsvProperty(Subject, new string[] { Subject }, stringConverter));
            wrapper.AddProperty(new CsvProperty(LessonDay, new string[] { LessonDay }, CsvConverterFactory.CreateEnumConverter<DayOfWeek>("G", maybeDBNull: true)));
            wrapper.AddProperty(new CsvProperty(LessonBegin, new string[] { LessonBegin }, CsvConverterFactory.CreateConverter(CsvTypeCode.TimeSpan, maybeDBNull: true)));

            return wrapper;
        }

        private static DataTable InitDataTable()
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add(
                new DataColumn(PupilsName));

            dataTable.Columns.Add(
                new DataColumn(Subject)
                );

            dataTable.Columns.Add(
                new DataColumn(LessonDay, typeof(DayOfWeek))
                );

            dataTable.Columns.Add(
                new DataColumn(LessonBegin, typeof(TimeSpan))
                );

            dataTable.Rows.Add(new object[] { "Susi Meyer", "Piano", DayOfWeek.Wednesday, new TimeSpan(14,30,0) });
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

            foreach(DataRow? dataRow in dataTable.Rows)
            {
                if(dataRow is null)
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
