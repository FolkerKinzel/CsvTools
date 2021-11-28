using System;
using System.Data;
using System.Globalization;
using System.IO;
using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.TypeConversions;
using FolkerKinzel.CsvTools.TypeConversions.Converters;

namespace Examples
{
    public static class CsvToDataTable
    {
        private const string PUPILS_NAME = "Name";
        private const string SUBJECT = "Subject";
        private const string LESSON_DAY = "Day";
        private const string LESSON_BEGIN = "Begin";
        private const string FILE_NAME = "DataTable.csv";

        public static void TestCsvToDataTable()
        {
            using DataTable dataTable = InitDataTable();

            CsvRecordWrapper wrapper = InitCsvRecordWrapper();

            // Write the CSV file:
            // (We can sort the columns of the CSV file differently than those 
            // of the DataTable - CsvRecordWrapper will reorder that.)
            string[] columns = 
                new string[] { SUBJECT, LESSON_BEGIN, PUPILS_NAME, LESSON_DAY };

            using (var writer = new CsvWriter(FILE_NAME, columns))
            {
                // (CsvWriter reuses the same record.)
                wrapper.Record = writer.Record;

                foreach (DataRow? obj in dataTable.Rows)
                {
                    if (obj is DataRow dataRow)
                    {
                        // The properties of the CsvRecordWrapper match the columns
                        // of the DataTable in data type and order (but not the 
                        // columns of the CSV file).
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
            using (var reader = new CsvReader(FILE_NAME))
            {
                foreach (CsvRecord record in reader.Read())
                {
                    wrapper.Record = record;
                    DataRow dataRow = dataTable.NewRow();
                    dataTable.Rows.Add(dataRow);

                    // It doesn't matter that the columns in the CSV file have a
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
            var wrapper = new CsvRecordWrapper();

            // Store the stringConverter because you can reuse the same 
            // converter for more than one property in CsvRecordWrapper.
            ICsvTypeConverter stringConverter = 
                CsvConverterFactory.CreateConverter(CsvTypeCode.String, maybeDBNull: true);

            wrapper.AddProperty
                (
                    new CsvColumnNameProperty(PUPILS_NAME,
                                    new string[] { PUPILS_NAME },
                                    stringConverter)
                );
            wrapper.AddProperty
                (
                    new CsvColumnNameProperty(SUBJECT,
                                    new string[] { SUBJECT },
                                    stringConverter)
                );
            wrapper.AddProperty
                (
                    new CsvColumnNameProperty(LESSON_DAY,
                                    new string[] { LESSON_DAY },
                                    CsvConverterFactory
                                        .CreateEnumConverter<DayOfWeek>("G", maybeDBNull: true))
                );
            wrapper.AddProperty
                (
                    new CsvColumnNameProperty(LESSON_BEGIN,
                                    new string[] { LESSON_BEGIN },
                                    CsvConverterFactory
                                        .CreateConverter(CsvTypeCode.TimeSpan, maybeDBNull: true))
                );

            return wrapper;
        }


        private static DataTable InitDataTable()
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add(new DataColumn(PUPILS_NAME));
            dataTable.Columns.Add(new DataColumn(SUBJECT));
            dataTable.Columns.Add(new DataColumn(LESSON_DAY, typeof(DayOfWeek)));
            dataTable.Columns.Add(new DataColumn(LESSON_BEGIN, typeof(TimeSpan)));

            _ = dataTable.Rows.Add(
                new object[] { "Susi Meyer", "Piano", DayOfWeek.Wednesday, new TimeSpan(14, 30, 0) });
            _ = dataTable.Rows.Add(
                new object[] { "Carl Czerny", "Piano", DayOfWeek.Thursday, new TimeSpan(15, 15, 0) });
            _ = dataTable.Rows.Add(
                new object[] { "Frederic Chopin" });

            return dataTable;
        }


        private static void WriteConsole(DataTable dataTable)
        {
            Console.WriteLine("Csv file:");
            Console.WriteLine();
            Console.WriteLine(File.ReadAllText(FILE_NAME));

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
                        DBNull dBNull => "<DBNull>".PadRight(padding),
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
