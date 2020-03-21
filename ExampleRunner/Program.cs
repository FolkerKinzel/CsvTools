using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FolkerKinzel.CsvTools;
using FolkerKinzel.CsvTools.Helpers;
using FolkerKinzel.CsvTools.Helpers.Converters;
using FolkerKinzel.CsvTools.Helpers.Converters.Internals;

namespace ExampleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var dtoConverter = CsvConverterFactory.CreateConverter(CsvTypeCode.TimeSpan);

            string? dt = dtoConverter.ConvertToString(DateTime.Now.TimeOfDay);

            TimeSpan dto = (TimeSpan)dtoConverter.Parse(dt)!;


            var conv = CsvConverterFactory.CreateConverter(CsvTypeCode.Int32, true);


            uint iNull = 44;

            var hexConv = CsvConverterFactory.CreateHexConverter(CsvTypeCode.UInt32);

            string? converted = hexConv.ConvertToString(iNull);
            
            

            string csv = "Spalte1;Spalte2;Spalte3" + Environment.NewLine +
                         "Wert1;\"Wert2;" + Environment.NewLine + Environment.NewLine + "Rest\";" + Environment.NewLine +
                         "Wert1;Wert3;Wert4" + Environment.NewLine;

            using var reader = new StringReader(csv);
            using var csvReader = new CsvReader(reader);

            //CsvRecord[] arr = csvReader.Read().Where(x => x["spalte1"] == "Wert1").ToArray();


            //foreach (var record in (new CsvReader2(reader)).Read())
            //{
            //    Console.WriteLine(record);
            //}

            //object o = Guid.NewGuid();

            //Console.WriteLine(o);
            //Console.WriteLine(ObjectToStringConverter.ConvertToString(o));


            using var textWriter = new StringWriter();
            using var writer = new CsvWriter(textWriter, new string[] { "Col1", "Col2" });

            var wrapper = new CsvRecordWrapper();

            var converter = new StringConverter();

            var prop1 = new CsvProperty("Spalte1", new string[] { "Spa?te1", "co*1" }, converter);

            var prop2 = new CsvProperty("Spalte2", new string[] { "Col2", "nichtda", "col2" }, converter);

            wrapper.AddProperty(prop1);
            wrapper.AddProperty(prop2);

            //wrapper.UnregisterProperty(prop1.PropertyName);

            //wrapper.ReplaceProperty("Spalte2", prop1);

            wrapper.SetRecord(writer.Record);


            //wrapper.RegisterProperty(prop1);

            wrapper[0] = "Hallo";

            dynamic dynWrapper = wrapper;

            dynWrapper.Spalte2 = "Folker";

            string? s1 = dynWrapper[0];

            string? s2 = dynWrapper.Spalte2;

            dynWrapper.Spalte2 = 17;


            //var intConv = new CsvIntConverter();

            //string? s = intConv.ConvertBack((int?)null);


            //string test = "FischXXXmärktXXXtreiben";

            //int tests = 500000;



            //string pattern = WildCardToRegular("*fisch*MÄrkt*TRE?BEN*");

            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline);
            //watch.Stop();

            //bool res2 = false;

            //for (int i = 0; i < tests; i++)
            //{
            //    watch.Start();
            //    res2 = regex.IsMatch(test);
            //    watch.Stop();
            //}

            //Console.WriteLine(watch.ElapsedMilliseconds);

            //watch.Reset();



            //watch.Start();
            //regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.Compiled);
            //watch.Stop();


            //for (int i = 0; i < tests; i++)
            //{
            //    watch.Start();
            //    res2 = regex.IsMatch(test);
            //    watch.Stop();
            //}

            //Console.WriteLine(watch.ElapsedMilliseconds);
            //Console.WriteLine(res2);
        }
    }
}
