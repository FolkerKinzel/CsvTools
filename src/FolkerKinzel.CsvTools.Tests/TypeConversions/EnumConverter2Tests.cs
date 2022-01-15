using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.CsvTools.TypeConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.CsvTools.TypeConversions.Converters.Intls;
using FolkerKinzel.CsvTools.TypeConversions.Converters;

namespace FolkerKinzel.CsvTools.TypeConversions.Tests
{
    [TestClass()]
    public class EnumConverter2Tests
    {
        [TestMethod()]
        public void EnumConverter2Test()
        {
            var conv = new EnumConverter<DayOfWeek>();

            string? outp = (string?)conv.ConvertToString(null);
            Assert.IsNotNull(outp);

            DayOfWeek res = conv.Parse(outp.AsSpan());

            Assert.AreEqual(default, res);
        }
    }
}