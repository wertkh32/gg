using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NUnit.Framework;
using System.Globalization;

namespace gg
{
    [TestFixture]
    /// <summary>
    /// <author>Yeo Jie Shun</author>
    /// </summary>
    class DateTimeParserTest
    {
        //userInput,expected desc,expected tag,expected time, describe test  
        static CultureInfo usCulture = CultureInfo.CreateSpecificCulture("en-US");
        static object[] tests = 
        {
                new object[] {"tmr", DateTime.Parse(DateTime.Now.Date.AddDays(1).AddHours(23.9833333).ToString()).ToString(), "tmr test"},
                new object[] {"tmr 5pm",DateTime.Parse(DateTime.Now.Date.AddDays(1).AddHours(17).ToShortDateString()).AddHours(17).ToString(), "tmr test with time"},
                new object[] {"5pm tmr",DateTime.Parse(DateTime.Now.Date.AddDays(1).AddHours(17).ToShortDateString()).AddHours(17).ToString(), "tmr test with time swapped"},
                new object[] {"tmr 2359",DateTime.Parse(DateTime.Now.Date.AddDays(1).AddHours(23.9833333).ToShortDateString()).AddHours(23).AddMinutes(59).ToString(), "tmr test with time"},
                new object[] {"tmr 5pm", DateTime.Parse(DateTime.Now.Date.AddDays(1).AddHours(17).ToShortDateString()).AddHours(17).ToString(), "tmr test with time and tag"},
                new object[] {"17/09/2011",DateTime.Parse("09/17/2011", usCulture).AddHours(23.9833333).ToString(), "specific date"},
                new object[] {"17/09", DateTime.Parse("09/17/2011", usCulture).AddHours(23.9833333).ToString(), "short date"},
                new object[] {"17-9",  DateTime.Parse("09/17/2011", usCulture).AddHours(23.9833333).ToString(), "short date variation"},
                new object[] {"17-9 5pm", DateTime.Parse("09/17/2011", usCulture).AddHours(17).ToString(), "short date and time"},
                new object[] {"sun 5pm", DateTime.Parse("11/06/2011", usCulture).AddHours(17).ToString(), "next week and time"},
                new object[] {"11 feb", DateTime.Parse("02/11/2011", usCulture).AddHours(23.9833333).ToString(), "natural date"},
                new object[] {"oct", DateTime.Parse("10/1/2011", usCulture).AddHours(23.9833333).ToString(), "natural month only"},
                new object[] {"11 oct 5pm",  DateTime.Parse("10/11/2011", usCulture).AddHours(17).ToString(), "natural date with time"},
                new object[] {" ",  DateTime.MinValue.ToString(), "empty string"},
                new object[] {"oct 13 things to do get my things done (((!##$#! 5pm",  DateTime.Parse("10/13/2011", usCulture).AddHours(17).ToString(), "messy string with time"},
                new object[] {"fri",  DateTime.Parse("11/11/2011", usCulture).AddHours(23.9833333).ToString(), "next week only"},
                new object[] {"930pm",  DateTime.Now.Date.AddHours(21.5).ToString(), "time only"},
                new object[] {"0900pm",  DateTime.Now.Date.AddHours(21).ToString(), "4 digit time"},
                new object[] {"today",  DateTime.Now.Date.AddHours(23.98333333).ToString(), "today test"},
        };

        [Test, TestCaseSource("tests")]
        public static void TestParser(string input, string expected, string message)
        {
            DateTimeParser dtParser = new DateTimeParser(input);
            DateTime dateTime = dtParser.GetDateTimeResult();
            Assert.AreEqual(expected, dateTime.ToString(), message);
        }
    }
}
